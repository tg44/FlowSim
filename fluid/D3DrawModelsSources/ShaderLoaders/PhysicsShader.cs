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

namespace fluid.D3DrawModelsSources
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
        public void Shuddown()
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


                var computeShader = new ComputeShader(device, effectShaderByteCode);



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
        public void unboundShaderRes(DeviceContext deviceContext)
        {





        }

        private bool SetShaderParameters(DeviceContext deviceContext, ShaderResourceView velocity, ShaderResourceView density, ShaderResourceView pressure, ShaderResourceView divergence)
        {
            try
            {
                if (velocity != null)
                {
                    eff.GetVariableByName("velocity").AsShaderResource().SetResource(velocity);
                }

                if (density != null)
                {
                    eff.GetVariableByName("density").AsShaderResource().SetResource(density);
                }

                if (pressure != null)
                {
                    eff.GetVariableByName("pressure").AsShaderResource().SetResource(pressure);
                }

                if (divergence != null)
                {
                    eff.GetVariableByName("divergence").AsShaderResource().SetResource(divergence);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }
        public bool Render(DeviceContext deviceContext)
        {
            // Set the shader parameters that it will use for rendering.
            //if (!SetShaderParameters(deviceContext, velocity, density, pressure, divergence))
            //    return false;




            // advect + add source
            //effect->GetVariableByName("injectionPos")->AsVector()->SetFloatVector((float*)&injectionPosition);
            //effect->GetVariableByName("injectionIntensity")->AsVector()->SetFloatVector((float*)&injectionIntensity);

            //effect->GetVariableByName("velocity")->AsShaderResource()->SetResource(velocitySrv);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            //effect->GetVariableByName("density")->AsShaderResource()->SetResource(densitySrv);
            eff.GetVariableByName("density").AsShaderResource().SetResource(Volume.fdvolumeDensity);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(Volume.fdvolumeTemp);
            //effect->GetVariableByName("outputVelocity")->AsUnorderedAccessView()->SetUnorderedAccessView(nextVelocityUav);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);
            //effect->GetVariableByName("outputDensity")->AsUnorderedAccessView()->SetUnorderedAccessView(nextDensityUav);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set(Volume.nextDensityTextureUav);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set(Volume.nextTemperatureTextureUav);

            //effect->GetTechniqueByName("gridFluid")->GetPassByName("advect")->Apply(0, context);
            eff.GetTechniqueByName("gridFluid").GetPassByName("advect").Apply(deviceContext);
            //context->Dispatch(32, 32, 32);
            deviceContext.Dispatch((int)Volume.vDimension.X, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            //effect->GetVariableByName("outputVelocity")->AsUnorderedAccessView()->SetUnorderedAccessView(NULL);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            //effect->GetVariableByName("outputDensity")->AsUnorderedAccessView()->SetUnorderedAccessView(NULL);
            eff.GetVariableByName("outputDensity").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("outputTemperature").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            //swap(velocityTexture, nextVelocityTexture);
            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            //swap(velocitySrv, nextVelocitySrv);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            //swap(velocityUav, nextVelocityUav);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);
            //swap(densityTexture, nextDensityTexture);
            swap(ref Volume.volumeDensityTex, ref Volume.nextDensityTexture);
            //swap(densitySrv, nextDensitySrv);
            swap(ref Volume.fdvolumeDensity, ref Volume.nextDensityTexturesrv);
            //swap(densityUav, nextDensityUav);
            swap(ref Volume.volumeDensityTexUav, ref Volume.nextDensityTextureUav);

            swap(ref Volume.volumeTempTex, ref Volume.nextTemperatureTexture);
            swap(ref Volume.fdvolumeTemp, ref Volume.nextTemperatureTexturesrv);
            swap(ref Volume.volumeTempTexUav, ref Volume.nextTemperatureTextureUav);

            //context->ClearState();
            deviceContext.ClearState();

            //temperature
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            eff.GetVariableByName("density").AsShaderResource().SetResource(Volume.fdvolumeDensity);
            eff.GetVariableByName("temperature").AsShaderResource().SetResource(Volume.fdvolumeTemp);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);

            eff.GetTechniqueByName("gridFluid").GetPassByName("bouyancy").Apply(deviceContext);
            deviceContext.Dispatch((int)Volume.vDimension.X, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            deviceContext.ClearState();

            //// divergence
            //effect->GetVariableByName("velocity")->AsShaderResource()->SetResource(velocitySrv);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            //effect->GetVariableByName("outputDivergence")->AsUnorderedAccessView()->SetUnorderedAccessView(divergenceUav);
            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set(Volume.volumeDivergenceTexUav);

            //effect->GetTechniqueByName("gridFluid")->GetPassByName("divergence")->Apply(0, context);
            eff.GetTechniqueByName("gridFluid").GetPassByName("divergence").Apply(deviceContext);
            //context->Dispatch(32, 32, 32);
            deviceContext.Dispatch((int)Volume.vDimension.X, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            //effect->GetVariableByName("outputDivergence")->AsUnorderedAccessView()->SetUnorderedAccessView(NULL);
            eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            //context->ClearState();
            deviceContext.ClearState();

            //// jacobi pressure
            //float nullColor[] = {0, 0, 0, 0};
            Vector4 nullColor = new Vector4(0, 0, 0, 0);
            //context->ClearUnorderedAccessViewFloat(pressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(Volume.volumePressureTexUav, nullColor);
            //context->ClearUnorderedAccessViewFloat(nextPressureUav, nullColor);
            deviceContext.ClearUnorderedAccessView(Volume.nextPressureTextureUav, nullColor);
            //for(int i=0; i<30; i++)
            for (int i = 0; i < (int)Volume.vDimension.X; i++)
            {
                //{
                //context->ClearState();
                deviceContext.ClearState();

                //effect->GetVariableByName("divergence")->AsShaderResource()->SetResource(divergenceSrv);
                eff.GetVariableByName("divergence").AsShaderResource().SetResource(Volume.fdvolumeDivergence);
                //effect->GetVariableByName("pressure")->AsShaderResource()->SetResource(pressureSrv);
                eff.GetVariableByName("pressure").AsShaderResource().SetResource(Volume.fdvolumePressure);
                //effect->GetVariableByName("outputPressure")->AsUnorderedAccessView()->SetUnorderedAccessView(nextPressureUav);
                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set(Volume.nextPressureTextureUav);

                //effect->GetTechniqueByName("gridFluid")->GetPassByName("jacobiPressure")->Apply(0, context);
                eff.GetTechniqueByName("gridFluid").GetPassByName("jacobiPressure").Apply(deviceContext);
                //context->Dispatch(32, 32, 32);
                deviceContext.Dispatch((int)Volume.vDimension.X, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

                //effect->GetVariableByName("outputPressure")->AsUnorderedAccessView()->SetUnorderedAccessView(NULL);
                eff.GetVariableByName("outputPressure").AsUnorderedAccessView().Set((UnorderedAccessView)null);

                //swap(pressureTexture, nextPressureTexture);
                swap(ref Volume.volumePressureTex, ref Volume.nextPressureTexture);
                //swap(pressureSrv, nextPressureSrv);
                swap(ref Volume.fdvolumePressure, ref Volume.nextPressureTexturesrv);
                //swap(pressureUav, nextPressureUav);
                swap(ref Volume.volumePressureTexUav, ref Volume.nextPressureTextureUav);

                //}
            }
            //context->ClearState();
            deviceContext.ClearState();

            //// project
            //effect->GetVariableByName("pressure")->AsShaderResource()->SetResource(pressureSrv);
            eff.GetVariableByName("pressure").AsShaderResource().SetResource(Volume.fdvolumePressure);
            //effect->GetVariableByName("velocity")->AsShaderResource()->SetResource(velocitySrv);
            eff.GetVariableByName("velocity").AsShaderResource().SetResource(Volume.fdvolumeVelocity);
            //effect->GetVariableByName("outputVelocity")->AsUnorderedAccessView()->SetUnorderedAccessView(nextVelocityUav);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set(Volume.nextVelocityTextureUav);

            //effect->GetTechniqueByName("gridFluid")->GetPassByName("project")->Apply(0, context);
            eff.GetTechniqueByName("gridFluid").GetPassByName("project").Apply(deviceContext);
            //context->Dispatch(32, 32, 32);
            deviceContext.Dispatch((int)Volume.vDimension.X, (int)Volume.vDimension.Y, (int)Volume.vDimension.Z);

            //effect->GetVariableByName("outputVelocity")->AsUnorderedAccessView()->SetUnorderedAccessView(NULL);
            eff.GetVariableByName("outputVelocity").AsUnorderedAccessView().Set((UnorderedAccessView)null);

            //swap(velocityTexture, nextVelocityTexture);
            swap(ref Volume.volumeVelocityTex, ref Volume.nextVelocityTexture);
            //swap(velocitySrv, nextVelocitySrv);
            swap(ref Volume.fdvolumeVelocity, ref Volume.nextVelocityTexturesrv);
            //swap(velocityUav, nextVelocityUav);
            swap(ref Volume.volumeVelocityTexUav, ref Volume.nextVelocityTextureUav);

            //context->ClearState();
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
