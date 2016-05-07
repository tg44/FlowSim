﻿namespace fluid
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cpuStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.vidMemoryStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.vidTypeStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusRefresher = new System.Windows.Forms.Timer(this.components);
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.selectionRangeSliderWrapper2 = new fluid.SelectionRangeSliderWrapper();
            this.selectionRangeSliderWrapper1 = new fluid.SelectionRangeSliderWrapper();
            this.modelMover1 = new fluid.Forms.ModelMover();
            this.d3DPanel1 = new fluid.D3DPanel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(549, 13);
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
            this.button2.Location = new System.Drawing.Point(549, 43);
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
            this.listBox1.Location = new System.Drawing.Point(549, 73);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(75, 108);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.selectionChanged);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(548, 213);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Simulation";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(549, 187);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "SavePreset";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(631, 43);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(161, 21);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cpuStatusLabel,
            this.toolStripStatusLabel1,
            this.fpsStatusLabel,
            this.toolStripStatusLabel2,
            this.vidMemoryStatusLabel,
            this.toolStripStatusLabel3,
            this.vidTypeStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 565);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1039, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cpuStatusLabel
            // 
            this.cpuStatusLabel.Name = "cpuStatusLabel";
            this.cpuStatusLabel.Size = new System.Drawing.Size(30, 17);
            this.cpuStatusLabel.Text = "CPU";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // fpsStatusLabel
            // 
            this.fpsStatusLabel.Name = "fpsStatusLabel";
            this.fpsStatusLabel.Size = new System.Drawing.Size(26, 17);
            this.fpsStatusLabel.Text = "FPS";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // vidMemoryStatusLabel
            // 
            this.vidMemoryStatusLabel.Name = "vidMemoryStatusLabel";
            this.vidMemoryStatusLabel.Size = new System.Drawing.Size(68, 17);
            this.vidMemoryStatusLabel.Text = "vidMemory";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // vidTypeStatusLabel
            // 
            this.vidTypeStatusLabel.Name = "vidTypeStatusLabel";
            this.vidTypeStatusLabel.Size = new System.Drawing.Size(48, 17);
            this.vidTypeStatusLabel.Text = "vidType";
            // 
            // statusRefresher
            // 
            this.statusRefresher.Interval = 1000;
            this.statusRefresher.Tick += new System.EventHandler(this.statusRefresher_Tick);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(801, 43);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(161, 21);
            this.comboBox2.TabIndex = 11;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // selectionRangeSliderWrapper2
            // 
            this.selectionRangeSliderWrapper2.Location = new System.Drawing.Point(718, 261);
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
            this.selectionRangeSliderWrapper1.Location = new System.Drawing.Point(718, 187);
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
            this.modelMover1.Location = new System.Drawing.Point(631, 73);
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
            this.d3DPanel1.Size = new System.Drawing.Size(512, 512);
            this.d3DPanel1.TabIndex = 0;
            this.d3DPanel1.Text = "d3DPanel1";
            // 
            // SimulationForm2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 587);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.comboBox1);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel cpuStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel fpsStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel vidMemoryStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel vidTypeStatusLabel;
        private System.Windows.Forms.Timer statusRefresher;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}