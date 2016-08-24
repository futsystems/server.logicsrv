using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    public class RSPriceLimit2 : RuleBase, IOrderCheck
    {

        private decimal _ratio = 0;//用于内部使用的值
        /// <summary>
        /// 参数值
        /// </summary>
        public override string Value
        {
            get
            {
                return _ratio.ToString();
            }
            set
            {
                try
                {
                    _ratio = Convert.ToDecimal(value);
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
            if (NeedCheckSymbol(o.oSymbol) && !IsInSymbolSet(o.oSymbol))
                return true;

            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
            if (!o.IsEntryPosition) return true;

            Tick k = TLCtxHelper.CmdUtils.GetTickSnapshot(o.Symbol);

            //价格超过涨跌幅度 则拒绝
            if (k.PreSettlement == 0) return true;
            if (k.UpperLimit <= 0) return true;

            bool flag = k.Trade - k.PreSettlement > 0;
            if (flag && o.PositionSide) return true;
            if ((!flag) && (!o.PositionSide)) return true;

            //开仓条件 涨跌幅小鱼设定的百分比
            decimal d = k.UpperLimit - k.PreSettlement;
            decimal d2 = (d - Math.Abs(k.Trade - k.PreSettlement)) /k.PreSettlement * 100m;
            bool flag2 = d2 <= this._ratio;
            //当前距离涨跌停幅度小于设定值 
            if (flag2)
            {
                msg = this.RuleDescription + " 委托被拒绝";
                o.Comment = msg;
            }
            return !flag2;

        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return "开仓条件:距离涨跌停 " + Util.GetEnumDescription(this.Compare) + " " + _ratio.ToString() + "% [" + SymbolSet + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "涨跌停检查:涨跌停禁止开仓"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "价格离涨跌停还剩%X,拒绝开仓"; }
        }

        public static new bool CanSetCompare { get { return false; } }

        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.LessEqual; } }
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
