using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontServer
{
    public  interface IConnectionState
    {
        string LoginID { get; set; }

        bool Authorized { get; set; }
    }
}
