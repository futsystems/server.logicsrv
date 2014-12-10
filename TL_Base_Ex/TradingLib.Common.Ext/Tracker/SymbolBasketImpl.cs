using System;
using System.Collections.Generic;
using System.Collections;
using TradingLib.API;
using System.IO;


namespace TradingLib.Common
{
    /// <summary>
    /// Holds collections of securities.
    /// </summary>
    [Serializable]
    public class SymbolBasketImpl : SymbolBasket
    {
        /// <summary>
        /// 复制一个合约Basket
        /// </summary>
        /// <param name="copy"></param>
        public SymbolBasketImpl(SymbolBasket copy)
        {
            foreach (Symbol s in copy)
                Add(s);
            Name = copy.Name;
        }


        /// <summary>
        /// 从某个合约对象创建一个合约列表
        /// </summary>
        /// <param name="firstsec">security</param>
        public SymbolBasketImpl(Symbol firstsec)
        {
            Add(firstsec);
        }

        /// <summary>
        /// 从一组合约对象创建一个合约列表
        /// </summary>
        /// <param name="securities"></param>
        public SymbolBasketImpl(Symbol[] symbols)
        {
            foreach (Symbol s in symbols)
                Add(s);
        }
        public SymbolBasketImpl() { }

        public Symbol this[int index] { get { return symbols[index]; } set { symbols[index] = value; } }
        public Symbol this[string sym]
        {
            get
            {
                foreach (Symbol s in symbols)
                    if (s.Symbol == sym)
                        return s;
                return null;
            }
            set
            {
            }

        }
        ThreadSafeList<Symbol> symbols = new ThreadSafeList<Symbol>();
        string _name = "";
        public string Name { get { return _name; } set { _name = value; } }
        public int Count { get { return symbols.Count; } }
        public bool HasSymbol { get { return symbols.Count > 0; } }

        
        /// <summary>
        /// adds a security if not already present
        /// 如果某个合约对象不存在 则增加该合约
        /// </summary>
        /// <param name="s"></param>
        public void Add(Symbol s) 
        { 
            if (!contains(s)) 
                symbols.Add(s); 
        }


        //是否包含合约
        bool contains(string sym) { foreach (Symbol s in symbols) if (s.Symbol == sym) return true; return false; }
        bool contains(Symbol sec) { return symbols.Contains(sec); }

        /// <summary>
        /// 检查basket中是否含有某个特定的symbol
        /// </summary>
        /// <param name="s"></param>
        public bool HaveSymbol(string sym) { return contains(sym); }
        /// <summary>
        /// adds contents of another basket to this one.
        /// will not result in duplicate symbols
        /// </summary>
        /// <param name="mb"></param>
        public void Add(SymbolBasket mb)
        {
            for (int i = 0; i < mb.Count; i++)
                this.Add(mb[i]);
        }
        
        /// <summary>
        /// removes all elements of baskets that match.
        /// unmatching elements are ignored
        /// </summary>
        /// <param name="mb"></param>
        public void Remove(SymbolBasket mb)
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < symbols.Count; i++)
                for (int j = 0; j < mb.Count; j++)
                    if (symbols[i].Symbol == mb[j].Symbol)
                        remove.Add(i);
            for (int i = remove.Count - 1; i >= 0; i--)
                symbols.RemoveAt(remove[i]);
        }

        /// <summary>
        /// remove single symbol from basket
        /// </summary>
        /// <param name="symbol"></param>
        public void Remove(string symbol) 
        { 
            int i = -1; 
            for (int j = 0; j < symbols.Count; j++) 
            {
                if (symbols[j].Symbol == symbol) 
                    i = j; 
                if (i != -1) 
                    symbols.RemoveAt(i); 
            }
        }
        /// <summary>
        /// remove index of a particular symbol
        /// </summary>
        /// <param name="i"></param>
        public void Remove(int i) { symbols.RemoveAt(i); }
        /// <summary>
        /// remove security from basket
        /// </summary>
        /// <param name="s"></param>
        public void Remove(Symbol s) { symbols.Remove(s); }
        /// <summary>
        /// empty basket
        /// </summary>
        public void Clear() { symbols.Clear(); }

        public IEnumerator GetEnumerator() { foreach (SymbolImpl s in symbols) yield return s; }

        //获得Security数组
        public Symbol[] ToArray()
        {
            return symbols.ToArray();
        }
        //获得symbol数组
        public string[] ToSymArray()
        {
            string[] syms = new string[symbols.Count];
            for (int i = 0; i < syms.Length; i++)
                syms[i] = symbols[i].Symbol;
            return syms;
        }

    }
   
}
