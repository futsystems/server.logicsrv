using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using DotLiquid;
using TradingLib.Mixins.DotLiquid;

namespace TradingLib.Contrib.NotifyCentre
{
    public class Render
    {
        static Render defaultinstance = null;


        //在资源目录CriticalNotify加载tpl
        TemplateTracker tpltk;
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static Render()
        {
            defaultinstance = new Render();
        }

        /// <summary>
        /// 私有构造行数
        /// </summary>
        private Render()
        {
            //生成模板维护器
            tpltk = TemplateTracker.CreateTemplateTracker(Util.GetResourceDirectory("NotifyGW"), "tpl");
        }


        /// <summary>
        /// 将EmailDrop 渲染成邮件 返回
        /// </summary>
        /// <param name="drop"></param>
        /// <returns></returns>
        internal static IEmail RenderEmail(EmailTempleArg arg, EmailDrop drop)
        {
            //生成邮件对象
            string[] emaillist = drop.GetNotifyList(arg.NotifeeType);
            if (emaillist == null || emaillist.Length == 0) return null;

            //渲染出body 和 subject
            string body = string.Empty;
            bool ret = defaultinstance.tpltk.Render(arg.BodyTPL, drop, out body);
            string subject = string.Empty;
            bool ret2 = defaultinstance.tpltk.Render(arg.SubjectTPL,drop,out subject);

            //渲染出错返回nul
            if ((!ret) || (!ret2)) return null;
            return new Email(subject,body,emaillist);
        }

        
    }
}
