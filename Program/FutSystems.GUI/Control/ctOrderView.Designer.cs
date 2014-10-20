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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.num = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnCancelAll = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnCancelOrder = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnFilterCancelError = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFilterFilled = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFilterPlaced = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFilterAll = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.orderGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.num);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.btnCancelAll);
            this.kryptonPanel1.Controls.Add(this.btnCancelOrder);
            this.kryptonPanel1.Controls.Add(this.btnFilterCancelError);
            this.kryptonPanel1.Controls.Add(this.btnFilterFilled);
            this.kryptonPanel1.Controls.Add(this.btnFilterPlaced);
            this.kryptonPanel1.Controls.Add(this.btnFilterAll);
            this.kryptonPanel1.Controls.Add(this.orderGrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(838, 219);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // num
            // 
            this.num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.num.Location = new System.Drawing.Point(806, 194);
            this.num.Name = "num";
            this.num.Size = new System.Drawing.Size(19, 18);
            this.num.TabIndex = 8;
            this.num.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonLabel1.Location = new System.Drawing.Point(717, 194);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 7;
            this.kryptonLabel1.Values.Text = "记录条数:";
            // 
            // btnCancelAll
            // 
            this.btnCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelAll.Location = new System.Drawing.Point(384, 191);
            this.btnCancelAll.Name = "btnCancelAll";
            this.btnCancelAll.Size = new System.Drawing.Size(64, 25);
            this.btnCancelAll.TabIndex = 6;
            this.btnCancelAll.Values.Text = "全 撤";
            // 
            // btnCancelOrder
            // 
            this.btnCancelOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelOrder.Location = new System.Drawing.Point(314, 191);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(64, 25);
            this.btnCancelOrder.TabIndex = 5;
            this.btnCancelOrder.Values.Text = "撤 单";
            // 
            // btnFilterCancelError
            // 
            this.btnFilterCancelError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterCancelError.Location = new System.Drawing.Point(201, 194);
            this.btnFilterCancelError.Name = "btnFilterCancelError";
            this.btnFilterCancelError.Size = new System.Drawing.Size(94, 18);
            this.btnFilterCancelError.TabIndex = 4;
            this.btnFilterCancelError.Values.Text = "已撤单/错误";
            // 
            // btnFilterFilled
            // 
            this.btnFilterFilled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterFilled.Location = new System.Drawing.Point(132, 194);
            this.btnFilterFilled.Name = "btnFilterFilled";
            this.btnFilterFilled.Size = new System.Drawing.Size(63, 18);
            this.btnFilterFilled.TabIndex = 3;
            this.btnFilterFilled.Values.Text = "已成交";
            // 
            // btnFilterPlaced
            // 
            this.btnFilterPlaced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterPlaced.Location = new System.Drawing.Point(76, 194);
            this.btnFilterPlaced.Name = "btnFilterPlaced";
            this.btnFilterPlaced.Size = new System.Drawing.Size(50, 18);
            this.btnFilterPlaced.TabIndex = 2;
            this.btnFilterPlaced.Values.Text = "挂单";
            // 
            // btnFilterAll
            // 
            this.btnFilterAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterAll.Location = new System.Drawing.Point(3, 194);
            this.btnFilterAll.Name = "btnFilterAll";
            this.btnFilterAll.Size = new System.Drawing.Size(77, 18);
            this.btnFilterAll.TabIndex = 1;
            this.btnFilterAll.Values.Text = "所有委托";
            // 
            // orderGrid
            // 
            this.orderGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.orderGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.orderGrid.Location = new System.Drawing.Point(0, 0);
            this.orderGrid.Name = "orderGrid";
            this.orderGrid.RowTemplate.Height = 23;
            this.orderGrid.Size = new System.Drawing.Size(838, 188);
            this.orderGrid.TabIndex = 0;
            // 
            // ctOrderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.DoubleBuffered = true;
            this.Name = "ctOrderView";
            this.Size = new System.Drawing.Size(838, 219);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orderGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterCancelError;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterFilled;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterPlaced;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterAll;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView orderGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancelAll;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancelOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel num;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;

    }
}
