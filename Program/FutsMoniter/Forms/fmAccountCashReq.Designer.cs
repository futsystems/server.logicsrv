namespace FutsMoniter
{
    partial class fmAccountCashReq
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmAccountCashReq));
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnAccountCashReq = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ctCashOperationAccount = new FutsMoniter.ctCashOperation();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.btnAccountCashReq);
            this.kryptonPanel1.Controls.Add(this.ctCashOperationAccount);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(752, 451);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnAccountCashReq
            // 
            this.btnAccountCashReq.Location = new System.Drawing.Point(641, 12);
            this.btnAccountCashReq.Name = "btnAccountCashReq";
            this.btnAccountCashReq.Size = new System.Drawing.Size(99, 25);
            this.btnAccountCashReq.TabIndex = 4;
            this.btnAccountCashReq.Values.Text = "提交出入金";
            // 
            // ctCashOperationAccount
            // 
            this.ctCashOperationAccount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ctCashOperationAccount.Location = new System.Drawing.Point(0, 47);
            this.ctCashOperationAccount.Name = "ctCashOperationAccount";
            this.ctCashOperationAccount.Size = new System.Drawing.Size(752, 404);
            this.ctCashOperationAccount.TabIndex = 0;
            this.ctCashOperationAccount.ViewType = FutsMoniter.CashOpViewType.Account;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(13, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(330, 18);
            this.kryptonLabel1.TabIndex = 5;
            this.kryptonLabel1.Values.Text = "交易帐户出入金请求提交后,请联系相关财务人员确认!";
            // 
            // fmAccountCashReq
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 451);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmAccountCashReq";
            this.Text = "交易帐户出入金";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ctCashOperation ctCashOperationAccount;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAccountCashReq;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}