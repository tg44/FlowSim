using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using System.Drawing;
//using SharpDX.Toolkit.Graphics;
//using Effect = SharpDX.Toolkit.Graphics.Effect;

namespace fluid.D3Draw.ShaderLoaders
{
    class VolumeShader
    {
        public static readonly string RENDER_BACK = "RenderPositionBack";
        public static readonly string RENDER_FRONT = "RenderPositionFront";
        public static readonly string RENDER_VOLUME = "RayCastSimple";
        public static readonly string RENDER_DIRECTION = "RayCastDirection";
        public static readonly string RENDER_TEST = "RayCastTest";
        public static readonly string RENDER_CONSTANT_COLOR = "RenderConstansColor";

        [StructLayout(LayoutKind.Sequential)]
        internal struct Vertex
        {
            public static int AppendAlignedElement = 12;

            public Vector3 position;
            public Vector2 texture;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        InputLayout frontLayout;
        InputLayout backLayout;
        InputLayout volumeLayout;

        InputLayout Layout;



        Buffer ConstantMatrixBuffer { get; set; }
        private string ShadersFilePath = @"..\..\Shaders\";


        Effect eff;
        EffectTechnique efftech;


        //EffectShaderResourceVariable texture;

        public bool Initialize(SharpDX.Direct3D11.Device device, IntPtr windowHandler)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowHandler, "RayCasting.fx");
        }

        public void Dispose()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShutdownShader();
        }

        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldViewProj, Vector3 v3Size, String renderTech, ShaderResourceView fronttex, ShaderResourceView backtex, ShaderResourceView wdepthtex, ShaderResourceView fdepthtex, ShaderResourceView bdepthtex, ShaderResourceView volumePressure, ShaderResourceView volumeTemp, SizeF heat, SizeF sensi)
        {
            // Set the shader parameters that it will use for rendering.
            if (!SetShaderParameters(deviceContext, worldViewProj, v3Size, fronttex, backtex, wdepthtex, fdepthtex, bdepthtex, volumePressure, volumeTemp, heat, sensi))
                return false;

            if (!InitializeTechnique(deviceContext.Device, renderTech))
                return false;

            // Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            unboundShaderRes(deviceContext);

            return true;
        }
        public bool Render2(DeviceContext deviceContext, int indexCount, Matrix worldViewProj, Vector3 v3Size, String renderTech, ShaderResourceView fronttex, ShaderResourceView backtex, ShaderResourceView volumetex)
        {
            return true;
        }

        private void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            EffectPass renderpass = efftech.GetPassByIndex(0);

            for (int i = 0; i < efftech.Description.PassCount; ++i)
            {
                renderpass.Apply(deviceContext);
                deviceContext.DrawIndexed(indexCount, 0, 0);
            }


        }

        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldViewProj, Vector3 v3Size, ShaderResourceView fronttex, ShaderResourceView backtex, ShaderResourceView wdepthtex, ShaderResourceView fdepthtex, ShaderResourceView bdepthtex, ShaderResourceView volumePressure, ShaderResourceView volumeTemp, SizeF heat, SizeF sensi)
        {
            try
            {

                float maxSize = (float)Math.Max(v3Size.X, Math.Max(v3Size.Y, v3Size.Z));
                int Iteration = (int)(maxSize * (1.0f / 1.0f)); //second 1.0 coud be stepScale

                //Matrix wm = Matrix.Translation(0.3f, 0.3f, -2.0f);
                //wm.Transpose();
                //eff.GetVariableByName("World").AsMatrix().SetMatrix(worldMatrix);
                /*Vector3 StepSize = new Vector3(1.0f / v3Size.X, 1.0f / v3Size.Y, 1.0f / v3Size.Z);

                eff.GetVariableByName("WorldViewProj").AsMatrix().SetMatrix(worldViewProj);
                eff.GetVariableByName("StepSize").AsVector().Set(StepSize);
                /*eff.GetVariableByName("Iterations").AsScalar().Set(Iteration);
                */
                float[] scaleFaktorV3 = (Vector3.One / (Vector3.One * maxSize / (v3Size * Vector3.One))).ToArray();
                eff.GetVariableByName("ScaleFactor").AsVector().Set(new Vector4(scaleFaktorV3[0], scaleFaktorV3[1], scaleFaktorV3[2], 1.0f));


                eff.GetVariableByName("WorldViewProj").AsMatrix().SetMatrix(worldViewProj);
                eff.GetVariableByName("StepSize").AsVector().Set(Vector3.One / v3Size);
                eff.GetVariableByName("Iterations").AsScalar().Set(Iteration);

                eff.GetVariableByName("Heatmap").AsVector().Set(new Vector2(heat.Height, heat.Width));
                eff.GetVariableByName("Sensitivity").AsVector().Set(new Vector2(sensi.Height, sensi.Width));

                //eff.GetVariableByName("ScaleFactor").AsVector().Set(Vector4.One);


                if (fronttex != null)
                {
                    eff.GetVariableByName("Front").AsShaderResource().SetResource(fronttex);
                    //deviceContext.PixelShader.SetShaderResource(0, fronttex);
                }

                if (backtex != null)
                {
                    eff.GetVariableByName("Back").AsShaderResource().SetResource(backtex);
                    //deviceContext.PixelShader.SetShaderResource(1, backtex);
                }

                if (volumePressure != null)
                {
                    eff.GetVariableByName("VolumePressure").AsShaderResource().SetResource(volumePressure);
                    //deviceContext.PixelShader.SetShaderResource(2, volumetex);
                }

                if (volumeTemp != null)
                {
                    eff.GetVariableByName("VolumeTemp").AsShaderResource().SetResource(volumeTemp);
                    //deviceContext.PixelShader.SetShaderResource(2, volumetex);
                }

                if (wdepthtex != null)
                {
                    eff.GetVariableByName("WDepth").AsShaderResource().SetResource(wdepthtex);
                    //deviceContext.PixelShader.SetShaderResource(2, volumetex);
                }
                if (fdepthtex != null)
                {
                    eff.GetVariableByName("FDepth").AsShaderResource().SetResource(fdepthtex);
                    //deviceContext.PixelShader.SetShaderResource(2, volumetex);
                }
                if (bdepthtex != null)
                {
                    eff.GetVariableByName("BDepth").AsShaderResource().SetResource(bdepthtex);
                    //deviceContext.PixelShader.SetShaderResource(2, volumetex);
                }
                //eff.GetVariableByName("projectionMatrix").AsMatrix().SetMatrix(projectionMatrix);

                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }
        /*public bool setVolumeTexture(ShaderResourceView volumetex)
        {
            try
            {
                eff.GetVariableByName("Volume").AsShaderResource().SetResource(volumetex);
                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }*/
        private void ShutdownShader()
        {
            // Release the matrix constant buffer.
            if (ConstantMatrixBuffer != null)
            {
                ConstantMatrixBuffer.Dispose();
                ConstantMatrixBuffer = null;
            }

            // Release the layout.
            if (Layout != null)
            {
                Layout.Dispose();
                Layout = null;
            }
            if (frontLayout != null)
            {
                frontLayout.Dispose();
                frontLayout = null;
            }
            if (backLayout != null)
            {
                backLayout.Dispose();
                backLayout = null;
            }
            if (volumeLayout != null)
            {
                volumeLayout.Dispose();
                volumeLayout = null;
            }

            /*
            // Release the pixel shader.
            if (PixelShader != null)
            {
                PixelShader.Dispose();
                PixelShader = null;
            }

            // Release the vertex shader.
            if (VertexShader != null)
            {
                VertexShader.Dispose();
                VertexShader = null;
            }*/
        }
        public void unboundShaderRes(DeviceContext deviceContext)
        {
            //eff.GetVariableByName("outputDivergence").AsUnorderedAccessView().Set((UnorderedAccessView)null);
            eff.GetVariableByName("Front").AsShaderResource().SetResource(null);
            deviceContext.PixelShader.SetShaderResource(0, null);
            eff.GetVariableByName("Back").AsShaderResource().SetResource(null);
            deviceContext.PixelShader.SetShaderResource(1, null);
            eff.GetVariableByName("WDepth").AsShaderResource().SetResource(null);
            deviceContext.PixelShader.SetShaderResource(2, null);
            eff.GetVariableByName("FDepth").AsShaderResource().SetResource(null);
            deviceContext.PixelShader.SetShaderResource(3, null);
            eff.GetVariableByName("BDepth").AsShaderResource().SetResource(null);
            deviceContext.PixelShader.SetShaderResource(4, null);
            eff.GetVariableByName("VolumePressure").AsShaderResource().SetResource(null);
            //deviceContext.PixelShader.SetShaderResource(5, null);
            eff.GetVariableByName("VolumeTemp").AsShaderResource().SetResource(null);
            //deviceContext.PixelShader.SetShaderResource(6, null);

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


                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                Console.Out.WriteLine(ex.Message);
                return false;
            };
        }
        private bool InitializeTechnique(Device device, String effTech)
        {
            /*
            if (effTech == VolumeShader.RENDER_FRONT && frontLayout != null)
            {
                Layout = frontLayout;
                return true;
            }
            if (effTech == VolumeShader.RENDER_BACK && backLayout != null)
            {
                Layout = backLayout;
                return true;
            }
            if (effTech == VolumeShader.RENDER_VOLUME && volumeLayout != null)
            {
                Layout = volumeLayout;
                return true;
            }*/
            try
            {
                //Layout = null;
                efftech = eff.GetTechniqueByName(effTech);

                var inputElements = new InputElement[]
                {
                    new InputElement()
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = Vertex.AppendAlignedElement,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };
                Layout = new InputLayout(device, efftech.GetPassByIndex(0).Description.Signature, inputElements);
                /*
                if (effTech == VolumeShader.RENDER_FRONT)
                {
                    frontLayout = Layout;
                }
                if (effTech == VolumeShader.RENDER_BACK)
                {
                    backLayout = Layout;
                }
                if (effTech == VolumeShader.RENDER_VOLUME)
                {
                    volumeLayout = Layout;
                }*/

                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                Console.Out.WriteLine(ex.Message);
                return false;
            };
        }

        public bool Render(DeviceContext deviceContext, int p1, Matrix worldViewProj, string p2)
        {
            return Render(deviceContext, p1, worldViewProj, Vector3.One, p2, null, null, null, null, null, null, null);
        }
        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldViewProj, Vector3 v3Size, String renderTech, ShaderResourceView fronttex, ShaderResourceView backtex, ShaderResourceView wdepthtex, ShaderResourceView fdepthtex, ShaderResourceView bdepthtex, ShaderResourceView volumePressure, ShaderResourceView volumeTemp)
        {
            return Render(deviceContext, indexCount, worldViewProj, v3Size, renderTech, fronttex, backtex, wdepthtex, fdepthtex, bdepthtex, volumePressure, volumeTemp, new SizeF(0, 1), new SizeF(0, 1));
        }
        public bool Render2(DeviceContext deviceContext, int p1, Matrix worldViewProj, string p2)
        {
            return true;
        }

        public bool RenderSphere(DeviceContext deviceContext, int p1, Matrix worldViewProj)
        {
            return Render(deviceContext, p1, worldViewProj, Vector3.One, VolumeShader.RENDER_CONSTANT_COLOR, null, null, null, null, null, null, null);
        }

        public bool Render(DeviceContext deviceContext, int p1, Matrix worldViewProj, Vector3 vDimension, String p2, Volume volume)
        {
            return Render(deviceContext, p1, worldViewProj, vDimension, p2, volume.frontSRV, volume.backSRV, volume.inObjectsDepthSRV, volume.frontDepthSRV, volume.backDepthSRV, volume.fdvolumeDensity, volume.fdvolumeTemp, volume.Heatmap, volume.Sensitivitymap);
        }
    }
}
