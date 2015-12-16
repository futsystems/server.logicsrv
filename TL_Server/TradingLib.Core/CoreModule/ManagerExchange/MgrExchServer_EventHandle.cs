using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {


        void tl_ClientUnregistedEvent(MgrClientInfo client)
        {
            logger.Info(string.Format("Client:{0} unregist from tlserver", client.Location.ClientID));
            CustInfoEx o = null;
            customerExInfoMap.TryRemove(client.Location.ClientID, out o);
        }

        void tl_ClientRegistedEvent(MgrClientInfo client)
        {
            logger.Info(string.Format("Client:{0} regist to tlserver", client.Location.ClientID));
            customerExInfoMap[client.Location.ClientID] = new CustInfoEx(client);
        }




        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();

            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.ORDERNOTIFY);
            //f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.EXECUTENOTIFY);

            //客户端主动发起请求的功能
            f.Add(MessageTypes.SENDORDER);//发送委托
            //f.Add(MessageTypes.ORDERCANCELREQUEST);//发送取消委托tongg
            f.Add(MessageTypes.QRYACCOUNTINFO);//查询账户信息
            //f.Add(MessageTypes.QRYCANOPENPOSITION);//查询可开
            f.Add(MessageTypes.REQCHANGEPASS);//请求修改密码


            return f.ToArray();
        }

        void tl_newSendOrderRequest(Order o)
        {
            //对外发送委托 注 这里需要按照对应持仓的情况进行有效分拆，看是否括约的今仓或昨仓
            //如果跨越的昨仓与今仓则需要分拆 并且将开平标识进行规范化
            TLCtxHelper.ModuleExCore.SendOrderInternal(o);
        }

        void tl_newOrderActionRequest(OrderAction o)
        {
            if (o.ActionFlag == QSEnumOrderActionFlag.Delete)
            {
                if (o.OrderID != 0)
                {
                    TLCtxHelper.ModuleExCore.CancelOrder(o.OrderID);
                }
            }
        }

        void tl_newLoginRequest(MgrClientInfo clientinfo, LoginRequest request, ref LoginResponse response)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询具有查看某个帐户account权限的Manager地址
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        ILocation[] tl_GetLocationsViaAccountEvent(string account)
        {
            return customerExInfoMap.Values.Where(ex => ex.RightAccessAccount(account)).Select(ex2=>ex2.Location).ToArray();
        }

        /// <summary>
        /// 通知对象列表
        /// 管理端登入后会在内存中创建一个管理端Agent
        /// </summary>
        IEnumerable<CustInfoEx> NotifyTargets
        {
            get
            {
                return customerExInfoMap.Values;
            }
        }

    }
}
