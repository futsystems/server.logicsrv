using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using CTPService;

namespace FrontServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //int size = Marshal.SizeOf(CTPService.FTDC_Header);
            CTPServiceHost host = new CTPServiceHost();
            host.Start();

            System.Threading.Thread.Sleep(int.MaxValue);
        }
    }
}
