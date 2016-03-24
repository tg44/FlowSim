using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fluid.Forms;
using fluid.HMDP;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace fluid.CoreDraw
{
    abstract class DrawerBase : ID3Drawer
    {
        protected Camera _camera { get; set; }

        protected DX11 DX11 { get; set; }

        public HMDPTypeEnum HmdpRenderType { get; set; }

        public Camera Camera { get { return _camera; } set { _camera = value; } }
        public FPS FPS { get { return _fps; } }
        public CPU CPU { get { return _cpu; } }

        protected FPS _fps;
        protected CPU _cpu;

        public bool PhisicsStep { get; set; }
        public bool PhisicsStarted { get; set; }
        public int PhisicsStepSize { get; set; }
        public abstract SizeF Heatmap { get; set; }
        public abstract SizeF Sensitivitymap { get; set; }

        protected bool initBase(DX11 DX11, SizeF Heatmap, SizeF Sensitivitymap)
        {
            if (this.DX11 != null) return false;

            this.DX11 = DX11;
            _fps = new FPS();
            _fps.Initialize();

            _cpu = new CPU();
            _cpu.Initialize();

            return true;
        }

        public abstract void Frame();
        public abstract void addVars(RenderTargetView renderView, SwapChain swapChain, DeviceContext deviceContext, SharpDX.Direct3D11.Device device, int Width, int Height, IntPtr Handler);
        public abstract void Dispose();
        public abstract IMovableModel addFileLoader(HMDPLoader loader);
        public abstract bool init(DX11 DX11, SizeF Heatmap, SizeF Sensitivitymap);
    }
}
