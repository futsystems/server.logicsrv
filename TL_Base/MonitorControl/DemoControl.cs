﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradingLib.GUI
{
    public partial class DemoControl : MonitorControl
    {
        public DemoControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 界面按钮事件 触发后向服务端提交一个请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            this.Request("demo", "helloworld", args.Text);
        }

        [CallbackAttr("demo","helloworld")]
        void OnHelloworld(string result)
        {
            ctDebug1.GotDebug(result);
        }
    }
}