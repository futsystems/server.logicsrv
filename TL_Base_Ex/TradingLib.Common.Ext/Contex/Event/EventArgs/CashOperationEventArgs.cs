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
        public CashOperationEventArgs(QSEnumCashOpEventType eventType, JsonWrapperCashOperation cashOperation)
        {
            this.EventType = eventType;
            this.CashOperation = cashOperation;
        }

        public QSEnumCashOpEventType EventType { get; set; }

        public JsonWrapperCashOperation CashOperation { get; set; }
    }
}
