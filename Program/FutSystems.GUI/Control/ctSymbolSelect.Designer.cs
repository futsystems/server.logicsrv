namespace FutSystems.GUI
{
    partial class ctSymbolSelect
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
            this.btnDel = new Telerik.WinControls.UI.RadButton();
            this.btnAdd = new Telerik.WinControls.UI.RadButton();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.symlist = new Telerik.WinControls.UI.RadListControl();
            this.exchlist = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.secgrid = new Telerik.WinControls.UI.RadGridView();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.expire = new Telerik.WinControls.UI.RadDropDownList();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnDel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symlist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchlist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expire)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.expire);
            this.radPanel1.Controls.Add(this.radLabel4);
            this.radPanel1.Controls.Add(this.btnDel);
            this.radPanel1.Controls.Add(this.btnAdd);
            this.radPanel1.Controls.Add(this.radLabel3);
            this.radPanel1.Controls.Add(this.radLabel2);
            this.radPanel1.Controls.Add(this.symlist);
            this.radPanel1.Controls.Add(this.exchlist);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Controls.Add(this.secgrid);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(484, 281);
            this.radPanel1.TabIndex = 0;
            // 
            // btnDel
            // 
            this.btnDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDel.Location = new System.Drawing.Point(322, 117);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(63, 24);
            this.btnDel.TabIndex = 18;
            this.btnDel.Text = "<<删除";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(322, 87);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(63, 24);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "添加>>";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(3, 6);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(81, 16);
            this.radLabel3.TabIndex = 16;
            this.radLabel3.Text = "交易品种列表";
            // 
            // radLabel2
            // 
            this.radLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel2.Location = new System.Drawing.Point(391, 6);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(56, 16);
            this.radLabel2.TabIndex = 15;
            this.radLabel2.Text = "自选合约";
            // 
            // symlist
            // 
            this.symlist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.symlist.Location = new System.Drawing.Point(391, 26);
            this.symlist.Name = "symlist";
            this.symlist.Size = new System.Drawing.Size(90, 252);
            this.symlist.TabIndex = 14;
            this.symlist.Text = "radListControl1";
            // 
            // exchlist
            // 
            this.exchlist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.exchlist.Location = new System.Drawing.Point(165, 4);
            this.exchlist.Name = "exchlist";
            this.exchlist.Size = new System.Drawing.Size(133, 18);
            this.exchlist.TabIndex = 13;
            this.exchlist.Text = "--";
            this.exchlist.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.exchlist_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.exchlist.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(115, 6);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(44, 16);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "交易所";
            // 
            // secgrid
            // 
            this.secgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.secgrid.Location = new System.Drawing.Point(3, 26);
            this.secgrid.Name = "secgrid";
            this.secgrid.Size = new System.Drawing.Size(315, 252);
            this.secgrid.TabIndex = 0;
            this.secgrid.Text = "radGridView1";
            // 
            // radLabel4
            // 
            this.radLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radLabel4.Location = new System.Drawing.Point(324, 37);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(56, 16);
            this.radLabel4.TabIndex = 19;
            this.radLabel4.Text = "到期月份";
            // 
            // expire
            // 
            this.expire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.expire.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.expire.Location = new System.Drawing.Point(322, 59);
            this.expire.Name = "expire";
            this.expire.Size = new System.Drawing.Size(63, 18);
            this.expire.TabIndex = 20;
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.expire.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // ctSymbolSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctSymbolSelect";
            this.Size = new System.Drawing.Size(484, 281);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnDel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symlist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchlist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expire)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadGridView secgrid;
        private Telerik.WinControls.UI.RadDropDownList exchlist;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadListControl symlist;
        private Telerik.WinControls.UI.RadButton btnDel;
        private Telerik.WinControls.UI.RadButton btnAdd;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadDropDownList expire;
    }
}
