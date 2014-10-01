using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    public static class SerializationUtils
    {
        // Methods
        public static Dictionary<K, V> ReadDict<K, V>(SerializationReader reader)
        {
            int capacity = reader.ReadInt32();
            if (capacity == -1)
            {
                return null;
            }
            Dictionary<K, V> dictionary = new Dictionary<K, V>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                K local = (K)reader.ReadObject();
                V local2 = (V)reader.ReadObject();
                dictionary[local] = local2;
            }
            return dictionary;
        }

        public static List<T> ReadList<T>(SerializationReader reader) where T : IOwnedDataSerializableAndRecreatable, new()
        {
            int capacity = reader.ReadInt32();
            List<T> list = new List<T>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                T local2 = default(T);
                T item = (local2 == null) ? Activator.CreateInstance<T>() : default(T);
                item.DeserializeOwnedData(reader, null);
                list.Add(item);
            }
            return list;
        }

        public static Dictionary<Security, IList<Bar>> ReadSymbolBarList(SerializationReader reader)
        {
            int capacity = reader.ReadInt32();
            Dictionary<Security, IList<Bar>> dictionary = new Dictionary<Security, IList<Bar>>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                
                //symbol.DeserializeOwnedData(reader, null);
                string secstr = reader.ReadString();
                Security symbol = SecurityImpl.Deserialize(secstr);
                //dictionary[symbol] = ReadList<Bar>(reader);
            }
            return dictionary;
        }

        public static void WriteDict<K, V>(SerializationWriter writer, Dictionary<K, V> data)
        {
            if (data == null)
            {
                writer.Write(-1);
            }
            else
            {
                writer.Write(data.Count);
                foreach (KeyValuePair<K, V> pair in data)
                {
                    writer.WriteObject(pair.Key);
                    writer.WriteObject(pair.Value);
                }
            }
        }

        public static void WriteList<T>(SerializationWriter writer, IList<T> data) where T : IOwnedDataSerializableAndRecreatable
        {
            writer.Write(data.Count);
            foreach (T local in data)
            {
                local.SerializeOwnedData(writer, null);
            }
        }

        public static void WriteSymbolBarList(SerializationWriter writer, Dictionary<Security, IList<Bar>> dict)
        {
            writer.Write(dict.Count);
            foreach (KeyValuePair<Security, IList<Bar>> pair in dict)
            {
                //pair.Key.SerializeOwnedData(writer, null);
                writer.WriteStringDirect(SecurityImpl.Serialize(pair.Key));
                //WriteList<Bar>(writer, pair.Value);
            }
        }

        // Nested Types
        /*
        public static class Specialized
        {
            // Methods
            public static Dictionary<Symbol, RList<BarData>> ReadAccountInfoSymbols(SerializationReader reader)
            {
                Dictionary<Symbol, RList<BarData>> dictionary = new Dictionary<Symbol, RList<BarData>>();
                int num = reader.ReadInt32();
                for (int i = 0; i < num; i++)
                {
                    Symbol symbol = new Symbol();
                    symbol.DeserializeOwnedData(reader, null);
                    List<BarData> items = SerializationUtils.ReadList<BarData>(reader);
                    dictionary[symbol] = new RList<BarData>(items);
                }
                return dictionary;
            }

            public static void WriteAccountInfoSymbols(SerializationWriter writer, Dictionary<Symbol, RList<BarData>> accountInfoSymbols)
            {
                writer.Write(accountInfoSymbols.Count);
                foreach (KeyValuePair<Symbol, RList<BarData>> pair in accountInfoSymbols)
                {
                    pair.Key.SerializeOwnedData(writer, null);
                    SerializationUtils.WriteList<BarData>(writer, pair.Value.Items);
                }
            }
        }**/
    }

 

}
