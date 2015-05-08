using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.Race
{
    /// <summary>
    /// 定义了比赛晋级规则,不同类型的比赛调用不同类型的检查规则进行检查,检查结果为保留 淘汰 晋级 三个结果
    /// 针对比赛规则,可以在这里进行修改数据,或者直接封装成单独的Dll进行加载,这样可以为客户提供不同的晋级模式
    /// 这里只是简单定义了一个策略模板
    /// </summary>
    public class RaceRule
    {
        public static QSEnumRaceType AccountRaceType(RaceService rs)
        {
            if (rs.RaceID.StartsWith("PRERACE")) return QSEnumRaceType.PRERACE;
            if (rs.RaceID == "SEMIRACE") return QSEnumRaceType.SEMIRACE;
            if (rs.RaceID == "REAL1") return QSEnumRaceType.REAL1;
            if (rs.RaceID == "REAL2") return QSEnumRaceType.REAL2;
            if (rs.RaceID == "REAL3") return QSEnumRaceType.REAL3;
            if (rs.RaceID == "REAL4") return QSEnumRaceType.REAL4;
            if (rs.RaceID == "REAL5") return QSEnumRaceType.REAL5;
            return QSEnumRaceType.PRERACE;
        }

        /// <summary>
        /// 定义不同比赛阶段的淘汰与晋级常量
        /// </summary>
        public const decimal PRERACE_PROMOT = 0.25M;//0.2 加折算
        public const decimal PRERACE_ELIMINATE = -0.2M;
        public const decimal SEMIRACE_PROMOT = 0.25M;//0.25加折算
        public const decimal SEMIRACE_ELIMINATE = -0.10M;
        public const decimal REAL1_PROMOT = 0.3M;
        public const decimal REAL1_ELIMINATE = -0.10M;
        public const decimal REAL2_PROMOT = 0.3M;
        public const decimal REAL2_ELIMINATE = -0.10M;
        public const decimal REAL3_PROMOT = 0.3M;
        public const decimal REAL3_ELIMINATE = -0.10M;
        public const decimal REAL4_PROMOT = 0.3M;
        public const decimal REAL4_ELIMINATE = -0.10M;
        public const decimal REAL5_PROMOT = 0.3M;
        public const decimal REAL5_ELIMINATE = -0.10M;

        /// <summary>
        /// 当检查账户结果为淘汰或者晋级后 获得该账户的下个状态
        /// 晋级淘汰 目的状态
        /// </summary>
        /// <returns></returns>
        public static QSEnumAccountRaceStatus GetRaceStatus(QSEnumRaceType type, QSEnumRaceCheckResult result)
        {
            switch (type)
            {
                case QSEnumRaceType.PRERACE:
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户
                            return QSEnumAccountRaceStatus.ELIMINATE;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入复赛
                            return QSEnumAccountRaceStatus.INSEMIRACE;
                        }

                    }
                    break;
                case QSEnumRaceType.SEMIRACE:
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户
                            return QSEnumAccountRaceStatus.ELIMINATE;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL1
                            return QSEnumAccountRaceStatus.INREAL1;
                        }
                    }
                    break;
                case QSEnumRaceType.REAL1://25万
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户
                            return QSEnumAccountRaceStatus.ELIMINATE;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL2
                            return QSEnumAccountRaceStatus.INREAL2;
                        }
                    }
                    break;
                case QSEnumRaceType.REAL2://50万
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户
                            return QSEnumAccountRaceStatus.ELIMINATE;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL3
                            return QSEnumAccountRaceStatus.INREAL3;
                        }
                    }
                    break;
                case QSEnumRaceType.REAL3://100万
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户 进入复赛
                            return QSEnumAccountRaceStatus.INSEMIRACE;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL4
                            return QSEnumAccountRaceStatus.INREAL4;
                        }
                    }
                    break;
                case QSEnumRaceType.REAL4://200万
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户 进入REAL1
                            return QSEnumAccountRaceStatus.INREAL1;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL5
                            return QSEnumAccountRaceStatus.INREAL5;
                        }
                    }
                    break;
                case QSEnumRaceType.REAL5://300万
                    {
                        if (result == QSEnumRaceCheckResult.ELIMINATE)
                        {
                            //淘汰账户 进入REAL2
                            return QSEnumAccountRaceStatus.INREAL2;
                        }
                        if (result == QSEnumRaceCheckResult.PROMOT)
                        {
                            //进入REAL
                            return QSEnumAccountRaceStatus.TOP;
                        }
                    }
                    break;
                default:
                    return QSEnumAccountRaceStatus.INPRERACE;
            }
            return QSEnumAccountRaceStatus.INPRERACE;
        }
        /// <summary>
        /// 获取不同比赛类型的初始资金
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static decimal StartEquity(QSEnumRaceType t)
        {
            switch (t)
            {
                case QSEnumRaceType.PRERACE:
                    return 500000;
                case QSEnumRaceType.SEMIRACE:
                    return 500000;
                case QSEnumRaceType.REAL1:
                    return 500000;
                case QSEnumRaceType.REAL2:
                    return 1000000;
                case QSEnumRaceType.REAL3:
                    return 2000000;
                case QSEnumRaceType.REAL4:
                    return 3000000;
                case QSEnumRaceType.REAL5:
                    return 5000000;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 计算晋级权益
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static decimal EliminateEquity(QSEnumRaceType t)
        {
            switch (t)
            {
                case QSEnumRaceType.PRERACE:
                    return 500000 * (1 + PRERACE_ELIMINATE);
                case QSEnumRaceType.SEMIRACE:
                    return 500000 * (1 + SEMIRACE_ELIMINATE);
                case QSEnumRaceType.REAL1:
                    return 500000 * (1 + REAL1_ELIMINATE);
                case QSEnumRaceType.REAL2:
                    return 1000000 * (1 + REAL2_ELIMINATE);
                case QSEnumRaceType.REAL3:
                    return 2000000 * (1 + REAL3_ELIMINATE);
                case QSEnumRaceType.REAL4:
                    return 3000000 * (1 + REAL4_ELIMINATE);
                case QSEnumRaceType.REAL5:
                    return 5000000 * (1 + REAL5_ELIMINATE);
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 计算淘汰权益
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static decimal PromptEquity(QSEnumRaceType t)
        {
            switch (t)
            {
                case QSEnumRaceType.PRERACE:
                    return 500000 * (1 + PRERACE_PROMOT);
                case QSEnumRaceType.SEMIRACE:
                    return 500000 * (1 + SEMIRACE_PROMOT);
                case QSEnumRaceType.REAL1:
                    return 500000 * (1 + REAL1_PROMOT);
                case QSEnumRaceType.REAL2:
                    return 1000000 * (1 + REAL2_PROMOT);
                case QSEnumRaceType.REAL3:
                    return 2000000 * (1 + REAL3_PROMOT);
                case QSEnumRaceType.REAL4:
                    return 3000000 * (1 + REAL4_PROMOT);
                case QSEnumRaceType.REAL5:
                    return 5000000 * (1 + REAL5_PROMOT);
                default:
                    return 0;
            }
        }

        public static decimal CalMargin(decimal start, decimal end)
        {
            return (end - start) / start;
        }
        /// <summary>
        /// 初赛检查
        /// 参赛开始以来,累计盈利达到20% 晋级,亏损10%淘汰,淘汰者报名预赛
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult PRERACECheck(IAccount account)
        {
            //加入晋级时间限制,用于防止中间晋级过程中，权益出错导致 连续晋级 (account.RaceEntryTime - DateTime.Now).TotalDays < 5

            decimal start = StartEquity(QSEnumRaceType.PRERACE);
            //计算折算收益 obverseProfit来计算盈利率
            decimal end = start + ProfitCal.CalObverseProfit(account);

            //晋级需要折算利润,淘汰的时候不折算利润
            if (CalMargin(start, account.NowEquity) <= PRERACE_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= PRERACE_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;

        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到20% 晋级,亏损10%淘汰 淘汰者报名预赛
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult SEMIRACECheck(IAccount account)
        {
            decimal start = StartEquity(QSEnumRaceType.SEMIRACE);
            decimal end = start + ProfitCal.CalObverseProfit(account);
            if (CalMargin(start, account.NowEquity) <= SEMIRACE_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= SEMIRACE_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;


        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到40% 晋级,亏损15%淘汰,淘汰者报名预赛
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult REAL1Check(IAccount account)
        {

            decimal start = StartEquity(QSEnumRaceType.REAL1);
            decimal end = account.NowEquity;

            if (CalMargin(start, end) <= REAL1_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= REAL1_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;
        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到40% 晋级,亏损15%淘汰,淘汰者报名预赛
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult REAL2Check(IAccount account)
        {

            decimal start = StartEquity(QSEnumRaceType.REAL2);
            decimal end = account.NowEquity;

            if (CalMargin(start, end) <= REAL2_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= REAL2_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;
        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到40% 晋级,亏损15%淘汰，淘汰者复赛
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult REAL3Check(IAccount account)
        {

            decimal start = StartEquity(QSEnumRaceType.REAL3);
            decimal end = account.NowEquity;

            if (CalMargin(start, end) <= REAL3_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= REAL3_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;
        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到40% 晋级,亏损15%淘汰，淘汰者进入real1
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult REAL4Check(IAccount account)
        {

            decimal start = StartEquity(QSEnumRaceType.REAL4);
            decimal end = account.NowEquity;

            if (CalMargin(start, end) <= REAL4_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= REAL4_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;
        }

        /// <summary>
        /// 参赛开始以来,累计盈利达到40% 晋级,亏损15%淘汰 淘汰者进入real2
        /// </summary>
        /// <param name="account"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static QSEnumRaceCheckResult REAL5Check(IAccount account)
        {

            decimal start = StartEquity(QSEnumRaceType.REAL5);
            decimal end = account.NowEquity;

            if (CalMargin(start, end) <= REAL5_ELIMINATE)
                return QSEnumRaceCheckResult.ELIMINATE;
            if (CalMargin(start, end) >= REAL5_PROMOT)
                return QSEnumRaceCheckResult.PROMOT;
            else
                return QSEnumRaceCheckResult.STAY;
        }


        public static QSEnumAccountRaceStatus NextRacePromptStatus(QSEnumAccountRaceStatus src)
        {
            switch (src)
            {
                case QSEnumAccountRaceStatus.INPRERACE:
                    return QSEnumAccountRaceStatus.INSEMIRACE;
                case QSEnumAccountRaceStatus.INSEMIRACE:
                    return QSEnumAccountRaceStatus.INREAL1;
                case QSEnumAccountRaceStatus.INREAL1:
                    return QSEnumAccountRaceStatus.INREAL2;
                case QSEnumAccountRaceStatus.INREAL2:
                    return QSEnumAccountRaceStatus.INREAL3;
                case QSEnumAccountRaceStatus.INREAL3:
                    return QSEnumAccountRaceStatus.INREAL4;
                case QSEnumAccountRaceStatus.INREAL4:
                    return QSEnumAccountRaceStatus.INREAL5;
                case QSEnumAccountRaceStatus.INREAL5:
                    return QSEnumAccountRaceStatus.TOP;
                default:
                    return QSEnumAccountRaceStatus.NORACE;
            }
        }

    }
}
