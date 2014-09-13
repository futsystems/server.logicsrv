//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using TradingLib.Common;
//using TradingLib.API;
//using TradingLib;
//namespace OrderRuleSet
//{
//    public class RSSymbolFilter : IOrderCheck
//    {
//        private IAccountExp _acc;
//        public IAccountExp Account { get { return _acc; } set { _acc = value; } }

//        /// <summary>
//        /// 数据库全局ID
//        /// </summary>
//        public int ID { get; set; }

//        private bool _enable = true;
//        public bool Enable { get { return _enable; } set { _enable = value; } }

//        //合约检查不设置value,只是这合约列表
//        bool _cansetvalue = false;
//        public bool CanSetValue { get { return _cansetvalue; } }

//        bool _cansetscompare = true;
//        public bool CanSetCompare { get { return _cansetscompare; } }

//        bool _cansetsymbols = true;
//        public bool CanSetSymbols { get { return _cansetsymbols; } }

//        public string ValueName { get { return "可交易品种"; } }
      
//        //用于验证客户端的输入值是否正确
//        public bool ValidSetting(out string msg)
//        {
//            msg = "";
//            try
//            {
//                if (!(_comparetype == QSEnumCompareType.In || _comparetype == QSEnumCompareType.Out))
//                {
//                    msg = "品种检查逻辑关系只能选择之内或除外";
//                    return false;
//                }
//                if (_symbolset == null || _symbolset.Length < 1)
//                {
//                    msg = "请选择品种列表";
//                    return false;
//                }
//                //DateTime dt = Util.ToDateTime(Util.ToTLDate(DateTime.Now), Convert.ToInt32(value));
//                return true;
//            }
//            catch (Exception ex)
//            {
                
//                return true;
//            }

//        }

//        //private object _val;
//        public string Value { get; set; }

//        public QSEnumCompareType _comparetype;
//        public QSEnumCompareType Compare { get { return _comparetype; } set { _comparetype = value; } }

//        //检查的合约列表
//        string[] _symbolset;
//        public string SymbolSet
//        {
//            get
//            {
//                if (_symbolset == null)
//                    return string.Empty;
//                else
//                    return string.Join("|", _symbolset);
//            }
//            set
//            {
//                if (value != null && value != string.Empty)
//                {
//                    _symbolset = value.Split('|');
//                }
//            }
//        }

//        //规则检查函数
//        public bool checkOrder(Order o, out string msg)
//        {
//            msg = string.Empty;
//            if (!_enable)
//                return true;
//            /*
//            Security msec = _acc.ClearCentre.getMasterSecurity(o.symbol);
//            if (msec == null)
//            {
//                msg = "系统内没有该合约信息";
//                o.comment = o.comment + "|" + msg;
//                return false;
//            }**/
//            Symbol symbol = o.oSymbol;
//            msg = string.Empty;

//            if (Compare == QSEnumCompareType.In)
//            {
//                if (_symbolset == null || !_symbolset.Contains(SymbolHelper.genSecurityCode(symbol.Symbol)))//如果可交易合约列表为空 或者 列表中没有该有效合约 则我们拒绝该Order
//                {
//                    msg = RuleDescription + " 无权交易该合约,委托被拒绝";
//                    o.comment = o.comment + "|" + msg;
//                    return false;
//                }
//            }
//            else if (Compare == QSEnumCompareType.Out)
//            {
//                if (_symbolset.Contains(SymbolHelper.genSecurityCode(symbol.Symbol)))//如果可交易合约列表为空 或者 列表中没有该有效合约 则我们拒绝该Order
//                {
//                    msg = RuleDescription + " 无权交易该合约,委托被拒绝";
//                    o.comment = o.comment + "|" + msg;
//                    return false;
//                }
//            }
//            return true;
 
//        }

//        public string Key
//        {
//            get
//            {
//                string s = this.GetType().FullName + "," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
//                return s;
//            }
//        }

//        public string ToText()
//        {
//            string s = this.GetType().FullName + ","+Enable.ToString()+"," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
//            return s;
//        }
//        public string RuleDescription
//        {
//            get
//            {
//                return "开仓条件:可交易品种 " + LibUtil.GetEnumDescription(_comparetype) + " " + "[" + SymbolSet + "]";
//                //return "";
//            }
//        }

//        //从配置文件得到对应的规则实例,用于进行检查
//        public IRule FromText(string rule)
//        {
//            string[] p = rule.Split(',');
//            Enable = bool.Parse(p[1]);
//            Compare = (QSEnumCompareType)Enum.Parse(typeof(QSEnumCompareType), p[2], true);
//            Value = p[3];
//            SymbolSet = p[4];

//            return this;
//        }

//        public static string Name
//        {
//            get { return "RSSymbolFilter:品种检查"; }
//        }
//        public static string Description
//        {
//            get { return "帐户可以交易列表之内或之外的品种"; }
//        }


//        /// <summary>
//        /// 初始化风控规则
//        /// </summary>
//        /// <param name="item"></param>
//        public void FromRuleItem(IRuleItem item)
//        {
//            ID = item.ID;
//            Enable = item.Enable;
//            Compare = item.Compare;
//            Value = item.Value;
//            SymbolSet = item.SymbolSet;
//        }

//        /// <summary>
//        /// 生成IRuleItem用于储存输出
//        /// </summary>
//        /// <returns></returns>
//        public IRuleItem ToRuleItem()
//        {
//            IRuleItem item = new RuleItem();
//            item.ID = this.ID;
//            item.Enable = this.Enable;
//            item.Compare = this.Compare;
//            item.Value = this.Value;
//            item.SymbolSet = this.SymbolSet;
//            item.RuleType = QSEnumRuleType.OrderRule;
//            return item;
//        }

//    }
//}
