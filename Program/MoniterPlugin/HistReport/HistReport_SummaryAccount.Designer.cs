namespace TradingLib.HistReport
{
    partial class HistReport_SummaryAccount
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
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnQry = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.end_agent = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.start_agent = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.summaryAccountGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbAccount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbCashIn = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbCashOut = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.summaryAccountGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.lbCashOut);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel1.Controls.Add(this.lbCashIn);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.lbAccount);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.btnQry);
            this.kryptonPanel1.Controls.Add(this.end_agent);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.start_agent);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.summaryAccountGrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(759, 427);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(77, 9);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(98, 21);
            this.account.TabIndex = 12;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(3, 13);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 11;
            this.kryptonLabel3.Values.Text = "交易帐户:";
            // 
            // btnQry
            // 
            this.btnQry.Location = new System.Drawing.Point(548, 7);
            this.btnQry.Name = "btnQry";
            this.btnQry.Size = new System.Drawing.Size(70, 25);
            this.btnQry.TabIndex = 10;
            this.btnQry.Values.Text = "查 询";
            // 
            // end_agent
            // 
            this.end_agent.Location = new System.Drawing.Point(433, 11);
            this.end_agent.Name = "end_agent";
            this.end_agent.Size = new System.Drawing.Size(109, 20);
            this.end_agent.TabIndex = 9;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(366, 12);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 8;
            this.kryptonLabel2.Values.Text = "结束时间:";
            // 
            // start_agent
            // 
            this.start_agent.Location = new System.Drawing.Point(251, 11);
            this.start_agent.Name = "start_agent";
            this.start_agent.Size = new System.Drawing.Size(109, 20);
            this.start_agent.TabIndex = 7;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(182, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 6;
            this.kryptonLabel1.Values.Text = "开始时间:";
            // 
            // summaryAccountGrid
            // 
            this.summaryAccountGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.summaryAccountGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.summaryAccountGrid.Location = new System.Drawing.Point(3, 73);
            this.summaryAccountGrid.Name = "summaryAccountGrid";
            this.summaryAccountGrid.RowTemplate.Height = 23;
            this.summaryAccountGrid.Size = new System.Drawing.Size(756, 354);
            this.summaryAccountGrid.TabIndex = 0;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(3, 49);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 13;
            this.kryptonLabel4.Values.Text = "交易帐户:";
            // 
            // lbAccount
            // 
            this.lbAccount.Location = new System.Drawing.Point(77, 49);
            this.lbAccount.Name = "lbAccount";
            this.lbAccount.Size = new System.Drawing.Size(19, 18);
            this.lbAccount.TabIndex = 14;
            this.lbAccount.Values.Text = "--";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(177, 49);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 15;
            this.kryptonLabel5.Values.Text = "累计入金:";
            // 
            // lbCashIn
            // 
            this.lbCashIn.Location = new System.Drawing.Point(251, 49);
            this.lbCashIn.Name = "lbCashIn";
            this.lbCashIn.Size = new System.Drawing.Size(19, 18);
            this.lbCashIn.TabIndex = 16;
            this.lbCashIn.Values.Text = "--";
            // 
            // lbCashOut
            // 
            this.lbCashOut.Location = new System.Drawing.Point(440, 49);
            this.lbCashOut.Name = "lbCashOut";
            this.lbCashOut.Size = new System.Drawing.Size(19, 18);
            this.lbCashOut.TabIndex = 18;
            this.lbCashOut.Values.Text = "--";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(366, 49);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel6.TabIndex = 17;
            this.kryptonLabel6.Values.Text = "累计出金:";
            // 
            // HistReport_SummaryAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "HistReport_SummaryAccount";
            this.Size = new System.Drawing.Size(759, 427);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.summaryAccountGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox account;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQry;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker end_agent;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker start_agent;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView summaryAccountGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbAccount;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbCashOut;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbCashIn;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
    }
}
