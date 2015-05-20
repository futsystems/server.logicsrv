using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 管理端界面权限
    /// 用于控制管理端的界面元素的现实
    /// </summary>
    public class UIAccess : JsonObjectBase
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public int id { get; set; }


        /// <summary>
        /// 域ID
        /// </summary>
        public int domain_id { get; set; }


        /// <summary>
        /// 界面权限名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desp { get; set; }

        /// <summary>
        /// 是否有权进行交易操作
        /// </summary>
        [PermissionFieldAttr("交易操作","是否有权向分帐户下单或平仓")]
        public bool r_execution { get; set; }

        [PermissionFieldAttr("冻结帐户", "是否有权冻结或激活交易帐户")]
        public bool r_block { get; set; }

        /// <summary>
        /// 默认帐号添加规则
        /// 在有实盘的情况下默认添加实盘帐户，并且帐户类型和路由列表不可选
        /// 如果没有实盘帐号则隐藏具体的帐户类型
        /// 如果允许代理添加模拟帐号,则代理可以自行选择帐号类型
        /// </summary>
        [PermissionFieldAttr("添加模拟帐号", "是否有权添加模拟帐号")]
        public bool r_simacc { get; set; }

        [PermissionFieldAttr("提交出入金", "是否有权限提交出入金")]
        public bool r_cashop { get; set; }

        [PermissionFieldAttr("下级代理出入金", "下级代理是否有权提交出入金")]
        public bool r_cashop_subagent { get; set; }

        [PermissionFieldAttr("自动确认出入金", "自动确认出入金")]
        public bool r_cashop_auto_confirm { get; set; }


    }
}
