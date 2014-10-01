using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于实现单边计费或者日内不对称计费
    /// </summary>
    public class CommissionHelper
    {

        /// <summary>
        /// 检查某个合约日内交易的手续费设置,单边计费
        /// </summary>
        /// <param name="symbolcode"></param>
        /// <param name="needcal"></param>
        /// <returns></returns>
        public static bool AnyCommissionSetting(string symbolcode,out decimal commission)
        {

            commission = 0;
            bool re = false;
            switch (symbolcode)
            { 
                //单边计费品种,日内平仓手续费为0
                case "ag"://银
                case "au"://金
                case "cu"://铜
                case "bu"://沥青

                case "SR"://糖
                case "FG"://玻璃
                case "RM"://菜籽粕
                case "RS"://油菜籽
                case "TC"://动力煤
                    re=true;
                    break;
                case "j":
                    re = true;
                    commission = 0.00011M/2;
                    break;
                case "jm":
                    re = true;
                    commission = 0.00012M/2;
                    break;
                default:
                    re = false;
                    break;
            }

            return re;
        
        }

    }
}
