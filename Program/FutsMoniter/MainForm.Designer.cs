namespace FutsMoniter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.radStatusStrip1 = new Telerik.WinControls.UI.RadStatusStrip();
            this.radLabelElement1 = new Telerik.WinControls.UI.RadLabelElement();
            this.statusmessage = new Telerik.WinControls.UI.RadLabelElement();
            this.radLabelElement2 = new Telerik.WinControls.UI.RadLabelElement();
            this.radMenuItem1 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem3 = new Telerik.WinControls.UI.RadMenuItem();
            this.btnOpenClearCentre = new Telerik.WinControls.UI.RadMenuItem();
            this.btnCloseOpenCentre = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem8 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.btnRouter = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem3 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.btnSystemStatus = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem2 = new Telerik.WinControls.UI.RadMenuItem();
            this.btnSecEdit = new Telerik.WinControls.UI.RadMenuItem();
            this.btnSymbolEdit = new Telerik.WinControls.UI.RadMenuItem();
            this.btnExchange = new Telerik.WinControls.UI.RadMenuItem();
            this.btnMarketTime = new Telerik.WinControls.UI.RadMenuItem();
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.radPageViewPage1 = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctAccountMontier1 = new FutsMoniter.Controls.ctAccountMontier();
            this.radPageViewPage2 = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctDebug1 = new FutSystems.GUI.ctDebug();
            this.windows8Theme1 = new Telerik.WinControls.Themes.Windows8Theme();
            this.office2010BlackTheme1 = new Telerik.WinControls.Themes.Office2010BlackTheme();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.radMenuItem4 = new Telerik.WinControls.UI.RadMenuItem();
            this.btnQryHist = new Telerik.WinControls.UI.RadMenuItem();
            this.btnStatistic = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem5 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem6 = new Telerik.WinControls.UI.RadMenuItem();
            this.office2010SilverTheme1 = new Telerik.WinControls.Themes.Office2010SilverTheme();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.radPageViewPage1.SuspendLayout();
            this.radPageViewPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radStatusStrip1
            // 
            this.radStatusStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radLabelElement1,
            this.statusmessage,
            this.radLabelElement2});
            this.radStatusStrip1.Location = new System.Drawing.Point(0, 631);
            this.radStatusStrip1.Name = "radStatusStrip1";
            this.radStatusStrip1.Size = new System.Drawing.Size(1139, 22);
            this.radStatusStrip1.TabIndex = 0;
            this.radStatusStrip1.Text = "radStatusStrip1";
            this.radStatusStrip1.ThemeName = "Windows8";
            // 
            // radLabelElement1
            // 
            this.radLabelElement1.AccessibleDescription = "消息:";
            this.radLabelElement1.AccessibleName = "消息:";
            this.radLabelElement1.Name = "radLabelElement1";
            this.radStatusStrip1.SetSpring(this.radLabelElement1, false);
            this.radLabelElement1.Text = "消息:";
            this.radLabelElement1.TextWrap = true;
            this.radLabelElement1.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // statusmessage
            // 
            this.statusmessage.Name = "statusmessage";
            this.radStatusStrip1.SetSpring(this.statusmessage, true);
            this.statusmessage.Text = "";
            this.statusmessage.TextWrap = true;
            this.statusmessage.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radLabelElement2
            // 
            this.radLabelElement2.AccessibleDescription = "连接:";
            this.radLabelElement2.AccessibleName = "连接:";
            this.radLabelElement2.Name = "radLabelElement2";
            this.radStatusStrip1.SetSpring(this.radLabelElement2, false);
            this.radLabelElement2.Text = "连接:";
            this.radLabelElement2.TextWrap = true;
            this.radLabelElement2.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radMenuItem1
            // 
            this.radMenuItem1.AccessibleDescription = "radMenuItem1";
            this.radMenuItem1.AccessibleName = "radMenuItem1";
            this.radMenuItem1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem3,
            this.radMenuItem8,
            this.radMenuSeparatorItem1,
            this.btnRouter,
            this.radMenuSeparatorItem3,
            this.btnSystemStatus});
            this.radMenuItem1.Name = "radMenuItem1";
            this.radMenuItem1.Text = "系统维护";
            this.radMenuItem1.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radMenuItem3
            // 
            this.radMenuItem3.AccessibleDescription = "清算中心";
            this.radMenuItem3.AccessibleName = "清算中心";
            this.radMenuItem3.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.btnOpenClearCentre,
            this.btnCloseOpenCentre});
            this.radMenuItem3.Name = "radMenuItem3";
            this.radMenuItem3.Text = "清算中心";
            this.radMenuItem3.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // btnOpenClearCentre
            // 
            this.btnOpenClearCentre.AccessibleDescription = "开启清算中心";
            this.btnOpenClearCentre.AccessibleName = "开启清算中心";
            this.btnOpenClearCentre.Name = "btnOpenClearCentre";
            this.btnOpenClearCentre.Text = "开启清算中心";
            this.btnOpenClearCentre.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnOpenClearCentre.Click += new System.EventHandler(this.btnOpenClearCentre_Click);
            // 
            // btnCloseOpenCentre
            // 
            this.btnCloseOpenCentre.AccessibleDescription = "关闭清算中心";
            this.btnCloseOpenCentre.AccessibleName = "关闭清算中心";
            this.btnCloseOpenCentre.Name = "btnCloseOpenCentre";
            this.btnCloseOpenCentre.Text = "关闭清算中心";
            this.btnCloseOpenCentre.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnCloseOpenCentre.Click += new System.EventHandler(this.btnCloseOpenCentre_Click);
            // 
            // radMenuItem8
            // 
            this.radMenuItem8.AccessibleDescription = "结算中心";
            this.radMenuItem8.AccessibleName = "结算中心";
            this.radMenuItem8.Name = "radMenuItem8";
            this.radMenuItem8.Text = "结算中心";
            this.radMenuItem8.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radMenuSeparatorItem1
            // 
            this.radMenuSeparatorItem1.AccessibleDescription = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.AccessibleName = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // btnRouter
            // 
            this.btnRouter.AccessibleDescription = "路由中心";
            this.btnRouter.AccessibleName = "路由中心";
            this.btnRouter.Name = "btnRouter";
            this.btnRouter.Text = "路由中心";
            this.btnRouter.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnRouter.Click += new System.EventHandler(this.btnRouter_Click);
            // 
            // radMenuSeparatorItem3
            // 
            this.radMenuSeparatorItem3.AccessibleDescription = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.AccessibleName = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.Name = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.Text = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // btnSystemStatus
            // 
            this.btnSystemStatus.AccessibleDescription = "系统状态";
            this.btnSystemStatus.AccessibleName = "系统状态";
            this.btnSystemStatus.Name = "btnSystemStatus";
            this.btnSystemStatus.Text = "系统状态";
            this.btnSystemStatus.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnSystemStatus.Click += new System.EventHandler(this.btnSystemStatus_Click);
            // 
            // radMenuItem2
            // 
            this.radMenuItem2.AccessibleDescription = "radMenuItem2";
            this.radMenuItem2.AccessibleName = "radMenuItem2";
            this.radMenuItem2.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.btnSecEdit,
            this.btnSymbolEdit,
            this.btnExchange,
            this.btnMarketTime});
            this.radMenuItem2.Name = "radMenuItem2";
            this.radMenuItem2.Text = "合约管理";
            this.radMenuItem2.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // btnSecEdit
            // 
            this.btnSecEdit.AccessibleDescription = "radMenuItem4";
            this.btnSecEdit.AccessibleName = "radMenuItem4";
            this.btnSecEdit.Name = "btnSecEdit";
            this.btnSecEdit.Text = "品种管理";
            this.btnSecEdit.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnSecEdit.Click += new System.EventHandler(this.btnSecEdit_Click);
            // 
            // btnSymbolEdit
            // 
            this.btnSymbolEdit.AccessibleDescription = "radMenuItem5";
            this.btnSymbolEdit.AccessibleName = "radMenuItem5";
            this.btnSymbolEdit.Name = "btnSymbolEdit";
            this.btnSymbolEdit.Text = "合约管理";
            this.btnSymbolEdit.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnSymbolEdit.Click += new System.EventHandler(this.btnSymbolEdit_Click);
            // 
            // btnExchange
            // 
            this.btnExchange.AccessibleDescription = "radMenuItem4";
            this.btnExchange.AccessibleName = "radMenuItem4";
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Text = "交易所管理";
            this.btnExchange.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // btnMarketTime
            // 
            this.btnMarketTime.AccessibleDescription = "radMenuItem5";
            this.btnMarketTime.AccessibleName = "radMenuItem5";
            this.btnMarketTime.Name = "btnMarketTime";
            this.btnMarketTime.Text = "交易时间管理";
            this.btnMarketTime.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnMarketTime.Click += new System.EventHandler(this.btnMarketTime_Click);
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.radPageViewPage1);
            this.radPageView1.Controls.Add(this.radPageViewPage2);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView1.Location = new System.Drawing.Point(0, 18);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.radPageViewPage1;
            this.radPageView1.Size = new System.Drawing.Size(1139, 613);
            this.radPageView1.TabIndex = 2;
            this.radPageView1.ThemeName = "Windows8";
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.radPageView1.GetChildAt(0))).StripButtons = Telerik.WinControls.UI.StripViewButtons.None;
            // 
            // radPageViewPage1
            // 
            this.radPageViewPage1.Controls.Add(this.ctAccountMontier1);
            this.radPageViewPage1.Location = new System.Drawing.Point(5, 27);
            this.radPageViewPage1.Name = "radPageViewPage1";
            this.radPageViewPage1.Size = new System.Drawing.Size(1129, 581);
            this.radPageViewPage1.Text = "帐户列表";
            // 
            // ctAccountMontier1
            // 
            this.ctAccountMontier1.DebugEnable = true;
            this.ctAccountMontier1.DebugLevel = TradingLib.API.QSEnumDebugLevel.INFO;
            this.ctAccountMontier1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctAccountMontier1.Location = new System.Drawing.Point(0, 0);
            this.ctAccountMontier1.Name = "ctAccountMontier1";
            this.ctAccountMontier1.Size = new System.Drawing.Size(1129, 581);
            this.ctAccountMontier1.TabIndex = 0;
            // 
            // radPageViewPage2
            // 
            this.radPageViewPage2.Controls.Add(this.ctDebug1);
            this.radPageViewPage2.Location = new System.Drawing.Point(5, 27);
            this.radPageViewPage2.Name = "radPageViewPage2";
            this.radPageViewPage2.Size = new System.Drawing.Size(1129, 579);
            this.radPageViewPage2.Text = "系统日志";
            // 
            // ctDebug1
            // 
            this.ctDebug1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDebug1.EnableSearching = true;
            this.ctDebug1.ExternalTimeStamp = 0;
            this.ctDebug1.Location = new System.Drawing.Point(0, 0);
            this.ctDebug1.Margin = new System.Windows.Forms.Padding(2);
            this.ctDebug1.Name = "ctDebug1";
            this.ctDebug1.Size = new System.Drawing.Size(1129, 579);
            this.ctDebug1.TabIndex = 0;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // radMenu1
            // 
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem1,
            this.radMenuItem2,
            this.radMenuItem4,
            this.radMenuItem5});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.Size = new System.Drawing.Size(1139, 18);
            this.radMenu1.TabIndex = 1;
            this.radMenu1.Text = "radMenu1";
            this.radMenu1.ThemeName = "Windows8";
            // 
            // radMenuItem4
            // 
            this.radMenuItem4.AccessibleDescription = "radMenuItem4";
            this.radMenuItem4.AccessibleName = "radMenuItem4";
            this.radMenuItem4.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.btnQryHist,
            this.btnStatistic});
            this.radMenuItem4.Name = "radMenuItem4";
            this.radMenuItem4.Text = "记录查询";
            this.radMenuItem4.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // btnQryHist
            // 
            this.btnQryHist.AccessibleDescription = "历史记录查询";
            this.btnQryHist.AccessibleName = "历史记录查询";
            this.btnQryHist.Name = "btnQryHist";
            this.btnQryHist.Text = "历史记录查询";
            this.btnQryHist.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnQryHist.Click += new System.EventHandler(this.btnQryHist_Click);
            // 
            // btnStatistic
            // 
            this.btnStatistic.AccessibleDescription = "统计报表";
            this.btnStatistic.AccessibleName = "统计报表";
            this.btnStatistic.Name = "btnStatistic";
            this.btnStatistic.Text = "统计报表";
            this.btnStatistic.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnStatistic.Click += new System.EventHandler(this.btnStatistic_Click);
            // 
            // radMenuItem5
            // 
            this.radMenuItem5.AccessibleDescription = "radMenuItem5";
            this.radMenuItem5.AccessibleName = "radMenuItem5";
            this.radMenuItem5.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem6});
            this.radMenuItem5.Name = "radMenuItem5";
            this.radMenuItem5.Text = "radMenuItem5";
            this.radMenuItem5.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radMenuItem6
            // 
            this.radMenuItem6.AccessibleDescription = "radMenuItem6";
            this.radMenuItem6.AccessibleName = "radMenuItem6";
            this.radMenuItem6.Name = "radMenuItem6";
            this.radMenuItem6.Text = "radMenuItem6";
            this.radMenuItem6.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.radMenuItem6.Click += new System.EventHandler(this.radMenuItem6_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1139, 653);
            this.Controls.Add(this.radPageView1);
            this.Controls.Add(this.radMenu1);
            this.Controls.Add(this.radStatusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "MainForm";
            this.ThemeName = "Office2010Black";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.radPageViewPage1.ResumeLayout(false);
            this.radPageViewPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadStatusStrip radStatusStrip1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem2;
        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage1;
        private Controls.ctAccountMontier ctAccountMontier1;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage2;
        private Telerik.WinControls.Themes.Windows8Theme windows8Theme1;
        private FutSystems.GUI.ctDebug ctDebug1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem3;
        private Telerik.WinControls.UI.RadMenuItem btnOpenClearCentre;
        private Telerik.WinControls.UI.RadMenuItem btnCloseOpenCentre;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem8;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem btnRouter;
        private Telerik.WinControls.UI.RadMenuItem btnSecEdit;
        private Telerik.WinControls.UI.RadMenuItem btnSymbolEdit;
        private Telerik.WinControls.UI.RadMenuItem btnExchange;
        private Telerik.WinControls.UI.RadMenuItem btnMarketTime;
        private Telerik.WinControls.Themes.Office2010BlackTheme office2010BlackTheme1;
        //private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem2;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem3;
        private Telerik.WinControls.UI.RadMenuItem btnSystemStatus;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.Themes.Office2010SilverTheme office2010SilverTheme1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem4;
        private Telerik.WinControls.UI.RadMenuItem btnQryHist;
        private Telerik.WinControls.UI.RadMenuItem btnStatistic;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem5;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem6;
        private Telerik.WinControls.UI.RadLabelElement radLabelElement1;
        private Telerik.WinControls.UI.RadLabelElement statusmessage;
        private Telerik.WinControls.UI.RadLabelElement radLabelElement2;
    }
}
