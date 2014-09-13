namespace Strategy.GUI
{
    partial class fmStopTrailing
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this._pos = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._ask = new System.Windows.Forms.Label();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this._osize = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel16 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel15 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.trailing2 = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.start2 = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.trailing1 = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel14 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.start1 = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel13 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.breakeven = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel12 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.stoploss = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel11 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._profitTakestop = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._breakEven = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._stoploss = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._adverse = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._favor = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonGroupBox3 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this._bid = new System.Windows.Forms.Label();
            this.bidSize = new System.Windows.Forms.Label();
            this._last = new System.Windows.Forms.Label();
            this._unrealizedpl = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.askSize = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this._avgcost = new System.Windows.Forms.Label();
            this.btnmemo = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox3)).BeginInit();
            this.kryptonGroupBox3.Panel.SuspendLayout();
            this.kryptonGroupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(233, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 22);
            this.button1.TabIndex = 5;
            this.button1.Text = "平仓";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button2.BackColor = System.Drawing.Color.Green;
            this.button2.Location = new System.Drawing.Point(175, 225);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(58, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "卖";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button3.BackColor = System.Drawing.Color.Red;
            this.button3.Location = new System.Drawing.Point(117, 225);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(58, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "买";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnmemo);
            this.kryptonPanel1.Controls.Add(this._pos);
            this.kryptonPanel1.Controls.Add(this.button3);
            this.kryptonPanel1.Controls.Add(this.button2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel1.Controls.Add(this.button1);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel1.Controls.Add(this._profitTakestop);
            this.kryptonPanel1.Controls.Add(this._breakEven);
            this.kryptonPanel1.Controls.Add(this._stoploss);
            this.kryptonPanel1.Controls.Add(this._adverse);
            this.kryptonPanel1.Controls.Add(this._favor);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox3);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(294, 292);
            this.kryptonPanel1.TabIndex = 9;
            // 
            // _pos
            // 
            this._pos.Location = new System.Drawing.Point(212, 103);
            this._pos.Name = "_pos";
            this._pos.Size = new System.Drawing.Size(19, 18);
            this._pos.TabIndex = 51;
            this._pos.Target = this._ask;
            this._pos.Values.Text = "--";
            // 
            // _ask
            // 
            this._ask.AutoSize = true;
            this._ask.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ask.ForeColor = System.Drawing.Color.Yellow;
            this._ask.Location = new System.Drawing.Point(96, 5);
            this._ask.Name = "_ask";
            this._ask.Size = new System.Drawing.Size(18, 16);
            this._ask.TabIndex = 3;
            this._ask.Text = "--";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(170, 103);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel7.TabIndex = 50;
            this.kryptonLabel7.Values.Text = "持仓:";
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(3, 123);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this._osize);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel8);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel16);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel15);
            this.kryptonGroupBox1.Panel.Controls.Add(this.trailing2);
            this.kryptonGroupBox1.Panel.Controls.Add(this.start2);
            this.kryptonGroupBox1.Panel.Controls.Add(this.trailing1);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel14);
            this.kryptonGroupBox1.Panel.Controls.Add(this.start1);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel13);
            this.kryptonGroupBox1.Panel.Controls.Add(this.breakeven);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel12);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stoploss);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel11);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(286, 97);
            this.kryptonGroupBox1.TabIndex = 10;
            this.kryptonGroupBox1.Text = "参数设置";
            this.kryptonGroupBox1.Values.Heading = "参数设置";
            // 
            // _osize
            // 
            this._osize.Location = new System.Drawing.Point(62, 47);
            this._osize.Name = "_osize";
            this._osize.Size = new System.Drawing.Size(55, 20);
            this._osize.TabIndex = 54;
            this._osize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(8, 49);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel8.TabIndex = 53;
            this.kryptonLabel8.Target = this._ask;
            this.kryptonLabel8.Values.Text = "手数";
            // 
            // kryptonLabel16
            // 
            this.kryptonLabel16.Location = new System.Drawing.Point(123, 47);
            this.kryptonLabel16.Name = "kryptonLabel16";
            this.kryptonLabel16.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel16.TabIndex = 52;
            this.kryptonLabel16.Target = this._ask;
            this.kryptonLabel16.Values.Text = "二级";
            // 
            // kryptonLabel15
            // 
            this.kryptonLabel15.Location = new System.Drawing.Point(123, 24);
            this.kryptonLabel15.Name = "kryptonLabel15";
            this.kryptonLabel15.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel15.TabIndex = 51;
            this.kryptonLabel15.Target = this._ask;
            this.kryptonLabel15.Values.Text = "一级";
            // 
            // trailing2
            // 
            this.trailing2.Location = new System.Drawing.Point(224, 45);
            this.trailing2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.trailing2.Name = "trailing2";
            this.trailing2.Size = new System.Drawing.Size(55, 20);
            this.trailing2.TabIndex = 50;
            this.trailing2.ValueChanged += new System.EventHandler(this.trailing2_ValueChanged);
            // 
            // start2
            // 
            this.start2.Location = new System.Drawing.Point(161, 45);
            this.start2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.start2.Name = "start2";
            this.start2.Size = new System.Drawing.Size(55, 20);
            this.start2.TabIndex = 49;
            this.start2.ValueChanged += new System.EventHandler(this.start2_ValueChanged);
            // 
            // trailing1
            // 
            this.trailing1.Location = new System.Drawing.Point(224, 22);
            this.trailing1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.trailing1.Name = "trailing1";
            this.trailing1.Size = new System.Drawing.Size(55, 20);
            this.trailing1.TabIndex = 48;
            this.trailing1.ValueChanged += new System.EventHandler(this.trailing1_ValueChanged);
            // 
            // kryptonLabel14
            // 
            this.kryptonLabel14.Location = new System.Drawing.Point(222, 3);
            this.kryptonLabel14.Name = "kryptonLabel14";
            this.kryptonLabel14.Size = new System.Drawing.Size(51, 18);
            this.kryptonLabel14.TabIndex = 47;
            this.kryptonLabel14.Target = this._ask;
            this.kryptonLabel14.Values.Text = "回吐值";
            // 
            // start1
            // 
            this.start1.Location = new System.Drawing.Point(161, 22);
            this.start1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.start1.Name = "start1";
            this.start1.Size = new System.Drawing.Size(55, 20);
            this.start1.TabIndex = 46;
            this.start1.ValueChanged += new System.EventHandler(this.start1_ValueChanged);
            // 
            // kryptonLabel13
            // 
            this.kryptonLabel13.Location = new System.Drawing.Point(159, 3);
            this.kryptonLabel13.Name = "kryptonLabel13";
            this.kryptonLabel13.Size = new System.Drawing.Size(51, 18);
            this.kryptonLabel13.TabIndex = 45;
            this.kryptonLabel13.Target = this._ask;
            this.kryptonLabel13.Values.Text = "起步值";
            // 
            // breakeven
            // 
            this.breakeven.Location = new System.Drawing.Point(64, 23);
            this.breakeven.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.breakeven.Name = "breakeven";
            this.breakeven.Size = new System.Drawing.Size(55, 20);
            this.breakeven.TabIndex = 44;
            this.breakeven.ValueChanged += new System.EventHandler(this.breakeven_ValueChanged);
            // 
            // kryptonLabel12
            // 
            this.kryptonLabel12.Location = new System.Drawing.Point(62, 3);
            this.kryptonLabel12.Name = "kryptonLabel12";
            this.kryptonLabel12.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel12.TabIndex = 43;
            this.kryptonLabel12.Target = this._ask;
            this.kryptonLabel12.Values.Text = "保本";
            // 
            // stoploss
            // 
            this.stoploss.Location = new System.Drawing.Point(3, 23);
            this.stoploss.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.stoploss.Name = "stoploss";
            this.stoploss.Size = new System.Drawing.Size(55, 20);
            this.stoploss.TabIndex = 42;
            this.stoploss.ValueChanged += new System.EventHandler(this.stoploss_ValueChanged);
            // 
            // kryptonLabel11
            // 
            this.kryptonLabel11.Location = new System.Drawing.Point(1, 3);
            this.kryptonLabel11.Name = "kryptonLabel11";
            this.kryptonLabel11.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel11.TabIndex = 41;
            this.kryptonLabel11.Target = this._ask;
            this.kryptonLabel11.Values.Text = "止损";
            // 
            // _profitTakestop
            // 
            this._profitTakestop.Location = new System.Drawing.Point(212, 85);
            this._profitTakestop.Name = "_profitTakestop";
            this._profitTakestop.Size = new System.Drawing.Size(19, 18);
            this._profitTakestop.TabIndex = 49;
            this._profitTakestop.Target = this._ask;
            this._profitTakestop.Values.Text = "--";
            // 
            // _breakEven
            // 
            this._breakEven.Location = new System.Drawing.Point(212, 67);
            this._breakEven.Name = "_breakEven";
            this._breakEven.Size = new System.Drawing.Size(19, 18);
            this._breakEven.TabIndex = 48;
            this._breakEven.Target = this._ask;
            this._breakEven.Values.Text = "--";
            // 
            // _stoploss
            // 
            this._stoploss.Location = new System.Drawing.Point(212, 49);
            this._stoploss.Name = "_stoploss";
            this._stoploss.Size = new System.Drawing.Size(19, 18);
            this._stoploss.TabIndex = 47;
            this._stoploss.Target = this._ask;
            this._stoploss.Values.Text = "--";
            // 
            // _adverse
            // 
            this._adverse.Location = new System.Drawing.Point(212, 31);
            this._adverse.Name = "_adverse";
            this._adverse.Size = new System.Drawing.Size(19, 18);
            this._adverse.TabIndex = 46;
            this._adverse.Target = this._ask;
            this._adverse.Values.Text = "--";
            // 
            // _favor
            // 
            this._favor.Location = new System.Drawing.Point(212, 13);
            this._favor.Name = "_favor";
            this._favor.Size = new System.Drawing.Size(19, 18);
            this._favor.TabIndex = 45;
            this._favor.Target = this._ask;
            this._favor.Values.Text = "--";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(170, 85);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel5.TabIndex = 44;
            this.kryptonLabel5.Values.Text = "止盈:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(170, 67);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 43;
            this.kryptonLabel4.Values.Text = "保本:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(170, 49);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 42;
            this.kryptonLabel3.Values.Text = "止损:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(170, 31);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 41;
            this.kryptonLabel2.Values.Text = "最差:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(170, 13);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 40;
            this.kryptonLabel1.Target = this._ask;
            this.kryptonLabel1.Values.Text = "最优:";
            // 
            // kryptonGroupBox3
            // 
            this.kryptonGroupBox3.Location = new System.Drawing.Point(3, 3);
            this.kryptonGroupBox3.Name = "kryptonGroupBox3";
            // 
            // kryptonGroupBox3.Panel
            // 
            this.kryptonGroupBox3.Panel.Controls.Add(this.panel2);
            this.kryptonGroupBox3.Size = new System.Drawing.Size(161, 118);
            this.kryptonGroupBox3.TabIndex = 39;
            this.kryptonGroupBox3.Text = "行 情";
            this.kryptonGroupBox3.Values.Heading = "行 情";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this._ask);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this._bid);
            this.panel2.Controls.Add(this.bidSize);
            this.panel2.Controls.Add(this._last);
            this.panel2.Controls.Add(this._unrealizedpl);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.askSize);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this._avgcost);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(151, 84);
            this.panel2.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.Aqua;
            this.label14.Location = new System.Drawing.Point(1, 2);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "买";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.Aqua;
            this.label15.Location = new System.Drawing.Point(80, 2);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "卖";
            // 
            // _bid
            // 
            this._bid.AutoSize = true;
            this._bid.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bid.ForeColor = System.Drawing.Color.Yellow;
            this._bid.Location = new System.Drawing.Point(18, 5);
            this._bid.Name = "_bid";
            this._bid.Size = new System.Drawing.Size(18, 16);
            this._bid.TabIndex = 2;
            this._bid.Text = "--";
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
            // _last
            // 
            this._last.AutoSize = true;
            this._last.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._last.ForeColor = System.Drawing.Color.Yellow;
            this._last.Location = new System.Drawing.Point(28, 40);
            this._last.Name = "_last";
            this._last.Size = new System.Drawing.Size(18, 16);
            this._last.TabIndex = 27;
            this._last.Text = "--";
            // 
            // _unrealizedpl
            // 
            this._unrealizedpl.AutoSize = true;
            this._unrealizedpl.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._unrealizedpl.ForeColor = System.Drawing.Color.Yellow;
            this._unrealizedpl.Location = new System.Drawing.Point(114, 62);
            this._unrealizedpl.Name = "_unrealizedpl";
            this._unrealizedpl.Size = new System.Drawing.Size(18, 16);
            this._unrealizedpl.TabIndex = 20;
            this._unrealizedpl.Text = "--";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.Aqua;
            this.label16.Location = new System.Drawing.Point(86, 60);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 19;
            this.label16.Text = "浮差";
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
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.Color.Aqua;
            this.label17.Location = new System.Drawing.Point(0, 38);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 26;
            this.label17.Text = "成交";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.Color.Aqua;
            this.label18.Location = new System.Drawing.Point(0, 60);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(29, 12);
            this.label18.TabIndex = 17;
            this.label18.Text = "成本";
            // 
            // _avgcost
            // 
            this._avgcost.AutoSize = true;
            this._avgcost.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._avgcost.ForeColor = System.Drawing.Color.Yellow;
            this._avgcost.Location = new System.Drawing.Point(28, 62);
            this._avgcost.Name = "_avgcost";
            this._avgcost.Size = new System.Drawing.Size(18, 16);
            this._avgcost.TabIndex = 18;
            this._avgcost.Text = "--";
            // 
            // btnmemo
            // 
            this.btnmemo.Location = new System.Drawing.Point(4, 225);
            this.btnmemo.Name = "btnmemo";
            this.btnmemo.Size = new System.Drawing.Size(41, 18);
            this.btnmemo.TabIndex = 52;
            this.btnmemo.Values.Text = "说 明";
            this.btnmemo.LinkClicked += new System.EventHandler(this.btnmemo_LinkClicked);
            // 
            // fmStopTrailing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 292);
            this.ControlBox = false;
            this.Controls.Add(this.kryptonPanel1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmStopTrailing";
            this.Text = "动态止损止盈";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fmStopTrailing_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            this.kryptonGroupBox3.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox3)).EndInit();
            this.kryptonGroupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        //private TradingLib.GUI.ctTimesLineChart ctTimesLineChart1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.Label _ask;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label _bid;
        private System.Windows.Forms.Label bidSize;
        private System.Windows.Forms.Label _last;
        private System.Windows.Forms.Label _unrealizedpl;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label askSize;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label _avgcost;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _profitTakestop;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _breakEven;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _stoploss;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _adverse;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _favor;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel16;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel15;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown trailing2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown start2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown trailing1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel14;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown start1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel13;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown breakeven;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel12;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown stoploss;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel11;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _pos;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown _osize;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel btnmemo;

    }
}