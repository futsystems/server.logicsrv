namespace TradingLib.Quant.GUI
{
    partial class StrategySimulation
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._progress = new System.Windows.Forms.ToolStripProgressBar();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.ShowResults = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.clearDebug = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.debugEnable = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.strategyClassName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.strategyFriendlyName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.debugControl1 = new TradingLib.GUI.DebugControl();
            this.startSimulation = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.msg = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.showBackTestReport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this._progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 268);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(502, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel1.Text = "进度";
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(250, 16);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.showBackTestReport);
            this.kryptonPanel1.Controls.Add(this.ShowResults);
            this.kryptonPanel1.Controls.Add(this.clearDebug);
            this.kryptonPanel1.Controls.Add(this.debugEnable);
            this.kryptonPanel1.Controls.Add(this.strategyClassName);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.strategyFriendlyName);
            this.kryptonPanel1.Controls.Add(this.debugControl1);
            this.kryptonPanel1.Controls.Add(this.startSimulation);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.msg);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(502, 268);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // ShowResults
            // 
            this.ShowResults.Location = new System.Drawing.Point(430, 0);
            this.ShowResults.Name = "ShowResults";
            this.ShowResults.Size = new System.Drawing.Size(70, 22);
            this.ShowResults.TabIndex = 10;
            this.ShowResults.Values.Text = "显示结果";
            this.ShowResults.Click += new System.EventHandler(this.ShowResults_Click);
            // 
            // clearDebug
            // 
            this.clearDebug.Location = new System.Drawing.Point(429, 27);
            this.clearDebug.Name = "clearDebug";
            this.clearDebug.Size = new System.Drawing.Size(70, 22);
            this.clearDebug.TabIndex = 9;
            this.clearDebug.Values.Text = "清除日志";
            this.clearDebug.Click += new System.EventHandler(this.clearDebug_Click);
            // 
            // debugEnable
            // 
            this.debugEnable.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.debugEnable.Location = new System.Drawing.Point(181, 27);
            this.debugEnable.Name = "debugEnable";
            this.debugEnable.Size = new System.Drawing.Size(78, 18);
            this.debugEnable.TabIndex = 8;
            this.debugEnable.Text = "打印日志";
            this.debugEnable.Values.Text = "打印日志";
            // 
            // strategyClassName
            // 
            this.strategyClassName.Location = new System.Drawing.Point(246, 3);
            this.strategyClassName.Name = "strategyClassName";
            this.strategyClassName.Size = new System.Drawing.Size(19, 18);
            this.strategyClassName.TabIndex = 7;
            this.strategyClassName.Values.Text = "--";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(181, 3);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 6;
            this.kryptonLabel3.Values.Text = "策略配置:";
            // 
            // strategyFriendlyName
            // 
            this.strategyFriendlyName.Location = new System.Drawing.Point(73, 3);
            this.strategyFriendlyName.Name = "strategyFriendlyName";
            this.strategyFriendlyName.Size = new System.Drawing.Size(19, 18);
            this.strategyFriendlyName.TabIndex = 5;
            this.strategyFriendlyName.Values.Text = "--";
            // 
            // debugControl1
            // 
            this.debugControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.debugControl1.EnableSearching = true;
            this.debugControl1.ExternalTimeStamp = 0;
            this.debugControl1.Location = new System.Drawing.Point(0, 50);
            this.debugControl1.Margin = new System.Windows.Forms.Padding(2);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(500, 216);
            this.debugControl1.TabIndex = 2;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // startSimulation
            // 
            this.startSimulation.Location = new System.Drawing.Point(265, 27);
            this.startSimulation.Name = "startSimulation";
            this.startSimulation.Size = new System.Drawing.Size(70, 22);
            this.startSimulation.TabIndex = 2;
            this.startSimulation.Values.Text = "开始回测";
            this.startSimulation.Click += new System.EventHandler(this.startSimulation_Click);
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(26, 27);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 4;
            this.kryptonLabel2.Values.Text = "信息:";
            // 
            // msg
            // 
            this.msg.Location = new System.Drawing.Point(73, 27);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(19, 18);
            this.msg.TabIndex = 3;
            this.msg.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "策略配置:";
            // 
            // showBackTestReport
            // 
            this.showBackTestReport.Location = new System.Drawing.Point(341, 27);
            this.showBackTestReport.Name = "showBackTestReport";
            this.showBackTestReport.Size = new System.Drawing.Size(70, 22);
            this.showBackTestReport.TabIndex = 11;
            this.showBackTestReport.Values.Text = "回测报告";
            this.showBackTestReport.Click += new System.EventHandler(this.showBackTestReport_Click);
            // 
            // StrategySimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 290);
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StrategySimulation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统回测";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar _progress;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel msg;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton startSimulation;
        private TradingLib.GUI.DebugControl debugControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel strategyFriendlyName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel strategyClassName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox debugEnable;
        private ComponentFactory.Krypton.Toolkit.KryptonButton clearDebug;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ShowResults;
        private ComponentFactory.Krypton.Toolkit.KryptonButton showBackTestReport;
    }
}