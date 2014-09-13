
/*************************************************************************************************
 为整个系统框架定义了基本接口,IDebug 提供日志输出功能 IService 基本服务接口
 Connecter:系统对外发送委托 接收市场数据组件 对接brokerAPI
 Service:系统接收客户端报单,数据请求组件 对接 客户端API
 DataRouter:数据路由 整合多个IDataFeed
 BrokerRouter:下单路由 整合多个IBroker
 TradingServer:通过整合Service,DataRouter,BrokerRouter实现客户端 broker 之间的双向转发
 ClearCentre:清算中心,记录所有客户端交易信息与记录
 RiskCentre:风控中心,为所有的客户端建立风险控制规则并实时监控
 
**************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace TradingLib.API
{
	public interface IMail
	{
		event EmailDel SendEmailEvent;
	}

	/// <summary>
	/// debug接口,组件实现了这个接口可以对外输出日志
	/// </summary>
	public interface IDebug
	{
		/// <summary>
		/// 日志级别
		/// </summary>
		QSEnumDebugLevel DebugLevel { get; set; }

		/// <summary>
		/// 是否输出日志
		/// </summary>
		bool DebugEnable { get; set; }

		/// <summary>
		/// 打开日志输出功能
		/// </summary>
		bool VerboseDebugging { get; set; }

		/// <summary>
		/// 对外输出日志信息
		/// </summary>
		event DebugDelegate SendDebugEvent;
	}

	/// <summary>
	/// 服务类的基本接口 启动 停止 是否可以用
	/// </summary>
	public interface IService
	{
		/// <summary>
		/// 当前服务状态是否有效
		/// </summary>
		bool IsLive { get; }

		/// <summary>
		/// 启动服务
		/// </summary>
		void Start ();

		/// <summary>
		/// 停止服务
		/// </summary>
		void Stop ();
	}
}
