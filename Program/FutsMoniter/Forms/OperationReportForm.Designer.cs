namespace FutsMoniter
{
    partial class OperationReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.opgrid = new Telerik.WinControls.UI.RadGridView();
            this.ctGridExport1 = new FutsMoniter.ctGridExport();
            this.btnRefresh = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // opgrid
            // 
            this.opgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.opgrid.Location = new System.Drawing.Point(-3, 1);
            this.opgrid.Name = "opgrid";
            this.opgrid.Size = new System.Drawing.Size(796, 384);
            this.opgrid.TabIndex = 0;
            this.opgrid.Text = "radGridView1";
            // 
            // ctGridExport1
            // 
            this.ctGridExport1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ctGridExport1.Grid = null;
            this.ctGridExport1.Location = new System.Drawing.Point(0, 391);
            this.ctGridExport1.Name = "ctGridExport1";
            this.ctGridExport1.Size = new System.Drawing.Size(110, 24);
            this.ctGridExport1.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(719, 391);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(58, 24);
            this.btnRefresh.TabIndex = 39;
            this.btnRefresh.Text = "刷 新";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // OperationReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 415);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.ctGridExport1);
            this.Controls.Add(this.opgrid);
            this.Name = "OperationReportForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "当日运营报表";
            ((System.ComponentModel.ISupportInitialize)(this.opgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView opgrid;
        private ctGridExport ctGridExport1;
        private Telerik.WinControls.UI.RadButton btnRefresh;
    }
}