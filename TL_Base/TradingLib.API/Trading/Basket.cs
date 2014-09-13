using System;
using System.Collections;

namespace TradingLib.API
{
    /*
    public interface Basket
    {

        string Name { get; set; }
        Symbol this[int index] { get; set; }
        Symbol this[string sym] { get; set; }
        int Count { get; }
        void Add(string symbol);
        void Add(string[] symbols);
        void Add(Symbol newsecurity);
        void Add(Basket newbasket);
        void Remove(Basket subtractbasket);
        void Remove(int i);
        void Remove(Symbol s);
        void Remove(string symbol);
        void Clear();
        bool HaveSymbol(string sym);
        Symbol[] ToArray();
        string[] ToSymArray();
        IEnumerator GetEnumerator();
    }**/

    /// <summary>
    /// 合约列表对象用于维护和管理一组合约
    /// </summary>
    public interface SymbolBasket
    {
        string Name { get; set; }
        Symbol this[int index] { get; set; }
        Symbol this[string sym] { get; set; }
        int Count { get; }
        void Add(string symbol);
        void Add(string[] symbols);
        void Add(Symbol newsymbol);
        void Add(SymbolBasket newbasket);
        void Remove(SymbolBasket subtractbasket);
        void Remove(int i);
        void Remove(Symbol s);
        void Remove(string symbol);
        void Clear();
        bool HaveSymbol(string sym);
        Symbol[] ToArray();
        string[] ToSymArray();
        IEnumerator GetEnumerator();

    }
}
