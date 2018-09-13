namespace LicManager
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
            this.label1 = new System.Windows.Forms.Label();
            this.deploy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.account_cnt = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.enableAPI = new System.Windows.Forms.CheckBox();
            this.enableAPP = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.hardwareId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.expreDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.counter_cnt = new System.Windows.Forms.NumericUpDown();
            this.agent_cnt = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.account_cnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.counter_cnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent_cnt)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "部署编号:";
            // 
            // deploy
            // 
            this.deploy.Location = new System.Drawing.Point(237, 37);
            this.deploy.Name = "deploy";
            this.deploy.Size = new System.Drawing.Size(221, 35);
            this.deploy.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "交易账户数量:";
            // 
            // account_cnt
            // 
            this.account_cnt.Location = new System.Drawing.Point(237, 93);
            this.account_cnt.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.account_cnt.Name = "account_cnt";
            this.account_cnt.Size = new System.Drawing.Size(120, 35);
            this.account_cnt.TabIndex = 3;
            this.account_cnt.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 238);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "API接口可用:";
            // 
            // enableAPI
            // 
            this.enableAPI.AutoSize = true;
            this.enableAPI.Location = new System.Drawing.Point(237, 237);
            this.enableAPI.Name = "enableAPI";
            this.enableAPI.Size = new System.Drawing.Size(28, 27);
            this.enableAPI.TabIndex = 7;
            this.enableAPI.UseVisualStyleBackColor = true;
            // 
            // enableAPP
            // 
            this.enableAPP.AutoSize = true;
            this.enableAPP.Location = new System.Drawing.Point(237, 285);
            this.enableAPP.Name = "enableAPP";
            this.enableAPP.Size = new System.Drawing.Size(28, 27);
            this.enableAPP.TabIndex = 9;
            this.enableAPP.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "移动端可用:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(925, 274);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(192, 51);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "生成License";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // hardwareId
            // 
            this.hardwareId.Location = new System.Drawing.Point(764, 37);
            this.hardwareId.Name = "hardwareId";
            this.hardwareId.Size = new System.Drawing.Size(340, 35);
            this.hardwareId.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(599, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 24);
            this.label6.TabIndex = 11;
            this.label6.Text = "硬件编号:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(599, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 24);
            this.label7.TabIndex = 13;
            this.label7.Text = "到期日期:";
            // 
            // expreDate
            // 
            this.expreDate.Location = new System.Drawing.Point(764, 95);
            this.expreDate.Name = "expreDate";
            this.expreDate.Size = new System.Drawing.Size(245, 35);
            this.expreDate.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "分区数量:";
            // 
            // counter_cnt
            // 
            this.counter_cnt.Location = new System.Drawing.Point(237, 181);
            this.counter_cnt.Name = "counter_cnt";
            this.counter_cnt.Size = new System.Drawing.Size(120, 35);
            this.counter_cnt.TabIndex = 5;
            this.counter_cnt.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // agent_cnt
            // 
            this.agent_cnt.Location = new System.Drawing.Point(237, 136);
            this.agent_cnt.Name = "agent_cnt";
            this.agent_cnt.Size = new System.Drawing.Size(120, 35);
            this.agent_cnt.TabIndex = 16;
            this.agent_cnt.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(72, 138);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 24);
            this.label8.TabIndex = 15;
            this.label8.Text = "柜员数量:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 357);
            this.Controls.Add(this.agent_cnt);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.expreDate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.hardwareId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.enableAPP);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.enableAPI);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.counter_cnt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.account_cnt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.deploy);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "License管理端";
            ((System.ComponentModel.ISupportInitialize)(this.account_cnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.counter_cnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent_cnt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox deploy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown account_cnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox enableAPI;
        private System.Windows.Forms.CheckBox enableAPP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox hardwareId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker expreDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown counter_cnt;
        private System.Windows.Forms.NumericUpDown agent_cnt;
        private System.Windows.Forms.Label label8;
    }
}

