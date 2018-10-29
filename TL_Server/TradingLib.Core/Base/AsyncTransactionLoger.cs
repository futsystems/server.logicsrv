using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Diagnostics;//记得加入此引用
using TradingLib.Common;
using TradingLib.API;
using TradingLib;

namespace TradingLib.Core
{
    

    /*
     * 数据核心产生的一些问题
     * 1.系统会产生一些重复的单子(大多数是因为CTP回报Order多次,数据库没有进行对应检查,而进行的多次插入)
     * 2.模拟交易账号应该不会存在重复单,但是也存在了重复单
     * 
     * 
     * 
     * 
     * 
     * */
    /// <summary>
    ///模拟交易Broker用于将Order cancle trade交易信息安全的记录到数据库
    ///通过数据库操作缓存队列以及数据库重试连击技术，可以使得交易日志能够在数据库断开连接或者重新启动数据库的情况下交易日志信息部丢失
    ///
    /// 关键交易数据储存组件 需要确保交易数据记录完备
    /// 1.数据库工作正常时实时将数据记录到数据库
    /// 2.当数据库工作异常时将没有正确记录的数据放入缓存中,当数据库正常时重新进行数据操作
    /// 3.同时将数据实时记录到本地磁盘文件,用于防止软件奔溃时恢复数据
    /// 
    /// </summary>
    public class AsyncTransactionLoger:BaseSrvObject
    {
        //注当缓存超过后系统记录的交易信息就会发生错误。因此这里我们需要放大缓存大小并且在控制面板中需要监视。
        const int MAXLOG = 100000;


        RingBuffer<Order> _ocache;//委托插入缓存
        RingBuffer<Order> _oupdatecache;//委托更新缓存
        RingBuffer<Trade> _tcache;//成交插入缓存
        RingBuffer<OrderAction> _oactioncache;//委托操作缓存
        RingBuffer<PositionCloseDetail> _posclosecache;//平仓明细缓存
        RingBuffer<PositionDetail> _posdetailcache;//持仓明细缓存
        RingBuffer<ExchangeSettlement> _exsettlementcache;//交易所结算缓存
        RingBuffer<CashTransaction> _cashtxncache;//出入金记录缓存

        RingBuffer<AgentCommissionSplit> _agentcommissionsplit;//代理手续费拆分缓存
        RingBuffer<CashTransaction> _agentcashtxncache;//出入金记录缓存


        /// <summary>
        /// 是否有缓存数据
        /// 在执行结算时 要等待所有缓存数据写入完毕后才可以执行结算操作
        /// </summary>
        public bool HaveCacheData {

            get
            {
                return _ocache.hasItems || _oupdatecache.hasItems || _tcache.hasItems || _oactioncache.hasItems || _posclosecache.hasItems || _posdetailcache.hasItems || _exsettlementcache.hasItems || _cashtxncache.hasItems || _agentcommissionsplit.hasItems || _agentcashtxncache.hasItems;
            }
        }
        /// <summary>
        /// create an asynchronous responder
        /// </summary>
        public AsyncTransactionLoger() : this(MAXLOG) { }
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        /// <param name="maximb"></param>
        public AsyncTransactionLoger(int maxbr)
            : base("AsyncTransactionLoger")
        {
            _ocache = new RingBuffer<Order>(maxbr);
            _oupdatecache = new RingBuffer<Order>(maxbr);
            _tcache = new RingBuffer<Trade>(maxbr);
            _oactioncache = new RingBuffer<OrderAction>(maxbr);

            _posclosecache = new RingBuffer<PositionCloseDetail>(maxbr);
            _posdetailcache = new RingBuffer<PositionDetail>(maxbr);

            _exsettlementcache = new RingBuffer<ExchangeSettlement>(maxbr);
            _cashtxncache = new RingBuffer<CashTransaction>(maxbr);

            _agentcommissionsplit = new RingBuffer<AgentCommissionSplit>(maxbr);
            _agentcashtxncache = new RingBuffer<CashTransaction>(maxbr);
        }

       
        /// <summary>
        /// stop the read threads and shutdown (call on exit)
        /// </summary>
        public void Stop()
        {
            if (!_loggo) return;
            _loggo = false;
            ThreadTracker.Unregister(_logthread);
            _logthread.Join();
            _logthread = null;
        }

        public void Start()
        {
            IEnumerable<DataRepositoryLog> logs = DataRepositoryLogger.LoadDataRepositoryLogs(TLCtxHelper.ModuleSettleCentre.Tradingday);
            
            int ordernum = logs.Where(l => l.RepositoryType == EnumDataRepositoryType.InsertOrder).Count();
            int tradenum = logs.Where(l => l.RepositoryType == EnumDataRepositoryType.InsertTrade).Count();
            int actionnum = logs.Where(l => l.RepositoryType == EnumDataRepositoryType.InsertOrderAction).Count();
            int txnnum = logs.Where(l => l.RepositoryType == EnumDataRepositoryType.InsertCashTransaction).Count();
            int closenum = logs.Where(l => l.RepositoryType == EnumDataRepositoryType.InsertPositionCloseDetail).Count();

            logger.Info(string.Format("DataRepository load logs Order:{0} Trade:{1} Action:{2} CashTxn:{3} PositionClose:{4}", ordernum, tradenum, actionnum, txnnum, closenum));
            //TODO::判断当前数据库数据与日志文件是否一致，不一致则删除当天数据库记录然后用日志进行数据恢复

            if (_loggo) return;
            _loggo = true;


            _logthread = new Thread(this.readedata);
            _logthread.Name = "AsyncTransaction logger";
            //_logthread.IsBackground = true; //设置IsBackgrouond之后 manualreset会失效 导致newlog后 线程还是处于等待状态直至TimeOut才会写入数据
            ThreadTracker.Register(_logthread);
            _logthread.Start();
        }


        public const int SLEEPDEFAULTMS = 2000;
        static ManualResetEvent _logwaiting = new ManualResetEvent(false);
        Thread _logthread = null;
        bool _loggo = false;
        int _delay = 0;

        /// <summary>
        /// 异步交易信息记录系统拥有1000条的缓存数据,实验的时候发现插入数据错误,后来研究发现 插入数据的先后有关系
        /// 当我们在trade cache停留的时候 一致有新的交易被送进来，但是ordercache里面的order却没有发送到数据库 从而造成的问题就是 当trade里面出现对应的order时候 我们并没有插入到该order
        /// 因此我们在插入数据的时候要检查 Order是优先插入的 有了order我们才可以先交易。 因此Order被完全插入完毕后我们才开始插入交易数据
        /// </summary>
        void readedata()
        {
            while (_loggo)
            {
                try
                {
                    //插入委托
                    while (_ocache.hasItems)
                    {
                        Order o = _ocache.Read();
                        DBInsertOrder(o);
                        Thread.Sleep(_delay);
                    }

                    //更新委托状态
                    while (!_ocache.hasItems && _oupdatecache.hasItems)
                    {
                        Order o = _oupdatecache.Read();
                        DBUpdateOrder(o);
                        Thread.Sleep(_delay);
                    }

                    //插入成交
                    while (!_ocache.hasItems && _tcache.hasItems)
                    {
                        Trade f = _tcache.Read();
                        DBInsertTrade(f);
                        Thread.Sleep(_delay);
                    }

                    //委托操作插入
                    while (!_ocache.hasItems && _oactioncache.hasItems)
                    {
                        OrderAction action = _oactioncache.Read();
                        DBInsertOrderAction(action);
                        Thread.Sleep(_delay);
                    }

                    //插入平仓明细数据
                    while (_posclosecache.hasItems)
                    {
                        PositionCloseDetail close = _posclosecache.Read();
                        DBInsertPositionCloseDetail(close);
                        Thread.Sleep(_delay);
                    }
                    //插入持仓明细数据
                    while (_posdetailcache.hasItems)
                    {
                        PositionDetail pd = _posdetailcache.Read();
                        DBInsertPositionDetail(pd);
                        Thread.Sleep(_delay);
                    }
                    //插入交易所结算记录
                    while (_exsettlementcache.hasItems)
                    {
                        ExchangeSettlement settle = _exsettlementcache.Read();
                        DBInsertExchangeSettlement(settle);
                        Thread.Sleep(_delay);
                    }
                    //插入出入金记录
                    while (_cashtxncache.hasItems)
                    {
                        CashTransaction txn = _cashtxncache.Read();
                        DBInsertCashTransaction(txn);
                        Thread.Sleep(_delay);
                    }
                    //插入手续费代理分润
                    while (_agentcommissionsplit.hasItems)
                    {
                        AgentCommissionSplit split = _agentcommissionsplit.Read();
                        DBInsertAgentCommissioinSplit(split);
                    }
                    //插入代理出入金数据
                    while (_agentcashtxncache.hasItems)
                    {
                        CashTransaction txn = _agentcashtxncache.Read();
                        DBInsertAgentCashTransaction(txn);
                    }

                    // clear current flag signal
                    _logwaiting.Reset();
                    // wait for a new signal to continue reading
                    _logwaiting.WaitOne(SLEEPDEFAULTMS);
                }
                
                //以下代码段通过捕捉交易日志插入部分的异常,将没有正常插入的数据重新返回到缓存队列
                //mysql则通过不断尝试进行数据库连接,当连接成功后重新将日志插入数据库
                catch (DataRepositoryException ex)
                {
                    logger.Error(string.Format("数据储存发生错误 Method:{0} Data:{1} Error:{2}", ex.RepositoryType, ex.RepositoryData.ToString(), ex.InnerException.ToString()));
                }
                catch (Exception ex)
                {
                    logger.Error(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString());
                }
            }
        }
     
        #region 插入或更新交易数据
        /// <summary>
        /// 将新的需要记录的数据记录下来 从而实现异步处理防止阻塞通讯主线程
        /// 数据记录需要copy模式,否则引用对象得其他线程访问时候会出现数据错误 比如成交数目与实际成交数目无法对应等问题。
        /// </summary>
        /// <param name="k"></param>
        public void NewOrder(Order o)
        {
            Order oc = new OrderImpl(o);//复制委托 防止委托参数发生变化
            _ocache.Write(oc);

            newlog();
        }
        public void UpdateOrder(Order o)
        {
            Order oc = new OrderImpl(o);
            _oupdatecache.Write(oc);
            newlog();
        }
        
        public void NewTrade(Trade f)
        {
            Trade nf = new TradeImpl(f);
            _tcache.Write(nf);
            newlog();
        }
        
        public void NewOrderAction(OrderAction action)
        {
            _oactioncache.Write(action);
            newlog();
        }


        public void NewPositionCloseDetail(PositionCloseDetail pc)
        {
            _posclosecache.Write(pc);
            newlog();
        }

        public void NewCashTransaction(CashTransaction txn)
        {
            _cashtxncache.Write(txn);
            newlog();
        }




        /**
         *  持仓明细与交易所结算数据是在结算过程中产生的结算数据,不需要通过DataRep日志系统进行记录
         * */
        public void NewPositionDetail(PositionDetail pd)
        {
            _posdetailcache.Write(pd);
            newlog();
        }

        public void NewExchangeSettlement(ExchangeSettlement settle)
        {
            _exsettlementcache.Write(settle);
            newlog();
        }

        public void NewAgentCommissionSplit(AgentCommissionSplit split)
        {
            _agentcommissionsplit.Write(split);
            newlog();
        }
        public void NewAgentCashTransaction(CashTransaction txn)
        {
            _agentcashtxncache.Write(txn);
            newlog();
        }

        
        #endregion



        #region 数据库插入/更新操作
        void DBInsertOrder(Order o)
        {
            try
            {
                bool re = ORM.MTradingInfo.InsertOrder(o);
                if (re)
                {
                    logger.Debug(string.Format("Insert Order Success:{0}", o.GetOrderInfo()));
                }
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertOrder, o, ex);
            }
        }

        void DBUpdateOrder(Order o)
        {
            try
            {
                bool re = ORM.MTradingInfo.UpdateOrderStatus(o);
                if (re)
                {
                    logger.Debug(string.Format("Update Order Success:{0}", o.GetOrderInfo()));
                }
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.UpdateOrder, o, ex);
            }
        }

        void DBInsertTrade(Trade f)
        {
            try
            {
                bool re = ORM.MTradingInfo.InsertTrade(f);
                if (re)
                {
                    logger.Debug(string.Format("Insert Trade Success:{0}", f.GetTradeInfo()));
                }
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertTrade, f, ex);
            }
        }

        void DBInsertOrderAction(OrderAction action)
        {
            try
            {
                bool re = ORM.MTradingInfo.InsertOrderAction(action);
                if (re)
                {
                    logger.Debug(string.Format("Insert OrderAction Success:{0}", action.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertOrderAction, action, ex);
            }
        }

        void DBInsertPositionCloseDetail(PositionCloseDetail close)
        {
            try
            {
                bool re = ORM.MSettlement.InsertPositionCloseDetail(close);
                if (re)
                {
                    logger.Debug(string.Format("Insert PositionCloseDetail Success:{0}", close.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertPositionCloseDetail, close, ex);
            }
        }

        void DBInsertPositionDetail(PositionDetail pd)
        {
            try
            {
                ORM.MSettlement.InsertPositionDetail(pd);
                logger.Debug(string.Format("Insert PositionDetail Success:{0}", pd.ToString()));
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertPositionDetail, pd, ex);
            }
        }

        void DBInsertExchangeSettlement(ExchangeSettlement settle)
        {
            try
            {
                ORM.MSettlement.InsertExchangeSettlement(settle);
                logger.Debug(string.Format("Insert ExchangeSettlement Success:{0}", settle.ToString()));
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertExchangeSettlement, settle, ex);
            }
        }

        void DBInsertCashTransaction(CashTransaction txn)
        {
            try
            {
                ORM.MCashTransaction.InsertCashTransaction(txn);
                logger.Debug(string.Format("Insert CashTransaction Success:{0}", txn.ToString()));
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertCashTransaction, txn, ex);
            }
        }

        void DBInsertAgentCommissioinSplit(AgentCommissionSplit split)
        {
            try
            {
                ORM.MAgentCommissionSplit.AddAgentCommissionSplit(split);
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertAgentCommissionSplit, split, ex);
            }
        }
        void DBInsertAgentCashTransaction(CashTransaction txn)
        {
            try
            {
                ORM.MAgentCashTransaction.InsertCashTransaction(txn);
                logger.Debug(string.Format("Insert AgentCashTransaction Success:{0}", txn.ToString()));
            }
            catch (Exception ex)
            {
                throw new DataRepositoryException(EnumDataRepositoryType.InsertAgetCashTransaction, txn, ex);
            }
        }

        #endregion

        private void newlog()
        {
            if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set();
            }
            
        }
    }


}
