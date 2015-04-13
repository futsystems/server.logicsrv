namespace FutsMoniter
{
    partial class fmSyncSymbol
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
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.cbVendor = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.cutVendor = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.ckSyncMainDomain = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbVendor)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ckSyncMainDomain);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.cbVendor);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.cutVendor);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(280, 163);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(198, 126);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(70, 25);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Values.Text = "提 交";
            // 
            // cbVendor
            // 
            this.cbVendor.DropDownWidth = 121;
            this.cbVendor.Location = new System.Drawing.Point(113, 39);
            this.cbVendor.Name = "cbVendor";
            this.cbVendor.Size = new System.Drawing.Size(121, 21);
            this.cbVendor.TabIndex = 3;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(12, 42);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(95, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "选择实盘帐户:";
            // 
            // cutVendor
            // 
            this.cutVendor.Location = new System.Drawing.Point(113, 12);
            this.cutVendor.Name = "cutVendor";
            this.cutVendor.Size = new System.Drawing.Size(19, 18);
            this.cutVendor.TabIndex = 1;
            this.cutVendor.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(95, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "当前同步方式:";
            // 
            // ckSyncMainDomain
            // 
            this.ckSyncMainDomain.Location = new System.Drawing.Point(113, 80);
            this.ckSyncMainDomain.Name = "ckSyncMainDomain";
            this.ckSyncMainDomain.Size = new System.Drawing.Size(91, 18);
            this.ckSyncMainDomain.TabIndex = 5;
            this.ckSyncMainDomain.Values.Text = "从主域同步";
            // 
            // fmSyncSymbol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 163);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmSyncSymbol";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "合约同步帐户设置";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbVendor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel cutVendor;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbVendor;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox ckSyncMainDomain;
    }
}