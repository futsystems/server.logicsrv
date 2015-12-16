using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface ICore:IDisposable
    {
        string CoreId { get; }

        void Start();

        void Stop();

    }
}
