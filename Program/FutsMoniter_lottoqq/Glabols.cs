using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using Common.Logging;

namespace FutsMoniter
{
    
    public partial class Globals
    {
        static ILog logger = LogManager.GetLogger("Moniter");

        public static ConfigFile Config = null;



        static Globals()
        { 
            try
            {
                Config = ConfigFile.GetConfigFile("moniter_lottoqq.cfg");
                Util.SendLogEvent += new ILogItemDel(Util_SendLogEvent);
            }
            catch(Exception ex)
            {
            
            }
                
        }

        static void Util_SendLogEvent(ILogItem log)
        {
            Log(log.Message, log.Level, log.Programe);
        }

        /// <summary>
        /// 登入回报 用于获得登入基本信息
        /// </summary>
        public static MgrLoginResponse LoginResponse { get; set; }

        /// <summary>
        /// 获得全局界面访问权限对象
        /// </summary>
        public static UIAccess UIAccess { get { return LoginResponse.UIAccess; } }

        /// <summary>
        /// 域信息
        /// </summary>
        public static Domain Domain { get { return LoginResponse.Domain; } }

        public static void UpdateDomain(DomainImpl domain)
        {
            if(domain.ID == Globals.LoginResponse.Domain.ID)
                Globals.LoginResponse.Domain = domain;
        }

        /// <summary>
        /// 管理主域ID
        /// </summary>
        public static int? BaseMGRFK { get; set; }

        /// <summary>
        /// 管理ID
        /// </summary>
        public static int? MGRID { get; set; }

        /// <summary>
        /// 管理端对应的对象
        /// </summary>
        public static ManagerSetting Manager { get; set; }


        public static bool RootRight
        {
            get
            {
                if (Manager == null)
                    return false;
                if (Manager.Type == QSEnumManagerType.ROOT)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 路由状态权限
        /// </summary>
        /// <returns></returns>
        public static bool RightRouter
        {
            get
            {
                if (Manager == null)
                    return false;
                if (Manager.Type == QSEnumManagerType.ROOT)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 出入金操作权限
        /// </summary>
        /// <returns></returns>
        public static bool RightCashOperation
        {
            get
            {
                if (Manager == null)
                    return false;
                if (Manager.Type == QSEnumManagerType.ROOT)
                    return true;
                else
                    return false;
            }
        }

        public static bool RightAgent
        {
            get
            {
                if (Manager == null)
                    return false;
                if (Manager.Type == QSEnumManagerType.ROOT)
                    return true;
                else
                    return false;
            }
        }

        public static bool RightAddManger
        {
            get
            {
                if (Manager == null)
                    return false;
                if (Manager.Type == QSEnumManagerType.ROOT)
                    return true;
                if (Manager.Type == QSEnumManagerType.AGENT)
                    return true;
                return false;
            }
        }

        

        /// <summary>
        /// 全局TLClient用于进行客户端调用
        /// 这里可以限制成一定的功能接口
        /// </summary>
        public static TLClientNet TLClient { get { return _client; } }
        static TLClientNet _client;
        public static void RegisterClient(TLClientNet client)
        {
            _client = client;
        }

        //static TradingInfoTracker _infotracker;
        //public static TradingInfoTracker TradingInfoTracker { get { return _infotracker; } }
        //public static void RegisterInfoTracker(TradingInfoTracker tracker)
        //{
        //    _infotracker = tracker;
        //}

        //static IBasicInfo _basicinfo;
        //public static IBasicInfo BasicInfoTracker { get { return _basicinfo; } }
        //public static void RegisterBasicInfoTracker(IBasicInfo basic)
        //{
        //    _basicinfo = basic;
        //}

        //static ICallbackCentre _callbackcentre;
        //public static ICallbackCentre LogicEvent { get { return _callbackcentre; } }
        //public static bool CallbackCentreReady { get { return _callbackcentre != null; } }
        //public static void RegisterCallBackCentre(ICallbackCentre callbackcentre)
        //{
        //    _callbackcentre = callbackcentre;
        //}

        public static TradingInfoTracker TradingInfoTracker { get { return _ctx.TradingInfoTracker; } }

        public static IBasicInfo BasicInfoTracker { get { return _ctx.BasicInfoTracker; } }
        /// <summary>
        /// 逻辑处理对象
        /// </summary>
        public static ILogicHandler LogicHandler { get { return _ctx; } }


        public static ICallbackCentre LogicEvent { get { return _ctx; } }

        static Ctx _ctx;
        internal static void RegisterCTX(Ctx ctx)
        {
            _ctx = ctx;
        }
        




        #region 全局参数部分,软件相关特性依赖于全局参数
        public static int HeaderHeight = 26;
        public static int RowHeight = 24;

        public static System.Drawing.Color LongSideColor = System.Drawing.Color.Crimson;
        public static System.Drawing.Color ShortSideColor = System.Drawing.Color.LimeGreen;

        public static System.Drawing.Font BoldFont = new Font("微软雅黑", 9, FontStyle.Bold);

        public static System.Windows.Forms.Form MainForm = null;


        /// <summary>
        /// 主体名称
        /// </summary>
        //public static string ThemeName = "Office2010Black";
        //public static string ThemeName = "Office2010Silver";

        /// <summary>
        /// 前置窗体
        /// </summary>
        public static bool TopMost = false;

        /// <summary>
        /// 是否启用声音提示
        /// </summary>
        public static bool VoiceEnable = true;



        #endregion


        public static LoginStatus LoginStatus = new LoginStatus(false,"",false);


        public static void Debug(string msg)
        {
            Log(msg, QSEnumDebugLevel.DEBUG);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="program"></param>
        public static void Log(string message, QSEnumDebugLevel level, string program = null)
        {
            if(program != null)
            {
                message = string.Format("{0}:{1}",program,message);
            }
            switch (level)
            { 
                case QSEnumDebugLevel.DEBUG:
                    logger.Debug(message);
                    break;
                case QSEnumDebugLevel.ERROR:
                    logger.Error(message);
                    break;
                case QSEnumDebugLevel.FATAL:
                    logger.Fatal(message);
                    break;
                case QSEnumDebugLevel.INFO:
                    logger.Info(message);
                    break;
                case QSEnumDebugLevel.WARNING:
                    logger.Warn(message);
                    break;
            }
        }

        //public static string CompanyName = "分帐户柜台系统";
        //public static string Version = "1.0.0";

        public static int BasePort = 5570;

        public static string[] GetServers(string srv)
        {
            string[] servers;
            switch (srv)
            {
                case "DX1":
                    servers = new string[] { "access-dx-1.milfut.com", "access-dx-1.milfut.com" };
                    break;
                case "DX2":
                    servers = new string[] { "access-dx-1.milfut.com", "access-dx-1.milfut.com" };
                    break;
                case "BGP":
                    servers = new string[] { "access-bgp.milfut.com" };
                    break;
                case "LOCAL":
                    servers = new string[] { "127.0.0.1" };
                    break;
                case "VPN":
                    servers = new string[] { "logic.milfut.com" };
                    break;
                case "FJ01":
                    servers = new string[] { "58.22.109.73" };
                    break;
                default:
                    servers = new string[] { "logic.milfut.com" };
                    break;
            }
            return servers;
        }

    }

    public struct LoginStatus
    {
        public LoginStatus(bool isreported,string message,bool initsuccess)
        {
            IsReported = false;
            InitMessage = "";
            IsInitSuccess = false;
            needReport = false;
        }

        public void SetInitMessage(string message)
        {
            InitMessage = message;
            IsReported = false;
            needReport = true;
        }
        public void Reset()
        {
            IsReported = false;
            InitMessage = "";
            IsInitSuccess = false;
            needReport = false;
        }
        public bool needReport;
        public bool IsReported;
        public string InitMessage;
        public bool IsInitSuccess;
    }
}
