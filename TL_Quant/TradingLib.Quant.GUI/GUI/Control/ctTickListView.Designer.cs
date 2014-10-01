namespace TradingLib.Quant.View
{
    partial class ctTickListView
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
            this.gridView = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.DateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Trade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AskSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BidSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            this.gridView.AllowUserToResizeRows = false;
            this.gridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridView.ColumnHeadersHeight = 22;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DateTime,
            this.Trade,
            this.tSize,
            this.Ask,
            this.AskSize,
            this.Bid,
            this.BidSize});
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.Location = new System.Drawing.Point(0, 0);
            this.gridView.MultiSelect = false;
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RowHeadersVisible = false;
            this.gridView.RowTemplate.Height = 23;
            this.gridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridView.Size = new System.Drawing.Size(655, 176);
            this.gridView.TabIndex = 0;
            // 
            // DateTime
            // 
            this.DateTime.HeaderText = "时间";
            this.DateTime.Name = "DateTime";
            this.DateTime.ReadOnly = true;
            // 
            // Trade
            // 
            this.Trade.HeaderText = "最新价";
            this.Trade.Name = "Trade";
            this.Trade.ReadOnly = true;
            // 
            // tSize
            // 
            this.tSize.HeaderText = "现手";
            this.tSize.Name = "tSize";
            this.tSize.ReadOnly = true;
            // 
            // Ask
            // 
            this.Ask.HeaderText = "卖价";
            this.Ask.Name = "Ask";
            this.Ask.ReadOnly = true;
            // 
            // AskSize
            // 
            this.AskSize.HeaderText = "卖量";
            this.AskSize.Name = "AskSize";
            this.AskSize.ReadOnly = true;
            // 
            // Bid
            // 
            this.Bid.HeaderText = "买价";
            this.Bid.Name = "Bid";
            this.Bid.ReadOnly = true;
            // 
            // BidSize
            // 
            this.BidSize.HeaderText = "买量";
            this.BidSize.Name = "BidSize";
            this.BidSize.ReadOnly = true;
            // 
            // ctTickListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridView);
            this.Name = "ctTickListView";
            this.Size = new System.Drawing.Size(655, 176);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView gridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Trade;
        private System.Windows.Forms.DataGridViewTextBoxColumn tSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ask;
        private System.Windows.Forms.DataGridViewTextBoxColumn AskSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bid;
        private System.Windows.Forms.DataGridViewTextBoxColumn BidSize;
    }
}
