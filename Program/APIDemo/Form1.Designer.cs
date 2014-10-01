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
            this.label1 = new System.Windows.Forms.Label();
            this.IPAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Port = new System.Windows.Forms.TextBox();
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
            this.qryorder_orderid = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnQryOrder = new System.Windows.Forms.Button();
            this.qryorder_symbol = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.submintnum = new System.Windows.Forms.TextBox();
            this.qrysettle_day = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.offsetflag = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.login_mac = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.btnQryOpenSize = new System.Windows.Forms.Button();
            this.qymaxvol_symbol = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.btnQrySymbol = new System.Windows.Forms.Button();
            this.qrysymbol_Symbol = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.logintype = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.changepass_newpass = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.changepass_oldpass = new System.Windows.Forms.TextBox();
            this.btnContirbRequest = new System.Windows.Forms.Button();
            this.contribargs = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.contribcmdstr = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.contribmodule = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnConfirmSettlement = new System.Windows.Forms.Button();
            this.btnQrySettlementInfo = new System.Windows.Forms.Button();
            this.btnQrySettleConfirm = new System.Windows.Forms.Button();
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
            // convertnum
            // 
            this.convertnum.Location = new System.Drawing.Point(83, 726);
            this.convertnum.Name = "convertnum";
            this.convertnum.Size = new System.Drawing.Size(142, 21);
            this.convertnum.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 735);
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
            this.passwd.Location = new System.Drawing.Point(208, 92);
            this.passwd.Name = "passwd";
            this.passwd.Size = new System.Drawing.Size(75, 21);
            this.passwd.TabIndex = 16;
            this.passwd.Text = "123456";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(173, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "密码";
            // 
            // loginid
            // 
            this.loginid.Location = new System.Drawing.Point(64, 91);
            this.loginid.Name = "loginid";
            this.loginid.Size = new System.Drawing.Size(91, 21);
            this.loginid.TabIndex = 14;
            this.loginid.Text = "9280001";
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
            this.sendorder_symbol.Text = "IF1410";
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
            this.btnBuy.Location = new System.Drawing.Point(387, 176);
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
            this.btnSell.Location = new System.Drawing.Point(387, 201);
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
            // qryorder_orderid
            // 
            this.qryorder_orderid.Location = new System.Drawing.Point(183, 376);
            this.qryorder_orderid.Name = "qryorder_orderid";
            this.qryorder_orderid.Size = new System.Drawing.Size(108, 21);
            this.qryorder_orderid.TabIndex = 48;
            this.qryorder_orderid.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(125, 385);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 47;
            this.label8.Text = "委托编号";
            // 
            // btnQryOrder
            // 
            this.btnQryOrder.Location = new System.Drawing.Point(387, 380);
            this.btnQryOrder.Name = "btnQryOrder";
            this.btnQryOrder.Size = new System.Drawing.Size(69, 23);
            this.btnQryOrder.TabIndex = 46;
            this.btnQryOrder.Text = "qryorder";
            this.btnQryOrder.UseVisualStyleBackColor = true;
            this.btnQryOrder.Click += new System.EventHandler(this.btnQryOrder_Click_1);
            // 
            // qryorder_symbol
            // 
            this.qryorder_symbol.Location = new System.Drawing.Point(47, 376);
            this.qryorder_symbol.Name = "qryorder_symbol";
            this.qryorder_symbol.Size = new System.Drawing.Size(71, 21);
            this.qryorder_symbol.TabIndex = 45;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 385);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 44;
            this.label6.Text = "合约";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.submintnum);
            this.groupBox1.Controls.Add(this.qrysettle_day);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.offsetflag);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.login_mac);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.btnQryOpenSize);
            this.groupBox1.Controls.Add(this.qymaxvol_symbol);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.btnQrySymbol);
            this.groupBox1.Controls.Add(this.qrysymbol_Symbol);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.logintype);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.changepass_newpass);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.changepass_oldpass);
            this.groupBox1.Controls.Add(this.btnContirbRequest);
            this.groupBox1.Controls.Add(this.contribargs);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.contribcmdstr);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.contribmodule);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.btnConfirmSettlement);
            this.groupBox1.Controls.Add(this.btnQrySettlementInfo);
            this.groupBox1.Controls.Add(this.btnQrySettleConfirm);
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
            this.groupBox1.Controls.Add(this.qryorder_orderid);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.convertnum);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.IPAddress);
            this.groupBox1.Controls.Add(this.btnQryOrder);
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
            this.groupBox1.Location = new System.Drawing.Point(816, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 778);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(289, 212);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(29, 12);
            this.label32.TabIndex = 96;
            this.label32.Text = "循环";
            // 
            // submintnum
            // 
            this.submintnum.Location = new System.Drawing.Point(319, 203);
            this.submintnum.Name = "submintnum";
            this.submintnum.Size = new System.Drawing.Size(61, 21);
            this.submintnum.TabIndex = 97;
            this.submintnum.Text = "1";
            // 
            // qrysettle_day
            // 
            this.qrysettle_day.Location = new System.Drawing.Point(78, 351);
            this.qrysettle_day.Name = "qrysettle_day";
            this.qrysettle_day.Size = new System.Drawing.Size(197, 21);
            this.qrysettle_day.TabIndex = 95;
            this.qrysettle_day.Text = "20140926";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(19, 354);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(53, 12);
            this.label31.TabIndex = 94;
            this.label31.Text = "查询日期";
            // 
            // offsetflag
            // 
            this.offsetflag.FormattingEnabled = true;
            this.offsetflag.Location = new System.Drawing.Point(313, 182);
            this.offsetflag.Name = "offsetflag";
            this.offsetflag.Size = new System.Drawing.Size(67, 20);
            this.offsetflag.TabIndex = 93;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(278, 187);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(29, 12);
            this.label30.TabIndex = 92;
            this.label30.Text = "开平";
            // 
            // login_mac
            // 
            this.login_mac.Location = new System.Drawing.Point(55, 121);
            this.login_mac.Name = "login_mac";
            this.login_mac.Size = new System.Drawing.Size(123, 21);
            this.login_mac.TabIndex = 91;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(24, 124);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(23, 12);
            this.label29.TabIndex = 90;
            this.label29.Text = "MAC";
            // 
            // btnQryOpenSize
            // 
            this.btnQryOpenSize.Location = new System.Drawing.Point(366, 680);
            this.btnQryOpenSize.Name = "btnQryOpenSize";
            this.btnQryOpenSize.Size = new System.Drawing.Size(90, 23);
            this.btnQryOpenSize.TabIndex = 89;
            this.btnQryOpenSize.Text = "qryopensize";
            this.btnQryOpenSize.UseVisualStyleBackColor = true;
            this.btnQryOpenSize.Click += new System.EventHandler(this.btnQryOpenSize_Click);
            // 
            // qymaxvol_symbol
            // 
            this.qymaxvol_symbol.Location = new System.Drawing.Point(47, 680);
            this.qymaxvol_symbol.Name = "qymaxvol_symbol";
            this.qymaxvol_symbol.Size = new System.Drawing.Size(108, 21);
            this.qymaxvol_symbol.TabIndex = 88;
            this.qymaxvol_symbol.Text = "IF1409";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 689);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(29, 12);
            this.label28.TabIndex = 87;
            this.label28.Text = "合约";
            // 
            // btnQrySymbol
            // 
            this.btnQrySymbol.Location = new System.Drawing.Point(366, 651);
            this.btnQrySymbol.Name = "btnQrySymbol";
            this.btnQrySymbol.Size = new System.Drawing.Size(90, 23);
            this.btnQrySymbol.TabIndex = 86;
            this.btnQrySymbol.Text = "qrysymbol";
            this.btnQrySymbol.UseVisualStyleBackColor = true;
            this.btnQrySymbol.Click += new System.EventHandler(this.btnQrySymbol_Click);
            // 
            // qrysymbol_Symbol
            // 
            this.qrysymbol_Symbol.Location = new System.Drawing.Point(47, 653);
            this.qrysymbol_Symbol.Name = "qrysymbol_Symbol";
            this.qrysymbol_Symbol.Size = new System.Drawing.Size(108, 21);
            this.qrysymbol_Symbol.TabIndex = 85;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(6, 662);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(29, 12);
            this.label27.TabIndex = 84;
            this.label27.Text = "合约";
            // 
            // logintype
            // 
            this.logintype.Location = new System.Drawing.Point(320, 92);
            this.logintype.Name = "logintype";
            this.logintype.Size = new System.Drawing.Size(65, 21);
            this.logintype.TabIndex = 83;
            this.logintype.Text = "1";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(289, 101);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(29, 12);
            this.label26.TabIndex = 82;
            this.label26.Text = "方式";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(366, 619);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 23);
            this.button1.TabIndex = 81;
            this.button1.Text = "changepass";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(234, 630);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(41, 12);
            this.label25.TabIndex = 79;
            this.label25.Text = "老密码";
            // 
            // changepass_newpass
            // 
            this.changepass_newpass.Location = new System.Drawing.Point(277, 623);
            this.changepass_newpass.Name = "changepass_newpass";
            this.changepass_newpass.Size = new System.Drawing.Size(55, 21);
            this.changepass_newpass.TabIndex = 80;
            this.changepass_newpass.Text = "123456";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 630);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 12);
            this.label23.TabIndex = 75;
            this.label23.Text = "登入名";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(47, 623);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(71, 21);
            this.textBox1.TabIndex = 76;
            this.textBox1.Text = "5880002";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(122, 630);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(41, 12);
            this.label24.TabIndex = 77;
            this.label24.Text = "老密码";
            // 
            // changepass_oldpass
            // 
            this.changepass_oldpass.Location = new System.Drawing.Point(165, 623);
            this.changepass_oldpass.Name = "changepass_oldpass";
            this.changepass_oldpass.Size = new System.Drawing.Size(60, 21);
            this.changepass_oldpass.TabIndex = 78;
            this.changepass_oldpass.Text = "123456";
            // 
            // btnContirbRequest
            // 
            this.btnContirbRequest.Location = new System.Drawing.Point(366, 557);
            this.btnContirbRequest.Name = "btnContirbRequest";
            this.btnContirbRequest.Size = new System.Drawing.Size(90, 48);
            this.btnContirbRequest.TabIndex = 74;
            this.btnContirbRequest.Text = "contirbreq";
            this.btnContirbRequest.UseVisualStyleBackColor = true;
            this.btnContirbRequest.Click += new System.EventHandler(this.btnContirbRequest_Click);
            // 
            // contribargs
            // 
            this.contribargs.Location = new System.Drawing.Point(55, 584);
            this.contribargs.Name = "contribargs";
            this.contribargs.Size = new System.Drawing.Size(180, 21);
            this.contribargs.TabIndex = 73;
            this.contribargs.Text = "all";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(24, 593);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(29, 12);
            this.label22.TabIndex = 72;
            this.label22.Text = "参数";
            // 
            // contribcmdstr
            // 
            this.contribcmdstr.Location = new System.Drawing.Point(164, 557);
            this.contribcmdstr.Name = "contribcmdstr";
            this.contribcmdstr.Size = new System.Drawing.Size(71, 21);
            this.contribcmdstr.TabIndex = 71;
            this.contribcmdstr.Text = "qrysym";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(133, 566);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(29, 12);
            this.label21.TabIndex = 70;
            this.label21.Text = "命令";
            // 
            // contribmodule
            // 
            this.contribmodule.Location = new System.Drawing.Point(47, 557);
            this.contribmodule.Name = "contribmodule";
            this.contribmodule.Size = new System.Drawing.Size(71, 21);
            this.contribmodule.TabIndex = 69;
            this.contribmodule.Text = "basicinfo";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 566);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(29, 12);
            this.label20.TabIndex = 68;
            this.label20.Text = "模块";
            // 
            // btnConfirmSettlement
            // 
            this.btnConfirmSettlement.Location = new System.Drawing.Point(316, 325);
            this.btnConfirmSettlement.Name = "btnConfirmSettlement";
            this.btnConfirmSettlement.Size = new System.Drawing.Size(140, 23);
            this.btnConfirmSettlement.TabIndex = 67;
            this.btnConfirmSettlement.Text = "confirmsettlement";
            this.btnConfirmSettlement.UseVisualStyleBackColor = true;
            this.btnConfirmSettlement.Click += new System.EventHandler(this.btnConfirmSettlement_Click);
            // 
            // btnQrySettlementInfo
            // 
            this.btnQrySettlementInfo.Location = new System.Drawing.Point(316, 351);
            this.btnQrySettlementInfo.Name = "btnQrySettlementInfo";
            this.btnQrySettlementInfo.Size = new System.Drawing.Size(140, 23);
            this.btnQrySettlementInfo.TabIndex = 66;
            this.btnQrySettlementInfo.Text = "qrysettlement";
            this.btnQrySettlementInfo.UseVisualStyleBackColor = true;
            this.btnQrySettlementInfo.Click += new System.EventHandler(this.btnQrySettlementInfo_Click);
            // 
            // btnQrySettleConfirm
            // 
            this.btnQrySettleConfirm.Location = new System.Drawing.Point(19, 325);
            this.btnQrySettleConfirm.Name = "btnQrySettleConfirm";
            this.btnQrySettleConfirm.Size = new System.Drawing.Size(140, 23);
            this.btnQrySettleConfirm.TabIndex = 65;
            this.btnQrySettleConfirm.Text = "qrysettlementconfirm";
            this.btnQrySettleConfirm.UseVisualStyleBackColor = true;
            this.btnQrySettleConfirm.Click += new System.EventHandler(this.btnQrySettleConfirm_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(395, 147);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 23);
            this.button2.TabIndex = 64;
            this.button2.Text = "unreg";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(316, 147);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(69, 23);
            this.btnSub.TabIndex = 63;
            this.btnSub.Text = "regsym";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // symlist
            // 
            this.symlist.Location = new System.Drawing.Point(52, 149);
            this.symlist.Name = "symlist";
            this.symlist.Size = new System.Drawing.Size(247, 21);
            this.symlist.TabIndex = 62;
            this.symlist.Text = "IF1409";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(17, 158);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 12);
            this.label19.TabIndex = 61;
            this.label19.Text = "合约";
            // 
            // btnDemo
            // 
            this.btnDemo.Location = new System.Drawing.Point(387, 717);
            this.btnDemo.Name = "btnDemo";
            this.btnDemo.Size = new System.Drawing.Size(69, 49);
            this.btnDemo.TabIndex = 60;
            this.btnDemo.Text = "Demo";
            this.btnDemo.UseVisualStyleBackColor = true;
            this.btnDemo.Click += new System.EventHandler(this.btnDemo_Click);
            // 
            // btnQryInvestor
            // 
            this.btnQryInvestor.Location = new System.Drawing.Point(387, 523);
            this.btnQryInvestor.Name = "btnQryInvestor";
            this.btnQryInvestor.Size = new System.Drawing.Size(69, 23);
            this.btnQryInvestor.TabIndex = 59;
            this.btnQryInvestor.Text = "qryinvestor";
            this.btnQryInvestor.UseVisualStyleBackColor = true;
            this.btnQryInvestor.Click += new System.EventHandler(this.btnQryInvestor_Click);
            // 
            // btnQryAcc
            // 
            this.btnQryAcc.Location = new System.Drawing.Point(387, 494);
            this.btnQryAcc.Name = "btnQryAcc";
            this.btnQryAcc.Size = new System.Drawing.Size(69, 23);
            this.btnQryAcc.TabIndex = 58;
            this.btnQryAcc.Text = "qryacc";
            this.btnQryAcc.UseVisualStyleBackColor = true;
            this.btnQryAcc.Click += new System.EventHandler(this.btnQryAcc_Click);
            // 
            // qrymaxvol_symbol
            // 
            this.qrymaxvol_symbol.Location = new System.Drawing.Point(47, 468);
            this.qrymaxvol_symbol.Name = "qrymaxvol_symbol";
            this.qrymaxvol_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrymaxvol_symbol.TabIndex = 57;
            this.qrymaxvol_symbol.Text = "IF1408";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 477);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(29, 12);
            this.label18.TabIndex = 56;
            this.label18.Text = "合约";
            // 
            // btnQryMaxVol
            // 
            this.btnQryMaxVol.Location = new System.Drawing.Point(387, 465);
            this.btnQryMaxVol.Name = "btnQryMaxVol";
            this.btnQryMaxVol.Size = new System.Drawing.Size(69, 23);
            this.btnQryMaxVol.TabIndex = 55;
            this.btnQryMaxVol.Text = "qrymaxvol";
            this.btnQryMaxVol.UseVisualStyleBackColor = true;
            this.btnQryMaxVol.Click += new System.EventHandler(this.btnQryMaxVol_Click);
            // 
            // qrypos_symbol
            // 
            this.qrypos_symbol.Location = new System.Drawing.Point(47, 441);
            this.qrypos_symbol.Name = "qrypos_symbol";
            this.qrypos_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrypos_symbol.TabIndex = 54;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 450);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 53;
            this.label17.Text = "合约";
            // 
            // btnQryPosition
            // 
            this.btnQryPosition.Location = new System.Drawing.Point(387, 438);
            this.btnQryPosition.Name = "btnQryPosition";
            this.btnQryPosition.Size = new System.Drawing.Size(69, 23);
            this.btnQryPosition.TabIndex = 52;
            this.btnQryPosition.Text = "qrypos";
            this.btnQryPosition.UseVisualStyleBackColor = true;
            this.btnQryPosition.Click += new System.EventHandler(this.btnQryPosition_Click);
            // 
            // btnQryTrade
            // 
            this.btnQryTrade.Location = new System.Drawing.Point(387, 409);
            this.btnQryTrade.Name = "btnQryTrade";
            this.btnQryTrade.Size = new System.Drawing.Size(69, 23);
            this.btnQryTrade.TabIndex = 51;
            this.btnQryTrade.Text = "qrytrade";
            this.btnQryTrade.UseVisualStyleBackColor = true;
            this.btnQryTrade.Click += new System.EventHandler(this.btnQryTrade_Click);
            // 
            // qrytrade_symbol
            // 
            this.qrytrade_symbol.Location = new System.Drawing.Point(47, 414);
            this.qrytrade_symbol.Name = "qrytrade_symbol";
            this.qrytrade_symbol.Size = new System.Drawing.Size(71, 21);
            this.qrytrade_symbol.TabIndex = 50;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 423);
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
            this.ctDebug1.Size = new System.Drawing.Size(811, 778);
            this.ctDebug1.TabIndex = 0;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 778);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ctDebug1);
            this.Name = "Form1";
            this.Text = "交易客户端";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private APIDemo.DebugControl ctDebug1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IPAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Port;
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
        private System.Windows.Forms.Button btnQrySettleConfirm;
        private System.Windows.Forms.Button btnQrySettlementInfo;
        private System.Windows.Forms.Button btnConfirmSettlement;
        private System.Windows.Forms.Button btnContirbRequest;
        private System.Windows.Forms.TextBox contribargs;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox contribcmdstr;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox contribmodule;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox changepass_newpass;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox changepass_oldpass;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox logintype;
        private System.Windows.Forms.Button btnQrySymbol;
        private System.Windows.Forms.TextBox qrysymbol_Symbol;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btnQryOpenSize;
        private System.Windows.Forms.TextBox qymaxvol_symbol;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox login_mac;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ComboBox offsetflag;
        private System.Windows.Forms.TextBox qrysettle_day;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox submintnum;
    }
}

