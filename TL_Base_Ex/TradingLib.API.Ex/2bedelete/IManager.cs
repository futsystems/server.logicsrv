//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    public interface IManager : IRiskOperation
//    {
//        /// <summary>
//        /// 请求比赛统计
//        /// </summary>
//        void RequestRaceStatistic();
//        /// <summary>
//        /// 上传某个账户的合约信息
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
//        void QryAccountRaceInfo(string account);

//        /// <summary>
//        /// 回补某个交易账户的日内交易信息
//        /// </summary>
//        /// <param name="account"></param>
//        void ResuemAccount(string account);
//        /// <summary>
//        /// 设定账户观察列表
//        /// </summary>
//        /// <param name="acclist"></param>
//        void SetWatchList(string acclist);

//        /// <summary>
//        /// 查询账户财务信息
//        /// </summary>
//        /// <param name="account"></param>
//        void QryAccountInfo(string account);

//    }
//}
