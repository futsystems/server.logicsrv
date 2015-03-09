using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 清算中心功能接口
    /// 
    /// </summary>
    public interface IClearCentre
    {

        /// <summary>
        /// 载入交易帐户
        /// 为该用户生成基本交易数据结构，并维护该帐户的实时交易信息，以形成交易状态
        /// </summary>
        /// <param name="account"></param>
        void CacheAccount(IAccount account);


        /// <summary>
        /// 丢弃交易帐户
        /// 从内存中将某交易帐户的交易数据结构销毁
        /// </summary>
        /// <param name="account"></param>
        void DropAccount(IAccount account);
    }
}
