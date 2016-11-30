using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using TradingLib.API;
using TradingLib.Common;
using CTPService.Struct;


namespace CTPService
{
    public class TLRequestInfo : RequestInfo<byte[]>
    {
        public TLRequestInfo(string key, EnumFTDType ftdtype, EnumFTDTagType ftdtag,ftd_hdr ftdhdr, List<PktData<IFieldId>> ftdfields, byte[] data)
            : base(key, data)
        {
            this.FTDType = ftdtype;
            this.FTDTag = ftdtag;
            this.FTDHeader = ftdhdr;
            this.FTDFields = ftdfields;
        }

        /// <summary>
        /// FTD报文类型
        /// </summary>
        public EnumFTDType FTDType { get; set; }

        public EnumFTDTagType FTDTag { get; set; }
        /// <summary>
        /// FTD报头
        /// </summary>
        public Struct.ftd_hdr FTDHeader;

        /// <summary>
        /// FTDC正文数据包
        /// 每个业务数据结构 由FTDC头与FTDC结构体构成
        /// </summary>
        public List<PktData<IFieldId>> FTDFields { get; set; }

        
    }
}
