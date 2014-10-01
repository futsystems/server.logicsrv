using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Auth.UCenter
{
    [ContribAttr("UCenterAuth","UCenter独立认证中心","用于交易平台使用UCenter独立用户中心进行登入认证")]
    public class UCenterAuth:BaseSrvObject,IContrib
    {

        ConnectionPoll<UCenterDB> conn;


        public UCenterAuth()
            : base("UCenterAuth")
        {
            conn = new ConnectionPoll<UCenterDB>("192.168.2.213", "admin", "hky123456", "thinksns_3_1", 3306);
        }
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            TLCtxHelper.Debug("AuthCenter bind authuser event");
            //TLCtxHelper.EventSession.AuthUserEvent += new AuthUserDel(AuthUser);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }


        /// <summary>
        /// 统一认证入口
        /// 返回
        /// >0:返回了正确的uid表面认证通过
        /// 0:消息路由中的认证事件没有有效绑定到认证入口
        /// -1:邮件地址不存在
        /// -2:手机号码不存在
        /// -3:用户名不存在
        /// -4:交易帐号不存在
        /// -9:密码错误
        /// 用户uid,p130xxxxx,email
        /// 系统支持3种ID认证方式 全局用户ID,手机号码,电子邮件地址
        /// 认证完毕返回全局ID
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <param name="authtype">-1系统只能识别 0邮件 1手机 2username 3uid 4account</param>
        /// <returns></returns>
        int AuthUser(string login, string password,int authtype)
        {
            UCenterDB db = conn.mysqlDB;
            DataSet ds = db.GetUserByEmail(login);
            //获得按email搜索到的用户对象
            DataTable tb = ds.Tables["users"];
            //TLCtxHelper.Debug("login:" + login + " passwd:" + password);
            //如果没有搜索到用户对象则认证失败
            if (tb.Rows.Count < 1)
            {
                return -1;
            }

            //利用user数据中的salt以及提供的密码运算md5散列值
            DataRow dr = tb.Rows[0];
            string ps = MD5Encoding(MD5Encoding(password) + dr["login_salt"]).ToString();
            TLCtxHelper.Debug("login:" + login + " passwd:" + password + " md5:" + ps.ToString());
            //比较密码散列值,相符则认证成功,否则认证失败
            if (ps.Equals(dr["password"].ToString()))
            {
                int uid = Convert.ToInt32(dr["uid"]);
                TLCtxHelper.Debug("Request Uid:" + uid.ToString());
                return uid;
            }
            return -9;
        }

        /// <summary>
        /// MD5 加密字符串
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <returns>加密后字符串</returns>
        static string MD5Encoding(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

    }

    class UCenterDB : mysqlDBBase
    {
        public DataSet GetUserByEmail(string email)
        {
            this.SqlReady();
            string sql = String.Format("select * from ts_user  WHERE login = '{0}'", email);
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "users");
            return retSet;
        }
    }
}
