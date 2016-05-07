using SharpDX.Direct3D11;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;
namespace fluid.CoreDraw
{
    class TextureHelper
    {
        public Texture2D Tex;

        public ShaderResourceView SRV;

        public UnorderedAccessView UAV;

        public bool withUAV = false;

        public TextureHelper(Texture2DDescription texDesc, Device device, bool withUAV)
        {
            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(device);
            Tex = Texture2D.New(toolkitdiv, texDesc);
            SRV = new ShaderResourceView(device, Tex);
            if (withUAV)
            {
                this.withUAV = true;
                UAV = new UnorderedAccessView(device, Tex);
            }
        }

        public TextureHelper(SharpDX.Direct3D11.Texture2D texDesc, Device device, bool withUAV = false)
        {
            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(device);
            Tex = Texture2D.New(toolkitdiv, texDesc);
            SRV = new ShaderResourceView(device, Tex);
            if (withUAV)
            {
                this.withUAV = true;
                UAV = new UnorderedAccessView(device, Tex);
            }
        }

        public void Dispose()
        {
            SRV.Dispose();
            if (withUAV)
            {
                UAV.Dispose();
            }
            Tex.Dispose();
        }

        public static void Swap(TextureHelper a, TextureHelper b)
        {
            swap(ref a.Tex, ref b.Tex);
            swap(ref a.SRV, ref b.SRV);
            if (a.withUAV)
            {
                swap(ref a.UAV, ref b.UAV);
            }
        }

        #region swaps

        private static void swap(ref Texture2D a, ref Texture2D b)
        {
            Texture2D t = a;
            a = b;
            b = t;
        }

        private static void swap(ref SharpDX.Direct3D11.Texture2D a, ref SharpDX.Direct3D11.Texture2D b)
        {
            SharpDX.Direct3D11.Texture2D t = a;
            a = b;
            b = t;
        }

        private static void swap(ref ShaderResourceView a, ref ShaderResourceView b)
        {
            ShaderResourceView t = a;
            a = b;
            b = t;
        }

        private static void swap(ref UnorderedAccessView a, ref UnorderedAccessView b)
        {
            UnorderedAccessView t = a;
            a = b;
            b = t;
        }

        #endregion swaps

    }
}
