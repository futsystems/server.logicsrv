namespace TradingLib.Quant.GUI
{
    partial class BarGenerator
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
            this.symbol = new System.Windows.Forms.TextBox();
            this.isSaveBar = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.generator = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.interval = new System.Windows.Forms.NumericUpDown();
            this.bartintervaltype = new System.Windows.Forms.ComboBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._progress = new System.Windows.Forms.ToolStripProgressBar();
            this.ctDebug1 = new TradingLib.Quant.GUI.ctDebug();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interval)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.ctDebug1);
            this.kryptonPanel1.Controls.Add(this.symbol);
            this.kryptonPanel1.Controls.Add(this.isSaveBar);
            this.kryptonPanel1.Controls.Add(this.generator);
            this.kryptonPanel1.Controls.Add(this.interval);
            this.kryptonPanel1.Controls.Add(this.bartintervaltype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(292, 320);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(47, 0);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(101, 21);
            this.symbol.TabIndex = 8;
            // 
            // isSaveBar
            // 
            this.isSaveBar.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.isSaveBar.Location = new System.Drawing.Point(157, 53);
            this.isSaveBar.Name = "isSaveBar";
            this.isSaveBar.Size = new System.Drawing.Size(70, 18);
            this.isSaveBar.TabIndex = 7;
            this.isSaveBar.Text = "保存Bar";
            this.isSaveBar.Values.Text = "保存Bar";
            // 
            // generator
            // 
            this.generator.Location = new System.Drawing.Point(233, 49);
            this.generator.Name = "generator";
            this.generator.Size = new System.Drawing.Size(57, 22);
            this.generator.TabIndex = 6;
            this.generator.Values.Text = "生成";
            this.generator.Click += new System.EventHandler(this.generator_Click);
            // 
            // interval
            // 
            this.interval.Location = new System.Drawing.Point(47, 53);
            this.interval.Name = "interval";
            this.interval.Size = new System.Drawing.Size(101, 21);
            this.interval.TabIndex = 5;
            this.interval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // bartintervaltype
            // 
            this.bartintervaltype.FormattingEnabled = true;
            this.bartintervaltype.Location = new System.Drawing.Point(48, 27);
            this.bartintervaltype.Name = "bartintervaltype";
            this.bartintervaltype.Size = new System.Drawing.Size(100, 20);
            this.bartintervaltype.TabIndex = 4;
            this.bartintervaltype.SelectedIndexChanged += new System.EventHandler(this.bartintervaltype_SelectedIndexChanged);
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(4, 53);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "重复";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(5, 28);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "频率";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(38, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "合约";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this._progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 298);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(292, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel1.Text = "进度";
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(200, 16);
            // 
            // ctDebug1
            // 
            this.ctDebug1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ctDebug1.ExternalTimeStamp = 0;
            this.ctDebug1.Location = new System.Drawing.Point(0, 80);
            this.ctDebug1.Name = "ctDebug1";
            this.ctDebug1.Size = new System.Drawing.Size(292, 240);
            this.ctDebug1.TabIndex = 9;
            this.ctDebug1.TimeStamps = true;
            this.ctDebug1.UseExternalTimeStamp = false;
            // 
            // BarGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 320);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BarGenerator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BarGenerator";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interval)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        //private TradingLib.GUI.DebugControl debugControl1;
        private System.Windows.Forms.NumericUpDown interval;
        private System.Windows.Forms.ComboBox bartintervaltype;
        private ComponentFactory.Krypton.Toolkit.KryptonButton generator;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar _progress;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox isSaveBar;
        private System.Windows.Forms.TextBox symbol;
        private ctDebug ctDebug1;
    }
}