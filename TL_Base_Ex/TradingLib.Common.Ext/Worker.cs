using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Logging;


namespace TradingLib.Common
{
    public class Worker
    {
        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
        const int SLEEPDEFAULTMS = 200;

        ConcurrentDictionary<string, WorkerItem> workerItemMap = new ConcurrentDictionary<string, WorkerItem>();

        bool _workGo = false;
        Thread workerThread;

        ILog logger = LogManager.GetLogger("Worker");


        public void StartWorker()
        {
            logger.Info("Start worker thread");
            if (_workGo) return;
            _workGo = true;
            workerThread = new Thread(WorkProcess);
            workerThread.Name = "Worker Thread";
            workerThread.Start();
            ThreadTracker.Register(workerThread);
        }




        void NewWorkerItem()
        {
            if ((workerThread != null) && (workerThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
                logger.Info("set event");
            }
        }


        void WorkProcess()
        {
            while (_workGo)
            {
                foreach (var item in workerItemMap.Values)
                {
                    item.DoWorker();
                }

                // clear current flag signal
                _sendwaiting.Reset();
                
                // wait for a new signal to continue reading
                _sendwaiting.WaitOne(SLEEPDEFAULTMS);
                //logger.Info("******");
            }
        }


        


        public void Register(WorkerItem item)
        {
            workerItemMap.TryAdd(item.ID, item);
            item.NewWorkEvent += new Action<WorkerItem>(item_NewWorkEvent);
        }

        void item_NewWorkEvent(WorkerItem obj)
        {
            NewWorkerItem();
        }


    }


    public class WorkerItem
    {

        public event Action<WorkerItem> NewWorkEvent = delegate { };
        Action work = null;
        string id = string.Empty;
        public WorkerItem(Action item)
        {
            id = System.Guid.NewGuid().ToString();
            work = item;
        }

        public string ID { get { return id; } }
        
        public void DoWorker()
        {

            if (work != null)
            {
                work();
            }
        }

        /// <summary>
        /// 通知线程 有新的任务需要执行
        /// </summary>
        public void NewWork()
        {
            NewWorkEvent(this);
        }
    }
}
