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

        bool _connlost = false;
        void client_ConnectionClosed(object sender, Krs.Ats.IBNet.ConnectionClosedEventArgs e)
        {
            logger.Info("conneced closed");
            NotifyDisconnected();
        }

        // these are fatal errors from IB
        private static readonly HashSet<int> ErrorCodes = new HashSet<int>
        {
            100, 101, 103, 138, 139, 142, 143, 144, 145, 200, 203, 300,301,302,306,308,309,310,311,316,317,320,321,322,323,324,326,327,330,331,332,333,344,346,354,357,365,366,381,384,401,414,431,432,438,501,502,503,504,505,506,507,508,510,511,512,513,514,515,516,517,518,519,520,521,522,523,524,525,526,527,528,529,530,531,10000,10001,10005,10013,10015,10016,10021,10022,10023,10024,10025,10026,10027,1300
        };

        // these are warning messages from IB
        private static readonly HashSet<int> WarningCodes = new HashSet<int>
        {
            102, 104, 105, 106, 107, 109, 110, 111, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 129, 131, 132, 133, 134, 135, 136, 137, 140, 141, 146, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 201, 202, 303,313,314,315,319,325,328,329,334,335,336,337,338,339,340,341,342,343,345,347,348,349,350,352,353,355,356,358,359,360,361,362,363,364,367,368,369,370,371,372,373,374,375,376,377,378,379,380,382,383,385,386,387,388,389,390,391,392,393,394,395,396,397,398,399,400,402,403,404,405,406,407,408,409,410,411,412,413,417,418,419,420,421,422,423,424,425,426,427,428,429,430,433,434,435,436,437,439,440,441,442,443,444,445,446,447,448,449,450,1100,10002,10003,10006,10007,10008,10009,10010,10011,10012,10014,10018,10019,10020,1101,1102,2100,2101,2102,2103,2104,2105,2106,2107,2108,2109,2110
        };

        // these require us to issue invalidated order events
        private static readonly HashSet<int> InvalidatingCodes = new HashSet<int>
        {
            104, 105, 106, 107, 109, 110, 111, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 129, 131, 132, 133, 134, 135, 136, 137, 140, 141, 146, 147, 148, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 163, 167, 168, 201, 202,313,314,315,325,328,329,334,335,336,337,338,339340,341,342,343,345,347,348,349,350,352,353,355,356,358,359,360,361,362,363,364,367,368,369,370,371,372,373,374,375,376,377,378,379,380,382,383,387,388,389,390,391,392,393,394,395,396,397,398,400,401,402,403,404,405,406,407,408,409,410,411,412,413,417,418,419,421,423,424,427,428,429,433,434,435,436,437,439,440,441,442,443,444,445,446,447,448,449,10002,10006,10007,10008,10009,10010,10011,10012,10014,10020,2102
        };

        void client_Error(object sender, Krs.Ats.IBNet.ErrorEventArgs e)
        {

            //if(e.ErrorCode = ErrorMessage.FailSendOrder)
            logger.Error(string.Format("IBClient error tickerid:{0} code:{1} message:{2}", e.TickerId, e.ErrorCode, e.ErrorMsg));
            
            int code = Convert.ToInt32(e.ErrorCode.ToString());

            if (InvalidatingCodes.Contains(code))
            {
                Order lo = LocalID2Order(e.TickerId.ToString());//找到对应的本地委托
                if (lo == null) return;
                Order fatherOrder = SonID2FatherOrder(lo.id);
                if (fatherOrder == null) return;

                lo.Status = QSEnumOrderStatus.Reject;
                tk.GotOrder(lo); //Broker交易信息管理器
                this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

                RspInfo info = new RspInfoImpl();
                info.ErrorID = (int)e.ErrorCode;
                info.ErrorMessage = string.Format("IB Order Error:{0}", code);
                fatherOrder.Status = QSEnumOrderStatus.Reject;
                fatherOrder.Comment = string.Format("IB Order Error:{0}", code);
                NotifyOrderError(fatherOrder, info);
            }

            //标注TWS重连事件 当连接断开时不允许发单
            if (code == 1100)
            {
                _connlost = true;
            }
            if (code == 1102 || code == 1101)
            {
                _connlost = false;
            }
            //switch (code)
            //{
                    
            //    case "135"://撤单时找不到该委托
            //        {
            //            Order lo = LocalID2Order(e.TickerId.ToString());//找到对应的本地委托
            //            if (lo == null) return;
            //            Order fatherOrder = SonID2FatherOrder(lo.id);
            //            if (fatherOrder == null) return;

            //            OrderAction action = new OrderActionImpl();
            //            action.Account = fatherOrder.Account;
            //            action.ActionFlag = QSEnumOrderActionFlag.Delete;
            //            action.Exchagne = "";
            //            action.Symbol = fatherOrder.Symbol;
            //            action.OrderID = fatherOrder.id;
            //            RspInfo info = new RspInfoImpl();
            //            info.ErrorID=(int)e.ErrorCode;
            //            info.ErrorMessage = "找不到编号为:"+e.TickerId.ToString()+"的委托";
            //            NotifyOrderOrderActionError(action, info);

            //            lo.Status = QSEnumOrderStatus.Reject;
            //            //lo.Comment = e.ErrorMsg;
            //            tk.GotOrder(lo); //Broker交易信息管理器
            //            this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

            //            fatherOrder.Status = QSEnumOrderStatus.Reject;
            //            fatherOrder.Comment = "找不到编号为:" + e.TickerId.ToString() + "的委托";
            //            NotifyOrder(fatherOrder);
            //            NotifyOrderError(fatherOrder, info);

            //            return;
            //        }
                
            //        {

            //            Order lo = LocalID2Order(e.TickerId.ToString());//找到对应的本地委托
            //            if (lo == null) return;
            //            Order fatherOrder = SonID2FatherOrder(lo.id);
            //            if (fatherOrder == null) return;

            //            lo.Status = QSEnumOrderStatus.Reject;
            //            //lo.Comment = e.ErrorMsg;
            //            tk.GotOrder(lo); //Broker交易信息管理器
            //            this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

            //            RspInfo info = new RspInfoImpl();
            //            info.ErrorID = (int)e.ErrorCode;
            //            info.ErrorMessage = "找不到对应的合约";
            //            fatherOrder.Status = QSEnumOrderStatus.Reject;
            //            fatherOrder.Comment = "找不到对应的合约";
            //            NotifyOrderError(fatherOrder, info);


            //            return;
            //        }
            //    default:
            //        break;
            //}
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
