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


namespace TradingLib.GUI
{
 
    /// <summary>
    /// 管理端动态控件的基类
    /// 其余控件需从该控件进行集成并获得对系统的相关调用
    /// </summary>
    public partial class MonitorControl : UserControl
    {
        public MonitorControl()
        {
            InitializeComponent();

            this.Log("moniter control init from here",QSEnumDebugLevel.DEBUG);

            List<HandlerInfo> infolist = GUIHelper.FindHandler(this);
            foreach (HandlerInfo info in infolist)
            {
                Log(string.Format("Handler module:{0} command:{1}", info.Attr.Module, info.Attr.Cmd),QSEnumDebugLevel.INFO);
            }

            this.Log("base control construct finished",QSEnumDebugLevel.DEBUG);

            
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Log("base control load",QSEnumDebugLevel.INFO);
        }



        IMGRClient _client = null;

        /// <summary>
        /// 注入管理端核心通讯组件
        /// 这样本
        /// </summary>
        /// <param name="client"></param>
        public void SetClient(IMGRClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="module">模块标识</param>
        /// <param name="cmd">命令标识</param>
        /// <param name="args">参数</param>
        protected void Request(string module, string cmd, string args)
        {
            if (_client != null)
            {
                _client.ReqContribRequest(module, cmd, args);
            }
            else
            {
                throw new NullReferenceException("MGRClient can not be null");
            }
        }

        /// <summary>
        /// 对外输出日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        protected void Log(string message, QSEnumDebugLevel level)
        {
            Util.Log(new LogItem(message, level, this.GetType().Name));
        }




    }
}
