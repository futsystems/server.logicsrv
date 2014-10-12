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
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnFilterCancelOrReject = new Telerik.WinControls.UI.RadRadioButton();
            this.btnFilterConfirmed = new Telerik.WinControls.UI.RadRadioButton();
            this.btnFilterPending = new Telerik.WinControls.UI.RadRadioButton();
            this.opgrid = new Telerik.WinControls.UI.RadGridView();
            this.ctGridExport1 = new FutsMoniter.ctGridExport();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterCancelOrReject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterConfirmed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterPending)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.ctGridExport1);
            this.radPanel1.Controls.Add(this.btnFilterCancelOrReject);
            this.radPanel1.Controls.Add(this.btnFilterConfirmed);
            this.radPanel1.Controls.Add(this.btnFilterPending);
            this.radPanel1.Controls.Add(this.opgrid);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(639, 356);
            this.radPanel1.TabIndex = 0;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            // 
            // btnFilterCancelOrReject
            // 
            this.btnFilterCancelOrReject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterCancelOrReject.Location = new System.Drawing.Point(130, 336);
            this.btnFilterCancelOrReject.Name = "btnFilterCancelOrReject";
            this.btnFilterCancelOrReject.Size = new System.Drawing.Size(73, 16);
            this.btnFilterCancelOrReject.TabIndex = 6;
            this.btnFilterCancelOrReject.Text = "取消/拒绝";
            this.btnFilterCancelOrReject.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterCancelOrReject_ToggleStateChanged);
            // 
            // btnFilterConfirmed
            // 
            this.btnFilterConfirmed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterConfirmed.Location = new System.Drawing.Point(67, 336);
            this.btnFilterConfirmed.Name = "btnFilterConfirmed";
            this.btnFilterConfirmed.Size = new System.Drawing.Size(58, 16);
            this.btnFilterConfirmed.TabIndex = 5;
            this.btnFilterConfirmed.Text = "已确认";
            this.btnFilterConfirmed.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterConfirmed_ToggleStateChanged);
            // 
            // btnFilterPending
            // 
            this.btnFilterPending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterPending.Location = new System.Drawing.Point(3, 336);
            this.btnFilterPending.Name = "btnFilterPending";
            this.btnFilterPending.Size = new System.Drawing.Size(58, 16);
            this.btnFilterPending.TabIndex = 4;
            this.btnFilterPending.Text = "待处理";
            this.btnFilterPending.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.btnFilterPending_ToggleStateChanged);
            // 
            // opgrid
            // 
            this.opgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.opgrid.ForeColor = System.Drawing.Color.Black;
            this.opgrid.Location = new System.Drawing.Point(0, 0);
            this.opgrid.Name = "opgrid";
            this.opgrid.Size = new System.Drawing.Size(639, 330);
            this.opgrid.TabIndex = 0;
            this.opgrid.Text = "radGridView1";
            this.opgrid.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.opgrid_CellFormatting);
            this.opgrid.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.opgrid_ContextMenuOpening);
            // 
            // ctGridExport1
            // 
            this.ctGridExport1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ctGridExport1.Grid = null;
            this.ctGridExport1.Location = new System.Drawing.Point(527, 331);
            this.ctGridExport1.Name = "ctGridExport1";
            this.ctGridExport1.Size = new System.Drawing.Size(110, 24);
            this.ctGridExport1.TabIndex = 7;
            // 
            // ctCashOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctCashOperation";
            this.Size = new System.Drawing.Size(639, 356);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterCancelOrReject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterConfirmed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilterPending)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadGridView opgrid;
        private Telerik.WinControls.UI.RadRadioButton btnFilterCancelOrReject;
        private Telerik.WinControls.UI.RadRadioButton btnFilterConfirmed;
        private Telerik.WinControls.UI.RadRadioButton btnFilterPending;
        private ctGridExport ctGridExport1;
    }
}
