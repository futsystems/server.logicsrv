//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
    
//    /// <summary>
//    /// 记录交易客户端通讯的信息
//    /// </summary>
//    public class ClientInfo : IClientInfo
//    {
//        public static QSEnumFrontType CheckFrontType(string frontid)
//        {
//            if (string.IsNullOrEmpty(frontid))
//                return QSEnumFrontType.Direct;
//            if (frontid.ToLower().StartsWith("web"))
//                return QSEnumFrontType.WebAccess;
//            if (frontid.ToLower().StartsWith("front"))
//                return QSEnumFrontType.FastAccess;
//            if (frontid.ToLower().StartsWith("ev"))
//                return QSEnumFrontType.EvAccess;
//            else
//                return QSEnumFrontType.Unknown;
//        }

//        /// <summary>
//        /// 用户全局UID
//        /// </summary>
//        public int UID { get; set; }

//        /// <summary>
//        /// 该客户端通过哪个前端注册上来
//        /// </summary>
//        public string FrontID { get; set; }
//        /// <summary>
//        /// 连接端硬件码
//        /// </summary>
//        public string HardWareCode { get; set; }
//        /// <summary>
//        /// 客户端的IP地址
//        /// </summary>
//        public string IPAddress { get; set; }

//        /// <summary>
//        /// 客户端通讯唯一编号,即通讯客户端的Address
//        /// </summary>
//        public string Address { get; set; }
//        /// <summary>
//        /// 客户端最近心跳时间
//        /// </summary>
//        public DateTime HeartBeat { get; set; }
//        /// <summary>
//        /// 客户端登入系统的用户ID
//        /// </summary>
//        public string AccountID { get; set; }

//        /// <summary>
//        /// 客户端登入ID
//        /// </summary>
//        public string LoginID { get; set; }


//        /// <summary>
//        /// 前置类别
//        /// </summary>
//        public QSEnumFrontType FrontType { get; set; }
//        /// <summary>
//        /// 监察该客户端是否登入
//        /// </summary>
//        public bool IsLoggedIn
//        {
//            get
//            {
//                if (AccountID == null)
//                    return false;
//                else
//                    return AccountID != string.Empty;
//            }
//        }
        
//        public ClientInfo(IClientInfo copythis)
//        {
//            Address = copythis.Address;
//            AccountID = copythis.AccountID;
//            HeartBeat = copythis.HeartBeat;
//            FrontID = copythis.FrontID;
//            FrontType = CheckFrontType(FrontID);
//        }
        

//    }
    

//    /// <summary>
//    /// 记录交易客户端通讯的信息
//    /// </summary>
//    public class TrdClientInfo:IClientInfo
//    {
//        /// <summary>
//        /// 用户全局UID
//        /// </summary>
//        public int UID { get; set; }
//        /// <summary>
//        /// 该客户端通过哪个前端注册上来
//        /// </summary>
//        public string FrontID { get; set; }
//        /// <summary>
//        /// 连接端硬件码
//        /// </summary>
//        public string HardWareCode { get; set; }
//        /// <summary>
//        /// 客户端的IP地址
//        /// </summary>
//        public string IPAddress { get; set; }
//        /// <summary>
//        /// 客户端通讯唯一编号,即通讯客户端的Address
//        /// </summary>
//        public string Address { get; set; }
//        /// <summary>
//        /// 客户端最近心跳时间
//        /// </summary>
//        public DateTime HeartBeat { get; set; }
//        /// <summary>
//        /// 客户端所绑定的交易ID
//        /// </summary>
//        public string AccountID { get; set; }

//        /// <summary>
//        /// 客户端登入ID
//        /// </summary>
//        public string LoginID { get; set; }
//        /// <summary>
//        /// 前置类别
//        /// </summary>
//        public QSEnumFrontType FrontType { get; set; }

//        /// <summary>
//        /// 监察该客户端是否登入
//        /// </summary>
//        public bool IsLoggedIn
//        {
//            get
//            {
//                if (AccountID == null)
//                    return false;
//                else
//                    return AccountID != string.Empty;
//            }
//        }
        
//        public TrdClientInfo(string clientid, string account, string stocks, DateTime heartbeat,string frontid="")
//        {
//            Address = clientid;
//            Stocks = stocks;
//            AccountID = account;
//            HeartBeat = heartbeat;
//            FrontID = frontid;
//            FrontType =  ClientInfo.CheckFrontType(FrontID);
//        }
//        public TrdClientInfo(TrdClientInfo copythis)
//        {
//            Address = copythis.Address;
//            Stocks = copythis.Stocks;
//            AccountID = copythis.AccountID;
//            HeartBeat = copythis.HeartBeat;
//            FrontID = copythis.FrontID;
//            FrontType = ClientInfo.CheckFrontType(FrontID);
            
//        }
//        public TrdClientInfo(string ID,string front="")
//        {
//            Address = ID;
//            FrontID = front;
//            FrontType = ClientInfo.CheckFrontType(FrontID);
//        }
        
//        public string Stocks {get;set;}//记录客户端请求的symbol数据集

//        public override string ToString()
//        {
//            string d="|";
//            return (FrontID != null ? FrontID : "") + d + (Address != null ? Address : "") + d + (AccountID != null ? AccountID : "") + d + (Stocks != null ? Stocks : "") + d + (HeartBeat != null ? HeartBeat.ToString() : "") + d + (IPAddress != null ? IPAddress.ToString() : "") + d + (HardWareCode != null ? HardWareCode.ToString() : "");
//        }

//        public static TrdClientInfo FromString(string session)
//        {
//            string[] p = session.Split('|');
//            if (p.Length < 5) return null;
//            TrdClientInfo info = new TrdClientInfo(p[1]);
//            info.FrontID = p[0];
//            info.AccountID = p[2];
//            info.Stocks = p[3];
//            info.HeartBeat = Convert.ToDateTime(p[4]);
//            info.IPAddress = p[5];
//            info.HardWareCode = p[6];
//            info.FrontType = ClientInfo.CheckFrontType(info.FrontID);
//            return info;
//        }
 
//    }

//    /// <summary>
//    /// 记录管理客户端通讯的信息包括交易信号接收端
//    /// </summary>
//    public class MgrClientInfo:IClientInfo
//    {
//        /// <summary>
//        /// 用户全局UID
//        /// </summary>
//        public int UID { get; set; }
//        /// <summary>
//        /// 该客户端通过哪个前端注册上来
//        /// </summary>
//        public string FrontID { get; set; }
//        /// <summary>
//        /// 连接端硬件码
//        /// </summary>
//        public string HardWareCode { get; set; }
//        /// <summary>
//        /// 客户端的IP地址
//        /// </summary>
//        public string IPAddress { get; set; }
//        //public string Accounts { get; set; }
//        //public QSEnumCompareType CustomerType { get; set; }
//        /// <summary>
//        /// 客户端通讯唯一编号,即通讯客户端的Address
//        /// </summary>
//        public string Address { get; set; }
//        /// <summary>
//        /// 客户端最近心跳时间
//        /// </summary>
//        public DateTime HeartBeat { get; set; }
//        /// <summary>
//        /// 客户端登入系统的用户ID
//        /// </summary>
//        public string AccountID { get; set; }

//        /// <summary>
//        /// 客户端登入ID
//        /// </summary>
//        public string LoginID { get; set; }
//        /// <summary>
//        /// 前置类别
//        /// </summary>
//        public QSEnumFrontType FrontType { get; set; }


//        public MgrClientInfo(string clientid, string customer, string accounts, DateTime heartbeat)
//        {
//            Address = clientid;//管理端标示
//            Accounts = accounts;//
//            AccountID = customer;
//            HeartBeat = heartbeat;
//            FrontType = QSEnumFrontType.Direct;
//        }
//        public MgrClientInfo(MgrClientInfo copythis)
//        {
//            Address = copythis.Address;
//            Accounts = copythis.Accounts;
//            AccountID = copythis.AccountID;
//            HeartBeat = copythis.HeartBeat;
//            FrontType = QSEnumFrontType.Direct;
//        }
//        public MgrClientInfo(string ID)
//        {
//            Address = ID;
//            FrontType = QSEnumFrontType.Direct;
//        }
       
//        public string Accounts = "";//记录客户端请求的symbol数据集
//        public QSEnumCustomerType CustomerType;
//        //检查客户端是否已经登入
//        public bool IsLoggedIn { get {
//            if (AccountID == null)
//                return false;
//            else
//                return AccountID != string.Empty;
//        } }

//        public override string ToString()
//        {
//            string d = "|";
//            return (Address != null ? Address : "") + d + (AccountID != null ? AccountID : "") + d + (HeartBeat != null ? HeartBeat.ToString() : "") + d + (IPAddress != null ? IPAddress.ToString() : "") + d + (HardWareCode != null ? HardWareCode.ToString() : "");
//        }

//        public static MgrClientInfo FromString(string session)
//        {
//            string[] p = session.Split('|');
//            if (p.Length < 5) return null;
//            MgrClientInfo info = new MgrClientInfo(p[0]);
//            info.AccountID = p[1];
//            info.HeartBeat = Convert.ToDateTime(p[2]);
//            info.IPAddress = p[3];
//            info.HardWareCode = p[4];
//            info.FrontType = QSEnumFrontType.Direct;
//            return info;
//        }
//    } 

//}
