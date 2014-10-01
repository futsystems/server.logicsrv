using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ZeroMQ;
using TradingLib.API;
using System.Threading;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class ZeromqThroughPut
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        /// <summary>
        /// 为每个地址建立一个throughput,用于记录该地址的信息通过频率
        /// </summary>
        ConcurrentDictionary<string, ThroughputTracker<ZMessage>> addressThroughPut = new ConcurrentDictionary<string, ThroughputTracker<ZMessage>>();


        bool _collectgo = false;
        Thread collectThread = null;

        void Start()
        {
            if (_collectgo) return;
            _collectgo = true;
            collectThread = new Thread(collectProc);
            collectThread.IsBackground = true;
            collectThread.Start();
        }

        void Stop()
        {
            if (!_collectgo) return;
            _collectgo = false;
            collectThread.Abort();
            collectThread = null;
        }
        void collectProc()
        {
            while (_collectgo)
            {
                ThroughputTracker<ZMessage> dead;
                List<string> tobedelete = new List<string>();
                foreach (string key in addressThroughPut.Keys)
                {
                    if (addressThroughPut[key].IsDead)
                    {
                        tobedelete.Add(key);
                    }
                }
                //统一从映射表中删除对应的客户端流控
                for (int i = 0; i < tobedelete.Count; i++)
                {
                    addressThroughPut.TryRemove(tobedelete[i], out dead);
                }
                //等待多少时间后,再次运行一次超时客户端清除
                Thread.Sleep(Const.CLEARDEADSESSIONPERIOD * 1000);
            }
        }
        /// <summary>
        /// 将zmessage 压人流控组件,如果正常则返回True,如果超过流量则返回False
        /// </summary>
        /// <param name="zmsg"></param>
        /// <returns></returns>
        public bool NewZmessage(string clientaddress, ZMessage zmsg)
        {
            ThroughputTracker<ZMessage> througthput;
            if (addressThroughPut.TryGetValue(clientaddress, out througthput))
            {

            }
            else
            {
                addressThroughPut.TryAdd(clientaddress, new ThroughputTracker<ZMessage>());
                througthput = addressThroughPut[clientaddress];
                througthput.SendDebugEvent += new API.DebugDelegate(debug);
            }
            lock (througthput)
            {
                //得到每个地址的流控实例后
                througthput.newItem(zmsg);
                return (!througthput.IsOverLoad);//如果没有超过流限,则返回True

            }
        }
    }
}
