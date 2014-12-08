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
        //List<Security> symbols = new List<Security>();//储存security的列表
        ThreadSafeList<Symbol> symbols = new ThreadSafeList<Symbol>();
        string _name = "";
        public string Name { get { return _name; } set { _name = value; } }
        public int Count { get { return symbols.Count; } }
        public bool HasSymbol { get { return symbols.Count > 0; } }

        /// <summary>
        /// adds a security if not already present
        /// 通过合约字头增加某个合约对象,如果不存在该合约对象则不增加
        /// </summary>
        /// <param name="sym"></param>
        //public void Add(string sym) 
        //{ 
        //    Symbol osym = BasicTracker.SymbolTracker[sym];
        //    if(osym != null)
        //    {
        //        Add(osym); 
        //    }
        //}
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
        /// 通过合约字段列表 增加合约
        /// </summary>
        ///// <param name="syms"></param>
        //public void Add(string[] syms)
        //{
        //    for (int i = 0; i < syms.Length; i++)
        //        this.Add(syms[i]);
        //}
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


        //序列化basket
        //public static string Serialize(SymbolBasket b)
        //{
        //    List<string> s = new List<string>();
        //    for (int i = 0; i < b.Count; i++) s.Add(b[i].Symbol);
        //    return string.Join(",", s.ToArray());
        //}
        ////反序列化basket
        //public static SymbolBasketImpl Deserialize(string serialBasket)
        //{
        //    SymbolBasketImpl mb = new SymbolBasketImpl();
        //    if ((serialBasket == null) || (serialBasket == "")) return mb;
        //    string[] r = serialBasket.Split(',');//字符串,分割 通过SecurityImpl进行解析
        //    for (int i = 0; i < r.Length; i++)
        //    {
        //        if (r[i] == "") continue;
        //        string[] syms = r[i].Split(' ');
        //        Symbol sec = BasicTracker.SymbolTracker[syms[0]];//SecurityImpl.Parse(r[i]);
        //        if (sec!= null)
        //            mb.Add(sec);
        //    }
        //    return mb;
        //}
        /*
        public static Basket FromFile(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                string file = sr.ReadToEnd();
                sr.Close();
                string[] syms = file.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                BasketImpl b = new BasketImpl(syms);
                b.Name = Path.GetFileNameWithoutExtension(filename);
                return b;
            }
            catch { }
            return new BasketImpl();
        }

        public static void ToFile(Basket b, string filename) { ToFile(b, filename, false); }
        public static void ToFile(Basket b, string filename, bool append)
        {
            StreamWriter sw = new StreamWriter(filename, append);
            for (int i = 0; i < b.Count; i++)
                sw.WriteLine(b[i].Symbol);
            sw.Close();
        }
            
        
            * **/
        //public override string ToString() { return Serialize(this); }
        //public static SymbolBasketImpl FromString(string serialbasket) { return Deserialize(serialbasket); }
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
