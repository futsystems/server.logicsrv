namespace TradingLib.Quant.GUI
{
    partial class fmAddNewService
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
            this.srvgrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.servicename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.author = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisdata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.livedata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.execution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.serviceFriendlyName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.next = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srvgrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.next);
            this.kryptonPanel1.Controls.Add(this.serviceFriendlyName);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.srvgrid);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(407, 320);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // srvgrid
            // 
            this.srvgrid.AllowUserToAddRows = false;
            this.srvgrid.AllowUserToDeleteRows = false;
            this.srvgrid.AllowUserToOrderColumns = true;
            this.srvgrid.AllowUserToResizeColumns = false;
            this.srvgrid.AllowUserToResizeRows = false;
            this.srvgrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.srvgrid.ColumnHeadersHeight = 22;
            this.srvgrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.servicename,
            this.author,
            this.hisdata,
            this.livedata,
            this.execution});
            this.srvgrid.Location = new System.Drawing.Point(0, 59);
            this.srvgrid.Name = "srvgrid";
            this.srvgrid.RowHeadersVisible = false;
            this.srvgrid.RowHeadersWidth = 22;
            this.srvgrid.RowTemplate.Height = 23;
            this.srvgrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.srvgrid.Size = new System.Drawing.Size(407, 227);
            this.srvgrid.TabIndex = 0;
            // 
            // servicename
            // 
            this.servicename.HeaderText = "服务名称";
            this.servicename.Name = "servicename";
            // 
            // author
            // 
            this.author.HeaderText = "作者";
            this.author.Name = "author";
            // 
            // hisdata
            // 
            this.hisdata.HeaderText = "历史数据";
            this.hisdata.Name = "hisdata";
            // 
            // livedata
            // 
            this.livedata.HeaderText = "实盘数据";
            this.livedata.Name = "livedata";
            // 
            // execution
            // 
            this.execution.HeaderText = "成交接口";
            this.execution.Name = "execution";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 35);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(92, 18);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "可用服务列表";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(92, 18);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "输入服务别名";
            // 
            // serviceFriendlyName
            // 
            this.serviceFriendlyName.Location = new System.Drawing.Point(101, 3);
            this.serviceFriendlyName.Name = "serviceFriendlyName";
            this.serviceFriendlyName.Size = new System.Drawing.Size(131, 21);
            this.serviceFriendlyName.TabIndex = 3;
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(323, 292);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(81, 22);
            this.next.TabIndex = 4;
            this.next.Values.Text = "下一步";
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // fmAddNewService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 320);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "fmAddNewService";
            this.Text = "添加服务";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srvgrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView srvgrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn servicename;
        private System.Windows.Forms.DataGridViewTextBoxColumn author;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisdata;
        private System.Windows.Forms.DataGridViewTextBoxColumn livedata;
        private System.Windows.Forms.DataGridViewTextBoxColumn execution;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox serviceFriendlyName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton next;
    }
}