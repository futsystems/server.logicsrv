using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace CTPService
{
    public class  CTPConvert
    {
        public static QSEnumOffsetFlag ConvOffsetFlag(TThostFtdcOffsetFlagType offset)
        {
            switch (offset)
            {
                case TThostFtdcOffsetFlagType.Open: return QSEnumOffsetFlag.OPEN;
                case TThostFtdcOffsetFlagType.Close: return QSEnumOffsetFlag.CLOSE;
                case TThostFtdcOffsetFlagType.CloseToday: return QSEnumOffsetFlag.CLOSETODAY;
                case TThostFtdcOffsetFlagType.CloseYesterday: return QSEnumOffsetFlag.CLOSEYESTERDAY;
                default:
                    return QSEnumOffsetFlag.CLOSE;
            }
        }

        public static TThostFtdcOffsetFlagType ConvOffsetFlag(QSEnumOffsetFlag offset)
        {
            switch (offset)
            {
                case QSEnumOffsetFlag.OPEN: return TThostFtdcOffsetFlagType.Open;
                case QSEnumOffsetFlag.CLOSE: return TThostFtdcOffsetFlagType.Close;
                case QSEnumOffsetFlag.CLOSETODAY: return TThostFtdcOffsetFlagType.CloseToday;
                case QSEnumOffsetFlag.CLOSEYESTERDAY: return TThostFtdcOffsetFlagType.CloseYesterday;
                default:
                    return TThostFtdcOffsetFlagType.Close;
            }
        }
    }
}
