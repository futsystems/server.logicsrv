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
            webUpdateAccount.Click += new EventHandler(webUpdateAccount_Click);
            webDeposit.Click += new EventHandler(webDeposit_Click);
            webWitdhdraw.Click += new EventHandler(webWitdhdraw_Click);
            webQueryUser.Click += new EventHandler(webQueryUser_Click);
        }

        void webQueryUser_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);
            string ret = client.ReqQueryUser(web_domainId.Text, webupdateacc_acc.Text);
            logger.Info("resoult:" + ret);
        }

        void webWitdhdraw_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);
            string ret = client.ReqWithdraw(web_domainId.Text, webupdateacc_acc.Text, webAmount.Text);
            logger.Info("resoult:" + ret);
        }

        void webDeposit_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);
            string ret = client.ReqDeposit(web_domainId.Text, webupdateacc_acc.Text, webAmount.Text);
            logger.Info("resoult:" + ret);
        }

        void webUpdateAccount_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);
            string ret = client.ReqUpdateAccount(web_domainId.Text, webupdateacc_acc.Text, webupdateacc_name.Text, webupdateacc_qq.Text, webupdateacc_mobile.Text, webupdateacc_idcard.Text, webupdateacc_bank.Text, webupdateacc_branch.Text, webupdateacc_bankac.Text);
            logger.Info("resoult:" + ret);
        }



        void webAddAccount_Click(object sender, EventArgs e)
        {
            HttpAPIClient client = new HttpAPIClient(web_url.Text, md5key.Text);


            string ret = client.ReqAddAccount(web_domainId.Text, web_userID.Text, web_agentID.Text, web_currency.Text);
            logger.Info("resoult:" + ret);
        }
    }
}
