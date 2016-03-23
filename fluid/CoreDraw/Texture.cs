using System;
using SharpDX.Direct3D11;
using System.Drawing;
using System.IO;

namespace fluid.CoreDraw
{
    class Texture
    {

        public ShaderResourceView TextureResource { get; private set; }

        public bool Initialize(Device device, string fileName)
        {
            try
            {
                // Load the texture file.
                TextureResource = ShaderResourceView.FromFile(device, fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Initialize(Device device, Image image)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                TextureResource = ShaderResourceView.FromMemory(device, ms.ToArray());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            // Release the texture resource.
            if (TextureResource != null)
            {
                TextureResource.Dispose();
                TextureResource = null;
            }
        }

    }
}
