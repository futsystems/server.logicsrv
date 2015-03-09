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

        #region 添加或删除交易帐户到清算服务的内存数据哭
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
