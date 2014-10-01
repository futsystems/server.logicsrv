namespace TradingLib.Quant.GUI
{
    partial class BackTestReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackTestReport));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tabholder = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.openBackTestFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBackTestFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabholder)).BeginInit();
            this.tabholder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(781, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tabholder
            // 
            this.tabholder.Bar.TabStyle = ComponentFactory.Krypton.Toolkit.TabStyle.LowProfile;
            this.tabholder.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.tabholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabholder.Location = new System.Drawing.Point(0, 25);
            this.tabholder.Name = "tabholder";
            this.tabholder.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2});
            this.tabholder.SelectedIndex = 0;
            this.tabholder.Size = new System.Drawing.Size(781, 388);
            this.tabholder.TabIndex = 1;
            this.tabholder.Text = "kryptonNavigator1";
            // 
            // kryptonPage1
            // 
            this.kryptonPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage1.Flags = 65534;
            this.kryptonPage1.LastVisibleSet = true;
            this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage1.Name = "kryptonPage1";
            this.kryptonPage1.Size = new System.Drawing.Size(779, 363);
            this.kryptonPage1.Text = "kryptonPage1";
            this.kryptonPage1.ToolTipTitle = "Page ToolTip";
            this.kryptonPage1.UniqueName = "F6CDDBFCA0E34B9828B9EFB3F546C0A1";
            // 
            // kryptonPage2
            // 
            this.kryptonPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage2.Flags = 65534;
            this.kryptonPage2.LastVisibleSet = true;
            this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage2.Name = "kryptonPage2";
            this.kryptonPage2.Size = new System.Drawing.Size(779, 363);
            this.kryptonPage2.Text = "kryptonPage2";
            this.kryptonPage2.ToolTipTitle = "Page ToolTip";
            this.kryptonPage2.UniqueName = "6AF1AADA6CF04C8AAC8F8CB9B3608307";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBackTestFileToolStripMenuItem,
            this.saveBackTestFileToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(47, 22);
            this.toolStripSplitButton1.Text = "工具";
            // 
            // openBackTestFileToolStripMenuItem
            // 
            this.openBackTestFileToolStripMenuItem.Name = "openBackTestFileToolStripMenuItem";
            this.openBackTestFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openBackTestFileToolStripMenuItem.Text = "打开回测文件";
            // 
            // saveBackTestFileToolStripMenuItem
            // 
            this.saveBackTestFileToolStripMenuItem.Name = "saveBackTestFileToolStripMenuItem";
            this.saveBackTestFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveBackTestFileToolStripMenuItem.Text = "保存回测文件";
            // 
            // BackTestReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 413);
            this.Controls.Add(this.tabholder);
            this.Controls.Add(this.toolStrip1);
            this.Name = "BackTestReport";
            this.Text = "BackTestReport";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabholder)).EndInit();
            this.tabholder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private ComponentFactory.Krypton.Navigator.KryptonNavigator tabholder;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem openBackTestFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBackTestFileToolStripMenuItem;
    }
}