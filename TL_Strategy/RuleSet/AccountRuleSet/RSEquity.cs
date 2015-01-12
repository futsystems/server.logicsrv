using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace AccountRuleSet
{
    public class RSEquity :RuleBase, IAccountCheck
    {
        private decimal _equity=0;//用于内部使用的值
        public override string Value
        {
            get { return _equity.ToString(); }
            set
            {

                try
                {
                    _equity = Convert.ToDecimal(value);
                }
                catch (Exception ex)
                { }
            } 
        }



        bool flatStart = false;//强平开始
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal equity = this.Account.NowEquity;//获得该账户的当前权益
            bool ret = equity <= _equity;//当前权益小于设定权益
            if (!ret || flatStart) return true;//如果该账户有利润或者已经强平则直接返回

            if (ret && !flatStart)
            {
                if (this.Account.Execute)
                    this.Account.InactiveAccount();//冻结账户
                if (this.Account.AnyPosition)
                {
                    msg = RuleDescription + ":全平所有仓位并冻结账户";
                    this.Account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                }
                flatStart = true;//开始平仓
                return false;
            }
            else
                return true;
        }

        public override string RuleDescription
        {
            get
            {
                return "帐户权益 " + Util.GetEnumDescription(this.Compare) + " " + _equity.ToString("N2") + "[" + SymbolSet + "]" + "强平持仓并禁止交易";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "强平[帐户权益]"; }
        }
        public static new string Description
        {
            get { return "帐户权益小于设定值时,强平持仓并禁止交易"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "当前权益"; } }

        /// <summary>
        /// 不用设置比较关系
        /// </summary>
        public static new bool CanSetCompare { get { return false; } }

        /// <summary>
        /// 默认比较关系大于等于
        /// </summary>
        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.Less; } }
        
        /// <summary>
        /// 不用设置品种集合
        /// </summary>
        public static new bool CanSetSymbols { get { return false; } }

        //用于验证客户端的输入值是否正确
        public static new bool ValidSetting(RuleItem item, out string msg)
        {
            try
            {
                decimal v = decimal.Parse(item.Value);
                if (v < 0)
                {
                    msg = "请去掉负号";
                    return false;
                }
                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = "请设定有效数值";
                return false;
            }
            
        }

        #endregion
    }
}
