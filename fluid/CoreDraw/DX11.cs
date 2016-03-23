using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.Direct3D;

namespace fluid.CoreDraw
{
    class DX11
    {
        private bool VerticalSyncEnabled = true;
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; private set; }
        public SwapChain SwapChain { get; private set; }
        public Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        public RenderTargetView RenderTargetView { get; private set; }
        public RenderTargetView RenderTargetViewInObj { get; private set; }
        public Texture2D DepthStencilBuffer { get; set; }
        public Texture2D DepthStencilBufferInObj { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public DepthStencilView DepthStencilView { get; private set; }
        public DepthStencilView DepthStencilViewInObj { get; private set; }
        private RasterizerState RasterState { get; set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix OrthoMatrix { get; private set; }

        public BlendState AlphaEnableBlendingState { get; private set; }
        public BlendState AlphaDisableBlendingState { get; private set; }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public IntPtr Handle { get; private set; }
        public float ScreenNear = 0.1f;
        public float ScreenDepth = 1000.0f;



        public DX11()
        {
        }

        public bool Initialize(IntPtr windowHandle, int Height, int Width)
        {
            try
            {
                this.Height = Height;
                this.Width = Width;

                Handle = windowHandle;
                // Create a DirectX graphics interface factory.
                var factory = new Factory();
                // Use the factory to create an adapter for the primary graphics interface (video card).
                var adapter = factory.GetAdapter(0);
                // Get the primary adapter output (monitor).
                var monitor = adapter.Outputs[0];
                // Get modes that fit the DXGI_FORMAT_R8G8B8A8_UNORM display format for the adapter output (monitor).
                var modes = monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);
                // Now go through all the display modes and find the one that matches the screen width and height.
                // When a match is found store the the refresh rate for that monitor, if vertical sync is enabled. 
                // Otherwise we use maximum refresh rate.
                var rational = new Rational(0, 1);
                if (VerticalSyncEnabled)
                {
                    foreach (var mode in modes)
                    {
                        if (mode.Width == Width && mode.Height == Height)
                        {
                            rational = new Rational(mode.RefreshRate.Numerator, mode.RefreshRate.Denominator);
                            break;
                        }
                    }
                }

                // Get the adapter (video card) description.
                var adapterDescription = adapter.Description;

                // Store the dedicated video card memory in megabytes.
                VideoCardMemory = adapterDescription.DedicatedVideoMemory >> 10 >> 10;

                // Convert the name of the video card to a character array and store it.
                VideoCardDescription = adapterDescription.Description;

                // Release the adapter output.
                monitor.Dispose();

                // Release the adapter.
                adapter.Dispose();

                // Release the factory.
                factory.Dispose();

                // Initialize the swap chain description.
                var swapChainDesc = new SwapChainDescription()
                {
                    // Set to a single back buffer.
                    BufferCount = 1,
                    // Set the width and height of the back buffer.
                    ModeDescription = new ModeDescription(Width, Height, rational, Format.R8G8B8A8_UNorm),
                    // Set the usage of the back buffer.
                    Usage = Usage.RenderTargetOutput,
                    // Set the handle for the window to render to.
                    OutputHandle = windowHandle,
                    // Turn multisampling off.
                    SampleDescription = new SampleDescription(1, 0),
                    // Set to full screen or windowed mode.
                    IsWindowed = true,
                    // Don't set the advanced flags.
                    Flags = SwapChainFlags.None,
                    // Discard the back buffer content after presenting.
                    SwapEffect = SwapEffect.Discard
                };

                // Create the swap chain, Direct3D device, and Direct3D device context.
                Device device;
                SwapChain swapChain;
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out device, out swapChain);

                Device = device;
                SwapChain = swapChain;
                DeviceContext = device.ImmediateContext;

                // Get the pointer to the back buffer.
                var backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);

                // Create the render target view with the back buffer pointer.
                RenderTargetView = new RenderTargetView(device, backBuffer);
                Texture2DDescription rtvdesc = new Texture2DDescription
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.R8G8B8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default
                };
                var rtvtex = new Texture2D(device, rtvdesc);
                RenderTargetViewInObj = new RenderTargetView(device, rtvtex);

                // Release pointer to the back buffer as we no longer need it.
                backBuffer.Dispose();

                // Initialize and set up the description of the depth buffer.
                var depthBufferDesc = new Texture2DDescription()
                {
                    Width = Width,
                    Height = Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R32_Typeless,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Create the texture for the depth buffer using the filled out description.
                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);
                DepthStencilBufferInObj = new Texture2D(device, depthBufferDesc);

                ResetDeviceContext();


                // Setup and create the projection matrix.
                ProjectionMatrix = Matrix.PerspectiveFovLH((float)(Math.PI / 4), ((float)Width / Height), ScreenNear, ScreenDepth);

                // Initialize the world matrix to the identity matrix.
                WorldMatrix = Matrix.Identity;

                // Create an orthographic projection matrix for 2D rendering.
                OrthoMatrix = Matrix.OrthoLH(Width, Height, ScreenNear, ScreenDepth);

                var blendStateDesc = new BlendStateDescription();
                blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
                blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
                blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                // Create the blend state using the description.
                AlphaEnableBlendingState = new BlendState(device, blendStateDesc);
                //AlphaEnableBlendingState.SetPrivateData("aeblendstate");

                // Modify the description to create an disabled blend state description.
                blendStateDesc.RenderTarget[0].IsBlendEnabled = false;
                // Create the blend state using the description.
                AlphaDisableBlendingState = new BlendState(device, blendStateDesc);

                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            // Before shutting down set to windowed mode or when you release the swap chain it will throw an exception.
            if (SwapChain != null)
            {
                SwapChain.SetFullscreenState(false, null);
            }

            if (RasterState != null)
            {
                RasterState.Dispose();
                RasterState = null;
            }

            if (AlphaEnableBlendingState != null)
            {
                AlphaEnableBlendingState.Dispose();
                AlphaEnableBlendingState = null;
            }

            if (AlphaDisableBlendingState != null)
            {
                AlphaDisableBlendingState.Dispose();
                AlphaDisableBlendingState = null;
            }

            if (DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }

            if (DepthStencilViewInObj != null)
            {
                DepthStencilViewInObj.Dispose();
                DepthStencilViewInObj = null;
            }

            if (DepthStencilState != null)
            {
                DepthStencilState.Dispose();
                DepthStencilState = null;
            }

            if (DepthStencilBuffer != null)
            {
                DepthStencilBuffer.Dispose();
                DepthStencilBuffer = null;
            }

            if (DepthStencilBufferInObj != null)
            {
                DepthStencilBufferInObj.Dispose();
                DepthStencilBufferInObj = null;
            }

            if (RenderTargetView != null)
            {
                RenderTargetView.Dispose();
                RenderTargetView = null;
            }
            if (RenderTargetViewInObj != null)
            {
                RenderTargetViewInObj.Dispose();
                RenderTargetViewInObj = null;
            }

            if (SwapChain != null)
            {
                SwapChain.Dispose();
                SwapChain = null;
            }

            DeviceContext.Dispose();

            if (Device != null)
            {
                var deviceDebug = new DeviceDebug(Device);
                deviceDebug.ReportLiveDeviceObjects(ReportingLevel.Detail);
                Device.Dispose();
                Device = null;
            }



        }

        public void BeginScene(float red, float green, float blue, float alpha)
        {
            BeginScene(new Color4(red, green, blue, alpha));
        }
        public void BeginScene(Color4 color)
        {
            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            DeviceContext.ClearDepthStencilView(DepthStencilViewInObj, DepthStencilClearFlags.Depth, 1, 0);
            // Clear the back buffer.
            DeviceContext.ClearRenderTargetView(RenderTargetView, color);
            DeviceContext.ClearRenderTargetView(RenderTargetViewInObj, new Color4(0.0f));
        }

        public void EndScene()
        {
            // Present the back buffer to the screen since rendering is complete.
            if (VerticalSyncEnabled)
            {
                // Lock to screen refresh rate.
                SwapChain.Present(1, PresentFlags.None);
            }
            else
            {
                // Present as fast as possible.
                SwapChain.Present(0, PresentFlags.None);
            }
        }

        public void TurnOnAlphaBlending()
        {
            // Setup the blend factor.
            var blendFactor = new Color4(0, 0, 0, 0);

            // Turn on the alpha blending.
            DeviceContext.OutputMerger.SetBlendState(AlphaEnableBlendingState, blendFactor, -1);
        }

        public void TurnOffAlphaBlending()
        {
            // Setup the blend factor.
            var blendFactor = new Color4(0, 0, 0, 0);

            // Turn on the alpha blending.
            DeviceContext.OutputMerger.SetBlendState(AlphaDisableBlendingState, blendFactor, -1);
        }
        public void TurnOnInObjectRender()
        {
            DeviceContext.OutputMerger.SetTargets(DepthStencilViewInObj, RenderTargetViewInObj);
        }
        public void TurnOffInObjectRender()
        {
            DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);
        }

        public void ResetDeviceContext()
        {
            // Initialize and set up the description of the stencil state.
            var depthStencilDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                // Stencil operation if pixel front-facing.
                FrontFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                // Stencil operation if pixel is back-facing.
                BackFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            };

            if (DepthStencilState != null)
            {
                DepthStencilState.Dispose();
                DepthStencilState = null;
            }


            // Create the depth stencil state.
            DepthStencilState = new DepthStencilState(Device, depthStencilDesc);

            // Set the depth stencil state.
            DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);

            // Initialize and set up the depth stencil view.
            var depthStencilViewDesc = new DepthStencilViewDescription()
            {
                Format = Format.D32_Float,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            };

            if (DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }
            if (DepthStencilViewInObj != null)
            {
                DepthStencilViewInObj.Dispose();
                DepthStencilViewInObj = null;
            }

            // Create the depth stencil view.
            DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer, depthStencilViewDesc);
            DepthStencilViewInObj = new DepthStencilView(Device, DepthStencilBufferInObj, depthStencilViewDesc);

            // Bind the render target view and depth stencil buffer to the output render pipeline.
            DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

            // Setup the raster description which will determine how and what polygon will be drawn.
            var rasterDesc = new RasterizerStateDescription()
            {
                IsAntialiasedLineEnabled = false,
                CullMode = CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = .0f,
                IsDepthClipEnabled = true,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = .0f
            };


            if (RasterState != null)
            {
                RasterState.Dispose();
                RasterState = null;
            }

            // Create the rasterizer state from the description we just filled out.
            RasterState = new RasterizerState(Device, rasterDesc);

            // Now set the rasterizer state.
            DeviceContext.Rasterizer.State = RasterState;

            // Setup and create the viewport for rendering.
            DeviceContext.Rasterizer.SetViewport(0, 0, Width, Height, 0, 1);
        }

    }
}