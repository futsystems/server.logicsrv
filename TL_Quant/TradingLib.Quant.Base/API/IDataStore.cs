using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 数据访问插件接口,实现IDataStore即可通过plugin调用在系统内使用该插件进行数据访问
    /// </summary>
    public interface IDataStore
    {

        // Methods
        void FlushAll();
        IDataAccessor<Bar> GetBarStorage(SecurityFreq symbol);
        IDataAccessor<Tick> GetTickStorage(Security symbol);
    }

    






}
