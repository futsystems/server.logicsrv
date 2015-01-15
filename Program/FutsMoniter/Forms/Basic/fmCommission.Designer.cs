namespace FutsMoniter
{
    partial class fmCommission
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
            this.commissionGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.templatelist = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.templatename = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commissionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.templatename);
            this.kryptonPanel1.Controls.Add(this.commissionGrid);
            this.kryptonPanel1.Controls.Add(this.templatelist);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(807, 442);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // commissionGrid
            // 
            this.commissionGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commissionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commissionGrid.Location = new System.Drawing.Point(140, 31);
            this.commissionGrid.Name = "commissionGrid";
            this.commissionGrid.RowTemplate.Height = 23;
            this.commissionGrid.Size = new System.Drawing.Size(667, 411);
            this.commissionGrid.TabIndex = 1;
            // 
            // templatelist
            // 
            this.templatelist.Dock = System.Windows.Forms.DockStyle.Left;
            this.templatelist.Location = new System.Drawing.Point(0, 0);
            this.templatelist.Name = "templatelist";
            this.templatelist.Size = new System.Drawing.Size(140, 442);
            this.templatelist.TabIndex = 0;
            // 
            // templatename
            // 
            this.templatename.Location = new System.Drawing.Point(261, 7);
            this.templatename.Name = "templatename";
            this.templatename.Size = new System.Drawing.Size(19, 18);
            this.templatename.TabIndex = 2;
            this.templatename.Values.Text = "--";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(146, 7);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(109, 18);
            this.kryptonLabel2.TabIndex = 3;
            this.kryptonLabel2.Values.Text = "当前手续费模板:";
            // 
            // fmCommission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 442);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "fmCommission";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "手续费模板设置";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commissionGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonListBox templatelist;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView commissionGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel templatename;
    }
}