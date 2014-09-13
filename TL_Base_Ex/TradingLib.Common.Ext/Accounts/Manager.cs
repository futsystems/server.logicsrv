using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class Manager
    {
        public Manager()
        {
            Login = string.Empty;
            User_Id = 0;
            Type = QSEnumManagerType.ROOT;
            Name = string.Empty;
            Mobile = string.Empty;
            QQ = string.Empty;
            AccLimit = 0;
            mgr_id = 0;
            BaseManager = null;

        }

        /// <summary>
        /// 数据库ID编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 登入名
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// 对应UCenter UserID
        /// </summary>
        public int User_Id { get; set; }

        /// <summary>
        /// 管理员类别
        /// </summary>
        public QSEnumManagerType Type { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 交易帐号数目限制
        /// </summary>
        public int AccLimit { get; set; }


        /// <summary>
        /// 交易managerid
        /// </summary>
        public int mgr_id { get; set; }

        /// <summary>
        /// BaseManager用于标注该管理帐号隶属于哪个Agent,如果是系统级的管理帐户的话直接隶属于ROOT
        /// </summary>
        public Manager BaseManager { get; set; }



    }
}
