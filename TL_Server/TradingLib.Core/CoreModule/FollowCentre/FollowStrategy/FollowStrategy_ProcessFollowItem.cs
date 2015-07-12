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
    /// <summary>
    /// 跟单处理线程
    /// 
    /// </summary>
    public partial class FollowStrategy
    {

        Thread followthread = null;
        bool followgo = false;
        /// <summary>
        /// 启动跟单实例线程
        /// </summary>
        public void Start()
        {
            if (followgo) return;
            followgo = true;
            followthread = new Thread(ThreadRun);
            followthread.IsBackground = true;
            followthread.Start();

        }

        /// <summary>
        /// 停止跟单实例线程
        /// </summary>
        public void Stop()
        {
            if (!followgo) return;
            followgo = false;
            followthread.Abort();

        }
        RingBuffer<TradeFollowItem> followbuffer = new RingBuffer<TradeFollowItem>();
        Queue<TradeFollowItem> followQueue = new Queue<TradeFollowItem>();

        /// <summary>
        /// 将跟单项从缓存中移到队列
        /// 移动和处理在唯一的线程中处理避免出现多个线程处理造成异常
        /// </summary>
        void MoveItemIn()
        {
            while (followbuffer.hasItems)
            {
                followQueue.Enqueue(followbuffer.Read());
            }
        }

        /// <summary>
        /// 处理跟单项
        /// </summary>
        void ProcessFollowItem()
        {
            try
            {
                //无跟单项目直接返回
                if (followQueue.Count == 0) return;

                //遍历跟单队列
                TradeFollowItem[] items = followQueue.ToArray();
                followQueue.Clear();

                foreach (TradeFollowItem item in items)
                {
                    //action引擎生成对应的action
                    FollowAction action = GenAction(item);

                    if (action != null)
                    {

                        //记录该action

                        //执行该action
                        DoAction(action);
                    }
                }

            }
            catch (Exception ex)
            { 
            
            }
        }
        void ThreadRun()
        {
            while (followgo)
            {
                MoveItemIn();
            }
        }


    }
}
