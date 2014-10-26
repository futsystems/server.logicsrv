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
    public class TLCtxHelper
    {
        private static TLCtxHelper defaultInstance;
        private TLContext ctx;

        private IndicatorEvent m_IndicatorEvent;//交易信息类
        private SessionEvent<TrdClientInfo> m_SessionEvent;
        private AccountEvent m_AccountEvent;
        private ExContribEvent m_ExContribEvent;
        private CashOperationEvent m_CashOperationEvent;
        private SystemEvent m_SystemEvent;

        private IUtil m_util;
        public static bool IsReady { get; set; }

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
            this.m_CashOperationEvent = new CashOperationEvent();
        }

        public static void Release()
        {
            defaultInstance.ctx = null;
            defaultInstance.m_AccountEvent = null;
            defaultInstance.m_ExContribEvent = null;
            defaultInstance.m_IndicatorEvent = null;
            defaultInstance.m_SessionEvent = null;
            defaultInstance.m_CashOperationEvent = null;
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
        /// 出入金请求操作事件
        /// </summary>
        public static CashOperationEvent CashOperationEvent
        {
            get
            {
                if (defaultInstance.m_CashOperationEvent == null)
                    defaultInstance.m_CashOperationEvent = new CashOperationEvent();
                return defaultInstance.m_CashOperationEvent;
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


        /// <summary>
        /// 交易帐号 操作
        /// </summary>
        public static IAccountOperationCritical CmdAccountCritical
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IAccountOperationCritical;
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
        public static IUtil CmdUtil
        {
            get
            {
                if (defaultInstance.m_util == null)
                    defaultInstance.m_util = new CoreUtil();
                return defaultInstance.m_util;
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
        /// <summary>
        /// 初始化全局标准输出入口
        /// </summary>
        /// <param name="debug"></param>
        //public static event DebugDelegate SendDebugEvent = null;


        /// <summary>
        /// 全局标准输出入口,用于在屏幕或者信息面板输出系统内的日志信息
        /// </summary>
        /// <param name="msg"></param>
        //public static void Debug(string msg)
        //{
        //    Util.Debug(msg);
        //}

        public static Profiler Profiler = new Profiler();

        /// <summary>
        /// 全局日志事件
        /// 绑定该事件可以获得系统所有对象的log输出
        /// </summary>
        //public static event ILogItemDel SendLogEvent = null;
        //static bool _consoleEnable = true;
        //public static bool ConsoleEnable { get { return _consoleEnable; } set { _consoleEnable = value; } }
        //public static void Log(ILogItem item)
        //{
        //    Util.Log(item);
        //}

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
