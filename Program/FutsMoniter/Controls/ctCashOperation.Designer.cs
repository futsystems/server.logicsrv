namespace FutsMoniter
{
    partial class ctCashOperation
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
            this.btnFilterCancelOrReject = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFilterConfirmed = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFilterPending = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.opgrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnFilterCancelOrReject);
            this.kryptonPanel1.Controls.Add(this.btnFilterConfirmed);
            this.kryptonPanel1.Controls.Add(this.btnFilterPending);
            this.kryptonPanel1.Controls.Add(this.opgrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(826, 340);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnFilterCancelOrReject
            // 
            this.btnFilterCancelOrReject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterCancelOrReject.Location = new System.Drawing.Point(743, 310);
            this.btnFilterCancelOrReject.Name = "btnFilterCancelOrReject";
            this.btnFilterCancelOrReject.Size = new System.Drawing.Size(80, 18);
            this.btnFilterCancelOrReject.TabIndex = 4;
            this.btnFilterCancelOrReject.Values.Text = "取消/拒绝";
            // 
            // btnFilterConfirmed
            // 
            this.btnFilterConfirmed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterConfirmed.Location = new System.Drawing.Point(674, 310);
            this.btnFilterConfirmed.Name = "btnFilterConfirmed";
            this.btnFilterConfirmed.Size = new System.Drawing.Size(63, 18);
            this.btnFilterConfirmed.TabIndex = 3;
            this.btnFilterConfirmed.Values.Text = "已确认";
            // 
            // btnFilterPending
            // 
            this.btnFilterPending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterPending.Location = new System.Drawing.Point(605, 310);
            this.btnFilterPending.Name = "btnFilterPending";
            this.btnFilterPending.Size = new System.Drawing.Size(63, 18);
            this.btnFilterPending.TabIndex = 2;
            this.btnFilterPending.Values.Text = "待处理";
            // 
            // opgrid
            // 
            this.opgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.opgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.opgrid.Location = new System.Drawing.Point(0, 0);
            this.opgrid.Name = "opgrid";
            this.opgrid.RowTemplate.Height = 23;
            this.opgrid.Size = new System.Drawing.Size(826, 300);
            this.opgrid.TabIndex = 0;
            // 
            // ctCashOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctCashOperation";
            this.Size = new System.Drawing.Size(826, 340);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView opgrid;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterCancelOrReject;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterConfirmed;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnFilterPending;
    }
}
