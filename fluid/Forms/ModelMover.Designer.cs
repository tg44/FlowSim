namespace fluid.Forms
{
    partial class ModelMover
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudx = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudy = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudz = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudr = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudr)).BeginInit();
            this.SuspendLayout();
            // 
            // nudx
            // 
            this.nudx.DecimalPlaces = 2;
            this.nudx.Location = new System.Drawing.Point(27, 2);
            this.nudx.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudx.Name = "nudx";
            this.nudx.Size = new System.Drawing.Size(120, 20);
            this.nudx.TabIndex = 0;
            this.nudx.ValueChanged += new System.EventHandler(this.valChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y:";
            // 
            // nudy
            // 
            this.nudy.Location = new System.Drawing.Point(27, 28);
            this.nudy.Name = "nudy";
            this.nudy.Size = new System.Drawing.Size(120, 20);
            this.nudy.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Z:";
            // 
            // nudz
            // 
            this.nudz.Location = new System.Drawing.Point(27, 54);
            this.nudz.Name = "nudz";
            this.nudz.Size = new System.Drawing.Size(120, 20);
            this.nudz.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(157, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Rotation:";
            // 
            // nudr
            // 
            this.nudr.Location = new System.Drawing.Point(213, 2);
            this.nudr.Name = "nudr";
            this.nudr.Size = new System.Drawing.Size(120, 20);
            this.nudr.TabIndex = 6;
            // 
            // ModelMover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudr);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudz);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudx);
            this.Name = "ModelMover";
            this.Size = new System.Drawing.Size(344, 78);
            ((System.ComponentModel.ISupportInitialize)(this.nudx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudz;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudr;
    }
}
