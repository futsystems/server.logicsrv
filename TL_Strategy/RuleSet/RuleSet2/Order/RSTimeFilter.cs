using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace RuleSet2.Order
{
    /// <summary>
    /// 最大交易手数限制
    /// </summary>
    public class RSTimeFilter : RuleBase, IOrderCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 品种列表
        /// </summary>
        private List<string> sec_list = new List<string>();

        /// <summary>
        /// 参数值
        /// </summary>
        public override string Value
        {
            get { return _args; }
            set
            {

                try
                {
                    _args = value;
                    //解析json参数
                    var args = _args.DeserializeObject();

                    //解析品种列表
                    string secs = args["sec_list"].ToString();
                    if (!string.IsNullOrEmpty(secs))
                    {
                        foreach (var sec in secs.Split(','))
                        {
                            sec_list.Add(sec);
                        }
                    }


                    _timeFilterStr = args["timespan"].ToString();
                    _tslist.Clear();
                    if (string.IsNullOrEmpty(_timeFilterStr)) return;
                    foreach (var str in _timeFilterStr.Split(','))
                    {
                        RuleTimeSpan ts = RuleTimeSpan.Deserialize(str);
                        if (ts != null)
                        {
                            _tslist.Add(ts);
                        }
                    }
                }
                catch (Exception ex)
                { }
            } 

        }

        string _timeFilterStr = string.Empty;
        List<RuleTimeSpan> _tslist = new List<RuleTimeSpan>();

        /// <summary>
        /// 检查是否在设定的时间区间内
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool ValidTimeFilter(int time)
        {
            if (_tslist.Count == 0) return true;
            foreach (var tmp in _tslist)
            {
                if (tmp.InSpan(time))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 委托检查逻辑过程,如果接受委托返回true,拒绝委托返回false
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool checkOrder(TradingLib.API.Order o, out string msg)
        {
            msg = string.Empty;

            Symbol symbol = o.oSymbol;

            //设置了品种 且当前委托不在品种内 则不执行风控检查 (如果未设置品种 则所有委托进行检查)
            if (sec_list.Count>0 && !sec_list.Contains(o.oSymbol.SecurityFamily.Code))
                return true;

            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
            if (!o.IsEntryPosition) return true;

            bool ret = ValidTimeFilter(Util.ToTLTime());
            if (!ret)
            {
                msg = RuleDescription + " 委托被拒绝";
                o.Comment = msg;
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
                return "开仓条件:交易时间段在" + _timeFilterStr + " [" + string.Join(",", sec_list.ToArray()) + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "交易时间段检查"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "在交易时间段内,允许开仓"; }
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
