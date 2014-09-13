//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
////
////using TradingLib.Data;

//namespace TradingLib.API
//{

//    public interface IMgrClient : IManager, IAccountOperation, IAccountOperationCritical
//    {
//        /// <summary>
//        /// 创建新的交易账户 指定帐户类别 已经代理编号
//        /// </summary>
//        void AddAccount(QSEnumAccountCategory ca,string agentcode);
//        /// <summary>
//        /// 请求比赛统计信息
//        /// </summary>
//        void RequestRaceStatistic();

//        /// <summary>
//        /// 订阅所有市场数据
//        /// </summary>
//        void SubscribeAll();

//        /*
//        /// <summary>
//        /// 上传账户合约信息
//        /// </summary>
//        /// <param name="account"></param>
//        void UploadAccountSecurityTable(string account);

//        /// <summary>
//        /// 下载账户合约信息
//        /// </summary>
//        /// <param name="account"></param>
//        void DownloadAccountSecurityTable(string account);

//        /// <summary>
//        /// 下载默认合约列表
//        /// </summary>
//        void DownloadDefaultSecurity();

//        /// <summary>
//        /// 上传合约列表
//        /// </summary>
//        void UploadDefaultSecurity();
//        /// <summary>
//        /// 查询某个账户的profile信息
//        /// </summary>
//        /// <param name="account"></param>
//        void QryAccountProfile(string account);
//        /// <summary>
//        /// 查询某个账户的比赛信息
//        /// </summary>
//        /// <param name="account"></param>
//        void QryAccountRaceInfo(string account);**/

//        void QryFinServiceInfo(string account);

//    }
//}
