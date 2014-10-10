using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 所有交易策略的母类
    /// </summary>
    public abstract class StrategyBase:IStrategy
    {

        #region 扩展函数接口 简化策略操作

        #region 获得交易信息
        /// <summary>
        /// 获得某个合约的仓位数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public PositionTracker Positions
        {
            get { return StrategyData.StrategySupport.Positions; }
        }

        public bool HasPosition(string symbol)
        {
            return StrategyData.StrategySupport.HasPosition(symbol);
        }

        public bool HasLongPosition(string symbol)
        {
            return StrategyData.StrategySupport.HasLongPositon(symbol);
        }

        public bool HasShortPosition(string symbol)
        {
            return StrategyData.StrategySupport.HasShortPosition(symbol);
        }

        /// <summary>
        /// 获得某个ID的委托数据
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public Order GetOrder(long orderid)
        {
            return _strategydata.TradingInfoTracker.OrderManager.SentOrder(orderid);
        }
        /// <summary>
        /// 获得某个委托的成交列表
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public List<Trade> GetTrades(long orderid)
        {
            List<Trade> list = new List<Trade>();

            foreach (Trade f in _strategydata.TradingInfoTracker.TradeManager)
            {
                if (f.id == orderid)
                {
                    list.Add(f);
                }
            }
            return list;
        }
        #endregion

        #region 委托/仓位操作

        /// <summary>
        /// 建立多头仓位(市价)
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void EntryLongPosition(string symbol, int size, string comment = "")
        {
            StrategyData.StrategySupport.EntryLongPosition(symbol, size, comment);
        }
        /// <summary>
        /// 建立空头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void EntryShortPosition(string symbol, int size, string comment = "")
        {
            StrategyData.StrategySupport.EntryShortPosition(symbol, size, comment);
        }

        /// <summary>
        /// 市价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        public void BuyMarket(string symbol,int size,string comment="")
        {
            StrategyData.StrategySupport.BuyMarket(symbol, size, comment);
        }
        /// <summary>
        /// 市价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        public void SellMarket(string symbol, int size,string comment="")
        {
            StrategyData.StrategySupport.SellMarket(symbol, size, comment);
        }
        /// <summary>
        /// 限价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        public void BuyLimit(string symbol, int size, double price,string comment="")
        {
            StrategyData.StrategySupport.BuyLimit(symbol, size, price,comment);
        }


        /// <summary>
        /// 限价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        public void SellLimit(string symbol, int size, double price,string comment="")
        {
            StrategyData.StrategySupport.SellLimit(symbol, size, price, comment);
        }
        /// <summary>
        /// 追价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="stop"></param>
        public void BuyStop(string symbol, int size, double stop,string comment="")
        {
            StrategyData.StrategySupport.BuyStop(symbol, size, stop, comment);
        }
        /// <summary>
        /// 追价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="stop"></param>
        public void SellStop(string symbol, int size, double stop,string comment="")
        {
            StrategyData.StrategySupport.SellStop(symbol, size, stop, comment);
        }
        /// <summary>
        /// 平掉某个合约的所有仓位
        /// </summary>
        /// <param name="symbol"></param>
        public void FlatPosition(string symbol,string comment="")
        {
            StrategyData.StrategySupport.FlatPosition(symbol,comment);
        }

        /// <summary>
        /// 平掉所有仓位
        /// </summary>
        public void FlatAllPositions()
        {
            StrategyData.StrategySupport.FlatAllPositions();
        }

        #endregion


        #endregion

        #region 外部数据对策的数据驱动

        public int CurrentDate = 0;
        public int CurrentTime = 0;
        /// <summary>
        /// 获得Tick数据
        /// </summary>
        /// <param name="tick"></param>
        public virtual  void GotTick(Security sec,Tick tick,Bar partialbar)
        {
            //记录当前Tick时间
            CurrentDate = tick.date;
            //StrategyData.TickDate = tick.date;
            CurrentTime = tick.time;
            //StrategyData.TickTime = tick.time;
        }

        /// <summary>
        /// 获得新生成的Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public virtual  void GotBar(Bar bar)
        { 
        
        
        }
        /// <summary>
        /// 获得委托数据
        /// </summary>
        /// <param name="order"></param>
        public virtual void GotOrder(Order order)
        { 
        

        }
        /// <summary>
        /// 获得成交数据
        /// </summary>
        /// <param name="fill"></param>
        public virtual  void GotFill(Trade fill)
        { 
        
        }
        /// <summary>
        /// 获得委托取消数据
        /// </summary>
        /// <param name="orderid"></param>
        public virtual  void GotOrderCancel(long orderid)
        { 
        
        }

        public virtual void GotEntryPosition(Trade fll,PositionDataPair data)
        { 
        
        }
        public virtual void GotExitPosiiton(Trade fill,PositionDataPair data)
        { 
        
        }
        /// <summary>
        /// 获得其他信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="msgid"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        { 
        
        }

        #endregion


        #region stratebase对外触发的事件,用于将策略对外部的操作转发到对应的接口上去
        //策略系统对外触发的事件 策略通过数据处理后形成对外部的操作
        //public event DebugDelegate SendDebugEvent;//日志输出
        public void Print(string msg)
        {
            StrategyData.StrategySupport.Print(msg);
        }
        
        public void SendOrder(Order o)
        {
            StrategyData.StrategySupport.SendOrder(o);
        }
        
        public void CancelOrder(long val)
        {
            //if (SendCancelEvent != null)
            //     SendCancelEvent(val,11);
            StrategyData.StrategySupport.CancelOrder(val);
        }
        /*
        public event MessageDelegate SendMessageEvent;//发送其他信息
        public void SendMessage()
        {
            if (SendMessageEvent != null)
            {
                //SendMessageEvent(type
            }
        }**/

        #endregion


        /// <summary>
        /// 策略运行数据,所有策略在改数据环境中运行
        /// </summary>
        IStrategyData _strategydata;
        /// <summary>
        /// 运行策略
        /// </summary>
        /// <param name="basedata"></param>
        public  virtual void Start(IStrategyData basedata)
        {
            _strategydata = basedata;

            this.Start();
        }

        public virtual void Start()
        {

        }

 

 



        /// <summary>
        /// 停止运行策略
        /// </summary>
        public virtual void Stop()
        {
            
        }

        #region 属性
        public ITBars Bars { get { return _strategydata.Bars; } }
        public ChartObjectManager ChartObjects { get; set;}
        /// <summary>
        /// 指标管理器
        /// </summary>
        public IndicatorCollections Indicators { get { return _strategydata.Indicators; } }
        //public PositionManager PositionManager { get; set;}
        //public SeriesManager Series { get; set;}

        /// <summary>
        /// 获得策略设置 所交易的合约
        /// </summary>
        public List<Security> Symbols { get { return new List<Security>(_strategydata.Symbols); } }
        /// <summary>
        /// 获得全局StraegyData数据
        /// </summary>
        public IStrategyData StrategyData { get { return _strategydata; } }

        /// <summary>
        /// 获得策略参数
        /// </summary>
        public StrategyParameters StrategyParameters { get { return _strategydata.StrategyParameters; } }



        #endregion

    }
}
