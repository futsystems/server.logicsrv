using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 定时任务 被TaskCentre调用的定时执行的操作
    /// </summary>
    public partial class ClearCentre
    {

        [TaskAttr("夜盘开启交易中心", 20, 50, 0, "每天晚上20:50:5开启清算中心")]
        [TaskAttr("白盘开启交易中心", 8, 50, 0, "每天白天8:50:5开启清算中心")]
        public void Task_OpenClearCentre()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday) return;
            this.OpenClearCentre();
            Notify("开启清算中心[" + DateTime.Now.ToString() + "]", " ");
            debug("开启清算中心,准备接受客户委托", QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 关闭清算中心
        /// </summary>
        [TaskAttr("夜盘关闭清算中心", 2, 35, 0, "夜盘关闭清算中心")]
        [TaskAttr("日盘关闭清算中心", 15, 20, 0, "日盘关闭清算中心")]
        public void Task_CloseClearCentre()
        {
            this.CloseClearCentre();
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday) return;
            Notify("关闭清算中心[" + DateTime.Now.ToString() + "]", " ");
            debug("关闭清算中心,将拒绝所有客户委托", QSEnumDebugLevel.INFO);
        }





        /// <summary>
        /// 开启清算中心
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "opencc", "opencc - Open ClearCentre", "开启交易中心,接收客户端提交的委托")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "opencc", "opencc - Open ClearCentre", "开启交易中心,接受客户端提交的委托")]
        public void EXCH_OpenClearCentre()
        {
            OpenClearCentre();
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝收客户端提交的委托")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝客户端提交的委托")]
        public void EXCH_CloseClearCentre()
        {
            CloseClearCentre();
        }

    }
}
