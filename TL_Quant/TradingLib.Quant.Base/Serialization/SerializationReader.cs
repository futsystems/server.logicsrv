using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Collections;
using System.Diagnostics;


using System.Runtime.Serialization.Formatters.Binary;
namespace TradingLib.Quant.Base
{
    public sealed class SerializationReader : BinaryReader
    {
        // Fields
        private long x16e2b67bb7cbd5e8;
        private string[] x53a040afa62b19d1;
        private static readonly BitArray xd297edfab8187def = new BitArray(0);
        private object[] xd2a26bd8068e723c;

        // Methods
        public SerializationReader(byte[] data)
            : this(new MemoryStream(data))
        {
        }

        public SerializationReader(Stream stream)
            : base(stream)
        {
            this.x16e2b67bb7cbd5e8 = this.ReadInt64();
            stream.Position = this.x16e2b67bb7cbd5e8;
            this.x53a040afa62b19d1 = new string[this.ReadOptimizedInt32()];
            for (int i = 0; i < this.x53a040afa62b19d1.Length; i++)
            {
                this.x53a040afa62b19d1[i] = base.ReadString();
            }
            this.xd2a26bd8068e723c = new object[this.ReadOptimizedInt32()];
            for (int j = 0; j < this.xd2a26bd8068e723c.Length; j++)
            {
                this.xd2a26bd8068e723c[j] = this.ReadObject();
            }
            stream.Position = 8;
        }

        [Conditional("DEBUG")]
        public void DumpStringTables(ArrayList list)
        {
            list.AddRange(this.x53a040afa62b19d1);
        }

        public ArrayList ReadArrayList()
        {
            if (this.x69810e1eaf86c507() == SerializedType.NullType)
            {
                return null;
            }
            return new ArrayList(this.ReadOptimizedObjectArray());
        }

        public BitArray ReadBitArray()
        {
            if (this.x69810e1eaf86c507() == SerializedType.NullType)
            {
                return null;
            }
            return this.ReadOptimizedBitArray();
        }

        public BitVector32 ReadBitVector32()
        {
            return new BitVector32(this.ReadInt32());
        }

        public bool[] ReadBooleanArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new bool[0];
            }
            return this.x433c453d4c75e26e();
        }

        public byte[] ReadByteArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new byte[0];
            }
            return this.x4e16ed72ad2b50d4();
        }

        public byte[] ReadBytesDirect(int count)
        {
            return base.ReadBytes(count);
        }

        public char[] ReadCharArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new char[0];
            }
            return this.x778ccab42919df76();
        }

        public DateTime ReadDateTime()
        {
            return new DateTime(this.ReadInt64());
        }

        public DateTime[] ReadDateTimeArray()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new DateTime[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            DateTime[] timeArray = new DateTime[this.ReadOptimizedInt32()];
            for (int i = 0; i < timeArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    timeArray[i] = this.ReadDateTime();
                }
                else
                {
                    timeArray[i] = this.ReadOptimizedDateTime();
                }
            }
            return timeArray;
        }

        public decimal[] ReadDecimalArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new decimal[0];
            }
            return this.x1740117a7178c2f4();
        }

        public double[] ReadDoubleArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new double[0];
            }
            return this.x66b89805c2e9a28d();
        }

        public Guid ReadGuid()
        {
            return new Guid(this.ReadBytes(0x10));
        }

        public Guid[] ReadGuidArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new Guid[0];
            }
            return this.xf5abc6ffb6161760();
        }

        public short[] ReadInt16Array()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new short[0];
            }
            return this.x805cf4bf61232cfa();
        }

        public int[] ReadInt32Array()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new int[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            int[] numArray = new int[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    numArray[i] = this.ReadInt32();
                }
                else
                {
                    numArray[i] = this.ReadOptimizedInt32();
                }
            }
            return numArray;
        }

        public long[] ReadInt64Array()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new long[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            long[] numArray = new long[this.ReadOptimizedInt64()];
            for (int i = 0; i < numArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    numArray[i] = this.ReadInt64();
                }
                else
                {
                    numArray[i] = this.ReadOptimizedInt64();
                }
            }
            return numArray;
        }

        public object ReadObject()
        {
            return this.xdec6687f61b4db71((SerializedType)this.ReadByte());
        }

        public object[] ReadObjectArray()
        {
            return this.ReadObjectArray(null);
        }

        public object[] ReadObjectArray(Type elementType)
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyObjectArrayType:
                    if (elementType != null)
                    {
                        return (object[])Array.CreateInstance(elementType, 0);
                    }
                    return new object[0];

                case SerializedType.EmptyTypedArrayType:
                    throw new Exception();
            }
            return this.ReadOptimizedObjectArray(elementType);
        }

        public ArrayList ReadOptimizedArrayList()
        {
            return new ArrayList(this.ReadOptimizedObjectArray());
        }

        public BitArray ReadOptimizedBitArray()
        {
            int num = this.ReadOptimizedInt32();
            if (num == 0)
            {
                return xd297edfab8187def;
            }
            return new BitArray(base.ReadBytes((num + 7) / 8)) { Length = num };
        }

        public BitVector32 ReadOptimizedBitVector32()
        {
            return new BitVector32(base.Read7BitEncodedInt());
        }

        public bool[] ReadOptimizedBooleanArray()
        {
            return this.ReadBooleanArray();
        }

        public DateTime ReadOptimizedDateTime()
        {
            BitVector32 vector = new BitVector32((this.ReadByte() | (this.ReadByte() << 8)) | (this.ReadByte() << 0x10));
            DateTime time = new DateTime(vector[SerializationWriter.DateYearMask], vector[SerializationWriter.DateMonthMask], vector[SerializationWriter.DateDayMask]);
            if (vector[SerializationWriter.DateHasTimeOrKindMask] != 1)
            {
                return time;
            }
            byte num = this.ReadByte();
            if (num == 0)
            {
                this.ReadByte();
                return time;
            }
            return time.Add(this.x323cad03cb8afc4d(num));
        }

        public DateTime[] ReadOptimizedDateTimeArray()
        {
            return this.ReadDateTimeArray();
        }

        public decimal ReadOptimizedDecimal()
        {
            byte num = this.ReadByte();
            int lo = 0;
            int mid = 0;
            int hi = 0;
            byte scale = 0;
            if ((num & 2) != 0)
            {
                scale = this.ReadByte();
            }
            if ((num & 4) == 0)
            {
                if ((num & 0x20) != 0)
                {
                    lo = this.ReadOptimizedInt32();
                }
                else
                {
                    lo = this.ReadInt32();
                }
            }
            if ((num & 8) == 0)
            {
                if ((num & 0x40) != 0)
                {
                    mid = this.ReadOptimizedInt32();
                }
                else
                {
                    mid = this.ReadInt32();
                }
            }
            if ((num & 0x10) == 0)
            {
                if ((num & 0x80) != 0)
                {
                    hi = this.ReadOptimizedInt32();
                }
                else
                {
                    hi = this.ReadInt32();
                }
            }
            return new decimal(lo, mid, hi, (num & 1) != 0, scale);
        }

        public decimal[] ReadOptimizedDecimalArray()
        {
            return this.ReadDecimalArray();
        }

        public int ReadOptimizedInt32()
        {
            byte num3;
            int num = 0;
            int num2 = 0;
            do
            {
                num3 = this.ReadByte();
                num |= (num3 & 0x7f) << num2;
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);
            return num;
        }

        public int[] ReadOptimizedInt32Array()
        {
            return this.ReadInt32Array();
        }

        public long ReadOptimizedInt64()
        {
            byte num3;
            long num = 0;
            int num2 = 0;
            do
            {
                num3 = this.ReadByte();
                num |= (num3 & 0x7f) << num2;
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);
            return num;
        }

        public long[] ReadOptimizedInt64Array()
        {
            return this.ReadInt64Array();
        }

        public object[] ReadOptimizedObjectArray()
        {
            return this.ReadOptimizedObjectArray(null);
        }

        public object[] ReadOptimizedObjectArray(Type elementType)
        {
            int length = this.ReadOptimizedInt32();
            object[] objArray = (elementType == null) ? new object[length] : ((object[])Array.CreateInstance(elementType, length));
            for (int i = 0; i < objArray.Length; i++)
            {
                SerializedType type = (SerializedType)this.ReadByte();
                switch (type)
                {
                    case SerializedType.NullSequenceType:
                        i += this.ReadOptimizedInt32();
                        break;

                    case SerializedType.DuplicateValueSequenceType:
                        {
                            object obj2 = objArray[i] = this.ReadObject();
                            int num3 = this.ReadOptimizedInt32();
                            while (num3-- > 0)
                            {
                                objArray[++i] = obj2;
                            }
                            break;
                        }
                    case SerializedType.DBNullSequenceType:
                        {
                            int num4 = this.ReadOptimizedInt32();
                            objArray[i] = DBNull.Value;
                            while (num4-- > 0)
                            {
                                objArray[++i] = DBNull.Value;
                            }
                            break;
                        }
                    default:
                        if (type != SerializedType.NullType)
                        {
                            objArray[i] = this.xdec6687f61b4db71(type);
                        }
                        break;
                }
            }
            return objArray;
        }

        public void ReadOptimizedObjectArrayPair(out object[] values1, out object[] values2)
        {
            values1 = this.ReadOptimizedObjectArray(null);
            values2 = new object[values1.Length];
            for (int i = 0; i < values2.Length; i++)
            {
                SerializedType type = (SerializedType)this.ReadByte();
                switch (type)
                {
                    case SerializedType.DuplicateValueType:
                        values2[i] = values1[i];
                        break;

                    case SerializedType.NullSequenceType:
                        i += this.ReadOptimizedInt32();
                        break;

                    case SerializedType.DBNullSequenceType:
                        {
                            int num3 = this.ReadOptimizedInt32();
                            values2[i] = DBNull.Value;
                            while (num3-- > 0)
                            {
                                values2[++i] = DBNull.Value;
                            }
                            break;
                        }
                    case SerializedType.DuplicateValueSequenceType:
                        {
                            values2[i] = values1[i];
                            int num2 = this.ReadOptimizedInt32();
                            while (num2-- > 0)
                            {
                                values2[++i] = values1[i];
                            }
                            break;
                        }
                    default:
                        if (type != SerializedType.NullType)
                        {
                            values2[i] = this.xdec6687f61b4db71(type);
                        }
                        break;
                }
            }
        }

        public string ReadOptimizedString()
        {
            SerializedType type = this.x69810e1eaf86c507();
            if (type < SerializedType.NullType)
            {
                return this.x84728a32b0ed8543((int)type);
            }
            if (type == SerializedType.NullType)
            {
                return null;
            }
            if (type == SerializedType.YStringType)
            {
                return "Y";
            }
            if (type == SerializedType.NStringType)
            {
                return "N";
            }
            if (type == SerializedType.SingleCharStringType)
            {
                return char.ToString(this.ReadChar());
            }
            if (type == SerializedType.SingleSpaceType)
            {
                return " ";
            }
            if (type != SerializedType.EmptyStringType)
            {
                throw new InvalidOperationException("Unrecognized TypeCode");
            }
            return string.Empty;
        }

        public string[] ReadOptimizedStringArray()
        {
            return (string[])this.ReadOptimizedObjectArray(typeof(string));
        }

        public TimeSpan ReadOptimizedTimeSpan()
        {
            return this.x323cad03cb8afc4d(this.ReadByte());
        }

        public TimeSpan[] ReadOptimizedTimeSpanArray()
        {
            return this.ReadTimeSpanArray();
        }

        public Type ReadOptimizedType()
        {
            return this.ReadOptimizedType(true);
        }

        public Type ReadOptimizedType(bool throwOnError)
        {
            return Type.GetType(this.ReadOptimizedString(), throwOnError);
        }

        public uint ReadOptimizedUInt32()
        {
            byte num3;
            uint num = 0;
            int num2 = 0;
            do
            {
                num3 = this.ReadByte();
                num |= (uint)((num3 & 0x7f) << num2);
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);
            return num;
        }

        public uint[] ReadOptimizedUInt32Array()
        {
            return this.ReadUInt32Array();
        }

        public ulong ReadOptimizedUInt64()
        {
            byte num3;
            ulong num = 0;
            int num2 = 0;
            do
            {
                num3 = this.ReadByte();
                //num |= (num3 & 0x7f) << num2;
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);
            return num;
        }

        public ulong[] ReadOptimizedUInt64Array()
        {
            return this.ReadUInt64Array();
        }

        public void ReadOwnedData(IOwnedDataSerializable target, object context)
        {
            target.DeserializeOwnedData(this, context);
        }

        public sbyte[] ReadSByteArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new sbyte[0];
            }
            return this.xc3b83f9c7da84140();
        }

        public float[] ReadSingleArray()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new float[0];
            }
            return this.x237fe6762997de16();
        }

        public override string ReadString()
        {
            return this.ReadOptimizedString();
        }

        public string[] ReadStringArray()
        {
            return (string[])this.ReadObjectArray(typeof(string));
        }

        public string ReadStringDirect()
        {
            return base.ReadString();
        }

        public TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(this.ReadInt64());
        }

        public TimeSpan[] ReadTimeSpanArray()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new TimeSpan[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            TimeSpan[] spanArray = new TimeSpan[this.ReadOptimizedInt32()];
            for (int i = 0; i < spanArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    spanArray[i] = this.ReadTimeSpan();
                }
                else
                {
                    spanArray[i] = this.ReadOptimizedTimeSpan();
                }
            }
            return spanArray;
        }

        public object ReadTokenizedObject()
        {
            return this.xd2a26bd8068e723c[this.ReadOptimizedInt32()];
        }

        public Type ReadType()
        {
            return this.ReadType(true);
        }

        public Type ReadType(bool throwOnError)
        {
            if (this.x69810e1eaf86c507() == SerializedType.NullType)
            {
                return null;
            }
            return Type.GetType(this.ReadOptimizedString(), throwOnError);
        }

        public Array ReadTypedArray()
        {
            return (Array)this.xd2b5480591ecb75b(this.x69810e1eaf86c507(), null);
        }

        public ushort[] ReadUInt16Array()
        {
            switch (this.x69810e1eaf86c507())
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new ushort[0];
            }
            return this.x88bf991245665e28();
        }

        public uint[] ReadUInt32Array()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new uint[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            uint[] numArray = new uint[this.ReadOptimizedUInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    numArray[i] = this.ReadUInt32();
                }
                else
                {
                    numArray[i] = this.ReadOptimizedUInt32();
                }
            }
            return numArray;
        }

        public ulong[] ReadUInt64Array()
        {
            SerializedType type = this.x69810e1eaf86c507();
            switch (type)
            {
                case SerializedType.NullType:
                    return null;

                case SerializedType.EmptyTypedArrayType:
                    return new ulong[0];
            }
            BitArray array = this.x106f49cb979892ad(type);
            ulong[] numArray = new ulong[this.ReadOptimizedInt64()];
            for (int i = 0; i < numArray.Length; i++)
            {
                if ((array == null) || ((array != xd297edfab8187def) && !array[i]))
                {
                    numArray[i] = this.ReadUInt64();
                }
                else
                {
                    numArray[i] = this.ReadOptimizedUInt64();
                }
            }
            return numArray;
        }

        private BitArray x106f49cb979892ad(SerializedType xe1fd8722c6088f8f)
        {
            BitArray array = null;
            if (xe1fd8722c6088f8f == SerializedType.FullyOptimizedTypedArrayType)
            {
                return xd297edfab8187def;
            }
            if (xe1fd8722c6088f8f == SerializedType.PartiallyOptimizedTypedArrayType)
            {
                array = this.ReadOptimizedBitArray();
            }
            return array;
        }

        private decimal[] x1740117a7178c2f4()
        {
            decimal[] numArray = new decimal[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadOptimizedDecimal();
            }
            return numArray;
        }

        private float[] x237fe6762997de16()
        {
            float[] numArray = new float[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadSingle();
            }
            return numArray;
        }

        private TimeSpan x323cad03cb8afc4d(byte x48674cc96a14913d)
        {
            long ticks = 0;
            BitVector32 vector = new BitVector32(x48674cc96a14913d | (this.ReadByte() << 8));
            bool flag = vector[SerializationWriter.HasTimeSection] == 1;
            bool flag2 = vector[SerializationWriter.HasSecondsSection] == 1;
            bool flag3 = vector[SerializationWriter.HasMillisecondsSection] == 1;
            if (flag3)
            {
                vector = new BitVector32((vector.Data | (this.ReadByte() << 0x10)) | (this.ReadByte() << 0x18));
            }
            else if (flag2 && flag)
            {
                vector = new BitVector32(vector.Data | (this.ReadByte() << 0x10));
            }
            if (flag)
            {
                ticks += vector[SerializationWriter.HoursSection] * 0x861c46800L;
                ticks += vector[SerializationWriter.MinutesSection] * 0x23c34600;
            }
            if (flag2)
            {
                ticks += vector[(!flag && !flag3) ? SerializationWriter.MinutesSection : SerializationWriter.SecondsSection] * 0x989680;
            }
            if (flag3)
            {
                ticks += vector[SerializationWriter.MillisecondsSection] * 0x2710;
            }
            if (vector[SerializationWriter.HasDaysSection] == 1)
            {
                ticks += this.ReadOptimizedInt32() * 0xc92a69c000L;
            }
            if (vector[SerializationWriter.IsNegativeSection] == 1)
            {
                ticks = -ticks;
            }
            return new TimeSpan(ticks);
        }

        private bool[] x433c453d4c75e26e()
        {
            BitArray array = this.ReadOptimizedBitArray();
            bool[] flagArray = new bool[array.Count];
            for (int i = 0; i < flagArray.Length; i++)
            {
                flagArray[i] = array[i];
            }
            return flagArray;
        }

        private byte[] x4e16ed72ad2b50d4()
        {
            return base.ReadBytes(this.ReadOptimizedInt32());
        }

        private double[] x66b89805c2e9a28d()
        {
            double[] numArray = new double[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadDouble();
            }
            return numArray;
        }

        private SerializedType x69810e1eaf86c507()
        {
            return (SerializedType)this.ReadByte();
        }

        private char[] x778ccab42919df76()
        {
            return base.ReadChars(this.ReadOptimizedInt32());
        }

        private short[] x805cf4bf61232cfa()
        {
            short[] numArray = new short[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadInt16();
            }
            return numArray;
        }

        private string x84728a32b0ed8543(int x5f50b64a167621fe)
        {
            return this.x53a040afa62b19d1[(this.ReadOptimizedInt32() << 7) + x5f50b64a167621fe];
        }

        private ushort[] x88bf991245665e28()
        {
            ushort[] numArray = new ushort[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadUInt16();
            }
            return numArray;
        }

        private sbyte[] xc3b83f9c7da84140()
        {
            sbyte[] numArray = new sbyte[this.ReadOptimizedInt32()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.ReadSByte();
            }
            return numArray;
        }

        private object xd2b5480591ecb75b(SerializedType x795f2c37ec1d01a3, Type x39ff2051a869549a)
        {
            if (x795f2c37ec1d01a3 == SerializedType.StringArrayType)
            {
                return this.ReadOptimizedStringArray();
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int32ArrayType)
            {
                return this.ReadInt32Array();
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int64ArrayType)
            {
                return this.ReadInt64Array();
            }
            if (x795f2c37ec1d01a3 == SerializedType.DecimalArrayType)
            {
                return this.x1740117a7178c2f4();
            }
            if (x795f2c37ec1d01a3 == SerializedType.TimeSpanArrayType)
            {
                return this.ReadTimeSpanArray();
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt32ArrayType)
            {
                return this.ReadUInt32Array();
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt64ArrayType)
            {
                return this.ReadUInt64Array();
            }
            if (x795f2c37ec1d01a3 == SerializedType.DateTimeArrayType)
            {
                return this.ReadDateTimeArray();
            }
            if (x795f2c37ec1d01a3 == SerializedType.BooleanArrayType)
            {
                return this.x433c453d4c75e26e();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ByteArrayType)
            {
                return this.x4e16ed72ad2b50d4();
            }
            if (x795f2c37ec1d01a3 == SerializedType.CharArrayType)
            {
                return this.x778ccab42919df76();
            }
            if (x795f2c37ec1d01a3 == SerializedType.DoubleArrayType)
            {
                return this.x66b89805c2e9a28d();
            }
            if (x795f2c37ec1d01a3 == SerializedType.SingleArrayType)
            {
                return this.x237fe6762997de16();
            }
            if (x795f2c37ec1d01a3 == SerializedType.GuidArrayType)
            {
                return this.xf5abc6ffb6161760();
            }
            if (x795f2c37ec1d01a3 == SerializedType.SByteArrayType)
            {
                return this.xc3b83f9c7da84140();
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int16ArrayType)
            {
                return this.x805cf4bf61232cfa();
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt16ArrayType)
            {
                return this.x88bf991245665e28();
            }
            if (x795f2c37ec1d01a3 == SerializedType.EmptyTypedArrayType)
            {
                return Array.CreateInstance((x39ff2051a869549a != null) ? x39ff2051a869549a : this.ReadOptimizedType(), 0);
            }
            if (x795f2c37ec1d01a3 == SerializedType.OtherTypedArrayType)
            {
                return this.ReadOptimizedObjectArray(this.ReadOptimizedType());
            }
            if (x795f2c37ec1d01a3 == SerializedType.ObjectArrayType)
            {
                return this.ReadOptimizedObjectArray(x39ff2051a869549a);
            }
            if (((x795f2c37ec1d01a3 != SerializedType.FullyOptimizedTypedArrayType) && (x795f2c37ec1d01a3 != SerializedType.PartiallyOptimizedTypedArrayType)) && (x795f2c37ec1d01a3 != SerializedType.NonOptimizedTypedArrayType))
            {
                return null;
            }
            BitArray array = this.x106f49cb979892ad(x795f2c37ec1d01a3);
            int length = this.ReadOptimizedInt32();
            if (x39ff2051a869549a == null)
            {
                x39ff2051a869549a = this.ReadOptimizedType();
            }
            Array array2 = Array.CreateInstance(x39ff2051a869549a, length);
            for (int i = 0; i < length; i++)
            {
                if (array == null)
                {
                    array2.SetValue(this.ReadObject(), i);
                }
                else if ((array == xd297edfab8187def) || !array[i])
                {
                    IOwnedDataSerializable target = (IOwnedDataSerializable)Activator.CreateInstance(x39ff2051a869549a);
                    this.ReadOwnedData(target, null);
                    array2.SetValue(target, i);
                }
            }
            return array2;
        }

        private object xdec6687f61b4db71(SerializedType x795f2c37ec1d01a3)
        {
            if (x795f2c37ec1d01a3 == SerializedType.NullType)
            {
                return null;
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int32Type)
            {
                return this.ReadInt32();
            }
            if (x795f2c37ec1d01a3 == SerializedType.EmptyStringType)
            {
                return string.Empty;
            }
            if (x795f2c37ec1d01a3 < SerializedType.NullType)
            {
                return this.x84728a32b0ed8543((int)x795f2c37ec1d01a3);
            }
            if (x795f2c37ec1d01a3 == SerializedType.BooleanFalseType)
            {
                return false;
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroInt32Type)
            {
                return 0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedInt32Type)
            {
                return this.ReadOptimizedInt32();
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedDecimalType)
            {
                return this.ReadOptimizedDecimal();
            }
            if (x795f2c37ec1d01a3 == SerializedType.DecimalType)
            {
                return this.ReadDecimal();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroDecimalType)
            {
                return 0M;
            }
            if (x795f2c37ec1d01a3 == SerializedType.YStringType)
            {
                return "Y";
            }
            if (x795f2c37ec1d01a3 == SerializedType.DateTimeType)
            {
                return this.ReadDateTime();
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedDateTimeType)
            {
                return this.ReadOptimizedDateTime();
            }
            if (x795f2c37ec1d01a3 == SerializedType.SingleCharStringType)
            {
                return char.ToString(this.ReadChar());
            }
            if (x795f2c37ec1d01a3 == SerializedType.SingleSpaceType)
            {
                return " ";
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneInt32Type)
            {
                return 1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneDecimalType)
            {
                return 1M;
            }
            if (x795f2c37ec1d01a3 == SerializedType.BooleanTrueType)
            {
                return true;
            }
            if (x795f2c37ec1d01a3 == SerializedType.NStringType)
            {
                return "N";
            }
            if (x795f2c37ec1d01a3 == SerializedType.DBNullType)
            {
                return DBNull.Value;
            }
            if (x795f2c37ec1d01a3 == SerializedType.ObjectArrayType)
            {
                return this.ReadOptimizedObjectArray();
            }
            if (x795f2c37ec1d01a3 == SerializedType.EmptyObjectArrayType)
            {
                return new object[0];
            }
            if (x795f2c37ec1d01a3 == SerializedType.MinusOneInt32Type)
            {
                return -1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.MinusOneInt64Type)
            {
                return (long)(-1);
            }
            if (x795f2c37ec1d01a3 == SerializedType.MinusOneInt16Type)
            {
                return (short)(-1);
            }
            if (x795f2c37ec1d01a3 == SerializedType.MinDateTimeType)
            {
                return DateTime.MinValue;
            }
            if (x795f2c37ec1d01a3 == SerializedType.GuidType)
            {
                return this.ReadGuid();
            }
            if (x795f2c37ec1d01a3 == SerializedType.EmptyGuidType)
            {
                return Guid.Empty;
            }
            if (x795f2c37ec1d01a3 == SerializedType.TimeSpanType)
            {
                return this.ReadTimeSpan();
            }
            if (x795f2c37ec1d01a3 == SerializedType.MaxDateTimeType)
            {
                return DateTime.MaxValue;
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroTimeSpanType)
            {
                return TimeSpan.Zero;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedTimeSpanType)
            {
                return this.ReadOptimizedTimeSpan();
            }
            if (x795f2c37ec1d01a3 == SerializedType.DoubleType)
            {
                return this.ReadDouble();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroDoubleType)
            {
                return 0.0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int64Type)
            {
                return this.ReadInt64();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroInt64Type)
            {
                return (long)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedInt64Type)
            {
                return this.ReadOptimizedInt64();
            }
            if (x795f2c37ec1d01a3 == SerializedType.Int16Type)
            {
                return this.ReadInt16();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroInt16Type)
            {
                return (short)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.SingleType)
            {
                return this.ReadSingle();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroSingleType)
            {
                return 0f;
            }
            if (x795f2c37ec1d01a3 == SerializedType.ByteType)
            {
                return this.ReadByte();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroByteType)
            {
                return (byte)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OtherType)
            {
                return new BinaryFormatter().Deserialize(this.BaseStream);
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt16Type)
            {
                return this.ReadUInt16();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroUInt16Type)
            {
                return (ushort)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt32Type)
            {
                return this.ReadUInt32();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroUInt32Type)
            {
                return 0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedUInt32Type)
            {
                return this.ReadOptimizedUInt32();
            }
            if (x795f2c37ec1d01a3 == SerializedType.UInt64Type)
            {
                return this.ReadUInt64();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroUInt64Type)
            {
                return (ulong)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OptimizedUInt64Type)
            {
                return this.ReadOptimizedUInt64();
            }
            if (x795f2c37ec1d01a3 == SerializedType.BitVector32Type)
            {
                return this.ReadBitVector32();
            }
            if (x795f2c37ec1d01a3 == SerializedType.CharType)
            {
                return this.ReadChar();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroCharType)
            {
                return '\0';
            }
            if (x795f2c37ec1d01a3 == SerializedType.SByteType)
            {
                return this.ReadSByte();
            }
            if (x795f2c37ec1d01a3 == SerializedType.ZeroSByteType)
            {
                return (sbyte)0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneByteType)
            {
                return (byte)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneDoubleType)
            {
                return 1.0;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneCharType)
            {
                return '\x0001';
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneInt16Type)
            {
                return (short)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneInt64Type)
            {
                return (long)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneUInt16Type)
            {
                return (ushort)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneUInt32Type)
            {
                return 1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneUInt64Type)
            {
                return (ulong)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneSByteType)
            {
                return (sbyte)1;
            }
            if (x795f2c37ec1d01a3 == SerializedType.OneSingleType)
            {
                return 1f;
            }
            if (x795f2c37ec1d01a3 == SerializedType.BitArrayType)
            {
                return this.ReadOptimizedBitArray();
            }
            if (x795f2c37ec1d01a3 == SerializedType.TypeType)
            {
                return Type.GetType(this.ReadOptimizedString(), false);
            }
            if (x795f2c37ec1d01a3 == SerializedType.ArrayListType)
            {
                return this.ReadOptimizedArrayList();
            }
            if (x795f2c37ec1d01a3 == SerializedType.SingleInstanceType)
            {
                try
                {
                    return Activator.CreateInstance(Type.GetType(this.ReadStringDirect()), true);
                }
                catch
                {
                    return null;
                }
            }
            if (x795f2c37ec1d01a3 == SerializedType.OwnedDataSerializableAndRecreatableType)
            {
                object obj2 = Activator.CreateInstance(this.ReadOptimizedType(), true);
                this.ReadOwnedData((IOwnedDataSerializable)obj2, null);
                return obj2;
            }
            object obj3 = this.xd2b5480591ecb75b(x795f2c37ec1d01a3, null);
            if (obj3 == null)
            {
                throw new InvalidOperationException("Unrecognized TypeCode: " + x795f2c37ec1d01a3);
            }
            return obj3;
        }

        private Guid[] xf5abc6ffb6161760()
        {
            Guid[] guidArray = new Guid[this.ReadOptimizedInt32()];
            for (int i = 0; i < guidArray.Length; i++)
            {
                guidArray[i] = this.ReadGuid();
            }
            return guidArray;
        }

        // Properties
        public long BytesRemaining
        {
            get
            {
                return (this.x16e2b67bb7cbd5e8 - this.BaseStream.Position);
            }
        }
    }


}
