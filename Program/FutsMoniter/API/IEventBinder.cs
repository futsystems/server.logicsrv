using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IEventBinder
    {
        /// <summary>
        /// 用于响应初始化结束事件
        /// 当管理员成功登入系统并且加载基础数据完毕后,调用注册逐渐的初始化结束响应函数
        /// 预加载组件在系统初始化之前便生成了界面元素，但是没有正常加载基础数据，这里用于通知对应的组件访问基础数据进行初始化
        /// 
        /// 一般情况下OnInitFinished 在初始化完成之后 会调用RegEvent进行事件延期注册 这里可能还包括界面调整等一些设置
        /// </summary>
        //void OnInitFinished();

        /// <summary>
        /// 订阅事件，将控件的响应函数注册到对应的事件上去，用于响应特定回报并处理
        /// 这里需要CallBackCentre回调中心完成初始化后才可以进行
        /// 在界面卡发过程中由于注册事件放在构造函数中会造成在界面编辑过程中出现Globls.CallBackCentre引用为空的错误这里需要进行不为空的判断
        /// </summary>
        void OnInit();

        /// <summary>
        /// 注销事件订阅
        /// </summary>
        void OnDisposed();
    }
}
