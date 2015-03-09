﻿using System;
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
                _cfgdb.UpdateConfig("AddFinServiceOnCreate", QSEnumCfgType.Bool, false, "默认是否开通的配资服务");
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


            //将服务的计费日志导出
            FinTracker.FinServiceTracker.GotFeeChargeItemEvent += new FeeChargeItemDel(_chargelog.newFeeChargeItem);

            //配资资金查询事件
            TLCtxHelper.ExContribEvent.GetFinAmmountAvabileEvent += new AccountFinAmmountDel(ExContribEvent_GetFinAmmountAvabileEvent);

            //手续费调整事件
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(ExContribEvent_AdjustCommissionEvent);
            
            //获得成交事件
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
            
            //获得持仓回合关闭事件 一个交易开平结束
            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new PositionRoundClosedDel(EventIndicator_GotPositionClosedEvent);

            //帐户添加事件
            TLCtxHelper.EventAccount.AccountAddEvent += new AccoundIDDel(EventAccount_AccountAddEvent);

            //帐户激活事件
            TLCtxHelper.EventAccount.AccountActiveEvent += new AccoundIDDel(EventAccount_AccountActiveEvent);

            //出入金事件
            TLCtxHelper.EventSystem.CashOperationRequest += new EventHandler<CashOperationEventArgs>(CashOperationEvent_CashOperationRequest);

            //结算前 结算前事件
            TLCtxHelper.EventSystem.BeforeSettleEvent += new EventHandler<SystemEventArgs>(EventSystem_BeforeSettleEvent);
            TLCtxHelper.EventSystem.AfterSettleEvent += new EventHandler<SystemEventArgs>(EventSystem_AfterSettleEvent);

            


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

        /// <summary>
        /// 查询某个交易帐户的配资服务所扩展的配资资金
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        decimal ExContribEvent_GetFinAmmountAvabileEvent(string account)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            if (stub == null) return 0;
            return stub.FinService.GetFinAmountAvabile();
        }



        /// <summary>
        /// 在帐户结算前 配资服务中按日收费部分
        /// 比如收取利息 盈利分成需要生成服务收费记录并统一采集后对帐户作出入金操作以作扣费
        /// 在扣费完成后执行结算 则将扣费记录对应的结算日内
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventSystem_BeforeSettleEvent(object sender, SystemEventArgs e)
        {
            debug("系统将进行结算,结算前配资中心执行交易帐户收费结算 用于收取盘后结算的费用", QSEnumDebugLevel.INFO);

            //1.运行所有配资服务的结算响应回调 比如按每天收取利息 或者按盈利分红的计费模式
            foreach (FinServiceStub stub in FinTracker.FinServiceTracker)
            {
                stub.FinService.OnSettle();//执行结算回调 比如盈利分红的收费 则在onsettle中执行计费与记录
            }

            //2.检查当天所有的收费记录，对于结算后收取的 进行出入金操作 将盘后计算的配资费用通过出入金方式从帐户中扣除
            //如果计费缓存不为空则等待计费记录写入数据库
            while (!_chargelog.IsEmpty)
            {
                Util.sleep(500);
            }

            foreach (FeeChargeItem item in ORM.MFeeChargeItem.SelectFeeChargeItemAfterSettle())
            {
                Util.Debug("结算后采集计费:" + item.Comment);
                if (item.CollectType == EnumFeeCollectType.CollectAfterSettle)
                {
                    try
                    {
                        TLCtxHelper.CmdAccount.CashOperation(item.Account, item.TotalFee * -1, "", item.Comment);
                    }
                    catch (FutsRspError ex)
                    {
                        debug("FinService CashOperation error:" + ex.ErrorMessage, QSEnumDebugLevel.ERROR);
                    }
                    catch (Exception ex)
                    {
                        debug("FinService CashOperation general error:"+ex.ToString(), QSEnumDebugLevel.ERROR);
                    }
                }
            }
        }


        /// <summary>
        /// 交易帐户结算后 执行代理结算 
        /// 将代理当日的服务收费，代理佣金，出入金等记录形成结算记录插入并更新代理的Balance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventSystem_AfterSettleEvent(object sender, SystemEventArgs e)
        {
            debug("核心系统结算完毕,结算后进行代理上财务结算，生成代理算记录", QSEnumDebugLevel.INFO);
            //1.结算代理商
            foreach (Manager mgr in BasicTracker.ManagerTracker.GetBaseManagers())
            {
                ORM.MAgentSettlement.SettleAgent(mgr);
            }
        }


        /// <summary>
        /// 响应出入金操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CashOperationEvent_CashOperationRequest(object sender, CashOperationEventArgs e)
        {
            //出入金操作
            string account = e.CashOperation.Account;
            if (string.IsNullOrEmpty(account)) return;//如果不是交易帐户的出入金
            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            if (stub == null) return;//不存在对应的配资服务


            debug("配资本中心获得出入金事件,对配资服务进行调整", QSEnumDebugLevel.INFO);

            stub.FinService.OnCashOperation(e.CashOperation);
        }

        /// <summary>
        /// 响应帐户添加事件 当有实盘帐号生成时
        /// 按照设定的参数进行操作
        /// 比如默认添加某种类型的配资服务
        /// </summary>
        /// <param name="account"></param>
        void EventAccount_AccountAddEvent(string account)
        {
            if (_addservice)
            {
                DBServicePlan sp = FinTracker.ServicePlaneTracker[_defaultspclassname];
                IAccount acc = TLCtxHelper.CmdAccount[account];
                //如果是实盘帐号则默认给他开通配资服务
                if (acc != null && sp != null && (acc.Category == QSEnumAccountCategory.REAL))
                {
                    //如果帐户存在并且服务计划存在 则为该帐户添加对应的配资服务
                    FinTracker.FinServiceTracker.AddFinService(account, sp.ID);
                }
            }
        }

        /// <summary>
        /// 帐户激活事件
        /// 用于重置服务相关状态
        /// </summary>
        /// <param name="account"></param>
        void EventAccount_AccountActiveEvent(string account)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            if (stub == null) return;//不存在对应的配资服务

            stub.FinService.OnAccountActive(account);

        }
    }
}