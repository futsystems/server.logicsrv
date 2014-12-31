using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{

    public class MConfig:MBase
    {
        public static IEnumerable<ConfigItem> SelectConfigItem(string moduleid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM cfg_module WHERE moduleid='{0}'", moduleid);
                IEnumerable<ConfigItem> result = db.Connection.Query<ConfigItem>(query, null);
                return result;
            }
        }

        public static int UpdateConfigItem(ConfigItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "UPDATE cfg_module SET cfgtype = @cfgtype, cfgvalue=@cfgvalue ,cfgdescription = @cfgdescription WHERE moduleid = @moduleid AND cfgname = @cfgname";
                int row = db.Connection.Execute(query, new { cfgtype = item.CfgType, cfgvalue = item.CfgValue, cfgdescription = item.Description, moduleid = item.ModuleID, cfgname = item.CfgName });
                return row;
            }
        }

        public static int InsertConfigItem(ConfigItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "INSERT INTO cfg_module (cfgname,cfgtype,cfgvalue,cfgdescription,moduleid) values (@cfgname,@cfgtype,@cfgvalue,@cfgdescription,@moduleid)";
                int row = db.Connection.Execute(query, new { cfgname = item.CfgName, cfgtype = item.CfgType.ToString(), cfgvalue = item.CfgValue, cfgdescription = item.Description, moduleid = item.ModuleID });
                return row;
            }
        }


    }
}
