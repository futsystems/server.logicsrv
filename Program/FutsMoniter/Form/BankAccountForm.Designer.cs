namespace FutsMoniter
{
    partial class BankAccountForm
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
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.lbmgrid = new Telerik.WinControls.UI.RadLabel();
            this.name = new Telerik.WinControls.UI.RadTextBox();
            this.bankac = new Telerik.WinControls.UI.RadTextBox();
            this.branch = new Telerik.WinControls.UI.RadTextBox();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.ctBankList1 = new FutsMoniter.ctBankList();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbmgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bankac)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.branch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(25, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(70, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "管理主域ID:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(61, 35);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 1;
            this.radLabel2.Text = "姓名:";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(61, 87);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(34, 16);
            this.radLabel4.TabIndex = 3;
            this.radLabel4.Text = "帐号:";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(36, 114);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(59, 16);
            this.radLabel5.TabIndex = 4;
            this.radLabel5.Text = "开户支行:";
            // 
            // lbmgrid
            // 
            this.lbmgrid.Location = new System.Drawing.Point(101, 13);
            this.lbmgrid.Name = "lbmgrid";
            this.lbmgrid.Size = new System.Drawing.Size(14, 16);
            this.lbmgrid.TabIndex = 5;
            this.lbmgrid.Text = "--";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(101, 35);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(110, 18);
            this.name.TabIndex = 6;
            // 
            // bankac
            // 
            this.bankac.Location = new System.Drawing.Point(101, 85);
            this.bankac.Name = "bankac";
            this.bankac.Size = new System.Drawing.Size(196, 18);
            this.bankac.TabIndex = 8;
            // 
            // branch
            // 
            this.branch.Location = new System.Drawing.Point(101, 112);
            this.branch.Name = "branch";
            this.branch.Size = new System.Drawing.Size(196, 18);
            this.branch.TabIndex = 9;
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(219, 155);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(78, 24);
            this.radButton1.TabIndex = 10;
            this.radButton1.Text = "提 交";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // ctBankList1
            // 
            this.ctBankList1.Location = new System.Drawing.Point(61, 59);
            this.ctBankList1.Name = "ctBankList1";
            this.ctBankList1.Size = new System.Drawing.Size(150, 20);
            this.ctBankList1.TabIndex = 7;
            // 
            // BankAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 183);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.branch);
            this.Controls.Add(this.bankac);
            this.Controls.Add(this.ctBankList1);
            this.Controls.Add(this.name);
            this.Controls.Add(this.lbmgrid);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BankAccountForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改收款银行卡信息";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbmgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bankac)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.branch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel lbmgrid;
        private Telerik.WinControls.UI.RadTextBox name;
        private ctBankList ctBankList1;
        private Telerik.WinControls.UI.RadTextBox bankac;
        private Telerik.WinControls.UI.RadTextBox branch;
        private Telerik.WinControls.UI.RadButton radButton1;
    }
}