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

        /// <summary>
        /// 跟单账户对象
        /// 封装了底层下单账户IAccount已经相关操作接口
        /// </summary>
        FollowAccount followAccount=null;
        //SignalTracker signalTracker = null;
        OrderSourceTracker sourceTracker = null;

        ILog logger = null;
        ConcurrentDictionary<int, ISignal> signalMap = null;
        SignalFollowItemTracker followitemtracker = null;


        public FollowStrategy(FollowStrategyConfig cfg)
        {
            logger = LogManager.GetLogger("FollowStrategy:"+cfg.Token);
            this.Config = cfg;

            followitemtracker = new SignalFollowItemTracker();
            sourceTracker = new OrderSourceTracker();

            this.WorkState = QSEnumFollowWorkState.Shutdown;//初始状态处于停止状态
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.Token, this.ID);
        }
        public void Init()
        {

            //1.初始化下单账户
            followAccount = FollowAccount.CreateFollowAccount(this.Account);
            if (followAccount == null)
            {
                logger.Warn(string.Format("跟单账户:{0}不存在,策略无效直接返回", this.Account));
                return;
            }

            //2.绑定行情事件 获得市场含情
            logger.Info("bind market data event");
            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(EventIndicator_GotTickEvent);

            //3.跟单账户交易事件,获得对应下单账户的委托和成交回报
            logger.Info("绑定跟单帐户交易事件");
            followAccount.GotFillEvent += new FillDelegate(followAccount_GotFillEvent);
            followAccount.GotOrderEvent += new OrderDelegate(followAccount_GotOrderEvent);

            //4.绑定信号事件 初始化信号维护器 并加载设置的信号;
            logger.Info("绑定信号源交易事件");
            //从维护器中获得策略信号map
            signalMap = FollowTracker.SignalTracker.GetStrategySignals(this.ID);

            //绑定信号源事件
            foreach (var signal in signalMap.Values)
            {
                BindSignal(signal);
                //为每个信号初始化跟单项目维护器
                followitemtracker.InitFollowItemTracker(signal);
            }
        }

        




        void EventIndicator_GotTickEvent(Tick t)
        {
            //throw new NotImplementedException();
        }

        //ISignal GetSignal(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return null;
        //    }

        //    ISignal signal = null;
        //    if (signalMap.TryGetValue(token, out signal))
        //    {
        //        return signal;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 策略实例是否包含信号Signal
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        //bool HaveSignal(ISignal signal)
        //{
        //    if (string.IsNullOrEmpty(signal.Token)) return false;
        //    if (signalMap.Keys.Contains(signal.Token)) return true;
        //    return false;
        //}


        /// <summary>
        /// 添加信号到策略对象
        /// </summary>
        public void BindSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }

            //2.绑定Signal事件
            signal.GotFillEvent += new FillDelegate(OnSignalFillEvent);
            signal.GotOrderEvent += new OrderDelegate(OnSignalOrderEvent);
            signal.GotPositionEvent += new Action<ISignal, Trade, IPositionEvent>(OnSignalPositionEvent);
        }

       



        /// <summary>
        /// 从策略对象删除信号
        /// </summary>
        /// <param name="siginal"></param>
        public void UnbindSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }

            //2.解绑Signal事件
            signal.GotFillEvent -= new FillDelegate(OnSignalFillEvent);
            signal.GotOrderEvent -= new OrderDelegate(OnSignalOrderEvent);
            signal.GotPositionEvent -= new Action<ISignal, Trade, IPositionEvent>(OnSignalPositionEvent);
        }




    }
}
