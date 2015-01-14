using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class MgrClientInfo:ClientInfoBase
    {
        public MgrClientInfo()
            :base()
        {
            this.Manager = null;
        }

        public override void BindState(object obj)
        {
            if (obj != null && obj is Manager)
            {
                this.Manager = obj as Manager;
                this.Authorized = true;
            }
            else
            {
                this.Authorized = false;
            }
        }
        

        public Manager Manager { get; private set; }

    }
}
