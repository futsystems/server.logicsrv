namespace DataFeed.IB
{
    partial class Form1
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
            this.debugControl1 = new TradeLink.AppKit.DebugControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // debugControl1
            // 
            this.debugControl1.EnableSearching = true;
            this.debugControl1.ExternalTimeStamp = 0;
            this.debugControl1.Location = new System.Drawing.Point(0, 1);
            this.debugControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(459, 201);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            this.debugControl1.UseExternalTimeStamp = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(474, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 213);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.debugControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private TradeLink.AppKit.DebugControl debugControl1;
        private System.Windows.Forms.Button button1;
    }
}