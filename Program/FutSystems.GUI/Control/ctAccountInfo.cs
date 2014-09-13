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
        //        raceStatus.Text = LibUtil.GetEnumDescription(info.RaceStatus);
        //        obverseProfit.Text = LibUtil.FormatDisp(info.ObverseProfit);
        //        promptDiff.Text = LibUtil.FormatDisp(info.PromptEquity - info.StartEquity - info.ObverseProfit);
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
            accountCategory.Text = LibUtil.GetEnumDescription(info.Category);
            interday.Text = info.IntraDay ? "日内交易" : "隔夜交易";


            lastequity.Text = LibUtil.FormatDisp(info.LastEquity);
            nowequity.Text = LibUtil.FormatDisp(info.NowEquity);

            realizedpl.Text = LibUtil.FormatDisp(info.RealizedPL);
            unrealizedpl.Text = LibUtil.FormatDisp(info.UnRealizedPL);
            profit.Text = LibUtil.FormatDisp(info.Profit);
            //marginUsed.Text = LibUtil.FormatDisp(info.Margin);
           // marginFrozen.Text = LibUtil.FormatDisp(info.ForzenMargin);

            commission.Text = LibUtil.FormatDisp(info.Commission);
            //buypower.Text = LibUtil.FormatDisp(info.BuyPower);
            cashin.Text = LibUtil.FormatDisp(info.CashIn);
            cashout.Text = LibUtil.FormatDisp(info.CashOut);


        
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
