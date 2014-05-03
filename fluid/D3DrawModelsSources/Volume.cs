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

        float[] scalar;
        Texture3D volumeTex;
        ShaderResourceView fdvolume;
        Vector3 vDimension = new Vector3(20, 20, 20);

        int frames = 0;
        int animatetime = 0;

        //VolumeModelBox VolumeModelBox;

        //private float[,,] Data;
        //private Texture3D VolumeTexture;
        public bool Initialize(DX11 dx11)
        {
            DX11 = dx11;

            VolumeBox = new Model();
            if (!VolumeBox.Initialize(DX11.Device, "Cube.txt", "seafloor.dds"))
                Console.Out.WriteLine("Error on cube load!");
            /*
            VolumeModelBox = new VolumeModelBox();
            if (!VolumeModelBox.Initialize(DX11.Device))
                Console.Out.WriteLine("Error on cube load!");
            */
            VolumeShader = new VolumeShader();
            VolumeShader.Initialize(DX11.Device, DX11.Handle);

            fvolume = new ShaderResourceView(DX11.DeviceContext.Device, ReadVolumeFromFile());
            init3DTexture();
            fdvolume = new ShaderResourceView(DX11.DeviceContext.Device, volumeTex);


            return true;
        }

        public bool Render(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            //worldMatrix = worldMatrix * Matrix.Translation(-1.5f, 0, -1.5f);
            Matrix worldViewProj = worldMatrix * viewMatrix * projectionMatrix;

            Texture2D back = RenderToTexture(worldViewProj, VolumeShader.RENDER_BACK);
            Texture2D front = RenderToTexture(worldViewProj, VolumeShader.RENDER_FRONT);

            ShaderResourceView bview = new ShaderResourceView(DX11.DeviceContext.Device, back);
            ShaderResourceView fview = new ShaderResourceView(DX11.DeviceContext.Device, front);

            frames++;
            if (frames > 30)
            {
                frames = 0;
                animatetime++;
                animate3DTexture(animatetime);
            }

            DX11.TurnOnAlphaBlending();
            VolumeBox.Render(DX11.DeviceContext);
            if (!VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, vDimension, VolumeShader.RENDER_VOLUME, fview, bview, fdvolume))
                return false;
            //if (!VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, magicTeapot, VolumeShader.RENDER_VOLUME, fview, bview, fvolume))
            //    return false;
            DX11.TurnOffAlphaBlending();


            bview.Dispose();
            fview.Dispose();


            back.Dispose();
            front.Dispose();


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
                Format = Format.R32_Float,
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
            DX11.DeviceContext.OutputMerger.SetRenderTargets(renderToTextureRTV);

            // 4) Don't forget to setup the viewport for this particular render target
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            VolumeBox.Render(DX11.DeviceContext);
            VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, renderMethod);

            DX11.DeviceContext.OutputMerger.SetTargets(DX11.DepthStencilView, DX11.RenderTargetView);
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            renderToTextureRTV.Dispose();
            return renderToTexture;
        }

        private void init3DTexture()
        {
            int size = (int)(vDimension.X * vDimension.Y * vDimension.Z) * 2;
            scalar = new float[size];
            for (int i = 0; i < vDimension.X; i++)
            {
                for (int j = 0; j < vDimension.Y; j++)
                {
                    for (int k = 0; k < vDimension.Z; k++)
                    {
                        int l = (int)(i * vDimension.Y * vDimension.Z * 2 + j * vDimension.Z * 2 + k * 2);
                        scalar[l] = 0.9f;
                        scalar[l + 1] = l / (vDimension.X * vDimension.Y * vDimension.Z * 2.0f);
                    }
                }
            }
            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(DX11.Device);
            volumeTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource,
                Format = Format.R32G32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Write,
                Usage = ResourceUsage.Dynamic
            });

            volumeTex.SetData<float>(scalar);
        }

        public void animate3DTexture(int t)
        {
            for (int i = 0; i < vDimension.X; i++)
            {
                for (int j = 0; j < vDimension.Y; j++)
                {
                    for (int k = 0; k < vDimension.Z; k++)
                    {
                        scalar[adresser(i, j, k, 0)] = Noise.noise(new Vector4(i / 10.0f, j / 10f, k / 10f, t / 200f));
                        scalar[adresser(i, j, k, 1)] = Noise.noise4D(new Vector4(i / 10.0f, j / 10.0f, k / 10.0f, (t + 1) / 100.0f));
                    }
                }
            }/*
            float maxval = float.MinValue;
            float minval = float.MaxValue;
            for (int i = 0; i < scalar.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (scalar[i] > maxval) maxval = scalar[i];
                    if (scalar[i] < minval) minval = scalar[i];
                }
            }
            if (minval != maxval)
            {
                for (int i = 0; i < scalar.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        scalar[i] = (scalar[i] - minval) / (maxval - minval);
                    }
                }
            }
            */
            volumeTex.SetData<float>(scalar);
        }
        private int adresser(int x, int y, int z, int w)
        {
            if (x > vDimension.X) x = (int)vDimension.X;
            if (y > vDimension.Y) y = (int)vDimension.Y;
            if (z > vDimension.Z) z = (int)vDimension.Z;
            if (w > 2) w = 2;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (z < 0) z = 0;
            if (w < 0) w = 0;
            return (int)(x * vDimension.Y * vDimension.Z * 2 + y * vDimension.Z * 2 + z * 2 + w);
        }
        /*private Texture2D RenderToTexture2(Matrix worldViewProj, string renderMethod, VolumeModelBox vmb)
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
        }*/
        public void Shutdown()
        {
            /*if (frontRenderTexture != null) { frontRenderTexture.Dispose(); }
            if (frontRTV != null) { frontRTV.Dispose(); }
            if (frontView != null) { frontView.Dispose(); }
            if (backRenderTexture != null) { backRenderTexture.Dispose(); }
            if (backRTV != null) { backRTV.Dispose(); }
            if (backView != null) { backView.Dispose(); }
            if (fvolume != null) { fvolume.Dispose(); }*/
        }
        /*public bool Render2(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {


            Matrix myWorld = Matrix.Identity;
            VolumeShader.setVolumeTexture(fvolume);
            int j = 0;
            while (j < 5)
            {
                int k = 0;
                while (k < 5)
                {
                    int l = 0;
                    while (l < 5)
                    {
                        myWorld = Matrix.Translation(j, k, l);
                        //VolumeModelBox VolumeModelBoxin = new VolumeModelBox();
                        //if (!VolumeModelBoxin.Initialize(DX11.Device))
                        //   return false;

                        Matrix worldViewProj = myWorld * worldMatrix * viewMatrix * projectionMatrix;
                        Texture2D back = RenderToTexture2(worldViewProj, VolumeShader.RENDER_BACK, VolumeModelBox);
                        Texture2D front = RenderToTexture2(worldViewProj, VolumeShader.RENDER_FRONT, VolumeModelBox);

                        ShaderResourceView bview = new ShaderResourceView(DX11.DeviceContext.Device, back);
                        ShaderResourceView fview = new ShaderResourceView(DX11.DeviceContext.Device, front);

                        DX11.TurnOnAlphaBlending();

                        VolumeModelBox.Render(DX11.DeviceContext);
                        if (!VolumeShader.Render(DX11.DeviceContext, VolumeModelBox.IndexCount, worldViewProj, magicTeapot, VolumeShader.RENDER_VOLUME, fview, bview, null))
                            return false;

                        bview.Dispose();
                        fview.Dispose();

                        back.Dispose();
                        front.Dispose();

                        DX11.TurnOffAlphaBlending();

                        //VolumeModelBoxin.Shutdown();
                        l++;
                    }
                    k++;
                }
                j++;
            }
            return true;
        }*/
    }
}
