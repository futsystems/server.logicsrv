namespace FutsMoniter
{
    partial class ServicePlanChangeForm
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
            this.lbaccount = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.lbcurrentsp = new Telerik.WinControls.UI.RadLabel();
            this.cbServicePlan = new Telerik.WinControls.UI.RadDropDownList();
            this.btnSubmit = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbaccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcurrentsp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbServicePlan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(12, 12);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(59, 16);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "交易帐号:";
            // 
            // lbaccount
            // 
            this.lbaccount.Location = new System.Drawing.Point(77, 12);
            this.lbaccount.Name = "lbaccount";
            this.lbaccount.Size = new System.Drawing.Size(14, 16);
            this.lbaccount.TabIndex = 1;
            this.lbaccount.Text = "--";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(12, 58);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(59, 16);
            this.radLabel2.TabIndex = 2;
            this.radLabel2.Text = "服务计划:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(12, 34);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(59, 16);
            this.radLabel3.TabIndex = 3;
            this.radLabel3.Text = "当前计划:";
            // 
            // lbcurrentsp
            // 
            this.lbcurrentsp.Location = new System.Drawing.Point(77, 34);
            this.lbcurrentsp.Name = "lbcurrentsp";
            this.lbcurrentsp.Size = new System.Drawing.Size(14, 16);
            this.lbcurrentsp.TabIndex = 4;
            this.lbcurrentsp.Text = "--";
            // 
            // cbServicePlan
            // 
            this.cbServicePlan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbServicePlan.Location = new System.Drawing.Point(77, 56);
            this.cbServicePlan.Name = "cbServicePlan";
            this.cbServicePlan.Size = new System.Drawing.Size(94, 18);
            this.cbServicePlan.TabIndex = 27;
            this.cbServicePlan.Text = "--";
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.cbServicePlan.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(142, 111);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(87, 24);
            this.btnSubmit.TabIndex = 28;
            this.btnSubmit.Text = "提 交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // ServicePlanChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 142);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.cbServicePlan);
            this.Controls.Add(this.lbcurrentsp);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.lbaccount);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ServicePlanChangeForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改服务计划";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbaccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcurrentsp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbServicePlan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubmit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel lbaccount;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel lbcurrentsp;
        private Telerik.WinControls.UI.RadDropDownList cbServicePlan;
        private Telerik.WinControls.UI.RadButton btnSubmit;
    }
}