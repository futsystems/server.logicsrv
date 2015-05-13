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
        /// 初始化视区控件
        /// </summary>
        void InitPage()
        {
            kryptonDockingManager.AddToWorkspace("Workspace", GetWorkspacePages());
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Right, GetModulePage());
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewTradingInfoReal() });
            
            //加载默认布局
            //if (System.IO.File.Exists("config.xml"))
            //{
            //    kryptonDockingManager.LoadConfigFromFile("config.xml");
            //}
            //显示所有页面
            kryptonDockingManager.ShowAllPages();
        }

        public void AddWorkspacePage(MonitorControl control)
        {
            workspacelist.Add(control);
        }
        List<MonitorControl> workspacelist = new List<MonitorControl>();

        KryptonPage[] GetWorkspacePages()
        {
            List<KryptonPage> pagelist = new List<KryptonPage>();
            pagelist.Add(NewAccMoniter());
            foreach (MonitorControl ct in workspacelist)
            {
                pagelist.Add(NewPage(ct.GetType().FullName + "[W]", ct.Title, 2, ct));
            }
            return pagelist.ToArray();
        }


        List<MonitorControl> controlist = new List<MonitorControl>();

        public void AddModulePage(MonitorControl control)
        {
            controlist.Add(control);
        }

        KryptonPage[] GetModulePage()
        {
            List<KryptonPage> pagelist = new List<KryptonPage>();
            pagelist.Add(NewQuote());
            pagelist.Add(NewAccFinInfo());
            //如果域有配资模块权限则加载配资模块面板
            if (Globals.Domain.Module_FinService)
            {
                pagelist.Add(NewFinService());
            }

            foreach(MonitorControl ct in controlist)
            {
                pagelist.Add(NewPage(ct.GetType().FullName+"[M]",ct.Title, 2, ct));
            }
            
            return pagelist.ToArray();
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

        const string PAGE_FINSERVICE = "FINSERVICE";
        const string PAGE_QUOTE = "QUOTE";
        const string PAGE_TRADINGINFO = "TRADINGINFO";
        const string PAGE_ACCINFO = "ACCINFO";
        const string PAGE_ACCMONITER = "ACCMONITER";

        bool existpage(string name)
        {
            return pagemap.Keys.Contains(name);
        }
        private KryptonPage NewQuote()
        {

            return NewPage(PAGE_QUOTE, "报价与下单", 2, new ctQuoteMoniter());
        }

        private KryptonPage NewTradingInfoReal()
        {
            return NewPage(PAGE_TRADINGINFO, "交易记录", 2, new ctTradingInfoReal());
        }

        private KryptonPage NewFinService()
        {
            return NewPage(PAGE_FINSERVICE, "配资服务", 2, new ctFinService());
        }

        private KryptonPage NewAccFinInfo()
        {
            return NewPage(PAGE_ACCINFO, "财务信息", 2, new ctFinanceInfo());
        }

        private KryptonPage NewAccMoniter()
        {
            return NewPage(PAGE_ACCMONITER, "帐户监控", 2, new ctAccountMontier());
        }



        private int _count = 1;
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
