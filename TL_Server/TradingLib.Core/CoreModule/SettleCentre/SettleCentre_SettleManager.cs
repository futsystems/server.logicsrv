﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;


namespace TradingLib.Core
{
    public partial class SettleCentre
    {
        /// <summary>
        /// 删除某个交易日的结算数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteSettleInfo", "DeleteSettleInfo - 删除某个交易日的结算数据", "删除某个交易日的结算数据", QSEnumArgParseType.Json)]
        public void CTE_DeleteSettleInfo(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);
            //获得对应的交易日
            int settleday = int.Parse(data["settleday"].ToString());

            if (!TradingCalendar.IsTradingday(settleday))
            {
                throw new FutsRspError(string.Format("{0}不是交易日", settleday));
            }


            ORM.MSettlement.DeletePositionDetails(settleday);
            ORM.MSettlement.DeleteSettlement(settleday);
            ORM.MSettlement.DeleteHoldPositionRound(settleday);

            session.OperationSuccess(string.Format("删除交易日:{0}结算信息成功", settleday));
        }

        /// <summary>
        /// 查询结算状态
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySettleStatus", "QrySettleStatus - 查询结算状态", "查询结算状态")]
        public void CTE_QrySettleStatus(ISession session)
        {
            var status = new { last_settleday = _lastsettleday, next_settleday = _nexttradingday, current_settleday = _tradingday };
            session.ReplyMgr(status);
        }


        /// <summary>
        /// 查询当前持仓数据
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryPositionHold", "QryPositionHold - 查询当前持仓", "查询当前持仓")]
        public void CTE_QryPositionHold(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            //将所有持仓生成PositionEx回报
            var data = TLCtxHelper.ModuleClearCentre.TotalPositions.Where(pos => !pos.isFlat).Select(pos => pos.GenPositionEx()).ToArray();

            session.ReplyMgr(data);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "FlatPositionHold", "FlatPositionHold - 平掉当前隔夜持仓", "平掉当前隔夜持仓", QSEnumArgParseType.Json)]
        public void CTE_FlatPositionHold(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);

            //交易帐户
            var acct = data["account"].ToString();
            //持仓合约
            var symbol = data["symbol"].ToString();
            //持仓方向
            var side = bool.Parse(data["side"].ToString());
            //以上参数用于确定某个持仓对象


            //平仓时间
            int time = int.Parse(data["time"].ToString());
            //平仓数量
            var size = int.Parse(data["size"].ToString());
            //平仓价格
            decimal price = decimal.Parse(data["price"].ToString());
            //平今，平昨标志
            var flag = (QSEnumOffsetFlag)Enum.Parse(typeof(QSEnumOffsetFlag), data["offset_flag"].ToString());


            Trade f = new TradeImpl(symbol, price, size * (side ? 1 : -1));
            f.xDate = _tradingday;//设定成交时间为 交易日
            f.xTime = time;
            f.OffsetFlag = flag;

            f.Account = acct;
            IAccount account = TLCtxHelper.ModuleAccountManager[acct];

            f.oSymbol = account.Domain.GetSymbol(f.Symbol);

            if (f.oSymbol == null)
            {
                throw new FutsRspError(string.Format("合约:{0}不存在", f.Symbol));
            }

            if (account == null)
            {
                throw new FutsRspError(string.Format("交易帐户:{0}不存在", acct));
            }

            Position pos = account.GetPosition(f.Symbol, !f.Side);
            if (f.UnsignedSize > pos.UnsignedSize)
            {
                throw new FutsRspError(string.Format("平仓数量大于持仓数量"));
            }

            //TODO:改进时间检查
            //时间检查
            //IMarketTime mt = f.oSymbol.SecurityFamily.MarketTime;
            //if (!mt.IsInMarketTime(f.xTime))
            //{
            //    throw new FutsRspError("平仓时间不在交易时间段内");
            //}

            f.Broker = "SIMBROKER";

            Order o = new MarketOrder(f.Symbol, f.Side, f.UnsignedSize);

            o.oSymbol = f.oSymbol;
            o.Account = f.Account;
            o.Date = f.xDate;
            o.Time = Util.ToTLTime(Util.ToDateTime(f.xDate, f.xTime) - new TimeSpan(0, 0, 1));
            o.Status = QSEnumOrderStatus.Filled;
            o.OffsetFlag = f.OffsetFlag;
            o.Broker = f.Broker;

            //委托成交之后
            o.TotalSize = o.Size;
            o.Size = 0;
            o.FilledSize = o.UnsignedSize;

            //注意这里需要获得可用的委托流水和成交流水号

            long ordid = 0;// _exchsrv.futs_InsertOrderManual(o);


            f.id = ordid;
            f.OrderSeq = o.OrderSeq;
            f.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
            f.TradeID = "xxxxx";//随机产生的成交编号

            Util.sleep(100);
            //_exchsrv.futs_InsertTradeManual(f);

            /* 在进行手工插入时 由于清算中心根据清算中心的状态来决定是否保存交易记录到数据库，在历史手工结算时
             * 清算中心需要记录这条交易记录
             * 
             * 
             * 
             * 
             * **/
            session.OperationSuccess("手工平仓成功");
        }




        /// <summary>
        /// 查询结算价信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySettlementPrice", "QrySettlementPrice - 查询结算价信息", "查询结算价信息", QSEnumArgParseType.Json)]
        public void CTE_QrySettlementPrice(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);
            //获得对应的交易日
            int currentday = int.Parse(data["settleday"].ToString());

            //发送结算价信息
            session.ReplyMgr(_settlementPriceTracker.SettlementPrices.ToArray());
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateSettlementPrice", "UpdateSettlementPrice - 更新结算价格信息", "更新结算价格信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateSettlementPrice(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var settlementPrice = JsonMapper.ToObject<MarketData>(json);
            if (settlementPrice != null)
            {
                _settlementPriceTracker.UpdateSettlementPrice(settlementPrice);
                session.ReplyMgr(_settlementPriceTracker[settlementPrice.Symbol]);
            }

            session.OperationSuccess("结算价更新成功");
        }

        /// <summary>
        /// 设定当前结算中心日期，用于恢复当日的交易信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RollBackToDay", "RollBackToDay - 回滚到某个日期并加载交易数据", "回滚到某日并加载交易数据", QSEnumArgParseType.Json)]
        public void CTE_RollBackToDay(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);

            //获得对应的交易日
            int currentday = int.Parse(data["currentday"].ToString());

            //设定当前日期
            this.SetCurrentDay(currentday);

            //重新加载合约数据
            BasicTracker.SymbolTracker.Reload();

            //重置结算价格维护器
            _settlementPriceTracker.Clear();

            //加载当前交易日的结算价信息
            _settlementPriceTracker.LoadSettlementPrice(this.NextTradingday);

            //重置清算中心 用于加载对应的交易数据
            TLCtxHelper.ModuleClearCentre.Reset();
            
            //获得所有未平持仓 并查询是否有结算价记录 没有记录则插入到数据库
            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions.Where(pos => !pos.isFlat))
            {
                //如果该未平持仓没有对应的结算价信息 则我们在list中加入该合约 用于推送到手工结算窗口让管理员进行填写
                if (_settlementPriceTracker[pos.Symbol] == null)
                {
                    //插入该结算价信息记录 价格为-1
                    _settlementPriceTracker.UpdateSettlementPrice(new MarketData() { Settlement = -1, SettleDay = this.NextTradingday, Symbol = pos.Symbol });
                }
            }

            foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //触发帐户变化事件
            }

            session.ReplyMgr("xxxxxx");
            session.OperationSuccess(string.Format("系统回滚到交易日:{0}成功", currentday));
        }

        /// <summary>
        /// 对某个交易日执行结算操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ReSettle", "ReSettle - 重新执行结算", "重新执行结算", QSEnumArgParseType.Json)]
        public void CTE_ReSettle(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);
            //获得对应的交易日
            int settleday = int.Parse(data["settleday"].ToString());

            if (!TradingCalendar.IsTradingday(settleday))
            {
                throw new FutsRspError(string.Format("{0}不是交易日", settleday));
            }

            int cursettleday = this.CurrentTradingday;//
            logger.Info(string.Format("重新执行交易日:{0}的结算操作，当前交易日为:{1}", settleday, cursettleday));

            if (settleday != cursettleday)
            {
                throw new FutsRspError(string.Format("当前交易数据交易日不符,请先加载交易日:{0}的交易数据", settleday));
            }

            //通过系统事件中继触发结算前事件
            //TLCtxHelper.EventSystem.FireBeforeSettleEvent(this, new SystemEventArgs());
            //this.IsInSettle = true;//标识结算中心处于结算状态

            //绑定结算价格
            this.BindSettlementPrice();

            //A:储存 结算数据(各业持仓,持仓回合,当日交易记录)
            //保存结算持仓数据和对应的PR数据
            this.SaveHoldInfo();
            //保存持仓明细
            this.SavePositionDetails();
            //转储到历史记录表
            this.Dump2Log();

            //B:结算交易帐户形成结算记录
            this.SettleAccount();

            session.OperationSuccess(string.Format("交易日:{0}结算完成", settleday));
        }

        /// <summary>
        /// 查询结算价信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ResetSystem", "ResetSystem - 重置当前系统 进入工作状态", "重置当前系统 进入工作状态", QSEnumArgParseType.Json)]
        public void CTE_QrySettlementPrice(ISession session)
        {
            this.Reset();

            this.ResetSystem();
        }
    }
}