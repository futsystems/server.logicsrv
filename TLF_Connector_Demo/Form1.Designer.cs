namespace QuantBox.Demo
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
            this.debugControl1 = new QuantBox.Demo.DebugControl();
            this.address = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.brokerid = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pass = new System.Windows.Forms.TextBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
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
            this.debugControl1.Size = new System.Drawing.Size(518, 443);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = false;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // address
            // 
            this.address.Location = new System.Drawing.Point(576, 4);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(202, 21);
            this.address.TabIndex = 1;
            this.address.Text = "tcp://222.66.235.70:61205";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(523, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "CTP地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(523, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "brokerID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(523, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Account";
            // 
            // brokerid
            // 
            this.brokerid.Location = new System.Drawing.Point(576, 31);
            this.brokerid.Name = "brokerid";
            this.brokerid.Size = new System.Drawing.Size(61, 21);
            this.brokerid.TabIndex = 5;
            this.brokerid.Text = "1026";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(576, 58);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(77, 21);
            this.username.TabIndex = 6;
            this.username.Text = "000044";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(670, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Pass";
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(705, 58);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(77, 21);
            this.pass.TabIndex = 8;
            this.pass.Text = "123456";
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(538, 95);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 9;
            this.btnInit.Text = "初始化";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(624, 95);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 10;
            this.btnLogin.Text = "登入";
            this.btnLogin.UseVisualStyleBackColor = true;
            //this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 443);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.pass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.username);
            this.Controls.Add(this.brokerid);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.address);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DebugControl debugControl1;
        private System.Windows.Forms.TextBox address;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox brokerid;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnLogin;
    }
}

