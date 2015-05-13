using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace OrderRuleSet
{
    /// <summary>
    /// 交易品种，设置在某个时间段内禁止交易某些品种
    /// </summary>
    public class RSSecurityBlock : RuleBase, IOrderCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        private int start_time = 0;


        /// <summary>
        /// 结束时间
        /// </summary>
        private int end_time = 0;


        /// <summary>
        /// 禁止开仓
        /// </summary>
        private bool block_open = false;

        /// <summary>
        /// 禁止平仓
        /// </summary>
        private bool block_close = false;


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
                    start_time = int.Parse(args["start_time"].ToString());
                    end_time = int.Parse(args["end_time"].ToString());

                    block_open = bool.Parse(args["block_open"].ToString());
                    block_close = bool.Parse(args["block_close"].ToString());

                    foreach (string code in args["sec_list"].ToString().Split(','))
                    {
                        sec_list.Add(code);
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
        public bool checkOrder(Order o, out string msg)
        {

            msg = string.Empty;

            bool needcheck = false;
            //起止时间均为0,标识全天检查
            if (start_time == 0 && end_time == 0)
            {
                needcheck = true;
            }
            else
            {
                int now = Util.ToTLTime();
                
                //如果不全为0 则需要在开始和结束之间才进行检查
                if (now >= start_time && now <= end_time)
                {
                    needcheck = true;
                }
            }

            if (needcheck)
            {
                if ((block_open && o.IsEntryPosition) || (block_close && !o.IsEntryPosition))//禁止开仓并且该委托是开仓委托 或者 禁止平仓且委托为平仓委托
                {
                    needcheck = true;
                }
                else
                {
                    needcheck = false;
                }
            }

            if (needcheck)
            {
                if (sec_list.Contains(o.oSymbol.SecurityFamily.Code))
                {

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 该规则内容
        /// </summary>
        public override string RuleDescription
        {
            get
            {
                return "禁止交易选中的品种:"+string.Join(",",sec_list.ToArray());
            }
        }


        #region 覆写静态对象
        /// <summary>
        /// 规则名称
        /// </summary>
        public static new string Title
        {
            get { return "品种黑名单,禁止交易选中的品种"; }
        }

        /// <summary>
        /// 规则描述
        /// </summary>
        public static new string Description
        {
            get { return "禁止交易选中的品种"; }
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
