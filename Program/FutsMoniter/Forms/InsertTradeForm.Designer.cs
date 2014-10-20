namespace FutsMoniter
{
    partial class InsertTradeForm
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
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.price = new Telerik.WinControls.UI.RadSpinEditor();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            this.symbol = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.timestr = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.cbside = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.size = new Telerik.WinControls.UI.RadSpinEditor();
            this.cboffsetflag = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel8 = new Telerik.WinControls.UI.RadLabel();
            this.account = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.price)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timestr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbside)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(12, 131);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(59, 16);
            this.radLabel4.TabIndex = 3;
            this.radLabel4.Text = "成交价格:";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(38, 106);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(34, 16);
            this.radLabel5.TabIndex = 4;
            this.radLabel5.Text = "合约:";
            // 
            // price
            // 
            this.price.ForeColor = System.Drawing.Color.Black;
            this.price.Location = new System.Drawing.Point(78, 128);
            this.price.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.price.Name = "price";
            this.price.Size = new System.Drawing.Size(87, 19);
            this.price.TabIndex = 9;
            this.price.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.price.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Text = "0";
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.price.GetChildAt(0).GetChildAt(2).GetChildAt(0))).ForeColor = System.Drawing.Color.Black;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.price.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(171, 175);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(77, 24);
            this.btnSubmit.TabIndex = 10;
            this.btnSubmit.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(78, 104);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(87, 18);
            this.symbol.TabIndex = 14;
            // 
            // radLabel7
            // 
            this.radLabel7.Location = new System.Drawing.Point(171, 35);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(82, 16);
            this.radLabel7.TabIndex = 14;
            this.radLabel7.Text = "格式(时:分:秒)";
            // 
            // timestr
            // 
            this.timestr.Location = new System.Drawing.Point(78, 33);
            this.timestr.Name = "timestr";
            this.timestr.Size = new System.Drawing.Size(87, 18);
            this.timestr.TabIndex = 12;
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(12, 35);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 13;
            this.radLabel3.Text = "提交时间:";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(37, 59);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(34, 16);
            this.radLabel1.TabIndex = 15;
            this.radLabel1.Text = "买卖:";
            // 
            // cbside
            // 
            this.cbside.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbside.Location = new System.Drawing.Point(77, 57);
            this.cbside.Name = "cbside";
            this.cbside.Size = new System.Drawing.Size(88, 18);
            this.cbside.TabIndex = 16;
            this.cbside.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbside.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(37, 82);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(34, 16);
            this.radLabel2.TabIndex = 17;
            this.radLabel2.Text = "手数:";
            // 
            // size
            // 
            this.size.ForeColor = System.Drawing.Color.Black;
            this.size.Location = new System.Drawing.Point(78, 79);
            this.size.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(87, 19);
            this.size.TabIndex = 18;
            this.size.TabStop = false;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.size.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Text = "0";
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.size.GetChildAt(0).GetChildAt(2).GetChildAt(0))).ForeColor = System.Drawing.Color.Black;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.size.GetChildAt(0).GetChildAt(2).GetChildAt(0))).Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // cboffsetflag
            // 
            this.cboffsetflag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cboffsetflag.Location = new System.Drawing.Point(77, 151);
            this.cboffsetflag.Name = "cboffsetflag";
            this.cboffsetflag.Size = new System.Drawing.Size(88, 18);
            this.cboffsetflag.TabIndex = 20;
            this.cboffsetflag.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cboffsetflag.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel6
            // 
            this.radLabel6.Location = new System.Drawing.Point(37, 153);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(34, 16);
            this.radLabel6.TabIndex = 19;
            this.radLabel6.Text = "开平:";
            // 
            // radLabel8
            // 
            this.radLabel8.Location = new System.Drawing.Point(12, 12);
            this.radLabel8.Name = "radLabel8";
            this.radLabel8.Size = new System.Drawing.Size(59, 16);
            this.radLabel8.TabIndex = 21;
            this.radLabel8.Text = "交易帐户:";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(78, 11);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(14, 16);
            this.account.TabIndex = 22;
            this.account.Text = "--";
            // 
            // InsertTradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 206);
            this.Controls.Add(this.account);
            this.Controls.Add(this.radLabel8);
            this.Controls.Add(this.cboffsetflag);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.size);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.cbside);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radLabel7);
            this.Controls.Add(this.symbol);
            this.Controls.Add(this.timestr);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.price);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.radLabel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsertTradeForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插入成交";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.price)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timestr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbside)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboffsetflag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.account)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadSpinEditor price;
        private Telerik.WinControls.UI.RadButton btnSubmit;
        private Telerik.WinControls.UI.RadTextBox symbol;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadTextBox timestr;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDropDownList cbside;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadSpinEditor size;
        private Telerik.WinControls.UI.RadDropDownList cboffsetflag;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel radLabel8;
        private Telerik.WinControls.UI.RadLabel account;
    }
}