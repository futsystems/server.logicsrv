using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class ExCoreNoTrading : ExCore, IModuleExCore
    {
        const string CoreName = "MsgExch";

        public ExCoreNoTrading()
            : base(ExCoreNoTrading.CoreName)
        {


        }


        public void Start()
        { 
        
        }

        public void Stop()
        { 
        
        }


    }
        

}
