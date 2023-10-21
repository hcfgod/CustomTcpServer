using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using InfinityServer.App;
using InfinityServer.Classes.Server.PacketSystem.PacketHandlers;
using InfinityServer.Classes.Utils;
using Newtonsoft.Json;

namespace InfinityServer.Classes.Server.PacketSystem
{
    public class PacketHandler
    {
        private ConcurrentDictionary<string, IPacketHandler> packetHandlers = new ConcurrentDictionary<string, IPacketHandler>();

        public PacketHandler()
        {
            // Initialize packet handlers
            packetHandlers.TryAdd("Client Public Key", new ClientPublicKeyPacketHandler());
            packetHandlers.TryAdd("Aes Ready For Use", new AesReadyForUsePacketHandler());

            packetHandlers.TryAdd("User Registration Request", new UserRegistrationPacketHandler());
            packetHandlers.TryAdd("User Login Request", new UserLoginRequestPacketHandler());

            packetHandlers.TryAdd("User Details Request", new UserDetailsRequestPacketHandler());
        }

        public byte[] SerializePacket(Packet packet)
        {
            try
            {
                if (packet.EncryptionFlag)
                {
                    packet.Data = CryptoUtility.AesEncrypt(packet.Data);  // Your Encrypt method
                }

                packet.GenerateChecksum();

                string jsonString = JsonConvert.SerializeObject(packet);
                return Encoding.UTF8.GetBytes(jsonString);
            }
            catch (JsonException jsonEx)
            {
                // Handle JSON serialization errors
                InfinityApplication.Instance.Logger.Error($"(PacketHandler.cs) - SerializePacket(): JSON Serialization Error: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Handle other errors
                InfinityApplication.Instance.Logger.Error($"(PacketHandler.cs) - SerializePacket(): General Error: {ex.Message}");
                return null;
            }
        }

        public Packet DeserializePacket(byte[] data)
        {
            try
            {
                string json = Encoding.UTF8.GetString(data);
                Packet packet = JsonConvert.DeserializeObject<Packet>(json);

                // Validate checksum
                string originalChecksum = packet.Checksum;

                packet.GenerateChecksum();

                if (originalChecksum != packet.Checksum)
                {
                    InfinityApplication.Instance.Logger.Error($"(PacketHandler.cs) - DeserializePacket(): Checksum validation failed.");
                    return null;
                }

                if (packet.EncryptionFlag)
                {
                    packet.Data = CryptoUtility.AesDecrypt(packet.Data);
                }

                return packet;
            }
            catch (JsonException jsonEx)
            {
                string receivedDataStr = Encoding.UTF8.GetString(data);
                InfinityApplication.Instance.Logger.Error($"(PacketHandler.cs) - DeserializePacket(): JSON Deserialization Error: {jsonEx.Message}. Received data: {receivedDataStr}");
                return null;
            }
            catch (Exception ex)
            {
                InfinityApplication.Instance.Logger.Error($"(PacketHandler.cs) - DeserializePacket(): General Error: {ex.Message}");
                return null;
            }
        }

        public Packet CreateNewPacket(byte[] data, string packetType, bool encryptionFlag = false, string version = "0.1")
        {
            Packet packet = new Packet
            {
                PacketID = Guid.NewGuid(),
                PacketType = packetType,
                Timestamp = DateTime.UtcNow,
                Data = data,
                EncryptionFlag = encryptionFlag,

                Version = version,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                SenderID = "Server",
                ReceiverID = "Client",
            };

            return packet;
        }

        public byte[] CreateAndSerializePacket(byte[] data, string packetType, bool encryptionFlag = false, string version = "0.1")
        {
            Packet packet = new Packet
            {
                PacketID = Guid.NewGuid(),
                PacketType = packetType,
                Timestamp = DateTime.UtcNow,
                Data = data,
                EncryptionFlag = encryptionFlag,

                Version = version,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                SenderID = "Server",
                ReceiverID = "Client",
            };

            byte[] serializedPacket = SerializePacket(packet);

            return serializedPacket;
        }

        public async Task CreateAndSendPacketAsync(InfinityTcpServer infinityTcpServer, byte[] data, string packetType, string guid = "", bool encryptionFlag = false, string version = "0.1")
        {
            Packet packet = new Packet
            {
                PacketID = Guid.NewGuid(),
                PacketType = packetType,
                Timestamp = DateTime.UtcNow,
                Data = data,
                EncryptionFlag = encryptionFlag,

                Version = version,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                SenderID = infinityTcpServer.GetServerGuid.ToString(),
                ReceiverID = guid,
            };

            byte[] serializedPacket = SerializePacket(packet);

            if(guid == string.Empty)
            {
                guid = infinityTcpServer.GetServerGuid.ToString();
            }

            await infinityTcpServer.GetTcpServer.SendAsync(Guid.Parse(guid), serializedPacket);
        }

        public void CreateSendAndWaitPacket(int waitDealy, InfinityTcpServer infinityTcpServer, byte[] data, string packetType, string guid = "", bool encryptionFlag = false, string version = "0.1")
        {
            Packet packet = new Packet
            {
                PacketID = Guid.NewGuid(),
                PacketType = packetType,
                Timestamp = DateTime.UtcNow,
                Data = data,
                EncryptionFlag = encryptionFlag,

                Version = version,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                SenderID = infinityTcpServer.GetServerGuid.ToString(),
                ReceiverID = guid,
            };

            byte[] serializedPacket = SerializePacket(packet);

            if (guid == string.Empty)
            {
                guid = infinityTcpServer.GetServerGuid.ToString();
            }

            infinityTcpServer.GetTcpServer.SendAndWait(waitDealy, Guid.Parse(guid), serializedPacket);
        }

        public void ProcessPacket(Packet packet, ClientHandler clientHandler)
        {
            if (packetHandlers.TryGetValue(packet.PacketType, out IPacketHandler handler))
            {
                handler.Handle(packet, clientHandler);
            }
            else
            {
                InfinityApplication.Instance.Logger.Warning($"Unknown packet type {packet.PacketType}");
            }
        }
    }
}
