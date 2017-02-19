using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Core
{
    /// <summary>
    /// 跟单策略对象
    /// 通过跟单策略配置生成跟单策略实体进行运行
    /// 类似与传统的量化 响应市场行情通过运算后输出市场操作
    /// </summary>
    public partial class FollowStrategy
    {

        /// <summary>
        /// 从某个跟单配置生成策略对象
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static FollowStrategy CreateStrategy(FollowStrategyConfig cfg)
        {
            FollowStrategy strategy = new FollowStrategy(cfg);
            strategy.Init();

            return strategy;
        }

        /// <summary>
        /// 数据库统一编号
        /// </summary>
        public int ID { get { return this.Config.ID; } }

        /// <summary>
        /// 策略编号
        /// </summary>
        public string Token { get { return this.Config.Token; } }

        /// <summary>
        /// 发单账户ID
        /// </summary>
        public string Account { get { return this.Config.Account; } }


        /// <summary>
        /// 跟单策略工作状态
        /// </summary>
        public QSEnumFollowWorkState WorkState { get; set; }

        /// <summary>
        /// 跟单策略配置
        /// </summary>
        public FollowStrategyConfig Config { get; set; }

        ILog logger = null;

        /// <summary>
        /// 跟单账户对象
        /// 封装了底层下单账户IAccount已经相关操作接口
        /// </summary>
        FollowAccount followAccount=null;
        
        /// <summary>
        /// 委托与跟单项映射关系
        /// </summary>
        OrderSourceTracker sourceTracker = null;
        /// <summary>
        /// 跟单策略信号源
        /// </summary>
        ConcurrentDictionary<int, ISignal> signalMap = null;


        /// <summary>
        /// 跟单项map
        /// </summary>
        ConcurrentDictionary<string,FollowItem> followKeyItemMap = new ConcurrentDictionary<string,FollowItem>();
        /// <summary>
        /// 跟单项与本地键 对应关系 比如开仓成交 在平仓成交信号触发时需要通过对应的开仓成交编号获得对应的跟单项
        /// </summary>
        ConcurrentDictionary<string, FollowItem> localKeyItemMap = new ConcurrentDictionary<string, FollowItem>();

        public FollowItem GetFollowItem(string followkey)
        {
            if (string.IsNullOrEmpty(followkey)) return null;
            FollowItem target = null;
            if (followKeyItemMap.TryGetValue(followkey, out target))
            {
                return target;
            }
            return null;
        }

        /// <summary>
        /// 判定某个委托ID是否已经触发过开仓跟单项了
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsOrderFollowed(long orderid)
        {
            return followKeyItemMap.Values.Any(item => item.SignalTrade != null && item.SignalTrade.id == orderid);
        }

        FollowItem GetEntryFollowItemViaLocalKey(string localKey)
        {
            if (string.IsNullOrEmpty(localKey)) return null;
            FollowItem target = null;
            if (localKeyItemMap.TryGetValue(localKey, out target))
            {
                return target;
            }
            return null;
        }
      

        public FollowStrategy(FollowStrategyConfig cfg)
        {
            logger = LogManager.GetLogger("FollowStrategy:"+cfg.Token);
            this.Config = cfg;
            sourceTracker = new OrderSourceTracker();

            this.WorkState = QSEnumFollowWorkState.Shutdown;//初始状态处于停止状态
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.Token, this.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {

            logger.Info(string.Format("FollowStrategy:{0}-{0} init",this.Config.ID,this.Config.Token));
            //1.初始化下单账户
            followAccount = FollowAccount.CreateFollowAccount(this.Account);
            if (followAccount == null)
            {
                logger.Warn(string.Format("跟单账户:{0}不存在,策略无效直接返回", this.Account));
                return;
            }

            //2.绑定行情事件 获得市场含情
            logger.Info("Wire market data event");
            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(EventIndicator_GotTickEvent);

            //3.跟单账户交易事件,获得对应下单账户的委托和成交回报
            logger.Info("Wire event of followaccount");
            followAccount.GotFillEvent += new FillDelegate(followAccount_GotFillEvent);
            followAccount.GotOrderEvent += new OrderDelegate(followAccount_GotOrderEvent);

            //4.绑定信号事件 初始化信号维护器 并加载设置的信号;
            logger.Info("Wire event of signals");
            //从维护器中获得策略信号map
            signalMap = FollowTracker.SignalTracker.GetStrategySignals(this.ID);

            //绑定信号源事件
            foreach (var signal in signalMap.Values)
            {
                BindSignal(signal);
            }
        }

        




        void EventIndicator_GotTickEvent(Tick t)
        {
            //throw new NotImplementedException();
        }


        /// <summary>
        /// 跟单策略添加信号
        /// </summary>
        /// <param name="signal"></param>
        public void AppendSignal(ISignal signal)
        { 
            //1.绑定信号事件
            BindSignal(signal);
            //2.初始化信号所对应的数据维护器
            //followitemtracker.InitFollowItemTracker(signal);
        
        }

        public void RemoveSignal(ISignal signal)
        { 
            //1.解绑事件绑定
            UnbindSignal(signal);
            //2.平掉将该信号对应的持仓
        }

        /// <summary>
        /// 绑定信号事件
        /// </summary>
        void BindSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }

            //2.绑定Signal事件
            signal.GotFillEvent += new FillDelegate(OnSignalFillEvent);
            signal.GotOrderEvent += new OrderDelegate(OnSignalOrderEvent);
            signal.GotPositionEvent += new Action<ISignal, Trade, PositionEvent>(OnSignalPositionEvent);
        }



        /// <summary>
        /// 从策略对象删除信号
        /// </summary>
        /// <param name="siginal"></param>
        void UnbindSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }

            if (signalMap.Keys.Contains(signal.ID))
            {
                //2.解绑Signal事件
                signal.GotFillEvent -= new FillDelegate(OnSignalFillEvent);
                signal.GotOrderEvent -= new OrderDelegate(OnSignalOrderEvent);
                signal.GotPositionEvent -= new Action<ISignal, Trade, PositionEvent>(OnSignalPositionEvent);
            }
        }




    }
}
