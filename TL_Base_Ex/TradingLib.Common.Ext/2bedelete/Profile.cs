using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /*
    /// <summary>
    /// 获得交易账号对应的用户profile
    /// </summary>
    public class Profile:IProfile
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public QSEnumGender Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 网站/blog
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 个人描述
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// 网站昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }


        public static string Serialize(IProfile p)
        {
            string d = "|";
            return p.Name + d + p.Gender.ToString() + d + p.Birthday.ToString() + d + p.Location + d + p.Address + d + p.WebSite + d + p.QQ + d + p.Phone + d + p.PersonID + d + p.About + d + p.NickName + d + p.Email + d + p.Account;
        
        
        }


        public static IProfile FromString(string msg)
        {
            string[] p = msg.Split('|');
            if (p.Length < 10) return null;
            Profile pf = new Profile();
            pf.Name = p[0];
            pf.Gender = (QSEnumGender)Enum.Parse(typeof(QSEnumGender), p[1]);
            pf.Birthday = Convert.ToDateTime(p[2]);
            pf.Location = p[3];
            pf.Address = p[4];
            pf.WebSite = p[5];
            pf.QQ = p[6];
            pf.Phone = p[7];
            pf.PersonID = p[8];
            pf.About = p[9];
            pf.NickName = p[10];
            pf.Email = p[11];
            pf.Account = p[12];
            return pf;
        }

    }**/
}
