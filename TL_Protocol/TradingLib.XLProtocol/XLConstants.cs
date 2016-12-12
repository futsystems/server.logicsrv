using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.XLProtocol
{
    public class XLConstants
    {
        /// <summary>
        /// 协议头长度
        /// </summary>
        public const ushort PROTO_HEADER_LEN = 4;

        /// <summary>
        /// 正文头长度
        /// </summary>
        public const ushort DATA_HEADER_LEN = 16;

        /// <summary>
        /// 域头长度
        /// </summary>
        public const ushort FIELD_HEADER_LEN = 4;

        /// <summary>
        /// 版本号
        /// </summary>
        public const byte XL_VER_1 = (byte)1;

        /// <summary>
        /// 编码类别 0:无压缩 3:简单压缩
        /// </summary>
        public const byte XL_ENC_NONE = (byte)0;
        public const byte XL_ENC_LZ = (byte)3;

    }
}
