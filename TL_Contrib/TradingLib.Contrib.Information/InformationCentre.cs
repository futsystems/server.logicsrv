using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;

using System.Data;


namespace TradingLib.Contrib
{
    /// <summary>
    /// 信息中心 用于采集对应的信息然后集中向webinfopubserver转发,这样web客户端可以得到实时的数据更新
    /// 同时封装了用户注册信息,用于风控管理端获得帐号所对应的用户注册信息比如联系方式,地址等相关信息
    /// 1.用于向web端转发相关交易信息数据
    /// 2.本地交易服务获得web端的相关数据,web数据库与交易数据库是分离的
    /// </summary>
    [ContribAttr("InformationCentre","web数据","将web端的注册用户信息封装后提供给交易系统")]
    public class InformationCentre:BaseSrvObject,IContrib
    {
        ConnectionPoll<mysqlDBWebsite> conn;
        WebInfoPub webinfopub = null;
        public InformationCentre():base("InformationCentre")
        {
            ConfigFile cfg = ConfigFile.GetConfigFile("information.cfg");
            conn = new ConnectionPoll<mysqlDBWebsite>(cfg["DBAddress"].AsString(), cfg["DBUser"].AsString(), cfg["DBPass"].AsString(), cfg["DBName"].AsString(), cfg["DBPort"].AsInt());
            webinfopub = new WebInfoPub(cfg["WebPubAddress"].AsString(), cfg["WebPubPort"].AsInt());
            webinfopub.SendDebugEvent += new DebugDelegate(msgdebug);
            webinfopub.Start();
        }

        #region IContrib 接口实现
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {

            //TLCtxHelper.Ctx.MessageMgr.GetUserProfileEvent += new GetUserProfileDel(this.GetUserProfile);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 运行
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

        #endregion


        public IProfile GetUserProfile(string account,int userid)
        {
            /*
            mysqlDBWebsite db = conn.mysqlDB;
            DataSet reset =  db.getProfile(userid);
            conn.Return(db);

            DataTable dt = reset.Tables["profile"];
            if (dt.Rows.Count < 1) return null;
            DataRow dr = dt.Rows[0];
            Profile pf = new Profile();
            pf.About = Convert.ToString(prasedbvalue(dr["about"]));
            pf.Account = account;
            pf.Address = Convert.ToString(prasedbvalue(dr["address"]));
            DateTime d = new DateTime(1970, 1, 1);
            DateTime.TryParse(prasedbvalue(dr["birthday"]),out d);
            pf.Birthday = d;
            pf.Gender = (QSEnumGender)Enum.Parse(typeof(QSEnumGender), Convert.ToString(prasedbvalue(dr["gender"])));
            pf.Email = Convert.ToString(prasedbvalue(dr["email"]));
            pf.Location = Convert.ToString(prasedbvalue(dr["location"]));
            pf.Name = Convert.ToString(prasedbvalue(dr["name"]));
            pf.NickName = Convert.ToString(prasedbvalue(dr["username"]));
            pf.PersonID = Convert.ToString(prasedbvalue(dr["personID"]));
            pf.Phone = Convert.ToString(prasedbvalue(dr["phone"]));
            pf.QQ = Convert.ToString(prasedbvalue(dr["qq"]));
            pf.WebSite = Convert.ToString(prasedbvalue(dr["website"]));

            return pf;**/
            return null;
        }
        string prasedbvalue(object o)
        {
            if (o is System.DBNull)
            {
                return string.Empty;
            }
            else
            {
                return o.ToString();
            }
        }

        /// <summary>
        /// 向webinfopub转发登入信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="online"></param>
        /// <param name="clientID"></param>
        public void newSessionUpdate(string accid, bool online, string clientID)
        {
            //Message msg = new Message(MessageTypes.MGRSESSIONUPDATE, accid + "," + online.ToString() + "," + clientID.Split('-')[0] + "****");
            Message msg = new Message(MessageTypes.LOGINUPDATE, accid + "," + online.ToString() + "," + clientID.Split('-')[0] + "****");
            webinfopub.notify_sessioninfo(msg);
        }

        /// <summary>
        /// 向webinfopub转发tick数据
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            Message msg = new Message(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k));
            webinfopub.notify_tick(msg);
        }
        /// <summary>
        /// 发送最新的交易统计信息
        /// </summary>
        /// <param name="tradernum"></param>
        /// <param name="ordernum"></param>
        /// <param name="margin"></param>
        //public void newStatistic(IWebStatistic w)
        //{
        //    webinfopub.notify_statistic(w.TotalAccounts,w.TotalOrders,w.TotalMargin);
        //}

    }
}
