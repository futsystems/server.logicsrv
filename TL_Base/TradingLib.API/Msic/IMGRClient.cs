using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 管理端底层通讯接口
    /// 用于解除具体实现的依赖，将管理端底层接口注入到相关控件中
    /// 控件就可以进行相关操作 比如向服务端提交请求
    /// 
    /// </summary>
    public interface IMGRClient
    {
        /// <summary>
        /// 提交扩展命令请求
        /// 
        /// 服务端相关模块对函数进行标记 然后通过管理端直接进行扩展命令请求 就可以调用该函数
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="cmd">命令</param>
        /// <param name="args">参数</param>
        void ReqContribRequest(string module, string cmd, string args);

        /// <summary>
        /// 注册请求回调函数
        /// 用于向系统回调中心 注册一个回调函数
        /// 
        /// </summary>
        /// <param name="module">回调函数用于处理的模块</param>
        /// <param name="cmd">回调函数用于响应的命理</param>
        /// <param name="result">执行命令所返回的结果</param>
        /// <param name="islast">返回结束标识</param>
        void RegisterCallback(string module,string cmd,Action<string,bool> handler);

        /// <summary>
        /// 注册通知回调函数
        /// 
        /// 服务端向管理端发送通知，通过向系统回调中心 注册一个回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        void RegisterNotifyCallback(string module, string cmd, Action<string> handler);

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void UnRegisterCallback(string module,string cmd,Action<string,bool> handler);

        /// <summary>
        /// 注销 通知回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void UnRegisterNotifyCallback(string module, string cmd, Action<string> handler);


        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="leve"></param>
        void Log(string message, QSEnumDebugLevel leve);

    }
}
