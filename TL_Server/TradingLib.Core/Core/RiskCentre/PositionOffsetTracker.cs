using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 记录了所有帐户服务端的止盈与止损参数,当满足触发条件时进行相关操作
    /// </summary>
    public class PositionOffsetTracker:BaseSrvObject
    {
        public event AssignOrderIDDel AssignOrderIDEvent;
        public event OrderDelegate SendOrderEvent;
        public event LongDelegate CancelOrderEvent;

        public PositionOffsetTracker(ClearCentre clearcentre)
            : base("PositionOffsetTracker")
        {
            _clearcentre = clearcentre;
        }

        ClearCentre _clearcentre = null;
        
        
        //取消委托后等待一个tick再进行处理
        bool WaitAfterCancel = true;



        #region PositionOffset 数据结构
        /// <summary>
        /// 用于维护某个帐号 下所有的positionoffset列表
        /// </summary>
        ConcurrentDictionary<string, ThreadSafeList<PositionOffset>> accountPosOffsetMap = new ConcurrentDictionary<string, ThreadSafeList<PositionOffset>>();

        /// <summary>
        /// 用于维护某个合约下 所有account的positionoffset列表
        /// </summary>
        ConcurrentDictionary<string, ThreadSafeList<PositionOffset>> symbolPosOffsetMap = new ConcurrentDictionary<string, ThreadSafeList<PositionOffset>>();

        /// <summary>
        /// account-symbol 序号 所对应的所有positionoffset映射
        /// </summary>
        ConcurrentDictionary<string,PositionOffset> keyPosOffsetMap = new ConcurrentDictionary<string,PositionOffset>();

        #endregion

        public PositionOffsetArgs[] GetPositionOffset(string account)
        {
            if (accountPosOffsetMap.Keys.Contains(account))
            {
                List<PositionOffsetArgs> l = new List<PositionOffsetArgs>();
                foreach (PositionOffset po in accountPosOffsetMap[account])
                {
                    l.AddRange(po.Args);
                }
                return l.ToArray();
            }
            else
            {
                return new PositionOffsetArgs[] { };
            }
        }

        #region 响应外部数据输入 参数 行情 取消
        /// <summary>
        /// 更新PositionOffset 参数
        /// </summary>
        /// <param name="args"></param>
        public void GotPositionOffsetArgs(PositionOffsetArgs args)
        {
            string key = args.Account + "-" + args.Symbol;
            //未生成该数据集
            if (!keyPosOffsetMap.Keys.Contains(key))
            {
                debug("没有对应" + key + " 止盈止损监视器,生成对应的监视器", QSEnumDebugLevel.INFO);
                PositionOffset po = new PositionOffset(args.Account, args.Symbol,args.Side);

                keyPosOffsetMap.TryAdd(key, po);

                //未存在对应账户的列表集合
                if(!accountPosOffsetMap.Keys.Contains(args.Account))
                {
                    accountPosOffsetMap.TryAdd(args.Account,new ThreadSafeList<PositionOffset>());
                }
                //将positionoffset放入帐户对应的po列表
                accountPosOffsetMap[args.Account].Add(po);

                //未存在symbol所对应的列表集合
                if (!symbolPosOffsetMap.Keys.Contains(args.Symbol))
                {
                    symbolPosOffsetMap.TryAdd(args.Symbol, new ThreadSafeList<PositionOffset>());
                }
                //将positionoffset放入symbol对应的po列表
                symbolPosOffsetMap[args.Symbol].Add(po);
            }

            //已经生成了对应account-symbol的positionoffset对象,则更新参数
            keyPosOffsetMap[key].GotPositionOffsetArgs(args);
        }

        public void GotTick(Tick k)
        {
            string sym = k.symbol;

            if(symbolPosOffsetMap.Keys.Contains(sym))
            {
                foreach(PositionOffset po in  symbolPosOffsetMap[sym])
                {
                    ProcessPosiitonOffset(po, k);
                }
            }
        }

        public void GotCancel(long oid)
        {
            debug("获得取消回报.." + oid.ToString(), QSEnumDebugLevel.INFO);
            Order o = _clearcentre.SentOrder(oid);
            string key = o.Account + "-" + o.symbol;
            if (keyPosOffsetMap.Keys.Contains(key))
            {
                PositionOffset po = keyPosOffsetMap[key];

                //获得取消回报 表明该委托已经被取消
                if (oid == po.LossTakeOrder)
                    po.LossTakeOrder = -1;

                if (oid == po.ProfitTakeOrder)
                    po.ProfitTakeOrder = -1;
                
            }
        }
        #endregion

        /// <summary>
        /// 通过对positionoffsset进行Tick驱动,从而决定是否需要进行平仓
        /// 如果需要进行平仓,则对外发送平仓委托。并且针对需要平仓未及时平掉的仓位实行报警
        /// </summary>
        /// <param name="po"></param>
        /// <param name="k"></param>
        void ProcessPosiitonOffset(PositionOffset po, Tick k)
        {

            //是否设置止盈止损
            bool needcheck = po.NeedCheck;

            if (!needcheck) return;//没有有效止盈与止损参数 直接返回

            if (po.Position == null)
            {
                debug("未获得持仓数据,获得持仓数据", QSEnumDebugLevel.INFO);
                po.Position = _clearcentre.getPosition(po.Account, po.Symbol,po.Side);
            }

            //仍然没有对应的持仓数据 则直接返回
            if (po.Position == null)
                return;

            //是否触发平仓操作
            bool flatsend = po.NeedTakeProfit || po.NeedTakeLoss;
            
            //如果触发平仓操作并 仓位已经 flat,则重置参数 ->  触发了止盈与止损并平仓了 则表明是止盈或止损平仓
            if (flatsend && po.Position.isFlat) 
            {
                debug("触发平仓操作成功,重置止盈止损参数", QSEnumDebugLevel.INFO);
                po.Reset();
                return;
            }

            //如果设置了止盈止损,但是没有持仓了 则重置止盈止损 -> 手工或者其他方式平仓
            if (po.Position.isFlat)
            {
                debug("持仓被平仓,止盈止损重置", QSEnumDebugLevel.INFO);
                po.Reset();
                return;
            }

            //没有触发平仓,并且有持仓 则我们进行检查
            if (!flatsend)
            {
                po.NeedTakeLoss = po.CheckTakeLoss(k);
                if (po.NeedTakeLoss)
                {
                    po.LossFireCount++;
                    po.LossTakeTime = DateTime.Now;
                    po.LossTakeOrder = FlatPosition(po.Position,"服务端止损");
                    debug("触发服务端止损:"+po.LossTakePrice.ToString() +" "+po.LossTakeOrder.ToString(), QSEnumDebugLevel.INFO);
                }

                po.NeedTakeProfit = po.CheckTakeProfit(k);//检查是否需要止盈
                //如果需要止盈则发送委托
                if (po.NeedTakeProfit)
                {
                    po.ProfitFireCount++;//累加触发次数
                    po.ProfitTakeTime = DateTime.Now;//记录触发时间
                    po.ProfitTakeOrder = FlatPosition(po.Position,"服务端止盈");//平仓
                    debug("触发服务端止盈"+po.ProfitTakePrice.ToString() +" "+po.ProfitTakeOrder.ToString(), QSEnumDebugLevel.INFO);

                }
                return;
            }


            //如果标记需要执行止盈操作 并且仍然有持仓 则检查是否需要重新发送委托
            if (po.NeedTakeProfit)
            {
                //如果时间超过重发间隔,重新发送委托
                if (DateTime.Now.Subtract(po.ProfitTakeTime).TotalSeconds > PositionOffset.SendOrderDelay && po.ProfitFireCount < PositionOffset.SendRetry)
                {
                    if (po.ProfitTakeOrder > 0)//表明发送过委托 但是由于某些原因没有被成交,则取消该委托后重新发送委托
                    {
                        CancelOrder(po.ProfitTakeOrder);
                        if (WaitAfterCancel) return;
                    }
                    else
                    {
                        po.ProfitFireCount++;//累加触发次数
                        po.ProfitTakeTime = DateTime.Now;//记录触发时间
                        po.ProfitTakeOrder = FlatPosition(po.Position,"服务端止盈");//平仓
                    }
                }
                if (po.ProfitFireCount == PositionOffset.SendRetry)
                {
                    if (po.ProfitTakeOrder > 0)//表明发送过委托 但是由于某些原因没有被成交,则取消该委托后重新发送委托
                    {
                        CancelOrder(po.ProfitTakeOrder);
                    }
                    //达到最大触发次数 记录错误并报警
                    debug(po.ToString() +"多次触发 异常", QSEnumDebugLevel.ERROR);
                    po.Reset();
                }

            }
            //如果标记需要执行止损操作 并且仍然有持仓 则检查是否需要重新发送委托
            if (po.NeedTakeLoss)
            {
                if (DateTime.Now.Subtract(po.LossTakeTime).TotalSeconds > PositionOffset.SendOrderDelay && po.LossFireCount < PositionOffset.SendRetry)
                {
                    if (po.LossTakeOrder > 0)
                    {
                        CancelOrder(po.LossTakeOrder);
                        if (WaitAfterCancel) return;
                    }
                    else
                    {
                        po.LossFireCount++;
                        po.LossTakeTime = DateTime.Now;
                        po.LossTakeOrder = FlatPosition(po.Position,"服务端止损");
                    }
                }
                if (po.LossFireCount == PositionOffset.SendRetry)
                {
                    //达到最大触发次数 记录错误并报警
                    if (po.LossTakeOrder > 0)
                    {
                        CancelOrder(po.LossTakeOrder);
                    }
                    debug(po.ToString() + "多次触发 异常", QSEnumDebugLevel.ERROR);
                    po.Reset();
                }
            }


        }

        /// <summary>
        /// 重置参数
        /// </summary>
        public void Clear()
        {
            keyPosOffsetMap.Clear();
            accountPosOffsetMap.Clear();
            symbolPosOffsetMap.Clear();
        }



        /// <summary>
        /// 平调某个仓位
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        long FlatPosition(Position pos,string comment)
        {
            Order o = new MarketOrderFlat(pos);
            //if (o.side) o.price = 1000;
            //if (!o.side) o.price = 3000;
            o.Account = pos.Account;
            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
            o.OrderSource = QSEnumOrderSource.SRVPOSITIONOFFSET;
            o.comment =comment;

            //绑定委托编号
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);
            
            BasicTracker.SymbolTracker.TrckerOrderSymbol(o);
            
            //Security _sec = _clearcentre.getMasterSecurity(o.symbol);
            //if (_sec == null)
            //{
            //    debug("无有效合约", QSEnumDebugLevel.ERROR);
            //    return 1;
            //}
            //o.Exchange = _sec.DestEx;
            //o.LocalSymbol = o.symbol;
            
            if (SendOrderEvent != null)
                SendOrderEvent(o);
            return o.id;
        }

        void CancelOrder(long oid)
        {
            if (CancelOrderEvent != null)
                CancelOrderEvent(oid);
        }
    }
}
