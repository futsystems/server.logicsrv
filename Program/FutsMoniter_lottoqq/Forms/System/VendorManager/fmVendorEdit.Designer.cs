namespace FutsMoniter
{
    partial class fmVendorEdit
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.id = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.futcompany = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.lastequity = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.marginlimit = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.description = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonButton1 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonButton1);
            this.kryptonPanel1.Controls.Add(this.description);
            this.kryptonPanel1.Controls.Add(this.marginlimit);
            this.kryptonPanel1.Controls.Add(this.lastequity);
            this.kryptonPanel1.Controls.Add(this.futcompany);
            this.kryptonPanel1.Controls.Add(this.name);
            this.kryptonPanel1.Controls.Add(this.id);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(292, 289);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(12, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(81, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "帐户全局ID:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(52, 40);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "名称:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(25, 68);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "期货公司:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(25, 94);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "昨日权益:";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(52, 144);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel5.TabIndex = 5;
            this.kryptonLabel5.Values.Text = "描述:";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(25, 120);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel6.TabIndex = 6;
            this.kryptonLabel6.Values.Text = "资金限额:";
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(99, 12);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(19, 18);
            this.id.TabIndex = 7;
            this.id.Values.Text = "--";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(99, 36);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(132, 22);
            this.name.TabIndex = 8;
            // 
            // futcompany
            // 
            this.futcompany.Location = new System.Drawing.Point(99, 64);
            this.futcompany.Name = "futcompany";
            this.futcompany.Size = new System.Drawing.Size(132, 22);
            this.futcompany.TabIndex = 9;
            // 
            // lastequity
            // 
            this.lastequity.DecimalPlaces = 2;
            this.lastequity.Location = new System.Drawing.Point(99, 92);
            this.lastequity.Maximum = new decimal(new int[] {
            50000000,
            0,
            0,
            0});
            this.lastequity.Name = "lastequity";
            this.lastequity.Size = new System.Drawing.Size(132, 20);
            this.lastequity.TabIndex = 10;
            // 
            // marginlimit
            // 
            this.marginlimit.DecimalPlaces = 2;
            this.marginlimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.marginlimit.Location = new System.Drawing.Point(99, 118);
            this.marginlimit.Maximum = new decimal(new int[] {
            50000000,
            0,
            0,
            0});
            this.marginlimit.Name = "marginlimit";
            this.marginlimit.Size = new System.Drawing.Size(132, 20);
            this.marginlimit.TabIndex = 11;
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(52, 168);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(179, 64);
            this.description.TabIndex = 12;
            // 
            // kryptonButton1
            // 
            this.kryptonButton1.Location = new System.Drawing.Point(190, 252);
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Size = new System.Drawing.Size(90, 25);
            this.kryptonButton1.TabIndex = 13;
            this.kryptonButton1.Values.Text = "提 交";
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // fmVendorEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 289);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmVendorEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButton1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox description;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown marginlimit;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown lastequity;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox futcompany;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel id;
    }
}