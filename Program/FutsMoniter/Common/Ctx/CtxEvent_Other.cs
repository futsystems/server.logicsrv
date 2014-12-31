using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {

        public event Action<RspInfo> GotRspInfoEvent;

        /// <summary>
        /// 基础数据加载完毕
        /// </summary>
        public event VoidDelegate GotBasicInfoDoneEvent;

        /// <summary>
        /// 获得域更新事件
        /// </summary>
        public event Action<DomainImpl> GotDomainEvent;

    }
}
