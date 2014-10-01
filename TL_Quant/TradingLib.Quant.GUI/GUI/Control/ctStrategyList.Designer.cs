namespace TradingLib.Quant.GUI
{
    partial class ctStrategyList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctStrategyList));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.strategylist = new System.Windows.Forms.ListBox();
            this.reload = new System.Windows.Forms.ToolStripButton();
            this.strategymemo = new System.Windows.Forms.RichTextBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reload});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(220, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // strategylist
            // 
            this.strategylist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.strategylist.FormattingEnabled = true;
            this.strategylist.ItemHeight = 12;
            this.strategylist.Location = new System.Drawing.Point(0, 25);
            this.strategylist.Name = "strategylist";
            this.strategylist.Size = new System.Drawing.Size(220, 256);
            this.strategylist.TabIndex = 2;
            // 
            // reload
            // 
            this.reload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reload.Image = ((System.Drawing.Image)(resources.GetObject("reload.Image")));
            this.reload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reload.Name = "reload";
            this.reload.Size = new System.Drawing.Size(23, 22);
            this.reload.Text = "刷新可用策略列表";
            this.reload.Click += new System.EventHandler(this.reload_Click);
            // 
            // strategymemo
            // 
            this.strategymemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.strategymemo.BackColor = System.Drawing.SystemColors.Info;
            this.strategymemo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.strategymemo.Location = new System.Drawing.Point(0, 281);
            this.strategymemo.Name = "strategymemo";
            this.strategymemo.Size = new System.Drawing.Size(220, 91);
            this.strategymemo.TabIndex = 3;
            this.strategymemo.Text = "";
            // 
            // ctStrategyList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.strategymemo);
            this.Controls.Add(this.strategylist);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ctStrategyList";
            this.Size = new System.Drawing.Size(220, 372);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton reload;
        private System.Windows.Forms.ListBox strategylist;
        private System.Windows.Forms.RichTextBox strategymemo;

    }
}
