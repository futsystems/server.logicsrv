namespace FutsMoniter
{
    partial class ctSettlementPrice
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
            this.gridSettlementPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridSettlementPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSettlementPrice
            // 
            this.gridSettlementPrice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSettlementPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSettlementPrice.Location = new System.Drawing.Point(0, 0);
            this.gridSettlementPrice.Name = "gridSettlementPrice";
            this.gridSettlementPrice.RowTemplate.Height = 23;
            this.gridSettlementPrice.Size = new System.Drawing.Size(560, 279);
            this.gridSettlementPrice.TabIndex = 0;
            // 
            // ctSettlementPrice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridSettlementPrice);
            this.Name = "ctSettlementPrice";
            this.Size = new System.Drawing.Size(560, 279);
            ((System.ComponentModel.ISupportInitialize)(this.gridSettlementPrice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView gridSettlementPrice;
    }
}
