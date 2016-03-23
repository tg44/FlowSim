using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;

namespace fluid.D3DrawModelsSources.ShaderLoaders
{
    class PhysicsShader
    {
        private string ShadersFilePath = @"..\..\Shaders\";

        InputLayout velocityLayout;
        InputLayout densityLayout;
        InputLayout pressureLayout;
        InputLayout divergenceLayout;
        Volume Volume;


        Effect eff;
        //EffectTechnique efftech;

        public bool Initialize(SharpDX.Direct3D11.Device device, IntPtr windowHandler, Volume volume)
        {
            Volume = volume;
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowHandler, "gridfluid.fx");
        }
        public void Dispose()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShutdownShader();
        }
        private bool InitializeShader(Device device, IntPtr windowHandler, string vsFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = ShadersFilePath + vsFileName;

                // Compile the vertex shader code.
                var effectShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "fx_5_0", ShaderFlags.Debug, EffectFlags.None);
                eff = new Effect(device, effectShaderByteCode);


                //var computeShader = new ComputeShader(device, effectShaderByteCode);



                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                Console.Out.WriteLine(ex.Message);
                return false;
            };
        }
        private void ShutdownShader()
        {

            // Release the layout.
            if (velocityLayout != null)
            {
                velocityLayout.Dispose();
                velocityLayout = null;
            }
            if (densityLayout != null)
            {
                densityLayout.Dispose();
                densityLayout = null;
            }
            if (pressureLayout != null)
            {
                pressureLayout.Dispose();
                pressureLayout = null;
            }
            if (divergenceLayout != null)
            {
                divergenceLayout.Dispose();
                divergenceLayout = null;
            }

        }



        public bool Render(DeviceContext deviceContext)
        {
            // Set the shader parameters that it will use for rendering.
            //if (!SetShaderParameters(deviceContext, velocity, density, pressure, divergence))
            //    return false;

            /*
            //wall
            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref Volume.vDimension);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(Volume.fdvolumeWallTex);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("wall").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            deviceContext.ClearState();
            */
            // advect + add source
            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref Volume.vDimension);


            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("density").AsShaderResource().SetResource(Volume.fdvolumeDensity);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(Volume.fdvolumeTemp);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(Volume.nextDensityTextureUav);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set(Volume.nextTemperatureTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("advect").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            swap(ref Volume.volumeDensityTex, ref Volume.nextDensityTexture);
            swap(ref Volume.fdvolumeDensity, ref Volume.nextDensityTexturesrv);
            swap(ref Volume.volumeDensityTexUav, ref Volume.nextDensityTextureUav);

            swap(ref Volume.volumeTempTex, ref Volume.nextTemperatureTexture);
            swap(ref Volume.fdvolumeTemp, ref Volume.nextTemperatureTexturesrv);
            swap(ref Volume.volumeTempTexUav, ref Volume.nextTemperatureTextureUav);

            deviceContext.ClearState();

            //temperature
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("density").AsShaderResource().SetResource(Volume.fdvolumeDensity);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(Volume.fdvolumeTemp);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("bouyancy").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            deviceContext.ClearState();




            //// divergence
            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref Volume.vDimension);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(Volume.fdvolumeWallTex);
            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set(Volume.volumeDivergenceTexUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("divergence").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            deviceContext.ClearState();

            //// jacobi pressure
            Vector4 nullColor = new Vector4(0, 0, 0, 0);
            //context->ClearUnorderedAccessViewFloat(pressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(Volume.volumePressureTexUav, nullColor);
            //context->ClearUnorderedAccessViewFloat(nextPressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(Volume.nextPressureTextureUav, nullColor);
            //for(int i=0; i<30; i++)

            //TODO: ez itt fixen 10
            for (int i = 0; i < 2; i++)
            {
                deviceContext.ClearState();

                eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref Volume.vDimension);
                eff.GetVariableByName("wall").AsShaderResource().SetResource(Volume.fdvolumeWallTex);
                eff.GetVariableByName("divergence").AsShaderResource().SetResource(Volume.fdvolumeDivergence);
                eff.GetVariableByName("pressure").AsShaderResource().SetResource(Volume.fdvolumePressure);
                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set(Volume.nextPressureTextureUav);

                eff.GetTechniqueByName("gridFluid").GetPassByName("jacobiPressure").Apply(deviceContext);
                deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set((UnorderedAccessView)null);

                swap(ref Volume.volumePressureTex, ref Volume.nextPressureTexture);
                swap(ref Volume.fdvolumePressure, ref Volume.nextPressureTexturesrv);
                swap(ref Volume.volumePressureTexUav, ref Volume.nextPressureTextureUav);

            }
            deviceContext.ClearState();

            //// project
            eff.GetVariableByName("gridSize").AsVector().Set<Vector3>(ref Volume.vDimension);
            eff.GetVariableByName("wall").AsShaderResource().SetResource(Volume.fdvolumeWallTex);
            eff.GetVariableByName("pressure").AsShaderResource().SetResource(Volume.fdvolumePressure);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("density").AsShaderResource().SetResource(Volume.fdvolumeDensity);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(Volume.nextDensityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("project").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X / 128, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            swap(ref Volume.volumeDensityTex, ref Volume.nextDensityTexture);
            swap(ref Volume.fdvolumeDensity, ref Volume.nextDensityTexturesrv);
            swap(ref Volume.volumeDensityTexUav, ref Volume.nextDensityTextureUav);
            deviceContext.ClearState();



            //unboundShaderRes(deviceContext);

            return true;
        }

        #region swaps

        private void swap(ref Texture3D a, ref Texture3D b)
        {
            Texture3D t = a;
            a = b;
            b = t;
        }

        private void swap(ref SharpDX.Toolkit.Graphics.Texture3D a, ref SharpDX.Toolkit.Graphics.Texture3D b)
        {
            SharpDX.Toolkit.Graphics.Texture3D t = a;
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
