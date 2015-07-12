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
    /// 跟单策略对象
    /// 通过跟单策略配置生成跟单策略实体进行运行
    /// 类似与传统的量化 响应市场行情通过运算后输出市场操作
    /// </summary>
    public partial class FollowStrategy
    {
        /// <summary>
        /// 数据库统一编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 策略编号
        /// </summary>
        public string InstanceID{ get; set; }

        /// <summary>
        /// 发单账户ID
        /// </summary>
        public string AccountID{get;set;}

        /// <summary>
        /// 跟单策略配置
        /// </summary>
        public FollowStrategyConfig Config { get; set; }

        /// <summary>
        /// 跟单账户对象
        /// 封装了底层下单账户IAccount已经相关操作接口
        /// </summary>
        FollowAccount followAccount=null;
        SignalTracker signalTracker = null;
        OrderSourceTracker sourceTracker = null;

        public void Init()
        {
            //初始化下单账户
            followAccount = FollowAccount.CreateFollowAccount(this.AccountID);
            if (followAccount == null)
            { 
                
            }

            //初始化事件绑定 用于获得市场含情以及对应下单账户的委托和成交回报
            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(EventIndicator_GotTickEvent);
            followAccount.GotFillEvent += new FillDelegate(followAccount_GotFillEvent);
            followAccount.GotOrderEvent += new OrderDelegate(followAccount_GotOrderEvent);

            //初始化信号维护器 并加载设置的信号
            signalTracker = new SignalTracker();
        }

        




        void EventIndicator_GotTickEvent(Tick t)
        {
            throw new NotImplementedException();
        }

        



        ConcurrentDictionary<string, ISignal> signalMap = new ConcurrentDictionary<string, ISignal>();


        ISignal GetSignal(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            ISignal signal = null;
            if (signalMap.TryGetValue(token, out signal))
            {
                return signal;
            }
            return null;
        }

        /// <summary>
        /// 策略实例是否包含信号Signal
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        bool HaveSignal(ISignal signal)
        {
            if (string.IsNullOrEmpty(signal.Token)) return false;
            if (signalMap.Keys.Contains(signal.Token)) return true;
            return false;
        }


        /// <summary>
        /// 添加信号到策略对象
        /// </summary>
        public void AppendSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }
            //1.添加信号到map
            if (HaveSignal(signal))
            {
                return;
            }

            signalMap.TryAdd(signal.Token, signal);

            //2.绑定Signal事件
            signal.GotFillEvent += new FillDelegate(OnSignalFillEvent);
            signal.GotOrderEvent += new OrderDelegate(OnSignalOrderEvent);
            signal.GotPositionEvent += new Action<ISignal, Trade, IPositionEvent>(OnSignalPositionEvent);
        }

       



        /// <summary>
        /// 从策略对象删除信号
        /// </summary>
        /// <param name="siginal"></param>
        public void RemoveSignal(ISignal signal)
        {
            if (signal == null)
            {
                return;
            }
            //1.删除信号
            if (!HaveSignal(signal))
            {
                return;
            }

            ISignal target=null;
            //2.解绑Signal事件
            signal.GotFillEvent -= new FillDelegate(OnSignalFillEvent);
            signal.GotOrderEvent -= new OrderDelegate(OnSignalOrderEvent);
            signal.GotPositionEvent -= new Action<ISignal, Trade, IPositionEvent>(OnSignalPositionEvent);

            signalMap.TryRemove(signal.Token, out target);
        }




    }
}
