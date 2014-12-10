namespace FutsMoniter.Controls.Base
{
    partial class ctRouterType
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
            this.routeType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.lbroutetype = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.routeType)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.routeType);
            this.kryptonPanel1.Controls.Add(this.lbroutetype);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(156, 21);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // routeType
            // 
            this.routeType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.routeType.DropDownWidth = 121;
            this.routeType.Location = new System.Drawing.Point(78, 0);
            this.routeType.Name = "routeType";
            this.routeType.Size = new System.Drawing.Size(78, 21);
            this.routeType.TabIndex = 16;
            this.routeType.Text = "--";
            // 
            // lbroutetype
            // 
            this.lbroutetype.Location = new System.Drawing.Point(0, 3);
            this.lbroutetype.Name = "lbroutetype";
            this.lbroutetype.Size = new System.Drawing.Size(68, 18);
            this.lbroutetype.TabIndex = 15;
            this.lbroutetype.Values.Text = "路由通道:";
            // 
            // ctRouterType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctRouterType";
            this.Size = new System.Drawing.Size(156, 21);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.routeType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox routeType;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbroutetype;
    }
}
