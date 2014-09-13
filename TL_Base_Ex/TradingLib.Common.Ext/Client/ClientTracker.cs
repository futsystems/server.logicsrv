using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 客户端跟踪器
    /// 用于记录客户端的活动情况
    /// 记录Account对应的当天登入次数/机器码/IP地址等
    /// </summary>
    public class ClientTracker
    {


    }

    /// <summary>
    /// 记录客户端行为信息
    /// </summary>
    public class ClientTrackerInfo
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string Account{get;set;}
        /// <summary>
        /// 登入次数
        /// </summary>
        public int LoginNum { get; set; }
        /// <summary>
        /// 硬件代码
        /// </summary>
        public string HardWareCode { get; set; }
        /// <summary>
        /// 外网地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 当前登入的潜质ID
        /// </summary>
        public string FrontID { get; set; }

        public string API_Type { get; set; }

        public string API_Version { get; set; }

        /// <summary>
        /// 在线时长
        /// </summary>
        public long OnlineSecend { get; set; }//在线秒数
        /// <summary>
        /// 登入时间
        /// </summary>
        public DateTime LoginTime { get; set; }//登入时间 用于计算在线时间
        /// <summary>
        /// 当日所有时长
        /// </summary>
        public TimeSpan TotalTime
        {
            get {
                if (LoginTime != DateTime.MinValue && IsLogined)
                    return new TimeSpan(0, 0, (int)OnlineSecend + (int)(DateTime.Now - LoginTime).TotalSeconds);
                else
                    return new TimeSpan(0, 0, (int)OnlineSecend);
            }
        }
        /// <summary>
        /// 是否有相同IP
        /// </summary>
        public bool IsSameIP { get; set; }
        /// <summary>
        /// 是否有相同硬件代码
        /// </summary>
        public bool IsSameHardCode { get; set; }
        /// <summary>
        /// 是否登入
        /// </summary>
        public bool IsLogined { get; set; }


        ClientInfoBase _info;
        public ClientInfoBase ClientInfo { get { return _info; } }
        public void Reset()
        {
            LoginNum = 0;
            HardWareCode = "";
            IPAddress = "";
            OnlineSecend = 0;
            LoginTime = DateTime.MinValue;
            IsLogined = false;
            IsSameIP = false;
            IsSameHardCode = false;
        }

        public ClientTrackerInfo(string account,ClientInfoBase info)
        {
            Account = account;
            LoginNum = 0;
            HardWareCode = "";
            IPAddress = "";
            OnlineSecend = 0;
            LoginTime = DateTime.MinValue;
            IsLogined = false;
            IsSameIP = false;
            IsSameHardCode = false;
            API_Type = "Win";
            API_Version = "2.4.0";
            _info = info;
        }
    }
}
