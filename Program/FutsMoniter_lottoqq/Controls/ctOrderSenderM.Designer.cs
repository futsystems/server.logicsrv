namespace FutsMoniter
{
    partial class ctOrderSenderM
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnInsertTrade = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSell = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnBuy = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.price = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.size = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.cboffsetflag = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.cbordertype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.symbol = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbordertype)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnInsertTrade);
            this.kryptonPanel1.Controls.Add(this.btnSell);
            this.kryptonPanel1.Controls.Add(this.btnBuy);
            this.kryptonPanel1.Controls.Add(this.price);
            this.kryptonPanel1.Controls.Add(this.size);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel8);
            this.kryptonPanel1.Controls.Add(this.cboffsetflag);
            this.kryptonPanel1.Controls.Add(this.cbordertype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.symbol);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(616, 57);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // btnInsertTrade
            // 
            this.btnInsertTrade.Location = new System.Drawing.Point(544, 4);
            this.btnInsertTrade.Name = "btnInsertTrade";
            this.btnInsertTrade.Size = new System.Drawing.Size(66, 48);
            this.btnInsertTrade.StateCommon.Content.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnInsertTrade.TabIndex = 14;
            this.btnInsertTrade.Values.Text = "插 入";
            // 
            // btnSell
            // 
            this.btnSell.Location = new System.Drawing.Point(472, 4);
            this.btnSell.Name = "btnSell";
            this.btnSell.Size = new System.Drawing.Size(66, 48);
            this.btnSell.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.LimeGreen;
            this.btnSell.StateCommon.Content.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSell.TabIndex = 13;
            this.btnSell.Values.Text = "卖 出";
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(400, 4);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(66, 48);
            this.btnBuy.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.Crimson;
            this.btnBuy.StateCommon.Content.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBuy.TabIndex = 12;
            this.btnBuy.Values.Text = "买 入";
            // 
            // price
            // 
            this.price.Location = new System.Drawing.Point(320, 32);
            this.price.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.price.Name = "price";
            this.price.Size = new System.Drawing.Size(74, 20);
            this.price.TabIndex = 11;
            // 
            // size
            // 
            this.size.Location = new System.Drawing.Point(320, 4);
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(74, 20);
            this.size.TabIndex = 10;
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(281, 34);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel7.TabIndex = 9;
            this.kryptonLabel7.Values.Text = "价格:";
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(281, 7);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel8.TabIndex = 8;
            this.kryptonLabel8.Values.Text = "数量:";
            // 
            // cboffsetflag
            // 
            this.cboffsetflag.DropDownWidth = 89;
            this.cboffsetflag.Location = new System.Drawing.Point(159, 31);
            this.cboffsetflag.Name = "cboffsetflag";
            this.cboffsetflag.Size = new System.Drawing.Size(116, 21);
            this.cboffsetflag.TabIndex = 7;
            this.cboffsetflag.Text = "--";
            // 
            // cbordertype
            // 
            this.cbordertype.DropDownWidth = 89;
            this.cbordertype.Location = new System.Drawing.Point(186, 4);
            this.cbordertype.Name = "cbordertype";
            this.cbordertype.Size = new System.Drawing.Size(89, 21);
            this.cbordertype.TabIndex = 6;
            this.cbordertype.Text = "--";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(112, 34);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel6.TabIndex = 5;
            this.kryptonLabel6.Values.Text = "开平:";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(112, 7);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 4;
            this.kryptonLabel5.Values.Text = "委托类型:";
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(43, 38);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(23, 16);
            this.symbol.StateCommon.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.symbol.TabIndex = 3;
            this.symbol.Values.Text = "--";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(4, 34);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 2;
            this.kryptonLabel4.Values.Text = "合约:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(43, 11);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(23, 16);
            this.account.StateCommon.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.account.TabIndex = 1;
            this.account.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(4, 7);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "帐户:";
            // 
            // ctOrderSenderM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctOrderSenderM";
            this.Size = new System.Drawing.Size(616, 57);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbordertype)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbordertype;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel account;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cboffsetflag;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnInsertTrade;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSell;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnBuy;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown price;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown size;
    }
}
