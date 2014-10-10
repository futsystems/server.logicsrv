using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IEmail
    {
        string[] Receivers{get;set;}
        string Subject{get;set;}
        string Body{get;set;}
    }
}
