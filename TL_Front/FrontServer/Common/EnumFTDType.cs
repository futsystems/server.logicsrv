using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTPService
{
    public enum EnumFTDType
    {
        /// <summary>
        /// 报文无意义 一般用于心跳
        /// </summary>
        FTDTypeNone = 0,
        FTDTypeCompressed, 
        FTDTypeFTDC
    }


}
