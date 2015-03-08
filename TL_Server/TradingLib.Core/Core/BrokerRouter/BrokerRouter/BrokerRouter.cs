using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

using System.Threading;

namespace TradingLib.Core
{

    /* 关于无法平仓的死循环逻辑
     * 当前帐户持有仓位,同时下有反向委托，系统识别该委托为平仓
     * 该委托由于某些原因一直处于place submited状态 没有正常提交到broker
     * broker正常send的委托均处于open,partfilled等状态
     * 
     * A.客户端下市价平仓时【postflag为unknow自动识别】,br进行持仓计算，会将place submited的委托计入未成交平仓委托，broker会自动撤单然后当所有未成交平仓委托撤单后再提交最新的平仓委托
     * 而由于place submited委托不在broker管理访问，导致用于无法撤出，同时最新待提交的为委托处于place状态一直没有正式通过broker.send进行发送，而且每提交一次委托 未成交队列中就增加一条委托记录
     * 
     * B.客户端下市价平仓时【postflag为close强制】，br进行持仓计算，会将place submited的委托计入未成交平仓委托，导致返回可平持仓数量不足。
     * 
     * 因此在msgexch中有单独的定时程序进行超时 place submit状态的委托进行处理，防止帐户陷入死循环状态。
     * 
     * 理论上place submit的委托是不会长时间存在的，正常工作状态下这些状态只会存在很短的时间。
     * 
     * 
     * 
     * 
     * 
     * 
     * */
    /// <summary>
    /// 成交路由中心
    /// 用于承接系统的交易,提交委托,取消委托等
    /// 1.交易路由中心,将进来的委托按照一定规则找到对应的交易接口并通过交易接口进行对应的交易操作 
    /// 2.交易路由收到交易接口返回的回报时,将这些回报转发给TradingServer
    /// 3.交易路由内置组合委托中心,用于提供交易所所不支持的委托类型
    /// 4.交易路由中心内置委托智能处理器,该处理器用于处理自动撤单(市价平仓),反手等事务
    /// 
    /// 5.交易路由中心操作接口是线程安全的,内置2个队列一个队列用于处理交易系统提交上来的委托以及其他操作,排队处理
    ///   另一个队列处理成交接口返回过来的回报,将这些回报逐一返回给交易系统
    /// </summary>
    public partial class BrokerRouter:BaseSrvObject,IBrokerRouter
    {
        public const string PROGRAM = "BrokerRouter";




        TIFEngine _tifengine;
        private ClearCentre _clearCentre;
        //private DataFeedRouter _datafeedRouter;

        OrderTransactionHelper _ordHelper;

        /// <summary>
        /// 数据路由,用于让交易接口得到需要的tick数据等信息
        /// </summary>
        //public DataFeedRouter DataFeedRouter { get { return _datafeedRouter; } 
        //    set { 
        //        _datafeedRouter = value;
        //        //_datafeedRouter.GotTickEvent += new TickDelegate(_tifengine.GotTick);
                
        //           } }

        public BrokerRouter(ClearCentre c):base("BrokerRouter")
        {
            _clearCentre = c;
            _tifengine = new TIFEngine();
            //_tifengine.SendDebugEvent +=new DebugDelegate(msgdebug);
            //_tifengine.SendOrderEvent += new OrderDelegate(route_SendOrder);
            _tifengine.SendCancelEvent += new LongDelegate(RouterCancelOrder);

            _ordHelper = new OrderTransactionHelper("BrokerRouter");
            //_ordHelper.SendDebugEvent +=new DebugDelegate(msgdebug);
            //_ordHelper.SendOrderEvent += new OrderDelegate(broker_sendorder);

            //初始化委托分拆器
            InitSplitTracker();

            StartProcessMsgOut();
        }

        event TickDelegate GotTickEvent;
        public void GotTick(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);
            
        }


        public void Reset()
        {
            _ordHelper.Clear();
            _tifengine.Clear();
            //重启模拟交易 
            //IBroker b = GetSimBroker();
            //if (b == null) return;
            //b.Stop();
            //Thread.Sleep(1000);
            //b.Start();

        }

        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            StartProcessMsgOut();
            _ordHelper.Start();
            _tifengine.Start();
            ResumeRouterOrder();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            _ordHelper.Stop();
            _tifengine.Stop();
            StopProcessMsgOut();
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();

        }
    }
}
