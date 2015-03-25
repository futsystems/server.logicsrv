using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.MoniterControl;
using FutsMoniter.Common;

namespace FutsMoniter
{
    partial class MainForm
    {


        /// <summary>
        /// 加载管理端控件
        /// </summary>
        void LoadMoniterControl()
        {
            foreach (Type type in MoniterPlugin.GetMoniterControlTypes())
            {
                MoniterControl ctl = MoniterPlugin.CreateMoniterControl(type.FullName);

                //注册控件
                MoniterControlHelper.RegisterControl(ctl);

                //添加到对应的容器中
                this.AppendWorkspacePage(ctl);

            }
        }
    }
}
