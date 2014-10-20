namespace FutsMoniter
{
    partial class HistQryForm
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
            this.qrypage = new Telerik.WinControls.UI.RadPageView();
            this.historder = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctHistOrder1 = new FutsMoniter.ctHistOrder();
            this.histtrade = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctHistTrade1 = new FutsMoniter.ctHistTrade();
            this.histposition = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctHistPosition1 = new FutsMoniter.ctHistPosition();
            this.histcash = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctHistCashTransaction1 = new FutsMoniter.ctHistCashTransaction();
            this.histsettle = new Telerik.WinControls.UI.RadPageViewPage();
            this.settlebox = new System.Windows.Forms.RichTextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.account = new Telerik.WinControls.UI.RadTextBox();
            this.btnQryHist = new Telerik.WinControls.UI.RadButton();
            this.settleday = new Telerik.WinControls.UI.RadDateTimePicker();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.ctGridExport1 = new FutsMoniter.ctGridExport();
            ((System.ComponentModel.ISupportInitialize)(this.qrypage)).BeginInit();
            this.qrypage.SuspendLayout();
            this.historder.SuspendLayout();
            this.histtrade.SuspendLayout();
            this.histposition.SuspendLayout();
            this.histcash.SuspendLayout();
            this.histsettle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryHist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settleday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // qrypage
            // 
            this.qrypage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.qrypage.Controls.Add(this.historder);
            this.qrypage.Controls.Add(this.histtrade);
            this.qrypage.Controls.Add(this.histposition);
            this.qrypage.Controls.Add(this.histcash);
            this.qrypage.Controls.Add(this.histsettle);
            this.qrypage.Location = new System.Drawing.Point(0, 37);
            this.qrypage.Name = "qrypage";
            this.qrypage.SelectedPage = this.histsettle;
            this.qrypage.Size = new System.Drawing.Size(792, 433);
            this.qrypage.TabIndex = 0;
            this.qrypage.Text = "radPageView1";
            this.qrypage.SelectedPageChanged += new System.EventHandler(this.qrypage_SelectedPageChanged);
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.qrypage.GetChildAt(0))).StripButtons = Telerik.WinControls.UI.StripViewButtons.None;
            // 
            // historder
            // 
            this.historder.Controls.Add(this.ctHistOrder1);
            this.historder.Location = new System.Drawing.Point(10, 35);
            this.historder.Name = "historder";
            this.historder.Size = new System.Drawing.Size(771, 387);
            this.historder.Text = "委 托";
            // 
            // ctHistOrder1
            // 
            this.ctHistOrder1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistOrder1.Location = new System.Drawing.Point(0, 0);
            this.ctHistOrder1.Name = "ctHistOrder1";
            this.ctHistOrder1.Size = new System.Drawing.Size(771, 387);
            this.ctHistOrder1.TabIndex = 0;
            // 
            // histtrade
            // 
            this.histtrade.Controls.Add(this.ctHistTrade1);
            this.histtrade.Location = new System.Drawing.Point(10, 31);
            this.histtrade.Name = "histtrade";
            this.histtrade.Size = new System.Drawing.Size(1048, 439);
            this.histtrade.Text = "成 交";
            // 
            // ctHistTrade1
            // 
            this.ctHistTrade1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistTrade1.Location = new System.Drawing.Point(0, 0);
            this.ctHistTrade1.Name = "ctHistTrade1";
            this.ctHistTrade1.Size = new System.Drawing.Size(1048, 439);
            this.ctHistTrade1.TabIndex = 0;
            // 
            // histposition
            // 
            this.histposition.Controls.Add(this.ctHistPosition1);
            this.histposition.Location = new System.Drawing.Point(10, 31);
            this.histposition.Name = "histposition";
            this.histposition.Size = new System.Drawing.Size(1048, 439);
            this.histposition.Text = "持 仓";
            // 
            // ctHistPosition1
            // 
            this.ctHistPosition1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistPosition1.Location = new System.Drawing.Point(0, 0);
            this.ctHistPosition1.Name = "ctHistPosition1";
            this.ctHistPosition1.Size = new System.Drawing.Size(1048, 439);
            this.ctHistPosition1.TabIndex = 0;
            // 
            // histcash
            // 
            this.histcash.Controls.Add(this.ctHistCashTransaction1);
            this.histcash.Location = new System.Drawing.Point(10, 31);
            this.histcash.Name = "histcash";
            this.histcash.Size = new System.Drawing.Size(1048, 439);
            this.histcash.Text = "出入金";
            // 
            // ctHistCashTransaction1
            // 
            this.ctHistCashTransaction1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistCashTransaction1.Location = new System.Drawing.Point(0, 0);
            this.ctHistCashTransaction1.Name = "ctHistCashTransaction1";
            this.ctHistCashTransaction1.Size = new System.Drawing.Size(1048, 439);
            this.ctHistCashTransaction1.TabIndex = 0;
            // 
            // histsettle
            // 
            this.histsettle.Controls.Add(this.settlebox);
            this.histsettle.Location = new System.Drawing.Point(10, 35);
            this.histsettle.Name = "histsettle";
            this.histsettle.Size = new System.Drawing.Size(771, 387);
            this.histsettle.Text = "结算单";
            // 
            // settlebox
            // 
            this.settlebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settlebox.Location = new System.Drawing.Point(0, 0);
            this.settlebox.Name = "settlebox";
            this.settlebox.Size = new System.Drawing.Size(771, 387);
            this.settlebox.TabIndex = 0;
            this.settlebox.Text = "";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(13, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "交易帐号:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(76, 11);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(137, 18);
            this.account.TabIndex = 15;
            this.account.Text = "9580001";
            // 
            // btnQryHist
            // 
            this.btnQryHist.Location = new System.Drawing.Point(423, 7);
            this.btnQryHist.Name = "btnQryHist";
            this.btnQryHist.Size = new System.Drawing.Size(58, 24);
            this.btnQryHist.TabIndex = 38;
            this.btnQryHist.Text = "查 询";
            this.btnQryHist.Click += new System.EventHandler(this.btnQryHist_Click);
            // 
            // settleday
            // 
            this.settleday.Location = new System.Drawing.Point(285, 11);
            this.settleday.Name = "settleday";
            this.settleday.Size = new System.Drawing.Size(117, 18);
            this.settleday.TabIndex = 39;
            this.settleday.TabStop = false;
            this.settleday.Text = "Monday, September 01, 2014";
            this.settleday.Value = new System.DateTime(2014, 9, 1, 15, 53, 33, 406);
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(232, 13);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(47, 16);
            this.radLabel2.TabIndex = 40;
            this.radLabel2.Text = "交易日:";
            // 
            // ctGridExport1
            // 
            this.ctGridExport1.Grid = null;
            this.ctGridExport1.Location = new System.Drawing.Point(671, 7);
            this.ctGridExport1.Name = "ctGridExport1";
            this.ctGridExport1.Size = new System.Drawing.Size(110, 24);
            this.ctGridExport1.TabIndex = 41;
            // 
            // HistQryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.ctGridExport1);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.settleday);
            this.Controls.Add(this.btnQryHist);
            this.Controls.Add(this.account);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.qrypage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistQryForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "历史记录查询";
            this.ThemeName = "ControlDefault";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HistQryForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.qrypage)).EndInit();
            this.qrypage.ResumeLayout(false);
            this.historder.ResumeLayout(false);
            this.histtrade.ResumeLayout(false);
            this.histposition.ResumeLayout(false);
            this.histcash.ResumeLayout(false);
            this.histsettle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnQryHist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settleday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadPageView qrypage;
        private Telerik.WinControls.UI.RadPageViewPage historder;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox account;
        private Telerik.WinControls.UI.RadButton btnQryHist;
        private Telerik.WinControls.UI.RadDateTimePicker settleday;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadPageViewPage histtrade;
        private Telerik.WinControls.UI.RadPageViewPage histposition;
        private Telerik.WinControls.UI.RadPageViewPage histcash;
        private Telerik.WinControls.UI.RadPageViewPage histsettle;
        private ctHistOrder ctHistOrder1;
        private ctHistTrade ctHistTrade1;
        private ctHistPosition ctHistPosition1;
        private ctHistCashTransaction ctHistCashTransaction1;
        private System.Windows.Forms.RichTextBox settlebox;
        private ctGridExport ctGridExport1;
    }
}
