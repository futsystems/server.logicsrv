using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontServer
{
    public  interface IConnectionState
    {
        /// <summary>
        /// IP地址
        /// </summary>
        string IPAddress { get; set; }

        string LoginID { get; set; }

        bool Authorized { get; set; }
    }
}
