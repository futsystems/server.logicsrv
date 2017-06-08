using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class ManagerSetting
    {
        /// <summary>
        /// 数据库ID编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 登入名
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pass { get; set; }

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
        /// 下级代理数量
        /// </summary>
        public int AgentLimit { get; set; }

        /// <summary>
        /// 代理有限额度
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// 上级代理
        /// </summary>
        public int parent_fk { get; set; }

        /// <summary>
        /// 交易managerid
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int domain_id { get; set; }

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        //public int Permission_ID { get; set; }
    }

    public class Manager:ManagerSetting
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
            mgr_fk = 0;
            BaseManager = null;
            domain_id = 0;
            Pass = "123456";
        }



        /// <summary>
        /// 管理域ID
        /// Root或者代理下面的柜员的MgrID一致
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int BaseMgrID { get { return mgr_fk; } }

        /// <summary>
        /// BaseManager用于标注该管理帐号隶属于哪个Agent,如果是系统级的管理帐户的话直接隶属于ROOT
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Manager BaseManager { get; set; }

        /// <summary>
        /// 上级代理对象
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Manager ParentManager { get; set; }


        /// <summary>
        /// 分区域对象
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Domain Domain { get; internal set; }


        /// <summary>
        /// 代理AgentAccount
        /// </summary>
        public IAgent AgentAccount { get; set; }

        public override string ToString()
        {
            return string.Format("Manager[{0}]:{1} Type:{2} BaseFK:{3} ParentFK:{4}", this.ID, this.Login, this.Type, this.mgr_fk, this.parent_fk);
        }
    }
}
