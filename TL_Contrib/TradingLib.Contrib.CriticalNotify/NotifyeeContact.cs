using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.NotifyCentre
{
    public class NotifyeeContact
    {
        /// <summary>
        /// 数据库全局ID
        /// </summary>
        public int ID { get;set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 微信号码
        /// </summary>
        public string WeiXin { get; set; }


        /// <summary>
        /// 通知者类别
        /// </summary>
        public EnumNotifeeType NotifeeType { get; set; }

        public bool IsEmailValid
        {
            get
            {
                return true;
            }
        }

        public bool IsMobileValid
        {
            get
            {
                return true;
            }
        }

        public bool IsWeiXinValid
        {
            get
            {
                return true;
            }
        }
    }
}
