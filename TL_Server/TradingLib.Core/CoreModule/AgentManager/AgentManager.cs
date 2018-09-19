using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace TradingLib.Core
{
    [CoreAttr(AgentManager.CoreName, "代理帐户管理模块", "代理帐户管理模块用于管理交易账户，添加，删除，修改，加载等")]
    public partial class AgentManager : BaseSrvObject, IModuleAgentManager
    {
        const string CoreName = "AgentManager";
        public string CoreId { get { return this.PROGRAME; } }


        public AgentManager() :
            base(AgentManager.CoreName)
        {


            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
        }

        /// <summary>
        /// 监听全局分账户成交
        /// </summary>
        /// <param name="t"></param>
        void EventIndicator_GotFillEvent(Trade t)
        {
            fillbuffer.Write(t);
            NewWork();
        }


        /// <summary>
        /// 获得所有管理结算账户
        /// </summary>
        public IEnumerable<IAgent> Agents { get { return BasicTracker.AgentTracker.Agents; } }

        IdTracker txnIDTracker = new IdTracker();

        public void AssignTxnID(CashTransaction txn)
        {
            txn.TxnID = string.Format("AG{0}", txnIDTracker.AssignId);
        }
        public void CashOperation(CashTransaction txn)
        {

            logger.Info("AgentCashOperation ID:" + txn.Account + " Amount:" + txn.Amount.ToString() + " Txntype:" + txn.TxnType.ToString() + " EquityType:" + txn.EquityType.ToString());
            IAgent agent = BasicTracker.AgentTracker[txn.Account];
            if (agent == null)
            {
                throw new FutsRspError("代理帐户不存在");
            }

            //执行时间检查 
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.StandbyMode)
            {
                throw new FutsRspError("系统正在结算,禁止出入金操作");
            }

            if (txn.TxnType == QSEnumCashOperation.WithDraw)
            {
                if (txn.EquityType == QSEnumEquityType.OwnEquity && txn.Amount > agent.NowEquity)
                {
                    throw new FutsRspError("出金额度大于帐户权益");
                }
                if (txn.EquityType == QSEnumEquityType.CreditEquity && txn.Amount > agent.NowCredit)
                {
                    throw new FutsRspError("出金额度大于帐户信用额度");
                }
            }

            //生成唯一序列号
            this.AssignTxnID(txn);
            agent.LoadCashTrans(txn);
            TLCtxHelper.ModuleDataRepository.NewAgentCashTransactioin(txn);
            //TLCtxHelper.EventAccount.FireAccountCashOperationEvent(txn.Account,txn., Math.Abs(amount));
        }




        RingBuffer<Trade> fillbuffer = new RingBuffer<Trade>(10000);
        const int SLEEPDEFAULTMS = 100;
        static ManualResetEvent _workerwaiting = new ManualResetEvent(false);
        Thread _workthread = null;
        bool _workergo = false;
        void NewWork()
        {
            if ((_workthread != null) && (_workthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _workerwaiting.Set();
            }
        }

        void Process()
        {
            while (_workergo)
            {
                try
                {
                    while (fillbuffer.hasItems)
                    {
                        var f = fillbuffer.Read();
                        if (f == null) continue;

                        IAccount acc = TLCtxHelper.ModuleAccountManager[f.Account];
                        Manager mgr = BasicTracker.ManagerTracker[acc.Mgr_fk];

                        SplitCommission(mgr, f);
                    }

                    // clear current flag signal
                    _workerwaiting.Reset();
                    //logger.Info("process send");
                    // wait for a new signal to continue reading
                    _workerwaiting.WaitOne(SLEEPDEFAULTMS);

                }
                catch (Exception ex)
                {
                    logger.Error("Process Error:" + ex.ToString());
                }
            }
        }


        /// <summary>
        /// 分拆某个成交的手续费
        /// </summary>
        /// <param name="f"></param>
        void SplitCommission(Manager mgr, Trade f)
        {
            if (mgr == null || mgr.AgentAccount == null) return;
            decimal cost;
            decimal income;

            income = f.Commission;
            cost = mgr.AgentAccount.CalCommission(f);//计算代理手续费成本

            AgentCommissionSplit split = new AgentCommissionSplitImpl(mgr.AgentAccount, f, cost, income);
            mgr.AgentAccount.LoadCommissionSplit(split);
            TLCtxHelper.ModuleDataRepository.NewAgentCommissionSplit(split);

            //父子 编号不同
            while (mgr.ParentManager.mgr_fk != mgr.mgr_fk)
            {
                mgr = mgr.ParentManager;
                if (mgr == null || mgr.AgentAccount == null) return;

                income = cost;
                cost = mgr.AgentAccount.CalCommission(f);//计算代理手续费成本

                split = new AgentCommissionSplitImpl(mgr.AgentAccount, f, cost, income);
                mgr.AgentAccount.LoadCommissionSplit(split);
                TLCtxHelper.ModuleDataRepository.NewAgentCommissionSplit(split);
            }
        }




        public void Start()
        {
            logger.StatusStart(this.PROGRAME);
            if (_workergo) return;
            _workergo = true;
            _workthread = new Thread(Process);
            _workthread.IsBackground = false;
            _workthread.Start();

        }

        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
            _workergo = false;
            _workthread.Join();
            logger.Warn("**************** agent worker thread stopped");
        }



    }
}
