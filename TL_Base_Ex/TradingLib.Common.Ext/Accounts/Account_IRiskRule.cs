﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountImpl
    {
        bool _ruleitemloaded = false;
        public bool RuleItemLoaded { get { return _ruleitemloaded; } set { _ruleitemloaded = value; } }//账户规则加载


        /// <summary>
        /// 委托风控规则map 风控规则用数据库ID作为一索引
        /// </summary>
        private ConcurrentDictionary<int, IOrderCheck> _ordchekMap = new ConcurrentDictionary<int, IOrderCheck>();
        
        /// <summary>
        /// 委托风控检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckOrderRule(Order o, out string msg)
        {
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
            if (!_ordchekMap.TryAdd(rc.ID, rc))
            {
                logger.Warn(string.Format("OrderCheck ID:{0} already added", rc.ID));
            }
        }

        /// <summary>
        /// 通过全局ID删除委托检查
        /// </summary>
        /// <param name="id"></param>
        public void DelOrderCheck(int id)
        {
            IOrderCheck oc;
            if (!_ordchekMap.TryRemove(id,out oc))
            {
                logger.Warn(string.Format("OrderCheck ID:{0} not loaded",id));
            }
        }

        public IEnumerable<IOrderCheck> OrderChecks { get { return _ordchekMap.Values; } }


        private ConcurrentDictionary<int, IAccountCheck> _accchekMap = new ConcurrentDictionary<int, IAccountCheck>();

        /// <summary>
        /// 帐户规则检查
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckAccountRule(out string msg)
        {
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

        /// <summary>
        /// 清空帐户规则
        /// </summary>
        public void ClearAccountCheck()
        {
            _accchekMap.Clear();
        }

        /// <summary>
        /// 添加帐户风控规则
        /// </summary>
        /// <param name="rc"></param>
        public void AddAccountCheck(IAccountCheck rc)
        {
            if (!_accchekMap.TryAdd(rc.ID, rc))
            {
                logger.Warn(string.Format("AccountCheck ID:{0} already added", rc.ID));
            }
        }

        /// <summary>
        /// 删除某条帐户风控规则
        /// </summary>
        /// <param name="id"></param>
        public void DelAccountCheck(int id)
        {
            IAccountCheck ic;
            if (!_accchekMap.TryRemove(id, out ic))
            {
                logger.Warn(string.Format("AccountCheck ID:{0} not loaded",id));
            }
        }

        public IEnumerable<IAccountCheck> AccountChecks { get { return _accchekMap.Values; } }


    }
}
