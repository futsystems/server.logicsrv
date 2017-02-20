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
            FollowTracker.EntryFollowItemOpen += new Action<FollowItem>(FollowTracker_EntryFollowItemOpen);
            FollowTracker.EntryFollowItemClose += new Action<FollowItem>(FollowTracker_EntryFollowItemClose);
            TLCtxHelper.EventAccount.AccountAddEvent += new Action<IAccount>(EventAccount_AccountAddEvent);
            TLCtxHelper.EventAccount.AccountDelEvent += new Action<IAccount>(EventAccount_AccountDelEvent);
            TLCtxHelper.EventSystem.SettleResetEvent +=new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
            TLCtxHelper.EventSystem.AfterSettleEvent += new EventHandler<SystemEventArgs>(EventSystem_AfterSettleEvent);

            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(EventIndicator_GotTickEvent);
        }

        void EventIndicator_GotTickEvent(Tick t)
        {
            try
            {
                if (t == null) return;
                if (t.UpdateType != "X") return;

                //遍历所有处于持仓状态的跟单项
                foreach (var item in followItemHoldMap.Values.Where(entry => entry.Protect != null && entry.Symbol == t.Symbol && entry.PositionHoldSize > 0 && entry.FlatTrigger==false))
                {
                    //执行止损检查
                    if (item.Protect.StopEnable)
                    {
                        decimal cost = item.FollowPrice;
                        decimal target = item.Protect.CalcStopTarget(item.FollowSide, cost);
                        //logger.Info(string.Format("followkey:{0} cost:{1} stop:{2}", item.FollowKey, cost, target));
                        bool hit = item.FollowSide ? t.Trade <= target : t.Trade >= target;
                        if (hit && item.FlatTrigger == false)
                        {
                            FollowItem exit = FollowItem.CreateFlatFollowItem(item, QSEnumFollowItemTriggerType.StrategyExitTrigger);
                            item.Link(exit);
                            item.FlatTrigger = true;
                            item.Strategy.NewFollowItem(exit);
                        }
                    }
                    //执行止盈检查
                    if (item.Protect.Profit1Enable)
                    {
                        decimal cost = item.FollowPrice;
                        decimal target = item.Protect.CalcProfit1Target(item.FollowSide, cost);
                        //logger.Info(string.Format("followkey:{0} cost:{1} profit:{2}", item.FollowKey, cost, target));
                        bool hit = item.FollowSide ? t.Trade >= target : t.Trade <= target;
                        if (hit && item.FlatTrigger == false)
                        {
                            FollowItem exit = FollowItem.CreateFlatFollowItem(item, QSEnumFollowItemTriggerType.StrategyExitTrigger);
                            item.Link(exit);
                            item.FlatTrigger = true;
                            item.Strategy.NewFollowItem(exit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Process Tick Error:" + ex.ToString());
            }

        }

        ConcurrentDictionary<string, FollowItem> followItemHoldMap = new ConcurrentDictionary<string, FollowItem>();
        void FollowTracker_EntryFollowItemClose(FollowItem obj)
        {
            FollowItem target = null;
            followItemHoldMap.TryRemove(obj.FollowKey, out target);
            logger.Info(string.Format("FollowKey:{0} close", obj.FollowKey));
        }

        void FollowTracker_EntryFollowItemOpen(FollowItem obj)
        {
            followItemHoldMap.TryAdd(obj.FollowKey, obj);
            logger.Info(string.Format("FollowKey:{0} open", obj.FollowKey));
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
