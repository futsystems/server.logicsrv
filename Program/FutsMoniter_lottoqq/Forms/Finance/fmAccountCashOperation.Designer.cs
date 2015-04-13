namespace FutsMoniter
{
    partial class fmAccountCashOperation
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
            this.ctReceivableBankList1 = new FutsMoniter.ctReceivableBankList();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.amount = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.cashoptype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cashoptype)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ctReceivableBankList1);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.amount);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.cashoptype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(246, 185);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // ctReceivableBankList1
            // 
            this.ctReceivableBankList1.BankSelected = 0;
            this.ctReceivableBankList1.Location = new System.Drawing.Point(26, 87);
            this.ctReceivableBankList1.Name = "ctReceivableBankList1";
            this.ctReceivableBankList1.Size = new System.Drawing.Size(173, 23);
            this.ctReceivableBankList1.TabIndex = 1;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(100, 9);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(97, 21);
            this.account.TabIndex = 8;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(164, 148);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(70, 25);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // amount
            // 
            this.amount.Location = new System.Drawing.Point(100, 61);
            this.amount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.amount.Name = "amount";
            this.amount.Size = new System.Drawing.Size(97, 20);
            this.amount.TabIndex = 5;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(53, 61);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "金额:";
            // 
            // cashoptype
            // 
            this.cashoptype.DropDownWidth = 97;
            this.cashoptype.Location = new System.Drawing.Point(100, 34);
            this.cashoptype.Name = "cashoptype";
            this.cashoptype.Size = new System.Drawing.Size(97, 21);
            this.cashoptype.TabIndex = 3;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(26, 37);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "请求类型:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(26, 13);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "交易帐号:";
            // 
            // fmAccountCashOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 185);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmAccountCashOperation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "提现或充值";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cashoptype)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cashoptype;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown amount;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox account;
        private ctReceivableBankList ctReceivableBankList1;
    }
}