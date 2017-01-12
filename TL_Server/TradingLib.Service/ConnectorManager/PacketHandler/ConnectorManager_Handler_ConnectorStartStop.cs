using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;


namespace TradingLib.ServiceManager
{
    internal enum QSEnumConnectorOperation
    {
        /// <summary>
        /// 启动交易接口
        /// </summary>
        Start,//启动

        /// <summary>
        /// 停止交易接口
        /// </summary>
        Stop,//停止
    }


    public partial class ConnectorManager
    {

        void ConnectorOperation(ISession session, ConnectorConfig cfg, QSEnumConnectorOperation op)
        {
            try
            {
                if (cfg.Interface != null && cfg.Interface.IsValid)
                {
                    Action<string> handler = null;
                    switch (op)
                    {
                        case QSEnumConnectorOperation.Start:
                            {
                                handler = (cfg.Interface.Type == QSEnumConnectorType.Broker ? new Action<string>(StartBroker) : new Action<string>(StartDataFeed));
                                break;
                            }
                        case QSEnumConnectorOperation.Stop:
                            {
                                handler = (cfg.Interface.Type == QSEnumConnectorType.Broker ? new Action<string>(StopBroker) : new Action<string>(StopDataFeed));
                                break;
                            }
                        default:
                            handler = null;
                            break;
                    }
                    if (handler != null)
                    {
                        handler(cfg.Token);
                        session.OperationSuccess(string.Format("通道[{0}]操作成功", cfg.Token));
                        NotifyConnectorStatus(session, cfg);
                    }


                }
                else
                {
                    throw new FutsRspError("通道底层接口异常");
                }
            }
            catch (FutsRspError ex)
            {
                //通知该通道对应的管理员
                session.OperationError(ex);
            }
        }

        void NotifyConnectorStatus(ISession session, ConnectorConfig cfg)
        {
            ConnectorStatus status = GetConnectorStatus(cfg);//获得通道状态
            IEnumerable<ILocation> locations = cfg.Domain.GetRootLocations();//获得该域的所有管理员地址
            if (locations.Count() > 0)
            {
                session.NotifyMgr("NotifyConnectorStatus", status, locations);
            }
        }

        void StartBroker(string token)
        {
            IBroker b = FindBroker(token);
            if (b == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (b.IsLive)//已经启动
            {
                throw new FutsRspError("通道已启动");
            }
            string msg = string.Empty;
            bool s = b.Start(out msg);
            if (!s)
            {
                throw new FutsRspError(msg);
            }
        }

        void StopBroker(string token)
        {
            IBroker b = FindBroker(token);
            if (b == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (!b.IsLive)//已经停止
            {
                throw new FutsRspError("通道已停止");
            }

            b.Stop();
        }

        ///// <summary>
        ///// 重启成交接口
        ///// </summary>
        ///// <param name="token"></param>
        //void RestartBroker(string token)
        //{
        //    IBroker b = FindBroker(token);
        //    if (b == null)//未找到
        //    {
        //        throw new FutsRspError("通道不存在");
        //    }
        //    if (b.IsLive)//已经启动则停止
        //    {
        //        b.Stop();
        //    }
        //    string msg = string.Empty;
        //    bool s = b.Start(out msg);
        //    if (!s)
        //    {
        //        throw new FutsRspError(msg);
        //    }
        //}

        void StartDataFeed(string token)
        {
            IDataFeed df = FindDataFeed(token);
            if (df == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (df.IsLive)//已经停止
            {
                throw new FutsRspError("通道已启动");
            }
            df.Start();
        }

        void StopDataFeed(string token)
        {
            IDataFeed df = FindDataFeed(token);
            if (df == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (!df.IsLive)//已经停止
            {
                throw new FutsRspError("通道已停止");
            }
            df.Stop();
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "StartConnector", "StartConnector - StartConnector", "启动通道")]
        public void CTE_StartConnector(ISession session, int id)
        {
            try
            {
                logger.Info("启动通道");
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {
                    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(id);
                    if (cfg.domain_id == manger.domain_id)//有权利
                    {
                        Action<ISession, ConnectorConfig, QSEnumConnectorOperation> action = new Action<ISession, ConnectorConfig, QSEnumConnectorOperation>(ConnectorOperation);
                        action.BeginInvoke(session, cfg, QSEnumConnectorOperation.Start, null, null);
                    }
                    else
                    {
                        throw new FutsRspError("无权操作该通道");
                    }
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "StopConnector", "StartConnector - StartConnector", "启动通道")]
        public void CTE_StopConnector(ISession session, int id)
        {
            try
            {
                logger.Info("停止通道");
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {
                    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(id);
                    if (cfg.domain_id == manger.domain_id)//有权利
                    {
                        Action<ISession, ConnectorConfig, QSEnumConnectorOperation> action = new Action<ISession, ConnectorConfig, QSEnumConnectorOperation>(ConnectorOperation);
                        action.BeginInvoke(session, cfg, QSEnumConnectorOperation.Stop, null, null);
                    }
                    else
                    {
                        throw new FutsRspError("无权操作该通道");
                    }
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


    }
}
