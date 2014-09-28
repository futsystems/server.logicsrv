using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 管理端登入请求
    /// </summary>
    public class MGRLoginRequest:RequestPacket
    {
        public string LoginID { get; set; }
        public string Passwd { get; set; }

        public MGRLoginRequest()
        {
            _type = MessageTypes.MGRLOGINREQUEST;
        }


        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.LoginID);
            sb.Append(d);
            sb.Append(this.Passwd);
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            LoginID = rec[0];
            Passwd = rec[1];
        }
    }

    public class RspMGRLoginResponse : RspResponsePacket
    {
        /// <summary>
        /// 登入ID
        /// </summary>
        public string LoginID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 管理员类别
        /// </summary>
        public QSEnumManagerType ManagerType { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 是否认证通过
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int mgr_fk { get; set; }


        public RspMGRLoginResponse()
        {
            _type = MessageTypes.MGRLOGINRESPONSE;
            LoginID = string.Empty;
            Name = string.Empty;
            ManagerType = QSEnumManagerType.MONITER;
            Mobile = string.Empty;
            QQ = string.Empty;
            mgr_fk = 0;
        }

        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.LoginID);
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this.ManagerType.ToString());
            sb.Append(d);
            sb.Append(this.Mobile);
            sb.Append(d);
            sb.Append(this.QQ);
            sb.Append(d);
            sb.Append(this.Authorized.ToString());
            sb.Append(d);
            sb.Append(this.mgr_fk.ToString());
            return sb.ToString();
            
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split(',');
            this.LoginID = rec[0];
            this.Name = rec[1];
            this.ManagerType = (QSEnumManagerType)Enum.Parse(typeof(QSEnumManagerType), rec[2]);
            this.Mobile = rec[3];
            this.QQ = rec[4];
            this.Authorized = bool.Parse(rec[5]);
            this.mgr_fk = int.Parse(rec[6]);
        }
    }
}
