namespace FutsMoniter
{
    partial class fmHistQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmHistQuery));
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnQryHist = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.settleday = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.pageOrder = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.pageTrade = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.pagePosition = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.btnExport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ctHistOrder1 = new FutsMoniter.ctHistOrder();
            this.ctHistTrade1 = new FutsMoniter.ctHistTrade();
            this.ctHistPosition1 = new FutsMoniter.ctHistPosition();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
            this.kryptonNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pageOrder)).BeginInit();
            this.pageOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pageTrade)).BeginInit();
            this.pageTrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pagePosition)).BeginInit();
            this.pagePosition.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnExport);
            this.kryptonPanel1.Controls.Add(this.btnQryHist);
            this.kryptonPanel1.Controls.Add(this.settleday);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.kryptonNavigator1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(856, 471);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnQryHist
            // 
            this.btnQryHist.Location = new System.Drawing.Point(718, 8);
            this.btnQryHist.Name = "btnQryHist";
            this.btnQryHist.Size = new System.Drawing.Size(60, 25);
            this.btnQryHist.TabIndex = 5;
            this.btnQryHist.Values.Text = "查 询";
            // 
            // settleday
            // 
            this.settleday.Location = new System.Drawing.Point(289, 10);
            this.settleday.Name = "settleday";
            this.settleday.Size = new System.Drawing.Size(147, 20);
            this.settleday.TabIndex = 4;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(228, 12);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel2.TabIndex = 3;
            this.kryptonLabel2.Values.Text = "结算日:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(86, 8);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(126, 21);
            this.account.TabIndex = 2;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "交易帐户:";
            // 
            // kryptonNavigator1
            // 
            this.kryptonNavigator1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonNavigator1.Bar.TabStyle = ComponentFactory.Krypton.Toolkit.TabStyle.LowProfile;
            // 
            // 
            // 
            this.kryptonNavigator1.Button.CloseButton.AllowInheritExtraText = false;
            this.kryptonNavigator1.Button.CloseButton.UniqueName = "E853F08D37CD4B9F0BB445C9A7AD6247";
            this.kryptonNavigator1.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.kryptonNavigator1.Button.ContextButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.kryptonNavigator1.Location = new System.Drawing.Point(0, 39);
            this.kryptonNavigator1.Name = "kryptonNavigator1";
            this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.pageOrder,
            this.pageTrade,
            this.pagePosition});
            this.kryptonNavigator1.SelectedIndex = 0;
            this.kryptonNavigator1.Size = new System.Drawing.Size(856, 432);
            this.kryptonNavigator1.TabIndex = 0;
            this.kryptonNavigator1.Text = "kryptonNavigator1";
            // 
            // pageOrder
            // 
            this.pageOrder.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.pageOrder.Controls.Add(this.ctHistOrder1);
            this.pageOrder.Flags = 65534;
            this.pageOrder.LastVisibleSet = true;
            this.pageOrder.MinimumSize = new System.Drawing.Size(50, 50);
            this.pageOrder.Name = "pageOrder";
            this.pageOrder.Size = new System.Drawing.Size(854, 407);
            this.pageOrder.Text = "委 托";
            this.pageOrder.ToolTipTitle = "Page ToolTip";
            this.pageOrder.UniqueName = "590193F15ED84477088CD0F38750A7C3";
            // 
            // pageTrade
            // 
            this.pageTrade.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.pageTrade.Controls.Add(this.ctHistTrade1);
            this.pageTrade.Flags = 65534;
            this.pageTrade.LastVisibleSet = true;
            this.pageTrade.MinimumSize = new System.Drawing.Size(50, 50);
            this.pageTrade.Name = "pageTrade";
            this.pageTrade.Size = new System.Drawing.Size(854, 407);
            this.pageTrade.Text = "成 交";
            this.pageTrade.ToolTipTitle = "Page ToolTip";
            this.pageTrade.UniqueName = "484B4FDD7D58432A7BA5AEDB09324076";
            // 
            // pagePosition
            // 
            this.pagePosition.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.pagePosition.Controls.Add(this.ctHistPosition1);
            this.pagePosition.Flags = 65534;
            this.pagePosition.LastVisibleSet = true;
            this.pagePosition.MinimumSize = new System.Drawing.Size(50, 50);
            this.pagePosition.Name = "pagePosition";
            this.pagePosition.Size = new System.Drawing.Size(854, 407);
            this.pagePosition.Text = "持 仓";
            this.pagePosition.ToolTipTitle = "Page ToolTip";
            this.pagePosition.UniqueName = "93EA66440495432B228CD438AC6F3C4F";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(784, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(60, 25);
            this.btnExport.TabIndex = 6;
            this.btnExport.Values.Text = "导 出";
            // 
            // ctHistOrder1
            // 
            this.ctHistOrder1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistOrder1.Location = new System.Drawing.Point(0, 0);
            this.ctHistOrder1.Name = "ctHistOrder1";
            this.ctHistOrder1.Size = new System.Drawing.Size(854, 407);
            this.ctHistOrder1.TabIndex = 0;
            // 
            // ctHistTrade1
            // 
            this.ctHistTrade1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistTrade1.Location = new System.Drawing.Point(0, 0);
            this.ctHistTrade1.Name = "ctHistTrade1";
            this.ctHistTrade1.Size = new System.Drawing.Size(854, 407);
            this.ctHistTrade1.TabIndex = 0;
            // 
            // ctHistPosition1
            // 
            this.ctHistPosition1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctHistPosition1.Location = new System.Drawing.Point(0, 0);
            this.ctHistPosition1.Name = "ctHistPosition1";
            this.ctHistPosition1.Size = new System.Drawing.Size(854, 407);
            this.ctHistPosition1.TabIndex = 0;
            // 
            // fmHistQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 471);
            this.Controls.Add(this.kryptonPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fmHistQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "交易记录查询";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
            this.kryptonNavigator1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pageOrder)).EndInit();
            this.pageOrder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pageTrade)).EndInit();
            this.pageTrade.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pagePosition)).EndInit();
            this.pagePosition.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker settleday;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox account;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
        private ComponentFactory.Krypton.Navigator.KryptonPage pageOrder;
        private ComponentFactory.Krypton.Navigator.KryptonPage pageTrade;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQryHist;
        private ComponentFactory.Krypton.Navigator.KryptonPage pagePosition;
        private ctHistOrder ctHistOrder1;
        private ctHistTrade ctHistTrade1;
        private ctHistPosition ctHistPosition1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnExport;
    }
}