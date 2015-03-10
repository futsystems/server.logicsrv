using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;//记得加入此引用


using TradingLib.API;
using TradingLib.Common;


/*委托核心处理流程
 * 1.客户端发送直接委托
 * 2.客户发送快捷指令,服务端生成对应的委托
 * 3.管理端发送委托
 * 4.系统内部风控生成委托
 * 
 * 关于委托的状态
 * 1.委托初始状态为Unknown
 * 2.经过风控检查 满足接受委托条件后 委托状态->Placed 此时表面系统接受了该委托 但是没有提交到 成交接口
 * 3.在lock(account)内 系统将委托提交到BrokerRouter 然后通过Broker检查开平状态 并选择对应的成交接口对外发送(成交接口必须实现线程安全)
 * 4.成交接口返回委托需要有一定的时间延迟,因此在计算冻结资金时需要将提交到接口的Order也计算在内,否则当连续发送委托会导致在成交接口返回的这个时间差内,其他委托检查到的资金占用偏小(漏掉了已经发送到成交接口的委托)
 * 发送到成交接口 则资金就必须被冻结。因此在原有的Open/(委托在成交接口处于等待成交状态) partfilled Submited必须计算在冻结资金的范围内
 * 
 **/
namespace TradingLib.Core
{

    /// <summary>
    /// TradingServer是整体的中转站,他负责底层的tlserver处理将客户端请求进行逻辑处理后分发到对应其他的组件
    /// 并且接受其他组件回报过来的信息并转给客户端
    /// </summary>
    public partial class MsgExchServer : ExCore, IMessageExchange, IModuleExCore
    {
        const string CoreName = "MsgExch";
        

        public int ClientNum { get { return tl.NumClients; } }//连接的客户端数目
        public int ClientLoggedInNum { get { return tl.NumClientsLoggedIn; } }//登入的客户端数目
        
        bool _valid = false;
        public bool isValid { get { return _valid; } }

        TLServer_Exch tl;

        bool needConfirmSettlement = true;
        public MsgExchServer()
            : base(MsgExchServer.CoreName)
        {
            try
            {

                debug("初始化TradingServer");

                
                if (!_cfgdb.HaveConfig("TLServerIP"))
                {
                    _cfgdb.UpdateConfig("TLServerIP", QSEnumCfgType.String,"*", "TL_MQ监听IP地址");
                }
                if (!_cfgdb.HaveConfig("TLPort"))
                {
                    _cfgdb.UpdateConfig("TLPort", QSEnumCfgType.Int, 5570, "TL_MQ监听Base端口");
                }
                if (!_cfgdb.HaveConfig("VerbDebug"))
                {
                    _cfgdb.UpdateConfig("VerbDebug", QSEnumCfgType.Bool,false.ToString(), "是否输出verb日志");
                }

                if (!_cfgdb.HaveConfig("NeedConfirmSettlement"))
                {
                    _cfgdb.UpdateConfig("NeedConfirmSettlement", QSEnumCfgType.Bool,true, "是否需要确认结算单");
                }
                needConfirmSettlement = _cfgdb["NeedConfirmSettlement"].AsBool();

                tl = new TLServer_Exch(CoreName,_cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), true);

                //tl = new TLServer_Exch("TradingServer", _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt());
                //VerboseDebugging = _cfgdb["VerbDebug"].AsBool();
                tl.ProviderName = Providers.QSPlatform;
                tl.NumWorkers = 5;


                //设定日志输出
                //tl.VerboseDebugging = false;
                //tlserver内部直接发送的消息通过回调将消息缓存到外部缓存中进行队列发送
                tl.CachePacketEvent +=new IPacketDelegate(CachePacket);
                //查找对应交易账户
                tl.newLoginRequest += new LoginRequestDel<TrdClientInfo>(tl_newLoginRequest);
                //处理Symbol数据请求
                tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
                //处理Feature请求
                tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
                //处理Order提交
                tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
                //处理OrderAction操作
                tl.newOrderActionRequest += new OrderActionRequestDel(tl_newOrderActionRequest);

                //处理其他请求消息
                tl.newPacketRequest += new PacketRequestDel(tl_newPacketRequest);

                tl.ClientRegistedEvent += (TrdClientInfo c) =>
                    {
                        TLCtxHelper.EventSession.FireClientConnectedEvent(c);
                        //if (ClientRegistedEvent != null)
                        //{
                        //    ClientRegistedEvent(c);
                        //}
                        //debug("客户端:" + c.Location.ClientID + " 注册到系统", QSEnumDebugLevel.INFO);
                    };
                tl.ClientUnregistedEvent += (TrdClientInfo c) =>
                    {
                        TLCtxHelper.EventSession.FireClientDisconnectedEvent(c);
                        //if (ClientUnregistedEvent != null)
                        //{
                        //    ClientUnregistedEvent(c);
                        //}
                        //debug("客户端:" + c.Location.ClientID + " 从系统注销", QSEnumDebugLevel.INFO);
                    };
                tl.ClientLoginInfoEvent += (TrdClientInfo c, bool login) =>
                    {
                        TLCtxHelper.EventSession.FireClientLoginInfoEvent(c, login);
                        //if (ClientLoginInfoEvent != null)
                        //{
                        //    ClientLoginInfoEvent(c, login);
                        //}
                        debug("客户端:" + c.Location.ClientID + " 登入状态:"+login.ToString(), QSEnumDebugLevel.INFO);
                    };

                //初始化优先发送缓存对象
                InitPriorityBuffer();

                //启动消息服务
                StartMessageRouter();

                //订阅系统事件
                TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
            }
            catch (Exception ex)
            {
                debug("初始化服务异常:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                throw (new QSTradingServerInitError(ex));
            }
        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            this.Reset();

            
        }

        


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            tl.Dispose();
            tl = null;

            //是否将messagerouter放入stop start的地方
            StopMessageRouter();
        }






        #region 其他函数部分
        /// <summary>
        /// 返回某个交易帐户所有终端
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<ClientInfoBase> GetNotifyTargets(string account)
        {
            return tl.ClientsForAccount(account);
        }

        /// <summary>
        /// 恢复交易连接数据
        /// </summary>
        public void RestoreSession()
        {
            tl.RestoreSession();
        }

        void CachePacket(IPacket packet)
        {
            _packetcache.Write(packet);
        }



        #endregion

        #region 开始 停止部分
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            //debug("##########启动交易服务###################",QSEnumDebugLevel.INFO);
            try
            {
                tl.Start();
                
            }
            catch (Exception ex)
            {
                _valid = false;
                return;
            }
            _valid = true;
            if (_valid)
            {
                debug("Trading Server Starting success");
                Notify("启动", "启动时间:" + DateTime.Now.ToString());
            }
            else
                debug("Trading Server Starting failed.");
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            if (tl != null && tl.IsLive)
            {
                tl.Stop();
            }
        }
        #endregion
    }
}
