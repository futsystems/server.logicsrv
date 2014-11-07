using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 查询通道
    /// </summary>
    public class MGRQryConnectorRequest:RequestPacket
    {
        public MGRQryConnectorRequest()
        {
            _type = MessageTypes.MGRQRYCONNECTOR;
        }
    }


    public class ConnectorInfo
    {
        public ConnectorInfo()
        {
            this.ClassName = string.Empty;
            this.Status = false;
            this.Token = string.Empty;
            this.Type = QSEnumConnectorType.Broker;
        }

        public ConnectorInfo(IBroker broker)
        {
            this.ClassName = broker.GetType().FullName;
            this.Status = broker.IsLive;
            this.Token = broker.Title;
            this.Type = QSEnumConnectorType.Broker;
        }
        

        public ConnectorInfo(IDataFeed df)
        {
            this.ClassName = df.GetType().FullName;
            this.Status = df.IsLive;
            this.Token = df.Title;
            this.Type = QSEnumConnectorType.DataFeed;
        }



        /// <summary>
        /// 通道名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 通道状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 通道描述
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 通道类型
        /// </summary>
        public QSEnumConnectorType Type { get; set; }

        public  string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ClassName);
            sb.Append(d);
            sb.Append(this.Status.ToString());
            sb.Append(d);
            sb.Append(this.Token);
            sb.Append(d);
            sb.Append(this.Type);
            return sb.ToString();
        }

        public void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.ClassName = rec[0];
            this.Status = bool.Parse(rec[1]);
            this.Token = rec[2];
            this.Type = (QSEnumConnectorType)Enum.Parse(typeof(QSEnumConnectorType), rec[3]);
        }
    }


    /// <summary>
    /// 查询通道回报
    /// </summary>
    public class RspMGRQryConnectorResponse : RspResponsePacket
    {
        public RspMGRQryConnectorResponse()
        {
            _type = MessageTypes.MGRCONNECTORRESPONSE;
            Connector = new ConnectorInfo();
        }



        public ConnectorInfo Connector { get; set; } 
        public override string ResponseSerialize()
        {
            return Connector.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            Connector.Deserialize(content);
        }

    }

    /// <summary>
    /// 请求启动交易通道
    /// </summary>
    public class MGRReqStartBrokerRequest : RequestPacket
    {
        public MGRReqStartBrokerRequest()
        {
            _type = MessageTypes.MGRSTARTBROKER;
            this.FullName = string.Empty;
        }

        public string FullName { get; set; }

        public override string ContentSerialize()
        {
            return this.FullName;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.FullName = contentstr;
        }
    }

    /// <summary>
    /// 请求停止交易通道
    /// </summary>
    public class MGRReqStopBrokerRequest : RequestPacket
    {
        public MGRReqStopBrokerRequest()
        {
            _type = MessageTypes.MGRSTOPBROKER;
            this.FullName = string.Empty;
        }

        public string FullName { get; set; }

        public override string ContentSerialize()
        {
            return this.FullName;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.FullName = contentstr;
        }
    }

    /// <summary>
    /// 请求启动行情通道
    /// </summary>
    public class MGRReqStartDataFeedRequest : RequestPacket
    {
        public MGRReqStartDataFeedRequest()
        {
            _type = MessageTypes.MGRSTARTDATAFEED;
            this.FullName = string.Empty;
        }

        public string FullName { get; set; }

        public override string ContentSerialize()
        {
            return this.FullName;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.FullName = contentstr;
        }
    }

    /// <summary>
    /// 请求停止行情通道
    /// </summary>
    public class MGRReqStopDataFeedRequest : RequestPacket
    {
        public MGRReqStopDataFeedRequest()
        {
            _type = MessageTypes.MGRSTOPDATAFEED;
            this.FullName = string.Empty;
        }

        public string FullName { get; set; }

        public override string ContentSerialize()
        {
            return this.FullName;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.FullName = contentstr;
        }
    }
}
