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

        /// <summary>
        /// 回话类型
        /// </summary>
        public QSEnumSessionType SessionType { get; private set; }


        public Client2Session(ClientInfoBase client)
        {
            _client = client;
            //this.AccountID = string.Empty;//交易员帐号
            //this.MGRLoginName = string.Empty;//管理员帐号
            //this.MGRID = 0;
            //this.MGRFK = 0;

            this.SessionType = QSEnumSessionType.CLIENT;
            this.ContirbID = string.Empty;
            this.CMDStr = string.Empty;
            this.RequestID = 0;
        }

        /// <summary>
        /// 交易帐号 用于交易服务
        /// </summary>
        public string AuthorizedID 
        {
            get
            {
                if (this.SessionType == QSEnumSessionType.CLIENT)
                {
                    return this.Account != null ? this.Account.ID : "";
                }
                else if (this.SessionType == QSEnumSessionType.MANAGER)
                {
                    return this.Manager != null ? this.Manager.Login : "";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Manager对象
        /// </summary>
        public Manager Manager { get; private set; }

        /// <summary>
        /// 交易帐户对象
        /// </summary>
        public IAccount Account { get; private set; }


        public void BindManager(Manager manger)
        {
            this.SessionType = QSEnumSessionType.MANAGER;
            this.Manager = manger;
            //this.MGRFK = manger.mgr_fk;
            //this.MGRID = manger.ID;
            //this.MGRLoginName = manger.Login;
        }

        public void BindAccount(IAccount account)
        {
            this.SessionType = QSEnumSessionType.CLIENT;
            this.Account = account;
        }

        //#region Manager 对象



        /////// <summary>
        /////// 管理员登入名
        /////// </summary>
        ////public string MGRLoginName { get; private set; }

        /////// <summary>
        /////// 管理员ID
        /////// </summary>
        ////public int MGRID { get; private set; }

        /////// <summary>
        /////// 管理员主域ID
        /////// </summary>
        ////public int MGRFK { get; private set; }


       



        //#endregion

        /// <summary>
        /// 回话是否已经登入
        /// </summary>
        public bool Authorized { get { return _client.Authorized; } }

        /// <summary>
        /// 回话对端地址
        /// </summary>
        public ILocation Location { get { return _client.Location; } }

        //public string LoginID { get { return _client.LoginID; } }



        
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
