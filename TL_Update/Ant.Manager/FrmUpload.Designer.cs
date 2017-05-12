namespace Ant.Manager
{
    partial class FrmUpload
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpload));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtIPAddress = new System.Windows.Forms.ToolStripTextBox();
            this.txtPort = new System.Windows.Forms.ToolStripTextBox();
            this.cmdConnection = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdUpload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.cbAppName = new System.Windows.Forms.ToolStripComboBox();
            this.imgFile = new System.Windows.Forms.PictureBox();
            this.imtTotal = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtError = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.vXGJ = new System.Windows.Forms.RadioButton();
            this.vStd = new System.Windows.Forms.RadioButton();
            this.vDZ = new System.Windows.Forms.RadioButton();
            this.cbAutoSuffix = new System.Windows.Forms.CheckBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.lbUnit = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imtTotal)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txtIPAddress,
            this.txtPort,
            this.cmdConnection,
            this.toolStripSeparator1,
            this.cmdUpload,
            this.toolStripSeparator2,
            this.cmdClose,
            this.toolStripLabel3,
            this.cbAppName,
            this.toolStripLabel2,
            this.lbUnit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(587, 31);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(70, 28);
            this.toolStripLabel1.Text = "服务端地址:";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(100, 31);
            this.txtIPAddress.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(50, 31);
            this.txtPort.Text = "9560";
            // 
            // cmdConnection
            // 
            this.cmdConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdConnection.Image = global::Ant.Manager.Properties.Resources._1330346151_stock_connect;
            this.cmdConnection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdConnection.Name = "cmdConnection";
            this.cmdConnection.Size = new System.Drawing.Size(28, 28);
            this.cmdConnection.Text = "连接到服务器";
            this.cmdConnection.Click += new System.EventHandler(this.cmdConnection_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // cmdUpload
            // 
            this.cmdUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdUpload.Enabled = false;
            this.cmdUpload.Image = global::Ant.Manager.Properties.Resources._1330163992_Update;
            this.cmdUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdUpload.Name = "cmdUpload";
            this.cmdUpload.Size = new System.Drawing.Size(28, 28);
            this.cmdUpload.Text = "更新";
            this.cmdUpload.Click += new System.EventHandler(this.cmdUpload_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // cmdClose
            // 
            this.cmdClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdClose.Image = global::Ant.Manager.Properties.Resources._1330161383_exit;
            this.cmdClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(28, 28);
            this.cmdClose.Text = "关闭";
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click_1);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(34, 28);
            this.toolStripLabel3.Text = "程序:";
            // 
            // cbAppName
            // 
            this.cbAppName.Name = "cbAppName";
            this.cbAppName.Size = new System.Drawing.Size(80, 31);
            // 
            // imgFile
            // 
            this.imgFile.Location = new System.Drawing.Point(21, 90);
            this.imgFile.Name = "imgFile";
            this.imgFile.Size = new System.Drawing.Size(547, 24);
            this.imgFile.TabIndex = 1;
            this.imgFile.TabStop = false;
            // 
            // imtTotal
            // 
            this.imtTotal.Location = new System.Drawing.Point(21, 126);
            this.imtTotal.Name = "imtTotal";
            this.imtTotal.Size = new System.Drawing.Size(547, 24);
            this.imtTotal.TabIndex = 0;
            this.imtTotal.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtError
            // 
            this.txtError.AutoSize = true;
            this.txtError.ForeColor = System.Drawing.Color.Red;
            this.txtError.Location = new System.Drawing.Point(12, 102);
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(0, 12);
            this.txtError.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "部署:";
            // 
            // txtUnit
            // 
            this.txtUnit.Location = new System.Drawing.Point(63, 45);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(100, 21);
            this.txtUnit.TabIndex = 5;
            // 
            // vXGJ
            // 
            this.vXGJ.AutoSize = true;
            this.vXGJ.Location = new System.Drawing.Point(85, 2);
            this.vXGJ.Name = "vXGJ";
            this.vXGJ.Size = new System.Drawing.Size(41, 16);
            this.vXGJ.TabIndex = 9;
            this.vXGJ.TabStop = true;
            this.vXGJ.Text = "XGJ";
            this.vXGJ.UseVisualStyleBackColor = true;
            // 
            // vStd
            // 
            this.vStd.AutoSize = true;
            this.vStd.Location = new System.Drawing.Point(132, 2);
            this.vStd.Name = "vStd";
            this.vStd.Size = new System.Drawing.Size(59, 16);
            this.vStd.TabIndex = 10;
            this.vStd.TabStop = true;
            this.vStd.Text = "标准版";
            this.vStd.UseVisualStyleBackColor = true;
            // 
            // vDZ
            // 
            this.vDZ.AutoSize = true;
            this.vDZ.Location = new System.Drawing.Point(197, 2);
            this.vDZ.Name = "vDZ";
            this.vDZ.Size = new System.Drawing.Size(59, 16);
            this.vDZ.TabIndex = 11;
            this.vDZ.TabStop = true;
            this.vDZ.Text = "定制版";
            this.vDZ.UseVisualStyleBackColor = true;
            // 
            // cbAutoSuffix
            // 
            this.cbAutoSuffix.AutoSize = true;
            this.cbAutoSuffix.Location = new System.Drawing.Point(3, 3);
            this.cbAutoSuffix.Name = "cbAutoSuffix";
            this.cbAutoSuffix.Size = new System.Drawing.Size(72, 16);
            this.cbAutoSuffix.TabIndex = 12;
            this.cbAutoSuffix.Text = "自动后缀";
            this.cbAutoSuffix.UseVisualStyleBackColor = true;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(38, 28);
            this.toolStripLabel2.Text = "UNIT:";
            // 
            // lbUnit
            // 
            this.lbUnit.ForeColor = System.Drawing.Color.Red;
            this.lbUnit.Name = "lbUnit";
            this.lbUnit.Size = new System.Drawing.Size(15, 28);
            this.lbUnit.Text = "--";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbAutoSuffix);
            this.panel1.Controls.Add(this.vXGJ);
            this.panel1.Controls.Add(this.vDZ);
            this.panel1.Controls.Add(this.vStd);
            this.panel1.Location = new System.Drawing.Point(184, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 27);
            this.panel1.TabIndex = 13;
            // 
            // FrmUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 162);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.imgFile);
            this.Controls.Add(this.imtTotal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpload";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "更新文件";
            this.Load += new System.EventHandler(this.FrmUpload_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imtTotal)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imtTotal;
        private System.Windows.Forms.PictureBox imgFile;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtIPAddress;
        private System.Windows.Forms.ToolStripButton cmdConnection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton cmdUpload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cmdClose;
        private System.Windows.Forms.ToolStripTextBox txtPort;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label txtError;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox cbAppName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.RadioButton vXGJ;
        private System.Windows.Forms.RadioButton vStd;
        private System.Windows.Forms.RadioButton vDZ;
        private System.Windows.Forms.CheckBox cbAutoSuffix;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel lbUnit;
        private System.Windows.Forms.Panel panel1;
    }
}