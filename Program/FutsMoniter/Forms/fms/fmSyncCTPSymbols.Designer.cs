//namespace FutsMoniter
//{
//    partial class fmSyncCTPSymbols
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
//            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.ctpaddress = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
//            this.brokerid = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
//            this.username = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
//            this.password = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
//            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.defaulttradeable = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
//            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.status = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
//            this.btnSyncSymbol = new ComponentFactory.Krypton.Toolkit.KryptonButton();
//            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
//            this.kryptonPanel1.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // kryptonPanel1
//            // 
//            this.kryptonPanel1.Controls.Add(this.btnSyncSymbol);
//            this.kryptonPanel1.Controls.Add(this.status);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
//            this.kryptonPanel1.Controls.Add(this.defaulttradeable);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
//            this.kryptonPanel1.Controls.Add(this.password);
//            this.kryptonPanel1.Controls.Add(this.username);
//            this.kryptonPanel1.Controls.Add(this.brokerid);
//            this.kryptonPanel1.Controls.Add(this.ctpaddress);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
//            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
//            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
//            this.kryptonPanel1.Name = "kryptonPanel1";
//            this.kryptonPanel1.Size = new System.Drawing.Size(412, 221);
//            this.kryptonPanel1.TabIndex = 0;
//            // 
//            // kryptonLabel1
//            // 
//            this.kryptonLabel1.Location = new System.Drawing.Point(24, 13);
//            this.kryptonLabel1.Name = "kryptonLabel1";
//            this.kryptonLabel1.Size = new System.Drawing.Size(107, 18);
//            this.kryptonLabel1.TabIndex = 0;
//            this.kryptonLabel1.Values.Text = "CTP服务器地址:";
//            // 
//            // kryptonLabel2
//            // 
//            this.kryptonLabel2.Location = new System.Drawing.Point(67, 41);
//            this.kryptonLabel2.Name = "kryptonLabel2";
//            this.kryptonLabel2.Size = new System.Drawing.Size(63, 18);
//            this.kryptonLabel2.TabIndex = 1;
//            this.kryptonLabel2.Values.Text = "BrokerID:";
//            // 
//            // ctpaddress
//            // 
//            this.ctpaddress.Location = new System.Drawing.Point(137, 9);
//            this.ctpaddress.Name = "ctpaddress";
//            this.ctpaddress.Size = new System.Drawing.Size(230, 22);
//            this.ctpaddress.TabIndex = 2;
//            this.ctpaddress.Text = "tcp://183.129.188.37:41205";
//            // 
//            // brokerid
//            // 
//            this.brokerid.Location = new System.Drawing.Point(137, 37);
//            this.brokerid.Name = "brokerid";
//            this.brokerid.Size = new System.Drawing.Size(230, 22);
//            this.brokerid.TabIndex = 3;
//            this.brokerid.Text = "1019";
//            // 
//            // username
//            // 
//            this.username.Location = new System.Drawing.Point(137, 65);
//            this.username.Name = "username";
//            this.username.Size = new System.Drawing.Size(230, 22);
//            this.username.TabIndex = 4;
//            this.username.Text = "00000025";
//            // 
//            // password
//            // 
//            this.password.Location = new System.Drawing.Point(137, 93);
//            this.password.Name = "password";
//            this.password.Size = new System.Drawing.Size(230, 22);
//            this.password.TabIndex = 5;
//            this.password.Text = "123456";
//            // 
//            // kryptonLabel3
//            // 
//            this.kryptonLabel3.Location = new System.Drawing.Point(75, 69);
//            this.kryptonLabel3.Name = "kryptonLabel3";
//            this.kryptonLabel3.Size = new System.Drawing.Size(55, 18);
//            this.kryptonLabel3.TabIndex = 6;
//            this.kryptonLabel3.Values.Text = "用户名:";
//            // 
//            // kryptonLabel4
//            // 
//            this.kryptonLabel4.Location = new System.Drawing.Point(89, 97);
//            this.kryptonLabel4.Name = "kryptonLabel4";
//            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
//            this.kryptonLabel4.TabIndex = 7;
//            this.kryptonLabel4.Values.Text = "密码:";
//            // 
//            // defaulttradeable
//            // 
//            this.defaulttradeable.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
//            this.defaulttradeable.Location = new System.Drawing.Point(137, 122);
//            this.defaulttradeable.Name = "defaulttradeable";
//            this.defaulttradeable.Size = new System.Drawing.Size(91, 18);
//            this.defaulttradeable.TabIndex = 8;
//            this.defaulttradeable.Text = "默认可交易";
//            this.defaulttradeable.Values.Text = "默认可交易";
//            // 
//            // kryptonLabel5
//            // 
//            this.kryptonLabel5.Location = new System.Drawing.Point(24, 149);
//            this.kryptonLabel5.Name = "kryptonLabel5";
//            this.kryptonLabel5.Size = new System.Drawing.Size(253, 18);
//            this.kryptonLabel5.TabIndex = 9;
//            this.kryptonLabel5.Values.Text = "系统将自动从CTP交易接口获取合约列表";
//            // 
//            // kryptonLabel6
//            // 
//            this.kryptonLabel6.Location = new System.Drawing.Point(24, 173);
//            this.kryptonLabel6.Name = "kryptonLabel6";
//            this.kryptonLabel6.Size = new System.Drawing.Size(41, 18);
//            this.kryptonLabel6.TabIndex = 10;
//            this.kryptonLabel6.Values.Text = "状态:";
//            // 
//            // status
//            // 
//            this.status.Location = new System.Drawing.Point(67, 173);
//            this.status.Name = "status";
//            this.status.Size = new System.Drawing.Size(19, 18);
//            this.status.TabIndex = 11;
//            this.status.Values.Text = "--";
//            // 
//            // btnSyncSymbol
//            // 
//            this.btnSyncSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.btnSyncSymbol.Location = new System.Drawing.Point(310, 184);
//            this.btnSyncSymbol.Name = "btnSyncSymbol";
//            this.btnSyncSymbol.Size = new System.Drawing.Size(90, 25);
//            this.btnSyncSymbol.TabIndex = 12;
//            this.btnSyncSymbol.Values.Text = "同步合约";
//            //this.btnSyncSymbol.Click += new System.EventHandler(this.btnSubmit_Click);
//            // 
//            // fmSyncCTPSymbols
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(412, 221);
//            this.Controls.Add(this.kryptonPanel1);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
//            this.MaximizeBox = false;
//            this.MinimizeBox = false;
//            this.Name = "fmSyncCTPSymbols";
//            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
//            this.Text = "同步CTP期货合约";
//            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
//            this.kryptonPanel1.ResumeLayout(false);
//            this.kryptonPanel1.PerformLayout();
//            this.ResumeLayout(false);

//        }

//        #endregion

//        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
//        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox defaulttradeable;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
//        private ComponentFactory.Krypton.Toolkit.KryptonTextBox password;
//        private ComponentFactory.Krypton.Toolkit.KryptonTextBox username;
//        private ComponentFactory.Krypton.Toolkit.KryptonTextBox brokerid;
//        private ComponentFactory.Krypton.Toolkit.KryptonTextBox ctpaddress;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel status;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
//        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
//        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSyncSymbol;
//    }
//}