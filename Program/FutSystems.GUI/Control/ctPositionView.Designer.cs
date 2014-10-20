namespace FutSystems.GUI
{
    partial class ctPositionView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnShowHold = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnShowAll = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.btnFlat = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnFlatAll = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnReserve = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.num = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.isDoubleFlat = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.positiongrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positiongrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.positiongrid);
            this.kryptonPanel1.Controls.Add(this.isDoubleFlat);
            this.kryptonPanel1.Controls.Add(this.num);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.btnCancel);
            this.kryptonPanel1.Controls.Add(this.btnReserve);
            this.kryptonPanel1.Controls.Add(this.btnFlatAll);
            this.kryptonPanel1.Controls.Add(this.btnFlat);
            this.kryptonPanel1.Controls.Add(this.btnShowAll);
            this.kryptonPanel1.Controls.Add(this.btnShowHold);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(872, 261);
            this.kryptonPanel1.TabIndex = 2;
            // 
            // btnShowHold
            // 
            this.btnShowHold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowHold.Location = new System.Drawing.Point(4, 236);
            this.btnShowHold.Name = "btnShowHold";
            this.btnShowHold.Size = new System.Drawing.Size(50, 18);
            this.btnShowHold.TabIndex = 0;
            this.btnShowHold.Values.Text = "持仓";
            // 
            // btnShowAll
            // 
            this.btnShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowAll.Location = new System.Drawing.Point(60, 236);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(77, 18);
            this.btnShowAll.TabIndex = 1;
            this.btnShowAll.Values.Text = "当日明细";
            // 
            // btnFlat
            // 
            this.btnFlat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFlat.Location = new System.Drawing.Point(143, 233);
            this.btnFlat.Name = "btnFlat";
            this.btnFlat.Size = new System.Drawing.Size(59, 25);
            this.btnFlat.TabIndex = 2;
            this.btnFlat.Values.Text = "平 仓";
            // 
            // btnFlatAll
            // 
            this.btnFlatAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFlatAll.Location = new System.Drawing.Point(208, 233);
            this.btnFlatAll.Name = "btnFlatAll";
            this.btnFlatAll.Size = new System.Drawing.Size(59, 25);
            this.btnFlatAll.TabIndex = 3;
            this.btnFlatAll.Values.Text = "全 平";
            // 
            // btnReserve
            // 
            this.btnReserve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReserve.Location = new System.Drawing.Point(273, 233);
            this.btnReserve.Name = "btnReserve";
            this.btnReserve.Size = new System.Drawing.Size(59, 25);
            this.btnReserve.TabIndex = 4;
            this.btnReserve.Values.Text = "反 手";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(338, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 25);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Values.Text = "撤 单";
            // 
            // num
            // 
            this.num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.num.Location = new System.Drawing.Point(846, 236);
            this.num.Name = "num";
            this.num.Size = new System.Drawing.Size(19, 18);
            this.num.TabIndex = 10;
            this.num.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonLabel1.Location = new System.Drawing.Point(757, 236);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(68, 18);
            this.kryptonLabel1.TabIndex = 9;
            this.kryptonLabel1.Values.Text = "记录条数:";
            // 
            // isDoubleFlat
            // 
            this.isDoubleFlat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.isDoubleFlat.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.isDoubleFlat.Location = new System.Drawing.Point(403, 236);
            this.isDoubleFlat.Name = "isDoubleFlat";
            this.isDoubleFlat.Size = new System.Drawing.Size(78, 18);
            this.isDoubleFlat.TabIndex = 11;
            this.isDoubleFlat.Text = "双击平仓";
            this.isDoubleFlat.Values.Text = "双击平仓";
            // 
            // positiongrid
            // 
            this.positiongrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.positiongrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.positiongrid.Location = new System.Drawing.Point(0, 0);
            this.positiongrid.Name = "positiongrid";
            this.positiongrid.RowTemplate.Height = 23;
            this.positiongrid.Size = new System.Drawing.Size(872, 230);
            this.positiongrid.TabIndex = 12;
            // 
            // ctPositionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctPositionView";
            this.Size = new System.Drawing.Size(872, 261);
            this.Load += new System.EventHandler(this.ctPositionView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positiongrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnReserve;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnFlatAll;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnFlat;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnShowAll;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton btnShowHold;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel num;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox isDoubleFlat;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView positiongrid;
    }
}
