namespace DataClient
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
            this.debugControl1 = new TLDataClient.DebugControl();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnMQClient = new System.Windows.Forms.Button();
            this.btnQryService = new System.Windows.Forms.Button();
            this.btnStopMQClient = new System.Windows.Forms.Button();
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
            this.debugControl1.Size = new System.Drawing.Size(120, 405);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(725, 21);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "初始化";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnMQClient
            // 
            this.btnMQClient.Location = new System.Drawing.Point(583, 71);
            this.btnMQClient.Name = "btnMQClient";
            this.btnMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnMQClient.TabIndex = 2;
            this.btnMQClient.Text = "StartMQClient";
            this.btnMQClient.UseVisualStyleBackColor = true;
            this.btnMQClient.Click += new System.EventHandler(this.btnMQClient_Click);
            // 
            // btnQryService
            // 
            this.btnQryService.Location = new System.Drawing.Point(715, 71);
            this.btnQryService.Name = "btnQryService";
            this.btnQryService.Size = new System.Drawing.Size(75, 23);
            this.btnQryService.TabIndex = 3;
            this.btnQryService.Text = "QryService";
            this.btnQryService.UseVisualStyleBackColor = true;
            this.btnQryService.Click += new System.EventHandler(this.btnQryService_Click);
            // 
            // btnStopMQClient
            // 
            this.btnStopMQClient.Location = new System.Drawing.Point(583, 100);
            this.btnStopMQClient.Name = "btnStopMQClient";
            this.btnStopMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnStopMQClient.TabIndex = 4;
            this.btnStopMQClient.Text = "StopMQClient";
            this.btnStopMQClient.UseVisualStyleBackColor = true;
            this.btnStopMQClient.Click += new System.EventHandler(this.btnStopMQClient_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 405);
            this.Controls.Add(this.btnStopMQClient);
            this.Controls.Add(this.btnQryService);
            this.Controls.Add(this.btnMQClient);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "行情客户端";
            this.ResumeLayout(false);

        }

        #endregion

        private TLDataClient.DebugControl debugControl1;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnMQClient;
        private System.Windows.Forms.Button btnQryService;
        private System.Windows.Forms.Button btnStopMQClient;
    }
}

