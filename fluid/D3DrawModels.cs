using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fluid.D3DrawModelsSources;
using SharpDX;

namespace fluid
{
    class D3DrawModels : D3Drawer
    {

        public Camera _camera { get; set; }
        private Model Box { get; set; }
        private ColorShader ColorShader { get; set; }
        private LightShader LightShader { get; set; }
        private Light Light { get; set; }

        private Triangle Triangle { get; set; }
        private Model Floor { get; set; }
        private Volume Volume { get; set; }

        private DX11 DX11 { get; set; }

        Camera D3Drawer.Camera { get { return _camera; } set { } }
        FPS D3Drawer.FPS { get { return _fps; } }
        CPU D3Drawer.CPU { get { return _cpu; } }
        private FPS _fps;
        private CPU _cpu;

        bool D3Drawer.init(DX11 DX11)
        {
            this.DX11 = DX11;
            _fps = new FPS();
            _fps.Initialize();

            _cpu = new CPU();
            _cpu.Initialize();
            // Create the camera object
            _camera = new Camera();

            // Set the initial position of the camera.
            _camera.SetPosition(1, 1, 6);

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
            if (!Volume.Initialize(DX11))
                Console.Out.WriteLine("Error on volume load!");

            return true;
        }

        float r = 0;
        void D3Drawer.Frame()
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

            // Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(rotation, out worldMatrix);
            worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(0, 1, 0));

            /*Triangle.Render(DX11.DeviceContext);
            /*if (!VolumeShader.Render(DX11.DeviceContext, Triangle.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
                return false;*/
            /*if (!ColorShader.Render(DX11.DeviceContext, Triangle.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
                return false;

            Box.Render(DX11.DeviceContext);
            if (!LightShader.Render(DX11.DeviceContext, Box.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Box.Texture.TextureResource, Light.Direction, Light.DiffuseColor))
                return false;
            */
            Floor.Render(DX11.DeviceContext);
            if (!LightShader.Render(DX11.DeviceContext, Floor.IndexCount, DX11.WorldMatrix, viewMatrix, projectionMatrix, Floor.Texture.TextureResource, Light.Direction, Light.DiffuseColor))
                return false;
            /*
            worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(3, 0, 3));
            //render the volume
            VolumeBox.Render(DX11.DeviceContext);
            if (!ColorShader.Render(DX11.DeviceContext, VolumeBox.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
                return false;

            worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-6, 0, -6));
            */

            Volume.Render(worldMatrix, viewMatrix, projectionMatrix);

            DX11.EndScene();
            return true;
        }


        void D3Drawer.Dispose() { }

        void D3Drawer.addVars(RenderTargetView renderView, SwapChain swapChain, DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler)
        { }
    }
}
