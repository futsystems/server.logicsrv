using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace AccountRuleSet
{
    public class RSMaxLossPercent2Step : IAccountCheck
    {
        private IAccountExp _acc;
        public IAccountExp Account { get { return _acc; } set { _acc = value; } }

        //是否激活
        private bool _enable = true;
        public bool Enable { get { return _enable; } set { _enable = value; } }

        bool _cansetvalue = false;
        public bool CanSetValue { get { return _cansetvalue; } }
        bool _cansetscompare = false;
        public bool CanSetCompare { get { return _cansetscompare; } }
        bool _cansetsymbols = false;
        public bool CanSetSymbols { get { return _cansetsymbols; } }


        //比较值的名称
        public string ValueName { get { return "账户损失比例"; } }
        private decimal _profit;//用于内部使用的值

        private decimal _percent;
        public string Value { get { return ""; } set { } }
        //用于验证客户端的输入值是否正确
        public bool ValidSetting(out string msg)
        {
            msg = "";
            return true;
        }
        //比较方式
        public QSEnumCompareType _comparetype;
        public QSEnumCompareType Compare { get { return _comparetype; } set { _comparetype = value; } }
        //检查的合约列表
        string[] _symbolset;
        public string SymbolSet
        {
            get
            {
                if (_symbolset == null)
                    return string.Empty;
                else
                    return string.Join("|", _symbolset);
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    _symbolset = value.Split('|');
                }
            }
        }

        DateTime _flattime;
        bool flatdone = false;
        bool flatStart = false;
        int CancelRetry = 4;
        int _cancelnum = 0;
        
        //计算账户的信息然后返回动态的止损百分比
        decimal losspercent(IAccountExp acc)
        {
            decimal observerprofit = 0;// acc.ObverseProfit;
            decimal startequity = 0;// acc.StartEquity;
            if (startequity <= 0) return 2;//如果账户初始权益无效,则返回默认的2%强平止损
            decimal percent = (observerprofit / startequity)*100;
            //折算收益率<=10%则2%强平止损, 折算收益率>10%则4%强平止损
            if (percent <= 10)
            {
                return 2;
            }
            else
            {
                return 4;
            }

        
        }
        //规则检查函数
        //Margin检查用于检查保证金占用
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal profit = _acc.Profit;//获得该账户的当日利润
            if (profit >= 0 || flatdone) return true;//如果该账户有利润或者已经禁止了交易并且强平完成则直接返回
            profit = Math.Abs(profit);
            //当账户亏算则 计算 昨日总权益与亏损比例之积
            _profit = losspercent(_acc) * 0;// _acc.StartEquity / 100;
            bool ret = false;
            switch (_comparetype)//进行逻辑比较
            {
                case QSEnumCompareType.Equals:
                    ret = (profit == _profit);
                    break;
                case QSEnumCompareType.Greater:
                    ret = (profit > _profit);
                    break;
                case QSEnumCompareType.GreaterEqual:
                    ret = (profit >= _profit);
                    break;
                case QSEnumCompareType.Less:
                    ret = (profit < _profit);
                    break;
                case QSEnumCompareType.LessEqual:
                    ret = (profit <= _profit);
                    break;
                default:
                    break;
            }

            //平仓被启动后 我们需要一直检验 持仓情况
            if (flatStart)
            {
                //如果仓位已经平掉则 设定flatdone为true 强平仓位完成
                if (!_acc.AnyPosition)
                {
                    //如果平掉了所有仓位 则标注 平仓结束
                    flatdone = true;
                    return true;
                }
                else
                {
                    //如果还有仓位则延迟15秒再次平仓
                    if ((DateTime.Now - _flattime).TotalSeconds > 15 && _cancelnum < CancelRetry)//15秒后任然没有平仓 则再次平仓(累计尝试平仓3次)
                    {
                        _cancelnum++;
                        
                        _flattime = DateTime.Now;
                        msg = "RSMaxLossPercent2Step 账户:" + _acc.ID + "平仓未成功 再次平仓(" + _cancelnum.ToString() + ")";
                        _acc.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                        return false;
                    }
                    //如果强平4次任然没有平掉对应的仓位,则我们向管理端报警(每30秒进行一次报警)
                    if (_cancelnum == CancelRetry && (DateTime.Now - _flattime).TotalSeconds > 30)
                    {
                        _flattime = DateTime.Now;
                        //报警程序
                    }
                    return true;
                }
            }
            //如果强平条件满足并且没有启动强平就强平仓位并禁止交易
            if (ret && !flatStart)
            {
                if (_acc.Execute)
                    _acc.InactiveAccount();//冻结账户
                //如果有仓位则平仓问题1,如果交易没有成交 会造成频繁发单。这里需要有个机制来解决这个问题
                //连续发送3次平仓指令,若还有持仓说明平仓单有问题
                //第一次触发平仓委托
                if (_cancelnum == 0 && _acc.AnyPosition)
                {
                    _cancelnum++;
                    _acc.InactiveAccount();
                    msg = RuleDescription + ":全平所有仓位并冻结账户";
                    _acc.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                    _flattime = DateTime.Now;
                    flatStart = true;//开始平仓
                }
                return false;
            }
            else
                return true;
        }
        public string Key
        {
            get
            {
                string s = this.GetType().FullName + "," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
                return s;
            }
        }

        //文本保存规则
        public string ToText()
        {
            string s = this.GetType().FullName + "," + Enable.ToString() + "," + Compare.ToString() + "," + Value.ToString() + "," + SymbolSet.ToString();
            return s;
        }
        public string RuleDescription
        {
            get
            {
                return "账户强平仓位并禁止交易(折算收益率10%以内 2%强平 10%以上 4%强平 )";
            }
        }
        //从配置文件得到对应的规则实例,用于进行检查
        //RSMargin:1:0.1
        public IRule FromText(string rule)
        {
            string[] p = rule.Split(',');
            Enable = bool.Parse(p[1]);
            Compare = (QSEnumCompareType)Enum.Parse(typeof(QSEnumCompareType), p[2], true);
            Value = p[3];
            SymbolSet = p[4];
            return this;
        }
        public static string Name
        {
            get { return "强平[亏损占初始金额2阶段(比赛)]"; }
        }
        public static string Description
        {
            get { return "两阶段止损折算收益率<=10% 则为2%强平止损,折算收益率>10%则为4%强平止损(初始权益)"; }
        }



    }
}
