namespace TradingLib.Quant.GUI
{
    partial class ctStrategySimulation
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
            this.clearLog = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.showTradeInChart = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.showBackTestReport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.startSimulation = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ctDebug1 = new TradingLib.Quant.GUI.ctDebug();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._progress = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.clearLog);
            this.kryptonPanel1.Controls.Add(this.showTradeInChart);
            this.kryptonPanel1.Controls.Add(this.showBackTestReport);
            this.kryptonPanel1.Controls.Add(this.startSimulation);
            this.kryptonPanel1.Controls.Add(this.ctDebug1);
            this.kryptonPanel1.Controls.Add(this.statusStrip1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(690, 331);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // clearLog
            // 
            this.clearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearLog.Location = new System.Drawing.Point(381, 3);
            this.clearLog.Name = "clearLog";
            this.clearLog.Size = new System.Drawing.Size(72, 22);
            this.clearLog.TabIndex = 9;
            this.clearLog.Values.Text = "清空日志";
            this.clearLog.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // showTradeInChart
            // 
            this.showTradeInChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showTradeInChart.Enabled = false;
            this.showTradeInChart.Location = new System.Drawing.Point(615, 3);
            this.showTradeInChart.Name = "showTradeInChart";
            this.showTradeInChart.Size = new System.Drawing.Size(72, 22);
            this.showTradeInChart.TabIndex = 8;
            this.showTradeInChart.Values.Text = "查看信号";
            this.showTradeInChart.Click += new System.EventHandler(this.showTradeInChart_Click);
            // 
            // showBackTestReport
            // 
            this.showBackTestReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showBackTestReport.Enabled = false;
            this.showBackTestReport.Location = new System.Drawing.Point(537, 3);
            this.showBackTestReport.Name = "showBackTestReport";
            this.showBackTestReport.Size = new System.Drawing.Size(72, 22);
            this.showBackTestReport.TabIndex = 7;
            this.showBackTestReport.Values.Text = "查看报告";
            this.showBackTestReport.Click += new System.EventHandler(this.showBackTestReport_Click);
            // 
            // startSimulation
            // 
            this.startSimulation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startSimulation.Location = new System.Drawing.Point(459, 3);
            this.startSimulation.Name = "startSimulation";
            this.startSimulation.Size = new System.Drawing.Size(72, 22);
            this.startSimulation.TabIndex = 6;
            this.startSimulation.Values.Text = "开始回测";
            this.startSimulation.Click += new System.EventHandler(this.startSimulation_Click);
            // 
            // ctDebug1
            // 
            this.ctDebug1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ctDebug1.ExternalTimeStamp = 0;
            this.ctDebug1.Location = new System.Drawing.Point(0, 31);
            this.ctDebug1.Name = "ctDebug1";
            this.ctDebug1.Size = new System.Drawing.Size(690, 277);
            this.ctDebug1.TabIndex = 5;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this._progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 309);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(690, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(55, 17);
            this.toolStripStatusLabel1.Text = "回测进度";
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(200, 16);
            // 
            // ctStrategySimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctStrategySimulation";
            this.Size = new System.Drawing.Size(690, 331);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar _progress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ctDebug ctDebug1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton startSimulation;
        private ComponentFactory.Krypton.Toolkit.KryptonButton showTradeInChart;
        private ComponentFactory.Krypton.Toolkit.KryptonButton showBackTestReport;
        private ComponentFactory.Krypton.Toolkit.KryptonButton clearLog;
    }
}
