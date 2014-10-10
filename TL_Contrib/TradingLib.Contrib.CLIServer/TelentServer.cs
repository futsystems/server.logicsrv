using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.CLI
{
    /// <summary>
    /// Telent服务器,用于接收命令行客户端登入以及执行服务端的管理与监控工作
    /// </summary>
    public class TelnetServer:BaseSrvObject
    {

        private int m_iPort = 0;
        private string m_sLogin;
        private string m_sPwd;
        private Dictionary<string, ContribCommand> m_Cmds = new Dictionary<string, ContribCommand>();
        private Socket m_ListeningSocket = null;
        private List<Socket> m_Connections = new List<Socket>();
        public TelnetServer(int iPort, List<ContribCommand> Cmds)
        {
            m_iPort = iPort;
            //将支持的命令复制到本地
            if (Cmds != null)
            {
                foreach (ContribCommand TC in Cmds)
                    if (!m_Cmds.ContainsKey(TC.Command))
                        m_Cmds.Add(TC.Command, TC);
            }
        }
        public TelnetServer(int iPort, string sLogin, string sPwd, List<ContribCommand> Cmds)
            : this(iPort, Cmds)
        {
            m_sLogin = sLogin;
            m_sPwd = sPwd;
        }
        Thread _telnetThread = null;
        bool _telnetgo = false;
        public void Start()
        {
            if (_telnetgo) return;
            _telnetgo = true;
            _telnetThread = new Thread(new ThreadStart(serverThread));
            _telnetThread.IsBackground = true;
            _telnetThread.Start();
        }
        public void Stop()
        {
            if (!_telnetgo) return;
            _telnetgo = false;
            try
            {
                //Shutdown the listener
                m_ListeningSocket.Close();
            }
            catch { }
            //Shutdown any open connections
            foreach (Socket S in m_Connections)
            {
                try
                {
                    S.Close();
                }
                catch { }
            }


            //LibUtil.WaitThreadStop(_telnetThread);


            if (_telnetThread != null && _telnetThread.IsAlive)
                _telnetThread.Abort();

        }
        private void serverThread()
        {
            try
            {
                //Open a listener socket and accept a connections
                TLCtxHelper.Debug("Telnet server start listening at:" + m_iPort.ToString());
                m_ListeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ListeningSocket.Bind(new System.Net.IPEndPoint(0, m_iPort));
                m_ListeningSocket.Listen(1);
                while (true)
                    runServer(m_ListeningSocket.Accept());
            }
            catch { } //If an exception occurs on the call to accept shutdown the server
        }
        /// <summary>
        /// 当有新的连接建立时,调用该函数进行处理与该socket之间的通讯
        /// </summary>
        /// <param name="S"></param>
        private void runServer(Socket S)
        {
            try
            {
                //debug("New telnet connection created",QSEnumDebugLevel.INFO);
                m_Connections.Add(S);
                S.Send(new byte[] 
                { 
                    (byte)TelnetOpCodes.IAC, 
                    (byte)TelnetOpCodes.DONT, 
                    (byte)TelnetOpCodes.ECHO 
                });
                S.Send(new byte[] 
                { 
                    (byte)TelnetOpCodes.IAC, 
                    (byte)TelnetOpCodes.WILL, 
                    (byte)TelnetOpCodes.SUPPRESSGO 
                });
                //登入
                while (!string.IsNullOrEmpty(m_sLogin) && !processLogin(S)) { };
                //处理请求
                processRequests(S);
                S.Close();
            }
            catch { }
        }
        /// <summary>
        /// 处理登入请求
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        private bool processLogin(Socket S)
        {
            List<byte> DataBytes = new List<byte>();
            List<byte> ProtocolBytes = new List<byte>();

            string sLogin = "";
            string sPwd = "";
            S.Send(Encoding.UTF8.GetBytes("\r\nlogin: "));

            while (DataBytes.Count < 1)
            {
                TelnetSocketReader.ReadData(S, new KeyValuePair<List<byte>, List<byte>>(ProtocolBytes, DataBytes));
                processProtcolData(ProtocolBytes);
            }
            sLogin = Encoding.GetEncoding("ISO-8859-1").GetString(DataBytes.ToArray(), 0, DataBytes.Count);
            sLogin = sLogin.Replace("\n", "");
            sLogin = sLogin.Replace("\r", "");
            DataBytes.Clear();
            ProtocolBytes.Clear();

            S.Send(Encoding.ASCII.GetBytes("password: "));
            while (DataBytes.Count < 1)
            {
                TelnetSocketReader.ReadData(S, new KeyValuePair<List<byte>, List<byte>>(ProtocolBytes, DataBytes), false);
                processProtcolData(ProtocolBytes);
            }
            sPwd = Encoding.GetEncoding("ISO-8859-1").GetString(DataBytes.ToArray(), 0, DataBytes.Count);
            sPwd = sPwd.Replace("\n", "");
            sPwd = sPwd.Replace("\r", "");

            if (m_sLogin.Equals(sLogin) && !string.IsNullOrEmpty(m_sPwd) && m_sPwd.Equals(sPwd))
                return true;
            else
                S.Send(Encoding.ASCII.GetBytes("\r\nIncorrect login or password\r\n"));
            return false;
        }


        void processProtcolData(List<byte> ProtocolBytes)
        {
            ProtocolBytes.Clear(); //We just toss the protcol bytes for now. Need to do something better
        }

        /// <summary>
        /// 处理该Socket过来的请求
        /// </summary>
        /// <param name="S"></param>
        private void processRequests(Socket S)
        {
            string sPrompt = "\r\n> ";
            S.Send(Encoding.ASCII.GetBytes(sPrompt));

            List<byte> DataBytes = new List<byte>();
            List<byte> ProtocolBytes = new List<byte>();
            string sCmd = "";
            while (true)
            {
                TelnetSocketReader.ReadData(S,
                    new KeyValuePair<List<byte>, List<byte>>(ProtocolBytes, DataBytes));
                processProtcolData(ProtocolBytes);
                sCmd += Encoding.GetEncoding("ISO-8859-1").GetString(DataBytes.ToArray(), 0, DataBytes.Count);
                //如果命令以"\r\n"结束,则表明是一个命令
                if (sCmd.EndsWith("\r\n"))
                {
                    DataBytes.Clear();
                    sCmd = sCmd.Replace("\n", "");
                    sCmd = sCmd.Replace("\r", "");

                    TLCtxHelper.Debug("Cli Command str:" + sCmd);

                    string[] cmdstrlsit = sCmd.Split(' ');
                    string cmd = "";
                    bool gotcmd = false;
                    List<string> args = new List<string>();
                    foreach (string s in cmdstrlsit)
                    {
                        //TLCtxHelper.Debug("CMDStr:" + s);
                        if (string.IsNullOrEmpty(s))
                            continue;
                        if (!gotcmd)
                        {
                            cmd = s;
                            gotcmd = true;
                            continue;
                        }
                        args.Add(s);
                    }
                    //TLCtxHelper.Debug("CMD:"+cmd + "  arg:"+string.Join(",",args));
                    //退出连接
                    if (cmd.ToLower() != "exit")
                    {
                        string re = TLCtxHelper.Ctx.MessageCLIHandler(cmd, string.Join(",", args));
                        if (string.IsNullOrEmpty(re))
                            re = "Executed Successfull";
                        S.Send(Encoding.UTF8.GetBytes(re));
                    }
                    else
                    {
                        //退出命令行连接
                        return; //Client requests to terminate connection
                    }
                    //命令处理完毕后将命令重置,并发送命令提示符
                    sCmd = "";
                    S.Send(Encoding.UTF8.GetBytes(sPrompt));
                }
            }
        }
    }



    class TelnetSocketReader
    {
        static public void ReadData(Socket S, KeyValuePair<List<byte>, List<byte>> ExtractedBytes)
        {
            ReadData(S, ExtractedBytes, true);
        }
        static public void ReadData(Socket S, KeyValuePair<List<byte>, List<byte>> ExtractedBytes, bool bEcho)
        {
            List<byte> ProtocolBytes = ExtractedBytes.Key;
            List<byte> DataBytes = ExtractedBytes.Value;
            byte[] InboundBuffer = new byte[1];
            string sCmd = "";
            while (true)
            {
                int iBytesRecd = S.Receive(InboundBuffer);

                if (InboundBuffer[0] == (byte)TelnetOpCodes.IAC)
                {
                    S.Receive(InboundBuffer); //Pull the protocol command off the socket
                    int iCmd = InboundBuffer[0];
                    switch (iCmd)
                    {
                        case (int)TelnetOpCodes.DO:
                        case (int)TelnetOpCodes.DONT:
                        case (int)TelnetOpCodes.WILL:
                        case (int)TelnetOpCodes.WONT:
                            {
                                S.Receive(InboundBuffer); //Pull the protocol option off the socket
                                ProtocolBytes.AddRange(new byte[] 
                            { 
                                (byte)TelnetOpCodes.IAC, 
                                (byte)iCmd, 
                                (byte)InboundBuffer[0] 
                            });
                                break;
                            }
                        default:
                            break;
                    }
                    if (ProtocolBytes.Count < 1)
                        ProtocolBytes.AddRange(new byte[] { (byte)TelnetOpCodes.IAC, (byte)iCmd });
                    return;
                }
                else
                {
                    if (InboundBuffer[0] == (byte)8)//8=keys.back 
                    {
                        //We provide some limiting command line editing here, process backspace.
                        //Pull the last byte we received off DataBytes, send appropriate response.
                        if (DataBytes.Count > 0)
                        {
                            DataBytes.RemoveAt(DataBytes.Count - 1);
                            S.Send(new byte[] { 27, 91, 68, 27, 91, 75 });
                        }
                    }
                    else
                    {
                        DataBytes.Add(InboundBuffer[0]);
                        if (bEcho)
                            S.Send(InboundBuffer);
                        sCmd += Encoding.GetEncoding("ISO-8859-1").GetString(DataBytes.ToArray(), 0, DataBytes.Count);
                        if (sCmd.EndsWith("\r\n"))
                            return;
                    }
                }
            }
        }
    }

    public enum TelnetOpCodes
    {
        //Majority of these not currently supported but listed here for completeness
        //We don't break out "commands" and "options" int seperate enum but just lump them all together for now (very simple server)
        //Below however we at least visibly seperate the options from the commands
        ECHO = 1,
        SUPPRESSGO = 3,     //When the interweb was young one side would talk, then wait and listen, then talk. Now everybody talks at the same time
        STATUS = 5,
        TIMINGMARK = 6,
        LOGOUT = 18,
        TTYPE = 24,         //Terminal Types 
        NAWS = 31,          //Window Size (Negotiate About Window Size). Allows for negotiate of the clients window size
        TSPEED = 32,
        LFLOW = 33,
        LINEMODE = 34,
        XLOCAL = 35,
        ENVIRON = 36,
        AUTHENTICATION = 37,
        ENCRYPTION = 38,
        NEWENV = 39,


        SE = 240,           //End a subnegotiation 
        NOP = 241,          //No Operation,  
        DM = 242,           //Data Mark - Idicates the position of a synch evebt within the data stream
        BRK = 243,          //Indicates "the break" key was hit
        IP = 244,           //Suspend - Abort the communication 
        AO = 245,           //Abort output - Process should continue to run to completion but stop sending output to the user
        AYT = 246,          //Are You There... is the other end still alive? If so respond (A NOP will do)
        EC = 247,           //Receiver should delete the last character received in the data stream
        EL = 248,           //Delete character back to the last  CLRF (but don't delete the CRLF)
        GA = 249,           //Go ahead. This was used along with "SUPPRESSGO" (or actually the lack there of). Tells the other end "I'm done talking, your turn"
        SB = 250,           //Subnegotiation - When the communication gets ver specific (aka going past the telnet protocol)

        //Will, Wont, Do, and Don't all require an option code to follow them
        WILL = 251,         //Indicates the desire to begin performing, or confirmation that
        //you are now performing, the indicated option
        WONT = 252,         //Indicates the refusal to perform, or continue performing, the indicated option.
        DO = 253,           //Indicates the request that the other party perform, or confirmation that you are expecting
        //the other party to perform, the indicated option
        DONT = 254,         //Indicates the demand that the other party stop performing,
        //or confirmation that you are no longer expecting the other party to perform, the indicated option
        IAC = 255           //Interrpret As Command. This is the guy sent down the stream that indicates we are processing a command rather than data
    }
}
