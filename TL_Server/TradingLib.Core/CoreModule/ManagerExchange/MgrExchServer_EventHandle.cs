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

        ///<summary>
        /// 管理端触发的提交委托事件
        /// </summary>
        //public event OrderDelegate SendOrderEvent;
        /// <summary>
        /// 管理端提交的取消委托事件
        /// </summary>
        //public event LongDelegate SendOrderCancelEvent;


        void tl_ClientUnregistedEvent(MgrClientInfo client)
        {
            debug(string.Format("Client:{0} unregist from tlserver", client.Location.ClientID), QSEnumDebugLevel.INFO);
            CustInfoEx o = null;
            customerExInfoMap.TryRemove(client.Location.ClientID, out o);
        }

        void tl_ClientRegistedEvent(MgrClientInfo client)
        {
            debug(string.Format("Client:{0} regist to tlserver", client.Location.ClientID), QSEnumDebugLevel.INFO);
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
            //if (SendOrderEvent != null)
            //    SendOrderEvent(o);
            TLCtxHelper.ModuleExCore.SendOrderInternal(o);
        }

        void tl_newOrderActionRequest(OrderAction o)
        {
            if (o.ActionFlag == QSEnumOrderActionFlag.Delete)
            {
                if (o.OrderID != 0)
                {
                    TLCtxHelper.ModuleExCore.CancelOrder(o.OrderID);

                    //if (SendOrderCancelEvent != null)
                    //    SendOrderCancelEvent(o.OrderID);
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

            //当以account为对象查找通知对象时,遍历所有链接的管理端,如果对应的管理端有权限查看该帐户,则进行通知
            //List<ILocation> locations = new List<ILocation>();
            //foreach (CustInfoEx cst in customerExInfoMap.Values)
            //{
            //    locations.Add(cst.Location);
            //}

            //return locations.ToArray();
        }


        IEnumerable<CustInfoEx> NotifyTarges
        {
            get
            {
                return customerExInfoMap.Values;
            }
        }

    }
}
