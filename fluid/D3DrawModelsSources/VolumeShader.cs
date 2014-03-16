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

namespace fluid.D3DrawModelsSources
{
    class VolumeShader
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Vertex
        {
            public static int AppendAlignedElement = 12;

            public Vector3 position;
            public Vector4 color;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        InputLayout Layout { get; set; }
        Buffer ConstantMatrixBuffer { get; set; }
        private string ShadersFilePath = @"..\..\Shaders\";

        Effect eff;
        EffectTechnique efftech;
        EffectPass[] passes;
        //EffectShaderResourceVariable texture;

        public bool Initialize(SharpDX.Direct3D11.Device device, IntPtr windowHandler)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowHandler, "test.fx");
        }

        public void Shuddown()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShuddownShader();
        }

        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Set the shader parameters that it will use for rendering.
            if (!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix))
                return false;

            // Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            return true;
        }

        private void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            //deviceContext.VertexShader.Set(VertexShader);
            //deviceContext.PixelShader.Set(PixelShader);

            EffectPass renderpass = efftech.GetPassByIndex(0);
            renderpass.Apply(deviceContext);
            deviceContext.DrawIndexedInstanced(36 * 3, indexCount, 0, 0, 0);
            //deviceContext.DrawIndexed(indexCount, 0, 0);
            /*
            foreach (var pass in passes)
            {
                pass.Apply(deviceContext);
                // Render the triangle.
                deviceContext.DrawIndexed(indexCount, 0, 0);
                //deviceContext.DrawIndexedInstanced(36*3, indexCount, 0, 0, 0);

            }
            */
            
        }

        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            try
            {
                
                // Transpose the matrices to prepare them for shader.
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();
                
                eff.GetVariableByName("worldMatrix").AsMatrix().SetMatrix(worldMatrix);
                eff.GetVariableByName("viewMatrix").AsMatrix().SetMatrix(viewMatrix);
                eff.GetVariableByName("projectionMatrix").AsMatrix().SetMatrix(projectionMatrix);
                
                /*
                var matrixBuffer = new MatrixBuffer()
                {
                    world = worldMatrix,
                    view = viewMatrix,
                    projection = projectionMatrix
                };
                EffectConstantBuffer frameConstantBuffer =  this.eff.GetConstantBufferByIndex(0);
                */
                /* set values */
                //frameConstantBuffer.Set<MatrixBuffer>(0, matrixBuffer);
               // frameConstantBuffer.Set
                /*
                // Lock the constant buffer so it can be written to.
                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

                // Copy the matrices into the constant buffer.
                var matrixBuffer = new MatrixBuffer()
                {
                    world = worldMatrix,
                    view = viewMatrix,
                    projection = projectionMatrix
                };
                mappedResource.Write(matrixBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

                // Set the position of the constant buffer in the vertex shader.
                var bufferNumber = 0;

                // Finally set the constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferNumber, ConstantMatrixBuffer);
                */
                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        private void ShuddownShader()
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

        private bool InitializeShader(Device device, IntPtr windowHandler, string vsFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = ShadersFilePath + vsFileName;

                // Compile the vertex shader code.
                var effectShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "RayCastingVertexShader", "fx_5_0", ShaderFlags.Debug, EffectFlags.None);
                eff = new Effect(device, effectShaderByteCode);
                efftech = eff.GetTechniqueByName("RenderColor");
                passes = Enumerable.Range(0, efftech.Description.PassCount).Select(i => efftech.GetPassByIndex(i)).ToArray();


                // Now setup the layout of the data that goes into the shader.
                // This setup needs to match the VertexType structure in the Model and in the shader.
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
						SemanticName = "COLOR",
						SemanticIndex = 0,
						Format = Format.R32G32B32A32_Float,
						Slot = 0,
						AlignedByteOffset = ColorShader.Vertex.AppendAlignedElement,
						Classification = InputClassification.PerVertexData,
						InstanceDataStepRate = 0
					}
				};

                // Create the vertex input the layout.
                Layout = new InputLayout(device, passes[0].Description.Signature, inputElements);

                // Release the vertex and pixel shader buffers, since they are no longer needed.
                effectShaderByteCode.Dispose();

                // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
                var matrixBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<Matrix>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new Buffer(device, matrixBufferDesc);

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
