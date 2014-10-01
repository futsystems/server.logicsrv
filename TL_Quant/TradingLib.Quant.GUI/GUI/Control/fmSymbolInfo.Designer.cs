namespace TradingLib.Quant.GUI
{
    partial class fmSymbolInfo
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.symbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.type = new System.Windows.Forms.ComboBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.exchange = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.muplite = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pricetick = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel9 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel10 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.entrycommission = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.exitcommission = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.ok = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ok);
            this.kryptonPanel1.Controls.Add(this.exitcommission);
            this.kryptonPanel1.Controls.Add(this.entrycommission);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel10);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel9);
            this.kryptonPanel1.Controls.Add(this.pricetick);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel1.Controls.Add(this.muplite);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.exchange);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.type);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.symbol);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(293, 306);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 16);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(65, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "合约代码";
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(116, 12);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(100, 21);
            this.symbol.TabIndex = 1;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(12, 40);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(65, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "合约类别";
            // 
            // type
            // 
            this.type.FormattingEnabled = true;
            this.type.Location = new System.Drawing.Point(116, 40);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(100, 20);
            this.type.TabIndex = 3;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(15, 66);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(51, 18);
            this.kryptonLabel3.TabIndex = 4;
            this.kryptonLabel3.Values.Text = "交易所";
            // 
            // exchange
            // 
            this.exchange.Location = new System.Drawing.Point(116, 66);
            this.exchange.Name = "exchange";
            this.exchange.Size = new System.Drawing.Size(19, 18);
            this.exchange.TabIndex = 5;
            this.exchange.Values.Text = "--";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(12, 90);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(65, 18);
            this.kryptonLabel5.TabIndex = 6;
            this.kryptonLabel5.Values.Text = "合约乘数";
            // 
            // muplite
            // 
            this.muplite.Location = new System.Drawing.Point(116, 90);
            this.muplite.Name = "muplite";
            this.muplite.Size = new System.Drawing.Size(19, 18);
            this.muplite.TabIndex = 7;
            this.muplite.Values.Text = "--";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(15, 114);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(24, 18);
            this.kryptonLabel7.TabIndex = 8;
            this.kryptonLabel7.Values.Text = "跳";
            // 
            // pricetick
            // 
            this.pricetick.Location = new System.Drawing.Point(116, 114);
            this.pricetick.Name = "pricetick";
            this.pricetick.Size = new System.Drawing.Size(19, 18);
            this.pricetick.TabIndex = 9;
            this.pricetick.Values.Text = "--";
            // 
            // kryptonLabel9
            // 
            this.kryptonLabel9.Location = new System.Drawing.Point(12, 142);
            this.kryptonLabel9.Name = "kryptonLabel9";
            this.kryptonLabel9.Size = new System.Drawing.Size(78, 18);
            this.kryptonLabel9.TabIndex = 10;
            this.kryptonLabel9.Values.Text = "开仓手续费";
            // 
            // kryptonLabel10
            // 
            this.kryptonLabel10.Location = new System.Drawing.Point(12, 166);
            this.kryptonLabel10.Name = "kryptonLabel10";
            this.kryptonLabel10.Size = new System.Drawing.Size(78, 18);
            this.kryptonLabel10.TabIndex = 11;
            this.kryptonLabel10.Values.Text = "平仓手续费";
            // 
            // entrycommission
            // 
            this.entrycommission.Location = new System.Drawing.Point(116, 138);
            this.entrycommission.Name = "entrycommission";
            this.entrycommission.Size = new System.Drawing.Size(100, 21);
            this.entrycommission.TabIndex = 12;
            // 
            // exitcommission
            // 
            this.exitcommission.Location = new System.Drawing.Point(116, 166);
            this.exitcommission.Name = "exitcommission";
            this.exitcommission.Size = new System.Drawing.Size(100, 22);
            this.exitcommission.TabIndex = 13;
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(209, 281);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(81, 22);
            this.ok.TabIndex = 1;
            this.ok.Values.Text = "保存修改";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // fmSymbolInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 306);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmSymbolInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "合约信息";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private System.Windows.Forms.ComboBox type;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel pricetick;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel muplite;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel exchange;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel10;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel9;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox exitcommission;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox entrycommission;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ok;
    }
}