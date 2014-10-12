namespace FutsMoniter
{
    partial class CashOperationForm
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.lbmgrfk = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            this.cashoptype = new Telerik.WinControls.UI.RadDropDownList();
            this.amount = new Telerik.WinControls.UI.RadSpinEditor();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbmgrfk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashoptype)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(25, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(70, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "管理主域ID:";
            // 
            // lbmgrfk
            // 
            this.lbmgrfk.Location = new System.Drawing.Point(101, 13);
            this.lbmgrfk.Name = "lbmgrfk";
            this.lbmgrfk.Size = new System.Drawing.Size(14, 16);
            this.lbmgrfk.TabIndex = 1;
            this.lbmgrfk.Text = "--";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(36, 37);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 2;
            this.radLabel3.Text = "请求类型:";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(61, 62);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(34, 16);
            this.radLabel4.TabIndex = 3;
            this.radLabel4.Text = "金额:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(127, 104);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(68, 24);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // cashoptype
            // 
            this.cashoptype.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cashoptype.Location = new System.Drawing.Point(101, 35);
            this.cashoptype.Name = "cashoptype";
            this.cashoptype.Size = new System.Drawing.Size(94, 18);
            this.cashoptype.TabIndex = 17;
            this.cashoptype.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cashoptype.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // amount
            // 
            this.amount.ForeColor = System.Drawing.Color.Black;
            this.amount.Location = new System.Drawing.Point(101, 59);
            this.amount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.amount.Name = "amount";
            this.amount.Size = new System.Drawing.Size(94, 19);
            this.amount.TabIndex = 18;
            this.amount.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.amount.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Text = "0";
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.amount.GetChildAt(0).GetChildAt(2).GetChildAt(0))).ForeColor = System.Drawing.Color.Black;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.amount.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // CashOperationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 137);
            this.Controls.Add(this.amount);
            this.Controls.Add(this.cashoptype);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.lbmgrfk);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CashOperationForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "提现或充值";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbmgrfk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashoptype)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel lbmgrfk;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadButton btnSubmit;
        private Telerik.WinControls.UI.RadDropDownList cashoptype;
        private Telerik.WinControls.UI.RadSpinEditor amount;
    }
}