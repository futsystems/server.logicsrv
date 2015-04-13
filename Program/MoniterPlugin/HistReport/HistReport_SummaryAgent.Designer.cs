namespace TradingLib.HistReport
{
    partial class HistReport_SummaryAgent
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
            this.btnQry = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.gridAgentSummary = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.end_agent = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.start_agent = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.ctAgentList1 = new TradingLib.MoniterControl.ctAgentList();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgentSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.btnQry);
            this.kryptonPanel1.Controls.Add(this.gridAgentSummary);
            this.kryptonPanel1.Controls.Add(this.end_agent);
            this.kryptonPanel1.Controls.Add(this.start_agent);
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(804, 527);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnQry
            // 
            this.btnQry.Location = new System.Drawing.Point(586, 2);
            this.btnQry.Name = "btnQry";
            this.btnQry.Size = new System.Drawing.Size(70, 25);
            this.btnQry.TabIndex = 16;
            this.btnQry.Values.Text = "查 询";
            // 
            // gridAgentSummary
            // 
            this.gridAgentSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridAgentSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAgentSummary.Location = new System.Drawing.Point(3, 34);
            this.gridAgentSummary.Name = "gridAgentSummary";
            this.gridAgentSummary.RowTemplate.Height = 23;
            this.gridAgentSummary.Size = new System.Drawing.Size(798, 490);
            this.gridAgentSummary.TabIndex = 15;
            // 
            // end_agent
            // 
            this.end_agent.Location = new System.Drawing.Point(471, 7);
            this.end_agent.Name = "end_agent";
            this.end_agent.Size = new System.Drawing.Size(109, 20);
            this.end_agent.TabIndex = 14;
            // 
            // start_agent
            // 
            this.start_agent.Location = new System.Drawing.Point(289, 7);
            this.start_agent.Name = "start_agent";
            this.start_agent.Size = new System.Drawing.Size(109, 20);
            this.start_agent.TabIndex = 13;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.CurrentAgentFK = 0;
            this.ctAgentList1.EnableAny = false;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.EnableSelf = true;
            this.ctAgentList1.Location = new System.Drawing.Point(3, 7);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(211, 21);
            this.ctAgentList1.TabIndex = 12;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(404, 9);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 18;
            this.kryptonLabel2.Values.Text = "结束时间:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(220, 9);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 17;
            this.kryptonLabel1.Values.Text = "开始时间:";
            // 
            // HistReport_SummaryAgent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "HistReport_SummaryAgent";
            this.Size = new System.Drawing.Size(804, 527);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgentSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQry;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView gridAgentSummary;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker end_agent;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker start_agent;
        private MoniterControl.ctAgentList ctAgentList1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}
