using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{

    public class CommissionTemplateItemSetting
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
        public QSEnumChargeType ChargeType { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int Template_ID { get; set; }
    }


    /// <summary>
    /// 手续费模板的设置项
    /// </summary>
    public class CommissionTemplateItem : CommissionTemplateItemSetting
    {
        

        /// <summary>
        /// 获得该手续费项目的键值
        /// </summary>
        /// <returns></returns>
        public string GetItemKey()
        {
            return string.Format("{0}-{1}", this.Code, this.Month);
        }

        /// <summary>
        /// 从基准手续费和开平标识 计算手续费率
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public decimal GetCommission(decimal c,QSEnumOffsetFlag offset)
        {
            if (ChargeType == QSEnumChargeType.Absolute)
            {
                switch (offset)
                { 
                    case QSEnumOffsetFlag.OPEN://开仓手续费
                        return this.OpenByMoney != 0 ? this.OpenByMoney : this.OpenByVolume;
                    case QSEnumOffsetFlag.CLOSE://平仓手续费
                    case QSEnumOffsetFlag.CLOSEYESTERDAY:
                        return this.CloseByMoney != 0 ? this.CloseByMoney : this.CloseByVolume;
                    case QSEnumOffsetFlag.CLOSETODAY://平今手续费
                        return this.CloseTodayByMoney != 0 ? this.CloseTodayByMoney : this.CloseTodayByVolume;
                    default:
                        return this.OpenByMoney != 0 ? this.OpenByMoney : this.OpenByVolume;
                }
            }
            else
            {
                switch (offset)
                {
                    case QSEnumOffsetFlag.OPEN://开仓手续费
                        return c + (c < 1 ? this.OpenByMoney : this.OpenByVolume);
                    case QSEnumOffsetFlag.CLOSE://平仓手续费
                    case QSEnumOffsetFlag.CLOSEYESTERDAY:
                        return c + (c < 1 ? this.CloseByMoney : this.CloseByVolume);
                    case QSEnumOffsetFlag.CLOSETODAY://平今手续费
                        return c + (c < 1 ? this.CloseTodayByMoney : this.CloseTodayByVolume);
                    default:
                        return c + (c < 1 ? this.OpenByMoney : this.OpenByVolume);
                }
            
            }
        }
    }
}
