using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Lottoqq.Race
{
    public class RaceError : QSError
    {
        public RaceError(string msg)
            : base(new Exception(), msg)
        {

        }

    }
    /// <summary>
    /// 比赛服务未绑定帐号
    /// </summary>
    public class RaceErrorAccountNull : RaceError
    {
        public RaceErrorAccountNull()
            : base("比赛服务未绑定有效帐号")
        { 
        
        }
    }

    public class RaceErrorServiceLoaded : RaceError
    {
        public RaceErrorServiceLoaded()
            : base("比赛服务已经加载")
        { 
            
        }
    }

    public class RaceServiceNotExit : RaceError
    {
        public RaceServiceNotExit()
            :base("比赛服务不存在")
        { }
    }
    //public class 
}
