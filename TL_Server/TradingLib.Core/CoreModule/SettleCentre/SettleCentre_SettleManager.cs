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
        /// 查询结算状态
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySettleStatus", "QrySettleStatus - 查询结算状态", "查询结算状态")]
        public void CTE_QrySettleStatus(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }
            var status = new { last_settleday = _lastsettleday, current_settleday = _tradingday,settle_mode=_settlemode };
            session.ReplyMgr(status);
        }


        /// <summary>
        /// 查询当前持仓数据
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryPositionHold", "QryPositionHold - 查询当前持仓", "查询当前持仓")]
        public void CTE_QryPositionHold(ISession session)
        {
            logger.Info("查询当前持仓");

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

            f.oSymbol = account.Domain.GetSymbol(f.Exchange,f.Symbol);

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
            session.ReplyMgr(_settlementPriceTracker[currentday].ToArray());
        }

        /// <summary>
        /// 更新结算价
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
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
                session.ReplyMgr(_settlementPriceTracker[this.Tradingday, settlementPrice.Symbol]);
            }

            session.OperationSuccess("结算价更新成功");
        }

        /// <summary>
        /// 回滚交易记录到某个交易日 用于执行手工结算
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

            //手工结算 设定结算中心工作模式为历史结算模式 同时关闭清算中心
            this.SettleMode = QSEnumSettleMode.HistSettleMode;
            TLCtxHelper.ModuleClearCentre.CloseClearCentre();

            var data = JsonMapper.ToObject(json);

            //获得对应的交易日
            int histday = int.Parse(data["currentday"].ToString());

            _tradingday = histday;
            _lastsettleday = Util.ToDateTime(_tradingday, 0).AddDays(-1).ToTLDate();//默认上一个结算日为当前交易日的上一个日期

            //将数据库中相关记录恢复到结算日状态
            ORM.MSettlement.RollBackToSettleday(this.Tradingday);

            //重新加载合约数据
            BasicTracker.SymbolTracker.Reload(this.Tradingday);

            //重置结算价格维护器
            _settlementPriceTracker.Clear(this.Tradingday);

            //加载当前交易日的结算价信息
            _settlementPriceTracker.LoadSettlementPrice(this.Tradingday);

            //重置清算中心 用于加载对应的交易数据
            TLCtxHelper.ModuleClearCentre.Reset();

            //重新启动通道 用于Broker加载对应的交易数据
            TLCtxHelper.ServiceRouterManager.Reset();

            //获得所有未平持仓 并查询是否有结算价记录 没有记录则插入到数据库
            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions.Where(pos => !pos.isFlat))
            {
                //如果该未平持仓没有对应的结算价信息 则我们在list中加入该合约 用于推送到手工结算窗口让管理员进行填写
                if (_settlementPriceTracker[this.Tradingday, pos.Symbol] == null)
                {
                    //插入该结算价信息记录 价格为-1
                    _settlementPriceTracker.UpdateSettlementPrice(new MarketData() { Settlement = -1, SettleDay = this.Tradingday, Symbol = pos.Symbol });
                }
                else
                {
                    MarketData d = _settlementPriceTracker[this.Tradingday, pos.Symbol];
                    pos.SettlementPrice = d.Settlement;
                    pos.LastSettlementPrice = d.PreSettlement;
                }
            }

            //触发帐户变化事件 用于更新昨日权益
            foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                TLCtxHelper.EventAccount.FireAccountChangeEent(acc);
            }

            session.ReplyMgr("rollback");//管理段通过Rollback回报来触发 查询持仓，查询结算价等相关操作
            session.OperationSuccess(string.Format("系统回滚到交易日:{0}成功", histday));
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ReSettleExchange", "ReSettleExchange - 重新执行交易所结算", "重新执行交易所结算", QSEnumArgParseType.Json)]
        public void CTE_SettleExchange(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = JsonMapper.ToObject(json);
            //获得对应的交易日
            string excode  = data["exchange"].ToString();
            IExchange exchagne = BasicTracker.ExchagneTracker[excode];
            if (exchagne == null)
            {
                throw new FutsRspError("交易所:" + excode + "不存在");
            }
            List<IExchange> exlist = new List<IExchange>();
            exlist.Add(exchagne);
            //结算交易所
            SettleExchange(exlist,this.Tradingday);

            session.OperationSuccess("交易所结算完毕");
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
            logger.Info(string.Format("Settle Tradingday:{0} Manually，Current Tradingday:{1}", settleday, _tradingday));

            if (settleday != _tradingday)
            {
                throw new FutsRspError(string.Format("当前交易数据交易日不符,请先加载交易日:{0}的交易数据", settleday));
            }

            //执行结算过程
            //1.交易所结算
            SettleExchange(BasicTracker.ExchagneTracker.Exchanges.ToList<IExchange>(), _tradingday);

            //2.帐户结算
            SettleAccount();

            //保存已结算交易记录
            DumpDataToLogTable();

            session.OperationSuccess(string.Format("交易日:{0}结算完成", settleday));
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ResetSystem", "ResetSystem - 重置系统", "重置系统")]
        public void CTE_ReSettle(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }
            //执行结算重置
            this.SetteReset();


        }
        /// <summary>
        /// 转储已结算交易记录
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "StoreSettledData", "StoreSettledData - 转储已结算交易记录", "将tmp表中的交易记录转储到交易记录历史表")]
        public void CTE_ReqDumpSettledData(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()))
            {
                throw new FutsRspError("无权进行该操作");
            }
            //转储所有记录
            StoreAllData();
            session.OperationSuccess("转储交易记录成功");
        }

        /// <summary>
        /// 删除上个结算日及以前所有已结算数据
        /// 加入结算异常检查
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteSettledData", "DeleteSettledData - 删除已结算交易记录", "将tmp表中的已结算交易记录删除")]
        public void CTE_DeleteSettledData(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager == null || (!manager.IsRoot()) || (!manager.Domain.Super))
            {
                throw new FutsRspError("无权进行该操作");
            }
            //转储所有记录
            StoreAllData();
            //查询上个结算日以前的未结算委托
            int onum = ORM.MTradingInfo.GetUnsettledAcctOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
            int tnum = ORM.MTradingInfo.GetUnsettledAcctTradeNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
            if (onum > 0 || tnum > 0)
            {
                throw new FutsRspError(string.Format("结算日:{0} 之前有未结算委托与成交数据", TLCtxHelper.ModuleSettleCentre.LastSettleday));
            }

            //删除上个交易日以前的所有已结算数据
            ORM.MTradingInfo.DeleteSettledTradingInfo();
            session.OperationSuccess("删除已结算临时数据成功");
        }


    }
}