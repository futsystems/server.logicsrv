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
    public class RSMaxTradeSize : RuleBase, IOrderCheck
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

                    _tradeSize = int.Parse(args["trade_size"].ToString());

                    //解析品种列表
                    string secs = args["sec_list"].ToString();
                    if (!string.IsNullOrEmpty(secs))
                    {
                        foreach (var sec in secs.Split(','))
                        {
                            sec_list.Add(sec);
                        }
                    }




                }
                catch (Exception ex)
                { }
            } 

        }

        int _tradeSize = 0;

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

            //设置品种列表 且当前交易品种不在列表内 则不检查手数限制
            if (sec_list.Count > 0 && (!sec_list.Contains(o.oSymbol.SecurityFamily.Code)))
            {
                return true;
            }
            //如果没有指定品种列表 则直接返回True 不检查该委托的最大交易手数
            //if (!sec_list.Contains(o.oSymbol.SecurityFamily.Code))
            //    return true;
            //判断是开仓还是平仓如果是开仓则进行判断拒绝,平仓则直接允许
            if (!o.IsEntryPosition) return true;
            //查询当前品种的所有开仓成交 并计算累加开仓手数
            List<Trade> entryTrades = Account.Trades.Where(tmp => tmp.oSymbol.SecurityFamily.Code == o.oSymbol.SecurityFamily.Code && tmp.IsEntryPosition).ToList();
            int entrySize = entryTrades.Sum(tmp => Math.Abs(tmp.xSize));

            List<TradingLib.API.Order> pendingOrders = Account.Orders.Where(tmp => tmp.oSymbol.SecurityFamily.Code == o.oSymbol.SecurityFamily.Code && tmp.IsEntryPosition).Where(tmp => tmp.IsPending()).ToList();
            int pendingOrderSize = pendingOrders.Sum(tmp => Math.Abs(tmp.Size));

            bool ret = entrySize + pendingOrderSize + o.UnsignedSize <= _tradeSize;//当前持仓数量+欲开仓术量 小于等于 最大持仓数量
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
                return "开仓条件:最大交易手数小于" + _tradeSize.ToString() + " [" + string.Join(",", sec_list.ToArray()) + "]"; ;
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "交易手数检查:最大交易手数"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "交易手数小于设定值,允许开仓"; }
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
