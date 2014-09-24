using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;

/**
 * 配资模块
 * 配资模块是用于扩展帐户资金，通过扩展帐户可用资金或者改变保证金计算方式达到提高杠杆的目的
 * 配资按服务模式可以分为
 * 1.日息收入
 * 2.盈利分红
 * 3.手续费加成
 * 4.股指转向
 * 等
 * 每种不同配资模式 有不同的资金扩展模式和收费模式 同时也有不同的风控模式
 * a.实现目的 每个客户可以选择不同的配资服务，按照配资服务的不同设定不同的费率参数 从而实现不同的计费
 * b.按收费方式分成结算后计算和盘中手续费计算等方式 将每个帐户的服务费进行统计
 * c.将服务费分润到对应的交易所
 * 
 * 
 *
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * **/
namespace TradingLib.Contrib.FinService
{

    [ContribAttr(FinServiceCentre.ContribName, "FinServiceCentre期货配资扩展", "用于提供配资业务逻辑")]
    public partial class FinServiceCentre : ContribSrvObject, IContrib
    {


        const string ContribName = "FinServiceCentre";
        FeeChargeItemLogger _chargelog = new FeeChargeItemLogger();
        ConfigDB _cfgdb;

        public FinServiceCentre()
            : base(FinServiceCentre.ContribName)
        {

        }

        string _defaultspclassname = "TradingLib.Contrib.FinService.SPSpecialIF";
        bool _addservice = true;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("FinServiceCentre loading......", QSEnumDebugLevel.INFO);

            //从数据库加载参数
            _cfgdb = new ConfigDB(FinServiceCentre.ContribName);
            if (!_cfgdb.HaveConfig("DefaultSPClassName"))
            {
                _cfgdb.UpdateConfig("DefaultSPClassName", QSEnumCfgType.String, "TradingLib.Contrib.FinService.SPSpecialIF", "默认开通的配资服务计划");
            }
            _defaultspclassname = _cfgdb["DefaultSPClassName"].AsString();

            if (!_cfgdb.HaveConfig("AddFinServiceOnCreate"))
            {
                _cfgdb.UpdateConfig("AddFinServiceOnCreate", QSEnumCfgType.Bool,true, "默认是否开通的配资服务");
            }
            _addservice = _cfgdb["AddFinServiceOnCreate"].AsBool();

            //从扩展目录加载所有实现IFinService的类型
            IList<Type> types = PluginHelper.GetImplementors("Contrib", typeof(IFinService));
            foreach (Type t in types)
            {
                debug("Load ServicePlane Type:" + t.FullName, QSEnumDebugLevel.INFO);
                //同步服务计划 ServicePlane
                FinTracker.ServicePlaneTracker.InitServicePlan(t);
            }

            debug("Load Service Instance......", QSEnumDebugLevel.INFO);
            FinTracker.FinServiceTracker.ToArray();

            //foreach (FinServiceStub fs in FinTracker.FinServiceTracker.ToArray())
            //{
            //    debug("finservice:" + fs.ToString(), QSEnumDebugLevel.INFO);
            //    //fs.InitFinService();
            //}

            FinTracker.FinServiceTracker.GotFeeChargeItemEvent += new FeeChargeItemDel(_chargelog.newFeeChargeItem);
            //手续费调整事件
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(ExContribEvent_AdjustCommissionEvent);
            //获得成交事件
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
            //获得持仓回合关闭事件 一个交易开平结束
            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new PositionRoundClosedDel(EventIndicator_GotPositionClosedEvent);

            //帐户添加事件
            TLCtxHelper.EventAccount.AccountAddEvent += new AccountIdDel(EventAccount_AccountAddEvent);
        }

        /// <summary>
        /// 响应帐户添加事件 当有实盘帐号生成时
        /// 按照设定的参数进行操作
        /// 比如默认添加某种类型的配资服务
        /// </summary>
        /// <param name="account"></param>
        void EventAccount_AccountAddEvent(string account)
        {
            if(_addservice)
            {
                DBServicePlan sp = FinTracker.ServicePlaneTracker[_defaultspclassname];
                IAccount acc = TLCtxHelper.CmdAccount[account];
                //如果是实盘帐号则默认给他开通配资服务
                if (acc != null && sp!=null && (acc.Category == QSEnumAccountCategory.REAL|| acc.Category == QSEnumAccountCategory.DEALER))
                {
                    //如果帐户存在并且服务计划存在 则为该帐户添加对应的配资服务
                    FinTracker.FinServiceTracker.AddFinService(account,sp.ID);
                }
            }
        }






        

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            _chargelog.Start();

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }
    }
}
