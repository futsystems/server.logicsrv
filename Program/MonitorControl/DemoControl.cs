using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.MoniterControl
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
            this.Request("MgrExchServer", "HelloWorld", args.Text);
        }


        /// <summary>
        /// CallbackAttr标注 注册一个回调函数
        /// </summary>
        /// <param name="result"></param>
        [CallbackAttr("MgrExchServer", "HelloWorld")]
        public void OnHelloworld(string result)
        {
            ctDebug1.GotDebug(result);
        }
    }
}
