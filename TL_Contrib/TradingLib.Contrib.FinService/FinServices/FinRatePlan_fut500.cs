using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib
{
    #if YJ

    #else
    /// <summary>
    /// 按日收取利息
    /// </summary>
    public class InterestRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.INTEREST;
        public QSEnumFinServiceType Type { get { return _type; } set { _type = value; } }

        public decimal CalRate(IAccount account, decimal finammount)
        {
            /*
            decimal n = finammount / 10000;
            //2万以内的配资 收费25
            if (n < 2)
                return n * 20;
            //5万以内的配资 收费20
            if (n < 5)
                return n * 18;
            //10万以内配资 收费16
            if (n < 10)
                return n * 16;
            //25万以内配资 收费15
            if (n < 25)
                return n * 15;
            return n * 14;**/
            decimal n = finammount / 10000;
            //2万以内的配资 收费25
            //if (n < 2)
            //    return n * 25;
            //5万以内的配资 收费20
            if (n < 5)
                return n * FinGlobals.FinFeeS5;
            //10万以内配资 收费16
            if (n < 20)
                return n * FinGlobals.FinFeeS20;
            //25万以内配资 收费15
            if (n < 50)
                return n * FinGlobals.FinFeeS50;
            return n * 14;
        }

        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }
    }

     
    //手续费费率,按照标准手续费1.1倍收取,然后按照当日手续费总和来计算加成比例,10万500手续费 100万 5000手续费
    public class CommissionRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.COMMISSION;
        public QSEnumFinServiceType Type { get { return _type; } }
        
        //10万 1000 手续费  免收管理费
        //10万 500手续费    则收取手续费总计的10%作为管理费
        //10万 手续费不足500 收取100元管理费      
        public decimal CalRate(IAccount account, decimal finammount)
        {
            decimal r = account.Commission/(finammount/10000);
            if (r < 50) return (finammount / 10000)*10;
            if (r >= 50 && r < 100) return account.Commission * 0.1M;
            if (r >= 100) return 0;
            return 0;
        }
        //手续费类别的计费,按照标准手续费的1.1倍进行收取
        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission *1.1M;
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }
    }

    /// <summary>
    /// 比例分红计划
    /// 手续费为标准手续费率 提高20%
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
                return account.Profit * FinGlobals.BonusRate;//盈利收费为当日盈利额10%
        }

        /// <summary>
        /// 分红费率计划 手续费是标准手续费 加成20%进行收取
        /// </summary>
        /// <param name="size"></param>
        /// <param name="commission"></param>
        /// <returns></returns>
        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission * (1 + FinGlobals.BonusRateCommissionMargin);
        }

        public decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }
    }
    public class SPECIALIFRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.SPECIAL_IF;
        public QSEnumFinServiceType Type { get { return _type; } }

        public decimal CalRate(IAccount account, decimal finammount)
        {
            decimal n = finammount / 10000;
            return n * 14M;
        }

        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }
        /// <summary>
        /// 夜盘特价 日息万8,则在白天 对应的融资额度就为0
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }


    }
    /// <summary>
    /// 晚上单独计价
    /// </summary>
    public class SPECIALNIGHTRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.SPECIAL_NIGHT;
        public QSEnumFinServiceType Type { get { return _type; } }
        public decimal CalRate(IAccount account, decimal finammount)
        {
            decimal n = finammount / 10000;
            //单独业盘6元
            return n * 6;
        }

        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }
        /// <summary>
        /// 夜盘特价 日息万8,则在白天 对应的融资额度就为0
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }
    }

    /// <summary>
    /// 白天单独计价
    /// </summary>
    public class SPECIALDAYRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.SPECIAL_DAY;
        public QSEnumFinServiceType Type { get { return _type; } }
        public decimal CalRate(IAccount account, decimal finammount)
        {
            decimal n = finammount / 10000;
            //单独日盘8元
            return n * 8;
        }

        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }
        /// <summary>
        /// 夜盘特价 日息万8,则在白天 对应的融资额度就为0
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public decimal AdjustFinAmmount(decimal ammount)
        {
            //如果是业盘则返回可用资金,如果是白天则返回0,资金不可用
            return ammount;
        }
    }

    /// <summary>
    /// 股指特殊计价,按每笔盈亏计算
    /// </summary>
    public class SPECIALIFFJRatePlan : IRatePlan
    {
        QSEnumFinServiceType _type = QSEnumFinServiceType.SPECIAL_IF_FJ;
        public QSEnumFinServiceType Type { get { return _type; } }
        public decimal CalRate(IAccount account, decimal finammount)
        {
            return 0;
        }

        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            //平仓操作才计算手续费
            if (fill.PositionOperation == QSEnumPosOperation.ExitPosition)
            {
                if (positionround.IsClosed)
                {
                    decimal profit = positionround.Profit;//该次交易回合的盈利
                    int size = positionround.Size;//该次交易回合的成交手数

                    //注:后期这里需要该进程每个帐号按其代理设定的费用标准进行收费
                    //如果盈利则按盈利标准扣除手续费
                    if (profit > 0)
                    {
                        return size * FinGlobals.CommissionPerWin;
                    }
                    else//如果亏损则按亏损标准扣除手续费
                    {
                        return size * FinGlobals.CommissionPerLoss;
                    }
                    
                }
                else
                { 
                    //平仓操作 但是positionround为open 系统内部逻辑错误
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            
        }
        /// <summary>
        /// 夜盘特价 日息万8,则在白天 对应的融资额度就为0
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public decimal AdjustFinAmmount(decimal ammount)
        {
            //该配资策略不按传统保证金进行计算,按每手需要的资金进行计算
            return 0;
        }
    }




    #endif

}
