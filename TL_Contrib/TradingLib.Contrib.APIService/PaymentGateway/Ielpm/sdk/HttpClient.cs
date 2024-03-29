﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk;

namespace ielpm_merchant_web_demo.com.ielpm.merchant.web.sdk
{
    public class HttpClient
    {
        // 请求的URL
        private string requestUrl = "";

        // 返回结果
        private string result;

        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        public HttpClient(string url)
        {
            requestUrl = url;
        }
        /// <summary>
        /// 建立请求，以模拟远程HTTP的POST请求方式构造并获取银联的处理结果
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <returns>银联处理结果</returns>
        public int Send(SortedDictionary<string, string> sParaTemp, Encoding encoder)
        {
            string strRequestData = IELPMUtil.CreateLinkString(sParaTemp, true, true);
            //把数组转换成流中所需字节数组类型
            byte[] bytesRequestData = encoder.GetBytes(strRequestData);
            HttpWebResponse HttpWResp = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //设置HttpWebRequest基本信息
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
                myReq.Method = "post";
                myReq.ContentType = "application/x-www-form-urlencoded";
                //填充POST数据
                myReq.ContentLength = bytesRequestData.Length;
                Stream requestStream = myReq.GetRequestStream();  //获得请求流
                requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                requestStream.Close();
                //发送POST数据请求服务器                
                HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                //获取服务器返回信息
                StreamReader reader = new StreamReader(myStream, encoder);
                result = reader.ReadToEnd();
                //释放
                myStream.Close();

                return (int)HttpWResp.StatusCode;
            }
            catch (Exception exp)
            {
                result = "报错：" + exp.Message;
                return 0;
            }

        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }

    }
}