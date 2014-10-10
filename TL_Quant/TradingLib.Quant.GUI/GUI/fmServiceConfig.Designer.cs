namespace TradingLib.Quant.GUI
{
    partial class fmServiceConfig
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
            this.paperbroker = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.dell = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.add = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.servicegrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.friendlyname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisdata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.livedata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exuctoin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servicegrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.paperbroker);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.dell);
            this.kryptonPanel1.Controls.Add(this.add);
            this.kryptonPanel1.Controls.Add(this.servicegrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(380, 255);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // paperbroker
            // 
            this.paperbroker.Location = new System.Drawing.Point(104, 230);
            this.paperbroker.Name = "paperbroker";
            this.paperbroker.Size = new System.Drawing.Size(19, 18);
            this.paperbroker.TabIndex = 4;
            this.paperbroker.Values.Text = "--";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 230);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(95, 18);
            this.kryptonLabel1.TabIndex = 3;
            this.kryptonLabel1.Values.Text = "模拟成交服务:";
            // 
            // dell
            // 
            this.dell.Location = new System.Drawing.Point(308, 230);
            this.dell.Name = "dell";
            this.dell.Size = new System.Drawing.Size(69, 22);
            this.dell.TabIndex = 2;
            this.dell.Values.Text = "删除服务";
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(234, 230);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(68, 22);
            this.add.TabIndex = 1;
            this.add.Values.Text = "添加服务";
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // servicegrid
            // 
            this.servicegrid.AllowUserToAddRows = false;
            this.servicegrid.AllowUserToDeleteRows = false;
            this.servicegrid.AllowUserToResizeRows = false;
            this.servicegrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.servicegrid.ColumnHeadersHeight = 22;
            this.servicegrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.servicegrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.friendlyname,
            this.name,
            this.hisdata,
            this.livedata,
            this.exuctoin});
            this.servicegrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.servicegrid.Location = new System.Drawing.Point(0, 0);
            this.servicegrid.MultiSelect = false;
            this.servicegrid.Name = "servicegrid";
            this.servicegrid.ReadOnly = true;
            this.servicegrid.RowHeadersVisible = false;
            this.servicegrid.RowTemplate.Height = 23;
            this.servicegrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.servicegrid.Size = new System.Drawing.Size(380, 224);
            this.servicegrid.TabIndex = 0;
            // 
            // friendlyname
            // 
            this.friendlyname.HeaderText = "服务编号";
            this.friendlyname.Name = "friendlyname";
            this.friendlyname.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "服务名称";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // hisdata
            // 
            this.hisdata.HeaderText = "历史数据";
            this.hisdata.Name = "hisdata";
            this.hisdata.ReadOnly = true;
            // 
            // livedata
            // 
            this.livedata.HeaderText = "实时数据";
            this.livedata.Name = "livedata";
            this.livedata.ReadOnly = true;
            // 
            // exuctoin
            // 
            this.exuctoin.HeaderText = "成交接口";
            this.exuctoin.Name = "exuctoin";
            this.exuctoin.ReadOnly = true;
            // 
            // fmServiceConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 255);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmServiceConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "编辑服务";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servicegrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView servicegrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn friendlyname;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisdata;
        private System.Windows.Forms.DataGridViewTextBoxColumn livedata;
        private System.Windows.Forms.DataGridViewTextBoxColumn exuctoin;
        private ComponentFactory.Krypton.Toolkit.KryptonButton dell;
        private ComponentFactory.Krypton.Toolkit.KryptonButton add;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel paperbroker;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}