using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 提交新的委托前取消一组委托 用于强平持仓(撤单后再平仓)
    /// </summary>
    public class OrderTransactionDeleteBeforeInsert
    {
        public OrderTransactionDeleteBeforeInsert(List<long> olist, Order norder)
        {
            orderToBeDeleted = olist;
            orderToBeSend = norder;
        }
        //得到一个取消回报
        public void GotCancel(long id)
        {
            lock (this)
            {
                if (orderToBeDeleted.Contains(id))
                {
                    orderToBeDeleted.Remove(id);
                }
            }
        }
        public List<long> orderToBeDeleted;
        public Order orderToBeSend;
        public override string ToString()
        {
            return "先取消委托:" + string.Join(",", orderToBeDeleted.ToArray()) + " 然后再提交委托:" + orderToBeSend.ToString();
        }
    }

    /// <summary>
    /// 用于反手操作
    /// </summary>
    public class OrderTransactionFlatPositionBeforeInsert
    {
        public OrderTransactionFlatPositionBeforeInsert(Position pos, Order order)
        {
            positionToBeFlated = pos;
            orderToBeSend = order;
        }

        public void GotFill(Trade fill)
        { 
            //检查仓位是否已经平掉,注这里不需要维护自己的positin由系统的tracker统一维护position列表
            lock (this)
            {
                if (positionToBeFlated.isFlat)
                {
                    positionToBeFlated = null;
                }
            }
        }
        public Position positionToBeFlated;
        public Order orderToBeSend;
        public override string ToString()
        {
            return "先平仓:" + positionToBeFlated.ToString() + " 然后再提交委托:" + orderToBeSend.ToString();
        }
    }

   

    public class OrderTransactionHelper
    {
        /// <summary>
        /// 当委托前取消一组委托事务中 所有的委托均取消了，触发发送委托事件
        /// </summary>
        public event OrderDelegate SendOrderEvent;

        string PROGRAME = "OrderTransactionHelper";
        string _prefix = "";
        public event DebugDelegate SendDebugEvent;

        public OrderTransactionHelper(string prefix)
        {
            _prefix = prefix;
        }
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        public void Clear()
        {
            _orderDeleteBeforeInsertList.Clear();
            _orderFlatBeforeInsertList.Clear();
        }
        //增加一个委托前取消 事务
        public void AddDelBeforeInsert(long[] oarray, Order o)
        {
            OrderTransactionDeleteBeforeInsert n = new OrderTransactionDeleteBeforeInsert(oarray.ToList(),o);
            debug(PROGRAME +":新增事务:" +n.ToString());
            _orderDeleteBeforeInsertList.Add(n);
        }

        public void AddFlatPositionBeforeInsert(Position pos, Order o)
        {
            OrderTransactionFlatPositionBeforeInsert n = new OrderTransactionFlatPositionBeforeInsert(pos, o);
            _orderFlatBeforeInsertList.Add(n);
            debug(PROGRAME + ":新增事务:" + n.ToString());
        }


        public void GotFill(Trade fill)
        {
            FlatBeforeInsert_GotFill(fill);
        }
        void FlatBeforeInsert_GotFill(Trade fill)
        {
            lock (_orderFlatBeforeInsertList)
            {
                foreach (OrderTransactionFlatPositionBeforeInsert fbi in _orderFlatBeforeInsertList)
                {
                    fbi.GotFill(fill);
                }
            }
        }
        public void GotCancel(long oid)
        {
            DelBeforeInsert_GotCancel(oid);
           
        }
        //委托前取消事务 得到一个新的取消
        void DelBeforeInsert_GotCancel(long oid)
        {

            lock (_orderDeleteBeforeInsertList)
            {
                foreach (OrderTransactionDeleteBeforeInsert dbi in _orderDeleteBeforeInsertList)
                {
                    debug(PROGRAME +":委托前取消事务得到取消回报");
                    dbi.GotCancel(oid);
                }
            }
        }

        public void Start()
        {
            if(_processOrdActTransaction_go) return;
            _processOrdActTransaction_go = true;
            processOrdActTransaction_Thread = new Thread(processOrdActTransaction);
            processOrdActTransaction_Thread.IsBackground = true;
            processOrdActTransaction_Thread.Name = "OrderActionTransactoin@"+_prefix;
            processOrdActTransaction_Thread.Start();
            ThreadTracker.Register(processOrdActTransaction_Thread);

        }

        public void Stop()
        {
            if (!_processOrdActTransaction_go) return;
            ThreadTracker.Unregister(processOrdActTransaction_Thread);
            _processOrdActTransaction_go = false;
            int mainwait = 0;
            while (processOrdActTransaction_Thread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            processOrdActTransaction_Thread.Abort();
            processOrdActTransaction_Thread = null;
        }

        
        //委托前取消事务 列表
        List<OrderTransactionDeleteBeforeInsert> _orderDeleteBeforeInsertList = new List<OrderTransactionDeleteBeforeInsert>();
        List<OrderTransactionFlatPositionBeforeInsert> _orderFlatBeforeInsertList = new List<OrderTransactionFlatPositionBeforeInsert>();
        bool _processOrdActTransaction_go = false;
        Thread processOrdActTransaction_Thread;
        //static ManualResetEvent _waiting = new ManualResetEvent(false);
        //事务监听处理流程
        void processOrdActTransaction()
        {
            while (_processOrdActTransaction_go)
            {
                try
                {
                    //处理提交委托前取消委托
                    lock (_orderDeleteBeforeInsertList)
                    {
                        List<OrderTransactionDeleteBeforeInsert> handleddbi = new List<OrderTransactionDeleteBeforeInsert>();
                        for (int i = 0; i < _orderDeleteBeforeInsertList.Count; i++)
                        {
                            //检查如果委托前取消已经全部取消了,怎我们进行发送委托 同时记录该事务，以将其从队列中删除
                            if (_orderDeleteBeforeInsertList[i].orderToBeDeleted.Count == 0)
                            {
                                if (SendOrderEvent != null)
                                    SendOrderEvent(_orderDeleteBeforeInsertList[i].orderToBeSend);
                                handleddbi.Add(_orderDeleteBeforeInsertList[i]);
                            }
                        }
                        //将处理完毕的删除
                        foreach (OrderTransactionDeleteBeforeInsert dbi in handleddbi)
                        {
                            _orderDeleteBeforeInsertList.Remove(dbi);
                        }
                    }

                    //处理提交委托前平仓事务
                    lock (_orderFlatBeforeInsertList)
                    {
                        List<OrderTransactionFlatPositionBeforeInsert> handledfbi = new List<OrderTransactionFlatPositionBeforeInsert>();
                        for (int i = 0; i < _orderFlatBeforeInsertList.Count; i++)
                        {
                            if (_orderFlatBeforeInsertList[i].positionToBeFlated == null)
                            {
                                if (SendOrderEvent != null)
                                    SendOrderEvent(_orderFlatBeforeInsertList[i].orderToBeSend);
                                handledfbi.Add(_orderFlatBeforeInsertList[i]);
                            }
                        }
                        foreach (OrderTransactionFlatPositionBeforeInsert fbi in handledfbi)
                        {
                            _orderFlatBeforeInsertList.Remove(fbi);
                        }
                    }

                    Thread.Sleep(10);
                    // clear current flag signal
                    //_waiting.Reset();
                    // wait for a new signal to continue reading
                    //_waiting.WaitOne(5000);
                }
                catch (Exception ex)
                {
                    debug(PROGRAME +":OrderTransactionHelper error:"+ex.ToString());
                }

            }
        }
    }
}
