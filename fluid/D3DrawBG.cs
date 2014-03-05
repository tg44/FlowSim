using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fluid.D3DrawModelsSources;

namespace fluid
{
    class D3DrawBG : D3Drawer
    {
        private int count = 0;

        private RenderTargetView renderView;
        // private RenderTargetView backbufferView;
        private SwapChain swapChain;
        private DeviceContext deviceContext;
        private SharpDX.Direct3D11.Device device;
        private int Width;
        private int Height;


        bool D3Drawer.init(DX11 DX11)
        {
            return true;
        }
        void D3Drawer.Frame()
        {
            count++;
            deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height, 0.0f, 1.0f));
            deviceContext.OutputMerger.SetTargets(renderView);
            float c = count / 10000f;
            deviceContext.ClearRenderTargetView(renderView, new SharpDX.Color4(1.0f / c, 1.0f / c, 1.0f / c, 0));

            swapChain.Present(0, PresentFlags.None);

        }

        void D3Drawer.addVars(RenderTargetView renderView, SwapChain swapChain, DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler)
        {
            this.renderView = renderView;
            this.swapChain = swapChain;
            this.deviceContext = deviceContext;
            this.device = device;
            this.Height = Height;
            this.Width = Width;

        }
        void D3Drawer.Dispose() { }
    }
}
