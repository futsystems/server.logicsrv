using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using TradingLib.API;
using TradingLib.Common;


namespace TraddingSrvCLI
{
    public class demoJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Util.Debug("it is executed to here");
        }
    }
}
