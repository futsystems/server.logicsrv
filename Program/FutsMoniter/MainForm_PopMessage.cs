using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter.Common;


namespace FutsMoniter
{
    partial class MainForm
    {

        fmPopMessage popwindow = new fmPopMessage();
        System.ComponentModel.BackgroundWorker bg;

        RingBuffer<RspInfo> infobuffer = new RingBuffer<RspInfo>(1000);
        void InitBW()
        {
            bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.ProgressChanged += new ProgressChangedEventHandler(bg_ProgressChanged);
            bg.RunWorkerAsync();
        }

        /// <summary>
        /// 当后台线程有触发时 调用显示窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RspInfo info = e.UserState as RspInfo;
            System.Drawing.Point p = PointToScreen(statusStrip1.Location);
            p = new System.Drawing.Point(p.X, p.Y - popwindow.Height + statusStrip1.Height);

            popwindow.Location = p;
            popwindow.PopMessage(info);
        }

        /// <summary>
        /// 后台工作流程 当缓存中有数据是通过ReportProgress进行触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //检查变量 然后对外触发 
                while (infobuffer.hasItems)
                {
                    RspInfo info = infobuffer.Read();
                    bg.ReportProgress(1, info);
                    Util.sleep(1000);
                }
                Util.sleep(50);
            }
        }


    }
}
