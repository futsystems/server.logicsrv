using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {

        /// <summary>
        /// 准备交易重置
        /// </summary>
        
        public void Task_Ready_Reset()
        {
            if (!TLCtxHelper.Ctx.SettleCentre.IsTradingday) return;//非交易日 不重置
            //1.重新启动Fastick对应的DataFeed 实盘交易服务负责重启tick数据DataFeed
            //if (config.LoadMode == QSEnumAccountLoadMode.REAL)
            //    RestartCTPDataFeed();
            Thread.Sleep(2000);
            //2.重置路由中心 清空OrderHelper未完成事务 并重新启动模拟交易引擎
            _brokerRouter.Reset();
            //3.重置数据中心 清空Tick快照 并重启建立到FasTickPub的连接
            _datafeedRouter.Reset();
            //Notify("重置路由", DateTime.Now.ToString() + " 重置路由,准备开盘接受委托");

        }

        /// <summary>
        /// 结算重置
        /// </summary>
        [TaskAttr("清算中心结算完毕后重置", 16,00, 5, "清算中心结算后重置")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "settlereset", "settlereset - settlereset", "结算后重置,用于清空内存中当日交易记录以及成交与行情路由中的日内数据")]
        public void Task_Settle_Reset()
        {
            if (!TLCtxHelper.Ctx.SettleCentre.IsTradingday) return;
            //重置清算中心 清空交易记录 更新账户昨日权益 清算日期等 从数据库恢复今日数据
            _clearCentre.Reset();
            //重置风空中心 重新加载账户风控规则
            _riskCentre.Reset();
            //重置路由中心 清空OrderHelper未完成事务
            _brokerRouter.Reset();
            //重置数据中心 清空Tick快照
            _datafeedRouter.Reset();
            //Notify("结算后重置清算与风控[" + DateTime.Now.ToString() + "]", " ");
            this.Notify("结算后重置清算中心与风控[" + DateTime.Now.ToShortDateString() + "]", " ");
        }



    }
}
