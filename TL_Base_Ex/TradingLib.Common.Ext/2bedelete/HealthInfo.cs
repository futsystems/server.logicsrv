using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /*
    public class HealthInfo : IHealthInfo
    {

        public double CPUTotal { get; set; }//CPU利用率
        public double CPUQS { get; set; }//CPU利用率
        public double MemoryQS { get; set; }//服务端进程内存使用量
        public double CPUMysql { get; set; }//CPU利用率
        public double MemoryMysql { get; set; }//服务端进程内存使用量

        public double BandWidthUsage { get; set; }//网络带宽使用量
        public int AccountClientNum { get; set; }//交易终端个数
        public int AccountClientLogedInNum { get; set; }//交易终端登入个数
        public int CustomerClientNum { get; set; }//用户终端个数
        public int CustomerClientLogedInNum { get; set; }//用户终端登入个数

        public int MysqlConnection { get; set; }//数据库连接个数
        public int MysqlQueryNum { get; set; }//数据库平均查询数量

        public int CacheOrder { get; set; }//缓存中的委托
        public int CacheCancel { get; set; }//缓存中的取消
        public int CacheTrade { get; set; }//缓存中的成交
        public int CachePosTrans { get; set; }//缓存中的仓位变化
        public int CacheOrderUpdate { get; set; }//缓存中委托更新

        public static string Series(IHealthInfo h)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(h.CPUTotal);
            sb.Append(d);
            sb.Append(h.CPUQS);
            sb.Append(d);
            sb.Append(h.MemoryQS);
            sb.Append(d);
            sb.Append(h.CPUMysql);
            sb.Append(d);
            sb.Append(h.MemoryMysql);
            sb.Append(d);
            sb.Append(h.BandWidthUsage);
            sb.Append(d);
            sb.Append(h.AccountClientNum);
            sb.Append(d);
            sb.Append(h.AccountClientLogedInNum);
            sb.Append(d);
            sb.Append(h.CustomerClientNum);
            sb.Append(d);
            sb.Append(h.CustomerClientLogedInNum);
            sb.Append(d);
            sb.Append(h.MysqlConnection);
            sb.Append(d);
            sb.Append(h.MysqlQueryNum);
            sb.Append(d);
            sb.Append(h.CacheOrder);
            sb.Append(d);
            sb.Append(h.CacheCancel);
            sb.Append(d);
            sb.Append(h.CacheTrade);
            sb.Append(d);
            sb.Append(h.CachePosTrans);
            sb.Append(d);
            sb.Append(h.CacheOrderUpdate);

            return sb.ToString();

        }

        public static IHealthInfo Deseries(string msg)
        {
            string[] p = msg.Split(',');
            HealthInfo h = new HealthInfo();
            h.CPUTotal = double.Parse(p[0]);
            h.CPUQS = double.Parse(p[1]);
            h.MemoryQS = double.Parse(p[2]);
            h.CPUMysql = double.Parse(p[3]);
            h.MemoryMysql = double.Parse(p[4]);
            h.BandWidthUsage = double.Parse(p[5]);

            h.AccountClientNum = int.Parse(p[6]);
            h.AccountClientLogedInNum = int.Parse(p[7]);
            h.CustomerClientNum = int.Parse(p[8]);
            h.CustomerClientLogedInNum = int.Parse(p[9]);
            h.MysqlConnection = int.Parse(p[10]);
            h.MysqlQueryNum = int.Parse(p[11]);
            h.CacheOrder = int.Parse(p[12]);
            h.CacheCancel = int.Parse(p[13]);
            h.CacheTrade = int.Parse(p[14]);
            h.CachePosTrans = int.Parse(p[15]);
            h.CacheOrderUpdate = int.Parse(p[16]);
            return h;
        }

    }**/
}
