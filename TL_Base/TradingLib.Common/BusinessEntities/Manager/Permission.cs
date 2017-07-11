using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Reflection;

namespace TradingLib.Common
{
    /// <summary>
    /// 管理端界面权限
    /// 用于控制管理端的界面元素的现实
    /// </summary>
    public class Permission : JsonObjectBase
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
        /// 管理员主域ID
        /// </summary>
        public int manager_id { get; set; }


        /// <summary>
        /// 界面权限名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desp { get; set; }

        /// <summary>
        /// 添加交易帐户
        /// </summary>
        [PermissionFieldAttr("添加帐户", "添加子帐户权限")]
        public bool r_account_add { get; set; }

        /// <summary>
        /// 删除交易帐户权限
        /// </summary>
        [PermissionFieldAttr("删除帐户", "删除子账户权限")]
        public bool r_account_del { get; set; }


        [PermissionFieldAttr("编辑账户", "修改账户属性权限")]
        public bool r_account_edit { get; set; }

        /// <summary>
        /// 设置手续费权限
        /// </summary>
        [PermissionFieldAttr("修改帐户模板", "设定帐户手续费/保证金/交易参数权限")]
        public bool r_account_edit_template { get; set; }

        /// <summary>
        /// 修改帐户基本信息
        /// </summary>
        [PermissionFieldAttr("编辑账户基础信息", "修改账户基础信息权限")]
        public bool r_account_edit_profile { get; set; }


        /// <summary>
        /// 是否有权冻结交易账户
        /// </summary>
        [PermissionFieldAttr("冻结帐户", "冻结或激活交易帐户权限")]
        public bool r_account_edit_execution { get; set; }


        /// <summary>
        /// 是否有权修改帐户隔夜设置
        /// </summary>
        [PermissionFieldAttr("隔夜设置", "修改帐户隔夜设置权限")]
        public bool r_account_edit_interday { get; set; }

        /// <summary>
        /// 查询交易密码
        /// </summary>
        [PermissionFieldAttr("查询账户密码", "查询账户密码权限")]
        public bool r_account_qry_pass { get; set; }


        [PermissionFieldAttr("修改账户密码", "修改账户密码权限")]
        public bool r_account_edit_pass { get; set; }

        /// <summary>
        /// 执行出入金操作
        /// </summary>
        [PermissionFieldAttr("账户出入金", "出入金操作权限")]
        public bool r_account_cashop { get; set; }



        /// <summary>
        /// 是否有权进行交易操作
        /// </summary>
        [PermissionFieldAttr("交易操作","向子帐户下单或平仓权限")]
        public bool r_execution { get; set; }

        /// <summary>
        /// 是否有权设定风控规则
        /// </summary>
        [PermissionFieldAttr("风控规则", "设定帐户风控规则权限")]
        public bool r_riskrule { get; set; }


        [PermissionFieldAttr("添加/编辑模板", "添加/编辑手续费、保证金、交易参数模板")]
        public bool r_template_edit { get; set; }


        /// <summary>
        /// 获取某个字段值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool GetPermission(string field)
        {
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            var p = propertyInfos.Where(item => item.Name == field).FirstOrDefault();
            if (p == null) return false;
            return (bool)p.GetValue(this, null);
        }
    }
}
