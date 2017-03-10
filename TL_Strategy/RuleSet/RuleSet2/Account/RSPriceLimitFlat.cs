using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    /// <summary>
    /// 距离涨跌停多少时 强平持仓
    /// </summary>
    public class RSPriceLimitFlat : RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 涨幅触发值
        /// </summary>
        decimal up_pect = 0;

        /// <summary>
        /// 跌幅触发值
        /// </summary>
        decimal dn_pect = 0;

        /// <summary>
        /// 品种列表
        /// </summary>
        private List<string> sec_list = new List<string>();

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
                    _ratio = decimal.Parse(args["ratio"].ToString());

                }
                catch (Exception ex)
                { }
            }
        }

        decimal _ratio = 0;


        //bool flatStart = false;//强平触发
        //bool iswarnning = false;//是否处于报警状态

        List<string> posflatfired = new List<string>();
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            //遍历所有未平持仓
            foreach (var pos in this.Account.Positions.Where(pos => !pos.isFlat))
            {
                string key = pos.GetPositionKey();
                if (posflatfired.Contains(key)) continue;//如果触发列表包含该持仓key则直接返回

                //如果品种列表为空 或者 持仓品种在对应的列表内 则执行检查
                if (sec_list.Count == 0 || sec_list.Contains(pos.oSymbol.SecurityFamily.Code))
                {
                    Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(pos.oSymbol.Exchange,pos.oSymbol.Symbol);
                    if (k == null) continue;

                    //判定是否可以有效计算涨跌幅
                    if (k.PreSettlement <= 0) continue;
                    if (k.UpperLimit <= 0) continue;

                    decimal val = k.Settlement > 0 ? k.Settlement : k.PreClose;//从结算价和preclose中获得有效昨日价格
                    decimal pect = (k.Trade - val) / val;

                    //距离涨跌停小于设定值 则强平该持仓
                    decimal d = k.UpperLimit - k.PreSettlement;
                    decimal d2 = (d - Math.Abs(k.Trade - k.PreSettlement)) / k.PreSettlement * 100m;
                    bool flag2 = d2 <= this._ratio;

                    if (flag2)
                    {
                        TLCtxHelper.ModuleRiskCentre.FlatPosition(pos, pos.UnsignedSize,  QSEnumOrderSource.RISKCENTRE, "涨跌停强平");
                        posflatfired.Add(key);
                        return false;
                    }
                }
            }
            return true;
        }

        public override string RuleDescription
        {
            get
            {
                return "距离涨跌停小于" + _ratio.ToFormatStr() + " 强平持仓" + " [" + string.Join(",", sec_list.ToArray()) + "]";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "距离涨跌停小于X% 强平持仓"; }
        }

        public static new string Description
        {
            get { return "距离涨跌停小于X% 强平持仓"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "当前权益"; } }

        /// <summary>
        /// 不用设置比较关系
        /// </summary>
        public static new bool CanSetCompare { get { return false; } }

        /// <summary>
        /// 默认比较关系大于等于
        /// </summary>
        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.Less; } }

        /// <summary>
        /// 不用设置品种集合
        /// </summary>
        public static new bool CanSetSymbols { get { return false; } }

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
