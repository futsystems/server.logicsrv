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
        /// 账户所对应的清算适配器,用于从清算中心获得账户对应的信息
        /// </summary>
        //public IAccountClearCentre ClearCentre { get { return _acc.ClearCentre; } }

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
        /// 是否日内交易
        /// </summary>
        public bool IntraDay { get { return _acc.IntraDay; } }



        /// <summary>
        /// 服务设定的配资额度
        /// </summary>
        //public decimal FinAmmountTotal { get { return _acc.FinAmmountTotal; } }//配资额度
        /// <summary>
        /// 当前有效配资额度
        /// </summary>
        //public decimal FinAmmountAvabile { get { return _acc.FinAmmountAvabile; } }

        /// <summary>
        /// 上次结算日
        /// </summary>
        public DateTime SettleDateTime { get { return _acc.SettleDateTime; } }

        //public decimal ObverseProfit { get { return _acc.ObverseProfit; } }


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

        public void FlatPosition(Position pos, QSEnumOrderSource source, string comment)
        {
            _acc.FlatPosition(pos, source, comment);
        }

        #endregion

        #region 【IAccTradingInfo】

        public bool AnyPosition { get { return _acc.AnyPosition; } }//是否有持仓
        /// <summary>
        /// 获得账户当前持仓
        /// </summary>
        public Position[] Positions
        {
            get
            {
                return _acc.Positions;
            }
        }

        public Order[] Ordres { get { return _acc.Ordres; } }//获得当日所有委托
        public Trade[] Trades { get { return _acc.Trades; } }//获得当日所有成交
        public long[] Cancels { get { return _acc.Cancels; } }//获得当日所有取消
        public Position[] PositionsHold { get { return _acc.PositionsHold; } }
        public Position getPosition(string symbol,bool side)//获得某个symbol的持仓信息
        {
            return _acc.getPosition(symbol,side);
        }


        public int CanOpenSize(Symbol symbol)
        {
            return _acc.CanOpenSize(symbol);
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
        /// 账户初始权益
        /// </summary>
        //public decimal StartEquity { get { return _acc.StartEquity; } }

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
        /// 总占用资金 = 个品种占用资金之和
        /// </summary>
        public decimal MoneyUsed { get { return _acc.MoneyUsed; } }

        /// <summary>
        /// 总净值 帐户当前权益=总净值
        /// </summary>
        public decimal TotalLiquidation { get { return _acc.TotalLiquidation; } }//帐户总净值


        public decimal AvabileFunds { get { return _acc.AvabileFunds; } }//帐户总可用资金



        /// <summary>
        /// 保证金占用
        /// </summary>
        public decimal Margin { get{return _acc.Margin;} }

        /// <summary>
        /// 保证金冻结
        /// </summary>
        public decimal MarginFrozen { get{return _acc.Margin;}}
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
        /// 获得帐户总可用资金
        /// </summary>
        /// <returns></returns>
        public decimal GetFundAvabile()
        {
            return _acc.GetFundAvabile();
        }

        /// <summary>
        /// 获得帐户所有资金包含已经使用和未使用资金
        /// </summary>
        /// <returns></returns>
        public decimal GetFundTotal()
        {
            return _acc.GetFundTotal();
        }

        /// <summary>
        /// 获得所使用资金
        /// </summary>
        /// <returns></returns>
        public decimal GetFundUsed()
        {
            return _acc.GetFundUsed();
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
