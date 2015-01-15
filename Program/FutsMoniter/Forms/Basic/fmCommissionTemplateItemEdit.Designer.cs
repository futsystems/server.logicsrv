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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.openbymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.openbyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.closetodaybyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closetodaybymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closebyvolume = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.closebymoney = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.id = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.code = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.btnSubmit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel13 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.chargetype = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.month = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.bymoney = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.byvolume = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chargetype)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.month)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.byvolume);
            this.kryptonPanel1.Controls.Add(this.bymoney);
            this.kryptonPanel1.Controls.Add(this.month);
            this.kryptonPanel1.Controls.Add(this.chargetype);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel13);
            this.kryptonPanel1.Controls.Add(this.btnSubmit);
            this.kryptonPanel1.Controls.Add(this.code);
            this.kryptonPanel1.Controls.Add(this.id);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel1.Controls.Add(this.closebyvolume);
            this.kryptonPanel1.Controls.Add(this.closebymoney);
            this.kryptonPanel1.Controls.Add(this.closetodaybyvolume);
            this.kryptonPanel1.Controls.Add(this.closetodaybymoney);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel1.Controls.Add(this.openbyvolume);
            this.kryptonPanel1.Controls.Add(this.openbymoney);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(277, 346);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(41, 12);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(53, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "项目ID:";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(26, 36);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "品种代码:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(53, 60);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(41, 18);
            this.kryptonLabel3.TabIndex = 3;
            this.kryptonLabel3.Values.Text = "月份:";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(12, 106);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel4.TabIndex = 4;
            this.kryptonLabel4.Values.Text = "开仓手续费:";
            // 
            // openbymoney
            // 
            this.openbymoney.DecimalPlaces = 6;
            this.openbymoney.Location = new System.Drawing.Point(101, 106);
            this.openbymoney.Name = "openbymoney";
            this.openbymoney.Size = new System.Drawing.Size(131, 20);
            this.openbymoney.TabIndex = 5;
            // 
            // openbyvolume
            // 
            this.openbyvolume.DecimalPlaces = 6;
            this.openbyvolume.Location = new System.Drawing.Point(101, 132);
            this.openbyvolume.Name = "openbyvolume";
            this.openbyvolume.Size = new System.Drawing.Size(131, 20);
            this.openbyvolume.TabIndex = 6;
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(12, 158);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel5.TabIndex = 7;
            this.kryptonLabel5.Values.Text = "开今手续费:";
            // 
            // closetodaybyvolume
            // 
            this.closetodaybyvolume.DecimalPlaces = 6;
            this.closetodaybyvolume.Location = new System.Drawing.Point(100, 184);
            this.closetodaybyvolume.Name = "closetodaybyvolume";
            this.closetodaybyvolume.Size = new System.Drawing.Size(131, 20);
            this.closetodaybyvolume.TabIndex = 9;
            // 
            // closetodaybymoney
            // 
            this.closetodaybymoney.DecimalPlaces = 6;
            this.closetodaybymoney.Location = new System.Drawing.Point(100, 158);
            this.closetodaybymoney.Name = "closetodaybymoney";
            this.closetodaybymoney.Size = new System.Drawing.Size(131, 20);
            this.closetodaybymoney.TabIndex = 8;
            // 
            // closebyvolume
            // 
            this.closebyvolume.DecimalPlaces = 6;
            this.closebyvolume.Location = new System.Drawing.Point(100, 236);
            this.closebyvolume.Name = "closebyvolume";
            this.closebyvolume.Size = new System.Drawing.Size(131, 20);
            this.closebyvolume.TabIndex = 11;
            // 
            // closebymoney
            // 
            this.closebymoney.DecimalPlaces = 6;
            this.closebymoney.Location = new System.Drawing.Point(100, 210);
            this.closebymoney.Name = "closebymoney";
            this.closebymoney.Size = new System.Drawing.Size(131, 20);
            this.closebymoney.TabIndex = 10;
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(12, 210);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(82, 18);
            this.kryptonLabel6.TabIndex = 12;
            this.kryptonLabel6.Values.Text = "开仓手续费:";
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(100, 12);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(19, 18);
            this.id.TabIndex = 19;
            this.id.Values.Text = "--";
            // 
            // code
            // 
            this.code.Location = new System.Drawing.Point(101, 32);
            this.code.Name = "code";
            this.code.Size = new System.Drawing.Size(131, 21);
            this.code.TabIndex = 20;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(195, 309);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(70, 25);
            this.btnSubmit.TabIndex = 21;
            this.btnSubmit.Values.Text = "提 交";
            // 
            // kryptonLabel13
            // 
            this.kryptonLabel13.Location = new System.Drawing.Point(26, 263);
            this.kryptonLabel13.Name = "kryptonLabel13";
            this.kryptonLabel13.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel13.TabIndex = 22;
            this.kryptonLabel13.Values.Text = "收费方式:";
            // 
            // chargetype
            // 
            this.chargetype.DropDownWidth = 132;
            this.chargetype.Location = new System.Drawing.Point(100, 262);
            this.chargetype.Name = "chargetype";
            this.chargetype.Size = new System.Drawing.Size(132, 21);
            this.chargetype.TabIndex = 23;
            // 
            // month
            // 
            this.month.DropDownWidth = 132;
            this.month.Location = new System.Drawing.Point(100, 60);
            this.month.Name = "month";
            this.month.Size = new System.Drawing.Size(132, 21);
            this.month.TabIndex = 24;
            // 
            // bymoney
            // 
            this.bymoney.Location = new System.Drawing.Point(101, 84);
            this.bymoney.Name = "bymoney";
            this.bymoney.Size = new System.Drawing.Size(63, 18);
            this.bymoney.TabIndex = 25;
            this.bymoney.Values.Text = "按金额";
            // 
            // byvolume
            // 
            this.byvolume.Location = new System.Drawing.Point(168, 84);
            this.byvolume.Name = "byvolume";
            this.byvolume.Size = new System.Drawing.Size(63, 18);
            this.byvolume.TabIndex = 26;
            this.byvolume.Values.Text = "按手数";
            // 
            // fmCommissionTemplateItemEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 346);
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
            ((System.ComponentModel.ISupportInitialize)(this.chargetype)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.month)).EndInit();
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
    }
}