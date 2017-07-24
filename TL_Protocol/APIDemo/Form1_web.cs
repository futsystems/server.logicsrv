using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace APIClient
{
    partial class Form1
    {


       

        void WireEvent_Web()
        {
            webAddAccount.Click += new EventHandler(webAddAccount_Click);
        }



        void webAddAccount_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);


            string ret = client.ReqAddAccount(web_domainId.Text, web_userID.Text, web_agentID.Text, web_currency.Text);
            logger.Info("resoult:" + ret);
        }
    }
}
