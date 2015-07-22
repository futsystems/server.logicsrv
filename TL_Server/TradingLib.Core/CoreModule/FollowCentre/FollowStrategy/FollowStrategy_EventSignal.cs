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
                logger.Info(string.Format("Signal:{0} PositionEvent:{1}", signal.GetInfo(), pe.GetInfo()));
                //1.过滤器过滤

                //2.生成跟单项目
                TradeFollowItem followitem = null;
                FollowItemTracker tk = followitemtracker[signal.ID];
                if (tk == null)
                {
                    logger.Warn(string.Format("Signal:{0}'s followitemtracker is not inited."));
                    return;
                }
                //如果是开仓事件 则直接生成
                if (pe.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    followitem = new TradeFollowItem(this, signal,trade, pe);
                }
                else//平仓事件需要查找对应的开仓跟单项目
                {
                    TradeFollowItem entryitem = tk[QSEnumPositionEventType.EntryPosition, pe.PositionExit.OpenTradeID];
                    if (entryitem == null)
                    {
                        logger.Info("ExitPoitionEvent has no EntryFollowItem,ignored");
                        return;
                    }

                    //如果开仓跟单项目需要平仓跟单项目 则直接生成跟单项目
                    if (entryitem.NeedExitFollow)
                    {
                        followitem = new TradeFollowItem(this, signal, trade, pe);

                        //将平仓跟单项目绑定到开仓跟单项目
                        entryitem.NewExitFollowItem(followitem);
                        //将开仓跟单项目绑定到平仓跟单项目
                        followitem.NewEntryFollowItem(entryitem);
                    }
                }

                //3.将该新建跟单项写入待处理缓存
                if (followitem != null)
                {
                    //信号跟单项目维护器记录该跟单项目
                    tk.GotTradeFollowItem(followitem);
                    //放入缓存
                    followbuffer.Write(followitem);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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
