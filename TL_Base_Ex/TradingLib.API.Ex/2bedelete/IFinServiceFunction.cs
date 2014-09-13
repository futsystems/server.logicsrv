//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// 配资需要的函数接口
//    /// </summary>
//    public interface IFinServiceFunction
//    {
//        event AccountFinAmmountDel GetAccountFinAmmountAvabileEvent;
//        event AccountFinAmmountDel GetAccountFinAmmountTotalEvent;
//        event AdjustCommissionDel AdjustCommissionEvent;
//        /// <summary>
//        /// 获得账户的当前可用配资额
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        decimal GetAccountFinAmmountAvabile(string account);

//        /// <summary>
//        /// 获得账户设定的配资额度
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        decimal GetAccountFinAmmountTotal(string account);
//    }
//}
