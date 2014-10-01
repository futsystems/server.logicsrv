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
    public class RegisterSymbolsRequest:RequestPacket
    {
        List<string> symlist = new List<string>();
        public RegisterSymbolsRequest()
        {
            _type = API.MessageTypes.REGISTERSTOCK;
        }

        /// <summary>
        /// 设定合约
        /// </summary>
        /// <param name="symbols"></param>
        public void SetSymbols(string[] symbols)
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

    public class UnregisterSymbolsRequest : RequestPacket
    {
        public UnregisterSymbolsRequest()
        {
            _type = MessageTypes.CLEARSTOCKS;
        }
    }


}
