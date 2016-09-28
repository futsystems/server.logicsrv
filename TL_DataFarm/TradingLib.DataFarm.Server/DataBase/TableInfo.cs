//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.Common.DataFarm
//{
//    /// <summary>
//    /// 历史数据表信息
//    /// </summary>
//    public class HistTableInfo
//    {

//        /// <summary>
//        /// 名称
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// 记录数量
//        /// </summary>
//        public long Count { get; set; }

//        public DateTime CreateTime { get; set; }

//        public DateTime AccessTime { get; set; }

//        public DateTime ModifiedTime { get; set; }

//        public override string ToString()
//        {
//            return "Name:{0} Count:{1} Created:{2} Modified:{3}".Put(this.Name, this.Count, this.CreateTime, this.ModifiedTime);
//        }
//    }
//}
