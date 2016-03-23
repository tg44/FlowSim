using fluid.D3Draw.ShaderLoaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace fluid.CoreDraw
{
    class Model
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ModelFormat
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
        }
        Buffer VertexBuffer { get; set; }
        Buffer IndexBuffer { get; set; }

        private string ModelFilePath = @"..\..\Models\";
        int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public ModelFormat[] ModelObject { get; private set; }
        public Texture Texture { get; protected set; }

        public bool Initialize(SharpDX.Direct3D11.Device device, string modelFormatFilename, string textureFileName)
        {
            // Load in the model data.
            if (!LoadModel(modelFormatFilename, false))
                return false;
            if (!InitializeBuffers(device))
                return false;
            if (!LoadTexture(device, textureFileName))
                return false;
            return true;
        }

        protected bool LoadModel(string modelFormatFilename, bool invertV)
        {
            modelFormatFilename = ModelFilePath + modelFormatFilename;
            List<string> lines = null;

            try
            {
                lines = new List<string>(File.ReadLines(modelFormatFilename));

                var vertexCountString = lines[0].Split(new char[] { ':' })[1].Trim();
                VertexCount = int.Parse(vertexCountString);
                IndexCount = VertexCount;
                ModelObject = new ModelFormat[VertexCount];

                for (var i = 4; i < lines.Count && i < 4 + VertexCount; i++)
                {
                    //lines[i] = lines[i].Replace(".", ",");
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";

                    var modelArray = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    ModelObject[i - 4] = new ModelFormat()
                    {
                        x = float.Parse(modelArray[0], NumberStyles.Any, ci),
                        y = float.Parse(modelArray[1], NumberStyles.Any, ci),
                        z = float.Parse(modelArray[2], NumberStyles.Any, ci),
                        tu = float.Parse(modelArray[3], NumberStyles.Any, ci),
                        tv = float.Parse(modelArray[4], NumberStyles.Any, ci),
                        nx = float.Parse(modelArray[5], NumberStyles.Any, ci),
                        ny = float.Parse(modelArray[6], NumberStyles.Any, ci),
                        nz = float.Parse(modelArray[7], NumberStyles.Any, ci)
                    };
                    if (invertV)
                    {
                        ModelObject[i - 4].tu = 1 - ModelObject[i - 4].tu;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            textureFileName = ModelFilePath + textureFileName;

            // Create the texture object.
            Texture = new Texture();

            // Initialize the texture object.
            Texture.Initialize(device, textureFileName);

            return true;
        }
        public virtual void Dispose()
        {
            // Release the model texture.
            ReleaseTexture();

            // Release the vertex and index buffers.
            ShutdownBuffers();

            // Release the model data.
            ReleaseModel();
        }
        private void ReleaseTexture()
        {
            // Release the texture object.
            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }
        }
        private void ReleaseModel()
        {
            ModelObject = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);
        }
        protected bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Create the vertex array.
                var vertices = new LightShader.Vertex[VertexCount];
                // Create the index array.
                var indices = new int[IndexCount];

                for (var i = 0; i < VertexCount; i++)
                {
                    vertices[i] = new LightShader.Vertex()
                    {
                        position = new Vector3(ModelObject[i].x, ModelObject[i].y, ModelObject[i].z),
                        texture = new Vector2(ModelObject[i].tu, ModelObject[i].tv),
                        normal = new Vector3(ModelObject[i].nx, ModelObject[i].ny, ModelObject[i].nz)
                    };

                    indices[i] = i;
                }

                // Create the vertex buffer.
                VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ShutdownBuffers()
        {
            // Return the index buffer.
            if (IndexBuffer != null)
            {
                IndexBuffer.Dispose();
                IndexBuffer = null;
            }

            // Release the vertex buffer.
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }
        }

        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, 32, 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

    }
}
