//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.Core
//{
//    public class CoreGlobal
//    {
//        #region 数据库全局信息
//        public static void InitDBConfig(string name, int port)
//        {
//            DBName = name;
//            DBPort = port;
//        }

//        public static string DBName = "quantshop";
//        public static int DBPort = 3306;
//        #endregion

//        #region 交易帐号全局信息
//        public static void InitAccountConfig(int dealerprefix=170, int dealernum=3, int loaneeprefix=168, int loaneenum=3)
//        {
//            //DealerPrefix = dealerprefix;
//            //DealerAccountNum = dealernum;

//            //LoaneePrefix = loaneeprefix;
//            //LoaneeAccountNum = loaneenum;

//            //将全局参数传递到mysql组件
//            MySql.SqlGlobal.InitAccountConfig(dealerprefix, dealernum, loaneeprefix, loaneenum); 
//        }

//        /// <summary>
//        /// 交易员帐号前缀
//        /// </summary>
//        //public static int DealerPrefix = 17;
//        /// <summary>
//        /// 交易员帐号位数
//        /// </summary>
//        //public static int DealerAccountNum = 4;
//        /// <summary>
//        /// 配子帐号前缀
//        /// </summary>
//        //public static int LoaneePrefix = 168;
//        /// <summary>
//        /// 配资本帐号位数
//        /// </summary>
//        //public static int LoaneeAccountNum = 3;

        
//        #endregion

//        #region 清算中心全局设置

//        //public static void InitClearCentre(decimal defaultsimammount,string defaultpass)
//        //{ 
//            //DefaultSimAmmount = defaultsimammount;
//            //DefaultPass = defaultpass;
//        //}
//        //public static decimal DefaultSimAmmount = 250000;

//        //public static string DefaultPass = "123456";
//        #endregion

//        #region 交易中心全局设置


//        //public static void InitTradingSrv(int minbeforeclose)
//        //{
//            //ClearPosBeforeMarketClose = minbeforeclose;
//        //}
//        //public static int ClearPosBeforeMarketClose = 5;//收盘前提前多少时间平仓
//        #endregion

//        #region 风控中心全局

//        //public static void InitRiskCentre(bool hotsim, bool hotreal,string hotlist,bool mkttimecheck)
//        //{
//            //HotCheckSim = hotsim;//模拟检查热门合约
//            //HotCheckReal = hotreal;//实盘检查热门合约
//            //HotSymbolBaskets = hotlist;//热门合约列表
//            //MarketOpenTimeCheck = mkttimecheck;//是否检查市场开市时间
//       // }
//        //public static string HotSymbolBaskets = string.Empty;//主力合约组名称
//        //public static bool HotCheckSim = false;
//        //public static bool HotCheckReal = false;
//        //public static bool MarketOpenTimeCheck = true;



//        #endregion

//        #region 福建特殊配资参数

//        //public static void InitFinServiceFJ(bool fj_fixenable,decimal ifcomm,decimal winfeeagent,decimal lossfeeagent,decimal pledgeagent,
//        //    decimal winfeecust,decimal lossfeecust,decimal marginperlot,decimal marginflat)
//        //{
//            //EnableFixedMargin = fj_fixenable;
//            //CommissionRate = ifcomm;
//            //ServiceFeePerLossAgent = lossfeeagent;
//            //ServiceFeePerWinAgent = winfeeagent;
//            //PledgeAgent = pledgeagent;

//            //CommissionPerWin = winfeecust;
//            //CommissionPerLoss = lossfeecust;

//            //MarginPerLot = marginperlot;
//            //MarginPerLotStop = marginflat;

//        //}
//        /*
//        //是否激活默认固定金额版本的配资
//        public static bool EnableFixedMargin = false;

//        public static decimal MarginPerLot = 5000;//单手股指需要的资金
//        public static decimal MarginPerLotStop = 2000;//每手保证金降低到该数值时,执行强平

//        //终端客户费用收取
//        public static decimal CommissionPerWin = 200;//盈利情况下单手扣费
//        public static decimal CommissionPerLoss = 100;//亏损情况下单手扣费

//        //收取代理的费用
//        public static decimal CommissionRate = 0.3M;//万分之0.3的交易手续费
//        public static decimal ServiceFeePerWinAgent = 100;//盈利情况下单手扣费
//        public static decimal ServiceFeePerLossAgent = 50;//亏损情况下单手扣费
//        public static decimal PledgeAgent = 10;//每手收取的代理押金
//        **/

//        /// <summary>
//        /// 获得资金可以交易的总手数
//        /// 强平线以上就可以开1手。
//        /// 5000以上开2手
//        /// 10000以上开3手
//        /// 算法
//        /// 资金/5000 +1;
//        /// </summary>
//        /// <param name="equity"></param>
//        /// <returns></returns>
//        /*
//        public static int GetFixMarginLots(decimal equity)
//        {
//            if (equity < MarginPerLot)
//            {
//                if (equity > MarginPerLotStop)
//                    return 1;
//                else
//                    return 0;
//            }
//            else
//            {
//                return (int)(equity / MarginPerLot) + 1;
//            }
        
//        }*/
//        #endregion

//        #region transport底层传输全局信息
//        /// <summary>
//        /// 初始化底层传输全局变量
//        /// </summary>
//        /// <param name="enableaccess"></param>
//        /// <param name="concurrentuser"></param>
//        public static void InitTLMQ(bool enableaccess, int concurrentuser)
//        {
//            EnableAccess = enableaccess;
//            ConcurrentUser = concurrentuser;
//        }
//        /// <summary>
//        /// 是否允许使用接入服务器
//        /// </summary>
//        public static bool EnableAccess = false;
//        /// <summary>
//        /// 同时在线并发
//        /// </summary>
//        public static int ConcurrentUser = 5;

//        #endregion


//        /*
//        #region 配资全局设置

//        public static int FinPower = 10;//配资比例
//        public static int HoldOverNightPower = 3;//隔夜融资比例
//        public static decimal FinFlatRate = 0.25M;//强平比例
//        public static decimal CutBuyPowerRate = 0.5M;//减额比例
//        public static bool IsCutInMarket = true;//盘中减额
//        public static bool IsCutAfterMarket = true;//盘后减额

//        public static decimal FinFeeS5 = 15;
//        public static decimal FinFeeS20 = 13.8M;
//        public static decimal FinFeeS50 = 11.8M;

//        public static decimal BonusRate = 0.2M;//分红比例
//        public static decimal BonusRateCommissionMargin = 0.2M;//分红方式手续费加成

//        public static void InitFinServiceConfig(int power,int powernight,decimal flatrate,decimal cutpwoerrate,bool cutinmarket,bool cutaftermarket,decimal finfees5,decimal finfees20,decimal finfees50,decimal bonusrate,decimal bonuscommissionmargin)
//        {
//            FinPower = power;
//            HoldOverNightPower = powernight;
//            FinFlatRate = flatrate;
//            CutBuyPowerRate = cutpwoerrate;
//            IsCutInMarket = cutinmarket;
//            IsCutAfterMarket = cutaftermarket;

//            FinFeeS5 = finfees5;
//            FinFeeS20 = finfees20;
//            FinFeeS50 = finfees20;

//            BonusRate = bonusrate;
//            BonusRateCommissionMargin = bonuscommissionmargin;
//        }
//        #endregion
//        **/

//        #region 流控全局设置
//        //public const int
//        //public const int StartNum = 100;//当消息数量累计到多少时开始启动检测
//        //public const int CheckNum = 100;//启动检测后跟踪消息的数目(在这个数目内计算TP)
//        //public const double RejectValue = 1.5;//TP数值达到多少后拒绝该地址的消息
//        //public const double StopValue = 1;//Tp数值降低到多少后停止检测

//        #endregion


//    }
//}
