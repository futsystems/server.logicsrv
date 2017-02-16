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
            FollowTracker.NotifyTradeFollowItemEvent += new Action<TradeFollowItem>(NotifyFollowItem);

            //TLCtxHelper.EventSystem.SettleDataStoreEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleDataStoreEvent);
            //TLCtxHelper.EventSystem.SettleEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleEvent);
            //TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
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

        /// <summary>
        /// 全局结算事件响应
        /// 用于执行结算 记录当日跟单结果信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventSystem_SettleEvent(object sender, SystemEventArgs e)
        {
            //
        }


        /// <summary>
        /// 全局数据转储事件响应
        /// 用于储存日内跟单记录并清空日内记录表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventSystem_SettleDataStoreEvent(object sender, SystemEventArgs e)
        {
            //执行数据转储操作
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

        ConcurrentDictionary<int, FollowStrategy> strategyMap = new ConcurrentDictionary<int, FollowStrategy>();
        bool _followstart = false;
        public void Start()
        {

            FollowTracker.Init();

            logger.Info("从配置文件加载跟单策略实例");
            //初始化跟单策略
            foreach (var cfg in FollowTracker.StrategyCfgTracker.StrategyConfigs)
            {
                InitStrategy(cfg);
            }
            
            //恢复跟单项目数据
            RestoreFollowItemData();

            //启动跟单策略
            foreach (var strategy in strategyMap.Values)
            {
                strategy.Start();
            }

            FollowTracker.Inited = true;
            _followstart = true;

        }


        

        

        public void Stop()
        { 
        
        }
    }
}
