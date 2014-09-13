//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;
//using TradingLib.Core;


//namespace TradingLib.Contrib
//{
//    /// <summary>
//    /// 管理管理员数据
//    /// </summary>
//    public class CustomerManager
//    {

//        ConnectionPoll<mysqlCustomer> conn;

//        public CustomerManager()
//        {
//            conn = new ConnectionPoll<mysqlCustomer>("127.0.0.1", "root", "123456", CoreGlobal.DBName, CoreGlobal.DBPort);
//        }

//        const string CUSTID = "管理员ID";
//        const string CUSTTYPE = "权限类别";
//        const string CUSTNAME = "姓名";
//        const string CUSTQQ = "QQ号码";
//        const string CUSTPHONE = "电话号码";
//        public DataTable GetCustomersTable()
//        {
//            DataTable table = new DataTable();
//            table.Columns.Add(CUSTID);
//            table.Columns.Add(CUSTTYPE);
//            table.Columns.Add(CUSTNAME);
//            table.Columns.Add(CUSTQQ);
//            table.Columns.Add(CUSTPHONE);
//            mysqlCustomer db = conn.mysqlDB;
//            DataSet ds = db.getCustomers();
//            DataTable dt = ds.Tables["customers"];
//            conn.Return(db);
//            List<Customer> clist = new List<Customer>();
//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                DataRow dr = dt.Rows[i];
//                string custid = Convert.ToString(dr["customer"]);
//                string pass = Convert.ToString(dr["pass"]);
//                QSEnumCustomerType type = (QSEnumCustomerType)Enum.Parse(typeof(QSEnumCustomerType), Convert.ToString(dr["type"]));
//                string acclist = Convert.ToString(dr["accounts"]);
//                string name = Convert.ToString(dr["name"]);
//                string qq = Convert.ToString(dr["qq"]);
//                string phone = Convert.ToString(dr["phone"]);

//                table.Rows.Add(new object[] { custid, type, name, qq, phone });
//            }
//            return table;
//        }
//        public Customer GetCustomer(string custid)
//        {
//            mysqlCustomer db = conn.mysqlDB;
//            DataSet ds = db.getCustomer(custid);
//            DataTable dt = ds.Tables["customers"];
//            conn.Return(db);
//            if (dt.Rows.Count == 0) return null;
//            DataRow dr = dt.Rows[0];
//            string id = Convert.ToString(dr["customer"]);
//            string pass = Convert.ToString(dr["pass"]);
//            QSEnumCustomerType type = (QSEnumCustomerType)Enum.Parse(typeof(QSEnumCustomerType), Convert.ToString(dr["type"]));
//            string acclist = Convert.ToString(dr["accounts"]);
//            string name = Convert.ToString(dr["name"]);
//            string qq = Convert.ToString(dr["qq"]);
//            string phone = Convert.ToString(dr["phone"]);
//            Customer c = new Customer(id, pass, type, acclist, name, qq, phone);
//            return c;
//        }
//        public List<Customer> GetCustomers()
//        {
//            mysqlCustomer db = conn.mysqlDB;
//            DataSet ds = db.getCustomers();
//            DataTable dt = ds.Tables["customers"];
//            conn.Return(db);
//            List<Customer> clist = new List<Customer>();
//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                DataRow dr = dt.Rows[i];
//                string custid = Convert.ToString(dr["customer"]);
//                string pass = Convert.ToString(dr["pass"]);
//                QSEnumCustomerType type = (QSEnumCustomerType)Enum.Parse(typeof(QSEnumCustomerType), Convert.ToString(dr["type"]));
//                string acclist = Convert.ToString(dr["accounts"]);
//                string name = Convert.ToString(dr["name"]);
//                string qq = Convert.ToString(dr["qq"]);
//                string phone = Convert.ToString(dr["phone"]);
//                Customer c = new Customer(custid, pass, type, acclist, name, qq, phone);
//                clist.Add(c);
//            }
//            return clist;
//        }
//        public bool AddCustomer(Customer cust)
//        {
//            TLCtxHelper.Debug("添加管理账户");
//            try
//            {
//                mysqlCustomer db = conn.mysqlDB;
//                bool re =  db.AddCustomer(cust.CustomerID, cust.CustomerPass, cust.CustomerType, cust.AccountList, cust.CustomerName, cust.CustomerQQ, cust.CustomerPhone);
//                conn.Return(db);
//                return re;
//            }
//            catch (Exception ex)
//            {
//                TLCtxHelper.Debug("添加账户出错:" + ex.ToString());
//                return false;
//            }
            
//        }

//        public bool DelCustomer(string custid)
//        {
//            try
//            {
//                mysqlCustomer db = conn.mysqlDB;
//                bool re = db.DelCustomer(custid);
//                conn.Return(db);
//                return re;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }
//    }
//}
