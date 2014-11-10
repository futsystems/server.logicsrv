﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter.Common
{
    public static class ManagerUtils
    {
        public static int GetBaseMGR(this Manager mgr)
        {
            return mgr.mgr_fk;
        }

        /// <summary>
        /// 判断当前管理员是否是超级管理员
        /// </summary>
        /// <returns></returns>
        public static bool RightRootDomain(this Manager mgr)
        {
            return mgr.Type == QSEnumManagerType.ROOT;
        
        }

    }
}