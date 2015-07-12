using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{

    /// <summary>
    /// 信号基类
    /// 信号系统支持多种信号类型
    /// 1.CTP接口信号 信号对象通过绑定CTP信号通道实现信号采集
    /// 2.资管分账户信号 信号对象通过绑定资管系统帐号实现对应交易信号的采集
    /// </summary>
    public class SignalFactory
    {
        /// <summary>
        /// 通过信号配置对象创建对应的信号对象
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static ISignal CreateSignal(SignalConfig cfg)
        {
            switch (cfg.SignalType)
            {
                case QSEnumSignalType.Account:
                    {
                        SignalWrapperAccount sig = new SignalWrapperAccount();
                        sig.Init(cfg);
                        return sig;
                    }
                case QSEnumSignalType.Connector:
                    {
                        return null;
                    }
                default :
                    return null;
                
            }
        }
    }
}
