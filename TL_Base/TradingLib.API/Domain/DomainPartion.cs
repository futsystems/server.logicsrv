using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 按域分区接口 实现了该接口的对象 表明需要按域来进行分区
    /// </summary>
    public interface IDomainPartition
    {
        Domain Domain { get; set; }
    }
}
