///////////////////////////////////////////////////////////////////////////////////////
// 查询合约
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 注册合约实时行情数据
    /// </summary>
    public class RegisterSymbolTickRequest:RequestPacket
    {
        List<string> symlist;
        public RegisterSymbolTickRequest()
        {
            _type = API.MessageTypes.REGISTERSYMTICK;
            symlist = new List<string>();
        }

        /// <summary>
        /// 注册合约
        /// </summary>
        /// <param name="symbol"></param>
        public void Register(string symbol)
        {
            this.Register(new string[] { symbol });
        }

        /// <summary>
        /// 注册合约
        /// </summary>
        /// <param name="symbols"></param>
        public void Register(string[] symbols)
        {
            foreach (string sym in symbols)
            {
                if (string.IsNullOrEmpty(sym) || symlist.Contains(sym))
                    continue;
                symlist.Add(sym);
            }
        }

        public  List<string> SymbolList
        {
            get
            {
                return symlist;
            }
        }

        public string Symbols
        {
            get
            {
                return string.Join(",", symlist.ToArray());
            }  
        }

        public override string ContentSerialize()
        {
            return string.Join(",", symlist.ToArray());
        }

        public override void ContentDeserialize(string reqstr)
        {
            symlist.Clear();
            string[] rec = reqstr.Split(',');
            foreach (string sym in rec)
            {
                if (string.IsNullOrEmpty(sym))
                    continue;
                symlist.Add(sym);
            }
        }
    }

    /// <summary>
    /// 注销合约实时行情数据
    /// </summary>
    public class UnregisterSymbolTickRequest : RequestPacket
    {
        List<string> symlist;
        public UnregisterSymbolTickRequest()
        {
            _type = MessageTypes.UNREGISTERSYMTICK;
            symlist = new List<string>();
        }

        /// <summary>
        /// 注册合约
        /// </summary>
        /// <param name="symbol"></param>
        public void Unregister(string symbol)
        {
            this.Unregister(new string[] { symbol });
        }

        /// <summary>
        /// 注册合约
        /// </summary>
        /// <param name="symbols"></param>
        public void Unregister(string[] symbols)
        {
            foreach (string sym in symbols)
            {
                if (string.IsNullOrEmpty(sym) || symlist.Contains(sym))
                    continue;
                symlist.Add(sym);
            }
        }

        public List<string> SymbolList
        {
            get
            {
                return symlist;
            }
        }

        public string Symbols
        {
            get
            {
                return string.Join(",", symlist.ToArray());
            }
        }

        public override string ContentSerialize()
        {
            return string.Join(",", symlist.ToArray());
        }

        public override void ContentDeserialize(string reqstr)
        {
            symlist.Clear();
            string[] rec = reqstr.Split(',');
            foreach (string sym in rec)
            {
                if (string.IsNullOrEmpty(sym))
                    continue;
                symlist.Add(sym);
            }
        }


    }


}
