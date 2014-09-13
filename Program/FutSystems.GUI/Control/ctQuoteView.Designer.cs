namespace FutSystems.GUI
{
    partial class ctQuoteView
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
            this.quoteGrid = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.quoteGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // quoteGrid
            // 
            this.quoteGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quoteGrid.Location = new System.Drawing.Point(0, 0);
            this.quoteGrid.Name = "quoteGrid";
            this.quoteGrid.Size = new System.Drawing.Size(727, 249);
            this.quoteGrid.TabIndex = 0;
            this.quoteGrid.Text = "radGridView1";
            // 
            // ctQuoteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.quoteGrid);
            this.Name = "ctQuoteView";
            this.Size = new System.Drawing.Size(727, 249);
            this.Load += new System.EventHandler(this.ctQuoteView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.quoteGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView quoteGrid;
    }
}
