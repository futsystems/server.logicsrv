namespace FutsMoniter
{
    partial class AddAccountForm
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
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.accountType = new Telerik.WinControls.UI.RadDropDownList();
            this.account = new Telerik.WinControls.UI.RadTextBox();
            this.password = new Telerik.WinControls.UI.RadTextBox();
            this.btnAddAccount = new Telerik.WinControls.UI.RadButton();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.password)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddAccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(13, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "帐号类别:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(36, 37);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 1;
            this.radLabel2.Text = "帐号:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(13, 61);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 2;
            this.radLabel3.Text = "交易密码:";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(13, 79);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(2, 2);
            this.radLabel4.TabIndex = 3;
            // 
            // accountType
            // 
            this.accountType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.accountType.Location = new System.Drawing.Point(78, 11);
            this.accountType.Name = "accountType";
            this.accountType.Size = new System.Drawing.Size(109, 18);
            this.accountType.TabIndex = 16;
            this.accountType.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.accountType.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(78, 35);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(109, 18);
            this.account.TabIndex = 17;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(78, 59);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(109, 18);
            this.password.TabIndex = 18;
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Location = new System.Drawing.Point(156, 149);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(110, 24);
            this.btnAddAccount.TabIndex = 19;
            this.btnAddAccount.Text = "添加交易帐号";
            this.btnAddAccount.Click += new System.EventHandler(this.btnAddAccount_Click);
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.Location = new System.Drawing.Point(25, 83);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(162, 20);
            this.ctAgentList1.TabIndex = 22;
            // 
            // AddAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 185);
            this.Controls.Add(this.ctAgentList1);
            this.Controls.Add(this.btnAddAccount);
            this.Controls.Add(this.password);
            this.Controls.Add(this.account);
            this.Controls.Add(this.accountType);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddAccountForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加交易帐号";
            this.ThemeName = "ControlDefault";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.password)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddAccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadDropDownList accountType;
        private Telerik.WinControls.UI.RadTextBox account;
        private Telerik.WinControls.UI.RadTextBox password;
        private Telerik.WinControls.UI.RadButton btnAddAccount;
        private ctAgentList ctAgentList1;
    }
}
