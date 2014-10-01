using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public enum SerializedType : byte
    {
        ArrayListType = 0xc7,
        BitArrayType = 0xc4,
        BitVector32Type = 0xc1,
        BooleanArrayType = 0xcf,
        BooleanFalseType = 0x86,
        BooleanTrueType = 0x85,
        ByteArrayType = 0xd0,
        ByteType = 0x87,
        CharArrayType = 0xd1,
        CharType = 0x89,
        DateTimeArrayType = 210,
        DateTimeType = 0xba,
        DBNullSequenceType = 0x83,
        DBNullType = 130,
        DecimalArrayType = 0xd3,
        DecimalType = 0x8a,
        DoubleArrayType = 0xd4,
        DoubleType = 0x8b,
        DuplicateValueSequenceType = 0xc3,
        DuplicateValueType = 0xc2,
        EmptyGuidType = 0xc0,
        EmptyObjectArrayType = 0xca,
        EmptyStringType = 0xb5,
        EmptyTypedArrayType = 0xc9,
        FullyOptimizedTypedArrayType = 0xcc,
        GuidArrayType = 0xd6,
        GuidType = 0xbf,
        Int16ArrayType = 0xd7,
        Int16Type = 0x8d,
        Int32ArrayType = 0xd8,
        Int32Type = 0x8e,
        Int64ArrayType = 0xd9,
        Int64Type = 0x8f,
        MaxDateTimeType = 0xbc,
        MinDateTimeType = 0xbb,
        MinusOneInt16Type = 0xab,
        MinusOneInt32Type = 0xac,
        MinusOneInt64Type = 0xad,
        NonOptimizedTypedArrayType = 0xcb,
        NStringType = 0xb9,
        NullSequenceType = 0x81,
        NullType = 0x80,
        ObjectArrayType = 200,
        OneByteType = 0x9f,
        OneCharType = 0xa1,
        OneDecimalType = 0xa2,
        OneDoubleType = 0xa3,
        OneInt16Type = 0xa5,
        OneInt32Type = 0xa6,
        OneInt64Type = 0xa7,
        OneSByteType = 160,
        OneSingleType = 0xa4,
        OneUInt16Type = 0xa8,
        OneUInt32Type = 0xa9,
        OneUInt64Type = 170,
        OptimizedDateTimeType = 0xb2,
        OptimizedDecimalType = 180,
        OptimizedInt32Type = 0xae,
        OptimizedInt64Type = 0xb0,
        OptimizedTimeSpanType = 0xb3,
        OptimizedUInt32Type = 0xaf,
        OptimizedUInt64Type = 0xb1,
        OtherType = 0x84,
        OtherTypedArrayType = 0xce,
        OwnedDataSerializableAndRecreatableType = 0xe0,
        PartiallyOptimizedTypedArrayType = 0xcd,
        Reserved1 = 0xff,
        Reserved10 = 0xf6,
        Reserved11 = 0xf5,
        Reserved12 = 0xf4,
        Reserved13 = 0xf3,
        Reserved14 = 0xf2,
        Reserved15 = 0xf1,
        Reserved16 = 240,
        Reserved17 = 0xef,
        Reserved18 = 0xee,
        Reserved19 = 0xed,
        Reserved2 = 0xfe,
        Reserved20 = 0xec,
        Reserved21 = 0xeb,
        Reserved22 = 0xea,
        Reserved23 = 0xe9,
        Reserved24 = 0xe8,
        Reserved25 = 0xe7,
        Reserved26 = 230,
        Reserved27 = 0xe5,
        Reserved28 = 0xe4,
        Reserved29 = 0xe3,
        Reserved3 = 0xfd,
        Reserved30 = 0xe2,
        Reserved31 = 0xe1,
        Reserved4 = 0xfc,
        Reserved5 = 0xfb,
        Reserved6 = 250,
        Reserved7 = 0xf9,
        Reserved8 = 0xf8,
        Reserved9 = 0xf7,
        SByteArrayType = 0xda,
        SByteType = 0x88,
        SingleArrayType = 0xd5,
        SingleCharStringType = 0xb7,
        SingleInstanceType = 0xc6,
        SingleSpaceType = 0xb6,
        SingleType = 140,
        StringArrayType = 0xdf,
        TimeSpanArrayType = 0xdb,
        TimeSpanType = 0xbd,
        TypeType = 0xc5,
        UInt16ArrayType = 220,
        UInt16Type = 0x90,
        UInt32ArrayType = 0xdd,
        UInt32Type = 0x91,
        UInt64ArrayType = 0xde,
        UInt64Type = 0x92,
        YStringType = 0xb8,
        ZeroByteType = 0x93,
        ZeroCharType = 0x95,
        ZeroDecimalType = 150,
        ZeroDoubleType = 0x97,
        ZeroInt16Type = 0x99,
        ZeroInt32Type = 0x9a,
        ZeroInt64Type = 0x9b,
        ZeroSByteType = 0x94,
        ZeroSingleType = 0x98,
        ZeroTimeSpanType = 190,
        ZeroUInt16Type = 0x9c,
        ZeroUInt32Type = 0x9d,
        ZeroUInt64Type = 0x9e
    }

 

 

}
