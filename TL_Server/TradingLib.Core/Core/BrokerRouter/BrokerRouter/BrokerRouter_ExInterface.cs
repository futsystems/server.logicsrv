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

        #region 本地向Broker发情的交易请求
        /// <summary>
        /// 向Broker发送Order,TradingServer统一通过 BrokerRouter 发送委托,BrokerRouter 则在本地按一定的规则找到对应的
        /// 交易接口将委托发送出去
        /// SendOrder->RouterSendOrder->[EngineSendOrder,BrokerSendOrder,XBrokerSendOrder]
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            try
            {
                //检查通过,则通过该broker发送委托 拒绝的委托通过 ordermessage对外发送
                if (o.Status != QSEnumOrderStatus.Reject)
                {
                    //按照委托方式发送委托直接发送或通过本地委托模拟器进行发送
                    debug("Send  Order To Broker Side:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    //这里需要针对委托进行判断 如果需要拆分 则将委托拆分成多个委托然后统一对外提交
                    //模拟成交不用拆分 实盘委托拆分，如何在多个成交帐户间进行路由，是否也需要像单帐户一样，做单边趋势
                    //比如帐户A有多头，则卖出操作优先路由到A帐户进行平仓，帐户B有空头,则买入操作优先路由到B帐户进行平仓
                    //在帐户A中的买入委托 是否必须在A帐户中进行平仓？还是可以在B帐户中进行平仓(如果A帐户和B帐户都是启用净持仓)
                    string errorTitle = string.Empty;
                    bool ret = RouterSendOrder(o, out errorTitle);
                    //如果委托正常提交 则对外回报委托
                    if (ret)
                    {
                        Broker_GotOrder(o);
                    }
                    //如果提交委托异常,则回报委托错误
                    else
                    {
                        GotOrderErrorNotify(o, errorTitle);
                    }
                    //调用broker_sendorder对外发送委托 如果委托状态为拒绝 则表明委托被broker_send部分拒绝了，我们不用再标记委托状态为submited 因此不用再进入下一步
                    //if (o.Status != QSEnumOrderStatus.Reject)
                    //{
                    //    //o.Status = QSEnumOrderStatus.Submited;//委托状态修正为 已经提交到Broker
                    //    GotOrder(o);
                    //}
                    //
                    //GotOrder(o);
                }
            }
            catch (Exception ex)
            {
                debug("BrokerRouter Send Order Error:" + (o == null ? "Null" : o.ToString()), QSEnumDebugLevel.ERROR);
                debug(ex.ToString());
            }
        }

        /// <summary>
        /// broker级发送委托 这里按照委托类型可能有委托处于presubmit状态
        /// 也就是系统内模拟的委托类型，处于监听中待条件出发向路由发送
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorTitle"></param>
        /// <returns></returns>
        bool RouterSendOrder(Order o, out string errorTitle)
        {

            //如果是内部模拟的委托类型则通过委托模拟器发送 其余则通过实际路由发送
            return SendOrderOut(o, out errorTitle);

            //检查TIF设定，然后根据需要是否要通过TIFEngine来处理Order
            //switch (o.TimeInForce)
            //{
            //    case QSEnumTimeInForce.DAY:
            //        return route_SendOrder(o,out errorTitle);
            //    default:
            //        _tifengine.SendOrder(o);//问题1:相关交易通道没有开启但是我们已经将委托交由TIFEngine管理，会产生屡次取消或者取消错误
            //        break;
            //}
        }


        /// <summary>
        /// 路由发送委托 该委托为直接的父委托或子委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorTitle"></param>
        /// <returns></returns>
        bool BrokerSendOrder(Order o, out string errorTitle)
        {
            debug("BrokerSendOrder select broker and send through broker", QSEnumDebugLevel.INFO);
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
                    debug("没有可以交易的通道 |" + o.GetOrderInfo(), QSEnumDebugLevel.WARNING);
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":向broker发送委托错误:" + ex.ToString() + ex.StackTrace.ToString(), QSEnumDebugLevel.ERROR);
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
                GotOrderErrorNotify(o, "EXECUTION_BROKER_NOT_FOUND");
                debug(PROGRAM + ":没有可以交易的通道 |" + o.ToString(), QSEnumDebugLevel.WARNING);
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
                debug(PROGRAM + ":Route Cancel to Broker Side:" + val.ToString(), QSEnumDebugLevel.INFO);
                RouterCancelOrder(val);
            }
            catch (Exception ex)
            {
                debug("BrokerRouter CancelOrder Error:" + val.ToString(), QSEnumDebugLevel.ERROR);
                debug(ex.ToString());
            }

        }
        //通过路由选择器将委托取消发送出去
        void RouterCancelOrder(long val)
        {
            try
            {
                //debug("取消委托到这里...", QSEnumDebugLevel.MUST);
                Order o = TLCtxHelper.CmdTotalInfo.SentOrder(val);//通过orderID在清算中心找到对应的Order
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
                debug(PROGRAM + ":向broker发送取消委托出错:" + ex.ToString() + ex.StackTrace.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        #endregion
    }
}
