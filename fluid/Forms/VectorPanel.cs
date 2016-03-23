using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;

namespace fluid.Forms
{
    public partial class VectorPanel : UserControl
    {
        Vector3 vector = new Vector3(0, 0, 0);
        public VectorPanel()
        {
            InitializeComponent();
        }
        [Description("Title of the panel.")]
        public string Title
        {
            get { return title; }
            set { title = value; label1.Text = value; Invalidate(); }
        }
        string title = "Title";

        [Description("vector")]
        public Vector3 Vector
        {
            get { return vector; }
            set
            {
                vector = value;
                Invalidate();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            vector.X = trackBar1.Value / 10.0f;
            SelectionChanged(sender, e);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            vector.Y = trackBar2.Value / 10.0f;
            SelectionChanged(sender, e);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            vector.Z = trackBar3.Value / 10.0f;
            SelectionChanged(sender, e);
        }

        [Description("Fired when SelectedMin or SelectedMax changes.")]
        public event EventHandler SelectionChanged;
    }
}
