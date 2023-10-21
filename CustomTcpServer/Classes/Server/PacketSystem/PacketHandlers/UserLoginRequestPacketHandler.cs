using InfinityServer.Classes.Server.Security;
using InfinityServer.Classes.Server.User;
using Newtonsoft.Json;
using System.Text;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class UserLoginRequestPacketHandler : IPacketHandler
    {
        private InfinityTcpServer _tcpServer;
        private PacketHandler _serverPacketHandler;

        public async void Handle(Packet packet, ClientHandler clientHandler)
        {
            _tcpServer = clientHandler.GetInfinityTcpServer;
            _serverPacketHandler = _tcpServer.GetServerPacketHandler;

            // Convert bytes to string
            string jsonString = Encoding.UTF8.GetString(packet.Data);

            // Deserialize the JSON string to UserAuthDetails object
            UserAuthDetails userAuthDetails = JsonConvert.DeserializeObject<UserAuthDetails>(jsonString);

            if (await AuthenticationService.AuthenticateUser(userAuthDetails.Username, userAuthDetails.PasswordHash))
            {
                // Send a successful login packet as a response
                byte[] dataBytes = Encoding.UTF8.GetBytes("Login Successful");
                await _serverPacketHandler.CreateAndSendPacketAsync(_tcpServer, dataBytes, "Login Response", clientHandler.ClientGuid.ToString(), true);
            }
            else
            {
                // Send a failed login packet as a response
                byte[] dataBytes = Encoding.UTF8.GetBytes("Login Failed");
                await _serverPacketHandler.CreateAndSendPacketAsync(_tcpServer, dataBytes, "Login Response", clientHandler.ClientGuid.ToString(), true);
            }
        }
    }
}
