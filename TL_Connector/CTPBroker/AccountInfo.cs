using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTP;

namespace Broker.CTP
{
    public class AccountInfo
    {
        public static AccountInfo genAccountInfoData(ThostFtdcTradingAccountField acinfo)
        {

            AccountInfo a = new AccountInfo();
            a.Available = acinfo.Available;
            a.CloseProfit = acinfo.CloseProfit;
            a.Commission = acinfo.Commission;
            a.CurrMargin = acinfo.CurrMargin;
            a.Deposit = acinfo.Deposit;
            a.FrozenCommission = acinfo.FrozenCommission;
            a.PositionProfit = acinfo.PositionProfit;
            a.PreBalance = acinfo.PreBalance;
            a.Reserve = acinfo.Reserve;
            a.Withdraw = acinfo.Withdraw;
            a.WithdrawQuota = acinfo.WithdrawQuota;
            a.Static = acinfo.PreBalance + acinfo.Deposit - acinfo.Withdraw;
            a.CurrMargin = acinfo.PreBalance + acinfo.Deposit - acinfo.Withdraw + acinfo.PositionProfit + acinfo.CloseProfit - acinfo.Commission;

            return a;
        }

		

        public double Available;
        public double CloseProfit;
        public double Commission;
        public double CurrMargin;
        public double Deposit;
        public double FrozenCommission;
        public double FrozenMargin;
        public double PositionProfit;
        public double PreBalance;
        public double Reserve;
        public double Withdraw;
        public double WithdrawQuota;
        public double Static;
        public double Current;
    }
}
