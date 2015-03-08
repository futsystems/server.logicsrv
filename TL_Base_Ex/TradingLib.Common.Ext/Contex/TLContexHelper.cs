using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Reflection;

namespace TradingLib.Common
{
    /// <summary>
    /// 单例全局上下文
    /// </summary>
    public class TLCtxHelper:IDisposable
    {
        private static TLCtxHelper defaultInstance;
        private TLContext ctx;

        /// <summary>
        /// 交易类事件与消息
        /// </summary>
        private IndicatorEvent m_IndicatorEvent;

        /// <summary>
        /// 回话类 注册 注销 登入
        /// </summary>
        private SessionEvent<TrdClientInfo> m_SessionEvent;

        /// <summary>
        /// 帐户类事件
        /// </summary>
        private AccountEvent m_AccountEvent;

        /// <summary>
        /// 扩展事件
        /// </summary>
        private ExContribEvent m_ExContribEvent;

        /// <summary>
        /// 系统类事件
        /// </summary>
        private SystemEvent m_SystemEvent;


        private IUtil m_util;

        public static bool IsReady { get; set; }

        static TLVersion _version=null;
        /// <summary>
        /// 版本信息
        /// </summary>
        public static TLVersion Version 
        {
            get
            {
                if (_version == null)
                {
                    _version = ORM.MSystem.GetVersion();
                }
                return _version;
            }
            
        }

        public static void PrintVersion()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("".PadLeft(Util.GetAvabileConsoleWidth() / 2 - 1, '.'));
            //Version:0.65
            Util.ConsoleColorStatus(string.Format(". Version:{0}",Version.Version), ".", QSEnumInfoColor.INFOGREEN, QSEnumInfoColor.INFOGREEN);
            Util.ConsoleColorStatus(string.Format(". Build:{0}", Version.BuildNum), ".", QSEnumInfoColor.INFOGREEN, QSEnumInfoColor.INFOGREEN);
            
            Util.ConsoleColorStatus(string.Format(". LastUpdate:{0}", "20141123"), ".", QSEnumInfoColor.INFOGREEN, QSEnumInfoColor.INFOGREEN);
            Util.ConsoleColorStatus(string.Format(". Author:{0}", "QianBo"), ".", QSEnumInfoColor.INFOGREEN, QSEnumInfoColor.INFOGREEN);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("".PadLeft(Util.GetAvabileConsoleWidth() / 2 - 1, '.'));
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        static TLCtxHelper()
        {
            defaultInstance = new TLCtxHelper();
            IsReady = false;
        }

        public TLCtxHelper()
        {
            this.ctx = new TLContext();
            this.m_IndicatorEvent = new IndicatorEvent();
            this.m_SessionEvent = new SessionEvent<TrdClientInfo>();
            this.m_AccountEvent = new AccountEvent();
            this.m_ExContribEvent = new ExContribEvent();
        }

        public void Dispose()
        { 
        
        }

        public static void DisposeInstance()
        {
            if (defaultInstance != null)
            {
                defaultInstance.ctx = null;
                defaultInstance.m_AccountEvent = null;
                defaultInstance.m_ExContribEvent = null;
                defaultInstance.m_IndicatorEvent = null;
                defaultInstance.m_SessionEvent = null;
            }
        }

        public static TLContext Ctx
        {
            get
            {
                if (defaultInstance.ctx == null)
                    defaultInstance.ctx = new TLContext();
                return defaultInstance.ctx;
            }
        }

        /// <summary>
        /// 交易信息类事件集合
        /// </summary>
        public static IndicatorEvent EventIndicator
        {
            get
            {
                if (defaultInstance.m_IndicatorEvent == null)
                    defaultInstance.m_IndicatorEvent = new IndicatorEvent();
                return defaultInstance.m_IndicatorEvent;
            }
        }

        /// <summary>
        /// 会话类事件集合
        /// </summary>
        public static SessionEvent<TrdClientInfo> EventSession
        {
            get
            {
                if (defaultInstance.m_SessionEvent == null)
                    defaultInstance.m_SessionEvent = new SessionEvent<TrdClientInfo>();
                return defaultInstance.m_SessionEvent;
            }
        }

        /// <summary>
        /// 交易帐户类事件
        /// </summary>
        public static AccountEvent EventAccount
        {
            get
            {
                if (defaultInstance.m_AccountEvent == null)
                    defaultInstance.m_AccountEvent = new AccountEvent();
                return defaultInstance.m_AccountEvent;
            }
        }

        /// <summary>
        /// 系统类事件
        /// </summary>
        public static SystemEvent EventSystem
        {
            get
            {
                if (defaultInstance.m_SystemEvent == null)
                    defaultInstance.m_SystemEvent = new SystemEvent();
                return defaultInstance.m_SystemEvent;
            }
        }

        /// <summary>
        /// 扩展模块强关系事件
        /// </summary>
        public static ExContribEvent ExContribEvent
        {
            get
            {
                if (defaultInstance.m_ExContribEvent == null)
                    defaultInstance.m_ExContribEvent = new ExContribEvent();
                return defaultInstance.m_ExContribEvent;
            }
        }

        /// <summary>
        /// 交易帐号类操作
        /// </summary>
        public static IAccountOperation CmdAccount
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IAccountOperation;
            }
        }


        public static ITotalAccountInfo CmdTotalInfo
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as ITotalAccountInfo;
            }
        }

        internal static IGotTradingRecord CmdGotTradingRecord
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IGotTradingRecord;
            }
        }
        /// <summary>
        /// 认证与出入金请求
        /// </summary>
        public static IAuthCashOperation CmdAuthCashOperation
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IAuthCashOperation;
            }
        }

        /// <summary>
        /// 结算中心
        /// </summary>
        public static ISettleCentre CmdSettleCentre
        {
            get
            {
                return defaultInstance.ctx.SettleCentre as ISettleCentre;
            }
        }


        /// <summary>
        /// 风控中心
        /// </summary>
        public static IRiskCentre CmdRiskCentre
        {
            get
            {
                return defaultInstance.ctx.RiskCentre as IRiskCentre;
            }
        }

        /// <summary>
        /// 辅助类操作函数
        /// </summary>
        public static IUtil CmdUtils
        {
            get
            {
                if (defaultInstance.m_util == null)
                    defaultInstance.m_util = new CoreUtil();
                return defaultInstance.m_util;
            }
        }

        /// <summary>
        /// 系统交易路由管理器
        /// </summary>
        public static IBrokerRouter BrokerRouter
        {
            get
            {
                return defaultInstance.ctx.BrokerRouter as IBrokerRouter;
            }
        }

        /// <summary>
        /// 系统加载完毕后绑定扩展模块的事件
        /// </summary>
        public static void BindContribEvent()
        {
            defaultInstance.ctx.BindContribEvent();
        }


        #region 【全局日志 通知】
        public static Profiler Profiler = new Profiler();


        /// <summary>
        /// 全局发送邮件事件
        /// 绑定该事件可以获得系统所有对象的Email发送事件
        /// </summary>
        public static event EmailDel SendEmailEvent = null;

        /// <summary>
        /// 全局发送邮件入口
        /// </summary>
        /// <param name="email"></param>
        public static void Email(IEmail email)
        {
            if (SendEmailEvent != null)
                SendEmailEvent(email);
        }
        #endregion


    }
}
