using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;

namespace FutsMoniter.Common
{
    public abstract class SplashScreenApplicationContext : ApplicationContext
    {
        private Form _SplashScreenForm;//启动窗体
        private Form _PrimaryForm;//主窗体
        private System.Timers.Timer _SplashScreenTimer;
        private int _SplashScreenTimerInterVal = 5000;//默认是启动窗体显示5秒
        private bool _bSplashScreenClosed = false;
        private delegate void DisposeDelegate();//关闭委托，下面需要使用控件的Invoke方法，该方法需要这个委托

        public SplashScreenApplicationContext()
        {
            //如果有更新则我们先进行更新 然后再重新启动
            if (!this.onUpdate())
            {
                this.ShowSplashScreen();//这里创建和显示启动窗体
                this.MainFormLoad();//这里创建和显示启动主窗体
            }
            else
            {
                //关闭本进程等待更新程序进行覆盖更新
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        protected abstract bool onUpdate();
        protected abstract void OnCreateSplashScreenForm();

        protected abstract void OnCreateMainForm();

        protected abstract void SetSeconds();

        protected Form SplashScreenForm
        {
            set
            {
                this._SplashScreenForm = value;
            }
        }

        protected Form PrimaryForm
        {//在派生类中重写OnCreateMainForm方法，在MainFormLoad方法中调用OnCreateMainForm方法
            //  ,在这里才会真正调用Form1(主窗体)的构造函数，即在启动窗体显示后再调用主窗体的构造函数
            //  ，以避免这种情况:主窗体构造所需时间较长,在屏幕上许久没有响应，看不到启动窗体       
            set
            {
                this._PrimaryForm = value;
            }
        }

        //未设置启动画面停留时间时，使用默认时间
        protected int SecondsShow
        {
            set
            {
                if (value != 0)
                {
                    this._SplashScreenTimerInterVal = 1000 * value;
                }
            }
        }

        private void ShowSplashScreen()
        {
            this.SetSeconds();//设定显示时间
            this.OnCreateSplashScreenForm();
            this._SplashScreenTimer = new System.Timers.Timer(((double)(this._SplashScreenTimerInterVal)));
            _SplashScreenTimer.Elapsed += new System.Timers.ElapsedEventHandler(new System.Timers.ElapsedEventHandler(this.SplashScreenDisplayTimeUp));

            this._SplashScreenTimer.AutoReset = false;
            Thread DisplaySpashScreenThread = new Thread(new ThreadStart(DisplaySplashScreen));
            //DisplaySpashScreenThread.SetApartmentState(ApartmentState.STA);
            DisplaySpashScreenThread.Start();
            //Application.Run(this._SplashScreenForm);
        }

        private void DisplaySplashScreen()
        {
            try
            {
                this._SplashScreenTimer.Enabled = true;
                Application.Run(this._SplashScreenForm);
            }
            catch (Exception ex)
            { 
                
            }
        }

        //方式2.将第一屏幕显示时间设置成无限长,则实现登入窗口,则登入的时候需要手工关闭第一屏
        public void CloseSplashScreen()
        {
            this._SplashScreenTimer.Dispose();
            this._SplashScreenTimer = null;
            this._bSplashScreenClosed = true;
        }

        //方式1.第一屏幕设定最小显示时间,时间过后关闭第一屏,然后显示主屏
        private void SplashScreenDisplayTimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._SplashScreenTimer.Dispose();
            this._SplashScreenTimer = null;
            this._bSplashScreenClosed = true;
        }

        private void MainFormLoad()
        {
            
            this.OnCreateMainForm();//如果启动时间很短,并且第一屏幕显示时间也很短，那么就直接显示主屏幕了因此这里需要设置第一屏显示时间
            while (!(this._bSplashScreenClosed))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            DisposeDelegate SplashScreenFormDisposeDelegate = new DisposeDelegate(this._SplashScreenForm.Dispose);
            this._SplashScreenForm.Invoke(SplashScreenFormDisposeDelegate);
            this._SplashScreenForm = null;


            //必须先显示，再激活，否则主窗体不能在启动窗体消失后出现
            this._PrimaryForm.Show();
            this._PrimaryForm.Activate();
            this._PrimaryForm.Closed += new EventHandler(_PrimaryForm_Closed);
        }

        private void _PrimaryForm_Closed(object sender, EventArgs e)
        {
            base.ExitThread();
        }
    }
    
}
