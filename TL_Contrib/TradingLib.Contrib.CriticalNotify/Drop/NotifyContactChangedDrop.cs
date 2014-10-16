using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using DotLiquid;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Contrib.NotifyCentre
{
    public class AccountContactChangedDrop: EmailDrop
    {
        public string _account = string.Empty;
        AccountContact _ct = null;
        public AccountContactChangedDrop(string account)
            :base("AccountContactChange")
        {
            _account = account;
            _ct = ContactTracker.GetAccountContract(account);
        }

        public string Account { get { return _ct.Account; } }

        public string Email { get { return _ct.Email; } }

        public string Mobile { get { return _ct.Mobile; } }


        public string WeiXin { get { return _ct.WeiXin; } }


        public override EnumNotifeeType[] GetNotifyTargets()
        {
            return new EnumNotifeeType[] { EnumNotifeeType.Account };
        }


        public override string[] GetNotifyList(EnumNotifeeType type)
        {
            switch (type)
            {
                case EnumNotifeeType.Account:
                    {
                        //获得交易帐户的邮件地址
                        if (_ct == null) return null;
                        if (_ct.IsEmailValid)
                        {
                            return new string[] { _ct.Email };
                        }
                        else
                        {
                            return null;
                        }
                    }
                default:
                    return null;
            }
        }


    }
}
