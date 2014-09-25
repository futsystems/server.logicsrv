using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    public class RSMargin :RuleBase,IOrderCheck
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
            if (!this.Enable)//如果该策略没激活 则直接返回True
                return true;
            
            Symbol symbol = o.oSymbol;

            //如果不需要检查该合约则直接返回true
            if (!NeedCheckSymbol(symbol))
                return true;

            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
            Position pos = Account.GetPosition(o.symbol,o.side);
            if ((pos.isLong && !o.side) || (pos.isShort && o.side)) return true;

            //如果是开仓，可用资金-该Order需要占用的保证金为该Order成交后所剩余可用资金，该资金比例是我们监控的主要项目
            decimal power = Account.AvabileFunds - Account.CalOrderFundRequired(o, 0);

            decimal per = (power / Account.NowEquity) * 100;

            bool ret = per >= _percent;
            if (!ret)
            {
                msg = RuleDescription + " 不满足,委托被拒绝";
                o.comment = msg;
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
                return "开仓条件:当可用资金比例 " + LibUtil.GetEnumDescription(this.Compare) + " " + _percent.ToString("N2") + "%" + " [" + SymbolSet + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "RSMargin:风险比例检查"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "当可用资金大于设定百分比时,该委托可接收(可用资金是当前可用资金扣除该委托成交所占用保证金),可用资金比例大于20% 则参考值填写20"; }
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
