//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;


//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 用于将clearcentresrv的部分功能函数暴露给Account调用,但是Account不能获得太多功能调用(越权使用)
//    /// 将IClearCentre的功能进行一定程度的封装,使得Account里面方便调用获得财务信息,交易信息等
//    /// </summary>
//    public class ClearCentreAdapterToAccount:IAccountClearCentre
//    {
//        private IAccount _acc;
//        private IClearCentreSrv _clearcentre;
//        public ClearCentreAdapterToAccount(IAccount a,  IClearCentreSrv c)
//        {
//            _acc = a;
//            _clearcentre = c;
        
//        }

//        //public Security getMasterSecurity(string symbol)
//        //{
//        //    return null;// _clearcentre.getMasterSecurity(symbol);
//        //}

        

//        //public decimal FinAmmountAvabile { get { return _clearcentre.GetAccountFinAmmountAvabile(_acc.ID); } }
//        //public decimal FinAmmountTotal { get { return _clearcentre.GetAccountFinAmmountTotal(_acc.ID); } }

//        //public decimal Margin { get { return _clearcentre.CalMargin(_acc); } }//计算保证金占用
//        //public decimal FrozenMargin { get { return _clearcentre.CalFrozenMargin(_acc); } }//计算占用保证金
//        //public decimal RealizedPL { get { return _clearcentre.CalRealizedPL(_acc); } }//计算平仓利润
//        //public decimal UnRealizedPL { get { return _clearcentre.CalUnRealizedPL(_acc); } }//计算未平仓利润
//        //public decimal Commission { get { return _clearcentre.CalCommission(_acc); } }//计算佣金


//        #region 【IFianceFutBase】
//        public decimal FutMarginUsed { get { return _clearcentre.CalFutMargin(_acc); } }//期货保证金
//        public decimal FutMarginFrozen { get { return _clearcentre.CalFutMarginFrozen(_acc); } }//期货冻结保证金
//        public decimal FutRealizedPL { get { return _clearcentre.CalFutRealizedPL(_acc); } }//期货平仓盈亏 
//        public decimal FutUnRealizedPL { get { return _clearcentre.CalFutUnRealizedPL(_acc); } }//期货浮动盈亏
//        public decimal FutCommission { get { return _clearcentre.CalFutCommission(_acc); } }//期货交易手续费
//        public decimal FutSettleUnRealizedPL { get { return _clearcentre.CalFutSettleUnRealizedPL(_acc); } }//期货结算时盯市盈亏
//        #endregion

//        #region【IFinanceOptBase】
//        public decimal OptPositionCost { get { return _clearcentre.CalOptPositionCost(_acc); } }//期权持仓成本
//        public decimal OptPositionValue { get { return _clearcentre.CalOptPositionValue(_acc); } }//期权持仓市值
//        public decimal OptRealizedPL { get { return _clearcentre.CalOptRealizedPL(_acc); } }//期权平仓盈亏
//        public decimal OptCommission { get { return _clearcentre.CalOptCommission(_acc); } }//期权手续费
//        public decimal OptMoneyFrozen { get { return _clearcentre.CalOptMoneyFrozen(_acc); } }//期权资金冻结
//        public decimal OptSettlePositionValue { get { return _clearcentre.CalOptSettlePositionValue(_acc);} }//期权结算市值
//        #endregion


//        #region 【IFinanceINNOVBase】
//        public decimal InnovPositionCost { get { return _clearcentre.CalInnovPositionCost(_acc); } }
//        public decimal InnovPositionValue { get { return _clearcentre.CalInnovPositionValue(_acc); } }
//        public decimal InnovCommission { get { return _clearcentre.CalInnovCommission(_acc); } }
//        public decimal InnovRealizedPL { get { return _clearcentre.CalInnovRealizedPL(_acc); } }
//        public decimal InnovMargin { get { return _clearcentre.CalInnovMargin(_acc); } }
//        public decimal InnovMarginFrozen { get { return _clearcentre.CalInnovMarginFrozen(_acc); } }//异化合约保证金
//        public decimal InnovSettlePositionValue { get { return _clearcentre.CalInnovSettlePositionValue(_acc); } }
//        #endregion

//        #region 【IAccOperation】
//        /// <summary>
//        /// 强平帐户持仓
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="comment"></param>
//        public void FlatPosition(QSEnumOrderSource source, string comment)
//        {
//            //_clearcentre.FlatPosition(_acc.ID, source, comment);
//        }

//        /// <summary>
//        /// 冻结帐户
//        /// </summary>
//        public void InactiveAccount()
//        {
//            _clearcentre.InactiveAccount(_acc.ID);
//        }

//        /*
//        public void FlatPosition(QSEnumOrderSource source, string comment)
//        {
//            _clearcentre.FlatPosition(_acc.ID, source, comment);
//        }

//        public void InactiveAccount()
//        {
//            _clearcentre.InactiveAccount(_acc.ID);

//        }**/

//        public void FlatPosition(Position pos, QSEnumOrderSource source, string comment)
//        {
//            //_clearcentre.FlatPosition(pos, source, comment);
//        }
//        #endregion


//        #region 【IAccTradingInfo】
//        //public bool AnyPosition
//        //{
//        //    get { return _clearcentre.AnyPosition(_acc.ID); }
//        //}

//        //public Position[] Positions { get { return _clearcentre.getPositions(_acc.ID); } }
//        //public Order[] Ordres { get { return _clearcentre.getOrders(_acc.ID); } }//获得当日所有委托
//        //public Trade[] Trades { get { return _clearcentre.getTrades(_acc.ID); } }//获得当日所有成交
//        //public long[] Cancels { get { return _clearcentre.getCancels(_acc.ID); } }//获得当日所有取消
//        //public Position[] PositionsHold { get { return _clearcentre.getPositionHold(_acc.ID); } }

//        /// <summary>
//        /// 获得某个symbol的持仓数据
//        /// </summary>
//        /// <param name="symbol"></param>
//        ///// <returns></returns>
//        //public Position getPosition(string symbol,bool side)
//        //{
//        //    return _clearcentre.getPosition(_acc.ID, symbol,side);
//        //}

//        #endregion


//        /// <summary>
//        /// 查询某个帐户某个委托需要占用的资金
//        /// </summary>
//        /// <param name="o"></param>
//        /// <returns></returns>
//        public decimal CalOrderFundRequired(Order o,decimal defaultvalue=0)//计算某个委托的冻结保证金
//        {
//            return _clearcentre.CalOrderFundRequired(o, defaultvalue);
//        }

//        public decimal GetAvabilePrice(string symbol)
//        {
//            return _clearcentre.GetAvabilePrice(symbol);
//        }

//    }
//}
