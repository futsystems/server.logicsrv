namespace FutsMoniter
{
    partial class fmInsertTrade
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.cboffsetflag = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.price = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.symbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.size = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.cbside = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.timestr = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbside)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel8);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.cboffsetflag);
            this.kryptonPanel1.Controls.Add(this.price);
            this.kryptonPanel1.Controls.Add(this.symbol);
            this.kryptonPanel1.Controls.Add(this.size);
            this.kryptonPanel1.Controls.Add(this.cbside);
            this.kryptonPanel1.Controls.Add(this.timestr);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(299, 262);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(199, 33);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(94, 18);
            this.kryptonLabel8.TabIndex = 15;
            this.kryptonLabel8.Values.Text = "格式(时:分:秒)";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(227, 225);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(60, 25);
            this.btnSubmit.TabIndex = 14;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // cboffsetflag
            // 
            this.cboffsetflag.DropDownWidth = 100;
            this.cboffsetflag.Location = new System.Drawing.Point(87, 168);
            this.cboffsetflag.Name = "cboffsetflag";
            this.cboffsetflag.Size = new System.Drawing.Size(100, 21);
            this.cboffsetflag.TabIndex = 13;
            // 
            // price
            // 
            this.price.Location = new System.Drawing.Point(87, 142);
            this.price.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.price.Name = "price";
            this.price.Size = new System.Drawing.Size(100, 20);
            this.price.TabIndex = 12;
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(87, 114);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(100, 21);
            this.symbol.TabIndex = 11;
            // 
            // size
            // 
            this.size.Location = new System.Drawing.Point(87, 88);
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(100, 20);
            this.size.TabIndex = 10;
            // 
            // cbside
            // 
            this.cbside.DropDownWidth = 100;
            this.cbside.Location = new System.Drawing.Point(87, 61);
            this.cbside.Name = "cbside";
            this.cbside.Size = new System.Drawing.Size(100, 21);
            this.cbside.TabIndex = 9;
            // 
            // timestr
            // 
            this.timestr.Location = new System.Drawing.Point(87, 33);
            this.timestr.Name = "timestr";
            this.timestr.Size = new System.Drawing.Size(100, 21);
            this.timestr.TabIndex = 8;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(87, 13);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(19, 18);
            this.account.TabIndex = 7;
            this.account.Values.Text = "--";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(39, 171);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel7.TabIndex = 6;
            this.kryptonLabel7.Values.Text = "开平:";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(13, 144);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel6.TabIndex = 5;
            this.kryptonLabel6.Values.Text = "成交价格:";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(39, 118);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel5.TabIndex = 4;
            this.kryptonLabel5.Values.Text = "合约:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(39, 90);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 3;
            this.kryptonLabel4.Values.Text = "手数:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(39, 64);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "买卖:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(13, 37);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "提交时间:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(13, 13);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "交易帐户:";
            // 
            // fmInsertTrade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 262);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmInsertTrade";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插入成交";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbside)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown size;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbside;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox timestr;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel account;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown price;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cboffsetflag;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
    }
}