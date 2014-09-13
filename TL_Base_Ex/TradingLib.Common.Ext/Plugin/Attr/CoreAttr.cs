using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{

    [AttributeUsage(AttributeTargets.Class)]//该特性只能用于类
    public class CoreAttr : TLObjectAttribute
    {
        string _coreId;
        /// <summary>
        /// 核心模块的CoreID
        /// </summary>
        public string CoreId { get { return _coreId; } }

        public CoreAttr(string coreid,string name,string description)
            : base(name, "", description, "TLF", "1.0.0", "Qianbo")
        {
            _coreId = coreid;
        }
    }
}
