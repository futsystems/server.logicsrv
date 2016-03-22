using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;
using TradingLib.API;
using TradingLib.Common;


namespace TCPServiceHost
{
    /// <summary>
    /// 头部定长协议
    /// 用于头部长度固定，头部中包含消息体长度的协议类型
    /// </summary>
    public class TLReceiveFilter : FixedHeaderReceiveFilter<TLRequestInfo>
    {
        public TLReceiveFilter()
            : base(8)
        { 
            
        }
        const int HEADERSIZE = 8;
        const int LENGTHOFFSET = 0;
        const int TYPEOFFSET = 4;
        /// <summary>
        /// 从定长头部获得消息体长度
        /// TradingLib Message协议定义 4字节消息长度(包含头长度和消息体长度) 4字节消息类型 以及消息体
        /// length type body
        /// </summary>
        /// <param name="header"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            //MessageTypes type = (MessageTypes)BitConverter.ToInt32(header, offset + TYPEOFFSET);
            int totallen = BitConverter.ToInt32(header, offset + LENGTHOFFSET);
            //返回消息体长度
            return totallen - HEADERSIZE;
        }

        /// <summary>
        /// 从消息体数据中解析RequestInfo
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bodyBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override TLRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            int totallen = BitConverter.ToInt32(header.Array,LENGTHOFFSET);
            MessageTypes type = (MessageTypes)BitConverter.ToInt32(header.Array,TYPEOFFSET);
            string content = string.Empty;
            byte[] data = null;
            if(length>0)
            {
                if (type != MessageTypes.BARRESPONSEBIN)
                {
                    content = System.Text.Encoding.UTF8.GetString(bodyBuffer, offset, length);
                }
                else
                {
                    data = new byte[length];
                    Array.Copy(bodyBuffer, offset, data, 0, length);
                }
            }
            Message message = new Message(type, content,data,totallen);

            return new TLRequestInfo(type.ToString(),content,message);
        }

          
    }
}
