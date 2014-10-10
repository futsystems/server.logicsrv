using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 指标接口
    /// </summary>
    public interface IIndicator : ISeries
    {
        /// <summary>
        /// 运算,每个Tick到达时均要运行一次运算
        /// </summary>
        void Caculate();
        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        
        /// <summary>
        /// 绑定计算用到的参数
        /// </summary>
        /// <param name="inputs"></param>
        void SetInputs(params ISeries[] inputs);

        /// <summary>
        /// 获得计算用到的参数
        /// </summary>
        /// <returns></returns>
        ISeries[] GetInputs();


    }
}
