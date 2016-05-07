using SharpDX.Direct3D11;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;
namespace fluid.CoreDraw
{
    class TextureWithNextHelper
    {
        public TextureHelper Actual;

        public TextureHelper Next;

        public TextureWithNextHelper(Texture2DDescription texDesc, Device device)
        {
            Actual = new TextureHelper(texDesc, device, true);
            Next = new TextureHelper(texDesc, device, true);
        }

        public TextureWithNextHelper(SharpDX.Direct3D11.Texture2D texDesc, Device device)
        {
            Actual = new TextureHelper(texDesc, device, true);
            Next = new TextureHelper(texDesc, device, true);
        }

        public Texture2D Tex { get { return Actual.Tex; } }
        public ShaderResourceView SRV { get { return Actual.SRV; } }
        public UnorderedAccessView UAV { get { return Next.UAV; } }

        public void Swap()
        {
            TextureHelper.Swap(Actual, Next);
        }

        public void Dispose()
        {
            Actual.Dispose();
            Next.Dispose();
        }
    }
}
