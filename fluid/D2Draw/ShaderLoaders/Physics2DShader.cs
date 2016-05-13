using fluid.CoreDraw;
using fluid.D3Draw.ShaderLoaders;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Device = SharpDX.Direct3D11.Device;

namespace fluid.D2Draw.ShaderLoaders
{

    class Physics2DShader : TextureShader
    {
        InputLayout Layout { get; set; }

        Effect eff;

        public Vector2 vDimension = new Vector2(512, 512);

        #region textures

        public TextureWithNextHelper pressure;
        public TextureWithNextHelper divergence;
        public TextureWithNextHelper density;
        public TextureWithNextHelper velocity;
        public TextureWithNextHelper temperature;
        public TextureHelper staticTemperature;
        public TextureHelper staticDust;
        public TextureHelper velocyField;
        public TextureHelper wall;
        #endregion textures

        public float dt = 1 / 100.0f;

        public Vector2 Heat = new Vector2(0, 1);
        public Vector2 Sensitivity = new Vector2(0, 1);

        private string ShadersFilePath = @"..\..\Shaders\";

        public Physics2DShader() { }

        public bool Initialize(DX11 DX11, SharpDX.Direct3D11.Texture2D wall, SharpDX.Direct3D11.Texture2D temp, SharpDX.Direct3D11.Texture2D velocity, SharpDX.Direct3D11.Texture2D dust)
        {
            vDimension.X = DX11.Width;
            vDimension.Y = DX11.Height;
            // Initialize the vertex and pixel shaders.
            InitializeShader(DX11.Device, DX11.Handle, "grid2d.fx");
            InitializeTextures(DX11.Device, wall, temp, velocity, dust);
            return true;
        }

        private void InitializeTextures(Device device, SharpDX.Direct3D11.Texture2D wallField, SharpDX.Direct3D11.Texture2D tempField, SharpDX.Direct3D11.Texture2D velocityField, SharpDX.Direct3D11.Texture2D dustField)
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
            pressure = new TextureWithNextHelper(descR, device);
            divergence = new TextureWithNextHelper(descR, device);
            density = new TextureWithNextHelper(descR, device);
            velocity = new TextureWithNextHelper(descRGBA, device);
            temperature = new TextureWithNextHelper(descR, device);


            staticTemperature = new TextureHelper(tempField, device);
            velocyField = new TextureHelper(velocityField, device);
            wall = new TextureHelper(wallField, device, false);
            staticDust = new TextureHelper(dustField, device, false);
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
            csAdvect(deviceContext);

            //csTermoTransfer(deviceContext);
            //temperature
            csBouyancy(deviceContext);
            ////velocyfield
            csVelocyField(deviceContext);
            ////wall
            //csWall(deviceContext);
            //// divergence
            csDivergence(deviceContext);
            //// jacobi pressure
            csJacobi(deviceContext, 50);

            csTermoTransfer(deviceContext, 5);
            //// project
            csProject(deviceContext);

            return density.SRV;
        }

        private void csProject(DeviceContext deviceContext)
        {
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("gridSize").AsVector().Set<Vector2>(ref vDimension);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(wall.SRV);
            eff.GetVariableByName("pressure").AsShaderResource().SetResource(pressure.SRV);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("density").AsShaderResource().SetResource(density.SRV);
            eff.GetVariableByName("staticDensity").AsShaderResource().SetResource(staticDust.SRV);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(velocity.UAV);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(density.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("project").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            velocity.Swap();

            deviceContext.ClearState();
        }

        private void csJacobi(DeviceContext deviceContext, int iterationNumber)
        {
            Vector4 nullColor = new Vector4(0, 0, 0, 0);
            deviceContext.ClearUnorderedAccessView(pressure.Actual.UAV, nullColor);
            deviceContext.ClearUnorderedAccessView(pressure.Next.UAV, nullColor);
            for (int i = 0; i < iterationNumber; i++)
            {
                eff.GetVariableByName("dt").AsScalar().Set(dt);

                eff.GetVariableByName("gridSize").AsVector().Set<Vector2>(ref vDimension);
                eff.GetVariableByName("wall").AsShaderResource().SetResource(wall.SRV);
                eff.GetVariableByName("divergence").AsShaderResource().SetResource(divergence.SRV);
                eff.GetVariableByName("pressure").AsShaderResource().SetResource(pressure.SRV);
                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set(pressure.UAV);

                eff.GetTechniqueByName("gridFluid").GetPassByName("jacobiPressure").Apply(deviceContext);
                deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set((UnorderedAccessView)null);

                pressure.Swap();

                deviceContext.ClearState();

            }
        }

        private void csDivergence(DeviceContext deviceContext)
        {
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("gridSize").AsVector().Set<Vector2>(ref vDimension);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(wall.SRV);
            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set(divergence.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("divergence").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            divergence.Swap();

            deviceContext.ClearState();
        }

        private void csWall(DeviceContext deviceContext)
        {
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(wall.SRV);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(velocity.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("wall").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            velocity.Swap();

            deviceContext.ClearState();
        }

        private void csVelocyField(DeviceContext deviceContext)
        {
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocyFieldScale").AsScalar().Set(dt);

            eff.GetVariableByName("velocyFieldScale").AsScalar().Set(0.5f);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("velocyfield").AsShaderResource().SetResource(velocyField.SRV);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(velocity.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("velocyfield").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            velocity.Swap();

            deviceContext.ClearState();
        }

        private void csBouyancy(DeviceContext deviceContext)
        {
            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("density").AsShaderResource().SetResource(density.SRV);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(temperature.SRV);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(velocity.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("bouyancy").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            velocity.Swap();

            deviceContext.ClearState();
        }

        private void csTermoTransfer(DeviceContext deviceContext, int iterationCount)
        {
            for (int i = 0; i < iterationCount; i++)
            {
                eff.GetVariableByName("dt").AsScalar().Set(dt);
                eff.GetVariableByName("gridSize").AsVector().Set<Vector2>(ref vDimension);
                eff.GetVariableByName("temperature").AsShaderResource().SetResource(temperature.SRV);
                eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set(temperature.UAV);

                eff.GetTechniqueByName("gridFluid").GetPassByName("termotransfer").Apply(deviceContext);
                deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

                eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set((UnorderedAccessView)null);

                temperature.Swap();

                deviceContext.ClearState();
            }
        }

        private void csAdvect(DeviceContext deviceContext)
        {
            eff.GetVariableByName("gridSize").AsVector().Set<Vector2>(ref vDimension);

            eff.GetVariableByName("dt").AsScalar().Set(dt);

            eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            eff.GetVariableByName("density").AsShaderResource().SetResource(density.SRV);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(temperature.SRV);
            eff.GetVariableByName("statictemp").AsShaderResource().SetResource(staticTemperature.SRV);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(wall.SRV);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(velocity.UAV);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(density.UAV);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set(temperature.UAV);

            eff.GetTechniqueByName("gridFluid").GetPassByName("advect").Apply(deviceContext);
            deviceContext.Dispatch((int)vDimension.X / 128, (int)vDimension.Y, 1);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            velocity.Swap();
            density.Swap();
            temperature.Swap();

            deviceContext.ClearState();
        }

        public override bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture)
        {
            RenderTexture(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix);
            return true;
        }

        public void RenderTexture(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, RenderTextureEnum rendertexType = RenderTextureEnum.def)
        {

            Matrix worldViewProj = Matrix.Multiply(Matrix.Multiply(worldMatrix, viewMatrix), projectionMatrix);

            EffectTechnique efftech = eff.GetTechniqueByName("gridFluid");
            EffectPass renderpass = null;
            if (RenderTextureEnum.def == rendertexType)
            {
                renderpass = eff.GetTechniqueByName("gridFluid").GetPassByName("visualize");
                eff.GetVariableByName("density").AsShaderResource().SetResource(density.SRV);
                eff.GetVariableByName("temperature").AsShaderResource().SetResource(temperature.SRV);
            }
            else if (RenderTextureEnum.velocity == rendertexType)
            {
                renderpass = eff.GetTechniqueByName("gridFluid").GetPassByName("visualizeVel");
                eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity.SRV);
            }
            else
            {
                renderpass = eff.GetTechniqueByName("gridFluid").GetPassByName("visualizeF1");
                if (RenderTextureEnum.divergence == rendertexType)
                    eff.GetVariableByName("density").AsShaderResource().SetResource(divergence.SRV);
                if (RenderTextureEnum.heat == rendertexType)
                    eff.GetVariableByName("density").AsShaderResource().SetResource(temperature.SRV);
                if (RenderTextureEnum.pressure == rendertexType)
                    eff.GetVariableByName("density").AsShaderResource().SetResource(pressure.SRV);
            }




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


    }
}
