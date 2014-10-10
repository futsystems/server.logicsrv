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
            this.btnYes = new Telerik.WinControls.UI.RadButton();
            this.btnNo = new Telerik.WinControls.UI.RadButton();
            this.mMessage = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.btnYes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(234, 97);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(60, 24);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "确定";
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(300, 97);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(60, 24);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "取消";
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // mMessage
            // 
            this.mMessage.Location = new System.Drawing.Point(12, 24);
            this.mMessage.Name = "mMessage";
            this.mMessage.Size = new System.Drawing.Size(14, 16);
            this.mMessage.TabIndex = 2;
            this.mMessage.Text = "--";
            // 
            // fmConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 124);
            this.Controls.Add(this.mMessage);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "fmConfirm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "确认操作";
            this.ThemeName = "ControlDefault";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.btnYes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnYes;
        private Telerik.WinControls.UI.RadButton btnNo;
        private Telerik.WinControls.UI.RadLabel mMessage;
    }
}
