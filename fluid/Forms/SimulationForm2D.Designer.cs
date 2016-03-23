namespace fluid
{
    partial class SimulationForm2D
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.selectionRangeSliderWrapper2 = new fluid.SelectionRangeSliderWrapper();
            this.selectionRangeSliderWrapper1 = new fluid.SelectionRangeSliderWrapper();
            this.modelMover1 = new fluid.Forms.ModelMover();
            this.d3DPanel1 = new fluid.D3DPanel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(419, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.sartRender);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(419, 43);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Load model";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(419, 73);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(75, 108);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.selectionChanged);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(418, 213);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Simulation";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(419, 187);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "SavePreset";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // selectionRangeSliderWrapper2
            // 
            this.selectionRangeSliderWrapper2.Location = new System.Drawing.Point(588, 261);
            this.selectionRangeSliderWrapper2.Max = 1F;
            this.selectionRangeSliderWrapper2.Min = -1F;
            this.selectionRangeSliderWrapper2.Name = "selectionRangeSliderWrapper2";
            this.selectionRangeSliderWrapper2.SelectedMax = 1F;
            this.selectionRangeSliderWrapper2.SelectedMin = 0F;
            this.selectionRangeSliderWrapper2.Size = new System.Drawing.Size(244, 68);
            this.selectionRangeSliderWrapper2.TabIndex = 8;
            this.selectionRangeSliderWrapper2.Title = "Sensitivity";
            this.selectionRangeSliderWrapper2.SelectionChanged += new System.EventHandler(this.SensitivityChanged);
            // 
            // selectionRangeSliderWrapper1
            // 
            this.selectionRangeSliderWrapper1.Location = new System.Drawing.Point(588, 187);
            this.selectionRangeSliderWrapper1.Max = 1F;
            this.selectionRangeSliderWrapper1.Min = -1F;
            this.selectionRangeSliderWrapper1.Name = "selectionRangeSliderWrapper1";
            this.selectionRangeSliderWrapper1.SelectedMax = 1F;
            this.selectionRangeSliderWrapper1.SelectedMin = 0F;
            this.selectionRangeSliderWrapper1.Size = new System.Drawing.Size(244, 68);
            this.selectionRangeSliderWrapper1.TabIndex = 7;
            this.selectionRangeSliderWrapper1.Title = "Heat";
            this.selectionRangeSliderWrapper1.SelectionChanged += new System.EventHandler(this.HeatChanged);
            // 
            // modelMover1
            // 
            this.modelMover1.Location = new System.Drawing.Point(501, 73);
            this.modelMover1.Model = null;
            this.modelMover1.Name = "modelMover1";
            this.modelMover1.Size = new System.Drawing.Size(344, 78);
            this.modelMover1.TabIndex = 4;
            // 
            // d3DPanel1
            // 
            this.d3DPanel1.drawer = null;
            this.d3DPanel1.Heatmap = new System.Drawing.SizeF(0F, 1F);
            this.d3DPanel1.Location = new System.Drawing.Point(12, 12);
            this.d3DPanel1.Name = "d3DPanel1";
            this.d3DPanel1.PhisicsStarted = true;
            this.d3DPanel1.PhisicsStepSize = 0;
            this.d3DPanel1.PhisicStep = true;
            this.d3DPanel1.Sensitivitymap = new System.Drawing.SizeF(0F, 1F);
            this.d3DPanel1.Size = new System.Drawing.Size(400, 400);
            this.d3DPanel1.TabIndex = 0;
            this.d3DPanel1.Text = "d3DPanel1";
            // 
            // SimulationForm2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 418);
            this.Controls.Add(this.selectionRangeSliderWrapper2);
            this.Controls.Add(this.selectionRangeSliderWrapper1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.modelMover1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.d3DPanel1);
            this.Name = "SimulationForm2D";
            this.Text = "SimulationForm2D";
            this.ResumeLayout(false);

        }

        #endregion

        private D3DPanel d3DPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private Forms.ModelMover modelMover1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private SelectionRangeSliderWrapper selectionRangeSliderWrapper1;
        private SelectionRangeSliderWrapper selectionRangeSliderWrapper2;
    }
}