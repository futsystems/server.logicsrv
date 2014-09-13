using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.GUI
{
    public partial class fmAddSymbol : KryptonForm
    {
        public fmAddSymbol()
        {
            InitializeComponent();
           // Factory.IDataSourceFactory(expire).BindDataSource(UIUtil.genExpireList());
        }

        Dictionary<string, SecurityImpl> secmap = new Dictionary<string, SecurityImpl>();
        private void add_Click(object sender, EventArgs e)
        {
            /*
            if (ctSecurityList1.SelectedMasterSec == null || !ctSecurityList1.SelectedMasterSec.isValid) return;
            string monthcode = SymbolHelper.genExpireCode(ctSecurityList1.SelectedMasterSec, ((ValueObject<int>)expire.SelectedItem).Value);
            string symcode = ctSecurityList1.SelectedMasterSec.Symbol;
            string symbol = symcode + monthcode;
            if (secmap.Keys.Contains(symbol)) return;
            SecurityImpl sec = GenSecurity(symbol, ctSecurityList1.SelectedMasterSec);
            secmap.Add(symbol, sec);
            
            //QuantGlobals.GDebug(sec.FullName + "|" + sec.Multiple.ToString() + "|" + sec.Margin.ToString() + "|" + sec.PriceTick.ToString() + "|" + sec.EntryCommission.ToString() + "|" + sec.ExitCommission.ToString() +"|" +sec.MasterSecurity);

            seclist.Items.Add(symbol);
            **/
        }


        public List<SecurityImpl> SecurityList { get { return secmap.Values.ToList(); } }

        //利用MastSecurity主品种信息 与 合约代码 生成对应的security(symbol)信息
        SecurityImpl GenSecurity(string symbol, Security MastSecurity)
        {
            SecurityImpl sec = new SecurityImpl(symbol,MastSecurity.DestEx,MastSecurity.Type);
            sec.MasterSecurity = MastSecurity.FullName;
            sec.Currency = MastSecurity.Currency;
            sec.EntryCommission = MastSecurity.EntryCommission;
            sec.ExitCommission = MastSecurity.ExitCommission;
            sec.Margin = MastSecurity.Margin;//保证金
            sec.Multiple = MastSecurity.Multiple;//合约乘数
            sec.PriceTick = MastSecurity.PriceTick;//最小价格变动

            return sec;

        }

        private void del_Click(object sender, EventArgs e)
        {
            if (seclist.SelectedItem == null) return;
            string symbol = seclist.SelectedItem.ToString();
            if (secmap.Keys.Contains(symbol))
                secmap.Remove(symbol);
            seclist.Items.Remove(seclist.SelectedItem);
            //QuantGlobals.GDebug(" symbol:" + symbol + " now seclist num:" + secmap.Count.ToString());
        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
