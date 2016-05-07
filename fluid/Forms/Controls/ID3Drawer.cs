using fluid.CoreDraw;
using fluid.Forms;
using fluid.HMDP;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;

namespace fluid
{
    interface ID3Drawer
    {
        void Frame();
        void addVars(RenderTargetView renderView,
        SwapChain swapChain,
        DeviceContext deviceContext,
        SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler);
        bool init(DX11 DX11, SizeF Heatmap, SizeF Sensitivitymap);
        void Dispose();
        FPS FPS { get; }
        CPU CPU { get; }
        Camera Camera { get; set; }

        SizeF Heatmap { get; set; }

        SizeF Sensitivitymap { get; set; }

        bool PhisicsStep { get; set; }
        bool PhisicsStarted { get; set; }
        int PhisicsStepSize { get; set; }

        HMDPTypeEnum HmdpRenderType { get; set; }
        RenderTextureEnum TextureRenderType { get; set; }

        IMovableModel addFileLoader(HMDP.HMDPLoader loader);

    }
}
