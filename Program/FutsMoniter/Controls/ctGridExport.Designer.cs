namespace FutsMoniter
{
    partial class ctGridExport
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
            this.radSplitButton1 = new Telerik.WinControls.UI.RadSplitButton();
            this.btnExportPDF = new Telerik.WinControls.UI.RadMenuItem();
            this.btnExportExcel = new Telerik.WinControls.UI.RadMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitButton1)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radSplitButton1);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(110, 24);
            this.radPanel1.TabIndex = 0;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            // 
            // radSplitButton1
            // 
            this.radSplitButton1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.btnExportPDF,
            this.btnExportExcel});
            this.radSplitButton1.Location = new System.Drawing.Point(0, 0);
            this.radSplitButton1.Name = "radSplitButton1";
            this.radSplitButton1.Size = new System.Drawing.Size(110, 24);
            this.radSplitButton1.TabIndex = 58;
            this.radSplitButton1.Text = "输文件出";
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.AccessibleDescription = "导出PDF";
            this.btnExportPDF.AccessibleName = "导出PDF";
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Text = "导出PDF";
            this.btnExportPDF.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.AccessibleDescription = "导出到Excel";
            this.btnExportExcel.AccessibleName = "导出到Excel";
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Text = "导出到Excel";
            this.btnExportExcel.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // ctGridExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctGridExport";
            this.Size = new System.Drawing.Size(110, 24);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radSplitButton1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadSplitButton radSplitButton1;
        private Telerik.WinControls.UI.RadMenuItem btnExportPDF;
        private Telerik.WinControls.UI.RadMenuItem btnExportExcel;
    }
}
