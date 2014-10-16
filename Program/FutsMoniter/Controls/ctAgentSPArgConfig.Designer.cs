namespace FutsMoniter
{
    partial class ctAgentSPArgConfig
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
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.ctFinServicePlanList1 = new FutsMoniter.ctFinServicePlanList();
            this.ctAgentList1 = new FutsMoniter.ctAgentList();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            this.gparg = new Telerik.WinControls.UI.RadGroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.lbcollecttype = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.lbchargetype = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.lbsptitle = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gparg)).BeginInit();
            this.gparg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).BeginInit();
            this.radGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcollecttype)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbchargetype)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbsptitle)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radGroupBox1);
            this.radPanel1.Controls.Add(this.gparg);
            this.radPanel1.Controls.Add(this.radGroupBox2);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(614, 271);
            this.radPanel1.TabIndex = 0;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radGroupBox1.Controls.Add(this.ctFinServicePlanList1);
            this.radGroupBox1.Controls.Add(this.ctAgentList1);
            this.radGroupBox1.Controls.Add(this.btnSubmit);
            this.radGroupBox1.HeaderText = "操 作";
            this.radGroupBox1.Location = new System.Drawing.Point(428, 3);
            this.radGroupBox1.Name = "radGroupBox1";
            // 
            // 
            // 
            this.radGroupBox1.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox1.Size = new System.Drawing.Size(183, 265);
            this.radGroupBox1.TabIndex = 25;
            this.radGroupBox1.Text = "操 作";
            // 
            // ctFinServicePlanList1
            // 
            this.ctFinServicePlanList1.Location = new System.Drawing.Point(5, 47);
            this.ctFinServicePlanList1.Name = "ctFinServicePlanList1";
            this.ctFinServicePlanList1.Size = new System.Drawing.Size(164, 22);
            this.ctFinServicePlanList1.TabIndex = 31;
            this.ctFinServicePlanList1.ServicePlanSelectedChangedEvent += new TradingLib.API.VoidDelegate(this.ctFinServicePlanList1_ServicePlanSelectedChangedEvent);
            // 
            // ctAgentList1
            // 
            this.ctAgentList1.Location = new System.Drawing.Point(7, 21);
            this.ctAgentList1.Name = "ctAgentList1";
            this.ctAgentList1.Size = new System.Drawing.Size(162, 20);
            this.ctAgentList1.TabIndex = 1;
            this.ctAgentList1.AgentSelectedChangedEvent += new TradingLib.API.VoidDelegate(this.ctAgentList1_AgentSelectedChangedEvent);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(84, 214);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(88, 36);
            this.btnSubmit.TabIndex = 30;
            this.btnSubmit.Text = "提交修改";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // gparg
            // 
            this.gparg.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.gparg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gparg.Controls.Add(this.tableLayoutPanel);
            this.gparg.HeaderText = "参数列表";
            this.gparg.Location = new System.Drawing.Point(186, 3);
            this.gparg.Name = "gparg";
            // 
            // 
            // 
            this.gparg.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.gparg.Size = new System.Drawing.Size(236, 265);
            this.gparg.TabIndex = 24;
            this.gparg.Text = "参数列表";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 283F));
            this.tableLayoutPanel.Location = new System.Drawing.Point(2, 18);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 9;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(232, 216);
            this.tableLayoutPanel.TabIndex = 11;
            // 
            // radGroupBox2
            // 
            this.radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.radGroupBox2.Controls.Add(this.radLabel2);
            this.radGroupBox2.Controls.Add(this.lbcollecttype);
            this.radGroupBox2.Controls.Add(this.radLabel4);
            this.radGroupBox2.Controls.Add(this.lbchargetype);
            this.radGroupBox2.Controls.Add(this.radLabel5);
            this.radGroupBox2.Controls.Add(this.lbsptitle);
            this.radGroupBox2.HeaderText = "基本参数";
            this.radGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.radGroupBox2.Name = "radGroupBox2";
            // 
            // 
            // 
            this.radGroupBox2.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox2.Size = new System.Drawing.Size(177, 265);
            this.radGroupBox2.TabIndex = 23;
            this.radGroupBox2.Text = "基本参数";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(5, 26);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(59, 16);
            this.radLabel2.TabIndex = 10;
            this.radLabel2.Text = "服务计划:";
            // 
            // lbcollecttype
            // 
            this.lbcollecttype.Location = new System.Drawing.Point(70, 70);
            this.lbcollecttype.Name = "lbcollecttype";
            this.lbcollecttype.Size = new System.Drawing.Size(14, 16);
            this.lbcollecttype.TabIndex = 15;
            this.lbcollecttype.Text = "--";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(5, 48);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(59, 16);
            this.radLabel4.TabIndex = 11;
            this.radLabel4.Text = "计费方式:";
            // 
            // lbchargetype
            // 
            this.lbchargetype.Location = new System.Drawing.Point(70, 48);
            this.lbchargetype.Name = "lbchargetype";
            this.lbchargetype.Size = new System.Drawing.Size(14, 16);
            this.lbchargetype.TabIndex = 14;
            this.lbchargetype.Text = "--";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(5, 70);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(59, 16);
            this.radLabel5.TabIndex = 12;
            this.radLabel5.Text = "采集方式:";
            // 
            // lbsptitle
            // 
            this.lbsptitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbsptitle.ForeColor = System.Drawing.Color.Crimson;
            this.lbsptitle.Location = new System.Drawing.Point(70, 26);
            this.lbsptitle.Name = "lbsptitle";
            this.lbsptitle.Size = new System.Drawing.Size(14, 16);
            this.lbsptitle.TabIndex = 13;
            this.lbsptitle.Text = "--";
            // 
            // ctAgentSPArgConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctAgentSPArgConfig";
            this.Size = new System.Drawing.Size(614, 271);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gparg)).EndInit();
            this.gparg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).EndInit();
            this.radGroupBox2.ResumeLayout(false);
            this.radGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcollecttype)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbchargetype)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbsptitle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel lbcollecttype;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel lbchargetype;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel lbsptitle;
        private Telerik.WinControls.UI.RadGroupBox gparg;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private ctFinServicePlanList ctFinServicePlanList1;
        private ctAgentList ctAgentList1;
        private Telerik.WinControls.UI.RadButton btnSubmit;
    }
}
