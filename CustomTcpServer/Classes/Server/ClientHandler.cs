using InfinityServer.Classes.Server.PacketSystem;
using InfinityServer.Classes.Server.Security;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using WatsonTcp;

namespace InfinityServer.Classes.Server
{
    public class ClientHandler
    {
        private InfinityTcpServer _infinityTcpServer;
        private RateLimiter _rateLimiter;

        private Guid _clientGUID;

        public Guid ClientGuid { get { return _clientGUID; } }

        public InfinityTcpServer GetInfinityTcpServer => _infinityTcpServer;

        public ClientHandler(InfinityTcpServer infinityTcpServer, Guid ClientGUID)
        {
            _rateLimiter = new RateLimiter();
            _infinityTcpServer = infinityTcpServer;
            _clientGUID = ClientGUID;
        }

        public async void ClientConnected(ConnectionEventArgs e)
        {
            byte[] clientGuidData = Encoding.UTF8.GetBytes(_clientGUID.ToString());
            await _infinityTcpServer.GetServerPacketHandler.CreateAndSendPacketAsync(_infinityTcpServer, clientGuidData, "Client Guid", _clientGUID.ToString());

            SendClientTheServerPublicKey();
        }

        public async void StreamReceived(object sender, StreamReceivedEventArgs e)
        {
            if(_rateLimiter.IsRateLimited(_clientGUID.ToString()))
            {
                MessageBox.Show("Send a Packet too the user letting them know they have to wait one minute before doing anything else.");
                return;
            }

            byte[] receivedData = new byte[e.ContentLength];

            await e.DataStream.ReadAsync(receivedData, 0, receivedData.Length);
            Packet receivedPacket = _infinityTcpServer.GetServerPacketHandler.DeserializePacket(receivedData);

            _infinityTcpServer.GetServerPacketHandler.ProcessPacket(receivedPacket, this);
        }

        private async void SendClientTheServerPublicKey()
        {
            // Send the client our public server key
            string jsonString = JsonConvert.SerializeObject(_infinityTcpServer.GetServerKeyManager.GetPublicKey());
            byte[] serverPublicKeyData = Encoding.UTF8.GetBytes(jsonString);
            await _infinityTcpServer.GetServerPacketHandler.CreateAndSendPacketAsync(_infinityTcpServer, serverPublicKeyData, "Server Public Key", _clientGUID.ToString());
        }
    }
}