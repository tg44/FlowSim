using System;
using System.Windows.Forms;

namespace fluid.Forms
{
    public partial class ModelMover : UserControl
    {
        public IMovableModel Model { get { return _model; } set { _model = value; resync(); } }

        private IMovableModel _model;
        public ModelMover()
        {
            InitializeComponent();

            nudx.DecimalPlaces = 2;
            nudx.Increment = 10;
            nudx.Maximum = 2000;
            nudx.Minimum = 0;

            nudy.DecimalPlaces = 2;
            nudy.Increment = 10;
            nudy.Maximum = 2000;
            nudy.Minimum = 0;

            nudz.DecimalPlaces = 2;
            nudz.Increment = 10;
            nudz.Maximum = 2000;
            nudz.Minimum = 0;

            nudr.DecimalPlaces = 2;
            nudr.Increment = 1;
            nudr.Maximum = 359;
            nudr.Minimum = 0;

            //nudx.ValueChanged += valChanged; //in propertyes
            nudy.ValueChanged += valChanged;
            nudz.ValueChanged += valChanged;
            nudr.ValueChanged += valChanged;
            checkBox1.CheckedChanged += valChanged;
        }



        private void resync()
        {
            if (Model == null) return;

            nudx.ValueChanged -= valChanged;
            nudy.ValueChanged -= valChanged;
            nudz.ValueChanged -= valChanged;
            nudr.ValueChanged -= valChanged;
            checkBox1.CheckedChanged -= valChanged;

            nudx.Value = (decimal)Model.X;
            nudy.Value = (decimal)Model.Y;
            if (Model.is2D())
            {
                nudz.Value = (decimal)0;
                nudz.Enabled = false;
            }
            else
            {
                nudz.Value = (decimal)Model.Z;
                nudz.Enabled = true;
            }
            nudr.Value = (decimal)Model.R;
            checkBox1.Checked = Model.Active;

            nudx.ValueChanged += valChanged;
            nudy.ValueChanged += valChanged;
            nudz.ValueChanged += valChanged;
            nudr.ValueChanged += valChanged;
            checkBox1.CheckedChanged += valChanged;
        }

        private void valChanged(object sender, EventArgs e)
        {
            if (Model == null) return;


            Model.X = (float)nudx.Value;
            Model.Y = (float)nudy.Value;
            if (Model.is2D())
            {
                Model.Z = (float)nudz.Value;
            }
            Model.R = (float)nudr.Value;
            Model.Active = checkBox1.Checked;
        }
    }
}
