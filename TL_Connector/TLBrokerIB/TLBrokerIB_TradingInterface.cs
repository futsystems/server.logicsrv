using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace Broker.Live
{
    public partial class TLBrokerIB
    {

        public void SendOrder(BinaryOptionOrder o)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 通过接口提交委托
        /// IB接口 可以跨越多空进行买卖 不用判定开平或者可平仓数量,只需要直接讲对应的数量提交即可
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            logger.Info("XAPI[" + this.Token + "] Send Order:" + o.GetOrderInfo());

            //复制接口接受到的委托 并设置相关字段 lo相当于是原始委托o的一个子委托
            Order lo = new OrderImpl(o);
            //接口发出的委托相当于是接口接受委托的一个 一对一关系的子委托
            lo.FatherID = o.id;
            lo.FatherBreed = o.Breed;

            lo.Breed = QSEnumOrderBreedType.BROKER;
            lo.Broker = this.Token;

            lo.id = orderIdtk.AssignId;
            lo.BrokerLocalOrderID = "";
            lo.BrokerRemoteOrderID = "";
            lo.OrderSeq = 0;
            lo.OrderRef = "";
            lo.FrontIDi = 0;
            lo.SessionIDi = 0;
            lo.Account = this.Token;

            //生成IB 合约对象
            Krs.Ats.IBNet.Contract contract = Symbol2Contract(lo.oSymbol);

            Krs.Ats.IBNet.Order order = new Krs.Ats.IBNet.Order();

            order.Action = lo.Side ? Krs.Ats.IBNet.ActionSide.Buy : Krs.Ats.IBNet.ActionSide.Sell;
            if (lo.LimitPrice != 0)
            {
                order.LimitPrice = lo.LimitPrice;
                order.OrderType = Krs.Ats.IBNet.OrderType.Limit;
            }
            else
            {
                order.OrderType = Krs.Ats.IBNet.OrderType.Market;
            }


            order.TotalQuantity = Math.Abs(lo.TotalSize);

            int orderid = NextOrderId;
            client.PlaceOrder(orderid, contract, order);

            lo.Status = QSEnumOrderStatus.Submited;
            lo.BrokerLocalOrderID = orderid.ToString();

            o.Status = QSEnumOrderStatus.Submited;//标注原来的委托已提交
            //o.BrokerLocalOrderID = orderid.ToString();//将本地递增编号设置成BrokerLocalID

            //记录委托map
            //近端ID委托map 用于记录递增的OrderId与委托映射关系
            localOrderID_map.TryAdd(lo.BrokerLocalOrderID, lo);
            //记录父委托和子委托
            sonFathOrder_Map.TryAdd(lo.id, o);
            fatherOrder_Map.TryAdd(o.id, o);
            fatherSonOrder_Map.TryAdd(o.id, lo);

            //发送子委托时 记录到数据库
            this.LogBrokerOrder(lo);

            logger.Info("Send Order To IB TWS, BrokerLocalID:" + orderid.ToString());
        }

        public void CancelOrder(long oid)
        {
            logger.Info("XAPI[" + this.Token + "] Cancel Order:" + oid.ToString());

            Order soneorder = FatherID2SonOrder(oid);
            client.CancelOrder(Convert.ToInt16(soneorder.BrokerLocalOrderID));

        }
    }
}
