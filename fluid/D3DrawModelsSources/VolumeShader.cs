﻿using System;
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

            EffectPass renderpass = efftech.GetPassByIndex(0);

            for (int i = 0; i < efftech.Description.PassCount; ++i)
            {
                renderpass.Apply(deviceContext);
                deviceContext.DrawIndexed(indexCount, 0, 0);
            }

            
        }

        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            try
            {

                eff.GetVariableByName("worldMatrix").AsMatrix().SetMatrix(worldMatrix);
                eff.GetVariableByName("viewMatrix").AsMatrix().SetMatrix(viewMatrix);
                eff.GetVariableByName("projectionMatrix").AsMatrix().SetMatrix(projectionMatrix);

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
                var effectShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName,"fx_5_0", ShaderFlags.None, EffectFlags.None);
                eff = new Effect(device, effectShaderByteCode);
                efftech = eff.GetTechniqueByName("RenderColor");

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
               
                Layout = new InputLayout(device, efftech.GetPassByIndex(0).Description.Signature, inputElements);

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