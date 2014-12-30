//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.Common
//{
//    public enum QSEnumCfgType
//    {
//        Int,
//        Decimal,
//        String,
//        Bool
//    }

//    /// <summary>
//    /// 设置
//    /// </summary>
//    public class ConfigItem
//    {
//        public string CfgName { get; set; }
//        public string CfgValue { get; set; }
//        public QSEnumCfgType CfgType { get; set; }
//        public string ModuleID { get; set; }
//        public string Description { get; set; }

//        public int AsInt()
//        { 
//            int i=0;
//            if (int.TryParse(this.CfgValue, out i))
//            {
//                return i;
//            }
//            return 0;
//        }

//        public Decimal AsDecimal()
//        {
//            decimal d = 0;
//            if (decimal.TryParse(this.CfgValue, out d))
//            {
//                return d;
//            }
//            return 0;
//        }

//        public string AsString()
//        {
//            return this.CfgValue;
//        }

//        public bool AsBool()
//        {
//            bool b = true;
//            if (bool.TryParse(this.CfgValue, out b))
//            {
//                return b;
//            }
//            return true;
//        }


//    }
//}
