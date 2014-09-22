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


        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("$$$$$$$$$$$$$$$ loading...............", QSEnumDebugLevel.INFO);
            IList<Type> types = PluginHelper.GetImplementors("Contrib", typeof(IFinService));
            debug("got types:" + types.Count.ToString(), QSEnumDebugLevel.INFO);
            foreach (Type t in types)
            {
                debug("加载服务计划:" + t.FullName, QSEnumDebugLevel.INFO);
                //同步服务计划 ServicePlane
                FinTracker.ServicePlaneTracker.InitServicePlan(t);
                //DBServicePlan sp = FinTracker.ServicePlaneTracker[t.FullName];
                //FinTracker.ArgumentTracker.GetAgentArgument(2,sp.ID);

            }

            debug("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx", QSEnumDebugLevel.INFO);
            foreach (FinServiceStub fs in FinTracker.FinServiceTracker.ToArray())
            {
                debug("finservice:" + fs.ToString(), QSEnumDebugLevel.INFO);
                //fs.InitFinService();
            }

            FinTracker.FinServiceTracker.GotFeeChargeItemEvent += new FeeChargeItemDel(_chargelog.newFeeChargeItem);
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(ExContribEvent_AdjustCommissionEvent);
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new IPositionRoundDel(EventIndicator_GotPositionClosedEvent);
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
