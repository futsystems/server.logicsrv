//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.Common;
//using TradingLib.API;
//namespace OrderRuleSet
//{
//    public class RSTime : IOrderCheck
//    {

//        private IAccountExp _acc;
//        public IAccountExp Account { get { return _acc; } set { _acc = value; } }

//        /// <summary>
//        /// 数据库全局ID
//        /// </summary>
//        public int ID { get; set; }

//        private bool _enable=true;
//        public bool Enable { get{return _enable;} set{_enable=value;}}

//        bool _cansetvalue = true;
//        public bool CanSetValue { get { return _cansetvalue; } }
//        bool _cansetscompare = true;
//        public bool CanSetCompare { get { return _cansetscompare; } }
//        bool _cansetsymbols = true;
//        public bool CanSetSymbols { get { return _cansetsymbols; } }


//        public string ValueName { get { return "开仓时间"; } }
//        private int _time;
//        //public DateTime Time { get { return _time; } set { _time = value; } }

//        //用于验证客户端的输入值是否正确
//        public bool ValidSetting(out string msg)
//        {
//            msg = "";
//            try
//            {
//                if (_comparetype == QSEnumCompareType.In || _comparetype == QSEnumCompareType.Out)
//                {
//                    msg = "时间检查不能选择之内或除外比较选项";
//                    return false;
//                }
//                try
//                {
//                    int.Parse(_rawvalue);
//                }
//                catch (Exception ex)
//                {
//                    msg = "时间必须是整数格式";
//                    return false;
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                return true;
//            }
            
//        }

//        //private object _val;
//        string _rawvalue = "";
//        public string Value { get 
//            { return _time.ToString(); } 
            
//            set 
//            {
//                try
//                {
//                    _rawvalue = value;
//                    _time = Convert.ToInt32(value);
//                }
//                catch (Exception ex)
//                { 
                    
//                }
            
//            } 
//        }

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
//        public bool checkOrder(Order o,out string msg)
//        {

//            msg = string.Empty;
//            if (!_enable)
//                return true;
//            if (_symbolset!=null&& !_symbolset.Contains(SymbolHelper.genSecurityCode(o.symbol)))
//                return true;//如果当前Order的symbol不在我们检查行列，我们直接返回Ture
//            bool ret = false;
//            switch (_comparetype)
//            {
//                case QSEnumCompareType.Equals:
//                    ret = (o.time == _time);
//                    break;
//                case QSEnumCompareType.Greater:
//                    ret =  (o.time > _time);
//                    break;
//                case QSEnumCompareType.GreaterEqual:
//                    ret =  (o.time >= _time);
//                    break;
//                case QSEnumCompareType.Less:
//                    ret = (o.time < _time);
//                    break;
//                case QSEnumCompareType.LessEqual:
//                    ret = (o.time <= _time);
//                    break;
//                default:
//                    break;
//            }
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

//        public string ToText()
//        {
//            string s = this.GetType().FullName+"," +Enable.ToString()+","+ Compare.ToString() + "," +Value.ToString()+","+SymbolSet.ToString();
//            return s;
//        }
//        public string RuleDescription
//        {
//            get
//            {
//                return "开仓条件:开仓时间 " + LibUtil.GetEnumDescription(_comparetype) + " " + Util.FT2DT(_time).ToString("HH:mm:ss") + "[" + SymbolSet + "]";
//            }
//        }

//        //从配置文件得到对应的规则实例,用于进行检查
//        public  IRule FromText(string rule)
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
//            get { return "RSTime:时间检查"; }
//        }
//        public static string Description
//        {
//            get { return "当委托时间[Compare]设定时间,该委托可接收(选定品种则检查委托品种,不选品种则检查所有品种),时间格式 13点14分20秒 输入 131420"; }
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
