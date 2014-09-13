namespace APIDemo
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
            this.btnBrokerNameRequest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.IPAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Port = new System.Windows.Forms.TextBox();
            this.btnBrokerName = new System.Windows.Forms.Button();
            this.btnVersion = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            this.convertnum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStatClient = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.passwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.loginid = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.currentaccount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.sendorder_symbol = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnBuy = new System.Windows.Forms.Button();
            this.sendorder_size = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.sendorder_price = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSell = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.orderaction_orderid = new System.Windows.Forms.TextBox();
            this.btnCancelorder = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.orderaction_orderref = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.orderaction_exchid = new System.Windows.Forms.TextBox();
            this.orderaction_sysid = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.qryorder_orderid = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnQryOrder = new System.Windows.Forms.Button();
            this.qryorder_symbol = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.symlist = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnDemo = new System.Windows.Forms.Button();
            this.btnQryInvestor = new System.Windows.Forms.Button();
            this.btnQryAcc = new System.Windows.Forms.Button();
            this.qrymaxvol_symbol = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnQryMaxVol = new System.Windows.Forms.Button();
            this.qrypos_symbol = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnQryPosition = new System.Windows.Forms.Button();
            this.btnQryTrade = new System.Windows.Forms.Button();
            this.qrytrade_symbol = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.ctDebug1 = new APIDemo.DebugControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrokerNameRequest
            // 
            this.btnBrokerNameRequest.Location = new System.Drawing.Point(268, 664);
            this.btnBrokerNameRequest.Name = "btnBrokerNameRequest";
            this.btnBrokerNameRequest.Size = new System.Drawing.Size(108, 23);
            this.btnBrokerNameRequest.TabIndex = 1;
            this.btnBrokerNameRequest.Text = "ReqBrokerName";
            this.btnBrokerNameRequest.UseVisualStyleBackColor = true;
            this.btnBrokerNameRequest.Click += new System.EventHandler(this.btnBrokerNameRequest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "IPAddress";
            // 
            // IPAddress
            // 
            this.IPAddress.Location = new System.Drawing.Point(83, 16);
            this.IPAddress.Name = "IPAddress";
            this.IPAddress.Size = new System.Drawing.Size(100, 21);
            this.IPAddress.TabIndex = 3;
            this.IPAddress.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(224, 17);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(75, 21);
            this.Port.TabIndex = 5;
            this.Port.Text = "5570";
            // 
            // btnBrokerName
            // 
            this.btnBrokerName.Location = new System.Drawing.Point(226, 707);
            this.btnBrokerName.Name = "btnBrokerName";
            this.btnBrokerName.Size = new System.Drawing.Size(128, 23);
            this.btnBrokerName.TabIndex = 6;
            this.btnBrokerName.Text = "Packet-BrokerName";
            this.btnBrokerName.UseVisualStyleBackColor = true;
            this.btnBrokerName.Click += new System.EventHandler(this.btnBrokerName_Click);
            // 
            // btnVersion
            // 
            this.btnVersion.Location = new System.Drawing.Point(11, 637);
            this.btnVersion.Name = "btnVersion";
            this.btnVersion.Size = new System.Drawing.Size(128, 23);
            this.btnVersion.TabIndex = 7;
            this.btnVersion.Text = "Packet-Version";
            this.btnVersion.UseVisualStyleBackColor = true;
            this.btnVersion.Click += new System.EventHandler(this.btnVersion_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(83, 707);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(128, 23);
            this.btnConvert.TabIndex = 8;
            this.btnConvert.Text = "Packet-Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // convertnum
            // 
            this.convertnum.Location = new System.Drawing.Point(83, 666);
            this.convertnum.Name = "convertnum";
            this.convertnum.Size = new System.Drawing.Size(142, 21);
            this.convertnum.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 675);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "循环次数";
            // 
            // btnStatClient
            // 
            this.btnStatClient.Location = new System.Drawing.Point(19, 43);
            this.btnStatClient.Name = "btnStatClient";
            this.btnStatClient.Size = new System.Drawing.Size(128, 39);
            this.btnStatClient.TabIndex = 11;
            this.btnStatClient.Text = "启动TLClient";
            this.btnStatClient.UseVisualStyleBackColor = true;
            this.btnStatClient.Click += new System.EventHandler(this.btnStatClient_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(387, 89);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(69, 23);
            this.btnLogin.TabIndex = 12;
            this.btnLogin.Text = "login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // passwd
            // 
            this.passwd.Location = new System.Drawing.Point(224, 92);
            this.passwd.Name = "passwd";
            this.passwd.Size = new System.Drawing.Size(75, 21);
            this.passwd.TabIndex = 16;
            this.passwd.Text = "123456";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "密码";
            // 
            // loginid
            // 
            this.loginid.Location = new System.Drawing.Point(76, 91);
            this.loginid.Name = "loginid";
            this.loginid.Size = new System.Drawing.Size(100, 21);
            this.loginid.TabIndex = 14;
            this.loginid.Text = "5880002";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "登入名";
            // 
            // currentaccount
            // 
            this.currentaccount.AutoSize = true;
            this.currentaccount.Location = new System.Drawing.Point(241, 55);
            this.currentaccount.Name = "currentaccount";
            this.currentaccount.Size = new System.Drawing.Size(17, 12);
            this.currentaccount.TabIndex = 19;
            this.currentaccount.Text = "--";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "当前交易帐号";
            // 
            // sendorder_symbol
            // 
            this.sendorder_symbol.Location = new System.Drawing.Point(47, 181);
            this.sendorder_symbol.Name = "sendorder_symbol";
            this.sendorder_symbol.Size = new System.Drawing.Size(71, 21);
            this.sendorder_symbol.TabIndex = 27;
            this.sendorder_symbol.Text = "IF1408";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 190);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "合约";
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(387, 164);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(69, 23);
            this.btnBuy.TabIndex = 28;
            this.btnBuy.Text = "buy";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // sendorder_size
            // 
            this.sendorder_size.Location = new System.Drawing.Point(160, 181);
            this.sendorder_size.Name = "sendorder_size";
            this.sendorder_size.Size = new System.Drawing.Size(23, 21);
            this.sendorder_size.TabIndex = 30;
            this.sendorder_size.Text = "1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(130, 190);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 29;
            this.label10.Text = "数量";
            // 
            // sendorder_price
            // 
            this.sendorder_price.Location = new System.Drawing.Point(226, 181);
            this.sendorder_price.Name = "sendorder_price";
            this.sendorder_price.Size = new System.Drawing.Size(46, 21);
            this.sendorder_price.TabIndex = 32;
            this.sendorder_price.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(196, 190);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 31;
            this.label11.Text = "价格";
            // 
            // btnSell
            // 
            this.btnSell.Location = new System.Drawing.Point(387, 193);
            this.btnSell.Name = "btnSell";
            this.btnSell.Size = new System.Drawing.Size(69, 23);
            this.btnSell.TabIndex = 33;
            this.btnSell.Text = "sell";
            this.btnSell.UseVisualStyleBackColor = true;
            this.btnSell.Click += new System.EventHandler(this.btnSell_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(17, 241);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 34;
            this.label12.Text = "委托编号";
            // 
            // orderaction_orderid
            // 
            this.orderaction_orderid.Location = new System.Drawing.Point(76, 232);
            this.orderaction_orderid.Name = "orderaction_orderid";
            this.orderaction_orderid.Size = new System.Drawing.Size(196, 21);
            this.orderaction_orderid.TabIndex = 35;
            this.orderaction_orderid.Text = "0";
            // 
            // btnCancelorder
            // 
            this.btnCancelorder.Location = new System.Drawing.Point(387, 230);
            this.btnCancelorder.Name = "btnCancelorder";
            this.btnCancelorder.Size = new System.Drawing.Size(69, 76);
            this.btnCancelorder.TabIndex = 36;
            this.btnCancelorder.Text = "cancel";
            this.btnCancelorder.UseVisualStyleBackColor = true;
            this.btnCancelorder.Click += new System.EventHandler(this.btnCancelorder_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 262);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 37;
            this.label13.Text = "本地编号";
            // 
            // orderaction_orderref
            // 
            this.orderaction_orderref.Location = new System.Drawing.Point(75, 259);
            this.orderaction_orderref.Name = "orderaction_orderref";
            this.orderaction_orderref.Size = new System.Drawing.Size(197, 21);
            this.orderaction_orderref.TabIndex = 38;
            this.orderaction_orderref.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(17, 294);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 12);
            this.label14.TabIndex = 39;
            this.label14.Text = "ExchID";
            // 
            // orderaction_exchid
            // 
            this.orderaction_exchid.Location = new System.Drawing.Point(75, 285);
            this.orderaction_exchid.Name = "orderaction_exchid";
            this.orderaction_exchid.Size = new System.Drawing.Size(64, 21);
            this.orderaction_exchid.TabIndex = 40;
            this.orderaction_exchid.Text = "0";
            // 
            // orderaction_sysid
            // 
            this.orderaction_sysid.Location = new System.Drawing.Point(211, 286);
            this.orderaction_sysid.Name = "orderaction_sysid";
            this.orderaction_sysid.Size = new System.Drawing.Size(64, 21);
            this.orderaction_sysid.TabIndex = 42;
            this.orderaction_sysid.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(153, 295);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 12);
            this.label15.TabIndex = 41;
            this.label15.Text = "SysID";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(307, 635);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 23);
            this.button1.TabIndex = 43;
            this.button1.Text = "buy";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // qryorder_orderid
            // 
            this.qryorder_orderid.Location = new System.Drawing.Point(183, 321);
            this.qryorder_orderid.Name = "qryorder_orderid";
            this.qryorder_orderid.Size = new System.Drawing.Size(108, 21);
            this.qryorder_orderid.TabIndex = 48;
            this.qryorder_orderid.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(125, 330);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 47;
            this.label8.Text = "委托编号";
            // 
            // btnQryOrder
            // 
            this.btnQryOrder.Location = new System.Drawing.Point(387, 325);
            this.btnQryOrder.Name = "btnQryOrder";
            this.btnQryOrder.Size = new System.Drawing.Size(69, 23);
            this.btnQryOrder.TabIndex = 46;
            this.btnQryOrder.Text = "qryorder";
            this.btnQryOrder.UseVisualStyleBackColor = true;
            this.btnQryOrder.Click += new System.EventHandler(this.btnQryOrder_Click_1);
            // 
            // qryorder_symbol
            // 
            this.qryorder_symbol.Location = new System.Drawing.Point(47, 321);
            this.qryorder_symbol.Name = "qryorder_symbol";
            this.qryorder_symbol.Size = new System.Drawing.Size(71, 21);
            this.qryorder_symbol.TabIndex = 45;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 330);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 44;
            this.label6.Text = "合约";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnSub);
            this.groupBox1.Controls.Add(this.symlist);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.btnDemo);
            this.groupBox1.Controls.Add(this.btnQryInvestor);
            this.groupBox1.Controls.Add(this.btnQryAcc);
            this.groupBox1.Controls.Add(this.qrymaxvol_symbol);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.btnQryMaxVol);
            this.groupBox1.Controls.Add(this.qrypos_symbol);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.btnQryPosition);
            this.groupBox1.Controls.Add(this.btnQryTrade);
            this.groupBox1.Controls.Add(this.qrytrade_symbol);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.btnStatClient);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.qryorder_orderid);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.convertnum);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.btnConvert);
            this.groupBox1.Controls.Add(this.IPAddress);
            this.groupBox1.Controls.Add(this.btnVersion);
            this.groupBox1.Controls.Add(this.btnQryOrder);
            this.groupBox1.Controls.Add(this.btnBrokerName);
            this.groupBox1.Controls.Add(this.btnBrokerNameRequest);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.qryorder_symbol);
            this.groupBox1.Controls.Add(this.Port);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.orderaction_sysid);
            this.groupBox1.Controls.Add(this.loginid);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.orderaction_exchid);
            this.groupBox1.Controls.Add(this.passwd);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.currentaccount);
            this.groupBox1.Controls.Add(this.orderaction_orderref);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnCancelorder);
            this.groupBox1.Controls.Add(this.sendorder_symbol);
            this.groupBox1.Controls.Add(this.orderaction_orderid);
            this.groupBox1.Controls.Add(this.btnBuy);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnSell);
            this.groupBox1.Controls.Add(this.sendorder_size);
            this.groupBox1.Controls.Add(this.sendorder_price);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(866, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 737);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(395, 127);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 23);
            this.button2.TabIndex = 64;
            this.button2.Text = "unreg";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(316, 127);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(69, 23);
            this.btnSub.TabIndex = 63;
            this.btnSub.Text = "regsym";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // symlist
            // 
            this.symlist.Location = new System.Drawing.Point(52, 129);
            this.symlist.Name = "symlist";
            this.symlist.Size = new System.Drawing.Size(247, 21);
            this.symlist.TabIndex = 62;
            this.symlist.Text = "IF1408";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(17, 138);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 12);
            this.label19.TabIndex = 61;
            this.label19.Text = "合约";
            // 
            // btnDemo
            // 
            this.btnDemo.Location = new System.Drawing.Point(387, 653);
            this.btnDemo.Name = "btnDemo";
            this.btnDemo.Size = new System.Drawing.Size(69, 49);
            this.btnDemo.TabIndex = 60;
            this.btnDemo.Text = "Demo";
            this.btnDemo.UseVisualStyleBackColor = true;
            this.btnDemo.Click += new System.EventHandler(this.btnDemo_Click);
            // 
            // btnQryInvestor
            // 
            this.btnQryInvestor.Location = new System.Drawing.Point(387, 468);
            this.btnQryInvestor.Name = "btnQryInvestor";
            this.btnQryInvestor.Size = new System.Drawing.Size(69, 23);
            this.btnQryInvestor.TabIndex = 59;
            this.btnQryInvestor.Text = "qryinvestor";
            this.btnQryInvestor.UseVisualStyleBackColor = true;
            this.btnQryInvestor.Click += new System.EventHandler(this.btnQryInvestor_Click);
            // 
            // btnQryAcc
            // 
            this.btnQryAcc.Location = new System.Drawing.Point(387, 439);
            this.btnQryAcc.Name = "btnQryAcc";
            this.btnQryAcc.Size = new System.Drawing.Size(69, 23);
            this.btnQryAcc.TabIndex = 58;
            this.btnQryAcc.Text = "qryacc";
            this.btnQryAcc.UseVisualStyleBackColor = true;
            this.btnQryAcc.Click += new System.EventHandler(this.btnQryAcc_Click);
            // 
            // qrymaxvol_symbol
            // 
            this.qrymaxvol_symbol.Location = new System.Drawing.Point(47, 413);
            this.qrymaxvol_symbol.Name = "qrymaxvol_symbol";
            this.qrymaxvol_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrymaxvol_symbol.TabIndex = 57;
            this.qrymaxvol_symbol.Text = "IF1408";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 422);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(29, 12);
            this.label18.TabIndex = 56;
            this.label18.Text = "合约";
            // 
            // btnQryMaxVol
            // 
            this.btnQryMaxVol.Location = new System.Drawing.Point(387, 410);
            this.btnQryMaxVol.Name = "btnQryMaxVol";
            this.btnQryMaxVol.Size = new System.Drawing.Size(69, 23);
            this.btnQryMaxVol.TabIndex = 55;
            this.btnQryMaxVol.Text = "qrymaxvol";
            this.btnQryMaxVol.UseVisualStyleBackColor = true;
            this.btnQryMaxVol.Click += new System.EventHandler(this.btnQryMaxVol_Click);
            // 
            // qrypos_symbol
            // 
            this.qrypos_symbol.Location = new System.Drawing.Point(47, 386);
            this.qrypos_symbol.Name = "qrypos_symbol";
            this.qrypos_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrypos_symbol.TabIndex = 54;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 395);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 53;
            this.label17.Text = "合约";
            // 
            // btnQryPosition
            // 
            this.btnQryPosition.Location = new System.Drawing.Point(387, 383);
            this.btnQryPosition.Name = "btnQryPosition";
            this.btnQryPosition.Size = new System.Drawing.Size(69, 23);
            this.btnQryPosition.TabIndex = 52;
            this.btnQryPosition.Text = "qrypos";
            this.btnQryPosition.UseVisualStyleBackColor = true;
            this.btnQryPosition.Click += new System.EventHandler(this.btnQryPosition_Click);
            // 
            // btnQryTrade
            // 
            this.btnQryTrade.Location = new System.Drawing.Point(387, 354);
            this.btnQryTrade.Name = "btnQryTrade";
            this.btnQryTrade.Size = new System.Drawing.Size(69, 23);
            this.btnQryTrade.TabIndex = 51;
            this.btnQryTrade.Text = "qrytrade";
            this.btnQryTrade.UseVisualStyleBackColor = true;
            this.btnQryTrade.Click += new System.EventHandler(this.btnQryTrade_Click);
            // 
            // qrytrade_symbol
            // 
            this.qrytrade_symbol.Location = new System.Drawing.Point(47, 359);
            this.qrytrade_symbol.Name = "qrytrade_symbol";
            this.qrytrade_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrytrade_symbol.TabIndex = 50;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 368);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 49;
            this.label16.Text = "合约";
            // 
            // ctDebug1
            // 
            this.ctDebug1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ctDebug1.EnableSearching = true;
            this.ctDebug1.ExternalTimeStamp = 0;
            this.ctDebug1.Location = new System.Drawing.Point(0, 0);
            this.ctDebug1.Margin = new System.Windows.Forms.Padding(2);
            this.ctDebug1.Name = "ctDebug1";
            this.ctDebug1.Size = new System.Drawing.Size(861, 737);
            this.ctDebug1.TabIndex = 0;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 737);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ctDebug1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private APIDemo.DebugControl ctDebug1;
        private System.Windows.Forms.Button btnBrokerNameRequest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IPAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Button btnBrokerName;
        private System.Windows.Forms.Button btnVersion;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.TextBox convertnum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStatClient;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox passwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox loginid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label currentaccount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox sendorder_symbol;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.TextBox sendorder_size;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox sendorder_price;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSell;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox orderaction_orderid;
        private System.Windows.Forms.Button btnCancelorder;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox orderaction_orderref;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox orderaction_exchid;
        private System.Windows.Forms.TextBox orderaction_sysid;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox qryorder_orderid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnQryOrder;
        private System.Windows.Forms.TextBox qryorder_symbol;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnQryTrade;
        private System.Windows.Forms.TextBox qrytrade_symbol;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox qrypos_symbol;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnQryPosition;
        private System.Windows.Forms.TextBox qrymaxvol_symbol;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnQryMaxVol;
        private System.Windows.Forms.Button btnQryAcc;
        private System.Windows.Forms.Button btnQryInvestor;
        private System.Windows.Forms.Button btnDemo;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.TextBox symlist;
        private System.Windows.Forms.Label label19;
    }
}

