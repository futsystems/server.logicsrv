///////////////////////////////////////////////////////////////////////////////////////
// 用于查询历史行情
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class QryBarRequest:RequestPacket
    {
        public QryBarRequest()
        {
            _type = MessageTypes.BARREQUEST;
            this.Symbol = "";
            this.IntervalType = BarInterval.CustomTime;
            this.Interval = 30;
            this.MaxCount = 500;
            this.Start = DateTime.MinValue;
            this.End = DateTime.MaxValue;
            this.FromEnd = true;
        }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 最大返回Bar个数
        /// </summary>
        public long MaxCount { get; set; }


        /// <summary>
        /// 是否从最新的数据开始
        /// </summary>
        public bool FromEnd { get; set; }

        /// <summary>
        /// 间隔类别
        /// </summary>
        public BarInterval IntervalType { get; set; }


        /// <summary>
        /// 间隔数
        /// </summary>
        public int Interval { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d=',';
            sb.Append(this.Symbol);
            sb.Append(d);
            sb.Append((int)this.IntervalType);
            sb.Append(d);
            sb.Append(this.Interval);
            sb.Append(d);
            sb.Append(this.Start);
            sb.Append(d);
            sb.Append(this.End);
            sb.Append(d);
            sb.Append(this.MaxCount);
            sb.Append(d);
            sb.Append(this.FromEnd);
            return sb.ToString();

        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Symbol = rec[0];
            this.IntervalType = (BarInterval)int.Parse(rec[1]);
            this.Interval = int.Parse(rec[2]);
            this.Start = DateTime.Parse(rec[3]);
            this.End = DateTime.Parse(rec[4]);
            this.MaxCount = long.Parse(rec[5]);
            this.FromEnd = bool.Parse(rec[6]);
        }


    }

    public class RspQryBarResponse : RspResponsePacket
    {
        public RspQryBarResponse()
        {
            _type = MessageTypes.BARRESPONSE;
            this.Bar = null;
        }

        public Bar Bar { get; set; }
        public override string ResponseSerialize()
        {
            if (this.Bar == null)
                return string.Empty;
            return BarImpl.Serialize(this.Bar);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                this.Bar = null;
            else
                this.Bar = BarImpl.Deserialize(content);
        }


    }
}
