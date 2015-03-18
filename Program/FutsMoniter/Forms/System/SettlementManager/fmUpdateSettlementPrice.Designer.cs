namespace FutsMoniter
{
    partial class fmUpdateSettlementPrice
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
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbSettleday = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbSymbol = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ndPrice = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.ndPrice);
            this.kryptonPanel1.Controls.Add(this.lbPrice);
            this.kryptonPanel1.Controls.Add(this.lbSymbol);
            this.kryptonPanel1.Controls.Add(this.lbSettleday);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(224, 165);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(32, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "结算日:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(45, 38);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(38, 20);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "合约:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(8, 64);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(75, 20);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "当前结算价:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(32, 90);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel4.TabIndex = 3;
            this.kryptonLabel4.Values.Text = "更新为:";
            // 
            // lbSettleday
            // 
            this.lbSettleday.Location = new System.Drawing.Point(89, 12);
            this.lbSettleday.Name = "lbSettleday";
            this.lbSettleday.Size = new System.Drawing.Size(20, 20);
            this.lbSettleday.TabIndex = 4;
            this.lbSettleday.Values.Text = "--";
            // 
            // lbSymbol
            // 
            this.lbSymbol.Location = new System.Drawing.Point(89, 38);
            this.lbSymbol.Name = "lbSymbol";
            this.lbSymbol.Size = new System.Drawing.Size(20, 20);
            this.lbSymbol.TabIndex = 5;
            this.lbSymbol.Values.Text = "--";
            // 
            // lbPrice
            // 
            this.lbPrice.Location = new System.Drawing.Point(89, 64);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(20, 20);
            this.lbPrice.TabIndex = 6;
            this.lbPrice.Values.Text = "--";
            // 
            // ndPrice
            // 
            this.ndPrice.DecimalPlaces = 2;
            this.ndPrice.Location = new System.Drawing.Point(89, 88);
            this.ndPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ndPrice.Name = "ndPrice";
            this.ndPrice.Size = new System.Drawing.Size(99, 22);
            this.ndPrice.TabIndex = 7;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(145, 128);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(67, 25);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // fmUpdateSettlementPrice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 165);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmUpdateSettlementPrice";
            this.Text = "更新结算价信息";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown ndPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbSymbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbSettleday;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
    }
}