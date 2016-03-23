using fluid.D2Draw.ShaderLoaders;
using fluid.D3DrawModelsSources;
using fluid.D3DrawModelsSources.DrawTools;
using fluid.D3DrawModelsSources.ShaderLoaders;
using fluid.Forms;
using fluid.HMDP;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace fluid.D2Draw
{
    class D2DrawSimulation : D3Drawer
    {
        private Camera _camera { get; set; }

        public Camera Camera { get { return _camera; } set { _camera = value; } }

        private DX11 DX11 { get; set; }

        private Model Wall { get; set; }

        private TextureShader TextureShader { get; set; }

        private Physics2DShader Physics2DShader { get; set; }

        private List<Drawable2DHMDP> models = new List<Drawable2DHMDP>();

        private Dictionary<string, RenderTargetView> RTVmap = new Dictionary<string, RenderTargetView>();

        private DepthStencilView tmpDSV;

        //private UnorderedAccessView mergedTextUav;

        private bool physics = false;

        #region texturereferences and stuff
        Texture2D tmpDepthText;
        Texture2D heatText;
        Texture2D solidText;
        Texture2D throughText;
        Texture2D mergedText;
        ShaderResourceView outsrv;
        #endregion

        private Dictionary<string, ShaderResourceView> SRVmap = new Dictionary<string, ShaderResourceView>();


        #region interface not implemented



        public CPU CPU
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public FPS FPS
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SizeF Heatmap
        {
            get
            {
                return new SizeF(Physics2DShader.Heat.X, Physics2DShader.Heat.Y);
            }

            set
            {
                if (Physics2DShader != null)
                {
                    Physics2DShader.Heat.Y = value.Width;
                    Physics2DShader.Heat.X = value.Height;
                }
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

        public SizeF Sensitivitymap
        {
            get
            {
                return new SizeF(Physics2DShader.Sensitivity.X, Physics2DShader.Sensitivity.Y);
            }

            set
            {
                if (Physics2DShader != null)
                {
                    Physics2DShader.Sensitivity.Y = value.Width;
                    Physics2DShader.Sensitivity.X = value.Height;
                }
            }
        }

        public MovableModel addFileLoader(HMDPLoader loader)
        {
            throw new NotImplementedException();
        }

        public void addVars(RenderTargetView renderView, SwapChain swapChain, DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler)
        {
            throw new NotImplementedException();
        }
        #endregion

        internal void addModelList(IList<Drawable2DHMDP> models)
        {
            foreach (var item in models)
            {
                this.models.Add(item);
            }
        }

        public void Dispose()
        {
            foreach (var item in RTVmap)
            {
                item.Value.Dispose();
            }
            tmpDSV.Dispose();

            tmpDepthText.Dispose();
            heatText.Dispose();
            solidText.Dispose();
            throughText.Dispose();

            foreach (var item in models)
            {
                item.Dispose();
            }

        }
        private ShaderResourceView PhysicsStep()
        {
            //test
            _camera.Render2D();
            var viewMatrix = _camera.ViewMatrix;
            var worldMatrix = DX11.WorldMatrix;
            var projectionMatrix = DX11.ProjectionMatrix;
            RenderToTextures(worldMatrix, viewMatrix, projectionMatrix);
            //testc off
            return Physics2DShader.RenderPhysics(DX11.DeviceContext);
            //return outsrv;
        }
        public void Frame()
        {
            if (physics)
            {
                outsrv = PhysicsStep();
                DX11.ResetDeviceContext();
            }
            renderScene();

        }


        public void renderScene()
        {
            DX11.BeginScene(0.4f, 0.6f, 0.6f, 1.0f);

            _camera.Render2D();

            var viewMatrix = _camera.ViewMatrix;
            var worldMatrix = DX11.WorldMatrix;
            var projectionMatrix = DX11.ProjectionMatrix;


            //Wall.Render(DX11.DeviceContext);
            //TextureShader.Render(DX11.DeviceContext, Wall.IndexCount, worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-1f, 0, -1f)), viewMatrix, projectionMatrix, Wall.Texture.TextureResource);



            foreach (var model in models)
            {
                model.switchTexture("icon");
                model.Render(DX11.DeviceContext, DX11.WorldMatrix, viewMatrix, projectionMatrix, TextureShader);

            }

            DX11.TurnOnAlphaBlending();

            Wall.Render(DX11.DeviceContext);
            worldMatrix = Matrix.Multiply(worldMatrix, Matrix.Translation(-1f, 0.001f, -1f));
            Physics2DShader.RenderTexture(DX11.DeviceContext, Wall.IndexCount, worldMatrix, viewMatrix, projectionMatrix);

            DX11.TurnOffAlphaBlending();

            DX11.EndScene();
        }

        public bool init(DX11 DX11, SizeF Heatmap, SizeF Sensitivitymap)
        {
            if (this.DX11 != null) return false;

            this.DX11 = DX11;

            _camera = new Camera();

            // Set the initial position of the camera.
            _camera.SetPosition(0, 0, 1.3f);
            //TODO: a kamera itt még resetelve van
            //TODO: lefosok egy akkora síkot h kitöltse a kamera képét

            Wall = new Model();
            if (!Wall.Initialize(DX11.Device, "Floor.txt", "frame.dds"))
                Console.Out.WriteLine("Error on floor load!");

            TextureShader = new TextureShader();
            TextureShader.Initialize(DX11.Device, DX11.Handle);

            #region texture inits
            RenderTargetView heatRTV;
            RenderTargetView solidRTV;
            RenderTargetView throughRTV;

            Texture2DDescription desc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32G32B32A32_Float,
                Width = DX11.Width,
                Height = DX11.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            heatText = new Texture2D(DX11.Device, desc);
            solidText = new Texture2D(DX11.Device, desc);
            throughText = new Texture2D(DX11.Device, desc);
            mergedText = new Texture2D(DX11.Device, desc);
            heatRTV = new RenderTargetView(DX11.Device, heatText);
            solidRTV = new RenderTargetView(DX11.Device, solidText);
            throughRTV = new RenderTargetView(DX11.Device, throughText);

            RTVmap["heat"] = heatRTV;
            RTVmap["solid"] = solidRTV;
            RTVmap["through"] = throughRTV;

            tmpDepthText = new Texture2D(DX11.Device, DX11.DepthStencilBufferInObj.Description);
            tmpDSV = new DepthStencilView(DX11.Device, tmpDepthText, DX11.DepthStencilViewInObj.Description);
            //mergedTextUav = new UnorderedAccessView(DX11.DeviceContext.Device, mergedText);

            SRVmap["heat"] = new ShaderResourceView(DX11.DeviceContext.Device, heatText);
            SRVmap["solid"] = new ShaderResourceView(DX11.DeviceContext.Device, solidText);
            SRVmap["through"] = new ShaderResourceView(DX11.DeviceContext.Device, throughText);
            SRVmap["merged"] = new ShaderResourceView(DX11.DeviceContext.Device, mergedText);
            outsrv = SRVmap["merged"];
            #endregion

            //kell a 3 textúra
            _camera.Render();
            var viewMatrix = _camera.ViewMatrix;
            var worldMatrix = DX11.WorldMatrix;
            var projectionMatrix = DX11.ProjectionMatrix;

            RenderToTextures(worldMatrix, viewMatrix, projectionMatrix);



            Physics2DShader = new Physics2DShader();
            Physics2DShader.Initialize(DX11, solidText, heatText, throughText);



            return true;
        }

        private void RenderToTextures(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {

            foreach (var item in RTVmap)
            {
                RenderTargetView renderToTextureRTV = item.Value;

                //clear them
                DX11.DeviceContext.ClearDepthStencilView(tmpDSV, DepthStencilClearFlags.Depth, 1, 0);
                DX11.DeviceContext.ClearRenderTargetView(renderToTextureRTV, new Color4(0.0f));

                // 3) Bind the render target to the output in your rendering code:
                DX11.DeviceContext.OutputMerger.SetRenderTargets(tmpDSV, renderToTextureRTV);

                // 4) Don't forget to setup the viewport for this particular render target
                DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);

                foreach (var model in models)
                {
                    model.switchTexture(item.Key);
                    model.Render(DX11.DeviceContext, worldMatrix, viewMatrix, projectionMatrix, TextureShader, true);
                    //DX11.DeviceContext.ClearDepthStencilView(tmpDSV, DepthStencilClearFlags.Depth, 1, 0);
                }
            }

            DX11.DeviceContext.OutputMerger.SetTargets(DX11.DepthStencilView, DX11.RenderTargetView);
            DX11.DeviceContext.Rasterizer.SetViewport(0, 0, DX11.Width, DX11.Height, 0, 1);
            DX11.TurnOffInObjectRender();

        }

        public bool PhisicsStarted
        {
            get
            {
                return physics;
            }

            set
            {
                physics = value;
            }
        }
    }
}
