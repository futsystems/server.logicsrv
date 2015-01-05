using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.API;

namespace TradingLib.Common
{
    public enum QSEnumFrontType
    {
        [Description("直连")]
        Direct,//直接连接到逻辑服务器
        [Description("FastAccess")]
        FastAccess,//通过FastAccess连接到逻辑服务器
        [Description("EVAccess")]
        EVAccess,//libevent前置接入的客户端 libevent自维护心跳信息
        [Description("WebAccess")]
        WebAccess,//通过webaccess连接到逻辑服务器

        [Description("位置类别")]
        Unknown,//通过webaccess连接到逻辑服务器
    }

    public delegate void ClientInfoDelegate<T>(T client) where T : ClientInfoBase;
    public delegate void ClientLoginInfoDelegate<T>(T client,bool login) where T:ClientInfoBase;


    /// <summary>
    /// 记录交易客户端通讯的信息
    /// </summary>
    public class ClientInfoBase
    {
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public ILocation Location { get; set; }

        /// <summary>
        /// 连接端硬件码
        /// </summary>
        public string HardWareCode { get; set; }

        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 客户名称比如Pobo记录客户端通过哪个客户端软件进行登入
        /// </summary>
        public string ProductInfo { get; set; }

        /// <summary>
        /// 客户端最近心跳时间
        /// </summary>
        public DateTime HeartBeat { get; set; }

        /// <summary>
        /// 客户端登入ID
        /// 登入ID与回话ISession中AuthorizedID区别
        /// 登入按认证方式可以通过邮件或手机登入，但是回话中的Authoerized必须与对应的交易帐户所对应
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 监察该客户端是否登入
        /// </summary>
        public bool Authorized { get; protected set; }

        /// <summary>
        /// 前置类别
        /// </summary>
        public QSEnumFrontType FrontType { get; set; }

        /// <summary>
        /// 前置整数编号
        /// </summary>
        public int FrontIDi { get; set; }

        /// <summary>
        /// 前置整数编号
        /// </summary>
        public int SessionIDi { get; set; }


        public ClientInfoBase(ClientInfoBase copythis)
        {
            Location = new Location(copythis.Location.FrontID, copythis.Location.ClientID);

            HardWareCode = copythis.HardWareCode;
            IPAddress = copythis.IPAddress;
            ProductInfo = copythis.ProductInfo;

            HeartBeat = copythis.HeartBeat;
            LoginID = copythis.LoginID;
            Authorized = copythis.Authorized;
            FrontType = copythis.FrontType;

            FrontIDi = copythis.FrontIDi;
            SessionIDi = copythis.SessionIDi;
        }

        public ClientInfoBase()
        {
            Location = new Location();

            HardWareCode = string.Empty;
            IPAddress = string.Empty;
            ProductInfo = string.Empty;
            HeartBeat = DateTime.Now;
            LoginID = string.Empty;
            Authorized = false;
            FrontType = QSEnumFrontType.Unknown;
            FrontIDi = 0;
            SessionIDi = 0;
        }

        /// <summary>
        /// 初始化客户端信息 用于记录客户端地址
        /// </summary>
        /// <param name="frontid"></param>
        /// <param name="clientid"></param>
        public void Init(string frontid, string clientid)
        {
            Location.FrontID = frontid;
            Location.ClientID = clientid;
            FrontType = ClientInfoBase.GetFrontType(frontid);
        }

        public ClientInfoBase(string frontid, string clientid)
        {
            Location = new Location(frontid, clientid);

            HardWareCode = string.Empty;
            IPAddress = string.Empty;
            ProductInfo = string.Empty;

            HeartBeat = DateTime.Now;
            LoginID = string.Empty;
            Authorized = false;
        }

        /// <summary>
        /// 通过前置编号获得前置类型
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        public static QSEnumFrontType GetFrontType(string frontid)
        {
            //如果前置ID为空或者null则为直连类型
            if (string.IsNullOrEmpty(frontid))
            {
                return QSEnumFrontType.Direct;
            }
            else if (frontid.ToUpper().StartsWith("EV-"))
            {
                return QSEnumFrontType.EVAccess;
            }
            else if(frontid.ToUpper().StartsWith("FRONT-"))
            {
                return QSEnumFrontType.FastAccess;
            }
            else
            {
                return QSEnumFrontType.Unknown;
            }
        }

        /// <summary>
        /// 授权某个用户
        /// </summary>
        public virtual void AuthorizedSuccess()
        {
            this.Authorized = true;
        }
        /// <summary>
        /// 不授权某个用户
        /// </summary>
        public virtual void AuthorizedFail()
        {
            this.Authorized = false;
        }


        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(Location.ClientID);
            sb.Append(d);
            sb.Append(Location.FrontID);
            sb.Append(d);
            sb.Append(HardWareCode);
            sb.Append(d);
            sb.Append(IPAddress);
            sb.Append(d);
            sb.Append(ProductInfo);
            sb.Append(d);
            sb.Append(HeartBeat.ToString());
            sb.Append(d);
            sb.Append(LoginID.ToString());
            sb.Append(d);
            sb.Append(Authorized.ToString());
            sb.Append(d);
            sb.Append(FrontType.ToString());
            sb.Append(d);
            sb.Append(FrontIDi.ToString());
            sb.Append(d);
            sb.Append(SessionIDi.ToString());
            sb.Append('|');
            sb.Append(SubSerialize());
            return sb.ToString();

        }



        public void Deserialize(string str)
        {
            string[] r = str.Split('|');
            if (r.Length == 2)
            {
                string[] rec = r[0].Split(',');
                if (rec.Length == 11)
                {
                    Location.ClientID = rec[0];
                    Location.FrontID = rec[1];
                    HardWareCode = rec[2];
                    IPAddress = rec[3];
                    ProductInfo = rec[4];
                    HeartBeat = DateTime.Parse(rec[5]);
                    LoginID = rec[6];
                    Authorized = bool.Parse(rec[7]);
                    FrontType = (QSEnumFrontType)Enum.Parse(typeof(QSEnumFrontType), rec[8]);
                    FrontIDi = int.Parse(rec[9]);
                    SessionIDi = int.Parse(rec[10]);
                }
                this.SubDeserialize(r[1]);
                
            }
        }


        public virtual string SubSerialize()
        {
            return string.Empty;
        }

        public virtual void SubDeserialize(string str)
        {

        }


        
    }
}
