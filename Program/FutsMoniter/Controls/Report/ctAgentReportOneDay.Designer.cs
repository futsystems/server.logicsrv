﻿namespace FutsMoniter
{
    partial class ctAgentReportOneDay
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
            this.totalgrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.q_settleday = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.btnQryReport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnQryReport);
            this.kryptonPanel1.Controls.Add(this.q_settleday);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.ctAgentList1);
            this.kryptonPanel1.Controls.Add(this.totalgrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(629, 340);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // totalgrid
            // 
            this.totalgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.totalgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.totalgrid.Location = new System.Drawing.Point(0, 0);
            this.totalgrid.Name = "totalgrid";
            this.totalgrid.RowTemplate.Height = 23;
            this.totalgrid.Size = new System.Drawing.Size(629, 300);
            this.totalgrid.TabIndex = 0;
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ctAgentList1.EnableAny = false;
            this.ctAgentList1.EnableDefaultBaseMGR = true;
            this.ctAgentList1.EnableSelected = true;
            this.ctAgentList1.Location = new System.Drawing.Point(130, 306);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(183, 25);
            this.ctAgentList1.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonLabel1.Location = new System.Drawing.Point(320, 310);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "结算日:";
            // 
            // q_settleday
            // 
            this.q_settleday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.q_settleday.Location = new System.Drawing.Point(381, 308);
            this.q_settleday.Name = "q_settleday";
            this.q_settleday.Size = new System.Drawing.Size(135, 20);
            this.q_settleday.TabIndex = 3;
            // 
            // btnQryReport
            // 
            this.btnQryReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQryReport.Location = new System.Drawing.Point(538, 306);
            this.btnQryReport.Name = "btnQryReport";
            this.btnQryReport.Size = new System.Drawing.Size(70, 25);
            this.btnQryReport.TabIndex = 4;
            this.btnQryReport.Values.Text = "查 询";
            // 
            // ctAgentReportOneDay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctAgentReportOneDay";
            this.Size = new System.Drawing.Size(629, 340);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView totalgrid;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ctAgentList ctAgentList1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQryReport;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker q_settleday;
    }
}