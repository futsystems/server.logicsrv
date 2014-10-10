namespace FutsMoniter
{
    partial class AgentProfitReportForm
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
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.TotalReportPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.TotalReportDayRangePage = new Telerik.WinControls.UI.RadPageViewPage();
            this.DetailReportPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctProfitReportOneDay1 = new FutsMoniter.ctProfitReportOneDay();
            this.ctProfitReportDayRange1 = new FutsMoniter.ctProfitReportDayRange();
            this.ctDetailReportByAccount1 = new FutsMoniter.ctDetailReportByAccount();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.TotalReportPage.SuspendLayout();
            this.TotalReportDayRangePage.SuspendLayout();
            this.DetailReportPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.TotalReportPage);
            this.radPageView1.Controls.Add(this.TotalReportDayRangePage);
            this.radPageView1.Controls.Add(this.DetailReportPage);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView1.Location = new System.Drawing.Point(0, 0);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.TotalReportPage;
            this.radPageView1.Size = new System.Drawing.Size(792, 470);
            this.radPageView1.TabIndex = 43;
            this.radPageView1.Text = "radPageView1";
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.radPageView1.GetChildAt(0))).StripButtons = Telerik.WinControls.UI.StripViewButtons.None;
            // 
            // TotalReportPage
            // 
            this.TotalReportPage.Controls.Add(this.ctProfitReportOneDay1);
            this.TotalReportPage.Location = new System.Drawing.Point(10, 31);
            this.TotalReportPage.Name = "TotalReportPage";
            this.TotalReportPage.Size = new System.Drawing.Size(771, 428);
            this.TotalReportPage.Text = "单日汇总";
            // 
            // TotalReportDayRangePage
            // 
            this.TotalReportDayRangePage.Controls.Add(this.ctProfitReportDayRange1);
            this.TotalReportDayRangePage.Location = new System.Drawing.Point(10, 31);
            this.TotalReportDayRangePage.Name = "TotalReportDayRangePage";
            this.TotalReportDayRangePage.Size = new System.Drawing.Size(771, 428);
            this.TotalReportDayRangePage.Text = "多日流水";
            // 
            // DetailReportPage
            // 
            this.DetailReportPage.Controls.Add(this.ctDetailReportByAccount1);
            this.DetailReportPage.Location = new System.Drawing.Point(10, 31);
            this.DetailReportPage.Name = "DetailReportPage";
            this.DetailReportPage.Size = new System.Drawing.Size(771, 428);
            this.DetailReportPage.Text = "单日帐户明细";
            // 
            // ctProfitReportOneDay1
            // 
            this.ctProfitReportOneDay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctProfitReportOneDay1.Location = new System.Drawing.Point(0, 0);
            this.ctProfitReportOneDay1.Name = "ctProfitReportOneDay1";
            this.ctProfitReportOneDay1.Size = new System.Drawing.Size(771, 428);
            this.ctProfitReportOneDay1.TabIndex = 0;
            // 
            // ctProfitReportDayRange1
            // 
            this.ctProfitReportDayRange1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctProfitReportDayRange1.Location = new System.Drawing.Point(0, 0);
            this.ctProfitReportDayRange1.Name = "ctProfitReportDayRange1";
            this.ctProfitReportDayRange1.Size = new System.Drawing.Size(771, 428);
            this.ctProfitReportDayRange1.TabIndex = 0;
            // 
            // ctDetailReportByAccount1
            // 
            this.ctDetailReportByAccount1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDetailReportByAccount1.Location = new System.Drawing.Point(0, 0);
            this.ctDetailReportByAccount1.Name = "ctDetailReportByAccount1";
            this.ctDetailReportByAccount1.Size = new System.Drawing.Size(771, 428);
            this.ctDetailReportByAccount1.TabIndex = 0;
            // 
            // AgentProfitReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.radPageView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgentProfitReportForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "代理商分润报表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AgentProfitReportForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.TotalReportPage.ResumeLayout(false);
            this.TotalReportDayRangePage.ResumeLayout(false);
            this.DetailReportPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage TotalReportPage;
        private Telerik.WinControls.UI.RadPageViewPage DetailReportPage;
        private Telerik.WinControls.UI.RadPageViewPage TotalReportDayRangePage;
        private ctProfitReportOneDay ctProfitReportOneDay1;
        private ctProfitReportDayRange ctProfitReportDayRange1;
        private ctDetailReportByAccount ctDetailReportByAccount1;
    }
}