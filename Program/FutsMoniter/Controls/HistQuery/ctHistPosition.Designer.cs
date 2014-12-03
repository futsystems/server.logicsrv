namespace FutsMoniter
{
    partial class ctHistPosition
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
            this.positiongrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.positiongrid)).BeginInit();
            this.SuspendLayout();
            // 
            // positiongrid
            // 
            this.positiongrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.positiongrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positiongrid.Location = new System.Drawing.Point(0, 0);
            this.positiongrid.Name = "positiongrid";
            this.positiongrid.RowTemplate.Height = 23;
            this.positiongrid.Size = new System.Drawing.Size(1024, 577);
            this.positiongrid.TabIndex = 0;
            // 
            // ctHistPosition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.positiongrid);
            this.Name = "ctHistPosition";
            this.Size = new System.Drawing.Size(1024, 577);
            ((System.ComponentModel.ISupportInitialize)(this.positiongrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView positiongrid;
    }
}
