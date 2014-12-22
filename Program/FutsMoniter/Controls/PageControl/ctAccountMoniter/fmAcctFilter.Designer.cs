namespace FutsMoniter
{
    partial class fmAcctFilter
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.accexecute = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ctAccountType1 = new FutsMoniter.Controls.Base.ctAccountType();
            this.ctRouterGroupList1 = new FutsMoniter.Controls.Base.ctRouterGroupList();
            this.ctRouterType1 = new FutsMoniter.Controls.Base.ctRouterType();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Controls.Add(this.accexecute);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.ctAccountType1);
            this.kryptonPanel1.Controls.Add(this.ctRouterGroupList1);
            this.kryptonPanel1.Controls.Add(this.ctRouterType1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(232, 169);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.CurrentAgentFK = 0;
            this.ctAgentList1.EnableAny = true;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.EnableSelf = true;
            this.ctAgentList1.Location = new System.Drawing.Point(12, 63);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(190, 21);
            this.ctAgentList1.TabIndex = 13;
            // 
            // accexecute
            // 
            this.accexecute.DropDownWidth = 128;
            this.accexecute.Location = new System.Drawing.Point(78, 9);
            this.accexecute.Name = "accexecute";
            this.accexecute.Size = new System.Drawing.Size(124, 21);
            this.accexecute.TabIndex = 12;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(27, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 11;
            this.kryptonLabel1.Values.Text = "状态:";
            // 
            // ctAccountType1
            // 
            this.ctAccountType1.AccountType = TradingLib.API.QSEnumAccountCategory.SIMULATION;
            this.ctAccountType1.EnableAny = true;
            this.ctAccountType1.Location = new System.Drawing.Point(27, 36);
            this.ctAccountType1.Name = "ctAccountType1";
            this.ctAccountType1.Size = new System.Drawing.Size(175, 21);
            this.ctAccountType1.SmallSpace = false;
            this.ctAccountType1.TabIndex = 9;
            // 
            // ctRouterGroupList1
            // 
            this.ctRouterGroupList1.EnableAny = true;
            this.ctRouterGroupList1.Location = new System.Drawing.Point(12, 117);
            this.ctRouterGroupList1.Name = "ctRouterGroupList1";
            this.ctRouterGroupList1.RouterGroup = null;
            this.ctRouterGroupList1.RouterGroupID = 0;
            this.ctRouterGroupList1.Size = new System.Drawing.Size(190, 21);
            this.ctRouterGroupList1.TabIndex = 8;
            // 
            // ctRouterType1
            // 
            this.ctRouterType1.EnableAny = true;
            this.ctRouterType1.Location = new System.Drawing.Point(0, 90);
            this.ctRouterType1.Name = "ctRouterType1";
            this.ctRouterType1.Size = new System.Drawing.Size(202, 21);
            this.ctRouterType1.TabIndex = 7;
            // 
            // fmAcctFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 169);
            this.Controls.Add(this.kryptonPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmAcctFilter";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox accexecute;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private FutsMoniter.Controls.Base.ctAccountType ctAccountType1;
        private FutsMoniter.Controls.Base.ctRouterGroupList ctRouterGroupList1;
        private FutsMoniter.Controls.Base.ctRouterType ctRouterType1;
        private ctAgentList ctAgentList1;
    }
}