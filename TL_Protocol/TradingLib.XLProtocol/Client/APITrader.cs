using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.XLProtocol;
//using Common.Logging;
using TradingLib.XLProtocol.V1;
#if JSON
using Newtonsoft.Json;
#endif

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
        /// 委托操作回报
        /// 委托操作异常 服务端通给出对应回报
        /// </summary>
        public event Action<XLInputOrderActionField, ErrorField, uint, bool> OnRspOrderAction = delegate { };
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
        //string _serverIP = string.Empty;
        //int _port = 0;
        SocketClient _socketClient = null;
        //ILog logger = LogManager.GetLogger("APITtrader");

        public bool IsConnected
        {
            get
            {
                if (_socketClient == null) return false;
                return _socketClient.IsOpen;
            }
        }

        public APITrader()
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
                    case XLMessageType.T_RSP_ORDER:
                        {
                            XLOrderField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLOrderField)pkt.FieldList[0];
                            }
                            else
                            {
                                response = new XLOrderField();
                            }
                            OnRspQryOrder(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                            
                        }
                    case XLMessageType.T_RTN_ORDER:
                        {
                            XLOrderField notify = (XLOrderField)pkt.FieldList[0];
                            OnRtnOrder(notify);
                            break;
                        }

                    case XLMessageType.T_RSP_TRADE:
                        {
                            XLTradeField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLTradeField)pkt.FieldList[0];
                            }
                            else
                            {
                                response = new XLTradeField();
                            }
                            OnRspQryTrade(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RTN_TRADE:
                        {
                            XLTradeField notify = (XLTradeField)pkt.FieldList[0];
                            OnRtnTrade(notify);
                            break;
                        }
                    case XLMessageType.T_RSP_POSITION:
                        {
                            XLPositionField response;
                            if (pkt.FieldList.Count > 0)
                            {
                                response = (XLPositionField)pkt.FieldList[0];
                            }
                            else
                            {
                                response = new XLPositionField();
                            }
                            OnRspQryPosition(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RTN_POSITIONUPDATE:
                        {
                            XLPositionField notify = (XLPositionField)pkt.FieldList[0];
                            OnRtnPosition(notify);
                            break;
                        }
                    case XLMessageType.T_RSP_ACCOUNT:
                        {
                            XLTradingAccountField response = (XLTradingAccountField)pkt.FieldList[0];
                            OnRspQryTradingAccount(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_MAXORDVOL:
                        {
                            XLQryMaxOrderVolumeField response = (XLQryMaxOrderVolumeField)pkt.FieldList[0];
                            OnRspQryMaxOrderVol(response, NoError, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_INSERTORDER:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0];
                            XLInputOrderField response = (XLInputOrderField)pkt.FieldList[1];
                            OnRspOrderInsert(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
                            break;
                        }
                    case XLMessageType.T_RSP_ORDERACTION:
                        {
                            ErrorField rsp = (ErrorField)pkt.FieldList[0];
                            XLInputOrderActionField response = (XLInputOrderActionField)pkt.FieldList[1];
                            OnRspOrderAction(response, rsp, dataHeader.RequestID, (int)dataHeader.IsLast == 1 ? true : false);
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

        /// <summary>
        /// 提交委托操作
        /// </summary>
        /// <param name="req"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool ReqOrderAction(XLInputOrderActionField req, uint requestID)
        {
            XLPacketData pktData = new XLPacketData(XLMessageType.T_REQ_ORDERACTION);
            pktData.AddField(req);
            return SendPktData(pktData, XLEnumSeqType.SeqReq, requestID);
        }

#endregion


        public void HeartBeat()
        {
            _socketClient.RequestHeartBeat();
        }

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
