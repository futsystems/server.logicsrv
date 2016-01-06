using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    /// <summary>
    /// 设定合约价格变动超过一定幅度时 执行强平
    /// </summary>
    public class RSMaxChange : RuleBase, IAccountCheck
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
                    var args = TradingLib.Mixins.Json.JsonMapper.ToObject(_args);
                    up_pect = decimal.Parse(args["up_pect"].ToString());//上涨触发值
                    dn_pect = decimal.Parse(args["dn_pect"].ToString());//跌幅触发值

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



        bool flatStart = false;//强平触发
        bool iswarnning = false;//是否处于报警状态

        List<string> posflatfired = new List<string>();
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal equity = this.Account.NowEquity;//获得该账户的当前权益

            //遍历所有未平持仓
            foreach (var pos in this.Account.Positions.Where(pos => !pos.isFlat))
            {
                string key = pos.GetPositionKey();
                if (posflatfired.Contains(key)) continue;//如果触发列表包含该持仓key则直接返回

                //如果品种列表为空 或者 持仓品种在对应的列表内 则执行检查
                if (sec_list.Count == 0 || sec_list.Contains(pos.oSymbol.SecurityFamily.Code))
                {
                    Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(pos.Symbol);
                    if (k == null) continue;
                    decimal val = k.Settlement > 0 ? k.Settlement : k.PreClose;//从结算价和preclose中获得有效昨日价格
                    decimal pect = (k.Trade - val) / val;
                    
                    //上涨
                    if (pect > 0)
                    {
                        //上涨幅度超过设定值 强平持仓
                        if (pect * 100 > up_pect)
                        { 
                            this.Account.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "合约涨跌幅触发");
                            posflatfired.Add(key);
                            return false;
                        }
                    }
                    else//下跌
                    {
                        //下跌幅度超过设定值 强平持仓
                        if (Math.Abs(pect) * 100 > dn_pect)
                        {
                            this.Account.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "合约涨跌幅触发");
                            posflatfired.Add(key);
                            return false;
                        }
                    
                    }
                }
                
            }
            return true;
        }

        public override string RuleDescription
        {
            get
            {
                return "涨幅大于" + up_pect.ToString("N2") + "或跌幅大于" + dn_pect.ToString("N2") + "强平持仓";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "合约价格涨跌幅超过设定值时,强平对应持仓"; }
        }

        public static new string Description
        {
            get { return "监控合约价格涨跌幅,涨跌幅度超过设定值时,触发对应持仓的强平操作"; }
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
