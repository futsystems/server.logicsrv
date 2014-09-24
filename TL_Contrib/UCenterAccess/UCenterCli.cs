using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins;

namespace Lottoqq.UCenter
{
    internal class UCenterCli
    {

        string _ucaddress = "127.0.0.1";
        int _port = 9000;

        object _lock = new object();
        public UCenterCli(string address,int port)
        {
            _ucaddress = address;
            _port = port;
        
        }

        bool _reqgo = false;
        public bool IsLive { get { return _reqgo; } }
        /// <summary>
        /// 认证某个用户
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string AuthUser(string login, string pass)
        { 
            //string rstr = "authuser|"+login+","+pass;
            string rstr = JsonRequest.Request().SetMthod("authuser").SetArgs(new string[] { login, pass }).ToJson();
            return request(rstr);
        }
        public string RegisterUser(string uref, string pass)
        {
            string rstr = "registeruser|" + uref + "," + pass;
            return request(rstr);
        }


        /// <summary>
        /// 向某组客户端发送消息,该组客户端通过filter过滤条件进行过滤
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="notifyname"></param>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public string Broadcast(string filter, string notifyname, string jsonstr)
        {
            string rstr = JsonRequest.Request().SetMthod("sendpcmesssage").SetArgs(new string[] { filter, notifyname, jsonstr }).ToJson();
            return request(rstr);
        }

        //ZmqContext ctx;
        TimeSpan timeout = new TimeSpan(0, 0, 3);
        string timeoutreply = TradingLib.Mixins.JsonReply.GenericError(9999, "API调用超时").ToJson();
        string errorreply = TradingLib.Mixins.JsonReply.GenericError(9998, "服务端调用异常").ToJson();
        string request(string request)
        {
            //锁定_socket,防止多个线程同时进行请求访问

            string message = null;
            lock (reqsockt)
            {
                try
                {
                    reqsockt.Send(request, Encoding.UTF8);
                    message = reqsockt.Receive(Encoding.UTF8, timeout);
                    if (message == null)
                    {
                        
                        Console.WriteLine("Close bad req socket and init it");
                        reqsockt.Close();
                        reqsockt = ctx.CreateSocket(SocketType.REQ);
                        reqsockt.Connect(ReqAddress);
                        return timeoutreply;
                    }
                    else
                    {
                        return message;
                    }
                }
                catch (Exception ex)
                {

                    return errorreply;
                }
            }
        }

        string ReqAddress { get { return "tcp://" + _ucaddress + ":" + _port; } }
        ZmqContext ctx;
        ZmqSocket reqsockt;
        public void Init()
        {
            if (_reqgo) return;
            _reqgo = true;
            ctx = ZmqContext.Create();
            reqsockt = ctx.CreateSocket(SocketType.REQ);
            reqsockt.Connect(this.ReqAddress);
        }
    }
}
