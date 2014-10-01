namespace FutsMoniter
{
    partial class UpdatePassForm
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.oldpass = new Telerik.WinControls.UI.RadTextBox();
            this.newpass = new Telerik.WinControls.UI.RadTextBox();
            this.newpass2 = new Telerik.WinControls.UI.RadTextBox();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldpass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newpass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newpass2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(22, 15);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(47, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "旧密码:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(22, 39);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(47, 16);
            this.radLabel2.TabIndex = 1;
            this.radLabel2.Text = "新密码:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(10, 66);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 2;
            this.radLabel3.Text = "确认密码:";
            // 
            // oldpass
            // 
            this.oldpass.Location = new System.Drawing.Point(76, 13);
            this.oldpass.Name = "oldpass";
            this.oldpass.Size = new System.Drawing.Size(100, 18);
            this.oldpass.TabIndex = 3;
            // 
            // newpass
            // 
            this.newpass.Location = new System.Drawing.Point(75, 37);
            this.newpass.Name = "newpass";
            this.newpass.Size = new System.Drawing.Size(100, 18);
            this.newpass.TabIndex = 4;
            // 
            // newpass2
            // 
            this.newpass2.Location = new System.Drawing.Point(75, 64);
            this.newpass2.Name = "newpass2";
            this.newpass2.Size = new System.Drawing.Size(100, 18);
            this.newpass2.TabIndex = 5;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(119, 109);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 24);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // UpdatePassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 145);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.newpass2);
            this.Controls.Add(this.newpass);
            this.Controls.Add(this.oldpass);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePassForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改柜员密码";
            this.ThemeName = "ControlDefault";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldpass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newpass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newpass2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadTextBox oldpass;
        private Telerik.WinControls.UI.RadTextBox newpass;
        private Telerik.WinControls.UI.RadTextBox newpass2;
        private Telerik.WinControls.UI.RadButton btnSubmit;
    }
}
