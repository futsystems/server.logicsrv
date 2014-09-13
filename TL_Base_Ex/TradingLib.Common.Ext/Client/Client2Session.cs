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
        }

        /// <summary>
        /// 交易帐号 用于交易服务
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 管理ID用于 管理服务
        /// </summary>
        public string ManagerID { get; set; }

        public string SessionID { get { return _client.Location.ClientID; } }

        public string FrontID { get { return _client.Location.FrontID; } }

        public string ClientID { get { return _client.Location.ClientID; } }

        public string LoginID { get { return _client.LoginID; } }

        public bool IsLoggedIn { get { return _client.Authorized; } }

        

        string _contribid = "";

        /// <summary>
        /// 对应的扩展模块编号
        /// </summary>
        public string ContirbID { get { return _contribid; } set { _contribid = value; } }

        string _cmdstr = ""; 
        /// <summary>
        /// 对应的扩展模块命令
        /// </summary>
        public string CMDStr { get { return _cmdstr; } set { _cmdstr = value; } }


        int _requestid = 0;
        public int RequestID { get { return _requestid; } set { _requestid = value; } }

    }
}
