namespace FutsMoniter
{
    partial class ctTLPermissionEdit
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
            this.pmDesp = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pmValue = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.pmTitle = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.pmDesp);
            this.kryptonPanel1.Controls.Add(this.pmValue);
            this.kryptonPanel1.Controls.Add(this.pmTitle);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(670, 25);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // pmDesp
            // 
            this.pmDesp.Location = new System.Drawing.Point(245, 4);
            this.pmDesp.Name = "pmDesp";
            this.pmDesp.Size = new System.Drawing.Size(19, 18);
            this.pmDesp.TabIndex = 2;
            this.pmDesp.Values.Text = "--";
            // 
            // pmValue
            // 
            this.pmValue.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.pmValue.Location = new System.Drawing.Point(165, 4);
            this.pmValue.Name = "pmValue";
            this.pmValue.Size = new System.Drawing.Size(64, 18);
            this.pmValue.TabIndex = 1;
            this.pmValue.Text = "有权限";
            this.pmValue.Values.Text = "有权限";
            // 
            // pmTitle
            // 
            this.pmTitle.Location = new System.Drawing.Point(3, 3);
            this.pmTitle.Name = "pmTitle";
            this.pmTitle.Size = new System.Drawing.Size(19, 18);
            this.pmTitle.TabIndex = 0;
            this.pmTitle.Values.Text = "--";
            // 
            // ctTLPermissionEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctTLPermissionEdit";
            this.Size = new System.Drawing.Size(670, 25);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel pmTitle;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox pmValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel pmDesp;
    }
}
