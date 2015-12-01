using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 启动DataFeed
    /// </summary>
    public class MDReqStartDataFeedRequest : RequestPacket
    {
        public MDReqStartDataFeedRequest()
        {
            _type = MessageTypes.MGRSTARTDATAFEED;
            this.DataFeed = QSEnumDataFeedTypes.CTP;
        }

        public QSEnumDataFeedTypes DataFeed { get; set; }

        public override string ContentSerialize()
        {
            return ((int)this.DataFeed).ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), contentstr);
        }
    }

    /// <summary>
    /// 停止DataFeed
    /// </summary>
    public class MDReqStopDataFeedRequest : RequestPacket
    {
        public MDReqStopDataFeedRequest()
        {
            _type = MessageTypes.MGRSTOPDATAFEED;
        }

        public QSEnumDataFeedTypes DataFeed { get; set; }

        public override string ContentSerialize()
        {
            return ((int)this.DataFeed).ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), contentstr);
        }
    }


    /// <summary>
    /// 注册合约数据
    /// </summary>
    public class MDRegisterSymbolsRequest : RequestPacket
    {
        public MDRegisterSymbolsRequest()
        {
            _type = MessageTypes.MGRREGISTERSYMBOLS;
            this.DataFeed = QSEnumDataFeedTypes.DEFAULT;
            this.SymbolList = new List<string>();
            this.Exchange = string.Empty;
        }

        /// <summary>
        /// 行情源
        /// </summary>
        public QSEnumDataFeedTypes DataFeed { get; set; }


        /// <summary>
        /// 合约
        /// </summary>
        public List<string> SymbolList { get; set; }


        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }


        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append((int)this.DataFeed);
            sb.Append(d);
            sb.Append(this.Exchange);
            sb.Append(d);
            string str = string.Empty;
            if(this.SymbolList!= null && this.SymbolList.Count>0)
            {
                str = string.Join(" ",this.SymbolList.ToArray());
            }
            sb.Append(str);

            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), rec[0]);
            this.Exchange = rec[1];
            string[] syms = rec[1].Split(' ');
            this.SymbolList.Clear();
            foreach(var symbol in syms)
            {
                this.SymbolList.Add(symbol);
            }
        }

    }
}
