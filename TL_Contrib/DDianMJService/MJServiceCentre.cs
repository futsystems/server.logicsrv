using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace Lottoqq.MJService
{
    [ContribAttr(MJServiceCentre.ContribName, "秘籍服务中心", "秘籍服务中心为帐户提供了秘籍服务扩展,用于提供帐户交易异化乐透类型的合约")]
    public partial class MJServiceCentre : ContribSrvObject, IContrib
    {
        const string ContribName = "DDianMJService";
        public MJServiceCentre()
            : base(MJServiceCentre.ContribName)
        {


        }
        ConfigDB _cfgdb;
        MJServiceTracker mjtracker = null;

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(MJServiceCentre.ContribName);

            if (!_cfgdb.HaveConfig("Commission_L1"))
            {
                _cfgdb.UpdateConfig("Commission_L1", QSEnumCfgType.Decimal,10, "L1手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("Commission_L2"))
            {
                _cfgdb.UpdateConfig("Commission_L2", QSEnumCfgType.Decimal, 10, "L2手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("Commission_L3"))
            {
                _cfgdb.UpdateConfig("Commission_L3", QSEnumCfgType.Decimal, 10, "L3手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("Commission_L4"))
            {
                _cfgdb.UpdateConfig("Commission_L4", QSEnumCfgType.Decimal, 10, "L4手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("Commission_L5"))
            {
                _cfgdb.UpdateConfig("Commission_L5", QSEnumCfgType.Decimal, 10, "L5手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("Commission_LT"))
            {
                _cfgdb.UpdateConfig("Commission_LT", QSEnumCfgType.Decimal, 10, "LT手续费加收比例");
            }

            if (!_cfgdb.HaveConfig("DemoType"))
            {
                _cfgdb.UpdateConfig("DemoType", QSEnumCfgType.String, "ByCounts", "新帐户默认创建的乐透服务类型");
            }

            if (!_cfgdb.HaveConfig("DemoDays"))
            {
                _cfgdb.UpdateConfig("DemoDays", QSEnumCfgType.Int, 5, "乐透服务免费使用时长");
            }

            if (!_cfgdb.HaveConfig("DemoCounts"))
            {
                _cfgdb.UpdateConfig("DemoCounts", QSEnumCfgType.Int, 100, "乐透服务免费使用次数");
            }








            mjtracker = new MJServiceTracker();
            MJService[] ms = mjtracker.MJServices;
            foreach (MJService mj in ms)
            {
                debug(mj.ToString(),QSEnumDebugLevel.MUST);
            }

            //用户创建时 添加秘籍服务
            TLCtxHelper.EventAccount.AccountAddEvent += new AccountIdDel(OnAccountAdded);

            //有成交时 如果该成交是秘籍服务产生的成交 则进行手续费调整
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(OnAdjustCommissionEvent);

        }



        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            //用户创建时 添加秘籍服务
            TLCtxHelper.EventAccount.AccountAddEvent -= new AccountIdDel(OnAccountAdded);

            //有成交时 如果该成交是秘籍服务产生的成交 则进行手续费调整
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent -= new AdjustCommissionDel(OnAdjustCommissionEvent);
            base.Dispose();
        }
        
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("starting..................",QSEnumDebugLevel.INFO);

            
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


        decimal OnAdjustCommissionEvent(Trade fill, IPositionRound positionround)
        {
            //检查对应帐户的秘籍服务
            MJService mj = mjtracker[fill.Account];
            if (mj != null)
            {
                //如果交易的是乐透品种 则按秘籍服务进行收费 //或者之间按照交易所为INNOV进行判断
                if (fill.oSymbol.SecurityFamily.Code == "LOTTO")
                {
                    return mj.AdjustCommission(fill, positionround);
                }
            }
            //如果秘籍服务不存在 则直接返回原来的手续费
            return fill.Commission;
            
        }


        void OnAccountAdded(string account)
        {
            try
            {
                IAccount acc = TLCtxHelper.CmdAccount[account];

                if (acc == null)
                {
                    //无有效Account 
                    debug("交易帐户:" + account + "不存在,无法为其绑定秘籍服务", QSEnumDebugLevel.WARNING);
                }

                //为用户创建秘籍服务
                AddMJService(acc, MJConstant.DEFAULT_FEETYPE, MJConstant.DEFAULT_LEVEL);

                //延长秘籍服务至我们预先设定的体验天数
                ExtensionService(acc,0,0,MJConstant.DEFAULT_DEMO_DAYS);
            }
            catch (Exception ex)
            {
                debug("帐户创建后为其绑定秘籍服务失败:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }

        }


        /// <summary>
        /// 为某个帐号添加某个类型的秘籍服务 并制定秘籍档位为多少级
        /// </summary>
        /// <param name="account"></param>
        /// <param name="feetype"></param>
        /// <param name="level"></param>
        void AddMJService(IAccount acc, QSEnumMJFeeType feetype, QSEnumMJServiceLevel level)
        {
            

            MJService mj = new MJService(feetype, level);
            mj.Account = acc;

            //添加秘籍服务
            mjtracker.AddMJService(mj);

           
        }


        /// <summary>
        /// 延长秘籍服务时间
        /// </summary>
        /// <param name="account"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        void ExtensionService(IAccount acc, int year, int month, int day)
        {
            MJService mj = mjtracker[acc.ID];
            if (mj == null)
            { 
                //account do not have mjservice avabile
                throw new MJErrorMJServiceNotExist();
            }

            if (mj.FeeType == QSEnumMJFeeType.ByCommission)
            {
                throw new MJError(MJConstant.FEETYPE_NOT_SUPPORT_EXTTIME);
            }

            DateTime tnow = mj.IsExpired ? DateTime.Now : mj.ExpiredDate;//如果秘籍服务已经过期,则返回当前时间,如果秘籍服务已经过期则返回当前时间
            DateTime t = tnow.AddYears(year).AddMonths(month).AddDays(day);
            mjtracker.UpdateMJExpire(mj, t);
        }

        
        /// <summary>
        /// 更新某个帐户的秘籍服务计费类别
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        void ChangeFeeType(IAccount acc, QSEnumMJFeeType type)
        {
            MJService mj = mjtracker[acc.ID];
            if (mj == null)
            {
                //account do not have mjservice avabile
                throw new MJErrorMJServiceNotExist();
            }

            //计费类别相同
            if (mj.FeeType == type)
            {
                throw new MJError(MJConstant.FEETYPE_SAME);
            }

            
            mjtracker.UpdateMJFeeType(mj, type);
        }

        /// <summary>
        /// 修改秘籍服务的秘籍档位
        /// </summary>
        /// <param name="account"></param>
        /// <param name="level"></param>
        void ChangeMJLevel(IAccount acc, QSEnumMJServiceLevel level)
        {
            MJService mj = mjtracker[acc.ID];
            if (mj == null)
            {
                //account do not have mjservice avabile
                throw new MJErrorMJServiceNotExist();
            }

            //计费类别相同
            if (mj.Level == level)
            {
                throw new MJError(MJConstant.MJLEVEL_SAME);
            }

            //降低秘籍合约
            if ((int)mj.Level > (int)level)
            {
                acc.FlatPosition(QSEnumOrderSource.UNKNOWN, "降级秘籍合约,全平持仓");
                Thread.Sleep(1000);
            }
            mjtracker.UpdateMJLevel(mj, level);
        }
    }
}
