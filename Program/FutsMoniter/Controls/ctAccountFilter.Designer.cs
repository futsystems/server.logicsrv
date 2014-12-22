namespace FutsMoniter.Controls
{
    partial class ctAccountFilter
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.ctAccountType1 = new FutsMoniter.Controls.Base.ctAccountType();
            this.ctRouterGroupList1 = new FutsMoniter.Controls.Base.ctRouterGroupList();
            this.ctRouterType1 = new FutsMoniter.Controls.Base.ctRouterType();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.accexecute = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.accexecute);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Controls.Add(this.ctAccountType1);
            this.kryptonPanel1.Controls.Add(this.ctRouterGroupList1);
            this.kryptonPanel1.Controls.Add(this.ctRouterType1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(274, 243);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.EnableAny = false;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.EnableSelf = true;
            this.ctAgentList1.Location = new System.Drawing.Point(15, 47);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(190, 21);
            this.ctAgentList1.TabIndex = 4;
            // 
            // ctAccountType1
            // 
            this.ctAccountType1.AccountType = TradingLib.API.QSEnumAccountCategory.SIMULATION;
            this.ctAccountType1.EnableAny = false;
            this.ctAccountType1.Location = new System.Drawing.Point(30, 74);
            this.ctAccountType1.Name = "ctAccountType1";
            this.ctAccountType1.Size = new System.Drawing.Size(175, 21);
            this.ctAccountType1.SmallSpace = false;
            this.ctAccountType1.TabIndex = 3;
            // 
            // ctRouterGroupList1
            // 
            this.ctRouterGroupList1.EnableAny = false;
            this.ctRouterGroupList1.Location = new System.Drawing.Point(15, 128);
            this.ctRouterGroupList1.Name = "ctRouterGroupList1";
            this.ctRouterGroupList1.RouterGroup = null;
            this.ctRouterGroupList1.Size = new System.Drawing.Size(190, 21);
            this.ctRouterGroupList1.TabIndex = 2;
            // 
            // ctRouterType1
            // 
            this.ctRouterType1.EnableAny = false;
            this.ctRouterType1.Location = new System.Drawing.Point(3, 101);
            this.ctRouterType1.Name = "ctRouterType1";
            this.ctRouterType1.Size = new System.Drawing.Size(202, 21);
            this.ctRouterType1.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(30, 23);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 5;
            this.kryptonLabel1.Values.Text = "状态:";
            // 
            // accexecute
            // 
            this.accexecute.DropDownWidth = 128;
            this.accexecute.Location = new System.Drawing.Point(77, 20);
            this.accexecute.Name = "accexecute";
            this.accexecute.Size = new System.Drawing.Size(128, 21);
            this.accexecute.TabIndex = 6;
            // 
            // ctAccountFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctAccountFilter";
            this.Size = new System.Drawing.Size(274, 243);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Base.ctRouterGroupList ctRouterGroupList1;
        private Base.ctRouterType ctRouterType1;
        private Base.ctAccountType ctAccountType1;
        private ctAgentList ctAgentList1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox accexecute;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}
