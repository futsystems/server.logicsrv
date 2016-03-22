///////////////////////////////////////////////////////////////////////////////////////
// 用于查询历史行情
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public enum EnumBarResponseType
    {
        PLAINTEXT,
        BINARY,
    }
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
            this.BarResponseType = EnumBarResponseType.PLAINTEXT;
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


        /// <summary>
        /// 返回方式
        /// </summary>
        public EnumBarResponseType BarResponseType { get; set; }

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
            sb.Append(d);
            sb.Append(this.BarResponseType);
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
            this.BarResponseType = (EnumBarResponseType)Enum.Parse(typeof(EnumBarResponseType), rec[7]);
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

    public class RspQryBarResponseBin:IPacket
    {
        public QSEnumPacketType PacketType { get; protected set; }

        /// <summary>
        /// 请求数据包前置ID
        /// </summary>
        public string FrontID { get; protected set; }

        /// <summary>
        /// 请求数据包客户端Client
        /// </summary>
        public string ClientID { get; protected set; }

        /// <summary>
        /// 逻辑数据包RequestID
        /// </summary>
        public int RequestID { get; protected set; }


        /// <summary>
        /// Packet对应的底层传输的二进制数据 用于提供给底层传输传进行传输
        /// </summary>
        public byte[] Data { get { return this.SerializeBin(); } }

        /// <summary>
        /// 默认消息类型为未知类型
        /// </summary>
        protected MessageTypes _type = MessageTypes.UNKNOWN_MESSAGE;

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypes Type { get { return _type; } }

        /// <summary>
        /// 消息内容
        /// 消息内容需要序列化对应的逻辑数据包
        /// </summary>
        public string Content { get { return "Bin Bar Response"; } }


        /// <summary>
        /// 二进制数据反序列化
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeBin(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            List<BarImpl> barlsit = new List<BarImpl>();

            for (int i = 0; i < data.Length / 88; i++)
            {
                BarImpl bar = BarImpl.Read(reader);
                this.Add(bar);
            }
        }

        /// <summary>
        /// 序列化成二进制
        /// </summary>
        /// <returns></returns>
        public byte[] SerializeBin()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter b = new BinaryWriter(ms);
            byte[] sizebyte = BitConverter.GetBytes(8 + this.Bars.Count * 88);
            byte[] typebyte = BitConverter.GetBytes((int)MessageTypes.BIN_BARRESPONSE);

            b.Write(sizebyte);
            b.Write(typebyte);

            for (int i = 0; i < this.Bars.Count; i++)
            {
                BarImpl.Write(b, this.Bars[i]);
            }
            return ms.GetBuffer();
        }


        /// <summary>
        /// 序列化成字符串 由子类提供序列化函数
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 反序列化成对象
        /// </summary>
        /// <param name="reqstr"></param>
        /// <returns></returns>
        public virtual void Deserialize(string reqstr)
        {
            throw new NotImplementedException();
        }


        public RspQryBarResponseBin()
        {
            _type = MessageTypes.BIN_BARRESPONSE;
            this.Bars = new List<BarImpl>();
        }

        public void Add(BarImpl bar)
        {
            this.Bars.Add(bar);
        }

        
        public List<BarImpl> Bars { get; set; }


        public static RspQryBarResponseBin CreateResponse(QryBarRequest request)
        {
            RspQryBarResponseBin response = new RspQryBarResponseBin();
            response.RequestID = request.RequestID;
            response.FrontID = request.FrontID;
            response.ClientID = request.ClientID;

            response.PacketType = QSEnumPacketType.RSPRESPONSE;

            return response;
        }


    }

}
