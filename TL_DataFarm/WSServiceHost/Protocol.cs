using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSServiceHost
{

    public class RspInfo
    {
        /// <summary>
        /// 错误代码 0表示正常
        /// </summary>
        public int ErrorId { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    public class RequestBase
    {
        /// <summary>
        /// 消息类别
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int ReqId { get; set; }

        /// <summary>
        /// 消息载体
        /// </summary>
        public virtual object Payload { get; set; }
    }


    public class ResponseBase
    {
        /// <summary>
        /// 消息类别
        /// </summary>
        public string MsgType { get; set; }


        /// <summary>
        /// 回报
        /// </summary>
        public RspInfo RspInfo { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int ReqId { get; set; }

        /// <summary>
        /// 是否是最后一个回报消息
        /// 请求可能会对应多条回报
        /// </summary>
        public bool IsLast { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public virtual object Payload { get; set; }
    }


    public class NotifyBase
    {
        public string MsgType { get; set; }


    }
}
