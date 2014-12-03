namespace FutsMoniter
{
    partial class fmChangeInvestor
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
            this.cbnosetbank = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.bankac = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.broker = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ctBankList1 = new FutsMoniter.ctBankList();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.cbnosetbank);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.bankac);
            this.kryptonPanel1.Controls.Add(this.broker);
            this.kryptonPanel1.Controls.Add(this.name);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.ctBankList1);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(279, 228);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // cbnosetbank
            // 
            this.cbnosetbank.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.cbnosetbank.Location = new System.Drawing.Point(86, 158);
            this.cbnosetbank.Name = "cbnosetbank";
            this.cbnosetbank.Size = new System.Drawing.Size(132, 18);
            this.cbnosetbank.TabIndex = 10;
            this.cbnosetbank.Text = "不设置银行卡信息";
            this.cbnosetbank.Values.Text = "不设置银行卡信息";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(187, 191);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(80, 25);
            this.btnSubmit.TabIndex = 9;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // bankac
            // 
            this.bankac.Location = new System.Drawing.Point(86, 118);
            this.bankac.Name = "bankac";
            this.bankac.Size = new System.Drawing.Size(162, 21);
            this.bankac.TabIndex = 8;
            // 
            // broker
            // 
            this.broker.AcceptsReturn = true;
            this.broker.Location = new System.Drawing.Point(86, 64);
            this.broker.Name = "broker";
            this.broker.Size = new System.Drawing.Size(162, 21);
            this.broker.TabIndex = 7;
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(86, 36);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(122, 21);
            this.name.TabIndex = 6;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(86, 12);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(19, 18);
            this.account.TabIndex = 5;
            this.account.Values.Text = "--";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(12, 118);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "银行帐号:";
            // 
            // ctBankList1
            // 
            this.ctBankList1.BankSelected = 0;
            this.ctBankList1.Location = new System.Drawing.Point(35, 92);
            this.ctBankList1.Name = "ctBankList1";
            this.ctBankList1.Size = new System.Drawing.Size(173, 20);
            this.ctBankList1.TabIndex = 3;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(12, 60);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "期货公司:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(39, 36);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "姓名:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "交易帐号:";
            // 
            // fmChangeInvestor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 228);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmChangeInvestor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改投资者信息";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ctBankList ctBankList1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox bankac;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox broker;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel account;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox cbnosetbank;
    }
}