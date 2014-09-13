using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib
{
    #if YJ

    //#else
    /// <summary>
    /// 利息收入计划
    /// </summary>
    public class InterestRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.INTEREST;
        public QSEnumFinServiceType Type { get { return _type; } set { _type = value; } }

        public decimal CalRate(IAccount account, decimal finammount)
        {
            decimal n = finammount / 10000;
            //2万以内的配资 收费25
            if (n < 2)
                return n * 25;
            //5万以内的配资 收费20
            if (n < 5)
                return n * 20;
            //10万以内配资 收费16
            if (n < 10)
                return n * 16;
            //25万以内配资 收费15
            if (n < 25)
                return n * 15;
            return n * 14;
        }

        public decimal AdjustCommission(int size, decimal commission)
        {
            return commission;
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }
    }



    public class CommissionRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.COMMISSION;
        public QSEnumFinServiceType Type { get { return _type; } }

        public decimal CalRate(IAccount account, decimal finammount)
        {

            return 0;

        }
        public decimal AdjustCommission(int size, decimal commission)
        {
            return commission;
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }
    }

    /// <summary>
    /// 比例分红计划
    /// </summary>
    public class BonusRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.BONUS;
        public QSEnumFinServiceType Type { get { return _type; } }
        public decimal CalRate(IAccount account, decimal finammount)
        {
            if (account.RealizedPL <= 0)
                return 0;//不盈利不收费
            else
                return account.Profit * 0.2M;//盈利收费为当日盈利额10%
        }

        /// <summary>
        /// 分红费率计划 手续费是标准手续费 加成20%
        /// </summary>
        /// <param name="size"></param>
        /// <param name="commission"></param>
        /// <returns></returns>
        public decimal AdjustCommission(int size, decimal commission)
        {
            return commission * 1.1M;
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }

        
    }
    #endif

}
