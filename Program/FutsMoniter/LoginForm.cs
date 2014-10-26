using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TradingLib.API;

using FutsMoniter.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class LoginForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public event ServerLoginDel ServerLoginEvent;
        public event VoidDelegate ResetEvent;

        Starter mStart;
        public LoginForm(Starter starter)
        {
            InitializeComponent();

            mStart = starter;
            btnLogin.Enabled = false;


            if (Globals.Config["HeaderImg"].AsString() == "OEM")
            {
                imageheader.Image = Properties.Resources.header_oem;
            }
            

           
            string[] addresses = Globals.Config["Servers"].AsString().Split(',');
            foreach (string s in addresses)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                servers.Items.Add(s);
            }
            servers.SelectedIndex = 0;

            if (addresses.Length == 1)
            {
                label1.Location = new Point(label1.Location.X, label1.Location.Y - 20);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - 20);

                username.Location = new Point(username.Location.X, username.Location.Y - 20);
                password.Location = new Point(password.Location.X, password.Location.Y - 20);

                //btnLogin.Location = new Point(btnLogin.Location.X, btnLogin.Location.Y + 11);
                //btnExit.Location = new Point(btnExit.Location.X, btnExit.Location.Y + 11);


                //隐藏服务端选择
                label0.Visible = false;
                servers.Visible = false;
                servers.SelectedIndex = 0;

            }

            ckremberuser.Checked = Properties.Settings.Default.remberuser;
            ckremberpass.Checked = Properties.Settings.Default.remberpass;
            if (Properties.Settings.Default.remberuser)
            {
                username.Text = Properties.Settings.Default.login;
            }
            if (Properties.Settings.Default.remberpass)
            {
                password.Text = Properties.Settings.Default.pass;
            }

            
            InitBW();
            
            WireEvent();
        }

        void WireEvent()
        { 
            btnLogin.Click +=new EventHandler(btnLogin_Click);
            btnExit.Click += new EventHandler(btnExit_LinkClicked);
        }


        public void ShowMessage(string msg)
        {
            if (message.InvokeRequired)
            {
                Invoke(new DebugDelegate(ShowMessage), new object[] { msg });
            }
            else
            {
                message.Text = msg;
            }
        }

        private void btnExit_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.remberuser = ckremberuser.Checked;
            Properties.Settings.Default.remberpass = ckremberpass.Checked;

            if (ckremberuser.Checked)
            {
                Properties.Settings.Default.login = username.Text;
            }

            if (ckremberpass.Checked)
            {
                Properties.Settings.Default.pass = password.Text;
            }
            else
            {
                Properties.Settings.Default.pass = string.Empty;
            }
            Properties.Settings.Default.Save();

            string srvaddress = servers.SelectedItem.ToString();
            Globals.LoginStatus.Reset();
            new Thread(delegate() {
                if (ServerLoginEvent != null)
                {
                    ServerLoginEvent(srvaddress, username.Text, password.Text);
                }
            }).Start();
            this.btnLogin.Enabled = false;
           
        }
        

        
        public void EnableLogin()
        {
            btnLogin.Enabled = true;
        }

        void Reset()
        {
            if (ResetEvent != null)
                ResetEvent();
            Globals.LoginStatus.IsInitSuccess = false;
            Globals.LoginStatus.IsReported = false;
            Globals.LoginStatus.needReport = false;
        }

        //后台工作线程,用于检查信息并做弹出窗口
        #region 线程内处理消息并触发显示
        /// <summary>
        /// 登入窗口建立线程循环检查全局变量
        /// 登入窗口点击按钮,启动后台登入线程,后台登入线程动态的更新登入窗口的界面,同时将登入的消息实时的写入到系统中
        /// 如果登入窗口的线程检测到该信息,则执行弹窗提醒或者是进入主界面
        /// </summary>
        private System.ComponentModel.BackgroundWorker bg;
        //private PopMessage pmsg = new PopMessage();
        private void bgDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                if (Globals.LoginStatus.IsInitSuccess && !Globals.LoginStatus.IsReported)
                {
                    Globals.LoginStatus.IsReported = true;
                    mStart.CloseSplashScreen();
                }
                else if (!Globals.LoginStatus.IsReported && Globals.LoginStatus.needReport)//需要报告并且没有报告过
                {
                    //如果登入过程中遇到异常则会进入这个流程
                    bg.ReportProgress(1);
                    Globals.LoginStatus.IsReported = true;
                    Globals.LoginStatus.needReport = false;
                    //清理内存 然后激活登入按钮 重新登入
                    this.btnLogin.Enabled = true;
                    if (ResetEvent != null)
                        ResetEvent();
                }
                Thread.Sleep(50);
            }
        }
        //启动后台工作进程 用于检查信息并调用弹出窗口
        private void InitBW()
        {
            bg = new System.ComponentModel.BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.DoWork += new System.ComponentModel.DoWorkEventHandler(bgDoWork);
            bg.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bg_ProgressChanged);
            bg.RunWorkerAsync();
        }
        //显示服务端返回过来的信息窗口
        private void bg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //MessageBox.Show(Globals.LoginStatus.InitMessage);
            fmConfirm.Show(Globals.LoginStatus.InitMessage);
        }
        #endregion



        private bool m_isMouseDown = false;
        private Point m_mousePos = new Point();
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_mousePos = Cursor.Position;
            m_isMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            m_isMouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (m_isMouseDown)
            {
                Point tempPos = Cursor.Position;
                this.Location = new Point(Location.X + (tempPos.X - m_mousePos.X), Location.Y + (tempPos.Y - m_mousePos.Y));
                m_mousePos = Cursor.Position;
            }
        }

        private void imageheader_MouseDown(object sender, MouseEventArgs e)
        {
            m_mousePos = Cursor.Position;
            m_isMouseDown = true;
        }

        private void imageheader_MouseUp(object sender, MouseEventArgs e)
        {
            m_isMouseDown = false;
        }

        private void imageheader_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isMouseDown)
            {
                Point tempPos = Cursor.Position;
                this.Location = new Point(Location.X + (tempPos.X - m_mousePos.X), Location.Y + (tempPos.Y - m_mousePos.Y));
                m_mousePos = Cursor.Position;
            }
        }

    }
}
