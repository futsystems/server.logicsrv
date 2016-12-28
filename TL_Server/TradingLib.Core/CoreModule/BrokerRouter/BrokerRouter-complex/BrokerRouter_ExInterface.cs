using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    /// <summary>
    /// 发送委托和取消委托部分
    /// </summary>
    public partial class BrokerRouter
    {

        public void SendOrder(BinaryOptionOrder o)
        {
            try
            {
                IBroker broker = TLCtxHelper.ServiceRouterManager.DefaultSimBOBroker;
                if (o.Status != EnumBOOrderStatus.Reject)
                {
                    logger.Info("Send BO-Order To Broker Side:" + o.ToString());
                    if (broker != null && broker.IsLive)
                    {
                        //o.Broker = broker.Token;//通过Broker发送委托时,将Token设定到委托对应字段
                        broker.SendOrder(o);
                        //接口侧提交委托异常
                        if (o.Status == EnumBOOrderStatus.Reject)
                        {
                            //GotOrderErrorNotify(o, "EXECUTION_BROKER_PLACEORDER_ERROR");
                        }
                        //return true;
                    }
                    else
                    {
                        //如果没有交易通道则拒绝该委托
                        o.Status = EnumBOOrderStatus.Reject;
                        //errorTitle = "EXECUTION_BROKER_NOT_FOUND";
                        //logger.Warn("没有可以交易的通道 |" + o.GetOrderInfo());
                        //return false;
                    }

                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        /// <summary>
        /// 向Broker发送Order,TradingServer统一通过 BrokerRouter 发送委托,BrokerRouter 则在本地按一定的规则找到对应的
        /// 交易接口将委托发送出去
        /// SendOrder->RouterSendOrder->[EngineSendOrder,BrokerSendOrder,XBrokerSendOrder]
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order order)
        {
            try
            {
                //发送委托前执行复制 MsgExch处复制的委托由ClearCentre负责维护并更新状态,OrderRouter处复制的委托通过Broker下单并记录为FatherOrder 接口返回更新状态后 复制后丢入BrokerRouter队列 进行对外处理 
                //这样可以确保ClearCentre委托状态与下单委托状态不干扰,同时接口处返回的状态直接复制后进行处理，避免接口状态更新对中间状态干扰
                Order o = new OrderImpl(order);
                if (o.Status != QSEnumOrderStatus.Reject)
                {
                    logger.Info("Route Order To Broker Side:" + o.GetOrderInfo());
                    string errorTitle = string.Empty;
                    bool ret = SendOrderOut(o, out errorTitle);
                    if (ret)
                    {
                        Broker_GotOrder(o);
                    }
                    else
                    {
                        RspInfo info = RspInfoEx.Fill(errorTitle);
                        o.Comment = info.ErrorMessage;
                        Broker_GotOrderError(o, info);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BrokerRouter Send Order Error:" + (order == null ? "Null" : order.ToString()));
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long val)
        {
            try
            {
                logger.Info("Route Cancel to Broker Side:" + val.ToString());
                Order o = TLCtxHelper.ModuleClearCentre.SentOrder(val);//通过orderID在清算中心找到对应的Order
                bool splited = this.IsOrderSplited(o);//判断委托是否被分拆
                if (!splited)
                {
                    BrokerCancelOrder(o);
                }
                else //如果委托分拆过则通过分拆器取消委托
                {
                    _splittracker.CancelFatherOrder(val);//
                }
            }
            catch (Exception ex)
            {
                logger.Error("BrokerRouter CancelOrder Error:" + val.ToString());
                logger.Error(ex);
            }
        }


        /// <summary>
        /// broker级发送委托 这里按照委托类型可能有委托处于presubmit状态
        /// 也就是系统内模拟的委托类型，处于监听中待条件出发向路由发送
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorTitle"></param>
        /// <returns></returns>
        //bool RouterSendOrder(Order o, out string errorTitle)
        //{

        //    //如果是内部模拟的委托类型则通过委托模拟器发送 其余则通过实际路由发送
        //    return SendOrderOut(o, out errorTitle);

        //    //检查TIF设定，然后根据需要是否要通过TIFEngine来处理Order
        //    //switch (o.TimeInForce)
        //    //{
        //    //    case QSEnumTimeInForce.DAY:
        //    //        return route_SendOrder(o,out errorTitle);
        //    //    default:
        //    //        _tifengine.SendOrder(o);//问题1:相关交易通道没有开启但是我们已经将委托交由TIFEngine管理，会产生屡次取消或者取消错误
        //    //        break;
        //    //}
        //}


        /// <summary>
        /// 路由发送委托 该委托为直接的父委托或子委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorTitle"></param>
        /// <returns></returns>
        bool BrokerSendOrder(Order o, out string errorTitle)
        {
            //logger.Info("Select Broker Send Order Out");
            errorTitle = string.Empty;
            try
            {
                //这里可以设计 路由返回机制 从而实现从而实现容错处理
                IBroker broker = SelectBroker(o);
                if (broker != null && broker.IsLive)
                {
                    o.Broker = broker.Token;//通过Broker发送委托时,将Token设定到委托对应字段
                    broker.SendOrder(o);
                    //接口侧提交委托异常
                    if (o.Status == QSEnumOrderStatus.Reject)
                    {
                        errorTitle = "EXECUTION_BROKER_PLACEORDER_ERROR";
                        return false;
                    }
                    return true;
                }
                else
                {
                    //如果没有交易通道则拒绝该委托
                    o.Status = QSEnumOrderStatus.Reject;
                    errorTitle = "EXECUTION_BROKER_NOT_FOUND";
                    logger.Warn("没有可以交易的通道 |" + o.GetOrderInfo());
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("BrokerSendOrder Error:" + ex.ToString());
                o.Status = QSEnumOrderStatus.Reject;
                errorTitle = "EXECUTION_BROKER_PLACEORDER_ERROR";
                return false;
            }
        }




        void BrokerCancelOrder(Order o)
        {
            IBroker broker = SelectBroker(o,true);
            if (broker != null && broker.IsLive)
            {
                broker.CancelOrder(o.id);
            }
            else
            {
                //如果没有交易通道则拒绝该委托
                o.Status = QSEnumOrderStatus.Reject;
                RspInfo info = RspInfoEx.Fill("EXECUTION_BROKER_NOT_FOUND");
                o.Comment = info.ErrorMessage;
                Broker_GotOrderError(o, info);
                logger.Warn(PROGRAME + ":没有可以交易的通道 |" + o.ToString());
            }
        }
    }
}
