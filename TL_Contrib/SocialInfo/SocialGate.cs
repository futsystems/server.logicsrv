using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;

namespace SocialLink
{
    /// <summary>
    /// SocialGate用于与社交系统建立通讯通道
    /// 1.获得社交系统的相关参数比如用户的基本信息,粉丝,帖子等
    /// 2.调用社交系统相关调用,包含推送信息,发送分享等交易系统需要使用的操作
    /// 第一阶段通过 数据库直接进行访问
    /// 第二阶段通过 Redis来获得社交系统数据,统一调用API执行相关调用
    /// </summary>
    [ContribAttr(
                "SocialInfo",
                "社交账户信息",
                "社交账户信息用于获取社交系统相关数据,未来设计成与社交系统通讯的组件,比如获得某个帐户粉丝数目,当日赞数等,同时可以通过社交API接口向社交系统提交相关数据")]
    public partial class SocialGate:BaseSrvObject,IContrib
    {

        ConnectionPoll<SocialDB> conn;


        public SocialGate()
            : base("SocialInfo")
        {
            conn = new ConnectionPoll<SocialDB>("192.168.2.213", "admin", "hky123456", "thinksns_3_1", 3306);
        }


        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {


        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }


        /// <summary>
        /// 获得某个帐户的粉丝数量
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        int GetFollowersByEmail(string email)
        {
            SocialDB db = conn.mysqlDB;
            int num_followes = db.GetFollowers(email);
            conn.Return(db);

            return num_followes;
        }
    }
}
