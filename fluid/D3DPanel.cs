﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX;
using fluid.D3DrawModelsSources;

namespace fluid
{
    class D3DPanel : Control
    {
        private Boolean renderingOn;
        private D3Drawer _drawer;
        public D3Drawer drawer { get { return _drawer; } set { if (!renderingOn)_drawer = value; } }
        private DX11 DX11 { get; set; }
        public Camera Camera { get { return drawer.Camera; } }

        private SizeF _Heatmap = new SizeF(0, 1);
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
            if (DX11 != null) return;
            DX11 = new DX11();
            DX11.Initialize(this.Handle, this.Height, this.Width);
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
            if (Disposing)
            {
                drawer.Dispose();
                DX11.Shutdown();

            }
            base.Dispose(disposing);
        }

        public void RefreshDrawer()
        {

        }



    }
}
