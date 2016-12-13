using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.XLProtocol;
using Common.Logging;
using TradingLib.XLProtocol.V1;
using Newtonsoft.Json;

namespace TradingLib.XLProtocol.Client
{
    public class APITrader
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
        /// 查询委托回报
        /// </summary>
        public event Action<XLOrderField, ErrorField, uint, bool> OnRspQryOrder = delegate { };

        /// <summary>
        /// 查询成交回报
        /// </summary>
        public event Action<XLTradeField, ErrorField, uint, bool> OnRspQryTrade = delegate { };

        /// <summary>
        /// 查询持仓回报
        /// </summary>
        public event Action<XLPositionField, ErrorField, uint, bool> OnRspQryPosition = delegate { };

        /// <summary>
        /// 查询交易账户回报
        /// </summary>
        public event Action<XLTradingAccountField, ErrorField, uint, bool> OnRspQryTradingAccount = delegate { };

        /// <summary>
        /// 查询最大报单数量回报
        /// </summary>
        public event Action<XLQryMaxOrderVolumeField, ErrorField, uint, bool> OnRspQryMaxOrderVol = delegate { };


        /// <summary>
        /// 提交委托回报
        /// 当委托参数异常或者柜台拒绝委托时 给出该回报
        /// </summary>
        public event Action<XLInputOrderField, ErrorField, uint, bool> OnRspOrderInsert = delegate { };

        /// <summary>
        /// 委托实时通知
        /// </summary>
        public event Action<XLOrderField> OnRtnOrder = delegate { };

        /// <summary>
        /// 成交实时通知
        /// </summary>
        public event Action<XLTradeField> OnRtnTrade = delegate { };

        /// <summary>
        /// 持仓实时通知
        /// </summary>
        public event Action<XLPositionField> OnRtnPosition = delegate { };
        #endregion
        string _serverIP = string.Empty;
        int _port = 0;
        SocketClient _socketClient = null;
        ILog logger = LogManager.GetLogger("APITtrader");

        public APITrader(string serverIP, int port)
        {
            _serverIP = serverIP;
            _port = port;
            _socketClient = new SocketClient();
            _socketClient.ThreadBegin += new Action(_socketClient_ThreadBegin);
            _socketClient.ThreadExit += new Action(_socketClient_ThreadExit);
            _socketClient.DataReceived += new Action<XLProtocolHeader, byte[], int>(_socketClient_DataReceived);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        { 
            if(_socketClient.StartClient(_serverIP,_port))
            {
                OnServerConnected();
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
        public void Join()
        {
            _socketClient.Wait();
        }

        /// <summary>
        /// 停止接口线程
        /// </summary>
        public void Release()
        {
            _socketClient.Close();
        }

        void _socketClient_DataReceived(XLProtocolHeader header, byte[] data, int offset)
        {
            try
            {
                XLMessageType msgType = (XLMessageType)header.XLMessageType;
                XLDataHeader dataHeader;
                XLPacketData pkt = XLPacketData.Deserialize(msgType, data, offset, out dataHeader);
                logger.Debug(string.Format("PktData Recv Type:{0} Size:{1}", msgType,dataHeader.FieldLength + XLConstants.PROTO_HEADER_LEN + XLConstants.DATA_HEADER_LEN));
                switch (msgType)
                {
                    case XLMessageType.T_RSP_LOGIN:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0].FieldData;
                            XLRspLoginField response = (XLRspLoginField)pkt.FieldList[1].FieldData;
                            OnRspUserLogin(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_UPDATEPASS:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0].FieldData;
                            XLRspUserPasswordUpdateField response = (XLRspUserPasswordUpdateField)pkt.FieldList[1].FieldData;
                            OnRspUserPasswordUpdate(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_SYMBOL:
                        {
                            XLSymbolField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLSymbolField)pkt.FieldList[0].FieldData;
                            }
                            else
                            {
                                response = new XLSymbolField();
                            }
                            OnRspQrySymbol(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_ORDER:
                        {
                            XLOrderField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLOrderField)pkt.FieldList[0].FieldData;
                            }
                            else
                            {
                                response = new XLOrderField();
                            }
                            OnRspQryOrder(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                            
                        }
                    case XLMessageType.T_RTN_ORDER:
                        {
                            XLOrderField notify = (XLOrderField)pkt.FieldList[0].FieldData;
                            OnRtnOrder(notify);
                            break;
                        }

                    case XLMessageType.T_RSP_TRADE:
                        {
                            XLTradeField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLTradeField)pkt.FieldList[0].FieldData;
                            }
                            else
                            {
                                response = new XLTradeField();
                            }
                            OnRspQryTrade(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RTN_TRADE:
                        {
                            XLTradeField notify = (XLTradeField)pkt.FieldList[0].FieldData;
                            OnRtnTrade(notify);
                            break;
                        }
                    case XLMessageType.T_RSP_POSITION:
                        {
                            XLPositionField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLPositionField)pkt.FieldList[0].FieldData;
                            }
                            else
                            {
                                response = new XLPositionField();
                            }
                            OnRspQryPosition(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RTN_POSITIONUPDATE:
                        {
                            XLPositionField notify = (XLPositionField)pkt.FieldList[0].FieldData;
                            OnRtnPosition(notify);
                            break;
                        }
                    case XLMessageType.T_RSP_ACCOUNT:
                        {
                            XLTradingAccountField response = (XLTradingAccountField)pkt.FieldList[0].FieldData;
                            OnRspQryTradingAccount(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_MAXORDVOL:
                        {
                            XLQryMaxOrderVolumeField response = (XLQryMaxOrderVolumeField)pkt.FieldList[0].FieldData;
                            OnRspQryMaxOrderVol(response, new ErrorField(), dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_INSERTORDER:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0].FieldData;
                            XLInputOrderField response = (XLInputOrderField)pkt.FieldList[1].FieldData;
                            OnRspOrderInsert(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    default:
                        logger.Info(string.Format("Unhandled Pkt:{0}", msgType));
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Data Process Error:{0}", ex.ToString()));
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
        /// 请求查询委托
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryOrder(XLQryOrderField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_ORDER);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 请求查询成交
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryTrade(XLQryTradeField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_TRADE);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryPosition(XLQryPositionField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_POSITION);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 查询交易账户
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryTradingAccount(XLQryTradingAccountField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_ACCOUNT);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 查询最大报单数量
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool QryMaxOrderVol(XLQryMaxOrderVolumeField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_QRY_MAXORDVOL);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqQry, requestID);
        }

        /// <summary>
        /// 请求提交委托
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool ReqOrderInsert(XLInputOrderField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_REQ_INSERTORDER);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

        #endregion



        bool SendPktData(XLPacketData pktData, XLEnumSeqType seqType,uint requestID)
        {
            byte[] data = XLPacketData.PackToBytes(pktData, XLEnumSeqType.SeqReq, 0, requestID, true);
            logger.Debug(string.Format("PktData Send Type:{0} Size:{1}", pktData.MessageType, data.Length));
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
