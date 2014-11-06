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


            Util.sleep(500000);
        }
    }
}
