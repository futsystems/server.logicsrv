using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    /// <summary>
    /// 交易通道平仓逻辑
    /// 系统整体按照先开先平的原则进行平仓,在平仓时,需要获得持仓明细数据 然后找出当前在平的持仓明细，从而进行判断是否需要进行拆单
    /// 
    /// </summary>
    public partial class BrokerRouter
    {
        /// <summary>
        /// 对外发送委托主体逻辑
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorTitle"></param>
        /// <returns></returns>
        bool SendOrderOut(Order o,out string errorTitle)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
            errorTitle = string.Empty;

            //1.如果是模拟交易则直接通过broker发送委托,broker_sendorder会按从路由选择器获得对应的路由
            if (account.OrderRouteType == QSEnumOrderTransferType.SIM)
                return BrokerSendOrder(o, out errorTitle);
            
            //2.需要通过成交接口处理的委托
            if (o.IsEntryPosition)
            {   
                //如果是开仓委托则直接发送
                return BrokerSendOrder(o, out errorTitle);
            }
            else
            {
                //平仓委托通过分析判断后对外发送
                return XBrokerSendOrder(o, out errorTitle);//是否需要拆解委托
            }
        }

        /// <summary>
        /// 平仓委托的处理
        /// 平仓委托经过检查后返回一组经过分拆或没有分拆的原始委托
        /// </summary>
        /// <param name="o"></param>
        bool XBrokerSendOrder(Order o,out string errorTitle)
        {
            logger.Info("XBrokerSendOrder Check Split,Send Order Out");
            IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
            Position pos = account.GetPosition(o.Symbol, o.PositionSide);//获得该委托对应预操作的持仓对象

            //筛选出没有平掉的持仓明细
            List<PositionDetail> poslisttoclose = new List<PositionDetail>();
            
            //遍历所有未平仓持仓明细数据,然后将欲平持仓放入列表
            int tocloseize = o.UnsignedSize;
            Dictionary<string, int> brokerclosemap = new Dictionary<string, int>();
            foreach(PositionDetail positiondetail in pos.PositionDetailTotal.Where(pd=>!pd.IsClosed()))
            {
                //如果不存在该broker token则添加
                if (!brokerclosemap.Keys.Contains(positiondetail.Broker))
                {
                    brokerclosemap.Add(positiondetail.Broker, 0);
                }

                poslisttoclose.Add(positiondetail);
                //平仓量 如果提交的平仓量>当前持仓明细 则取持仓明细的所有持仓量
                int closeamount = tocloseize > positiondetail.Volume ? positiondetail.Volume : tocloseize;
                brokerclosemap[positiondetail.Broker] += closeamount;//将对应的成交通道 需要提交的平仓数量进行累加
                //扣除平仓量
                tocloseize = tocloseize - closeamount;
                //如果平仓数量小于等于0了，表明我们要平的持仓已经累加完毕
                if(tocloseize<=0)
                    break;
            }

            string broker = string.Empty;
            //如果不需要拆分委托 则直接返回原始委托
            errorTitle = string.Empty;
            if (brokerclosemap.Count == 1)
            {
                logger.Info("PositionDetails to be closed are in same broker,send order directly.");
                o.Broker = brokerclosemap.Keys.First();//设定预发送委托的BrokerToken
                return BrokerSendOrder(o, out errorTitle);
            }
            else
            {
                logger.Info("PositionDetails to be closed are in diferent broker,send order via spliter.");
                splitedordermap.TryAdd(o.id, o);
                _splittracker.SendFatherOrder(o, SplitOrder(o, brokerclosemap));
                return true;
            }
        }

        /// <summary>
        /// 查找某个路由侧委托
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Order SentRouterOrder(long val)
        {
            return _splittracker.SentSonOrder(val);
        }

        /// <summary>
        /// 从数据库恢复日内委托数据
        /// 该函数需要在清算中心数据加载后才可以被调用,否则我们无法获得到对应的父委托(分帐户侧数据)
        /// </summary>
        void ResumeRouterOrder()
        {
            logger.Info("resume router roder....");
            //从数据库恢复子委托数据(路由侧分解的子委托)
            IEnumerable<Order> orderlist = TLCtxHelper.ModuleDataRepository.SelectRouterOrders();
            //生成父子委托对
            List<FatherSonOrderPair> pairs = GetOrderPairs(orderlist);
            //恢复到分解器
            foreach (FatherSonOrderPair pair in pairs)
            {
                _splittracker.ResumeOrder(pair);
                //将父委托记录到本地缓存
                splitedordermap.TryAdd(pair.FatherOrder.id, pair.FatherOrder);
            }
            logger.Info("resumed router order num:" + splitedordermap.Count.ToString());
        }

        List<FatherSonOrderPair> GetOrderPairs(IEnumerable<Order> sonOrders)
        {
            Dictionary<long, FatherSonOrderPair> pairmap = new Dictionary<long, FatherSonOrderPair>();
            foreach (Order o in sonOrders)
            {
                //路由侧委托都是从帐户侧分解的 这里不用判断 直接通过清算中心查询父委托ID
                Order father = TLCtxHelper.ModuleClearCentre.SentOrder(o.FatherID);
                   
                //如果存在父委托
                if (father != null)
                {
                    //如果不存在该父委托 则增加
                    if (!pairmap.Keys.Contains(father.id))
                    {
                        pairmap[father.id] = new FatherSonOrderPair(father);
                    }
                    //将子委托加入到列表
                    pairmap[father.id].SonOrders.Add(o);
                }
            }
            return pairmap.Values.ToList();
        }

        /// <summary>
        /// 记录拆分的父委托
        /// </summary>
        ConcurrentDictionary<long, Order> splitedordermap = new ConcurrentDictionary<long, Order>();

        /// <summary>
        /// 判断某委托是否被分拆过
        /// 取消委托，如果委托分拆过，则委托需要通过分拆器进行撤销
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool IsOrderSplited(Order o)
        {
            if (splitedordermap.Keys.Contains(o.id))
                return true;
            return false;
        }
        

        IdTracker _routersonidtk = new IdTracker(5);
        OrderSplitTracker _splittracker = null;

        void InitSplitTracker()
        {
            _splittracker = new OrderSplitTracker("Router");
            //委托分拆器发送子委托通过接口发送
            _splittracker.SendSonOrderEvent += new OrderDelegate(SendSonOrderEvent);
            //委托分拆器发送取消委托通过接口取消
            _splittracker.CancelSonOrderEvent += new OrderDelegate(CancelSonOrderEvent);

            //委托分拆器请求分拆委托,调用本地实现的分拆逻辑SplitOrder
            //_splittracker.SplitOrdereEvent += new Func<Order, List<Order>>(SplitOrder);

            //委托分拆器更新父委托 本地对外通知父委托更新
            _splittracker.GotFatherOrderEvent += new OrderDelegate(Broker_GotOrder);
            //_splittracker.GotFatherCancelEvent += 
            //委托分拆器更新成交 本地对外通知成交更新
            _splittracker.GotFatherFillEvent += new FillDelegate(Broker_GotFill);
            //委托分拆器更新错误 本地对外通知错误更新
            _splittracker.GotFatherOrderErrorEvent += new OrderErrorDelegate(Broker_GotOrderError);
        }

        void CancelSonOrderEvent(Order order)
        {
            logger.Info("委托分拆器调用BrokerSendOrder取消委托:" + order.GetOrderInfo());
            BrokerCancelOrder(order);
        }

        /// <summary>
        /// 底层发送委托,包含路由选择
        /// </summary>
        /// <param name="order"></param>
        void SendSonOrderEvent(Order order)
        {
            logger.Info("委托分拆器调用BrokerSendOrder发送委托:" + order.GetOrderInfo());
            string error=string.Empty;
            BrokerSendOrder(order, out error);
        }


        /// <summary>
        /// 分拆委托
        /// 函数输入
        /// 将委托按给出的映射进行分拆
        /// </summary>
        /// <param name="o"></param>
        /// <param name="needsplit"></param>
        /// <returns></returns>
        List<Order> SplitOrder(Order o,Dictionary<string, int> splitmap)
        {
            List<Order> orderlist = new List<Order>();
            foreach (string key in splitmap.Keys)
            {
                Order neworder = new OrderImpl(o);
                neworder.TotalSize = splitmap[key]*(o.Side?1:-1);
                neworder.Size = neworder.TotalSize;

                //设定分解后委托父ID
                neworder.FatherID = o.id;//标注该委托的父委托为原始委托ID
                neworder.FatherBreed = o.Breed;
                //设定当前分解源
                neworder.Breed = QSEnumOrderBreedType.ROUTER;

                neworder.Broker = key;
                neworder.id = _routersonidtk.AssignId;

                neworder.BrokerLocalOrderID = "";
                neworder.BrokerRemoteOrderID = "";
                neworder.OrderSeq = 0;
                neworder.OrderRef = "";
                neworder.FrontIDi = 0;
                neworder.SessionIDi = 0;
                //重置委托相关状态
                //neworder.Account = key;拆分后委托Account不变,否则该委托无法通过帐户路由去找到对应的数据
                orderlist.Add(neworder);
                LogRouterOrder(neworder);//记录分解的委托
            }
            return orderlist;
        }
    }
}
