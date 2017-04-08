using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.XLProtocol;
//using Common.Logging;
using TradingLib.XLProtocol.V1;
using Newtonsoft.Json;

namespace TradingLib.XLProtocol.Client
{
    public class APIMarket
    {

        #region 对外暴露的事件
        public event Action<ErrorField> OnRspError = delegate { };
        /// <summary>
        /// 服务端连接建立
        /// </summary>
        public event Action OnServerConnected = delegate { };

        /// <summary>
        /// 服务端连接断开
        /// </summary>
        public event Action<int> OnServerDisconnected = delegate { };


        /// <summary>
        /// 登入回报
        /// </summary>
        public event Action<XLRspLoginField, ErrorField, uint, bool> OnRspUserLogin = delegate { };

        /// <summary>
        /// 修改密码回报
        /// </summary>
        public event Action<XLRspUserPasswordUpdateField, ErrorField, uint, bool> OnRspUserPasswordUpdate = delegate { };

        /// <summary>
        /// 查询合约回报
        /// </summary>
        public event Action<XLSymbolField, ErrorField, uint, bool> OnRspQrySymbol = delegate { };


        /// <summary>
        /// 查询分时数据回报
        /// </summary>
        public event Action<XLMinuteDataField, ErrorField, uint, bool> OnRspQryMinuteData = delegate { };

        /// <summary>
        /// 查询Bar数据回报
        /// </summary>
        public event Action<XLBarDataField, ErrorField, uint, bool> OnRspQryBarData = delegate { };
        /// <summary>
        /// 市场行情回报
        /// </summary>
        public event Action<XLDepthMarketDataField> OnDepthMarketDataField = delegate { };

        #endregion

        SocketClient _socketClient = null;
        //ILog logger = LogManager.GetLogger("APIMarket");
        public bool IsConnected
        {
            get
            {
                if (_socketClient == null) return false;
                return _socketClient.IsOpen;
            }
        }
        public APIMarket()
        {
            _socketClient = new SocketClient();
            _socketClient.DataReceived += new Action<XLProtocolHeader, byte[], int>(_socketClient_DataReceived);
             _socketClient.Connected += new Action(_socketClient_Connected);
            _socketClient.Disconnected += new Action(_socketClient_Disconnected);
        }

        void _socketClient_Disconnected()
        {
            OnServerDisconnected(0);
            
        }

        void _socketClient_Connected()
        {
            OnServerConnected();
        }
        

        public void RegisterServer(string serverip, int port)
        {
            _socketClient.RegisterServer(serverip, port);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        { 
            if(_socketClient.Connect())
            {
                //OnServerConnected();
            }
            else
            {
                ErrorField rsp = new ErrorField();
                rsp.ErrorID = 2;
                rsp.ErrorMsg = "连接建立失败";
                OnRspError(rsp);
            }
        }

        /// <summary>
        /// 等待接口线程结束运行
        /// </summary>
        //public void Join()
        //{
        //    _socketClient.Wait();
        //}

        /// <summary>
        /// 停止接口线程
        /// </summary>
        public void Release()
        {
            _socketClient.Close();
        }

        bool _verb = true;

        public bool Verbose { get { return _verb; } set { _verb = value; } }

        static ErrorField NoError = new ErrorField();

        void _socketClient_DataReceived(XLProtocolHeader header, byte[] data, int offset)
        {
            try
            {
                XLMessageType msgType = (XLMessageType)header.XLMessageType;
                XLDataHeader dataHeader;
                XLPacketData pkt = XLPacketData.Deserialize(msgType, data, offset, out dataHeader);
                //if(_verb) logger.Debug(string.Format("PktData Recv Type:{0} Size:{1}", msgType,dataHeader.FieldLength + XLConstants.PROTO_HEADER_LEN + XLConstants.DATA_HEADER_LEN));
                switch (msgType)
                {
                    case XLMessageType.T_RSP_ERROR:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0];
                            OnRspError(rsp);
                            break;
                        }
                    case XLMessageType.T_RSP_LOGIN:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0];
                            XLRspLoginField response = (XLRspLoginField)pkt.FieldList[1];
                            OnRspUserLogin(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_UPDATEPASS:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0];
                            XLRspUserPasswordUpdateField response = (XLRspUserPasswordUpdateField)pkt.FieldList[1];
                            OnRspUserPasswordUpdate(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_SYMBOL:
                        {
                            XLSymbolField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLSymbolField)pkt.FieldList[0];
                            }
                            else
                            {
                                response = new XLSymbolField();
                            }
                            OnRspQrySymbol(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RTN_MARKETDATA:
                        {
                            XLDepthMarketDataField marketData;
                            if (pkt.FieldList.Count > 0)
                            {
                                marketData = (XLDepthMarketDataField)pkt.FieldList[0];
                                OnDepthMarketDataField(marketData);
                            }
                            break;
                        }
                    case XLMessageType.T_RSP_MINUTEDATA:
                        {
                            XLMinuteDataField minuteData;
                            if (pkt.FieldList.Count > 0)
                            {
                                for (int i = 0; i < pkt.FieldList.Count; i++)
                                {
                                    minuteData = (XLMinuteDataField)pkt.FieldList[i];
                                    OnRspQryMinuteData(minuteData, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 && i==pkt.FieldList.Count-1);
                                }
                            }
                            break;
                        }
                    case XLMessageType.T_RSP_BARDATA:
                        {
                            XLBarDataField barData;
                            if (pkt.FieldList.Count > 0)
                            {
                                for (int i = 0; i < pkt.FieldList.Count; i++)
                                {
                                    barData = (XLBarDataField)pkt.FieldList[i];
                                    OnRspQryBarData(barData, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 && i == pkt.FieldList.Count - 1);
                                }
                            }
                            break;
                        }
                    default:
                        //logger.Info(string.Format("Unhandled Pkt:{0}", msgType));
                        break;
                }
            }
            catch (Exception ex)
            {
                //logger.Error(string.Format("Data Process Error:{0}", ex.ToString()));
            }
        }

        void _socketClient_ThreadExit()
        {
            //logger.Info("SocketClient Thred Exit");
        }

        void _socketClient_ThreadBegin()
        {
            //logger.Info("SocketClient Thred Begin");
        }


        #region 接口操作
        /// <summary>
        /// 请求登入
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool ReqUserLogin(XLReqLoginField req,uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_REQ_LOGIN);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        /// <summary>
        /// 请求更新密码
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool ReqUserPasswordUpdate(XLReqUserPasswordUpdateField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_REQ_UPDATEPASS);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        /// <summary>
        /// 请求查询合约
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QrySymbol(XLQrySymbolField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_SYMBOL);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 请求注册行情数据
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool SubscribeMarketData(string[] symbols, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_REQ_MARKETDATA);
            foreach (var symbol in symbols)
            {
                var tmp = new XLSpecificSymbolField() { SymbolID = symbol };
                pktData.AddField(tmp);
            }
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        /// <summary>
        /// 按交易日查询合约分时数据
        /// 交易日为0 则查询当前交易日分时数据
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="symbol"></param>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public bool QryMinuteData(string exchange, string symbol, int tradingday, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_MINUTEDATA);
            XLQryMinuteDataField field = new XLQryMinuteDataField();
            field.ExchangeID = exchange;
            field.SymbolID = symbol;
            field.TradingDay = tradingday;

            pktData.AddField(field);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        /// <summary>
        /// 查询当前交易日某个时间之后的所有分时数据
        /// 用于更新分时图新增数据
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryMinuteData(string exchange, string symbol, long start, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_MINUTEDATA);
            XLQryMinuteDataField field = new XLQryMinuteDataField();
            field.ExchangeID = exchange;
            field.SymbolID = symbol;
            field.Start = start;

            pktData.AddField(field);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }


        public bool QryBarData(string exchange, string symbol, int interval, int startIndex, int maxCount,uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_BARDATA);
            XLQryBarDataField field = new XLQryBarDataField();
            field.ExchangeID = exchange;
            field.SymbolID = symbol;
            field.Interval = interval;
            field.StartIndex = startIndex;
            field.MaxCount = maxCount;
            field.Start = long.MinValue;
            field.End = long.MaxValue;
            field.HavePartial = true;

            pktData.AddField(field);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        public bool QryBarData(string exchange, string symbol, int interval, long start, long end, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_BARDATA);
            XLQryBarDataField field = new XLQryBarDataField();
            field.ExchangeID = exchange;
            field.SymbolID = symbol;
            field.Interval = interval;
            field.StartIndex = 0;
            field.MaxCount = 0;
            field.Start = start;
            field.End = end;
            field.HavePartial = true;

            pktData.AddField(field);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        /// <summary>
        /// Converts a DateTime to TradeLink Date (eg July 11, 2006 = 20060711)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ToTLDate(DateTime dt)
        {

            return (dt.Year * 10000) + (dt.Month * 100) + dt.Day;
        }
        /// <summary>
        /// gets tradelink time from date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        static int ToTLTime(DateTime date)
        {
            return DT2FT(date);
        }
        static int DT2FT(DateTime d) { return TL2FT(d.Hour, d.Minute, d.Second); }
        static int TL2FT(int hour, int min, int sec) { return hour * 10000 + min * 100 + sec; }
        static long ToTLDateTime(DateTime dt)
        {
            if (dt == DateTime.MinValue) return long.MinValue;
            if (dt == DateTime.MaxValue) return long.MaxValue;

            return ((long)ToTLDate(dt) * 1000000) + (long)ToTLTime(dt);
        }

        #endregion



        bool SendPktData(XLPacketData pktData, XLEnumSeqType seqType,uint requestID)
        {
            byte[] data = XLPacketData.PackToBytes(pktData, XLEnumSeqType.SeqReq, 0, requestID, true);
            //if (_verb) logger.Debug(string.Format("PktData Send Type:{0} Size:{1}", pktData.MessageType, data.Length));
            //logger.Info(string.Format("PktData Send Type:{0} Data:{1} RequestID:{2}", pktData.MessageType, JsonConvert.SerializeObject(pktData), requestID));
            return SendData(data, data.Length);
        }

        bool SendData(byte[] data, int count)
        {
            if (_socketClient.IsOpen)
            {
                int size = _socketClient.Send(data, count);
                if (size == count)
                {
                    return true;
                }
                else
                {
                    OnServerDisconnected(0x1002);
                    return false;
                }
            }
            return false;
        }
    }
}
