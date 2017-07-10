using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum QSEnumManagerType
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("管理员")]
        ROOT = 0,//系统总权限 可以进行所有操作

        /// <summary>
        /// 代理商
        /// </summary>
        [Description("下级会员")]
        AGENT = 1,//代理人员,代理人员对其开设的帐户有绝对权限,

        /// <summary>
        /// 员工账户
        /// 员工账户为管理员/代理开设 用于辅助管理域操作 具体权限通过权限模板进行限定
        /// </summary>
        [Description("员工")]
        STAFF = 2,


        /// <summary>
        /// 风控管理员角色
        /// </summary>
        //[Description("风控员")]
        //RISKER = 3,//风控人员 可以进行强平,设置帐户相关全新或风控规则

        /// <summary>
        /// 观察员
        /// </summary>
        //[Description("观察员")]
        //MONITER = 4,//观察员,无法进行帐户设置,只能查看帐户当前状态
    }
}
