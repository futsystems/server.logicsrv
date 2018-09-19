using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    
    public class MSystem:MBase
    {

        public static TLVersion GetVersion()
        { 
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM system";
                SystemInformation info = db.Connection.Query<SystemInformation>(query).SingleOrDefault<SystemInformation>();
                TLVersion v = new TLVersion();
                v.Major = info.Maj;
                v.Minor = info.Min;
                v.Fix = info.Fix;
                v.Date = info.Date;
                v.ProductType = info.ProductType;
                v.DeployID = info.DeployID;
                System.OperatingSystem osInfo = System.Environment.OSVersion;
                v.Platfrom = System.Environment.OSVersion.Platform;
                
                return v;
            }
        }

        /// <summary>
        /// 更版本数据
        /// </summary>
        /// <param name="t"></param>
        public static void UpdateVersion(int major, int minor, int fix)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE system SET maj='{0}' ,min='{1}' ,fix='{2}'",major,minor,fix);
                db.Connection.Execute(query);
            }
        }

        public static void UpdateDeployID(string deploy)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE system SET deployid='{0}' ", deploy);
                db.Connection.Execute(query);
            }
        }

    }
}
