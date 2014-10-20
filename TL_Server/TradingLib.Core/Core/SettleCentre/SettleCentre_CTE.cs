using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 关于系统结算
    /// 系统结算时 如果帐户有持仓 则结算当时的权益并不会停止不动
    /// 系统仍然会根据行情的变化而产生数据变化
    /// 同时隔夜持仓也是进行了利润结转 不利于获得持仓的历史成本
    /// 这里进行改进
    /// 1.保存结算持仓时 记录持仓的历史成本 同时记录该持仓的结算价格 用于记录当时的盯市盈亏
    /// 2.系统结算报表记录时,将每个持仓的盯市盈亏 累计后计入浮动盈亏
    /// 3.帐户重置时,需要从数据库加载所有数据而不是将当时的权益进行结转，应为当时的权益可能已经和执行结算时候的权益发生了变化
    /// </summary>
    public partial class SettleCentre
    {

        [ContribCommandAttr(QSEnumCommandSource.CLI, "settlestatus", "settlestatus - settlestatus", "settlestatus")]
        public string CTE_SettleStatus()
        {
            return string.Format("last settleday:{0} next tradingday:{1} current tradingday:{2} istradingday:{3}", _lastsettleday, _nexttradingday, _tradingday,IsTradingday);
        }


        //#region 交易记录检验
        ///// <summary>
        ///// 数据检验
        ///// </summary>
        //[TaskAttr("清算中心数据检验", 15, 22, 35, "清算中心数据检验")]
        //public void Task_DataCheck()
        //{
        //    if (IsNormal && !IsTradingday) return;//如果是正常状态 且当前是非交易日则直接返回
        //    this.CheckAccountLastEquity();
        //    this.CheckTradingInfo();
        //    Notify("数据检验[" + DateTime.Now.ToString() + "]", " ");
        //}
        //#endregion


        #region 大结算过程
        //1.采集结算价格 保存结算持仓与日内交易记录
        /// <summary>
        /// 转储数据,将当天的交易信息 储存到历史交易信息表中
        /// </summary>
        [TaskAttr("清算中心转储交易记录", 15, 30, 05, "清算中心转储交易记录")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "datastore", "datastore - datastore", "datastore")]
        public void Task_DataStore()
        {
            //通过系统事件中继触发结算前事件
            TLCtxHelper.EventSystem.FireBeforeSettleEvent(this,new SystemEventArgs());

            this.IsInSettle = true;//标识结算中心处于结算状态
            if (IsNormal && !IsTradingday) return;//结算中心正常 但不是交易日 不做记录转储
            //先将内存中的PR数据保存到数据库 在保存pr数据之前,先清空了当日的pr临时数据表(这里的清空 会造成 清空其他程序加载账户的数据 ？？)
            this.BindPositionSettlePrice();//采集持仓结算价
            this.SaveHoldInfo();//保存结算持仓数据 包括当前持仓和当前持仓对应的PositionRound数据
            this.Dump2Log();//将委托 成交 撤单 PR数据保存到对应的log_表 所有的转储操作均是replace into不会存在重复操作
            Notify("保存交易数据[" + DateTime.Now.ToString() + "]", " ");
        }

        //2.执行帐户结算 生成结算记录,并更新帐户结算权益与结算时间 所有帐户结算完毕后更新系统最新结算日
        /// <summary>
        /// 定时结算交易账户任务
        /// 非交易日不用进行结算
        /// 结算时间15:50 在15:50-16:00之间开机会导致无法找到对应的交易日
        /// </summary>
        [TaskAttr("清算中心执行当日结算", 15, 32, 5, "清算中心执行当日结算")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "settle", "settle - clean the interday tmp table after reset", "清算中心结算交易帐户")]
        public void Task_SettleAccount()
        {
            if (IsNormal && !IsTradingday) return;
            this.SettleAccount();
            //触发结算后记录
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());
            Notify("结算交易账户[" + DateTime.Now.ToString() + "]", " ");
        }

        //3.系统结算完毕后 清空日内临时交易记录表
        /// <summary>
        /// 结算后清空临时数据表 用于准备进入下一个交易日
        /// 需要在重置前进行清空,清空临时表是以交易日来进行清空的，而清算中心重置后 交易日会发生改变导致无法清空日内记录表
        /// </summary>
        [TaskAttr("清空日内交易记录[SQL]", 15, 58, 5, "清空日内交易缓存")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "cleanafterreset", "cleanafterreset - clean the interday tmp table after reset", "结算后清空日内交易临时数据表")]
        public void Task_CleanAfterReset()
        {
            TLCtxHelper.EventSystem.FireBeforeSettleResetEvent(this, new SystemEventArgs());
            if (IsNormal && !IsTradingday) return;
            this.CleanTempTable();
        }

        
        //4.重置结算中心交易日信息 重置清算中心交易帐户 将昨日扎帐
        //清算中心，风控中心，以及数据路由 成交路由都需要进行重置
        //开盘前需要重置
        [TaskAttr("重置结算中心-结算后", 16, 00, 5, "结算后重置结算中心")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "resetsc", "resetsc - reset settlecentre trading day", "重置结算中心")]
        public void Task_ResetTradingday()
        {
            if (IsNormal && !IsTradingday) return;
            debug("系统重置，清算中心重置帐户，风控中心重置规则", QSEnumDebugLevel.INFO);
            //重置结算中心
            this.Reset();

            //15:50分结算帐户完毕后 4点从数据库重新加载帐户权益数据,此时数据库加载的数据是按结算重置后加载的日期信息 即结算日向前滚动一日
            _clearcentre.Reset();

            //重置风控中心，清空内存缓存数据
            _riskcentre.Reset();

            this.IsInSettle = false;//标识系统结算完毕
            TLCtxHelper.EventSystem.FireAfterSettleResetEvent(this, new SystemEventArgs());
            
        }

        /// <summary>
        /// 执行历史结算
        /// 从某个交易日开始从数据库加载历史持仓,出入金,日内交易记录 恢复对应结算日的数据然后进行结算转储存
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "settleround", "settleround - 执行一次结算并重置交易系统状态", "执行结算并重置系统状态")]
        public void HistSettleRound()
        {
            //通过系统事件中继触发结算前事件
            TLCtxHelper.EventSystem.FireBeforeSettleEvent(this, new SystemEventArgs());

            //A:储存当前数据
            this.BindPositionSettlePrice();//采集持仓结算价
            this.SaveHoldInfo();//保存结算持仓数据和对应的PR数据
            this.Dump2Log();//转储到历史记录表

            //B:结算交易帐户形成结算记录
            this.SettleAccount();
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());

            TLCtxHelper.EventSystem.FireBeforeSettleResetEvent(this, new SystemEventArgs());
            //C:清空当日交易记录
            this.CleanTempTable();

            //D:重置系统状态
            //重置结算中心 形成新的最后结算日 下一交易日和当前交易日数据
            this.Reset();
            //重置清算中心，加载下一交易日的交易记录
            _clearcentre.Reset();
            //重置风控中心，清空内存缓存数据
            _riskcentre.Reset();

            TLCtxHelper.EventSystem.FireAfterSettleResetEvent(this, new SystemEventArgs());
        }
        #endregion




        #region 定时开启 关闭清算中心 并在夜盘收盘后更新交易日信息 
        [TaskAttr("重置结算中心-夜盘收盘后", 3, 00, 5, "夜盘收盘后重置结算中心")]
        public void Task_ResetTradingdayNieght()
        {
            debug("结算中心重置交易日信息", QSEnumDebugLevel.INFO);
            this.Reset();
        }

        [TaskAttr("晚上开盘前开启交易中心", 20, 50, 5, "每天晚上20:50:5开启清算中心")]
        [TaskAttr("白天开盘前开启交易中心", 8, 50, 5, "每天白天8:50:5开启清算中心")]
        public void Task_OpenClearCentre()
        {
            if (IsNormal && !IsTradingday) return;
            _clearcentre.OpenClearCentre();
            Notify("开启清算中心[" + DateTime.Now.ToString() + "]", " ");
            debug("开启清算中心,准备接受客户委托", QSEnumDebugLevel.INFO);

        }

        /// <summary>
        /// 关闭清算中心
        /// </summary>
        [TaskAttr("夜盘关闭清算中心", 2, 35, 5, "夜盘关闭清算中心")]
        [TaskAttr("日盘关闭清算中心", 15, 20, 5, "日盘关闭清算中心")]
        public void Task_CloseClearCentre()
        {
            _clearcentre.CloseClearCentre();
            if (!IsTradingday) return;
            Notify("关闭清算中心[" + DateTime.Now.ToString() + "]", " ");
            debug("关闭清算中心,将拒绝所有客户委托", QSEnumDebugLevel.INFO);
        }


        #endregion

    }
}
