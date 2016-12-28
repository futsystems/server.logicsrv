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
    public partial class ClearCentre : BaseSrvObject, IModuleClearCentre
    {
        const string CoreName = "ClearCentre";
        public string CoreId { get { return this.PROGRAME; } }

        PositionRoundTracker prt;//记录交易回合信息

        ConfigDB _cfgdb;

        /// <summary>
        /// 分帐户交易数据维护器
        /// </summary>
        protected AccountTracker acctk = new AccountTracker();

        /// <summary>
        /// 总交易数据维护器
        /// </summary>
        protected TotalTracker totaltk = new TotalTracker();



        public ClearCentre():
            base("ClearCentre")
        {
            Status = QSEnumClearCentreStatus.CCINIT;

            //1.加载配置文件
            _cfgdb = new ConfigDB(ClearCentre.CoreName);

            try
            {
                acctk.NewPositionEvent += new Action<Position>(acctk_NewPositionEvent);
                acctk.NewPositionCloseDetailEvent += new Action<Trade, PositionCloseDetail>(acctk_NewPositionCloseDetailEvent);
                acctk.NewPositionDetailEvent += new Action<Trade, PositionDetail>(acctk_NewPositionDetailEvent);
     
                //帐户交易数据维护器产生 平仓明细事件
                //acctk.NewPositionCloseDetailEvent += new Action<Trade,PositionCloseDetail>(acctk_NewPositionCloseDetailEvent);
                
                //初始化PositionRound生成器
                prt = new PositionRoundTracker();

                Status = QSEnumClearCentreStatus.CCINITFINISH;
            }
            catch (Exception ex)
            {
                Util.Debug("ex:" + ex.ToString());
                throw (new QSClearCentreInitError(ex, "ClearCentre初始化错误"));
            }

            //TLCtxHelper.EventSystem.SettleResetEvent +=new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        //void  EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        //{
        //    this.Reset();
        //}

        void acctk_NewPositionDetailEvent(Trade arg1, PositionDetail arg2)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[arg1.Account];
            if (account != null)
            {
                account.FirePositoinDetailEvent(arg1, arg2);
            }
        }

        /// <summary>
        /// 当有持仓关闭时出发持仓关闭时间
        /// </summary>
        /// <param name="obj"></param>
        //void acctk_NewPositionCloseDetailEvent(Trade obj1, PositionCloseDetail obj2)
        //{
            
        //}

        //当帐户交易对象维护器产生持仓时，我们将持仓加入total维护其列表用于快速访问
        void acctk_NewPositionEvent(Position obj)
        {
            logger.Info("new postion created " + obj.GetPositionKey());
            totaltk.NewPosition(obj);

        }


        /// <summary>
        /// 保存平仓明细记录
        /// </summary>
        /// <param name="obj"></param>
        void acctk_NewPositionCloseDetailEvent(Trade f,PositionCloseDetail obj)
        {
            if (_status == QSEnumClearCentreStatus.CCOPEN)
            {
                logger.Info("New PositionCloseDetail:" + obj.GetPositionCloseStr());
                TLCtxHelper.ModuleDataRepository.NewPositionCloseDetail(obj);

                IAccount account = TLCtxHelper.ModuleAccountManager[obj.Account];
                if (account != null)
                {
                    account.FirePositionCloseDetailEvent(f, obj);
                }
            }
        }

        /// <summary>
        /// 清算中心重置,我们只需要清空交易记录,然后重新从数据库加载数据,即可
        /// 在每日结算完毕后,清算中心会重置,
        /// 重置交易帐户数据 清空帐户交易记录 然后从数据库加载数据恢复到内存
        /// </summary>
        public void Reset()
        {
            Status = QSEnumClearCentreStatus.CCRESET;
            
            //清空分帐户维护器交易记录
            logger.Info("清算中心重置");
            foreach (IAccount a in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                a.Reset();
                acctk.ResetAccount(a);
            }

            //清空总维护器数据
            totaltk.Clear();

            //清空positionround tracker 数据 准备初始化数据
            prt.Clear();

            //从数据中恢复数据用于得到当前最新状态包含持仓,PR,出入金等数据
            Restore();

            Status = QSEnumClearCentreStatus.CCRESETFINISH;
        }


        public IEnumerable<PositionRound> TotalRoundOpend
        {
            get
            {
                return prt.RoundClosed;
            }
        }

        #region 启动 停止 销毁
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            Restore();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            acctk.Dispose();
        }
        #endregion


        #region 添加或删除交易帐户到清算服务的内存数据哭
        /// <summary>
        /// 将某个账户缓存到服务器内存，注意检查是否已经存在该账户
        /// 生成该账户所对应的数据对象用于实时储存交易信息与合约信息
        /// </summary>
        /// <param name="a"></param>
        public void CacheAccount(IAccount a)
        {
            acctk.CacheAccount(a);
        }

        /// <summary>
        /// 将某个帐户从内存中删除
        /// </summary>
        /// <param name="a"></param>
        public void DropAccount(IAccount a)
        {
            //将该交易帐户的委托 成交 持仓 从统计维护器中删除
            foreach (Order o in a.Orders)
            {
                totaltk.DropOrder(o);
            }

            foreach (Trade f in a.Trades)
            {
                totaltk.DropFill(f);
            }

            foreach (Position p in a.Positions)
            {
                totaltk.DropPosition(p);
            }
            //将帐户从帐户维护器中删除
            acctk.DropAccount(a);
        }

        /// <summary>
        /// 清空某个交易帐户的交易记录
        /// </summary>
        public void ResetAccount(IAccount a)
        {
            //将该交易帐户的委托 成交 持仓 从统计维护器中删除
            foreach (Order o in a.Orders)
            {
                totaltk.DropOrder(o);
            }

            foreach (Trade f in a.Trades)
            {
                totaltk.DropFill(f);
            }

            foreach (Position p in a.Positions)
            {
                totaltk.DropPosition(p);
            }

            //将交易帐户 交易记录维护器中该帐户的交易记录清空
            acctk.ResetAccount(a);
        }
        #endregion


        #region 清算中心 开启 关闭 以及状态更新
        QSEnumClearCentreStatus oldstatus = QSEnumClearCentreStatus.UNKNOWN;
        QSEnumClearCentreStatus _status = QSEnumClearCentreStatus.UNKNOWN;
        /// <summary>
        /// 清算中心状态
        /// </summary>
        public QSEnumClearCentreStatus Status
        {
            get { return _status; }
            private set
            {
                oldstatus = Status;//先保存原先状态
                _status = value;//再设定新的状态
                //更新当前状态 
                UpdateStatus();
            }
        }
       
        
        void UpdateStatus()
        {
            //状态由 恢复->恢复完成改变 则我们检查当前的时间,如果是清算中心接收委托时间段 则设定对应状态
            //交易日内 盘中 启动软件
            if (oldstatus == QSEnumClearCentreStatus.CCRESTORE && Status == QSEnumClearCentreStatus.CCRESTOREFINISH)
            {
                //结算时间前5分钟 重置时候后5分钟
                DateTime close = Util.ToDateTime(Util.ToTLDate(DateTime.Now),TLCtxHelper.ModuleSettleCentre.SettleTime).AddMinutes(-5);
                DateTime open = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TLCtxHelper.ModuleSettleCentre.ResetTime).AddMinutes(5);

                //如果在关闭时间区间内则系统关闭 否则系统开启
                bool islcose = Util.IsInPeriod(close, open);
                if (!islcose)
                    Status = QSEnumClearCentreStatus.CCOPEN;
                else
                    Status = QSEnumClearCentreStatus.CCCLOSE;
            }
        }

        /// <summary>
        /// 开启清算中心
        /// </summary>
        public void OpenClearCentre()
        {
            Status = QSEnumClearCentreStatus.CCOPEN;
        }
        /// <summary>
        /// 关闭清算中心
        /// </summary>
        public void CloseClearCentre()
        {
            Status = QSEnumClearCentreStatus.CCCLOSE;
        }
        #endregion

    }
}
