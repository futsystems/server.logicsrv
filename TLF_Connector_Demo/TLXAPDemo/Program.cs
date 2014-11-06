using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TradingLib.BrokerXAPI.Interop;
using TradingLib.BrokerXAPI;
using TradingLib.API;
using TradingLib.Common;


namespace TLXAPDemo
{
    class Program
    {
        static void debug(string msg)
        {
            Console.WriteLine(msg);
        }
        static void Main(string[] args)
        {
            /*
            TLBrokerWrapperProxy broker = new TLBrokerWrapperProxy("lib", "TLBrokerCTP");

            int result = broker.democall();
            debug("get result:" + result.ToString());

            string strret = broker.demostringcall("it is ok here");
            debug("get result:" + strret);

            int xret = broker.demointcall(20);
            debug("get result:" + xret.ToString());

            ErrorField field = new ErrorField();
            debug("size of errorfield:" + Marshal.SizeOf(typeof(ErrorField)).ToString());
            field.ErrorID = 10;
            field.ErrorMsg = "error_message";

            field.ErrorMsg2 = "itisokapple";

            broker.demostructcall(ref field);
             * */
            //TLBrokerWrapperProxy wrapper = new TLBrokerWrapperProxy(@"libbroker\", "TLBrokerWrapper.dll");

            //wrapper.Dispose();

            //bool valid = XAPIHelper.ValidBrokerInterface(@"libbroker\CTP630\", "TLBrokerCTP.dll", @"libbroker\", "TLBrokerWrapper.dll");

            //Util.Debug("valid :" + valid.ToString());

            //Util.sleep(10000);
            //TLBrokerProxy broker = new TLBrokerProxy(@"libbroker\CTP630\", "TLBrokerCTP.dll");
            //broker.DestoryBroker(broker.Handle);
            while (true)
            {
                TLBroker broker = new TLBroker(@"libbroker\CTP630\", "TLBrokerCTP.dll");
                broker.SendLogItemEvent += new ILogItemDel(Util.Log);

                XServerInfoField srvinfo = new XServerInfoField();
                srvinfo.ServerAddress = "182.131.17.110";
                srvinfo.ServerPort = 62205;
                srvinfo.Field1 = "16377";//BrokerID
                //srvinfo.ServerAddress = "gw-release-ctcc1.lottoqq.com";
                //srvinfo.ServerPort = 40255;
                srvinfo.Field1 = "8000";//BrokerID
                srvinfo.Field2 = "xo";
                srvinfo.Field3 = "helloworld";

                broker.SetServerInfo(srvinfo);

                XUserInfoField userinfo = new XUserInfoField();
                userinfo.UserID = "70108058";
                userinfo.Password = "123456";
                //userinfo.UserID = "6666";
                //userinfo.Password = "123456";
                userinfo.Field1 = "16377";
                userinfo.Field2 = "field2";

                broker.SetUserInfo(userinfo);

                broker.Start();

                Util.sleep(2000);
                broker.Dispose();
                broker = null;
            }
            //Util.sleep(5000);
            //Util.Debug("Release broker.....");
            //broker.Dispose();

            if (true)
            {
                TLBroker broker = new TLBroker(@"libbroker\CTP630\", "TLBrokerCTP.dll");

                broker.SendLogItemEvent += new ILogItemDel(Util.Log);

                XServerInfoField srvinfo = new XServerInfoField();
                srvinfo.ServerAddress = "182.131.17.110";
                srvinfo.ServerPort = 62205;
                srvinfo.Field1 = "16377";//BrokerID
                //srvinfo.ServerAddress = "gw-release-ctcc1.lottoqq.com";
                //srvinfo.ServerPort = 40255;
                srvinfo.Field1 = "8000";//BrokerID
                srvinfo.Field2 = "xo";
                srvinfo.Field3 = "helloworld";

                broker.SetServerInfo(srvinfo);

                XUserInfoField userinfo = new XUserInfoField();
                userinfo.UserID = "70108058";
                userinfo.Password = "123456";
                //userinfo.UserID = "6666";
                //userinfo.Password = "123456";
                userinfo.Field1 = "16377";
                userinfo.Field2 = "field2";

                broker.SetUserInfo(userinfo);

                broker.Start();

                Util.sleep(5000);
                //Order order = new OrderImpl();
                //order.Account="xxxx";
                //order.Exchange = "CFFEX";
                //order.OffsetFlag = QSEnumOffsetFlag.OPEN;
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
                order.OffsetFlag = QSEnumOffsetFlag.OPEN;
                //order.OrderStatus = API.QSEnumOrderStatus.Opened;
                order.ID = "635499244662031251";
                Console.WriteLine("offset:" + order.OffsetFlag.ToString());
                Console.WriteLine("offset:" + (byte)order.OffsetFlag);
                //Console.WriteLine("side:" + order.Side.ToString());
                string localid = broker._wrapper.SendOrder(ref order);
                //broker.SendOrder()
            }
            Util.sleep(500000);
        }
    }
}
