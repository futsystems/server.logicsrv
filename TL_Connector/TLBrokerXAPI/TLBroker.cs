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
            _broker = new TLBrokerProxy(@"lib\", @"TLBrokerCTP.dll");
            _wrapper.Register(_broker.Handle);



            XServerInfoField srvinfo = new XServerInfoField();
            srvinfo.ServerAddress = "22.33.44.55";
            srvinfo.ServerPort = 3556;
            srvinfo.Field1 = "8000";
            srvinfo.Field2 = "xo";
            srvinfo.Field3="helloworld";

            _wrapper.Connect(ref srvinfo);

            XUserInfoField userinfo = new XUserInfoField();
            userinfo.UserID = "username";
            userinfo.Password = "password";
            userinfo.Field1 = "field1";
            userinfo.Field2 = "field2";
            _wrapper.Login(ref userinfo);

            _wrapper.OnRtnTradeEvent +=new CBRtnTrade(_wrapper_OnRtnTradeEvent);

            XOrderField order = new XOrderField();
            Console.WriteLine("order size:" + Marshal.SizeOf(typeof(XOrderField)).ToString());
            order.Date = 123234;
            order.Time = 54321;
            order.Symbol = "if1411";
            order.Exchange = "cffex";
            order.Side = true;
            order.TotalSize = 10;
            order.FilledSize = 6;
            order.UnfilledSize = 4;
            order.LimitPrice = 245.555;
            order.StopPrice = 321.333;
            order.OffsetFlag = API.QSEnumOffsetFlag.OPEN;
            order.OrderStatus = API.QSEnumOrderStatus.Opened;
            order.OrderID = "xyaerdfew32432dewrer";
            Console.WriteLine("offset:" + order.OffsetFlag.ToString());
            Console.WriteLine("offset:" + (byte)order.OffsetFlag);
            //Console.WriteLine("side:" + order.Side.ToString());
            _wrapper.SendOrder(ref order);
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
