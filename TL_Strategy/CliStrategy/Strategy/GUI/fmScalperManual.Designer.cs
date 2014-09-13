namespace Strategy.GUI
{
    partial class fmScalperManual
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
            this.components = new System.ComponentModel.Container();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.memo = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.kryptonGroupBox3 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ask = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bid = new System.Windows.Forms.Label();
            this.bidSize = new System.Windows.Forms.Label();
            this.last = new System.Windows.Forms.Label();
            this.unpl = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.askSize = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.avgCost = new System.Windows.Forms.Label();
            this.kryptonGroupBox2 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.isPrice = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.price = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.isOffset = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.setLimit = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.tif = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.offset = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.possize = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.setIsProfitOrderPark = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.setSize = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.setTargetProfit = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.setLoss = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox3)).BeginInit();
            this.kryptonGroupBox3.Panel.SuspendLayout();
            this.kryptonGroupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).BeginInit();
            this.kryptonGroupBox2.Panel.SuspendLayout();
            this.kryptonGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.memo);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox3);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox2);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel1.Controls.Add(this.button3);
            this.kryptonPanel1.Controls.Add(this.button2);
            this.kryptonPanel1.Controls.Add(this.button1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(304, 297);
            this.kryptonPanel1.TabIndex = 37;
            // 
            // memo
            // 
            this.memo.Location = new System.Drawing.Point(3, 229);
            this.memo.Name = "memo";
            this.memo.Size = new System.Drawing.Size(41, 18);
            this.memo.TabIndex = 39;
            this.memo.Values.Text = "说 明";
            this.memo.LinkClicked += new System.EventHandler(this.memo_LinkClicked);
            // 
            // kryptonGroupBox3
            // 
            this.kryptonGroupBox3.Location = new System.Drawing.Point(1, 1);
            this.kryptonGroupBox3.Name = "kryptonGroupBox3";
            // 
            // kryptonGroupBox3.Panel
            // 
            this.kryptonGroupBox3.Panel.Controls.Add(this.panel1);
            this.kryptonGroupBox3.Size = new System.Drawing.Size(161, 118);
            this.kryptonGroupBox3.TabIndex = 38;
            this.kryptonGroupBox3.Text = "行 情";
            this.kryptonGroupBox3.Values.Heading = "行 情";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ask);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.bid);
            this.panel1.Controls.Add(this.bidSize);
            this.panel1.Controls.Add(this.last);
            this.panel1.Controls.Add(this.unpl);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.askSize);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.avgCost);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(151, 84);
            this.panel1.TabIndex = 32;
            // 
            // ask
            // 
            this.ask.AutoSize = true;
            this.ask.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ask.ForeColor = System.Drawing.Color.Yellow;
            this.ask.Location = new System.Drawing.Point(96, 5);
            this.ask.Name = "ask";
            this.ask.Size = new System.Drawing.Size(18, 16);
            this.ask.TabIndex = 3;
            this.ask.Text = "--";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Aqua;
            this.label1.Location = new System.Drawing.Point(1, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "买";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Aqua;
            this.label2.Location = new System.Drawing.Point(80, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "卖";
            // 
            // bid
            // 
            this.bid.AutoSize = true;
            this.bid.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bid.ForeColor = System.Drawing.Color.Yellow;
            this.bid.Location = new System.Drawing.Point(18, 5);
            this.bid.Name = "bid";
            this.bid.Size = new System.Drawing.Size(18, 16);
            this.bid.TabIndex = 2;
            this.bid.Text = "--";
            // 
            // bidSize
            // 
            this.bidSize.AutoSize = true;
            this.bidSize.ForeColor = System.Drawing.Color.Yellow;
            this.bidSize.Location = new System.Drawing.Point(24, 22);
            this.bidSize.Name = "bidSize";
            this.bidSize.Size = new System.Drawing.Size(17, 12);
            this.bidSize.TabIndex = 4;
            this.bidSize.Text = "--";
            // 
            // last
            // 
            this.last.AutoSize = true;
            this.last.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.last.ForeColor = System.Drawing.Color.Yellow;
            this.last.Location = new System.Drawing.Point(28, 40);
            this.last.Name = "last";
            this.last.Size = new System.Drawing.Size(18, 16);
            this.last.TabIndex = 27;
            this.last.Text = "--";
            // 
            // unpl
            // 
            this.unpl.AutoSize = true;
            this.unpl.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unpl.ForeColor = System.Drawing.Color.Yellow;
            this.unpl.Location = new System.Drawing.Point(114, 62);
            this.unpl.Name = "unpl";
            this.unpl.Size = new System.Drawing.Size(18, 16);
            this.unpl.TabIndex = 20;
            this.unpl.Text = "--";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Aqua;
            this.label8.Location = new System.Drawing.Point(86, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "浮差";
            // 
            // askSize
            // 
            this.askSize.AutoSize = true;
            this.askSize.ForeColor = System.Drawing.Color.Yellow;
            this.askSize.Location = new System.Drawing.Point(103, 23);
            this.askSize.Name = "askSize";
            this.askSize.Size = new System.Drawing.Size(17, 12);
            this.askSize.TabIndex = 5;
            this.askSize.Text = "--";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Aqua;
            this.label10.Location = new System.Drawing.Point(0, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "成交";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Aqua;
            this.label6.Location = new System.Drawing.Point(0, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "成本";
            // 
            // avgCost
            // 
            this.avgCost.AutoSize = true;
            this.avgCost.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.avgCost.ForeColor = System.Drawing.Color.Yellow;
            this.avgCost.Location = new System.Drawing.Point(28, 62);
            this.avgCost.Name = "avgCost";
            this.avgCost.Size = new System.Drawing.Size(18, 16);
            this.avgCost.TabIndex = 18;
            this.avgCost.Text = "--";
            // 
            // kryptonGroupBox2
            // 
            this.kryptonGroupBox2.Location = new System.Drawing.Point(168, 1);
            this.kryptonGroupBox2.Name = "kryptonGroupBox2";
            // 
            // kryptonGroupBox2.Panel
            // 
            this.kryptonGroupBox2.Panel.Controls.Add(this.isPrice);
            this.kryptonGroupBox2.Panel.Controls.Add(this.price);
            this.kryptonGroupBox2.Panel.Controls.Add(this.isOffset);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel8);
            this.kryptonGroupBox2.Panel.Controls.Add(this.setLimit);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel3);
            this.kryptonGroupBox2.Panel.Controls.Add(this.tif);
            this.kryptonGroupBox2.Panel.Controls.Add(this.offset);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel2);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroupBox2.Size = new System.Drawing.Size(131, 118);
            this.kryptonGroupBox2.TabIndex = 37;
            this.kryptonGroupBox2.Text = "委托设置";
            this.kryptonGroupBox2.Values.Heading = "委托设置";
            // 
            // isPrice
            // 
            this.isPrice.Location = new System.Drawing.Point(106, 53);
            this.isPrice.Name = "isPrice";
            this.isPrice.Size = new System.Drawing.Size(18, 12);
            this.isPrice.TabIndex = 41;
            this.isPrice.Values.Text = "";
            this.isPrice.CheckedChanged += new System.EventHandler(this.isPrice_CheckedChanged);
            // 
            // price
            // 
            this.price.Location = new System.Drawing.Point(40, 49);
            this.price.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.price.Name = "price";
            this.price.Size = new System.Drawing.Size(64, 20);
            this.price.TabIndex = 8;
            // 
            // isOffset
            // 
            this.isOffset.Checked = true;
            this.isOffset.Location = new System.Drawing.Point(108, 29);
            this.isOffset.Name = "isOffset";
            this.isOffset.Size = new System.Drawing.Size(18, 12);
            this.isOffset.TabIndex = 40;
            this.isOffset.Values.Text = "";
            this.isOffset.CheckedChanged += new System.EventHandler(this.isOffset_CheckedChanged);
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(3, 51);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel8.TabIndex = 7;
            this.kryptonLabel8.Values.Text = "价格:";
            // 
            // setLimit
            // 
            this.setLimit.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.setLimit.Location = new System.Drawing.Point(68, 3);
            this.setLimit.Name = "setLimit";
            this.setLimit.Size = new System.Drawing.Size(54, 18);
            this.setLimit.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.setLimit.TabIndex = 6;
            this.setLimit.Text = "市 价";
            this.setLimit.Values.Text = "市 价";
            this.setLimit.CheckedChanged += new System.EventHandler(this.setLimit_CheckedChanged);
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 5;
            this.kryptonLabel3.Values.Text = "委托类型:";
            // 
            // tif
            // 
            this.tif.Enabled = false;
            this.tif.Location = new System.Drawing.Point(70, 73);
            this.tif.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.tif.Name = "tif";
            this.tif.Size = new System.Drawing.Size(47, 20);
            this.tif.TabIndex = 3;
            // 
            // offset
            // 
            this.offset.Enabled = false;
            this.offset.Location = new System.Drawing.Point(40, 25);
            this.offset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.offset.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.offset.Name = "offset";
            this.offset.Size = new System.Drawing.Size(64, 20);
            this.offset.TabIndex = 2;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 73);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "延迟撤单:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 27);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "偏移:";
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(1, 121);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.possize);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel7);
            this.kryptonGroupBox1.Panel.Controls.Add(this.setIsProfitOrderPark);
            this.kryptonGroupBox1.Panel.Controls.Add(this.setSize);
            this.kryptonGroupBox1.Panel.Controls.Add(this.setTargetProfit);
            this.kryptonGroupBox1.Panel.Controls.Add(this.setLoss);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel4);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel5);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel6);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(160, 102);
            this.kryptonGroupBox1.TabIndex = 36;
            this.kryptonGroupBox1.Text = "止盈止损";
            this.kryptonGroupBox1.Values.Heading = "止盈止损";
            // 
            // possize
            // 
            this.possize.Location = new System.Drawing.Point(129, 53);
            this.possize.Name = "possize";
            this.possize.Size = new System.Drawing.Size(19, 18);
            this.possize.TabIndex = 15;
            this.possize.Values.Text = "--";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(95, 53);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel7.TabIndex = 13;
            this.kryptonLabel7.Values.Text = "持仓:";
            // 
            // setIsProfitOrderPark
            // 
            this.setIsProfitOrderPark.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.setIsProfitOrderPark.Location = new System.Drawing.Point(99, 27);
            this.setIsProfitOrderPark.Name = "setIsProfitOrderPark";
            this.setIsProfitOrderPark.Size = new System.Drawing.Size(51, 18);
            this.setIsProfitOrderPark.TabIndex = 12;
            this.setIsProfitOrderPark.Text = "挂单";
            this.setIsProfitOrderPark.Values.Text = "挂单";
            // 
            // setSize
            // 
            this.setSize.Location = new System.Drawing.Point(46, 51);
            this.setSize.Name = "setSize";
            this.setSize.Size = new System.Drawing.Size(47, 20);
            this.setSize.TabIndex = 11;
            this.setSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // setTargetProfit
            // 
            this.setTargetProfit.Location = new System.Drawing.Point(46, 27);
            this.setTargetProfit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.setTargetProfit.Name = "setTargetProfit";
            this.setTargetProfit.Size = new System.Drawing.Size(47, 20);
            this.setTargetProfit.TabIndex = 10;
            // 
            // setLoss
            // 
            this.setLoss.Location = new System.Drawing.Point(46, 1);
            this.setLoss.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.setLoss.Name = "setLoss";
            this.setLoss.Size = new System.Drawing.Size(47, 20);
            this.setLoss.TabIndex = 9;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 8;
            this.kryptonLabel4.Values.Text = "止损:";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(3, 51);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel5.TabIndex = 7;
            this.kryptonLabel5.Values.Text = "手数:";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(3, 27);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel6.TabIndex = 6;
            this.kryptonLabel6.Values.Text = "止盈:";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Red;
            this.button3.Location = new System.Drawing.Point(162, 200);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(45, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "买";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.buy_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Green;
            this.button2.Location = new System.Drawing.Point(210, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(45, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "卖";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.sell_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 201);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 22);
            this.button1.TabIndex = 11;
            this.button1.Text = "平仓";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.flat_Click);
            // 
            // fmScalperManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 297);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmScalperManual";
            this.Text = "Scalper";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fmScalperManual_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.kryptonGroupBox3.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox3)).EndInit();
            this.kryptonGroupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.kryptonGroupBox2.Panel.ResumeLayout(false);
            this.kryptonGroupBox2.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).EndInit();
            this.kryptonGroupBox2.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label ask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label bid;
        private System.Windows.Forms.Label bidSize;
        private System.Windows.Forms.Label last;
        private System.Windows.Forms.Label unpl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label askSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label avgCost;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown tif;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown offset;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox setLimit;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox setIsProfitOrderPark;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown setSize;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown setTargetProfit;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown setLoss;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel possize;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel memo;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown price;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton isOffset;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton isPrice;

        //private TradingLib.GUI.ctTradeInfo ctTradeInfo1;
    }
}