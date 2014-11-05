using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.BrokerXAPI.Interop;
using System.Runtime.InteropServices;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.BrokerXAPI
{
    public class TLBroker
    {
        TLBrokerProxy _broker;
        TLBrokerWrapperProxy _wrapper;
        public TLBroker()
        {
            _wrapper = new TLBrokerWrapperProxy(@"lib\", @"TLBrokerWrapper.dll");
            _broker = new TLBrokerProxy(@"lib\CTP630\", @"TLBrokerCTP.dll");
            _wrapper.Register(_broker.Handle);

            _wrapper.OnRtnTradeEvent += new CBRtnTrade(_wrapper_OnRtnTradeEvent);
            _wrapper.OnRtnOrderEvent += new CBRtnOrder(_wrapper_OnRtnOrderEvent);
            _wrapper.OnConnectedEvent += new CBOnConnected(_wrapper_OnConnectedEvent);
            _wrapper.OnDisconnectedEvent += new CBOnDisconnected(_wrapper_OnDisconnectedEvent);
            _wrapper.OnLoginEvent += new CBOnLogin(_wrapper_OnLoginEvent);
            _wrapper.OnRtnOrderErrorEvent += new CBRtnOrderError(_wrapper_OnRtnOrderErrorEvent);


            XServerInfoField srvinfo = new XServerInfoField();
            srvinfo.ServerAddress = "222.66.235.70";
            srvinfo.ServerPort = 61205;
            srvinfo.Field1 = "8000";//BrokerID
            //srvinfo.ServerAddress = "gw-release-ctcc1.lottoqq.com";
            //srvinfo.ServerPort = 40255;
            srvinfo.Field1 = "8000";//BrokerID
            srvinfo.Field2 = "xo";
            srvinfo.Field3 = "helloworld";
            _wrapper.Connect(ref srvinfo);






            Util.sleep(2000);
            XOrderField order = new XOrderField();
            Console.WriteLine("order size:" + Marshal.SizeOf(typeof(XOrderField)).ToString());
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();
            order.Symbol = "cu1501";
            order.Exchange = "SHFE";
            order.Side = false;
            order.TotalSize = 3;
            order.FilledSize = 0;
            order.UnfilledSize = 0;
            order.LimitPrice = 47000;
            order.StopPrice = 0;
            order.OffsetFlag = API.QSEnumOffsetFlag.OPEN;
            //order.OrderStatus = API.QSEnumOrderStatus.Opened;
            order.ID = "635499244662031251";
            Console.WriteLine("offset:" + order.OffsetFlag.ToString());
            Console.WriteLine("offset:" + (byte)order.OffsetFlag);
            //Console.WriteLine("side:" + order.Side.ToString());
            string localid = _wrapper.SendOrder(ref order);
            Util.Debug("Localid:" + localid,QSEnumDebugLevel.MUST);
            Util.sleep(25000);

            

            

            
        }

        void _wrapper_OnRtnOrderErrorEvent(ref XOrderField pOrder, ref XErrorField pError)
        {
            Util.Debug("order localid:" + pOrder.LocalID + " errorid:" + pError.ErrorID.ToString() + " errmsg:" + pError.ErrorMsg,QSEnumDebugLevel.MUST);
        }

        void _wrapper_OnLoginEvent(ref XRspUserLoginField pRspUserLogin)
        {
            Util.Debug("broker login replly,errorid:" + pRspUserLogin.ErrorID.ToString() + " errormessage:" + pRspUserLogin.ErrorMsg,QSEnumDebugLevel.MUST);
        }

        void _wrapper_OnDisconnectedEvent()
        {
            Util.Debug("TLBroker disconnected...", QSEnumDebugLevel.MUST);
        }

        void _wrapper_OnConnectedEvent()
        {
            Util.Debug("TLBroker connected... try to login", QSEnumDebugLevel.MUST);
            XUserInfoField userinfo = new XUserInfoField();
            userinfo.UserID = "00000021";
            userinfo.Password = "123456";
            //userinfo.UserID = "6666";
            //userinfo.Password = "123456";
            userinfo.Field1 = "1026";
            userinfo.Field2 = "field2";
            _wrapper.Login(ref userinfo);
        }

        void _wrapper_OnRtnOrderEvent(ref XOrderField pOrder)
        {
            Util.Debug("got order reply ???????????????????????????");
            Util.Debug(" data:" + pOrder.Date + " exchange:" + pOrder.Exchange + " filledsize:" + pOrder.FilledSize.ToString() + " limitprice:" + pOrder.LimitPrice + " offsetflag:" + pOrder.OffsetFlag.ToString() + " orderid:" + pOrder.ID + " status:" + pOrder.OrderStatus.ToString() + " side:" + pOrder.Side + " stopprice:" + pOrder.StopPrice.ToString() + " symbol:" + pOrder.Symbol + " totalsize:" + pOrder.TotalSize.ToString() + " unfilledsize:" + pOrder.UnfilledSize.ToString());
        }

        void _wrapper_OnRtnTrade(ref XTradeField pTrade)
        {
            Console.WriteLine("got new trade.. 1");
        }

        void _wrapper_OnRtnTrade2(ref XTradeField pTrade)
        {
            Console.WriteLine("got new trade.. 2");
        }

        void _wrapper_OnRtnTradeEvent(ref XTradeField pTrade)
        {
            Console.WriteLine("got new trade..???????????????????????");

            Util.Debug("tradefield commission:" + pTrade.Commission + " date:" + pTrade.Date.ToString() + " exchange:" + pTrade.Exchange + " offsetflag:" + pTrade.OffsetFlag.ToString() + " price:" + pTrade.Price.ToString() + " side:" + pTrade.Side.ToString() + " size:" + pTrade.Size.ToString() + " symbol:" + pTrade.Symbol + " time:" + pTrade.Time + " tradeid:" + pTrade.TradeID);
        }
    }
}
