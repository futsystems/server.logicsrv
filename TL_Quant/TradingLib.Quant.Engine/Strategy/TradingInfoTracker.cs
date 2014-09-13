using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Runtime.Serialization;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 用于记录某个策略的交易记录.委托/取消/下单/等信息
    /// 用于记录对应的虚拟财务信息
    /// 通过对交易结果的信息的加工形成策略的评奖信息
    /// 同时应为持有了仓位数据可以形成其他一些扩展事件
    /// </summary>
    [Serializable]
    public class TradingInfoTracker :ISerializable,ITradingInfoTracker
    {
        public event PositionEventDel SendEntryPosiitonEvent;
        public event PositionEventDel SendExitPositionEvent;
        public event PositionEventDel SendChangePositionEvent;

        public event PositionEventDel SendHistoryPositionDataEvent;

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        OrderTracker _ordTracker;//记录策略产生的委托
        PositionTracker _postTracker;//记录策略产生的持仓数据
        List<Trade> _tradeTracker;//记录策略产生的成交记录
        PositionRoundTracker _prt;
        public OrderTracker OrderManager { get { return _ordTracker; } }
        public PositionTracker PositionManager { get { return _postTracker; } }
        public List<Trade> TradeManager { get { return _tradeTracker; } }
        Dictionary<string, PositionDataPair> openPositionData = new Dictionary<string, PositionDataPair>();

        [NonSerialized]
        StrategyData _strategydata;
        public TradingInfoTracker(StrategyData strategyData)
        {


            _ordTracker = new TradingLib.Common.OrderTracker();
            _postTracker = new TradingLib.Common.PositionTracker();
            _tradeTracker = new List<Trade>();
            _prt = new PositionRoundTracker();
            _strategydata = strategyData;
        }

        public PositionRound[] GetPositionRoundList()
        {
            return _prt.RoundClosed;
        }
        public void SerializeOwnedData(SerializationWriter w, object context)
        {
            w.WriteObject(_ordTracker);
            //OrderTracker tmp = new TradingLib.Common.OrderTracker();
            //Order o = new MarketOrder("IF", 20);
            //tmp.GotOrder(o);
            //w.WriteObject(tmp);
            w.WriteObject(_postTracker);//持仓数据
            w.WriteObject(_tradeTracker);//成交记录
            w.WriteObject(_prt);//持仓回合数据
            w.WriteObject(openPositionData);
        }

        public void DeserializeOwnedData(SerializationReader r, object context)
        {
            _ordTracker = (OrderTracker)r.ReadObject();
            _postTracker = (PositionTracker)r.ReadObject();
            _tradeTracker = (List<Trade>)r.ReadObject();
            _prt = (PositionRoundTracker)r.ReadObject();
            openPositionData = (Dictionary<string, PositionDataPair>)r.ReadObject();
        }

        protected TradingInfoTracker(SerializationInfo info, StreamingContext context)
        {
            SerializationReader r = new SerializationReader((byte[]) info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(r, context);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter w = new SerializationWriter();
            this.SerializeOwnedData(w, context);
            info.AddValue("data", w.ToArray());
        }

        public void GotOrder(Order o)
        {
            _ordTracker.GotOrder(o);
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        public void GotTick(Tick k)
        {
            //PositionTracker.GotTick(k);
            foreach (PositionDataPair info in this.openPositionData.Values)
            {
                info.Position.GotTick(k);
            }
        }
        /// <summary>
        /// 获得Open持仓数据集合
        /// </summary>
        /// <returns></returns>
        public List<PositionDataPair> GetOpenPositionData()
        {
            return openPositionData.Values.ToList();
        }

        public PositionDataPair GetPositionData(string key)
        { 
            PositionDataPair data;
            if (openPositionData.TryGetValue(key, out data))
                return data;
            else
                return new PositionDataPair();
        }

        public void GotFill(Trade f)
        {
            //debug("trading info got trade :" + f.ToString());
            Security sec = _strategydata.GetSymbolByName(f.symbol);
            if (sec == null) return;

            

            int beforesize = _postTracker[f.symbol].UnsignedSize;
            double oldCost = (double)_postTracker[f.symbol].AvgPrice;

            _tradeTracker.Add(f);
            _ordTracker.GotFill(f);
            _postTracker.GotFill(f);

            Position newPosition = _postTracker[f.symbol];
            decimal highest = newPosition.Highest;//获得持仓最高价
            decimal lowest = newPosition.Lowest;//获得持仓以来最低价

            
            int aftersize = newPosition.UnsignedSize;
            

            PositionTransaction postrans=null;
            
            
            //计算手续费
            decimal c = -1;
            //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
            //这里是针对管理端以及数据库恢复的成交数据
            //注意这里需要使用f.UnsignedSize 否则计算出来的手续费有可能是负数
            if (aftersize > beforesize)//成交后持仓数量大于成交前数量 开仓或者加仓
            {
                if (f.Commission < 0)
                {
                    if (sec.EntryCommission < 1)//百分比计算费率
                        c = sec.EntryCommission * f.xprice * f.UnsignedSize * sec.Multiple;
                    else
                        c = sec.EntryCommission * f.UnsignedSize;
                }
            }
            if (aftersize < beforesize)//成交后持仓数量小于成交后数量 平仓或者减仓
            {
                if (f.Commission < 0)
                {
                    if (sec.ExitCommission < 1)//百分比计算费率
                        c = sec.ExitCommission * f.xprice * f.UnsignedSize * sec.Multiple;
                    else
                        c = sec.ExitCommission * f.UnsignedSize;
                }
            }
            //设定手续费
            if (c >= 0)
                f.Commission = c;

            //设定成交的 开 平 加 减 方式
            if (aftersize != beforesize)
            {
                QSEnumPosOperation op = PositionTransaction.GenPositionOperation(beforesize, aftersize);
                f.PositionOperation = op;

                postrans = new PositionTransaction(f.Account, f.symbol, sec.FullName, sec.Multiple, Util.ToDateTime(f.xdate, f.xtime), f.xsize, f.xprice, op, highest, lowest, c);
                _prt.GotPositionTransaction(new PositionTransaction(postrans));
            }

            
            

            


            //当成交信息发送后我们对外触发仓位变化事件
            if (f.PositionOperation == QSEnumPosOperation.EntryPosition)
            {
                //新建持仓
                string key = PositionRound.GetPRKey(newPosition);
                PositionDataPair positiondata = new PositionDataPair(newPosition,_prt[key],sec,oldCost);
                openPositionData.Add(key,positiondata);

                newPosition.Highest = f.xprice;
                newPosition.Lowest = f.xprice;
                //向strategyhistory发送成交信息
                if (SendHistoryPositionDataEvent != null)
                    SendHistoryPositionDataEvent(f, positiondata);

                //向strategy发送扩展的仓位事件
                if (SendEntryPosiitonEvent != null)
                    SendEntryPosiitonEvent(f,positiondata);
            }
            else if (f.PositionOperation == QSEnumPosOperation.ExitPosition)
            {
                string key = PositionRound.GetPRKey(newPosition);
                PositionDataPair positiondata = openPositionData[key];
                positiondata.PositionCost = oldCost;

                openPositionData.Remove(key);

                //向strategyhistory发送成交信息
                if (SendHistoryPositionDataEvent != null)
                    SendHistoryPositionDataEvent(f, positiondata);

                //向strategy发送扩展的仓位事件
                if (SendExitPositionEvent != null)
                    SendExitPositionEvent(f, positiondata);

                //如果是平仓 则需要将持仓过程中的 最高 最低 价格归为
                newPosition.Highest = decimal.MinValue;
                newPosition.Lowest = decimal.MaxValue;
            }
            else
            {
                string key = PositionRound.GetPRKey(newPosition);
                PositionDataPair positiondata = openPositionData[key];
                positiondata.PositionCost = oldCost;

                if (SendHistoryPositionDataEvent != null)
                    SendHistoryPositionDataEvent(f, positiondata);
                
            }

            //向策略发送成交信息
            if (GotFillEvent != null)
                GotFillEvent(f);

        }

        


        public void GotCancel(long val)
        {
            _ordTracker.GotCancel(val);
            if (GotCancelEvent != null)
                GotCancelEvent(val);
        }

        /// <summary>
        /// 记录完委托后 转发一个委托事件
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 记录完取消后 转发一个取消事件
        /// </summary>
        public event LongDelegate GotCancelEvent;

        /// <summary>
        /// 记录完一个成交后 转发一个成交事件
        /// </summary>
        public event FillDelegate GotFillEvent;
    }
}
