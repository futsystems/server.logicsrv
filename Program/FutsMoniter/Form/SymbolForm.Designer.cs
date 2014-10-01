namespace FutsMoniter
{
    partial class SymbolForm
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
            this.symgrid = new Telerik.WinControls.UI.RadGridView();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.cbsecurity = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.cbexchange = new Telerik.WinControls.UI.RadDropDownList();
            this.cbtradeable = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.btnAddSymbol = new Telerik.WinControls.UI.RadButton();
            this.btnSyncSymbols = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.symgrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbsecurity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbexchange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtradeable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddSymbol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSyncSymbols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // symgrid
            // 
            this.symgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.symgrid.Location = new System.Drawing.Point(0, 31);
            this.symgrid.Name = "symgrid";
            this.symgrid.Size = new System.Drawing.Size(919, 357);
            this.symgrid.TabIndex = 3;
            this.symgrid.Text = "radGridView1";
            this.symgrid.DoubleClick += new System.EventHandler(this.symgrid_DoubleClick);
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(0, 9);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(59, 16);
            this.radLabel5.TabIndex = 27;
            this.radLabel5.Text = "品种类别:";
            // 
            // cbsecurity
            // 
            this.cbsecurity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbsecurity.Location = new System.Drawing.Point(65, 9);
            this.cbsecurity.Name = "cbsecurity";
            this.cbsecurity.Size = new System.Drawing.Size(94, 18);
            this.cbsecurity.TabIndex = 26;
            this.cbsecurity.Text = "--";
            this.cbsecurity.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbsecurity_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbsecurity.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(165, 9);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(47, 16);
            this.radLabel1.TabIndex = 29;
            this.radLabel1.Text = "交易所:";
            // 
            // cbexchange
            // 
            this.cbexchange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbexchange.Location = new System.Drawing.Point(230, 9);
            this.cbexchange.Name = "cbexchange";
            this.cbexchange.Size = new System.Drawing.Size(163, 18);
            this.cbexchange.TabIndex = 28;
            this.cbexchange.Text = "--";
            this.cbexchange.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbexchange_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbexchange.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // cbtradeable
            // 
            this.cbtradeable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbtradeable.Location = new System.Drawing.Point(464, 9);
            this.cbtradeable.Name = "cbtradeable";
            this.cbtradeable.Size = new System.Drawing.Size(86, 18);
            this.cbtradeable.TabIndex = 30;
            this.cbtradeable.Text = "--";
            this.cbtradeable.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cbtradeable_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbtradeable.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(399, 9);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(59, 16);
            this.radLabel2.TabIndex = 31;
            this.radLabel2.Text = "交易标识:";
            // 
            // btnAddSymbol
            // 
            this.btnAddSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSymbol.Location = new System.Drawing.Point(809, 3);
            this.btnAddSymbol.Name = "btnAddSymbol";
            this.btnAddSymbol.Size = new System.Drawing.Size(110, 24);
            this.btnAddSymbol.TabIndex = 35;
            this.btnAddSymbol.Text = "手工添加合约";
            this.btnAddSymbol.Click += new System.EventHandler(this.btnAddSymbol_Click);
            // 
            // btnSyncSymbols
            // 
            this.btnSyncSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncSymbols.Location = new System.Drawing.Point(680, 3);
            this.btnSyncSymbols.Name = "btnSyncSymbols";
            this.btnSyncSymbols.Size = new System.Drawing.Size(110, 24);
            this.btnSyncSymbols.TabIndex = 36;
            this.btnSyncSymbols.Text = "同步CTP期货合约";
            this.btnSyncSymbols.Click += new System.EventHandler(this.btnSyncSymbols_Click);
            // 
            // SymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 388);
            this.Controls.Add(this.btnSyncSymbols);
            this.Controls.Add(this.btnAddSymbol);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.cbtradeable);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.cbexchange);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.cbsecurity);
            this.Controls.Add(this.symgrid);
            this.Name = "SymbolForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "合约列表";
            this.ThemeName = "ControlDefault";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SymbolForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.symgrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbsecurity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbexchange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtradeable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddSymbol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSyncSymbols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView symgrid;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadDropDownList cbsecurity;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDropDownList cbexchange;
        private Telerik.WinControls.UI.RadDropDownList cbtradeable;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton btnAddSymbol;
        private Telerik.WinControls.UI.RadButton btnSyncSymbols;
    }
}
