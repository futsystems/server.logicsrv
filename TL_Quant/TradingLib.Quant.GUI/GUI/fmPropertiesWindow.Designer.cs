namespace TradingLib.Quant.GUI
{
    partial class fmPropertiesWindow
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
            this.propertiesWindow1 = new TradingLib.Quant.GUI.PropertiesWindow();
            this.SuspendLayout();
            // 
            // propertiesWindow1
            // 
            this.propertiesWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesWindow1.Location = new System.Drawing.Point(0, 0);
            this.propertiesWindow1.Name = "propertiesWindow1";
            this.propertiesWindow1.Size = new System.Drawing.Size(271, 456);
            this.propertiesWindow1.TabIndex = 0;
            // 
            // fmPropertiesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 456);
            this.Controls.Add(this.propertiesWindow1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "fmPropertiesWindow";
            this.Text = "参数编辑";
            this.ResumeLayout(false);

        }

        #endregion

        private PropertiesWindow propertiesWindow1;
    }
}