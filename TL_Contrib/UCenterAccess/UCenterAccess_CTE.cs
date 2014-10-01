using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace Lottoqq.UCenter
{
    public partial class UCenterAccess
    {
        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "registeruser",
            "registeruser - register user with userref and password",
            "向UCenter统一认证中心注册用户",
            false)]
        public void CTE_RegisterUser(ISession session, string uref, string password)
        {
            TLCtxHelper.Debug("registeruser is called");
            //Send(session, new { Name = name, Amount = amount });
            string retstr = _ucli.RegisterUser(uref, password);
            //将消息直接返回给客户端端
            //Send(session, retstr);
        }

        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "requestraceuser",
            "requestraceuser - requestraceuser 请求比赛交易帐号",
            "提供用户名与密码让后给该帐户申请比赛帐号",
            false)]
        public void CTE_RequestRaceAccount(ISession session,string uref, string pass)
        { 
            //1.认证
            string retstr = _ucli.AuthUser(uref, pass);
            string userid = "";
            JsonData data = JsonMapper.ToObject(retstr);
            //认证失败
            if (Convert.ToInt32(data["Result"].ToString()) != 0)
            {
                debug("uref:" + uref + " pass:" + pass + " 认证错误,无法申请交易帐号",QSEnumDebugLevel.INFO);
                //Send(session, JsonReply.GenericError(ReplyType.AuthError, "用户名或密码错误").ToJson());
            }
            else
            {
                //2.添加交易帐号
                userid = data["UID"].ToString();
                string account = string.Empty;
                //string account = TLCtxHelper.CmdClearCentre.RequestRaceAccount(userid, "123456");
                // TLCtxHelper.CmdAccount
                if (string.IsNullOrEmpty(account))
                {
                    debug("创建交易帐号出错", QSEnumDebugLevel.INFO);
                    //Send(session, JsonReply.GenericError(ReplyType.AccountCreatedError, "创建比赛帐号出错").ToJson());
                }
                else
                {
                    debug("创建交易帐号成功:"+account, QSEnumDebugLevel.INFO);
                    //Send(session,ReplyHelper.Success_Generic);
                }
            }
        }

       
        /// <summary>
        /// 通过UCenter广播消息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="notifyname"></param>
        /// <param name="jsonstr"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "broadcast", "broadcast - 广播消息", "广播消息")]
        public void BroadcastMessage(string filter, string notifyname, string jsonstr)
        {
            _ucli.Broadcast(filter,  notifyname, jsonstr);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "demoauth", "demoauth - 模拟认证", "通过交易系统的UCenter模块进行模拟认证")]
        public string CTE_DemoAuth(string username, string pass)
        {
            return _ucli.AuthUser(username, pass);
        }

    }
}
