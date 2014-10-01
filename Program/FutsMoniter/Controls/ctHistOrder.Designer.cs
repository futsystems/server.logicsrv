namespace FutsMoniter
{
    partial class ctHistOrder
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
            this.orderGrid = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // orderGrid
            // 
            this.orderGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.orderGrid.Location = new System.Drawing.Point(0, 0);
            this.orderGrid.Name = "orderGrid";
            this.orderGrid.Size = new System.Drawing.Size(791, 330);
            this.orderGrid.TabIndex = 0;
            this.orderGrid.Text = "radGridView1";
            this.orderGrid.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.orderGrid_CellFormatting);
            // 
            // ctHistOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.orderGrid);
            this.Name = "ctHistOrder";
            this.Size = new System.Drawing.Size(791, 330);
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView orderGrid;
    }
}
