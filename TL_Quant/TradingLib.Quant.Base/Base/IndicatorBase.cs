using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Quant.Base;
namespace TradingLib.Quant.Base
{
    //策略体系中定义了数据驱动 由哪个个数据驱动了该策略 tick数据驱动了公式的计算
    [Serializable]
    public abstract class IndicatorBase : MarshalByRefObject,IIndicator
    {
        public event DebugDelegate SendDebugEvent;

        public ISeriesChartSettings ChartSettings { get; set; }
        protected void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        protected ISeries[] inputs;
        protected int numberOfInputs;


        //数据集合本身是ISeries 指标本身也要实现ISeries 这样才可以利用公式进行嵌套计算
        //private DataSeries _resoult

        private bool _valid = false;
        //用于计算的源数据
        //private ISeries _datalist;


        //需要将bar也封装成支持ISeries接口的数据 这样公式体系可以统一对数据进行计算

        public static int VARIABLE_NUM_INPUTS = -1;
        public IndicatorBase(int numOfInput)
        {
            numberOfInputs = numOfInput;
            if (numOfInput != VARIABLE_NUM_INPUTS)
            {
                this.inputs = new ISeries[numOfInput];
            }
            this.ChartSettings = new SeriesChartSettings();
            //this.ChartSettings.ChartPaneName = QuantGlobals.PriceChartName;
        }
        /// <summary>
        /// 获得输入数据序列
        /// </summary>
        /// <returns></returns>
        public virtual ISeries[] GetInputs()
        {
            if (this.inputs == null)
            {
                return new ISeries[0];
            }
            return (ISeries[])this.inputs.Clone();
        }
        /// <summary>
        /// 绑定输入数据序列
        /// </summary>
        /// <param name="newInputs"></param>
        public virtual void SetInputs(params ISeries[] newInputs)
        {
            if (this.numberOfInputs == VARIABLE_NUM_INPUTS)//给定-1则输入参数的数目根据setinputs的数值大小自动决定
            {
                this.inputs = new ISeries[newInputs.Length];
            }
            else if (newInputs.Length != this.numberOfInputs)
            {
                string message = string.Concat(new object[] { "Need exactly ", this.numberOfInputs, " input series, ", newInputs.Length });
                if (newInputs.Length == 1)
                {
                    message = message + " was passed in.";
                }
                else
                {
                    message = message + " were passed in.";
                }
                throw new ArgumentException(message);
            }
            for (int i = 0; i < this.inputs.Length; i++)
            {
                this.inputs[i] = newInputs[i];
            }
        }







        //计算由indicator计算所生成的数据 indicator实现了ISeries接口 可以用于公式计算
        //public IndicatorBase(ISeries series)
        //{
        //    _datalist = series;
        //}
        /*
        public IndicatorBase(BarList barlist)
        {
            _datalist = new BarList2Series(barlist);
        }
        public IndicatorBase(BarList barlist,BarDataType datatype)
        {
            _datalist = new BarList2Series(barlist, datatype);
           
        }
        public IndicatorBase(BarList barlist, BarDataType datatype, BarInterval interval)
        {
            _datalist = new BarList2Series(barlist, datatype, interval);
        }
        */

        //支持ISeries接口 用于放入其他函数中进行计算(计算指标的指标)
        public abstract int Count { get; }


        public abstract double LookBack(int n);
        public abstract double this[int index] { get; }
        public abstract double[] Data { get; }
        public abstract double First { get; }
        public abstract double Last { get; }

        //支持indicator接口
        public abstract void Caculate();//计算
        public abstract void Reset();//重置



        /*
        protected ISeries OrigDataSeries
        {
            get
            {
                return _datalist;
            }

        }**/
        //指标是否有效
        public bool isValid
        {
            get
            {
                return _valid;
            }
            protected set
            {
                _valid = value;
            }
        }

        /// <summary>
        /// 多少个周期前指标序列 与某个ISeries序列相交
        /// </summary>
        /// <param name="series"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public virtual EnumCross Cross(ISeries series, int N)
        {
            if (((N >= 0) && isValid) && ((series.Count >= (N + 2)) && (this.Count >= (N + 2))))
            {
                double num = series.LookBack(N + 1) - LookBack(N + 1);
                double num2 = series.LookBack(N) - LookBack(N);
                if ((num < 0.0) && (num2 > 0.0))
                {
                    return EnumCross.Below;
                }
                if ((num > 0.0) && (num2 < 0.0))
                {
                    return EnumCross.Above;
                }
            }
            return EnumCross.None;
        }

        //ISeries与一个数值相交
        /// <summary>
        ///  多少个周期前指标序列 与某个数值序列相交
        /// </summary>
        /// <param name="dvalue"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public virtual EnumCross Cross(double dvalue, int N)
        {
            if (((N >= 0) && isValid) && (this.Count >= (N + 2)))
            {
                double num = dvalue - LookBack(N + 1);
                double num2 = dvalue - LookBack(N);
                if ((num < 0.0) && (num2 > 0.0))
                {
                    return EnumCross.Below;
                }
                if ((num > 0.0) && (num2 < 0.0))
                {
                    return EnumCross.Above;
                }
            }
            return EnumCross.None;
        }

        //回溯0代表 当前数值进行了交叉,比如价格原本在2300下方，目前价格穿越了2300 但不代表当前bar就收在2300之上
        /// <summary>
        /// 穿过某个ISeries 如果以close价格计算,当前价格波动 很有可能k线结束后 相交状态改变
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public virtual EnumCross Cross(ISeries series)
        {
            return Cross(series, 0);
        }
        /// <summary>
        /// 穿过某个数值,如果以Close价格计算的指标 k线结束后 该状态可能发生变化
        /// </summary>
        /// <param name="dvalue"></param>
        /// <returns></returns>
        public virtual EnumCross Cross(double dvalue)
        {
            return Cross(dvalue, 0);
        }

        //确认已经形成了穿越 指上个价格成功的形成了交叉
        /// <summary>
        /// 确认相交 回溯1个Bar 
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public virtual EnumCross Crossed(ISeries series)
        {
            return Cross(series, 1);
        }
        /// <summary>
        /// 确认相交
        /// </summary>
        /// <param name="dvalue"></param>
        /// <returns></returns>
        public virtual EnumCross Crossed(double dvalue)
        {
            return Cross(dvalue, 1);
        }
    }
}
