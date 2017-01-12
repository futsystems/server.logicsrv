using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_SessionInfo
    {
        ///// <summary>
        ///// 将某个客户端信息绑定到交易帐户对象
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="client"></param>
        //public static void BindClient(this IAccount account, TrdClientInfo client)
        //{
        //    SessionInfo info = new SessionInfo();
        //    info.Account = account.ID;
        //    info.ClientID = client.Location.ClientID;
        //    info.FrontID = client.Location.FrontID;
        //    info.IPAddress = client.IPAddress;
        //    info.ProductInfo = client.ProductInfo;

        //    //客户登入成功后 设定交易帐户登入标识和回话信息字段
        //    account.IsLogin = true;
        //    account.SessionInfo = Newtonsoft.Json.JsonConvert.SerializeObject(info);

        //}

        //public static void UnBindClient(this IAccount account)
        //{
        //    account.IsLogin = false;
        //    account.SessionInfo = string.Empty;
        //}
    }
}