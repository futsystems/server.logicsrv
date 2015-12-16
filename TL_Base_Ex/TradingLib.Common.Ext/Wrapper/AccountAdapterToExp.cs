using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 暴露给接口调用的IAccount对象,裁剪掉了部分功能提供一个安全的界面给外部
    /// </summary>
    public class AccountAdapterToExp:IAccountExp
    {
        IAccount _acc;
        public AccountAdapterToExp(IAccount acc)
        {
            _acc = acc;
        }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string ID { get { return _acc.ID; } }

        /// <summary>
        /// 账户委托转发通道类型 模拟还是实盘
        /// </summary>
        public QSEnumOrderTransferType OrderRouteType { get { return _acc.OrderRouteType; } }

        /// <summary>
        /// 账户类型 配资客户还是交易员
        /// </summary>
        public QSEnumAccountCategory Category { get { return _acc.Category; } }

        /// <summary>
        /// 帐户激活或者冻结
        /// </summary>
        public bool Execute { get { return _acc.Execute; } }

        /// <summary>
        /// 交易帐户是否处于警告状态
        /// </summary>
        public bool IsWarn { get { return _acc.IsWarn; } }
        /// <summary>
        /// 是否日内交易
        /// </summary>
        public bool IntraDay { get { return _acc.IntraDay; } }


        /// <summary>
        /// 上次结算日
        /// </summary>
        public DateTime SettleDateTime { get { return _acc.SettleDateTime; } }

      
        #region【IAccOperation】

        /// <summary>
        /// 强平帐户持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(QSEnumOrderSource source, string comment)
        { 
            //_acc.fla
            _acc.FlatPosition(source, comment);
        }

        /// <summary>
        /// 冻结帐户
        /// </summary>
        public void InactiveAccount()
        {
            _acc.InactiveAccount();
        }

        /// <summary>
        /// 激活帐户
        /// </summary>
        public void ActiveAccount()
        {
            _acc.ActiveAccount();
        }

        public void FlatPosition(Position pos, QSEnumOrderSource source, string comment)
        {
            _acc.FlatPosition(pos, source, comment);
        }

        /// <summary>
        /// 撤掉帐户下所有委托
        /// </summary>
        public void CancelOrder(QSEnumOrderSource source, string cancelreason)
        {
            _acc.CancelOrder(source, cancelreason);
        }

        /// <summary>
        /// 撤掉帐户下某个合约的所有委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(string symbol, QSEnumOrderSource source, string cancelreason)
        {
            _acc.CancelOrder(symbol, source, cancelreason);
        }

        /// <summary>
        /// 撤掉帐户下的某个为头
        /// </summary>
        /// <param name="order"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(Order order, QSEnumOrderSource source, string cancelreason)
        {
            _acc.CancelOrder(order, source, cancelreason);
        }


        public void Warn(bool iswarnning,string message="")
        {
            _acc.Warn(iswarnning,message);
        }
        #endregion

        #region 【IAccTradingInfo】

        public bool AnyPosition { get { return _acc.AnyPosition; } }//是否有持仓
        /// <summary>
        /// 获得账户当前持仓
        /// </summary>
        public IEnumerable<Position> Positions{get{return _acc.Positions; } }

        //public IEnumerable<Position> PositionsNet { get { return _acc.PositionsNet; } }

        /// <summary>
        /// 多头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsLong { get { return _acc.PositionsLong; } }

        /// <summary>
        /// 空头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsShort { get { return _acc.PositionsShort; } }

        public IEnumerable<Order> Orders { get { return _acc.Orders; } }//获得当日所有委托
        public IEnumerable<Trade> Trades { get { return _acc.Trades; } }//获得当日所有成交
        //public long[] Cancels { get { return _acc.Cancels; } }//获得当日所有取消
        public IEnumerable<PositionDetail> YdPositions { get { return _acc.YdPositions; } }

        /// <summary>
        /// 获得某个合约的持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetPosition(string symbol, bool side)
        { 
            return _acc.GetPosition(symbol,side);
        }


        /// <summary>
        /// 获得某个合约的净持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //public Position GetPositionNet(string symbol)
        //{
        //    return _acc.GetPositionNet(symbol);
        //}


        public int CanOpenSize(Symbol symbol,bool side,QSEnumOffsetFlag offset)
        {
            return _acc.CanOpenSize(symbol,side,offset);
        }
        #endregion

        #region 【IFinanceTotal】
        // <summary>
        /// 当前动态权益
        /// </summary>
        public decimal NowEquity { get { return _acc.NowEquity; } }

        /// <summary>
        /// 上期权益
        /// </summary>
        public decimal LastEquity { get { return _acc.LastEquity; } set { } }

        /// <summary>
        /// 平仓利润
        /// </summary>
        public decimal RealizedPL { get { return _acc.RealizedPL; } }
        /// <summary>
        /// 未平仓利润
        /// </summary>
        public decimal UnRealizedPL { get { return _acc.UnRealizedPL; } }

        /// <summary>
        /// 结算时的盯市盈亏
        /// </summary>
        public decimal SettleUnRealizedPL { get { return _acc.SettleUnRealizedPL; } }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get { return _acc.Commission; } }

        /// <summary>
        /// 净利
        /// </summary>
        public decimal Profit { get { return _acc.Profit; } }
        /// <summary>
        /// 入金
        /// </summary>
        public decimal CashIn { get { return _acc.CashIn; } }
        /// <summary>
        /// 出金
        /// </summary>
        public decimal CashOut { get { return _acc.CashOut; } }

        /// <summary>
        /// 保证金占用
        /// </summary>
        public decimal Margin { get { return _acc.Margin; } }

        /// <summary>
        /// 保证金冻结
        /// </summary>
        public decimal MarginFrozen { get { return _acc.Margin; } }

        /// <summary>
        /// 总占用资金 = 个品种占用资金之和
        /// </summary>
        public decimal MoneyUsed { get { return _acc.MoneyUsed; } }

        /// <summary>
        /// 帐户总可用资金
        /// </summary>
        public decimal AvabileFunds { get { return _acc.AvabileFunds; } }

        /// <summary>
        /// 总净值 帐户当前权益=总净值
        /// </summary>
        public decimal TotalLiquidation { get { return _acc.TotalLiquidation; } }

        /// <summary>
        /// 信用额度
        /// </summary>
        public decimal Credit { get { return _acc.Credit; } }

        /// <summary>
        /// 昨日优先资金
        /// </summary>
        public decimal LastCredit { get { return _acc.LastCredit; } set { throw new NotImplementedException(); } }

        /// <summary>
        /// 优先资金入金
        /// </summary>
        public decimal CreditCashIn { get { return _acc.CreditCashIn; } }

        /// <summary>
        /// 优先资金出金
        /// </summary>
        public decimal CreditCashOut { get { return _acc.CreditCashOut; } }
        #endregion

        #region 【IAccCal】
        /// <summary>
        /// 检查某个帐户是否可以接受某个委托 
        /// 用于保证金资金检查
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool CanFundTakeOrder(Order o)
        {
            string msg;
            return _acc.CanFundTakeOrder(o,out msg);
        }
        /// <summary>
        /// 获得某个合约的可用资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            return _acc.GetFundAvabile(symbol);
        }

        /// <summary>
        /// 计算某个委托所占用资金
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public decimal CalOrderFundRequired(Order o,decimal defaultvalue)
        {
            return _acc.CalOrderFundRequired(o,defaultvalue);

        }
        #endregion
    }
}
