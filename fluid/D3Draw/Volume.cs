using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX;
using System.IO;
using SharpDX.DXGI;
using Texture3D = SharpDX.Toolkit.Graphics.Texture3D;
using SharpDX.Direct3D;
using System.Drawing;
using fluid.CoreDraw;
using fluid.D3Draw.ShaderLoaders;

namespace fluid.D3Draw
{
    class Volume
    {
        DX11 DX11;
        Model VolumeBox;
        public VolumeShader VolumeShader { get; private set; }
        public PhysicsShader PhysicsShader { get; private set; }

        //ShaderResourceView fvolume;

        Vector3 magicTeapot = new Vector3(256, 256, 178);

        float[] scalar;
        public Texture3D volumePressureTex;
        public Texture3D volumeDivergenceTex;
        public Texture3D volumeDensityTex;
        public Texture3D volumeVelocityTex;
        public Texture3D volumeTempTex;
        public Texture3D volumeWallTex;
        public Texture3D volumeVelocityFieldTex;

        public Texture3D nextVelocityTexture;
        public Texture3D nextDensityTexture;
        public Texture3D nextPressureTexture;
        public Texture3D nextTemperatureTexture;

        public ShaderResourceView fdvolumePressure;
        public ShaderResourceView fdvolumeTemp;
        public ShaderResourceView fdvolumeDivergence;
        public ShaderResourceView fdvolumeDensity;
        public ShaderResourceView fdvolumeVelocity;
        public ShaderResourceView fdvolumeWallTex;
        public ShaderResourceView fdvolumeVelocityFieldTex;

        public ShaderResourceView nextVelocityTexturesrv;
        public ShaderResourceView nextDensityTexturesrv;
        public ShaderResourceView nextPressureTexturesrv;
        public ShaderResourceView nextTemperatureTexturesrv;

        public UnorderedAccessView volumePressureTexUav;
        public UnorderedAccessView volumeDivergenceTexUav;
        public UnorderedAccessView volumeDensityTexUav;
        public UnorderedAccessView volumeVelocityTexUav;
        public UnorderedAccessView volumeTempTexUav;
        public UnorderedAccessView volumeWallTexUav;
        public UnorderedAccessView volumeVelocityFieldTexUav;

        public UnorderedAccessView nextVelocityTextureUav;
        public UnorderedAccessView nextDensityTextureUav;
        public UnorderedAccessView nextPressureTextureUav;
        public UnorderedAccessView nextTemperatureTextureUav;


        public Vector3 vDimension = new Vector3(128, 128, 128);

        Texture2D frontText;
        Texture2D backText;
        Texture2D frontDepthText;
        Texture2D backDepthText;

        public ShaderResourceView frontSRV;
        public ShaderResourceView backSRV;
        public ShaderResourceView frontDepthSRV;
        public ShaderResourceView backDepthSRV;
        public ShaderResourceView inObjectsDepthSRV;

        RenderTargetView frontRTV;
        RenderTargetView backRTV;
        DepthStencilView frontDepthDSV;
        DepthStencilView backDepthDSV;

        int frames = 0;

        public SizeF Heatmap { get; set; }
        public SizeF Sensitivitymap { get; set; }
        //int animatetime = 0;

        //VolumeModelBox VolumeModelBox;

        //private float[,,] Data;
        //private Texture3D VolumeTexture;
        public Volume()
        {
            Heatmap = new SizeF(0, 1);
            Sensitivitymap = new SizeF(0, 1);
        }
        public bool Initialize(DX11 dx11, SizeF Heatmap, SizeF Sensitivitymap)
        {
            DX11 = dx11;

            this.Heatmap = Heatmap;
            this.Sensitivitymap = Sensitivitymap;

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

            PhysicsShader = new PhysicsShader();
            PhysicsShader.Initialize(DX11.Device, DX11.Handle, this);

            init3DTextures();
            //fvolume = new ShaderResourceView(DX11.DeviceContext.Device, ReadVolumeFromFile());
            fdvolumePressure = new ShaderResourceView(DX11.DeviceContext.Device, volumePressureTex);
            fdvolumeTemp = new ShaderResourceView(DX11.DeviceContext.Device, volumeTempTex);
            fdvolumeDivergence = new ShaderResourceView(DX11.DeviceContext.Device, volumeDivergenceTex);
            fdvolumeDensity = new ShaderResourceView(DX11.DeviceContext.Device, volumeDensityTex);
            fdvolumeVelocity = new ShaderResourceView(DX11.DeviceContext.Device, volumeVelocityTex);
            fdvolumeVelocityFieldTex = new ShaderResourceView(DX11.DeviceContext.Device, volumeVelocityFieldTex);
            fdvolumeWallTex = new ShaderResourceView(DX11.DeviceContext.Device, volumeWallTex);

            nextVelocityTexturesrv = new ShaderResourceView(DX11.DeviceContext.Device, nextVelocityTexture);
            nextDensityTexturesrv = new ShaderResourceView(DX11.DeviceContext.Device, nextDensityTexture);
            nextPressureTexturesrv = new ShaderResourceView(DX11.DeviceContext.Device, nextPressureTexture);
            nextTemperatureTexturesrv = new ShaderResourceView(DX11.DeviceContext.Device, nextTemperatureTexture);

            volumePressureTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumePressureTex);
            volumeDivergenceTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeDivergenceTex);
            volumeDensityTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeDensityTex);
            volumeVelocityTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeVelocityTex);
            volumeTempTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeTempTex);
            volumeVelocityFieldTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeVelocityFieldTex);
            volumeWallTexUav = new UnorderedAccessView(DX11.DeviceContext.Device, volumeWallTex);

            nextVelocityTextureUav = new UnorderedAccessView(DX11.DeviceContext.Device, nextVelocityTexture);
            nextDensityTextureUav = new UnorderedAccessView(DX11.DeviceContext.Device, nextDensityTexture);
            nextPressureTextureUav = new UnorderedAccessView(DX11.DeviceContext.Device, nextPressureTexture);
            nextTemperatureTextureUav = new UnorderedAccessView(DX11.DeviceContext.Device, nextTemperatureTexture);

            volumePressureTexUav.DebugName = "vptu";
            volumeDensityTexUav.DebugName = "vdtu";


            Texture2DDescription desc = new Texture2DDescription
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
            };
            frontText = new Texture2D(DX11.Device, desc);
            backText = new Texture2D(DX11.Device, desc);
            frontRTV = new RenderTargetView(DX11.Device, frontText);
            backRTV = new RenderTargetView(DX11.Device, backText);
            frontSRV = new ShaderResourceView(DX11.Device, frontText);
            backSRV = new ShaderResourceView(DX11.Device, backText);

            frontDepthText = new Texture2D(DX11.Device, DX11.DepthStencilBufferInObj.Description);
            backDepthText = new Texture2D(DX11.Device, DX11.DepthStencilBufferInObj.Description);

            frontDepthDSV = new DepthStencilView(DX11.Device, frontDepthText, DX11.DepthStencilViewInObj.Description);
            backDepthDSV = new DepthStencilView(DX11.Device, backDepthText, DX11.DepthStencilViewInObj.Description);

            ShaderResourceViewDescription dsrvDesc = new ShaderResourceViewDescription()
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0,
                }
            };

            inObjectsDepthSRV = new ShaderResourceView(DX11.DeviceContext.Device, DX11.DepthStencilBufferInObj, dsrvDesc);
            frontDepthSRV = new ShaderResourceView(DX11.DeviceContext.Device, frontDepthText, dsrvDesc);
            backDepthSRV = new ShaderResourceView(DX11.DeviceContext.Device, backDepthText, dsrvDesc);

            return true;
        }

        public bool PhisicsStep()
        {
            PhysicsShader.Render(DX11.DeviceContext);
            DX11.ResetDeviceContext();
            return true;
        }


        public bool Render(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            //worldMatrix = worldMatrix * Matrix.Translation(-1.5f, 0, -1.5f);
            Matrix worldViewProj = worldMatrix * viewMatrix * projectionMatrix;

            frames++;
            if (frames == 12)
                PhisicsStep();
            if (frames == 24)
                PhisicsStep();
            if (frames == 20)
            {
                PhisicsStep();
                frames = 0;
            }

            //volumePressureTex.SetData<float>(scalar);

            /*Texture2D back =*/
            RenderToTexture(worldViewProj, VolumeShader.RENDER_BACK);
            /*Texture2D front =*/
            RenderToTexture(worldViewProj, VolumeShader.RENDER_FRONT);

            //ShaderResourceView bview = new ShaderResourceView(DX11.DeviceContext.Device, back);
            //ShaderResourceView fview = new ShaderResourceView(DX11.DeviceContext.Device, front);

            DX11.TurnOffInObjectRender();
            //DX11.DeviceContext.OutputMerger.ResetTargets();
            //frames++;
            //if (frames > 30)
            //{
            //frames = 0;
            //animatetime++;
            //animate3DTexture(animatetime);
            //}

            DX11.TurnOnAlphaBlending();
            VolumeBox.Render(DX11.DeviceContext);
            //if (!VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, vDimension, VolumeShader.RENDER_VOLUME, fview, bview, depthview, fdvolume))
            //    return false;
            //if (!VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, magicTeapot, VolumeShader.RENDER_VOLUME, fview, bview, fvolume))
            //    return false;
            bool k = VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, vDimension, VolumeShader.RENDER_VOLUME, this);
            if (!k) Console.Out.WriteLine("asd");
            DX11.TurnOffAlphaBlending();

            /*
            bview.Dispose();
            fview.Dispose();

            back.Dispose();
            front.Dispose();
            //VolumeShader.unboundShaderRes();
            */
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



        private void RenderToTexture(Matrix worldViewProj, string renderMethod)
        {


            RenderTargetView renderToTextureRTV;
            DepthStencilView renderToTextureDSV;
            // 2) Create a RenderTargetView (and an optional ShaderResourceView):
            if (renderMethod == VolumeShader.RENDER_BACK)
            {
                renderToTextureRTV = backRTV;
                renderToTextureDSV = backDepthDSV;
            }
            else
            {
                renderToTextureRTV = frontRTV;
                renderToTextureDSV = frontDepthDSV;
            }
            //var renderToTextureSRV = new ShaderResourceView(DX11.Device, renderToTexture);

            //clear them
            DX11.DeviceContext.ClearDepthStencilView(renderToTextureDSV, DepthStencilClearFlags.Depth, 1, 0);
            DX11.DeviceContext.ClearRenderTargetView(renderToTextureRTV, new Color4(0.0f));

            // 3) Bind the render target to the output in your rendering code:
            DX11.DeviceContext.OutputMerger.SetRenderTargets(renderToTextureDSV, renderToTextureRTV);

            // 4) Don't forget to setup the viewport for this particular render target
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            VolumeBox.Render(DX11.DeviceContext);
            VolumeShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldViewProj, renderMethod);

            DX11.DeviceContext.OutputMerger.SetTargets(DX11.DepthStencilView, DX11.RenderTargetView);
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

            //renderToTextureRTV.Dispose();
            //return renderToTexture;
        }

        private void init3DTextures()
        {
            int size = (int)(vDimension.X * vDimension.Y * vDimension.Z);
            scalar = new float[size];
            for (int i = 0; i < vDimension.X; i++)
            {
                for (int j = 0; j < vDimension.Y; j++)
                {
                    for (int k = 0; k < vDimension.Z; k++)
                    {
                        int l = (int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k);
                        scalar[l] = 0.2f;
                        //scalar[l] = 1 - l / (vDimension.X * vDimension.Y * vDimension.Z);
                    }
                }
            }
            for (int i = 30; i < 50; i++)
            {
                for (int j = 30; j < 50; j++)
                {
                    for (int k = 30; k < 50; k++)
                    {
                        int l = (int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k);
                        scalar[l] = 0.9f;
                        //scalar[l] = 1 - l / (vDimension.X * vDimension.Y * vDimension.Z);
                    }
                }
            }

            float[] scalar4d0 = new float[size * 4];
            for (int i = 0; i < scalar4d0.Length; i++)
            {
                scalar4d0[i] = 0;
            }
            float[] scalar1d0 = new float[size];
            for (int i = 0; i < scalar1d0.Length; i++)
            {
                scalar1d0[i] = 0;
            }
            float[] scalar4d0010 = new float[size * 4];
            for (int i = 0; i < scalar4d0010.Length; i++)
            {
                //if (i % 4 == 2) scalar4d0010[i] = 0.001f; else scalar4d0010[i] = 0;
                scalar4d0010[i] = 0f;
            }
            float[] scalar1d05 = new float[size];
            for (int i = 0; i < scalar1d0.Length; i++)
            {
                scalar1d05[i] = 0.5f;
            }


            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 20) * 4] = 0.5f;
            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 20) * 4 + 1] = 0.5f;
            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 20) * 4 + 2] = 0.5f;

            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 10) * 4] = 0.5f;
            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 10) * 4 + 1] = 0.5f;
            scalar4d0010[(int)(10 * vDimension.Y * vDimension.Z + 10 * vDimension.Z + 10) * 4 + 2] = 0.5f;

            float[] wall = initWall();

            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(DX11.Device);
            volumePressureTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeTempTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeDensityTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeVelocityTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeVelocityFieldTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeWallTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            volumeDivergenceTex = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });


            nextVelocityTexture = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            nextDensityTexture = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            nextPressureTexture = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });
            nextTemperatureTexture = Texture3D.New(toolkitdiv, new Texture3DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                Depth = (int)vDimension.Z,
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });


            volumePressureTex.SetData<float>(scalar);
            volumeTempTex.SetData<float>(scalar);
            volumeDensityTex.SetData<float>(scalar4d0);
            volumeDivergenceTex.SetData<float>(scalar1d05);
            volumeVelocityTex.SetData<float>(scalar4d0);
            volumeWallTex.SetData<float>(wall);
        }

        private float[] initWall()
        {
            int size = (int)(vDimension.X * vDimension.Y * vDimension.Z);
            float[] ret = new float[size * 4];
            /*
            //all vexel is a notwall
            for (int i = 0; i < size * 4; i++)
            {
                if (i % 4 == 3)
                {
                    ret[i] = 1;
                }
            }*/

            for (int i = 0; i < vDimension.X; i++)
            {
                Vector3 vec = new Vector3(0, 0, 0);
                if (i == 0) vec.X = 1;
                if (i == vDimension.X - 1) vec.X = -1;
                for (int j = 0; j < vDimension.Y; j++)
                {
                    vec.Y = 0;
                    if (j == 0) vec.Y = 1;
                    if (j == vDimension.Y - 1) vec.Y = -1;
                    for (int k = 0; k < vDimension.Z; k++)
                    {
                        vec.Z = 0;
                        if (k == 0) vec.Z = 1;
                        if (k == vDimension.Z - 1) vec.Z = -1;
                        ret[(int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k) * 4 + 0] = -vec.X;
                        ret[(int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k) * 4 + 1] = -vec.Y;
                        ret[(int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k) * 4 + 2] = -vec.Z;
                        ret[(int)(i * vDimension.Y * vDimension.Z + j * vDimension.Z + k) * 4 + 3] = vec.Length();
                    }
                }
            }


            return ret;
        }

        public void animate3DTexture(int t)
        {
            for (int i = 0; i < vDimension.X; i++)
            {
                for (int j = 0; j < vDimension.Y; j++)
                {
                    for (int k = 0; k < vDimension.Z; k++)
                    {
                        scalar[adresser(i, j, k, 0)] = Noise.noise(new Vector4(i / 10.0f, j / 10f, k / 10f, t / 200f)) + 0.3f;
                        scalar[adresser(i, j, k, 1)] = Noise.noise4D(new Vector4(i / 10.0f, j / 10.0f, k / 10.0f, (t + 1) / 100.0f));
                    }
                }
            }
            volumePressureTex.SetData<float>(scalar);
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
        public void Dispose()
        {
            /*if (frontRenderTexture != null) { frontRenderTexture.Dispose(); }
            if (frontRTV != null) { frontRTV.Dispose(); }
            if (frontView != null) { frontView.Dispose(); }
            if (backRenderTexture != null) { backRenderTexture.Dispose(); }
            if (backRTV != null) { backRTV.Dispose(); }
            if (backView != null) { backView.Dispose(); }
            if (fvolume != null) { fvolume.Dispose(); }*/
        }

    }
}
