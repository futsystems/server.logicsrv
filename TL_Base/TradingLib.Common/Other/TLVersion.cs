using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 系统版本信息
    /// </summary>
    public class TLVersion
    {

        /// <summary>
        /// 主版本号
        /// </summary>
        public int  Major { get; set; }
        /// <summary>
        /// 次版本号
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// 修正版本号
        /// </summary>
        public int Fix { get; set; }
        /// <summary>
        /// 版本日期
        /// </summary>
        public int Date { get; set; }

        string _version = null;
        public string Version
        {
            get
            { 
                if(_version == null)
                {
                    _version = string.Format("{0}.{1}.{2}.{3}",this.Major,this.Minor,this.Fix,this.Date);
                }
                return _version;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BuildNum
        {
            get
            {
                return this.Major * 10000 + this.Minor * 100 + Fix;
            }
        }

        public static string Serialize(TLVersion version)
        {
            return string.Format("{0},{1},{2},{3}",version.Major,version.Minor,version.Fix,version.Date);
        }

        public static TLVersion Deserialize(string content)
        {
            string[] rec = content.Split(',');
            TLVersion v = new TLVersion();
            v.Major = int.Parse(rec[0]);
            v.Minor = int.Parse(rec[1]);
            v.Fix = int.Parse(rec[2]);
            v.Date = int.Parse(rec[3]);
            return v;
        }
    }
}
