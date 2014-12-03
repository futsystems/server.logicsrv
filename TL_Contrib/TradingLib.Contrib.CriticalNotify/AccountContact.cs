using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Contrib.NotifyCentre
{
    /// <summary>
    /// 交易帐户通知信息对象
    /// 用于记录某个交易帐户的通知信息
    /// 比如电子邮件 手机号码 微信号等
    /// </summary>
    public class AccountContact
    {
        public AccountContact()
        {
            this.Account = string.Empty;
            this.Email = string.Empty;
            this.Mobile = string.Empty;
            this.WeiXin = string.Empty;
        }

        public AccountContact(string account)
        {
            this.Account = account;
            this.Email = string.Empty;
            this.Mobile = string.Empty;
            this.WeiXin = string.Empty;
        }
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

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
