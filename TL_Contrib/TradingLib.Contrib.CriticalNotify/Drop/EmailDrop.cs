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
    internal class EmailTempleArg
    {

        public EmailTempleArg(string template, EnumNotifeeType type)
        {
            this.NotifeeType = type;
            this.BodyTPL = template + "_email" + "_" + type.ToString().ToLower() + "_body";
            this.SubjectTPL = template + "_email" + "_" + type.ToString().ToLower() + "_subject";
        }
        public EnumNotifeeType NotifeeType { get;private set; }

        public string SubjectTPL { get; private set; }

        public string BodyTPL { get; private set; }
    }
    /// <summary>
    /// 对应的通知通过 xxx_body xxx_subject来制定标题和内容的模板
    /// 邮件drop父类
    /// </summary>
    public class EmailDrop : Drop
    {

        public EmailDrop(string templatename)
        {
            this.TemplateName = templatename;
        }

        public string TemplateName { get; set; }

        /// <summary>
        /// 获得通对应的通知对象类型列表
        /// 比如 该drop需要通知 交易员 以及出纳
        /// </summary>
        /// <returns></returns>
        public virtual EnumNotifeeType[] GetNotifyTargets()
        {
            return new EnumNotifeeType[] { EnumNotifeeType.Account, EnumNotifeeType.Agent, EnumNotifeeType.Cashier,EnumNotifeeType.Accountant,EnumNotifeeType.Accountant };
        }

        /// <summary>
        /// 获得指定类别的通知对象
        /// </summary>
        public virtual string[] GetNotifyList(EnumNotifeeType type)
        {
            return new string[] { };
        }


        /// <summary>
        /// 生成需要发送的邮件
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEmail> GenerateEmails()
        {
            List<IEmail> emaillist = new List<IEmail>();

            //遍历所有通知者类型 然后为每个角色生成对应的邮件
            foreach (EnumNotifeeType type in GetNotifyTargets())
            {
                EmailTempleArg arg = new EmailTempleArg(this.TemplateName, type);
                IEmail email = Render.RenderEmail(arg, this);
                if (email != null)
                    emaillist.Add(email);
            }

            return emaillist;
        }


    }


}
