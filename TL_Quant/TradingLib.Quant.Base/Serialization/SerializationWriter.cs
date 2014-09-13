using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using TradingLib.Quant;



namespace TradingLib.Quant.Base
{
    public sealed class SerializationWriter : BinaryWriter
    {
        // Fields
        private bool _x8d1c5938d6dc6e1c;
        internal static readonly BitVector32.Section DateDayMask = BitVector32.CreateSection(0x1f, DateMonthMask);
        internal static readonly BitVector32.Section DateHasTimeOrKindMask = BitVector32.CreateSection(1, DateDayMask);
        internal static readonly BitVector32.Section DateMonthMask = BitVector32.CreateSection(12, DateYearMask);
        internal static readonly BitVector32.Section DateYearMask = BitVector32.CreateSection(0x270f);
        public static int DefaultCapacity = 0x400;
        public static bool DefaultOptimizeForSize = true;
        internal static readonly BitVector32.Section HasDaysSection = BitVector32.CreateSection(1, IsNegativeSection);
        internal static readonly BitVector32.Section HasMillisecondsSection = BitVector32.CreateSection(1, HasSecondsSection);
        internal static readonly BitVector32.Section HasSecondsSection = BitVector32.CreateSection(1, HasTimeSection);
        internal static readonly BitVector32.Section HasTimeSection = BitVector32.CreateSection(1, HasDaysSection);
        public const int HighestOptimizable32BitValue = 0x1fffff;
        public const long HighestOptimizable64BitValue = 0x1ffffffffffffL;
        internal static readonly BitVector32.Section HoursSection = BitVector32.CreateSection(0x17, HasMillisecondsSection);
        internal static readonly BitVector32.Section IsNegativeSection = BitVector32.CreateSection(1);
        internal static readonly BitVector32.Section MillisecondsSection = BitVector32.CreateSection(0x400, SecondsSection);
        internal static readonly BitVector32.Section MinutesSection = BitVector32.CreateSection(0x3b, HoursSection);
        internal static readonly BitVector32.Section SecondsSection = BitVector32.CreateSection(0x3b, MinutesSection);
        private x557086cd7c4c9c95 x0a5906e675d32ddb;
        private const long x727a7317087b486d = 0x100000000000000L;
        private Hashtable x7fc945e98eefa8d4;
        private bool xd1aa6b0b69140b41;
        private static readonly BitArray xd297edfab8187def = new BitArray(0);
        private ArrayList xd2a26bd8068e723c;
        private const int xfb6392d152804f98 = 0x10000000;

        // Methods
        public SerializationWriter()
            : this(new MemoryStream(DefaultCapacity))
        {
        }

        public SerializationWriter(int capacity)
            : this(new MemoryStream(capacity))
        {
        }

        public SerializationWriter(Stream stream)
            : base(stream)
        {
            this.xd1aa6b0b69140b41 = DefaultOptimizeForSize;
            if (!stream.CanSeek)
            {
                throw new InvalidOperationException("Stream must be seekable");
            }
            this.Write((long)0);
            this.xd2a26bd8068e723c = new ArrayList();
            this.x7fc945e98eefa8d4 = new Hashtable();
            this.x0a5906e675d32ddb = new x557086cd7c4c9c95();
            this._x8d1c5938d6dc6e1c = false;
        }

        public long AppendTokenTables()
        {
            if (this._x8d1c5938d6dc6e1c)
            {
                throw new InvalidOperationException("The token table has already been appended to the stream.");
            }
            long position = this.BaseStream.Position;
            this.BaseStream.Position = 0;
            this.Write(position);
            this.BaseStream.Position = position;
            int count = this.x0a5906e675d32ddb.Count;
            this.x682e475b7c7d7847(this.x0a5906e675d32ddb.Count);
            for (int i = 0; i < count; i++)
            {
                base.Write(this.x0a5906e675d32ddb[i]);
            }
            this.x682e475b7c7d7847(this.xd2a26bd8068e723c.Count);
            for (int j = 0; j < this.xd2a26bd8068e723c.Count; j++)
            {
                this.WriteObject(this.xd2a26bd8068e723c[j]);
            }
            this._x8d1c5938d6dc6e1c = true;
            return (this.BaseStream.Position - position);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this._x8d1c5938d6dc6e1c)
            {
                this.AppendTokenTables();
            }
            base.Dispose(disposing);
        }

        [Conditional("DEBUG")]
        public void DumpTypeUsage()
        {
            StringBuilder builder = new StringBuilder("Type Usage Dump\r\n---------------\r\n");
            for (int i = 0; i < 0x100; i++)
            {
            }
            Console.WriteLine(builder);
        }

        public byte[] ToArray()
        {
            this.AppendTokenTables();
            return (this.BaseStream as MemoryStream).ToArray();
        }

        public void Write(BitArray value)
        {
            if (value == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.BitArrayType);
                this.WriteOptimized(value);
            }
        }

        public void Write(Guid value)
        {
            base.Write(value.ToByteArray());
        }

        public void Write(DateTime[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public override void Write(string value)
        {
            this.WriteOptimized(value);
        }

        public void Write(TimeSpan value)
        {
            this.Write(value.Ticks);
        }

        public void Write(bool[] values)
        {
            this.WriteOptimized(values);
        }

        public override void Write(byte[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public override void Write(char[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(Guid[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(ArrayList value)
        {
            if (value == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.ArrayListType);
                this.WriteOptimized(value);
            }
        }

        public void Write(BitVector32 value)
        {
            base.Write(value.Data);
        }

        public void Write(DateTime value)
        {
            this.Write(value.Ticks);
        }

        public void Write(decimal[] values)
        {
            this.WriteOptimized(values);
        }

        public void Write(double[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(short[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(int[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public void Write(long[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public void Write(object[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyObjectArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.ObjectArrayType);
                this.WriteObjectArray(values);
            }
        }

        public void Write(sbyte[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(float[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(TimeSpan[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public void Write(ushort[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void Write(uint[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public void Write(ulong[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.x61f02b574d41bbdb(values, null);
            }
        }

        public void Write(IOwnedDataSerializable target, object context)
        {
            target.SerializeOwnedData(this, context);
        }

        public void Write(Type value, bool fullyQualified)
        {
            if (value == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.TypeType);
                this.WriteOptimized(fullyQualified ? value.AssemblyQualifiedName : value.FullName);
            }
        }

        public void WriteBytesDirect(byte[] value)
        {
            base.Write(value);
        }

        public void WriteObject(object value)
        {
            if (value == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (value is string)
            {
                this.WriteOptimized((string)value);
            }
            else if (value is int)
            {
                int num = (int)value;
                switch (num)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroInt32Type);
                        return;

                    case -1:
                        this.xeac52d682f1b9262(SerializedType.MinusOneInt32Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneInt32Type);
                        return;
                }
                if ((this.xd1aa6b0b69140b41 && (num <= 0x1fffff)) && (num > 0))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedInt32Type);
                    this.x682e475b7c7d7847(num);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.Int32Type);
                    this.Write(num);
                }
            }
            else if (value == DBNull.Value)
            {
                this.xeac52d682f1b9262(SerializedType.DBNullType);
            }
            else if (value is bool)
            {
                this.xeac52d682f1b9262(((bool)value) ? SerializedType.BooleanTrueType : SerializedType.BooleanFalseType);
            }
            else if (value is decimal)
            {
                decimal num2 = (decimal)value;
                if(num2 ==0)
                    this.xeac52d682f1b9262(SerializedType.ZeroDecimalType);
                if(num2 ==1)
                    this.xeac52d682f1b9262(SerializedType.OneDecimalType);
                /*
                switch (num2.ToString())
                {
                    case "0":
                        this.xeac52d682f1b9262(SerializedType.ZeroDecimalType);
                        return;

                    case "":
                        this.xeac52d682f1b9262(SerializedType.OneDecimalType);
                        return;
                }**/
                if (this.xd1aa6b0b69140b41)
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedDecimalType);
                    this.WriteOptimized(num2);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.DecimalType);
                    this.Write(num2);
                }
            }
            else if (value is DateTime)
            {
                DateTime time = (DateTime)value;
                if (time == DateTime.MinValue)
                {
                    this.xeac52d682f1b9262(SerializedType.MinDateTimeType);
                }
                else if (time == DateTime.MaxValue)
                {
                    this.xeac52d682f1b9262(SerializedType.MaxDateTimeType);
                }
                else if (this.xd1aa6b0b69140b41 && ((time.Ticks % 0x2710) == 0))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedDateTimeType);
                    this.WriteOptimized(time);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.DateTimeType);
                    this.Write(time);
                }
            }
            else if (value is double)
            {
                double num3 = (double)value;
                if(num3 == 0)
                    this.xeac52d682f1b9262(SerializedType.ZeroDoubleType);
                if (num3 == 1) ;
                    this.xeac52d682f1b9262(SerializedType.OneDoubleType);
                /*
                switch (num3)
                {
                    case 0.0:
                        this.xeac52d682f1b9262(SerializedType.ZeroDoubleType);
                        return;

                    case 1.0:
                        this.xeac52d682f1b9262(SerializedType.OneDoubleType);
                        return;
                }**/
                this.xeac52d682f1b9262(SerializedType.DoubleType);
                this.Write(num3);
            }
            else if (value is float)
            {
                float num4 = (float)value;
                if(num4 == 0)
                    this.xeac52d682f1b9262(SerializedType.ZeroSingleType);
                if(num4 ==1)
                    this.xeac52d682f1b9262(SerializedType.OneSingleType);
                /*
                switch (num4)
                {
                    case 0f:
                        this.xeac52d682f1b9262(SerializedType.ZeroSingleType);
                        return;

                    case 1f:
                        this.xeac52d682f1b9262(SerializedType.OneSingleType);
                        return;
                }**/
                this.xeac52d682f1b9262(SerializedType.SingleType);
                this.Write(num4);
            }
            else if (value is short)
            {
                short num5 = (short)value;
                switch (num5)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroInt16Type);
                        return;

                    case -1:
                        this.xeac52d682f1b9262(SerializedType.MinusOneInt16Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneInt16Type);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.Int16Type);
                this.Write(num5);
            }
            else if (value is Guid)
            {
                Guid guid = (Guid)value;
                if (guid == Guid.Empty)
                {
                    this.xeac52d682f1b9262(SerializedType.EmptyGuidType);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.GuidType);
                    this.Write(guid);
                }
            }
            else if (value is long)
            {
                long num6 = (long)value;
                switch (num6)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroInt64Type);
                        return;

                    case -1:
                        this.xeac52d682f1b9262(SerializedType.MinusOneInt64Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneInt64Type);
                        return;
                }
                if ((this.xd1aa6b0b69140b41 && (num6 <= 0x1ffffffffffffL)) && (num6 > 0))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedInt64Type);
                    this.WriteOptimized(num6);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.Int64Type);
                    this.Write(num6);
                }
            }
            else if (value is byte)
            {
                byte num7 = (byte)value;
                switch (num7)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroByteType);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneByteType);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.ByteType);
                this.Write(num7);
            }
            else if (value is char)
            {
                char ch = (char)value;
                switch (ch)
                {
                    case '\0':
                        this.xeac52d682f1b9262(SerializedType.ZeroCharType);
                        return;

                    case '\x0001':
                        this.xeac52d682f1b9262(SerializedType.OneCharType);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.CharType);
                this.Write(ch);
            }
            else if (value is sbyte)
            {
                sbyte num8 = (sbyte)value;
                switch (num8)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroSByteType);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneSByteType);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.SByteType);
                this.Write(num8);
            }
            else if (value is uint)
            {
                uint num9 = (uint)value;
                switch (num9)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroUInt32Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneUInt32Type);
                        return;
                }
                if (this.xd1aa6b0b69140b41 && (num9 <= 0x1fffff))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedUInt32Type);
                    this.WriteOptimized(num9);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.UInt32Type);
                    this.Write(num9);
                }
            }
            else if (value is ushort)
            {
                ushort num10 = (ushort)value;
                switch (num10)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroUInt16Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneUInt16Type);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.UInt16Type);
                this.Write(num10);
            }
            else if (value is ulong)
            {
                ulong num11 = (ulong)value;
                switch (num11)
                {
                    case 0:
                        this.xeac52d682f1b9262(SerializedType.ZeroUInt64Type);
                        return;

                    case 1:
                        this.xeac52d682f1b9262(SerializedType.OneUInt64Type);
                        return;
                }
                if (this.xd1aa6b0b69140b41 && (num11 <= 0x1ffffffffffffL))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedUInt64Type);
                    this.WriteOptimized(num11);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.UInt64Type);
                    this.Write(num11);
                }
            }
            else if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan)value;
                if (span == TimeSpan.Zero)
                {
                    this.xeac52d682f1b9262(SerializedType.ZeroTimeSpanType);
                }
                else if (this.xd1aa6b0b69140b41 && ((span.Ticks % 0x2710) == 0))
                {
                    this.xeac52d682f1b9262(SerializedType.OptimizedTimeSpanType);
                    this.WriteOptimized(span);
                }
                else
                {
                    this.xeac52d682f1b9262(SerializedType.TimeSpanType);
                    this.Write(span);
                }
            }
            else if (value is Array)
            {
                this.x3fdf956453c139f0((Array)value, true);
            }
            else if (value is Type)
            {
                this.xeac52d682f1b9262(SerializedType.TypeType);
                this.WriteOptimized(value as Type);
            }
            else if (value is BitArray)
            {
                this.xeac52d682f1b9262(SerializedType.BitArrayType);
                this.WriteOptimized((BitArray)value);
            }
            else if (value is BitVector32)
            {
                this.xeac52d682f1b9262(SerializedType.BitVector32Type);
                this.Write((BitVector32)value);
            }
            else if (xa9c037eacd8c1fd5(value.GetType()))
            {
                this.xeac52d682f1b9262(SerializedType.OwnedDataSerializableAndRecreatableType);
                this.WriteOptimized(value.GetType());
                this.Write((IOwnedDataSerializable)value, null);
            }
            else if (value is x8c14939588a937db)
            {
                this.xeac52d682f1b9262(SerializedType.SingleInstanceType);
                Type wrappedType = (value as x8c14939588a937db).WrappedType;
                if (wrappedType.AssemblyQualifiedName.IndexOf(", mscorlib,") == -1)
                {
                    this.WriteStringDirect(wrappedType.AssemblyQualifiedName);
                }
                else
                {
                    this.WriteStringDirect(wrappedType.FullName);
                }
            }
            else if (value is ArrayList)
            {
                this.xeac52d682f1b9262(SerializedType.ArrayListType);
                this.WriteOptimized(value as ArrayList);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.OtherType);
                fullBinaryFormatter().Serialize(this.BaseStream, value);
            }
        }

        public void WriteOptimized(ArrayList value)
        {
            this.WriteObjectArray(value.ToArray());
        }

        public void WriteOptimized(BitArray value)
        {
            this.x682e475b7c7d7847(value.Length);
            if (value.Length > 0)
            {
                byte[] array = new byte[(value.Length + 7) / 8];
                value.CopyTo(array, 0);
                base.Write(array, 0, array.Length);
            }
        }

        public void WriteOptimized(BitVector32 value)
        {
            this.x682e475b7c7d7847(value.Data);
        }

        public void WriteOptimized(int value)
        {
            this.x682e475b7c7d7847(value);
        }

        public void WriteOptimized(Type value)
        {
            this.WriteOptimized((value.AssemblyQualifiedName.IndexOf(", mscorlib,") == -1) ? value.AssemblyQualifiedName : value.FullName);
        }

        public void WriteOptimized(uint value)
        {
            this.xc71b47d91e4a9011(value);
        }

        public void WriteOptimized(ulong value)
        {
            this.xfe9dfaf605f0201c(value);
        }

        public void WriteOptimized(bool[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.FullyOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void WriteOptimized(DateTime[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if ((values[i].Ticks % 0x2710) != 0)
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(decimal[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.FullyOptimizedTypedArrayType);
                this.x61f02b574d41bbdb(values);
            }
        }

        public void WriteOptimized(DateTime value)
        {
            BitVector32 vector = new BitVector32();
            vector[DateYearMask] = value.Year;
            vector[DateMonthMask] = value.Month;
            vector[DateDayMask] = value.Day;
            int num = 0;
            bool flag = value != value.Date;
            vector[DateHasTimeOrKindMask] = flag ? 1 : 0;
            int data = vector.Data;
            this.Write((byte)data);
            this.Write((byte)(data >> 8));
            this.Write((byte)(data >> 0x10));
            if (flag)
            {
                this.xb807f4d8ca625fa8(value.TimeOfDay, true, num);
            }
        }

        public void WriteOptimized(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            byte num = (byte)(bits[3] >> 0x10);
            byte num2 = 0;
            if ((bits[3] & -2147483648) != 0)
            {
                num2 = (byte)(num2 | 1);
            }
            if (num != 0)
            {
                num2 = (byte)(num2 | 2);
            }
            if (bits[0] == 0)
            {
                num2 = (byte)(num2 | 4);
            }
            else if ((bits[0] <= 0x1fffff) && (bits[0] >= 0))
            {
                num2 = (byte)(num2 | 0x20);
            }
            if (bits[1] == 0)
            {
                num2 = (byte)(num2 | 8);
            }
            else if ((bits[1] <= 0x1fffff) && (bits[0] >= 0))
            {
                num2 = (byte)(num2 | 0x40);
            }
            if (bits[2] == 0)
            {
                num2 = (byte)(num2 | 0x10);
            }
            else if ((bits[2] <= 0x1fffff) && (bits[0] >= 0))
            {
                num2 = (byte)(num2 | 0x80);
            }
            this.Write(num2);
            if (num != 0)
            {
                this.Write(num);
            }
            if ((num2 & 4) == 0)
            {
                if ((num2 & 0x20) != 0)
                {
                    this.x682e475b7c7d7847(bits[0]);
                }
                else
                {
                    this.Write(bits[0]);
                }
            }
            if ((num2 & 8) == 0)
            {
                if ((num2 & 0x40) != 0)
                {
                    this.x682e475b7c7d7847(bits[1]);
                }
                else
                {
                    this.Write(bits[1]);
                }
            }
            if ((num2 & 0x10) == 0)
            {
                if ((num2 & 0x80) != 0)
                {
                    this.x682e475b7c7d7847(bits[2]);
                }
                else
                {
                    this.Write(bits[2]);
                }
            }
        }

        public void WriteOptimized(int[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if ((values[i] < 0) || (values[i] > 0x1fffff))
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(long value)
        {
            this.x7b67e108f2abefdf(value);
        }

        public void WriteOptimized(string value)
        {
            if (value == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (value.Length == 1)
            {
                char ch = value[0];
                switch (ch)
                {
                    case 'Y':
                        this.xeac52d682f1b9262(SerializedType.YStringType);
                        return;

                    case 'N':
                        this.xeac52d682f1b9262(SerializedType.NStringType);
                        return;

                    case ' ':
                        this.xeac52d682f1b9262(SerializedType.SingleSpaceType);
                        return;
                }
                this.xeac52d682f1b9262(SerializedType.SingleCharStringType);
                this.Write(ch);
            }
            else if (value.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyStringType);
            }
            else
            {
                int num = this.x0a5906e675d32ddb.Add(value);
                this.Write((byte)(num % 0x80));
                this.x682e475b7c7d7847(num >> 7);
            }
        }

        public void WriteOptimized(TimeSpan value)
        {
            this.xb807f4d8ca625fa8(value, false, 0);
        }

        public void WriteOptimized(long[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if ((values[i] < 0) || (values[i] > 0x1ffffffffffffL))
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(object[] values)
        {
            this.WriteObjectArray(values);
        }

        public void WriteOptimized(TimeSpan[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if ((values[i].Ticks % 0x2710) != 0)
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(uint[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if (values[i] > 0x1fffff)
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(ulong[] values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else if (values.Length == 0)
            {
                this.xeac52d682f1b9262(SerializedType.EmptyTypedArrayType);
            }
            else
            {
                BitArray array = null;
                int num = 0;
                int num2 = 1 + ((int)(values.Length * (this.xd1aa6b0b69140b41 ? 0.8f : 0.6f)));
                for (int i = 0; (i < values.Length) && (num < num2); i++)
                {
                    if (values[i] > 0x1ffffffffffffL)
                    {
                        num++;
                    }
                    else
                    {
                        if (array == null)
                        {
                            array = new BitArray(values.Length);
                        }
                        array[i] = true;
                    }
                }
                if (num == 0)
                {
                    array = xd297edfab8187def;
                }
                else if (num >= num2)
                {
                    array = null;
                }
                this.x61f02b574d41bbdb(values, array);
            }
        }

        public void WriteOptimized(object[] values1, object[] values2)
        {
            this.WriteObjectArray(values1);
            int num = values2.Length - 1;
            for (int i = 0; i < values2.Length; i++)
            {
                object obj2 = values2[i];
                if ((obj2 == null) ? (values1[i] == null) : obj2.Equals(values1[i]))
                {
                    int num3 = 0;
                    while ((i < num) && ((values2[i + 1] == null) ? (values1[i + 1] == null) : values2[i + 1].Equals(values1[i + 1])))
                    {
                        num3++;
                        i++;
                    }
                    if (num3 == 0)
                    {
                        this.xeac52d682f1b9262(SerializedType.DuplicateValueType);
                    }
                    else
                    {
                        this.xeac52d682f1b9262(SerializedType.DuplicateValueSequenceType);
                        this.x682e475b7c7d7847(num3);
                    }
                }
                else if (obj2 == null)
                {
                    int num4 = 0;
                    while ((i < num) && (values2[i + 1] == null))
                    {
                        num4++;
                        i++;
                    }
                    if (num4 == 0)
                    {
                        this.xeac52d682f1b9262(SerializedType.NullType);
                    }
                    else
                    {
                        this.xeac52d682f1b9262(SerializedType.NullSequenceType);
                        this.x682e475b7c7d7847(num4);
                    }
                }
                else if (obj2 == DBNull.Value)
                {
                    int num5 = 0;
                    while ((i < num) && (values2[i + 1] == DBNull.Value))
                    {
                        num5++;
                        i++;
                    }
                    if (num5 == 0)
                    {
                        this.xeac52d682f1b9262(SerializedType.DBNullType);
                    }
                    else
                    {
                        this.xeac52d682f1b9262(SerializedType.DBNullSequenceType);
                        this.x682e475b7c7d7847(num5);
                    }
                }
                else
                {
                    this.WriteObject(obj2);
                }
            }
        }

        public void WriteStringDirect(string value)
        {
            base.Write(value);
        }

        public void WriteTokenizedObject(object value)
        {
            this.WriteTokenizedObject(value, false);
        }

        public void WriteTokenizedObject(object value, bool recreateFromType)
        {
            if (recreateFromType)
            {
                value = new x8c14939588a937db(value);
            }
            object obj2 = this.x7fc945e98eefa8d4[value];
            if (obj2 != null)
            {
                this.x682e475b7c7d7847((int)obj2);
            }
            else
            {
                int count = this.xd2a26bd8068e723c.Count;
                this.xd2a26bd8068e723c.Add(value);
                this.x7fc945e98eefa8d4[value] = count;
                this.x682e475b7c7d7847(count);
            }
        }

        public void WriteTypedArray(Array values)
        {
            if (values == null)
            {
                this.xeac52d682f1b9262(SerializedType.NullType);
            }
            else
            {
                this.x3fdf956453c139f0(values, true);
            }
        }

        private static bool x0cf208097fd00d6b(object[] objArray, Type x7e18581979d4c861)
        {
            foreach (object obj2 in objArray)
            {
                if ((obj2 != null) && (obj2.GetType() != x7e18581979d4c861))
                {
                    return false;
                }
            }
            return true;
        }

        private void x3fdf956453c139f0(Array xbcea506a33cf9111, bool xdc72039d7527d899)
        {
            Type elementType = xbcea506a33cf9111.GetType().GetElementType();
            if (elementType == typeof(object))
            {
                xdc72039d7527d899 = false;
            }
            if (elementType == typeof(string))
            {
                this.xeac52d682f1b9262(SerializedType.StringArrayType);
                this.WriteOptimized((object[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(int))
            {
                this.xeac52d682f1b9262(SerializedType.Int32ArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((int[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((int[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(short))
            {
                this.xeac52d682f1b9262(SerializedType.Int16ArrayType);
                this.x61f02b574d41bbdb((short[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(long))
            {
                this.xeac52d682f1b9262(SerializedType.Int64ArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((long[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((long[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(uint))
            {
                this.xeac52d682f1b9262(SerializedType.UInt32ArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((uint[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((uint[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(ushort))
            {
                this.xeac52d682f1b9262(SerializedType.UInt16ArrayType);
                this.x61f02b574d41bbdb((ushort[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(ulong))
            {
                this.xeac52d682f1b9262(SerializedType.UInt64ArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((ulong[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((ulong[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(float))
            {
                this.xeac52d682f1b9262(SerializedType.SingleArrayType);
                this.x61f02b574d41bbdb((float[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(double))
            {
                this.xeac52d682f1b9262(SerializedType.DoubleArrayType);
                this.x61f02b574d41bbdb((double[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(decimal))
            {
                this.xeac52d682f1b9262(SerializedType.DecimalArrayType);
                this.x61f02b574d41bbdb((decimal[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(DateTime))
            {
                this.xeac52d682f1b9262(SerializedType.DateTimeArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((DateTime[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((DateTime[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(TimeSpan))
            {
                this.xeac52d682f1b9262(SerializedType.TimeSpanArrayType);
                if (this.xd1aa6b0b69140b41)
                {
                    this.WriteOptimized((TimeSpan[])xbcea506a33cf9111);
                }
                else
                {
                    this.Write((TimeSpan[])xbcea506a33cf9111);
                }
            }
            else if (elementType == typeof(Guid))
            {
                this.xeac52d682f1b9262(SerializedType.GuidArrayType);
                this.x61f02b574d41bbdb((Guid[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(sbyte))
            {
                this.xeac52d682f1b9262(SerializedType.SByteArrayType);
                this.x61f02b574d41bbdb((sbyte[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(bool))
            {
                this.xeac52d682f1b9262(SerializedType.BooleanArrayType);
                this.x61f02b574d41bbdb((bool[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(byte))
            {
                this.xeac52d682f1b9262(SerializedType.ByteArrayType);
                this.x61f02b574d41bbdb((byte[])xbcea506a33cf9111);
            }
            else if (elementType == typeof(char))
            {
                this.xeac52d682f1b9262(SerializedType.CharArrayType);
                this.x61f02b574d41bbdb((char[])xbcea506a33cf9111);
            }
            else if (xbcea506a33cf9111.Length == 0)
            {
                this.xeac52d682f1b9262((elementType == typeof(object)) ? SerializedType.EmptyObjectArrayType : SerializedType.EmptyTypedArrayType);
                if (xdc72039d7527d899)
                {
                    this.WriteOptimized(elementType);
                }
            }
            else if (elementType == typeof(object))
            {
                this.xeac52d682f1b9262(SerializedType.ObjectArrayType);
                this.WriteObjectArray((object[])xbcea506a33cf9111);
            }
            else
            {
                BitArray array = xa9c037eacd8c1fd5(elementType) ? xd297edfab8187def : null;
                if (!elementType.IsValueType)
                {
                    if ((array == null) || !x0cf208097fd00d6b((object[])xbcea506a33cf9111, elementType))
                    {
                        if (!xdc72039d7527d899)
                        {
                            this.xeac52d682f1b9262(SerializedType.ObjectArrayType);
                        }
                        else
                        {
                            this.xeac52d682f1b9262(SerializedType.OtherTypedArrayType);
                            this.WriteOptimized(elementType);
                        }
                        this.WriteObjectArray((object[])xbcea506a33cf9111);
                        return;
                    }
                    for (int j = 0; j < xbcea506a33cf9111.Length; j++)
                    {
                        if (xbcea506a33cf9111.GetValue(j) == null)
                        {
                            if (array == xd297edfab8187def)
                            {
                                array = new BitArray(xbcea506a33cf9111.Length);
                            }
                            array[j] = true;
                        }
                    }
                }
                this.xa7f3a0b2a2e09d75(array, xbcea506a33cf9111.Length);
                if (xdc72039d7527d899)
                {
                    this.WriteOptimized(elementType);
                }
                for (int i = 0; i < xbcea506a33cf9111.Length; i++)
                {
                    if (array == null)
                    {
                        this.WriteObject(xbcea506a33cf9111.GetValue(i));
                    }
                    else if ((array == xd297edfab8187def) || !array[i])
                    {
                        this.Write((IOwnedDataSerializable)xbcea506a33cf9111.GetValue(i), null);
                    }
                }
            }
        }

        private static BinaryFormatter fullBinaryFormatter()
        {
            return new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Full };
        }

        private void x61f02b574d41bbdb(bool[] objArray)
        {
            this.WriteOptimized(new BitArray(objArray));
        }

        private void x61f02b574d41bbdb(byte[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            if (objArray.Length > 0)
            {
                base.Write(objArray);
            }
        }

        private void x61f02b574d41bbdb(char[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            if (objArray.Length > 0)
            {
                base.Write(objArray);
            }
        }

        private void x61f02b574d41bbdb(decimal[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                this.WriteOptimized(objArray[i]);
            }
        }

        private void x61f02b574d41bbdb(double[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (double num in objArray)
            {
                this.Write(num);
            }
        }

        private void x61f02b574d41bbdb(Guid[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (Guid guid in objArray)
            {
                this.Write(guid);
            }
        }

        private void x61f02b574d41bbdb(short[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (short num in objArray)
            {
                this.Write(num);
            }
        }

        private void x61f02b574d41bbdb(sbyte[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (sbyte num in objArray)
            {
                this.Write(num);
            }
        }

        private void x61f02b574d41bbdb(float[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (float num in objArray)
            {
                this.Write(num);
            }
        }

        private void x61f02b574d41bbdb(ushort[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            foreach (ushort num in objArray)
            {
                this.Write(num);
            }
        }

        private void x61f02b574d41bbdb(DateTime[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.WriteOptimized(objArray[i]);
                }
            }
        }

        private void x61f02b574d41bbdb(int[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.x682e475b7c7d7847(objArray[i]);
                }
            }
        }

        private void x61f02b574d41bbdb(long[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.x7b67e108f2abefdf(objArray[i]);
                }
            }
        }

        private void x61f02b574d41bbdb(TimeSpan[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.WriteOptimized(objArray[i]);
                }
            }
        }

        private void x61f02b574d41bbdb(uint[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.xc71b47d91e4a9011(objArray[i]);
                }
            }
        }

        private void x61f02b574d41bbdb(ulong[] objArray, BitArray xe128917fdd77f176)
        {
            this.xa7f3a0b2a2e09d75(xe128917fdd77f176, objArray.Length);
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((xe128917fdd77f176 == null) || ((xe128917fdd77f176 != xd297edfab8187def) && !xe128917fdd77f176[i]))
                {
                    this.Write(objArray[i]);
                }
                else
                {
                    this.xfe9dfaf605f0201c(objArray[i]);
                }
            }
        }

        private static ConstructorInfo x634c89908419e32e(Type x43163d22e8cd5a71)
        {
            return x43163d22e8cd5a71.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
        }

        private void x682e475b7c7d7847(int xbcea506a33cf9111)
        {
            uint num = (uint)xbcea506a33cf9111;
            while (num >= 0x80)
            {
                this.Write((byte)(num | 0x80));
                num = num >> 7;
            }
            this.Write((byte)num);
        }

        private void x7b67e108f2abefdf(long xbcea506a33cf9111)
        {
            ulong num = (ulong)xbcea506a33cf9111;
            while (num >= 0x80)
            {
                this.Write((byte)(num | 0x80));
                num = num >> 7;
            }
            this.Write((byte)num);
        }

        private void WriteObjectArray(object[] objArray)
        {
            this.x682e475b7c7d7847(objArray.Length);
            int num = objArray.Length - 1;
            for (int i = 0; i < objArray.Length; i++)
            {
                object obj2 = objArray[i];
                if ((i < num) && ((obj2 == null) ? (objArray[i + 1] == null) : obj2.Equals(objArray[i + 1])))
                {
                    int num3 = 1;
                    if (obj2 == null)
                    {
                        this.xeac52d682f1b9262(SerializedType.NullSequenceType);
                        i++;
                        while ((i < num) && (objArray[i + 1] == null))
                        {
                            num3++;
                            i++;
                        }
                    }
                    else if (obj2 == DBNull.Value)
                    {
                        this.xeac52d682f1b9262(SerializedType.DBNullSequenceType);
                        i++;
                        while ((i < num) && (objArray[i + 1] == DBNull.Value))
                        {
                            num3++;
                            i++;
                        }
                    }
                    else
                    {
                        this.xeac52d682f1b9262(SerializedType.DuplicateValueSequenceType);
                        i++;
                        while ((i < num) && obj2.Equals(objArray[i + 1]))
                        {
                            num3++;
                            i++;
                        }
                        this.WriteObject(obj2);
                    }
                    this.x682e475b7c7d7847(num3);
                }
                else
                {
                    this.WriteObject(obj2);
                }
            }
        }

        private void xa7f3a0b2a2e09d75(BitArray xe128917fdd77f176, int x961016a387451f05)
        {
            if (xe128917fdd77f176 == null)
            {
                this.xeac52d682f1b9262(SerializedType.NonOptimizedTypedArrayType);
            }
            else if (xe128917fdd77f176 == xd297edfab8187def)
            {
                this.xeac52d682f1b9262(SerializedType.FullyOptimizedTypedArrayType);
            }
            else
            {
                this.xeac52d682f1b9262(SerializedType.PartiallyOptimizedTypedArrayType);
                this.WriteOptimized(xe128917fdd77f176);
            }
            this.x682e475b7c7d7847(x961016a387451f05);
        }

        private static bool xa9c037eacd8c1fd5(Type x43163d22e8cd5a71)
        {
            if (x43163d22e8cd5a71.IsValueType)
            {
                return typeof(IOwnedDataSerializable).IsAssignableFrom(x43163d22e8cd5a71);
            }
            return (typeof(IOwnedDataSerializableAndRecreatable).IsAssignableFrom(x43163d22e8cd5a71) && (x634c89908419e32e(x43163d22e8cd5a71) != null));
        }

        [Conditional("THROW_IF_NOT_OPTIMIZABLE")]
        private static void xaa620640f189fe08(bool x29ca7772d281a736, string x1f25abf5fb75e795)
        {
            if (!x29ca7772d281a736)
            {
                //throw new OptimizationException(x1f25abf5fb75e795);
            }
        }

        private void xb807f4d8ca625fa8(TimeSpan xbcea506a33cf9111, bool x6bd9e1ba8021d000, int x2d64c3a5523181ee)
        {
            int num;
            BitVector32 vector = new BitVector32(x2d64c3a5523181ee);
            int num2 = Math.Abs(xbcea506a33cf9111.Hours);
            int num3 = Math.Abs(xbcea506a33cf9111.Minutes);
            int num4 = Math.Abs(xbcea506a33cf9111.Seconds);
            int num5 = Math.Abs(xbcea506a33cf9111.Milliseconds);
            bool flag = (num2 != 0) || (num3 != 0);
            int num6 = 0;
            if (x6bd9e1ba8021d000)
            {
                num = 0;
            }
            else
            {
                num = Math.Abs(xbcea506a33cf9111.Days);
                vector[IsNegativeSection] = (xbcea506a33cf9111.Ticks < 0) ? 1 : 0;
                vector[HasDaysSection] = (num != 0) ? 1 : 0;
            }
            if (flag)
            {
                vector[HasTimeSection] = 1;
                vector[HoursSection] = num2;
                vector[MinutesSection] = num3;
            }
            if (num4 != 0)
            {
                vector[HasSecondsSection] = 1;
                if (!flag && (num5 == 0))
                {
                    vector[MinutesSection] = num4;
                }
                else
                {
                    vector[SecondsSection] = num4;
                    num6++;
                }
            }
            if (num5 != 0)
            {
                vector[HasMillisecondsSection] = 1;
                vector[MillisecondsSection] = num5;
                num6 = 2;
            }
            int data = vector.Data;
            this.Write((byte)data);
            this.Write((byte)(data >> 8));
            if (num6 > 0)
            {
                this.Write((byte)(data >> 0x10));
            }
            if (num6 > 1)
            {
                this.Write((byte)(data >> 0x18));
            }
            if (num != 0)
            {
                this.x682e475b7c7d7847(num);
            }
        }

        private void xc71b47d91e4a9011(uint xbcea506a33cf9111)
        {
            while (xbcea506a33cf9111 >= 0x80)
            {
                this.Write((byte)(xbcea506a33cf9111 | 0x80));
                xbcea506a33cf9111 = xbcea506a33cf9111 >> 7;
            }
            this.Write((byte)xbcea506a33cf9111);
        }

        private void xeac52d682f1b9262(SerializedType x795f2c37ec1d01a3)
        {
            this.Write((byte)x795f2c37ec1d01a3);
        }

        private void xfe9dfaf605f0201c(ulong xbcea506a33cf9111)
        {
            while (xbcea506a33cf9111 >= 0x80)
            {
                this.Write((byte)(xbcea506a33cf9111 | 0x80));
                xbcea506a33cf9111 = xbcea506a33cf9111 >> 7;
            }
            this.Write((byte)xbcea506a33cf9111);
        }

        // Properties
        public bool OptimizeForSize
        {
            get
            {
                return this.xd1aa6b0b69140b41;
            }
            set
            {
                this.xd1aa6b0b69140b41 = value;
            }
        }

        // Nested Types
        private sealed class x557086cd7c4c9c95
        {
            // Fields
            private int[] x084a6c1006133d8f;
            private int x306a401dfa1ce0a0;
            private int x435d3cb48b86c624;
            private int x598e958664579abe;
            private static readonly int[] xb2f12218b05a20d1 = new int[] { 
            0x185, 0x607, 0x1807, 0x6011, 0x1800d, 0x30005, 0x60019, 0xc0001, 0x180005, 0x30000b, 0x60000d, 0xc00005, 0x1800013, 0x3000005, 0x6000017, 0xc000013, 
            0x18000005, 0x30000059, 0x60000005
         };
            private const float xcc4463e322fae906 = 0.72f;
            private int xe31bf898501baf9b;
            private string[] xfbef61d96554cb75;

            // Methods
            public x557086cd7c4c9c95()
            {
                this.x598e958664579abe = xb2f12218b05a20d1[this.x306a401dfa1ce0a0++];
                this.xfbef61d96554cb75 = new string[this.x598e958664579abe];
                this.x084a6c1006133d8f = new int[this.x598e958664579abe];
                this.x435d3cb48b86c624 = (int)(this.x598e958664579abe * 0.72f);
            }

            public int Add(string value)
            {
                int index = this.x407b398ce62a287e(value);
                int num2 = this.x084a6c1006133d8f[index];
                if (num2 != 0)
                {
                    return (num2 - 1);
                }
                this.xfbef61d96554cb75[this.xe31bf898501baf9b++] = value;
                this.x084a6c1006133d8f[index] = this.xe31bf898501baf9b;
                if (this.xe31bf898501baf9b > this.x435d3cb48b86c624)
                {
                    this.x12583168cc11d7a7();
                }
                return (this.xe31bf898501baf9b - 1);
            }

            private void x12583168cc11d7a7()
            {
                this.x598e958664579abe = xb2f12218b05a20d1[this.x306a401dfa1ce0a0++];
                this.x084a6c1006133d8f = new int[this.x598e958664579abe];
                string[] array = new string[this.x598e958664579abe];
                this.xfbef61d96554cb75.CopyTo(array, 0);
                this.xfbef61d96554cb75 = array;
                this.x49af0795ce5ff788();
            }

            private int x407b398ce62a287e(string xbcea506a33cf9111)
            {
                int num = xbcea506a33cf9111.GetHashCode() & 0x7fffffff;
                int index = num % this.x598e958664579abe;
                int num3 = (index > 1) ? index : 1;
                int num4 = this.x598e958664579abe;
                while (0 < num4--)
                {
                    int num5 = this.x084a6c1006133d8f[index];
                    if (num5 == 0)
                    {
                        return index;
                    }
                    if (string.CompareOrdinal(xbcea506a33cf9111, this.xfbef61d96554cb75[num5 - 1]) == 0)
                    {
                        return index;
                    }
                    index = (index + num3) % this.x598e958664579abe;
                }
                throw new InvalidOperationException("Failed to locate a bucket.");
            }

            private void x49af0795ce5ff788()
            {
                this.x435d3cb48b86c624 = (int)(this.x598e958664579abe * 0.72f);
                for (int i = 0; i < this.xe31bf898501baf9b; i++)
                {
                    int index = this.x407b398ce62a287e(this.xfbef61d96554cb75[i]);
                    this.x084a6c1006133d8f[index] = i + 1;
                }
            }

            // Properties
            public int Count
            {
                get
                {
                    return this.xe31bf898501baf9b;
                }
            }

            public string this[int index]
            {
                get
                {
                    return this.xfbef61d96554cb75[index];
                }
            }
        }

        private class x8c14939588a937db
        {
            // Fields
            private Type x7410274150a65d8d;

            // Methods
            public x8c14939588a937db(object value)
            {
                this.x7410274150a65d8d = value.GetType();
            }

            public override bool Equals(object obj)
            {
                return this.x7410274150a65d8d.Equals((obj as SerializationWriter.x8c14939588a937db).x7410274150a65d8d);
            }

            public override int GetHashCode()
            {
                return this.x7410274150a65d8d.GetHashCode();
            }

            // Properties
            public Type WrappedType
            {
                get
                {
                    return this.x7410274150a65d8d;
                }
            }
        }
    }


}
