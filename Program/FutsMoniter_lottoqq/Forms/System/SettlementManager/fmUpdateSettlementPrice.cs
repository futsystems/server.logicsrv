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

namespace FutsMoniter
{
    public partial class fmUpdateSettlementPrice : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmUpdateSettlementPrice()
        {
            InitializeComponent();
        }

        MarketData _price;
        public void SetSettlementPrice(MarketData price)
        {
            _price = price;
            lbPrice.Text = Util.FormatDecimal(price.Settlement);
            lbSettleday.Text = price.SettleDay.ToString();
            lbSymbol.Text = price.Symbol;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            decimal p = ndPrice.Value;
            if (MoniterUtils.WindowConfirm(string.Format("确认更新合约:{0}的结算价为:{1}", lbSymbol.Text, p)) == System.Windows.Forms.DialogResult.Yes)
            { 
                _price.Settlement = p;
                Globals.TLClient.ReqUpdateSettlementPrice(_price);
            }
        }


    }
}
