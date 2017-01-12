//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    public class MGRUpdateCategoryRequest:RequestPacket
//    {
//        public MGRUpdateCategoryRequest()
//        {
//            _type = MessageTypes.MGRUPDATEACCOUNTCATEGORY;
//            this.Account = string.Empty;
//            this.Category = QSEnumAccountCategory.SUBACCOUNT;
//        }


//        public string Account { get; set; }
//        public QSEnumAccountCategory Category { get; set; }

//        public override string ContentSerialize()
//        {
//            StringBuilder sb = new StringBuilder();
//            char d = ',';
//            sb.Append(this.Account);
//            sb.Append(d);
//            sb.Append(this.Category.ToString());
//            return sb.ToString();
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            string[] rec = contentstr.Split(',');
//            this.Account = rec[0];
//            this.Category = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), rec[1]); 
//        }
//    }
//}
