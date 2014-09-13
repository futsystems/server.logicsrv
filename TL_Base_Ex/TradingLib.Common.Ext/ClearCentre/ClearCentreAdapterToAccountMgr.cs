using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /*
    /// <summary>
    /// 用于将clearcentresrv的部分功能函数暴露给Account调用,但是Account不能获得太多功能调用(越权使用)
    /// 将IClearCentre的功能进行一定程度的封装,使得Account里面方便调用获得财务信息,交易信息等
    /// </summary>
    public class ClearCentreAdapterToAccountMgr:IAccountClearCentre
    {
        private IAccount _acc;
        private IClearCentreBase _clearcentre;
        public ClearCentreAdapterToAccountMgr(IAccount a, IClearCentreBase c)
        {
            _acc = a;
            _clearcentre = c;
        
        }

        public Security getMasterSecurity(string symbol)
        {
            return _clearcentre.getMasterSecurity(symbol);
        }

        public decimal CalFrozenMargin(Order o)//计算某个委托的冻结保证金
        {
            return _clearcentre.CalFrozenMargin(o);
        }


        public decimal Margin { get { throw new NotImplementedException(); } }//计算保证金占用
        public decimal FrozenMargin { get { throw new NotImplementedException(); } }//计算占用保证金
        public decimal FinAmmountAvabile { get { throw new NotImplementedException(); } }
        public decimal FinAmmountTotal { get { throw new NotImplementedException(); } }

        public decimal RealizedPL { get { throw new NotImplementedException(); } }//计算平仓利润
        public decimal UnRealizedPL { get { throw new NotImplementedException(); } }//计算未平仓利润
        public decimal Commission { get { throw new NotImplementedException(); } }//计算佣金
        public Position[] Positions { get { throw new NotImplementedException(); } }
        public Order[] Ordres { get { return _clearcentre.getOrders(_acc.ID); } }//获得当日所有委托
        public Trade[] Trades { get { return _clearcentre.getTrades(_acc.ID); } }//获得当日所有成交
        public long[] Cancels { get { return _clearcentre.getCancels(_acc.ID); } }//获得当日所有取消
        public Position[] PositionsHold { get { return _clearcentre.getPositionHold(_acc.ID); } }

        public decimal ObverseProfit { get { return 0; } }
        /// <summary>
        /// 获得某个symbol的持仓数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string symbol)
        {
            return _clearcentre.getPosition(_acc.ID, symbol);
        }

        public void UpdateMasterSecurity()
        {
            _clearcentre.ReloadAccountMasterSecurity(_acc.ID);
        }

        public object OrderTracker
        {
            get {
                return _clearcentre.getOrderTracker(_acc.ID);
            }
        }

        public object PositionTracker
        {
            get
            {
                return _clearcentre.getPositionTracker(_acc.ID);
            }
        }

        public bool AnyPosition
        {
            get { return _clearcentre.AnyPosition(_acc.ID); }
        }

        public void  FlatPosition(QSEnumOrderSource source, string comment)
        {
            _clearcentre.FlatPosition(_acc.ID,source, comment);
        }

        public void InactiveAccount()
        {
            _clearcentre.InactiveAccount(_acc.ID);

        }
    }**/
}
