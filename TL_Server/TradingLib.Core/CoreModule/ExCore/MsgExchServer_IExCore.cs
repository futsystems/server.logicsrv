using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MsgExchServer
    {

        public int OnLineTerminalNum
        {
            get
            {
                return tl.NumClientsLoggedIn;
            }
        }
        /// <summary>
        /// 绑定唯一的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void AssignOrderID(ref Order o)
        {
            if (o.id <= 0)
                o.id = _orderIDTracker.AssignId;
            //获得本地递增流水号
            if (o.OrderSeq <= 0)
                o.OrderSeq = this.NextOrderSeq;
        }

        public void AssignTradeID(ref Trade f)
        {
            //系统本地给成交赋日内唯一流水号 成交端的TradeID由接口负责
            f.TradeID = this.NextTradeID.ToString();
        }


        /// <summary>
        /// 对外提供取消委托操作
        /// 管理端取消委托
        /// </summary>
        /// <param name="val"></param>
        public void CancelOrder(long val)
        {
            try
            {
                logger.Info("Got CancelOrder :" + val);
                Order o = TLCtxHelper.ModuleClearCentre.SentOrder(val);
                //如果委托处于pending状态
                if (o.IsPending())
                {
                    //如果委托状态表面需要通过broker来取消委托 则通过broker来进行撤单
                    if (o.CanCancel())//opened partfilled
                    {
                        TLCtxHelper.ModuleBrokerRouter.CancelOrder(o.id);
                    }
                    else if (o.Status == QSEnumOrderStatus.Submited)//已经通过broker提交 该状态无法立即撤单 需要等待委托状态更新为Opened或者 被定时程序发现是一个错误委托
                    {
                        logger.Info("委托:" + val.ToString() + "处于Submited,等待broker返回");
                    }
                    else if (o.Status == QSEnumOrderStatus.Placed)//
                    {
                        logger.Info("委托:" + val.ToString() + "处于Placed,等待系统返回");
                    }
                }
                else
                {
                    logger.Info("委托:" + val.ToString() + "不可撤销");
                }

            }
            catch (Exception ex)
            {
                logger.Error("取消委托出错:" + ex.ToString());
                throw (new QSTradingServerCancleError(ex));
            }
        }


        /// <summary>
        /// A:对外提供委托操作
        /// 暴露给 客户端快捷指令
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            OrderRequestHandler(null,o, false, true);//外部委托
        }
        /// <summary>
        /// B:直接向接口下单(跳过部分风控检查 账户冻结检查,强平时间外交易时间检查,帐户自定义委托检查等)
        /// 该函数只暴露给风控中心 用于执行强平
        /// </summary>
        /// <param name="o"></param>
        public void SendOrderInternal(Order o)
        {
            OrderRequestHandler(null,o, true, true);//内部委托
        }
    }
}
