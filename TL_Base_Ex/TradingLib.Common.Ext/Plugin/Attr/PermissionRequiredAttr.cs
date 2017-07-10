using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TradingLib.API;


namespace TradingLib.Common
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class PermissionRequiredAttr : TLAttribute
    {
        /// <summary>
        /// 管理员类别
        /// </summary>
        public QSEnumManagerType RequiredManagerType { get; set; }

        /// <summary>
        /// 是否检查管理员类别
        /// </summary>
        bool checkManagerType = false;


        /// <summary>
        /// 权限字段
        /// </summary>
        public string RequiredPermissionField { get; set; }

        /// <summary>
        /// 是否检查权限字段
        /// </summary>
        bool checkPermissionField = false;

        public PermissionRequiredAttr(string permission)
        {
            this.RequiredPermissionField = permission;
            this.checkPermissionField = true;
        }


        public bool CheckManagerPermission(Manager mgr, out string msg)
        {
            msg = string.Empty;
            //检查权限字段
            if (this.checkPermissionField)
            {
                PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
                var pi = propertyInfos.FirstOrDefault(item => item.Name == this.RequiredPermissionField);
                if (pi == null) return false;

                var ret= (bool)pi.GetValue(mgr.Permission, null);
                if (!ret)
                {
                    msg = string.Format("无权限:{0}", BasicTracker.UIAccessTracker.GetPermissionTitle(this.RequiredPermissionField));
                    return false;
                }

            }


            return true;
        }
    }
}
