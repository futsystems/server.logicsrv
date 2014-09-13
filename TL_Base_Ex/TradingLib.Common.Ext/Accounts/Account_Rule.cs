using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        bool _rulitemloaded = false;
        public bool RuleItemLoaded { get { return _rulitemloaded; } set { _rulitemloaded = value; } }//账户规则加载
        


        #region 委托检查

        //返回账户IRuleCheck规则集合
        private ConcurrentDictionary<int, IOrderCheck> _ordchekMap = new ConcurrentDictionary<int, IOrderCheck>();
        //检查某个Order是否允许
        public bool CheckOrder(Order o, out string msg)
        {
            //string msg;
            LibUtil.Debug("account check order now ....");
            msg = "";
            foreach (IOrderCheck rc in _ordchekMap.Values)
            {
                if (!rc.checkOrder(o, out msg))
                    return false;
            }
            return true;
        }
        //清空委托检查规则集合
        public void ClearOrderCheck()
        {
            _ordchekMap.Clear();
        }
        //增加一条规则检查
        public void AddOrderCheck(IOrderCheck rc)
        {
            //rulecheck用totext来统一标识
            if(!_ordchekMap.Keys.Contains(rc.ID))
                _ordchekMap.TryAdd(rc.ID, rc);
        }

        /// <summary>
        /// 通过全局ID删除委托检查
        /// </summary>
        /// <param name="id"></param>
        public void DelOrderCheck(int id)
        {
            IOrderCheck oc;
            if (_ordchekMap.Keys.Contains(id))
            {
                _ordchekMap.TryRemove(id, out oc);
            }
        }
        public IOrderCheck[] OrderChecks { get { return _ordchekMap.Values.ToArray(); } }

        #endregion

        #region 账户检查
        //返回账户IRuleCheck规则集合
        public IAccountCheck[] AccountChecks { get { return _accchekMap.Values.ToArray(); } }
        private ConcurrentDictionary<int, IAccountCheck> _accchekMap = new ConcurrentDictionary<int, IAccountCheck>();
        //检查某个Order是否允许
        public bool CheckAccount(out string msg)
        {
            //string msg;
            msg = "";
            foreach (IAccountCheck rc in _accchekMap.Values)
            {
                if (!rc.CheckAccount(out msg))
                {

                    return false;
                }
            }
            return true;
        }
        //清空委托检查规则集合
        public void ClearAccountCheck()
        {
            _accchekMap.Clear();
        }
        //增加一条规则检查
        public void AddAccountCheck(IAccountCheck rc)
        {
            //rulecheck用totext来统一标识
            if (!_accchekMap.Keys.Contains(rc.ID))
                _accchekMap.TryAdd(rc.ID, rc);
        }
        //删除风控规则
        public void DelAccountCheck(int id)
        {
            IAccountCheck ic;
            _accchekMap.TryRemove(id, out ic);
        }
        #endregion

    }
}
