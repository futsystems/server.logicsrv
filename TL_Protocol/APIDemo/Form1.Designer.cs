namespace APIClient
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.exapiverbose = new System.Windows.Forms.CheckBox();
            this.btnExCancelOrder = new System.Windows.Forms.Button();
            this.btnExPlaceOrder = new System.Windows.Forms.Button();
            this.btnExQryMaxOrderVol = new System.Windows.Forms.Button();
            this.btnExQryTradingAccount = new System.Windows.Forms.Button();
            this.btnExQryPosition = new System.Windows.Forms.Button();
            this.btnExQryTrade = new System.Windows.Forms.Button();
            this.btnExQryOrder = new System.Windows.Forms.Button();
            this.btnExQrySymbol = new System.Windows.Forms.Button();
            this.btnExUpdatePass = new System.Windows.Forms.Button();
            this.exPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.exUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExLogin = new System.Windows.Forms.Button();
            this.btnStopEx = new System.Windows.Forms.Button();
            this.btnStartEx = new System.Windows.Forms.Button();
            this.exPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.exAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.wsAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnWSStart = new System.Windows.Forms.Button();
            this.debugControl1 = new APIClient.DebugControl();
            this.btnWSStop = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(969, 253);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.exapiverbose);
            this.tabPage1.Controls.Add(this.btnExCancelOrder);
            this.tabPage1.Controls.Add(this.btnExPlaceOrder);
            this.tabPage1.Controls.Add(this.btnExQryMaxOrderVol);
            this.tabPage1.Controls.Add(this.btnExQryTradingAccount);
            this.tabPage1.Controls.Add(this.btnExQryPosition);
            this.tabPage1.Controls.Add(this.btnExQryTrade);
            this.tabPage1.Controls.Add(this.btnExQryOrder);
            this.tabPage1.Controls.Add(this.btnExQrySymbol);
            this.tabPage1.Controls.Add(this.btnExUpdatePass);
            this.tabPage1.Controls.Add(this.exPass);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.exUser);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.btnExLogin);
            this.tabPage1.Controls.Add(this.btnStopEx);
            this.tabPage1.Controls.Add(this.btnStartEx);
            this.tabPage1.Controls.Add(this.exPort);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.exAddress);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(961, 227);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Socket/二进制";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // exapiverbose
            // 
            this.exapiverbose.AutoSize = true;
            this.exapiverbose.Location = new System.Drawing.Point(406, 41);
            this.exapiverbose.Name = "exapiverbose";
            this.exapiverbose.Size = new System.Drawing.Size(72, 16);
            this.exapiverbose.TabIndex = 20;
            this.exapiverbose.Text = "传输日志";
            this.exapiverbose.UseVisualStyleBackColor = true;
            // 
            // btnExCancelOrder
            // 
            this.btnExCancelOrder.Location = new System.Drawing.Point(146, 164);
            this.btnExCancelOrder.Name = "btnExCancelOrder";
            this.btnExCancelOrder.Size = new System.Drawing.Size(77, 23);
            this.btnExCancelOrder.TabIndex = 19;
            this.btnExCancelOrder.Text = "撤销委托";
            this.btnExCancelOrder.UseVisualStyleBackColor = true;
            // 
            // btnExPlaceOrder
            // 
            this.btnExPlaceOrder.Location = new System.Drawing.Point(146, 135);
            this.btnExPlaceOrder.Name = "btnExPlaceOrder";
            this.btnExPlaceOrder.Size = new System.Drawing.Size(77, 23);
            this.btnExPlaceOrder.TabIndex = 18;
            this.btnExPlaceOrder.Text = "提交委托";
            this.btnExPlaceOrder.UseVisualStyleBackColor = true;
            // 
            // btnExQryMaxOrderVol
            // 
            this.btnExQryMaxOrderVol.Location = new System.Drawing.Point(146, 106);
            this.btnExQryMaxOrderVol.Name = "btnExQryMaxOrderVol";
            this.btnExQryMaxOrderVol.Size = new System.Drawing.Size(111, 23);
            this.btnExQryMaxOrderVol.TabIndex = 17;
            this.btnExQryMaxOrderVol.Text = "查询最大报单数量";
            this.btnExQryMaxOrderVol.UseVisualStyleBackColor = true;
            // 
            // btnExQryTradingAccount
            // 
            this.btnExQryTradingAccount.Location = new System.Drawing.Point(148, 77);
            this.btnExQryTradingAccount.Name = "btnExQryTradingAccount";
            this.btnExQryTradingAccount.Size = new System.Drawing.Size(75, 23);
            this.btnExQryTradingAccount.TabIndex = 16;
            this.btnExQryTradingAccount.Text = "查询交易账户";
            this.btnExQryTradingAccount.UseVisualStyleBackColor = true;
            // 
            // btnExQryPosition
            // 
            this.btnExQryPosition.Location = new System.Drawing.Point(12, 193);
            this.btnExQryPosition.Name = "btnExQryPosition";
            this.btnExQryPosition.Size = new System.Drawing.Size(75, 23);
            this.btnExQryPosition.TabIndex = 15;
            this.btnExQryPosition.Text = "查询持仓";
            this.btnExQryPosition.UseVisualStyleBackColor = true;
            // 
            // btnExQryTrade
            // 
            this.btnExQryTrade.Location = new System.Drawing.Point(12, 164);
            this.btnExQryTrade.Name = "btnExQryTrade";
            this.btnExQryTrade.Size = new System.Drawing.Size(75, 23);
            this.btnExQryTrade.TabIndex = 14;
            this.btnExQryTrade.Text = "查询成交";
            this.btnExQryTrade.UseVisualStyleBackColor = true;
            // 
            // btnExQryOrder
            // 
            this.btnExQryOrder.Location = new System.Drawing.Point(12, 135);
            this.btnExQryOrder.Name = "btnExQryOrder";
            this.btnExQryOrder.Size = new System.Drawing.Size(75, 23);
            this.btnExQryOrder.TabIndex = 13;
            this.btnExQryOrder.Text = "查询委托";
            this.btnExQryOrder.UseVisualStyleBackColor = true;
            // 
            // btnExQrySymbol
            // 
            this.btnExQrySymbol.Location = new System.Drawing.Point(12, 106);
            this.btnExQrySymbol.Name = "btnExQrySymbol";
            this.btnExQrySymbol.Size = new System.Drawing.Size(75, 23);
            this.btnExQrySymbol.TabIndex = 12;
            this.btnExQrySymbol.Text = "查询合约";
            this.btnExQrySymbol.UseVisualStyleBackColor = true;
            // 
            // btnExUpdatePass
            // 
            this.btnExUpdatePass.Location = new System.Drawing.Point(12, 77);
            this.btnExUpdatePass.Name = "btnExUpdatePass";
            this.btnExUpdatePass.Size = new System.Drawing.Size(75, 23);
            this.btnExUpdatePass.TabIndex = 11;
            this.btnExUpdatePass.Text = "修改交易密码";
            this.btnExUpdatePass.UseVisualStyleBackColor = true;
            // 
            // exPass
            // 
            this.exPass.Location = new System.Drawing.Point(204, 34);
            this.exPass.Name = "exPass";
            this.exPass.Size = new System.Drawing.Size(53, 21);
            this.exPass.TabIndex = 10;
            this.exPass.Text = "123456";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(142, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Password:";
            // 
            // exUser
            // 
            this.exUser.Location = new System.Drawing.Point(59, 34);
            this.exUser.Name = "exUser";
            this.exUser.Size = new System.Drawing.Size(77, 21);
            this.exUser.TabIndex = 8;
            this.exUser.Text = "8500001";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "UserID:";
            // 
            // btnExLogin
            // 
            this.btnExLogin.Location = new System.Drawing.Point(308, 37);
            this.btnExLogin.Name = "btnExLogin";
            this.btnExLogin.Size = new System.Drawing.Size(75, 23);
            this.btnExLogin.TabIndex = 6;
            this.btnExLogin.Text = "登入";
            this.btnExLogin.UseVisualStyleBackColor = true;
            // 
            // btnStopEx
            // 
            this.btnStopEx.Location = new System.Drawing.Point(406, 5);
            this.btnStopEx.Name = "btnStopEx";
            this.btnStopEx.Size = new System.Drawing.Size(92, 23);
            this.btnStopEx.TabIndex = 5;
            this.btnStopEx.Text = "停止交易接口";
            this.btnStopEx.UseVisualStyleBackColor = true;
            // 
            // btnStartEx
            // 
            this.btnStartEx.Location = new System.Drawing.Point(308, 5);
            this.btnStartEx.Name = "btnStartEx";
            this.btnStartEx.Size = new System.Drawing.Size(92, 23);
            this.btnStartEx.TabIndex = 4;
            this.btnStartEx.Text = "启动交易接口";
            this.btnStartEx.UseVisualStyleBackColor = true;
            // 
            // exPort
            // 
            this.exPort.Location = new System.Drawing.Point(249, 7);
            this.exPort.Name = "exPort";
            this.exPort.Size = new System.Drawing.Size(53, 21);
            this.exPort.TabIndex = 3;
            this.exPort.Text = "41455";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口:";
            // 
            // exAddress
            // 
            this.exAddress.Location = new System.Drawing.Point(59, 7);
            this.exAddress.Name = "exAddress";
            this.exAddress.Size = new System.Drawing.Size(133, 21);
            this.exAddress.TabIndex = 1;
            this.exAddress.Text = "121.40.201.40";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "地址:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnWSStop);
            this.tabPage2.Controls.Add(this.btnWSStart);
            this.tabPage2.Controls.Add(this.wsAddress);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(961, 227);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "WebSocket/Json";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // wsAddress
            // 
            this.wsAddress.Location = new System.Drawing.Point(37, 9);
            this.wsAddress.Name = "wsAddress";
            this.wsAddress.Size = new System.Drawing.Size(169, 21);
            this.wsAddress.TabIndex = 3;
            this.wsAddress.Text = "ws://127.0.0.1:41655/";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "URL";
            // 
            // btnWSStart
            // 
            this.btnWSStart.Location = new System.Drawing.Point(212, 7);
            this.btnWSStart.Name = "btnWSStart";
            this.btnWSStart.Size = new System.Drawing.Size(92, 23);
            this.btnWSStart.TabIndex = 5;
            this.btnWSStart.Text = "启动交易接口";
            this.btnWSStart.UseVisualStyleBackColor = true;
            // 
            // debugControl1
            // 
            this.debugControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugControl1.EnableSearching = true;
            this.debugControl1.ExternalTimeStamp = 0;
            this.debugControl1.Location = new System.Drawing.Point(0, 254);
            this.debugControl1.Margin = new System.Windows.Forms.Padding(2);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(969, 291);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // btnWSStop
            // 
            this.btnWSStop.Location = new System.Drawing.Point(310, 7);
            this.btnWSStop.Name = "btnWSStop";
            this.btnWSStop.Size = new System.Drawing.Size(92, 23);
            this.btnWSStop.TabIndex = 6;
            this.btnWSStop.Text = "停止交易接口";
            this.btnWSStop.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 546);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "API客户端";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DebugControl debugControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox exPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox exAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartEx;
        private System.Windows.Forms.Button btnStopEx;
        private System.Windows.Forms.Button btnExLogin;
        private System.Windows.Forms.TextBox exPass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox exUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExUpdatePass;
        private System.Windows.Forms.Button btnExQrySymbol;
        private System.Windows.Forms.Button btnExQryOrder;
        private System.Windows.Forms.Button btnExQryTrade;
        private System.Windows.Forms.Button btnExQryPosition;
        private System.Windows.Forms.Button btnExQryTradingAccount;
        private System.Windows.Forms.Button btnExQryMaxOrderVol;
        private System.Windows.Forms.Button btnExPlaceOrder;
        private System.Windows.Forms.Button btnExCancelOrder;
        private System.Windows.Forms.CheckBox exapiverbose;
        private System.Windows.Forms.TextBox wsAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnWSStart;
        private System.Windows.Forms.Button btnWSStop;
    }
}

