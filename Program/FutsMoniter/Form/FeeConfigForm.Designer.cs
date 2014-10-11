namespace FutsMoniter
{
    partial class FeeConfigForm
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
            this.FinServiceFeePage = new Telerik.WinControls.UI.RadPageViewPage();
            this.ctAgentSPArgConfig1 = new FutsMoniter.ctAgentSPArgConfig();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.FinServiceFeePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.FinServiceFeePage);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView1.Location = new System.Drawing.Point(0, 0);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.FinServiceFeePage;
            this.radPageView1.Size = new System.Drawing.Size(689, 368);
            this.radPageView1.TabIndex = 0;
            this.radPageView1.Text = "radPageView1";
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.radPageView1.GetChildAt(0))).StripButtons = Telerik.WinControls.UI.StripViewButtons.None;
            // 
            // FinServiceFeePage
            // 
            this.FinServiceFeePage.Controls.Add(this.ctAgentSPArgConfig1);
            this.FinServiceFeePage.Location = new System.Drawing.Point(10, 35);
            this.FinServiceFeePage.Name = "FinServiceFeePage";
            this.FinServiceFeePage.Size = new System.Drawing.Size(668, 322);
            this.FinServiceFeePage.Text = "配资服务";
            // 
            // ctAgentSPArgConfig1
            // 
            this.ctAgentSPArgConfig1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctAgentSPArgConfig1.Location = new System.Drawing.Point(0, 0);
            this.ctAgentSPArgConfig1.Name = "ctAgentSPArgConfig1";
            this.ctAgentSPArgConfig1.Size = new System.Drawing.Size(668, 322);
            this.ctAgentSPArgConfig1.TabIndex = 0;
            // 
            // FeeConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 368);
            this.Controls.Add(this.radPageView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeeConfigForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "柜员费率设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FeeConfigForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.FinServiceFeePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage FinServiceFeePage;
        private ctAgentSPArgConfig ctAgentSPArgConfig1;
    }
}