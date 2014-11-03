using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fluid
{
    public partial class SelectionRangeSliderWrapper : UserControl
    {
        public SelectionRangeSliderWrapper()
        {
            InitializeComponent();
            label2.Text = selectionRangeSlider1.Min.ToString();
            label4.Text = selectionRangeSlider1.Max.ToString();
            label3.Text = "-";
            textBox1.Text = selectionRangeSlider1.SelectedMin.ToString();
            textBox2.Text = selectionRangeSlider1.SelectedMax.ToString();
            label1.Text = Title;
            panel1.Left = (int)(this.Width / 2.0 - 160 / 2.0);
        }

        /// <summary>
        /// Title of the slider.
        /// </summary>
        [Description("Title of the slider.")]
        public string Title
        {
            get { return title; }
            set { title = value; label1.Text = value; Invalidate(); }
        }
        string title = "Title";


        /// <summary>
        /// Minimum value of the slider.
        /// </summary>
        [Description("Minimum value of the slider.")]
        public float Min
        {
            get { return selectionRangeSlider1.Min; }
            set { selectionRangeSlider1.Min = value; Invalidate(); }
        }
        /// <summary>
        /// Maximum value of the slider.
        /// </summary>
        [Description("Maximum value of the slider.")]
        public float Max
        {
            get { return selectionRangeSlider1.Max; }
            set { selectionRangeSlider1.Max = value; Invalidate(); }
        }
        /// <summary>
        /// Minimum value of the selection range.
        /// </summary>
        [Description("Minimum value of the selection range.")]
        public float SelectedMin
        {
            get { return selectionRangeSlider1.SelectedMin; }
            set
            {
                selectionRangeSlider1.SelectedMin = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Maximum value of the selection range.
        /// </summary>
        [Description("Maximum value of the selection range.")]
        public float SelectedMax
        {
            get { return selectionRangeSlider1.SelectedMax; }
            set
            {
                selectionRangeSlider1.SelectedMax = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fired when SelectedMin or SelectedMax changes.
        /// </summary>
        [Description("Fired when SelectedMin or SelectedMax changes.")]
        public event EventHandler SelectionChanged;

        private void SelectionRangeSliderChanged(object sender, EventArgs e)
        {
            label2.Text = selectionRangeSlider1.Min.ToString();
            label4.Text = selectionRangeSlider1.Max.ToString();
            label3.Text = "-";
            textBox1.Text = selectionRangeSlider1.SelectedMin.ToString();
            textBox2.Text = selectionRangeSlider1.SelectedMax.ToString();
            if (SelectionChanged != null)
                SelectionChanged(this, null);
        }

        private void SelfResize(object sender, EventArgs e)
        {
            panel1.Left = (int)(this.Width / 2.0 - 160 / 2.0);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float t = (float)Convert.ToDouble(textBox1.Text);
                SelectedMin = t;
            }
            catch (Exception)
            {

            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float t = (float)Convert.ToDouble(textBox2.Text);
                SelectedMax = t;
            }
            catch (Exception)
            {

            }
        }


    }
}
