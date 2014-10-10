using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutSystems.GUI
{
    public partial class ctSymbolSelect : UserControl
    {

        public event SymbolDelegate DefaultBasketSymAdd;
        public event SymbolDelegate DefaultBasketSymDel;


        const string FULLNAME = "全称";
        const string SYMBOL = "代码";
        const string EXCHANGE = "交易所";
        const string DESCRIPTION = "名称";
        const string TYPE = "类型";
        const string MULTIPLE = "乘数";
        const string PRICETICK = "跳";
        const string MARGIN = "保证金";
        const string COMMISSIONENTRY = "开仓手续费";
        const string COMMISSIONEXIT = "平仓手续费";


        public ctSymbolSelect()
        {
            InitializeComponent();
            SetPreferences();
            this.Load += new EventHandler(ctSymbolSelect_Load);
            try
            {
                Factory.IDataSourceFactory(exchlist).BindDataSource(UIUtil.genExchangeList(true));
                Factory.IDataSourceFactory(expire).BindDataSource(UIUtil.genExpireList());
                LoadSecurity();
                LoadSymbol();
                
            }
            catch (Exception ex)
            {

            }
        }
        bool loaded = false;
        void ctSymbolSelect_Load(object sender, EventArgs e)
        {
            loaded = true;
        }/// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = secgrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = UIGlobals.HeaderHeight;
            grid.TableElement.RowHeight = UIGlobals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 
            //grid.Columns["全称"].IsVisible = false;

        }


        void LoadSecurity()
        {
            string sectype = string.Empty;
            string exchange = string.Empty;
            //Util.Debug("selectedvalue text:" + exchlist.SelectedText.ToString());
            if (exchlist.SelectedText == UIUtil.ANY)
            {
                exchange = string.Empty;
            }
            else
            {
                exchange = ((Exchange)exchlist.SelectedValue).Index;
            }

            //Util.Debug("selected exchange:" + exchange);
            secgrid.DataSource = null;// SecurityTracker.getSecuityTable(sectype, exchange);
            secgrid.Columns["全称"].IsVisible = false;
            //secgrid.DataSource
            //secList.DataSource = SecurityTracker.getSecuityTable(_accountID);

            /*
             if (ctSecurityList1.SelectedMasterSec == null || !ctSecurityList1.SelectedMasterSec.isValid) return;
            
            string monthcode = SymbolHelper.genExpireCode(ctSecurityList1.SelectedMasterSec, ((ValueObject<int>)expire.SelectedItem).Value);
            string sym = BasketTracker.addSecIntoBasket(ctSecurityList1.SelectedMasterSec, ctSecListBox.SelectedBasket,monthcode);
            ctSecListBox.onSecListChanged();
            if (DefaultBasketSymAdd != null && ctSecListBox.SelectedBasket == "Default")
            {
                Security s = BasketTracker.getSecFromBasket(sym, "Default");
                DefaultBasketSymAdd(s);
            }
             * **/
          
        }
      
        void LoadSymbol()
        {
            symlist.Items.Clear();

            string[] syms = null;// BasketTracker.getSymbolList("Default");
            foreach (string sym in syms)
            {
                symlist.Items.Add(sym);
            }
        }

        public string CurrentSecName
        {
            get
            {
                if (secgrid.SelectedRows.Count > 0)
                {
                    string secname = secgrid.CurrentRow.Cells["全称"].Value.ToString();//positiongrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[SYMBOL].Value.ToString();
                    return secname;
                }
                else
                {
                    return "";
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Symbol mastsec = null;// SecurityTracker.getSecurity(CurrentSecName);
            if (mastsec == null)
                MessageForm.Show("请选择交易品种！");

            string monthcode = null;// SymbolHelper.genExpireCode(mastsec, (int)expire.SelectedValue);
            string symbol = null;// BasketTracker.addSecIntoBasket(mastsec, "Default", monthcode);
            LoadSymbol();
            Symbol sec = null;// BasketTracker.getSecFromBasket(symbol, "Default");
            Util.Debug("sec add:" + symbol + " sec:" + sec.Symbol);
            if (DefaultBasketSymAdd != null && sec!=null)
            {
                DefaultBasketSymAdd(sec);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string symbol = symlist.Items[symlist.SelectedIndex].ToString();
            Symbol sec = null;// BasketTracker.getSecFromBasket(symbol, "Default");
            //BasketTracker.delSecFromBasket(symbol, "Default");
            LoadSymbol();
            if (DefaultBasketSymDel != null && sec !=null)
            {
                DefaultBasketSymDel(sec);
            }

        }

        private void exchlist_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (loaded)
            {
                LoadSecurity();
            }
        }
    }
}
