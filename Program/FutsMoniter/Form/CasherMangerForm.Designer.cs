namespace FutsMoniter
{
    partial class CasherMangerForm
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
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.accountCashOperationPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.agentCashOperationPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.accountCashOpQryPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.agentCashOpQryPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel8 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel9 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel10 = new Telerik.WinControls.UI.RadLabel();
            this.ctCashOperation1 = new FutsMoniter.ctCashOperation();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.agentCashOperationPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.radLabel8);
            this.radGroupBox1.Controls.Add(this.radLabel7);
            this.radGroupBox1.Controls.Add(this.radLabel9);
            this.radGroupBox1.Controls.Add(this.radLabel6);
            this.radGroupBox1.Controls.Add(this.radLabel10);
            this.radGroupBox1.Controls.Add(this.radLabel5);
            this.radGroupBox1.Controls.Add(this.radLabel4);
            this.radGroupBox1.Controls.Add(this.radLabel3);
            this.radGroupBox1.Controls.Add(this.radLabel2);
            this.radGroupBox1.Controls.Add(this.radLabel1);
            this.radGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radGroupBox1.HeaderText = "现金事务统计";
            this.radGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.radGroupBox1.Name = "radGroupBox1";
            // 
            // 
            // 
            this.radGroupBox1.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox1.Size = new System.Drawing.Size(692, 84);
            this.radGroupBox1.TabIndex = 0;
            this.radGroupBox1.Text = "现金事务统计";
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.accountCashOperationPage);
            this.radPageView1.Controls.Add(this.agentCashOperationPage);
            this.radPageView1.Controls.Add(this.accountCashOpQryPage);
            this.radPageView1.Controls.Add(this.agentCashOpQryPage);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radPageView1.Location = new System.Drawing.Point(0, 90);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.accountCashOperationPage;
            this.radPageView1.Size = new System.Drawing.Size(692, 410);
            this.radPageView1.TabIndex = 1;
            this.radPageView1.Text = "radPageView1";
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.radPageView1.GetChildAt(0))).StripButtons = Telerik.WinControls.UI.StripViewButtons.None;
            // 
            // accountCashOperationPage
            // 
            this.accountCashOperationPage.Location = new System.Drawing.Point(10, 31);
            this.accountCashOperationPage.Name = "accountCashOperationPage";
            this.accountCashOperationPage.Size = new System.Drawing.Size(671, 368);
            this.accountCashOperationPage.Text = "交易帐户";
            // 
            // agentCashOperationPage
            // 
            this.agentCashOperationPage.Controls.Add(this.ctCashOperation1);
            this.agentCashOperationPage.Location = new System.Drawing.Point(10, 35);
            this.agentCashOperationPage.Name = "agentCashOperationPage";
            this.agentCashOperationPage.Size = new System.Drawing.Size(764, 295);
            this.agentCashOperationPage.Text = "代理帐户";
            // 
            // accountCashOpQryPage
            // 
            this.accountCashOpQryPage.Location = new System.Drawing.Point(10, 35);
            this.accountCashOpQryPage.Name = "accountCashOpQryPage";
            this.accountCashOpQryPage.Size = new System.Drawing.Size(764, 295);
            this.accountCashOpQryPage.Text = "交易帐户历史查询";
            // 
            // agentCashOpQryPage
            // 
            this.agentCashOpQryPage.Location = new System.Drawing.Point(10, 35);
            this.agentCashOpQryPage.Name = "agentCashOpQryPage";
            this.agentCashOpQryPage.Size = new System.Drawing.Size(764, 295);
            this.agentCashOpQryPage.Text = "代理帐户历史查询";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(22, 26);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(72, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "待确认入金:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(476, 26);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(72, 16);
            this.radLabel2.TabIndex = 1;
            this.radLabel2.Text = "待支付出金:";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.radLabel3.ForeColor = System.Drawing.Color.Crimson;
            this.radLabel3.Location = new System.Drawing.Point(93, 18);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(89, 29);
            this.radLabel3.TabIndex = 2;
            this.radLabel3.Text = "25089.2";
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.radLabel4.ForeColor = System.Drawing.Color.LimeGreen;
            this.radLabel4.Location = new System.Drawing.Point(546, 18);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(77, 29);
            this.radLabel4.TabIndex = 3;
            this.radLabel4.Text = "3560.2";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(72, 53);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(22, 16);
            this.radLabel5.TabIndex = 4;
            this.radLabel5.Text = "共:";
            // 
            // radLabel6
            // 
            this.radLabel6.Location = new System.Drawing.Point(100, 53);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(14, 16);
            this.radLabel6.TabIndex = 5;
            this.radLabel6.Text = "--";
            // 
            // radLabel7
            // 
            this.radLabel7.Location = new System.Drawing.Point(120, 53);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(19, 16);
            this.radLabel7.TabIndex = 6;
            this.radLabel7.Text = "笔";
            // 
            // radLabel8
            // 
            this.radLabel8.Location = new System.Drawing.Point(574, 53);
            this.radLabel8.Name = "radLabel8";
            this.radLabel8.Size = new System.Drawing.Size(19, 16);
            this.radLabel8.TabIndex = 9;
            this.radLabel8.Text = "笔";
            // 
            // radLabel9
            // 
            this.radLabel9.Location = new System.Drawing.Point(554, 53);
            this.radLabel9.Name = "radLabel9";
            this.radLabel9.Size = new System.Drawing.Size(14, 16);
            this.radLabel9.TabIndex = 8;
            this.radLabel9.Text = "--";
            // 
            // radLabel10
            // 
            this.radLabel10.Location = new System.Drawing.Point(526, 53);
            this.radLabel10.Name = "radLabel10";
            this.radLabel10.Size = new System.Drawing.Size(22, 16);
            this.radLabel10.TabIndex = 7;
            this.radLabel10.Text = "共:";
            // 
            // ctCashOperation1
            // 
            this.ctCashOperation1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctCashOperation1.Location = new System.Drawing.Point(0, 0);
            this.ctCashOperation1.Name = "ctCashOperation1";
            this.ctCashOperation1.Size = new System.Drawing.Size(764, 295);
            this.ctCashOperation1.TabIndex = 0;
            // 
            // CasherMangerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 500);
            this.Controls.Add(this.radPageView1);
            this.Controls.Add(this.radGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CasherMangerForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "出纳中心";
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.agentCashOperationPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage accountCashOperationPage;
        private Telerik.WinControls.UI.RadPageViewPage agentCashOperationPage;
        private ctCashOperation ctCashOperation1;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadPageViewPage accountCashOpQryPage;
        private Telerik.WinControls.UI.RadPageViewPage agentCashOpQryPage;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel radLabel8;
        private Telerik.WinControls.UI.RadLabel radLabel9;
        private Telerik.WinControls.UI.RadLabel radLabel10;
    }
}