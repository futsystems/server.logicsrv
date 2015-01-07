using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.Common;

using System.Diagnostics;//记得加入此引用
using System.Collections.Concurrent;
using TradingLib.API;
using System.Reflection;
using System.Threading;

namespace TradingLib.Core
{
    

    //服务端风险控制模块,根据每个账户的设定，实时的检查Order是否符合审查要求予以确认或者决绝
    public partial class RiskCentre : BaseSrvObject, IRiskCentre,ICore
    {
        const string CoreName = "RiskCentre";
        /// <summary>
        /// 清算中心
        /// </summary>
        ClearCentre _clearcentre = null;
        public string CoreId { get { return CoreName; } }

        ConfigDB _cfgdb;

        bool _marketopencheck = true;
        public bool MarketOpenTimeCheck { get { return _marketopencheck; } }

        int _orderlimitsize = 10;
        string commentNoPositionForFlat = "无可平持仓";
        string commentOverFlatPositionSize = "可平持仓数量不足";

        public RiskCentre(ClearCentre clearcentre):base(CoreName)
        {
            _clearcentre = clearcentre;

            //1.加载配置文件
            _cfgdb = new ConfigDB(RiskCentre.CoreName);
            if (!_cfgdb.HaveConfig("MarketOpenTimeCheck"))
            {
                _cfgdb.UpdateConfig("MarketOpenTimeCheck", QSEnumCfgType.Bool,"True", "是否进行时间检查,交易日/合约交易时间段等");
            }
            //是否执行合约开市检查
            _marketopencheck = _cfgdb["MarketOpenTimeCheck"].AsBool();
            

            if (!_cfgdb.HaveConfig("OrderLimitSize"))
            {
                _cfgdb.UpdateConfig("OrderLimitSize", QSEnumCfgType.Int, 50, "单笔委托最大上限");
            }
            //最大委托数量
            _orderlimitsize = _cfgdb["OrderLimitSize"].AsInt();

            if (!_cfgdb.HaveConfig("CommentNoPositionForFlat"))
            {
                _cfgdb.UpdateConfig("CommentNoPositionForFlat", QSEnumCfgType.String,"无可平持仓", "无可平持仓消息");
            }
            commentNoPositionForFlat = _cfgdb["CommentNoPositionForFlat"].AsString();

            if (!_cfgdb.HaveConfig("CommentOverFlatPositionSize"))
            {
                _cfgdb.UpdateConfig("CommentOverFlatPositionSize", QSEnumCfgType.String, "可平持仓数量不足", "可平持仓数量不足消息");
            }
            commentOverFlatPositionSize = _cfgdb["CommentOverFlatPositionSize"].AsString();

            if (!_cfgdb.HaveConfig("FlatSendOrderFreq"))
            {
                _cfgdb.UpdateConfig("FlatSendOrderFreq", QSEnumCfgType.Int, 3, "强平重试时间间隔");
            }
            SENDORDERDELAY = _cfgdb["FlatSendOrderFreq"].AsInt();

            if (!_cfgdb.HaveConfig("FlatSendOrderRetryNum"))
            {
                _cfgdb.UpdateConfig("FlatSendOrderRetryNum", QSEnumCfgType.Int, 3, "强平重试次数");
            }
            SENDORDERRETRY = _cfgdb["FlatSendOrderRetryNum"].AsInt();

            


            //订阅持仓回合关闭事件
            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new PositionRoundClosedDel(GotPostionRoundClosed);

            //加载风空规则
            LoadRuleClass();

            //初始化日内平仓任务
            InitFlatTask();
        }

        /// <summary>
        /// 行情驱动止盈止损监控器进行工作,满足条件对外触发平仓指令
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            //_posoffsetracker.GotTick(k);
        }



        #region 【服务端止盈止损】客户端提交上来的持仓止盈止损 服务端检查
        /// <summary>
        /// 获得取消
        /// 取消事务是在处理队列中进行异步处理
        /// 而强平事务的完成是在positionround回报中同步处理
        /// </summary>
        /// <param name="oid"></param>
        public void GotOrder(Order o)
        {
            if (o.Status == QSEnumOrderStatus.Canceled)
            {
                long oid = o.id;
                foreach (RiskTaskSet ps in posflatlist)
                {

                    switch (ps.TaskType)
                    {

                        case QSEnumRiskTaskType.FlatPosition:
                            {
                                //如果不需要先撤单的 则跳过
                                if (!ps.NeedCancelFirst)
                                {
                                    if (ps.OrderID == oid)
                                        ps.OrderID = 0;
                                }
                                else//需要先撤单的 则检查撤单列表
                                {
                                    //如果待成交委托列表中包含对应的ID则先删除该委托
                                    if (ps.PendingOrders.Contains(oid))
                                    {
                                        ps.PendingOrders.Remove(oid);
                                    }
                                    //如果所有待成交委托均撤单完成 则标志canceldone
                                    if (ps.PendingOrders.Count == 0)
                                    {
                                        ps.CancelDone = true;
                                    }
                                }
                            }
                            break;
                        case QSEnumRiskTaskType.CancelOrder:
                        case QSEnumRiskTaskType.FlatAllPositions:
                            {
                                if (ps.OrderCancels.Contains(oid))
                                {
                                    ps.OrderCancels.Remove(oid);
                                }
                                if (ps.OrderCancels.Count == 0)
                                {
                                    ps.CancelDone = true;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 响应交易服务返回过来的ErrorOrder
        /// 比如风控中心强平 发送委托 但是委托被拒绝，则需要对该事件进行响应
        /// 否则超时后 会出现强平系统无法正常撤单的问题。而无法正常撤单则没有撤单回报,导致强平系统一直试图撤单
        /// </summary>
        /// <param name="error"></param>
        public void GotOrderError(Order order,RspInfo info)
        {
            debug("Got Orrder Error ID:" + order.id.ToString(), QSEnumDebugLevel.INFO);
            foreach (RiskTaskSet ps in posflatlist)
            {
                //如果委托被拒绝 并且委托ID是本地发送过去的ID 则将positionflatset的委托ID置0
                if (ps.OrderID == order.id && order.Status == QSEnumOrderStatus.Reject)
                    ps.OrderID = 0;
            }
        }

        /// <summary>
        /// 当有持仓平调后 遍历当地平仓事务列表，如果在列表中 则直接删除该平仓事务,表明该平仓事务已经完成
        /// 注意平仓事务列表为线程安全的list可以同时被多个线程操作
        /// 如果统一在processpostionflat中检查持仓情况会出现以下问题：如果持仓平调 但是在平调后立马再次开仓，此时PostioinFlat并没有从队列中删除，当再次扫描到该持仓时 系统会认为该持仓没有被及时平调,从而尝试撤单并重新强平，最后单子又无法撤单成功
        /// 
        /// 只有在FlatPosition类型的任务中真正触发委托
        /// 平仓任务出队在持仓回合中进行
        /// 取消任务队列在处理程序中进行
        /// 
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="pos"></param>
        void GotPostionRoundClosed(PositionRound pr, Position pos)
        { 
            string key = pos.GetPositionKey();

            RiskTaskSet[] list = posflatlist.Where(task => task.TaskType == QSEnumRiskTaskType.FlatPosition && task.Position.GetPositionKey().Equals(key)).ToArray();

            foreach (RiskTaskSet tmp in list)
            {
                debug("Position:" + tmp.Position.GetPositionKey() + " 已经平掉,从队列中移除", QSEnumDebugLevel.INFO);
                posflatlist.Remove(tmp);
                //通过事件中继触发事件
                if (PositionFlatEvent != null)
                {
                    PositionFlatEvent(this, new PositionFlatEventArgs(tmp.Position));
                }
            }
        }

        #endregion

       

        #region 重置
        /// <summary>
        /// 风空中心重置风控规则
        /// </summary>
        public void Reset()
        {
            debug("风控中心重置", QSEnumDebugLevel.INFO);
            //清空强平任务队列
            posflatlist.Clear();

            //清空帐户风控检查帐户列表
            ClearActiveAccount();

            Notify("风控中心重置(结算后)[" + DateTime.Now.ToShortDateString() + "]", " ");
        }

        #endregion

        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            //_posoffsetracker.Dispose();
            
        }
    }


    
    


}
