using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TradingLib.API;

namespace TradingLib.MoniterControl
{
    /// <summary>
    /// 管理端菜单操作
    /// 用于定义命令对象 然后动态的加载到菜单项目中
    /// 这里需要定义特性进行标注，从而实现可以将菜单加载到不同的地方
    /// </summary>

    public abstract class MoniterCommand : IMonterCommand
    {
        public MoniterCommand()
        { 
            
        }



        /// <summary>
        /// 用于响应系统菜单操作
        /// 菜单点击事件与该命令响应函数绑定，因此可以实现在扩展插件中定义操作响应函数
        /// 弹出对话框然后进行与服务端的交互
        /// 这样可以将系统的非相关业务进行剥离,然后动态的生成管理端界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnCommand(object sender, EventArgs e)
        {
            MessageBox.Show("HelloWorld Moniter");
        }
    }
}
