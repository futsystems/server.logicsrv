namespace FutsMoniter
{
    partial class ctDetailReportByAccount
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
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnQryReport = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.settleday = new Telerik.WinControls.UI.RadDateTimePicker();
            this.agent = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.totalgrid = new Telerik.WinControls.UI.RadGridView();
            this.ctGridExport1 = new FutsMoniter.ctGridExport();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settleday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.ctGridExport1);
            this.radPanel1.Controls.Add(this.btnQryReport);
            this.radPanel1.Controls.Add(this.radLabel2);
            this.radPanel1.Controls.Add(this.settleday);
            this.radPanel1.Controls.Add(this.agent);
            this.radPanel1.Controls.Add(this.radLabel5);
            this.radPanel1.Controls.Add(this.totalgrid);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(621, 270);
            this.radPanel1.TabIndex = 0;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            // 
            // btnQryReport
            // 
            this.btnQryReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQryReport.Location = new System.Drawing.Point(551, 242);
            this.btnQryReport.Name = "btnQryReport";
            this.btnQryReport.Size = new System.Drawing.Size(58, 24);
            this.btnQryReport.TabIndex = 55;
            this.btnQryReport.Text = "查 询";
            this.btnQryReport.Click += new System.EventHandler(this.btnQryReport_Click_1);
            // 
            // radLabel2
            // 
            this.radLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel2.Location = new System.Drawing.Point(364, 244);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(47, 16);
            this.radLabel2.TabIndex = 54;
            this.radLabel2.Text = "结算日:";
            // 
            // settleday
            // 
            this.settleday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settleday.Location = new System.Drawing.Point(417, 242);
            this.settleday.Name = "settleday";
            this.settleday.Size = new System.Drawing.Size(117, 18);
            this.settleday.TabIndex = 53;
            this.settleday.TabStop = false;
            this.settleday.Text = "Monday, September 01, 2014";
            this.settleday.Value = new System.DateTime(2014, 9, 1, 15, 53, 33, 406);
            // 
            // agent
            // 
            this.agent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.agent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.agent.Location = new System.Drawing.Point(238, 242);
            this.agent.Name = "agent";
            this.agent.Size = new System.Drawing.Size(109, 18);
            this.agent.TabIndex = 52;
            this.agent.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.agent.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel5
            // 
            this.radLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel5.Location = new System.Drawing.Point(185, 244);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(47, 16);
            this.radLabel5.TabIndex = 51;
            this.radLabel5.Text = "代理商:";
            // 
            // totalgrid
            // 
            this.totalgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.totalgrid.Location = new System.Drawing.Point(0, 0);
            this.totalgrid.Name = "totalgrid";
            this.totalgrid.Size = new System.Drawing.Size(621, 241);
            this.totalgrid.TabIndex = 50;
            this.totalgrid.Text = "radGridView1";
            // 
            // ctGridExport1
            // 
            this.ctGridExport1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ctGridExport1.Grid = null;
            this.ctGridExport1.Location = new System.Drawing.Point(3, 242);
            this.ctGridExport1.Name = "ctGridExport1";
            this.ctGridExport1.Size = new System.Drawing.Size(110, 24);
            this.ctGridExport1.TabIndex = 56;
            // 
            // ctDetailReportByAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctDetailReportByAccount";
            this.Size = new System.Drawing.Size(621, 270);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settleday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton btnQryReport;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDateTimePicker settleday;
        private Telerik.WinControls.UI.RadDropDownList agent;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadGridView totalgrid;
        private ctGridExport ctGridExport1;
    }
}
