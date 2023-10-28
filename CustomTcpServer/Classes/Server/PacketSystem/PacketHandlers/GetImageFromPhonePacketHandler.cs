using InfinityServer.Classes.Utils;
using System;
using System.Drawing;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    internal class GetImageFromPhonePacketHandler : IPacketHandler
    {
        public void Handle(Packet packet, ClientHandler clientHandler)
        {
            Image image = ImageUtils.ByteArrayToImage(packet.Data);

            string guid = Guid.NewGuid().ToString();
            image.Save("C:\\Users\\Keith\\Desktop\\SavedImagesFromPhone\\img_" + guid + ".png");
        }
    }
}
