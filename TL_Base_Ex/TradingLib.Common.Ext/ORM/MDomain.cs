using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MDomain:MBase
    {
        /// <summary>
        /// 获得所有域信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DomainImpl> SelectDomains()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM domain");
                return db.Connection.Query<DomainImpl>(query);
            }
        }

        public static void UpdateDomain(DomainImpl domain)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE domain SET name='{0}' ,linkman='{1}',mobile='{2}',qq='{3}',email='{4}',dateexpired='{5}',acclimit='{6}',routergrouplimit='{7}',routeritemlimit='{8}'  WHERE id='{9}'", domain.Name,domain.LinkMan,domain.Mobile,domain.QQ,domain.Email,domain.DateExpired,domain.AccLimit,domain.RouterGroupLimit,domain.RouterItemLimit,domain.ID);
                db.Connection.Execute(query);
            }
        }

        public static void InsertDomain(DomainImpl domain)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into domain (`name`,`linkman`,`mobile`,`qq`,`email`,`dateexpired`,`acclimit`,`routergrouplimit`,`routeritemlimit`,`datecreated`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", domain.Name, domain.LinkMan, domain.Mobile, domain.QQ, domain.Email, domain.DateExpired,domain.AccLimit, domain.RouterGroupLimit, domain.RouterItemLimit, Util.ToTLDate());
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => domain.ID = id, "id", "domain");
            }
        }
    }
}
