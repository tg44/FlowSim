using fluid.D2Draw;
using fluid.Forms;
using fluid.HMDP;
using fluid.HMDP.Preset;
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
    public partial class SimulationForm2D : Form
    {
        public SimulationForm2D()
        {
            InitializeComponent();
        }

        private void sartRender(object sender, EventArgs e)
        {
            d3DPanel1.drawer = new D2DrawModels();

            button2.Enabled = true;
            button3.Enabled = true;
            listBox1.DisplayMember = "Name";


            d3DPanel1.Initialize();

            d3DPanel1.startRendering();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //load dialog

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Package or preset (.hmdp, .xml)|*.hmdp;*.xml|Hardware Model Description Package (.hmdp)|*.hmdp|PresetFile (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            var userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                if (openFileDialog1.FileName.EndsWith(".hmdp"))
                {
                    HMDPLoader loader = new HMDPLoader();
                    loader.FileName = openFileDialog1.FileName;

                    //cucc hozzáadása a rajztérhez
                    var ret = d3DPanel1.drawer.addFileLoader(loader);

                    //beloadolt cucc hozzáadása a jobboldali listához
                    listBox1.Items.Add(ret);
                    listBox1.EndUpdate();
                }
                if (openFileDialog1.FileName.EndsWith(".xml"))
                {
                    PresetLoader prLoader = new PresetLoader();
                    prLoader.Init(openFileDialog1.FileName);

                    foreach (PresetItem item in prLoader.Items)
                    {
                        var ret = d3DPanel1.drawer.addFileLoader(item.Loader);
                        ret.X = item.X;
                        ret.Y = item.Y;
                        ret.Z = item.Z;
                        ret.R = item.Rotation;
                        listBox1.Items.Add(ret);
                    }
                    listBox1.EndUpdate();
                }

            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();

            var userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                PresetLoader.buildXml(listBox1.Items, openFileDialog1.FileName);
            }
        }

        private void selectionChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
                modelMover1.Model = (MovableModel)listBox1.Items[listBox1.SelectedIndex];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ui
            button2.Enabled = false;
            //modelMover1.Enabled = false;
            button3.Enabled = false;

            //d3dwindow reinit
            D2DrawModels modeller = (D2DrawModels)d3DPanel1.drawer;
            D2DrawSimulation sim = new D2DrawSimulation();
            sim.addModelList(modeller.Models);

            d3DPanel1.stopRendering();
            d3DPanel1.drawer = sim;
            d3DPanel1.Initialize();
            d3DPanel1.PhisicsStarted = true;

            sim.Camera = modeller.Camera;

            selectionRangeSliderWrapper1.SelectedMax = 0.1f;
            selectionRangeSliderWrapper2.SelectedMax = 0.1f;
            HeatChanged(this, null);
            SensitivityChanged(this, null);

            d3DPanel1.startRendering();
        }

        private void HeatChanged(object sender, EventArgs e)
        {
            d3DPanel1.Heatmap = new SizeF(selectionRangeSliderWrapper1.SelectedMin, selectionRangeSliderWrapper1.SelectedMax);
        }

        private void SensitivityChanged(object sender, EventArgs e)
        {
            d3DPanel1.Sensitivitymap = new SizeF(selectionRangeSliderWrapper2.SelectedMin, selectionRangeSliderWrapper2.SelectedMax);
        }

    }
}
