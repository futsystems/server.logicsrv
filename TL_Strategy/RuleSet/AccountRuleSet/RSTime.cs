using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace AccountRuleSet
{
    public class RSTime :RuleBase, IAccountCheck
    {
        private int _time=0;//用于内部使用的值
        public override string Value
        {
            get { return _time.ToString(); }
            set
            {

                try
                {
                    _time = Convert.ToInt32(value);
                }
                catch (Exception ex)
                { }
            } 
        }

        /// <summary>
        /// 是否需要检查品种
        /// 品种在列表内的执行时间检查 其余的不做检查
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected bool NeedCheckSymbol(Symbol symbol)
        {
            if (_symbolset == null || _symbolset.Count==0) return false;
            return true;
        }


        bool flatStart = false;//强平开始
        public bool CheckAccount(out string msg)
        {
            
            msg = string.Empty;
            int diff = Util.ToTLTime()-_time;//计算当前时间与设定时间的diff
            bool ret = (Math.Abs(diff)<5);
            if (!ret || flatStart) return true;//如果与设定时间相差10秒 直接返回

            if (ret && !flatStart)
            {
                //如果没有设定合约则强平所有持仓,那么我们需要冻结交易帐户
                //if (_symbolset == null && _symbolset.Count == 0)
                //{
                    if (this.Account.Execute)
                        this.Account.InactiveAccount();//冻结账户
                //}
                if (this.Account.AnyPosition)
                {
                    msg = RuleDescription + ":强平对应持仓";
                    foreach (var pos in this.Account.Positions.Where(p => !p.isFlat))
                    {
                        //设置了品种列表需要检查
                        if (_symbolset != null && _symbolset.Count > 0)
                        {
                            if (_symbolset.Contains(pos.oSymbol.SecurityFamily.Code))
                            {
                                this.Account.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, msg);
                            }
                        }
                        else
                        {
                            this.Account.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, msg);
                        }
                        Util.sleep(10);
                    }
                    
                    //this.Account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
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
                return "执行时间 " + Util.GetEnumDescription(this.Compare) + " " + _time.ToString() + "[" + SymbolSet + "]" + "强平持仓并禁止交易";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "强平[执行时间]"; }
        }
        public static new string Description
        {
            get { return "到执行时间时,强平持仓,145500代表14点55分00秒"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "执行时间"; } }

        /// <summary>
        /// 不用设置比较关系
        /// </summary>
        public static new bool CanSetCompare { get { return false; } }

        /// <summary>
        /// 默认比较关系大于等于
        /// </summary>
        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.Equals; } }
        
        /// <summary>
        /// 不用设置品种集合
        /// </summary>
        public static new bool CanSetSymbols { get { return true; } }

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
