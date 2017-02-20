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

    public partial class FollowCentre
    {
        Thread followthread = null;
        bool followgo = false;
        /// <summary>
        /// 启动跟单实例线程
        /// </summary>
        public void StartFollowItemWorker()
        {
            logger.Info("Start FollowItem Process Worker");
            if (followgo) return;
            followgo = true;
            followthread = new Thread(ThreadRun);
            followthread.IsBackground = false;
            followthread.Start();

        }

        /// <summary>
        /// 停止跟单实例线程
        /// </summary>
        public void StopFollowItemWorker()
        {
            if (!followgo) return;
            followgo = false;
            followthread.Abort();

        }

        void OnFollowItemCached(FollowItem obj)
        {
            followbuffer.Write(obj);
            NewFollowItem();
        }



        const int BufferSize = 1000;
        RingBuffer<FollowItem> followbuffer = new RingBuffer<FollowItem>(BufferSize);
        Queue<FollowItem> followQueue = new Queue<FollowItem>(BufferSize);
        const int SLEEPDEFAULTMS = 10000;
        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);


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
                    item.Strategy.ProcessFollowItem(item);

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

        void NewFollowItem()
        {
            if ((followthread != null) && (followthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
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

                // clear current flag signal
                _sendwaiting.Reset();
                // wait for a new signal to continue reading
                _sendwaiting.WaitOne(SLEEPDEFAULTMS);
                //logger.Info("wait");
            }
        }
    }
}
