using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 手续费模板的设置项
    /// </summary>
    public class CommissionTemplateItem
    {
        /// <summary>
        /// 设置项ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 品种代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 合约月份
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 开仓手续费 按金额
        /// </summary>
        public decimal OpenByMoney { get; set; }

        /// <summary>
        /// 开仓手续费 按手数
        /// </summary>
        public decimal OpenByVolume { get; set; }


        /// <summary>
        /// 平今手续费 按金额
        /// </summary>
        public decimal CloseTodayByMoney { get; set; }

        /// <summary>
        /// 平今手续费 按手数
        /// </summary>
        public decimal CloseTodayByVolume { get; set; }

        /// <summary>
        /// 平仓手续费 按金额
        /// </summary>
        public decimal CloseByMoney { get; set; }

        /// <summary>
        /// 平仓手续费 按手数
        /// </summary>
        public decimal CloseByVolume { get; set; }

        /// <summary>
        /// 加收方式
        /// </summary>
        QSEnumChargeType ChargeType { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// 获得该手续费项目的键值
        /// </summary>
        /// <returns></returns>
        public string GetItemKey()
        {
            return string.Format("{0}-{1}", this.Code, this.Month);
        }
    }
}
