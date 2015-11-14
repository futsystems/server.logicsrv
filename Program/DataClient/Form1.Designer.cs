﻿namespace DataClient
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Symbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.maxcount = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.fromend = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.start = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.end = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.btnQryBar = new System.Windows.Forms.Button();
            this.interval = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
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
            this.debugControl1.Size = new System.Drawing.Size(450, 405);
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
            this.btnMQClient.Location = new System.Drawing.Point(465, 12);
            this.btnMQClient.Name = "btnMQClient";
            this.btnMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnMQClient.TabIndex = 2;
            this.btnMQClient.Text = "StartMQClient";
            this.btnMQClient.UseVisualStyleBackColor = true;
            this.btnMQClient.Click += new System.EventHandler(this.btnMQClient_Click);
            // 
            // btnQryService
            // 
            this.btnQryService.Location = new System.Drawing.Point(627, 12);
            this.btnQryService.Name = "btnQryService";
            this.btnQryService.Size = new System.Drawing.Size(75, 23);
            this.btnQryService.TabIndex = 3;
            this.btnQryService.Text = "QryService";
            this.btnQryService.UseVisualStyleBackColor = true;
            this.btnQryService.Click += new System.EventHandler(this.btnQryService_Click);
            // 
            // btnStopMQClient
            // 
            this.btnStopMQClient.Location = new System.Drawing.Point(546, 12);
            this.btnStopMQClient.Name = "btnStopMQClient";
            this.btnStopMQClient.Size = new System.Drawing.Size(75, 23);
            this.btnStopMQClient.TabIndex = 4;
            this.btnStopMQClient.Text = "StopMQClient";
            this.btnStopMQClient.UseVisualStyleBackColor = true;
            this.btnStopMQClient.Click += new System.EventHandler(this.btnStopMQClient_Click);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(465, 53);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel1.TabIndex = 5;
            this.kryptonLabel1.Values.Text = "Symbol";
            // 
            // Symbol
            // 
            this.Symbol.Location = new System.Drawing.Point(522, 54);
            this.Symbol.Name = "Symbol";
            this.Symbol.Size = new System.Drawing.Size(63, 20);
            this.Symbol.TabIndex = 6;
            this.Symbol.Text = "HGZ5";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(591, 53);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(67, 20);
            this.kryptonLabel2.TabIndex = 7;
            this.kryptonLabel2.Values.Text = "MaxCount";
            // 
            // maxcount
            // 
            this.maxcount.Location = new System.Drawing.Point(664, 52);
            this.maxcount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.maxcount.Name = "maxcount";
            this.maxcount.Size = new System.Drawing.Size(52, 22);
            this.maxcount.TabIndex = 8;
            this.maxcount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // fromend
            // 
            this.fromend.Location = new System.Drawing.Point(725, 53);
            this.fromend.Name = "fromend";
            this.fromend.Size = new System.Drawing.Size(72, 20);
            this.fromend.TabIndex = 9;
            this.fromend.Values.Text = "FromEnd";
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(507, 79);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(209, 21);
            this.start.TabIndex = 10;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(465, 81);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(36, 20);
            this.kryptonLabel3.TabIndex = 11;
            this.kryptonLabel3.Values.Text = "Start";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(465, 106);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(31, 20);
            this.kryptonLabel4.TabIndex = 12;
            this.kryptonLabel4.Values.Text = "End";
            // 
            // end
            // 
            this.end.Location = new System.Drawing.Point(507, 106);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(209, 21);
            this.end.TabIndex = 13;
            // 
            // btnQryBar
            // 
            this.btnQryBar.Location = new System.Drawing.Point(725, 81);
            this.btnQryBar.Name = "btnQryBar";
            this.btnQryBar.Size = new System.Drawing.Size(75, 45);
            this.btnQryBar.TabIndex = 14;
            this.btnQryBar.Text = "QryBar";
            this.btnQryBar.UseVisualStyleBackColor = true;
            this.btnQryBar.Click += new System.EventHandler(this.btnQryBar_Click);
            // 
            // interval
            // 
            this.interval.Location = new System.Drawing.Point(533, 133);
            this.interval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.interval.Name = "interval";
            this.interval.Size = new System.Drawing.Size(52, 22);
            this.interval.TabIndex = 16;
            this.interval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(465, 133);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(51, 20);
            this.kryptonLabel5.TabIndex = 15;
            this.kryptonLabel5.Values.Text = "Interval";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 405);
            this.Controls.Add(this.interval);
            this.Controls.Add(this.kryptonLabel5);
            this.Controls.Add(this.btnQryBar);
            this.Controls.Add(this.end);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.start);
            this.Controls.Add(this.fromend);
            this.Controls.Add(this.maxcount);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.Symbol);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.btnStopMQClient);
            this.Controls.Add(this.btnQryService);
            this.Controls.Add(this.btnMQClient);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "行情客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TLDataClient.DebugControl debugControl1;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnMQClient;
        private System.Windows.Forms.Button btnQryService;
        private System.Windows.Forms.Button btnStopMQClient;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Symbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown maxcount;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox fromend;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker start;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker end;
        private System.Windows.Forms.Button btnQryBar;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown interval;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
    }
}

