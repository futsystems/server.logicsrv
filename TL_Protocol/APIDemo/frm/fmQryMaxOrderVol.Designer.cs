namespace APIClient.frm
{
    partial class fmQryMaxOrderVol
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
            this.label1 = new System.Windows.Forms.Label();
            this.symbol = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.direction = new System.Windows.Forms.ComboBox();
            this.offset = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "合约";
            // 
            // symbol
            // 
            this.symbol.Location = new System.Drawing.Point(78, 15);
            this.symbol.Name = "symbol";
            this.symbol.Size = new System.Drawing.Size(100, 21);
            this.symbol.TabIndex = 1;
            this.symbol.Text = "HSIZ6";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "方向";
            // 
            // direction
            // 
            this.direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.direction.FormattingEnabled = true;
            this.direction.Location = new System.Drawing.Point(79, 43);
            this.direction.Name = "direction";
            this.direction.Size = new System.Drawing.Size(99, 20);
            this.direction.TabIndex = 3;
            // 
            // offset
            // 
            this.offset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.offset.FormattingEnabled = true;
            this.offset.Location = new System.Drawing.Point(78, 69);
            this.offset.Name = "offset";
            this.offset.Size = new System.Drawing.Size(99, 20);
            this.offset.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "开平";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(103, 112);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "查询";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // fmQryMaxOrderVol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 156);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.offset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.direction);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.symbol);
            this.Controls.Add(this.label1);
            this.Name = "fmQryMaxOrderVol";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "fmQryMaxOrderVol";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox symbol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox direction;
        private System.Windows.Forms.ComboBox offset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSubmit;
    }
}