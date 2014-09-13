//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.Serialization.Json;
//using System.Runtime.Serialization;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 定义了json message wrapper可以将系统内的message转换成json
//    /// </summary>
//    [DataContract]
//    public class JSONMessage : IJSONData
//    {
//        [DataMember(Order = 0)]
//        public string Type { get; set; }

//        [DataMember(Order = 0)]
//        public string Content { get; set; }

//        public JSONMessage(Message msg)
//        {
//            Type = msg.Type.ToString();
//            Content = msg.Content;
//        }

//        /// <summary>
//        /// 生成主页所用的交易员,委托数量,保证金占用等统计信息
//        /// </summary>
//        /// <param name="tradernum"></param>
//        /// <param name="ordernum"></param>
//        /// <param name="margin"></param>
//        public JSONMessage(long tradernum, long ordernum, long margin)
//        {
//            Type = "HOMESTATISTIC";
//            Content = tradernum.ToString() + "," + ordernum.ToString() + "," + margin.ToString();
//        }

//        public string ToJSONString()
//        {
//            return JSONHelper.ToJSONString(this);
//        }
//    }
//}
