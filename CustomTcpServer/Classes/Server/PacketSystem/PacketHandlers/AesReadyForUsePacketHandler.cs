using System.Text;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class AesReadyForUsePacketHandler : IPacketHandler
    {
        public async void Handle(Packet packet, ClientHandler clientHandler)
        {
            string serverGuidString = clientHandler.GetInfinityTcpServer.GetServerGuid.ToString();
            byte[] serverGuidData = Encoding.UTF8.GetBytes(serverGuidString);
            await clientHandler.GetInfinityTcpServer.GetServerPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, serverGuidData, "Server Guid Packet", clientHandler.ClientGuid.ToString(), true);
        }
    }
}
