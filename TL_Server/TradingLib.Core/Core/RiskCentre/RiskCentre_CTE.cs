using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;


namespace TradingLib.Core
{
    public partial class RiskCentre
    {

        /// <summary>
        /// 数据检验
        /// </summary>
        [TaskAttr("帐户风控实时检查", 5, "帐户风控实时检查")]
        public void Task_DataCheck()
        {
            //debug("帐户风控检查", QSEnumDebugLevel.INFO);
            foreach (IAccount account in activeaccount.Values)
            {
                this.CheckAccount(account);
            }
        }
        #region 风控设置与操作
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryrulecfg", "qryrulecfg - 查询某个风控规则设置", "查询某个风控规则设置")]
        public string CTE_QryRuleSetting(string ruletype)
        {
            //Type t;
            //if (dicRule.TryGetValue(ruletype, out t))
            //{
            //    JsonWriter w = ReplyHelper.NewJWriterSuccess();
            //    ReplyHelper.FillJWriter(new JsonWrapperRuleInstance(t), w);
            //    ReplyHelper.EndWriter(w);

            //    return w.ToString();
            //}
            //return JsonReply.GenericError(ReplyType.RuleTypeNotFound).ToJson();
            return "";
        }

        /// <summary>
        /// ^分割参数 .分割合约
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "validrulecfg", "validrulecfg - 检查风控规则设置参数", "检查风控规则设置参数")]
        public string CTE_QryValidRuleConfig(string setting)
        {
            string[] args = setting.Split('^');
            if(args.Length != 5)
            {
                return JsonReply.GenericError(ReplyType.RuleConfigError, "风控规则参数个数需为5个").ToJson();
            }
            if(!string.IsNullOrEmpty(args[4]))
            {
                args[4] = string.Join("|",args[4].Split('.'));
            }
            string ruletype = args[0];
            Type t;
            //if (!dicRule.TryGetValue(ruletype, out t))
            //{
            //    return JsonReply.GenericError(ReplyType.RuleConfigError, "风控规则:" + ruletype + "不存在").ToJson();
            //}

            string cfgstr = string.Join(",", args);

            //IRule rc = ((IRule)Activator.CreateInstance(t)).FromText(cfgstr) as IRule;

            //string message="";
            //if (!rc.ValidSetting(out message))
            //{
            //    return JsonReply.GenericError(ReplyType.RuleConfigError, "参数设定错误:" + message).ToJson();
            //}
            return ReplyHelper.Success_Generic;
            
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryorderrule", "qryorderrule - 查询委托规则", "查询委托规则")]
        public string CTE_QryOrderRule()
        {
            //this.UpdateAccountCategory(account, category);
            List<JsonWrapperRule> list = new List<JsonWrapperRule>();
            //foreach (Type t in dicRule.Values)
            //{
            //    if (typeof(IOrderCheck).IsAssignableFrom(t))
            //    {
            //        list.Add(new JsonWrapperRule(t));
            //    }
            //}

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list, w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryorderruleset", "qryorderruleset - 查询某个帐户委托规则", "查询某个帐户委托规则")]
        public string CTE_QryOrderRuleSet(string account)
        { 
            IAccount acc = _clearcentre[account];
            List<JsonWrapperRuleInstance> list = new List<JsonWrapperRuleInstance>();
            if (acc != null)
            { 
                foreach(IOrderCheck rs in acc.OrderChecks)
                {
                    list.Add(new JsonWrapperRuleInstance(rs as IRule));
                }
            }

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list, w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
        
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryaccountrule", "qryaccountrule - 查询帐户规则", "查询帐户规则")]
        public string CTE_QryAccountRule()
        {
            List<JsonWrapperRule> list = new List<JsonWrapperRule>();
            //foreach (Type t in dicRule.Values)
            //{
            //    if (typeof(IAccountCheck).IsAssignableFrom(t))
            //    {
            //        list.Add(new JsonWrapperRule(t));
            //    }
            //}

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list, w);
            ReplyHelper.EndWriter(w);

            return w.ToString();

        }
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryaccountruleset", "qryaccountruleset - 查询某个帐户帐户检查规则", "查询某个帐户帐户检查规则")]
        public string CTE_QryAccountRuleSet(string account)
        {
            IAccount acc = _clearcentre[account];
            List<JsonWrapperRuleInstance> list = new List<JsonWrapperRuleInstance>();
            if (acc != null)
            {
                foreach (IAccountCheck rs in acc.AccountChecks)
                {
                    list.Add(new JsonWrapperRuleInstance(rs as IRule));
                }
            }

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list, w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "clearaccrule", "clearaccrule - 清空某个帐户的帐户检查规则", "清空某个帐户的帐户检查规则")]
        public void CTE_ClearAccountRule(string account)
        {
            IAccount acc = _clearcentre[account];
            if (acc != null)
            {
                acc.ClearAccountCheck();//清空帐户帐户规则列表
                //OrderCheckTracker.delRuleFromAccount(account);//xml文件操作清空该帐户帐户规则设置
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "clearorderrule", "clearorderrule - 清空某个帐户的委托检查规则", "清空某个帐户的委托检查规则")]
        public void CTE_ClearOrderRule(string account)
        {
            IAccount acc = _clearcentre[account];
            if (acc != null)
            {
                acc.ClearOrderCheck();
                //AccountCheckTracker.delRuleFromAccount(account);
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "addaccountrule", "addaccountrule - 为某个帐户添加帐户检查规则", "为某个帐户添加帐户检查规则")]
        public void CTE_AddAccountRule(string account, string setting)
        {
            IAccount acc = _clearcentre[account];
            string[] cfgs = setting.Split('^');//注意参数用^分割,逗号用于分隔整体参数,同时.号可能被设定为数值部分
            string rsname = cfgs[0];
            string cfgstr = string.Join(",", cfgs);

            if (acc != null)
            {
                //IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgstr) as IAccountCheck;
                //将规则记录到文本
                //AccountCheckTracker.addRuleIntoAccount(account, rc);
                //rc.Account = new AccountAdapterToExp(acc);
                //将规则添加到对应账户的规则集
                //acc.AddAccountCheck(rc);
            }
        }
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "delaccountrule", "delaccountrule - 为某个帐户删除帐户检查规则", "为某个帐户删除帐户检查规则")]
        public void CTE_DelAccountRule(string account, string setting)
        {
            IAccount acc = _clearcentre[account];
            string[] cfgs = setting.Split('^');//注意参数用^分割,逗号用于分隔整体参数,同时.号可能被设定为数值部分
            string rsname = cfgs[0];
            string cfgstr = string.Join(",", cfgs);

            if (acc != null)
            {
                //IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgstr) as IAccountCheck;
                //将规则记录到文本
                //AccountCheckTracker.delRuleFromAccount(account, rc);
                //将规则添加到对应账户的规则集
                //acc.DelAccountCheck(rc);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "addorderrule", "addorderrule - 为某个帐户添加委托检查规则", "为某个帐户添加委托检查规则")]
        public void CTE_AddOrderRule(string account, string setting)
        {
            IAccount acc = _clearcentre[account];
            string[] cfgs = setting.Split('^');//注意参数用^分割,逗号用于分隔整体参数,同时.号可能被设定为数值部分
            string rsname = cfgs[0];
            string cfgstr = string.Join(",", cfgs);

            if (acc != null)
            {
                //IOrderCheck rc = ((IOrderCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgstr) as IOrderCheck;
                //将规则记录到文本
                //OrderCheckTracker.addRuleIntoAccount(account, rc);
                //rc.Account = new AccountAdapterToExp(acc);
                //将规则添加到对应账户的规则集
                //acc.AddOrderCheck(rc);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "delorderrule", "delorderrule - 为某个帐户删除委托检查规则", "为某个帐户删除委托检查规则")]
        public void CTE_DelOrderRule(string account, string setting)
        {
            IAccount acc = _clearcentre[account];
            string[] cfgs = setting.Split('^');//注意参数用^分割,逗号用于分隔整体参数,同时.号可能被设定为数值部分
            string rsname = cfgs[0];
            string cfgstr = string.Join(",", cfgs);

            if (acc != null)
            {
                //IOrderCheck rc = ((IOrderCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgstr) as IOrderCheck;
                //将规则记录到文本
                //OrderCheckTracker.delRuleFromAccount(account, rc);
                //将规则添加到对应账户的规则集
                //acc.DelOrderCheck(rc);
            }
        }
        #endregion

        #region 帐户操作

        [CoreCommandAttr(QSEnumCommandSource.CLI, "flatpos", "flatpos - flat postion of account", "风控中心平掉某个帐户的某个合约的所有持仓")]
        public void CTE_FlatPosition(string account, string symbol)
        {
            Position pos = _clearcentre[account].GetPosition(symbol, true);
            if (pos != null && !pos.isFlat)
                FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "风控强平");
            Position pos2 =_clearcentre[account].GetPosition(symbol, false);
            if (pos2 != null && !pos2.isFlat)
                FlatPosition(pos2, QSEnumOrderSource.RISKCENTRE, "风控强平");
        }
        [CoreCommandAttr(QSEnumCommandSource.CLI, "fp", "fp - flat postion", "风控中心平仓测试")]
        public void CTE_FlatPosition()
        {
            CTE_FlatPosition("5880003", "IF1406");
        }

         [CoreCommandAttr(QSEnumCommandSource.CLI, "fpl", "fpl - flat postion", "风控中心平仓测试输出列表")]
        public string CTE_PostionFlatSetList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RiskTaskSet ps in posflatlist)
            {
                sb.Append(ps.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }
        #endregion

        #region 命令行操作
         [CoreCommandAttr(QSEnumCommandSource.CLI, "sectime", "sectime - sectime", "查看某个品种的市场时间")]
         public string CTE_SymbolMarketTime(string code)
         {
             SecurityFamily sec = BasicTracker.SecurityTracker[code];
             if (sec == null)
             {
                 return "品种不存在";
             }
             return sec.IsMarketTime.ToString();
         }
         [CoreCommandAttr(QSEnumCommandSource.CLI, "symtime", "symtime - symtime", "查看某个合约的市场时间")]
         public string CTE_SecurityMarketTime(string symbol)
         {
             //Symbol sym = BasicTracker.SymbolTracker[1,symbol];
             //if (sym == null)
             //{
             //    return "品种不存在";
             //}
             //return sym.IsMarketTime.ToString();
             return "";
         }

         [CoreCommandAttr(QSEnumCommandSource.CLI, "demoflat", "demoflat - demoflat", "强平某个帐户的持仓，先撤单，然后再强平")]
         public string CTE_DemoFalt1(string account)
         {
             IAccount acc = _clearcentre[account];
             if (acc != null)
             {
                 acc.FlatPosition(QSEnumOrderSource.RISKCENTRE, "DemoFlat");
                 return "操作成功";

             }
             else
             {
                 return "帐户不存在";
             }
         }

         [CoreCommandAttr(QSEnumCommandSource.CLI, "rtask", "rtask - rtask", "列出风控中心所有任务")]
         public string CTE_RTask()
         {
             foreach (RiskTaskSet task in posflatlist)
             {
                 debug("Account:" + task.Account + " Type:" + task.TaskType.ToString() + " SubTaskCnt:" + task.SubTask.Count.ToString(), QSEnumDebugLevel.INFO);
             }
             return "打印成功";
         }
        
         #endregion













         
         [TaskAttr("2秒检查待平持仓",1, "调度系统每1秒检查一次待平仓列表")]
         public void Task_ProcessPositionFlatSet()
         {
             this.ProcessPositionFlat();
         }

        
    }
}
