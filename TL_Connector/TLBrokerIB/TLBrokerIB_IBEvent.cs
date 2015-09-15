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

        void client_ConnectionClosed(object sender, Krs.Ats.IBNet.ConnectionClosedEventArgs e)
        {
            logger.Info("conneced closed");
            NotifyDisconnected();
        }

        void client_Error(object sender, Krs.Ats.IBNet.ErrorEventArgs e)
        {

            //if(e.ErrorCode = ErrorMessage.FailSendOrder)
            logger.Error(string.Format("IBClient error tickerid:{0} code:{1} message:{2}", e.TickerId, e.ErrorCode, e.ErrorMsg));

            string code = e.ErrorCode.ToString();
            switch (code)
            {
                case "135"://撤单时找不到该委托
                    {
                        Order lo = LocalID2Order(e.TickerId.ToString());//找到对应的本地委托
                        if (lo == null) return;
                        Order fatherOrder = SonID2FatherOrder(lo.id);
                        if (fatherOrder == null) return;

                        OrderAction action = new OrderActionImpl();
                        action.Account = fatherOrder.Account;
                        action.ActionFlag = QSEnumOrderActionFlag.Delete;
                        action.Exchagne = "";
                        action.Symbol = fatherOrder.Symbol;
                        action.OrderID = fatherOrder.id;
                        RspInfo info = new RspInfoImpl();
                        info.ErrorID=(int)e.ErrorCode;
                        info.ErrorMessage = "找不到编号为:"+e.TickerId.ToString()+"的委托";
                        NotifyOrderOrderActionError(action, info);

                        lo.Status = QSEnumOrderStatus.Reject;
                        //lo.Comment = e.ErrorMsg;
                        tk.GotOrder(lo); //Broker交易信息管理器
                        this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

                        fatherOrder.Status = QSEnumOrderStatus.Reject;
                        fatherOrder.Comment = "找不到编号为:" + e.TickerId.ToString() + "的委托";
                        NotifyOrder(fatherOrder);
                        NotifyOrderError(fatherOrder, info);

                        return;
                    }
                case "200"://找不到合约
                    {

                        Order lo = LocalID2Order(e.TickerId.ToString());//找到对应的本地委托
                        if (lo == null) return;
                        Order fatherOrder = SonID2FatherOrder(lo.id);
                        if (fatherOrder == null) return;

                        lo.Status = QSEnumOrderStatus.Reject;
                        //lo.Comment = e.ErrorMsg;
                        tk.GotOrder(lo); //Broker交易信息管理器
                        this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

                        RspInfo info = new RspInfoImpl();
                        info.ErrorID = (int)e.ErrorCode;
                        info.ErrorMessage = "找不到对应的合约";
                        fatherOrder.Status = QSEnumOrderStatus.Reject;
                        fatherOrder.Comment = "找不到对应的合约";
                        NotifyOrderError(fatherOrder, info);


                        return;
                    }
                default:
                    break;
            }
        }

        void client_NextValidId(object sender, Krs.Ats.IBNet.NextValidIdEventArgs e)
        {
            logger.Info("IBClient connected NetxValidId:" + e.OrderId);
            //对外触发通道连接事件
            _orderId = e.OrderId;
            NotifyConnected();
        }

        void client_ExecDetails(object sender, Krs.Ats.IBNet.ExecDetailsEventArgs e)
        {
            //logger.Info(string.Format("RequestID:{0} OrderID:{1} symbol:{2} acct:{3} avg price:{4} cumQty:{5} execid:{6} orderid:{7} permid:{8} price:{9} shares:{10}", e.RequestId, e.OrderId, e.Contract.Symbol, e.Execution.AccountNumber, e.Execution.AvgPrice, e.Execution.CumQuantity, e.Execution.ExecutionId, e.Execution.OrderId, e.Execution.OrderRef, e.Execution.PermId, e.Execution.Price, e.Execution.Shares));
            logger.Info("ExecDetails:" + TradingLib.Mixins.Json.JsonMapper.ToJson(e));

            Order o = LocalID2Order(e.OrderId.ToString());
            if (o != null)
            {
                Trade fill = (Trade)(new OrderImpl(o));

                //设定价格 数量 以及日期信息
                fill.xSize = (e.Execution.Side == Krs.Ats.IBNet.ExecutionSide.Bought ? 1 : -1) * e.Execution.Shares;
                fill.xPrice = (decimal)e.Execution.Price;
                //DateTime extime = Convert.ToDateTime(e.Execution.Time);
                //fill.xDate = Util.ToTLDate(extime);
                //fill.xTime = Util.ToTLTime(extime);
                fill.xDate = Util.ToTLDate();
                fill.xTime = Util.ToTLTime();

                //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                fill.BrokerTradeID = e.Execution.ExecutionId;
                fill.TradeID = fill.BrokerTradeID;

                //logger.Info()
                //Util.Info("获得子成交:" + sonfill.GetTradeDetail());
                tk.GotFill(fill);
                //记录接口侧成交数据
                this.LogBrokerTrade(fill);

                //找对应的父委托
                Order fatherOrder = FatherID2Order(o.FatherID);
                Trade fatherfill = (Trade)(new OrderImpl(fatherOrder));
                fatherfill.xSize = fill.xSize;
                fatherfill.xPrice = fill.xPrice;
                fatherfill.xDate = Util.ToTLDate();
                fatherfill.xTime = Util.ToTLTime();

                this.NotifyTrade(fatherfill);
            }
        }

        void client_OrderStatus(object sender, Krs.Ats.IBNet.OrderStatusEventArgs e)
        {
            logger.Info("OrderStatus:" + TradingLib.Mixins.Json.JsonMapper.ToJson(e));

            Order o = LocalID2Order(e.OrderId.ToString());//查找该委托编号对应的本地委托对象
            if (o != null)
            {
                o.FilledSize = e.Filled;
                o.Size = e.Remaining * (o.Side ? 1 : -1);//更新当前数量
                
                switch (e.Status)
                { 
                    case Krs.Ats.IBNet.OrderStatus.Submitted:
                        o.Status = QSEnumOrderStatus.Opened;
                        break;

                    case Krs.Ats.IBNet.OrderStatus.PartiallyFilled:
                        o.Status = QSEnumOrderStatus.PartFilled;
                        break;

                    case Krs.Ats.IBNet.OrderStatus.Filled:
                        o.Status = QSEnumOrderStatus.Filled;
                        break;

                    case Krs.Ats.IBNet.OrderStatus.Canceled:
                        o.Status = QSEnumOrderStatus.Canceled;
                        break;

                    default:
                        logger.Warn("Order Status not handled:" + Util.GetEnumDescription(e.Status));
                        break;
                }

                //更新并记录该委托
                tk.GotOrder(o);
                this.LogBrokerOrderUpdate(o);

                //更新对应的父委托
                Order fatherOrder = FatherID2Order(o.FatherID);
                fatherOrder.Size = o.Size;
                fatherOrder.FilledSize = o.FilledSize;
                fatherOrder.Status = o.Status;

                if (string.IsNullOrEmpty(fatherOrder.BrokerRemoteOrderID))
                {
                    fatherOrder.BrokerRemoteOrderID = e.OrderId.ToString();
                    fatherOrder.OrderSysID = e.OrderId.ToString();
                }


                this.NotifyOrder(fatherOrder);

            }

        }
    }
}
