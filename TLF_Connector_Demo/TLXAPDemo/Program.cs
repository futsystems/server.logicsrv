using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TradingLib.BrokerXAPI.Interop;
using TradingLib.BrokerXAPI;


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

            TLBroker broker = new TLBroker();
        }
    }
}
