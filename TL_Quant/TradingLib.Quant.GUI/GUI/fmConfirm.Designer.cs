namespace TradingLib.Quant.GUI
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.msg = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.confirm = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.cancle = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.cancle);
            this.kryptonPanel1.Controls.Add(this.confirm);
            this.kryptonPanel1.Controls.Add(this.msg);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(249, 112);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // msg
            // 
            this.msg.Location = new System.Drawing.Point(12, 12);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(19, 18);
            this.msg.TabIndex = 1;
            this.msg.Values.Text = "--";
            // 
            // confirm
            // 
            this.confirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.confirm.Location = new System.Drawing.Point(200, 87);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(46, 22);
            this.confirm.TabIndex = 1;
            this.confirm.Values.Text = "确 认";
            this.confirm.Click += new System.EventHandler(this.confirm_Click);
            // 
            // cancle
            // 
            this.cancle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancle.Location = new System.Drawing.Point(148, 87);
            this.cancle.Name = "cancle";
            this.cancle.Size = new System.Drawing.Size(46, 22);
            this.cancle.TabIndex = 2;
            this.cancle.Values.Text = "取 消";
            this.cancle.Click += new System.EventHandler(this.cancle_Click);
            // 
            // fmConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 112);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmConfirm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "操作确认";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel msg;
        private ComponentFactory.Krypton.Toolkit.KryptonButton cancle;
        private ComponentFactory.Krypton.Toolkit.KryptonButton confirm;
    }
}