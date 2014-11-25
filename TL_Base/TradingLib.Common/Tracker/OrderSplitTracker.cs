using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{

    public class FatherSonOrderPair
    {
        public FatherSonOrderPair(Order father)
        {
            this.FatherOrder = father;
            this.SonOrders = new List<Order>();
        }
        public Order FatherOrder { get; set; }

        public List<Order> SonOrders { get; set; }
    }
    /// <summary>
    /// 委托分拆管理器
    /// 用于将某个委托分拆成多个委托然后对外处理
    /// 输入侧，操作父委托，系统按照分解逻辑分解后，将委托分解成子委托，然后输出子委托的操作
    /// 当有子委托回报时,调用子委托回报输入,对外输出父委托回报
    /// 该组件实现了将某个委托按一定逻辑分拆后下发到子委托操作端
    /// 然后从子委托操作端获得回报处理后，再处理成父委托回报对外输出
    /// 
    /// 父委托编号->父委托
    /// 父委托编号->子委托列表
    /// 子委托编号->父委托
    /// </summary>
    public class OrderSplitTracker
    {
        string _token = string.Empty;
        public OrderSplitTracker(string name)
        {
            _token = name;
        }
        string Token { get { return _token; } }
        //保存父委托
        ConcurrentDictionary<long, Order> fatherOrder_Map = new ConcurrentDictionary<long, Order>();
        Order FatherID2Order(long id)
        {
            if (fatherOrder_Map.Keys.Contains(id))
            {
                return fatherOrder_Map[id];
            }
            return null;
        }

        //用于通过父委托ID找到对应的子委托
        ConcurrentDictionary<long, List<Order>> fatherSonOrder_Map = new ConcurrentDictionary<long, List<Order>>();//父子子委托映射关系
        //通过父委托ID找到对应的子委托对
        List<Order> FatherID2SonOrders(long id)
        {
            if (fatherSonOrder_Map.Keys.Contains(id))
                return fatherSonOrder_Map[id];
            return null;
        }

        //用于通过子委托ID找到对应的父委托
        ConcurrentDictionary<long, Order> sonFathOrder_Map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 通过子委托ID找到对应的父委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order SonID2FatherOrder(long id)
        {
            if (sonFathOrder_Map.Keys.Contains(id))
                return sonFathOrder_Map[id];
            return null;
        }


        /// <summary>
        /// 清空内存状态
        /// </summary>
        public void Clear()
        {
            fatherOrder_Map.Clear();
            fatherSonOrder_Map.Clear();
            sonFathOrder_Map.Clear();
        }
        /// <summary>
        /// 恢复父子委托关系
        /// </summary>
        /// <param name="father"></param>
        /// <param name="sonOrders"></param>
        public void ResumeOrder(FatherSonOrderPair pair)
        {
            fatherOrder_Map.TryAdd(pair.FatherOrder.id, pair.FatherOrder);
            fatherSonOrder_Map.TryAdd(pair.FatherOrder.id, pair.SonOrders);
            foreach(Order o in pair.SonOrders)
            {
                sonFathOrder_Map.TryAdd(o.id, pair.FatherOrder);
            }
        }

        #region 对外发送子委托操作
        /// <summary>
        /// 发送子委托
        /// </summary>
        public event OrderDelegate SendSonOrderEvent;
        void SendSonOrder(Order o)
        {
            if (SendSonOrderEvent != null)
                SendSonOrderEvent(o);
        }

        /// <summary>
        /// 取消子委托
        /// </summary>
        public event OrderDelegate CancelSonOrderEvent;
        void CancelSonOrder(Order o)
        {
            if (CancelSonOrderEvent != null)
              CancelSonOrderEvent(o);
        }
        #endregion

        #region 对外发送父委托回报
        /// <summary>
        /// 获得父委托回报
        /// </summary>
        public event OrderDelegate GotFatherOrderEvent;
        void GotFatherOrder(Order o)
        {
            if (GotFatherOrderEvent != null)
                GotFatherOrderEvent(o);
        }

        /// <summary>
        /// 获得父成交回报
        /// </summary>
        public event FillDelegate GotFatherFillEvent;
        void GotFatherFill(Trade f)
        {
            if (GotFatherFillEvent != null)
                GotFatherFillEvent(f);
        }
        /// <summary>
        /// 获得父委取消
        /// </summary>
        public event LongDelegate GotFatherCancelEvent;
        void GotFatherCancel(long oid)
        {
            if (GotFatherCancelEvent != null)
                GotFatherCancelEvent(oid);
        }

        /// <summary>
        /// 获得父委托错误回报
        /// </summary>
        public event OrderErrorDelegate GotFatherOrderErrorEvent;
        void GotFatherOrderError(Order o, RspInfo info)
        {
            if (GotFatherOrderErrorEvent != null)
                GotFatherOrderErrorEvent(o, info);
        }
        #endregion



        /// <summary>
        /// 分解委托
        /// </summary>
        public event Func<Order, List<Order>> SplitOrdereEvent;
        List<Order> SplitOrder(Order o)
        {
            if (SplitOrdereEvent != null)
                return SplitOrdereEvent(o);
            return new List<Order>();
        }


        #region 接受父委托端输入
        /// <summary>
        /// 发送父委托
        /// </summary>
        /// <param name="fathOrder"></param>
        public void SendFatherOrder(Order fathOrder)
        {
            Util.Debug("OrderSplitTracker[" + this.Token + "] Send FatherOrder:" + fathOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
            //1.分拆委托
            List<Order> sonOrders = SplitOrder(fathOrder);//分拆该委托

            //2.将委托加入映射map
            fatherOrder_Map.TryAdd(fathOrder.id, fathOrder);//保存付委托映射关系
            fatherSonOrder_Map.TryAdd(fathOrder.id, sonOrders);//保存父委托到子委托映射关系

            //2.统一发送子委托
            foreach (Order order in sonOrders)
            {
                sonFathOrder_Map.TryAdd(order.id, fathOrder);//保存子委托到父委托映射关系
                SendSonOrder(order);
            }
            //3.更新父委托状态
            //如果对应的子委托有正常提交的,那么父委托就是处于提交状态
            if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Submited))
            {
                fathOrder.Status = QSEnumOrderStatus.Submited;
            }
            //如果子委托状态为拒绝,则父委托的状态也为拒绝
            if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Reject))
            {
                fathOrder.Status = QSEnumOrderStatus.Reject;
            }
            Util.Debug("父子委托关系链条 " + fathOrder.id + "->[" + string.Join(",", sonOrders.Select(so => so.id)) + "]", QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 取消父委托
        /// </summary>
        /// <param name="oid"></param>
        public void CancelFatherOrder(long oid)
        {
            Order fatherOrder = FatherID2Order(oid);
            if (fatherOrder != null)
            {
                Util.Debug("OrderSplitTracker[" + this.Token + "] 取消父委托:" + fatherOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
                List<Order> sonOrders = FatherID2SonOrders(fatherOrder.id);//获得子委托
                //如果子委托状态处于pending状态 则发送取消
                foreach (Order o in sonOrders)
                {
                    if (o.IsPending())
                    {
                        CancelSonOrder(o);
                    }
                }
            }
            else
            {
                Util.Debug("Order:" + oid.ToString() + " is not in platform_order_map in broker", QSEnumDebugLevel.WARNING);
            }
        }
        #endregion


        #region 子委托端 交易信息输入
        /// <summary>
        /// 获得子委托回报
        /// </summary>
        /// <param name="o"></param>
        public void GotSonOrder(Order o)
        {
            //更新子委托数据完毕后 通过子委托找到父委托 然后转换状态并发送
            Order fatherOrder = SonID2FatherOrder(o.id);//获得父委托
            List<Order> sonOrders = FatherID2SonOrders(fatherOrder.id);//获得子委托列表
            
            fatherOrder.OrderSysID = fatherOrder.OrderSeq.ToString();//父委托OrderSysID编号 取系统的OrderSeq

            //更新父委托状态 成交数量 状态 以及 状态信息
            fatherOrder.FilledSize = sonOrders.Sum(so => so.FilledSize);//累加成交数量
            fatherOrder.Size = sonOrders.Sum(so => so.UnsignedSize) * (o.Side ? 1 : -1);//累加未成交数量
            //fatherOrder.Comment = "";//填入状态信息
            //组合状态
            QSEnumOrderStatus fstatus = fatherOrder.Status;
            //子委托全部成交 则父委托为全部成交
            if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Filled))
                fstatus = QSEnumOrderStatus.Filled;
            //子委托任一待成交,则父委托为待成交
            else if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Opened))
                fstatus = QSEnumOrderStatus.Opened;
            //子委托全部拒绝,则父委托为拒绝
            else if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Reject))
                fstatus = QSEnumOrderStatus.Reject;
            //子委托有任一取消,则父委托为取消
            else if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Canceled))
                fstatus = QSEnumOrderStatus.Canceled;
            //子委托有一个委托为部分成交
            else if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.PartFilled))
            {
                //另一个委托为取消，则父委托为取消
                if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Canceled))
                    fstatus = QSEnumOrderStatus.Canceled;

                //另一个委托为拒绝,则父委托为取消
                //if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Reject))
                fstatus = QSEnumOrderStatus.PartFilled;
            }
            fatherOrder.Status = fstatus;
            fatherOrder.Comment = o.Comment;
            Util.Debug("更新父委托:" + fatherOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
            GotFatherOrder(fatherOrder);
            if (fatherOrder.Status == QSEnumOrderStatus.Canceled)
            {
                GotFatherCancel(fatherOrder.id);
            }
        }

        /// <summary>
        /// 获得子委托成交回报
        /// </summary>
        /// <param name="f"></param>
        public void GotSonFill(Trade f)
        {
            //付委托对应的成交
            Order fatherOrder = SonID2FatherOrder(f.id);//获得父委托
            Trade fill = (Trade)(new OrderImpl(fatherOrder));

            //设定价格 数量 以及日期信息
            fill.xSize = f.UnsignedSize * (f.Side ? 1 : -1);
            fill.xPrice = (decimal)f.xPrice;

            fill.xDate = f.xDate;
            fill.xTime = f.xTime;
            //远端成交编号
            //fill.BrokerTradeID = trade.BrokerTradeID;
            //其余委托类的相关字段在Order处理中获得
            Util.Debug("获得父成交:" + fill.GetTradeDetail(), QSEnumDebugLevel.INFO);
            GotFatherFill(fill);
        }

        /// <summary>
        /// 获得子委托错误回报
        /// </summary>
        /// <param name="o"></param>
        /// <param name="error"></param>
        public void GotSonOrderError(Order o, RspInfo error)
        {
            Order fatherOrder = SonID2FatherOrder(o.id);//获得父委托
            List<Order> sonOrders = FatherID2SonOrders(fatherOrder.id);//获得所有子委托

            RspInfo info = new RspInfoImpl();
            info.ErrorID = error.ErrorID;
            info.ErrorMessage = error.ErrorMessage;

            bool isrejected = (fatherOrder.Status == QSEnumOrderStatus.Reject);
            //所有子委托为拒绝则父委托为拒绝
            if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Reject))
            {
                fatherOrder.Status = QSEnumOrderStatus.Reject;
            }
            //如果部分拒绝如何？另一部分处于成交状态，或者等待成就状态
            //fatherOrder.Status = QSEnumOrderStatus.Reject;
            fatherOrder.Comment = info.ErrorMessage;
            Util.Debug("更新父委托:" + fatherOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
            //父委托已经对外回报过拒绝则不再对外回报
            if (!isrejected)
                GotFatherOrderError(fatherOrder, info);
        }

        #endregion

    }
}
