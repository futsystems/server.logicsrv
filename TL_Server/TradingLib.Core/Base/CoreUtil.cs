using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Data;
using System.Diagnostics;

namespace TradingLib.Core
{
    //public delegate void ClientTrackerInfoSessionDel(ClientTrackerInfo info,bool login_flag);
    public class CoreUtil
    {

        public const string CCFLAT = "CLEARCENTREFLAT";//清算中心强平标识
        public const string CCFLATERROR = "CLEARCENTREFLAT(Error)";//清算中心强平标识


        

        /// <summary>
        /// 是否在结算后与重置前
        /// </summary>
        /// <returns></returns>
        public static bool IsSettle2Reset()
        {
            DateTime now = DateTime.Now;
            if (now > DateTime.Parse("15:40:5") && now < DateTime.Parse("15:59"))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检查当前是否是星期六的0:00->2:30
        /// </summary>
        /// <returns></returns>
        public static bool IsSat230()
        {
            DateTime now = DateTime.Now;
            if(now.DayOfWeek != DayOfWeek.Saturday)//不是星期六 则直接返回false
                return false;
            if (now <DateTime.Parse("2:30"))
                return true;
            return false;
        }

        /// <summary>
        /// 通过账户参赛以来的数据统计得到账户某段时间盈利天数，亏损天数，连盈天数，连亏天数，累积手续费
        /// </summary>
        public static void StaDayReport00(DataTable dt, out int winday, out int lossday, out int awinday, out int alossday, out decimal totalcommission)
        {
            List<decimal> plist = new List<decimal>();
            totalcommission = 0;
            winday = 0;
            lossday = 0;
            awinday = 0;
            alossday = 0;

            int _awinday = 0;
            int _alossday = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                decimal profit = Convert.ToDecimal(dr["netprofit"]);//单日盈利
                decimal commission = Convert.ToDecimal(dr["commission"]);//单日手续费
                totalcommission += commission;//累积手续费
                if (profit >= 0)
                {
                    winday += 1;
                    _awinday += 1;//连盈+1
                    _alossday = 0;//连亏置0
                }
                else
                {
                    lossday += 1;
                    _awinday = 0;//连盈置0
                    _alossday += 1;//连亏+1
                }
                awinday = _awinday > awinday ? _awinday : awinday;//记录最大连盈日
                alossday = _alossday > alossday ? _alossday : _alossday;//记录最大连亏日
            }

        }

        public static void StopMiniMySql(string user,string pass,int port)//string user,string pass,int port)
        {
            string cmd = System.Environment.CurrentDirectory + "\\mysql-mini\\bin\\mysqladmin.exe";
            string cmdconfig = String.Format("  --port={0} --user={1} --password={2} shutdown",port,user,pass);
            RunCMD(cmd, cmdconfig);
        }
        public static void StartMiniMySql()
        {
            string cmd = System.Environment.CurrentDirectory + "\\mysql-mini\\bin\\mysqld.exe";
            string cmdconfig = "  --no-defaults --port=3306";
            RunCMD(cmd, cmdconfig);
        }

        public static void RunMySqlCmd(string mysqlcmd,string host,string port,string user,string pass,string dbname,string sqlfile)
        {
            string cmdconfig = String.Format(
                            "-C -B --host={0} -P {1} --user={2} --password={3} --database={4} -e \"\\. {5}\"",
                            host,port,user,pass,dbname, sqlfile);
            RunCMD(mysqlcmd, cmdconfig);
        }

        private static void RunCMD(string cmd, string config)
        {
            var process = Process.Start(
                   new ProcessStartInfo
                   {
                       FileName = cmd,
                       Arguments =config,
                       ErrorDialog = false,
                       CreateNoWindow = true,
                       UseShellExecute = false,
                       RedirectStandardError = true,
                       RedirectStandardInput = true,
                       RedirectStandardOutput = true,
                       WorkingDirectory = Environment.CurrentDirectory,
                   }
               );
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.StandardInput.Close();
            process.WaitForExit();
        }
        public static void StartCmd(String workingDirectory, String command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            p.Close();

        }
    }
}
