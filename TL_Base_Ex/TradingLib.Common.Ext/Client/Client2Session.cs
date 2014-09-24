using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{

    /// <summary>
    /// 将Client转换成Session 适配器
    /// Client是内部底层使用的数据,可以用于相关字段的设置与修改
    /// 当暴露到外层逻辑时,我们需要进行转换,转换成ISession来表示一个逻辑上的会话概念,相关属性是只读的
    /// </summary>
    public class Client2Session:ISession
    {
        ClientInfoBase _client;//消息发送来源中的client对象

        public Client2Session(ClientInfoBase client)
        {
            _client = client;
            this.AccountID = string.Empty;
            this.ManagerID = string.Empty;
            this.SessionType = QSEnumSessionType.CLIENT;
            this.ContirbID = string.Empty;
            this.CMDStr = string.Empty;
            this.RequestID = 0;
        }

        /// <summary>
        /// 交易帐号 用于交易服务
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 管理ID用于 管理服务
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        /// 回话类型
        /// </summary>
        public QSEnumSessionType SessionType { get; set; }

        public string FrontID { get { return _client.Location.FrontID; } }

        public string ClientID { get { return _client.Location.ClientID; } }

        public string LoginID { get { return _client.LoginID; } }

        public bool IsLoggedIn { get { return _client.Authorized; } }

        
        /// <summary>
        /// 对应的扩展模块编号
        /// </summary>
        public string ContirbID { get; set; }

        /// <summary>
        /// 对应的扩展模块命令
        /// </summary>
        public string CMDStr { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID { get; set; }

    }
}
