namespace FutSystems.GUI
{
    partial class fmConfirm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmConfirm));
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnNo = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnYes = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.mMessage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnNo);
            this.kryptonPanel1.Controls.Add(this.btnYes);
            this.kryptonPanel1.Controls.Add(this.mMessage);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(327, 127);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnNo
            // 
            this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNo.Location = new System.Drawing.Point(253, 92);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(61, 25);
            this.btnNo.TabIndex = 2;
            this.btnNo.Values.Text = "取 消";
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnYes
            // 
            this.btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnYes.Location = new System.Drawing.Point(186, 92);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(61, 25);
            this.btnYes.TabIndex = 1;
            this.btnYes.Values.Text = "确 定";
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // mMessage
            // 
            this.mMessage.Location = new System.Drawing.Point(12, 22);
            this.mMessage.Name = "mMessage";
            this.mMessage.Size = new System.Drawing.Size(19, 18);
            this.mMessage.TabIndex = 0;
            this.mMessage.Values.Text = "--";
            // 
            // fmConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 127);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fmConfirm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "确认操作";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel mMessage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnYes;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnNo;
    }
}