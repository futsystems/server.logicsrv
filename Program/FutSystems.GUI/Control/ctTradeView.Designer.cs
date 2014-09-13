namespace FutSystems.GUI.Control
{
    partial class ctTradeView
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
            this.tradeGrid = new Telerik.WinControls.UI.RadGridView();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.num = new Telerik.WinControls.UI.RadLabel();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            ((System.ComponentModel.ISupportInitialize)(this.tradeGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tradeGrid
            // 
            this.tradeGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tradeGrid.Location = new System.Drawing.Point(0, 0);
            this.tradeGrid.Margin = new System.Windows.Forms.Padding(0);
            // 
            // tradeGrid
            // 
            this.tradeGrid.MasterTemplate.AllowAddNewRow = false;
            this.tradeGrid.MasterTemplate.AllowColumnChooser = false;
            this.tradeGrid.MasterTemplate.AllowColumnHeaderContextMenu = false;
            this.tradeGrid.MasterTemplate.AllowDeleteRow = false;
            this.tradeGrid.MasterTemplate.AllowDragToGroup = false;
            this.tradeGrid.MasterTemplate.AllowEditRow = false;
            this.tradeGrid.MasterTemplate.AllowRowResize = false;
            this.tradeGrid.MasterTemplate.EnableSorting = false;
            this.tradeGrid.MasterTemplate.ShowRowHeaderColumn = false;
            this.tradeGrid.Name = "tradeGrid";
            this.tradeGrid.ReadOnly = true;
            this.tradeGrid.ShowGroupPanel = false;
            this.tradeGrid.Size = new System.Drawing.Size(683, 207);
            this.tradeGrid.TabIndex = 0;
            this.tradeGrid.Text = "radGridView1";
            this.tradeGrid.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.tradeGrid_CellFormatting);
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel1.Location = new System.Drawing.Point(558, 210);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "记录条数:";
            // 
            // num
            // 
            this.num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.num.Location = new System.Drawing.Point(623, 210);
            this.num.Name = "num";
            this.num.Size = new System.Drawing.Size(14, 16);
            this.num.TabIndex = 2;
            this.num.Text = "--";
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.num);
            this.radPanel1.Controls.Add(this.tradeGrid);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(683, 232);
            this.radPanel1.TabIndex = 1;
            this.radPanel1.Text = "radPanel1";
            // 
            // ctTradeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctTradeView";
            this.Size = new System.Drawing.Size(683, 232);
            this.Load += new System.EventHandler(this.ctTradeView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tradeGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView tradeGrid;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel num;
        private Telerik.WinControls.UI.RadPanel radPanel1;
    }
}
