//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using TradingLib.Common;
//using TradingLib.API;

//namespace OrderRuleSet
//{
    
//    public class RSCommission : IOrderCheck
//    {
//        private IAccountExp _acc;
//        public IAccountExp Account { get { return _acc; } set { _acc = value; } }

//        /// <summary>
//        /// 数据库全局ID
//        /// </summary>
//        public int ID { get; set; }

//        //是否激活
//        private bool _enable = true;
//        public bool Enable { get { return _enable; } set { _enable = value; } }

//        bool _cansetvalue = true;
//        public bool CanSetValue { get { return _cansetvalue; } }
//        bool _cansetscompare = false;
//        public bool CanSetCompare { get { return _cansetscompare; } }
//        bool _cansetsymbols = false;
//        public bool CanSetSymbols { get { return _cansetsymbols; } }
//        //比较值的名称
//        public string ValueName { get { return "手续费"; } }

//        string _rawvalue = "";
//        private decimal _commission=1000;//用于内部使用的值
//        public string Value { get { return _commission.ToString(); } 
//            set {
//                try
//                {
//                    _rawvalue = value;
//                    _commission = Convert.ToDecimal(value);
//                }
//                catch (Exception ex)
//                { 
//                }
//                } 
//        }
//        //用于验证客户端的输入值是否正确
//        public bool ValidSetting(out string msg)
//        {
//            msg = "";
//            try
//            {
//                Convert.ToDecimal(_rawvalue);
//            }
//            catch (Exception ex)
//            {
//                msg = "请输入正确手续费额度";
//                return false;
//            }
//            return true;
//        }
//        //比较方式
//        public QSEnumCompareType _comparetype= QSEnumCompareType.LessEqual;
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
        
//        /// <summary>
//        /// 委托检查
//        /// </summary>
//        /// <param name="o"></param>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public bool checkOrder(Order o, out string msg)
//        {
//            //Security msec = _acc.ClearCentre.getMasterSecurity(o.symbol);
//            Symbol symbol = o.oSymbol;
//            msg = string.Empty;
//            if (!_enable)//如果该策略不起作用 则直接返回True
//                return true;
//            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
//            Position pos = _acc.getPosition(o.symbol);
//            if ((pos.isLong && !o.side) || (pos.isShort && o.side)) return true;
//            decimal commission = _acc.Commission;
//            bool ret = false;

//            ret = (commission <= _commission);
//            /*
//            switch (_comparetype)
//            {
//                case QSEnumCompareType.Equals:
//                    ret = true;
//                    break;
//                case QSEnumCompareType.Greater:
//                    ret = true;
//                    break;
//                case QSEnumCompareType.GreaterEqual:
//                    ret = true;
//                    break;
//                case QSEnumCompareType.Less:
//                    ret = (commission < _commission);
//                    break;
//                case QSEnumCompareType.LessEqual:
//                    ret = (commission <= _commission);
//                    break;
//                default:
//                    break;
//            }**/
//            if (!ret)
//            {
//                msg = RuleDescription + " 不满足,委托被拒绝";
//                o.comment = o.comment + "|" + msg;
//            }
//            return ret;

//        }
//        public string Key
//        {
//            get
//            {
//                string s = this.GetType().FullName + "," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
//                return s;
//            }
//        }
//        //文本保存规则
//        public string ToText()
//        {
//            string s = this.GetType().FullName + "," + Enable.ToString() + "," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
//            return s;
//        }
//        public string RuleDescription
//        {
//            get
//            {
//                return "开仓条件:手续费 小于 " + _commission.ToString("N4") + "[" + SymbolSet + "]";
//            }
//        }
//        //从配置文件得到对应的规则实例,用于进行检查
//        //RSMargin:1:0.1
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
//            get { return "RSCommission:手续费检查"; }
//        }
//        public static string Description
//        {
//            get { return "手续费[Compare]设定金额时,委托可接收"; }
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
