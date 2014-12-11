namespace FutsMoniter
{
    partial class fmPopMessage
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
            this.picbox = new System.Windows.Forms.PictureBox();
            this.title = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.message = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.picbox);
            this.kryptonPanel1.Controls.Add(this.title);
            this.kryptonPanel1.Controls.Add(this.message);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(244, 87);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // picbox
            // 
            this.picbox.BackColor = System.Drawing.Color.Transparent;
            this.picbox.Location = new System.Drawing.Point(31, 12);
            this.picbox.Name = "picbox";
            this.picbox.Size = new System.Drawing.Size(24, 24);
            this.picbox.TabIndex = 3;
            this.picbox.TabStop = false;
            // 
            // title
            // 
            this.title.Location = new System.Drawing.Point(65, 14);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(43, 16);
            this.title.StateCommon.ShortText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.title.TabIndex = 1;
            this.title.Values.Text = "title";
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(22, 41);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(61, 18);
            this.message.TabIndex = 2;
            this.message.Values.Text = "message";
            // 
            // fmPopMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 87);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fmPopMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "fmPopMessage";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel title;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel message;
        private System.Windows.Forms.PictureBox picbox;
    }
}