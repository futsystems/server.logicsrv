namespace FutsMoniter
{
    partial class MarketTimeInfoForm
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
            this.lbMTName = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.lbDesp = new Telerik.WinControls.UI.RadLabel();
            this.sesslist = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbMTName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbDesp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sesslist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(30, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(34, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "名称:";
            // 
            // lbMTName
            // 
            this.lbMTName.Location = new System.Drawing.Point(70, 13);
            this.lbMTName.Name = "lbMTName";
            this.lbMTName.Size = new System.Drawing.Size(14, 16);
            this.lbMTName.TabIndex = 1;
            this.lbMTName.Text = "--";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(17, 57);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(47, 16);
            this.radLabel2.TabIndex = 3;
            this.radLabel2.Text = "时间段:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(30, 35);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(34, 16);
            this.radLabel3.TabIndex = 4;
            this.radLabel3.Text = "描述:";
            // 
            // lbDesp
            // 
            this.lbDesp.Location = new System.Drawing.Point(70, 35);
            this.lbDesp.Name = "lbDesp";
            this.lbDesp.Size = new System.Drawing.Size(14, 16);
            this.lbDesp.TabIndex = 5;
            this.lbDesp.Text = "--";
            // 
            // sesslist
            // 
            this.sesslist.AutoSize = false;
            this.sesslist.Location = new System.Drawing.Point(17, 77);
            this.sesslist.Multiline = true;
            this.sesslist.Name = "sesslist";
            this.sesslist.Size = new System.Drawing.Size(241, 82);
            this.sesslist.TabIndex = 6;
            // 
            // MarketTimeInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 201);
            this.Controls.Add(this.sesslist);
            this.Controls.Add(this.lbDesp);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.lbMTName);
            this.Controls.Add(this.radLabel1);
            this.Name = "MarketTimeInfoForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "交易时间段";
            this.ThemeName = "ControlDefault";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbMTName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbDesp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sesslist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel lbMTName;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel lbDesp;
        private Telerik.WinControls.UI.RadTextBox sesslist;
    }
}
