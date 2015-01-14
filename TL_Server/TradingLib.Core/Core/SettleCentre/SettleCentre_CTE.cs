using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


/* 进一步完善结算机制
 * 目前结算 1.数据保存 结算持仓明细 当日交易记录 2.执行帐户结算 3.清空日内交易记录
 * 持仓对象的 closedpl是按照持仓开平过程中产生的平仓盈亏的实时累加， 对应通过累加持仓明细的 平仓盈亏也是得到相同的金额
 *           unrealizedpl 是按照最新价格计算的浮动盈亏，结算时需要计算 结算价格为基础的浮动盈亏
 *           按持仓成本-结算价格 来计算盯市浮动盈亏
 *           
 *           累加持仓明细的 盯市浮动盈亏 也应该得到相同的金额，在持仓明细的盯市浮动盈亏的计算过程中，采的是动态结算价格，在交易过程中该价格是持仓对象的最新价格，当持仓结算后，其对应的价格是结算价格。
 *           因此计算出来的浮动盈亏也是一致的。只是采用了2中不同的计算方式
 *           
 *           这里考虑结算过程中从 通常的财务计算过程分离，独立的去计算盯市浮动盈亏
 *           把持仓的成本移动到结算价,同时将移动部分的浮动盈亏结算到交易帐户中，逐日结算制度
 *           
 *           这里需要细化结算过程
 *           1.结算之前需要确认获得了所有结算价格
 *           2.价格推送到系统持仓后，保存持仓明细（持仓明细会按结算价格进行计算当前的盯市浮动盈亏）保存数据
 *           3.然后执行帐户结算，将帐户当日盈亏 手续费 出入金 盯市浮动盈亏 计算 形成结算记录
 *           4.结算单实际上是某个时间节点 所记录的状态数据的在线。
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
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

        bool settled = false;

        #region 大结算过程
        //1.采集结算价格 保存结算持仓与日内交易记录
        /// <summary>
        /// 转储数据,将当天的交易信息 储存到历史交易信息表中
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "datastore", "datastore - datastore", "datastore")]
        public void Task_DataStore()
        {
            if (IsNormal && !IsTradingday) return;//结算中心正常 但不是交易日 不做记录转储

            this.IsInSettle = true;//标识结算中心处于结算状态

            //通过系统事件中继触发结算前事件
            try
            {
                TLCtxHelper.EventSystem.FireBeforeSettleEvent(this, new SystemEventArgs());
            }
            catch (Exception ex)
            {
                debug("BeforeSettleEvent Fired error:" + ex.ToString(), QSEnumDebugLevel.FATAL);
            }

            
            //保存结算持仓对应的PR数据
            this.SaveHoldInfo();
            //保存当前持仓明细
            this.SavePositionDetails();//保存持仓明细
            //保存交易日志 委托 成交 委托操作
            this.Dump2Log();//将委托 成交 撤单 PR数据保存到对应的log_表 所有的转储操作均是replace into不会存在重复操作
            
        }


        //2.执行帐户结算 生成结算记录,并更新帐户结算权益与结算时间 所有帐户结算完毕后更新系统最新结算日
        /// <summary>
        /// 定时结算交易账户任务
        /// 非交易日不用进行结算
        /// 结算时间15:50 在15:50-16:00之间开机会导致无法找到对应的交易日
        /// 4点结算 否则在结算中心初始化交易日过程中会导致交易日判定不准确，交易日判定是以结算时间为界限，结算前是当前交易日，结算后就是下一交易日
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "settle", "settle - clean the interday tmp table after reset", "清算中心结算交易帐户")]
        public void Task_SettleAccount()
        {
            if (IsNormal && !IsTradingday) return;
            this.SettleAccount();
            //触发结算后记录
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());

            //结算后 重置结算中心 如果交易日没有发生变化，则出入金还会停留在上个结算日，而上个结算日已经结算，因此该出入金记录会被丢失 
            //出入金拒绝窗口就是结算时间段
            Util.sleep(1000);//防止交易日计算时与结算时间过度接近 造成当前交易日计算错误
            this.Reset();
            settled = true;//当日结算过 当日结算过则需要重置交易系统
            this.IsInSettle = false;//标识系统结算完毕
        }

        
        //3.重置结算中心交易日信息 重置清算中心交易帐户 将昨日扎帐
        //清算中心，风控中心，以及数据路由 成交路由都需要进行重置
        //开盘前需要重置
        //通过参数 设定时间注入到任务系统
        [ContribCommandAttr(QSEnumCommandSource.CLI, "resetsc", "resetsc - reset settlecentre trading day", "重置结算中心")]
        public void Task_ResetTradingday()
        {
            //debug("重置交易系统 isnaormal:"+IsNormal.ToString() +" istradingday:"+IsTradingday.ToString(),QSEnumDebugLevel.INFO);
            if (!settled) return;//没有结算就不重置交易系统
            debug("系统重置，清算中心重置帐户，风控中心重置规则 清空日内记录表", QSEnumDebugLevel.INFO);
            TLCtxHelper.EventSystem.FireBeforeSettleResetEvent(this, new SystemEventArgs());
            
            //清空日内交易记录
            if (_cleanTmp)
            {
                this.CleanTempTable();
            }

            //15:50分结算帐户完毕后 4点从数据库重新加载帐户权益数据,此时数据库加载的数据是按结算重置后加载的日期信息 即结算日向前滚动一日
            _clearcentre.Reset();
            //重置风控中心，清空内存缓存数据
            _riskcentre.Reset();
            //重置消息交换中心
            _exchsrv.Reset();
            //重置管理交换中心

            //重置任务中心
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
            this.SaveHoldInfo();//保存结算持仓数据和对应的PR数据
            this.SavePositionDetails();//保存持仓明细
            this.Dump2Log();//转储到历史记录表

            //B:结算交易帐户形成结算记录
            this.SettleAccount();
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());

            TLCtxHelper.EventSystem.FireBeforeSettleResetEvent(this, new SystemEventArgs());
            //C:清空当日交易记录
            if (_cleanTmp)
            {
                this.CleanTempTable();
            }

            //D:重置系统状态
            //重置结算中心 形成新的最后结算日 下一交易日和当前交易日数据
            this.Reset();
            //重置清算中心，加载下一交易日的交易记录
            _clearcentre.Reset();
            //重置风控中心，清空内存缓存数据
            _riskcentre.Reset();
            //重置消息交换中心
            _exchsrv.Reset();
            //重置管理交换中心

            //重置任务中心
            TLCtxHelper.EventSystem.FireAfterSettleResetEvent(this, new SystemEventArgs());
        }
        #endregion




        #region 定时开启 关闭清算中心 并在夜盘收盘后更新交易日信息 
        [TaskAttr("重置结算中心-夜盘收盘后", 3,0,0, "夜盘收盘后重置结算中心")]
        [TaskAttr("重置结算中心-夜盘收盘后", 8,0,0, "每天8点重置结算中心")]//判定当前交易日状态，系统很多其他事务是按结算状态来进行的
        public void Task_ResetTradingdayNieght()
        {
            debug("结算中心重置交易日信息", QSEnumDebugLevel.INFO);
            this.Reset();
        }

        [TaskAttr("夜盘开启交易中心", 20, 50,0, "每天晚上20:50:5开启清算中心")]
        [TaskAttr("白盘开启交易中心", 8, 50,0, "每天白天8:50:5开启清算中心")]
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
        [TaskAttr("夜盘关闭清算中心", 2, 35,0, "夜盘关闭清算中心")]
        [TaskAttr("日盘关闭清算中心", 15, 20,0, "日盘关闭清算中心")]
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
