namespace FutsMoniter
{
    partial class ctTLEdit
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
            this.argvalue = new Telerik.WinControls.UI.RadTextBox();
            this.argtitle = new Telerik.WinControls.UI.RadLabel();
            this.argvalue_label = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.argvalue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.argtitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.argvalue_label)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.argvalue_label);
            this.radPanel1.Controls.Add(this.argvalue);
            this.radPanel1.Controls.Add(this.argtitle);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(288, 26);
            this.radPanel1.TabIndex = 0;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).LeftWidth = 0F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).TopWidth = 0F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).RightWidth = 0F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).BottomWidth = 0F;
            // 
            // argvalue
            // 
            this.argvalue.Location = new System.Drawing.Point(93, 3);
            this.argvalue.Name = "argvalue";
            this.argvalue.Size = new System.Drawing.Size(131, 18);
            this.argvalue.TabIndex = 1;
            // 
            // argtitle
            // 
            this.argtitle.Location = new System.Drawing.Point(3, 3);
            this.argtitle.Name = "argtitle";
            this.argtitle.Size = new System.Drawing.Size(14, 16);
            this.argtitle.TabIndex = 0;
            this.argtitle.Text = "--";
            // 
            // argvalue_label
            // 
            this.argvalue_label.Location = new System.Drawing.Point(93, 3);
            this.argvalue_label.Name = "argvalue_label";
            this.argvalue_label.Size = new System.Drawing.Size(14, 16);
            this.argvalue_label.TabIndex = 1;
            this.argvalue_label.Text = "--";
            // 
            // ctTLEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctTLEdit";
            this.Size = new System.Drawing.Size(288, 26);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.argvalue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.argtitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.argvalue_label)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadLabel argtitle;
        private Telerik.WinControls.UI.RadTextBox argvalue;
        private Telerik.WinControls.UI.RadLabel argvalue_label;
    }
}
