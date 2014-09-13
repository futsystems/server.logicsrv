using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    [ContribAttr(RaceCentre.ContribName, "比赛模块", "比赛模块逻辑,在单个帐户下分别统计期货,期权,交易的绩效统计")]
    public partial class RaceCentre :ContribSrvObject, IContrib
    {
        const string ContribName = "LottoqqRace";

        ConfigDB _cfgdb;


        public RaceCentre()
            : base(RaceCentre.ContribName)
        {
        }

        AsyncPRLoger _prloger = null;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            _prloger = new AsyncPRLoger();

            //1.加载配置文件
            _cfgdb = new ConfigDB(RaceCentre.ContribName);
            if (!_cfgdb.HaveConfig("OPTFund"))
            {
                _cfgdb.UpdateConfig("OPTFund", QSEnumCfgType.Decimal, 20000, "期权比赛初始可用资金");
            }
            if (!_cfgdb.HaveConfig("MJFund"))
            {
                _cfgdb.UpdateConfig("MJFund", QSEnumCfgType.Decimal, 20000, "秘籍服务初始可用资金");
            }
            if (!_cfgdb.HaveConfig("FutFund"))
            {
                _cfgdb.UpdateConfig("FutFund", QSEnumCfgType.Decimal, 20000, "期货比赛初始可用资金");
            }

            if (!_cfgdb.HaveConfig("DefaultEntryRace"))
            {
                _cfgdb.UpdateConfig("DefaultEntryRace", QSEnumCfgType.Bool,true, "模拟参赛帐号创建时,是否默认为该帐号创建比赛服务");
            }
            RaceConstant.FutSimEquity = _cfgdb["FutFund"].AsDecimal();
            RaceConstant.OptSimEquity = _cfgdb["OPTFund"].AsDecimal();
            RaceConstant.MJSimEquity = _cfgdb["MJFund"].AsDecimal();

            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new IPositionRoundDel(EventIndicator_GotPositionClosedEvent);
            TLCtxHelper.EventAccount.AccountAddEvent += new AccountIdDel(EventAccount_AccountAddEvent);

            //用于加载比赛服务
            RaceService rs =  RaceHelper.RaceServiceTracker["noaccount"];
        }

        /// <summary>
        /// 当有新的帐号生成时,为该帐号添加比赛服务
        /// </summary>
        /// <param name="account"></param>
        void EventAccount_AccountAddEvent(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                debug("Account:" + account + " do not exist", QSEnumDebugLevel.ERROR);
                return;
            }

            RaceService rs = RaceHelper.RaceServiceTracker[account];
            if (rs != null)
            {
                debug("Account:" + account + " already have raceservice", QSEnumDebugLevel.ERROR);
                return;
            }

            if (acc.Category == QSEnumAccountCategory.DEALER)
            {
                debug("Account:" + account + " Type:" + acc.Category.ToString() + " add race service for it", QSEnumDebugLevel.INFO);
                this.AddRaceService(account);
            }
        }

        /// <summary>
        /// 有新的交易回合产生时,将属于比赛的回合数据记录到数据库
        /// </summary>
        /// <param name="pr"></param>
        void EventIndicator_GotPositionClosedEvent(IPositionRound pr)
        {
            debug("PR:" + pr.Account + "-" + pr.Symbol, QSEnumDebugLevel.INFO);

            _prloger.newPR(pr);
            _prloger.Dispose();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            TLCtxHelper.EventIndicator.GotPositionClosedEvent -= new IPositionRoundDel(EventIndicator_GotPositionClosedEvent);
            base.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("racecentre starting..................",QSEnumDebugLevel.INFO);
            _prloger.Start();
            
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _prloger.Stop();
        }


        /// <summary>
        /// 为交易帐户添加比赛服务
        /// </summary>
        /// <param name="account"></param>
        void AddRaceService(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new QSErrorAccountNotExist();
            }

            RaceService rs = new RaceService();

            rs.Account = acc;
            //参赛时间
            rs.EntryTime = DateTime.Now;
            
            rs.LastFutEquity = RaceConstant.FutSimEquity;
            rs.LastOptEquity = RaceConstant.OptSimEquity;
            rs.LastMJEquity = RaceConstant.MJSimEquity;
            //比赛帐户结算时间
            rs.SettleDay = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            rs.Status = QSEnumRaceStatus.INRACEE;

            //添加比赛服务到管理器和数据库
            RaceHelper.RaceServiceTracker.AddRaceService(rs);
        }


        /// <summary>
        /// 参加比赛
        /// </summary>
        /// <param name="account"></param>
        void EntryRace(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new QSErrorAccountNotExist();
            }

            RaceService rs = RaceHelper.RaceServiceTracker[account];
            if (rs == null)
            {
                throw new RaceServiceNotExit();
            }

            RaceHelper.RaceServiceTracker.EntryRace(rs);


        }

        /// <summary>
        /// 退出比赛
        /// </summary>
        /// <param name="account"></param>
        void ExitRace(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new QSErrorAccountNotExist();
            }

            RaceService rs = RaceHelper.RaceServiceTracker[account];
            if (rs == null)
            {
                throw new RaceServiceNotExit();
            }

            
            //更新比赛状态为norace
            RaceHelper.RaceServiceTracker.UpdateRaceStatus(rs, QSEnumRaceStatus.NORACE);
        }


        /// <summary>
        /// 冻结某个比赛服务
        /// </summary>
        /// <param name="account"></param>
        void BlockRace(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new QSErrorAccountNotExist();
            }

            RaceService rs = RaceHelper.RaceServiceTracker[account];
            if (rs == null)
            {
                throw new RaceServiceNotExit();
            }

            RaceHelper.RaceServiceTracker.UpdateRaceStatus(rs, QSEnumRaceStatus.BLOCK);
        }


        /// <summary>
        /// 生成比赛统计数据
        /// </summary>
        void RankRace()
        { 
            //1.清空原有的统计表格
            TradingLib.ORM.MRaceStatistic.ClearRaceStatic();
            List<RaceStatistic> statlist = new List<RaceStatistic>();
            //2.通过数据库查询生成对应的统计信息 比如累计手续费,累计盈利,累计亏损等

            //a.重新计算和采集持仓回合统计
            StatisticTracker.ReCollectPRStatistic();

            //遍历所有比赛服务,生成比赛统计
            foreach (RaceService rs in RaceHelper.RaceServiceTracker.RaceServices)
            {
                if (rs.IsActive)
                {
                    RaceStatistic stat = RaceStatistic.RaceService2Statistic(rs);
                    StatisticTracker.FillRaceStatistic(stat);
                    statlist.Add(stat);
                }
            }
            //3.插入统计表格
            foreach (RaceStatistic stat in statlist)
            {
                TradingLib.ORM.MRaceStatistic.InsertRaceStatistic(stat);
            }
        
        }

        /// <summary>
        /// 执行比赛服务结算
        /// </summary>
        void Settle()
        {
            foreach (RaceService rs in RaceHelper.RaceServiceTracker.RaceServices)
            {
                if (!rs.IsSettled)
                {
                    //结算比赛服务
                    TradingLib.ORM.MRaceSettle.SettleRaceService(rs);
                }
            }
        }

    }
}
