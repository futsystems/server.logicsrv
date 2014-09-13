using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// IQService定义了系统可以使用的服务插件,用于获得实时数据,历史数据,成交接口等
    /// </summary>
    public interface IQService : IDisposable
    {
        // Events
        event EventHandler<ServiceEventArgs> ServiceEvent;
        event DebugDelegate SendDebugEvent;
        /// <summary>
        /// GUID编号
        /// </summary>
        /// <returns></returns>
        string id();//GUID
        /// <summary>
        /// 版本号
        /// </summary>
        /// <returns></returns>
        string Version();
        
        /// <summary>
        /// 作者
        /// </summary>
        /// <returns></returns>
        string Author();
        /// <summary>
        /// 公司名
        /// </summary>
        /// <returns></returns>
        string CompanyName();
        /// <summary>
        /// 服务名
        /// </summary>
        /// <returns></returns>
        string ServiceName();
        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        string Description();
        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="connectOptions"></param>
        /// <returns></returns>
        bool Connect(ServiceConnectOptions connectOptions);//连接
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        bool Disconnect();//断开
        /// <summary>
        /// 获得历史数据接口
        /// </summary>
        /// <returns></returns>
        IBarDataRetrieval GetBarDataInterface();//历史数据
        /// <summary>
        /// 获得实时数据接口
        /// </summary>
        /// <returns></returns>
        ITickRetrieval GetTickDataInterface();//Tick数据
        /// <summary>
        /// 获得成交接口
        /// </summary>
        /// <returns></returns>
        IExBroker GetBrokerInterface();//成交接口

        /// <summary>
        /// 获得当前错误信息
        /// </summary>
        /// <returns></returns>
        string GetError();
        
        //bool HasCustomSettings();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool Initialize(SerializableDictionary<string, string> settings);

        /// <summary>
        /// 是否需要认证
        /// </summary>
        /// <returns></returns>
        bool NeedsAuthentication();
        bool NeedsPort();
        bool NeedsServerAddress();

        /// <summary>
        /// 显示设置窗口
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool ShowCustomSettingsForm(ref SerializableDictionary<string, string> settings);
        /// <summary>
        /// 是否支持多个实例
        /// </summary>
        /// <returns></returns>
        bool SupportsMultipleInstances();
        

        // Properties
        /// <summary>
        /// 是否支持历史数据
        /// </summary>
        bool HisDataAvailable { get; }//是否支持历史数据
        /// <summary>
        /// 是否支持实时数据
        /// </summary>
        bool TickDataAvailable { get; }//是否支持Tick数据
        /// <summary>
        /// 是否支持成交接口
        /// </summary>
        bool BrokerExecutionAvailable { get; }//是否支持成交

        
        /// <summary>
        /// 服务端地址
        /// </summary>
        string ServerAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; set; }
    }

 

 

}
