using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    public class RSOrderSizeLimit : RuleBase, IOrderCheck
    {

        private int _orderSize = 0;//用于内部使用的值
        /// <summary>
        /// 参数值
        /// </summary>
        public override string Value
        {
            get
            {
                return _orderSize.ToString();
            }
            set
            {
                try
                {
                    _orderSize = Convert.ToInt16(value);
                }
                catch (Exception ex)
                {

                }
            }

        }


        /// <summary>
        /// 委托检查逻辑过程,如果接受委托返回true,拒绝委托返回false
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool checkOrder(Order o, out string msg)
        {
            msg = string.Empty;
            Symbol symbol = o.oSymbol;

            //需要检查合约 且 不在合约集 则返回true
            if (NeedCheckSymbol(o.oSymbol) && !_symbolset.Contains(o.oSymbol.SecurityFamily.Code))
                return true;

            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
            if (!o.IsEntryPosition) return true;
            //查询当前品种的所有开仓成交 并计算累加开仓手数
            List<Trade> entryTrades = Account.Trades.Where(tmp => tmp.oSymbol.SecurityFamily.Code == o.oSymbol.SecurityFamily.Code && tmp.IsEntryPosition).ToList();
            int entrySize = entryTrades.Sum(tmp => Math.Abs(tmp.xSize));

            List<Order> pendingOrders =  Account.Orders.Where(tmp => tmp.oSymbol.SecurityFamily.Code == o.oSymbol.SecurityFamily.Code && tmp.IsEntryPosition).Where(tmp=>tmp.IsPending()).ToList();
            int pendingOrderSize = pendingOrders.Sum(tmp => Math.Abs(tmp.Size));


            bool ret = entrySize + pendingOrderSize +  o.UnsignedSize <= _orderSize;//当前持仓数量+欲开仓术量 小于等于 最大持仓数量
            if (!ret)
            {
                msg = RuleDescription + " 委托被拒绝";
                o.Comment = msg;
            }
            return ret;

        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return "开仓条件:最大交易手数 " + Util.GetEnumDescription(this.Compare) + " " + _orderSize.ToString() + " [" + SymbolSet + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "交易手数检查:最大交易手数"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "交易手数小于设定值,允许开仓"; }
        }

        public static new bool CanSetCompare { get { return false; } }

        public static QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.LessEqual; } }
        /// <summary>
        /// 验证ruleitem设置
        /// </summary>
        /// <param name="item"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static new bool ValidSetting(RuleItem item, out string msg)
        {
            msg = "";
            try
            {
                decimal p = 0;
                decimal.TryParse(item.Value, out p);
                if (p < 1 || p >= 100)
                {
                    msg = "比例参考值必须在[1-100]之间";
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = "请设定有效数字";
                return false;
            }
            return true;
        }
        #endregion


    }
}
