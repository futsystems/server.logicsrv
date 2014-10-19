namespace FutsMoniter
{
    partial class ChangeInvestorForm
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.account = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.name = new Telerik.WinControls.UI.RadTextBox();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            this.broker = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.bankac = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.ctBankList1 = new FutsMoniter.ctBankList();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.broker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bankac)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 12);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "交易账号:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(77, 12);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(14, 16);
            this.account.TabIndex = 1;
            this.account.Text = "--";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(37, 41);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 2;
            this.radLabel2.Text = "姓名:";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(77, 39);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(124, 18);
            this.name.TabIndex = 3;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(106, 157);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(110, 24);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "提交修改";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // broker
            // 
            this.broker.Location = new System.Drawing.Point(77, 70);
            this.broker.Name = "broker";
            this.broker.Size = new System.Drawing.Size(124, 18);
            this.broker.TabIndex = 6;
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(12, 72);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 5;
            this.radLabel3.Text = "期货公司:";
            // 
            // bankac
            // 
            this.bankac.Location = new System.Drawing.Point(77, 124);
            this.bankac.Name = "bankac";
            this.bankac.Size = new System.Drawing.Size(124, 18);
            this.bankac.TabIndex = 8;
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(12, 126);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(59, 16);
            this.radLabel4.TabIndex = 7;
            this.radLabel4.Text = "银行帐号:";
            // 
            // ctBankList1
            // 
            this.ctBankList1.Location = new System.Drawing.Point(37, 98);
            this.ctBankList1.Name = "ctBankList1";
            this.ctBankList1.Size = new System.Drawing.Size(150, 20);
            this.ctBankList1.TabIndex = 9;
            // 
            // ChangeInvestorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 196);
            this.Controls.Add(this.ctBankList1);
            this.Controls.Add(this.bankac);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.broker);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.name);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.account);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeInvestorForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改投资者信息";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.broker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bankac)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel account;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox name;
        private Telerik.WinControls.UI.RadButton btnSubmit;
        private Telerik.WinControls.UI.RadTextBox broker;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadTextBox bankac;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private ctBankList ctBankList1;
    }
}