using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.Common;

using System.Diagnostics;//记得加入此引用
using System.Collections.Concurrent;
using TradingLib.API;
using System.Reflection;
using System.Threading;

namespace TradingLib.Core
{
    

    //服务端风险控制模块,根据每个账户的设定，实时的检查Order是否符合审查要求予以确认或者决绝
    public partial class RiskCentre : BaseSrvObject, IModuleRiskCentre
    {
        const string CoreName = "RiskCentre";
        public string CoreId { get { return CoreName; } }

        ConfigDB _cfgdb;

        bool _marketopencheck = true;
        public bool MarketOpenTimeCheck { get { return _marketopencheck; } }

        int _orderlimitsize = 10;
        string commentNoPositionForFlat = "无可平持仓";
        string commentOverFlatPositionSize = "可平持仓数量不足";

        bool auctionEnable = false;
        bool _haltEnable = false;
        bool _cffexLimit = false;//中金所限制


        int _m1left = 30;//M1下单剩余时间
        int _m2left = 60;//M2下单剩余时间
        int _m5left = 150;//M5下单剩余时间
        int _m10left = 300;//M10下单剩余时间
        int _m15left = 450;//M15下单剩余时间
        int _m30left = 900;//M30下单剩余时间
        int _m60left = 1800;//M60下单剩余时间

        bool _enableStkT0 = false;

        public RiskCentre():base(CoreName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(RiskCentre.CoreName);
            if (!_cfgdb.HaveConfig("MarketOpenTimeCheck"))
            {
                _cfgdb.UpdateConfig("MarketOpenTimeCheck", QSEnumCfgType.Bool,"True", "是否进行时间检查,交易日/合约交易时间段等");
            }
            //是否执行合约开市检查
            _marketopencheck = _cfgdb["MarketOpenTimeCheck"].AsBool();
            

            if (!_cfgdb.HaveConfig("OrderLimitSize"))
            {
                _cfgdb.UpdateConfig("OrderLimitSize", QSEnumCfgType.Int, 100, "单笔委托最大上限");
            }
            //最大委托数量
            _orderlimitsize = _cfgdb["OrderLimitSize"].AsInt();

            if (!_cfgdb.HaveConfig("CommentNoPositionForFlat"))
            {
                _cfgdb.UpdateConfig("CommentNoPositionForFlat", QSEnumCfgType.String,"无可平持仓", "无可平持仓消息");
            }
            commentNoPositionForFlat = _cfgdb["CommentNoPositionForFlat"].AsString();

            if (!_cfgdb.HaveConfig("CommentOverFlatPositionSize"))
            {
                _cfgdb.UpdateConfig("CommentOverFlatPositionSize", QSEnumCfgType.String, "可平持仓数量不足", "可平持仓数量不足消息");
            }
            commentOverFlatPositionSize = _cfgdb["CommentOverFlatPositionSize"].AsString();

            if (!_cfgdb.HaveConfig("FlatSendOrderFreq"))
            {
                _cfgdb.UpdateConfig("FlatSendOrderFreq", QSEnumCfgType.Int, 3, "强平重试时间间隔");
            }
            SENDORDERDELAY = _cfgdb["FlatSendOrderFreq"].AsInt();

            if (!_cfgdb.HaveConfig("FlatSendOrderRetryNum"))
            {
                _cfgdb.UpdateConfig("FlatSendOrderRetryNum", QSEnumCfgType.Int, 3, "强平重试次数");
            }
            SENDORDERRETRY = _cfgdb["FlatSendOrderRetryNum"].AsInt();

            if (!_cfgdb.HaveConfig("AuctionEnable"))
            {
                _cfgdb.UpdateConfig("AuctionEnable", QSEnumCfgType.Bool,false, "集合竞价");
            }
            auctionEnable = _cfgdb["AuctionEnable"].AsBool();

            if (!_cfgdb.HaveConfig("HaltEnable"))
            {
                _cfgdb.UpdateConfig("HaltEnable", QSEnumCfgType.Bool, false, "股指熔断");
            }
            _haltEnable = _cfgdb["HaltEnable"].AsBool();

            if (!_cfgdb.HaveConfig("CFFEXLimit"))
            {
                _cfgdb.UpdateConfig("CFFEXLimit", QSEnumCfgType.Bool, false, "中金所限制");
            }
            _cffexLimit = _cfgdb["CFFEXLimit"].AsBool();



            if (!_cfgdb.HaveConfig("BO-M1Left"))
            {
                _cfgdb.UpdateConfig("BO-M1Left", QSEnumCfgType.Int, 30, "M1下单剩余时间");
            }
            _m1left = _cfgdb["BO-M1Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M2Left"))
            {
                _cfgdb.UpdateConfig("BO-M2Left", QSEnumCfgType.Int, 60, "M2下单剩余时间");
            }
            _m2left = _cfgdb["BO-M2Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M5Left"))
            {
                _cfgdb.UpdateConfig("BO-M5Left", QSEnumCfgType.Int, 150, "M5下单剩余时间");
            }
            _m5left = _cfgdb["BO-M5Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M10Left"))
            {
                _cfgdb.UpdateConfig("BO-M10Left", QSEnumCfgType.Int, 300, "M10下单剩余时间");
            }
            _m10left = _cfgdb["BO-M10Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M15Left"))
            {
                _cfgdb.UpdateConfig("BO-M15Left", QSEnumCfgType.Int, 450, "M15下单剩余时间");
            }
            _m15left = _cfgdb["BO-M15Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M30Left"))
            {
                _cfgdb.UpdateConfig("BO-M30Left", QSEnumCfgType.Int, 900, "M30下单剩余时间");
            }
            _m30left = _cfgdb["BO-M30Left"].AsInt();

            if (!_cfgdb.HaveConfig("BO-M60Left"))
            {
                _cfgdb.UpdateConfig("BO-M60Left", QSEnumCfgType.Int, 1800, "M60下单剩余时间");
            }
            _m60left = _cfgdb["BO-M60Left"].AsInt();

            if (!_cfgdb.HaveConfig("EnableStkT0"))
            {
                _cfgdb.UpdateConfig("EnableStkT0", QSEnumCfgType.Bool,false, "股票交易T+0");
            }
            _enableStkT0 = _cfgdb["EnableStkT0"].AsBool();

            //加载风空规则
            LoadRuleClass();

            //初始化日内平仓任务
            InitFlatTask();

            //订阅交易信息
            TLCtxHelper.EventIndicator.GotOrderEvent += new OrderDelegate(this.GotOrder);
            TLCtxHelper.EventIndicator.GotOrderErrorEvent += new OrderErrorDelegate(this.GotOrderError);

            //交易帐户激活
            TLCtxHelper.EventAccount.AccountActiveEvent += new Action<IAccount>(this.ResetRuleSet);

            //结算重置
            TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            this.Reset();
            
        }


        #region 重置
        /// <summary>
        /// 风空中心重置风控规则
        /// </summary>
        public void Reset()
        {
            logger.Info("风控中心重置");
            //清空强平任务队列
            posflatlist.Clear();

            //加载所有风控规则
            LoadRuleItemAll();
        }

        #endregion

        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            LoadRuleItemAll();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
        }
    }


    
    


}
