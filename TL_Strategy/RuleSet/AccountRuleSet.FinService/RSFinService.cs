using System;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace AccountRuleSet
{
    /// <summary>
    /// 配资风控策略
    /// 配资额50000 则本金为5000
    /// 计算出理论本金后 按本金的20%设定强平线 则为1000强平线
    /// 当权益低于10000之后,则强平并锁掉所有仓位
    /// </summary>
    public class RSFineService : IAccountCheck
    {
        private IAccountExp _acc;
        public IAccountExp Account { get { return _acc; } set { _acc = value; } }

        //是否激活
        private bool _enable = true;
        public bool Enable { get { return _enable; } set { _enable = value; } }

        bool _cansetvalue = true;
        public bool CanSetValue { get { return _cansetvalue; } }
        bool _cansetscompare = false;
        public bool CanSetCompare { get { return _cansetscompare; } }
        bool _cansetsymbols = false;
        public bool CanSetSymbols { get { return _cansetsymbols; } }


        //比较值的名称
        public string ValueName { get { return "账户权益"; } }
        private decimal _flatrate=0;//用于内部使用的值

        string _rawvalue = "";
        public string Value { get { return _flatrate.ToString(); } set {

            try
            {
                _rawvalue = value;
                _flatrate = Convert.ToDecimal(value);

            }
            catch (Exception ex)
            { 
            
            }
        }
        }
        //用于验证客户端的输入值是否正确
        public bool ValidSetting(out string msg)
        {
            msg = "";
            try
            {
                decimal r = Convert.ToDecimal(_rawvalue);
                if (r > 0 && r < 100)
                {

                }
                else
                {
                    msg = "强平比例在[1-100]之间";
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = "请输入有效数字";
                return false;
            }
            return true;
        }
        //比较方式
        public QSEnumCompareType _comparetype = QSEnumCompareType.LessEqual;
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
        bool flatdone = false;//完成强平
        bool flatStart = false;//强平开始
        int CancelRetry = 4;
        int _cancelnum = 0;
        //规则检查函数
        //Margin检查用于检查保证金占用
        public bool CheckAccount(out string msg)
        {
            //当前权益低于 早晨加载服务时设定的额度对应的强平金额,执行强平
            //系统会根据盘中的权益动态的调整可用额度。结算时候按照单日权益自动调整额度设定
            //同时需要检查 当前使用的保证金是否超过了 盘中动态可用保证金,如果超过则平掉部分仓位
            msg = string.Empty;
            decimal finammount = 0;// _acc.FinAmmountTotal;//获得账户配资额
            if (finammount <= 0) return true;//如果账户配资额度为0 则该规则无效，该规则是针对有配资额度的账户
            decimal equity = finammount / 10;//获得该账户的最低自由资金
            decimal _stopequity = equity*0.25M;//获得强平线
            decimal nowequity = _acc.NowEquity;

            if (nowequity > _stopequity || flatdone) return true;//如果当前权益大于强平线 或者已经被强平 则直接返回

            bool ret = false;
            ret = nowequity <= _stopequity;//当前权益<=强平线 标识

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
                    //如果还有仓位则延迟5秒再次平仓
                    if ((DateTime.Now - _flattime).TotalSeconds > 5 && _cancelnum < CancelRetry)//15秒后任然没有平仓 则再次平仓(累计尝试平仓3次)
                    {
                        _cancelnum++;
                        
                        _flattime = DateTime.Now;
                        msg = "RSFineService 账户:" + _acc.ID + "平仓未成功 再次平仓(" + _cancelnum.ToString() + ")";
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
            //如果强平条件满足并且没有启动强平 就强平仓位并禁止交易
            if (ret && !flatStart)
            {
                if (_acc.Execute)
                    _acc.InactiveAccount();//冻结账户
                //如果有仓位则平仓问题1,如果交易没有成交 会造成频繁发单。这里需要有个机制来解决这个问题
                //连续发送3次平仓指令,若还有持仓说明平仓单有问题
                //第一次触发平仓委托,并且账户有持仓
                if (_cancelnum == 0 && _acc.AnyPosition)
                {
                    _cancelnum++;
                    

                    msg = RuleDescription + ":全平所有仓位并冻结账户";
                    _acc.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);//全平持仓
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
                return "配资账户权益低于 配资额度所对应的初始权益一定比例(强平线"+_flatrate.ToString()+"% 强平仓位并禁止交易";
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
            get { return "强平[配资账户风控]"; }
        }
        public static string Description
        {
            get { return "账户权益亏损至强平线强平账户并禁止交易"; }
        }



    }
}
