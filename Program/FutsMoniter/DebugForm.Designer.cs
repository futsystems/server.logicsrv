namespace FutsMoniter
{
    partial class DebugForm
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
            this.ctDebug1 = new FutSystems.GUI.ctDebug();
            this.SuspendLayout();
            // 
            // ctDebug1
            // 
            this.ctDebug1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDebug1.EnableSearching = true;
            this.ctDebug1.ExternalTimeStamp = 0;
            this.ctDebug1.Location = new System.Drawing.Point(0, 0);
            this.ctDebug1.Margin = new System.Windows.Forms.Padding(2);
            this.ctDebug1.Name = "ctDebug1";
            this.ctDebug1.Size = new System.Drawing.Size(999, 508);
            this.ctDebug1.TabIndex = 0;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 508);
            this.Controls.Add(this.ctDebug1);
            this.Name = "DebugForm";
            this.Text = "DebugForm";
            this.ResumeLayout(false);

        }

        #endregion

        private FutSystems.GUI.ctDebug ctDebug1;
    }
}