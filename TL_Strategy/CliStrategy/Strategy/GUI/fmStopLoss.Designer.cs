namespace Strategy.GUI
{
    partial class fmStopLoss
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
            this.kryptonLinkLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._stopLoss = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this._targetprice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this._last = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLinkLabel1);
            this.kryptonPanel1.Controls.Add(this.kryptonGroupBox1);
            this.kryptonPanel1.Controls.Add(this._targetprice);
            this.kryptonPanel1.Controls.Add(this._last);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(210, 152);
            this.kryptonPanel1.TabIndex = 39;
            // 
            // kryptonLinkLabel1
            // 
            this.kryptonLinkLabel1.Location = new System.Drawing.Point(3, 113);
            this.kryptonLinkLabel1.Name = "kryptonLinkLabel1";
            this.kryptonLinkLabel1.Size = new System.Drawing.Size(41, 18);
            this.kryptonLinkLabel1.TabIndex = 40;
            this.kryptonLinkLabel1.Values.Text = "说 明";
            this.kryptonLinkLabel1.LinkClicked += new System.EventHandler(this.kryptonLinkLabel1_LinkClicked);
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(3, 40);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.kryptonLabel3);
            this.kryptonGroupBox1.Panel.Controls.Add(this._stopLoss);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(205, 67);
            this.kryptonGroupBox1.TabIndex = 40;
            this.kryptonGroupBox1.Text = "参数设置";
            this.kryptonGroupBox1.Values.Heading = "参数设置";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(8, 5);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 42;
            this.kryptonLabel3.Values.Text = "点数:";
            // 
            // _stopLoss
            // 
            this._stopLoss.Location = new System.Drawing.Point(52, 3);
            this._stopLoss.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._stopLoss.Name = "_stopLoss";
            this._stopLoss.Size = new System.Drawing.Size(47, 20);
            this._stopLoss.TabIndex = 44;
            // 
            // _targetprice
            // 
            this._targetprice.Location = new System.Drawing.Point(57, 24);
            this._targetprice.Name = "_targetprice";
            this._targetprice.Size = new System.Drawing.Size(19, 18);
            this._targetprice.TabIndex = 47;
            this._targetprice.Values.Text = "--";
            // 
            // _last
            // 
            this._last.Location = new System.Drawing.Point(-1, 24);
            this._last.Name = "_last";
            this._last.Size = new System.Drawing.Size(19, 18);
            this._last.TabIndex = 46;
            this._last.Values.Text = "--";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(57, 0);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(65, 18);
            this.kryptonLabel2.TabIndex = 41;
            this.kryptonLabel2.Values.Text = "触发价格";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(51, 18);
            this.kryptonLabel1.TabIndex = 40;
            this.kryptonLabel1.Values.Text = "最新价";
            // 
            // fmStopLoss
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 152);
            this.ControlBox = false;
            this.Controls.Add(this.kryptonPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmStopLoss";
            this.Text = "fmTargetProfit";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown _stopLoss;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _targetprice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel _last;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel kryptonLinkLabel1;
    }
}