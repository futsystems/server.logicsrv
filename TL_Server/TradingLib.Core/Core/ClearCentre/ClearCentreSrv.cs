using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TradingLib.API;

using TradingLib.Common;

using System.Data;
using System.Threading;
using System.Collections.Concurrent;



namespace TradingLib.Core
{
    /// <summary>
    /// 清算中心需要有多并发支持,这样才可以多线程接收客户端委托
    /// 1.初始化
    /// 2.从数据库恢复交易数据
    /// 3.结算账户
    /// 4.开启清算中心 //不做状态检查
    /// 5.关闭清算中心 //不做状态检查
    /// </summary>
    [CoreAttr(ClearCentre.CoreName,"清算中心","清算中心,用于维护交易帐号,交易记录,保证金核算,系统结算等功能")]
    public partial class ClearCentre : ClearCentreBase, IClearCentreSrv, ICore
    {
        const string CoreName = "ClearCentre";

        public string CoreId { get { return this.PROGRAME; } }

        #region 事件
        /// <summary>
        /// 添加交易帐号
        /// </summary>
        public event AccountIdDel AccountAddEvent;

        /// <summary>
        /// 删除交易帐号
        /// </summary>
        public event AccountIdDel AccountDelEvent;

        /// <summary>
        /// 激活交易帐号
        /// </summary>
        public event AccountIdDel AccountActiveEvent;

        /// <summary>
        /// 冻结交易帐号
        /// </summary>
        public event AccountIdDel AccountInActiveEvent;

        /// <summary>
        /// 加载交易帐号
        /// </summary>
        public event IAccountDel AccountCachedEvent;

        /// <summary>
        /// 调整手续费事件,对外触发手续费调整事件,用于相关逻辑进行手续费调整
        /// </summary>
        public event AdjustCommissionDel AdjustCommissionEvent;

        /// <summary>
        /// 交易回合结束
        /// </summary>
        public event PositionRoundClosedDel PositionRoundClosedEvent;

        /// <summary>
        /// 帐户修改事件
        /// </summary>
        public event AccountSettingChangedDel AccountChangedEvent;

        /// <summary>
        /// 对外触发带手续费的成交回报
        /// </summary>
        public event FillDelegate GotCommissionFill;

        #endregion

        QSEnumAccountLoadMode _loadmode = QSEnumAccountLoadMode.ALL;//账户加载模式

        AsyncTransactionLoger _asynLoger;//异步记录交易数据到数据库
        public AsyncTransactionLoger SqlLog { get { return _asynLoger; } }


        PositionRoundTracker prt;//记录交易回合信息
        /// <summary>
        /// 持仓回合管理器
        /// </summary>
        public PositionRoundTracker PositionRoundTracker { get { return this.prt; } }

        


        ConfigDB _cfgdb;
        decimal simAmount { get { return _cfgdb["DefaultSimAmount"].AsDecimal(); } }//获得默认资金
        string defaultpass { get { return _cfgdb["DefaultPass"].AsString(); } }


        int _maxorderseq = 0;
        object _orderseqobj = new object();
        /// <summary>
        /// 获得委托流水号
        /// </summary>
        public int NextOrderSeq
        {
            get
            {
                lock (_orderseqobj)
                {
                    _maxorderseq++;
                    return _maxorderseq;
                }
            }
        }

        public ClearCentre(QSEnumAccountLoadMode loadmode = QSEnumAccountLoadMode.ALL,QSEnumDebugLevel level = QSEnumDebugLevel.INFO):
            base("ClearCentre")
        {
            Status = QSEnumClearCentreStatus.CCINIT;

            //1.加载配置文件
            _cfgdb = new ConfigDB(ClearCentre.CoreName);
            if (!_cfgdb.HaveConfig("DefaultSimAmount"))
            {
                _cfgdb.UpdateConfig("DefaultSimAmount", QSEnumCfgType.Decimal, 1000000, "模拟帐户初始资金");
            }
            if (!_cfgdb.HaveConfig("DefaultPass"))
            {
                _cfgdb.UpdateConfig("DefaultPass", QSEnumCfgType.String,"123456", "模拟帐户在没有提供UserID进行直接创建时的默认交易密码");
            }

            DebugLevel = level;

            try
            {
                //设定加载模式
                _loadmode = loadmode;

                //初始化异步储存组件
                _asynLoger = new AsyncTransactionLoger();//获得交易信息数据库记录对象，用于记录委托，成交，取消等信息
                _asynLoger.SendDebugEvent += new DebugDelegate(msgdebug);
                
                //初始化PositionRound生成器
                prt = new PositionRoundTracker();
                prt.SendDebugEvent += new DebugDelegate(msgdebug);
                prt.FindSymbolEvent += (sym) => { return BasicTracker.SymbolTracker[sym]; };// new FindSecurity(getMasterSecurity);

                debug("Loading Accounts Infomation form database.....");
                //加载账户信息
                //LoadAccount();

                Status = QSEnumClearCentreStatus.CCINITFINISH;
            }
            catch (Exception ex)
            {
                TLCtxHelper.Debug("ex:" + ex.ToString());
                throw (new QSClearCentreInitError(ex, "ClearCentre初始化错误"));
            }
        }



        /// <summary>
        /// 查询当前是否是交易日
        /// </summary>
        public bool IsTradingday
        {
            get
            {
                return TLCtxHelper.Ctx.SettleCentre.IsTradingday;
            }
        }

        /// <summary>
        /// 清算中心重置,我们只需要清空交易记录,然后重新从数据库加载数据,即可
        /// 在每日结算完毕后,清算中心会重置,
        /// </summary>
        public void Reset()
        {
            if (!IsTradingday) return;//如果当天不是交易日则不进行重置
            Status = QSEnumClearCentreStatus.CCRESET;
            //清空账户交易记录
            debug("重置交易账户", QSEnumDebugLevel.INFO);
            foreach (IAccount a in this.Accounts)
            {
                acctk.ResetAccount(a);
                ////检查是否需要冻结帐号
                CheckAccountExecute(a);
            }
            //清空总计的交易合约
            totaltk.Clear();
            //清空positionround tracker 数据 准备初始化数据
            prt.Clear();
            //再从数据中恢复数据用于得到当前最新状态
            RestoreFromMysql();
            Notify("清算中心重置(结算后)[" + DateTime.Now.ToShortDateString() + "]", " ");
            Status = QSEnumClearCentreStatus.CCRESETFINISH;
        }



        #region 启动 停止 销毁
        public void InitAccount()
        {
            //加载所有帐户
            this.LoadAccount();
        }
        public void Start()
        {
            debug("启动清算中心...", QSEnumDebugLevel.INFO);
            _asynLoger.Start();
            //加载交易帐户
            //LoadAccount();
            //从数据库恢复日内交易信息
            RestoreFromMysql();
        }

        public void Stop()
        {
            debug("停止清算中心....", QSEnumDebugLevel.INFO);
            _asynLoger.Stop();
        }


        public override void Dispose()
        {
            base.Dispose();
            debug("ClearCentre Dispose Called", QSEnumDebugLevel.INFO);
            acctk.Dispose();
            _asynLoger.Dispose();
        }
        #endregion



        #region 清算中心 开启 关闭 以及状态更新
        const string OpenTime = "8:55";
        const string CloseTime = "15:15";
        const string NightOpenTime = "20:55";
        const string NightClosedTime = "23:59";
        const string NightOpenTime2 = "00:01";
        const string NightClosedTime2 = "02:30";

        QSEnumClearCentreStatus oldstatus = QSEnumClearCentreStatus.UNKNOWN;
        QSEnumClearCentreStatus _status = QSEnumClearCentreStatus.UNKNOWN;
        /// <summary>
        /// 清算中心状态
        /// </summary>
        internal QSEnumClearCentreStatus Status
        {
            get { return _status; }
            set
            {
                oldstatus = Status;//先保存原先状态
                _status = value;//再设定新的状态
                //更新当前状态 
                UpdateStatus();
            }
        }
       
        /// <summary>
        /// 自动更新清算中心的状态
        /// 1.清算中心初始化 ×
        /// 2.清算中心恢复交易数据 ×
        /// 3.清算中心开启 ×
        /// 4.清算中心关闭 ×
        /// 5.清算中心数据检验 
        /// 6.清算中心保存数据
        /// 6.清算中心结算
        /// 7.清算中心重置
        /// </summary>
        void UpdateStatus()
        {

            //状态由 恢复->恢复完成改变 则我们检查当前的时间,如果是清算中心接收委托事件 则设定对应状态
            //交易日内 盘中 启动软件
            if (oldstatus == QSEnumClearCentreStatus.CCRESTORE && Status == QSEnumClearCentreStatus.CCRESTOREFINISH)
            {
                bool day = LibUtil.IsInPeriod(Convert.ToDateTime(OpenTime), Convert.ToDateTime(CloseTime));
                bool nigth1 = LibUtil.IsInPeriod(Convert.ToDateTime(NightOpenTime), Convert.ToDateTime(NightClosedTime));
                bool night2 = LibUtil.IsInPeriod(Convert.ToDateTime(NightOpenTime2), Convert.ToDateTime(NightClosedTime2));

                if (day || nigth1 || night2)
                    Status = QSEnumClearCentreStatus.CCOPEN;
                else
                    Status = QSEnumClearCentreStatus.CCCLOSE;
            }
        }

        /// <summary>
        /// 开启清算中心
        /// </summary>
        internal void OpenClearCentre()
        {
            Status = QSEnumClearCentreStatus.CCOPEN;
        }
        /// <summary>
        /// 关闭清算中心
        /// </summary>
        internal void CloseClearCentre()
        {
            Status = QSEnumClearCentreStatus.CCCLOSE;
        }
        #endregion


        #region 辅助函数 
        protected void AccountChanged(IAccount account)
        {
            if (AccountChangedEvent != null)
                AccountChangedEvent(account);
        }
        #endregion


    }
}
