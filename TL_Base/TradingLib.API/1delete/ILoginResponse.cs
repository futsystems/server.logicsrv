//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    public interface ILoginResponse : IReply
//    {
//        /// <summary>
//        /// 登入ID 用于验证时候所输入的邮件地址/手机号码等
//        /// </summary>
//        string LoginID { get; set; }
//        /// <summary>
//        /// 服务类别
//        /// </summary>
//        QSEnumTLServiceType Service { get; set; }
//        /// <summary>
//        /// 回话编号
//        /// </summary>
//        string SessionID { get; set; }
//        /// <summary>
//        ///服务对应的交易帐号,非交易类服务则不需要提供该字段
//        /// </summary>
//        string Account { get; set; }



//        //---用户类信息----
//        /// <summary>
//        /// 邮件地址
//        /// </summary>
//        string Email { get; set; }
//        /// <summary>
//        /// 手机号码
//        /// </summary>
//        string Mobile { get; set; }
//        /// <summary>
//        /// 用户昵称
//        /// </summary>
//        string NickName { get; set; }
//        /// <summary>
//        /// 用户姓名
//        /// </summary>
//        string UserName { get; set; }
//        /// <summary>
//        /// 用户全局ID
//        /// </summary>
//        int UID { get; set; }

//    }
//}
