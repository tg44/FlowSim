using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;

namespace fluid.D3DrawModelsSources
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

        public void Shutdown()
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
