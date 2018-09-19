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
            this.Deploy = "TEST";
            this.Expire = DateTime.Now.AddDays(1);
            this.AccountCNT = 10;
            this.DomainCNT = 2;
            this.AgentCNT = 2;
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
    }
}
