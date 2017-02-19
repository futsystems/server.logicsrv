using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre : BaseSrvObject,IModuleFollowCentre
    {

        const string CoreName = "FollowCentre";

        public string CoreId { get { return CoreName; } }


        public FollowCentre()
            : base(FollowCentre.CoreName)
        {
            FollowTracker.NotifyFollowItemEvent += new Action<FollowItem>(NotifyFollowItem);
            TLCtxHelper.EventAccount.AccountAddEvent += new Action<IAccount>(EventAccount_AccountAddEvent);
            TLCtxHelper.EventAccount.AccountDelEvent += new Action<IAccount>(EventAccount_AccountDelEvent);
            TLCtxHelper.EventSystem.SettleResetEvent +=new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
            TLCtxHelper.EventSystem.AfterSettleEvent += new EventHandler<SystemEventArgs>(EventSystem_AfterSettleEvent);
        }

        void EventAccount_AccountDelEvent(IAccount obj)
        {
            try
            {
                ISignal signal = FollowTracker.SignalTracker[obj.ID];

                //从跟单策略中删除信号 删除信号过程中会自动判定信号是否为null 同时判定信号是否在跟单信号列表中
                foreach (var strategy in FollowTracker.FollowStrategyTracker.FollowStrategies)
                {
                    strategy.RemoveSignal(signal);
                }
                //从信号维护器中删除信号
                FollowTracker.SignalTracker.DelAccount(obj);
            }
            catch (Exception ex)
            {
                logger.Error("Process Del Account Error:" + ex.ToString());
            }
        }

        void EventAccount_AccountAddEvent(IAccount obj)
        {
            try
            {
                FollowTracker.SignalTracker.AddAccount(obj);
            }
            catch (Exception ex)
            {
                logger.Error("Process Add Account Error:" + ex.ToString());
            }
        }

        void EventSystem_AfterSettleEvent(object sender, SystemEventArgs e)
        {
            //执行数据转储操作 数据转储需要通过交易来进行
            try
            {
                //转储日内跟单记录
                ORM.MFollowItem.DumpInterdayFollowInfos();

                //清空日内跟单记录表
                ORM.MFollowItem.ClearInterdayFollowInfos();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 全局结算后重置事件
        /// 用于清空内存重置跟单系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            
        }

        public void Start()
        {

            FollowTracker.Init();

            logger.Info("从配置文件加载跟单策略实例");
            //初始化跟单策略
            foreach (var cfg in FollowTracker.StrategyCfgTracker.StrategyConfigs)
            {
                FollowTracker.FollowStrategyTracker.InitStrategy(cfg);
            }
            
            //恢复跟单项目数据
            RestoreFollowItemData();

            //启动跟单策略
            foreach (var strategy in FollowTracker.FollowStrategyTracker.FollowStrategies)
            {
                strategy.Start();
            }

            FollowTracker.Inited = true;

        }


        

        

        public void Stop()
        { 
        
        }
    }
}
