using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{

    public class BackendQry
    {
        public BackendQry(string key)
        {
            this.BackendRequestID = 0;
            this.SymbolFreqKey = key;
            this.RequestList = new ThreadSafeList<RequestDelayed>();
        }
        /// <summary>
        /// 从DataCore查询对应的请求编号
        /// </summary>
        public int BackendRequestID { get; set; }

        /// <summary>
        /// 从后端查询的合约频率数据键
        /// </summary>
        public string SymbolFreqKey { get;set; }

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

        ConcurrentDictionary<string, BackendQry> backendQryMap = new ConcurrentDictionary<string, BackendQry>();

        public override void BackendQryBar(IServiceHost host, IConnection conn, QryBarRequest request)
        {
            logger.Info("try to qry bar from DataCoreBackend");
            string tbkey = STSDBBase.GetTableName(request.Symbol, request.IntervalType, request.Interval);
            BackendQry target = null;

            //如果已经包含了该key对应的后端查询 则将本次查询加入到后端查询对象中的延迟查询对象列表
            if (backendQryMap.TryGetValue(tbkey, out target))
            {
                //BackendQry bq = new BackendQry(tbkey);
                //bq.RequestList.Add(new RequestDelayed(host,conn,request));
                target.RequestList.Add(new RequestDelayed(host, conn, request));
            }
            else
            {
                BackendQry bq = new BackendQry(tbkey);
                bq.RequestList.Add(new RequestDelayed(host, conn, request));

                //通过Backend进行查询
                _backend.QryBar(request);
            }

        }
        
    }
}
