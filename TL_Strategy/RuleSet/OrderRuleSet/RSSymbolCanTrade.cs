using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    public class RSSymbolCanTrade :RuleBase,IOrderCheck
    {

        private decimal _percent=0;//用于内部使用的值
        /// <summary>
        /// 参数值
        /// </summary>
        public override string Value 
        { 
            get 
            { 
                return _percent.ToString(); 
            } 
            set
            {
                try
                {
                    _percent = Convert.ToDecimal(value);
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
        public bool checkOrder(Order o,out string msg)
        {
           
            msg = string.Empty;
            if (!o.IsEntryPosition) return true;//平仓不检查
            Symbol symbol = o.oSymbol;

            if (IsInSymbolSet(symbol))
            {
                return true;

            }
            else
            {
                msg = RuleDescription + " 不满足,委托被拒绝";
                o.Comment = msg;
                return false;
            }            
        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return "开仓条件:只允许交易集合["+this.SymbolSet.Replace('_',' ')+"]";
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "合约检查:允许交易"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "允许交易设置的品种,品种用逗号分割"; }
        }

        public static new bool CanSetCompare { get { return false; } }

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
