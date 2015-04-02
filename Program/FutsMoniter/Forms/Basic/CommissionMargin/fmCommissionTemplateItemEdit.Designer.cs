namespace FutsMoniter
{
    partial class fmCommissionTemplateItemEdit
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
            this.setAllCodeMonth = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.gbPercent = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.percent = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.gbConfig = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.openbymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.byvolume = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.openbyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.bymoney = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.closetodaybymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closetodaybyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closebymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closebyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.setAllMonth = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.month = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.chargetype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel13 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.code = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.id = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbPercent)).BeginInit();
            this.gbPercent.Panel.SuspendLayout();
            this.gbPercent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbConfig)).BeginInit();
            this.gbConfig.Panel.SuspendLayout();
            this.gbConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chargetype)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.setAllCodeMonth);
            this.kryptonPanel1.Controls.Add(this.gbPercent);
            this.kryptonPanel1.Controls.Add(this.gbConfig);
            this.kryptonPanel1.Controls.Add(this.setAllMonth);
            this.kryptonPanel1.Controls.Add(this.month);
            this.kryptonPanel1.Controls.Add(this.chargetype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel13);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.code);
            this.kryptonPanel1.Controls.Add(this.id);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(289, 497);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // setAllCodeMonth
            // 
            this.setAllCodeMonth.Location = new System.Drawing.Point(100, 436);
            this.setAllCodeMonth.Name = "setAllCodeMonth";
            this.setAllCodeMonth.Size = new System.Drawing.Size(148, 20);
            this.setAllCodeMonth.TabIndex = 31;
            this.setAllCodeMonth.Values.Text = "修改所有品种所有月份";
            // 
            // gbPercent
            // 
            this.gbPercent.Location = new System.Drawing.Point(12, 327);
            this.gbPercent.Name = "gbPercent";
            // 
            // gbPercent.Panel
            // 
            this.gbPercent.Panel.Controls.Add(this.kryptonLabel7);
            this.gbPercent.Panel.Controls.Add(this.percent);
            this.gbPercent.Size = new System.Drawing.Size(233, 57);
            this.gbPercent.TabIndex = 30;
            this.gbPercent.Values.Heading = "百分比";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(3, 6);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(75, 20);
            this.kryptonLabel7.TabIndex = 29;
            this.kryptonLabel7.Values.Text = "开仓手续费:";
            // 
            // percent
            // 
            this.percent.DecimalPlaces = 2;
            this.percent.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.percent.Location = new System.Drawing.Point(91, 6);
            this.percent.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.percent.Name = "percent";
            this.percent.Size = new System.Drawing.Size(132, 22);
            this.percent.TabIndex = 28;
            // 
            // gbConfig
            // 
            this.gbConfig.Location = new System.Drawing.Point(12, 87);
            this.gbConfig.Name = "gbConfig";
            // 
            // gbConfig.Panel
            // 
            this.gbConfig.Panel.Controls.Add(this.kryptonLabel4);
            this.gbConfig.Panel.Controls.Add(this.openbymoney);
            this.gbConfig.Panel.Controls.Add(this.byvolume);
            this.gbConfig.Panel.Controls.Add(this.openbyvolume);
            this.gbConfig.Panel.Controls.Add(this.bymoney);
            this.gbConfig.Panel.Controls.Add(this.kryptonLabel5);
            this.gbConfig.Panel.Controls.Add(this.closetodaybymoney);
            this.gbConfig.Panel.Controls.Add(this.closetodaybyvolume);
            this.gbConfig.Panel.Controls.Add(this.closebymoney);
            this.gbConfig.Panel.Controls.Add(this.closebyvolume);
            this.gbConfig.Panel.Controls.Add(this.kryptonLabel6);
            this.gbConfig.Size = new System.Drawing.Size(233, 234);
            this.gbConfig.TabIndex = 27;
            this.gbConfig.Values.Heading = "费率设置";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(3, 31);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(75, 20);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "开仓手续费:";
            // 
            // openbymoney
            // 
            this.openbymoney.DecimalPlaces = 6;
            this.openbymoney.Location = new System.Drawing.Point(92, 31);
            this.openbymoney.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.openbymoney.Name = "openbymoney";
            this.openbymoney.Size = new System.Drawing.Size(131, 22);
            this.openbymoney.TabIndex = 5;
            // 
            // byvolume
            // 
            this.byvolume.Location = new System.Drawing.Point(159, 9);
            this.byvolume.Name = "byvolume";
            this.byvolume.Size = new System.Drawing.Size(60, 20);
            this.byvolume.TabIndex = 26;
            this.byvolume.Values.Text = "按手数";
            // 
            // openbyvolume
            // 
            this.openbyvolume.DecimalPlaces = 6;
            this.openbyvolume.Location = new System.Drawing.Point(92, 57);
            this.openbyvolume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.openbyvolume.Name = "openbyvolume";
            this.openbyvolume.Size = new System.Drawing.Size(131, 22);
            this.openbyvolume.TabIndex = 6;
            // 
            // bymoney
            // 
            this.bymoney.Location = new System.Drawing.Point(92, 9);
            this.bymoney.Name = "bymoney";
            this.bymoney.Size = new System.Drawing.Size(60, 20);
            this.bymoney.TabIndex = 25;
            this.bymoney.Values.Text = "按金额";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(3, 83);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(75, 20);
            this.kryptonLabel5.TabIndex = 7;
            this.kryptonLabel5.Values.Text = "平今手续费:";
            // 
            // closetodaybymoney
            // 
            this.closetodaybymoney.DecimalPlaces = 6;
            this.closetodaybymoney.Location = new System.Drawing.Point(91, 83);
            this.closetodaybymoney.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.closetodaybymoney.Name = "closetodaybymoney";
            this.closetodaybymoney.Size = new System.Drawing.Size(131, 22);
            this.closetodaybymoney.TabIndex = 8;
            // 
            // closetodaybyvolume
            // 
            this.closetodaybyvolume.DecimalPlaces = 6;
            this.closetodaybyvolume.Location = new System.Drawing.Point(91, 109);
            this.closetodaybyvolume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.closetodaybyvolume.Name = "closetodaybyvolume";
            this.closetodaybyvolume.Size = new System.Drawing.Size(131, 22);
            this.closetodaybyvolume.TabIndex = 9;
            // 
            // closebymoney
            // 
            this.closebymoney.DecimalPlaces = 6;
            this.closebymoney.Location = new System.Drawing.Point(91, 135);
            this.closebymoney.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.closebymoney.Name = "closebymoney";
            this.closebymoney.Size = new System.Drawing.Size(131, 22);
            this.closebymoney.TabIndex = 10;
            // 
            // closebyvolume
            // 
            this.closebyvolume.DecimalPlaces = 6;
            this.closebyvolume.Location = new System.Drawing.Point(91, 161);
            this.closebyvolume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.closebyvolume.Name = "closebyvolume";
            this.closebyvolume.Size = new System.Drawing.Size(131, 22);
            this.closebyvolume.TabIndex = 11;
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(3, 135);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(75, 20);
            this.kryptonLabel6.TabIndex = 12;
            this.kryptonLabel6.Values.Text = "平仓手续费:";
            // 
            // setAllMonth
            // 
            this.setAllMonth.Location = new System.Drawing.Point(101, 414);
            this.setAllMonth.Name = "setAllMonth";
            this.setAllMonth.Size = new System.Drawing.Size(136, 20);
            this.setAllMonth.TabIndex = 1;
            this.setAllMonth.Values.Text = "修改该品种所有月份";
            // 
            // month
            // 
            this.month.DropDownWidth = 132;
            this.month.Location = new System.Drawing.Point(100, 60);
            this.month.Name = "month";
            this.month.Size = new System.Drawing.Size(132, 21);
            this.month.TabIndex = 24;
            // 
            // chargetype
            // 
            this.chargetype.DropDownWidth = 132;
            this.chargetype.Location = new System.Drawing.Point(104, 387);
            this.chargetype.Name = "chargetype";
            this.chargetype.Size = new System.Drawing.Size(132, 21);
            this.chargetype.TabIndex = 23;
            // 
            // kryptonLabel13
            // 
            this.kryptonLabel13.Location = new System.Drawing.Point(31, 390);
            this.kryptonLabel13.Name = "kryptonLabel13";
            this.kryptonLabel13.Size = new System.Drawing.Size(63, 20);
            this.kryptonLabel13.TabIndex = 22;
            this.kryptonLabel13.Values.Text = "加收方式:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(207, 460);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(70, 25);
            this.btnSubmit.TabIndex = 21;
            this.btnSubmit.Values.Text = "提 交";
            // 
            // code
            // 
            this.code.Location = new System.Drawing.Point(101, 32);
            this.code.Name = "code";
            this.code.Size = new System.Drawing.Size(131, 20);
            this.code.TabIndex = 20;
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(100, 12);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(20, 20);
            this.id.TabIndex = 19;
            this.id.Values.Text = "--";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(53, 60);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(38, 20);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "月份:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(26, 36);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(63, 20);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "品种代码:";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(41, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(50, 20);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "项目ID:";
            // 
            // fmCommissionTemplateItemEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 497);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmCommissionTemplateItemEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "手续费项目";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.gbPercent.Panel.ResumeLayout(false);
            this.gbPercent.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbPercent)).EndInit();
            this.gbPercent.ResumeLayout(false);
            this.gbConfig.Panel.ResumeLayout(false);
            this.gbConfig.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbConfig)).EndInit();
            this.gbConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chargetype)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown closetodaybyvolume;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown closetodaybymoney;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown openbyvolume;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown openbymoney;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown closebyvolume;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown closebymoney;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel id;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox code;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSubmit;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel13;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox chargetype;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox month;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton bymoney;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton byvolume;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox setAllMonth;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox gbConfig;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox gbPercent;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown percent;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox setAllCodeMonth;
    }
}