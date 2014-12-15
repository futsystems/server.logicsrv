using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class DomainImpl : Domain
    {
        /// <summary>
        /// 域ID
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// 域名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 帐户数目限制
        /// </summary>
        public int AccLimit { get; set; }


        /// <summary>
        /// 过期日
        /// </summary>
        public int DateExpired { get; set; }


        /// <summary>
        /// 创建日
        /// </summary>
        public int DateCreated { get; set; }


        /// <summary>
        /// 是否是超级域
        /// </summary>
        public bool Super { get; set; }

        /// <summary>
        /// 独立安装标识
        /// </summary>
        public bool Dedicated { get; set; }

        /// <summary>
        /// 路由组数量限制
        /// </summary>
        public int RouterGroupLimit { get; set; }

        /// <summary>
        /// 路由组内路由项目数量限制
        /// </summary>
        public int RouterItemLimit { get; set; }

        /// <summary>
        /// 接口列表
        /// </summary>
        public string InterfaceList { get; set; }

        /// <summary>
        /// 配资服务计划列表
        /// </summary>
        public string FinSPList { get; set; }

        /// <summary>
        /// 代理模块
        /// </summary>
        public bool Module_Agent { get; set; }

        /// <summary>
        /// 是否支持多级代理
        /// </summary>
        public bool Module_SubAgent { get; set; }

        /// <summary>
        /// 配资模块
        /// </summary>
        public bool Module_FinService { get; set; }

        /// <summary>
        /// 在线出入金模块
        /// </summary>
        public bool Module_PayOnline { get; set; }


        /// <summary>
        /// 实盘路由
        /// </summary>
        public bool Router_Live { get; set; }

        /// <summary>
        /// 模拟路由
        /// </summary>
        public bool Router_Sim { get; set; }


    }
}
