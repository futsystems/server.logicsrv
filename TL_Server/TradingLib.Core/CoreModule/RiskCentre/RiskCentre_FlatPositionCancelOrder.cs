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
    internal enum QSEnumRiskTaskType
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


    internal enum QSEnumRiskTaskStatus
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        Inited,
        /// <summary>
        /// 子任务已创建
        /// </summary>
        SubTaskGenerated,
        /// <summary>
        /// 撤单已发送
        /// </summary>
        CancelSent,
        /// <summary>
        /// 撤单超时
        /// </summary>
        CancelTimeOut,
        /// <summary>
        /// 撤单完成
        /// </summary>
        CancelDone,
        /// <summary>
        /// 强平发送
        /// </summary>
        FlatSent,
        /// <summary>
        /// 强平超时
        /// </summary>
        FlatTimeOut,
        /// <summary>
        /// 强平完成
        /// </summary>
        FlatDone,
    }

    internal class RiskTaskSet
    {
        /// <summary>
        /// 对应的交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 风控操作任务类型
        /// </summary>
        public QSEnumRiskTaskType TaskType { get; set; }

        /// <summary>
        /// 委托来源
        /// </summary>
        public QSEnumOrderSource Source { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ForceCloseReason { get; set; }


        QSEnumRiskTaskStatus _status = QSEnumRiskTaskStatus.Inited;
        /// <summary>
        /// 任务状态
        /// </summary>
        public QSEnumRiskTaskStatus TaskStatus
        {
            get { return _status; }
            set { _status = value; }
        }

        #region 持仓数据
        /// <summary>
        /// 待平持仓
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 是否需要强平持仓
        /// </summary>
        public bool NeedFlatPosition
        {
            get { return this.Position != null; }
        }

        /// <summary>
        ///  平仓数量
        /// </summary>
        public int FlatSize { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainSize { get; set; }


        /// <summary>
        /// 发送的平仓委托OrderID
        /// </summary>
        public List<long> FlatOrderIDList { get; set; }


        /// <summary>
        /// 平仓指令发送时间
        /// </summary>
        public DateTime SentFlatTime { get; set; }

        /// <summary>
        /// 平仓指令发送次数
        /// </summary>
        public int FlatCount { get; set; }


        #endregion

        #region 撤单数据
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
            }
        }

        /// <summary>
        /// 是否需要取消委托
        /// </summary>
        public bool NeedCancelOrders
        {
            get
            {
                return _pendingorders.Count > 0;
            }
        }

        /// <summary>
        /// 取消委托发送时间
        /// </summary>
        public DateTime SentCancelTime { get; set; }

        /// <summary>
        /// 撤单次数
        /// </summary>
        public int CancelCount { get; set; }

        #endregion


        #region FlatAll数据
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
            }
        }

        /// <summary>
        /// 是否需要生成子任务平仓
        /// </summary>
        public bool NeedGenerateSubTask
        {
            get { return _pendingpositions.Count > 0; }
        }


        /// <summary>
        /// 由该任务开启的子任务
        /// </summary>
        public List<RiskTaskSet> SubTask { get; set; }
        #endregion

        public RiskTaskSet(string account, Position pos, List<long> pendingOrdres, int sizeFlat, QSEnumOrderSource source, string closeReason)
        {
            //设定平仓任务标识
            this.TaskType = QSEnumRiskTaskType.FlatPosition;

            this.Account = pos.Account;
            this.PendingOrders = pendingOrdres;
            this.Position = pos;
            //设定平仓数量以及剩余数量
            this.FlatSize = sizeFlat > pos.UnsignedSize ? pos.UnsignedSize : sizeFlat;
            this.RemainSize = pos.UnsignedSize - this.FlatSize;



            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closeReason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();

        }


        /// <summary>
        /// 先撤一组委托 然后再强平一组持仓，
        /// pendingpostions可以为
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="pendingpositons"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account, List<long> orders, List<Position> pendingpositons, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumRiskTaskType.FlatAllPositions;

            this.Account = account;
            this.PendingOrders = orders;
            this.PendingPositionFlat = pendingpositons;

            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closereason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();
        }

        /// <summary>
        /// 撤掉一组委托事务
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account, List<long> orders, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumRiskTaskType.CancelOrder;

            this.Account = account;
            PendingOrders = orders;

            this.SentFlatTime = DateTime.Now;
            this.FlatCount = 0;

            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closereason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();
        }

        public override string ToString()
        {
            string msg = string.Empty;
            switch (this.TaskType)
            {
                case QSEnumRiskTaskType.FlatPosition:
                    msg = "Pos:" + this.Position.GetPositionKey() + " Status:" + this.TaskStatus.ToString();
                    break;
                case QSEnumRiskTaskType.CancelOrder:
                case QSEnumRiskTaskType.FlatAllPositions:
                    msg = "Flat All Status:" + this.TaskStatus.ToString();//"CancelSent:"+this.CancelSent.ToString() +" CancelDone:"+this.CancelDone.ToString()+" SubTaskGen:"+this.SubTaskGenerated.ToString();
                    break;
                default:
                    break;
            }
            return this.Account + " " + this.TaskType.ToString() + " SubTaskCnt:" + this.SubTask.Count.ToString() + " | " + msg;
        }

        public string Title
        {
            get
            {
                string msg = string.Empty;
                switch (this.TaskType)
                {
                    case QSEnumRiskTaskType.FlatPosition:
                        msg = string.Format("[{0}-{1}/{2}]", this.Account, this.TaskType.ToString(), this.Position.GetPositionKey());
                        break;
                    case QSEnumRiskTaskType.CancelOrder:
                        msg = string.Format("[{0}-{1}]", this.Account, this.TaskType.ToString());
                        break;
                    case QSEnumRiskTaskType.FlatAllPositions:
                        msg = string.Format("[{0}-{1}]", this.Account, this.TaskType.ToString());
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }
    }


    /// <summary>
    /// 强平与撤单部分 最后会封装在队列中进行操作由统一的线程对外进行发送
    /// 避免多个节点调用发单或者撤单出现问题
    /// </summary>
    public partial class RiskCentre
    {

        /// <summary>
        /// 警告某个交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="iswarnning"></param>
        /// <param name="message"></param>
        public void Warn(string account, bool iswarnning, string message = "")
        {
            //为该帐户生成
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                logger.Info(string.Format("{0}警告,内容:{1}", iswarnning ? "设置" : "解除", message));
                acct.IsWarn = iswarnning;
                if (iswarnning)
                {
                    TLCtxHelper.EventAccount.FireAccountWarnOnEvent(account, message);//设置警告
                }
                else
                {
                    TLCtxHelper.EventAccount.FireAccountWarnOffEvent(account, message);//解除警告
                }

                TLCtxHelper.EventAccount.FireAccountChangeEent(acct);//设置帐户状态变化事件
            }
            else
            {
                logger.Warn(string.Format("Account:{0} is not existed", account));
            }
        }
        /// <summary>
        /// 持仓强平事件
        /// </summary>
        //public event EventHandler<PositionFlatEventArgs> PositionFlatEvent;

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
                if (ps.TaskType == QSEnumRiskTaskType.FlatPosition)
                {
                    if (ps.Position.GetPositionKey().Equals(key))
                    {
                        return true;
                    }
                }
                if (ps.TaskType == QSEnumRiskTaskType.FlatAllPositions)
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
        bool HaveFlatAllTask(string accid)
        {
            return posflatlist.Any(task => task.Account.Equals(accid) && task.TaskType == QSEnumRiskTaskType.FlatAllPositions);
        }

        /// <summary>
        /// 平掉某个账户的所有仓位(风控里面的 强平并冻结账户)
        /// 注,这里需要封装发单方式,若系统还有未成交的合约,我们需要先撤掉所有委托,然后再发新的委托
        /// 注:Brokerrouter内部 市价平仓有自动撤单机制,平仓是市价委托 因此可以自动提交由br进行撤单并平仓
        /// 但是平仓后 原来的建仓委托单可能没有被撤出，从而再次建仓。
        /// 
        /// 这里是平掉某个帐户的所有持仓 则意味着需要先撤掉所有委托 然后再平掉所有持仓
        /// 
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(string accid, QSEnumOrderSource source, string closereason = "系统强平")
        {
            //如果已经有该帐户的强平任务 则直接返回
            if (HaveFlatAllTask(accid))
            {
                return;
            }

            //为该帐户生成
            IAccount account = TLCtxHelper.ModuleAccountManager[accid];
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
        /// 这里只准对某个持仓进行操作,平掉该仓位之前需要撤掉该仓位合约的所有待成交委托
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
            this.FlatPosition(pos, pos.UnsignedSize, ordersource, closereason);
        }

        /// <summary>
        /// 平掉某个持仓部分仓位
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="size">平掉数量</param>
        /// <param name="closereason"></param>
        public void FlatPosition(Position pos, int sizeFlat, QSEnumOrderSource ordersource, string closereason)
        {
            if (IsPosFlatPending(pos))
                return;
            logger.Info(string.Format("RiskCentre FlatPosition Pos:{0} FlatSize:{1}", pos.ToString(), sizeFlat));

            if (sizeFlat <= 0)
            {
                logger.Error("Flat Size should >= 0");
                return;
            }

            sizeFlat = sizeFlat >= pos.UnsignedSize ? pos.UnsignedSize : sizeFlat;
            //生成平仓任务
            RiskTaskSet ps = CreateFlatPositionTask(pos, sizeFlat, ordersource, closereason);
            //放入列表中维护
            posflatlist.Add(ps);
        }

        /// <summary>
        /// 创建FlatPositionTask任务
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sizeFlat"></param>
        /// <param name="orderSource"></param>
        /// <param name="closeReason"></param>
        /// <returns></returns>
        RiskTaskSet CreateFlatPositionTask(Position pos, int sizeFlat, QSEnumOrderSource orderSource, string closeReason)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[pos.Account];
            if (account == null)
            {
                logger.Error(string.Format("FlatPositionTask Account:{0} do not exist", pos.Account));
                return null;
            }

            //获得对应持仓上的待成交委托
            List<long> pendingorders = account.GetPendingOrders(pos.Symbol).Select(o => o.id).ToList();
            //生成持仓强平事务
            return new RiskTaskSet(pos.Account, pos, pendingorders, sizeFlat, orderSource, closeReason);
        }


        #region 取消委托
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
        public void CancelOrder(string accid, QSEnumOrderSource ordersouce, string cancelreason = "系统强平")
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[accid];
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
            IAccount account = TLCtxHelper.ModuleAccountManager[accid];
            if (account != null)
            {
                List<long> olist = account.GetPendingOrders(symbol).Select(o => o.id).ToList();
                RiskTaskSet ps = new RiskTaskSet(accid, olist, QSEnumOrderSource.RISKCENTRE, cancelreason);

                posflatlist.Add(ps);
            }
        }

        #endregion





        /// <summary>
        /// 发送底层强平持仓委托
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        void SendFlatPositionOrder(RiskTaskSet set)
        {
            Position pos = set.Position;
            bool side = pos.isLong ? true : false;
            //绑定合约对象
            IAccount account = TLCtxHelper.ModuleAccountManager[pos.Account];

            if (pos.oSymbol.SecurityFamily.Exchange.EXCode.Equals("SHFE"))
            {
                logger.Info("Position:" + pos.GetPositionKey() + " is in Exchange:SHFE, we need to check Close/CloseToday Split");
                int voltd = pos.PositionDetailTodayNew.Sum(p => p.Volume);//今日持仓
                int volyd = pos.PositionDetailYdNew.Sum(p => p.Volume);//昨日持仓
                Tick snapshot = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(pos.oSymbol.Exchange,pos.Symbol);
                int flatsize = set.FlatSize;
                if (volyd != 0 && flatsize > 0)
                {
                    int ordernum = volyd <= flatsize ? volyd : flatsize;
                    flatsize -= ordernum;

                    Order oyd = new OrderImpl(pos.Symbol, ordernum * (side ? 1 : -1) * -1);

                    oyd.Account = pos.Account;
                    oyd.Exchange = pos.oSymbol.Exchange;
                    oyd.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                    oyd.OrderSource = set.Source;
                    oyd.ForceClose = true;
                    oyd.ForceCloseReason = set.ForceCloseReason;

                    //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
                    //if (AssignOrderIDEvent != null)
                    //    AssignOrderIDEvent(ref oyd);
                    TLCtxHelper.ModuleExCore.AssignOrderID(ref oyd);
                    account.TrckerOrderSymbol(ref oyd);

                    set.FlatOrderIDList.Add(oyd.id);
                    if (snapshot != null)
                    {
                        oyd.LimitPrice = oyd.Side ? snapshot.UpperLimit : snapshot.LowerLimit;
                    }
                    SendOrder(oyd);
                }
                if (voltd != 0 && flatsize > 0)
                {
                    int ordernum = voltd <= flatsize ? voltd : flatsize;
                    Order otd = new OrderImpl(pos.Symbol, ordernum * (side ? 1 : -1) * -1);

                    otd.Account = pos.Account;
                    otd.Exchange = pos.oSymbol.Exchange;
                    otd.OffsetFlag = QSEnumOffsetFlag.CLOSETODAY;
                    otd.OrderSource = set.Source;
                    otd.ForceClose = true;
                    otd.ForceCloseReason = set.ForceCloseReason;

                    //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
                    //if (AssignOrderIDEvent != null)
                    //    AssignOrderIDEvent(ref otd);
                    TLCtxHelper.ModuleExCore.AssignOrderID(ref otd);
                    account.TrckerOrderSymbol(ref otd);

                    set.FlatOrderIDList.Add(otd.id);
                    if (snapshot != null)
                    {
                        otd.LimitPrice = otd.Side ? snapshot.UpperLimit : snapshot.LowerLimit;
                    }
                    //otd.LimitPrice = 4500;//模拟不成交延迟撤单的情况
                    SendOrder(otd);
                }
                if (volyd != 0 || voltd != 0)
                {
                    //set.FlatSent = true;
                    set.FlatCount++;//发送强平次数递增
                    set.SentFlatTime = DateTime.Now;
                }
            }
            else
            {
                //生成市价委托
                Order o = new MarketOrder(pos.Symbol, !side, set.FlatSize);
                o.Account = pos.Account;
                o.Exchange = pos.oSymbol.Exchange;
                o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                o.OrderSource = set.Source;
                o.ForceClose = true;
                o.ForceCloseReason = set.ForceCloseReason;
                o.LimitPrice = 0;
                //o.price = 2500;//模拟不成交延迟撤单的情况

                //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
                //if (AssignOrderIDEvent != null)
                //    AssignOrderIDEvent(ref o);
                TLCtxHelper.ModuleExCore.AssignOrderID(ref o);
                account.TrckerOrderSymbol(ref o);

                //set.FlatSent = true;
                set.FlatCount++;//发送强平次数递增
                set.SentFlatTime = DateTime.Now;
                set.FlatOrderIDList.Add(o.id);

                //对外发送委托
                SendOrder(o);
            }
        }


        int SENDORDERDELAY = 3;
        int SENDORDERRETRY = 3;
        int CANCELORDERRETRY = 3;
        bool waitforcancel = true;

        /// <summary>
        /// 查看某主任务是否有未完成子任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        bool AnyPendingSubTask(RiskTaskSet task)
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

        //void PositionFlatFail(Position pos, string error)
        //{
        //    TLCtxHelper.EventSystem.FirePositionFlatEvent(this, new PositionFlatEventArgs(pos, error));
        //    //if (PositionFlatEvent != null)
        //    //    PositionFlatEvent(this, new PositionFlatEventArgs(pos, error));
        //}
        /// <summary>
        /// 维护风控任务列表
        /// 风控任务包含强平所有持仓，强平某个持仓，撤单等
        /// 风控任务在不同的状态间进行切换,每次状态切换只执行一个任务，待数据返回状态发生变化时候再执行后续步骤
        /// </summary>
        void ProcessRiskTask()
        {
            List<RiskTaskSet> addlist = new List<RiskTaskSet>();
            List<RiskTaskSet> removelist = new List<RiskTaskSet>();

            foreach (RiskTaskSet ps in posflatlist)
            {
                switch (ps.TaskStatus)
                {
                    case QSEnumRiskTaskStatus.Inited:
                        {
                            //是否需要先撤单
                            if (ps.NeedCancelOrders)
                            {
                                logger.Info(ps.Title + ":Cancel Orders");
                                foreach (long oid in ps.PendingOrders)
                                {
                                    this.CancelOrder(oid); //取消委托
                                    Util.sleep(10);
                                }
                                ps.SentCancelTime = DateTime.Now;
                                ps.TaskStatus = QSEnumRiskTaskStatus.CancelSent;
                                break;
                            }
                            //是否需要全平持仓
                            if (ps.NeedFlatPosition)
                            {
                                logger.Info(ps.Title + ":Send FlatOrder");
                                SendFlatPositionOrder(ps);
                                ps.TaskStatus = QSEnumRiskTaskStatus.FlatSent;
                                break;
                            }
                            //是否全平多个持仓 生成子任务
                            if (ps.NeedGenerateSubTask)
                            {
                                logger.Info(ps.Title + ":Generate SubTask");
                                foreach (Position pos in ps.PendingPositionFlat)
                                {
                                    RiskTaskSet ps2 = CreateFlatPositionTask(pos, pos.UnsignedSize, ps.Source, ps.ForceCloseReason);
                                    addlist.Add(ps2);
                                    ps.SubTask.Add(ps2);//添加子任务
                                }
                                ps.TaskStatus = QSEnumRiskTaskStatus.SubTaskGenerated;
                                break;
                            }
                            break;
                        }
                    case QSEnumRiskTaskStatus.CancelSent: //CancelDone由获得委托回报后 处理列表更新状态
                        {
                            if (ps.PendingOrders.Count > 0)
                            {
                                if (DateTime.Now.Subtract(ps.SentCancelTime).TotalSeconds >= SENDORDERDELAY)
                                {
                                    string reason = ps.Title + ":CancelTimeOut,Task Fail";
                                    logger.Info(reason);
                                    //ps.FlatFailNoticed = true;
                                    //PositionFlatFail(ps.Position, "POSFLAT_CANCEL_ERROR");
                                    //对外进行异常平仓报警
                                    //if (GotFlatFailedEvent != null)
                                    //{
                                    //    GotFlatFailedEvent(ps.Position, reason);
                                    //}
                                    ps.TaskStatus = QSEnumRiskTaskStatus.CancelTimeOut;
                                }
                            }
                            else
                            {
                                ps.TaskStatus = QSEnumRiskTaskStatus.CancelDone;
                            }
                            break;
                        }
                    case QSEnumRiskTaskStatus.FlatSent:
                        {
                            //持仓数量大于设定剩余数量 强平未完成
                            if (ps.Position.UnsignedSize > ps.RemainSize)
                            {
                                if (DateTime.Now.Subtract(ps.SentFlatTime).TotalSeconds >= SENDORDERDELAY && ps.FlatCount < SENDORDERRETRY)
                                {
                                    //debug("flat orderid:" + ps.OrderID.ToString(), QSEnumDebugLevel.WARNING);
                                    if (ps.FlatOrderIDList.Count > 0)//有委托编号表明有平仓委托在事务上，需要撤单
                                    {
                                        if (ps.CancelCount <= CANCELORDERRETRY)
                                        {
                                            logger.Info(ps.Title + ":Order Not Executed,Cancel:" + string.Join(",", ps.FlatOrderIDList.ToArray()));
                                            //这里会发生委托无法撤掉的分支逻辑 如果撤单多次没有撤掉 则跳出循环
                                            foreach (long oid in ps.FlatOrderIDList)
                                            {
                                                CancelOrder(oid);
                                            }
                                            ps.CancelCount++;//递增强平次数
                                            if (waitforcancel)
                                                continue;
                                        }
                                        else//发送的委托无法撤单，但是也不成交 执行异常
                                        {
                                            string msg = ps.Title + ":Order Not Executed,Cancel Without Reply,TaskFail";
                                            logger.Warn(msg);
                                            //PositionFlatFail(ps.Position, "POSFLAT_FLATORDER_CANCEL_ERROR");
                                            ////对外进行未正常平仓报警
                                            //if (GotFlatFailedEvent != null)
                                            //{
                                            //    GotFlatFailedEvent(ps.Position, msg);
                                            //}
                                            ps.TaskStatus = QSEnumRiskTaskStatus.FlatTimeOut;
                                        }
                                    }
                                    else
                                    {
                                        //如果没有委托编号 则表明委托已经撤掉 需要重新发送平仓委托 (如果通道拒绝则 OrderError会触发风控中心将委托置0，则显示原有委托已撤掉)
                                        logger.Info(ps.Title + ":Order Not Executed,Cancel Success,ReSend FlatOrder");
                                        SendFlatPositionOrder(ps);
                                        continue;
                                    }
                                }
                                if (ps.FlatCount == SENDORDERRETRY)
                                {
                                    string msg = ps.Title + ":Send FlatOrder Max Cnt:" + SENDORDERRETRY;
                                    logger.Warn(msg);
                                    if (ps.FlatOrderIDList.Count != 0)
                                    {
                                        foreach (long oid in ps.FlatOrderIDList)
                                        {
                                            //最后一次撤单
                                            CancelOrder(oid);
                                        }
                                    }
                                    //PositionFlatFail(ps.Position, "POSFLAT_OVER_MAXTRYNUMS");
                                    ps.TaskStatus = QSEnumRiskTaskStatus.FlatTimeOut;
                                    //对外进行未正常平仓报警
                                    //if (GotFlatFailedEvent != null)
                                    //{
                                    //    GotFlatFailedEvent(ps.Position,"尝试强平持仓多次,未成功");
                                    //}

                                }

                            }
                            else
                            {
                                ps.TaskStatus = QSEnumRiskTaskStatus.FlatDone;
                            }
                            break;
                        }
                    case QSEnumRiskTaskStatus.CancelDone:
                        {
                            if (ps.NeedGenerateSubTask)//
                            {
                                logger.Info(ps.Title + ":CancelDone,GenerateSubTask");
                                foreach (Position pos in ps.PendingPositionFlat)
                                {
                                    RiskTaskSet ps2 = CreateFlatPositionTask(pos, pos.UnsignedSize, ps.Source, ps.ForceCloseReason);
                                    addlist.Add(ps2);
                                    ps.SubTask.Add(ps2);//添加子任务
                                }
                                ps.TaskStatus = QSEnumRiskTaskStatus.SubTaskGenerated;
                                break;
                            }
                            //如果需要强平 则发送强平
                            if (ps.NeedFlatPosition)
                            {
                                logger.Info(ps.Title + ":CancelDone,SendFlatOrder");
                                SendFlatPositionOrder(ps);
                                ps.TaskStatus = QSEnumRiskTaskStatus.FlatSent;
                                break;
                            }
                            removelist.Add(ps);
                            break;
                        }
                    case QSEnumRiskTaskStatus.FlatDone://强平成功删除该任务
                        {
                            removelist.Add(ps);
                            break;
                        }
                    case QSEnumRiskTaskStatus.CancelTimeOut:
                    case QSEnumRiskTaskStatus.FlatTimeOut:
                        {
                            if (ps.NeedGenerateSubTask)
                            {
                                foreach (RiskTaskSet rs in ps.SubTask)
                                {
                                    removelist.Add(rs);
                                }
                            }
                            else
                            {
                                removelist.Add(ps);
                            }
                            break;
                        }
                    case QSEnumRiskTaskStatus.SubTaskGenerated:
                        {
                            if (!this.AnyPendingSubTask(ps))//如果没有任何子任务在队列中则删除父任务
                            {
                                logger.Info("All SubTask Removed,Remove Task:" + ps.Title);
                                removelist.Add(ps);
                            }
                        }
                        break;
                }
            }
            bool chg = false;
            //将新生成的任务和要删除的添加到队列或从队列中删除
            foreach (RiskTaskSet ps in addlist)
            {
                chg = true;
                logger.Info("RiskTask Added:" + ps.Title);
                posflatlist.Add(ps);
            }

            foreach (RiskTaskSet ps in removelist)
            {
                chg = true;
                logger.Info("RiskTask Removed:" + ps.Title);
                posflatlist.Remove(ps);
            }

            if (chg)
            {
                logger.Info("XXXXXXXXXXXXXXXXX Task List XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                foreach (RiskTaskSet ps in posflatlist)
                {
                    logger.Info(ps.ToString());
                }
            }
        }

        #endregion
    }



}