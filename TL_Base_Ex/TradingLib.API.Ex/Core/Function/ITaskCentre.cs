﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface ITaskCentre
    {
        /// <summary>
        /// 注册Task
        /// </summary>
        /// <param name="task"></param>
        void RegisterTask(ITask task);


        /// <summary>
        /// 通过UUID获得对应的Task
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        ITask this[string uuid] { get; }


    }
}