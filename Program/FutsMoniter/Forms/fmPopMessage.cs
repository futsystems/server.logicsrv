using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class fmPopMessage : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        Timer _timer = new Timer();
        public fmPopMessage()
        {
            InitializeComponent();
            ResetMessage();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = 200;
            this.FormClosing += new FormClosingEventHandler(fmPopMessage_FormClosing);
        }

        void ResetMessage()
        {
            title.Text = "标题";
            message.Text = "内容";
            picbox.Image = null;
        }

        void fmPopMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        int num = 0;
        int totalnum = 20;
        void _timer_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.05;
            //到达计数隐藏窗口
            if (num == totalnum)
            {
                _timer.Stop();
                this.Hide();
                ResetMessage();
            }
            num++;
        }



        /// <summary>
        /// 设定要显示的消息 绑定标题和内容区域
        /// </summary>
        /// <param name="info"></param>
        public void SetMessage(RspInfo info)
        {
            title.Text = info.ErrorID == 0 ? "操作成功" : "操作失败(" + info.ErrorID.ToString() + ")";
            message.Text = info.ErrorMessage;
            picbox.Image = info.ErrorID == 0 ? Properties.Resources.message_success : Properties.Resources.message_error;
        }

        public void PopMessage(RspInfo info=null)
        {
            if (info != null)
                this.SetMessage(info);
            //初始化状态
            this.Opacity = 1;
            num = 0;
            //显示窗口
            this.Show();
            //定时开始
            _timer.Start();
        }



    }
}
