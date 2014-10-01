using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    internal class JsonWrapperAccountLite
    { 
        IAccount _acc;
        public JsonWrapperAccountLite(IAccount acc)
        {
            _acc = acc;
        }

        public string Account { get { return _acc.ID; } }//帐户ID
        public decimal LastEquity { get { return _acc.LastEquity; } }//昨日权益
        public decimal NowEquity { get { return _acc.NowEquity; } }//当前动态权益

        public decimal RealizedPL { get { return _acc.RealizedPL; } }//平仓盈亏
        public decimal UnRealizedPL { get { return _acc.UnRealizedPL; } }//浮动盈亏
        public decimal Commission { get { return _acc.Commission; } }//手续费
        public decimal Profit { get { return _acc.Profit; } }//净利
        public decimal CashIn { get { return _acc.Profit; } }//入金
        public decimal CashOut { get { return _acc.CashOut; } }//出金
        public decimal MoneyUsed { get { return _acc.MoneyUsed; } } //总资金占用
        public decimal TotalLiquidation { get { return _acc.TotalLiquidation; } }//帐户总净值
        public decimal AvabileFunds { get { return _acc.AvabileFunds; } }//帐户总可用资金


        public QSEnumAccountCategory Category { get { return _acc.Category; } }//账户类别
        public QSEnumOrderTransferType OrderRouteType { get {return  _acc.OrderRouteType; } }//路由类别
        public bool Execute { get { return _acc.Execute; } }//冻结
        public bool IntraDay { get { return _acc.IntraDay; } }//日内

    }
    internal class JsonWrapperAccount
    {
        IAccount _acc;
        public JsonWrapperAccount(IAccount acc)
        {
            _acc = acc;
        }

        public string Account { get { return _acc.ID; } }//帐户ID
        public decimal LastEquity { get { return _acc.LastEquity; } }//昨日权益
        public decimal NowEquity { get { return _acc.NowEquity; } }//当前动态权益

        public decimal RealizedPL { get { return _acc.RealizedPL; } }//平仓盈亏
        public decimal UnRealizedPL { get { return _acc.UnRealizedPL; } }//浮动盈亏
        public decimal Commission { get { return _acc.Commission; } }//手续费
        public decimal Profit { get { return _acc.Profit; } }//净利
        public decimal CashIn { get { return _acc.Profit; } }//入金
        public decimal CashOut { get { return _acc.CashOut; } }//出金
        public decimal MoneyUsed { get { return _acc.MoneyUsed; } } //总资金占用
        public decimal TotalLiquidation { get { return _acc.TotalLiquidation; } }//帐户总净值
        public decimal AvabileFunds { get { return _acc.AvabileFunds; } }//帐户总可用资金


        public QSEnumAccountCategory Category { get { return _acc.Category; } }//账户类别
        public QSEnumOrderTransferType OrderRouteType { get {return  _acc.OrderRouteType; } }//路由类别
        public bool Execute { get { return _acc.Execute; } }//冻结
        public bool IntraDay { get { return _acc.IntraDay; } }//日内



        #region 多品种交易 账户财务数据
        public decimal FutMarginUsed { get { return _acc.CalFutMargin(); } }//期货占用保证金
        public decimal FutMarginFrozen { get { return _acc.CalFutMarginFrozen(); } }//期货冻结保证金
        public decimal FutRealizedPL { get { return _acc.CalFutRealizedPL(); } }//期货平仓盈亏
        public decimal FutUnRealizedPL { get { return _acc.CalFutUnRealizedPL(); } }//期货浮动盈亏
        public decimal FutCommission { get { return _acc.CalFutCommission(); } }//期货交易手续费
        public decimal FutCash { get { return _acc.CalFutCash(); } }//期货交易现金
        public decimal FutLiquidation { get { return _acc.CalFutLiquidation(); } }//期货总净值
        public decimal FutMoneyUsed { get { return _acc.CalFutMoneyUsed(); } }//期货资金占用
        public decimal FutAvabileFunds { get { return _acc.AvabileFunds; } }


        public decimal OptPositionCost { get { return _acc.CalOptPositionCost(); } }//期权持仓成本
        public decimal OptPositionValue { get { return _acc.CalOptPositionValue(); } }//期权持仓市值
        public decimal OptRealizedPL { get { return _acc.CalOptRealizedPL(); } }//期权平仓盈亏
        public decimal OptCommission { get { return _acc.CalOptCommission(); } }//期权交易手续费
        public decimal OptMoneyFrozen { get { return _acc.CalOptMoneyFrozen(); } }//期权资金冻结
        public decimal OptCash { get { return _acc.CalOptCash(); } }//期权交易现金
        public decimal OptMarketValue { get { return _acc.CalOptPositionValue(); } }//期权总市值
        public decimal OptLiquidation { get { return _acc.CalOptLiquidation(); } }//期权总净值
        public decimal OptMoneyUsed { get { return _acc.CalOptMoneyUsed(); } }//期权资金占用
        public decimal OptAvabileFunds { get { return _acc.AvabileFunds; } }

        public decimal InnovPositionCost { get { return _acc.CalInnovPositionCost(); } }//异化合约持仓成本
        public decimal InnovPositionValue { get { return _acc.CalInnovPositionValue(); } }//异化合约持仓市值
        public decimal InnovCommission { get { return _acc.CalInnovCommission(); } }//异化合约手续费
        public decimal InnovRealizedPL { get { return _acc.CalInnovRealizedPL(); } }//异化合约平仓盈亏
        public decimal InnovMargin { get { return _acc.CalInnovMargin(); } }//异化合约保证金
        public decimal InnovMarginFrozen { get { return _acc.CalInnovMarginFrozen(); } }//异化合约冻结


        public decimal InnovCash { get { return _acc.CalInnovCash(); } }//异化合约现金流
        public decimal InnovMarketValue { get { return _acc.CalInnovPositionValue(); } }//异化合约市值
        public decimal InnovLiquidation { get { return _acc.CalInnovLiquidation(); } }//异化合约净值
        public decimal InnovMoneyUsed { get { return _acc.CalInnovMoneyUsed(); } }//异化合约资金占用
        public decimal InnovAvabileFunds { get { return _acc.AvabileFunds; } }//异化合约可用资金
        #endregion

    }
}
