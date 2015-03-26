namespace FutsMoniter
{
    partial class fmSettleManager
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonGroupBox2 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.lbSettleMode = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbNextSettleday = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbCurrentday = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.lbLastSettleday = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.btnReSettle = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnDelSettleInfo = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.dpSettleday = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.btnLoadInfo = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.ctPositionHold1 = new FutsMoniter.ctPositionHold();
            this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.ctSettlementPrice1 = new FutsMoniter.ctSettlementPrice();
            this.btnResetSystem = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).BeginInit();
            this.kryptonGroupBox2.Panel.SuspendLayout();
            this.kryptonGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
            this.kryptonNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
            this.kryptonPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
            this.kryptonPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox2);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel1.Controls.Add(this.kryptonNavigator1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(654, 410);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonGroupBox2
            // 
            this.kryptonGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.kryptonGroupBox2.Name = "kryptonGroupBox2";
            // 
            // kryptonGroupBox2.Panel
            // 
            this.kryptonGroupBox2.Panel.Controls.Add(this.lbSettleMode);
            this.kryptonGroupBox2.Panel.Controls.Add(this.lbNextSettleday);
            this.kryptonGroupBox2.Panel.Controls.Add(this.lbCurrentday);
            this.kryptonGroupBox2.Panel.Controls.Add(this.lbLastSettleday);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel5);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel4);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel3);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel2);
            this.kryptonGroupBox2.Size = new System.Drawing.Size(241, 129);
            this.kryptonGroupBox2.TabIndex = 7;
            this.kryptonGroupBox2.Values.Heading = "结算信息";
            // 
            // lbSettleMode
            // 
            this.lbSettleMode.Location = new System.Drawing.Point(95, 79);
            this.lbSettleMode.Name = "lbSettleMode";
            this.lbSettleMode.Size = new System.Drawing.Size(19, 18);
            this.lbSettleMode.TabIndex = 8;
            this.lbSettleMode.Values.Text = "--";
            // 
            // lbNextSettleday
            // 
            this.lbNextSettleday.Location = new System.Drawing.Point(95, 55);
            this.lbNextSettleday.Name = "lbNextSettleday";
            this.lbNextSettleday.Size = new System.Drawing.Size(19, 18);
            this.lbNextSettleday.TabIndex = 7;
            this.lbNextSettleday.Values.Text = "--";
            // 
            // lbCurrentday
            // 
            this.lbCurrentday.Location = new System.Drawing.Point(95, 31);
            this.lbCurrentday.Name = "lbCurrentday";
            this.lbCurrentday.Size = new System.Drawing.Size(19, 18);
            this.lbCurrentday.TabIndex = 6;
            this.lbCurrentday.Values.Text = "--";
            // 
            // lbLastSettleday
            // 
            this.lbLastSettleday.Location = new System.Drawing.Point(95, 7);
            this.lbLastSettleday.Name = "lbLastSettleday";
            this.lbLastSettleday.Size = new System.Drawing.Size(19, 18);
            this.lbLastSettleday.TabIndex = 5;
            this.lbLastSettleday.Values.Text = "--";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(21, 79);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 4;
            this.kryptonLabel5.Values.Text = "结算模式:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(21, 31);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 3;
            this.kryptonLabel4.Values.Text = "当前日期:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(7, 55);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "下一结算日:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(7, 7);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "上一结算日:";
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(250, 3);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.btnResetSystem);
            this.kryptonGroupBox1.Panel.Controls.Add(this.btnReSettle);
            this.kryptonGroupBox1.Panel.Controls.Add(this.btnDelSettleInfo);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroupBox1.Panel.Controls.Add(this.dpSettleday);
            this.kryptonGroupBox1.Panel.Controls.Add(this.btnLoadInfo);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(392, 129);
            this.kryptonGroupBox1.TabIndex = 6;
            this.kryptonGroupBox1.Values.Heading = "手工结算";
            // 
            // btnReSettle
            // 
            this.btnReSettle.Location = new System.Drawing.Point(130, 69);
            this.btnReSettle.Name = "btnReSettle";
            this.btnReSettle.Size = new System.Drawing.Size(116, 25);
            this.btnReSettle.TabIndex = 4;
            this.btnReSettle.Values.Text = "重新结算";
            // 
            // btnDelSettleInfo
            // 
            this.btnDelSettleInfo.Location = new System.Drawing.Point(130, 38);
            this.btnDelSettleInfo.Name = "btnDelSettleInfo";
            this.btnDelSettleInfo.Size = new System.Drawing.Size(116, 25);
            this.btnDelSettleInfo.TabIndex = 3;
            this.btnDelSettleInfo.Values.Text = "删除结算数据";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(2, 7);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "结算日:";
            // 
            // dpSettleday
            // 
            this.dpSettleday.Location = new System.Drawing.Point(2, 38);
            this.dpSettleday.Name = "dpSettleday";
            this.dpSettleday.Size = new System.Drawing.Size(122, 20);
            this.dpSettleday.TabIndex = 1;
            // 
            // btnLoadInfo
            // 
            this.btnLoadInfo.Location = new System.Drawing.Point(130, 7);
            this.btnLoadInfo.Name = "btnLoadInfo";
            this.btnLoadInfo.Size = new System.Drawing.Size(116, 25);
            this.btnLoadInfo.TabIndex = 2;
            this.btnLoadInfo.Values.Text = "回 滚";
            // 
            // kryptonNavigator1
            // 
            this.kryptonNavigator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonNavigator1.Button.ButtonDisplayLogic = ComponentFactory.Krypton.Navigator.ButtonDisplayLogic.None;
            this.kryptonNavigator1.Button.CloseButtonAction = ComponentFactory.Krypton.Navigator.CloseButtonAction.None;
            this.kryptonNavigator1.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.kryptonNavigator1.Location = new System.Drawing.Point(0, 138);
            this.kryptonNavigator1.Name = "kryptonNavigator1";
            this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2});
            this.kryptonNavigator1.SelectedIndex = 0;
            this.kryptonNavigator1.Size = new System.Drawing.Size(654, 272);
            this.kryptonNavigator1.TabIndex = 5;
            this.kryptonNavigator1.Text = "kryptonNavigator1";
            // 
            // kryptonPage1
            // 
            this.kryptonPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage1.Controls.Add(this.ctPositionHold1);
            this.kryptonPage1.Flags = 65534;
            this.kryptonPage1.LastVisibleSet = true;
            this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage1.Name = "kryptonPage1";
            this.kryptonPage1.Size = new System.Drawing.Size(652, 247);
            this.kryptonPage1.Text = "隔夜持仓";
            this.kryptonPage1.ToolTipTitle = "Page ToolTip";
            this.kryptonPage1.UniqueName = "A547167AB10D46A8B9ACEF5AEC5FCBBF";
            // 
            // ctPositionHold1
            // 
            this.ctPositionHold1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctPositionHold1.Location = new System.Drawing.Point(0, 0);
            this.ctPositionHold1.Name = "ctPositionHold1";
            this.ctPositionHold1.Size = new System.Drawing.Size(652, 247);
            this.ctPositionHold1.TabIndex = 0;
            // 
            // kryptonPage2
            // 
            this.kryptonPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage2.Controls.Add(this.ctSettlementPrice1);
            this.kryptonPage2.Flags = 65534;
            this.kryptonPage2.LastVisibleSet = true;
            this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage2.Name = "kryptonPage2";
            this.kryptonPage2.Size = new System.Drawing.Size(652, 247);
            this.kryptonPage2.Text = "结算价";
            this.kryptonPage2.ToolTipTitle = "Page ToolTip";
            this.kryptonPage2.UniqueName = "F11BFEDFEC8A4A15648EFC526323572D";
            // 
            // ctSettlementPrice1
            // 
            this.ctSettlementPrice1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctSettlementPrice1.Location = new System.Drawing.Point(0, 0);
            this.ctSettlementPrice1.Name = "ctSettlementPrice1";
            this.ctSettlementPrice1.Size = new System.Drawing.Size(652, 247);
            this.ctSettlementPrice1.TabIndex = 1;
            // 
            // btnResetSystem
            // 
            this.btnResetSystem.Location = new System.Drawing.Point(252, 69);
            this.btnResetSystem.Name = "btnResetSystem";
            this.btnResetSystem.Size = new System.Drawing.Size(116, 25);
            this.btnResetSystem.TabIndex = 5;
            this.btnResetSystem.Values.Text = "重置系统";
            // 
            // fmSettleManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 410);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmSettleManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "结算管理";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonGroupBox2.Panel.ResumeLayout(false);
            this.kryptonGroupBox2.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).EndInit();
            this.kryptonGroupBox2.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
            this.kryptonNavigator1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
            this.kryptonPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
            this.kryptonPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker dpSettleday;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnReSettle;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDelSettleInfo;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnLoadInfo;
        private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
        //private ctSettlementPrice ctSettlementPrice1;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbSettleMode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbNextSettleday;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbCurrentday;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbLastSettleday;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ctPositionHold ctPositionHold1;
        private ctSettlementPrice ctSettlementPrice1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnResetSystem;
        //private ctSettlementPrice ctSettlementPrice1;
    }
}