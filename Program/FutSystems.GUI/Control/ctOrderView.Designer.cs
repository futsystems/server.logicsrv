namespace FutSystems.GUI
{
    partial class ctOrderView
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
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnCancelAll = new Telerik.WinControls.UI.RadButton();
            this.btnCancelOrder = new Telerik.WinControls.UI.RadButton();
            this.btnFilterCancelError = new Telerik.WinControls.UI.RadRadioButton();
            this.btnFilterFilled = new Telerik.WinControls.UI.RadRadioButton();
            this.btnFilterPlaced = new Telerik.WinControls.UI.RadRadioButton();
            this.btnFilterAll = new Telerik.WinControls.UI.RadRadioButton();
            this.orderGrid = new Telerik.WinControls.UI.RadGridView();
            this.num = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterCancelError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterFilled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterPlaced)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.num);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Controls.Add(this.btnCancelAll);
            this.radPanel1.Controls.Add(this.btnCancelOrder);
            this.radPanel1.Controls.Add(this.btnFilterCancelError);
            this.radPanel1.Controls.Add(this.btnFilterFilled);
            this.radPanel1.Controls.Add(this.btnFilterPlaced);
            this.radPanel1.Controls.Add(this.btnFilterAll);
            this.radPanel1.Controls.Add(this.orderGrid);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(697, 175);
            this.radPanel1.TabIndex = 0;
            this.radPanel1.Text = "radPanel1";
            // 
            // btnCancelAll
            // 
            this.btnCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelAll.Location = new System.Drawing.Point(353, 151);
            this.btnCancelAll.Name = "btnCancelAll";
            this.btnCancelAll.Size = new System.Drawing.Size(60, 22);
            this.btnCancelAll.TabIndex = 6;
            this.btnCancelAll.Text = "全 撤";
            this.btnCancelAll.Click += new System.EventHandler(this.btnCancelAll_Click);
            // 
            // btnCancelOrder
            // 
            this.btnCancelOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelOrder.Location = new System.Drawing.Point(287, 151);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(60, 22);
            this.btnCancelOrder.TabIndex = 5;
            this.btnCancelOrder.Text = "撤 单";
            this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            // 
            // btnFilterCancelError
            // 
            this.btnFilterCancelError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterCancelError.Location = new System.Drawing.Point(195, 154);
            this.btnFilterCancelError.Name = "btnFilterCancelError";
            this.btnFilterCancelError.Size = new System.Drawing.Size(86, 16);
            this.btnFilterCancelError.TabIndex = 4;
            this.btnFilterCancelError.Text = "已撤单/错误";
            this.btnFilterCancelError.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterCancelError_ToggleStateChanged);
            // 
            // btnFilterFilled
            // 
            this.btnFilterFilled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterFilled.Location = new System.Drawing.Point(131, 154);
            this.btnFilterFilled.Name = "btnFilterFilled";
            this.btnFilterFilled.Size = new System.Drawing.Size(58, 16);
            this.btnFilterFilled.TabIndex = 3;
            this.btnFilterFilled.Text = "已成交";
            this.btnFilterFilled.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterFilled_ToggleStateChanged);
            // 
            // btnFilterPlaced
            // 
            this.btnFilterPlaced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterPlaced.Location = new System.Drawing.Point(80, 154);
            this.btnFilterPlaced.Name = "btnFilterPlaced";
            this.btnFilterPlaced.Size = new System.Drawing.Size(45, 16);
            this.btnFilterPlaced.TabIndex = 2;
            this.btnFilterPlaced.Text = "挂单";
            this.btnFilterPlaced.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterPlaced_ToggleStateChanged);
            // 
            // btnFilterAll
            // 
            this.btnFilterAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterAll.Location = new System.Drawing.Point(4, 154);
            this.btnFilterAll.Name = "btnFilterAll";
            this.btnFilterAll.Size = new System.Drawing.Size(70, 16);
            this.btnFilterAll.TabIndex = 1;
            this.btnFilterAll.Text = "所有委托";
            this.btnFilterAll.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterAll_ToggleStateChanged);
            // 
            // orderGrid
            // 
            this.orderGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.orderGrid.Location = new System.Drawing.Point(0, 0);
            this.orderGrid.Margin = new System.Windows.Forms.Padding(0);
            this.orderGrid.Name = "orderGrid";
            this.orderGrid.Size = new System.Drawing.Size(697, 150);
            this.orderGrid.TabIndex = 0;
            this.orderGrid.Text = "radGridView1";
            this.orderGrid.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.orderGrid_CellFormatting);
            this.orderGrid.DoubleClick += new System.EventHandler(this.orderGrid_DoubleClick);
            // 
            // num
            // 
            this.num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.num.Location = new System.Drawing.Point(638, 153);
            this.num.Name = "num";
            this.num.Size = new System.Drawing.Size(14, 16);
            this.num.TabIndex = 8;
            this.num.Text = "--";
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel1.Location = new System.Drawing.Point(573, 153);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 7;
            this.radLabel1.Text = "记录条数:";
            // 
            // ctOrderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctOrderView";
            this.Size = new System.Drawing.Size(697, 175);
            this.Load += new System.EventHandler(this.ctOrderView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterCancelError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterFilled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterPlaced)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadGridView orderGrid;
        private Telerik.WinControls.UI.RadRadioButton btnFilterCancelError;
        private Telerik.WinControls.UI.RadRadioButton btnFilterFilled;
        private Telerik.WinControls.UI.RadRadioButton btnFilterPlaced;
        private Telerik.WinControls.UI.RadRadioButton btnFilterAll;
        private Telerik.WinControls.UI.RadButton btnCancelAll;
        private Telerik.WinControls.UI.RadButton btnCancelOrder;
        private Telerik.WinControls.UI.RadLabel num;
        private Telerik.WinControls.UI.RadLabel radLabel1;

    }
}
