using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class Email : IEmail
    {
        public string[] Receivers {get;set;}
        public string Subject{get;set;}
        public string Body{get;set;}

        public Email(string subject, string body, string[] receviers)
        {
            Subject = subject;
            Body = body;
            Receivers = receviers;
        }
    }
}
