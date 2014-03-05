using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Drawing;
using System.Windows.Forms;

namespace fluid
{
    class SharpDXRender
    {
        private SharpDX.Direct3D11.Device _device;
        private DeviceContext _deviceContext;
        private SwapChain _swapChain;

        

        public SharpDXRender(Form form)
        {
            SwapChainDescription swapChainDescription = new SwapChainDescription();
            swapChainDescription.BufferCount = 1;
            swapChainDescription.IsWindowed = true;
            swapChainDescription.ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            swapChainDescription.OutputHandle = form.Handle;
            swapChainDescription.SampleDescription = new SampleDescription(1, 0);
            swapChainDescription.Usage = Usage.RenderTargetOutput;

            //Creating Device
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out _device, out _swapChain);


            Texture2D backbuffer;
            RenderTargetView backbufferview;
            backbuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            backbufferview = new RenderTargetView(_device, backbuffer);

            _deviceContext = _device.ImmediateContext;
            _deviceContext.OutputMerger.SetTargets(backbufferview);


            Viewport vp = new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0f, 1f);
            _deviceContext.Rasterizer.SetViewport(vp);

            _deviceContext.ClearRenderTargetView(backbufferview, new Color4(0.3921f, 0.5843f, 0.9294f, 1f));
            _swapChain.Present(0, PresentFlags.None);

            _device.Dispose();
            _swapChain.Dispose();
            _deviceContext.Dispose();
            backbufferview.Dispose();
            backbuffer.Dispose();

        }
    }
}
