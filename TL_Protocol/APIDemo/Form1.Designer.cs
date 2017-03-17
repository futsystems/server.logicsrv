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
            this.wsBtnCancel = new System.Windows.Forms.Button();
            this.wsBtnPlaceOrder = new System.Windows.Forms.Button();
            this.wsBtnQryMaxOrderVol = new System.Windows.Forms.Button();
            this.wsBtnQryAccount = new System.Windows.Forms.Button();
            this.wsBtnQryPos = new System.Windows.Forms.Button();
            this.wsBtnQryTrade = new System.Windows.Forms.Button();
            this.wsBtnQryOrder = new System.Windows.Forms.Button();
            this.wsBtnQrySymbol = new System.Windows.Forms.Button();
            this.wsBtnUpdatePass = new System.Windows.Forms.Button();
            this.wsPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.wsUser = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.wsBtnLogin = new System.Windows.Forms.Button();
            this.btnWSStop = new System.Windows.Forms.Button();
            this.btnWSStart = new System.Windows.Forms.Button();
            this.wsAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.mdSymbols = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnMdSubMarket = new System.Windows.Forms.Button();
            this.mdPass = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mdUser = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnMdLogin = new System.Windows.Forms.Button();
            this.btnStopMd = new System.Windows.Forms.Button();
            this.btnStartMd = new System.Windows.Forms.Button();
            this.mdPort = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mdAddress = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.debugControl1 = new APIClient.DebugControl();
            this.btnMdQrySymbol = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
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
            this.tabPage2.Controls.Add(this.wsBtnCancel);
            this.tabPage2.Controls.Add(this.wsBtnPlaceOrder);
            this.tabPage2.Controls.Add(this.wsBtnQryMaxOrderVol);
            this.tabPage2.Controls.Add(this.wsBtnQryAccount);
            this.tabPage2.Controls.Add(this.wsBtnQryPos);
            this.tabPage2.Controls.Add(this.wsBtnQryTrade);
            this.tabPage2.Controls.Add(this.wsBtnQryOrder);
            this.tabPage2.Controls.Add(this.wsBtnQrySymbol);
            this.tabPage2.Controls.Add(this.wsBtnUpdatePass);
            this.tabPage2.Controls.Add(this.wsPassword);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.wsUser);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.wsBtnLogin);
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
            // wsBtnCancel
            // 
            this.wsBtnCancel.Location = new System.Drawing.Point(146, 164);
            this.wsBtnCancel.Name = "wsBtnCancel";
            this.wsBtnCancel.Size = new System.Drawing.Size(77, 23);
            this.wsBtnCancel.TabIndex = 28;
            this.wsBtnCancel.Text = "撤销委托";
            this.wsBtnCancel.UseVisualStyleBackColor = true;
            // 
            // wsBtnPlaceOrder
            // 
            this.wsBtnPlaceOrder.Location = new System.Drawing.Point(146, 135);
            this.wsBtnPlaceOrder.Name = "wsBtnPlaceOrder";
            this.wsBtnPlaceOrder.Size = new System.Drawing.Size(77, 23);
            this.wsBtnPlaceOrder.TabIndex = 27;
            this.wsBtnPlaceOrder.Text = "提交委托";
            this.wsBtnPlaceOrder.UseVisualStyleBackColor = true;
            // 
            // wsBtnQryMaxOrderVol
            // 
            this.wsBtnQryMaxOrderVol.Location = new System.Drawing.Point(146, 106);
            this.wsBtnQryMaxOrderVol.Name = "wsBtnQryMaxOrderVol";
            this.wsBtnQryMaxOrderVol.Size = new System.Drawing.Size(111, 23);
            this.wsBtnQryMaxOrderVol.TabIndex = 26;
            this.wsBtnQryMaxOrderVol.Text = "查询最大报单数量";
            this.wsBtnQryMaxOrderVol.UseVisualStyleBackColor = true;
            // 
            // wsBtnQryAccount
            // 
            this.wsBtnQryAccount.Location = new System.Drawing.Point(148, 77);
            this.wsBtnQryAccount.Name = "wsBtnQryAccount";
            this.wsBtnQryAccount.Size = new System.Drawing.Size(75, 23);
            this.wsBtnQryAccount.TabIndex = 25;
            this.wsBtnQryAccount.Text = "查询交易账户";
            this.wsBtnQryAccount.UseVisualStyleBackColor = true;
            // 
            // wsBtnQryPos
            // 
            this.wsBtnQryPos.Location = new System.Drawing.Point(12, 193);
            this.wsBtnQryPos.Name = "wsBtnQryPos";
            this.wsBtnQryPos.Size = new System.Drawing.Size(75, 23);
            this.wsBtnQryPos.TabIndex = 24;
            this.wsBtnQryPos.Text = "查询持仓";
            this.wsBtnQryPos.UseVisualStyleBackColor = true;
            // 
            // wsBtnQryTrade
            // 
            this.wsBtnQryTrade.Location = new System.Drawing.Point(12, 164);
            this.wsBtnQryTrade.Name = "wsBtnQryTrade";
            this.wsBtnQryTrade.Size = new System.Drawing.Size(75, 23);
            this.wsBtnQryTrade.TabIndex = 23;
            this.wsBtnQryTrade.Text = "查询成交";
            this.wsBtnQryTrade.UseVisualStyleBackColor = true;
            // 
            // wsBtnQryOrder
            // 
            this.wsBtnQryOrder.Location = new System.Drawing.Point(12, 135);
            this.wsBtnQryOrder.Name = "wsBtnQryOrder";
            this.wsBtnQryOrder.Size = new System.Drawing.Size(75, 23);
            this.wsBtnQryOrder.TabIndex = 22;
            this.wsBtnQryOrder.Text = "查询委托";
            this.wsBtnQryOrder.UseVisualStyleBackColor = true;
            // 
            // wsBtnQrySymbol
            // 
            this.wsBtnQrySymbol.Location = new System.Drawing.Point(12, 106);
            this.wsBtnQrySymbol.Name = "wsBtnQrySymbol";
            this.wsBtnQrySymbol.Size = new System.Drawing.Size(75, 23);
            this.wsBtnQrySymbol.TabIndex = 21;
            this.wsBtnQrySymbol.Text = "查询合约";
            this.wsBtnQrySymbol.UseVisualStyleBackColor = true;
            // 
            // wsBtnUpdatePass
            // 
            this.wsBtnUpdatePass.Location = new System.Drawing.Point(12, 77);
            this.wsBtnUpdatePass.Name = "wsBtnUpdatePass";
            this.wsBtnUpdatePass.Size = new System.Drawing.Size(75, 23);
            this.wsBtnUpdatePass.TabIndex = 20;
            this.wsBtnUpdatePass.Text = "修改交易密码";
            this.wsBtnUpdatePass.UseVisualStyleBackColor = true;
            // 
            // wsPassword
            // 
            this.wsPassword.Location = new System.Drawing.Point(200, 40);
            this.wsPassword.Name = "wsPassword";
            this.wsPassword.Size = new System.Drawing.Size(53, 21);
            this.wsPassword.TabIndex = 15;
            this.wsPassword.Text = "123456";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(138, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "Password:";
            // 
            // wsUser
            // 
            this.wsUser.Location = new System.Drawing.Point(55, 40);
            this.wsUser.Name = "wsUser";
            this.wsUser.Size = new System.Drawing.Size(77, 21);
            this.wsUser.TabIndex = 13;
            this.wsUser.Text = "8500001";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "UserID:";
            // 
            // wsBtnLogin
            // 
            this.wsBtnLogin.Location = new System.Drawing.Point(310, 38);
            this.wsBtnLogin.Name = "wsBtnLogin";
            this.wsBtnLogin.Size = new System.Drawing.Size(75, 23);
            this.wsBtnLogin.TabIndex = 11;
            this.wsBtnLogin.Text = "登入";
            this.wsBtnLogin.UseVisualStyleBackColor = true;
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
            // btnWSStart
            // 
            this.btnWSStart.Location = new System.Drawing.Point(212, 7);
            this.btnWSStart.Name = "btnWSStart";
            this.btnWSStart.Size = new System.Drawing.Size(92, 23);
            this.btnWSStart.TabIndex = 5;
            this.btnWSStart.Text = "启动交易接口";
            this.btnWSStart.UseVisualStyleBackColor = true;
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnMdQrySymbol);
            this.tabPage3.Controls.Add(this.mdSymbols);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.btnMdSubMarket);
            this.tabPage3.Controls.Add(this.mdPass);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.mdUser);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.btnMdLogin);
            this.tabPage3.Controls.Add(this.btnStopMd);
            this.tabPage3.Controls.Add(this.btnStartMd);
            this.tabPage3.Controls.Add(this.mdPort);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.mdAddress);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(961, 227);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "行情Socket";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // mdSymbols
            // 
            this.mdSymbols.Location = new System.Drawing.Point(59, 63);
            this.mdSymbols.Name = "mdSymbols";
            this.mdSymbols.Size = new System.Drawing.Size(198, 21);
            this.mdSymbols.TabIndex = 24;
            this.mdSymbols.Text = "HSIF7,CLG7";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 12);
            this.label12.TabIndex = 23;
            this.label12.Text = "合约:";
            // 
            // btnMdSubMarket
            // 
            this.btnMdSubMarket.Location = new System.Drawing.Point(308, 66);
            this.btnMdSubMarket.Name = "btnMdSubMarket";
            this.btnMdSubMarket.Size = new System.Drawing.Size(75, 23);
            this.btnMdSubMarket.TabIndex = 22;
            this.btnMdSubMarket.Text = "注册行情";
            this.btnMdSubMarket.UseVisualStyleBackColor = true;
            // 
            // mdPass
            // 
            this.mdPass.Location = new System.Drawing.Point(204, 34);
            this.mdPass.Name = "mdPass";
            this.mdPass.Size = new System.Drawing.Size(53, 21);
            this.mdPass.TabIndex = 21;
            this.mdPass.Text = "123456";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(142, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "Password:";
            // 
            // mdUser
            // 
            this.mdUser.Location = new System.Drawing.Point(59, 34);
            this.mdUser.Name = "mdUser";
            this.mdUser.Size = new System.Drawing.Size(77, 21);
            this.mdUser.TabIndex = 19;
            this.mdUser.Text = "8500001";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "UserID:";
            // 
            // btnMdLogin
            // 
            this.btnMdLogin.Location = new System.Drawing.Point(308, 37);
            this.btnMdLogin.Name = "btnMdLogin";
            this.btnMdLogin.Size = new System.Drawing.Size(75, 23);
            this.btnMdLogin.TabIndex = 17;
            this.btnMdLogin.Text = "登入";
            this.btnMdLogin.UseVisualStyleBackColor = true;
            // 
            // btnStopMd
            // 
            this.btnStopMd.Location = new System.Drawing.Point(406, 5);
            this.btnStopMd.Name = "btnStopMd";
            this.btnStopMd.Size = new System.Drawing.Size(92, 23);
            this.btnStopMd.TabIndex = 16;
            this.btnStopMd.Text = "停止行情接口";
            this.btnStopMd.UseVisualStyleBackColor = true;
            // 
            // btnStartMd
            // 
            this.btnStartMd.Location = new System.Drawing.Point(308, 5);
            this.btnStartMd.Name = "btnStartMd";
            this.btnStartMd.Size = new System.Drawing.Size(92, 23);
            this.btnStartMd.TabIndex = 15;
            this.btnStartMd.Text = "启动行情接口";
            this.btnStartMd.UseVisualStyleBackColor = true;
            // 
            // mdPort
            // 
            this.mdPort.Location = new System.Drawing.Point(249, 7);
            this.mdPort.Name = "mdPort";
            this.mdPort.Size = new System.Drawing.Size(53, 21);
            this.mdPort.TabIndex = 14;
            this.mdPort.Text = "55633";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(202, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 13;
            this.label10.Text = "端口:";
            // 
            // mdAddress
            // 
            this.mdAddress.Location = new System.Drawing.Point(59, 7);
            this.mdAddress.Name = "mdAddress";
            this.mdAddress.Size = new System.Drawing.Size(133, 21);
            this.mdAddress.TabIndex = 12;
            this.mdAddress.Text = "127.0.0.1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "地址:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(869, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(961, 227);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
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
            // btnMdQrySymbol
            // 
            this.btnMdQrySymbol.Location = new System.Drawing.Point(406, 37);
            this.btnMdQrySymbol.Name = "btnMdQrySymbol";
            this.btnMdQrySymbol.Size = new System.Drawing.Size(75, 23);
            this.btnMdQrySymbol.TabIndex = 25;
            this.btnMdQrySymbol.Text = "查询合约";
            this.btnMdQrySymbol.UseVisualStyleBackColor = true;
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
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
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
        private System.Windows.Forms.TextBox wsPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox wsUser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button wsBtnLogin;
        private System.Windows.Forms.Button wsBtnCancel;
        private System.Windows.Forms.Button wsBtnPlaceOrder;
        private System.Windows.Forms.Button wsBtnQryMaxOrderVol;
        private System.Windows.Forms.Button wsBtnQryAccount;
        private System.Windows.Forms.Button wsBtnQryPos;
        private System.Windows.Forms.Button wsBtnQryTrade;
        private System.Windows.Forms.Button wsBtnQryOrder;
        private System.Windows.Forms.Button wsBtnQrySymbol;
        private System.Windows.Forms.Button wsBtnUpdatePass;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox mdPass;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox mdUser;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnMdLogin;
        private System.Windows.Forms.Button btnStopMd;
        private System.Windows.Forms.Button btnStartMd;
        private System.Windows.Forms.TextBox mdPort;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox mdAddress;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox mdSymbols;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnMdSubMarket;
        private System.Windows.Forms.Button btnMdQrySymbol;
    }
}

