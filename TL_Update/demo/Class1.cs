using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo
{
    public class LICENSE
    {
        // Properties
        public string Company { get; set; }

        public string EMail {get;set;}
       

        public int MaxConnections {get;set;}
      

        public DateTime ValidDate {get;set;}

        public static LICENSE GetLicense()
        {
            LICENSE a = new LICENSE();

            string[] strArray = new string[] { "company", "test@test.com", "100", "20150101010101" };

            a.Company = strArray[0];
            a.EMail = strArray[1];
            a.MaxConnections = int.Parse(strArray[2]);
            a.ValidDate = new DateTime(long.Parse(strArray[3]));

            a.ValidDate = new DateTime(667411488000000000);


            return a;

        }
    }
}
