using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace WSServiceHost
{
    /// <summary>
    /// 请求处理
    /// 客户端通过WebSocket与服务端通讯 客户端提交请求
    /// 请求格式
    /// //登入
    /// {'msgtype':'LOGINREQUEST','payload':{'username':'demo','password':'demo'},'reqid':1}
    /// {'msgtype':'LOGINRESPONSE','payload':{''}}
    /// </summary>
    public partial class WSServiceHost
    {

    }
}
