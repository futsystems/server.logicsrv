using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace AccountRuleSet
{
    public class RSMaxLoss :RuleBase, IAccountCheck
    {
        private decimal _profit;//用于内部使用的值
        public override string Value
        {
            get { return _profit.ToString(); }
            set
            {

                try
                {
                    _profit = Convert.ToDecimal(value);
                }
                catch (Exception ex)
                { }
            } 
        }



        bool flatStart = false;//强平开始
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal profit = this.Account.Profit;//获得该账户的当日利润
            if (profit >= 0 || flatStart ) return true;//如果该账户有利润或者已经强平则直接返回

            profit = Math.Abs(profit);
            bool ret = profit > _profit;//判断损失是否已经超过了设定的数额

            //LibUtil.Debug("帐户风控检查,最大亏损额:"+_profit.ToString() +" 当前亏损额:"+profit.ToString());
            //如果强平条件满足并且没有启动强平就强平仓位并禁止交易
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
                return "账户损失 " + Util.GetEnumDescription(this.Compare) + " " + _profit.ToString("N2") + "[" + SymbolSet + "]" +"强平仓位并禁止交易";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "强平[亏损金额]"; }
        }
        public static new string Description
        {
            get { return "当最大损失大于设定数额时,系统将强平仓位并禁止账户交易"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "检查变量"; } }

        //用于验证客户端的输入值是否正确
        public static new bool ValidSetting(RuleItem item, out string msg)
        {
            try
            {
                //Convert.ToDecimal(_rawvalue);
            }
            catch (Exception ex)
            {
                msg = "请设定有效数值";
                return false;
            }
            msg = "";
            return true;
        }

        #endregion
    }
}
