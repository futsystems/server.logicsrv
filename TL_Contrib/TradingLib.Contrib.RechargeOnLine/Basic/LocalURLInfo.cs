using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.RechargeOnLine
{
    public class LocalURLInfo
    {
        public LocalURLInfo(string baseurl, string pagepath, string notifypath)
        {
            BaseURL = baseurl;
            PagePath = pagepath;
            NotifyPath = notifypath;
        }
        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseURL { get; set; }

        /// <summary>
        /// 客户端页面跳转访问路径
        /// </summary>
        public string PagePath { get; set; }


        /// <summary>
        /// 服务端通知访问路径
        /// </summary>
        public string NotifyPath { get; set; }

        public string PageURL 
        { 
            get 
            {
                return BaseURL + PagePath;
            }
        }

        public string NotifyURL
        {
            get
            {
                return BaseURL + NotifyPath;
            }
        }

    }
}
