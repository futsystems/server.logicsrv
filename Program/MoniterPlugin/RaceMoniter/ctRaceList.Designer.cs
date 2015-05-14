namespace TradingLib.RaceMoniter
{
    partial class ctRaceList
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
            this.raceGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnOpenNewRace = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnQryRaceList = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnExamineRace = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.raceGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // raceGrid
            // 
            this.raceGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.raceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.raceGrid.Location = new System.Drawing.Point(0, 46);
            this.raceGrid.Name = "raceGrid";
            this.raceGrid.RowTemplate.Height = 23;
            this.raceGrid.Size = new System.Drawing.Size(540, 271);
            this.raceGrid.TabIndex = 0;
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnExamineRace);
            this.kryptonPanel1.Controls.Add(this.btnOpenNewRace);
            this.kryptonPanel1.Controls.Add(this.btnQryRaceList);
            this.kryptonPanel1.Controls.Add(this.raceGrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(540, 317);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // btnOpenNewRace
            // 
            this.btnOpenNewRace.Location = new System.Drawing.Point(120, 15);
            this.btnOpenNewRace.Name = "btnOpenNewRace";
            this.btnOpenNewRace.Size = new System.Drawing.Size(90, 25);
            this.btnOpenNewRace.TabIndex = 2;
            this.btnOpenNewRace.Values.Text = "新开比赛";
            // 
            // btnQryRaceList
            // 
            this.btnQryRaceList.Location = new System.Drawing.Point(14, 15);
            this.btnQryRaceList.Name = "btnQryRaceList";
            this.btnQryRaceList.Size = new System.Drawing.Size(90, 25);
            this.btnQryRaceList.TabIndex = 1;
            this.btnQryRaceList.Values.Text = "查询比赛";
            // 
            // btnExamineRace
            // 
            this.btnExamineRace.Location = new System.Drawing.Point(447, 15);
            this.btnExamineRace.Name = "btnExamineRace";
            this.btnExamineRace.Size = new System.Drawing.Size(90, 25);
            this.btnExamineRace.TabIndex = 3;
            this.btnExamineRace.Values.Text = "手工考核";
            // 
            // ctRaceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "ctRaceList";
            this.Size = new System.Drawing.Size(540, 317);
            ((System.ComponentModel.ISupportInitialize)(this.raceGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView raceGrid;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnQryRaceList;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOpenNewRace;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnExamineRace;
    }
}
