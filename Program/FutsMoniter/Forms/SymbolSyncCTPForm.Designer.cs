namespace FutsMoniter
{
    partial class SymbolSyncCTPForm
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
            this.btnSyncSymbol = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.ctpaddress = new Telerik.WinControls.UI.RadTextBox();
            this.brokerid = new Telerik.WinControls.UI.RadTextBox();
            this.username = new Telerik.WinControls.UI.RadTextBox();
            this.password = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.status = new Telerik.WinControls.UI.RadLabel();
            this.defaulttradeable = new Telerik.WinControls.UI.RadCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnSyncSymbol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctpaddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokerid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.username)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.password)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.status)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaulttradeable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSyncSymbol
            // 
            this.btnSyncSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncSymbol.Location = new System.Drawing.Point(268, 187);
            this.btnSyncSymbol.Name = "btnSyncSymbol";
            this.btnSyncSymbol.Size = new System.Drawing.Size(98, 24);
            this.btnSyncSymbol.TabIndex = 0;
            this.btnSyncSymbol.Text = "同步合约";
            this.btnSyncSymbol.Click += new System.EventHandler(this.btnSyncSymbol_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(22, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(94, 16);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "CTP服务器地址:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(62, 35);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(54, 16);
            this.radLabel2.TabIndex = 2;
            this.radLabel2.Text = "BrokerID:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(69, 57);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(47, 16);
            this.radLabel3.TabIndex = 3;
            this.radLabel3.Text = "用户名:";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(82, 79);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(34, 16);
            this.radLabel4.TabIndex = 4;
            this.radLabel4.Text = "密码:";
            // 
            // ctpaddress
            // 
            this.ctpaddress.Location = new System.Drawing.Point(123, 8);
            this.ctpaddress.Name = "ctpaddress";
            this.ctpaddress.Size = new System.Drawing.Size(182, 18);
            this.ctpaddress.TabIndex = 5;
            this.ctpaddress.Text = "tcp://183.129.188.37:41205";
            // 
            // brokerid
            // 
            this.brokerid.Location = new System.Drawing.Point(123, 33);
            this.brokerid.Name = "brokerid";
            this.brokerid.Size = new System.Drawing.Size(182, 18);
            this.brokerid.TabIndex = 6;
            this.brokerid.Text = "1019";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(123, 55);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(182, 18);
            this.username.TabIndex = 7;
            this.username.Text = "00000025";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(123, 77);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(182, 18);
            this.password.TabIndex = 8;
            this.password.Text = "123456";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(22, 128);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(228, 16);
            this.radLabel5.TabIndex = 9;
            this.radLabel5.Text = "系统将自动从CTP交易接口获取合约列表";
            // 
            // radLabel6
            // 
            this.radLabel6.Location = new System.Drawing.Point(22, 150);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(59, 16);
            this.radLabel6.TabIndex = 10;
            this.radLabel6.Text = "当前状态:";
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(87, 150);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(14, 16);
            this.status.TabIndex = 11;
            this.status.Text = "--";
            // 
            // defaulttradeable
            // 
            this.defaulttradeable.Location = new System.Drawing.Point(123, 101);
            this.defaulttradeable.Name = "defaulttradeable";
            this.defaulttradeable.Size = new System.Drawing.Size(107, 16);
            this.defaulttradeable.TabIndex = 12;
            this.defaulttradeable.Text = "默认可交易";
            // 
            // SymbolSyncCTPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 214);
            this.Controls.Add(this.defaulttradeable);
            this.Controls.Add(this.status);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.brokerid);
            this.Controls.Add(this.ctpaddress);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btnSyncSymbol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SymbolSyncCTPForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "同步CTP期货合约";
            this.ThemeName = "ControlDefault";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SymbolSyncCTPForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.btnSyncSymbol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctpaddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokerid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.username)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.password)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.status)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaulttradeable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnSyncSymbol;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadTextBox ctpaddress;
        private Telerik.WinControls.UI.RadTextBox brokerid;
        private Telerik.WinControls.UI.RadTextBox username;
        private Telerik.WinControls.UI.RadTextBox password;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel status;
        private Telerik.WinControls.UI.RadCheckBox defaulttradeable;
    }
}
