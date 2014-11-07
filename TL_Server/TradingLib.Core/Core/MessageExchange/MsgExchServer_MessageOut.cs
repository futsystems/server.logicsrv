using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    internal class PriorityBuffer<T>
    {
        const int buffersize = 1000;
        public int Priority = 10;
        public RingBuffer<T> Buffer = new RingBuffer<T>(buffersize);

        /// <summary>
        /// 放入一个对象
        /// </summary>
        /// <param name="obj"></param>
        public void Write(T obj)
        {
            this.Buffer.Write(obj);
        }

        /// <summary>
        /// 读取一个对象
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            return this.Buffer.Read();
        }

        /// <summary>
        /// 是否有内容
        /// </summary>
        public bool HasItems
        {
            get
            {
                return this.Buffer.hasItems;
            }
        }
    }

    internal class PriorityBufferSet
    {
        Dictionary<Type, PriorityBuffer<RspResponsePacket>> rspresponsemap = new Dictionary<Type, PriorityBuffer<RspResponsePacket>>();
        SortedList<int, PriorityBuffer<RspResponsePacket>> prioritymap = new SortedList<int, PriorityBuffer<RspResponsePacket>>();

        public void AddRspResponseType(int priority, Type t)
        {
            if (rspresponsemap.Keys.Contains(t))
                throw new Exception("Type:" + t.ToString() + " already inserted");
            if (prioritymap.Keys.Contains(priority))
                throw new Exception("Priority:" + priority.ToString() + " already inserted");
            PriorityBuffer<RspResponsePacket> buffer = new PriorityBuffer<RspResponsePacket>();
            rspresponsemap.Add(t,buffer);
            prioritymap.Add(priority, buffer);

        }
        /// <summary>
        /// 如果返回False则表明该packet不由优先级队列维护 写入默认rsp缓存
        /// 按照具体的类型写入不同的队列
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool Write(RspResponsePacket packet)
        {
            //如果该RspResponse对象被优先级缓存列表维护 则写入对应的缓存 缓存在初始化时给定了具体的优先级，用优先级别对对象分组
            Type t = packet.GetType();
            if (rspresponsemap.Keys.Contains(t))
            {
                rspresponsemap[t].Write(packet);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否有对象需要发送
        /// </summary>
        public bool HasItems
        {
            get
            {
                foreach (PriorityBuffer<RspResponsePacket> b in rspresponsemap.Values)
                {
                    if (b.HasItems)
                        return true;
                }
                return false;
                
            }
        }

        
        /// <summary>
        /// 按照优先级 读取最优先发送的消息
        /// </summary>
        /// <returns></returns>
        public RspResponsePacket Read()
        {
            //按照优先队列 读取对一个队列中的对象
            foreach (PriorityBuffer<RspResponsePacket> b in prioritymap.Values)
            {
                if (b.HasItems)
                    return b.Read();
            }
            return null;
        }
        
    }


    public partial class MsgExchServer
    {


        #region 消息发送总线 向管理端发送对应的消息

        /// <summary>
        /// 通过缓存对外发送逻辑数据包
        /// </summary>
        /// <param name="packet"></param>
        public void Send(IPacket packet)
        {
            _packetcache.Write(packet);
        }


        /// <summary>
        /// 消息对外发送线程,由于ZeroMQ并不是线程安全的，因此我们不能运行多个线程去操作一个socket,当我们对外发送的时候统一调用了
        /// TLSend _trans.Send(data, clientID)
        /// 1.TradingServer形成的交易信息,我们需要通过MgrServer转发到对应的客户端TradingSrv->MgrServer->TLSend
        /// 2.客户端注册操作 以及回补相关账户交易信息时，也会形成MgrServer对外的信息发送Client->MgrServer->TLSend
        /// 这样Mgr其实有多个线程通过MgrServer对外发送信息,因此我们需要在这里建立信息缓存，将多个线程需要转发的消息一并放入
        /// 对应的缓存中，然后由单独的线程不间断的将这些缓存的信息统一发送出去
        /// </summary>
        void StartMessageRouter()
        {
            if (_readgo) return;
            _readgo = true;
            messageoutthread = new Thread(messageout);
            messageoutthread.IsBackground = true;
            messageoutthread.Name = "TradingServer MessageOut Thread";
            messageoutthread.Start();
            ThreadTracker.Register(messageoutthread);
        }

        void StopMessageRouter()
        {
            if (!_readgo) return;
            _readgo = false;
            ThreadTracker.Unregister(messageoutthread);
            int mainwait = 0;
            while (messageoutthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            messageoutthread.Abort();
            messageoutthread = null;

        }
        const int buffize = 20000;

        
        RingBuffer<Order> _ocache = new RingBuffer<Order>(buffize);//委托缓存
        //RingBuffer<long> _ccache = new RingBuffer<long>(buffize);//取消缓存
        RingBuffer<OrderErrorPack> _errorordercache = new RingBuffer<OrderErrorPack>(buffize);//委托错误缓存
        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(buffize);//成交缓存
        RingBuffer<PositionEx> _posupdatecache = new RingBuffer<PositionEx>(buffize);
        RingBuffer<IPacket> _packetcache = new RingBuffer<IPacket>(buffize);//数据包缓存队列

        //关于交易信息转发,交易信息转发时,我们需要区分是实时发生的交易信息转发还是请求回补的信息转发。
        //实时交易信息通过客户端权限检查自动将交易信息发送到所有有权
        bool _readgo = false;
        Thread messageoutthread;

        //当有其他消息的时候我们就转发其他消息
        bool notokforresume()
        {
            return _ocache.hasItems  || _fcache.hasItems || _posupdatecache.hasItems;
        }

        

        public string[] FilterClient(string filter)
        {
            //获得所有客户端地址
            //if (filter.ToUpper().Equals("ALL"))
            //{ 
            //    IEnumerable<string> list =
            //        from acc in tl.Clients
            //        //where f.Match(acc)
            //        select acc.Address;
            //    return list.ToArray();
            //}
            return new string[] { };
        }

        PriorityBufferSet prioritybuffer = new PriorityBufferSet();
        void InitPriorityBuffer()
        {
            prioritybuffer.AddRspResponseType(1, typeof(RspQryInvestorResponse));
            prioritybuffer.AddRspResponseType(2, typeof(RspQrySymbolResponse));

            //prioritybuffer.AddRspResponseType(3, typeof(RspQryTradeResponse));
            //prioritybuffer.AddRspResponseType(4, typeof(RspQryOrderResponse));
            //prioritybuffer.AddRspResponseType(5, typeof(RspQryPositionDetailResponse));
            //prioritybuffer.AddRspResponseType(6, typeof(RspQryPositionResponse));
            //prioritybuffer.AddRspResponseType(7, typeof(RspQryAccountInfoResponse));

        }
        RingBuffer<RspQryInvestorResponse> investorbuf = new RingBuffer<RspQryInvestorResponse>(buffize);//投资者查询缓存
        RingBuffer<RspQrySymbolResponse> symbolbuf = new RingBuffer<RspQrySymbolResponse>(buffize);//合约查询缓存

        RingBuffer<RspQryTradeResponse> rsptradebuf = new RingBuffer<RspQryTradeResponse>(buffize);//成交回报缓存
        RingBuffer<RspQryOrderResponse> rsporderbuf = new RingBuffer<RspQryOrderResponse>(buffize);//委托回报缓存
        RingBuffer<RspQryPositionResponse> rspposbuf = new RingBuffer<RspQryPositionResponse>(buffize);//持仓缓存
        RingBuffer<RspQryPositionDetailResponse> rspposdetailbuf = new RingBuffer<RspQryPositionDetailResponse>(buffize);//持仓明细缓存
        RingBuffer<RspQryAccountInfoResponse> rspaccountinfobuf = new RingBuffer<RspQryAccountInfoResponse>(buffize);//账户明细缓存

        /// <summary>
        /// 所有需要转发到客户端的消息均通过缓存进行，这样避免了多个线程同时操作一个ZeroMQ socket
        /// 所有发送消息的过程产生的异常将在这里进行捕捉
        /// </summary>
        void messageout()
        {
            while (_readgo)
            {
                try
                {
                    //发送委托
                    while (_ocache.hasItems)
                    {
                        tl.newOrder(_ocache.Read());
                    }
                    //转发委托错误
                    while (!_ocache.hasItems && _errorordercache.hasItems)
                    {
                        OrderErrorPack e = _errorordercache.Read();
                        ErrorOrderNotify notify = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(e.Order.Account);
                        notify.Order = e.Order;
                        notify.RspInfo = e.RspInfo;

                        tl.newOrderError(notify);
                    }
                    //发送成交
                    while (!_ocache.hasItems && _fcache.hasItems)
                    {
                        tl.newFill(_fcache.Read());

                    }
                    
                    //发送取消
                    //while (!_ocache.hasItems && _ccache.hasItems)
                    //{
                    //    //tl.newCancel(_clearcentre.SentOrder(_ccache.Read()));
                    //}
                    //发送持仓更新信息
                    while (_posupdatecache.hasItems && !_ocache.hasItems && !_fcache.hasItems)
                    {
                        tl.newPositionUpdate(_posupdatecache.Read());

                    }


                    //while (investorbuf.hasItems)
                    //{
                    //    tl.TLSend(investorbuf.Read());
                    //}
                    //while (symbolbuf.hasItems && (!investorbuf.hasItems))
                    //{
                    //    tl.TLSend(symbolbuf.Read());
                    //}
                    while (prioritybuffer.HasItems)
                    {
                        tl.TLSend(prioritybuffer.Read());
                    }

                    //当优先级的消息缓存没有消息的时候才发送其他消息
                    while (_packetcache.hasItems && (!prioritybuffer.HasItems))
                    {
                        tl.TLSend(_packetcache.Read());
                    }


                    //回报账户resume(客户端回复当日交易信息)
                    //while (_resuemcache.hasItems && !notokforresume())
                    //{
                    //    //处理请求恢复交易信息 只记录account,统一在单一的消息处理线程里面进行回报，这样可以防止多个worker线程 请求恢复造成同时调用clearcenter.getorders造成的冲突
                    //    ResumeSource resume = _resuemcache.Read();
                    //    //转发昨日持仓信息
                    //    foreach (Position pos in _clearcentre.getPositionHold(resume.Account))
                    //    {
                    //        //tl.RestorePosition(pos, resume.Source);
                    //    }
                    //    //转发当日委托
                    //    foreach (Order o in _clearcentre.getOrders(resume.Account))
                    //    {
                    //        //tl.RestoreOrder(o, resume.Source);
                    //    }
                    //    //转发当日成交
                    //    foreach (Trade f in _clearcentre.getTrades(resume.Account))
                    //    {
                    //        //tl.RestoreFill(f, resume.Source);
                    //    }
                    //    //转发当日取消
                    //    foreach (long oid in _clearcentre.getCancels(resume.Account))
                    //    {
                    //        //tl.RestoreCancel(_clearcentre.SentOrder(oid), resume.Source);
                    //    }
                    //    //转发当日持仓状态,过程数据需要指定接收客户端,状态数据可以多次接收
                    //    foreach (Position pos in _clearcentre.getPositions(resume.Account))
                    //    {
                    //        debug("will send postion update to client:" + pos.ToString());
                    //        //tl.newPosition(pos);
                    //    }

                    //    //回报完交易记录后 集中回报当前市场快照,用于驱动postion获得当前未平仓盈亏
                    //    Tick[] ticks = _datafeedRouter.GetTickSnapshot();
                    //    foreach (Tick k in ticks)
                    //    {
                    //        //这里可以考虑改进成只推送客户端订阅的合约快照
                    //        //tl.TLSend(TickImpl.Serialize(k), MessageTypes.TICKNOTIFY, resume.Source);
                    //    }

                    //    //发送回复数据完成信息
                    //    //tl.TLSend("Finish", MessageTypes.RESUMEFINISH,resume.Source);

                    //}
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    debug("消息发送线程出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }

        }
        #endregion


    }
}
