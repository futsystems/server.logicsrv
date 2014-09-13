using System;
using System.Collections.Generic;

using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace Broker.CTP
{
    /// <summary>
    /// route your orders through this component to paper trade with a live data feed
    /// 模拟交易
    /// </summary>
    public class CTPOrderEngerHelper
    {
        IdTracker _idt;
        public CTPOrderEngerHelper() : this(new IdTracker(), false) { }

        public CTPOrderEngerHelper(IdTracker idt, bool verb)
        {
            VerboseDebugging = verb;
            _idt = idt;
        }
        bool _usebidask = true;
        /// <summary>
        /// whether to use bid ask for fills, otherwise last is used.
        /// </summary>
        public bool UseBidAskFills { get { return _usebidask; } set { _usebidask = value; } }

        /// <summary>
        /// 当OrderEngine获得委托向客户端进行回报
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        //
        void GotOrder(Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }
        /// <summary>
        ///  当OrderEngine获得取消委托向客户端进行回报
        /// </summary>
        public event LongDelegate GotCancelEvent;
        void GotCancel(long oid)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
        /// <summary>
        /// OrderEngine对外发送取消
        /// </summary>
        public event LongDelegate SendCancelEvent;
        void api_SendCancel(long oid)
        {
            if (SendCancelEvent != null)
                SendCancelEvent(oid);
        }
        /// <summary>
        /// OrderEngine对外发送委托
        /// </summary>
        public event OrderDelegate SendOrderEvent;
        void api_SendOrder(Order o)
        {
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        public event OrderMessageDel GotOrderMessageEvent;
        void GotOrderMessage(Order o, string msg)
        {
            if (GotOrderMessageEvent != null)
                GotOrderMessageEvent(o, msg);
        }

        /// <summary>
        /// debug messages
        /// </summary>
        public event DebugDelegate SendDebugEvent;

        TickTracker _kt = new TickTracker();


        /// <summary>
        /// 产生新的tick用于引擎Fill Order
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            
            //_kt.addindex(k.symbol);
            //_kt.newTick(k);
            //
            //if (k.symbol == "m1305")
            //    debug("got tick:" + k.ToString());
            if (!_trackedSymbols.Contains(k.symbol))
                return;

            //debug("orderengine run here");
            //_kt.GotTick(k);
            process(k);
        }

        bool _noverb = true;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }
        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

             

        List<Order> orderPark = new List<Order>();
        /// <summary>
        /// send paper trade orders
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            if (o.id == 0)
            {
                GotOrderMessage(o, "OrderID为空,委托被拒绝");
                return;
            }
            //系统只维护追价委托,其他委托类型由CTP直接处理
            if (o.isStop)
            {
                lock (orderPark)
                {
                    debug("CTPOrderEngineHelper queueing order: " + o.ToString());
                    //模拟交易对新提交上来的order进行复制后保留,原来的order形成事件触发.这样本地的order fill process对order信息的修改就不会影响到对外触发事件的order.
                    //因为对外触发事件提交的order与本地按照tick fill order时使用的order是不同的副本
                    Order oc = new OrderImpl(o);
                    oc.Status = QSEnumOrderStatus.Opened;
                    orderPark.Add(oc);
                    //debug("order ID :" +orderPark[0].id.ToString());
                    _trackedSymbols.Add(oc.symbol);
                    GotOrder(oc);//给客户回报
                    GotOrderMessageEvent(o, "追价单|委托成功");
                }
            }
            else
            {

                api_SendOrder(o);//这里通过委托将市价单与限价单转发到CTP对应接口上
                
            }
        }

        /// <summary>
        /// 取消某个Order
        /// </summary>
        /// <param name="id"></param>
        public void CancelOrder(long id)
        {
            if (id == 0) return;
            lock (orderPark)
            {
                for (int i = 0; i < orderPark.Count; i++)
                {
                    //debug("orderID: " + orderPark[i].id.ToString());
                    if (id == orderPark[i].id)
                    {
                        Order o = orderPark[i];
                        orderPark.RemoveAt(i);
                        o.Status = QSEnumOrderStatus.Canceled;
                        GotOrder(o);
                        //如果取消的委托是本地维护的Order则给客户回报
                        debug("Order: "+id.ToString() +" 由OrderEngine管理 撤单后直接返回");
                        GotCancel(id);
                        return;
                    }
                }
                //如果本地没有维护该Order则将它发送到API接口处理
                debug("Order: "+id.ToString() +"并非由OrderEngine管理 通过API发送该Order");
                api_SendCancel(id);
            }
            

        }
        List<string> _trackedSymbols = new List<string>();
        void process(Tick nk)
        {
            Order[] orders;
            lock (orderPark)
            {
                // get copy of current orders
                orders = orderPark.ToArray();
                orderPark.Clear();
            }
            // get ready for unfilled orders
            List<Order> untigered = new List<Order>();
            // try to fill every order
            for (int i = 0; i < orders.Length; i++)
            {
                Order o = orders[i];
                if (nk.symbol == o.symbol)
                {
                    if (o.isStop)
                    {
                        //买单 买价大于stop  卖单 卖价小于stop 触发Order
                        if ((nk.hasBid && nk.bid >= o.stopp && o.side) || (nk.hasAsk && nk.ask <= o.stopp && !o.side))
                        {
                            GotOrderMessage(o, "追价单被触发 市价入场");
                            //条件单转换成市价单进行快速委托
                            Order no = new OrderImpl(o);//new MarketOrder(o.symbol, o.size, o.id);
                            no.price = 0;
                            no.stopp = 0;
                            api_SendOrder(no);
                        }
                        else
                        {
                            //如果没有触发则将Order放回到未触发队列
                            untigered.Add(o);
                        }
                    }
                }
                else
                {   //如果tick的symbol与order的不对应 则将该order返回到未触发列表
                    untigered.Add(o);
                }
 
            }
            //将未触发order回写到监控Order列表
            lock (orderPark)
            {
                // add orders back
                for (int i = 0; i < untigered.Count; i++)
                {
                    orderPark.Add(untigered[i]);
                    //_trackedSymbols.Add(untigered[i].symbol);
                }
            }
            //重置观察symbol列表
            lock (_trackedSymbols)
            {
                //清空原有追踪symbol列表,然后将没有触发Order添加到列表中
                _trackedSymbols.Clear();
                for (int i = 0; i < untigered.Count; i++)
                {
                    _trackedSymbols.Add(untigered[i].symbol);
                }
            }
           
        }
        
    }
}
