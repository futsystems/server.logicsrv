namespace FutsMoniter
{
    partial class fmRouterItem
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
            this.active = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.rule = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.cbpriority = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.cbvendor = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.itemid = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbpriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbvendor)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.active);
            this.kryptonPanel1.Controls.Add(this.rule);
            this.kryptonPanel1.Controls.Add(this.cbpriority);
            this.kryptonPanel1.Controls.Add(this.cbvendor);
            this.kryptonPanel1.Controls.Add(this.itemid);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(259, 258);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(147, 221);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(90, 25);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Values.Text = "提交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // active
            // 
            this.active.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.active.Location = new System.Drawing.Point(98, 181);
            this.active.Name = "active";
            this.active.Size = new System.Drawing.Size(19, 13);
            this.active.TabIndex = 9;
            this.active.Values.Text = "";
            // 
            // rule
            // 
            this.rule.Location = new System.Drawing.Point(24, 111);
            this.rule.Multiline = true;
            this.rule.Name = "rule";
            this.rule.Size = new System.Drawing.Size(200, 51);
            this.rule.TabIndex = 8;
            // 
            // cbpriority
            // 
            this.cbpriority.DropDownWidth = 126;
            this.cbpriority.Location = new System.Drawing.Point(98, 60);
            this.cbpriority.Name = "cbpriority";
            this.cbpriority.Size = new System.Drawing.Size(126, 21);
            this.cbpriority.TabIndex = 7;
            // 
            // cbvendor
            // 
            this.cbvendor.DropDownWidth = 126;
            this.cbvendor.Location = new System.Drawing.Point(98, 33);
            this.cbvendor.Name = "cbvendor";
            this.cbvendor.Size = new System.Drawing.Size(126, 21);
            this.cbvendor.TabIndex = 6;
            // 
            // itemid
            // 
            this.itemid.Location = new System.Drawing.Point(98, 12);
            this.itemid.Name = "itemid";
            this.itemid.Size = new System.Drawing.Size(19, 18);
            this.itemid.TabIndex = 5;
            this.itemid.Values.Text = "--";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(24, 179);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 4;
            this.kryptonLabel5.Values.Text = "接受开仓:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(24, 87);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 3;
            this.kryptonLabel4.Values.Text = "规则脚本:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(37, 63);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(55, 18);
            this.kryptonLabel3.TabIndex = 2;
            this.kryptonLabel3.Values.Text = "优先级:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(24, 36);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "实盘帐户:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(39, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(53, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "路由ID:";
            // 
            // fmRouterItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 258);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmRouterItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "路由";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbpriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbvendor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel itemid;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbpriority;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cbvendor;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox active;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox rule;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
    }
}