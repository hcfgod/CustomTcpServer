using System.IO;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class FileFromClientPacketHandler : IPacketHandler
    {
        public void Handle(Packet packet, ClientHandler clientHandler)
        {
            byte[] receivedFileData = packet.Data;

            string savePath = $"C:/Users/Keith/Desktop/received_file{packet.FileExtestion}"; // Replace with your desired path and file extension
            File.WriteAllBytes(savePath, receivedFileData);
        }
    }
}