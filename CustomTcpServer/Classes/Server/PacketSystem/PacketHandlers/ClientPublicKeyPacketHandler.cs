﻿using InfinityServer.App;
using InfinityServer.Classes.Utils;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class ClientPublicKeyPacketHandler : IPacketHandler
    {
        public async void Handle(Packet packet, ClientHandler clientHandler)
        {
            string clientPublicKeyJson = Encoding.UTF8.GetString(packet.Data);
            RSAParameters clientPublicKey = JsonConvert.DeserializeObject<RSAParameters>(clientPublicKeyJson);

            InfinityApplication.Instance.InfinityTcpServer.GetServerKeyManager.SetClientPublicKey(clientPublicKey);

            byte[] aesKey = CryptoUtility.AesKey;
            byte[] aesIV = CryptoUtility.AesIV;

            byte[] encryptedKeyBytes = CryptoUtility.RsaEncrypt(aesKey, clientPublicKey);
            await InfinityApplication.Instance.InfinityTcpServer.GetServerPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, encryptedKeyBytes, "Aes Key", clientHandler.ClientGuid.ToString());

            byte[] encryptedIVBytes = CryptoUtility.RsaEncrypt(aesIV, clientPublicKey);
            await InfinityApplication.Instance.InfinityTcpServer.GetServerPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, encryptedIVBytes, "Aes IV", clientHandler.ClientGuid.ToString());
        }
    }
}
