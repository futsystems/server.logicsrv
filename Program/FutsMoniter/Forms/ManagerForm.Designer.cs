namespace FutsMoniter
{
    partial class ManagerForm
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
            this.secgrid = new Telerik.WinControls.UI.RadGridView();
            this.ctBasicMangerInfo1 = new FutsMoniter.ctBasicMangerInfo();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // secgrid
            // 
            this.secgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.secgrid.Location = new System.Drawing.Point(2, 18);
            this.secgrid.Name = "secgrid";
            this.secgrid.Size = new System.Drawing.Size(724, 213);
            this.secgrid.TabIndex = 3;
            this.secgrid.Text = "radGridView1";
            this.secgrid.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.secgrid_ContextMenuOpening);
            this.secgrid.DoubleClick += new System.EventHandler(this.secgrid_DoubleClick);
            // 
            // ctBasicMangerInfo1
            // 
            this.ctBasicMangerInfo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctBasicMangerInfo1.Location = new System.Drawing.Point(0, 0);
            this.ctBasicMangerInfo1.Name = "ctBasicMangerInfo1";
            this.ctBasicMangerInfo1.Size = new System.Drawing.Size(728, 90);
            this.ctBasicMangerInfo1.TabIndex = 4;
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.secgrid);
            this.radGroupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radGroupBox1.HeaderText = "员工与代理";
            this.radGroupBox1.Location = new System.Drawing.Point(0, 96);
            this.radGroupBox1.Name = "radGroupBox1";
            // 
            // 
            // 
            this.radGroupBox1.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox1.Size = new System.Drawing.Size(728, 233);
            this.radGroupBox1.TabIndex = 5;
            this.radGroupBox1.Text = "员工与代理";
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 329);
            this.Controls.Add(this.radGroupBox1);
            this.Controls.Add(this.ctBasicMangerInfo1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagerForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "柜员列表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView secgrid;
        private ctBasicMangerInfo ctBasicMangerInfo1;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
    }
}