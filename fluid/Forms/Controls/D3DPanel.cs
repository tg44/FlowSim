using fluid.CoreDraw;
using SharpDX.Windows;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace fluid
{
    class D3DPanel : Control
    {
        private Boolean renderingOn;
        private ID3Drawer _drawer;
        public ID3Drawer drawer { get { return _drawer; } set { if (!renderingOn) _drawer = value; } }
        private DX11 DX11 { get; set; }
        public Camera Camera { get { return drawer.Camera; } }

        private SizeF _Heatmap = new SizeF(0, 1);

        public bool PhisicStep { get { return true; } set { if (_drawer != null) _drawer.PhisicsStep = true; } }

        public bool PhisicsStarted { get { return true; } set { if (_drawer != null) _drawer.PhisicsStarted = value; } }
        public int PhisicsStepSize { get { return 0; } set { if (_drawer != null) _drawer.PhisicsStepSize = value; } }

        public SizeF Heatmap
        {
            get { return _Heatmap; }
            //TODO not safe yet it will need a refactor
            set
            {
                //h>w !!!!!!
                _Heatmap.Height = value.Height > 1.0f ? 1.0f : value.Height;
                _Heatmap.Width = value.Width < -1.0f ? -1.0f : value.Width;
                if (_drawer != null && _drawer.Heatmap != null)
                    _drawer.Heatmap = _Heatmap;
            }
        }

        private SizeF _Sensitivitymap = new SizeF(0, 1);
        public SizeF Sensitivitymap
        {
            get { return _Sensitivitymap; }
            //TODO not safe yet it will need a refactor
            set
            {
                //h>w !!!!!!
                _Sensitivitymap.Height = value.Height > 1.0f ? 1.0f : value.Height;
                _Sensitivitymap.Width = value.Width < -1.0f ? -1.0f : value.Width;
                if (_drawer != null && _drawer.Sensitivitymap != null)
                    _drawer.Sensitivitymap = _Sensitivitymap;
            }
        }

        public D3DPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UserPaint, true);


            renderingOn = false;
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        public void Initialize()
        {
            if (renderingOn) return;
            if (DX11 == null)
            {
                DX11 = new DX11();
                DX11.Initialize(this.Handle, this.Height, this.Width);
            }
            //drawer.addVars(renderView, swapChain, deviceContext, device, Width, Height, this.Handle);
            drawer.init(DX11, _Heatmap, _Sensitivitymap);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!renderingOn)
            {
                pe.Graphics.Clear(System.Drawing.Color.CornflowerBlue);
                pe.Graphics.DrawString(Name, Font, Brushes.White, PointF.Empty);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
        public void startRendering()
        {
            if (renderingOn) return;
            renderingOn = true;
            RenderLoop.Run(this, () =>
            {
                if (!renderingOn)
                {
                    return;
                }
                drawer.Frame();
            });

        }

        public void stopRendering()
        {
            if (!renderingOn) return;
            renderingOn = false;
        }
        public string GetCardDesc()
        {
            return DX11.VideoCardDescription;
        }
        public string GetCardMem()
        {
            return DX11.VideoCardMemory.ToString();
        }
        public float FPS()
        {
            return drawer.FPS.Value;
        }
        public float CPU()
        {
            return drawer.CPU.Value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (drawer != null)
                    drawer.Dispose();
                if (DX11 != null)
                    DX11.Dispose();

            }
            base.Dispose(disposing);
        }

        public void RefreshDrawer()
        {

        }



    }
}
