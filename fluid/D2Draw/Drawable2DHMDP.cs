using fluid.D3DrawModelsSources;
using fluid.Forms;
using fluid.HMDP;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using fluid.D3DrawModelsSources.DrawTools;
using fluid.D3DrawModelsSources.ShaderLoaders;

namespace fluid.D2Draw
{
    class Drawable2DHMDP : Model, MovableModel
    {
        public HMDP2D Hmdp2D { get; set; }

        public HMDPInformation Info { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double R { get; set; }

        public string Name { get; set; }

        private Texture iconTex;
        private Texture heatTex;
        private Texture slidTex;
        private Texture throughTex;

        //betölti magának a floort
        public bool Initialize(SharpDX.Direct3D11.Device device)
        {
            // Load in the model data.
            if (!LoadModel("Floor.txt", true))
                return false;
            if (!InitializeBuffers(device))
                return false;
            if (!LoadTexture(device))
                return false;
            return true;
        }
        //kezeli magán a mátrixokat, amit vissza is tud adni



        //kezeli magán a textúra betöltését (Image-ből)
        private bool LoadTexture(SharpDX.Direct3D11.Device device)
        {
            // Create the texture object.
            Texture = new Texture();

            // Initialize the texture object.
            Texture.Initialize(device, Hmdp2D.Icon);

            iconTex = Texture;
            heatTex = new Texture();
            heatTex.Initialize(device, Hmdp2D.Heat);
            slidTex = new Texture();
            slidTex.Initialize(device, Hmdp2D.Solid);
            throughTex = new Texture();
            throughTex.Initialize(device, Hmdp2D.Throughput);

            return true;
        }

        //load all texture if not loaded
        //switch the actual
        public void switchTexture(string textureType)
        {
            if (textureType.Equals("icon")) Texture = iconTex;
            if (textureType.Equals("heat")) Texture = heatTex;
            if (textureType.Equals("solid")) Texture = slidTex;
            if (textureType.Equals("through")) Texture = throughTex;
        }

        internal void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, TextureShader textureShader)
        {
            Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix, textureShader, false);
        }
        internal void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, TextureShader textureShader, bool rotateColor)
        {
            base.Render(deviceContext);

            Matrix projection = Matrix.Identity;
            //scale
            projection = Matrix.Multiply(projection, Matrix.Scaling(Hmdp2D.x / 1000.0f, 1, Hmdp2D.y / 1000.0f));
            //rotate
            Matrix tmp;
            Matrix.RotationY((float)(Math.PI / 180 * R), out tmp);
            projection = Matrix.Multiply(projection, tmp);
            //move
            projection = Matrix.Multiply(projection, Matrix.Translation((float)(((float)X - 1000) / 1000.0f), (float)Z, (float)(((float)Y - 1000) / 1000.0f)));

            if (rotateColor)
            {
                Matrix.RotationZ((float)(Math.PI / 180 * R), out tmp);
                textureShader.Render(deviceContext, IndexCount, Matrix.Multiply(worldMatrix, projection), viewMatrix, projectionMatrix, Texture.TextureResource, tmp);
            }
            else
            {
                textureShader.Render(deviceContext, IndexCount, Matrix.Multiply(worldMatrix, projection), viewMatrix, projectionMatrix, Texture.TextureResource);
            }

        }

        public bool is2D()
        {
            return false;
        }

        public override void Dispose()
        {
            switchTexture("icon");

            if (heatTex != null)
            {
                heatTex.Dispose();
                heatTex = null;
            }
            if (slidTex != null)
            {
                slidTex.Dispose();
                slidTex = null;
            }
            if (throughTex != null)
            {
                throughTex.Dispose();
                throughTex = null;
            }

            base.Dispose();
        }




    }
}
