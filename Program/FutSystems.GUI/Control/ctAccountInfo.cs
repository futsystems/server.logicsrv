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
    public partial class ctAccountInfo : UserControl,IAccountView
    {
        public event VoidDelegate QueryAccountInfo;
        public event VoidDelegate QueryRaceInfo;
        public ctAccountInfo()
        {
            InitializeComponent();
            //racebox1.Visible = false;
        }

        //public void GotRaceInfo(IRaceInfo info)
        //{

        //    //if (_accounttype == QSEnumAccountCategory.QUALIFIER)
        //    {
        //        //MessageBox.Show("got race info");
        //        racebox1.Visible = true;
        //        raceType1.Text = "晋级赛";
        //        promptLevel.Text = info.RaceID;
        //        raceStatus.Text = Util.GetEnumDescription(info.RaceStatus);
        //        obverseProfit.Text = Util.FormatDecimal(info.ObverseProfit);
        //        promptDiff.Text = Util.FormatDecimal(info.PromptEquity - info.StartEquity - info.ObverseProfit);
        //    }
        //}
        //public void GotFinServiceInfo(IFinServiceInfo info)
        //{ 
            
        //}
        QSEnumAccountCategory _accounttype = QSEnumAccountCategory.SIMULATION;
        public void GotAccountInfo(IAccountInfo info)
        {
            account.Text = info.Account;
            execution.Text = info.Execute ? "允许交易" : "禁止交易";
            _accounttype = info.Category;
            accountCategory.Text = Util.GetEnumDescription(info.Category);
            interday.Text = info.IntraDay ? "日内交易" : "隔夜交易";


            lastequity.Text = Util.FormatDecimal(info.LastEquity);
            nowequity.Text = Util.FormatDecimal(info.NowEquity);

            realizedpl.Text = Util.FormatDecimal(info.RealizedPL);
            unrealizedpl.Text = Util.FormatDecimal(info.UnRealizedPL);
            profit.Text = Util.FormatDecimal(info.Profit);
            //marginUsed.Text = Util.FormatDecimal(info.Margin);
           // marginFrozen.Text = Util.FormatDecimal(info.ForzenMargin);

            commission.Text = Util.FormatDecimal(info.Commission);
            //buypower.Text = Util.FormatDecimal(info.BuyPower);
            cashin.Text = Util.FormatDecimal(info.CashIn);
            cashout.Text = Util.FormatDecimal(info.CashOut);


        
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (QueryAccountInfo != null)
                QueryAccountInfo();
        }

        private void btnQueryRace_Click(object sender, EventArgs e)
        {
            if (QueryRaceInfo != null)
                QueryRaceInfo();

        }
    }

    

}
