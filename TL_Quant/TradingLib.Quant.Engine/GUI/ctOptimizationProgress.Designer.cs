namespace TradingLib.Quant.Engine
{
    partial class ctOptimizationProgress
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
            this.kryptonPanel1 = new System.Windows.Forms.Panel();
            this.worker = new System.Windows.Forms.Label();
            this.kryptonLabel1 = new System.Windows.Forms.Label();
            this.message = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.worker);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.message);
            this.kryptonPanel1.Controls.Add(this.progress);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(317, 38);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // worker
            // 
            this.worker.Location = new System.Drawing.Point(3, 8);
            this.worker.Name = "worker";
            this.worker.Size = new System.Drawing.Size(19, 18);
            this.worker.TabIndex = 3;
            this.worker.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(50, 18);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Text = "信息:";
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(88, 18);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(19, 18);
            this.message.TabIndex = 1;
            this.message.Text = "--";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(55, 3);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(255, 15);
            this.progress.TabIndex = 0;
            // 
            // ctOptimizationProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctOptimizationProgress";
            this.Size = new System.Drawing.Size(317, 38);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel kryptonPanel1;
        private System.Windows.Forms.Label worker;
        private System.Windows.Forms.Label kryptonLabel1;
        private System.Windows.Forms.Label message;
        private System.Windows.Forms.ProgressBar progress;
    }
}
