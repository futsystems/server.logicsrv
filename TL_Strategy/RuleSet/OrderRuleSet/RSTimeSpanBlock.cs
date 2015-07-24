using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    public class RSTimeSpanBlock : RuleBase, IOrderCheck
    {
        private int _start = 0;
        private int _end = 0;
        /// <summary>
        /// 参数值
        /// </summary>
        public override string Value
        {
            get
            {
                return string.Format("{0}-{1}", _start, _end);
            }
            set
            {
                try
                {
                    string[] strs = value.Split('-');
                    if (strs.Length == 2)
                    {
                        _start = int.Parse(strs[0]);
                        _end = int.Parse(strs[1]);
                    }
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


            int now = Util.ToTLTime();
            bool ret = now >= _start && now <= _end;
            if (ret)
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
                return "禁止开仓条件:时间段 " +_start.ToString()+ " - " +_end.ToString() + " [" + SymbolSet + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "时间段内禁止开仓"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "在设定时间段内,禁止开仓,93000-103021"; }
        }

        public static new bool CanSetCompare { get { return false; } }

        public static new  QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.LessEqual; } }
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
