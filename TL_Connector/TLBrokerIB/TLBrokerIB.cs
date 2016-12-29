using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using Common.Logging;
using Krs.Ats.IBNet;

namespace Broker.Live
{
    /*
     * IB交易接口
     * 
     * */
    /// <summary>
    /// 净持仓下单模式
    /// 需要结合当前接口的持仓状态将原来的委托进行分拆 改变开平方向，从而达到净持仓的下单目的
    /// 状态一
    /// 持有多头2手，挂单卖开2手，系统判断后 转换成  卖平2手，此时系统可挂平仓单量为0
    /// 此时买开没有问题，
    /// 
    /// 在实盘成交侧 帐户侧委托通过分拆发送或直接发送的模式向接口发单，在发单过程成分帐户侧的交易记录由清算中心记录
    /// 成交侧的委托也需要记录到数据库,在接口加载时从数据库加载成交侧的交易数据
    /// 
    /// </summary>
    public partial class TLBrokerIB : TLBrokerBase,IBroker
    {

        IBClient client = null;
        IdTracker orderIdtk = null;
        int _orderId = 0;
        bool _working = false;
        int NextOrderId
        {
            get
            {

                int id = _orderId;
                _orderId++;
                return id;
            }
        }


        


    }
}
