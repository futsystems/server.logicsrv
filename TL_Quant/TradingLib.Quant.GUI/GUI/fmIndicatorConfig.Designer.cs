namespace TradingLib.Quant.GUI
{
    partial class fmIndicatorConfig
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
            this.seriesLineColor = new ComponentFactory.Krypton.Toolkit.KryptonColorButton();
            this.chartPane = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cancle = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.ok = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.serieslinetype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.seriesName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.seriesLineSize = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serieslinetype)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seriesLineSize)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.seriesLineSize);
            this.kryptonPanel1.Controls.Add(this.seriesLineColor);
            this.kryptonPanel1.Controls.Add(this.chartPane);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.tableLayoutPanel);
            this.kryptonPanel1.Controls.Add(this.cancle);
            this.kryptonPanel1.Controls.Add(this.ok);
            this.kryptonPanel1.Controls.Add(this.serieslinetype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.seriesName);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(225, 377);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // seriesLineColor
            // 
            this.seriesLineColor.Location = new System.Drawing.Point(3, 68);
            this.seriesLineColor.Name = "seriesLineColor";
            this.seriesLineColor.Size = new System.Drawing.Size(100, 25);
            this.seriesLineColor.TabIndex = 13;
            this.seriesLineColor.Values.Text = "选 色";
            // 
            // chartPane
            // 
            this.chartPane.DropDownWidth = 121;
            this.chartPane.Location = new System.Drawing.Point(3, 174);
            this.chartPane.Name = "chartPane";
            this.chartPane.Size = new System.Drawing.Size(121, 21);
            this.chartPane.TabIndex = 12;
            this.chartPane.Text = "kryptonComboBox2";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(3, 150);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel5.TabIndex = 11;
            this.kryptonLabel5.Values.Text = "绘制区域:";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 212);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(219, 129);
            this.tableLayoutPanel.TabIndex = 10;
            // 
            // cancle
            // 
            this.cancle.Location = new System.Drawing.Point(162, 51);
            this.cancle.Name = "cancle";
            this.cancle.Size = new System.Drawing.Size(53, 22);
            this.cancle.TabIndex = 9;
            this.cancle.Values.Text = "取 消";
            this.cancle.Click += new System.EventHandler(this.cancle_Click);
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(162, 20);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(53, 22);
            this.ok.TabIndex = 8;
            this.ok.Values.Text = "确 定";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // serieslinetype
            // 
            this.serieslinetype.DropDownWidth = 121;
            this.serieslinetype.Location = new System.Drawing.Point(3, 123);
            this.serieslinetype.Name = "serieslinetype";
            this.serieslinetype.Size = new System.Drawing.Size(121, 21);
            this.serieslinetype.TabIndex = 7;
            this.serieslinetype.Text = "kryptonComboBox2";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(147, 99);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "粗细:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(0, 99);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "线形:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(0, 51);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "颜色:";
            // 
            // seriesName
            // 
            this.seriesName.Location = new System.Drawing.Point(3, 27);
            this.seriesName.Name = "seriesName";
            this.seriesName.Size = new System.Drawing.Size(100, 21);
            this.seriesName.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(0, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(65, 18);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "指标名称";
            // 
            // seriesLineSize
            // 
            this.seriesLineSize.Location = new System.Drawing.Point(147, 123);
            this.seriesLineSize.Name = "seriesLineSize";
            this.seriesLineSize.Size = new System.Drawing.Size(60, 21);
            this.seriesLineSize.TabIndex = 14;
            this.seriesLineSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // fmIndicatorConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 377);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmIndicatorConfig";
            this.Text = "添加指标";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serieslinetype)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seriesLineSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox serieslinetype;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox seriesName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton cancle;
        private ComponentFactory.Krypton.Toolkit.KryptonButton ok;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox chartPane;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonColorButton seriesLineColor;
        private System.Windows.Forms.NumericUpDown seriesLineSize;
    }
}