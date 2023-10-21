using System.Drawing;
using System.IO;

namespace InfinityServer.Classes.Utils
{
    public class ImageUtils
    {
        public static byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                return memory.ToArray();
            }
        }

        public static Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream memory = new MemoryStream(byteArray))
            {
                return Image.FromStream(memory);
            }
        }
    }
}
