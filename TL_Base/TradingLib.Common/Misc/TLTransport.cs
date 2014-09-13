using TradingLib.API;
using System.Net;

namespace TradingLib.Common
{
    /// <summary>
    /// transport type
    /// </summary>
    public enum TLTransportType
    {
        None,
        IPCWM,
        TCPIP,
    }

    /// <summary>
    /// ip util class
    /// </summary>
    public class IPUtil
    {
        /// <summary>
        /// true if an ip address in string format is valid
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        public static bool isValidAddress(string ipaddr)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipaddr);
                return true;
            }
            catch {  }
            return false;
            
        }
        /// <summary>
        /// returns true if an array has at least one valid address
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public static bool hasValidAddress(string[] servers)
        {
            bool ok = false;
            foreach (string server in servers)
                ok |= isValidAddress(server);
            return ok;
        }

        public const int PRIVATEPORTSTART = 49152;
        public const int PRIVATEPORTEND = 65535;
        public const int TLDEFAULTBASEPORT = 5570;//交易通讯端口 5571(DNS) 5572(Tick)
        public const int TLDEFAULTTICKPORT = 5572;
        public const int TLDEFAULTMGRPORT = 6670;//管理端口
        public const int TLDEFAULTTESTPORT = 51000;
        public const string LOCALHOST = "127.0.0.1";
        public const string CLIENTNAME = "QSTradingClient";
        public const string SERVERNAME = "QSTradingServer";
        
        //====空消息内容
        public const string EMPTYMESSAGE = "ACK";

        //====服务hello server=============================================================================
        /// <summary>
        /// 在查询服务端是否有服务存在时,通过HelloServer来确认服务端服务存在,设定helloserver回报延迟
        /// </summary>
        public const int SOCKETREPLAYTIMEOUT = 5;

        /*
         客户端请求确认服务端存活的时间间隔可以放大一点,防止大量的heartbeatrequest请求
         
         */
        
        //====心跳机制常量==================================================================================
        //行情心跳机制
        public const int TICKHEARTBEATMS = 15 * 1000;//发送行情heartbeat的间隔

        public const int TICKHEARTBEATDEADMS = TICKHEARTBEATMS * 4; //在多少时间内没有收到tickheartbead则重新建立行情连接

        public const int TICKHEARTBEATCHECKFREQ = 1 * 1000;//行情心跳维护线程的检查频率
        /// <summary>
        /// 多少秒内没有收到服务端消息,则请求心跳回报,用于检测服务端是否存活 这里增加时间间隔到30秒
        /// </summary>
        public const int SENDHEARTBEATMS = 1*60*1000;//发送心跳请求延迟(单位秒,15秒内没有得到服务器的任何消息,则发送心跳请求)
        /// <summary>
        /// 在多少个请求间隔内没有得到服务端的心跳请求回报,则认为与服务端的连接丢失,客户端会尝试重新建立连接
        /// </summary>
        public const int HEARTBEATDEADMS = SENDHEARTBEATMS * 4;//(在4个心跳请求间隔内服务器没有回应,表明连接丢失,尝试重新建立连接)
        /// <summary>
        /// 默认检查服务器心跳回报间隔 20MS(心跳维护线程的刷新频率,每5MS检查上次服务器心跳时间)
        /// </summary>
        public const int DEFAULTWAIT = 20;

        //注:客户端按一定频率向服务端发送心跳信息,会再用一定量的服务器消息处理量
        //客户端10秒/次 那么10个客户端就是每秒1次,1000个客户端就是每秒100条心跳信息,这里需要延长发送间隔
        /// <summary>
        /// 默认向服务端发送心跳信息间隔 30秒(每30秒告诉服务器,客户端在运行)
        /// </summary>
        public const int HEARTBEATPERIOD = 30;//注意 clientsession文本化是按照heartbeat来进行的

        /// <summary>
        /// 断开连接后多少秒尝试连接一次服务器 默认5秒尝试一次连接
        /// </summary>
        public const int RECONNECTDELAY = 5;

        /// <summary>
        ///尝试连接次数
        /// </summary>
        public const int RECONNECTTIMES = 20;
        /// <summary>
        /// 客户端死亡时间,如果在多少秒内,没有收到客户端消息,则认为客户已经死亡,需要主动清理客户端列别以及SessionLoger中的数据库信息
        /// 2个heartbeatperiod内 如果没有收到客户端的心跳信息,则认为连接失效
        /// </summary>
        public const int CLIENTDEADTIME = HEARTBEATPERIOD * 3;

        /// <summary>
        /// 多少时间进行一次dead client清理,4个心跳间隔周期进行一次dead client 清除
        /// </summary>
        public const int CLEARDEADSESSIONPERIOD = HEARTBEATPERIOD*6;


        
        //====流控部分的系统常量=============================================================================
        /// <summary>
        /// 信息数起步检查,当某个客户端发送信息超过多少条时,进行流控检查
        /// </summary>
        public const int TPStartNum = 100;//当消息数量累计到多少时开始启动检测
        /// <summary>
        /// 当启动流控检查后,检查多少条信息来确定流控速度
        /// </summary>
        public const int TPCheckNum = 10000;//启动检测后跟踪消息的数目(在这个数目内计算TP)
        /// <summary>
        /// 当TP值大于多少时,拒绝客户端发送的消息
        /// </summary>
        public const double TPRejectValue = 1.5;//TP数值达到多少后拒绝该地址的消息
        /// <summary>
        /// 当TP值低于多少时,
        /// </summary>
        public const double TPStopValue = 1;//Tp数值降低到多少后停止检测
    }
}