using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{




    /// <summary>
    /// 域接口
    /// </summary>
    public interface Domain
    {
        /// <summary>
        /// 域ID
        /// </summary>
        int ID { get; set; }


        /// <summary>
        /// 域名称
        /// </summary>
        string Name { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        string LinkMan { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        string Mobile { get; set; }


        /// <summary>
        /// QQ号码
        /// </summary>
        string QQ { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        string Email { get; set; }


        


        /// <summary>
        /// 过期日
        /// </summary>
        int DateExpired { get; set; }


        /// <summary>
        /// 创建日
        /// </summary>
        int DateCreated { get; set; }

        /// <summary>
        /// 是否是超级域
        /// </summary>
        bool Super { get; set; }

        #region limit
        /// <summary>
        /// 帐户数目限制
        /// </summary>
        int AccLimit { get; set; }

        /// <summary>
        /// 路由组数量限制
        /// </summary>
        int RouterGroupLimit { get; set; }

        /// <summary>
        /// 路由组内路由项目数量限制
        /// </summary>
        int RouterItemLimit { get; set; }

        /// <summary>
        /// 接口列表
        /// </summary>
        string InterfaceList { get; set; }

        #endregion

    }
}
