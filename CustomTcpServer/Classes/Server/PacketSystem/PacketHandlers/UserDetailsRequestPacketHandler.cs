using InfinityServer.App;
using InfinityServer.Classes.Server.User;
using InfinityServer.Classes.Utils;
using Newtonsoft.Json;
using System.Text;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class UserDetailsRequestPacketHandler : IPacketHandler
    {
        private InfinityTcpServer _tcpServer;
        private PacketHandler _serverPacketHandler;

        public async void Handle(Packet packet, ClientHandler clientHandler)
        {
            _tcpServer = clientHandler.GetInfinityTcpServer;
            _serverPacketHandler = _tcpServer.GetServerPacketHandler;

            string username = Encoding.UTF8.GetString(packet.Data);

            UserDetails userDetails = await InfinityApplication.Instance.Database.GetUserDetailsByUsername(username);

            string userDetailsJsonString = JsonConvert.SerializeObject(userDetails);
            byte[] userDetailsData = Encoding.UTF8.GetBytes(userDetailsJsonString);

            await _serverPacketHandler.CreateAndSendPacketAsync(_tcpServer, userDetailsData, "User Details Response", clientHandler.ClientGuid.ToString(), true);
        }
    }
}
