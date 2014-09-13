namespace TradingLib.Quant.Engine
{
    partial class OptimizationDialog
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
            this.kryptonPanel1 = new System.Windows.Forms.Panel();
            this.grid = new SourceGrid.Grid();
            this.runnum = new System.Windows.Forms.Label();
            this.kryptonLabel1 = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.threadnum = new System.Windows.Forms.ComboBox();
            this.kryptonLabel2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.threadnum);
            this.kryptonPanel1.Controls.Add(this.grid);
            this.kryptonPanel1.Controls.Add(this.runnum);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.cancel);
            this.kryptonPanel1.Controls.Add(this.ok);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(379, 175);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // grid
            // 
            this.grid.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.grid.Dock = System.Windows.Forms.DockStyle.Top;
            this.grid.EnableSort = true;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid.Size = new System.Drawing.Size(379, 144);
            this.grid.TabIndex = 4;
            this.grid.TabStop = true;
            this.grid.ToolTipText = "";
            // 
            // runnum
            // 
            this.runnum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runnum.Location = new System.Drawing.Point(95, 154);
            this.runnum.Name = "runnum";
            this.runnum.Size = new System.Drawing.Size(19, 18);
            this.runnum.TabIndex = 3;
            this.runnum.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 154);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(95, 18);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Text = "预计运算次数:";
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(324, 150);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(52, 22);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "取消";
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(268, 150);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(52, 22);
            this.ok.TabIndex = 0;
            this.ok.Text = "开始";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // threadnum
            // 
            this.threadnum.FormattingEnabled = true;
            this.threadnum.Location = new System.Drawing.Point(194, 152);
            this.threadnum.Name = "threadnum";
            this.threadnum.Size = new System.Drawing.Size(58, 20);
            this.threadnum.TabIndex = 5;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonLabel2.Location = new System.Drawing.Point(120, 154);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 6;
            this.kryptonLabel2.Text = "开启线程:";
            // 
            // OptimizationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 175);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptimizationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "优化参数设置";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel kryptonPanel1;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label runnum;
        private System.Windows.Forms.Label kryptonLabel1;
        private SourceGrid.Grid grid;
        private System.Windows.Forms.Label kryptonLabel2;
        private System.Windows.Forms.ComboBox threadnum;
    }
}