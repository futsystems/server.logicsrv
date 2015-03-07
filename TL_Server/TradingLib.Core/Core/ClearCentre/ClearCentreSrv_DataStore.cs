using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        #region ���ݱ���

        string datacleanheader = "#####DataClean:";
        /// <summary>
        /// ��������������ʱ��¼��
        /// </summary>
        public void CleanTempTable()
        {
            debug(datacleanheader + "Clean Tmp_XXX Tables", QSEnumDebugLevel.INFO);
            ORM.MTradingInfo.ClearIntradayOrders(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(TLCtxHelper.CmdSettleCentre.NextTradingday);
            debug("Cleaned Success", QSEnumDebugLevel.INFO);
        }



        public void SaveHoldInfo()
        {
            debug(datastoreheader + "Save PositionRound Open Into DataBase", QSEnumDebugLevel.INFO);
            foreach (PositionRound pr in this.PositionRoundTracker.RoundOpened)
            {
                ORM.MSettlement.InsertHoldPositionRound(pr, TLCtxHelper.CmdSettleCentre.NextTradingday);
            }
            debug(datastoreheader + "Save Positionround Open Successfull", QSEnumDebugLevel.INFO);
        }

        string datastoreheader = "#####DataStore:";
        /// <summary>
        /// 1.����ֲ���ϸ
        /// �����и�ҹ�ֲ���ϸ���浽��ʷ�ֲ���ϸ�� ������һ�����������ɶ�Ӧ�ĳֲ�״̬
        /// ��������˳ֲ���ϸ�Ķ���ӯ�����㣬�ü�����Ҫ���ս����
        /// </summary>
        public void SavePositionDetails()
        {
            debug(datastoreheader + "Save PositionDetails....", QSEnumDebugLevel.MUST);

            //�������ϵͳ�ְֲ���һ�����߼���� ����� Ŀǰ�������۲�������ȡ�ֲ����¼������(�ֲ����¼� ��û��tickʱ���Գֲֳɱ�����)
            foreach (Position pos in this.TotalPositions)
            {
                if (_settleWithLatestPrice)//��������¼۽��н���
                {
                    pos.SettlementPrice = pos.LastPrice;//�����¼��趨���ֲֵĽ����
                }
                else
                {
                    if (pos.SettlementPrice == null)
                        pos.SettlementPrice = pos.LastPrice;
                }
            }


            int i = 0;
            //�������н����ʻ�
            foreach (IAccount account in this.Accounts)
            {
                //���������ʻ�������δƽ�ֲֳֶ���
                foreach (Position pos in account.GetPositionsHold())
                {
                    //������δƽ�ֲֳֶ����µ����гֲ���ϸ
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //�������ֲ���ϸʱҪ�������ո���Ϊ��ǰ
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                        //����ֲ���ϸ�����ݿ�
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            debug(string.Format("Saved {0} Account PositionDetails Successfull", i), QSEnumDebugLevel.INFO);

            i = 0;
            //�������гɽ��ӿ�
            foreach (IBroker broker in TLCtxHelper.Ctx.RouterManager.Brokers)
            {
                //�ӿ�û������ ��û�н�������
                if (!broker.IsLive)
                    continue;

                //�����ɽ��ӿ��гֲֵ� �ֲ֣����óֲֵĳֲ���ϸ���浽���ݿ�
                foreach (Position pos in broker.Positions.Where(p => !p.isFlat))
                {
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //�������ֲ���ϸʱҪ�������ո���Ϊ��ǰ
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                        //�趨��ʶ
                        pd.Broker = broker.Token;
                        pd.Breed = QSEnumOrderBreedType.BROKER;

                        //����ֲ���ϸ�����ݿ�
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            debug(string.Format("Saved {0} Broker PositionDetails Successfull", i), QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 2.�����ڽ������ݴ�������ʷ���׼�¼��
        /// </summary>
        public void Dump2Log()
        {
            debug(datastoreheader + "Dump TradingInfo(Order,Trade,OrderAction)", QSEnumDebugLevel.INFO);
            int onum, tnum, cnum, prnum;

            ORM.MTradingInfo.DumpIntradayOrders(out onum);
            ORM.MTradingInfo.DumpIntradayTrades(out tnum);
            ORM.MTradingInfo.DumpIntradayOrderActions(out cnum);
            ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            debug("Order       Saved:" + onum.ToString(), QSEnumDebugLevel.INFO);
            debug("Trade       Saved:" + tnum.ToString(), QSEnumDebugLevel.INFO);
            debug("OrderAction Saved:" + cnum.ToString(), QSEnumDebugLevel.INFO);
            debug("PosTrans    Saved:" + prnum.ToString(), QSEnumDebugLevel.INFO);
        }




        #endregion

        /// <summary>
        /// �������н����˻�
        /// �������
        /// 1.����lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity ���ݿ����ͨ��
        /// 2.���ս�����Ϻ��nowequity��Ϊ�˻����е�lastequity ���ݿ����ͨ��
        /// 3.�������������ĳЩ�����¼������Ȩ�治���ڸ��˻���nowequity
        /// 4.�˻�����ʱ��Ҫ��� �˻�������Ȩ���Ƿ������ݿ��¼������Ȩ��
        /// </summary>
        void SettleAccount()
        {
            debug(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", TLCtxHelper.CmdSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            foreach (IAccount acc in this.Accounts)
            {
                try
                {
                    ORM.MSettlement.SettleAccount(acc);
                }
                catch (Exception ex)
                {
                    debug(string.Format("SettleError,Account:{0} errors:{1}", acc.ID, ex.ToString()), QSEnumDebugLevel.ERROR);
                }
            }

            //�������������
            debug(string.Format("Update lastsettleday as:{0}",TLCtxHelper.CmdSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(TLCtxHelper.CmdSettleCentre.CurrentTradingday);
            debug("Settlement Done", QSEnumDebugLevel.INFO);
        }

    }
}
