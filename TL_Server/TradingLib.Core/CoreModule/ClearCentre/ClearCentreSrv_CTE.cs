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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "OpenClearCentre", "OpenClearCentre - open clearcentre", "开启清算中心")]
        public void CTE_OpenClearCentre(ISession session)
        {
            OpenClearCentre();
            session.RspMessage("清算中心成功开启");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CloseClearCentre", "CloseClearCentre - close clearcentre", "关闭清算中心")]
        public void CTE_CloseClearCentre(ISession session)
        {
            CloseClearCentre();
            session.RspMessage("清算中心成功关闭");
        }


        /// <summary>
        /// 开启清算中心
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "opencc", "opencc - Open ClearCentre", "开启交易中心,接收客户端提交的委托")]
        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "opencc", "opencc - Open ClearCentre", "开启交易中心,接受客户端提交的委托")]
        public void EXCH_OpenClearCentre()
        {
            OpenClearCentre();
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝收客户端提交的委托")]
        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝客户端提交的委托")]
        public void EXCH_CloseClearCentre()
        {
            CloseClearCentre();
        }



    }
}
