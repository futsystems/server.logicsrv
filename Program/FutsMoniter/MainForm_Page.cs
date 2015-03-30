﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MoniterControl;

using FutsMoniter.Common;

namespace FutsMoniter
{
    partial class MainForm
    {
        Dictionary<string, KryptonPage> pagemap = new Dictionary<string, KryptonPage>();
        /// <summary>
        /// 
        /// </summary>
        void InitPage()
        {
            kryptonDockingManager.AddToWorkspace("Workspace", GetWorkspacePages());
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewTradingInfoReal() });
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Right, GetModulePage());


            if (System.IO.File.Exists("config.xml"))
            {
                kryptonDockingManager.LoadConfigFromFile("config.xml");
            }

            kryptonDockingManager.ShowAllPages();
        }

        List<MoniterControl> workspacelist = new List<MoniterControl>();
        void AppendWorkspacePage(MoniterControl ctl)
        {
            workspacelist.Add(ctl);
        }

        KryptonPage[] GetWorkspacePages()
        {
            List<KryptonPage> pages = new List<KryptonPage>();
            pages.Add(NewAccMoniter());
            foreach (var ctl in workspacelist)
            {
                pages.Add(NewPage(ctl.GetType().FullName + "[W]", ctl.Title, 2, ctl));
            }
            return pages.ToArray();
        }


        List<MoniterControl> modulelist = new List<MoniterControl>();
        void AppendModulePage(MoniterControl ctl)
        {
            modulelist.Add(ctl);
        }
        KryptonPage[] GetModulePage()
        {
            List<KryptonPage> pages = new List<KryptonPage>();
            pages.Add(NewQuote());
            pages.Add(NewAccFinInfo());
            //如果域有配资模块权限则加载配资模块面板
            if (Globals.Domain.Module_FinService)
            {
                pages.Add(NewFinService());
            }

            foreach (var ctl in modulelist)
            {
                pages.Add(NewPage(ctl.GetType().FullName + "[M]", ctl.Title, 2, ctl));
            }

            return pages.ToArray();
        }
        void DestoryPage()
        {
            //隐藏所有Page
            kryptonDockingManager.HideAllPages();
            //
            kryptonDockableWorkspace.HideAllPages();

            kryptonDockingManager.RemoveAllPages(true);
            kryptonDockingManager.ClearAllStoredPages();
        }

        void ShowAllPage()
        {
            kryptonDockingManager.ShowAllPages();

            kryptonDockableWorkspace.ShowAllPages();
        }



        private void UpdateCell(KryptonWorkspaceCell cell)
        {
            cell.NavigatorMode = NavigatorMode.BarTabGroup;
        }

        bool existpage(string name)
        {
            return pagemap.Keys.Contains(name);
        }


        private KryptonPage NewFinService()
        {
            return NewPage(PAGE_FINSERVICE, "配资服务", 2, new ctFinService());
        }

        const string PAGE_FINSERVICE = "FINSERVICE";



        #region 生成基本页面 报价 交易信息 财务信息 帐户列表
        const string PAGE_QUOTE = "QUOTE";
        const string PAGE_TRADINGINFO = "TRADINGINFO";
        const string PAGE_ACCINFO = "ACCINFO";
        const string PAGE_ACCMONITER = "ACCMONITER";





        private KryptonPage NewQuote()
        {
            return NewPage(PAGE_QUOTE, "报价与下单", 2, new ctQuoteMoniter());
        }

        private KryptonPage NewTradingInfoReal()
        {
            return NewPage(PAGE_TRADINGINFO, "交易记录", 2, new ctTradingInfoReal());
        }

        private KryptonPage NewAccFinInfo()
        {
            return NewPage(PAGE_ACCINFO, "财务信息", 2, new ctFinanceInfo());
        }

        private KryptonPage NewAccMoniter()
        {
            return NewPage(PAGE_ACCMONITER, "帐户监控", 2, new ctAccountMontier());
        }
        #endregion





        /// <summary>
        /// 将某个control封装成page页 用于加载到窗体dockmanger中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="image"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private KryptonPage NewPage(string name, string title, int image, Control content)
        {
            //if (existpage(name))
            //{
            //    return null;
            //}
            // Create new page with title and image
            KryptonPage p = new KryptonPage();
            p.Text = title;
            p.TextTitle = title;
            p.TextDescription = title;
            p.UniqueName = name;
            //p.ImageSmall = imageListSmall.Images[image];

            // Add the control for display inside the page
            content.Dock = DockStyle.Fill;
            p.Controls.Add(content);
            //pagemap.Add(name, p);
            return p;
        }

    }
}
