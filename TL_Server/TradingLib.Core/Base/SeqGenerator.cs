using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Core
{
    /// <summary>
    /// 递增序号发生器
    /// </summary>
    public class SeqGenerator
    {
        /// <summary>
        /// 当前值
        /// </summary>
        int _currentSeq;
        public int CurrentSeq { get { return _currentSeq; } }
        /// <summary>
        /// 起步值
        /// </summary>
        int _startSeq;
        /// <summary>
        /// 步长最小
        /// </summary>
        int _stepMin;
        /// <summary>
        /// 步长最大
        /// </summary>
        int _stepMax;
        /// <summary>
        /// 是否使用随机递增
        /// </summary>
        bool _enableRandom;

        Random _random = new Random();

        public SeqGenerator(int startSeq, int stepMin, int stepMax, bool random, int currentSeq)
        {
            _startSeq = startSeq;
            _stepMin = stepMin;
            _stepMax = stepMax;
            _enableRandom = random;
            _currentSeq = currentSeq>_startSeq?currentSeq:_startSeq;
        }


        object _seqObj = new object();
        public int NextSeq
        {
            get
            {
                lock (_seqObj)
                {

                    if (_enableRandom)
                    {
                        _currentSeq += _random.Next(_stepMin, _stepMax);
                        return _currentSeq;
                    }
                    else
                    {
                        _currentSeq += 1;
                        return _currentSeq;
                    }
                }
            }
        
        }



    }
}
