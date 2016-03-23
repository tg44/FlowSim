using fluid.CoreDraw;
using fluid.D3Draw.ShaderLoaders;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Device = SharpDX.Direct3D11.Device;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace fluid.D2Draw.ShaderLoaders
{
    class Physics2DShader : TextureShader
    {
        InputLayout Layout { get; set; }

        Effect eff;

        public Vector3 vDimension = new Vector3(32, 32, 32);

        #region textures
        public Texture2D volumePressureTex;
        public Texture2D volumeDivergenceTex;
        public Texture2D volumeDensityTex;
        public Texture2D volumeVelocityTex;
        public Texture2D volumeTempTex;
        public Texture2D volumeStaticTempTex;
        public Texture2D volumeWallTex;
        public Texture2D volumeVelocityFieldTex;
        public Texture2D nextVelocityTexture;
        public Texture2D nextDensityTexture;
        public Texture2D nextPressureTexture;
        public Texture2D nextTemperatureTexture;

        public ShaderResourceView volumePressuresrv;
        public ShaderResourceView volumeTempsrv;
        public ShaderResourceView volumeStaticTempsrv;
        public ShaderResourceView volumeDivergencesrv;
        public ShaderResourceView volumeDensitysrv;
        public ShaderResourceView volumeVelocitysrv;
        public ShaderResourceView volumeWallTexsrv;
        public ShaderResourceView volumeVelocityFieldTexsrv;
        public ShaderResourceView nextVelocityTexturesrv;
        public ShaderResourceView nextDensityTexturesrv;
        public ShaderResourceView nextPressureTexturesrv;
        public ShaderResourceView nextTemperatureTexturesrv;

        public UnorderedAccessView volumePressureTexUav;
        public UnorderedAccessView volumeDivergenceTexUav;
        public UnorderedAccessView volumeDensityTexUav;
        public UnorderedAccessView volumeVelocityTexUav;
        public UnorderedAccessView volumeTempTexUav;
        //public UnorderedAccessView volumeVelocityFieldTexUav;
        public UnorderedAccessView nextVelocityTextureUav;
        public UnorderedAccessView nextDensityTextureUav;
        public UnorderedAccessView nextPressureTextureUav;
        public UnorderedAccessView nextTemperatureTextureUav;
        #endregion textures

        public float dt = 1 / 100.0f;

        public Vector2 Heat = new Vector2(0, 1);
        public Vector2 Sensitivity = new Vector2(0, 1);

        private string ShadersFilePath = @"..\..\Shaders\";

        public Physics2DShader() { }

        public bool Initialize(DX11 DX11, SharpDX.Direct3D11.Texture2D wall, SharpDX.Direct3D11.Texture2D temp, SharpDX.Direct3D11.Texture2D velocity)
        {
            vDimension.X = DX11.Width;
            vDimension.Y = DX11.Height;
            // Initialize the vertex and pixel shaders.
            InitializeShader(DX11.Device, DX11.Handle, "grid2d.fx");
            InitializeTextures(DX11.Device, wall, temp, velocity);
            return true;
        }

        private void InitializeTextures(Device device, SharpDX.Direct3D11.Texture2D wall, SharpDX.Direct3D11.Texture2D temp, SharpDX.Direct3D11.Texture2D velocity)
        {
            Texture2DDescription descR = new Texture2DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            Texture2DDescription descRGBA = new Texture2DDescription
            {
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
                Format = Format.R32G32B32A32_Float,
                Width = (int)vDimension.X,
                Height = (int)vDimension.Y,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            var toolkitdiv = SharpDX.Toolkit.Graphics.GraphicsDevice.New(device);

            volumePressureTex = Texture2D.New(toolkitdiv, descR);
            volumeTempTex = Texture2D.New(toolkitdiv, descRGBA);
            volumeStaticTempTex = Texture2D.New(toolkitdiv, temp);
            volumeDensityTex = Texture2D.New(toolkitdiv, descRGBA);
            volumeVelocityTex = Texture2D.New(toolkitdiv, descRGBA);
            volumeVelocityFieldTex = Texture2D.New(toolkitdiv, velocity);
            volumeWallTex = Texture2D.New(toolkitdiv, wall);
            volumeDivergenceTex = Texture2D.New(toolkitdiv, descR);

            nextVelocityTexture = Texture2D.New(toolkitdiv, descRGBA);
            nextDensityTexture = Texture2D.New(toolkitdiv, descRGBA);
            nextPressureTexture = Texture2D.New(toolkitdiv, descR);
            nextTemperatureTexture = Texture2D.New(toolkitdiv, descR);

            volumePressuresrv = new ShaderResourceView(device, volumePressureTex);
            volumeTempsrv = new ShaderResourceView(device, volumeTempTex);
            volumeStaticTempsrv = new ShaderResourceView(device, volumeStaticTempTex);
            volumeDivergencesrv = new ShaderResourceView(device, volumeDivergenceTex);
            volumeDensitysrv = new ShaderResourceView(device, volumeDensityTex);
            volumeVelocitysrv = new ShaderResourceView(device, volumeVelocityTex);
            volumeVelocityFieldTexsrv = new ShaderResourceView(device, volumeVelocityFieldTex);
            volumeWallTexsrv = new ShaderResourceView(device, volumeWallTex);

            nextVelocityTexturesrv = new ShaderResourceView(device, nextVelocityTexture);
            nextDensityTexturesrv = new ShaderResourceView(device, nextDensityTexture);
            nextPressureTexturesrv = new ShaderResourceView(device, nextPressureTexture);
            nextTemperatureTexturesrv = new ShaderResourceView(device, nextTemperatureTexture);

            volumePressureTexUav = new UnorderedAccessView(device, volumePressureTex);
            volumeDivergenceTexUav = new UnorderedAccessView(device, volumeDivergenceTex);
            volumeDensityTexUav = new UnorderedAccessView(device, volumeDensityTex);
            volumeVelocityTexUav = new UnorderedAccessView(device, volumeVelocityTex);
            volumeTempTexUav = new UnorderedAccessView(device, volumeTempTex);
            //volumeVelocityFieldTexUav = new UnorderedAccessView(device, volumeVelocityFieldTex);

            nextVelocityTextureUav = new UnorderedAccessView(device, nextVelocityTexture);
            nextDensityTextureUav = new UnorderedAccessView(device, nextDensityTexture);
            nextPressureTextureUav = new UnorderedAccessView(device, nextPressureTexture);
            nextTemperatureTextureUav = new UnorderedAccessView(device, nextTemperatureTexture);
        }

        public override void Dispose()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShuddownShader();
            base.Dispose();
        }

        public ShaderResourceView RenderPhysics(DeviceContext deviceContext)
        {
            // advect + add source
            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref vDimension);

            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("density").AsShaderResource().SetResource(volumeDensitysrv);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(volumeTempsrv);
            eff.GetVariableByName("statictemp").AsShaderResource().SetResource(volumeStaticTempsrv);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(nextVelocityTextureUav);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(nextDensityTextureUav);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set(nextTemperatureTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("advect").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref volumeVelocityTex, ref nextVelocityTexture);
            swap(ref volumeVelocitysrv, ref nextVelocityTexturesrv);
            swap(ref volumeVelocityTexUav, ref nextVelocityTextureUav);

            swap(ref volumeDensityTex, ref nextDensityTexture);
            swap(ref volumeDensitysrv, ref nextDensityTexturesrv);
            swap(ref volumeDensityTexUav, ref nextDensityTextureUav);

            swap(ref volumeTempTex, ref nextTemperatureTexture);
            swap(ref volumeTempsrv, ref nextTemperatureTexturesrv);
            swap(ref volumeTempTexUav, ref nextTemperatureTextureUav);

            deviceContext.ClearState();

            //temperature
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("density").AsShaderResource().SetResource(volumeDensitysrv);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(volumeTempsrv);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("bouyancy").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref volumeVelocityTex, ref nextVelocityTexture);
            swap(ref volumeVelocitysrv, ref nextVelocityTexturesrv);
            swap(ref volumeVelocityTexUav, ref nextVelocityTextureUav);

            deviceContext.ClearState();



            ////velocyfield
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocyFieldScale").AsScalar().Set(dt);

            eff.GetVariableByName("velocyFieldScale").AsScalar().Set(0.5f);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("velocyfield").AsShaderResource().SetResource(volumeVelocityFieldTexsrv);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("velocyfield").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref volumeVelocityTex, ref nextVelocityTexture);
            swap(ref volumeVelocitysrv, ref nextVelocityTexturesrv);
            swap(ref volumeVelocityTexUav, ref nextVelocityTextureUav);

            deviceContext.ClearState();
            /*
            ////wall
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(volumeWallTexsrv);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("wall").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref volumeVelocityTex, ref nextVelocityTexture);
            swap(ref volumeVelocitysrv, ref nextVelocityTexturesrv);
            swap(ref volumeVelocityTexUav, ref nextVelocityTextureUav);

            deviceContext.ClearState();
            */
            //// divergence
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref vDimension);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(volumeWallTexsrv);
            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set(volumeDivergenceTexUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("divergence").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            deviceContext.ClearState();

            //// jacobi pressure
            Vector4 nullColor = new Vector4(0, 0, 0, 0);
            //context->ClearUnorderedAccessViewFloat(pressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(volumePressureTexUav, nullColor);
            //context->ClearUnorderedAccessViewFloat(nextPressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(nextPressureTextureUav, nullColor);
            //for(int i=0; i<30; i++)
            //TODO: ez itt fixen 10
            for (int i = 0; i < 50; i++)
            {


                eff.GetVariableByName("dt").AsScalar().Set(dt);

                eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref vDimension);
                eff.GetVariableByName("wall").AsShaderResource().SetResource(volumeWallTexsrv);
                eff.GetVariableByName("divergence").AsShaderResource().SetResource(volumeDivergencesrv);
                eff.GetVariableByName("pressure").AsShaderResource().SetResource(volumePressuresrv);
                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set(nextPressureTextureUav);

                eff.GetTechniqueByName("gridFluid").GetPassByName("jacobiPressure").Apply(deviceContext);
                deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set((UnorderedAccessView)null);

                swap(ref volumePressureTex, ref nextPressureTexture);
                swap(ref volumePressuresrv, ref nextPressureTexturesrv);
                swap(ref volumePressureTexUav, ref nextPressureTextureUav);

                deviceContext.ClearState();
            }

            //// project
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref vDimension);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(volumeWallTexsrv);
            eff.GetVariableByName("pressure").AsShaderResource().SetResource(volumePressuresrv);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(volumeVelocitysrv);
            eff.GetVariableByName("density").AsShaderResource().SetResource(volumeDensitysrv);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(nextVelocityTextureUav);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(nextDensityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("project").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, (int)vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref volumeVelocityTex, ref nextVelocityTexture);
            swap(ref volumeVelocitysrv, ref nextVelocityTexturesrv);
            swap(ref volumeVelocityTexUav, ref nextVelocityTextureUav);

            swap(ref volumeDensityTex, ref nextDensityTexture);
            swap(ref volumeDensitysrv, ref nextDensityTexturesrv);
            swap(ref volumeDensityTexUav, ref nextDensityTextureUav);
            deviceContext.ClearState();



            return volumeDensitysrv;
        }

        public override bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture)
        {
            RenderTexture(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix);
            return true;
        }

        public void RenderTexture(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {

            Matrix worldViewProj = Matrix.Multiply(Matrix.Multiply(worldMatrix, viewMatrix), projectionMatrix);

            EffectTechnique efftech = eff.GetTechniqueByName("gridFluid");
            EffectPass renderpass = eff.GetTechniqueByName("gridFluid").GetPassByName("visualize");

            eff.GetVariableByName("divergence").AsShaderResource().SetResource(volumeDivergencesrv);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(volumeTempsrv);
            eff.GetVariableByName("WorldViewProj").AsMatrix().SetMatrix(worldViewProj);
            eff.GetVariableByName("Heatmap").AsVector().Set(Heat);
            eff.GetVariableByName("Sensitivity").AsVector().Set(Sensitivity);
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            for (int i = 0; i < efftech.Description.PassCount; ++i)
            {
                renderpass.Apply(deviceContext);
                deviceContext.DrawIndexed(indexCount, 0, 0);
            }

            deviceContext.ClearState();
        }



        private void ShuddownShader()
        {
            // Release the layout.
            if (Layout != null)
            {
                Layout.Dispose();
                Layout = null;
            }
        }

        private bool InitializeShader(Device device, IntPtr windowHandler, string vsFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = ShadersFilePath + vsFileName;
                var effectShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "fx_5_0", ShaderFlags.Debug, EffectFlags.None);
                eff = new Effect(device, effectShaderByteCode);

                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                Console.Out.WriteLine(ex.Message);
                return false;
            };
        }

        #region swaps

        private void swap(ref Texture2D a, ref Texture2D b)
        {
            Texture2D t = a;
            a = b;
            b = t;
        }

        private void swap(ref SharpDX.Direct3D11.Texture2D a, ref SharpDX.Direct3D11.Texture2D b)
        {
            SharpDX.Direct3D11.Texture2D t = a;
            a = b;
            b = t;
        }

        private void swap(ref ShaderResourceView a, ref ShaderResourceView b)
        {
            ShaderResourceView t = a;
            a = b;
            b = t;
        }

        private void swap(ref UnorderedAccessView a, ref UnorderedAccessView b)
        {
            UnorderedAccessView t = a;
            a = b;
            b = t;
        }

        #endregion swaps
    }
}
