namespace FutsMoniter
{
    partial class fmSettlement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmSettlement));
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnQryHist = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.settleday = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.account = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.settlebox = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.settlebox);
            this.kryptonPanel1.Controls.Add(this.btnQryHist);
            this.kryptonPanel1.Controls.Add(this.settleday);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.account);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(800, 601);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnQryHist
            // 
            this.btnQryHist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQryHist.Location = new System.Drawing.Point(730, 12);
            this.btnQryHist.Name = "btnQryHist";
            this.btnQryHist.Size = new System.Drawing.Size(60, 25);
            this.btnQryHist.TabIndex = 10;
            this.btnQryHist.Values.Text = "查 询";
            this.btnQryHist.Click += new System.EventHandler(this.btnQryHist_Click);
            // 
            // settleday
            // 
            this.settleday.Location = new System.Drawing.Point(286, 14);
            this.settleday.Name = "settleday";
            this.settleday.Size = new System.Drawing.Size(147, 20);
            this.settleday.TabIndex = 9;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(225, 16);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel2.TabIndex = 8;
            this.kryptonLabel2.Values.Text = "结算日:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(83, 12);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(126, 21);
            this.account.TabIndex = 7;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(9, 16);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 6;
            this.kryptonLabel1.Values.Text = "交易帐户:";
            // 
            // settlebox
            // 
            this.settlebox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settlebox.Location = new System.Drawing.Point(0, 41);
            this.settlebox.Name = "settlebox";
            this.settlebox.Size = new System.Drawing.Size(800, 560);
            this.settlebox.TabIndex = 11;
            this.settlebox.Text = "";
            // 
            // fmSettlement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.kryptonPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fmSettlement";
            this.Text = "查询结算单";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQryHist;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker settleday;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox account;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox settlebox;
    }
}