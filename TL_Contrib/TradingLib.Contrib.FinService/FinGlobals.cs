using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib
{
    public class FinGlobals
    {

        #region 配资全局设置
        public static int FinPower = 10;//配资比例
        public static int HoldOverNightPower = 3;//隔夜融资比例
        public static decimal FinFlatRate = 0.25M;//强平比例
        public static decimal CutBuyPowerRate = 0.5M;//减额比例
        public static bool IsCutInMarket = true;//盘中减额
        public static bool IsCutAfterMarket = true;//盘后减额

        public static decimal FinFeeS5 = 15;
        public static decimal FinFeeS20 = 13.8M;
        public static decimal FinFeeS50 = 11.8M;

        public static decimal BonusRate = 0.2M;//分红比例
        public static decimal BonusRateCommissionMargin = 0.2M;//分红方式手续费加成

        public static void InitFinServiceConfig(int power, int powernight, decimal flatrate, decimal cutpwoerrate, bool cutinmarket, bool cutaftermarket, decimal finfees5, decimal finfees20, decimal finfees50, decimal bonusrate, decimal bonuscommissionmargin)
        {
            FinPower = power;
            HoldOverNightPower = powernight;
            FinFlatRate = flatrate;
            CutBuyPowerRate = cutpwoerrate;
            IsCutInMarket = cutinmarket;
            IsCutAfterMarket = cutaftermarket;

            FinFeeS5 = finfees5;
            FinFeeS20 = finfees20;
            FinFeeS50 = finfees20;

            BonusRate = bonusrate;
            BonusRateCommissionMargin = bonuscommissionmargin;
        }
        #endregion

        #region 福建特殊配资参数

        public static void InitFinServiceFJ(bool fj_fixenable, decimal ifcomm, decimal winfeeagent, decimal lossfeeagent, decimal pledgeagent,
            decimal winfeecust, decimal lossfeecust, decimal marginperlot, decimal marginflat)
        {
            EnableFixedMargin = fj_fixenable;
            CommissionRate = ifcomm;
            ServiceFeePerLossAgent = lossfeeagent;
            ServiceFeePerWinAgent = winfeeagent;
            PledgeAgent = pledgeagent;

            CommissionPerWin = winfeecust;
            CommissionPerLoss = lossfeecust;

            MarginPerLot = marginperlot;
            MarginPerLotStop = marginflat;

        }
        //是否激活默认固定金额版本的配资
        public static bool EnableFixedMargin = false;

        public static decimal MarginPerLot = 5000;//单手股指需要的资金
        public static decimal MarginPerLotStop = 2000;//每手保证金降低到该数值时,执行强平

        //终端客户费用收取
        public static decimal CommissionPerWin = 200;//盈利情况下单手扣费
        public static decimal CommissionPerLoss = 100;//亏损情况下单手扣费

        //收取代理的费用
        public static decimal CommissionRate = 0.3M;//万分之0.3的交易手续费
        public static decimal ServiceFeePerWinAgent = 100;//盈利情况下单手扣费
        public static decimal ServiceFeePerLossAgent = 50;//亏损情况下单手扣费
        public static decimal PledgeAgent = 10;//每手收取的代理押金


        /// <summary>
        /// 获得资金可以交易的总手数
        /// 强平线以上就可以开1手。
        /// 5000以上开2手
        /// 10000以上开3手
        /// 算法
        /// 资金/5000 +1;
        /// </summary>
        /// <param name="equity"></param>
        /// <returns></returns>
        public static int GetFixMarginLots(decimal equity)
        {
            if (equity < MarginPerLot)
            {
                if (equity > MarginPerLotStop)
                    return 1;
                else
                    return 0;
            }
            else
            {
                return (int)(equity / MarginPerLot) + 1;
            }

        }
        #endregion
    }
}
