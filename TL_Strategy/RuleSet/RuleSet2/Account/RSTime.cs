using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    public class RSTime : RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 强平时间
        /// </summary>
        int flat_time = 0;

        List<string> sec_list = new List<string>();

        /// <summary>
        /// 损失报警线
        /// </summary>
        decimal loss_warn = 0;


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
                    flat_time = int.Parse(args["flat_time"].ToString());//强平线
                    string secs= args["sec_list"].ToString();
                    if (!string.IsNullOrEmpty(secs))
                    {
                        foreach (var sec in secs.Split(','))
                        {
                            sec_list.Add(sec);
                        }
                    }
                }
                catch (Exception ex)
                { 
                    
                }
            }
        }



        bool flatStart = false;//强平触发
        bool iswarnning = false;//是否处于报警状态

        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            int diff = Util.ToTLTime() - flat_time;//计算当前时间与设定时间的diff
            //Util.Debug("!!RSTime: diff:" + diff.ToString());
            bool ret = (Math.Abs(diff) < 5);//如果距离设定时间在5秒之内

            //不在5秒之内 或已经执行强平
            if (!ret || flatStart) return true;

            if(ret && !flatStart)
            {
                //if (this.Account.Execute)
                //    this.Account.InactiveAccount();//冻结账户
                //Util.Debug("~~~~~~~~~~~~~~~执行强平");
                if (this.Account.AnyPosition)
                {
                    //没有限定品种 全平所有
                    if (sec_list.Count == 0)
                    {
                        TLCtxHelper.ModuleAccountManager.InactiveAccount(this.Account.ID);
                        msg = RuleDescription + ":全平所有仓位并冻结账户";
                        TLCtxHelper.ModuleRiskCentre.FlatAllPositions(this.Account.ID, QSEnumOrderSource.RISKCENTREACCOUNTRULE,msg);
                    }
                    else
                    {
                        //遍历所有持仓 将品种列表内的持仓强平
                        foreach (var pos in this.Account.Positions.Where(pos => !pos.isFlat))
                        {
                            if (sec_list.Contains(pos.oSymbol.SecurityFamily.Code))
                            {
                                TLCtxHelper.ModuleRiskCentre.FlatPosition(pos, pos.UnsignedSize, QSEnumOrderSource.RISKCENTRE, string.Format("定时:{0}强平", flat_time));
                            }
                        }
                    }
                }
                flatStart = true;//开始平仓
                return false;
            }

            return true;
        }

        DateTime GetDateTime()
        {
            return Util.ToDateTime(Util.ToTLDate(), this.flat_time);
        }
        public override string RuleDescription
        {
            get
            {
                return string.Format("定时【{0}】强平[{1}]", GetDateTime().ToString("HH:mm:ss"), string.Join(",", sec_list.ToArray()));
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "在指定时间强平持仓(指定品种或全部)"; }
        }
        public static new string Description
        {
            get { return "在指定时间强平所有持仓或指定品种的持仓"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "强平时间"; } }

        /// <summary>
        /// 不用设置比较关系
        /// </summary>
        public static new bool CanSetCompare { get { return false; } }

        /// <summary>
        /// 默认比较关系大于等于
        /// </summary>
        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.Greater; } }

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
