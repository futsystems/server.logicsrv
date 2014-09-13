namespace FutsMoniter
{
    partial class RouterMoniterForm
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
            this.connectorgird = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.connectorgird)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // connectorgird
            // 
            this.connectorgird.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectorgird.Location = new System.Drawing.Point(0, 0);
            this.connectorgird.Name = "connectorgird";
            this.connectorgird.Size = new System.Drawing.Size(537, 151);
            this.connectorgird.TabIndex = 0;
            this.connectorgird.Text = "radGridView1";
            this.connectorgird.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.connectorgird_ContextMenuOpening);
            // 
            // RouterMoniterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 151);
            this.Controls.Add(this.connectorgird);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RouterMoniterForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统路由列表";
            this.ThemeName = "ControlDefault";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RouterMoniterForm_FormClosing);
            this.Load += new System.EventHandler(this.RouterMoniterForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.connectorgird)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView connectorgird;
    }
}
