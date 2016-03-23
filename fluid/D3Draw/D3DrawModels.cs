using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fluid.D3Draw;
using SharpDX;
using System.Drawing;
using fluid.Forms;
using fluid.CoreDraw;
using fluid.D3Draw.ShaderLoaders;

namespace fluid.D3Draw
{
    class D3DrawModels : DrawerBase
    {

        private Model Box { get; set; }
        private ColorShader ColorShader { get; set; }
        private LightShader LightShader { get; set; }
        private Light Light { get; set; }

        private Triangle Triangle { get; set; }
        private Model Floor { get; set; }
        private Volume Volume { get; set; }

        private SizeF _Heatmap;
        override public SizeF Heatmap { get { return _Heatmap; } set { _Heatmap = value; Volume.Heatmap = value; } }

        private SizeF _Sensitivitymap;
        override public SizeF Sensitivitymap { get { return _Sensitivitymap; } set { _Sensitivitymap = value; Volume.Sensitivitymap = value; } }

        override public bool init(DX11 DX11, SizeF Heatmap, SizeF Sensitivitymap)
        {
            if (!initBase(DX11, Heatmap, Sensitivitymap)) return false;

            // Create the camera object
            _camera = new Camera();

            // Set the initial position of the camera.
            _camera.SetPosition(1, 1, 6);

            this._Heatmap = Heatmap;
            this._Sensitivitymap = Sensitivitymap;

            // Create the model object.
            Box = new Model();

            // Initialize the model object.
            if (!Box.Initialize(DX11.Device, "Cube.txt", "seafloor.dds"))
                Console.Out.WriteLine("Error on cube load!");

            Floor = new Model();
            if (!Floor.Initialize(DX11.Device, "Floor.txt", "seafloor.dds"))
                Console.Out.WriteLine("Error on floor load!");

            Triangle = new Triangle();
            Triangle.Initialize(DX11.Device);

            // Create the color shader object.
            ColorShader = new ColorShader();

            // Initialize the color shader object.
            ColorShader.Initialize(DX11.Device, DX11.Handle);


            LightShader = new LightShader();
            LightShader.Initialize(DX11.Device, DX11.Handle);

            Light = new Light();

            // Iniialize the light object.
            Light.SetDiffuseColor(1, 1, 1f, 1f);
            Light.SetDirection(10, -10, -10);

            // Create the model object.
            Volume = new Volume();

            // Initialize the model object.
            if (!Volume.Initialize(DX11, Heatmap, Sensitivitymap))
                Console.Out.WriteLine("Error on volume load!");

            return true;
        }

        float r = 0;
        override public void Frame()
        {
            _fps.Frame();
            _cpu.Frame();
            Rotate();
            Render(r);

        }
        void Rotate()
        {
            r += (float)Math.PI * 0.001f;
        }

        private bool Render(float rotation)
        {

            DX11.BeginScene(0.5f, 0.5f, 0.5f, 1.0f);

            _camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = _camera.ViewMatrix;
            var worldMatrix = DX11.WorldMatrix;
            var projectionMatrix = DX11.ProjectionMatrix;


            worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-0.5f, 0, -0.5f));

            //render the box
            Box.Render(DX11.DeviceContext);
            Volume.VolumeShader.RenderSphere(DX11.DeviceContext, Box.IndexCount, worldMatrix * Matrix.Scaling(0.2f) * viewMatrix * projectionMatrix);


            DX11.TurnOnInObjectRender();

            //TODO: render floor?
            Floor.Render(DX11.DeviceContext);
            /*if (!LightShader.Render(DX11.DeviceContext, Floor.IndexCount, DX11.WorldMatrix * Matrix.RotationZ(1.14f) * Matrix.RotationY(rotation) * Matrix.Scaling(0.5f, 1, 0.4f), viewMatrix, projectionMatrix, Floor.Texture.TextureResource, Light.Direction, Light.DiffuseColor))
                return false;*/
            //render the box volume hide effect
            Box.Render(DX11.DeviceContext);
            Volume.VolumeShader.RenderSphere(DX11.DeviceContext, Box.IndexCount, worldMatrix * Matrix.Scaling(0.2f) * viewMatrix * projectionMatrix);

            DX11.TurnOffInObjectRender();


            if (PhisicsStep || (PhisicsStarted && _fps.count % PhisicsStepSize == 0))
            {
                Volume.PhisicsStep();
                PhisicsStep = false;
            }


            //render the actual Volume
            Volume.Render(worldMatrix, viewMatrix, projectionMatrix);

            DX11.EndScene();
            return true;
        }


        override public void Dispose()
        {
            if (Floor != null) Floor.Dispose();
            if (Volume != null) Volume.Dispose();
        }

        override public void addVars(RenderTargetView renderView, SwapChain swapChain, DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler)
        {
            throw new NotImplementedException();
        }

        override public IMovableModel addFileLoader(HMDP.HMDPLoader loader)
        {
            throw new NotImplementedException();
        }
    }
}
