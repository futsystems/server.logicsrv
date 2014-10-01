using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using Lottoqq.MJService;
using TradingLib.Mixins.DataBase;


namespace TradingLib.MySql
{
    internal class AccountId
    {
        public string AccId { get; set; }
    }
    public class MMJService:MBase
    {
        public static IEnumerable<MJService> SelectMJService()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT a.id,a.feetype,a.level,a.expireddate,a.accid  from lottoqq_mjservice a";
                IEnumerable<MJService> result = db.Connection.Query<MJService, AccountId, MJService>(query, (mj, acc) => { mj.Account = TLCtxHelper.CmdAccount[acc.AccId]; return mj; }, null, null, false, "accid", null, null).ToArray(); ;
                return result;
            }
        }

        /// <summary>
        /// 添加秘籍服务
        /// </summary>
        /// <param name="mjservice"></param>
        /// <returns></returns>
        public static int InsertMJService(MJService mjservice)
        {
            using (DBMySql db = new DBMySql())
            {
                if (mjservice.FeeType == QSEnumMJFeeType.ByMonth)
                {
                    mjservice.ExpiredDate = DateTime.Now;
                }
                const string query = "INSERT INTO lottoqq_mjservice (accid,feetype,level,expireddate) values (@ACCID,@FeeType,@Level,@ExpiredDate)";
                int row = db.Connection.Execute(query, new { ACCID = mjservice.Account.ID, FeeType = mjservice.FeeType.ToString(), Level = mjservice.Level.ToString(), ExpiredDate = mjservice.ExpiredDate });
                //更新对象的Id为数据库里新增的Id,假如增加之后不需要获得新增的对象，
                //只需将对象添加到数据库里，可以将下面的一行注释掉。
                SetIdentity(db.Connection, id => mjservice.ID = id, "id", "lottoqq_mjservice");

                return row;
            }
        }



        /// <summary>
        /// 更新秘籍收费类别
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int UpdateFeeType(MJService mj)
        {
            using (DBMySql db = new DBMySql())
            {

                const string query = "UPDATE lottoqq_mjservice SET feetype = @feetype WHERE id = @ID";
                int row = db.Connection.Execute(query, new { feetype = mj.FeeType.ToString(), ID = mj.ID });
                return row;
            }
        }

        /// <summary>
        /// 更新秘籍级别
        /// </summary>
        /// <param name="mj"></param>
        /// <returns></returns>
        public static int UpdateLevel(MJService mj)
        {
            using (DBMySql db = new DBMySql())
            {

                const string query = "UPDATE lottoqq_mjservice SET level = @level WHERE id = @ID";
                int row = db.Connection.Execute(query, new { level = mj.Level.ToString(), ID = mj.ID });


                return row;
            }
        }

        /// <summary>
        /// 更新秘籍有效期
        /// </summary>
        /// <param name="mj"></param>
        /// <returns></returns>
        public static int UpdateExpiredDate(MJService mj)
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "UPDATE lottoqq_mjservice SET expireddate = @expireddate WHERE id = @ID";
                int row = db.Connection.Execute(query, new { expireddate = mj.ExpiredDate, ID = mj.ID });


                return row;
            }
        }


        
    }
}
