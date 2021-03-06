﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fluid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sartRender(object sender, MouseEventArgs e)
        {
            //d3DPanel1.drawer=new D3DrawBG();
            d3DPanel1.drawer = new D3DrawModels();
            d3DPanel1.Initialize();
            refreshLabels();
            labelTimer.Enabled = true;
            d3DPanel1.startRendering();
            
        }
        private void refreshLabels()
        {
            label1.Text = "Video Card Desc:    " + d3DPanel1.GetCardDesc();
            label2.Text = "Video Card Memory:  " + d3DPanel1.GetCardMem();
            label3.Text = "Frames per sec:     " + d3DPanel1.FPS().ToString();
            label4.Text = "CPU usege:          " + d3DPanel1.CPU().ToString();
        }

        private void controlUp(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(0, -0.3f, 0);
        }

        private void controlDown(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(0, +0.3f, 0);
        }

        private void controlRight(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(0.3f, 0, 0);
        }

        private void controlLeft(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(-0.3f, 0, 0);
        }

        private void controlZoomIn(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(0, 0, 0.3f);
        }

        private void controlZoomOut(object sender, EventArgs e)
        {
            Vector3 lastPos = d3DPanel1.Camera.GetPosition();
            d3DPanel1.Camera.ModPositionInSphere(0, 0, -0.3f);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshLabels();
        }
    }
}
