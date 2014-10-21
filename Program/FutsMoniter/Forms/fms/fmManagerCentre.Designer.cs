namespace FutsMoniter
{
    partial class fmManagerCentre
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
            this.lbbasemgrfk = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ctBasicMangerInfo1 = new FutsMoniter.ctBasicMangerInfo();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.secgrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.lbbasemgrfk)).BeginInit();
            this.lbbasemgrfk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // lbbasemgrfk
            // 
            this.lbbasemgrfk.Controls.Add(this.kryptonGroupBox1);
            this.lbbasemgrfk.Controls.Add(this.ctBasicMangerInfo1);
            this.lbbasemgrfk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbbasemgrfk.Location = new System.Drawing.Point(0, 0);
            this.lbbasemgrfk.Name = "lbbasemgrfk";
            this.lbbasemgrfk.Size = new System.Drawing.Size(753, 360);
            this.lbbasemgrfk.TabIndex = 0;
            // 
            // ctBasicMangerInfo1
            // 
            this.ctBasicMangerInfo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctBasicMangerInfo1.Location = new System.Drawing.Point(0, 0);
            this.ctBasicMangerInfo1.Name = "ctBasicMangerInfo1";
            this.ctBasicMangerInfo1.Size = new System.Drawing.Size(753, 98);
            this.ctBasicMangerInfo1.TabIndex = 0;
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonGroupBox1.Location = new System.Drawing.Point(0, 104);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.secgrid);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(753, 256);
            this.kryptonGroupBox1.TabIndex = 1;
            this.kryptonGroupBox1.Text = "员工与代理";
            this.kryptonGroupBox1.Values.Heading = "员工与代理";
            // 
            // secgrid
            // 
            this.secgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.secgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.secgrid.Location = new System.Drawing.Point(0, 0);
            this.secgrid.Name = "secgrid";
            this.secgrid.RowTemplate.Height = 23;
            this.secgrid.Size = new System.Drawing.Size(749, 234);
            this.secgrid.TabIndex = 0;
            // 
            // fmManagerCentre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 360);
            this.Controls.Add(this.lbbasemgrfk);
            this.Name = "fmManagerCentre";
            this.Text = "fmManagerList";
            ((System.ComponentModel.ISupportInitialize)(this.lbbasemgrfk)).EndInit();
            this.lbbasemgrfk.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel lbbasemgrfk;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ctBasicMangerInfo ctBasicMangerInfo1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView secgrid;
    }
}