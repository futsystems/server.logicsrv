//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    public class MGRResumeAccountRequest:RequestPacket
//    {
//        public string ResumeAccount { get; set; }
//        public MGRResumeAccountRequest()
//        {
//            _type = MessageTypes.MGRRESUMEACCOUNT;
//            ResumeAccount = string.Empty;
//        }

//        public override string ContentSerialize()
//        {
//            return ResumeAccount;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            ResumeAccount = contentstr;
//        }
//    }

    
//    public class RspMGRResumeAccountResponse : RspResponsePacket
//    {
//        public string ResumeAccount { get; set; }
//        public QSEnumResumeStatus ResumeStatus { get; set; }
//        public RspMGRResumeAccountResponse()
//        {
//            _type = MessageTypes.MGRRESUMEACCOUNTRESPONE;
//            ResumeAccount = string.Empty;
//            ResumeStatus = QSEnumResumeStatus.BEGIN;
//        }

//        public bool IsBegin { get { return ResumeStatus == QSEnumResumeStatus.BEGIN; } }

//        public bool IsEnd { get { return ResumeStatus == QSEnumResumeStatus.END; } }
//        public override string ResponseSerialize()
//        {
//            StringBuilder sb = new StringBuilder();
//            char d = ',';
//            sb.Append(ResumeAccount);
//            sb.Append(d);
//            sb.Append(ResumeStatus.ToString());
//            return sb.ToString();
//        }

//        public override void ResponseDeserialize(string content)
//        {
//            string[] rec = content.Split(',');
//            ResumeAccount = rec[0];
//            ResumeStatus = (QSEnumResumeStatus)Enum.Parse(typeof(QSEnumResumeStatus), rec[1]);
//        }


//    }
//}
