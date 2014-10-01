namespace TradingLib.Quant.GUI
{
    partial class ctIndicatorList
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("趋势");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("动量");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("成交量");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("波动率");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("其他");
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Trend";
            treeNode1.Text = "趋势";
            treeNode2.Name = "Momentum";
            treeNode2.Text = "动量";
            treeNode3.Name = "Volume";
            treeNode3.Text = "成交量";
            treeNode4.Name = "Volatility";
            treeNode4.Text = "波动率";
            treeNode5.Name = "Other";
            treeNode5.Text = "其他";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            this.treeView1.Size = new System.Drawing.Size(179, 368);
            this.treeView1.TabIndex = 0;
            // 
            // ctIndicatorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Name = "ctIndicatorList";
            this.Size = new System.Drawing.Size(179, 368);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;


    }
}
