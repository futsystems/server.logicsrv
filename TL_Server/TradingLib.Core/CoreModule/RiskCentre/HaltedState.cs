using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 熔断状态
    /// </summary>
    public enum EnumHaltState
    { 
        /// <summary>
        /// 非熔断状态
        /// </summary>
        NOTHALTED,
        /// <summary>
        /// 5%熔断
        /// 停止交易15分钟
        /// </summary>
        PECT5,
        /// <summary>
        /// 7%熔断
        /// 暂停交易至收盘
        /// </summary>
        PECT7,
    }
    public class HaltedStateTracker
    {
        const decimal VALE_PECT5 = 0.05M;
        const decimal VALE_PECT7 = 0.07M;

        //Clean-Up interval
        internal static long HALTED_INTERVAL = 1 * 1000;
        DateTime _haltedstarttime = DateTime.Now;//熔断开始状态
        bool _ispect5fired = false;

        public HaltedStateTracker()
        {
            this.HaltState = EnumHaltState.NOTHALTED;

            //Create a Time to track the expired objects for cleanup.
            Timer aTimer = new Timer();
            aTimer.Enabled = true;
            aTimer.Interval = HALTED_INTERVAL;//定时器 用于定时检查熔断状态
            aTimer.Elapsed += new ElapsedEventHandler(HaltCheck);//定时检查熔断状态
            aTimer.Start();
        }

        private void HaltCheck(object sender, ElapsedEventArgs ea)//
        {
            //非熔断状态直接返回
            if (!IsHalted) return;
            
            //当前处于5%熔断 停止交易15分钟后 熔断解除
            if (this.HaltState == EnumHaltState.PECT5)
            {
                //如果熔断时间已经大于15分钟 则修改状态为 非熔断状态 此处时间需要判定是否包含了停盘时间
                int start = _haltedstarttime.ToTLTime();

                //如果是中午收盘前15分钟内触发的熔断 则需要下午开盘后才可以解除
                if (start <= 111500 || start >= 130000)
                {
                    if (DateTime.Now.Subtract(_haltedstarttime).TotalMinutes >= 15)
                    {
                        this.HaltState = EnumHaltState.NOTHALTED;
                    }
                }
                else
                {
                    //中午收盘前
                    DateTime nowt = DateTime.Now;
                    int now = nowt.ToTLTime();
                    if ( now <= 113000)
                    {
                        return;//15分钟熔断未结束
                    }
                    else if(now >=130000)
                    { 
                        double sp = (new DateTime(nowt.Year,nowt.Month,nowt.Day,11,30,00) - _haltedstarttime).TotalMinutes;
                        if (DateTime.Now.Subtract(new DateTime(nowt.Year, nowt.Month, nowt.Day, 13, 00, 00)).TotalMinutes > (5 - sp))
                        {
                            this.HaltState = EnumHaltState.NOTHALTED;
                        }
                    }
                }
            }
            //如果处于7%熔断状态 则持续到收盘
            if (this.HaltState == EnumHaltState.PECT7)
            {
                return;
            }
        }



        /// <summary>
        /// 是否处于熔断状态
        /// 如果处于熔断状态则对应的品种不可交易
        /// </summary>
        public bool IsHalted
        {
            get
            {
                if (this.HaltState == EnumHaltState.NOTHALTED)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 重置熔断状态
        /// </summary>
        public void Reset()
        {
            this.HaltState = EnumHaltState.NOTHALTED;
            _ispect5fired = false;

        }

        /// <summary>
        /// 熔断状态
        /// </summary>
        public EnumHaltState HaltState { get; set; }

        /// <summary>
        /// 处理行情根据行情数值触发熔断状态
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            if (k.Symbol != "CSI300") return;//响应 沪深300指数值
            if (k.Trade <= 0) return;
            if (k.PreClose <= 0) return;

            //如果当前处于熔断状态 则直接返回
            if (this.IsHalted) return;

            //计算当前指数涨跌幅度
            decimal pect = Math.Abs((k.Trade - k.PreClose) / k.PreClose);


            if (pect >= VALE_PECT7)
            {
                _haltedstarttime = DateTime.Now;
                this.HaltState = EnumHaltState.PECT7;
                return;
            }

            //5%熔断 只能触发一次
            if (pect >= VALE_PECT5 && (!_ispect5fired))
            {
                _ispect5fired = true;
                _haltedstarttime = DateTime.Now;
                this.HaltState = EnumHaltState.PECT5;
                return;
            }
            
            
        }
    }
}
