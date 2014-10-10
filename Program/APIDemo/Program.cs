using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace APIDemo
{
    public delegate void DebugDel(string msg);
    static class Program
    {
        public static event DebugDel SendDebugEvent;
        public static void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
