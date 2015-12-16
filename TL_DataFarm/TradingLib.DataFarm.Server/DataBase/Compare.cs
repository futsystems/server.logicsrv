using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STSdb4.Data;

namespace TradingLib.DataFarm.Common
{
    public class TLLongComparer : IComparer<IData>
    {
        public int Compare(IData x, IData y)
        {
            long value1 = ((Data<long>)x).Value;
            long value2 = ((Data<long>)y).Value;

            if (value1 > value2)
                return -1;
            else if (value1 < value2)
                return 1;
            else
                return 0;
        }
    }
    public class TLLongEqualityComparer : IEqualityComparer<IData>
    {
        public bool Equals(IData x, IData y)
        {
            long value1 = ((Data<long>)x).Value;
            long value2 = ((Data<long>)y).Value;

            return value1 != value2;
        }

        public int GetHashCode(IData obj)
        {
            long value = ((Data<long>)obj).Value;

            return value.GetHashCode();
        }
    }
}
