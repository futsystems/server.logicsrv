using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public class ContribEventAttr:TLAttribute
    {
        /// <summary>
        /// 目标扩展模块
        /// </summary>
        public string DestContrib { get { return _destContrib; } }
        string _destContrib;
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventStr { get { return _event; } }
        string _event;

        /// <summary>
        /// 帮助
        /// </summary>
        public string Help { get { return _help; } }
        string _help;
        /// <summary>
        /// 事件描述
        /// </summary>
        public string Description { get { return _description; } }
        string _description;

        public ContribEventAttr(string contrib,string sevent,string help,string description)
        {
            _destContrib = contrib;
            _event = sevent;
            _help = help;
            _description = description;
        }

    }
}
