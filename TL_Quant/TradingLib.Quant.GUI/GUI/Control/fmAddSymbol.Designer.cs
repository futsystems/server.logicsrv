namespace TradingLib.Quant.GUI
{
    partial class fmAddSymbol
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
            this.ctSecurityList1 = new TradingLib.GUI.ctSecurityList();
            this.seclist = new System.Windows.Forms.ListBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.expire = new System.Windows.Forms.ComboBox();
            this.add = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.del = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ok = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ok);
            this.kryptonPanel1.Controls.Add(this.del);
            this.kryptonPanel1.Controls.Add(this.add);
            this.kryptonPanel1.Controls.Add(this.expire);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.seclist);
            this.kryptonPanel1.Controls.Add(this.ctSecurityList1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(875, 390);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // ctSecurityList1
            // 
            this.ctSecurityList1.EditEnable = true;
            this.ctSecurityList1.Location = new System.Drawing.Point(238, 0);
            this.ctSecurityList1.Name = "ctSecurityList1";
            this.ctSecurityList1.Size = new System.Drawing.Size(637, 355);
            this.ctSecurityList1.TabIndex = 0;
            // 
            // seclist
            // 
            this.seclist.FormattingEnabled = true;
            this.seclist.ItemHeight = 12;
            this.seclist.Location = new System.Drawing.Point(3, 31);
            this.seclist.Name = "seclist";
            this.seclist.Size = new System.Drawing.Size(136, 352);
            this.seclist.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 7);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(106, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "当前添加的合约";
            // 
            // expire
            // 
            this.expire.FormattingEnabled = true;
            this.expire.Location = new System.Drawing.Point(145, 78);
            this.expire.Name = "expire";
            this.expire.Size = new System.Drawing.Size(78, 20);
            this.expire.TabIndex = 3;
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(145, 118);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(78, 22);
            this.add.TabIndex = 4;
            this.add.Values.Text = "添 加";
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // del
            // 
            this.del.Location = new System.Drawing.Point(145, 149);
            this.del.Name = "del";
            this.del.Size = new System.Drawing.Size(78, 22);
            this.del.TabIndex = 5;
            this.del.Values.Text = "删 除";
            this.del.Click += new System.EventHandler(this.del_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(794, 365);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(78, 22);
            this.ok.TabIndex = 6;
            this.ok.Values.Text = "确 认";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // fmAddSymbol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 390);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "fmAddSymbol";
            this.Text = "fmAddSymbol";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.ListBox seclist;
        //private TradingLib.GUI.ctSecurityList ctSecurityList1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton del;
        private ComponentFactory.Krypton.Toolkit.KryptonButton add;
        private System.Windows.Forms.ComboBox expire;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ok;
    }
}