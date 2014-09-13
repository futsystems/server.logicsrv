using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottoqq.MJService
{
    public class MJConstant
    {
        public static QSEnumMJFeeType DEFAULT_FEETYPE = QSEnumMJFeeType.ByMonth;//默认开通的秘籍服务是按月收费
        public static QSEnumMJServiceLevel DEFAULT_LEVEL = QSEnumMJServiceLevel.LT;//默认开通的秘籍级别是最高级别
        public static int DEFAULT_DEMO_DAYS = 30;//默认开通的秘籍服务的天数



        public static string FEETYPE_NOT_SUPPORT_EXTTIME = "手续费加成方式不支持延长有效时间";
        public static string FEETYPE_SAME = "秘籍服务计费类别相同";
        public static string MJLEVEL_SAME = "秘籍服务档位相同";


        public static decimal Commission_L1 = 0.4M;
        public static decimal Commission_L2 = 0.3M;
        public static decimal Commission_L3 = 0.2M;
        public static decimal Commission_L4 = 0.15M;
        public static decimal Commission_L5 = 0.13M;
        public static decimal Commission_LT = 0.1M;

        public static decimal GetCommissionPect(QSEnumMJServiceLevel level)
        {
            switch (level)
            { 
                case QSEnumMJServiceLevel.L1:
                    return Commission_L1;
                case QSEnumMJServiceLevel.L2:
                    return Commission_L2;
                case QSEnumMJServiceLevel.L3:
                    return Commission_L3;
                case QSEnumMJServiceLevel.L4:
                    return Commission_L4;
                case QSEnumMJServiceLevel.L5:
                    return Commission_L5;
                case QSEnumMJServiceLevel.LT:
                    return Commission_LT;
                default :
                    return 0.4M;
            }
        }
    }
}
