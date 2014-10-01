namespace TradingLib.Quant.GUI
{
    partial class fmServiceSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.serviceip = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.serviceport = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.user = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.pass = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.othersetting = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ok = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.servicename = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.desp = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.desp);
            this.kryptonPanel1.Controls.Add(this.servicename);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.ok);
            this.kryptonPanel1.Controls.Add(this.othersetting);
            this.kryptonPanel1.Controls.Add(this.pass);
            this.kryptonPanel1.Controls.Add(this.user);
            this.kryptonPanel1.Controls.Add(this.serviceport);
            this.kryptonPanel1.Controls.Add(this.serviceip);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(247, 273);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(11, 80);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "服务地址:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(11, 108);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "端口:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(11, 136);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "用户名:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(11, 160);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "密码:";
            // 
            // serviceip
            // 
            this.serviceip.Location = new System.Drawing.Point(85, 76);
            this.serviceip.Name = "serviceip";
            this.serviceip.Size = new System.Drawing.Size(146, 21);
            this.serviceip.TabIndex = 1;
            // 
            // serviceport
            // 
            this.serviceport.Location = new System.Drawing.Point(85, 104);
            this.serviceport.Name = "serviceport";
            this.serviceport.Size = new System.Drawing.Size(71, 21);
            this.serviceport.TabIndex = 5;
            // 
            // user
            // 
            this.user.Location = new System.Drawing.Point(85, 132);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(146, 21);
            this.user.TabIndex = 6;
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(85, 160);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(146, 21);
            this.pass.TabIndex = 7;
            // 
            // othersetting
            // 
            this.othersetting.Location = new System.Drawing.Point(11, 199);
            this.othersetting.Name = "othersetting";
            this.othersetting.Size = new System.Drawing.Size(79, 22);
            this.othersetting.TabIndex = 1;
            this.othersetting.Values.Text = "其他设定";
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(152, 239);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(79, 22);
            this.ok.TabIndex = 8;
            this.ok.Values.Text = "确 定";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(11, 3);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 9;
            this.kryptonLabel5.Values.Text = "服务名称:";
            // 
            // servicename
            // 
            this.servicename.Location = new System.Drawing.Point(85, 3);
            this.servicename.Name = "servicename";
            this.servicename.Size = new System.Drawing.Size(19, 18);
            this.servicename.TabIndex = 10;
            this.servicename.Values.Text = "--";
            // 
            // desp
            // 
            this.desp.Enabled = false;
            this.desp.Location = new System.Drawing.Point(11, 27);
            this.desp.Name = "desp";
            this.desp.Size = new System.Drawing.Size(220, 43);
            this.desp.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.desp.TabIndex = 11;
            this.desp.Text = "";
            // 
            // fmServiceSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 273);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "fmServiceSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "服务设定";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox pass;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox user;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox serviceport;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox serviceip;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ok;
        private ComponentFactory.Krypton.Toolkit.KryptonButton othersetting;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel servicename;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox desp;
    }
}