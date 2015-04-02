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

    }
}
