using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutsMoniter.Common;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    partial class MainForm
    {

        public void Login(string server, string username, string pass)
        {
            _servers = server;
            //在后台线程执行客户端tlclient初始化
            new Thread(InitClient).Start();

            string s = ".";
            DateTime now = DateTime.Now;
            while (!_connected && (DateTime.Now - now).TotalSeconds < 5)
            {
                ShowInfo("连接中" + s);
                Thread.Sleep(500);
                s += ".";
            }
            if (!_connected)
            {
                ShowInfo("连接超时,无法连接到服务器");
                Globals.LoginStatus.SetInitMessage("连接超时,无法连接到服务器");
                return;
            }
            else //如果连接服务器成功,则我们请求登入系统
            {
                debug("请求服务端登入");
                tlclient.ReqLogin(username, pass);
            }
            s = ".";
            now = DateTime.Now;
            while (!_gotloginrep && (DateTime.Now - now).TotalSeconds < 5)
            {
                ShowInfo("登入中" + s);
                Thread.Sleep(500);
                s += ".";
            }
            if (!_gotloginrep)
            {
                ShowInfo("登入超时,无法登入到服务器");
                Globals.LoginStatus.SetInitMessage("登入超时,无法登入到服务器");
                return;
            }
            else
            {
                if (_logined)
                {
                    ShowInfo("登入成功,请求下载帐户列表");

                    //查询基础数据
                    Globals.TLClient.ReqQryMarketTime();

                    s = ".";
                    now = DateTime.Now;
                    while (!_basicinfodone && (DateTime.Now - now).TotalSeconds < 10)
                    {
                        ShowInfo("查询基本数据中" + s);
                        Thread.Sleep(500);
                        s += ".";
                    }
                    //_basicinfodone由帐户加载过程中通过最后一个帐户信息带有的Islast进行设定
                    if (_basicinfodone)
                    {
                        if (Globals.Manager == null)
                        {
                            ShowInfo("柜员数据获取异常,请重新登入!");
                            Globals.LoginStatus.SetInitMessage("加载基础数据失败");

                        }
                        else
                        {
                            this.Text = string.Format("柜台系统-{0}    登入名:{1}    姓名:{2}    柜员类别:{3}", Globals.LoginResponse.Domain.Name, Globals.LoginResponse.LoginID, Globals.LoginResponse.Name, Util.GetEnumDescription(Globals.LoginResponse.ManagerType));// Globals.Config["CopName"].AsString() + " " + Globals.Config["Version"].AsString() + "           柜员用户名:" + Globals.Manager.Login + " 名称:" + Globals.Manager.Name + " 类型:" + Util.GetEnumDescription(Globals.Manager.Type);

                            //如果不是总平台柜员 隐藏
                            ShowInfo("初始化行情报表");
                            InitSymbol2View();

                            //触发初始化完成事件
                            Globals.OnInitFinished();

                            //设定全局登入状态 显示主界面
                            Globals.LoginStatus.IsInitSuccess = true;

                            

                        }
                    }
                    else
                    {
                        ShowInfo("加载基础数据失败!");
                        Globals.LoginStatus.SetInitMessage("加载基础数据失败");
                    }
                }
                else
                {
                    ShowInfo("登入失败!");
                    Globals.LoginStatus.SetInitMessage("登入失败");
                    return;
                }
            }

        }


        void InitClient()
        {
            tlclient = new TLClientNet(new string[] { _servers }, 6670, true);
            tlclient.OnDebugEvent += new DebugDelegate(debug);
            tlclient.DebugLevel = QSEnumDebugLevel.DEBUG;
            tlclient.OnConnectEvent += new VoidDelegate(tlclient_OnConnectEvent);
            tlclient.OnDisconnectEvent += new VoidDelegate(tlclient_OnDisconnectEvent);
            tlclient.OnLoginEvent += new RspMGRLoginResponseDel(tlclient_OnLoginEvent);
            tlclient.BindLogicHandler(this);
            Globals.RegisterClient(tlclient);
            _gotloginrep = false;
            _logined = false;
            _connected = false;
            tlclient.Start();

        }


        void tlclient_OnLoginEvent(RspMGRLoginResponse response)
        {
            debug("获得登入回报:" + response.ToString());
            _gotloginrep = true;
            if (response.LoginResponse.Authorized)
            {
                //登入的时候会的mgr_fk用于获得对应的编号
                //如果是代理 则 通过mgr_fk获得对应的Manager对象
                //如果是代理的员工 则服务端只会返回该员工的编号
                _logined = true;
                Globals.LoginResponse = response.LoginResponse;
                Globals.MGRID = response.LoginResponse.MGRID;//保存管理端登入获得的全局ID用于获取Manager列表时 绑定对应的Manager
                Globals.BaseMGRFK = response.LoginResponse.BaseMGRFK;
            }
            else
            {
                _logined = false;
            }
        }

        void tlclient_OnDisconnectEvent()
        {
            _connected = false;
        }


        void tlclient_OnConnectEvent()
        {
            _connected = true;
        }





    }
}
