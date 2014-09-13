namespace FutsMoniter
{
    partial class SecurityForm
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
            this.secgrid = new Telerik.WinControls.UI.RadGridView();
            this.cbsecurity = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.cbexchange = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.cbtradeable = new Telerik.WinControls.UI.RadDropDownList();
            this.btnAddSecurity = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbsecurity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbexchange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtradeable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddSecurity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // secgrid
            // 
            this.secgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.secgrid.Location = new System.Drawing.Point(0, 32);
            this.secgrid.Name = "secgrid";
            this.secgrid.Size = new System.Drawing.Size(1084, 465);
            this.secgrid.TabIndex = 2;
            this.secgrid.Text = "radGridView1";
            this.secgrid.DoubleClick += new System.EventHandler(this.secgrid_DoubleClick);
            // 
            // cbsecurity
            // 
            this.cbsecurity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbsecurity.Location = new System.Drawing.Point(65, 8);
            this.cbsecurity.Name = "cbsecurity";
            this.cbsecurity.Size = new System.Drawing.Size(94, 18);
            this.cbsecurity.TabIndex = 24;
            this.cbsecurity.Text = "--";
            this.cbsecurity.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbsecurity_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbsecurity.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(0, 8);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(59, 16);
            this.radLabel5.TabIndex = 25;
            this.radLabel5.Text = "品种类别:";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(170, 8);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(47, 16);
            this.radLabel1.TabIndex = 27;
            this.radLabel1.Text = "交易所:";
            // 
            // cbexchange
            // 
            this.cbexchange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbexchange.Location = new System.Drawing.Point(235, 8);
            this.cbexchange.Name = "cbexchange";
            this.cbexchange.Size = new System.Drawing.Size(163, 18);
            this.cbexchange.TabIndex = 26;
            this.cbexchange.Text = "--";
            this.cbexchange.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbexchange_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbexchange.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(404, 8);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(59, 16);
            this.radLabel2.TabIndex = 33;
            this.radLabel2.Text = "交易标识:";
            // 
            // cbtradeable
            // 
            this.cbtradeable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbtradeable.Location = new System.Drawing.Point(469, 8);
            this.cbtradeable.Name = "cbtradeable";
            this.cbtradeable.Size = new System.Drawing.Size(86, 18);
            this.cbtradeable.TabIndex = 32;
            this.cbtradeable.Text = "--";
            this.cbtradeable.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbtradeable_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbtradeable.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // btnAddSecurity
            // 
            this.btnAddSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSecurity.Location = new System.Drawing.Point(974, 2);
            this.btnAddSecurity.Name = "btnAddSecurity";
            this.btnAddSecurity.Size = new System.Drawing.Size(110, 24);
            this.btnAddSecurity.TabIndex = 34;
            this.btnAddSecurity.Text = "添加品种";
            this.btnAddSecurity.Click += new System.EventHandler(this.btnAddSecurity_Click);
            // 
            // SecurityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 497);
            this.Controls.Add(this.btnAddSecurity);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.cbtradeable);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.cbexchange);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.cbsecurity);
            this.Controls.Add(this.secgrid);
            this.Name = "SecurityForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "品种列表";
            this.ThemeName = "ControlDefault";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SecurityForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.secgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbsecurity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbexchange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtradeable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddSecurity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView secgrid;
        private Telerik.WinControls.UI.RadDropDownList cbsecurity;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDropDownList cbexchange;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDropDownList cbtradeable;
        private Telerik.WinControls.UI.RadButton btnAddSecurity;
    }
}
