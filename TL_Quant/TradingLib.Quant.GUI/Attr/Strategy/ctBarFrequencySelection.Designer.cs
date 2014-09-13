namespace TradingLib.Quant.Common
{
    partial class ctBarFrequencySelection
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
            this.barFreqList = new System.Windows.Forms.ListBox();
            this.more = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.SuspendLayout();
            // 
            // barFreqList
            // 
            this.barFreqList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.barFreqList.FormattingEnabled = true;
            this.barFreqList.ItemHeight = 12;
            this.barFreqList.Location = new System.Drawing.Point(0, 0);
            this.barFreqList.Name = "barFreqList";
            this.barFreqList.Size = new System.Drawing.Size(162, 232);
            this.barFreqList.TabIndex = 0;
            // 
            // more
            // 
            this.more.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.more.Location = new System.Drawing.Point(94, 233);
            this.more.Name = "more";
            this.more.Size = new System.Drawing.Size(65, 18);
            this.more.TabIndex = 1;
            this.more.Values.Text = "选择其他";
            this.more.LinkClicked += new System.EventHandler(this.more_LinkClicked);
            // 
            // ctBarFrequencySelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.more);
            this.Controls.Add(this.barFreqList);
            this.Name = "ctBarFrequencySelection";
            this.Size = new System.Drawing.Size(162, 251);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox barFreqList;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel more;
    }
}
