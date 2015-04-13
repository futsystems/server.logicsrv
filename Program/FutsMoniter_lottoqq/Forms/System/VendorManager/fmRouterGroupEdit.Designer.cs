namespace FutsMoniter
{
    partial class fmRouterGroupEdit
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
            this.rgdescrption = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.cbrgstrategytype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.rgname = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbrgstrategytype)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.rgdescrption);
            this.kryptonPanel1.Controls.Add(this.cbrgstrategytype);
            this.kryptonPanel1.Controls.Add(this.rgname);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(250, 205);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // rgdescrption
            // 
            this.rgdescrption.Location = new System.Drawing.Point(25, 88);
            this.rgdescrption.Multiline = true;
            this.rgdescrption.Name = "rgdescrption";
            this.rgdescrption.Size = new System.Drawing.Size(187, 55);
            this.rgdescrption.TabIndex = 23;
            // 
            // cbrgstrategytype
            // 
            this.cbrgstrategytype.DropDownWidth = 126;
            this.cbrgstrategytype.Location = new System.Drawing.Point(72, 37);
            this.cbrgstrategytype.Name = "cbrgstrategytype";
            this.cbrgstrategytype.Size = new System.Drawing.Size(140, 21);
            this.cbrgstrategytype.TabIndex = 22;
            // 
            // rgname
            // 
            this.rgname.Location = new System.Drawing.Point(72, 12);
            this.rgname.Name = "rgname";
            this.rgname.Size = new System.Drawing.Size(140, 21);
            this.rgname.TabIndex = 21;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(25, 64);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 20;
            this.kryptonLabel4.Values.Text = "描述:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(25, 40);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 19;
            this.kryptonLabel3.Values.Text = "策略:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(25, 16);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 18;
            this.kryptonLabel2.Values.Text = "名称:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(166, 167);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(67, 25);
            this.btnSubmit.TabIndex = 17;
            this.btnSubmit.Values.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // fmRouterGroupEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 205);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmRouterGroupEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加路由";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbrgstrategytype)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox rgdescrption;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbrgstrategytype;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox rgname;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
    }
}