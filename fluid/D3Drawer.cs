using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fluid.D3DrawModelsSources;

namespace fluid
{
    interface D3Drawer
    {
        void Frame();
        void addVars(RenderTargetView renderView,
        SwapChain swapChain,
        DeviceContext deviceContext,
        SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler);
        bool init(DX11 DX11);
        void Dispose();
        FPS FPS { get; }
        CPU CPU { get; }
        Camera Camera { get; set; }
    }
}
