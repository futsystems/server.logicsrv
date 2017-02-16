using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    
    public partial class FollowStrategy
    {

        
        /// <summary>
        /// 响应信号账户的持仓事件
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        void OnSignalPositionEvent(ISignal signal, Trade trade, IPositionEvent pe)
        {
            try
            {
                //策略关闭状态不接受任何信号
                if (this.WorkState == QSEnumFollowWorkState.Shutdown)
                    return;

                logger.Info(string.Format("Signal:{0} PositionEvent:{1}", signal.GetInfo(), pe.GetInfo()));
                //1.过滤器过滤

                //2.生成跟单项目
                TradeFollowItem followitem = null;
                //如果是开仓事件直接生成跟单项
                if (pe.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    //策略暂停状态 不接受任何开仓信号
                    if (this.WorkState == QSEnumFollowWorkState.Suspend)
                        return;
                    followitem = new TradeFollowItem(this, signal,trade, pe);
                }
                else//平仓事件需要查找对应的开仓跟单项目 做持仓判定以及数据绑定
                {
                    TradeFollowItem entryitem = GetEntryFollowItemViaLocalKey(pe.PositionExit.OpenTradeID);
                    if (entryitem == null)
                    {
                        logger.Info("ExitPoitionEvent has no EntryFollowItem,ignored");
                        return;
                    }
                    //开仓与平仓对象绑定时 平仓跟单项获得初始FollowKey
                    if (entryitem.NeedExitFollow)
                    {
                        followitem = new TradeFollowItem(this, signal, trade, pe);
                        entryitem.Link(followitem);
                    }
                }

                //3.将该新建跟单项写入待处理缓存
                if (followitem != null)
                {
                    NewFollowItem(followitem);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 将跟单项放入缓存并执行数据储存与对外通知
        /// </summary>
        /// <param name="item"></param>
        public void NewFollowItem(TradeFollowItem item)
        {
            CacheFollowItem(item);
            //数据库记录新生成的跟单项目
            FollowItemData data = item.ToFollowItemData();
            data.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            FollowTracker.FollowItemLogger.NewFollowItem(data);
            //对外通知跟单项
            FollowTracker.NotifyTradeFollowItem(item);

        }

        /// <summary>
        /// 将跟单项目放入缓存
        /// </summary>
        /// <param name="item"></param>
        void CacheFollowItem(TradeFollowItem item)
        {
            followKeyItemMap.TryAdd(item.FollowKey, item);
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                localKeyItemMap.TryAdd(item.GetLocalKey(), item);//记录开仓跟单项本地键映射关系 用于平仓信号时候查找对应的开仓跟单项
            }
            //放入处理队列
            followbuffer.Write(item);
        }

        void OnSignalOrderEvent(Order order)
        {
            //throw new NotImplementedException();
        }

        void OnSignalFillEvent(Trade t)
        {
            
            //throw new NotImplementedException();
        }
       
    }
}
