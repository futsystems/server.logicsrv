using System;
using System.Collections.Generic;
using TradingLib.API;
using System.Collections;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于记录Order状态
    /// </summary>
    [Serializable]
    public class OrderTracker : GotOrderIndicator, GotCancelIndicator, GotFillIndicator, GenericTrackerI, IEnumerable
    {
        public void Clear()
        {
            orders.Clear();
            sent.Clear();
            filled.Clear();
            canceled.Clear();
            
        }
        public int addindex(string txt, int ignore)
        {
            return orders.addindex(txt);
        }
        public string Display(string txt) { return string.Empty; }
        public string Display(int idx) { return this[idx].ToString(); }
        public string getlabel(int idx) { return orders.getlabel(idx); }
        public int Count { get { return orders.Count; } }
        public decimal ValueDecimal(string txt) { int idx = getindex(txt); if (idx < 0) return 0; return this[idx]; }
        public decimal ValueDecimal(int idx) { return this[idx]; }
        public object Value(string txt) { return ValueDecimal(txt); }
        public object Value(int idx) { return ValueDecimal(idx); }
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; } }
        public int addindex(string txt)
        {
            return orders.addindex(txt);
        }
        public int getindex(string txt) { return orders.getindex(txt); }
        public Type TrackedType { get { return typeof(int); } }
        /// <summary>
        /// get orders from tracker
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() { foreach (Order o in orders) yield return o; }
        /// <summary>
        /// create a tracker
        /// </summary>
        public OrderTracker()
        {
            orders.NewTxt += new TextIdxDelegate(orders_NewTxt);//当有新的委托被加入到记录器时
        }
        public event TextIdxDelegate NewTxt;

        /// <summary>
        /// 新的委托加入到记录器,我们需要增加对应的其他list以维护数据的统一
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="idx"></param>
        void orders_NewTxt(string txt, int idx)
        {
            int sentsize = (orders[idx].side? 1 : -1) * Math.Abs(orders[idx].TotalSize);//通过totalsize来获得委托的原始数量 size为委托的剩余数量
            v(txt + " sentsize: " + sentsize + " after: " + orders[idx].ToString());
            unknownsent.addindex(txt, false);
            sent.addindex(txt, sentsize);//第一次委托到达时候记录该委托数量为发送数量sentsize
            filled.addindex(txt, 0);
            canceled.addindex(txt, false);
            if (NewTxt != null)
                NewTxt(txt, idx);
        }


        //获得某个合约的所有pendingOrder
        /// <summary>
        /// 获得某个合约 买入或卖出 的等待成交委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public long[] getPending(string symbol,bool side)
        {
            List<long> olist = new List<long>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].symbol == symbol &&  orders[i].side == side && isPending(i))
                    olist.Add(orders[i].id);
            }
            return olist.ToArray();
        }

        /// <summary>
        /// 获得某个合约的所有待成交委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public long[] getPending(string symbol)
        {
            List<long> olist = new List<long>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].symbol == symbol && isPending(i))
                    olist.Add(orders[i].id);
            }
            return olist.ToArray();
        }

        /// <summary>
        /// 获得所有待成交委托
        /// </summary>
        /// <returns></returns>
        public long[] getPending()
        {
            List<long> olist = new List<long>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (isPending(i))
                    olist.Add(orders[i].id);
            }
            return olist.ToArray();
        }


        /// <summary>
        /// 获得所有未成交委托
        /// </summary>
        /// <returns></returns>
        public Order[] getPendingOrders()
        {
            List<Order> olist = new List<Order>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (isPending(i))
                    olist.Add(orders[i]);
            }
            return olist.ToArray();
        }
        

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="symbol"></param>
        ///// <returns></returns>
        //public int getUnfilledLong(string symbol)
        //{
        //    return getUnfilledSize(symbol, true);
        //}

        //public int getUnfilledShort(string symbol)
        //{
        //    return getUnfilledSize(symbol,false);
        //}

        /// <summary>
        /// 获得某个合约某个方向未成交数量
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public int getUnfilledSize(string symbol, bool side)
        { 
            int size=0;
            for(int i=0;i<orders.Count;i++)
            {
                if (orders[i].symbol == symbol && orders[i].side == side && isPending(i))
                    size = size + this[i];
            }
            return size;
        }



        //CTP不支持条件单,因此需要在系统内自己实现追价单.
        //系统OrderEngine接受了Order之后会给客户一个Order回报,ClearCenter也会记录并维护该Order
        //当CTP发送Order时候会检查正向开仓，反向则需要检查当前已委托数量
        /// <summary>
        /// 查询某个合约,在某个方向上的待成交合约
        /// 比如在平多头时,需要查询 卖出的委托[平掉当前多头] 如果待成交委托累计数量超过可平仓数 则出错
        /// 用于CTP下单检查
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public int getUnfilledSizeExceptStop(string symbol, bool side)
        {
            int size = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].symbol == symbol && orders[i].side == side && isPending(i) && (!orders[i].isStop))
                    size = size + this[i];

            }
            return size;
        }


        /// <summary>
        /// 获得某个合约某个方向的未成交委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        /*
        public int getUnfilledSize(string symbol, bool side)
        {
            int size = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].symbol == symbol && orders[i].side == side && isPending(i))
                    size = size + this[i];

            }
            return size;
        }
        **/


        GenericTracker<bool> unknownsent = new GenericTracker<bool>();
        GenericTracker<int> sent = new GenericTracker<int>();//记录发送数量
        GenericTracker<int> filled = new GenericTracker<int>();//记录成交数量
        GenericTracker<bool> canceled = new GenericTracker<bool>();//记录是否取消
        GenericTracker<Order> orders = new GenericTracker<Order>();//记录委托

        /// <summary>
        /// 是否等待成交,即委托没有完全成交,并且委托没有被取消
        /// 1.filled,partfilled 由ordertracker gotfill来记录
        /// 2.cancel 由ordertracker gotcancel来记录
        /// 3.reject ordertracker不进行记录,只是gotorder来记录不同的委托状态
        /// 
        /// placed 是指被系统接受,并没有到达任何成交路由
        /// submited 是被系统提交到成交路由中心,但是没有获得对应的成交接口的回报
        /// opened 是指被成交接口接收 并正常返回
        /// 
        /// 
        /// 保证金：系统检查保证金时由于委托提交到接口 接口返回委托有时间延迟，如果以Opened为标识来累加保证金 则会导致 某些委托躲过保证见检查 因此以锁内操作broker.sendorder中产生的submit状态来计算保证金
        /// 
        /// 系统可平仓位：当系统检查可平仓位时,需要检查当前等待成交的委托,比如持仓5收 当前等待成交的卖出委托3手 则可平仓位为2手。这里判断等待成交 也是要包含submit,否则会躲过成交路由检查,造成多次反向操作
        /// submited标识委托已经通过了BrokerRouter的开平检查,Placed只是表面委托通过了风控检查包括check1和check2
        /// 在没有提交到成交接口之前均不用计算保证金,因为这些委托还没有确认发到成交接口 意味着 发到成交接口的委托均要到计算到资金占用里面
        /// 在order从place->submit/Reject之间是否有时间间隙 被同一个帐户的其他委托所占用 ??
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isPending(long id) { return isPending(sent.getindex(id.ToString())); }
        bool isPending(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            
            Order o = orders[idx];

            /*
            if (canceled[idx]||o.Status==QSEnumOrderStatus.Reject || o.Status==QSEnumOrderStatus.Unknown)
                return false;
             * */
            //return Math.Abs(sent[idx]) != Math.Abs(filled[idx]);
            //通过委托状态来判定委托当前是否是Pending
            
            return IsPending(o);
        }



        /// <summary>
        /// OrderTracker静态函数 判断某个Order是否处于Pending状态
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsPending(Order o)
        {
            if (o.Status == QSEnumOrderStatus.Opened || o.Status == QSEnumOrderStatus.PartFilled || o.Status == QSEnumOrderStatus.Submited || o.Status == QSEnumOrderStatus.Placed)
                return true;
            return false;
        }

        /// <summary>
        /// 判断某个委托是否可以被撤销
        /// 通过Broker提交的委托
        /// 状态为Opened或者PartFilled这样的委托可以通过Broker进行撤单
        /// 这里有一个时间间隙
        /// MsgExch通过Broker.sendorder后的委托状态为Submited,在Submited和Broker返回接受后委托跟新为Opened之间有一个很小的时间间隙
        /// 在这个间隙内委托无法撤销,系统不确定委托是否正常提交到broker 提交到broker后的最终状态就是Opened或者Reject
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool CanCancel(Order o)
        {
            if (o.Status == QSEnumOrderStatus.Opened || o.Status == QSEnumOrderStatus.PartFilled)
                return true;
            return false;
        }

        /// <summary>
        /// 查看某个Order是否完全成交
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isCompleted(long id) { return isCompleted(sent.getindex(id.ToString())); }
        bool isCompleted(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            return sent[idx] == filled[idx];
        }

        /// <summary>
        /// 查看某个Order是否被取消
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool isCanceled(long id) { return isCanceled(sent.getindex(id.ToString())); }
        bool isCanceled(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            return canceled[idx];
        }

        /// <summary>
        /// 返回某个Order提交委托数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Sent(long id)
        {
            int idx = sent.getindex(id.ToString());
            if ((idx < 0) || (idx > sent.Count))
                return 0;
            return sent[idx];
        }
        /// <summary>
        /// 返回某个Order已成交数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Filled(long id)
        {
            int idx = sent.getindex(id.ToString());
            if ((idx < 0) || (idx > sent.Count))
                return 0;
            return filled[idx];
        }

        /// <summary>
        /// 返回某个提交的委托(Order)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order SentOrder(long id)
        {
            //debug("寻找Order: id" + id.ToString());
            if (orders.getindex(id.ToString()) != GenericTracker.UNKNOWN)
                return orders[id.ToString()];
            else
                return new OrderImpl();
        }



        /// <summary>
        /// 返回某个Order未成交数量(记录序号)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int this[int idx]
        {
            get 
            { 
                if ((idx<0) || (idx>filled.Count)) return 0;
                if (canceled[idx])
                    return 0;
                return Math.Abs(sent[idx])-Math.Abs(filled[idx]);
            }
        }

        /// <summary>
        /// 返回某个Order未成交数量(委托编号)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int this[long id]
        {
            get
            {
                int idx = sent.getindex(id.ToString());
                return this[idx];
            }
        }

        /// <summary>
        /// 查看某个Order是否被维护跟踪
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isTracked(long id)
        {
            int idx = sent.getindex(id.ToString());
            return idx != GenericTracker.UNKNOWN;
        }

        /// <summary>
        /// 返回所有委托数组
        /// </summary>
        /// <returns></returns>
        public Order[] ToArray()
        {
            return orders.ToArray();
        }
        public event DebugDelegate SendDebugEvent;
        protected void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        /*
         * 系统本身增加了未知委托的处理,取消,成交,等如果找不到对应的委托,则添加未知委托到系统记录,通过这样达到数据完整性
         * 
         * 
         * 
         * 
         * */
        /// <summary>
        /// 记录一个新的Order
        /// 如果是相同委托编号的委托,则更新委托状态
        /// </summary>
        /// <param name="o"></param>
        int r = 0;
        int l = 0;
        public virtual void GotOrder(Order o)
        {
            r = r + 1;
            if (o.id == 0)
            {
                debug(o.symbol + " can't track order with blank id!: " + o.ToString());
                return;
            }
            //debug("order:" + o.ToString() + " is tracked");
            int idx = sent.getindex(o.id.ToString());
            if (idx < 0)//记录器没有记录该委托
            {
                l = l + 1;
                idx = orders.addindex(o.id.ToString(), o);
                debug("Order Tracker got unique order:" + l.ToString());
            }
            else//记录器已经记录该委托,则进行状态修改更新
            {
                //发现某些情况下 委托状态与当前实际的ordertracker状态不同步,因此如果委托为opened,然后重新加载后
                //该委托仍然为opend 有可能被成交 通过委托来传递状态 或者由本地ordertracker来生成状态与成交数量 两种方式
                Order to = orders[idx];
                to.Status = o.Status;//更新委托状态
                to.Broker = o.Broker;//更新broker信息和ExchID
                to.BrokerKey = o.BrokerKey;
                to.OrderExchID = o.OrderExchID;
                to.comment = o.comment;//更新描述

                to.size = o.size;//更新委托当前未成交数量
                to.Filled = o.Filled;//更新成交数量
                
                
            }
            v(o.symbol + " order ack: " + o);
            debug("Order Tracker got order:"+r.ToString());
        }
        /// <summary>
        /// 记录一个Order取消
        /// </summary>
        /// <param name="id"></param>
        public virtual void GotCancel(long id)
        {
            if (id==0) return;
            int idx = sent.getindex(id.ToString());
            if (idx<0)
            {
                //如果OrderTracker没有该orderid则我们丢弃该cancel,并不需要增加一个空的order
                debug("no existing order found with cancel id: "+id);
                //idx = orders.addindex(id.ToString(), new OrderImpl());
                //unknownsent[idx] = true;
                //canceled[idx] = true;
                return;//如果没有维护该委托 则直接返回
            }
            canceled[idx] = true;
            //标注该委托状态为取消 这里的status在simbroker或者broker处已经被处理和修改 gotorder里面已经更新了委托状态
            SentOrder(id).Status = QSEnumOrderStatus.Canceled;
            /*
            if (_noverb) return;
            else
            {
                string symbol = "?";
                foreach (Order o in this)
                    if (o.id == id)
                        symbol = o.symbol;
                v(symbol + " canceled id: " + id);
            }*/
        }

        bool _fixsentonunknown = false;
        public bool FixSentSizeOnUnknown { get { return _fixsentonunknown; } set { _fixsentonunknown = value; } }

        /// <summary>
        /// 记录一笔成交
        /// </summary>
        /// <param name="f"></param>
        public virtual void GotFill(Trade f)
        {
            if (f.id == 0)
            {
                debug(f.symbol + " can't track order with blank id!: " + f.ToString());
                return;
            }
            
            int idx = sent.getindex(f.id.ToString());
            
            if (idx < 0)
            {
                debug("no existing order found with fillid: " + f.id);
                idx = orders.addindex(f.id.ToString(), new OrderImpl());
                unknownsent[idx] = true;
            }
            //累加某个委托的filled数量
            filled[idx] += (f.side ? 1 : -1) *Math.Abs(f.xsize);
            //fix sentsize on unknown 当由于某些原因，委托没有被记录到,而产生了不对应的成交,则我们根据成交来反向形成对应的委托
            if (FixSentSizeOnUnknown && unknownsent[idx])
                sent[idx] = filled[idx];
            v(f.symbol + " filled size: " + filled[idx] + " after: " + f.ToString());
        }

        bool _noverb = true;
        public bool VerboseDebugging
        {
            get { return !_noverb; }
            set
            {
                _noverb = !value;
            }
        }

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator(); // this will return the one
            // available through the object reference
            // ie. "IEnumerator<int> GetEnumerator"
        }
        public IEnumerator<T> GetEnumerator()
        {
            lock (m_lock)
            {
                for (int i = 0; i < m_list.Count; i++)
                    yield return m_list[i];
            }
        }**/
    }
}
