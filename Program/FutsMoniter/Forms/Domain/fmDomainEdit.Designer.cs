namespace FutsMoniter
{
    partial class fmDomainEdit
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
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonGroupBox2 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel16 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel15 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel14 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.routeritemlimit = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.routergrouplimit = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.acclimit = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel13 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel12 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel11 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.dateexpired = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel10 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.datecreated = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.email = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.qq = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.mobile = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.linkman = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.domainid = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).BeginInit();
            this.kryptonGroupBox2.Panel.SuspendLayout();
            this.kryptonGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox2);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(431, 485);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(329, 448);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(90, 25);
            this.btnSubmit.TabIndex = 14;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // kryptonGroupBox2
            // 
            this.kryptonGroupBox2.Location = new System.Drawing.Point(3, 225);
            this.kryptonGroupBox2.Name = "kryptonGroupBox2";
            // 
            // kryptonGroupBox2.Panel
            // 
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel16);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel15);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel14);
            this.kryptonGroupBox2.Panel.Controls.Add(this.routeritemlimit);
            this.kryptonGroupBox2.Panel.Controls.Add(this.routergrouplimit);
            this.kryptonGroupBox2.Panel.Controls.Add(this.acclimit);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel13);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel12);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel11);
            this.kryptonGroupBox2.Panel.Controls.Add(this.dateexpired);
            this.kryptonGroupBox2.Panel.Controls.Add(this.kryptonLabel10);
            this.kryptonGroupBox2.Size = new System.Drawing.Size(428, 213);
            this.kryptonGroupBox2.TabIndex = 13;
            this.kryptonGroupBox2.Text = "授权信息";
            this.kryptonGroupBox2.Values.Heading = "授权信息";
            // 
            // kryptonLabel16
            // 
            this.kryptonLabel16.Location = new System.Drawing.Point(238, 83);
            this.kryptonLabel16.Name = "kryptonLabel16";
            this.kryptonLabel16.Size = new System.Drawing.Size(160, 18);
            this.kryptonLabel16.TabIndex = 23;
            this.kryptonLabel16.Values.Text = "单路由组路由条目数上限";
            // 
            // kryptonLabel15
            // 
            this.kryptonLabel15.Location = new System.Drawing.Point(238, 59);
            this.kryptonLabel15.Name = "kryptonLabel15";
            this.kryptonLabel15.Size = new System.Drawing.Size(106, 18);
            this.kryptonLabel15.TabIndex = 22;
            this.kryptonLabel15.Values.Text = "路由组数目上限";
            // 
            // kryptonLabel14
            // 
            this.kryptonLabel14.Location = new System.Drawing.Point(238, 33);
            this.kryptonLabel14.Name = "kryptonLabel14";
            this.kryptonLabel14.Size = new System.Drawing.Size(92, 18);
            this.kryptonLabel14.TabIndex = 21;
            this.kryptonLabel14.Values.Text = "帐户数目上限";
            // 
            // routeritemlimit
            // 
            this.routeritemlimit.Location = new System.Drawing.Point(116, 83);
            this.routeritemlimit.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.routeritemlimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.routeritemlimit.Name = "routeritemlimit";
            this.routeritemlimit.Size = new System.Drawing.Size(116, 20);
            this.routeritemlimit.TabIndex = 20;
            this.routeritemlimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // routergrouplimit
            // 
            this.routergrouplimit.Location = new System.Drawing.Point(116, 57);
            this.routergrouplimit.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.routergrouplimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.routergrouplimit.Name = "routergrouplimit";
            this.routergrouplimit.Size = new System.Drawing.Size(116, 20);
            this.routergrouplimit.TabIndex = 19;
            this.routergrouplimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // acclimit
            // 
            this.acclimit.Location = new System.Drawing.Point(116, 31);
            this.acclimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.acclimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.acclimit.Name = "acclimit";
            this.acclimit.Size = new System.Drawing.Size(116, 20);
            this.acclimit.TabIndex = 18;
            this.acclimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // kryptonLabel13
            // 
            this.kryptonLabel13.Location = new System.Drawing.Point(26, 85);
            this.kryptonLabel13.Name = "kryptonLabel13";
            this.kryptonLabel13.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel13.TabIndex = 17;
            this.kryptonLabel13.Values.Text = "路由项目数:";
            // 
            // kryptonLabel12
            // 
            this.kryptonLabel12.Location = new System.Drawing.Point(38, 59);
            this.kryptonLabel12.Name = "kryptonLabel12";
            this.kryptonLabel12.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel12.TabIndex = 16;
            this.kryptonLabel12.Values.Text = "路由组数:";
            // 
            // kryptonLabel11
            // 
            this.kryptonLabel11.Location = new System.Drawing.Point(51, 33);
            this.kryptonLabel11.Name = "kryptonLabel11";
            this.kryptonLabel11.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel11.TabIndex = 15;
            this.kryptonLabel11.Values.Text = "帐户数:";
            // 
            // dateexpired
            // 
            this.dateexpired.Location = new System.Drawing.Point(116, 4);
            this.dateexpired.Name = "dateexpired";
            this.dateexpired.Size = new System.Drawing.Size(116, 20);
            this.dateexpired.TabIndex = 14;
            // 
            // kryptonLabel10
            // 
            this.kryptonLabel10.Location = new System.Drawing.Point(38, 6);
            this.kryptonLabel10.Name = "kryptonLabel10";
            this.kryptonLabel10.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel10.TabIndex = 13;
            this.kryptonLabel10.Values.Text = "过期日期:";
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel8);
            this.kryptonGroupBox1.Panel.Controls.Add(this.datecreated);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroupBox1.Panel.Controls.Add(this.email);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel2);
            this.kryptonGroupBox1.Panel.Controls.Add(this.qq);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel3);
            this.kryptonGroupBox1.Panel.Controls.Add(this.mobile);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel4);
            this.kryptonGroupBox1.Panel.Controls.Add(this.linkman);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel5);
            this.kryptonGroupBox1.Panel.Controls.Add(this.name);
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel6);
            this.kryptonGroupBox1.Panel.Controls.Add(this.domainid);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(431, 219);
            this.kryptonGroupBox1.TabIndex = 12;
            this.kryptonGroupBox1.Text = "基本信息";
            this.kryptonGroupBox1.Values.Heading = "基本信息";
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(43, 27);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel8.TabIndex = 12;
            this.kryptonLabel8.Values.Text = "创建日期:";
            // 
            // datecreated
            // 
            this.datecreated.Location = new System.Drawing.Point(119, 26);
            this.datecreated.Name = "datecreated";
            this.datecreated.Size = new System.Drawing.Size(19, 18);
            this.datecreated.TabIndex = 13;
            this.datecreated.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(44, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(69, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "DomainID:";
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(120, 159);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(158, 21);
            this.email.TabIndex = 11;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(70, 54);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "名称:";
            // 
            // qq
            // 
            this.qq.Location = new System.Drawing.Point(120, 132);
            this.qq.Name = "qq";
            this.qq.Size = new System.Drawing.Size(115, 21);
            this.qq.TabIndex = 10;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(56, 81);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "联系人:";
            // 
            // mobile
            // 
            this.mobile.Location = new System.Drawing.Point(120, 105);
            this.mobile.Name = "mobile";
            this.mobile.Size = new System.Drawing.Size(115, 21);
            this.mobile.TabIndex = 9;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(68, 108);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 3;
            this.kryptonLabel4.Values.Text = "手机:";
            // 
            // linkman
            // 
            this.linkman.Location = new System.Drawing.Point(120, 78);
            this.linkman.Name = "linkman";
            this.linkman.Size = new System.Drawing.Size(115, 21);
            this.linkman.TabIndex = 8;
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(76, 135);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(33, 18);
            this.kryptonLabel5.TabIndex = 4;
            this.kryptonLabel5.Values.Text = "QQ:";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(120, 50);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(202, 21);
            this.name.TabIndex = 7;
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(41, 162);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel6.TabIndex = 5;
            this.kryptonLabel6.Values.Text = "电子邮件:";
            // 
            // domainid
            // 
            this.domainid.Location = new System.Drawing.Point(120, 2);
            this.domainid.Name = "domainid";
            this.domainid.Size = new System.Drawing.Size(19, 18);
            this.domainid.TabIndex = 6;
            this.domainid.Values.Text = "--";
            // 
            // fmDomainEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 485);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmDomainEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fmDomainEdit";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonGroupBox2.Panel.ResumeLayout(false);
            this.kryptonGroupBox2.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox2)).EndInit();
            this.kryptonGroupBox2.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox email;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox qq;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox mobile;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox linkman;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel domainid;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel datecreated;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker dateexpired;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel10;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel13;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel12;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel11;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown routeritemlimit;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown routergrouplimit;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown acclimit;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel14;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel16;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel15;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
    }
}