namespace FutsMoniter
{
    partial class fmAddAccount
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
            this.ctRouterGroupList1 = new FutsMoniter.Controls.Base.ctRouterGroupList();
            this.btnAddAccount = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.password = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ctAccountType1 = new FutsMoniter.Controls.Base.ctAccountType();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ctAccountType1);
            this.kryptonPanel1.Controls.Add(this.ctRouterGroupList1);
            this.kryptonPanel1.Controls.Add(this.btnAddAccount);
            this.kryptonPanel1.Controls.Add(this.password);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(253, 219);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // ctRouterGroupList1
            // 
            this.ctRouterGroupList1.Location = new System.Drawing.Point(24, 126);
            this.ctRouterGroupList1.Name = "ctRouterGroupList1";
            this.ctRouterGroupList1.RouterGroudSelected = null;
            this.ctRouterGroupList1.Size = new System.Drawing.Size(199, 28);
            this.ctRouterGroupList1.TabIndex = 1;
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAccount.Location = new System.Drawing.Point(135, 182);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(106, 25);
            this.btnAddAccount.TabIndex = 7;
            this.btnAddAccount.Values.Text = "添加交易帐号";
            this.btnAddAccount.Click += new System.EventHandler(this.btnAddAccount_Click);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(86, 67);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(121, 21);
            this.password.TabIndex = 6;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(86, 39);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(121, 21);
            this.account.TabIndex = 5;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.EnableAny = false;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.EnableSelf = true;
            this.ctAgentList1.Location = new System.Drawing.Point(24, 95);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(185, 25);
            this.ctAgentList1.TabIndex = 3;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(39, 71);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "密码:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(39, 39);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "帐号:";
            // 
            // ctAccountType1
            // 
            this.ctAccountType1.EnableAny = false;
            this.ctAccountType1.Location = new System.Drawing.Point(35, 4);
            this.ctAccountType1.Name = "ctAccountType1";
            this.ctAccountType1.Size = new System.Drawing.Size(156, 29);
            this.ctAccountType1.TabIndex = 8;
            // 
            // fmAddAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 219);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmAddAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加交易帐户";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ctAgentList ctAgentList1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox password;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox account;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddAccount;
        private Controls.Base.ctRouterGroupList ctRouterGroupList1;
        private Controls.Base.ctAccountType ctAccountType1;
    }
}