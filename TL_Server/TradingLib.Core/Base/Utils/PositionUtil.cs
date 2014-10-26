using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public static class PositionUtil_core
    {

        /// <summary>
        /// 结算持仓 
        /// 进行持仓明细计算，最终持仓明细和当日平仓明细并存入数据库表
        /// </summary>
        /// <param name="pos"></param>
        public static void SettlePosition(this Position pos)
        { 
            IEnumerable<PositionCloseDetail> closedetails = null;
            //进行持仓明细生成计算
            IEnumerable<PositionDetail> posdetail_curr = pos.CalPositionDetail(out closedetails);

            //插入持仓明细
            foreach (PositionDetail posdetail in posdetail_curr)
            {
                //表明是新开持仓 插入数据时候需要将当前交易日设置进去
                if (posdetail.Tradingday == 0)
                {
                    posdetail.Tradingday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                }
                posdetail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                ORM.MSettlement.InsertPositionDetail(posdetail);//当前的下一个交易日为当前持仓明细的持仓日
            }

            //插入平仓明细
            foreach (PositionCloseDetail closedetail in closedetails)
            {
                closedetail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;

                ORM.MSettlement.InsertPositionCloseDetail(closedetail);
            }

        }
        /// <summary>
        /// 由持仓数据获得持仓明细数据
        /// 获得持仓明细的过程中会产生对应的平仓明细数据
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> CalPositionDetail(this Position pos,out IEnumerable<PositionCloseDetail> closedetails)
        {
            Util.Debug("Pos:" + pos.ToString());
            IEnumerable<Trade> trades = pos.Trades;//获得该持仓的当日成交记录
            //将日内成交记录分组成开仓成交与平仓成交
            //开仓成交记录
            IEnumerable<Trade> trades_open = trades.Where(f => f.IsEntryPosition);
            //平仓成交记录
            IEnumerable<Trade> trades_close = trades.Where(f => !f.IsEntryPosition);

            //昨日持仓明细列表 这里需要复制对应持仓的隔夜持仓明细数据 否则计算持仓明细的时候会修改该持仓明细的closevolume等数据
            IEnumerable<PositionDetail> pos_his_open = pos.YdPositionDetails.Select(f=>new PositionDetailImpl(f));//

            //开仓成交记录形成当日持仓明细列表
            IEnumerable<PositionDetail> pos_today_open = trades_open.Select(f => f.ToPositionDetail());

            //获得所有持仓明细的并集
            List<PositionDetail> pos_open = pos_his_open.Concat(pos_today_open).ToList();

            Util.Debug("当日新开仓明细");
            foreach (PositionDetail pd in pos_today_open)
            {
                Util.Debug(pd.GetPositionDetailStr());
            }

            //平仓明细列表
            List<PositionCloseDetail> pos_close_details = new List<PositionCloseDetail>();
            closedetails = pos_close_details;

            //没有平仓成交记录
            if (trades_close.Count() == 0)
            {
                Util.Debug("没有平仓成交记录,所有的开仓成交记录形成当日新开仓记录");
            }
            //有平仓汇总记录 
            else
            {
                Util.Debug("有平仓成交汇总记录,计算当日新开仓记录");
                //用当日开仓成交记录形成持仓明细 再用平仓汇总记录去执行平仓

                foreach (Trade close in trades_close)//遍历所有平仓成交记录 用平仓成交记录去平开仓成交记录形成的持仓明细
                {
                    Util.Debug("取平仓成交:" + close.GetTradeDetail());
                    int remainsize = Math.Abs(close.xsize);

                    foreach (PositionDetail pd in pos_open)
                    {
                        if (pd.IsClosed())
                        {
                            Util.Debug("持仓:" + pd.GetPositionDetailStr() + "已经全部平掉,取下一条持仓记录");
                            continue;
                        }

                        PositionCloseDetail closedetail = pd.ClosePositon(close, ref remainsize);

                        Util.Debug("获得平仓明细:" + closedetail.GetPositionCloseStr());
                        pos_close_details.Add(closedetail);

                        Util.Debug("持仓跟新:" + pd.GetPositionDetailStr() + " 平仓量:" + closedetail.CloseVolume.ToString() + " 剩余平仓量:" + remainsize.ToString());

                        //如果剩余平仓数量为0 则跳出持仓循环，取下一个平仓记录
                        if (remainsize == 0)
                        {
                            Util.Debug("平仓成交:" + close.GetTradeDetail() + " 全部用完，取下一条平仓成交记录");
                            break;
                        }
                    }


                    //foreach (PositionDetail pd in pos_today_open)
                    //{
                    //    //如果持仓已经关闭则取下一条新开持仓记录 
                    //    if (pd.IsClosed())
                    //    {
                    //        Util.Debug("持仓:" + pd.GetPositionDetailStr() + "已经全部平掉,取下一条持仓记录");
                    //        continue;
                    //    }

                    //    PositionCloseDetail closedetail = pd.ClosePositon(close, ref remainsize);

                    //    Util.Debug("获得平仓明细:" + closedetail.GetPositionCloseStr());
                    //    pos_close_details.Add(closedetail);

                    //    Util.Debug("持仓跟新:" + pd.GetPositionDetailStr() + " 平仓量:" + closedetail.CloseVolume.ToString() + " 剩余平仓量:" + remainsize.ToString());

                    //    //如果剩余平仓数量为0 则跳出持仓循环，取下一个平仓记录
                    //    if (remainsize == 0)
                    //    {
                    //        Util.Debug("平仓成交:" + close.GetTradeDetail() + " 全部用完，取下一条平仓成交记录");
                    //        break;
                    //    }
                    //}

                }

            }//有平仓记录处理结束

            //取昨日持仓明细与当日新开持仓明细 并集
            
            Util.Debug("当前持仓明细汇总");
            //当前最终持仓明细
            //IEnumerable<PositionDetail> pos_today_hist = pos_today_open.Where(pd => !pd.IsClosed());
            foreach (PositionDetail pd in pos_open)
            {

                Util.Debug(pd.GetPositionDetailStr());
            }

            return pos_open;
        }
    }
}
