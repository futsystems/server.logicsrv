using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class PositionFlatEventArgs:EventArgs
    {
        /// <summary>
        /// 持仓对象
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 附带信息
        /// </summary>
        public RspInfo RspInfo { get; set; }

        public PositionFlatEventArgs(Position pos)
        {
            this.Position = pos;
            this.RspInfo = new RspInfoImpl();
        }

        public PositionFlatEventArgs(Position pos, string errorkey)
        {
            this.Position = pos;
            this.RspInfo = RspInfoEx.Fill(errorkey);
        }

        public bool FlatSuccess
        {
            get
            {
                return this.RspInfo.ErrorID == 0;
            }
        }
    }
}
