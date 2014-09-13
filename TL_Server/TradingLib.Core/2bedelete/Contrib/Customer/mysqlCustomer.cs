//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using MySql.Data.MySqlClient;
//using TradingLib.API;


//namespace TradingLib.MySql
//{
//    public class mysqlCustomer:mysqlDBBase
//    {

//        /// <summary>
//        /// 获得管理员数据集
//        /// </summary>
//        /// <returns></returns>
//        public DataSet getCustomers()
//        {
//            this.SqlReady();
//            string sql = "select * from customers";
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "customers");
//            return retSet;
//        }

//        public DataSet getCustomer(string custid)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from customers where customer='{0}'",custid);
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "customers");
//            return retSet;
//        }
//        bool haveCustomer(string customerid)
//        {
//            this.SqlReady();
//            string sql = String.Format("select customer from customers where customer='{0}'",customerid);
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                myReader.GetString("customer");
//                return true;

//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//            finally
//            {
//                myReader.Close();

//            }
//        }

//        /// <summary>
//        /// 新增或者更新某个customer记录
//        /// </summary>
//        /// <param name="custid"></param>
//        /// <param name="pass"></param>
//        /// <param name="type"></param>
//        /// <param name="acclist"></param>
//        /// <param name="name"></param>
//        /// <param name="qq"></param>
//        /// <param name="phone"></param>
//        /// <returns></returns>
//        public bool AddCustomer(string custid, string pass, QSEnumCustomerType type,string acclist,string name,string qq, string phone)
//        {
//            this.SqlReady();
//            if (!haveCustomer(custid))//不存在该customer则添加该customer记录
//            {
//                string sql = String.Format("Insert into customers (`customer`,`pass`,`type`,`accounts`,`name`,`qq`,`phone`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", custid, pass, type.ToString(), acclist, name, qq, phone);
//                cmd.CommandText = sql;
//                if (cmd.ExecuteNonQuery() > 0)
//                    return true;
//                else
//                    return false;
//            }
//            else//存在该customer则进行信息更新
//            {
//                string sql = String.Format("UPDATE customers SET pass = '{0}',type='{1}',accounts='{2}',name='{3}',qq='{4}',phone='{5}' WHERE customer = '{6}'", pass, type.ToString(), acclist, name, qq, phone,custid);
//                cmd.CommandText = sql;
//                return (cmd.ExecuteNonQuery() > 0);
                
//            }
            
//        }
//        /// <summary>
//        /// 删除某个管理员信息
//        /// </summary>
//        /// <param name="custid"></param>
//        /// <returns></returns>
//        public bool DelCustomer(string custid)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from customers where customer='{0}'", custid);
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return true;
//            else
//                return false;
//        }
//    }
//}
