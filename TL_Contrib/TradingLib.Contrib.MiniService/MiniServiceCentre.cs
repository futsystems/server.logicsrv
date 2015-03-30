using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;


namespace TradingLib.Contrib.MiniService
{
    [ContribAttr(MiniServiceCentre.ContribName, "迷你合约服务", "用于提供帐户交易基于标准合约构建的迷你合约")]
    public partial class MiniServiceCentre : ContribSrvObject, IContrib
    {
        const string ContribName = "MiniService";
        public MiniServiceCentre()
            : base(MiniServiceCentre.ContribName)
        {


        }
        ConfigDB _cfgdb;
        //MJServiceTracker mjtracker = null;

        MinServiceTracker tracker = null;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(MiniServiceCentre.ContribName);

            if (!_cfgdb.HaveConfig("Commission_L1"))
            {
                _cfgdb.UpdateConfig("Commission_L1", QSEnumCfgType.Decimal, 10, "L1手续费加收比例");
            }

            tracker = new MinServiceTracker();
            //mjtracker = new MJServiceTracker();
            //MJService[] ms = mjtracker.MJServices;
            //foreach (MJService mj in ms)
            //{
            //    debug(mj.ToString(), QSEnumDebugLevel.MUST);
            //}

            //用户创建时 添加秘籍服务
            //TLCtxHelper.EventAccount.AccountAddEvent += new AccountIdDel(OnAccountAdded);

            //有成交时 如果该成交是秘籍服务产生的成交 则进行手续费调整
            //TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(OnAdjustCommissionEvent);

        }



        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            //用户创建时 添加秘籍服务
            //TLCtxHelper.EventAccount.AccountAddEvent -= new AccountIdDel(OnAccountAdded);

            //有成交时 如果该成交是秘籍服务产生的成交 则进行手续费调整
            //TLCtxHelper.ExContribEvent.AdjustCommissionEvent -= new AdjustCommissionDel(OnAdjustCommissionEvent);
            base.Dispose();
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("starting..................", QSEnumDebugLevel.INFO);


        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// 获得秘籍服务的持仓
        /// 交易证券品种是异化,合约字头为LTTO 并且持仓数量不为0
        /// </summary>
        /// <returns></returns>
        Position[] GetINNOVLottoPositoins()
        {
            return null;// TLCtxHelper.CmdTradingInfo.getPositions(null).Where(delegate(Position pos) { return !pos.isFlat && pos.oSymbol.SecurityType == SecurityType.INNOV && pos.oSymbol.SecurityFamily.Code.Equals("LOTTO"); }).ToArray();
        }


        decimal OnAdjustCommissionEvent(Trade fill, PositionRound positionround)
        {
            ////检查对应帐户的秘籍服务
            //MJService mj = mjtracker[fill.Account];
            //if (mj != null)
            //{
            //    //如果交易的是乐透品种 则按秘籍服务进行收费 //或者之间按照交易所为INNOV进行判断
            //    if (fill.oSymbol.SecurityFamily.Code == "LOTTO")
            //    {
            //        return mj.AdjustCommission(fill, positionround);
            //    }
            //}
            ////如果秘籍服务不存在 则直接返回原来的手续费
            //return fill.Commission;
            return 0;
        }


        void OnAccountAdded(string account)
        {
            //try
            //{
            //    IAccount acc = TLCtxHelper.CmdAccount[account];

            //    if (acc == null)
            //    {
            //        //无有效Account 
            //        debug("交易帐户:" + account + "不存在,无法为其绑定秘籍服务", QSEnumDebugLevel.WARNING);
            //    }

            //    //为用户创建秘籍服务
            //    AddMJService(acc, MJConstant.DEFAULT_FEETYPE, MJConstant.DEFAULT_LEVEL);

            //    //延长秘籍服务至我们预先设定的体验天数
            //    ExtensionService(acc, 0, 0, MJConstant.DEFAULT_DEMO_DAYS);
            //}
            //catch (Exception ex)
            //{
            //    debug("帐户创建后为其绑定秘籍服务失败:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            //}

        }


    }
}
