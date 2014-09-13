using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;

namespace TradingLib.Contrib
{
    /*
    public class WebStatisticReport : GeneralStatistic, IWebStatistic
    {
        /// <summary>
        /// 每个报告组件包含有GetReport,然后传入对应的参数,生成对应的报告
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static IWebStatistic GetReport(ClearCentreSrv cc)
        {
            return new WebStatisticReport(cc);
        }

        /// <summary>
        /// 注册报告组件,当我们使用这些报告组件时,我们需要先生成对应的组件后才可以在这些组件的基础上生成对应的Report
        /// </summary>
        /// <param name="cc"></param>
        public WebStatisticReport(ClearCentreSrv cc)
        {
            this.SetAccounts(cc.Accounts);
        }
       
        public int TotalAccounts
        {
            get 
            {
                return this.NumTotalAccount;
            }
        }

        public new int TotalOrders
        {
            get
            {
                return this.NumOrders;
            }
        }

        public new long TotalMargin
        {
            get
            {
                return (long)this.SumMargin;
            }
        }

    }
     * **/
}
