using fluid.D2Draw;
using fluid.D3DrawModelsSources;
using fluid.D3DrawModelsSources.DrawTools;
using fluid.D3DrawModelsSources.ShaderLoaders;
using fluid.Forms;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fluid.D2Draw
{
    class D2DrawModels : D3Drawer
    {
        private Camera _camera { get; set; }

        public Camera Camera { get { return _camera; } set { } }
        private DX11 DX11 { get; set; }

        private Model Wall { get; set; }

        private TextureShader TextureShader { get; set; }

        private List<Drawable2DHMDP> models = new List<Drawable2DHMDP>();

        private float z = 0.0f;

        public IList<Drawable2DHMDP> Models { get { return models.AsReadOnly(); } private set { } }


        public void Frame()
        {
            DX11.BeginScene(0.5f, 0.5f, 0.5f, 1.0f);

            _camera.Render2D();

            var viewMatrix = _camera.ViewMatrix;
            var worldMatrix = DX11.WorldMatrix;
            var projectionMatrix = DX11.ProjectionMatrix;

            //float rotation = 0.0f;


            //worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-0.5f, 0, -0.5f));


            Wall.Render(DX11.DeviceContext);
            TextureShader.Render(DX11.DeviceContext, Wall.IndexCount, worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-1f, -0f, -1f)), viewMatrix, projectionMatrix, Wall.Texture.TextureResource);

            foreach (var model in models)
            {
                model.Render(DX11.DeviceContext, DX11.WorldMatrix, viewMatrix, projectionMatrix, TextureShader);

            }

            DX11.EndScene();


        }

        public void addVars(SharpDX.Direct3D11.RenderTargetView renderView, SharpDX.DXGI.SwapChain swapChain, SharpDX.Direct3D11.DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler)
        { }

        public bool init(D3DrawModelsSources.DX11 DX11, System.Drawing.SizeF Heatmap, System.Drawing.SizeF Sensitivitymap)
        {
            if (this.DX11 != null) return false;

            this.DX11 = DX11;

            _camera = new Camera();

            // Set the initial position of the camera.
            _camera.SetPosition(-0.5f, 1.3f, -0.5f);
            _camera.lookAt = new Vector3(-0.5f, 0f, -0.5f);


            Wall = new Model();
            if (!Wall.Initialize(DX11.Device, "Floor.txt", "frame.dds"))
                Console.Out.WriteLine("Error on floor load!");

            TextureShader = new TextureShader();
            TextureShader.Initialize(DX11.Device, DX11.Handle);



            return true;
        }

        public MovableModel addFileLoader(HMDP.HMDPLoader loader)
        {
            z += 0.0001f;
            Drawable2DHMDP model = new Drawable2DHMDP();
            model.Hmdp2D = loader.Data2D;
            model.X = 0; ;
            model.Y = 0;
            model.Z = z;
            model.R = 0;
            model.Name = loader.Info.HardwareName;
            model.Info = loader.Info;
            model.Initialize(DX11.Device);

            models.Add(model);
            return model;
        }

        public void Dispose()
        {
            Wall.Dispose();
            Wall = null;
        }

        public FPS FPS
        {
            get { throw new NotImplementedException(); }
        }

        public CPU CPU
        {
            get { throw new NotImplementedException(); }
        }

        public System.Drawing.SizeF Heatmap
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Drawing.SizeF Sensitivitymap
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool PhisicsStep
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool PhisicsStarted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int PhisicsStepSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
