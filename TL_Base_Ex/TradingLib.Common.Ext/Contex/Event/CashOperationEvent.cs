using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Common
{
    /// <summary>
    /// 出入金操作事件
    /// 包含了一个出入金请求的过程
    /// </summary>
    public enum QSEnumCashOpEventType
    { 
        [Description("提交")]
        Request,
        [Description("确认")]
        Confirm,
        [Description("拒绝")]
        Reject,
        [Description("取消")]
        Cancel,
    }


    public class CashOperationEventArgs : EventArgs
    {
        public CashOperationEventArgs(QSEnumCashOpEventType eventType,JsonWrapperCashOperation cashOperation)
        {
            this.EventType = eventType;
            this.CashOperation = cashOperation;
        }

        public QSEnumCashOpEventType EventType { get; set; }

        public JsonWrapperCashOperation CashOperation { get; set; }
    }

    /// <summary>
    /// 暴露底层资金处理事件
    /// </summary>
    public class CashOperationEvent
    {
        /// <summary>
        /// 出入金请求 状态改变22:59
        /// </summary>
        public event EventHandler<CashOperationEventArgs> CashOperationRequest = delegate { };

        /// <summary>
        /// 通过事件中继 向系统推送资金处理事件
        /// 在每个调用到出入金操作状态改变的地方进行触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventType"></param>
        /// <param name="cashOperation"></param>
        public void FireCashOperation(object sender, QSEnumCashOpEventType eventType, JsonWrapperCashOperation cashOperation)
        {
            CashOperationEventArgs arg = new CashOperationEventArgs(eventType,cashOperation);
            CashOperationRequest(sender, arg);
        }

    }
}
