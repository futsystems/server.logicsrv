using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTPService
{
    public class Constanst
    {
        /// <summary>
        /// FTD报头 + FTDC报头长度 4+22=26
        /// </summary>
        public const int PROFTD_HDRLEN = 26;
        /// <summary>
        /// FTD头长度 22
        /// </summary>
        public const int FTD_HDRLEN = 22;

        /// <summary>
        /// 协议报头长度 4
        /// </summary>
        public const int PROTO_HDRLEN = 4;
        /// <summary>
        /// FTDC 长度 4
        /// </summary>
        public const int FTDC_HDRLEN = 4;

        public const byte FTDC_HEAD = (byte)1;

        public const byte FTDC_VER = (byte)12;

        public const byte THOST_ENC_NONE = (byte)0;

        public const byte THOST_ENC_LZ = (byte)3;
    }
}
