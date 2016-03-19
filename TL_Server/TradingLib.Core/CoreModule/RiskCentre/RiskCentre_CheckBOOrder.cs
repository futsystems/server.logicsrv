using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RiskCentre
    {
        #region 【委托检查1】

        /// <summary>
        /// 风控中心委托一段检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="acc"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter">是否是内部委托 比如风控系统产生的委托</param>
        /// <returns></returns>
        public bool CheckOrderStep(ref BinaryOptionOrder o,IAccount account,out bool needlog, out string errortitle,bool inter=false)
        {
            errortitle = string.Empty;
            needlog = true;

            //1.1检查结算中心是否正常状态 如果历史结算状态则需要将结算记录补充完毕后才可以接受新的委托
            if (!TLCtxHelper.ModuleSettleCentre.IsNormal)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //结算中心处于实时模式 才可以接受委托操作
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.LiveMode)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //1.3检查结算中心是否处于结算状态 结算状态不接受任何委托
            if (TLCtxHelper.ModuleSettleCentre.IsInSettle)
            {
                errortitle = "SETTLECENTRE_IN_SETTLE";//结算中心出入结算状态
                needlog = false;
                return false;
            }

            //2 清算中心检查
            //2.1检查清算中心是否出入接受委托状态(正常工作状态下系统会定时开启和关闭清算中心,如果是开发模式则可以通过手工来提前开启)
            if (TLCtxHelper.ModuleClearCentre.Status != QSEnumClearCentreStatus.CCOPEN)
            {
                errortitle = "CLEARCENTRE_CLOSED";//清算中心已关闭
                needlog = false;
                return false;
            }



            //3 合约检查
            //3.1合约是否存在
            if (!account.TrckerOrderSymbol(ref o))
            {
                errortitle = "BO_ERROR_SYMBOL_NOT_EXIST";//合约不存在
                needlog = false;
                return false;
            }

            //3.2合约是否可交易
            if (!o.oSymbol.IsTradeable)//合约不可交易
            {
                errortitle = "BO_ERROR_SYMBOL_NOTALLOWED";//合约不可交易
                needlog = false;
                return false;
            }
            //合约过期检查
            int exday = o.oSymbol.SecurityFamily.Exchange.GetExchangeTime().ToTLDate();
            if (o.oSymbol.IsExpired(exday))
            {
                errortitle = "BO_ERROR_SYMBOL_EXPIRE";//合约过期
                needlog = false;
                return false;
            }


            //交易时间检查
            int settleday = 0;
            QSEnumActionCheckResult result = o.oSymbol.SecurityFamily.CheckPlaceOrder(out settleday);
            if (result != QSEnumActionCheckResult.Allowed)
            {
                errortitle = "BO_ERROR_SYMBOL_NOT_MARKETTIME";//合约非交易时间段
                needlog = false;
                return false;
            }
            //设定交易日
            o.SettleDay = settleday;

            //特定交易日判定
            //if (!o.oSymbol.SecurityFamily.CheckSpecialHoliday())
            //{
            //    errortitle = "SYMBOL_NOT_MARKETTIME";
            //    needlog = false;
            //    return false;
            //}

            
            //下单金额为零
            if (o.Amount == 0)
            {
                errortitle = "BO_ERROR_AMOUNT_ZERO";//下单金额为零
                return false;
            }
            
            //行情检查
            Tick tk = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.BinaryOption.Symbol);
            if (tk == null || (!tk.IsValid()))
            {
                errortitle = "BO_ERROR_MARKETDATA_NULL";//市场行情异常
                return false;
            }

            //时间检查 
            int left = 0;
            bool timevalid = TimeLeftValid(o,out left);
            if (!timevalid)
            {
                errortitle = string.Format("{0}周期到期前{1}秒,禁止下单", o.BinaryOption.TimeSpanType, left);
                return false;
            }

            return true;

        }

        #endregion

        bool TimeLeftValid(BinaryOptionOrder o,out int targetleft)
        {
            DateTime now = DateTime.Now;
            DateTime next = Util.ToDateTime(BinaryOptionOrderImpl.CalcExpireTime(now.ToTLDateTime(), o.BinaryOption.TimeSpanType));
            int left = (int)next.Subtract(now).TotalSeconds;

            switch (o.BinaryOption.TimeSpanType)
            {
                case EnumBinaryOptionTimeSpan.MIN1:
                    targetleft = _m10left;
                    return left >= _m1left;
                case EnumBinaryOptionTimeSpan.MIN2:
                    targetleft = _m2left;
                    return left >= _m2left;
                case EnumBinaryOptionTimeSpan.MIN5:
                    targetleft = _m5left;
                    return left >= _m5left;
                case EnumBinaryOptionTimeSpan.MIN10:
                    targetleft = _m10left;
                    return left >= _m10left;
                case EnumBinaryOptionTimeSpan.MIN15:
                    targetleft = _m15left;
                    return left >= _m15left;
                case EnumBinaryOptionTimeSpan.MIN30:
                    targetleft = _m30left;
                    return left >= _m30left;
                case EnumBinaryOptionTimeSpan.MIN60:
                    targetleft = _m60left;
                    return left >= _m60left;
                default:
                    targetleft = 0;
                    return false;
            }
        }

    }
}
