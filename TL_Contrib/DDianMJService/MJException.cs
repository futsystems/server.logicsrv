using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Lottoqq.MJService
{
    public class MJErrorAccountNull:QSError
    {
        public MJErrorAccountNull()
            : base(new Exception(), "秘籍服务未设定有效帐号")
        { 
            
        }

        
    }

    public class MJErrorMJServiceLoaded : QSError
    {
        public MJErrorMJServiceLoaded()
            : base(new Exception(), "已经加载该帐户的秘籍服务")
        {
            
        }
    }

    public class MJErrorMJServiceNotExist : QSError
    {
        public MJErrorMJServiceNotExist()
            : base(new Exception(), "帐户没有绑定有效秘籍服务")
        { 
        
        }
    }

    public class MJError : QSError
    {
        public MJError(string msg)
            : base(new Exception(), msg)
        { 
            
        }
        
    }
}
