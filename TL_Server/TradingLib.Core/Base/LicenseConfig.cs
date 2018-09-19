using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Core
{
    public class LicenseConfig
    {

        static LicenseConfig instance;

        public static LicenseConfig Instance { get { return instance; } }


        static LicenseConfig()
        {
            if (instance == null)
            {
                instance = new LicenseConfig();
            }
        }

        private LicenseConfig()
        {
            //this.UseLicense = false;
            this.Deploy = "XXXX";
            this.Expire = DateTime.Now;
            this.AccountCNT = 100;
            this.DomainCNT = 2;
            this.AgentCNT = 10;
            this.EnableAPI = false;
            this.EnableAPP = false;
        }

        public string Deploy { get; set; }

        public DateTime Expire { get; set; }

        public int AccountCNT { get; set; }

        public int DomainCNT { get; set; }

        public int AgentCNT { get; set; }

        public bool EnableAPI { get; set; }

        public bool EnableAPP { get; set; }

        public bool IsExpired()
        {
            return this.Expire.Subtract(DateTime.Now).TotalDays < 0;
        }
      
    }
}
