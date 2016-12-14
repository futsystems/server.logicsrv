using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.XLProtocol
{

    /// <summary>
    /// JsonRequest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonRequest<T>
        where T : IXLField
    {
        public JsonRequest(XLMessageType msgType, T req, int requestID)
        {
            this.MessageType = msgType;
            this.Request = req;
            this.RequestID = requestID;
        }

        /// <summary>
        /// 消息类别
        /// </summary>
        public XLMessageType MessageType { get; set; }

        /// <summary>
        /// 域
        /// </summary>
        public T Request { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID { get; set; }
    }

    /// <summary>
    /// Json通知
    /// </summary>
    public class JsonNotify
    {
        public JsonNotify(XLMessageType msgType,IXLField notify)
        {
            this.MessageType = msgType;
            this.Notify = notify;
        }
        /// <summary>
        /// 消息类别
        /// </summary>
        public XLMessageType MessageType { get; set; }

        /// <summary>
        /// 域
        /// </summary>
        public IXLField Notify { get; set; }
    }

    /// <summary>
    /// Json响应
    /// </summary>
    public class JsonResponse
    {
        public JsonResponse(XLMessageType msgType, IXLField error, IXLField response, int requestID, bool isLast)
        {
            this.MessageType = msgType;
            this.Response = response;
            this.Error = error;
            this.RequestID = requestID;
            this.IsLast = isLast;
        }
        /// <summary>
        /// 消息类别
        /// </summary>
        public XLMessageType MessageType { get; set; }

        /// <summary>
        /// 域
        /// </summary>
        public IXLField Response { get; set; }

        /// <summary>
        /// 错误回报
        /// </summary>
        public IXLField Error { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID { get; set; }

        /// <summary>
        /// 是否是最后一条回报
        /// </summary>
        public bool IsLast { get; set; }


    }
}
