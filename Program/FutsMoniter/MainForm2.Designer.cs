namespace FutsMoniter
{
    partial class MainForm2
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
            this.tabSystem = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.kryptonRibbonGroupLines1 = new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupLines();
            this.kryptonRibbonGroup2 = new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup();
            this.tabBasicInfo = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.tabHistQuery = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.tabManger = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.kryptonRibbon1 = new ComponentFactory.Krypton.Ribbon.KryptonRibbon();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonRibbon1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabSystem
            // 
            this.tabSystem.Text = "系统管理";
            // 
            // kryptonRibbonGroupLines1
            // 
            this.kryptonRibbonGroupLines1.MaximumSize = ComponentFactory.Krypton.Ribbon.GroupItemSize.Small;
            // 
            // tabBasicInfo
            // 
            this.tabBasicInfo.Groups.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup[] {
            this.kryptonRibbonGroup2});
            this.tabBasicInfo.Text = "基础数据";
            // 
            // tabHistQuery
            // 
            this.tabHistQuery.Text = "历史查询";
            // 
            // tabManger
            // 
            this.tabManger.Text = "柜员管理";
            // 
            // kryptonRibbon1
            // 
            this.kryptonRibbon1.InDesignHelperMode = true;
            this.kryptonRibbon1.Name = "kryptonRibbon1";
            this.kryptonRibbon1.RibbonTabs.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab[] {
            this.tabSystem,
            this.tabBasicInfo,
            this.tabHistQuery,
            this.tabManger});
            this.kryptonRibbon1.SelectedTab = this.tabHistQuery;
            this.kryptonRibbon1.Size = new System.Drawing.Size(745, 111);
            this.kryptonRibbon1.TabIndex = 0;
            // 
            // MainForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 456);
            this.Controls.Add(this.kryptonRibbon1);
            this.CustomCaptionArea = new System.Drawing.Rectangle(411, 0, 309, 24);
            this.Name = "MainForm2";
            this.Text = "MainForm2";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonRibbon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab tabSystem;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupLines kryptonRibbonGroupLines1;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup kryptonRibbonGroup2;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab tabBasicInfo;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab tabHistQuery;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab tabManger;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbon kryptonRibbon1;

    }
}