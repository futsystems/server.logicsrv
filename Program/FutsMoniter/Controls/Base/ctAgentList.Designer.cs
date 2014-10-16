namespace FutsMoniter
{
    partial class ctAgentList
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.agent = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            ((System.ComponentModel.ISupportInitialize)(this.agent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // agent
            // 
            this.agent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.agent.Location = new System.Drawing.Point(50, 0);
            this.agent.Name = "agent";
            this.agent.Size = new System.Drawing.Size(109, 18);
            this.agent.TabIndex = 48;
            this.agent.Text = "--";
            this.agent.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.agent_SelectedIndexChanged);
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.agent.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Window;
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(0, 2);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(47, 16);
            this.radLabel5.TabIndex = 47;
            this.radLabel5.Text = "代理商:";
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radLabel5);
            this.radPanel1.Controls.Add(this.agent);
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(162, 20);
            this.radPanel1.TabIndex = 49;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.radPanel1.GetChildAt(0).GetChildAt(1))).Width = 0F;
            // 
            // ctAgentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Name = "ctAgentList";
            this.Size = new System.Drawing.Size(162, 20);
            ((System.ComponentModel.ISupportInitialize)(this.agent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadDropDownList agent;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadPanel radPanel1;
    }
}
