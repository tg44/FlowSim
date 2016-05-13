using fluid.CoreDraw;
using fluid.D3Draw.ShaderLoaders;
using fluid.Forms;
using fluid.HMDP;
using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace fluid.D2Draw
{
    class Drawable2DHMDP : Model, IMovableModel
    {
        public HMDP2D Hmdp2D { get; set; }

        public HMDPInformation Info { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double R { get; set; }

        public string Name { get; set; }
        public bool Active { get; set; }
        public HMDPTypeEnum ActiveHmdpType { get; private set; }

        private Texture iconTex;
        private Texture heatTex;
        private Texture solidTex;
        private Texture throughTex;

        //betölti magának a floort
        public bool Initialize(SharpDX.Direct3D11.Device device)
        {
            Active = true;
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
            solidTex = new Texture();
            solidTex.Initialize(device, Hmdp2D.Solid);
            throughTex = new Texture();
            throughTex.Initialize(device, Hmdp2D.Throughput);

            return true;
        }

        //load all texture if not loaded
        //switch the actual
        public void switchTexture(HMDPTypeEnum hmdpType)
        {
            this.ActiveHmdpType = hmdpType;
            if (hmdpType.Equals(HMDPTypeEnum.icon)) Texture = iconTex;
            if (hmdpType.Equals(HMDPTypeEnum.heat)) Texture = heatTex;
            if (hmdpType.Equals(HMDPTypeEnum.solid)) Texture = solidTex;
            if (hmdpType.Equals(HMDPTypeEnum.throughtput)) Texture = throughTex;
            if (ActiveHmdpType.Equals(HMDPTypeEnum.dust) && !Hmdp2D.Dust) Texture = solidTex;
        }

        internal void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, TextureShader textureShader)
        {
            Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix, textureShader, false);
        }
        internal void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, TextureShader textureShader, bool rotateColor)
        {
            if (!Active)
            {
                return;
            }

            if (ActiveHmdpType.Equals(HMDPTypeEnum.dust) && !Hmdp2D.Dust) return;
            if (!(ActiveHmdpType.Equals(HMDPTypeEnum.dust) || ActiveHmdpType.Equals(HMDPTypeEnum.icon)) && Hmdp2D.Dust) return;

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
            switchTexture(HMDPTypeEnum.icon);

            if (heatTex != null)
            {
                heatTex.Dispose();
                heatTex = null;
            }
            if (solidTex != null)
            {
                solidTex.Dispose();
                solidTex = null;
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
