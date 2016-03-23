namespace fluid
{
    partial class SimulationForm3D
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
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbuttonUp = new System.Windows.Forms.Button();
            this.cbuttonDown = new System.Windows.Forms.Button();
            this.cbuttonLeft = new System.Windows.Forms.Button();
            this.cbuttonRight = new System.Windows.Forms.Button();
            this.cbuttonZoomIn = new System.Windows.Forms.Button();
            this.cbuttonZoomOut = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTimer = new System.Windows.Forms.Timer(this.components);
            this.Sensitivity = new fluid.SelectionRangeSliderWrapper();
            this.HeatMap = new fluid.SelectionRangeSliderWrapper();
            this.d3DPanel1 = new fluid.D3DPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(451, 62);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.sartRender);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(451, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "asd";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(451, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(451, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "label3";
            // 
            // cbuttonUp
            // 
            this.cbuttonUp.Location = new System.Drawing.Point(46, 16);
            this.cbuttonUp.Name = "cbuttonUp";
            this.cbuttonUp.Size = new System.Drawing.Size(30, 30);
            this.cbuttonUp.TabIndex = 5;
            this.cbuttonUp.Text = "↑";
            this.cbuttonUp.UseVisualStyleBackColor = true;
            this.cbuttonUp.Click += new System.EventHandler(this.controlUp);
            // 
            // cbuttonDown
            // 
            this.cbuttonDown.Location = new System.Drawing.Point(46, 55);
            this.cbuttonDown.Name = "cbuttonDown";
            this.cbuttonDown.Size = new System.Drawing.Size(30, 30);
            this.cbuttonDown.TabIndex = 6;
            this.cbuttonDown.Text = "↓";
            this.cbuttonDown.UseVisualStyleBackColor = true;
            this.cbuttonDown.Click += new System.EventHandler(this.controlDown);
            // 
            // cbuttonLeft
            // 
            this.cbuttonLeft.Location = new System.Drawing.Point(11, 37);
            this.cbuttonLeft.Name = "cbuttonLeft";
            this.cbuttonLeft.Size = new System.Drawing.Size(30, 30);
            this.cbuttonLeft.TabIndex = 7;
            this.cbuttonLeft.Text = " ←";
            this.cbuttonLeft.UseVisualStyleBackColor = true;
            this.cbuttonLeft.Click += new System.EventHandler(this.controlLeft);
            // 
            // cbuttonRight
            // 
            this.cbuttonRight.Location = new System.Drawing.Point(81, 37);
            this.cbuttonRight.Name = "cbuttonRight";
            this.cbuttonRight.Size = new System.Drawing.Size(30, 30);
            this.cbuttonRight.TabIndex = 8;
            this.cbuttonRight.Text = "→";
            this.cbuttonRight.UseVisualStyleBackColor = true;
            this.cbuttonRight.Click += new System.EventHandler(this.controlRight);
            // 
            // cbuttonZoomIn
            // 
            this.cbuttonZoomIn.Location = new System.Drawing.Point(139, 16);
            this.cbuttonZoomIn.Name = "cbuttonZoomIn";
            this.cbuttonZoomIn.Size = new System.Drawing.Size(29, 29);
            this.cbuttonZoomIn.TabIndex = 9;
            this.cbuttonZoomIn.Text = "+";
            this.cbuttonZoomIn.UseVisualStyleBackColor = true;
            this.cbuttonZoomIn.Click += new System.EventHandler(this.controlZoomIn);
            // 
            // cbuttonZoomOut
            // 
            this.cbuttonZoomOut.Location = new System.Drawing.Point(139, 55);
            this.cbuttonZoomOut.Name = "cbuttonZoomOut";
            this.cbuttonZoomOut.Size = new System.Drawing.Size(29, 29);
            this.cbuttonZoomOut.TabIndex = 10;
            this.cbuttonZoomOut.Text = "-";
            this.cbuttonZoomOut.UseVisualStyleBackColor = true;
            this.cbuttonZoomOut.Click += new System.EventHandler(this.controlZoomOut);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(451, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "label4";
            // 
            // labelTimer
            // 
            this.labelTimer.Interval = 1000;
            this.labelTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Sensitivity
            // 
            this.Sensitivity.Location = new System.Drawing.Point(454, 361);
            this.Sensitivity.Max = 1F;
            this.Sensitivity.Min = -1F;
            this.Sensitivity.Name = "Sensitivity";
            this.Sensitivity.SelectedMax = 1F;
            this.Sensitivity.SelectedMin = 0F;
            this.Sensitivity.Size = new System.Drawing.Size(373, 87);
            this.Sensitivity.TabIndex = 13;
            this.Sensitivity.Title = "Transparency sensitivity";
            this.Sensitivity.SelectionChanged += new System.EventHandler(this.SensitivityChanged);
            // 
            // HeatMap
            // 
            this.HeatMap.Location = new System.Drawing.Point(454, 268);
            this.HeatMap.Max = 1F;
            this.HeatMap.Min = -1F;
            this.HeatMap.Name = "HeatMap";
            this.HeatMap.SelectedMax = 1F;
            this.HeatMap.SelectedMin = 0F;
            this.HeatMap.Size = new System.Drawing.Size(373, 87);
            this.HeatMap.TabIndex = 12;
            this.HeatMap.Title = "HeatMap";
            this.HeatMap.SelectionChanged += new System.EventHandler(this.HeatmapChanged);
            // 
            // d3DPanel1
            // 
            this.d3DPanel1.drawer = null;
            this.d3DPanel1.Heatmap = new System.Drawing.SizeF(0F, 1F);
            this.d3DPanel1.Location = new System.Drawing.Point(45, 62);
            this.d3DPanel1.Name = "d3DPanel1";
            this.d3DPanel1.PhisicStep = true;
            this.d3DPanel1.Sensitivitymap = new System.Drawing.SizeF(0F, 1F);
            this.d3DPanel1.Size = new System.Drawing.Size(400, 400);
            this.d3DPanel1.TabIndex = 0;
            this.d3DPanel1.Text = "d3DPanel1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbuttonUp);
            this.panel1.Controls.Add(this.cbuttonDown);
            this.panel1.Controls.Add(this.cbuttonLeft);
            this.panel1.Controls.Add(this.cbuttonRight);
            this.panel1.Controls.Add(this.cbuttonZoomOut);
            this.panel1.Controls.Add(this.cbuttonZoomIn);
            this.panel1.Location = new System.Drawing.Point(454, 154);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(183, 97);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(643, 138);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 113);
            this.panel2.TabIndex = 15;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(77, 81);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(118, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.stopPhisics);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(77, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "set sample rate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.startPhisics);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(4, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(66, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(77, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "phisics 1step";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.oneStep);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Sensitivity);
            this.Controls.Add(this.HeatMap);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.d3DPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private D3DPanel d3DPanel1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cbuttonUp;
        private System.Windows.Forms.Button cbuttonDown;
        private System.Windows.Forms.Button cbuttonLeft;
        private System.Windows.Forms.Button cbuttonRight;
        private System.Windows.Forms.Button cbuttonZoomIn;
        private System.Windows.Forms.Button cbuttonZoomOut;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer labelTimer;
        private SelectionRangeSliderWrapper HeatMap;
        private SelectionRangeSliderWrapper Sensitivity;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
    }
}