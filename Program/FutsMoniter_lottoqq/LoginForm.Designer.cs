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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.username = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.password = new ComponentFactory.Krypton.Toolkit.KryptonMaskedTextBox();
            this.ckremberuser = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.ckremberpass = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.message = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonManager1 = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.contractus = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnLogin = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(130, 9);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(185, 21);
            this.username.TabIndex = 23;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(130, 42);
            this.password.Name = "password";
            this.password.PasswordChar = '#';
            this.password.Size = new System.Drawing.Size(185, 21);
            this.password.TabIndex = 24;
            // 
            // ckremberuser
            // 
            this.ckremberuser.Location = new System.Drawing.Point(130, 69);
            this.ckremberuser.Name = "ckremberuser";
            this.ckremberuser.Size = new System.Drawing.Size(91, 18);
            this.ckremberuser.TabIndex = 28;
            this.ckremberuser.Values.Text = "记住用户名";
            // 
            // ckremberpass
            // 
            this.ckremberpass.Location = new System.Drawing.Point(250, 69);
            this.ckremberpass.Name = "ckremberpass";
            this.ckremberpass.Size = new System.Drawing.Size(78, 18);
            this.ckremberpass.TabIndex = 29;
            this.ckremberpass.Values.Text = "保存密码";
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(33, 138);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(21, 20);
            this.message.StateCommon.ShortText.Color1 = System.Drawing.Color.DarkGray;
            this.message.StateCommon.ShortText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.message.TabIndex = 30;
            this.message.Values.Text = "--";
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
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnLogin);
            this.kryptonPanel1.Controls.Add(this.contractus);
            this.kryptonPanel1.Controls.Add(this.pictureBox2);
            this.kryptonPanel1.Controls.Add(this.username);
            this.kryptonPanel1.Controls.Add(this.password);
            this.kryptonPanel1.Controls.Add(this.ckremberuser);
            this.kryptonPanel1.Controls.Add(this.ckremberpass);
            this.kryptonPanel1.Controls.Add(this.message);
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 179);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(431, 164);
            this.kryptonPanel1.TabIndex = 38;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.Location = new System.Drawing.Point(397, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(34, 33);
            this.btnClose.StateCommon.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.btnClose.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.btnClose.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnClose.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this.btnClose.StatePressed.Content.Padding = new System.Windows.Forms.Padding(0);
            this.btnClose.TabIndex = 41;
            this.btnClose.Values.ImageStates.ImageCheckedNormal = null;
            this.btnClose.Values.ImageStates.ImageCheckedPressed = null;
            this.btnClose.Values.ImageStates.ImageCheckedTracking = null;
            this.btnClose.Values.ImageStates.ImageDisabled = ((System.Drawing.Image)(resources.GetObject("btnClose.Values.ImageStates.ImageDisabled")));
            this.btnClose.Values.ImageStates.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnClose.Values.ImageStates.ImageNormal")));
            this.btnClose.Values.ImageStates.ImagePressed = ((System.Drawing.Image)(resources.GetObject("btnClose.Values.ImageStates.ImagePressed")));
            this.btnClose.Values.ImageStates.ImageTracking = ((System.Drawing.Image)(resources.GetObject("btnClose.Values.ImageStates.ImageTracking")));
            this.btnClose.Values.Text = "";
            // 
            // contractus
            // 
            this.contractus.Location = new System.Drawing.Point(336, 45);
            this.contractus.Name = "contractus";
            this.contractus.Size = new System.Drawing.Size(65, 18);
            this.contractus.TabIndex = 40;
            this.contractus.Values.Text = "联系我们";
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Controls.Add(this.btnClose);
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(431, 182);
            this.kryptonPanel2.StateCommon.Image = ((System.Drawing.Image)(resources.GetObject("kryptonPanel2.StateCommon.Image")));
            this.kryptonPanel2.TabIndex = 39;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(33, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(80, 80);
            this.pictureBox2.TabIndex = 37;
            this.pictureBox2.TabStop = false;
            // 
            // btnLogin
            // 
            this.btnLogin.AutoSize = true;
            this.btnLogin.Location = new System.Drawing.Point(130, 102);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(0);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(198, 34);
            this.btnLogin.StateCommon.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.btnLogin.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.btnLogin.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.btnLogin.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this.btnLogin.StatePressed.Content.Padding = new System.Windows.Forms.Padding(0);
            this.btnLogin.TabIndex = 42;
            this.btnLogin.Values.ImageStates.ImageCheckedNormal = null;
            this.btnLogin.Values.ImageStates.ImageCheckedPressed = null;
            this.btnLogin.Values.ImageStates.ImageCheckedTracking = null;
            this.btnLogin.Values.ImageStates.ImageDisabled = ((System.Drawing.Image)(resources.GetObject("kryptonButton1.Values.ImageStates.ImageDisabled")));
            this.btnLogin.Values.ImageStates.ImageNormal = ((System.Drawing.Image)(resources.GetObject("kryptonButton1.Values.ImageStates.ImageNormal")));
            this.btnLogin.Values.ImageStates.ImagePressed = ((System.Drawing.Image)(resources.GetObject("kryptonButton1.Values.ImageStates.ImagePressed")));
            this.btnLogin.Values.ImageStates.ImageTracking = ((System.Drawing.Image)(resources.GetObject("kryptonButton1.Values.ImageStates.ImageTracking")));
            this.btnLogin.Values.Text = "";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 343);
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.kryptonPanel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登入";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            this.kryptonPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //private Telerik.WinControls.Themes.Office2010SilverTheme office2010SilverTheme1;
        //private Telerik.WinControls.Themes.Office2010BlackTheme office2010BlackTheme1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox username;
        private ComponentFactory.Krypton.Toolkit.KryptonMaskedTextBox password;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox ckremberuser;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox ckremberpass;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel message;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel contractus;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnClose;
        private System.Windows.Forms.PictureBox pictureBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnLogin;
        //private Telerik.WinControls.Themes.Windows8Theme windows8Theme1;
    }
}