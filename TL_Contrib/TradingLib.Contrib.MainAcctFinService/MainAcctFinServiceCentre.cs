using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace TradingLib.Contrib.MainAcctFinService
{
    [ContribAttr(MainAcctFinServiceCentre.ContribName, "主帐户配资服务", "主帐户配资服务")]
    public partial class MainAcctFinServiceCentre:BaseSrvObject, IContrib
    {
        const string ContribName = "MainAcctFinService";
        public MainAcctFinServiceCentre()
            : base(MainAcctFinServiceCentre.ContribName)
        {

            TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            this.Reset();
        }


        FinServiceCentreStatus _status = new FinServiceCentreStatus();
        //FinServiceTracker _fstracker = null;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
        
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
            logger.Info("MainAcctFinService Starting.....");
            //_fstracker = new FinServiceTracker();
            FinGlobal.Init();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
        
        }


        public void Reset()
        {
            _status.Reset();
            FinGlobal.BrokerAccountInfoTracker.Clear();
            FinGlobal.BrokerTransTracker.Clear();
            //FinGlobal.FinServiceTracker
        }
        //ConcurrentDictionary<string, XAccountInfo> connectorAccountInfoMap = new ConcurrentDictionary<string, XAccountInfo>();

        //bool HaveXAccountInfo(string account)
        //{
        //    return connectorAccountInfoMap.Keys.Contains(account);
        //}

        //XAccountInfo GetAccountInfo(string account)
        //{
        //    if (string.IsNullOrEmpty(account))
        //        return new XAccountInfo();

        //    XAccountInfo target;
        //    if (connectorAccountInfoMap.TryGetValue(account, out target))
        //    {
        //        return target;
        //    }
        //    return new XAccountInfo();
        //}


        /// <summary>
        /// 获得某个交易帐户对应的底层帐户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        TLBroker GetBroker(string account)
        {
            IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);
            if (broker == null) return null;
            if (broker is TLBroker)
            {
                return broker as TLBroker;
            }
            return null;
        }

        #region 执行计费操作 根据设置生成收费记录

        void ChargeServiceFee(QSEnumChargeTime time)
        {
            //遍历所有交易帐户
            foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //帐户绑定了主帐户则对该帐户进行计费操作
                IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account.ID);
                if (broker != null)
                {
                    ChargeServiceFee(account, time);
                }
            }
        }

        /// <summary>
        /// 统一查询所有主帐户信息
        /// </summary>
        void QryBrokerAccountInfo()
        {
            //遍历所有交易帐户
            foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //帐户绑定了主帐户则对该帐户进行计费操作
                TLBroker broker = GetBroker(account.ID);
                if (broker != null && broker.IsLive)
                {
                    //查询
                    broker.QryAccountInfo();
                }
            }
        }

        /// <summary>
        /// 统一征收手续费
        /// </summary>
        void ChargeCommissionFee()
        {
            //遍历所有交易帐户
            foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //帐户绑定了主帐户则对该帐户进行计费操作
                IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account.ID);
                if (broker != null)
                {
                    ChargeCommissionFee(account);
                }
            }
        }

        /// <summary>
        /// 收取手续费
        /// </summary>
        /// <param name="account"></param>
        void ChargeCommissionFee(IAccount account)
        {
            if (HaveAccountCharged(account.ID, QSEnumFeeType.CommissionFee, TLCtxHelper.ModuleSettleCentre.NextTradingday))
            {
                return;
            }

            if (!FinGlobal.BrokerAccountInfoTracker.HaveXAccountInfo(account.ID))
            {
                logger.Warn(string.Format("Account:{0} have not got Connector XAccountInfo", account));
                return;
            }

            FinService fs = FinGlobal.FinServiceTracker[account.ID];
           

            //生成手续费计费项目
            BrokerAccountInfo info = FinGlobal.BrokerAccountInfoTracker.GetAccountInfo(account.ID);
            decimal diff = account.Commission - info.Commission;
            decimal commissionFee = diff;
            Fee f = Fee.CreateCommissionFee(account.ID, commissionFee);
            f.ChargeTime = QSEnumChargeTime.AfterTimeSpan;
            f.ChargeMethod = fs != null ? fs.ChargeMethod : QSEnumChargeMethod.AutoDepositCredit;
            f.Amount = 1;
            f.Comment = string.Format("客户手续费:{0} 主帐户手续费:{1}", Util.FormatDecimal(account.Commission), Util.FormatDecimal(info.Commission));
            //if (f.Amount > 0)
            {
                FinGlobal.FinServiceTracker.InsertFee(f);
            }
        }



        void ChargeServiceFee(IAccount account,QSEnumChargeTime time)
        {
            if (HaveAccountCharged(account.ID, QSEnumFeeType.FinServiceFee, TLCtxHelper.ModuleSettleCentre.NextTradingday))
            {
                return;
            }

            FinService fs = FinGlobal.FinServiceTracker[account.ID];
            if (fs == null)
            {
                logger.Warn(string.Format("Account:{0} have not set finservice", account));
                return;
            }
            //如果计费操作时间不符则直接返回
            if (fs.ChargeTime != time)
            {
                return;
            }
            decimal serviceFee = fs.CalServiceFee();
            Fee f = Fee.CreateServiceFee(account.ID,serviceFee);
            f.ChargeTime = fs.ChargeTime;
            f.ChargeMethod = fs.ChargeMethod;

            f.Comment = fs.GetDescription();
            if (f.Amount > 0)
            {
                logger.Info(f.ToString());
                FinGlobal.FinServiceTracker.InsertFee(f);
            }
        }

        /// <summary>
        /// 判断某个交易帐户当天是否已经收取过
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        bool HaveAccountCharged(string account, QSEnumFeeType type, int settleday)
        {
            return ORM.MFee.HaveCharged(account, type, settleday);
        }


        #endregion

        


        #region 收费操作 根据收费记录执行扣费

        void CollectFee()
        {
            foreach (var f in FinGlobal.FinServiceTracker.GetFees(TLCtxHelper.ModuleSettleCentre.NextTradingday))
            {
                CollectFee(f);
            }
        }
        void CollectFee(Fee f,QSEnumChargeMethod ?method= null)
        {
            //如果已经完成收费则返回
            if (f.Collected)
            {
                return;
            }
            string account = f.Account;
            decimal amount = f.Amount;

            string comment = string.Format("{0}-{1}-{2}", f.Settleday, f.Account, f.ID);
            //如果没有指定收费方法则按计费中的方法进行收费
            QSEnumChargeMethod m = method == null ? f.ChargeMethod : (QSEnumChargeMethod)method;

            switch (m)
            {
                    //手工收取
                case QSEnumChargeMethod.Manual:
                    break;

                    //通过劣后出金，优先入金的方式实行收费，比如当天需要收取客户利息50元，则客户权益减少50，优先权益增加50
                case QSEnumChargeMethod.AutoDepositCredit:
                    {
                        try
                        {
                            FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Placed);
                            if (f.FeeType == QSEnumFeeType.FinServiceFee)
                            {
                                //帐户客户权益出金
                                TLCtxHelper.ModuleAccountManager.CashOperation(account, amount * -1, QSEnumEquityType.OwnEquity, "", "");
                                //帐户优先权益入金
                                TLCtxHelper.ModuleAccountManager.CashOperation(account, amount, QSEnumEquityType.CreditEquity, "", string.Format("{0}-{1}", comment, f.Amount));
                            }
                            //系统的手续按照本地费率计算，多收的手续费可以通过从主帐户出金的方式或者优先资金入金的方式收取
                            if (f.FeeType == QSEnumFeeType.CommissionFee)
                            {
                                //这里客户权益不需要出金，因为手续费盘中已经通过计算多收取了 体现在了客户权益中
                                //帐户优先权益入金
                                TLCtxHelper.ModuleAccountManager.CashOperation(account, amount, QSEnumEquityType.CreditEquity, "", string.Format("{0}-{1}", comment, f.Amount));
                            }

                            FinGlobal.FinServiceTracker.FeeCollected(f);
                            FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Success);
                        }
                        catch (Exception ex)//支付异常
                        {
                            logger.Error("AutoDepositCredit for fee error:" + ex.ToString());
                            FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Fail, "计入优先异常");
                        }

                    }
                    break;

                    //通过从底层帐户出金
                case QSEnumChargeMethod.AutoWithdraw:
                    {
                        FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Placed);
                        //服务费同时从客户权益出金，然后从底层帐户出金
                        if (f.FeeType == QSEnumFeeType.FinServiceFee)
                        {
                            //帐户客户权益出金
                            TLCtxHelper.ModuleAccountManager.CashOperation(account, amount * -1, QSEnumEquityType.OwnEquity, "", "");

                            TLBroker broker = GetBroker(account);
                            if (broker == null)
                            {
                                FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Fail, "未绑定主帐户");
                                return;
                            }

                            //提交费用出金请求
                            FinGlobal.BrokerTransTracker.GotFeePlaced(broker, f);

                        }

                        //手续费 盘中客户权益经收取，从底层帐户出金
                        if (f.FeeType == QSEnumFeeType.CommissionFee)
                        {
                            TLBroker broker = GetBroker(account);
                            if (broker == null)
                            {
                                FinGlobal.FinServiceTracker.UpdateFeeStatus(f, QSEnumFeeStatus.Fail, "未绑定主帐户");
                                return;
                            }

                            //提交费用出金请求
                            FinGlobal.BrokerTransTracker.GotFeePlaced(broker, f);
                        }
                    }
                    break;
            }
        }

        #endregion

    }
}
