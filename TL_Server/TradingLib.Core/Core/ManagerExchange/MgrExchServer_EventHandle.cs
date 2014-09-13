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
        public event OrderDelegate SendOrderEvent;
        /// <summary>
        /// 管理端提交的取消委托事件
        /// </summary>
        public event LongDelegate SendOrderCancelEvent;


        void tl_ClientUnregistedEvent(MgrClientInfo client)
        {
            debug(string.Format("Client:{0} unregist from tlserver", client.Location.ClientID), QSEnumDebugLevel.INFO);
            CustInfoEx o = null;
            customerExInfoMap.TryRemove(client.Location.ClientID, out o);
        }

        void tl_ClientRegistedEvent(MgrClientInfo client)
        {
            debug(string.Format("Client:{0} regist to tlserver", client.Location.ClientID), QSEnumDebugLevel.INFO);
            customerExInfoMap[client.Location.ClientID] = new CustInfoEx(client.Location);
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
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        void tl_newOrderActionRequest(OrderAction o)
        {
            if (o.ActionFlag == QSEnumOrderActionFlag.Delete)
            {
                if (o.OrderID != 0)
                {
                    if (SendOrderCancelEvent != null)
                        SendOrderCancelEvent(o.OrderID);
                }
            }
        }

        void tl_newLoginRequest(MgrClientInfo clientinfo, LoginRequest request, ref LoginResponse response)
        {
            throw new NotImplementedException();
        }


        ILocation[] tl_GetLocationsViaAccountEvent(string account)
        {
            List<ILocation> locations = new List<ILocation>();
            foreach (CustInfoEx cst in customerExInfoMap.Values)
            {
                locations.Add(cst.Location);
            }

            return locations.ToArray();
        }

    }
}
