namespace TradingLib.Quant.GUI
{
    partial class ctStrategyProject
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.strategyGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.tabholder = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.strategyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabholder)).BeginInit();
            this.tabholder.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.splitContainer1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(856, 504);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(92, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "策略配置列表";
            // 
            // strategyGrid
            // 
            this.strategyGrid.AllowUserToAddRows = false;
            this.strategyGrid.AllowUserToDeleteRows = false;
            this.strategyGrid.AllowUserToResizeRows = false;
            this.strategyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.strategyGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.strategyGrid.ColumnHeadersHeight = 22;
            this.strategyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.strategyGrid.Location = new System.Drawing.Point(0, 24);
            this.strategyGrid.MultiSelect = false;
            this.strategyGrid.Name = "strategyGrid";
            this.strategyGrid.ReadOnly = true;
            this.strategyGrid.RowHeadersVisible = false;
            this.strategyGrid.RowTemplate.Height = 23;
            this.strategyGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.strategyGrid.Size = new System.Drawing.Size(856, 246);
            this.strategyGrid.StateCommon.Background.Color1 = System.Drawing.SystemColors.ControlDark;
            this.strategyGrid.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.strategyGrid.TabIndex = 1;
            this.strategyGrid.DoubleClick += new System.EventHandler(this.strategyGrid_DoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.kryptonPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabholder);
            this.splitContainer1.Size = new System.Drawing.Size(856, 504);
            this.splitContainer1.SplitterDistance = 270;
            this.splitContainer1.TabIndex = 3;
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel2.Controls.Add(this.strategyGrid);
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(856, 270);
            this.kryptonPanel2.TabIndex = 2;
            // 
            // tabholder
            // 
            this.tabholder.Bar.TabStyle = ComponentFactory.Krypton.Toolkit.TabStyle.StandardProfile;
            this.tabholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabholder.Location = new System.Drawing.Point(0, 0);
            this.tabholder.Name = "tabholder";
            this.tabholder.Size = new System.Drawing.Size(856, 230);
            this.tabholder.TabIndex = 0;
            this.tabholder.Text = "kryptonNavigator1";
            // 
            // ctStrategyProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctStrategyProject";
            this.Size = new System.Drawing.Size(856, 504);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.strategyGrid)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            this.kryptonPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabholder)).EndInit();
            this.tabholder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView strategyGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private ComponentFactory.Krypton.Navigator.KryptonNavigator tabholder;
    }
}
