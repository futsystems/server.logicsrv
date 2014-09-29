using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace TradingLib.Core
{
    internal class PositionFlatSet
    {
        /// <summary>
        /// 委托来源
        /// </summary>
        public QSEnumOrderSource Source { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ForceCloseReason { get; set; }

        /// <summary>
        /// 待平持仓
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 是否已经发送
        /// </summary>
        public bool FlatSent { get; set; }
        /// <summary>
        /// 平仓指令发送时间
        /// </summary>
        public DateTime SentTime { get; set; }

        /// <summary>
        /// 平仓指令发送次数
        /// </summary>
        public int FireCount { get; set; }

        /// <summary>
        /// 发送的平仓委托OrderID
        /// </summary>
        public long OrderID { get; set; }

        /// <summary>
        /// 强迫异常或失效通知
        /// </summary>
        public bool FlatFailNoticed { get; set; }

        List<long> _pendingorders = new List<long>();

        /// <summary>
        /// 是否需要先取消委托
        /// </summary>
        public bool NeedCancelFirst { get; set; }
        /// <summary>
        /// 取消已经发送
        /// </summary>
        public bool CancelSent { get; set; }
        /// <summary>
        /// 取消已经完成
        /// </summary>
        public bool CancelDone { get; set; }

        /// <summary>
        /// 待撤单委托编号
        /// 需要在平仓前撤掉的委托
        /// </summary>
        public List<long> PendingOrders 
        {
            get
            {   
                return _pendingorders;
            }

            set
            {
                _pendingorders = value;
                //如果待成交委托数量大于0 则需要先取消
                if (_pendingorders.Count > 0)
                {
                    NeedCancelFirst = true;
                }
            }
        
        }

        /// <summary>
        /// 平仓事务
        /// 强平某个持仓pos
        /// 其对应的待成交委托为pendingorders
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pendingorders"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public PositionFlatSet(Position pos,List<long> pendingorders, QSEnumOrderSource source, string closereason)
        {
            //持仓数据
            Position = pos;
            //待平仓数据
            this.PendingOrders = pendingorders;
            this.CancelDone = false;
            this.CancelSent = false;

            FlatSent = false;
            SentTime = DateTime.Now;
            FireCount = 1;
            Source = source;
            ForceCloseReason = closereason;
            FlatFailNoticed = false;
        }

        public override string ToString()
        {
            return "Pos:" + Position.Account + "-" + Position.Symbol + " OID:" + OrderID.ToString() + " FireCount:" + FireCount.ToString() + " SentTime:" + SentTime.ToString();
        }
    }


    /// <summary>
    /// 强平与撤单部分 最后会封装在队列中进行操作由统一的线程对外进行发送
    /// 避免多个节点调用发单或者撤单出现问题
    /// </summary>
    public partial class RiskCentre
    {

        /// <summary>
        /// 强迫成功事件
        /// </summary>
        public event PositionDelegate GotFlatSuccessEvent;

        /// <summary>
        /// 强迫异常事件
        /// </summary>
        public event PositionDelegate GotFlatFailedEvent;






        //待平仓列表,主要包含系统尾盘集中强平,系统内部风控强平等形成的平仓指令
        ThreadSafeList<PositionFlatSet> posflatlist = new ThreadSafeList<PositionFlatSet>();
        
        #region【持仓强平的循环检查与维护】

        /// <summary>
        /// 检查某个持仓是否在强平队列
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsPosFlatPending(Position pos)
        {
            //flatset = null;
            string key = pos.GetPositionKey();

            foreach (PositionFlatSet ps in posflatlist)
            {
                if (ps.Position.GetPositionKey().Equals(key))
                {
                    //flatset = ps;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 平掉某个账户的所有仓位(风控里面的 强平并冻结账户)
        /// 注,这里需要封装发单方式,若系统还有未成交的合约,我们需要先撤掉所有委托,然后再发新的委托
        /// 注:Brokerrouter内部 市价平仓有自动撤单机制,平仓是市价委托 因此可以自动提交由br进行撤单并平仓
        /// 但是平仓后 原来的建仓委托单可能没有被撤出，从而再次建仓。
        /// 1.指定要平仓的账户
        /// 2.指定该强平操作所产生委托所标识的Source
        /// 3.对该委托的标注
        /// 4.指定是否需要撤销该帐户的其他所有委托
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(string accid, QSEnumOrderSource source, string closereason = "系统强平")
        {
            //debug("平掉账户:" + accid + "所有仓位", QSEnumDebugLevel.INFO);
            IAccount account = _clearcentre[accid];
            foreach (Position pos in account!=null?account.Positions:new Position[]{})//遍历该账户的所有仓位 若不是空仓则市价平仓
            {
                if (!pos.isFlat)
                {
                    FlatPosition(pos, source, closereason);
                }
                Thread.Sleep(100);
            }
        }


        /// <summary>
        /// 通过风控中心平掉某个持仓
        /// </summary>
        /// <param name="pos">持仓对象</param>
        /// <param name="ourdersource">委托源</param>
        /// <param name="comment">平仓备注</param>
        /// <returns></returns>
        //public void FlatPosition(Position pos, QSEnumOrderSource ordersource, string closereason = "系统强平")
        //{
        //    FlatPosition(pos, ordersource, closereason, true);
        //}

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ordersource"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(Order order, QSEnumOrderSource ordersource, string cancelreason = "系统强平")
        {
            //撤掉某个委托
            this.CancelOrder(order.id);
            Util.sleep(10);
        }

        /// <summary>
        /// 撤掉某个帐户的所有待成交委托
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="ordersouce"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(string accid,QSEnumOrderSource ordersouce, string cancelreason = "系统强平")
        {
            IAccount account = _clearcentre[accid];
            if (account != null)
            {
                foreach (Order o in account.GetPendingOrders())
                {
                    this.CancelOrder(o.id);
                    Util.sleep(10);
                }
            }
        }

        /// <summary>
        /// 撤掉某个交易帐户下的某个合约的所有委托
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="symbol"></param>
        /// <param name="ordersource"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(string accid, string symbol, QSEnumOrderSource ordersource, string cancelreason = "系统强平")
        {
            IAccount account = _clearcentre[accid];
            if (account != null)
            {
                foreach (Order o in account.GetPendingOrders(symbol))
                {
                    this.CancelOrder(o.id);
                    Util.sleep(10);
                }
            }
        }
        /// <summary>
        /// 强平进入队列进行操作
        /// 在强平过程中需要检查对应的持仓方向上是否有委托
        /// 如果有委托挂单需要先撤单然后再强平
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="closereason"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public void FlatPosition(Position pos, QSEnumOrderSource ordersource, string closereason)
        {
            
            //如果该持仓已经在平仓队列中则直接返回
            if (IsPosFlatPending(pos))
                return;
            IAccount account = _clearcentre[pos.Account];
            debug("RiskCentre Flatpostion:" + pos.ToString(), QSEnumDebugLevel.INFO);
            //生成持仓强平事务
            List<long> pendingorders = account.GetPendingOrders(pos.Symbol).Select(o=>o.id).ToList();
            PositionFlatSet ps = new PositionFlatSet(pos,pendingorders, ordersource, closereason);
            //放入列表中维护
            posflatlist.Add(ps);

            /*
            //生成市价委托
            Order o = new MarketOrderFlat(pos);
            o.Account = pos.Account;
            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
            o.OrderSource = ordersource;
            o.ForceClose = true;
            o.ForceCloseReason = closereason;
            
            //o.price = 2500;//模拟不成交延迟撤单的情况

            //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);

            BasicTracker.SymbolTracker.TrckerOrderSymbol(o);

            //如果该合约上有待成交委托 则需要延迟平仓 否则会出现平仓不足的问题。要等待撤掉所有委托之后才可以平仓
            //这里有一个操作权的问题,是平仓撤单还是显示的调用撤单命令。在目前情况下是都需要的
            //平仓只管当前仓位是否有委托，如果有待成交委托则进行撤单，如果其他合约没有持仓 但是有挂单，在尾盘强平时 就需要撤掉所有的委托
            
            //获得该合约的所有待成交委托
            account.GetPendingOrders(pos.Symbol);
            //对外发送委托
            SendOrder(o);

            //第一次强平 将持仓信组成flatset放入系统队列
            if (first)
            {
                //将持仓加入监控列表
                PositionFlatSet ps = new PositionFlatSet(pos, ordersource, closereason);
                ps.OrderID = o.id;
                posflatlist.Add(ps);
            }
            return o.id;
             * **/
        }

        /// <summary>
        /// 发送底层强平持仓委托
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        void SendFlatPositionOrder(PositionFlatSet set)
        {
            Position pos = set.Position;
            //生成市价委托
            Order o = new MarketOrderFlat(pos);
            o.Account = pos.Account;
            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
            o.OrderSource = set.Source;
            o.ForceClose = true;
            o.ForceCloseReason = set.ForceCloseReason;
            //o.price = 2500;//模拟不成交延迟撤单的情况

            //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);

            //绑定合约对象
            BasicTracker.SymbolTracker.TrckerOrderSymbol(o);

            set.FlatSent = true;
            set.FireCount++;
            set.SentTime = DateTime.Now;
            set.OrderID = o.id;
            SendOrder(o);
        }


        int SENDORDERDELAY = 3;
        int SENDORDERRETRY = 3;
        bool waitforcancel = true;

        
        /// <summary>
        /// 监控强平持仓列表,用于观察委托是否正常平仓
        /// 强平是一个触发过程
        /// 系统内部的帐户风控规则检查或者定时检查 扫描到需要强平的仓位或者帐户就会进行强平
        /// 执行强平后 对应的持仓将进入队列进行监控处理 直到强平成功或者异常
        /// </summary>
        void ProcessPositionFlat()
        {
            //debug("检查待平仓列表...", QSEnumDebugLevel.INFO);
            foreach (PositionFlatSet ps in posflatlist)
            {
                //没有发送过强平委托
                if (!ps.FlatSent)
                {
                    //如果是要先取消再平仓的 则需要发送取消委托
                    if (ps.NeedCancelFirst)
                    {
                        //如果没有发送过取消委托 则遍历所有委托 发送取消
                        if (!ps.CancelSent)
                        {
                            debug(ps.Position.GetPositionKey()+":没有发送强平委托,且需要先撤单，发送撤单", QSEnumDebugLevel.INFO);
                            foreach (long oid in ps.PendingOrders)
                            {
                                this.CancelOrder(oid);
                                Util.sleep(10);//取消委托
                            }
                            ps.CancelSent = true;//标注 取消已经发出
                            continue;
                        }

                        //如果取消委托已经发送并且已经完成 同时是第一次发送强迫指令 则执行强平
                        if (ps.CancelSent && ps.CancelDone)
                        {
                            debug(ps.Position.GetPositionKey() + ":已经发送撤单,并且所有委托已经撤掉,继续执行强平逻辑", QSEnumDebugLevel.INFO);
                            SendFlatPositionOrder(ps);
                            continue;
                        }
                        //这里加入撤单超时检查
                        continue;
                    }

                    //如果不是需要先撤单的 则直接发送强平委托
                    debug(ps.Position.GetPositionKey() + ":没有发送过强平委托，发送强平委托", QSEnumDebugLevel.INFO);
                    SendFlatPositionOrder(ps);
                }
                //已经发送过强平委托
                else
                {
                    if (!ps.Position.isFlat)//如果有持仓 则检查时间 进行撤单并再次强平
                    {
                        //平仓发单时间后超过一定时间 但是平仓次数小于设定次数则撤单然后再次平仓
                        if (DateTime.Now.Subtract(ps.SentTime).TotalSeconds >= SENDORDERDELAY && ps.FireCount < SENDORDERRETRY)
                        {
                            if (ps.OrderID > 0)//有委托编号表明有平仓委托在事务上，需要撤单
                            {
                                debug(ps.Position.GetPositionKey() + " 时间超过3秒仍然没有平掉持仓,取消该委托", QSEnumDebugLevel.INFO);
                                CancelOrder(ps.OrderID);
                                if (waitforcancel)
                                    continue;
                            }
                            else
                            {
                                //如果没有委托编号 则表明委托已经撤掉 需要重新发送平仓委托
                                debug(ps.Position.GetPositionKey() + " 原有委托已撤掉,重新平仓", QSEnumDebugLevel.INFO);
                                SendFlatPositionOrder(ps);
                                continue;
                            }
                        }

                        //当达到强平尝试次数 并且没有发送过强平异常通知 执行强平异常逻辑
                        if (ps.FireCount == SENDORDERRETRY && (!ps.FlatFailNoticed))//报警的时间设定
                        {
                            debug(ps.Position.GetPositionKey() + " 平仓次数超过" + SENDORDERRETRY + " 出发警告通知", QSEnumDebugLevel.INFO);
                            if (ps.OrderID != 0)
                            {
                                CancelOrder(ps.OrderID);
                            }
                            ps.FlatFailNoticed = true;
                            //强迫异常后冻结交易帐户 等待处理 这里需要对外触发事件 相关扩展模块监听后进行通知
                            IAccount account = _clearcentre[ps.Position.Account];
                            if (account != null)
                            {
                                account.InactiveAccount();
                            }
                            //对外进行未正常平仓报警
                            if (GotFlatFailedEvent != null)
                            {
                                GotFlatFailedEvent(ps.Position);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保证金检查,当保证金超过可用资金时执行强制减仓
        /// 这里需要结合期货业务规则进行设计
        /// </summary>
        void ProcessPositionOff()
        { 
        
        }
        #endregion
    }
}
