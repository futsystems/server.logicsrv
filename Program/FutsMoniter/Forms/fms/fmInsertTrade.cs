using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmInsertTrade : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmInsertTrade()
        {
            InitializeComponent();
            cbside.Items.Add("买入");
            cbside.Items.Add("卖出");
            cbside.SelectedIndex = 0;

            cboffsetflag.Items.Add("开仓");
            //cboffsetflag.Items.Add("平仓");
            cboffsetflag.SelectedIndex = 0;

            timestr.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        public void SetAccount(string acc)
        {
            account.Text = acc;
        }
        public void SetSymbol(Symbol sym)
        {
            if (sym != null)
            {
                symbol.Text = sym.Symbol;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = DateTime.ParseExact(timestr.Text, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                string sym = symbol.Text;
                SymbolImpl symimpl = Globals.BasicInfoTracker.GetSymbol(sym);
                if (symimpl == null || !symimpl.IsTradeable)
                {
                    fmConfirm.Show("合约不存在或不可交易");
                    return;
                }

                decimal xprice = price.Value;
                if (xprice == 0)
                {
                    fmConfirm.Show("设定成交价格");
                    return;
                }

                int xsize = (int)size.Value;
                if (xsize == 0)
                {
                    fmConfirm.Show("手数需大于0");
                    return;
                }

                bool side = cbside.SelectedIndex == 0 ? true : false;
                QSEnumOffsetFlag flag = cboffsetflag.SelectedIndex == 0 ? QSEnumOffsetFlag.OPEN : QSEnumOffsetFlag.CLOSE;
                Trade f = new TradeImpl(sym, xprice, xsize * (side ? 1 : -1));
                f.xdate = Util.ToTLDate();
                f.xtime = Util.ToTLTime(t);
                f.OffsetFlag = flag;

                f.Account = account.Text;
                Globals.TLClient.ReqInsertTrade(f);
                this.Close();

            }
            catch (Exception ex)
            {
                fmConfirm.Show("请输入正确的参数!");
                return;
            }

        }
    }
}
