using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace RuleSet2.Order
{
    /// <summary>
    /// 交易品种黑名单/白名单 允许交易某些品种或禁止交易某些品种
    /// </summary>
    public class RSSecurityFilter : RuleBase, IOrderCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 是否是白名单
        /// </summary>
        private bool is_white = false;

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
                    var args = TradingLib.Mixins.Json.JsonMapper.ToObject(_args);

                    is_white = bool.Parse(args["is_white"].ToString());

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


        /// <summary>
        /// 委托检查逻辑过程,如果接受委托返回true,拒绝委托返回false
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool checkOrder(TradingLib.API.Order o, out string msg)
        {

            
            msg = string.Empty;
            
            //白名单策略 品种需在设定品种列表之内
            if (is_white)
            {
                if (sec_list.Contains(o.oSymbol.SecurityFamily.Code))
                {
                    return true;
                }
                msg = string.Format("品种:{0}不允许交易", o.oSymbol.SecurityFamily.Code);
                return false;
            }
            else//黑名单
            {
                if (sec_list.Contains(o.oSymbol.SecurityFamily.Code))
                {
                    msg = string.Format("品种:{0}不允许交易", o.oSymbol.SecurityFamily.Code);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return (is_white?"允许":"禁止")+"交易品种:"+string.Join(",",sec_list.ToArray());
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "品种黑/白名单,禁止或允许交易选定的品种"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "禁止/允许交易选中的品种"; }
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
