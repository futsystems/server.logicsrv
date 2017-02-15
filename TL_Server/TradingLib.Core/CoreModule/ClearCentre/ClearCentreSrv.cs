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

        bool _live = false;
        /// <summary>
        /// 清算中心是否处于开启状态 清算中心开启可以接受客户端提交的委托
        /// 结算前5分钟关闭清算中心 重置后5分钟开启清算中心
        /// 清算中心工作模式
        /// 1.程序启动后 加载未结算数据 清算中心根据当前时间判定 是否处于工作状态
        /// 2.程序常驻运行中 结算中心定时任务 自动关闭与开启清算中心
        /// 2.手工结算模式 设定交易日后 清算中心重置加载对应交易日的交易数据 此时需要设定清算中心未关闭 否则会多次执行数据库操作
        /// </summary>
        public bool IsLive { get { return _live; } }

        ConfigDB _cfgdb;

        /// <summary>
        /// 分帐户交易数据维护器
        /// </summary>
        protected AccountTracker acctk = null;

        /// <summary>
        /// 总交易数据维护器
        /// </summary>
        protected TotalTracker totaltk = null;



        public ClearCentre():
            base("ClearCentre")
        {

            //1.加载配置文件
            _cfgdb = new ConfigDB(ClearCentre.CoreName);

            
            acctk = new AccountTracker();
            totaltk = new TotalTracker();
            acctk.NewPositionEvent += new Action<Position>(acctk_NewPositionEvent);
            acctk.NewPositionCloseDetailEvent += new Action<Trade, PositionCloseDetail>(acctk_NewPositionCloseDetailEvent);
            acctk.NewPositionDetailEvent += new Action<Trade, PositionDetail>(acctk_NewPositionDetailEvent);

            logger.Info("Create data tracker");
            //TLCtxHelper.EventSystem.SettleResetEvent +=new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        //void  EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        //{
        //    this.Reset();
        //}

        /// <summary>
        /// 开仓后 持仓明细生成 触发对应账户的持仓明细事件
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void acctk_NewPositionDetailEvent(Trade arg1, PositionDetail arg2)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[arg1.Account];
            if (account != null)
            {
                account.FirePositoinDetailEvent(arg1, arg2);
            }
        }

        /// <summary>
        /// 账户维护器有新的持仓对象创建时 将持仓放入数据维护器 用于快速访问
        /// </summary>
        /// <param name="obj"></param>
        void acctk_NewPositionEvent(Position obj)
        {
            logger.Info("New Postion Created:" + obj.GetPositionKey());
            totaltk.NewPosition(obj);

        }

        /// <summary>
        /// 保存平仓明细记录
        /// </summary>
        /// <param name="obj"></param>
        void acctk_NewPositionCloseDetailEvent(Trade f,PositionCloseDetail obj)
        {
            if (this.IsLive)//手工结算时 手工平仓 如何记录平仓明细数据
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
            //清空分帐户维护器交易记录
            logger.Info("清算中心重置");
            foreach (IAccount a in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                a.Reset();
                acctk.Reset(a);
            }

            //清空总维护器数据
            totaltk.Clear();

            //加载交易记录
            LoadData();
        }



        #region 启动 停止 销毁
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            LoadData();

            //启动后根据当前时间判定清算中心状态 结算中心在结算前5分钟关闭系统 在重置系统后5分钟打开系统
            DateTime close = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TLCtxHelper.ModuleSettleCentre.SettleTime).AddMinutes(-5);
            DateTime open = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TLCtxHelper.ModuleSettleCentre.ResetTime).AddMinutes(5);

            //如果在关闭时间区间内则系统关闭 否则系统开启
            _live = !Util.IsInPeriod(close, open);


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

        ///// <summary>
        ///// 清空某个交易帐户的交易记录
        ///// </summary>
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
            acctk.Reset(a);
        }
        #endregion

        /// <summary>
        /// 开启清算中心
        /// </summary>
        public void OpenClearCentre()
        {
            logger.Info("Open ClearCentre");
            _live = true;
        }
        /// <summary>
        /// 关闭清算中心
        /// </summary>
        public void CloseClearCentre()
        {
            logger.Info("Close ClearCentre");
            _live = false;
        }
    }
}
