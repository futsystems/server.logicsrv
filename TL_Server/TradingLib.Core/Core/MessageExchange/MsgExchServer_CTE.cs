﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Core
{
    public partial class MsgExchServer
    {

        #region 状态类查询
        //[CoreCommandAttr(QSEnumCommandSource.MessageWeb,
        //                    "qrydatafeedrouterstatus",
        //                    "qrydatafeedrouterstatus - query datafeed router status",
        //                    "查询行情路由状态")]
        //public string Status_DataFeedRouter()
        //{
        //    DataFeedRouterStatus status = _datafeedRouter.GetRouterStatus();
        //    ReplyWriter writer = new ReplyWriter();
        //    string json = writer.Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(status).End().ToString();
        //    debug("got json router status:" + json, QSEnumDebugLevel.INFO);
        //    return json;
        //}

        #endregion

        [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "pconnlist",
                            "pconnlist - print client connection list",
                            "输出交易系统内的有效连接列表")]
        public string PrintConnectionList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TotalConnection:" + tl.NumClients + " Logedin:" + tl.NumClientsLoggedIn + ExComConst.Line);
            //foreach (IClientInfo info in tl.Clients)
            //{
            //    sb.Append(ExUtil.ClientInfo2Str(info));
            //}
            return sb.ToString();

        }

        [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "demotick",
                            "demotick - send demotick to system",
                            "向系统输出模拟tick数据 用于系统调试")]
        public string DemoTick(decimal lastsettle , decimal settleprice)
        {
            _datafeedRouter.DemoTick(lastsettle,settleprice);
            return "DemoTick Send";
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "printtick",
                            "printtick - print tick snapshot in memory",
                            "打印当前系统的行情快照")]
        public string PrintTickSnapshot()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tick k in _datafeedRouter.GetTickSnapshot())
            {
                if(k!= null && k.isValid)
                    sb.Append(TickImpl.Serialize(k)+Environment.NewLine);
            }
            return sb.ToString();
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "printtimespans",
                            "printtimespans - print timespans",
                            "打印当前市场交易小节")]
        public string PrintTimeSpans()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MktTime ent in BasicTracker.MarketTimeTracker.GetMarketTimes())
            {
                sb.Append("Start:" + ent.StartTime.ToString() + " - End:" + ent.EndTime.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

         [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "excludesym",
                            "excludesym - 排除行情系统的合约",
                            "从行情系统排除某个合约")]
        public void ExcludeSymbol(string symbol)
        {
            _datafeedRouter.ExcludeSymbol(symbol);
        }

         [CoreCommandAttr(QSEnumCommandSource.CLI,
                    "includesym",
                    "includesym - 包含行情系统的合约",
                    "从行情系统包含某个合约")]
         public void IncludeSymbol(string symbol)
         {
             _datafeedRouter.IncludeSymbol(symbol);
         }


        [CoreCommandAttr(QSEnumCommandSource.CLI,
                            "regsymbols",
                            "regsymbols - 订阅所有合约行情数据",
                            "订阅所有合约行情数据")]
        public string CTE_RegisterSymbols()
        {

            //_datafeedRouter.RegisterSymbols(BasicTracker.SymbolTracker.getBasketAvabile());

            return "registed";

        }

        //[TaskAttr("重置行情与成交路由", 16, 00, 5, "重置行情与成交路由")]
        public void Reset()
        {
            debug("重置行情与成交路由", QSEnumDebugLevel.INFO);
            _brokerRouter.Reset();
            _datafeedRouter.Reset();
        }


        /// <summary>
        /// 用于检查状体异常的委托
        /// 当委托处于unknown,placed,submited但是没有随着委托的进行 跟新为rejected,canceled,filled,partfilled
        /// 
        /// 当前系统架构 统一在Broker处维护委托状态
        /// 这里主要是通过外界返回的状态与内部委托状态进行同步，并非完全通过sentsize fillsize cancel进行状态切换与更新
        /// 后期这里会做改造
        /// broker->clearcentre->msgexch获得对应的委托然后再发送到客户端
        /// 
        /// simbroker加载委托只加载open partfill2种状态的委托
        /// 正常委托没有unknown状态 unknown状态的委托标识刚从委托源提交
        /// 经过1，2级风控检查后委托状态会变更为reject,placed
        /// 风控通过后会通过brokerrouter sendorder正常执行后委托变成submited
        /// 当broker获得委托返回后 系统更新order状态为opened 或者 canceled
        /// 然后委托进入等待成交阶段
        /// 
        /// </summary>
        [TaskAttr("交易中心委托状态检查",15, "交易中心委托状态检查")]
        public void CTE_OrderStatusCheck()
        {
            if (!TLCtxHelper.IsReady) return;
            //debug("检查异常状态的委托....", QSEnumDebugLevel.INFO);
            DateTime now = DateTime.Now;
            //遍历所有需要检查的委托 停留在placed 或者 submited unknown
            foreach (Order o in _clearcentre.TotalOrders.Where(o => statuscheck(o, now)))
            {
                IAccount acc = _clearcentre[o.Account];
                if (acc != null)
                {
                    AccountBase account = acc as AccountBase;
                    int sentsize = account.SentSize(o.id);
                    int fillsize = account.FilledSize(o.id);
                    bool iscancel = account.IsCanceled(o.id);
                    bool iscomplete = account.IsComplate(o.id);
                    debug("OrderStatus:" + o.Status.ToString() + " SentSize:" + sentsize.ToString() + " FillSize:" + fillsize.ToString() + " IsCanceled:" + iscancel.ToString() + " IsComplete:" + iscomplete.ToString(), QSEnumDebugLevel.INFO);
                    Order tmp = new OrderImpl(o);
                    //将全部成交或部分成交的委托进行补充处理
                    //全部成交
                    if (iscomplete)
                    {
                        tmp.Status = QSEnumOrderStatus.Filled;tmp.Size = 0;tmp.FilledSize = fillsize;
                        tmp.Comment = "全部成交(维)";
                        ReplyOrder(tmp);
                        continue;
                    }
                    //如果可能存在委托留在成交侧的，需要发送一次撤单指令 这样如果有委托在成交侧，可以撤单维持分帐户侧和成交侧状态一致
                    //部分成交
                    if (Math.Abs(fillsize) > 0 && Math.Abs(fillsize) < Math.Abs(sentsize))
                    {
                        tmp.Status = QSEnumOrderStatus.Canceled;tmp.Size = (sentsize - fillsize) * (tmp.Side ? 1 : -1);tmp.FilledSize = fillsize;
                        tmp.Comment = "部分成交(维)";
                        ReplyOrder(tmp);
                        //撤单
                        _brokerRouter.CancelOrder(o.id);
                        continue;
                    }
                    //未成交
                    if (fillsize == 0)
                    {
                        tmp.Status = QSEnumOrderStatus.Canceled;
                        tmp.Comment = "拒绝(维)";
                        ReplyOrder(tmp);
                        //撤单
                        _brokerRouter.CancelOrder(o.id);
                        continue;
                    }
                    //异常合约
                    if (sentsize == 0)
                    {
                        //标注委托状态
                        tmp.Status = QSEnumOrderStatus.Reject;
                        tmp.Comment = "拒绝(维)";
                        ReplyOrder(tmp);
                        //撤单
                        _brokerRouter.CancelOrder(o.id);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 返回提交委托5秒后仍然在Placed,Submited,Unknown状态的委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        bool statuscheck(Order o,DateTime now)
        {
            //委托状态为placed submited 并且 时间超过5秒，则该委托需要进入异常检查
            if (o.Status == QSEnumOrderStatus.Placed || o.Status == QSEnumOrderStatus.Submited || o.Status == QSEnumOrderStatus.Unknown)
            {
                if (now.Subtract(Util.ToDateTime(o.Date, o.Time)).TotalSeconds >= 5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
