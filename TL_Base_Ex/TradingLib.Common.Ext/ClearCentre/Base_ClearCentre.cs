using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Common
{
    /// <summary>
    /// 清算中心，为服务器维护了一批交易账户,以及每个交易账户的实时Order,trades,position的相关信息。
    /// </summary>
    public abstract partial class ClearCentreBase : BaseSrvObject
    {
        /// <summary>
        /// 分帐户交易数据维护器
        /// </summary>
        protected AccountTracker acctk = new AccountTracker();

        /// <summary>
        /// 总交易数据维护器
        /// </summary>
        protected TotalTracker totaltk = new TotalTracker();



        public ClearCentreBase(string name = "ClearCentreBase")
            : base(name)
        {
            acctk.NewPositionEvent += new Action<Position>(acctk_NewPositionEvent);
        }

        //当帐户交易对象维护器产生持仓时，我们将持仓加入total维护其列表用于快速访问
        void acctk_NewPositionEvent(Position obj)
        {
            Util.Debug("new postion created " + obj.GetPositionKey(), QSEnumDebugLevel.MUST);
            totaltk.NewPosition(obj);
        }

        #region 【IClearCentreBase】交易账户 操作
        /// <summary>
        /// 获得Account数组
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<IAccount> Accounts
        //{
        //    get { return acctk.Accounts; }
        //}
        /// <summary>
        /// 通过AccountID获得某个账户
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        //public IAccount this[string accid]
        //{
        //    get
        //    {
        //        return acctk[accid];
        //    }
        //}

        /// <summary>
        /// 查找某个userid的交易帐户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public IAccount QryAccount(int uid,QSEnumAccountCategory category)
        {
            return acctk.QryAccount(uid, category);
        }


        ///// <summary>
        ///// 查询是否有某个ID的账户
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public bool HaveAccount(string a)
        //{
        //    return acctk.HaveAccount(a);
        //}
        ///// <summary>
        ///// 查询是否有某个ID的账户并返回该账户
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="acc"></param>
        ///// <returns></returns>
        //public bool HaveAccount(string a, out IAccount acc)
        //{
        //    acc = null;
        //    return acctk.HaveAccount(a, out acc);
        //}

        /// <summary>
        /// 将某个账户缓存到服务器内存，注意检查是否已经存在该账户
        /// 生成该账户所对应的数据对象用于实时储存交易信息与合约信息
        /// </summary>
        /// <param name="a"></param>
        public void CacheAccount(IAccount a)
        {
            acctk.CacheAccount(a);
        }

        /// <summary>
        /// 将某个帐户从内存中删除
        /// </summary>
        /// <param name="a"></param>
        public void DropAccount(IAccount a)
        {
            acctk.DropAccount(a);
        }
        #endregion



    }
}
