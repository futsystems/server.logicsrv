using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    
    public class Globals
    {
        public static ConfigFile Config = null;

        /// <summary>
        /// 当前状态是否就绪
        /// 用于初始化过程中过滤界面的相关操作
        /// 比如在tlclient未初始化时候进行请求操作等
        /// </summary>
        public static bool EnvReady = false;

        static Globals()
        { 
            try
            {
                Config = ConfigFile.GetConfigFile("moniter.cfg");
            }
            catch(Exception ex)
            {
            
            }
                
        }
        /// <summary>
        /// 管理端对因的managerID
        /// </summary>
        public static int? MgrFK { get; set; }

        /// <summary>
        /// 管理端对应的对象
        /// </summary>
        public static Manager Manager { get; set; }


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

        static TradingInfoTracker _infotracker;
        public static TradingInfoTracker TradingInfoTracker { get { return _infotracker; } }
        public static void RegisterInfoTracker(TradingInfoTracker tracker)
        {
            _infotracker = tracker;
        }

        static IBasicInfo _basicinfo;
        public static IBasicInfo BasicInfoTracker { get { return _basicinfo; } }
        public static void RegisterBasicInfoTracker(IBasicInfo basic)
        {
            _basicinfo = basic;
        }

        static ICallbackCentre _callbackcentre;
        public static ICallbackCentre CallBackCentre { get { return _callbackcentre; } }

        public static void RegisterCallBackCentre(ICallbackCentre callbackcentre)
        {
            _callbackcentre = callbackcentre;
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

        public static event DebugDelegate SendDebugEvent;
        public static void Debug(string msg)
        {
            //logger.GotDebug(msg);
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
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
