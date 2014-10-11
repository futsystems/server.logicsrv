namespace FutsMoniter
{
    partial class LoginForm
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
            this.label1 = new Telerik.WinControls.UI.RadLabel();
            this.label2 = new Telerik.WinControls.UI.RadLabel();
            this.btnLogin = new Telerik.WinControls.UI.RadButton();
            this.message = new Telerik.WinControls.UI.RadLabel();
            this.btnExit = new System.Windows.Forms.LinkLabel();
            this.password = new System.Windows.Forms.MaskedTextBox();
            this.username = new Telerik.WinControls.UI.RadTextBox();
            this.label0 = new Telerik.WinControls.UI.RadLabel();
            this.servers = new Telerik.WinControls.UI.RadDropDownList();
            this.office2010SilverTheme1 = new Telerik.WinControls.Themes.Office2010SilverTheme();
            this.office2010BlackTheme1 = new Telerik.WinControls.Themes.Office2010BlackTheme();
            this.imageheader = new System.Windows.Forms.PictureBox();
            this.ckremberuser = new Telerik.WinControls.UI.RadCheckBox();
            this.ckremberpass = new Telerik.WinControls.UI.RadCheckBox();
            //this.windows8Theme1 = new Telerik.WinControls.Themes.Windows8Theme();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.message)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.username)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.servers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageheader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckremberuser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckremberpass)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(38, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(38, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "密  码:";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(279, 84);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(76, 31);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "登 入";
            this.btnLogin.ThemeName = "ControlDefault";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // message
            // 
            this.message.ForeColor = System.Drawing.Color.Silver;
            this.message.Location = new System.Drawing.Point(32, 188);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(14, 16);
            this.message.TabIndex = 6;
            this.message.Text = "--";
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.btnExit.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnExit.Location = new System.Drawing.Point(312, 185);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(49, 19);
            this.btnExit.TabIndex = 7;
            this.btnExit.TabStop = true;
            this.btnExit.Text = "退   出";
            this.btnExit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnExit_LinkClicked);
            // 
            // password
            // 
            this.password.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.password.Location = new System.Drawing.Point(91, 147);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(145, 26);
            this.password.TabIndex = 9;
            this.password.Text = "123456";
            // 
            // username
            // 
            this.username.AutoSize = false;
            this.username.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username.Location = new System.Drawing.Point(91, 116);
            this.username.Multiline = true;
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(145, 26);
            this.username.TabIndex = 8;
            this.username.Text = "888888";
            this.username.ThemeName = "ControlDefault";
            // 
            // label0
            // 
            this.label0.Location = new System.Drawing.Point(26, 91);
            this.label0.Name = "label0";
            this.label0.Size = new System.Drawing.Size(59, 16);
            this.label0.TabIndex = 10;
            this.label0.Text = "柜台地址:";
            // 
            // servers
            // 
            this.servers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.servers.Location = new System.Drawing.Point(91, 84);
            this.servers.Name = "servers";
            this.servers.Size = new System.Drawing.Size(145, 26);
            this.servers.TabIndex = 17;
            this.servers.Text = "--";
            this.servers.ThemeName = "ControlDefault";
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.servers.GetChildAt(0).GetChildAt(0))).Width = 5F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.servers.GetChildAt(0).GetChildAt(0))).LeftWidth = 5F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.servers.GetChildAt(0).GetChildAt(0))).TopWidth = 5F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.servers.GetChildAt(0).GetChildAt(0))).RightWidth = 5F;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.servers.GetChildAt(0).GetChildAt(0))).BottomWidth = 5F;
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.servers.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // imageheader
            // 
            this.imageheader.BackColor = System.Drawing.Color.White;
            this.imageheader.Dock = System.Windows.Forms.DockStyle.Top;
            this.imageheader.ErrorImage = null;
            this.imageheader.Image = global::FutsMoniter.Properties.Resources.header;
            this.imageheader.InitialImage = null;
            this.imageheader.Location = new System.Drawing.Point(0, 0);
            this.imageheader.Name = "imageheader";
            this.imageheader.Size = new System.Drawing.Size(385, 66);
            this.imageheader.TabIndex = 0;
            this.imageheader.TabStop = false;
            this.imageheader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageheader_MouseDown);
            this.imageheader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageheader_MouseMove);
            this.imageheader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageheader_MouseUp);
            // 
            // ckremberuser
            // 
            this.ckremberuser.Location = new System.Drawing.Point(279, 128);
            this.ckremberuser.Name = "ckremberuser";
            this.ckremberuser.Size = new System.Drawing.Size(82, 16);
            this.ckremberuser.TabIndex = 18;
            this.ckremberuser.Text = "记住用户名";
            // 
            // ckremberpass
            // 
            this.ckremberpass.Location = new System.Drawing.Point(279, 150);
            this.ckremberpass.Name = "ckremberpass";
            this.ckremberpass.Size = new System.Drawing.Size(70, 16);
            this.ckremberpass.TabIndex = 19;
            this.ckremberpass.Text = "保存密码";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(90)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(385, 225);
            this.Controls.Add(this.ckremberpass);
            this.Controls.Add(this.ckremberuser);
            this.Controls.Add(this.servers);
            this.Controls.Add(this.label0);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.message);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imageheader);
            this.DoubleBuffered = true;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.message)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.username)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.servers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageheader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckremberuser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckremberpass)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imageheader;
        private Telerik.WinControls.UI.RadLabel label1;
        private Telerik.WinControls.UI.RadLabel label2;
        private Telerik.WinControls.UI.RadButton btnLogin;
        private Telerik.WinControls.UI.RadLabel message;
        private System.Windows.Forms.LinkLabel btnExit;
        private System.Windows.Forms.MaskedTextBox password;
        private Telerik.WinControls.UI.RadTextBox username;
        private Telerik.WinControls.UI.RadLabel label0;
        private Telerik.WinControls.UI.RadDropDownList servers;
        private Telerik.WinControls.Themes.Office2010SilverTheme office2010SilverTheme1;
        private Telerik.WinControls.Themes.Office2010BlackTheme office2010BlackTheme1;
        private Telerik.WinControls.UI.RadCheckBox ckremberuser;
        private Telerik.WinControls.UI.RadCheckBox ckremberpass;
        //private Telerik.WinControls.Themes.Windows8Theme windows8Theme1;
    }
}