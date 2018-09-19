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
            this.btnGenSrv = new System.Windows.Forms.Button();
            this.hardwareId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.expreDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.counter_cnt = new System.Windows.Forms.NumericUpDown();
            this.agent_cnt = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnGenTerminal = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.updateServer = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.brokerServer = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.deploy2 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.account_cnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.counter_cnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent_cnt)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "部署编号:";
            // 
            // deploy
            // 
            this.deploy.Location = new System.Drawing.Point(229, 20);
            this.deploy.Name = "deploy";
            this.deploy.Size = new System.Drawing.Size(221, 35);
            this.deploy.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "交易账户数量:";
            // 
            // account_cnt
            // 
            this.account_cnt.Location = new System.Drawing.Point(229, 76);
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
            this.label4.Location = new System.Drawing.Point(28, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "API接口可用:";
            // 
            // enableAPI
            // 
            this.enableAPI.AutoSize = true;
            this.enableAPI.Location = new System.Drawing.Point(229, 220);
            this.enableAPI.Name = "enableAPI";
            this.enableAPI.Size = new System.Drawing.Size(28, 27);
            this.enableAPI.TabIndex = 7;
            this.enableAPI.UseVisualStyleBackColor = true;
            // 
            // enableAPP
            // 
            this.enableAPP.AutoSize = true;
            this.enableAPP.Location = new System.Drawing.Point(229, 268);
            this.enableAPP.Name = "enableAPP";
            this.enableAPP.Size = new System.Drawing.Size(28, 27);
            this.enableAPP.TabIndex = 9;
            this.enableAPP.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 269);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "移动端可用:";
            // 
            // btnGenSrv
            // 
            this.btnGenSrv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenSrv.Location = new System.Drawing.Point(969, 276);
            this.btnGenSrv.Name = "btnGenSrv";
            this.btnGenSrv.Size = new System.Drawing.Size(192, 51);
            this.btnGenSrv.TabIndex = 10;
            this.btnGenSrv.Text = "生成License";
            this.btnGenSrv.UseVisualStyleBackColor = true;
            // 
            // hardwareId
            // 
            this.hardwareId.Location = new System.Drawing.Point(648, 18);
            this.hardwareId.Name = "hardwareId";
            this.hardwareId.Size = new System.Drawing.Size(340, 35);
            this.hardwareId.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(483, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 24);
            this.label6.TabIndex = 11;
            this.label6.Text = "硬件编号:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(483, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 24);
            this.label7.TabIndex = 13;
            this.label7.Text = "到期日期:";
            // 
            // expreDate
            // 
            this.expreDate.Location = new System.Drawing.Point(648, 76);
            this.expreDate.Name = "expreDate";
            this.expreDate.Size = new System.Drawing.Size(245, 35);
            this.expreDate.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "分区数量:";
            // 
            // counter_cnt
            // 
            this.counter_cnt.Location = new System.Drawing.Point(229, 164);
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
            this.agent_cnt.Location = new System.Drawing.Point(229, 119);
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
            this.label8.Location = new System.Drawing.Point(64, 121);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 24);
            this.label8.TabIndex = 15;
            this.label8.Text = "柜员数量:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1197, 390);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnGenSrv);
            this.tabPage1.Controls.Add(this.expreDate);
            this.tabPage1.Controls.Add(this.agent_cnt);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.hardwareId);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.deploy);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.account_cnt);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.counter_cnt);
            this.tabPage1.Controls.Add(this.enableAPP);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.enableAPI);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1181, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "服务端";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnGenTerminal);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.updateServer);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.brokerServer);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.deploy2);
            this.tabPage2.Location = new System.Drawing.Point(8, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1181, 343);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "交易端/管理端";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnGenTerminal
            // 
            this.btnGenTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenTerminal.Location = new System.Drawing.Point(972, 275);
            this.btnGenTerminal.Name = "btnGenTerminal";
            this.btnGenTerminal.Size = new System.Drawing.Size(192, 51);
            this.btnGenTerminal.TabIndex = 11;
            this.btnGenTerminal.Text = "生成License";
            this.btnGenTerminal.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 124);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(142, 24);
            this.label11.TabIndex = 6;
            this.label11.Text = "更新服务器:";
            // 
            // updateServer
            // 
            this.updateServer.Location = new System.Drawing.Point(177, 121);
            this.updateServer.Name = "updateServer";
            this.updateServer.Size = new System.Drawing.Size(221, 35);
            this.updateServer.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(142, 24);
            this.label10.TabIndex = 4;
            this.label10.Text = "交易服务器:";
            // 
            // brokerServer
            // 
            this.brokerServer.Location = new System.Drawing.Point(177, 65);
            this.brokerServer.Name = "brokerServer";
            this.brokerServer.Size = new System.Drawing.Size(221, 35);
            this.brokerServer.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 24);
            this.label9.TabIndex = 2;
            this.label9.Text = "部署编号:";
            // 
            // deploy2
            // 
            this.deploy2.Location = new System.Drawing.Point(177, 15);
            this.deploy2.Name = "deploy2";
            this.deploy2.Size = new System.Drawing.Size(221, 35);
            this.deploy2.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 390);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "License管理端";
            ((System.ComponentModel.ISupportInitialize)(this.account_cnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.counter_cnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agent_cnt)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button btnGenSrv;
        private System.Windows.Forms.TextBox hardwareId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker expreDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown counter_cnt;
        private System.Windows.Forms.NumericUpDown agent_cnt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnGenTerminal;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox updateServer;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox brokerServer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox deploy2;
    }
}

