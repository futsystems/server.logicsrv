using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using Krs.Ats.IBNet;

namespace Broker.Live
{
    public partial class TLBrokerIB
    {
        public bool Start(out string msg)
        {
            msg = string.Empty;
            //初始化参数
            ParseConfigInfo();
            //初始化
            InitBroker();

            _working = true;

            //恢复交易记录
            Resume();

            return true;
        }

        /// <summary>
        /// 初始化Broker通道接口
        /// </summary>
        void InitBroker()
        {
            tk = new BrokerTracker(this);
            orderIdtk = new IdTracker(IdTracker.ConnectorOwnerIDStart + _cfg.ID);

            client = new IBClient();
            client.OrderStatus += new EventHandler<OrderStatusEventArgs>(client_OrderStatus);
            client.ExecDetails += new EventHandler<ExecDetailsEventArgs>(client_ExecDetails);
            client.NextValidId += new EventHandler<NextValidIdEventArgs>(client_NextValidId);
            client.Error += new EventHandler<ErrorEventArgs>(client_Error);
            client.ConnectionClosed += new EventHandler<ConnectionClosedEventArgs>(client_ConnectionClosed);

            client.Connect(_srvinfo.ServerAddress, _srvinfo.ServerPort, int.Parse(_usrinfo.Field1));
        }

        /// <summary>
        /// 恢复日内交易记录
        /// </summary>
        void Resume()
        {
            try
            {
                logger.Info("Resume trading info from clearcentre....");
                IEnumerable<TradingLib.API.Order> orderlist = ClearCentre.SelectBrokerOrders(this.Token);
                IEnumerable<Trade> tradelist = ClearCentre.SelectBrokerTrades(this.Token);
                IEnumerable<PositionDetail> positiondetaillist = ClearCentre.SelectBrokerPositionDetails(this.Token);

                //恢复隔夜持仓数据
                foreach (PositionDetail pd in positiondetaillist)
                {
                    tk.GotPosition(pd);
                }
                logger.Info(string.Format("Resumed {0} Positions", positiondetaillist.Count()));
                //恢复日内委托
                foreach (TradingLib.API.Order o in orderlist)
                {
                    if (!string.IsNullOrEmpty(o.BrokerLocalOrderID))//BrokerLocalOrderID不为空
                    {
                        if (!localOrderID_map.Keys.Contains(o.BrokerLocalOrderID))
                        {
                            localOrderID_map.TryAdd(o.BrokerLocalOrderID, o);
                        }
                        else
                        {
                            logger.Warn("Duplicate BrokerLocalOrderID,Order:" + o.GetOrderInfo());
                        }
                    }
                    
                    tk.GotOrder(o);

                }
                logger.Info(string.Format("Resumed {0} Orders", orderlist.Count()));
                //恢复日内成交
                foreach (Trade t in tradelist)
                {
                    tk.GotFill(t);
                }
                logger.Info(string.Format("Resumed {0} Trades", tradelist.Count()));

                //恢复委托父子关系对 然后恢复到委托分拆器
                List<FatherSonOrderPair> pairs = GetOrderPairs(orderlist);
                foreach (FatherSonOrderPair pair in pairs)
                {
                    fatherOrder_Map.TryAdd(pair.FatherOrder.id, pair.FatherOrder);
                    fatherSonOrder_Map.TryAdd(pair.FatherOrder.id, pair.SonOrders.FirstOrDefault());
                    foreach (TradingLib.API.Order o in pair.SonOrders)
                    {
                        sonFathOrder_Map.TryAdd(o.id, pair.FatherOrder);
                    }
                }

                //数据恢复完毕后再绑定平仓明细事件 否则数据恢复过程中会产生平仓明细数据
                tk.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(tk_NewPositionCloseDetailEvent);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 记录从Broker交易信息维护器产生的平仓明细
        /// </summary>
        /// <param name="obj"></param>
        void tk_NewPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            this.LogBrokerPositionClose(obj);
        }


        List<FatherSonOrderPair> GetOrderPairs(IEnumerable<TradingLib.API.Order> sonOrders)
        {
            Dictionary<long, FatherSonOrderPair> pairmap = new Dictionary<long, FatherSonOrderPair>();
            foreach (TradingLib.API.Order o in sonOrders)
            {
                TradingLib.API.Order father = null;
                if (o.FatherBreed != null)
                {
                    QSEnumOrderBreedType bt = (QSEnumOrderBreedType)o.FatherBreed;
                    father = ClearCentre.SentOrder(o.FatherID, QSEnumOrderBreedType.ACCT);
                    //if (bt == QSEnumOrderBreedType.ACCT)//如果直接分帐户侧分解 从清算中查找该委托
                    //{
                    //    father = ClearCentre.SentOrder(o.FatherID,QSEnumOrderBreedType.ACCT);
                    //}
                    //if (bt == QSEnumOrderBreedType.ROUTER)
                    //{
                    //    father = ClearCentre.SentOrder(o.FatherID, QSEnumOrderBreedType.ROUTER);
                    //}
                }
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




        public void Start()
        {

        }

        public bool IsLive
        {
            get
            {
                if (!_connlost)
                {
                    return _working;
                }
                return _working;
            }
        }

        public void Stop()
        {
            if (!this.IsLive) return;
            _working = false;

            logger.Info("Stop IBClient and clear memory");
            if (client != null && client.Connected)
            {
                client.Disconnect();
                client.Stop();
            }

            tk.Clear();
            tk = null;

            //清空Map
            localOrderID_map.Clear();
            fatherOrder_Map.Clear();
            sonFathOrder_Map.Clear();
            fatherSonOrder_Map.Clear();
        }
    }
}
