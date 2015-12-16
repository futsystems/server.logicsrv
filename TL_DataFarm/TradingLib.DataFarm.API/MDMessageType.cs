using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.DataFarm.API
{
    /// <summary>
    /// MessageType可以通过强制转换的形式进行扩展 避免频繁修改TradingLib.API中的MessageTypes
    /// 扩展消息使用10000,-10000之外的整数 (-10000,10000)之内底层库保留
    /// </summary>
    public class MDMessageType
    {
        //行情登入
        public MessageTypes MD_LoginRequest = (MessageTypes)10000;
        public MessageTypes MD_LoginResponse = (MessageTypes) (-10000);

        

        //订阅合约实时行情
        public MessageTypes MD_RegisterSymbolRequest = (MessageTypes)10001;
        public MessageTypes MD_RegisterSymbolResponse = (MessageTypes) (-10001);

        //查询历史k线数据
        public MessageTypes MD_QryHistBarRequest = (MessageTypes)10002;
        public MessageTypes MD_QryHistBarResponse = (MessageTypes)(-10002);



    }
}
