using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace TradingLib.Core
{
    /// <summary>
    /// 在风控中心维护的强平或者撤单任务中分以下几个情况
    /// 1.强平某个持仓，该动作会将该持仓对应合约的所有待成交委托撤掉，然后再进行强平，强平成功后从队列删除
    /// 2.撤单后或强平，撤掉一组委托，待撤单成功后按设置生成对应的强平仓位的事务加入队列
    /// </summary>
    internal enum QSEnumTaskType
    { 
        /// <summary>
        /// 强平仓位
        /// 包含自动检测到的对应待成交委托
        /// </summary>
        FlatPosition,

        /// <summary>
        /// 撤单 单纯撤单不涉及强平
        /// </summary>
        CancelOrder,

        /// <summary>
        /// 强平所有持仓 先撤单然后再强平
        /// </summary>
        FlatAllPositions,
    }

    //internal enum QSEnumTaskStatus
    //{ 
    //    /// <summary>
    //    /// 等待处理中
    //    /// </summary>
    //    Added,

    //    /// <summary>
    //    /// 等待子任务处理中
    //    /// </summary>
    //    SubTaskPending,

    //    /// <summary>
    //    /// 处理 完毕
    //    /// </summary>
    //    Finished

    //}

    internal class RiskTaskSet
    {
        /// <summary>
        /// 对应的交易帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 风控操作任务类型
        /// </summary>
        public QSEnumTaskType TaskType { get; set; }

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
        /// 发送的平仓委托OrderID
        /// </summary>
        public long OrderID { get; set; }

        /// <summary>
        /// 是否已经发送强平委托
        /// </summary>
        public bool FlatSent { get; set; }

        /// <summary>
        /// 强平成功 用于标志持仓曾经
        /// </summary>
        //public bool FlatDone { get; set; }

        /// <summary>
        /// 平仓指令发送时间
        /// </summary>
        public DateTime SentTime { get; set; }

        /// <summary>
        /// 平仓指令发送次数
        /// </summary>
        public int FireCount { get; set; }

        

        /// <summary>
        /// 强迫异常或失效通知
        /// </summary>
        public bool FlatFailNoticed { get; set; }

        



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
        /// 待成交委托列表
        /// </summary>
        List<long> _pendingorders = new List<long>();

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
        /// 是否有需要撤掉的委托
        /// </summary>
        public bool HaveOrderCancels { get; private set; }

        List<long> _ordercancels = new List<long>();
        /// <summary>
        /// 需要撤掉的委托列表
        /// </summary>
        public List<long> OrderCancels 
        {
            get
            {
                return _ordercancels;
            }
            set
            {
                _ordercancels = value;
                if (_ordercancels.Count > 0)
                {
                    HaveOrderCancels = true;
                }
            }
        }

       

        /// <summary>
        /// 是否有待强平持仓
        /// </summary>
        public bool HavePendingPostionFlat { get; private set; }

        List<Position> _pendingpositions = new List<Position>();
        /// <summary>
        /// 待强平持仓
        /// </summary>
        public List<Position> PendingPositionFlat
        {
            get
            {
                return _pendingpositions;
            }

            set
            {
                _pendingpositions = value;
                if (_pendingpositions.Count > 0)
                {
                    HavePendingPostionFlat = true;
                }
            }
        }

        /// <summary>
        /// 由该任务开启的子任务
        /// </summary>
        public List<RiskTaskSet> SubTask { get; set; }
        
        /// <summary>
        /// 是否已经生成了子任务
        /// </summary>
        public bool SubTaskGenerated { get; set; }
        /// <summary>
        /// 平仓事务
        /// 强平某个持仓pos
        /// 其对应的待成交委托为pendingorders
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pendingorders"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account,Position pos,List<long> pendingorders, QSEnumOrderSource source, string closereason)
        {
            //设定平仓任务标识
            TaskType = QSEnumTaskType.FlatPosition;
            this.Account = account;
            //持仓数据
            Position = pos;
            this.Account = pos.Account;
            //待平仓数据
            PendingOrders = pendingorders;

            CancelDone = false;
            CancelSent = false;
            FlatSent = false;

            SentTime = DateTime.Now;
            FireCount = 0;
            
            Source = source;
            ForceCloseReason = closereason;
            FlatFailNoticed = false;//强平异常通知
            SubTask = new List<RiskTaskSet>();
            SubTaskGenerated = false;
        }

        /// <summary>
        /// 先撤一组委托 然后再强平一组持仓，
        /// pendingpostions可以为
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="pendingpositons"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account,List<long> orders, List<Position> pendingpositons, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumTaskType.FlatAllPositions;
            this.Account = account;
            OrderCancels = orders;
            PendingPositionFlat = pendingpositons;

            CancelDone = false;
            CancelSent = false;
            FlatSent = false;

            SentTime = DateTime.Now;
            FireCount = 0;

            Source = source;
            ForceCloseReason = closereason;
            FlatFailNoticed = false;//强平异常通知
            SubTask = new List<RiskTaskSet>();
            SubTaskGenerated = false;
        }

        /// <summary>
        /// 队列中撤掉一组委托
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account, List<long> orders, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumTaskType.CancelOrder;
            this.Account = account;
            OrderCancels = orders;
            PendingPositionFlat = new List<Position>();//待强平持仓为空

            CancelDone = false;
            CancelSent = false;
            FlatSent = false;

            SentTime = DateTime.Now;
            FireCount = 0;

            Source = source;
            ForceCloseReason = closereason;
            FlatFailNoticed = false;//强平异常通知
            SubTask = new List<RiskTaskSet>();
            SubTaskGenerated = false;
        }

        public override string ToString()
        {
            string msg = string.Empty;
            switch(this.TaskType)
            {
                case QSEnumTaskType.FlatPosition:
                    msg = "Pos:"+this.Position.GetPositionKey() +" FlatSent:"+this.FlatSent.ToString() +" FirCnt:"+this.FireCount.ToString();
                    break;
                case QSEnumTaskType.CancelOrder:
                case QSEnumTaskType.FlatAllPositions:
                    msg = "CancelSent:"+this.CancelSent.ToString() +" CancelDone:"+this.CancelDone.ToString()+" SubTaskGen:"+this.SubTaskGenerated.ToString();
                    break;
                default:
                    break;
            }
            return this.Account + " " + this.TaskType.ToString() + " SubTaskCnt:" + this.SubTask.Count.ToString() +" | "+msg;
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
        public event PositionFlatFailDel GotFlatFailedEvent;






        //待平仓列表,主要包含系统尾盘集中强平,系统内部风控强平等形成的平仓指令
        ThreadSafeList<RiskTaskSet> posflatlist = new ThreadSafeList<RiskTaskSet>();

        /// <summary>
        /// 处于强平所有持仓过程中的帐户列表
        /// </summary>
        //ThreadSafeList<string> accountinfaltall = new ThreadSafeList<string>();
        #region【持仓强平的循环检查与维护】

        /// <summary>
        /// 检查某个持仓是否在强平队列
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsPosFlatPending(Position pos)
        {
            string key = pos.GetPositionKey();

            foreach (RiskTaskSet ps in posflatlist)
            {
                if (ps.TaskType == QSEnumTaskType.FlatPosition)
                {
                    if (ps.Position.GetPositionKey().Equals(key))
                    {
                        return true;
                    }
                }
                if(ps.TaskType == QSEnumTaskType.FlatAllPositions)
                {
                    //如果是先撤单然后再平仓类型的任务 需要检查先撤单再平仓的待平仓列表中是否包含了对应的持仓
                    if (ps.PendingPositionFlat.Where(p => p.GetPositionKey().Equals(key)).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 某个帐户是否已经提交了强平所有持仓的事务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //bool IsAccountInFlatAllTask(string account)
        //{
        //    return accountinfaltall.Contains(account);
        //}

        bool HaveFlatAllTask(string accid)
        {
            foreach (RiskTaskSet task in posflatlist)
            {
                if (task.Account == accid && task.TaskType == QSEnumTaskType.FlatAllPositions)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 平掉某个账户的所有仓位(风控里面的 强平并冻结账户)
        /// 注,这里需要封装发单方式,若系统还有未成交的合约,我们需要先撤掉所有委托,然后再发新的委托
        /// 注:Brokerrouter内部 市价平仓有自动撤单机制,平仓是市价委托 因此可以自动提交由br进行撤单并平仓
        /// 但是平仓后 原来的建仓委托单可能没有被撤出，从而再次建仓。
        /// 
        /// 这里是平掉某个帐户的所有持仓 则意味着需要先撤掉所有委托 然后再平掉所有持仓
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(string accid, QSEnumOrderSource source, string closereason = "系统强平")
        {
            //debug("平掉账户:" + accid + "所有仓位", QSEnumDebugLevel.INFO);
            if (HaveFlatAllTask(accid))
            {
                //debug("有未完成强平任务在队列中,直接返回", QSEnumDebugLevel.INFO);
                return;
            }
            
            IAccount account = _clearcentre[accid];
            //if (IsAccountInFlatAllTask(accid)) return;//强平所有持仓时需要冻结交易帐户

            if (account != null)
            {
                List<long> olist = account.GetPendingOrders().Select(o => o.id).ToList();
                List<Position> plist = account.GetPositionsHold().ToList();
                RiskTaskSet ps = new RiskTaskSet(accid, olist, plist, QSEnumOrderSource.RISKCENTRE, closereason);

                posflatlist.Add(ps);
            }
        }

        /// <summary>
        /// 平掉某个持仓
        /// 这里只准对某个持仓进行操作,平掉该仓位之前需要撤掉该仓位合约的所有待成交合约
        /// 
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
            //如果该持仓已经在平仓队列中则不进行处理直接返回
            if (IsPosFlatPending(pos))
                return;
            debug("RiskCentre Flatpostion:" + pos.ToString(), QSEnumDebugLevel.INFO);

            //生成平仓任务
            RiskTaskSet ps = GenFlatPostionSet(pos, ordersource, closereason);
            //放入列表中维护
            posflatlist.Add(ps);
        }




        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ordersource"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(Order order, QSEnumOrderSource ordersource, string cancelreason = "系统强平")
        {
            List<long> olist = new List<long>() { order.id };
            RiskTaskSet ps = new RiskTaskSet(order.Account, olist, QSEnumOrderSource.RISKCENTRE, cancelreason);

            posflatlist.Add(ps);
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
                List<long> olist = account.GetPendingOrders().Select(o => o.id).ToList();
                RiskTaskSet ps = new RiskTaskSet(accid, olist, QSEnumOrderSource.RISKCENTRE, cancelreason);

                posflatlist.Add(ps);
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
                List<long> olist = account.GetPendingOrders(symbol).Select(o => o.id).ToList();
                RiskTaskSet ps = new RiskTaskSet(accid, olist, QSEnumOrderSource.RISKCENTRE, cancelreason);

                posflatlist.Add(ps);
            }
        }


        /// <summary>
        /// 生成强平任务
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="closereason"></param>
        /// <returns></returns>
        RiskTaskSet GenFlatPostionSet(Position pos, QSEnumOrderSource ordersource, string closereason)
        {
            IAccount account = _clearcentre[pos.Account];
            if (account == null)
                return null;
            //获得对应持仓上的待成交委托
            List<long> pendingorders = account.GetPendingOrders(pos.Symbol).Select(o => o.id).ToList();
            //生成持仓强平事务
            return new RiskTaskSet(pos.Account, pos, pendingorders, ordersource, closereason);
        }

        /// <summary>
        /// 发送底层强平持仓委托
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        void SendFlatPositionOrder(RiskTaskSet set)
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
            IAccount account = _clearcentre[pos.Account];
            account.TrckerOrderSymbol(ref o);

            set.FlatSent = true;
            set.FireCount++;
            set.SentTime = DateTime.Now;
            set.OrderID = o.id;

            //对外发送委托
            SendOrder(o);
        }


        int SENDORDERDELAY = 3;
        int SENDORDERRETRY = 3;
        bool waitforcancel = true;

        bool AnySubTaskInList(RiskTaskSet task)
        {
            foreach (RiskTaskSet subtask in task.SubTask)
            {
                if (posflatlist.Contains(subtask))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 监控强平持仓列表,用于观察委托是否正常平仓
        /// 强平是一个触发过程
        /// 系统内部的帐户风控规则检查或者定时检查 扫描到需要强平的仓位或者帐户就会进行强平
        /// 执行强平后 对应的持仓将进入队列进行监控处理 直到强平成功或者异常
        /// </summary>
        void ProcessPositionFlat()
        {
            //debug("检查待平仓列表...", QSEnumDebugLevel.INFO);
            List<RiskTaskSet> addlist = new List<RiskTaskSet>();
            List<RiskTaskSet> removelist = new List<RiskTaskSet>();

            foreach (RiskTaskSet ps in posflatlist)
            {
                if (ps.FlatFailNoticed) continue;//如果已经通知过异常的 则直接跳过

                //如果生成了子任务 则检查是否子任务都已经完成并从队列中删除 如果已经删除 则父任务完成加入待删除列表
                if (ps.SubTaskGenerated)
                {
                    if (!AnySubTaskInList(ps))//如果没有任何子任务在队列中则删除父任务
                    {
                        debug("子任务全部完成,删除父任务", QSEnumDebugLevel.INFO);
                        removelist.Add(ps);
                    }
                }
                
                switch (ps.TaskType)
                {
                    case QSEnumTaskType.FlatPosition:
                        {
                            #region 强平持仓
                            //没有发送过强平委托
                            if (!ps.FlatSent)
                            {
                                //如果是要先取消再平仓的 则需要发送取消委托
                                if (ps.NeedCancelFirst)
                                {
                                    //如果没有发送过取消委托 则遍历所有委托 发送取消
                                    if (!ps.CancelSent)
                                    {
                                        debug(ps.Position.GetPositionKey() + ":没有发送强平委托,且需要先撤单，发送撤单", QSEnumDebugLevel.INFO);
                                        foreach (long oid in ps.PendingOrders)
                                        {
                                            this.CancelOrder(oid);
                                            Util.sleep(10);//取消委托
                                        }
                                        ps.CancelSent = true;//标注 取消已经发出
                                        ps.SentTime = DateTime.Now;//标注委托发送时间
                                    }
                                    else
                                    {

                                        //如果取消委托已经发送并且已经完成 同时是第一次发送强迫指令 则执行强平
                                        if (ps.CancelDone)
                                        {
                                            debug(ps.Position.GetPositionKey() + ":已经发送撤单,并且所有委托已经撤掉,继续执行强平逻辑", QSEnumDebugLevel.INFO);
                                            SendFlatPositionOrder(ps);
                                        }
                                        else
                                        {
                                            //这里加入撤单超时检查 取消未完成 并且超时
                                            if (DateTime.Now.Subtract(ps.SentTime).TotalSeconds >= SENDORDERDELAY && (!ps.FlatFailNoticed))
                                            {
                                                string reason = ps.Position.GetPositionKey() + " 强平过程异常,强平前撤单未成功";
                                                debug(reason, QSEnumDebugLevel.INFO);
                                                ps.FlatFailNoticed = true;
                                                //对外进行异常平仓报警
                                                if (GotFlatFailedEvent != null)
                                                {
                                                    GotFlatFailedEvent(ps.Position, reason);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    //如果不是需要先撤单的 则直接发送强平委托
                                    debug(ps.Position.GetPositionKey() + ":没有发送过强平委托，发送强平委托", QSEnumDebugLevel.INFO);
                                    SendFlatPositionOrder(ps);
                                }
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
                                            //这里会发生委托无法撤掉的分支逻辑 如果撤单多次没有撤掉 则跳出循环
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
                                            //最后一次撤单
                                            CancelOrder(ps.OrderID);
                                        }
                                        ps.FlatFailNoticed = true;
                                        //强迫异常后冻结交易帐户 等待处理 这里需要对外触发事件 相关扩展模块监听后进行通知
                                        //IAccount account = _clearcentre[ps.Position.Account];
                                        //if (account != null)
                                        //{
                                        //    account.InactiveAccount();
                                        //}
                                        //对外进行未正常平仓报警
                                        if (GotFlatFailedEvent != null)
                                        {
                                            GotFlatFailedEvent(ps.Position,"尝试强平持仓多次,未成功");
                                        }

                                    }
                                }
                            }
                            #endregion
                        }
                        break;
                    case QSEnumTaskType.CancelOrder:
                    case QSEnumTaskType.FlatAllPositions:
                        {
                            #region 撤单 撤单后按持仓情况 进行强平
                            if (ps.HaveOrderCancels)
                            {
                                if (!ps.CancelSent)//如果没有发送过撤单 则发送撤单
                                {
                                    debug("发送撤单指令", QSEnumDebugLevel.INFO);
                                    foreach (long oid in ps.OrderCancels)
                                    {
                                        this.CancelOrder(oid);
                                        Util.sleep(10);//取消委托
                                    }
                                    ps.CancelSent = true;//标注 取消已经发出
                                    ps.SentTime = DateTime.Now;
                                }
                                else//撤销委托发送出去后需要检查撤单是否成功 如果成功则
                                {
                                    //如果撤单操作完成 则将需要的
                                    if (ps.CancelDone)
                                    {
                                        //如果需要强平某些仓位 则
                                        if (ps.HavePendingPostionFlat) //如果有待成交委托 则生成子任务
                                        {
                                            if (!ps.SubTaskGenerated)//如果没有生成过子任务 则添加子任务
                                            {
                                                debug("队列批量撤单完成，生成平仓事务加入到队列", QSEnumDebugLevel.INFO);
                                                foreach (Position pos in ps.PendingPositionFlat)
                                                {
                                                    RiskTaskSet ps2 = GenFlatPostionSet(pos, ps.Source, ps.ForceCloseReason);
                                                    addlist.Add(ps2);
                                                    ps.SubTask.Add(ps2);//添加子任务
                                                }
                                                ps.SubTaskGenerated = true;//标记子任务已经添加
                                            }
                                        }
                                        else//如果没有待平仓委托 直接删除该任务
                                        {
                                            debug("删除撤单事务");
                                            removelist.Add(ps);
                                        }
                                    }
                                    if (!ps.CancelDone && DateTime.Now.Subtract(ps.SentTime).TotalSeconds >= SENDORDERDELAY && (!ps.FlatFailNoticed))
                                    {
                                        string reason = ps.Position.GetPositionKey() + " 撤单过程异常,未成功";
                                        debug(reason, QSEnumDebugLevel.INFO);
                                        ps.FlatFailNoticed = true;
                                        //对外进行异常平仓报警
                                        //if (GotFlatFailedEvent != null)
                                        //{
                                        //    GotFlatFailedEvent(ps.Position, reason);
                                        //}
                                    }
                                }
                            }
                            else//如果原来的撤单列表就是空的 则直接生成强平任务 插入到队列
                            {
                                if (ps.HavePendingPostionFlat)//如果有待成交委托 则生成子任务
                                {
                                    if (!ps.SubTaskGenerated)//如果没有生成过子任务 则添加子任务
                                    {
                                        debug("没有撤单列表，直接生成平仓事务加入到队列", QSEnumDebugLevel.INFO);
                                        foreach (Position pos in ps.PendingPositionFlat)
                                        {
                                            RiskTaskSet ps2 = GenFlatPostionSet(pos, ps.Source, ps.ForceCloseReason);
                                            addlist.Add(ps2);
                                            ps.SubTask.Add(ps2);
                                        }
                                        ps.SubTaskGenerated = true;
                                    }
                                }
                                else
                                {
                                    debug("没有待撤单列表，也没有待平仓列表 该事务为无效事务,删除事务");
                                    removelist.Add(ps);
                                }
                            }
                            #endregion
                        }
                        break;
                    default :
                        break;
                }


            }
            bool chg = false;
            //将新生成的任务和要删除的添加到队列或从队列中删除
            foreach (RiskTaskSet ps in addlist)
            {
                chg = true;
                debug("插入任务:" + ps.ToString(), QSEnumDebugLevel.INFO);
                posflatlist.Add(ps);
            }
            foreach (RiskTaskSet ps in removelist)
            {
                chg = true;
                debug("删除任务:" + ps.ToString(), QSEnumDebugLevel.INFO);
                posflatlist.Remove(ps);
            }
            if (chg)
            {
                debug("XXXXXXXXXXXXXXXXX task changed XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",QSEnumDebugLevel.INFO);
                foreach (RiskTaskSet ps in posflatlist)
                {
                    debug(ps.ToString(), QSEnumDebugLevel.INFO);
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
