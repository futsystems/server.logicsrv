//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// 配资服务接口
//    /// </summary>
//    public interface IFinService
//    {
//        /// <summary>
//        /// 服务所对应的账户
//        /// </summary>
//        IAccount Account { get; set; }
//        /// <summary>
//        /// 融资额度
//        /// </summary>
//        decimal FinAmmount { get; set; }

//        /// <summary>
//        /// 费用折扣
//        /// </summary>
//        decimal Discount { get; set; }

//        /// <summary>
//        /// 选定的费率计划
//        /// </summary>
//        IRatePlan RatePlan { get; set; }

//        /// <summary>
//        /// 该配资服务的居间人代码
//        /// </summary>
//        string AgentCode { get; set; }

//        /// <summary>
//        /// 该配资服务是否有效
//        /// </summary>
//        bool Active { get; set; }
//        /// <summary>
//        /// 获得可用额度这里是一个逻辑计算过程需要结合不同的费率计划以及安全保证金计算得到当前的可用配资额度
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        decimal GetAvabileAmmount();

//        /// <summary>
//        /// 手续费调整,不同的费率计划有不同的资费标准，比如倍率1.1倍，标准资费加5毛等相应的计算逻辑
//        /// </summary>
//        /// <param name="size"></param>
//        /// <param name="commission"></param>
//        /// <returns></returns>
//        decimal AdjustCommission(Trade fill, IPositionRound positionround);

//        /// <summary>
//        /// 获得当日计费
//        /// </summary>
//        /// <returns></returns>
//        decimal GetRate();

//        string ToString();

//    }

//    public interface IRatePlan
//    {
//        /// <summary>
//        /// 服务类别
//        /// </summary>
//        QSEnumFinServiceType Type {get;}
//        /// <summary>
//        /// 计算当日收费
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        decimal CalRate(IAccount account, decimal finammount);

//        /// <summary>
//        /// 手续费调整,不同的收费计划在标准手续费上有一定的调整
//        /// </summary>
//        /// <param name="size"></param>
//        /// <param name="commission"></param>
//        /// <returns></returns>
//        decimal AdjustCommission(Trade fill, IPositionRound positionround);

//        /// <summary>
//        /// 按照不同的费率计划调整融资额度
//        /// </summary>
//        /// <param name="ammount"></param>
//        /// <returns></returns>
//        decimal AdjustFinAmmount(decimal ammount);
//    }

    
//}
