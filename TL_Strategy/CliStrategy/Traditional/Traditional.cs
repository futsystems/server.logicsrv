using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Strategy.ExitPosition
{
    public class Traditional : PositionCheckTemplate, ITraditionalStrategy
    {
        //该position在配置窗口中显示的标题名称
        public static string Title
        {
            get { return "止盈止损"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "传统止盈止损"; }
        }


        StopLossArgs lossargs=null;
        /// <summary>
        /// 止损参数
        /// </summary>
        public StopLossArgs StopArgs { get { return lossargs; } set { lossargs = value; } }

        ProfitArgs profitargs = null;
        /// <summary>
        /// 止盈参数
        /// </summary>
        public ProfitArgs ProfitArgs { get { return profitargs; } set { profitargs = value; } }


        bool ordersent = false;

        //计算我们需要的平仓数量
        //1.设定数量为0或 大于持仓数量,则数量为当前持仓数量
        //2.设定数量小于持仓数量,则数量为设定数量
        int getSize(int size)
        {
            if (size > myPosition.UnsignedSize || size == 0) return myPosition.UnsignedSize;
            return size;
        }


        /// <summary>
        /// 充置 止损和止盈的 工作状态
        /// 如果是空仓,则我们停止止损和止盈
        /// </summary>
        /*
        void ResetEnable()
        {
            if (myPosition.isFlat)
            {
                //保证委托只发送一次,发送完后就不发送了。
                ordersent = false;
                if (profitargs != null) profitargs.Enable = false;
                if (StopArgs != null) StopArgs.Enable = false;
            }
        }**/
        bool isinresume = false;
        /// <summary>
        /// 如果系统出会恢复当日交易信息状态,我们将不进行持仓检查,有的时候会发生刚好持仓恢复过程中出现空持仓,而关闭止盈 止损
        /// 恢复置最后阶段,还是有持仓,但是止损 止盈被取消了的情况
        /// </summary>
        public bool IsInResume { get { return isinresume; } set { isinresume = value; } }

        DateTime last = DateTime.Now;
        /// <summary>
        /// 检查持仓
        /// </summary>
        /// <param name="msg"></param>
        public override void checkPosition(out string msg)
        {

            msg = "";
            //在数据恢复开始与结束均有相关事件触发,在数据恢复过程中,系统不执行持仓检查,因为数据恢复过程中的持仓是动态变化的,有可能错误止损 止盈
            if (isinresume)
            {
                D("********************数据恢复中 直接返回");
                return;//如果我们在数据恢复阶段,则我们直接返回不进行持仓检查
            }


            //如果当前持仓为0,则直接返回 并标记委托发送标记
            if (myPosition.isFlat)
            {
                ordersent = false;
                return;//空仓直接返回
            }

            //日志输出部分
            if ((DateTime.Now - last).TotalSeconds > 5)
            {
                last = DateTime.Now;
                if(StopArgs!=null && ProfitArgs!=null )
                    D("enable:" +StopArgs.Enable.ToString()+" "+ProfitArgs.Enable.ToString() + " StopArgs:" + StopArgs.ToString() + " ProfitArgs:" + ProfitArgs.ToString() + " stophit:" + StopArgs.Caculate(myPosition).ToString() + " profithit:" + ProfitArgs.Caculate(myPosition).ToString() + "resume:" + IsInResume.ToString());
            }
            if (myPosition.isLong)
            { 
                
                //止损
                if(StopArgs!=null && StopArgs.Enable)
                {
                    decimal hitprice = StopArgs.Caculate(myPosition);//获得触发价格
                    if (myPosition.LastPrice <= hitprice)
                    {
                        if (!ordersent)
                        {
                            SellMarket(getSize(StopArgs.Size), "止损");
                            ordersent = true;
                            D("止损触发:" + StopArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " size:" + getSize(StopArgs.Size).ToString());

                        }
                    }
                }

                //止盈
                if (ProfitArgs != null && ProfitArgs.Enable)
                {
                    if (profitargs.Type == ProfitOffsetType.TRAILING)
                    {
                        decimal hitprice = ProfitArgs.Caculate(myPosition);
                        if (hitprice > 0)
                        {
                            if (myPosition.LastPrice <= hitprice)
                            {
                                if (!ordersent)
                                {
                                    SellMarket(getSize(ProfitArgs.Size), "止盈");
                                    ordersent = true;
                                    D("跟踪止盈触发:" + ProfitArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " hightest:" + myPosition.Highest.ToString() + " size:" + getSize(profitargs.Size).ToString() + " value:" + profitargs.Value.ToString() + " start:"+ profitargs.Start.ToString());

                                }
                            }

                        }

                    }
                    else
                    {
                        decimal hitprice = ProfitArgs.Caculate(myPosition);
                        if (myPosition.LastPrice >= hitprice)
                        {
                            if (!ordersent)
                            {
                                SellMarket(getSize(ProfitArgs.Size), "止盈");
                                ordersent = true;
                                D("止盈触发:" + ProfitArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " size:" + getSize(StopArgs.Size).ToString());
                            }
                        }
                    }

                }

            
            }
            else if (myPosition.isShort)
            {
                //止损
                if (StopArgs != null && StopArgs.Enable)
                {
                    decimal hitprice = StopArgs.Caculate(myPosition);
                    if (myPosition.LastPrice >= hitprice)
                    {
                        if (!ordersent)
                        {
                            BuyMarket(getSize(StopArgs.Size), "止损");
                            ordersent = true;
                            D("止损触发:" + StopArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " size:" + getSize(StopArgs.Size).ToString());
                        }
                    }
                }

                //止盈
                if (ProfitArgs != null && ProfitArgs.Enable)
                {
                    if (profitargs.Type == ProfitOffsetType.TRAILING)
                    {
                        decimal hitprice = ProfitArgs.Caculate(myPosition);
                        if (hitprice > 0)
                        {
                            if (myPosition.LastPrice >= hitprice)
                            {
                                if (!ordersent)
                                {
                                    BuyMarket(getSize(ProfitArgs.Size), "止盈");
                                    ordersent = true;
                                    D("止盈触发:" + ProfitArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " size:" + getSize(StopArgs.Size).ToString());
                                }
                            }

                        }
                    }
                    else
                    {
                        decimal hitprice = ProfitArgs.Caculate(myPosition);
                        if (myPosition.LastPrice <= hitprice)
                        {
                            if (!ordersent)
                            {
                                BuyMarket(getSize(ProfitArgs.Size), "止盈");
                                ordersent = true;
                                D("止盈触发:" + ProfitArgs.ToString() + " hitpirce:" + hitprice.ToString() + " lastprice:" + myPosition.LastPrice.ToString() + " size:" + getSize(StopArgs.Size).ToString());
                            }
                        }
                    }
                }

            }
        
        }

        public override string ToText()
        {
            return "";
        }

        public override IPositionCheck FromText(string msg)
        {
            return this;
        }


    }
}
