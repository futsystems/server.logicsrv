using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.XLProtocol;

namespace TradingLib.XLProtocol.V1
{
    /// <summary>
    /// V1协议版本StructHelp
    /// </summary>
    public class StructHelp
    {

        public static byte[] StructToBytes(IXLField field)
        {
            XLFieldType fieldType = (XLFieldType)field.FieldID;
            switch (fieldType)
            {
                case XLFieldType.F_ERROR:
                    return XLStructHelp.StructToBytes<ErrorField>((ErrorField)field);
                case XLFieldType.F_REQ_LOGIN:
                    return XLStructHelp.StructToBytes<XLReqLoginField>((XLReqLoginField)field);
                case XLFieldType.F_RSP_LOGIN:
                    return XLStructHelp.StructToBytes<XLRspLoginField>((XLRspLoginField)field);
                case XLFieldType.F_REQ_UPDATEPASS:
                    return XLStructHelp.StructToBytes<XLReqUserPasswordUpdateField>((XLReqUserPasswordUpdateField)field);
                case XLFieldType.F_RSP_UPDATEPASS:
                    return XLStructHelp.StructToBytes<XLRspUserPasswordUpdateField>((XLRspUserPasswordUpdateField)field);
                case XLFieldType.F_QRY_SYMBOL:
                    return XLStructHelp.StructToBytes<XLQrySymbolField>((XLQrySymbolField)field);
                case XLFieldType.F_RSP_SYMBOL:
                    return XLStructHelp.StructToBytes<XLSymbolField>((XLSymbolField)field);
                case XLFieldType.F_QRY_ORDER:
                    return XLStructHelp.StructToBytes<XLQryOrderField>((XLQryOrderField)field);
                case XLFieldType.F_RSP_ORDER:
                    return XLStructHelp.StructToBytes<XLOrderField>((XLOrderField)field);
                case XLFieldType.F_QRY_TRADE:
                    return XLStructHelp.StructToBytes<XLQryTradeField>((XLQryTradeField)field);
                case XLFieldType.F_RSP_TRADE:
                    return XLStructHelp.StructToBytes<XLTradeField>((XLTradeField)field);
                case XLFieldType.F_QRY_POSITION:
                    return XLStructHelp.StructToBytes<XLQryPositionField>((XLQryPositionField)field);
                case XLFieldType.F_RSP_POSITION:
                    return XLStructHelp.StructToBytes<XLPositionField>((XLPositionField)field);
                case XLFieldType.F_QRY_ACCOUNT:
                    return XLStructHelp.StructToBytes<XLQryTradingAccountField>((XLQryTradingAccountField)field);
                case XLFieldType.F_RSP_ACCOUNT:
                    return XLStructHelp.StructToBytes<XLTradingAccountField>((XLTradingAccountField)field);
                case XLFieldType.F_QRY_MAXORDVOL:
                    return XLStructHelp.StructToBytes<XLQryMaxOrderVolumeField>((XLQryMaxOrderVolumeField)field);
                case XLFieldType.F_REQ_INSERTORDER:
                    return XLStructHelp.StructToBytes<XLInputOrderField>((XLInputOrderField)field);
                case XLFieldType.F_REQ_ORDERACTION:
                    return XLStructHelp.StructToBytes<XLInputOrderActionField>((XLInputOrderActionField)field);
                case XLFieldType.F_QRY_EXCHANGERATE:
                    return XLStructHelp.StructToBytes<XLQryExchangeRateField>((XLQryExchangeRateField)field);
                case XLFieldType.F_RSP_EXCHANGERATE:
                    return XLStructHelp.StructToBytes<XLExchangeRateField>((XLExchangeRateField)field);
                case XLFieldType.F_SYMBOL:
                    return XLStructHelp.StructToBytes<XLSpecificSymbolField>((XLSpecificSymbolField)field);
                case XLFieldType.F_MarketData:
                    return  XLStructHelp.StructToBytes<XLDepthMarketDataField>((XLDepthMarketDataField)field);

                case XLFieldType.F_QRY_SETTLEINFO:
                    return XLStructHelp.StructToBytes<XLQrySettlementInfoField>((XLQrySettlementInfoField)field);
                case XLFieldType.F_RSP_SETTLEINFO:
                    return XLStructHelp.StructToBytes<XLSettlementInfoField>((XLSettlementInfoField)field);
                case XLFieldType.F_QRY_SETTLE_SUMMARY:
                    return XLStructHelp.StructToBytes<XLQrySettleSummaryField>((XLQrySettleSummaryField)field);
                case XLFieldType.F_RSP_SETTLE_SUMMARY:
                    return XLStructHelp.StructToBytes<XLSettleSummaryField>((XLSettleSummaryField)field);
                case XLFieldType.F_QRY_CASH_TXN:
                    return XLStructHelp.StructToBytes<XLQryCashTxnField>((XLQryCashTxnField)field);
                case XLFieldType.F_RSP_CASH_TXN:
                    return XLStructHelp.StructToBytes<XLCashTxnField>((XLCashTxnField)field);


                case XLFieldType.F_QRY_MINUTEDATA:
                    return XLStructHelp.StructToBytes<XLQryMinuteDataField>((XLQryMinuteDataField)field);
                case XLFieldType.F_RSP_MINUTEDATA:
                    return XLStructHelp.StructToBytes<XLMinuteDataField>((XLMinuteDataField)field);

                case XLFieldType.F_Qry_BARDATA:
                    return XLStructHelp.StructToBytes<XLQryBarDataField>((XLQryBarDataField)field);
                case XLFieldType.F_RSP_BARDATA:
                    return XLStructHelp.StructToBytes<XLBarDataField>((XLBarDataField)field);
                default:
                    throw new Exception(string.Format("FieldType:{0} not supported", fieldType));
            }
        }

        public static IXLField BytesToStruct(byte[] data, int offset, XLFieldType type)
        {
            switch (type)
            {
                case XLFieldType.F_ERROR:
                    return XLStructHelp.BytesToStruct<ErrorField>(data, offset);
                case XLFieldType.F_REQ_LOGIN:
                    return XLStructHelp.BytesToStruct<XLReqLoginField>(data, offset);
                case XLFieldType.F_RSP_LOGIN:
                    return XLStructHelp.BytesToStruct<XLRspLoginField>(data, offset);
                case XLFieldType.F_REQ_UPDATEPASS:
                    return XLStructHelp.BytesToStruct<XLReqUserPasswordUpdateField>(data, offset);
                case XLFieldType.F_RSP_UPDATEPASS:
                    return XLStructHelp.BytesToStruct<XLRspUserPasswordUpdateField>(data, offset);
                case XLFieldType.F_QRY_SYMBOL:
                    return XLStructHelp.BytesToStruct<XLQrySymbolField>(data, offset);
                case XLFieldType.F_RSP_SYMBOL:
                    return XLStructHelp.BytesToStruct<XLSymbolField>(data, offset);
                case XLFieldType.F_QRY_ORDER:
                    return XLStructHelp.BytesToStruct<XLQryOrderField>(data, offset);
                case XLFieldType.F_RSP_ORDER:
                    return XLStructHelp.BytesToStruct<XLOrderField>(data, offset);
                case XLFieldType.F_QRY_TRADE:
                    return XLStructHelp.BytesToStruct<XLQryTradeField>(data, offset);
                case XLFieldType.F_RSP_TRADE:
                    return XLStructHelp.BytesToStruct<XLTradeField>(data, offset);
                case XLFieldType.F_QRY_POSITION:
                    return XLStructHelp.BytesToStruct<XLQryPositionField>(data, offset);
                case XLFieldType.F_RSP_POSITION:
                    return XLStructHelp.BytesToStruct<XLPositionField>(data, offset);
                case XLFieldType.F_QRY_ACCOUNT:
                    return XLStructHelp.BytesToStruct<XLQryTradingAccountField>(data, offset);
                case XLFieldType.F_RSP_ACCOUNT:
                    return XLStructHelp.BytesToStruct<XLTradingAccountField>(data, offset);
                case XLFieldType.F_QRY_MAXORDVOL:
                    return XLStructHelp.BytesToStruct<XLQryMaxOrderVolumeField>(data, offset);
                case XLFieldType.F_REQ_INSERTORDER:
                    return XLStructHelp.BytesToStruct<XLInputOrderField>(data, offset);
                case XLFieldType.F_REQ_ORDERACTION:
                    return XLStructHelp.BytesToStruct<XLInputOrderActionField>(data, offset);
                case XLFieldType.F_QRY_EXCHANGERATE:
                    return XLStructHelp.BytesToStruct<XLQryExchangeRateField>(data, offset);
                case XLFieldType.F_RSP_EXCHANGERATE:
                    return XLStructHelp.BytesToStruct<XLExchangeRateField>(data, offset);
                case XLFieldType.F_QRY_SETTLE_SUMMARY:
                    return XLStructHelp.BytesToStruct<XLQrySettleSummaryField>(data, offset);
                case XLFieldType.F_RSP_SETTLE_SUMMARY:
                    return XLStructHelp.BytesToStruct<XLSettleSummaryField>(data, offset);
                case XLFieldType.F_QRY_CASH_TXN:
                    return XLStructHelp.BytesToStruct<XLQryCashTxnField>(data, offset);
                case XLFieldType.F_RSP_CASH_TXN:
                    return XLStructHelp.BytesToStruct<XLCashTxnField>(data, offset);

                case XLFieldType.F_SYMBOL:
                    return XLStructHelp.BytesToStruct<XLSpecificSymbolField>(data, offset);
                case XLFieldType.F_MarketData:
                    return XLStructHelp.BytesToStruct<XLDepthMarketDataField>(data, offset);

                case XLFieldType.F_QRY_SETTLEINFO:
                    return XLStructHelp.BytesToStruct<XLQrySettlementInfoField>(data, offset);
                case XLFieldType.F_RSP_SETTLEINFO:
                    return XLStructHelp.BytesToStruct<XLSettlementInfoField>(data, offset);

                case XLFieldType.F_QRY_MINUTEDATA:
                    return XLStructHelp.BytesToStruct<XLQryMinuteDataField>(data, offset);
                case XLFieldType.F_RSP_MINUTEDATA:
                    return XLStructHelp.BytesToStruct<XLMinuteDataField>(data, offset);
                case XLFieldType.F_Qry_BARDATA:
                    return XLStructHelp.BytesToStruct<XLQryBarDataField>(data, offset);
                case XLFieldType.F_RSP_BARDATA:
                    return XLStructHelp.BytesToStruct<XLBarDataField>(data, offset);


                default:
                    throw new Exception(string.Format("FieldType:{0} not supported", type));
            }
        }
    }
}
