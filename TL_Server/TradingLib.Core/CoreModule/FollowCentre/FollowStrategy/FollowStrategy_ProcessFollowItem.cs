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
            logger.Info(string.Format("FollowStrategy:{0} Token:{1} is starting",this.ID,this.Token));
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

        const int BufferSize = 1000;
        RingBuffer<FollowItem> followbuffer = new RingBuffer<FollowItem>(BufferSize);
        Queue<FollowItem> followQueue = new Queue<FollowItem>(BufferSize);


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
                FollowItem[] items = followQueue.ToArray();
                followQueue.Clear();

                List<FollowItem> unclosed = new List<FollowItem>();

                foreach (FollowItem item in items)
                {
                    //action引擎生成对应的action
                    FollowAction action = GenAction(item);
                    if (action != null)
                    {
                        //记录该action

                        //执行该action
                        //输出actoin
                        logger.Debug(action.ToString());
                        DoAction(action);
                    }

                    //将处于未关闭状态的跟单项目放入观察列表
                    if (item.Stage != QSEnumFollowStage.ItemClosed)
                    {
                        unclosed.Add(item);
                    }
                }

                //将需要观察的跟单项目放入队列
                foreach (var item in unclosed)
                {
                    followQueue.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Process FollowItem Error:" + ex.ToString());
            }
        }

        void ThreadRun()
        {
            while (followgo)
            {
                //1.将缓存中的跟单项目移动到待处理队列
                MoveItemIn();
                //2.处理队列中的跟单项目
                ProcessFollowItem();

                Thread.Sleep(100);
            }
        }


    }
}
