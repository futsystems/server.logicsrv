﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.Client;
using TradingLib.XLProtocol.V1;
using Common.Logging;
using Newtonsoft.Json;
using System.Security;
using System.Security.Cryptography;
using WebSocket4Net;
namespace APIClient
{
    public partial class Form1 : Form
    {
        ILog logger = LogManager.GetLogger("APIClient");
        WebSocketWrapper websocket = null;

        public Form1()
        {
            InitializeComponent();
            ControlLogFactoryAdapter.SendDebugEvent += new Action<string>(ControlLogFactoryAdapter_SendDebugEvent);
            WireEvent();
            debugControl1.TimeStamps = false;
            //exAddress.Text = "121.40.201.40";
            //exAddress.Text = "127.0.0.1";
            exAddress.Text = "local.cloud-deploy.com";
        }

        void ControlLogFactoryAdapter_SendDebugEvent(string obj)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(ControlLogFactoryAdapter_SendDebugEvent), new object[] { obj });
            }
            else
            {
                debugControl1.GotDebug(obj);
            }
        }

        void WireEvent()
        {
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            btnStartEx.Click += new EventHandler(btnStartEx_Click);
            btnStopEx.Click += new EventHandler(btnStopEx_Click);
            exapiverbose.CheckStateChanged += new EventHandler(exapiverbose_CheckStateChanged);

            btnExLogin.Click += new EventHandler(btnExLogin_Click);
            btnExUpdatePass.Click += new EventHandler(btnExUpdatePass_Click);
            btnExQrySymbol.Click += new EventHandler(btnExQrySymbol_Click);
            btnExQryOrder.Click += new EventHandler(btnExQryOrder_Click);
            btnExQryTrade.Click += new EventHandler(btnExQryTrade_Click);
            btnExQryPosition.Click += new EventHandler(btnExQryPosition_Click);
            btnExQryTradingAccount.Click += new EventHandler(btnExQryTradingAccount_Click);
            btnExQryMaxOrderVol.Click += new EventHandler(btnExQryMaxOrderVol_Click);
            btnExPlaceOrder.Click += new EventHandler(btnExPlaceOrder_Click);
            btnExCancelOrder.Click += new EventHandler(btnExCancelOrder_Click);
            btnExQrySettle.Click += new EventHandler(btnExQrySettle_Click);
            btnExQrySettleSummary.Click += new EventHandler(btnExQrySettleSummary_Click);
            btnExQryBank.Click += new EventHandler(btnExQryBank_Click);
            btnExUpdateBankCard.Click += new EventHandler(btnExUpdateBankCard_Click);
            btnExCashOp.Click += new EventHandler(btnExCashOp_Click);

            btnWSStart.Click += new EventHandler(btnWSStart_Click);
            btnWSStop.Click += new EventHandler(btnWSStop_Click);
            wsBtnLogin.Click += new EventHandler(wsBtnLogin_Click);
            wsBtnUpdatePass.Click += new EventHandler(wsBtnUpdatePass_Click);
            wsBtnQrySymbol.Click += new EventHandler(wsBtnQrySymbol_Click);
            wsBtnQryOrder.Click += new EventHandler(wsBtnQryOrder_Click);
            wsBtnQryTrade.Click += new EventHandler(wsBtnQryTrade_Click);
            wsBtnQryPos.Click += new EventHandler(wsBtnQryPos_Click);
            wsBtnQryAccount.Click += new EventHandler(wsBtnQryAccount_Click);
            wsBtnQryMaxOrderVol.Click += new EventHandler(wsBtnQryMaxOrderVol_Click);
            wsBtnPlaceOrder.Click += new EventHandler(wsBtnPlaceOrder_Click);
            wsBtnCancel.Click += new EventHandler(wsBtnCancel_Click);


            btnStartMd.Click += new EventHandler(btnStartMd_Click);
            btnStopMd.Click += new EventHandler(btnStopMd_Click);
            btnMdLogin.Click += new EventHandler(btnMdLogin_Click);
            btnMdSubMarket.Click += new EventHandler(btnMdSubMarket_Click);
            btnMdQrySymbol.Click += new EventHandler(btnMdQrySymbol_Click);
            btnMdQryMinuteData.Click += new EventHandler(btnMdQryMinuteData_Click);
            btnQryBar1.Click += new EventHandler(btnQryBar1_Click);
            btnQryBar2.Click += new EventHandler(btnQryBar2_Click);

            btnHeartBeat.Click += new EventHandler(btnHeartBeat_Click);

            btnEncode.Click += new EventHandler(btnEncode_Click);
            btnDecode.Click += new EventHandler(btnDecode_Click);


            WireEvent_Web();
        }

        


       



        void btnDecode_Click(object sender, EventArgs e)
        {
            string tmp = "accNo=4312&accessMode=01&accessType=0&backUrl=http://&bizType=000000&currency=CNY&customerInfo={\"certifTp\":\"01\",\"certify_id\":\"\",\"customerNm\":\"\",\"phoneNo\":\"13682102678\"}&frontUrl=http://localhost&merId=2000000000003&merOrderId=P16010716485332N275JG&payType=0002&txnAmt=1&txnSubType=01&txnTime=20160107164853&txnType=01&version=1.0.088888888";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(tmp));

            string val = Convert.ToBase64String(t);
            logger.Info(val);

            tmp = "{\"certifTp\":\"01\",\"certify_id\":\"\",\"customerNm\":\"\",\"phoneNo\":\"13682102678\"}";
            logger.Info(Convert.ToBase64String(Encoding.UTF8.GetBytes(tmp)));
            //Convert.FromBase64String()
        }

        void btnEncode_Click(object sender, EventArgs e)
        {
            //string ret = TradingLib.Common.StringCipher.Encrypt(rawstr.Text, ekey.Text);
            //logger.Info("ret:" + ret);
        }



        void btnQryBar2_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            _mdApi.QryBarData(mdExchange.Text, mdSymbol.Text,false,int.Parse(mdInterval.Text),long.Parse(mdStart.Text),long.Parse(mdEnd.Text), ++_requestId);
        }

        void btnQryBar1_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            _mdApi.QryBarData(mdExchange.Text, mdSymbol.Text,false, int.Parse(mdInterval.Text), int.Parse(mdStartIndex.Text), int.Parse(mdMaxCount.Text), ++_requestId);
        }

        void btnMdQryMinuteData_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            if (string.IsNullOrEmpty(mdMinuteDataStart.Text))
            {
                _mdApi.QryMinuteData(mdExchange.Text, mdSymbol.Text, 0, ++_requestId);
            }
            else
            {
                _mdApi.QryMinuteData(mdExchange.Text, mdSymbol.Text,long.Parse(mdMinuteDataStart.Text), ++_requestId);
            }
        }

        void btnHeartBeat_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            _apiTrader.HeartBeat();
        }

        void btnMdQrySymbol_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            XLQrySymbolField field = new XLQrySymbolField();
            _mdApi.QrySymbol(field,++_requestId);
        }

        void btnMdSubMarket_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            string[] symbols = mdSymbols.Text.Split(',');
            _mdApi.SubscribeMarketData(symbols, ++_requestId);

        }

        APIMarket _mdApi = null;
        void btnMdLogin_Click(object sender, EventArgs e)
        {
            if (_mdApi == null) return;
            XLReqLoginField req = new XLReqLoginField();
            req.UserID = mdUser.Text;
            req.Password = mdPass.Text;
            req.UserProductInfo = "APIClient";
            req.MacAddress = "xxx";
            bool ret = _mdApi.ReqUserLogin(req, ++_requestId);
            logger.Info(string.Format("ReqUserLogin Send Success:{0}", ret));
        }

        void btnStopMd_Click(object sender, EventArgs e)
        {

            if (_mdApi == null) return;
            _mdApi.Release();
            _mdApi = null;
        }

        void btnStartMd_Click(object sender, EventArgs e)
        {
            _mdApi = new APIMarket();
            _mdApi.RegisterServer(mdAddress.Text, int.Parse(mdPort.Text));
            _mdApi.OnServerDisconnected += new Action<int>(_mdApi_OnServerDisconnected);
            _mdApi.OnServerConnected += new Action(_mdApi_OnServerConnected);
            _mdApi.OnRspError += new Action<ErrorField>(_mdApi_OnRspError);
            _mdApi.OnDepthMarketDataField += new Action<XLDepthMarketDataField>(_mdApi_OnDepthMarketDataField);
            _mdApi.OnRspQrySymbol += new Action<XLSymbolField, ErrorField, uint, bool>(_mdApi_OnRspQrySymbol);
            //_mdApi.OnRspQryMinuteData += new Action<XLMinuteDataField, ErrorField, uint, bool>(_mdApi_OnRspQryMinuteData);
           // _mdApi.OnRspQryBarData += new Action<XLBarDataField, ErrorField, uint, bool>(_mdApi_OnRspQryBarData);

            new Thread(() =>
            {
                _mdApi.Verbose = exapiverbose.Checked;
                _mdApi.Init();
                //_mdApi.Join();
                //logger.Info("MDAPI Thread Stopped");
            }).Start();
        }

        void _mdApi_OnRspQryBarData(XLBarDataField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info("bardata:" + JsonConvert.SerializeObject(arg1) + arg4.ToString());
        }

        void _mdApi_OnRspQryMinuteData(XLMinuteDataField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info("minutedata:" + JsonConvert.SerializeObject(arg1) + arg4.ToString());
        }

        void _mdApi_OnRspQrySymbol(XLSymbolField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info("Symbol:" + JsonConvert.SerializeObject(arg1));
        }

        void _mdApi_OnDepthMarketDataField(XLDepthMarketDataField obj)
        {
            logger.Info("MarketData:" + JsonConvert.SerializeObject(obj));
        }

        void _mdApi_OnRspError(ErrorField obj)
        {
            logger.Info(string.Format("OnRspError ID:{0} Msg:{1}", obj.ErrorID, obj.ErrorMsg));
        }

        void _mdApi_OnServerConnected()
        {
            logger.Info("Server Connected");
        }

        void _mdApi_OnServerDisconnected(int obj)
        {
            logger.Info("Server Disconnected:" + obj.ToString());
        }



        #region WebSocket协议 操作
        void wsBtnCancel_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            frm.fmJsonOrderAction fm = new frm.fmJsonOrderAction(websocket);
            fm.ShowDialog();
            fm.Close();
        }

        void wsBtnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            frm.fmJsonReqOrderInsert fm = new frm.fmJsonReqOrderInsert(websocket);
            fm.ShowDialog();
            fm.Close();
        }

        void wsBtnQryMaxOrderVol_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            frm.fmJsonQryMaxOrderVol fm = new frm.fmJsonQryMaxOrderVol(websocket);
            fm.ShowDialog();
            fm.Close();
        }

        void wsBtnUpdatePass_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            frm.fmJsonReqUserPasswordUpdate fm = new frm.fmJsonReqUserPasswordUpdate(websocket);
            fm.ShowDialog();
            fm.Close();
        }

        void wsBtnQryAccount_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            XLQryTradingAccountField field = new XLQryTradingAccountField();
            JsonRequest<XLQryTradingAccountField> qrySymbol = new JsonRequest<XLQryTradingAccountField>(XLMessageType.T_QRY_ACCOUNT, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(qrySymbol));
        }

        void wsBtnQryPos_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            XLQryPositionField field = new XLQryPositionField();
            JsonRequest<XLQryPositionField> qrySymbol = new JsonRequest<XLQryPositionField>(XLMessageType.T_QRY_POSITION, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(qrySymbol));
        }

        void wsBtnQryTrade_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            XLQryTradeField field = new XLQryTradeField();
            JsonRequest<XLQryTradeField> qrySymbol = new JsonRequest<XLQryTradeField>(XLMessageType.T_QRY_TRADE, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(qrySymbol));
        }

        void wsBtnQryOrder_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            XLQryOrderField field = new XLQryOrderField();
            JsonRequest<XLQryOrderField> qrySymbol = new JsonRequest<XLQryOrderField>(XLMessageType.T_QRY_ORDER, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(qrySymbol));
        }

        void wsBtnQrySymbol_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;
            XLQrySymbolField field = new XLQrySymbolField();
            JsonRequest<XLQrySymbolField> qrySymbol = new JsonRequest<XLQrySymbolField>(XLMessageType.T_QRY_SYMBOL, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(qrySymbol));
        }



        void wsBtnLogin_Click(object sender, EventArgs e)
        {
            if (websocket == null || websocket.State != WebSocketState.Open) return;

            XLReqLoginField field = new XLReqLoginField();
            field.UserID = wsUser.Text;
            field.Password = wsPassword.Text;
            field.UserProductInfo = "WebSocket";
            field.MacAddress = "XXXX";

            JsonRequest<XLReqLoginField> loginRequest = new JsonRequest<XLReqLoginField>(XLMessageType.T_REQ_LOGIN, field, (int)++_requestId);
            websocket.Send(XLPacketData.PackJsonRequest(loginRequest));
        }

        void btnWSStop_Click(object sender, EventArgs e)
        {
            if (websocket == null) return;
            if (websocket.State == WebSocketState.Open)
            {
                websocket.Close();
            }
        }

        
        void btnWSStart_Click(object sender, EventArgs e)
        {
            if (websocket == null)
            {
                websocket = new WebSocketWrapper(wsAddress.Text);
                websocket.Opened += new EventHandler(websocket_Opened);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.EnableAutoSendPing = true;
                websocket.AutoSendPingInterval = 3;
                
            }
            if (websocket.State == WebSocketState.Closed || websocket.State == WebSocketState.None)
            {
                websocket.Open();
            }

        }

        void websocket_Closed(object sender, EventArgs e)
        {
            logger.Info("closed");
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            logger.Info("Message:" + e.Message);
        }

        void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            logger.Error("Error:" + e.Exception.ToString());
        }

        void websocket_Opened(object sender, EventArgs e)
        {
            
            logger.Info("Opened:" + websocket.Version.ToString());
        }

        #endregion



        #region 二进制协议 操作

        void btnExCashOp_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLReqCashOperationField req = new XLReqCashOperationField();
            req.Amount = (double)cashopVal.Value;
            req.Args = "";
            req.Gateway = "";

            bool ret = _apiTrader.ReqCashOperation(req, ++_requestId);
            logger.Info(string.Format("ReqCashOperation Send Success:{0}", ret));
        }



        void exapiverbose_CheckStateChanged(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            _apiTrader.Verbose = exapiverbose.Checked;
        }

        void btnExCancelOrder_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            frm.fmOrderAction frm = new frm.fmOrderAction(_apiTrader);
            frm.ShowDialog();
            frm.Close();
        }

        void btnExPlaceOrder_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            frm.fmReqOrderInsert frm = new frm.fmReqOrderInsert(_apiTrader);
            frm.ShowDialog();
            frm.Close();
        }

        void btnExQrySettle_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQrySettlementInfoField req = new XLQrySettlementInfoField();
            bool ret = _apiTrader.QrySettlementInfo(req, ++_requestId);
            logger.Info(string.Format("QrySettleInfo Send Success:{0}", ret));
        }

        void btnExQrySettleSummary_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQrySettleSummaryField req = new XLQrySettleSummaryField();
            req.StartSettleday = 20160101;
            req.EndSettleday = 20200101;
            bool ret = _apiTrader.QrySettleSummary(req, ++_requestId);
            logger.Info(string.Format("QrySettleSummary Send Success:{0}", ret));
        }

        void btnExQryBank_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQryBankCardField req = new XLQryBankCardField();
            bool ret = _apiTrader.QryBankCard(req, ++_requestId);
            logger.Info(string.Format("QryBankCard Send Success:{0}", ret));
        }

        void btnExUpdateBankCard_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLReqUpdateBankCardField req = new XLReqUpdateBankCardField();
            req.BankAccount = "4444";
            req.BankBrch = "中山路支行";
            req.BankID = "1";
            req.CertCode = "320582000000";
            req.MobilePhone = "13888888";
            req.Name = "张三";
            
            bool ret = _apiTrader.ReqUpdateBankCard(req, ++_requestId);
            logger.Info(string.Format("ReqUpdateBankCard Send Success:{0}", ret));
        }


        void btnExQryMaxOrderVol_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            frm.fmQryMaxOrderVol frm = new frm.fmQryMaxOrderVol(_apiTrader);
            frm.ShowDialog();
            frm.Close();
        }

        void btnExQryTradingAccount_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQryTradingAccountField req = new XLQryTradingAccountField();
            bool ret = _apiTrader.QryTradingAccount(req, ++_requestId);
            logger.Info(string.Format("QryTradingAccount Send Success:{0}", ret));
        }

        void btnExQryPosition_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQryPositionField req = new XLQryPositionField();
            bool ret = _apiTrader.QryPosition(req, ++_requestId);
            logger.Info(string.Format("QryPosition Send Success:{0}", ret));
        }

        void btnExQryTrade_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQryTradeField req = new XLQryTradeField();
            bool ret = _apiTrader.QryTrade(req, ++_requestId);
            logger.Info(string.Format("QryTrade Send Success:{0}", ret));
        }

        void btnExQryOrder_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQryOrderField req = new XLQryOrderField();
            bool ret = _apiTrader.QryOrder(req, ++_requestId);
            logger.Info(string.Format("QryOrder Send Success:{0}", ret));
        }

        void btnExQrySymbol_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLQrySymbolField req = new XLQrySymbolField();
            bool ret = _apiTrader.QrySymbol(req, ++_requestId);
            logger.Info(string.Format("QrySymbol Send Success:{0}", ret));
        }

        void btnExUpdatePass_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            frm.fmReqUserPasswordUpdate frm = new frm.fmReqUserPasswordUpdate(_apiTrader);
            frm.ShowDialog();
            frm.Close();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ControlLogFactoryAdapter.SendDebugEvent -= new Action<string>(ControlLogFactoryAdapter_SendDebugEvent);
            if (_apiTrader != null)
            {
                _apiTrader.Release();
            }
            System.Environment.Exit(0);
        }

        uint _requestId = 0;


        void btnExLogin_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            XLReqLoginField req = new XLReqLoginField();
            req.UserID = exUser.Text;
            req.Password = exPass.Text;
            req.UserProductInfo = "APIClient";
            req.MacAddress = "xxx";
            bool ret = _apiTrader.ReqUserLogin(req, ++_requestId);
            logger.Info(string.Format("ReqUserLogin Send Success:{0}", ret));
        }

        
        


        APITrader _apiTrader = null;
        void btnStartEx_Click(object sender, EventArgs e)
        {
            _apiTrader = new APITrader();
            _apiTrader.RegisterServer(exAddress.Text, int.Parse(exPort.Text));
            _apiTrader.OnServerConnected += new Action(_apiTrader_OnServerConnected);
            _apiTrader.OnServerDisconnected += new Action<int>(_apiTrader_OnServerDisconnected);
            _apiTrader.OnRspError += new Action<ErrorField>(_apiTrader_OnRspError);
            _apiTrader.OnRspUserLogin += new Action<XLRspLoginField, ErrorField, uint, bool>(_apiTrader_OnRspUserLogin);
            _apiTrader.OnRspUserPasswordUpdate += new Action<XLRspUserPasswordUpdateField, ErrorField, uint, bool>(_apiTrader_OnRspUserPasswordUpdate);
            _apiTrader.OnRspQrySymbol += new Action<XLSymbolField, ErrorField, uint, bool>(_apiTrader_OnRspQrySymbol);
            _apiTrader.OnRspQryOrder += new Action<XLOrderField?, ErrorField, uint, bool>(_apiTrader_OnRspQryOrder);
            _apiTrader.OnRspQryTrade += new Action<XLTradeField?, ErrorField, uint, bool>(_apiTrader_OnRspQryTrade);
            _apiTrader.OnRspQryPosition += new Action<XLPositionField, ErrorField, uint, bool>(_apiTrader_OnRspQryPosition);
            _apiTrader.OnRspQryTradingAccount += new Action<XLTradingAccountField, ErrorField, uint, bool>(_apiTrader_OnRspQryTradingAccount);
            _apiTrader.OnRspQryMaxOrderVol += new Action<XLQryMaxOrderVolumeField, ErrorField, uint, bool>(_apiTrader_OnRspQryMaxOrderVol);
            _apiTrader.OnRspOrderInsert += new Action<XLInputOrderField, ErrorField, uint, bool>(_apiTrader_OnRspOrderInsert);
            _apiTrader.OnRspOrderAction += new Action<XLInputOrderActionField, ErrorField, uint, bool>(_apiTrader_OnRspOrderAction);
            _apiTrader.OnRspQrySettlementInfo += new Action<XLSettlementInfoField, ErrorField, uint, bool>(_apiTrader_OnRspQrySettlementInfo);
            _apiTrader.OnRtnOrder += new Action<XLOrderField>(_apiTrader_OnRtnOrder);
            _apiTrader.OnRtnTrade += new Action<XLTradeField>(_apiTrader_OnRtnTrade);
            _apiTrader.OnRtnPosition += new Action<XLPositionField>(_apiTrader_OnRtnPosition);
            _apiTrader.OnRspQrySettleSummary += new Action<XLSettleSummaryField?, ErrorField, uint, bool>(_apiTrader_OnRspQrySettleSummary);

            _apiTrader.OnRspQryBankCard += new Action<XLBankCardField?, ErrorField, uint, bool>(_apiTrader_OnRspQryBankCard);
            _apiTrader.OnRspCashOperation += new Action<XLCashOperationField?, ErrorField, uint, bool>(_apiTrader_OnRspCashOperation);
            //System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("en");
            
            //System.Threading.Thread.CurrentThread.CurrentCulture = info;
            //logger.Info("Encoding:" + Encoding.Default.ToString());
            new Thread(() =>
            {
                _apiTrader.Verbose = exapiverbose.Checked;
                _apiTrader.Init();
                //_apiTrader.Join();
                //logger.Info("API Thread Stopped");
            }).Start();
        }

        void _apiTrader_OnRspCashOperation(XLCashOperationField? arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQryBankCard(XLBankCardField? arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQrySettleSummary(XLSettleSummaryField? arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQrySettlementInfo(XLSettlementInfoField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspOrderAction(XLInputOrderActionField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspOrderInsert(XLInputOrderField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQryMaxOrderVol(XLQryMaxOrderVolumeField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQryTradingAccount(XLTradingAccountField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRtnPosition(XLPositionField obj)
        {
            logger.Info(string.Format("PositionNotify:{0}", JsonConvert.SerializeObject(obj)));
        }

        void _apiTrader_OnRspQryPosition(XLPositionField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRtnTrade(XLTradeField obj)
        {
            logger.Info(string.Format("TradeNotify:{0}", JsonConvert.SerializeObject(obj)));
        }

        void _apiTrader_OnRtnOrder(XLOrderField obj)
        {
            logger.Info(string.Format("OrderNotify:{0}", JsonConvert.SerializeObject(obj)));
        }


        void _apiTrader_OnRspQryTrade(XLTradeField? arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspQryOrder(XLOrderField? arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        /// <summary>
        /// 服务端封送结构体 字符串通过UTF8编码转换成byte数组
        /// c#默认使用GB2312编码来接收字符串因此导致出现乱码
        /// 将字符串通过GGB2312还原成byte数组然后用UTF8解码即可得到正确的字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        void _apiTrader_OnRspQrySymbol(XLSymbolField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            string s = arg1.SymbolName;
            byte[] b = Encoding.Default.GetBytes(s);
            string s2 = Encoding.UTF8.GetString(b);

            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspUserPasswordUpdate(XLRspUserPasswordUpdateField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspUserLogin(XLRspLoginField arg1, ErrorField arg2, uint arg3, bool arg4)
        {
            logger.Info(string.Format("Field:{0} Rsp:{1} RequestID:{2} IsLast:{3}", JsonConvert.SerializeObject(arg1), JsonConvert.SerializeObject(arg2), arg3, arg4));
        }

        void _apiTrader_OnRspError(ErrorField obj)
        {
            logger.Info(string.Format("OnRspError ID:{0} Msg:{1}", obj.ErrorID, obj.ErrorMsg));
        }

        void _apiTrader_OnServerDisconnected(int obj)
        {
            logger.Info("Server Disconnected:" + obj.ToString());
        }

        void _apiTrader_OnServerConnected()
        {
            logger.Info("Server Connected");
        }

        void DispError(ErrorField field)
        {
            if (field.ErrorID > 0)
            {
                logger.Info(string.Format("ID:{0} Msg", field.ErrorID, field.ErrorMsg));
            }
        }

        void btnStopEx_Click(object sender, EventArgs e)
        {
            if (_apiTrader == null) return;
            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    _apiTrader.Release();
                    _apiTrader = null;
                }
            );
        }

        #endregion

        ConcurrentDictionary<string, string> demoDict = new ConcurrentDictionary<string, string>();
        private void button1_Click(object sender, EventArgs e)
        {

            bool ret = false;
            string key = "xx";
            ret = demoDict.TryAdd(key, "22");
            logger.Info("first add:" + ret.ToString());
            ret = demoDict.TryAdd(key, "232");
            logger.Info("secend add:" + ret.ToString());
            logger.Info("value:" + demoDict[key]);

            string target = null;
            ret = demoDict.TryRemove(key, out target);
            logger.Info("first remove:" + ret.ToString());
            ret = demoDict.TryRemove(key, out target);
            logger.Info("secend remove:" + ret.ToString());

            HashSet<string> hashSet = new HashSet<string>();
            hashSet.Add(key);
            logger.Info("Count:" + hashSet.Count.ToString());
            hashSet.Add(key);
            logger.Info("Count:" + hashSet.Count.ToString());
        }

  



    }
}
