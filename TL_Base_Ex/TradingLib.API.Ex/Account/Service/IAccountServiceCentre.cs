//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// 服务中心 比如比赛服务 配资服务
//    /// 提供客户注册,取消等相关操作
//    /// 
//    /// </summary>
//    public interface IAccountServiceCentre
//    {

//        /// <summary>
//        /// 某个帐号注册该帐户服务
//        /// </summary>
//        /// <param name="account"></param>
//        void Register(IAccount account);


//        /// <summary>
//        /// 当某个帐户添加该服务器 系统执行的操作
//        /// 1.为该帐户添加风控规则
//        /// 2.发送邮件等辅助功能
//        /// </summary>
//        /// <param name="account"></param>
//        void OnServiceAdded(IAccount account);
//    }
//}
