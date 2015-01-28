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
            this.components = new System.ComponentModel.Container();
            this.imageheader = new System.Windows.Forms.PictureBox();
            this.label0 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.label1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.label2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.username = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.password = new ComponentFactory.Krypton.Toolkit.KryptonMaskedTextBox();
            this.servers = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.btnLogin = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ckremberuser = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.ckremberpass = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.message = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnExit = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonManager1 = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imageheader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.servers)).BeginInit();
            this.SuspendLayout();
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
            // label0
            // 
            this.label0.Location = new System.Drawing.Point(22, 97);
            this.label0.Name = "label0";
            this.label0.Size = new System.Drawing.Size(68, 18);
            this.label0.TabIndex = 20;
            this.label0.Values.Text = "柜台地址:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(35, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 18);
            this.label1.TabIndex = 21;
            this.label1.Values.Text = "用户名:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(49, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 18);
            this.label2.TabIndex = 22;
            this.label2.Values.Text = "密码:";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(97, 122);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(121, 22);
            this.username.TabIndex = 23;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(97, 155);
            this.password.Name = "password";
            this.password.PasswordChar = '#';
            this.password.Size = new System.Drawing.Size(121, 22);
            this.password.TabIndex = 24;
            // 
            // servers
            // 
            this.servers.DropDownWidth = 121;
            this.servers.Location = new System.Drawing.Point(97, 90);
            this.servers.Name = "servers";
            this.servers.Size = new System.Drawing.Size(121, 21);
            this.servers.TabIndex = 25;
            this.servers.Text = "--";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(279, 84);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(90, 31);
            this.btnLogin.TabIndex = 27;
            this.btnLogin.Values.Text = "登 入";
            // 
            // ckremberuser
            // 
            this.ckremberuser.Location = new System.Drawing.Point(278, 126);
            this.ckremberuser.Name = "ckremberuser";
            this.ckremberuser.Size = new System.Drawing.Size(91, 18);
            this.ckremberuser.TabIndex = 28;
            this.ckremberuser.Values.Text = "记住用户名";
            // 
            // ckremberpass
            // 
            this.ckremberpass.Location = new System.Drawing.Point(278, 150);
            this.ckremberpass.Name = "ckremberpass";
            this.ckremberpass.Size = new System.Drawing.Size(78, 18);
            this.ckremberpass.TabIndex = 29;
            this.ckremberpass.Values.Text = "保存密码";
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(22, 186);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(21, 20);
            this.message.StateCommon.ShortText.Color1 = System.Drawing.Color.DarkGray;
            this.message.StateCommon.ShortText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.message.TabIndex = 30;
            this.message.Values.Text = "--";
            // 
            // btnExit
            // 
            this.btnExit.LinkBehavior = ComponentFactory.Krypton.Toolkit.KryptonLinkBehavior.NeverUnderline;
            this.btnExit.Location = new System.Drawing.Point(315, 186);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(41, 18);
            this.btnExit.StateCommon.LongText.Color1 = System.Drawing.Color.Red;
            this.btnExit.StateCommon.LongText.Color2 = System.Drawing.Color.Red;
            this.btnExit.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.btnExit.StateCommon.ShortText.Color2 = System.Drawing.Color.Red;
            this.btnExit.StateNormal.ShortText.Color1 = System.Drawing.Color.Red;
            this.btnExit.StateNormal.ShortText.Color2 = System.Drawing.Color.Red;
            this.btnExit.TabIndex = 31;
            this.btnExit.Values.Text = "退 出";
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Silver;
            // 
            // kryptonManager1
            // 
            this.kryptonManager1.GlobalPalette = this.kryptonPalette1;
            this.kryptonManager1.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.Custom;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 217);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.message);
            this.Controls.Add(this.ckremberpass);
            this.Controls.Add(this.ckremberuser);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.servers);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label0);
            this.Controls.Add(this.imageheader);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登入";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.imageheader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.servers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imageheader;
        //private Telerik.WinControls.Themes.Office2010SilverTheme office2010SilverTheme1;
        //private Telerik.WinControls.Themes.Office2010BlackTheme office2010BlackTheme1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label0;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox username;
        private ComponentFactory.Krypton.Toolkit.KryptonMaskedTextBox password;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox servers;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnLogin;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox ckremberuser;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox ckremberpass;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel message;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel btnExit;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager1;
        //private Telerik.WinControls.Themes.Windows8Theme windows8Theme1;
    }
}