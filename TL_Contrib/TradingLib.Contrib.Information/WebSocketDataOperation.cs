using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
namespace TradingLib.Contrib
{
    
    /// <summary>
    /// 定义了websocketmessage格式的数据
    /// 1.name 调用的sockserver处理函数
    /// 2.data是我们需要向websocket client转发的数据端
    /// data 的格式 type content
    /// </summary>
    [DataContract]
    public class WebSocketMessage : IJSONData
    {
        [DataMember(Order = 0)]
        string name { get; set; }
        [DataMember(Order = 1)]
        JSONMessage data { get; set; }

        public WebSocketMessage(WebSocketNotifyType type, JSONMessage msg)
        {
            name = type.ToString();
            data = msg;
        }

        public string ToJSONString()
        {
            return JSONHelper.ToJSONString(this);
        }

    }
    /// <summary>
    /// 消息处理类型,需要与python端的函数名称一致
    /// </summary>
    public enum WebSocketNotifyType
    {
        /// <summary>
        /// 向所有listener转发对应的数据
        /// </summary>
        notify_all,
        notify_sessioninfo,//转发sessioninfo数据
        notify_tick,//转发tick数据

    }
}
