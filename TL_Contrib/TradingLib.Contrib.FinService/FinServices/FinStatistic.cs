using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib
{
   /* 
    public class FinStatistic : GeneralStatistic, IFinStatistic
    {
        /// <summary>
        /// 获得所有配资账户的统计
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public static IFinStatistic GetFinStatForTotal(IClearCentreBase cc, FinServiceCentre fc)
        {
            FinStatistic f = new FinStatistic(fc);
            List<IAccount> l = new List<IAccount>();
            //遍历清算中心中的配资账户 如果该账户有激活的配资服务就加入列表
            foreach (IAccount a in cc.Loanees)
            {
                IFinService fs = fc[a.ID];
                if (fs != null && fs.Active)
                    l.Add(a);
            }

            f.SetAccounts(l.ToArray());
            return f;
        }

        /// <summary>
        /// 获得所有配资账户的统计 外加实盘参数
        /// 增加条件,如果该账户路由设置是模拟 则计入统计集进行数据统计
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public static IFinStatistic GetFinStatForSIM(IClearCentreBase cc, FinServiceCentre fc)
        {
            FinStatistic f = new FinStatistic(fc);
            List<IAccount> l = new List<IAccount>();
            //遍历清算中心中的配资账户 如果该账户有激活的配资服务就加入列表
            foreach (IAccount a in cc.Loanees)
            {
                IFinService fs = fc[a.ID];
                if (fs != null && fs.Active && a.OrderRouteType == QSEnumOrderTransferType.SIM)
                    l.Add(a);
            }

            f.SetAccounts(l.ToArray());
            return f;
        }

        /// <summary>
        /// 获得所有配资账户的统计 外加实盘参数
        /// 增加条件,如果该账户路由设置是实盘 则计入统计集进行数据统计
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public static IFinStatistic GetFinStatForLIVE(IClearCentreBase cc, FinServiceCentre fc)
        {
            FinStatistic f = new FinStatistic(fc);
            List<IAccount> l = new List<IAccount>();
            //遍历清算中心中的配资账户 如果该账户有激活的配资服务就加入列表
            foreach (IAccount a in cc.Loanees)
            {
                IFinService fs = fc[a.ID];
                if (fs != null && fs.Active && a.OrderRouteType == QSEnumOrderTransferType.LIVE)
                    l.Add(a);
            }

            f.SetAccounts(l.ToArray());
            return f;
        }
        


        FinServiceCentre _fc;
        public FinStatistic(FinServiceCentre fc)

        {
            _fc = fc;
        }

        //返回所有激活的配资本服务额度
        public decimal SumFinAmmount
        {
            get
            {
                decimal r = 0;
                foreach (IAccount acc in this)
                {
                    r += _fc[acc.ID].FinAmmount;
                }
                return r;

            }
        }
        //返回所有激活的配资服务费用
        public decimal SumFinFee 
        { 
            get {

                decimal r = 0;
                foreach (IAccount acc in this)
                {
                    r += _fc[acc.ID].GetRate();
                }
                return r;
            } 
        }
    }**/
}
