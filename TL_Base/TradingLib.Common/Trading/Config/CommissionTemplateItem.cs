using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.Json;

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
        /// 上浮一定百分比
        /// </summary>
        public decimal Percent { get; set; }

        /// <summary>
        /// 加收方式
        /// </summary>
        public QSEnumChargeType ChargeType { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int Template_ID { get; set; }

        /// <summary>
        /// 品种类别
        /// </summary>
        public SecurityType SecurityType { get; set; }

        [NoJsonExportAttr()]
        public string CommissionItemKey
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.SecurityType, this.Code, this.Month);
            }
        }
    }


    /// <summary>
    /// 手续费模板的设置项
    /// </summary>
    public class CommissionTemplateItem : CommissionTemplateItemSetting
    {

        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="f"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public decimal CalcCommission(decimal basecommission,Trade f)
        {
            decimal commission = 0;
            switch (this.SecurityType)
            {
                case SecurityType.FUT:
                    {
                        switch (f.OffsetFlag)
                        {
                            case QSEnumOffsetFlag.OPEN://开仓手续费
                                commission = (this.OpenByMoney != 0 ? CalCommissionByMoney(f, this.OpenByMoney) : CalCommissionByVolume(f, this.OpenByVolume));
                                break;
                            case QSEnumOffsetFlag.CLOSE://平仓手续费
                            case QSEnumOffsetFlag.CLOSEYESTERDAY:
                                commission = (this.CloseByMoney != 0 ? CalCommissionByMoney(f, this.CloseByMoney) : CalCommissionByVolume(f, this.CloseByVolume));
                                break;
                            case QSEnumOffsetFlag.CLOSETODAY://平今手续费
                                commission = (this.CloseTodayByMoney != 0 ? CalCommissionByMoney(f, this.CloseTodayByMoney) : CalCommissionByVolume(f, this.CloseTodayByVolume));
                                break;
                            default:
                                commission = (this.OpenByMoney != 0 ? CalCommissionByMoney(f, this.OpenByMoney) : CalCommissionByVolume(f, this.OpenByVolume));
                                break;
                        }
                        break;
                    }
                case SecurityType.STK:
                    {
                        switch (f.OffsetFlag)
                        {
                            case QSEnumOffsetFlag.OPEN://开仓手续费
                                commission = (this.OpenByMoney != 0 ? CalCommissionByMoney(f, this.OpenByMoney) : CalCommissionByVolume(f, this.OpenByVolume));
                                break;
                            case QSEnumOffsetFlag.CLOSE://平仓手续费
                            case QSEnumOffsetFlag.CLOSEYESTERDAY:
                            case QSEnumOffsetFlag.CLOSETODAY:
                                commission = (this.CloseByMoney != 0 ? CalCommissionByMoney(f, this.CloseByMoney) : CalCommissionByVolume(f, this.CloseByVolume));
                                break;
                            default:
                                commission = (this.OpenByMoney != 0 ? CalCommissionByMoney(f, this.OpenByMoney) : CalCommissionByVolume(f, this.OpenByVolume));
                                break;
                        }
                        break;
                    }
                default:
                    commission = 0;
                    break;
            }

            switch (this.ChargeType)
            {
                case QSEnumChargeType.Absolute:
                    return commission;
                case QSEnumChargeType.Relative:
                    return basecommission + commission;
                case QSEnumChargeType.Percent:
                    return basecommission * (1 + this.Percent);
                default:
                    return basecommission;
            }

        }
        /// <summary>
        /// 按成交金额计算手续费
        /// </summary>
        /// <param name="f"></param>
        /// <param name="commissionrate"></param>
        /// <returns></returns>
        decimal CalCommissionByMoney(Trade f,decimal commissionrate)
        {
            return commissionrate * f.xPrice * f.UnsignedSize * f.oSymbol.Multiple;
        }

        /// <summary>
        /// 按成交手数计算手续费
        /// </summary>
        /// <param name="f"></param>
        /// <param name="commissionrate"></param>
        /// <returns></returns>
        decimal CalCommissionByVolume(Trade f, decimal commissionrate)
        {
            return commissionrate * f.UnsignedSize;
        }
    }
}
