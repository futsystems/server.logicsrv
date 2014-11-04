using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantBox.XAPI;
using QuantBox;
using QuantBox.XAPI.Callback;




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
            Queue queue = new Queue(@"lib\TLQueue.dll");

            debug("queue is inited");
        }
    }
}
