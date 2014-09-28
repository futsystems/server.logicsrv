using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户风控检查接口
    /// 用于加载帐户的委托风控规则和帐户风控规则
    /// </summary>
    public interface IAccRiskCheck
    {
        //RiskCentre用于风险检查,风险检查还是按照账户为分类 不同的账户设定有不同的风空检查规则，因此最终还是以Acount为单位进行风控检查
        //这两将规则统一放入到Account内部,由riskcentre统一调用进行检查
        #region 风控检查部分
        /// <summary>
        /// 帐户规则是否加载过
        /// </summary>
        bool RuleItemLoaded { get; set; }//账户规则加载
        /// <summary>
        /// 委托规则是否加载过
        /// </summary>
        //bool OrdRuleLoaded { get; set; }//委托规则加载

        /// <summary>
        /// 检查委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckOrder(Order o, out string msg);
        /// <summary>
        /// 检查账户状态
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckAccount(out string msg);
        /// <summary>
        /// 清除委托检查
        /// </summary>
        void ClearOrderCheck();
        /// <summary>
        /// 添加委托检查
        /// </summary>
        /// <param name="rc"></param>
        void AddOrderCheck(IOrderCheck rc);
        /// <summary>
        /// 删除委托检查
        /// </summary>
        /// <param name="rc"></param>
        //void DelOrderCheck(IOrderCheck rc);
        void DelOrderCheck(int id);

        /// <summary>
        /// 清除账户检查
        /// </summary>
        void ClearAccountCheck();
        /// <summary>
        /// 添加账户检查
        /// </summary>
        /// <param name="rc"></param>
        void AddAccountCheck(IAccountCheck rc);
        /// <summary>
        /// 删除账户检查
        /// </summary>
        /// <param name="rc"></param>
        //void DelAccountCheck(IAccountCheck rc);
        void DelAccountCheck(int id);

        /// <summary>
        /// 所有委托检查规则
        /// </summary>
        IOrderCheck[] OrderChecks { get; }

        /// <summary>
        /// 所有帐户检查规则
        /// </summary>
        IAccountCheck[] AccountChecks { get; }
        #endregion 
    }
}
