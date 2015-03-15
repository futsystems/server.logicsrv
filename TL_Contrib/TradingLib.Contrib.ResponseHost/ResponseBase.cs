using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 策略基类
    /// </summary>
    public abstract class ResponseBase:IResponse
    {

        IAccount _account = null;
        /// <summary>
        /// 该策略绑定的分帐户
        /// </summary>
        public IAccount Account { get { return _account; } }

        public void BindAccount(IAccount acc)
        {
            _account = acc;
        }

        /// <summary>
        /// 对应的策略模板ID
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// 策略实例ID
        /// </summary>
        public int InstanceID { get; set; }

        /// <summary>
        /// 策略标题
        /// </summary>
        public virtual string Title
        { 
            get
            {
                return "ResponseBase";
            }
        }

        protected string Programe { get { return string.Format("{0}-{1}", this.GetType().Name, this.InstanceID); } }

        Dictionary<string, Argument> _args = new Dictionary<string, Argument>();

        /// <summary>
        /// 初始化策略参数
        /// </summary>
        /// <param name="args"></param>
        public void InitArgument(Dictionary<string, Argument> args)
        {
            _args = args;
            //调用策略模板维护器设定参数(通过反射设定参数)
            Tracker.ResponseTemplateTracker.SetArgument(this, _args);
        }

        #region 市场外部响应
        public virtual void OnInit()
        { 
            
        }
        /// <summary>
        /// 获得外部市场行情
        /// </summary>
        /// <param name="tick"></param>
        public virtual void OnTick(Tick tick)
        { 
        
        }

        /// <summary>
        /// 获得委托回报
        /// </summary>
        /// <param name="order"></param>
        public virtual void OnOrder(Order order)
        {

        }

        /// <summary>
        /// 获得成交回报
        /// </summary>
        /// <param name="fill"></param>
        public virtual void OnFill(Trade fill)
        {

        }

        /// <summary>
        /// 获得委托取消
        /// </summary>
        /// <param name="oid"></param>
        public virtual void OnCancel(long oid)
        {

        }

        /// <summary>
        /// 定时调度，用于定时撤单或计时类操作 时间周期为50ms
        /// </summary>
        public virtual void OnTimer()
        { 
            
        }
        #endregion

        /// <summary>
        /// 获得该策略所有合约
        /// </summary>
        protected IEnumerable<Order> Orders { get { return this.Account.Orders; } }

        /// <summary>
        /// 获得该策略所有持仓对象
        /// </summary>
        protected IEnumerable<Position> Positions { get { return this.Account.Positions; } }

        /// <summary>
        /// 查找某个委托编号的委托对象
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        protected Order GetOrder(long oid)
        {
            return this.Orders.FirstOrDefault(o => o.id == oid);
        }

        /// <summary>
        /// 查找某个合约的持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        protected Position GetPosition(string symbol, bool side)
        {
            QSEnumPositionDirectionType type = side ? QSEnumPositionDirectionType.Long : QSEnumPositionDirectionType.Short;
            return this.Positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol) && pos.DirectionType == type);
        }


        SymbolBasket _basket = new SymbolBasketImpl();

        /// <summary>
        /// 判断合约是否被注册
        /// </summary>
        /// <param name="symbol"></param>
        public bool IsSymbolRegisted(string symbol)
        {
            return _basket.HaveSymbol(symbol);
        }

        /// <summary>
        /// 清除行情订阅
        /// </summary>
        protected void ClearSymbols()
        {
            _basket.Clear();
        }

        /// <summary>
        /// 订阅行情
        /// </summary>
        /// <param name="symbol"></param>
        protected void RegisterSymbol(string symbol)
        {
            _basket.Add(GetSymbol(symbol));
        }
        
        /// <summary>
        /// 获得某个合约对应的合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected Symbol GetSymbol(string symbol)
        {
            return this.Account.GetSymbol(symbol);
        }

        /// <summary>
        /// 获得某个合约的市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected Tick GetTickSnapshot(string symbol)
        {
            return TLCtxHelper.ModuleDataRouter.GetTickSnapshot(symbol);
        }


        protected void Log(string msg, QSEnumDebugLevel level)
        {
            Util.Log(msg, level, Programe);
        }

        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        protected void SendOrder(Order o)
        {
            o.Account = this.Account.ID;
            TLCtxHelper.ModuleExCore.SendOrder(o);
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="oid"></param>
        protected void CancelOrder(long oid)
        {
            TLCtxHelper.ModuleExCore.CancelOrder(oid);
        }
        /// <summary>
        /// 买入某个合约
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        protected Order Buy(string symbol, int size, decimal price=0)
        {
            Order o = new OrderImpl(symbol, true, size, price, 0, "", Util.ToTLTime(), Util.ToTLDate());
            this.SendOrder(o);
            return o;
        }

        /// <summary>
        /// 卖出某个合约
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        protected Order Sell(string symbol, int size, decimal price = 0)
        {
            Order o = new OrderImpl(symbol, true, size, price, 0, "", Util.ToTLTime(), Util.ToTLDate());
            this.SendOrder(o);
            return o;
        }

        /// <summary>
        /// 买入某个合约，偏离当前几条买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="ticks"></param>
        protected void Buy(string symbol, int size, int ticks)
        { 
            
        }



        protected void Sell(string symbol, int size, int ticks)
        { 
            
        }

        #region 对外操作
        
        #endregion

    }
}
