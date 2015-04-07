using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 简单路由
    /// 分帐户侧提交委托，通过绑定的通道进行下单
    /// 通道侧返回的委托，统一以对应的分帐户对系统内进行回报
    /// 
    /// </summary>
    public partial class BrokerRouterPassThrough:BaseSrvObject,IModuleBrokerRouter
    {

        const string CoreName = "BrokerRouterPassThrough";
        public string CoreId { get { return this.PROGRAME; } }

        ConfigDB _cfgdb;
        int _withdrawStart = 90000;
        int _withdrawEnd = 153000;

        int _nightStart = 210000;
        int _nightEnd = 23000;

        /// <summary>
        /// 交易帐户与通道的映射关系维护器
        /// </summary>
        //AccountConnectorTracker _acctokentracker = null;
        public BrokerRouterPassThrough()
            : base(BrokerRouterPassThrough.CoreName)
        {

            //_acctokentracker = new AccountConnectorTracker();
            //1.加载配置文件
            _cfgdb = new ConfigDB(BrokerRouterPassThrough.CoreName);

            //出金开始时间
            if (!_cfgdb.HaveConfig("WithDrawStartTime"))
            {
                _cfgdb.UpdateConfig("WithDrawStartTime", QSEnumCfgType.Int, 90000, "出金开始时间");
            }
            _withdrawStart = _cfgdb["WithDrawStartTime"].AsInt();

            //出金结束时间
            if (!_cfgdb.HaveConfig("WithDrawEndTime"))
            {
                _cfgdb.UpdateConfig("WithDrawEndTime", QSEnumCfgType.Int, 153000, "出金结束时间");
            }
            _withdrawEnd = _cfgdb["WithDrawEndTime"].AsInt();

            //夜盘交易开始
            if (!_cfgdb.HaveConfig("NightStartTime"))
            {
                _cfgdb.UpdateConfig("NightStartTime", QSEnumCfgType.Int, 210000, "夜盘开始时间");
            }
            _nightStart = _cfgdb["NightStartTime"].AsInt();

            //夜盘交易结束
            if (!_cfgdb.HaveConfig("NightEndTime"))
            {
                _cfgdb.UpdateConfig("NightEndTime", QSEnumCfgType.Int, 23000, "夜盘结束时间");
            }
            _nightEnd = _cfgdb["NightEndTime"].AsInt();


            StartProcessMsgOut();
            TLCtxHelper.EventAccount.AccountInactiveEvent += new AccoundIDDel(EventAccount_AccountInactiveEvent);
        }

        /// <summary>
        /// 交易帐户冻结
        /// 
        /// </summary>
        /// <param name="account"></param>
        void EventAccount_AccountInactiveEvent(string account)
        {
            
        }


        event TickDelegate GotTickEvent;
        public void GotTick(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);

        }


        public void Stop()
        { 
        
        }

        public void Start()
        { 
        
        }
        
    }
}
