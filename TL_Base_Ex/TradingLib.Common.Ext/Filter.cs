using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace TradingLib.Common
{
    /// <summary>
    /// 对象过滤器
    /// </summary>
    public abstract class ObjectFilter
    {

        /* filter operators */
        protected const int EQUAL = 1;
        protected const int APPROX = 2;
        protected const int GREATER = 3;
        protected const int LESS = 4;
        protected const int PRESENT = 5;
        protected const int SUBSTRING = 6;
        protected const int AND = 7;
        protected const int OR = 8;
        protected const int NOT = 9;

        /** filter operation */
        protected  int op;

        /** filter attribute or null if operation AND, OR or NOT */
        protected  string attr;

        /** filter operands */
        protected  object value;

        /* normalized filter string for Filter object */
        [NonSerialized]
        private volatile string filterString;

        /// <summary>
        /// 根据参数初始化ObjectFilter的一个新实例
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        public ObjectFilter()
        { 
            
        }
        /*
        public ObjectFilter(int operation, string attr, object value)
        {
            this.op = operation;
            this.attr = attr;
            this.value = value;
        }**/

        /// <summary>
        /// 初始化过滤器,由于使用泛型模板,无法通过无参new()直接初始化对象,new出对象后通过调用init 进行初始化
        /// </summary>
        /// <param name="operation">操作符</param>
        /// <param name="attr">属性/字段</param>
        /// <param name="value">值</param>
        public void Init(int operation, string attr, object value)
        {
            this.op = operation;
            this.attr = attr;
            this.value = value;
        }
        /// <summary>
        /// 根据过滤语句创建ObjectFilter的实例
        /// 这里指定具体的filter对象
        /// </summary>
        /// <param name="filterString"></param>
        /// <returns></returns>
        public static T Create<T>(string filterString)
            where T : ObjectFilter, new()
        {
            return new Parser<T>(filterString).Parse();
        }


        #region [match 匹配逻辑]
        /// <summary>
        /// 返回dictionary是否匹配当前ObjectFilter
        /// 可以被子类重载
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public abstract bool Match(object obj);
        /*
        {
            Dictionary<string, object> dictionary = null;
            if (obj is Dictionary<string, object>)
                dictionary = obj as Dictionary<string, object>;
            return InnerMatch(ConvertToCaseInsensitiveDictionary(dictionary));
        }
        **/
        /*
        /// <summary>
        /// 返回dictionary是否匹配指定的过滤字符串
        /// 泛型匹配,指定使用那个对象匹配器进行对象匹配
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="filterString"></param>
        /// <returns></returns>
        public static  bool Matches<T>(object obj, string filterString)
            where T : ObjectFilter, new()
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                return true;
            }

            T filterImpl = Create<T>(filterString);
            return filterImpl.Match(obj);
        }
        **/

        /*
        /// <summary>
        /// 返回dictionary是否匹配当前ObjectFilter（键区分大小写）
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public bool MatchCase(Dictionary<string, object> dictionary)
        {
            return InnerMatch(dictionary);
        }
        **/
        /// <summary>
        /// 返回dictionary是否匹配当前ObjectFilter（具体实现）
        /// InnerMatch会产生递归调用,最终属性匹配时通过获得对象属性来获得匹配值
        /// 因此在子类实现GetAttr函数就可以实现不同对象的属性匹配，具体匹配规则在父类中定义
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        protected bool InnerMatch(object obj)
        {
            switch (op)
            {
                case AND:
                    {
                        ObjectFilter[] filters = (ObjectFilter[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            if (!filters[i].InnerMatch(obj))
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                case OR:
                    {
                        ObjectFilter[] filters = (ObjectFilter[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            if (filters[i].InnerMatch(obj))
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                case NOT:
                    {
                        ObjectFilter filter = (ObjectFilter)value;
                        return !filter.InnerMatch(obj);
                    }

                case SUBSTRING:
                case EQUAL:
                case GREATER:
                case LESS:
                case APPROX:
                    {
                        object prop = (obj == null) ? null : GetAttr(obj,attr);
                        return Compare(op, prop, value);
                    }

                case PRESENT:
                    {
                        object prop = (obj == null) ? null : GetAttr(obj, attr);
                        return prop != null;
                    }
            }

            return false;
        }


        public abstract object GetAttr(object obj,string attr);
        /*
        {
            Dictionary<string,object> dic=null;
            if(obj is Dictionary<string,object>)
            {
                dic = obj as Dictionary<string,object>;
                return dic[attr];
            }
            return null;
        }**/
        #endregion



        /// <summary>
        /// 将当前对象转换为对应的经过标准化处理的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = filterString;
            if (result == null)
            {
                filterString = result = Normalize();
            }
            return result;
        }

        /// <summary>
        /// 返回当前对象对应的经过标准化处理的字符串
        /// </summary>
        /// <returns></returns>
        private string Normalize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');

            switch (op)
            {
                case AND:
                    {
                        sb.Append('&');

                        ObjectFilter[] filters = (ObjectFilter[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            sb.Append(filters[i].Normalize());
                        }

                        break;
                    }

                case OR:
                    {
                        sb.Append('|');

                        ObjectFilter[] filters = (ObjectFilter[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            sb.Append(filters[i].Normalize());
                        }

                        break;
                    }

                case NOT:
                    {
                        sb.Append('!');
                        ObjectFilter filter = (ObjectFilter)value;
                        sb.Append(filter.Normalize());

                        break;
                    }

                case SUBSTRING:
                    {
                        sb.Append(attr);
                        sb.Append('=');

                        String[] substrings = (String[])value;

                        for (int i = 0, size = substrings.Length; i < size; i++)
                        {
                            String substr = substrings[i];

                            if (substr == null) /* * */
                            {
                                sb.Append('*');
                            }
                            else /* xxx */
                            {
                                sb.Append(EncodeValue(substr));
                            }
                        }

                        break;
                    }
                case EQUAL:
                    {
                        sb.Append(attr);
                        sb.Append('=');
                        sb.Append(EncodeValue((String)value));

                        break;
                    }
                case GREATER:
                    {
                        sb.Append(attr);
                        sb.Append(">=");
                        sb.Append(EncodeValue((String)value));

                        break;
                    }
                case LESS:
                    {
                        sb.Append(attr);
                        sb.Append("<=");
                        sb.Append(EncodeValue((String)value));

                        break;
                    }
                case APPROX:
                    {
                        sb.Append(attr);
                        sb.Append("~=");
                        sb.Append(EncodeValue(ApproxString((String)value)));

                        break;
                    }

                case PRESENT:
                    {
                        sb.Append(attr);
                        sb.Append("=*");

                        break;
                    }
            }

            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// 比较两个ObjectFilter是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is ObjectFilter))
            {
                return false;
            }

            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// 返回该字符串的Hash代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        /// <summary>
        /// Encode the value string such that '(', '*', ')' and '\' are escaped.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string EncodeValue(string value)
        {
            bool encoded = false;
            int inlen = value.Length;
            int outlen = inlen << 1; /* inlen 2 */

            char[] output = new char[outlen];
            value.CopyTo(0, output, inlen, inlen);

            int cursor = 0;
            for (int i = inlen; i < outlen; i++)
            {
                char c = output[i];

                switch (c)
                {
                    case '(':
                    case '*':
                    case ')':
                    case '\\':
                        {
                            output[cursor] = '\\';
                            cursor++;
                            encoded = true;

                            break;
                        }
                }

                output[cursor] = c;
                cursor++;
            }

            return encoded ? new string(output, 0, cursor) : value;
        }

        /// <summary>
        /// 比较对象值
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected bool Compare(int operation, object value1, object value2)
        {
            if (value1 == null)
            {
                return false;
            }

            if (value1 is string)
            {
                return CompareString(operation, (string)value1, value2);
            }

            Type clazz = value1.GetType();

            if (clazz.IsArray)
            {

                Type type = clazz.GetElementType();

                if (type.IsPrimitive)
                {
                    return ComparePrimitiveArray(operation, type, value1, value2);
                }

                return CompareObjectArray(operation, (object[])value1, value2);
            }
            if (value1 is ICollection)
            {
                return CompareCollection(operation, (ICollection)value1, value2);
            }

            if (value1 is int)
            {
                return CompareInteger(operation, (int)value1, value2);
            }
            if (value1 is long)
            {
                return CompareLong(operation, (long)value1, value2);
            }
            if (value1 is byte)
            {
                return CompareByte(operation, (byte)value1, value2);
            }
            if (value1 is short)
            {
                return CompareShort(operation, (short)value1, value2);
            }
            if (value1 is char)
            {
                return CompareCharacter(operation, (char)value1, value2);
            }
            if (value1 is float)
            {
                return CompareFloat(operation, (float)value1, value2);
            }
            if (value1 is double)
            {
                return Compare_Double(operation, (double)value1, value2);
            }
            if (value1 is bool)
            {
                return CompareBoolean(operation, (bool)value1, value2);
            }
            if (value1 is IComparable)
            {
                return CompareComparable(operation, (IComparable)value1, value2);
            }

            return CompareUnknown(operation, value1, value2); // RFC 59
        }

        #region 不同对象的比较
        private bool CompareCollection(int operation, ICollection collection, object value2)
        {
            for (IEnumerator iterator = collection.GetEnumerator(); iterator.MoveNext(); )
            {
                if (Compare(operation, iterator.Current, value2))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CompareObjectArray(int operation, object[] array, object value2)
        {
            for (int i = 0, size = array.Length; i < size; i++)
            {
                if (Compare(operation, array[i], value2))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ComparePrimitiveArray(int operation, Type type, object primarray, object value2)
        {
            if (typeof(int) == type)
            {
                int[] array = (int[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareInteger(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(long) == type)
            {
                long[] array = (long[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareLong(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(byte) == type)
            {
                byte[] array = (byte[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareByte(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(short) == type)
            {
                short[] array = (short[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareShort(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(char) == type)
            {
                char[] array = (char[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareCharacter(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(float) == type)
            {
                float[] array = (float[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareFloat(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(double) == type)
            {
                double[] array = (double[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (Compare_Double(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (typeof(bool) == type)
            {
                bool[] array = (bool[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (CompareBoolean(operation, array[i], value2))
                    {
                        return true;
                    }
                }
                return false;
            }

            return false;
        }

        private bool CompareString(int operation, string str, object value2)
        {
            switch (operation)
            {
                case SUBSTRING:
                    {
                        string[] substrings = (string[])value2;
                        int pos = 0;
                        for (int i = 0, size = substrings.Length; i < size; i++)
                        {
                            string substr = substrings[i];

                            if (i + 1 < size) /* if this is not that last substr */
                            {
                                if (substr == null) /* * */
                                {
                                    string substr2 = substrings[i + 1];

                                    if (substr2 == null) /* ** */
                                        continue; /* ignore first star */
                                    /* xxx */
                                    int index = str.IndexOf(substr2, pos);
                                    if (index == -1)
                                    {
                                        return false;
                                    }

                                    pos = index + substr2.Length;
                                    if (i + 2 < size) // if there are more
                                        // substrings, increment
                                        // over the string we just
                                        // matched; otherwise need
                                        // to do the last substr
                                        // check
                                        i++;
                                }
                                else /* xxx */
                                {
                                    int len = substr.Length;
                                    if (str.Substring(pos, len) == substr)
                                    {
                                        pos += len;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else /* last substr */
                            {
                                if (substr == null) /* * */
                                {
                                    return true;
                                }
                                /* xxx */
                                return str.EndsWith(substr);
                            }
                        }

                        return true;
                    }
                case EQUAL:
                    {
                        return str.Equals(value2);
                    }
                case APPROX:
                    {
                        str = ApproxString(str);
                        string string2 = ApproxString((string)value2);

                        return str.Equals(string2, StringComparison.CurrentCultureIgnoreCase); ;
                    }
                case GREATER:
                    {
                        return str.CompareTo((string)value2) >= 0;
                    }
                case LESS:
                    {
                        return str.CompareTo((string)value2) <= 0;
                    }
            }
            return false;
        }

        private bool CompareInteger(int operation, int intval, object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            int intval2 = int.Parse(((string)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return intval == intval2;
                    }
                case GREATER:
                    {
                        return intval >= intval2;
                    }
                case LESS:
                    {
                        return intval <= intval2;
                    }
            }
            return false;
        }

        private bool CompareLong(int operation, long longval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            long longval2 = long.Parse(((String)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return longval == longval2;
                    }
                case GREATER:
                    {
                        return longval >= longval2;
                    }
                case LESS:
                    {
                        return longval <= longval2;
                    }
            }
            return false;
        }

        private bool CompareByte(int operation, byte byteval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }
            byte byteval2 = byte.Parse(((string)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return byteval == byteval2;
                    }
                case GREATER:
                    {
                        return byteval >= byteval2;
                    }
                case LESS:
                    {
                        return byteval <= byteval2;
                    }
            }
            return false;
        }

        private bool CompareShort(int operation, short shortval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }
            short shortval2 = short.Parse(((string)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return shortval == shortval2;
                    }
                case GREATER:
                    {
                        return shortval >= shortval2;
                    }
                case LESS:
                    {
                        return shortval <= shortval2;
                    }
            }
            return false;
        }

        private bool CompareCharacter(int operation, char charval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }
            char charval2 = (((string)value2).Trim())[0];
            switch (operation)
            {
                case EQUAL:
                    {
                        return charval == charval2;
                    }
                case APPROX:
                    {
                        return (charval == charval2) || (char.ToUpper(charval) == char.ToUpper(charval2))
                        || (char.ToUpper(charval) == char.ToUpper(charval2));
                    }
                case GREATER:
                    {
                        return charval >= charval2;
                    }
                case LESS:
                    {
                        return charval <= charval2;
                    }
            }
            return false;
        }

        private bool CompareBoolean(int operation, bool boolval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }
            bool boolval2 = Boolean.Parse(((String)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                case GREATER:
                case LESS:
                    {
                        return boolval == boolval2;
                    }
            }
            return false;
        }

        private bool CompareFloat(int operation, float floatval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            float floatval2 = float.Parse(((string)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return floatval.CompareTo(floatval2) == 0;
                    }
                case GREATER:
                    {
                        return floatval.CompareTo(floatval2) >= 0;
                    }
                case LESS:
                    {
                        return floatval.CompareTo(floatval2) <= 0;
                    }
            }
            return false;
        }

        private bool Compare_Double(int operation, double doubleval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }
            double doubleval2 = double.Parse(((string)value2).Trim());
            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return doubleval.CompareTo(doubleval2) == 0;
                    }
                case GREATER:
                    {
                        return doubleval.CompareTo(doubleval2) >= 0;
                    }
                case LESS:
                    {
                        return doubleval.CompareTo(doubleval2) <= 0;
                    }
            }
            return false;
        }

        private static readonly Type[] constructorType = new Type[] { typeof(string) };
        

        private bool CompareComparable(int operation, IComparable value1, object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            ConstructorInfo constructor;
            try
            {
                constructor = value1.GetType().GetConstructor(constructorType);
            }
            catch (InvalidOperationException e)
            {
                return false;
            }

            try
            {
                if (!constructor.IsPublic)
                    constructor.Invoke(new object[] { ((string)value2).Trim() });
            }
            catch (InvalidOperationException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

            switch (operation)
            {
                case APPROX:
                case EQUAL:
                    {
                        return value1.CompareTo(value2) == 0;
                    }
                case GREATER:
                    {
                        return value1.CompareTo(value2) >= 0;
                    }
                case LESS:
                    {
                        return value1.CompareTo(value2) <= 0;
                    }
            }
            return false;
        }

        private bool CompareUnknown(int operation, Object value1, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            ConstructorInfo constructor;
            try
            {
                constructor = value1.GetType().GetConstructor(constructorType);
            }
            catch (InvalidOperationException e)
            {
                return false;
            }

            try
            {
                if (!constructor.IsPublic)
                    constructor.Invoke(new object[] { ((string)value2).Trim() });
            }
            catch (InvalidOperationException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

            switch (operation)
            {
                case APPROX:
                case EQUAL:
                case GREATER:
                case LESS:
                    {
                        return value1.Equals(value2);
                    }
            }
            return false;
        }

        #endregion

        /**
        * Map a string for an APPROX (~=) comparison.
        *
        * This implementation removes white spaces. This is the minimum
        * implementation allowed by the OSGi spec.
        *
        * @param input Input string.
        * @return String ready for APPROX comparison.
        */
        private static string ApproxString(string input)
        {
            bool changed = false;
            char[] output = input.ToCharArray();
            int cursor = 0;
            for (int i = 0, length = output.Length; i < length; i++)
            {
                char c = output[i];

                if (char.IsWhiteSpace(c))
                {
                    changed = true;
                    continue;
                }

                output[cursor] = c;
                cursor++;
            }

            return changed ? new String(output, 0, cursor) : input;
        }

        /*
        /// <summary>
        /// 将Dictionary转换为一个Key不区分大小写的Dictionary
        /// </summary>
        /// <param name="oldDic"></param>
        /// <returns></returns>
        private Dictionary<string, object> ConvertToCaseInsensitiveDictionary(Dictionary<string, object> oldDic)
        {
            Dictionary<string, object> newDic = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

            if (oldDic != null)
            {

                foreach (string key in oldDic.Keys)
                {

                    if (newDic.ContainsKey(key))
                    {
                        throw new ArgumentException();
                    }

                    newDic.Add(key, oldDic[key]);
                }
            }

            return newDic;
        }
        **/
        /// <summary>
        /// 解析过滤字符串为ObjectFilter对象树
        /// </summary>
        private class Parser<T>
            where T : ObjectFilter, new()
        {
            private readonly string filterstring;
            private readonly char[] filterChars;
            private int pos;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="filterstring"></param>
            public Parser(string filterstring)
            {
                this.filterstring = filterstring;
                this.filterChars = filterstring.ToCharArray();
                this.pos = 0;
            }

            /// <summary>
            /// 解析
            /// </summary>
            /// <returns></returns>
            public T Parse()
            {
                T filter;

                try
                {
                    filter = ParseFilter();
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new FormatException(string.Format("Filter ended abruptly: {0}", filterstring));
                }

                if (pos != filterChars.Length)
                {
                    throw new FormatException(string.Format("Extraneous trailing characters: {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                return filter;
            }

            private T ParseFilter()
            {
                T filter;
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    throw new FormatException(string.Format("Missing '(': {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                pos++;

                filter = ParseFilterComp();

                SkipWhiteSpace();

                if (filterChars[pos] != ')')
                {
                    throw new FormatException(string.Format("Missing ')': {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                pos++;

                SkipWhiteSpace();

                return filter;
            }

            private T ParseFilterComp()
            {
                SkipWhiteSpace();

                char c = filterChars[pos];

                switch (c)
                {
                    case '&':
                        {
                            pos++;
                            return ParseAnd();
                        }
                    case '|':
                        {
                            pos++;
                            return ParseOr();
                        }
                    case '!':
                        {
                            pos++;
                            return ParseNot();
                        }
                }
                return ParseItem();
            }

            private T ParseAnd()
            {
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    throw new FormatException(string.Format("Missing '(': {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                IList<T> operands = new List<T>(10);

                while (filterChars[pos] == '(')
                {
                    T child = ParseFilter();
                    operands.Add(child);
                }
                T t = new T();
                t.Init(ObjectFilter.AND, null, operands.ToArray());
                return t;
            }

            private T ParseOr()
            {
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    throw new FormatException(string.Format("Missing '(': {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                IList<T> operands = new List<T>();

                while (filterChars[pos] == '(')
                {
                    T child = ParseFilter();
                    operands.Add(child);
                }

                T t = new T();
                t.Init(ObjectFilter.OR, null, operands.ToArray());
                return t;
            }

            private T ParseNot()
            {
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    throw new FormatException(string.Format("Missing '(': {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                T child = ParseFilter();

                T t = new T();
                t.Init(ObjectFilter.NOT, null, child);
                return t;
            }

            private T ParseItem()
            {
                string attr = ParseAttr();

                SkipWhiteSpace();

                switch (filterChars[pos])
                {
                    case '~':
                        {
                            if (filterChars[pos + 1] == '=')
                            {
                                pos += 2;
                                T t = new T();
                                t.Init(ObjectFilter.APPROX, attr, ParseValue());
                                return t;
                            }
                            break;
                        }
                    case '>':
                        {
                            if (filterChars[pos + 1] == '=')
                            {
                                pos += 2;
                                T t = new T();
                                t.Init(ObjectFilter.GREATER, attr, ParseValue());
                                return t;
                            }
                            break;
                        }
                    case '<':
                        {
                            if (filterChars[pos + 1] == '=')
                            {
                                pos += 2;
                                T t = new T();
                                t.Init(ObjectFilter.LESS, attr, ParseValue());
                                return t;
                            }
                            break;
                        }
                    case '=':
                        {
                            if (filterChars[pos + 1] == '*')
                            {
                                int oldpos = pos;
                                pos += 2;
                                SkipWhiteSpace();
                                if (filterChars[pos] == ')')
                                {
                                    T t = new T();
                                    t.Init(ObjectFilter.PRESENT, attr, null);
                                    return t;
                                }
                                pos = oldpos;
                            }

                            pos++;
                            object str = ParseSubstring();

                            if (str is string)
                            {
                                T t = new T();
                                t.Init(ObjectFilter.EQUAL, attr, str);
                                return t;
                            }

                            T t2 = new T();
                            t2.Init(ObjectFilter.SUBSTRING, attr, str);
                            return t2;
                        }
                }

                throw new FormatException(string.Format("Invalid operator: {0} in {1}", filterstring.Substring(pos), filterstring));
            }

            private string ParseAttr()
            {
                SkipWhiteSpace();

                int begin = pos;
                int end = pos;

                char c = filterChars[pos];

                while (c != '~' && c != '<' && c != '>' && c != '=' && c != '(' && c != ')')
                {
                    pos++;

                    if (!char.IsWhiteSpace(c))
                    {
                        end = pos;
                    }

                    c = filterChars[pos];
                }

                int length = end - begin;

                if (length == 0)
                {
                    throw new FormatException(string.Format("Missing attr: {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                return new string(filterChars, begin, length);
            }

            private string ParseValue()
            {
                StringBuilder sb = new StringBuilder(filterChars.Length - pos);

                bool parseloop = true;
                while (parseloop)
                {

                    char c = filterChars[pos];

                    switch (c)
                    {
                        case ')':
                            {
                                parseloop = false;
                                break;
                            }

                        case '(':
                            {
                                throw new FormatException(string.Format("Invalid value: {0} in {1}", filterstring.Substring(pos), filterstring));
                            }

                        case '\\':
                            {
                                pos++;
                                c = filterChars[pos];
                                sb.Append(c);
                                pos++;
                                break;
                            }

                        default:
                            {
                                sb.Append(c);
                                pos++;
                                break;
                            }
                    }
                }

                if (sb.Length == 0)
                {
                    throw new FormatException(string.Format("Missing value: {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                return sb.ToString();
            }

            private object ParseSubstring()
            {
                StringBuilder sb = new StringBuilder(filterChars.Length - pos);

                List<string> operands = new List<string>();

                bool parseloop = true;
                while (parseloop)
                {
                    char c = filterChars[pos];

                    switch (c)
                    {

                        case ')':
                            {

                                if (sb.Length > 0)
                                {
                                    operands.Add(sb.ToString());
                                }

                                parseloop = false;
                                break;
                            }

                        case '(':
                            {
                                throw new FormatException(string.Format("Invalid value: {0} in {1}", filterstring.Substring(pos), filterstring));
                            }

                        case '*':
                            {
                                if (sb.Length > 0)
                                {
                                    operands.Add(sb.ToString());
                                }

                                sb.Length = 0;

                                operands.Add(null);
                                pos++;

                                break;
                            }

                        case '\\':
                            {
                                pos++;
                                c = filterChars[pos];
                                sb.Append(c);
                                pos++;
                                break;
                            }

                        default:
                            {
                                sb.Append(c);
                                pos++;
                                break;
                            }
                    }
                }

                if (operands.Count == 0)
                {
                    throw new FormatException(string.Format("Missing value: {0} in {1}", filterstring.Substring(pos), filterstring));
                }

                if (operands.Count == 1)
                {
                    object single = operands[0];
                    if (single != null)
                    {
                        return single;
                    }
                }

                return operands.ToArray();
            }

            /// <summary>
            /// 跳过空白
            /// </summary>
            private void SkipWhiteSpace()
            {
                for (int length = filterChars.Length; (pos < length) && string.IsNullOrWhiteSpace(filterChars[pos].ToString()); )
                {
                    pos++;
                }
            }
        }
    }
}