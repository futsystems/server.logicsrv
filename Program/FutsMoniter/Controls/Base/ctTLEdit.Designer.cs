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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.argvalue = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.argvalue_label = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.argtitle = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.argboolcheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.argboolcheck);
            this.kryptonPanel1.Controls.Add(this.argvalue);
            this.kryptonPanel1.Controls.Add(this.argvalue_label);
            this.kryptonPanel1.Controls.Add(this.argtitle);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(257, 26);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // argvalue
            // 
            this.argvalue.Location = new System.Drawing.Point(115, 1);
            this.argvalue.Name = "argvalue";
            this.argvalue.Size = new System.Drawing.Size(136, 21);
            this.argvalue.TabIndex = 2;
            // 
            // argvalue_label
            // 
            this.argvalue_label.Location = new System.Drawing.Point(115, 3);
            this.argvalue_label.Name = "argvalue_label";
            this.argvalue_label.Size = new System.Drawing.Size(19, 18);
            this.argvalue_label.TabIndex = 1;
            this.argvalue_label.Values.Text = "--";
            // 
            // argtitle
            // 
            this.argtitle.Location = new System.Drawing.Point(4, 4);
            this.argtitle.Name = "argtitle";
            this.argtitle.Size = new System.Drawing.Size(19, 18);
            this.argtitle.TabIndex = 0;
            this.argtitle.Values.Text = "--";
            // 
            // argboolcheck
            // 
            this.argboolcheck.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.argboolcheck.Location = new System.Drawing.Point(115, 5);
            this.argboolcheck.Name = "argboolcheck";
            this.argboolcheck.Size = new System.Drawing.Size(19, 13);
            this.argboolcheck.TabIndex = 3;
            this.argboolcheck.Values.Text = "";
            // 
            // ctTLEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctTLEdit";
            this.Size = new System.Drawing.Size(257, 26);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox argvalue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel argvalue_label;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel argtitle;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox argboolcheck;

    }
}
