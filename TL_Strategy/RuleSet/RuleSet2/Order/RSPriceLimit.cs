using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace RuleSet2.Order
{
    /// <summary>
    /// 涨跌停风控
    /// </summary>
    public class RSPriceLimit : RuleBase, IOrderCheck
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
                    //解析设定值
                    _ratio = decimal.Parse(args["ratio"].ToString());
                   
                }
                catch (Exception ex)
                { }
            } 
        }

        decimal _ratio = 0;


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

            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Exchange, o.Symbol);
            //价格超过涨跌幅度 则拒绝
            if (k.PreSettlement <= 0) return true;
            if (k.UpperLimit <= 0) return true;

            bool flag = k.Trade - k.PreSettlement > 0;//价格上涨
            if (flag && o.PositionSide) return true;//价格上涨且为买入 
            if ((!flag) && (!o.PositionSide)) return true;//价格下跌 且为卖出

            //开仓条件 涨跌幅小鱼设定的百分比
            decimal d = k.UpperLimit - k.PreSettlement;
            decimal d2 = (d - Math.Abs(k.Trade - k.PreSettlement)) / k.PreSettlement * 100m;
            bool flag2 = d2 <= this._ratio;

            if (flag2)
            {
                msg = RuleDescription + " 委托被拒绝";
                o.Comment = msg;
            }
            return !flag2;
        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return "开仓条件:距离涨跌停小于" + _ratio.ToFormatStr() + "% [" + string.Join(",", sec_list.ToArray()) + "]";
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "涨跌停禁止开仓"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "距离涨跌停附近禁止开仓"; }
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
