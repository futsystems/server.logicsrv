namespace FutsMoniter
{
    partial class ctAccountMontier
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ctRouterGroupList1 = new FutsMoniter.Controls.Base.ctRouterGroupList();
            this.ctRouterType1 = new FutsMoniter.Controls.Base.ctRouterType();
            this.ctAccountType1 = new FutsMoniter.Controls.Base.ctAccountType();
            this.lbCurrentAccount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnAddAccount = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.num = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.acchodpos = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.accLogin = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.acct = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.accexecute = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.accountgrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ctRouterGroupList1);
            this.kryptonPanel1.Controls.Add(this.ctRouterType1);
            this.kryptonPanel1.Controls.Add(this.ctAccountType1);
            this.kryptonPanel1.Controls.Add(this.lbCurrentAccount);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel1.Controls.Add(this.btnAddAccount);
            this.kryptonPanel1.Controls.Add(this.num);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.acchodpos);
            this.kryptonPanel1.Controls.Add(this.accLogin);
            this.kryptonPanel1.Controls.Add(this.acct);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Controls.Add(this.accexecute);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(1380, 28);
            this.kryptonPanel1.TabIndex = 31;
            // 
            // ctRouterGroupList1
            // 
            this.ctRouterGroupList1.EnableAny = false;
            this.ctRouterGroupList1.Location = new System.Drawing.Point(991, 3);
            this.ctRouterGroupList1.Name = "ctRouterGroupList1";
            this.ctRouterGroupList1.RouterGroudSelected = null;
            this.ctRouterGroupList1.Size = new System.Drawing.Size(190, 21);
            this.ctRouterGroupList1.TabIndex = 18;
            // 
            // ctRouterType1
            // 
            this.ctRouterType1.EnableAny = true;
            this.ctRouterType1.Location = new System.Drawing.Point(680, 2);
            this.ctRouterType1.Name = "ctRouterType1";
            this.ctRouterType1.Size = new System.Drawing.Size(156, 25);
            this.ctRouterType1.TabIndex = 17;
            // 
            // ctAccountType1
            // 
            this.ctAccountType1.AccountType = TradingLib.API.QSEnumAccountCategory.SIMULATION;
            this.ctAccountType1.EnableAny = true;
            this.ctAccountType1.Location = new System.Drawing.Point(842, 2);
            this.ctAccountType1.Name = "ctAccountType1";
            this.ctAccountType1.Size = new System.Drawing.Size(142, 20);
            this.ctAccountType1.SmallSpace = false;
            this.ctAccountType1.TabIndex = 16;
            // 
            // lbCurrentAccount
            // 
            this.lbCurrentAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCurrentAccount.Location = new System.Drawing.Point(1354, 3);
            this.lbCurrentAccount.Name = "lbCurrentAccount";
            this.lbCurrentAccount.Size = new System.Drawing.Size(23, 22);
            this.lbCurrentAccount.StateCommon.ShortText.Color1 = System.Drawing.Color.Maroon;
            this.lbCurrentAccount.StateCommon.ShortText.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentAccount.TabIndex = 15;
            this.lbCurrentAccount.Values.Text = "--";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonLabel7.Location = new System.Drawing.Point(1210, 4);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel7.TabIndex = 14;
            this.kryptonLabel7.Values.Text = "选中帐号:";
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAccount.Location = new System.Drawing.Point(1136, 2);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(68, 25);
            this.btnAddAccount.TabIndex = 13;
            this.btnAddAccount.Values.Text = "添加帐户";
            // 
            // num
            // 
            this.num.Location = new System.Drawing.Point(632, 4);
            this.num.Name = "num";
            this.num.Size = new System.Drawing.Size(19, 18);
            this.num.TabIndex = 10;
            this.num.Values.Text = "--";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(567, 4);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 9;
            this.kryptonLabel4.Values.Text = "帐户总数:";
            // 
            // acchodpos
            // 
            this.acchodpos.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.acchodpos.Location = new System.Drawing.Point(510, 4);
            this.acchodpos.Name = "acchodpos";
            this.acchodpos.Size = new System.Drawing.Size(51, 18);
            this.acchodpos.TabIndex = 8;
            this.acchodpos.Text = "持仓";
            this.acchodpos.Values.Text = "持仓";
            // 
            // accLogin
            // 
            this.accLogin.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.accLogin.Location = new System.Drawing.Point(453, 4);
            this.accLogin.Name = "accLogin";
            this.accLogin.Size = new System.Drawing.Size(51, 18);
            this.accLogin.TabIndex = 7;
            this.accLogin.Text = "登入";
            this.accLogin.Values.Text = "登入";
            // 
            // acct
            // 
            this.acct.Location = new System.Drawing.Point(346, 2);
            this.acct.Name = "acct";
            this.acct.Size = new System.Drawing.Size(100, 21);
            this.acct.TabIndex = 6;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(309, 4);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 5;
            this.kryptonLabel3.Values.Text = "帐号:";
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.EnableAny = true;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.EnableSelf = true;
            this.ctAgentList1.Location = new System.Drawing.Point(123, 0);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(181, 26);
            this.ctAgentList1.TabIndex = 4;
            // 
            // accexecute
            // 
            this.accexecute.DropDownWidth = 121;
            this.accexecute.Location = new System.Drawing.Point(51, 2);
            this.accexecute.Name = "accexecute";
            this.accexecute.Size = new System.Drawing.Size(70, 21);
            this.accexecute.TabIndex = 3;
            this.accexecute.Text = "--";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(4, 4);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "状态:";
            // 
            // accountgrid
            // 
            this.accountgrid.AllowUserToAddRows = false;
            this.accountgrid.AllowUserToDeleteRows = false;
            this.accountgrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.accountgrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.accountgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.accountgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.accountgrid.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.accountgrid.Location = new System.Drawing.Point(0, 28);
            this.accountgrid.Name = "accountgrid";
            this.accountgrid.ReadOnly = true;
            this.accountgrid.RowHeadersVisible = false;
            this.accountgrid.RowTemplate.Height = 23;
            this.accountgrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.accountgrid.Size = new System.Drawing.Size(1377, 607);
            this.accountgrid.TabIndex = 30;
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Controls.Add(this.accountgrid);
            this.kryptonPanel2.Controls.Add(this.kryptonPanel1);
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(1380, 638);
            this.kryptonPanel2.TabIndex = 2;
            // 
            // ctAccountMontier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel2);
            this.DoubleBuffered = true;
            this.Name = "ctAccountMontier";
            this.Size = new System.Drawing.Size(1380, 638);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accexecute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddAccount;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel num;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox acchodpos;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox accLogin;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox acct;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ctAgentList ctAgentList1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox accexecute;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView accountgrid;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbCurrentAccount;

        private FutsMoniter.Controls.Base.ctAccountType ctAccountType1;
        private FutsMoniter.Controls.Base.ctRouterType ctRouterType1;
        private FutsMoniter.Controls.Base.ctRouterGroupList ctRouterGroupList1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;

        //private Telerik.WinControls.UI.RadPageViewPage LottoServicePage;

    }
}
