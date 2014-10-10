using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 比赛中心接口,不同制度的比赛实现该接口
    /// </summary>
    public interface IRaceCentre : IDebug, IMail
    {
        /// <summary>
        /// 某个账户进入某个级别的比赛,需要调用风控中心调整相关比赛等级的初始化风控规则
        /// </summary>
        event AccountEntryRaceDel AccountEntryRaceEvent;

        /// <summary>
        /// 某个交易帐号比赛状态发生变化,向客户端与管理端发送状态变化
        /// </summary>
        event IAccountChangedDel AccountRaceStatusChanged;

        /// <summary>
        /// 实时检查执行某个账户检测，查看是否符合风控规则,调用风控中心的算法来检查某个账户是否符合风控规则
        /// </summary>
        event IAccountCheckDel AccountCheckEvent;

        
        /// <summary>
        /// 某个账户报名比赛
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Signup(IAccount acc, out string msg);

        /// <summary>
        /// 查询某个账户的比赛状态信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        IRaceInfo GetAccountRaceInfo(string account);

        /// <summary>
        /// 手工淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        void EliminateAccount(IAccount acc);

        /// <summary>
        /// 手工晋级某个账户
        /// </summary>
        /// <param name="acc"></param>
        void PromptAccount(IAccount acc);

        /// <summary>
        /// 新开比赛
        /// </summary>
        void OpenNewPrerace();


        /// <summary>
        /// 比赛状态信息列表,罗列出比赛参数
        /// </summary>
        //List<IRaceStatistic> RaceStatList{get;}

        /// <summary>
        /// 考核比赛
        /// </summary>
        void Task_CheckRace();

        /// <summary>
        /// 清空比赛统计信息
        /// </summary>
        void Task_ClearRaceStatistic();

        /// <summary>
        /// 生成比赛统计数据
        /// </summary>
        void Task_GenRaceStatistic();

        /// <summary>
        /// 比赛服务启动
        /// </summary>
        //void Start();

        /// <summary>
        /// 比赛服务停止
        /// </summary>
        //void Stop();

    }
}
