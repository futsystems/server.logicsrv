namespace DataFarm
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.debugControl1 = new TLDataFarm.DebugControl();
            this.btnInitServer = new System.Windows.Forms.Button();
            this.TickProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // debugControl1
            // 
            this.debugControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.debugControl1.EnableSearching = true;
            this.debugControl1.ExternalTimeStamp = 0;
            this.debugControl1.Location = new System.Drawing.Point(0, 0);
            this.debugControl1.Margin = new System.Windows.Forms.Padding(2);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(611, 425);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // btnInitServer
            // 
            this.btnInitServer.Location = new System.Drawing.Point(731, 12);
            this.btnInitServer.Name = "btnInitServer";
            this.btnInitServer.Size = new System.Drawing.Size(54, 23);
            this.btnInitServer.TabIndex = 1;
            this.btnInitServer.Text = "启动";
            this.btnInitServer.UseVisualStyleBackColor = true;
            this.btnInitServer.Click += new System.EventHandler(this.btnInitServer_Click);
            // 
            // TickProcess
            // 
            this.TickProcess.Location = new System.Drawing.Point(731, 50);
            this.TickProcess.Name = "TickProcess";
            this.TickProcess.Size = new System.Drawing.Size(54, 23);
            this.TickProcess.TabIndex = 2;
            this.TickProcess.Text = "Tick";
            this.TickProcess.UseVisualStyleBackColor = true;
            this.TickProcess.Click += new System.EventHandler(this.TickProcess_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 425);
            this.Controls.Add(this.TickProcess);
            this.Controls.Add(this.btnInitServer);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private TLDataFarm.DebugControl debugControl1;
        private System.Windows.Forms.Button btnInitServer;
        private System.Windows.Forms.Button TickProcess;
    }
}

