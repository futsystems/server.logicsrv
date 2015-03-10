using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ExCore : BaseSrvObject, IExCore
    {
        public string CoreId { get { return this.PROGRAME; } }

        protected ConfigDB _cfgdb;

        string commentFilled = string.Empty;
        string commentPartFilled = string.Empty;
        string commentCanceled = string.Empty;
        string commentPlaced = string.Empty;
        string commentSubmited = string.Empty;
        string commentOpened = string.Empty;

        //ConfigDB ConfigDB { protected get { return _cfgdb; } }

        public ExCore(string Name)
            : base(Name)
        {

            //1.加载配置文件
            _cfgdb = new ConfigDB(this.PROGRAME);

            #region 委托流水号
            if (!_cfgdb.HaveConfig("StartOrderSeq"))
            {
                _cfgdb.UpdateConfig("StartOrderSeq", QSEnumCfgType.Int, 1000, "默认起始委托流水号");
            }
            _startOrderSeq = _cfgdb["StartOrderSeq"].AsInt();

            if (!_cfgdb.HaveConfig("RandomOrderSeqEnable"))
            {
                _cfgdb.UpdateConfig("RandomOrderSeqEnable", QSEnumCfgType.Bool, true, "启用委托流水号随机");
            }
            _enbaleRandomOrderSeq = _cfgdb["RandomOrderSeqEnable"].AsBool();

            if (!_cfgdb.HaveConfig("OrderSeqStepLow"))
            {
                _cfgdb.UpdateConfig("OrderSeqStepLow", QSEnumCfgType.Int, 50, "委托流水号随机步长低值");
            }
            _stepOrderSeqLow = _cfgdb["OrderSeqStepLow"].AsInt();

            if (!_cfgdb.HaveConfig("OrderSeqStepHigh"))
            {
                _cfgdb.UpdateConfig("OrderSeqStepHigh", QSEnumCfgType.Int, 100, "委托流水号随机步长高值");
            }
            _stepOrderSeqHigh = _cfgdb["OrderSeqStepHigh"].AsInt();
            #endregion

            #region 成交编号
            if (!_cfgdb.HaveConfig("StartTradeID"))
            {
                _cfgdb.UpdateConfig("StartTradeID", QSEnumCfgType.Int, 2000, "默认起始成交编号");
            }
            _startTradeID = _cfgdb["StartTradeID"].AsInt();

            if (!_cfgdb.HaveConfig("RandomTradeIDEnable"))
            {
                _cfgdb.UpdateConfig("RandomTradeIDEnable", QSEnumCfgType.Bool, true, "启用成交编号随机");
            }
            _enbaleRandomTradeID = _cfgdb["RandomTradeIDEnable"].AsBool();

            if (!_cfgdb.HaveConfig("TradeIDStepLow"))
            {
                _cfgdb.UpdateConfig("TradeIDStepLow", QSEnumCfgType.Int, 50, "成交编号号随机步长低值");
            }
            _stepTradeIDLow = _cfgdb["TradeIDStepLow"].AsInt();

            if (!_cfgdb.HaveConfig("TradeIDStepHigh"))
            {
                _cfgdb.UpdateConfig("TradeIDStepHigh", QSEnumCfgType.Int, 150, "成交编号随机步长高值");
            }
            _stepTradeIDHigh = _cfgdb["TradeIDStepHigh"].AsInt();
            #endregion

            if (!_cfgdb.HaveConfig("CommentFilled"))
            {
                _cfgdb.UpdateConfig("CommentFilled", QSEnumCfgType.String, "全部成交", "全部成交备注");
            }
            commentFilled = _cfgdb["CommentFilled"].AsString();

            if (!_cfgdb.HaveConfig("CommentPartFilled"))
            {
                _cfgdb.UpdateConfig("CommentPartFilled", QSEnumCfgType.String, "部分成交", "部分成交备注");
            }
            commentPartFilled = _cfgdb["CommentPartFilled"].AsString();

            if (!_cfgdb.HaveConfig("CommentCanceled"))
            {
                _cfgdb.UpdateConfig("CommentCanceled", QSEnumCfgType.String, "委托已取消", "取消委托备注");
            }
            commentCanceled = _cfgdb["CommentCanceled"].AsString();

            if (!_cfgdb.HaveConfig("CommentPlaced"))
            {
                _cfgdb.UpdateConfig("CommentPlaced", QSEnumCfgType.String, "已接受", "提交委托备注");
            }
            commentPlaced = _cfgdb["CommentPlaced"].AsString();

            if (!_cfgdb.HaveConfig("CommentSubmited"))
            {
                _cfgdb.UpdateConfig("CommentSubmited", QSEnumCfgType.String, "已提交", "发送委托备注");
            }
            commentSubmited = _cfgdb["CommentSubmited"].AsString();

            if (!_cfgdb.HaveConfig("CommentOpened"))
            {
                _cfgdb.UpdateConfig("CommentOpened", QSEnumCfgType.String, "已经报入", "取消委托备注");
            }
            commentOpened = _cfgdb["CommentOpened"].AsString();


            int maxorderseq = ORM.MTradingInfo.MaxOrderSeq();
            _maxOrderSeq = maxorderseq > _startOrderSeq ? maxorderseq : _startOrderSeq;
            debug("Max OrderSeq:" + _maxOrderSeq, QSEnumDebugLevel.INFO);

            int maxtradeid = ORM.MTradingInfo.MaxTradeID();
            _maxTradeID = maxtradeid > _startTradeID ? maxtradeid : _startTradeID;
            debug("Max TradeID:" + _maxTradeID, QSEnumDebugLevel.INFO);



            //订阅路由侧事件
            TLCtxHelper.EventRouter.GotTickEvent += new TickDelegate(this.OnTickEvent);

            TLCtxHelper.EventRouter.GotOrderEvent += new OrderDelegate(this.OnOrderEvent);
            TLCtxHelper.EventRouter.GotFillEvent += new FillDelegate(this.OnFillEvent);
            TLCtxHelper.EventRouter.GotCancelEvent += new LongDelegate(this.OnCancelEvent);

            TLCtxHelper.EventRouter.GotOrderErrorEvent += new OrderErrorDelegate(this.OnOrderErrorEvent);
            TLCtxHelper.EventRouter.GotOrderActionErrorEvent += new OrderActionErrorDelegate(this.OnOrderActionErrorEvent);


        }


        



        #region 委托编号 成交编号生成
        //委托编号生成器
        IdTracker _orderIDTracker = new IdTracker();

        //委托流水号
        int _maxOrderSeq = 0;//当前最大委托流水
        int _startOrderSeq = 0;//起始流水号
        bool _enbaleRandomOrderSeq = false;//是否随机委托流水号
        int _stepOrderSeqLow = 1;//步长最小值
        int _stepOrderSeqHigh = 10;//步长最大值
        Random _orderRandom = new Random(100);

        object _orderseqobj = new object();
        /// <summary>
        /// 获得委托流水号
        /// </summary>
        public int NextOrderSeq
        {

            get
            {
                lock (_orderseqobj)
                {

                    if (_enbaleRandomOrderSeq)
                    {
                        _maxOrderSeq += _orderRandom.Next(_stepOrderSeqLow, _stepOrderSeqHigh);
                        return _maxOrderSeq;
                    }
                    else
                    {
                        _maxOrderSeq += 1;
                        return _maxOrderSeq;
                    }
                }
            }
        }

        //成交流水号
        int _maxTradeID = 0;//当前最大委托流水
        int _startTradeID = 0;//起始流水号
        bool _enbaleRandomTradeID = false;//是否随机委托流水号
        int _stepTradeIDLow = 1;//步长最小值
        int _stepTradeIDHigh = 10;//步长最大值
        Random _tradeRandom = new Random(200);

        object _tradeseqobj = new object();
        /// <summary>
        /// 获得委托流水号
        /// </summary>
        public int NextTradeID
        {

            get
            {
                lock (_tradeseqobj)
                {

                    if (_enbaleRandomTradeID)
                    {
                        _maxTradeID += _tradeRandom.Next(_stepTradeIDLow, _stepTradeIDHigh);
                        return _maxTradeID;
                    }
                    else
                    {
                        _maxTradeID += 1;
                        return _maxTradeID;
                    }
                }
            }
        }

        /// <summary>
        /// 绑定唯一的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void AssignOrderID(ref Order o)
        {
            if (o.id <= 0)
                o.id = _orderIDTracker.AssignId;
            //获得本地递增流水号
            if (o.OrderSeq <= 0)
                o.OrderSeq = this.NextOrderSeq;
        }

        public void AssignTradeID(ref Trade f)
        {
            //系统本地给成交赋日内唯一流水号 成交端的TradeID由接口负责
            f.TradeID = this.NextTradeID.ToString();
        }

        #endregion



    }
}
