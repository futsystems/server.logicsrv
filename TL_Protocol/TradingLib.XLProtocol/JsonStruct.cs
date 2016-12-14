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

    public class JsonResponse<T>
        where T : IXLField
    {
        public JsonResponse(XLMessageType msgType,ErrorField error, T response, int requestID, bool isLast)
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
        public ErrorField Error { get; set; }

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
