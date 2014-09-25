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

        private IndicatorEvent m_IndicatorEvent;
        private SessionEvent<TrdClientInfo> m_SessionEvent;
        private AccountEvent m_AccountEvent;
        private ExContribEvent m_ExContribEvent;


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
        }

        public static void Release()
        {
            defaultInstance.ctx = null;
            defaultInstance.m_AccountEvent = null;
            defaultInstance.m_ExContribEvent = null;
            defaultInstance.m_IndicatorEvent = null;
            defaultInstance.m_SessionEvent = null;
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

        public static IAccountOperation CmdAccount
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IAccountOperation;
            }
        }

        //public static IAccountTradingInfo CmdTradingInfo
        //{
        //    get
        //    {
        //        return defaultInstance.ctx.ClearCentre as IAccountTradingInfo;
        //    }
        //}

        public static IAccountOperationCritical CmdAccountCritical
        {
            get
            {
                return defaultInstance.ctx.ClearCentre as IAccountOperationCritical;
            }
        }


        public static ISettleCentre CmdSettleCentre
        {
            get
            {
                return defaultInstance.ctx.SettleCentre as ISettleCentre;
            }
        }

        //public static IClearCentreOperation CmdClearCentre
        //{
        //    get
        //    {
        //        //TLCtxHelper.Debug("ClearCenter XXXX :" + (defaultInstance.ctx.ClearCentre is IClearCentreOperation).ToString());
        //        //return defaultInstance.ctx.ClearCentre as IClearCentreOperation;
        //        return defaultInstance.ctx.ClearCentre as IClearCentreOperation;
        //    }
        //}

        public static IRiskCentre CmdRiskCentre
        {
            get
            {
                return defaultInstance.ctx.RiskCentre as IRiskCentre;
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
        public static event DebugDelegate SendDebugEvent = null;
        /// <summary>
        /// 全局标准输出入口,用于在屏幕或者信息面板输出系统内的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        /// <summary>
        /// 全局日志事件
        /// 绑定该事件可以获得系统所有对象的log输出
        /// </summary>
        public static event LogDelegate SendLogEvent = null;
        /// <summary>
        /// 全局日志入口,用于向日志保存分发系统写入日志
        /// </summary>
        /// <param name="objname">产生日志的对象</param>
        /// <param name="msg">消息</param>
        /// <param name="level">级别</param>
        public static void Log(string objname, string msg, QSEnumDebugLevel level)
        {
            if (SendLogEvent != null)
                SendLogEvent(objname, msg, level);
        }

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
