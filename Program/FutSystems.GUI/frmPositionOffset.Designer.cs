namespace FutSystems.GUI
{
    partial class frmPositionOffset
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
            this.roundRectShape1 = new Telerik.WinControls.RoundRectShape(this.components);
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.directionLabel = new Telerik.WinControls.UI.RadLabel();
            this.btnUpdateOffset = new Telerik.WinControls.UI.RadButton();
            this.btnSwitchOffset = new Telerik.WinControls.UI.RadButton();
            this.switchLabel = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.start = new Telerik.WinControls.UI.RadSpinEditor();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.size = new Telerik.WinControls.UI.RadSpinEditor();
            this.sizelabel = new Telerik.WinControls.UI.RadLabel();
            this.typelist = new Telerik.WinControls.UI.RadDropDownList();
            this.value = new Telerik.WinControls.UI.RadSpinEditor();
            this.arglabel = new Telerik.WinControls.UI.RadLabel();
            this.typelabel = new Telerik.WinControls.UI.RadLabel();
            this.symbol = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.directionLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpdateOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSwitchOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.switchLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.start)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizelabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.typelist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arglabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.typelabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.radPanel1.Controls.Add(this.directionLabel);
            this.radPanel1.Controls.Add(this.btnUpdateOffset);
            this.radPanel1.Controls.Add(this.btnSwitchOffset);
            this.radPanel1.Controls.Add(this.switchLabel);
            this.radPanel1.Controls.Add(this.radLabel2);
            this.radPanel1.Controls.Add(this.start);
            this.radPanel1.Controls.Add(this.radLabel5);
            this.radPanel1.Controls.Add(this.size);
            this.radPanel1.Controls.Add(this.sizelabel);
            this.radPanel1.Controls.Add(this.typelist);
            this.radPanel1.Controls.Add(this.value);
            this.radPanel1.Controls.Add(this.arglabel);
            this.radPanel1.Controls.Add(this.typelabel);
            this.radPanel1.Controls.Add(this.symbol);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(163, 169);
            this.radPanel1.TabIndex = 0;
            // 
            // directionLabel
            // 
            this.directionLabel.Location = new System.Drawing.Point(106, 3);
            this.directionLabel.Name = "directionLabel";
            this.directionLabel.Size = new System.Drawing.Size(14, 16);
            this.directionLabel.TabIndex = 22;
            this.directionLabel.Text = "--";
            // 
            // btnUpdateOffset
            // 
            this.btnUpdateOffset.Location = new System.Drawing.Point(106, 135);
            this.btnUpdateOffset.Name = "btnUpdateOffset";
            this.btnUpdateOffset.Size = new System.Drawing.Size(47, 24);
            this.btnUpdateOffset.TabIndex = 21;
            this.btnUpdateOffset.Text = "更新";
            this.btnUpdateOffset.Click += new System.EventHandler(this.btnUpdateOffset_Click);
            // 
            // btnSwitchOffset
            // 
            this.btnSwitchOffset.Location = new System.Drawing.Point(52, 135);
            this.btnSwitchOffset.Name = "btnSwitchOffset";
            this.btnSwitchOffset.Size = new System.Drawing.Size(48, 24);
            this.btnSwitchOffset.TabIndex = 20;
            this.btnSwitchOffset.Text = "启用";
            this.btnSwitchOffset.Click += new System.EventHandler(this.btnSwitchOffset_Click);
            // 
            // switchLabel
            // 
            this.switchLabel.Location = new System.Drawing.Point(77, 113);
            this.switchLabel.Name = "switchLabel";
            this.switchLabel.Size = new System.Drawing.Size(14, 16);
            this.switchLabel.TabIndex = 19;
            this.switchLabel.Text = "--";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(12, 113);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 18;
            this.radLabel2.Text = "状态:";
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(77, 91);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(76, 19);
            this.start.TabIndex = 17;
            this.start.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.start.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(12, 91);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(59, 16);
            this.radLabel5.TabIndex = 16;
            this.radLabel5.Text = "跟踪起点:";
            // 
            // size
            // 
            this.size.Location = new System.Drawing.Point(77, 69);
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(76, 19);
            this.size.TabIndex = 15;
            this.size.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.size.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // sizelabel
            // 
            this.sizelabel.Location = new System.Drawing.Point(12, 69);
            this.sizelabel.Name = "sizelabel";
            this.sizelabel.Size = new System.Drawing.Size(59, 16);
            this.sizelabel.TabIndex = 14;
            this.sizelabel.Text = "止损手数:";
            // 
            // typelist
            // 
            this.typelist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.typelist.Location = new System.Drawing.Point(77, 23);
            this.typelist.Name = "typelist";
            this.typelist.Size = new System.Drawing.Size(76, 18);
            this.typelist.TabIndex = 13;
            this.typelist.Text = "--";
            this.typelist.SelectedValueChanged += new System.EventHandler(this.typelist_SelectedValueChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.typelist.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // value
            // 
            this.value.Location = new System.Drawing.Point(77, 47);
            this.value.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(76, 19);
            this.value.TabIndex = 10;
            this.value.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.value.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Text = "0";
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.value.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // arglabel
            // 
            this.arglabel.Location = new System.Drawing.Point(12, 47);
            this.arglabel.Name = "arglabel";
            this.arglabel.Size = new System.Drawing.Size(34, 16);
            this.arglabel.TabIndex = 3;
            this.arglabel.Text = "参数:";
            // 
            // typelabel
            // 
            this.typelabel.Location = new System.Drawing.Point(12, 25);
            this.typelabel.Name = "typelabel";
            this.typelabel.Size = new System.Drawing.Size(59, 16);
            this.typelabel.TabIndex = 2;
            this.typelabel.Text = "止损类别:";
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(52, 3);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(14, 16);
            this.symbol.TabIndex = 1;
            this.symbol.Text = "--";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 3);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(34, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "合约:";
            // 
            // frmPositionOffset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(163, 169);
            this.Controls.Add(this.radPanel1);
            this.Name = "frmPositionOffset";
            this.Shape = this.roundRectShape1;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmPositionOffset";
            this.Deactivate += new System.EventHandler(this.frmPositionOffset_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPositionOffset_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.directionLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpdateOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSwitchOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.switchLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.start)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizelabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.typelist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arglabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.typelabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.RoundRectShape roundRectShape1;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadLabel symbol;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel arglabel;
        private Telerik.WinControls.UI.RadLabel typelabel;
        private Telerik.WinControls.UI.RadSpinEditor value;
        private Telerik.WinControls.UI.RadSpinEditor start;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadSpinEditor size;
        private Telerik.WinControls.UI.RadLabel sizelabel;
        private Telerik.WinControls.UI.RadDropDownList typelist;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel switchLabel;
        private Telerik.WinControls.UI.RadButton btnUpdateOffset;
        private Telerik.WinControls.UI.RadButton btnSwitchOffset;
        private Telerik.WinControls.UI.RadLabel directionLabel;


    }
}
