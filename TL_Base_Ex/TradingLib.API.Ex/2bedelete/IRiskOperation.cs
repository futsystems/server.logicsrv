//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// 定义了风控操作接口
//    /// </summary>
//    public interface IRiskOperation
//    {
//        /// <summary>
//        /// 清除某个账户所有委托检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        void ClearOrderCheck(string accid);
//        /// <summary>
//        /// 为某个账户添加一条委托检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        void AddOrderCheck(string accid, IOrderCheck rc);

//        /// <summary>
//        /// 从某个账户中删除一条委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        void DelOrderCheck(string accid, IOrderCheck rc);

//        /// <summary>
//        /// 为某个账户清除账户检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        void ClearAccountCheck(string accid);

//        /// <summary>
//        /// 为某个账户增加账户检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        void AddAccountCheck(string accid, IAccountCheck rc);

//        /// <summary>
//        /// 为某个账户删除账户检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        void DelAccountCheck(string accid, IAccountCheck rc);

//    }
//}
