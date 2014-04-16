using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX;
using System.IO;
using SharpDX.DXGI;
using Texture3D = SharpDX.Toolkit.Graphics.Texture3D;

namespace fluid.D3DrawModelsSources
{
    class Volume
    {
        DX11 DX11;
        Model VolumeBox;
        VolumeShader VolumeShader;

        ShaderResourceView fvolume;

        Vector3 magicTeapot = new Vector3(256, 256, 178);

        VolumeModelBox VolumeModelBox;

        //private float[,,] Data;
        //private Texture3D VolumeTexture;
        public bool Initialize(DX11 dx11)
        {
            DX11 = dx11;
            VolumeBox = new Model();
            if (!VolumeBox.Initialize(DX11.Device, "Cube.txt", "seafloor.dds"))
                Console.Out.WriteLine("Error on cube load!");

            VolumeModelBox = new VolumeModelBox();
            if (!VolumeModelBox.Initialize(DX11.Device))
                Console.Out.WriteLine("Error on cube load!");

            VolumeShader = new VolumeShader();
            VolumeShader.Initialize(DX11.Device, DX11.Handle);

            fvolume = new ShaderResourceView(DX11.DeviceContext.Device, ReadVolumeFromFile());

            return true;
        }

        public bool Render(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Matrix worldViewProj = worldMatrix * viewMatrix * projectionMatrix;
            Texture2D back = RenderToTexture(worldViewProj, VolumeShader.RENDER_BACK);
            Texture2D front = RenderToTexture(worldViewProj, VolumeShader.RENDER_FRONT);

            ShaderResourceView bview = new ShaderResourceView(DX11.DeviceContext.Device, back);
            ShaderResourceView fview = new ShaderResourceView(DX11.DeviceContext.Device, front);

            VolumeBox.Render(DX11.DeviceContext);
            if (!VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, magicTeapot, VolumeShader.RENDER_VOLUME, fview, bview, fvolume))
                return false;

            bview.Dispose();
            fview.Dispose();

            back.Dispose();
            front.Dispose();

            return true;
        }

        public bool Render2(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {

            return true;
        }
        private Texture3D ReadVolumeFromFile()
        {

            FileStream fs = new FileStream(@"../../Models/teapot.raw", FileMode.Open);
            if (fs.Length > magicTeapot.X * magicTeapot.Y * magicTeapot.Z) { Console.Out.WriteLine("raw16"); }

            BinaryReader br = new BinaryReader(fs);
            byte[] buffer = new byte[(int)(magicTeapot.X * magicTeapot.Y * magicTeapot.Z)];
            int size = sizeof(byte);
            br.Read(buffer, 0, size * buffer.Length);
            br.Close();
            float[] scalar = new float[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                scalar[i] = (float)buffer[i] / byte.MaxValue;
            }
            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(DX11.Device);
            Texture3D volumeTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource,
                Format = Format.R8G8B8A8_UNorm,
                Width = (int)magicTeapot.X,
                Height = (int)magicTeapot.Y,
                Depth = (int)magicTeapot.Z,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            });

            volumeTex.SetData<float>(scalar);
            return volumeTex;
        }



        private Texture2D RenderToTexture(Matrix worldViewProj, string renderMethod)
        {
            Texture2D renderToTexture = new Texture2D(DX11.Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Width = DX11.Width,
                Height = DX11.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });

            // 2) Create a RenderTargetView (and an optional ShaderResourceView):
            var renderToTextureRTV = new RenderTargetView(DX11.Device, renderToTexture);
            //var renderToTextureSRV = new ShaderResourceView(DX11.Device, renderToTexture);



            // 3) Bind the render target to the output in your rendering code:
            DX11.DeviceContext.OutputMerger.SetTargets(renderToTextureRTV);

            // 4) Don't forget to setup the viewport for this particular render target
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            VolumeBox.Render(DX11.DeviceContext);
            VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, renderMethod);

            DX11.DeviceContext.OutputMerger.SetTargets(DX11.DepthStencilView, DX11.RenderTargetView);
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);
            renderToTextureRTV.Dispose();
            return renderToTexture;
        }
        private Texture2D RenderToTexture2(Matrix worldViewProj, string renderMethod, VolumeModelBox vmb)
        {
            Texture2D renderToTexture = new Texture2D(DX11.Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Width = DX11.Width,
                Height = DX11.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });

            // 2) Create a RenderTargetView (and an optional ShaderResourceView):
            var renderToTextureRTV = new RenderTargetView(DX11.Device, renderToTexture);
            //var renderToTextureSRV = new ShaderResourceView(DX11.Device, renderToTexture);



            // 3) Bind the render target to the output in your rendering code:
            DX11.DeviceContext.OutputMerger.SetTargets(renderToTextureRTV);

            // 4) Don't forget to setup the viewport for this particular render target
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            VolumeModelBox.Render(DX11.DeviceContext);
            VolumeShader.Render(DX11.DeviceContext, VolumeModelBox.IndexCount, worldViewProj, renderMethod);

            DX11.DeviceContext.OutputMerger.SetTargets(DX11.DepthStencilView, DX11.RenderTargetView);
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);
            renderToTextureRTV.Dispose();
            return renderToTexture;
        }
    }
}
