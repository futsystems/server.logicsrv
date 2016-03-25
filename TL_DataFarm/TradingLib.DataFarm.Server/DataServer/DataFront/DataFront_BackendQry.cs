using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.Common.DataFarm
{

    public class QryBarBackendQry:BackendQry
    {
        /// <summary>
        /// 从后端查询的合约频率数据键
        /// </summary>
        public string SymbolFreqKey { get; set; }
        public string Symbol { get; set; }
        public BarInterval IntervalType { get; set; }
        public int Interval { get; set; }
        public QryBarBackendQry()
        {
            this.SymbolFreqKey = string.Empty;
            this.Symbol = string.Empty;
            this.IntervalType = BarInterval.CustomTime;
            this.Interval = 0;
        }
        
    }
    public class BackendQry
    {

        public BackendQry()
        {
            this.RequestList = new ThreadSafeList<RequestDelayed>();
        }
        /// <summary>
        /// 从DataCore查询对应的请求编号
        /// </summary>
        public int BackendRequestID { get; set; }

        /// <summary>
        /// 查询列表
        /// </summary>
        public ThreadSafeList<RequestDelayed> RequestList { get; set; }
    }

    public class RequestDelayed
    {
        public RequestDelayed(IServiceHost host, IConnection conn, RequestPacket request)
        {
            this.ServiceHost = host;
            this.Connection = conn;
            this.Request = request;
        }
        public IServiceHost ServiceHost { get; set; }

        public IConnection Connection { get; set; }

        public MessageTypes RequestType { get { return this.Request.Type; } }
        /// <summary>
        /// 
        /// </summary>
        public RequestPacket Request { get; set; }
    }

    public partial class DataFront
    {
        /// <summary>
        /// 记录后端请求编号与请求映射
        /// </summary>
        ConcurrentDictionary<int, BackendQry> requestIdQryMap = new ConcurrentDictionary<int, BackendQry>();

        /// <summary>
        /// 记录SybolFreq与QryBarBackendQry映射
        /// </summary>
        ConcurrentDictionary<string, QryBarBackendQry> symfreqQryMap = new ConcurrentDictionary<string, QryBarBackendQry>();
        
        public override void BackendQryBar(IServiceHost host, IConnection conn, QryBarRequest request)
        {
            logger.Info("try to qry bar from DataCoreBackend");
            Symbol symbol = MDBasicTracker.SymbolTracker[request.Symbol];
            if (symbol == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                return;
            }
            string tbkey = STSDBBase.GetTableName(symbol, request.IntervalType, request.Interval);
            QryBarBackendQry target = null;

            //如果已经包含了该key对应的后端查询 则将本次查询加入到后端查询对象中的延迟查询对象列表
            if (symfreqQryMap.TryGetValue(tbkey, out target))
            {
                target.RequestList.Add(new RequestDelayed(host, conn, request));
            }
            else
            {
                target = new QryBarBackendQry();
                target.IntervalType = request.IntervalType;
                target.Interval = request.Interval;
                target.Symbol = request.Symbol;
                target.SymbolFreqKey = tbkey;

                target.RequestList.Add(new RequestDelayed(host, conn, request));

                symfreqQryMap.TryAdd(tbkey, target);
                
                //通过Backend进行查询
                target.BackendRequestID = _backend.QryBar(request);
                //添加到BackendRequestID-BackendQry map
                requestIdQryMap.TryAdd(target.BackendRequestID, target);

            }

        }
        int i = 0;
        int size = 0;
        void Backend_BarResponseEvent(RspQryBarResponse obj)
        {
            IHistDataStore store = this.GetHistDataSotre();
            //obj.Bar.Symbol = "HGZ5"; ;
            //obj.Bar.Interval = 30;
            //obj.Bar.IntervalType = BarInterval.CustomTime;
            if (size == 0)
            {
                size = obj.Data.Length;
            }
            Symbol symbol = MDBasicTracker.SymbolTracker["IF1603"];
            if (symbol == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", "IF1603"));
                return;
            }

            if (store != null)
            {
                //更新内存Bar数据缓存
                //i++;
                store.UpdateBar(symbol,obj.Bar as BarImpl);
                //logger.Info("cnt:" + i.ToString() + " size:" + size.ToString());
                //如果是最后一个Bar回报
                if (obj.IsLast)
                {
                    logger.Info(string.Format("Backend Request:{0} finished", obj.RequestID));
                    BackendQry target = null;
                    //通过RequestID查找到对应的BackendQry
                    if (requestIdQryMap.TryGetValue(obj.RequestID, out target))
                    { 
                        //Bar数据后端查询
                        if(target is QryBarBackendQry)
                        {
                            QryBarBackendQry qbqry = target as QryBarBackendQry;
                            //设置缓存标志
                            store.SetCached(qbqry.Symbol, qbqry.IntervalType, qbqry.Interval, true);
                            //遍历所有延迟请求进行处理
                            foreach (var request in target.RequestList)
                            {
                                this.ProcessRequest(request.ServiceHost, request.Connection, request.Request);
                            }
                        }
                    }
                }
            }
        }
        
    }
}
