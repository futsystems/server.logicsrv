using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class AccountCreation
    {
        public AccountCreation()
        {
            this.Account = string.Empty;
            this.Password = string.Empty;
            this.Category = QSEnumAccountCategory.SIMULATION;
            this.RouterType = QSEnumOrderTransferType.SIM;
            this.UserID = 0;
            this.Domain = null;
        }
        /// <summary>
        /// 指定帐户ID
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 设定的帐户和密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 帐户类型
        /// </summary>
        public QSEnumAccountCategory Category { get; set; }


        /// <summary>
        /// 路由类别
        /// </summary>
        public QSEnumOrderTransferType RouterType { get; set; }

        /// <summary>
        /// 用户ID预留与web站点帐户系统
        /// </summary>
        public int UserID { get; set; }


        /// <summary>
        /// 所属柜员
        /// </summary>
        public Manager BaseManager { get; set; }

        /// <summary>
        /// 路由组
        /// </summary>
        public RouterGroup RouteGroup { get; set; }

        /// <summary>
        /// 帐号所属域
        /// </summary>
        public Domain Domain { get; set; }


        
    }
}
