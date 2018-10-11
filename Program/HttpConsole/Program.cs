using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpSrv = new ConsoleServer(9090);
            httpSrv.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
