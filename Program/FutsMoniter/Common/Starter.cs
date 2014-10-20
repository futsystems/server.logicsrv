using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FutsMoniter.Common
{
    public class Starter : SplashScreenApplicationContext
    {
        LoginForm _loginform;
        MainForm mainfm;

        //用于调用升级逻辑,然后再显示启动窗口与主窗口
        protected override bool onUpdate()
        {
            //没有更新我们返回false 程序正常运行
            return false;

        }

        protected override void OnCreateSplashScreenForm()
        {
            _loginform = new LoginForm(this);
            this.SplashScreenForm = _loginform;//启动窗体
            _loginform.ServerLoginEvent += new ServerLoginDel(StartServerLogin);
            _loginform.ResetEvent += new TradingLib.API.VoidDelegate(_loginform_ResetEvent);

        }

        void _loginform_ResetEvent()
        {
            mainfm.Reset();
        }

        //执行登入
        void StartServerLogin(string server, string user, string pass)
        {
            //Globals.Debug("执行登入操作 Server:" + server);
            _loginform.ShowMessage("执行登入交易帐户");
            mainfm.Login(server, user, pass);

        }




        protected override void OnCreateMainForm()
        {
            //在线程中创建主窗体,防止登入界面卡顿
            //new Thread(delegate()
            //{
                _loginform.ShowMessage("初始化...");
                mainfm = new MainForm(_loginform.ShowMessage);
                this.PrimaryForm = mainfm;
                _loginform.EnableLogin();
                _loginform.ShowMessage("初始化完毕");

            //}).Start();
        }
        protected override void SetSeconds()
        {
            this.SecondsShow = 60 * 60;//启动窗体显示的时间(秒)
        }
    }


}
