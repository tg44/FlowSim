﻿namespace fluid
{
    partial class Form1
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
            this.d3DPanel1 = new fluid.D3DPanel();
            this.labelTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(388, 62);
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
            this.label1.Location = new System.Drawing.Point(388, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "asd";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(388, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(388, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "label3";
            // 
            // cbuttonUp
            // 
            this.cbuttonUp.Location = new System.Drawing.Point(423, 159);
            this.cbuttonUp.Name = "cbuttonUp";
            this.cbuttonUp.Size = new System.Drawing.Size(29, 29);
            this.cbuttonUp.TabIndex = 5;
            this.cbuttonUp.Text = "↑";
            this.cbuttonUp.UseVisualStyleBackColor = true;
            this.cbuttonUp.Click += new System.EventHandler(this.controlUp);
            // 
            // cbuttonDown
            // 
            this.cbuttonDown.Location = new System.Drawing.Point(423, 198);
            this.cbuttonDown.Name = "cbuttonDown";
            this.cbuttonDown.Size = new System.Drawing.Size(29, 29);
            this.cbuttonDown.TabIndex = 6;
            this.cbuttonDown.Text = "↓";
            this.cbuttonDown.UseVisualStyleBackColor = true;
            this.cbuttonDown.Click += new System.EventHandler(this.controlDown);
            // 
            // cbuttonLeft
            // 
            this.cbuttonLeft.Location = new System.Drawing.Point(388, 180);
            this.cbuttonLeft.Name = "cbuttonLeft";
            this.cbuttonLeft.Size = new System.Drawing.Size(29, 29);
            this.cbuttonLeft.TabIndex = 7;
            this.cbuttonLeft.Text = " ←";
            this.cbuttonLeft.UseVisualStyleBackColor = true;
            this.cbuttonLeft.Click += new System.EventHandler(this.controlLeft);
            // 
            // cbuttonRight
            // 
            this.cbuttonRight.Location = new System.Drawing.Point(458, 180);
            this.cbuttonRight.Name = "cbuttonRight";
            this.cbuttonRight.Size = new System.Drawing.Size(29, 29);
            this.cbuttonRight.TabIndex = 8;
            this.cbuttonRight.Text = "→";
            this.cbuttonRight.UseVisualStyleBackColor = true;
            this.cbuttonRight.Click += new System.EventHandler(this.controlRight);
            // 
            // cbuttonZoomIn
            // 
            this.cbuttonZoomIn.Location = new System.Drawing.Point(516, 159);
            this.cbuttonZoomIn.Name = "cbuttonZoomIn";
            this.cbuttonZoomIn.Size = new System.Drawing.Size(29, 29);
            this.cbuttonZoomIn.TabIndex = 9;
            this.cbuttonZoomIn.Text = "+";
            this.cbuttonZoomIn.UseVisualStyleBackColor = true;
            this.cbuttonZoomIn.Click += new System.EventHandler(this.controlZoomIn);
            // 
            // cbuttonZoomOut
            // 
            this.cbuttonZoomOut.Location = new System.Drawing.Point(516, 198);
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
            this.label4.Location = new System.Drawing.Point(388, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "label4";
            // 
            // d3DPanel1
            // 
            this.d3DPanel1.drawer = null;
            this.d3DPanel1.Location = new System.Drawing.Point(45, 62);
            this.d3DPanel1.Name = "d3DPanel1";
            this.d3DPanel1.Size = new System.Drawing.Size(337, 281);
            this.d3DPanel1.TabIndex = 0;
            this.d3DPanel1.Text = "d3DPanel1";
            // 
            // labelTimer
            // 
            this.labelTimer.Interval = 1000;
            this.labelTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 463);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbuttonZoomOut);
            this.Controls.Add(this.cbuttonZoomIn);
            this.Controls.Add(this.cbuttonRight);
            this.Controls.Add(this.cbuttonLeft);
            this.Controls.Add(this.cbuttonDown);
            this.Controls.Add(this.cbuttonUp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.d3DPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
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
    }
}