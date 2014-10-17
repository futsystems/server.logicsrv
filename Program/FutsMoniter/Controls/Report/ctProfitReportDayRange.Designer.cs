namespace FutsMoniter
{
    partial class ctProfitReportDayRange
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
            this.totalgrid = new Telerik.WinControls.UI.RadGridView();
            this.btnQryReport = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.start = new Telerik.WinControls.UI.RadDateTimePicker();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.end = new Telerik.WinControls.UI.RadDateTimePicker();
            this.ctGridExport1 = new FutsMoniter.ctGridExport();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.start)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.end)).BeginInit();
            this.SuspendLayout();
            // 
            // totalgrid
            // 
            this.totalgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.totalgrid.Location = new System.Drawing.Point(0, 0);
            this.totalgrid.Name = "totalgrid";
            this.totalgrid.Size = new System.Drawing.Size(694, 281);
            this.totalgrid.TabIndex = 2;
            this.totalgrid.Text = "radGridView1";
            // 
            // btnQryReport
            // 
            this.btnQryReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQryReport.Location = new System.Drawing.Point(633, 285);
            this.btnQryReport.Name = "btnQryReport";
            this.btnQryReport.Size = new System.Drawing.Size(58, 24);
            this.btnQryReport.TabIndex = 54;
            this.btnQryReport.Text = "查 询";
            this.btnQryReport.Click += new System.EventHandler(this.btnQryReport_Click);
            // 
            // radLabel2
            // 
            this.radLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel2.Location = new System.Drawing.Point(305, 287);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 53;
            this.radLabel2.Text = "开始:";
            // 
            // start
            // 
            this.start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.start.Location = new System.Drawing.Point(345, 285);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(117, 18);
            this.start.TabIndex = 52;
            this.start.TabStop = false;
            this.start.Text = "Monday, September 01, 2014";
            this.start.Value = new System.DateTime(2014, 9, 1, 15, 53, 33, 406);
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel1.Location = new System.Drawing.Point(468, 287);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(34, 16);
            this.radLabel1.TabIndex = 56;
            this.radLabel1.Text = "结束:";
            // 
            // end
            // 
            this.end.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.end.Location = new System.Drawing.Point(508, 285);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(117, 18);
            this.end.TabIndex = 55;
            this.end.TabStop = false;
            this.end.Text = "Monday, September 01, 2014";
            this.end.Value = new System.DateTime(2014, 9, 1, 15, 53, 33, 406);
            // 
            // ctGridExport1
            // 
            this.ctGridExport1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ctGridExport1.Grid = null;
            this.ctGridExport1.Location = new System.Drawing.Point(3, 285);
            this.ctGridExport1.Name = "ctGridExport1";
            this.ctGridExport1.Size = new System.Drawing.Size(110, 24);
            this.ctGridExport1.TabIndex = 57;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ctAgentList1.EnableAny = false;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.Location = new System.Drawing.Point(137, 285);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(162, 20);
            this.ctAgentList1.TabIndex = 58;
            // 
            // ctProfitReportDayRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctAgentList1);
            this.Controls.Add(this.ctGridExport1);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.end);
            this.Controls.Add(this.btnQryReport);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.start);
            this.Controls.Add(this.totalgrid);
            this.Name = "ctProfitReportDayRange";
            this.Size = new System.Drawing.Size(694, 315);
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.start)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.end)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView totalgrid;
        private Telerik.WinControls.UI.RadButton btnQryReport;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDateTimePicker start;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDateTimePicker end;
        private ctGridExport ctGridExport1;
        private ctAgentList ctAgentList1;
    }
}
