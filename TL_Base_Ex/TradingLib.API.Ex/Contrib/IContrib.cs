using System;

namespace TradingLib.API
{
    /// <summary>
    /// 外围功能插件接口
    /// </summary>
    public interface IContrib
    {
        /// <summary>
        /// 加载
        /// </summary>
        void OnLoad(); 
        /// <summary>
        /// 销毁
        /// </summary>
        void OnDestory(); 
        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }


}
