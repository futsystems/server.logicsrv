namespace FutsMoniter
{
    partial class ctQuoteMoniter
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
            this.quotenav = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.quote_cffex = new FutSystems.GUI.ViewQuoteList();
            this.page2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.quote_dce = new FutSystems.GUI.ViewQuoteList();
            this.page3 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.quote_czce = new FutSystems.GUI.ViewQuoteList();
            this.kryptonPage4 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.quote_shfe = new FutSystems.GUI.ViewQuoteList();
            this.ctOrderSenderM1 = new FutsMoniter.ctOrderSenderM();
            this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.quote_innov = new FutSystems.GUI.ViewQuoteList();
            ((System.ComponentModel.ISupportInitialize)(this.quotenav)).BeginInit();
            this.quotenav.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
            this.kryptonPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.page2)).BeginInit();
            this.page2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.page3)).BeginInit();
            this.page3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage4)).BeginInit();
            this.kryptonPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
            this.kryptonPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // quotenav
            // 
            this.quotenav.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.quotenav.Bar.TabStyle = ComponentFactory.Krypton.Toolkit.TabStyle.LowProfile;
            this.quotenav.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.quotenav.Button.ContextButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.quotenav.Location = new System.Drawing.Point(0, 0);
            this.quotenav.Name = "quotenav";
            this.quotenav.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.page2,
            this.page3,
            this.kryptonPage4,
            this.kryptonPage2});
            this.quotenav.SelectedIndex = 4;
            this.quotenav.Size = new System.Drawing.Size(634, 298);
            this.quotenav.TabIndex = 0;
            this.quotenav.Text = "kryptonNavigator1";
            // 
            // kryptonPage1
            // 
            this.kryptonPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage1.Controls.Add(this.quote_cffex);
            this.kryptonPage1.Flags = 65534;
            this.kryptonPage1.LastVisibleSet = true;
            this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage1.Name = "kryptonPage1";
            this.kryptonPage1.Size = new System.Drawing.Size(632, 273);
            this.kryptonPage1.Text = "中金所";
            this.kryptonPage1.ToolTipTitle = "Page ToolTip";
            this.kryptonPage1.UniqueName = "753A6023F7944749C39663B5E9CBB80C";
            // 
            // quote_cffex
            // 
            this.quote_cffex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_cffex.DNColor = System.Drawing.Color.Green;
            this.quote_cffex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quote_cffex.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_cffex.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.quote_cffex.HeaderFontColor = System.Drawing.Color.Turquoise;
            this.quote_cffex.Location = new System.Drawing.Point(0, 0);
            this.quote_cffex.MenuEnable = false;
            this.quote_cffex.Name = "quote_cffex";
            this.quote_cffex.QuoteBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_cffex.QuoteBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_cffex.QuoteFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_cffex.QuoteViewWidth = 1030;
            this.quote_cffex.SelectedQuoteRow = -1;
            this.quote_cffex.Size = new System.Drawing.Size(632, 273);
            this.quote_cffex.SymbolFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_cffex.SymbolFontColor = System.Drawing.Color.Green;
            this.quote_cffex.TabIndex = 2;
            this.quote_cffex.TableLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_cffex.Text = "viewQuoteList4";
            this.quote_cffex.UPColor = System.Drawing.Color.Red;
            // 
            // page2
            // 
            this.page2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.page2.Controls.Add(this.quote_dce);
            this.page2.Flags = 65534;
            this.page2.LastVisibleSet = true;
            this.page2.MinimumSize = new System.Drawing.Size(50, 50);
            this.page2.Name = "page2";
            this.page2.Size = new System.Drawing.Size(632, 330);
            this.page2.Text = "大商所";
            this.page2.ToolTipTitle = "Page ToolTip";
            this.page2.UniqueName = "A779D095D3EE4F7EF8AD0BF1FCEFE7A2";
            // 
            // quote_dce
            // 
            this.quote_dce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_dce.DNColor = System.Drawing.Color.Green;
            this.quote_dce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quote_dce.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_dce.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.quote_dce.HeaderFontColor = System.Drawing.Color.Turquoise;
            this.quote_dce.Location = new System.Drawing.Point(0, 0);
            this.quote_dce.MenuEnable = false;
            this.quote_dce.Name = "quote_dce";
            this.quote_dce.QuoteBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_dce.QuoteBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_dce.QuoteFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_dce.QuoteViewWidth = 1030;
            this.quote_dce.SelectedQuoteRow = -1;
            this.quote_dce.Size = new System.Drawing.Size(632, 330);
            this.quote_dce.SymbolFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_dce.SymbolFontColor = System.Drawing.Color.Green;
            this.quote_dce.TabIndex = 2;
            this.quote_dce.TableLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_dce.Text = "viewQuoteList3";
            this.quote_dce.UPColor = System.Drawing.Color.Red;
            // 
            // page3
            // 
            this.page3.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.page3.Controls.Add(this.quote_czce);
            this.page3.Flags = 65534;
            this.page3.LastVisibleSet = true;
            this.page3.MinimumSize = new System.Drawing.Size(50, 50);
            this.page3.Name = "page3";
            this.page3.Size = new System.Drawing.Size(632, 273);
            this.page3.Text = "郑商所";
            this.page3.ToolTipTitle = "Page ToolTip";
            this.page3.UniqueName = "768D1AAE0C354F6F78B8CBDFA739720B";
            // 
            // quote_czce
            // 
            this.quote_czce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_czce.DNColor = System.Drawing.Color.Green;
            this.quote_czce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quote_czce.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_czce.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.quote_czce.HeaderFontColor = System.Drawing.Color.Turquoise;
            this.quote_czce.Location = new System.Drawing.Point(0, 0);
            this.quote_czce.MenuEnable = false;
            this.quote_czce.Name = "quote_czce";
            this.quote_czce.QuoteBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_czce.QuoteBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_czce.QuoteFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_czce.QuoteViewWidth = 1030;
            this.quote_czce.SelectedQuoteRow = -1;
            this.quote_czce.Size = new System.Drawing.Size(632, 273);
            this.quote_czce.SymbolFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_czce.SymbolFontColor = System.Drawing.Color.Green;
            this.quote_czce.TabIndex = 2;
            this.quote_czce.TableLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_czce.Text = "viewQuoteList2";
            this.quote_czce.UPColor = System.Drawing.Color.Red;
            // 
            // kryptonPage4
            // 
            this.kryptonPage4.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage4.Controls.Add(this.quote_shfe);
            this.kryptonPage4.Flags = 65534;
            this.kryptonPage4.LastVisibleSet = true;
            this.kryptonPage4.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage4.Name = "kryptonPage4";
            this.kryptonPage4.Size = new System.Drawing.Size(632, 273);
            this.kryptonPage4.Text = "上期所";
            this.kryptonPage4.ToolTipTitle = "Page ToolTip";
            this.kryptonPage4.UniqueName = "7C9669E362BF4CD0E29A3058CF3869C0";
            // 
            // quote_shfe
            // 
            this.quote_shfe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_shfe.DNColor = System.Drawing.Color.Green;
            this.quote_shfe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quote_shfe.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_shfe.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.quote_shfe.HeaderFontColor = System.Drawing.Color.Turquoise;
            this.quote_shfe.Location = new System.Drawing.Point(0, 0);
            this.quote_shfe.MenuEnable = false;
            this.quote_shfe.Name = "quote_shfe";
            this.quote_shfe.QuoteBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_shfe.QuoteBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_shfe.QuoteFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_shfe.QuoteViewWidth = 1030;
            this.quote_shfe.SelectedQuoteRow = -1;
            this.quote_shfe.Size = new System.Drawing.Size(632, 273);
            this.quote_shfe.SymbolFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_shfe.SymbolFontColor = System.Drawing.Color.Green;
            this.quote_shfe.TabIndex = 2;
            this.quote_shfe.TableLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_shfe.Text = "viewQuoteList1";
            this.quote_shfe.UPColor = System.Drawing.Color.Red;
            // 
            // ctOrderSenderM1
            // 
            this.ctOrderSenderM1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ctOrderSenderM1.Location = new System.Drawing.Point(0, 298);
            this.ctOrderSenderM1.Name = "ctOrderSenderM1";
            this.ctOrderSenderM1.Size = new System.Drawing.Size(634, 57);
            this.ctOrderSenderM1.TabIndex = 1;
            // 
            // kryptonPage2
            // 
            this.kryptonPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage2.Controls.Add(this.quote_innov);
            this.kryptonPage2.Flags = 65534;
            this.kryptonPage2.LastVisibleSet = true;
            this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage2.Name = "kryptonPage2";
            this.kryptonPage2.Size = new System.Drawing.Size(632, 273);
            this.kryptonPage2.Text = "kryptonPage2";
            this.kryptonPage2.ToolTipTitle = "Page ToolTip";
            this.kryptonPage2.UniqueName = "6B72CA2ECCB3472E5881088932069857";
            // 
            // quote_innov
            // 
            this.quote_innov.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_innov.DNColor = System.Drawing.Color.Green;
            this.quote_innov.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quote_innov.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_innov.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.quote_innov.HeaderFontColor = System.Drawing.Color.Turquoise;
            this.quote_innov.Location = new System.Drawing.Point(0, 0);
            this.quote_innov.MenuEnable = false;
            this.quote_innov.Name = "quote_innov";
            this.quote_innov.QuoteBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_innov.QuoteBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_innov.QuoteFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_innov.QuoteViewWidth = 1030;
            this.quote_innov.SelectedQuoteRow = -1;
            this.quote_innov.Size = new System.Drawing.Size(632, 273);
            this.quote_innov.SymbolFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.quote_innov.SymbolFontColor = System.Drawing.Color.Green;
            this.quote_innov.TabIndex = 3;
            this.quote_innov.TableLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.quote_innov.Text = "viewQuoteList1";
            this.quote_innov.UPColor = System.Drawing.Color.Red;
            // 
            // ctQuoteMoniter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctOrderSenderM1);
            this.Controls.Add(this.quotenav);
            this.Name = "ctQuoteMoniter";
            this.Size = new System.Drawing.Size(634, 355);
            ((System.ComponentModel.ISupportInitialize)(this.quotenav)).EndInit();
            this.quotenav.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
            this.kryptonPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.page2)).EndInit();
            this.page2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.page3)).EndInit();
            this.page3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage4)).EndInit();
            this.kryptonPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
            this.kryptonPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Navigator.KryptonNavigator quotenav;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
        private ComponentFactory.Krypton.Navigator.KryptonPage page2;
        private ComponentFactory.Krypton.Navigator.KryptonPage page3;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage4;
        private FutSystems.GUI.ViewQuoteList quote_shfe;
        private FutSystems.GUI.ViewQuoteList quote_czce;
        private FutSystems.GUI.ViewQuoteList quote_dce;
        private FutSystems.GUI.ViewQuoteList quote_cffex;
        private ctOrderSenderM ctOrderSenderM1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
        private FutSystems.GUI.ViewQuoteList quote_innov;
    }
}
