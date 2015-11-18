namespace DataClient
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
            this.btnMQClient = new System.Windows.Forms.Button();
            this.btnQryService = new System.Windows.Forms.Button();
            this.btnStopMQClient = new System.Windows.Forms.Button();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Symbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.maxcount = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.fromend = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.start = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.end = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.btnQryBar = new System.Windows.Forms.Button();
            this.interval = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnRegisterSymbol = new System.Windows.Forms.Button();
            this.reg_symbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnUnRegSymbol = new System.Windows.Forms.Button();
            this.btnQryMT = new System.Windows.Forms.Button();
            this.btnQryExchange = new System.Windows.Forms.Button();
            this.btnQrySec = new System.Windows.Forms.Button();
            this.btnQrySymbol = new System.Windows.Forms.Button();
            this.port = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.debugControl1 = new TLDataClient.DebugControl();
            this.SuspendLayout();
            // 
            // btnMQClient
            // 
            this.btnMQClient.Location = new System.Drawing.Point(465, 12);
            this.btnMQClient.Name = "btnMQClient";
            this.btnMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnMQClient.TabIndex = 2;
            this.btnMQClient.Text = "StartMQClient";
            this.btnMQClient.UseVisualStyleBackColor = true;
            this.btnMQClient.Click += new System.EventHandler(this.btnMQClient_Click);
            // 
            // btnQryService
            // 
            this.btnQryService.Location = new System.Drawing.Point(627, 12);
            this.btnQryService.Name = "btnQryService";
            this.btnQryService.Size = new System.Drawing.Size(75, 23);
            this.btnQryService.TabIndex = 3;
            this.btnQryService.Text = "QryService";
            this.btnQryService.UseVisualStyleBackColor = true;
            this.btnQryService.Click += new System.EventHandler(this.btnQryService_Click);
            // 
            // btnStopMQClient
            // 
            this.btnStopMQClient.Location = new System.Drawing.Point(546, 12);
            this.btnStopMQClient.Name = "btnStopMQClient";
            this.btnStopMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnStopMQClient.TabIndex = 4;
            this.btnStopMQClient.Text = "StopMQClient";
            this.btnStopMQClient.UseVisualStyleBackColor = true;
            this.btnStopMQClient.Click += new System.EventHandler(this.btnStopMQClient_Click);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(465, 53);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel1.TabIndex = 5;
            this.kryptonLabel1.Values.Text = "Symbol";
            // 
            // Symbol
            // 
            this.Symbol.Location = new System.Drawing.Point(522, 54);
            this.Symbol.Name = "Symbol";
            this.Symbol.Size = new System.Drawing.Size(63, 20);
            this.Symbol.TabIndex = 6;
            this.Symbol.Text = "IF1511";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(591, 53);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(67, 20);
            this.kryptonLabel2.TabIndex = 7;
            this.kryptonLabel2.Values.Text = "MaxCount";
            // 
            // maxcount
            // 
            this.maxcount.Location = new System.Drawing.Point(664, 52);
            this.maxcount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.maxcount.Name = "maxcount";
            this.maxcount.Size = new System.Drawing.Size(52, 22);
            this.maxcount.TabIndex = 8;
            this.maxcount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // fromend
            // 
            this.fromend.Location = new System.Drawing.Point(725, 53);
            this.fromend.Name = "fromend";
            this.fromend.Size = new System.Drawing.Size(72, 20);
            this.fromend.TabIndex = 9;
            this.fromend.Values.Text = "FromEnd";
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(507, 79);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(209, 21);
            this.start.TabIndex = 10;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(465, 81);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(36, 20);
            this.kryptonLabel3.TabIndex = 11;
            this.kryptonLabel3.Values.Text = "Start";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(465, 106);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(31, 20);
            this.kryptonLabel4.TabIndex = 12;
            this.kryptonLabel4.Values.Text = "End";
            // 
            // end
            // 
            this.end.Location = new System.Drawing.Point(507, 106);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(209, 21);
            this.end.TabIndex = 13;
            // 
            // btnQryBar
            // 
            this.btnQryBar.Location = new System.Drawing.Point(725, 81);
            this.btnQryBar.Name = "btnQryBar";
            this.btnQryBar.Size = new System.Drawing.Size(75, 45);
            this.btnQryBar.TabIndex = 14;
            this.btnQryBar.Text = "QryBar";
            this.btnQryBar.UseVisualStyleBackColor = true;
            this.btnQryBar.Click += new System.EventHandler(this.btnQryBar_Click);
            // 
            // interval
            // 
            this.interval.Location = new System.Drawing.Point(533, 133);
            this.interval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.interval.Name = "interval";
            this.interval.Size = new System.Drawing.Size(52, 22);
            this.interval.TabIndex = 16;
            this.interval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(465, 133);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel5.TabIndex = 15;
            this.kryptonLabel5.Values.Text = "Interval";
            // 
            // btnRegisterSymbol
            // 
            this.btnRegisterSymbol.Location = new System.Drawing.Point(618, 165);
            this.btnRegisterSymbol.Name = "btnRegisterSymbol";
            this.btnRegisterSymbol.Size = new System.Drawing.Size(75, 22);
            this.btnRegisterSymbol.TabIndex = 17;
            this.btnRegisterSymbol.Text = "RegSym";
            this.btnRegisterSymbol.UseVisualStyleBackColor = true;
            this.btnRegisterSymbol.Click += new System.EventHandler(this.btnRegisterSymbol_Click);
            // 
            // reg_symbol
            // 
            this.reg_symbol.Location = new System.Drawing.Point(522, 165);
            this.reg_symbol.Name = "reg_symbol";
            this.reg_symbol.Size = new System.Drawing.Size(63, 20);
            this.reg_symbol.TabIndex = 19;
            this.reg_symbol.Text = "HGZ5";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(465, 164);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel6.TabIndex = 18;
            this.kryptonLabel6.Values.Text = "Symbol";
            // 
            // btnUnRegSymbol
            // 
            this.btnUnRegSymbol.Location = new System.Drawing.Point(722, 165);
            this.btnUnRegSymbol.Name = "btnUnRegSymbol";
            this.btnUnRegSymbol.Size = new System.Drawing.Size(75, 22);
            this.btnUnRegSymbol.TabIndex = 20;
            this.btnUnRegSymbol.Text = "UnregSym";
            this.btnUnRegSymbol.UseVisualStyleBackColor = true;
            this.btnUnRegSymbol.Click += new System.EventHandler(this.btnUnRegSymbol_Click);
            // 
            // btnQryMT
            // 
            this.btnQryMT.Location = new System.Drawing.Point(476, 202);
            this.btnQryMT.Name = "btnQryMT";
            this.btnQryMT.Size = new System.Drawing.Size(75, 22);
            this.btnQryMT.TabIndex = 21;
            this.btnQryMT.Text = "QryMT";
            this.btnQryMT.UseVisualStyleBackColor = true;
            this.btnQryMT.Click += new System.EventHandler(this.btnQryMT_Click);
            // 
            // btnQryExchange
            // 
            this.btnQryExchange.Location = new System.Drawing.Point(557, 202);
            this.btnQryExchange.Name = "btnQryExchange";
            this.btnQryExchange.Size = new System.Drawing.Size(75, 22);
            this.btnQryExchange.TabIndex = 22;
            this.btnQryExchange.Text = "QryExchange";
            this.btnQryExchange.UseVisualStyleBackColor = true;
            this.btnQryExchange.Click += new System.EventHandler(this.btnQryExchange_Click);
            // 
            // btnQrySec
            // 
            this.btnQrySec.Location = new System.Drawing.Point(641, 202);
            this.btnQrySec.Name = "btnQrySec";
            this.btnQrySec.Size = new System.Drawing.Size(75, 22);
            this.btnQrySec.TabIndex = 23;
            this.btnQrySec.Text = "QrySec";
            this.btnQrySec.UseVisualStyleBackColor = true;
            this.btnQrySec.Click += new System.EventHandler(this.btnQrySec_Click);
            // 
            // btnQrySymbol
            // 
            this.btnQrySymbol.Location = new System.Drawing.Point(725, 202);
            this.btnQrySymbol.Name = "btnQrySymbol";
            this.btnQrySymbol.Size = new System.Drawing.Size(75, 22);
            this.btnQrySymbol.TabIndex = 24;
            this.btnQrySymbol.Text = "QrySymbol";
            this.btnQrySymbol.UseVisualStyleBackColor = true;
            this.btnQrySymbol.Click += new System.EventHandler(this.btnQrySymbol_Click);
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(708, 12);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(81, 20);
            this.port.TabIndex = 25;
            this.port.Text = "5060";
            // 
            // debugControl1
            // 
            this.debugControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.debugControl1.EnableSearching = true;
            this.debugControl1.ExternalTimeStamp = 0;
            this.debugControl1.Location = new System.Drawing.Point(0, 0);
            this.debugControl1.Margin = new System.Windows.Forms.Padding(2);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(450, 405);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 405);
            this.Controls.Add(this.port);
            this.Controls.Add(this.btnQrySymbol);
            this.Controls.Add(this.btnQrySec);
            this.Controls.Add(this.btnQryExchange);
            this.Controls.Add(this.btnQryMT);
            this.Controls.Add(this.btnUnRegSymbol);
            this.Controls.Add(this.reg_symbol);
            this.Controls.Add(this.kryptonLabel6);
            this.Controls.Add(this.btnRegisterSymbol);
            this.Controls.Add(this.interval);
            this.Controls.Add(this.kryptonLabel5);
            this.Controls.Add(this.btnQryBar);
            this.Controls.Add(this.end);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.start);
            this.Controls.Add(this.fromend);
            this.Controls.Add(this.maxcount);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.Symbol);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.btnStopMQClient);
            this.Controls.Add(this.btnQryService);
            this.Controls.Add(this.btnMQClient);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "行情客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TLDataClient.DebugControl debugControl1;
        private System.Windows.Forms.Button btnMQClient;
        private System.Windows.Forms.Button btnQryService;
        private System.Windows.Forms.Button btnStopMQClient;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown maxcount;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox fromend;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker start;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker end;
        private System.Windows.Forms.Button btnQryBar;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown interval;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private System.Windows.Forms.Button btnRegisterSymbol;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox reg_symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private System.Windows.Forms.Button btnUnRegSymbol;
        private System.Windows.Forms.Button btnQryMT;
        private System.Windows.Forms.Button btnQryExchange;
        private System.Windows.Forms.Button btnQrySec;
        private System.Windows.Forms.Button btnQrySymbol;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox port;
    }
}

