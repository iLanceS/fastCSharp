using System;
using System.Collections.Generic;

namespace fastCSharp.testCase
{
    /// <summary>
    /// 测试数据
    /// </summary>
    class data
    {
        #region 测试数据枚举定义
        internal enum byteEnum : byte
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80
        }
        [Flags]
        internal enum byteFlagEnum : byte
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80
        }
        internal enum sByteEnum : sbyte
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40
        }
        [Flags]
        internal enum sByteFlagEnum : sbyte
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40
        }
        internal enum shortEnum : short
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000
        }
        [Flags]
        internal enum shortFlagEnum : short
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000
        }
        internal enum uShortEnum : ushort
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000
        }
        [Flags]
        internal enum uShortFlagEnum : ushort
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000
        }
        internal enum intEnum : int
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x4000000,
            AB = 0x8000000,
            BA = 0x10000000,
            BB = 0x20000000
        }
        [Flags]
        internal enum intFlagEnum : int
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x4000000,
            AB = 0x8000000,
            BA = 0x10000000,
            BB = 0x20000000
        }
        internal enum uIntEnum : uint
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x4000000,
            AB = 0x8000000,
            BA = 0x10000000,
            BB = 0x20000000
        }
        [Flags]
        internal enum uIntFlagEnum : uint
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x4000000,
            AB = 0x8000000,
            BA = 0x10000000,
            BB = 0x20000000,
        }
        internal enum longEnum : long
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x400000000000000,
            AB = 0x800000000000000,
            BA = 0x1000000000000000,
            BB = 0x2000000000000000
        }
        [Flags]
        internal enum longFlagEnum : long
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x400000000000000,
            AB = 0x800000000000000,
            BA = 0x1000000000000000,
            BB = 0x2000000000000000
        }
        internal enum uLongEnum : ulong
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x400000000000000,
            AB = 0x800000000000000,
            BA = 0x1000000000000000,
            BB = 0x2000000000000000
        }
        [Flags]
        internal enum uLongFlagEnum : ulong
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            E = 0x10,
            F = 0x20,
            G = 0x40,
            H = 0x80,
            I = 0x100,
            J = 0x200,
            K = 0x400,
            L = 0x800,
            M = 0x1000,
            N = 0x2000,
            O = 0x4000,
            P = 0x8000,
            Q = 0x10000,
            R = 0x20000,
            S = 0x40000,
            T = 0x80000,
            U = 0x100000,
            V = 0x200000,
            W = 0x400000,
            X = 0x800000,
            Y = 0x1000000,
            Z = 0x2000000,
            AA = 0x400000000000000,
            AB = 0x800000000000000,
            BA = 0x1000000000000000,
            BB = 0x2000000000000000
        }
        #endregion
        /// <summary>
        /// 空壳类型定义
        /// </summary>
        internal class noMemberClass
        {
        }
        /// <summary>
        /// 引用类型定义
        /// </summary>
        internal class memberClass
        {
            public string String;
            public int Int;
        }
        /// <summary>
        /// 值类型成员包装处理测试数据定义
        /// </summary>
        [fastCSharp.emit.boxSerialize]
        internal struct boxStruct
        {
            public int Int;
        }
        /// <summary>
        /// 引用类型成员包装处理测试数据定义
        /// </summary>
        [fastCSharp.emit.boxSerialize]
        internal struct boxClass
        {
            public memberClass Value;
        }
        /// <summary>
        /// 字段数据定义(引用类型外壳)
        /// </summary>
        internal class filedData
        {
            public bool Bool;
            public byte Byte;
            public sbyte SByte;
            public short Short;
            public ushort UShort;
            public int Int;
            public uint UInt;
            public long Long;
            public ulong ULong;
            public DateTime DateTime;
            public float Float;
            public double Double;
            public decimal Decimal;
            public Guid Guid;
            public char Char;
            public byteEnum ByteEnum;
            public byteFlagEnum ByteFlagEnum;
            public sByteEnum SByteEnum;
            public sByteFlagEnum SByteFlagEnum;
            public shortEnum ShortEnum;
            public shortFlagEnum ShortFlagEnum;
            public uShortEnum UShortEnum;
            public uShortFlagEnum UShortFlagEnum;
            public intEnum IntEnum;
            public intFlagEnum IntFlagEnum;
            public uIntEnum UIntEnum;
            public uIntFlagEnum UIntFlagEnum;
            public longEnum LongEnum;
            public longFlagEnum LongFlagEnum;
            public uLongEnum ULongEnum;
            public uLongFlagEnum ULongFlagEnum;
            public KeyValuePair<string, int> KeyValuePair;
            public keyValue<int, string> KeyValue;

            public bool? BoolNull;
            public byte? ByteNull;
            public sbyte? SByteNull;
            public short? ShortNull;
            public ushort? UShortNull;
            public int? IntNull;
            public uint? UIntNull;
            public long? LongNull;
            public ulong? ULongNull;
            public DateTime? DateTimeNull;
            public float? FloatNull;
            public double? DoubleNull;
            public decimal? DecimalNull;
            public Guid? GuidNull;
            public char? CharNull;
            public byteEnum? ByteEnumNull;
            public byteFlagEnum? ByteFlagEnumNull;
            public sByteEnum? SByteEnumNull;
            public sByteFlagEnum? SByteFlagEnumNull;
            public shortEnum? ShortEnumNull;
            public shortFlagEnum? ShortFlagEnumNull;
            public uShortEnum? UShortEnumNull;
            public uShortFlagEnum? UShortFlagEnumNull;
            public intEnum? IntEnumNull;
            public intFlagEnum? IntFlagEnumNull;
            public uIntEnum? UIntEnumNull;
            public uIntFlagEnum? UIntFlagEnumNull;
            public longEnum? LongEnumNull;
            public longFlagEnum? LongFlagEnumNull;
            public uLongEnum? ULongEnumNull;
            public uLongFlagEnum? ULongFlagEnumNull;
            public KeyValuePair<string, int?>? KeyValuePairNull;
            public keyValue<int?, string>? KeyValueNull;

            public string String;
            public string String2;
            public subString SubString;
            public subString SubString2;
            public memberClass MemberClass;
            public memberClass MemberClass2;
            public noMemberClass NoMemberClass;
            public noMemberClass NoMemberClass2;
            public noMemberClass NoMemberClass3;
            public boxStruct BoxStruct;
            public boxStruct BoxStruct2;
            public boxClass BoxClass;
            public boxClass BoxClass2;

            public bool[] BoolArray;
            public byte[] ByteArray;
            public sbyte[] SByteArray;
            public short[] ShortArray;
            public ushort[] UShortArray;
            public int[] IntArray;
            public uint[] UIntArray;
            public long[] LongArray;
            public ulong[] ULongArray;
            public DateTime[] DateTimeArray;
            public float[] FloatArray;
            public double[] DoubleArray;
            public decimal[] DecimalArray;
            public Guid[] GuidArray;
            public char[] CharArray;
            public byteEnum[] ByteEnumArray;
            public byteFlagEnum[] ByteFlagEnumArray;
            public sByteEnum[] SByteEnumArray;
            public sByteFlagEnum[] SByteFlagEnumArray;
            public shortEnum[] ShortEnumArray;
            public shortFlagEnum[] ShortFlagEnumArray;
            public uShortEnum[] UShortEnumArray;
            public uShortFlagEnum[] UShortFlagEnumArray;
            public intEnum[] IntEnumArray;
            public intFlagEnum[] IntFlagEnumArray;
            public uIntEnum[] UIntEnumArray;
            public uIntFlagEnum[] UIntFlagEnumArray;
            public longEnum[] LongEnumArray;
            public longFlagEnum[] LongFlagEnumArray;
            public uLongEnum[] ULongEnumArray;
            public uLongFlagEnum[] ULongFlagEnumArray;
            public KeyValuePair<string, int>[] KeyValuePairArray;
            public keyValue<int, string>[] KeyValueArray;

            public bool?[] BoolNullArray;
            public byte?[] ByteNullArray;
            public sbyte?[] SByteNullArray;
            public short?[] ShortNullArray;
            public ushort?[] UShortNullArray;
            public int?[] IntNullArray;
            public uint?[] UIntNullArray;
            public long?[] LongNullArray;
            public ulong?[] ULongNullArray;
            public DateTime?[] DateTimeNullArray;
            public float?[] FloatNullArray;
            public double?[] DoubleNullArray;
            public decimal?[] DecimalNullArray;
            public Guid?[] GuidNullArray;
            public char?[] CharNullArray;
            public byteEnum?[] ByteEnumNullArray;
            public byteFlagEnum?[] ByteFlagEnumNullArray;
            public sByteEnum?[] SByteEnumNullArray;
            public sByteFlagEnum?[] SByteFlagEnumNullArray;
            public shortEnum?[] ShortEnumNullArray;
            public shortFlagEnum?[] ShortFlagEnumNullArray;
            public uShortEnum?[] UShortEnumNullArray;
            public uShortFlagEnum?[] UShortFlagEnumNullArray;
            public intEnum?[] IntEnumNullArray;
            public intFlagEnum?[] IntFlagEnumNullArray;
            public uIntEnum?[] UIntEnumNullArray;
            public uIntFlagEnum?[] UIntFlagEnumNullArray;
            public longEnum?[] LongEnumNullArray;
            public longFlagEnum?[] LongFlagEnumNullArray;
            public uLongEnum?[] ULongEnumNullArray;
            public uLongFlagEnum?[] ULongFlagEnumNullArray;
            public KeyValuePair<string, int?>?[] KeyValuePairNullArray;
            public keyValue<int?, string>?[] KeyValueNullArray;

            public string[] StringArray;
            public subString[] SubStringArray;
            public memberClass[] MemberClassArray;
            public noMemberClass[] NoMemberClassArray;

            public bool[] BoolArray2;
            public byte[] ByteArray2;
            public sbyte[] SByteArray2;
            public short[] ShortArray2;
            public ushort[] UShortArray2;
            public int[] IntArray2;
            public uint[] UIntArray2;
            public long[] LongArray2;
            public ulong[] ULongArray2;
            public DateTime[] DateTimeArray2;
            public float[] FloatArray2;
            public double[] DoubleArray2;
            public decimal[] DecimalArray2;
            public Guid[] GuidArray2;
            public char[] CharArray2;
            public byteEnum[] ByteEnumArray2;
            public byteFlagEnum[] ByteFlagEnumArray2;
            public sByteEnum[] SByteEnumArray2;
            public sByteFlagEnum[] SByteFlagEnumArray2;
            public shortEnum[] ShortEnumArray2;
            public shortFlagEnum[] ShortFlagEnumArray2;
            public uShortEnum[] UShortEnumArray2;
            public uShortFlagEnum[] UShortFlagEnumArray2;
            public intEnum[] IntEnumArray2;
            public intFlagEnum[] IntFlagEnumArray2;
            public uIntEnum[] UIntEnumArray2;
            public uIntFlagEnum[] UIntFlagEnumArray2;
            public longEnum[] LongEnumArray2;
            public longFlagEnum[] LongFlagEnumArray2;
            public uLongEnum[] ULongEnumArray2;
            public uLongFlagEnum[] ULongFlagEnumArray2;
            public KeyValuePair<string, int>[] KeyValuePairArray2;
            public keyValue<int, string>[] KeyValueArray2;

            public bool?[] BoolNullArray2;
            public byte?[] ByteNullArray2;
            public sbyte?[] SByteNullArray2;
            public short?[] ShortNullArray2;
            public ushort?[] UShortNullArray2;
            public int?[] IntNullArray2;
            public uint?[] UIntNullArray2;
            public long?[] LongNullArray2;
            public ulong?[] ULongNullArray2;
            public DateTime?[] DateTimeNullArray2;
            public float?[] FloatNullArray2;
            public double?[] DoubleNullArray2;
            public decimal?[] DecimalNullArray2;
            public Guid?[] GuidNullArray2;
            public char?[] CharNullArray2;
            public byteEnum?[] ByteEnumNullArray2;
            public byteFlagEnum?[] ByteFlagEnumNullArray2;
            public sByteEnum?[] SByteEnumNullArray2;
            public sByteFlagEnum?[] SByteFlagEnumNullArray2;
            public shortEnum?[] ShortEnumNullArray2;
            public shortFlagEnum?[] ShortFlagEnumNullArray2;
            public uShortEnum?[] UShortEnumNullArray2;
            public uShortFlagEnum?[] UShortFlagEnumNullArray2;
            public intEnum?[] IntEnumNullArray2;
            public intFlagEnum?[] IntFlagEnumNullArray2;
            public uIntEnum?[] UIntEnumNullArray2;
            public uIntFlagEnum?[] UIntFlagEnumNullArray2;
            public longEnum?[] LongEnumNullArray2;
            public longFlagEnum?[] LongFlagEnumNullArray2;
            public uLongEnum?[] ULongEnumNullArray2;
            public uLongFlagEnum?[] ULongFlagEnumNullArray2;
            public KeyValuePair<string, int?>?[] KeyValuePairNullArray2;
            public keyValue<int?, string>?[] KeyValueNullArray2;

            public string[] StringArray2;
            public subString[] SubStringArray2;
            public memberClass[] MemberClassArray2;
            public noMemberClass[] NoMemberClassArray2;
            public noMemberClass[] NoMemberClassArray3;
            public boxStruct[] BoxStructArray;
            public boxStruct[] BoxStructArray2;
            public boxClass[] BoxClassArray;
            public boxClass[] BoxClassArray2;

            public List<bool> BoolList;
            public List<byte> ByteList;
            public List<sbyte> SByteList;
            public List<short> ShortList;
            public List<ushort> UShortList;
            public List<int> IntList;
            public List<uint> UIntList;
            public List<long> LongList;
            public List<ulong> ULongList;
            public List<DateTime> DateTimeList;
            public List<float> FloatList;
            public List<double> DoubleList;
            public List<decimal> DecimalList;
            public List<Guid> GuidList;
            public List<char> CharList;
            public List<byteEnum> ByteEnumList;
            public List<byteFlagEnum> ByteFlagEnumList;
            public List<sByteEnum> SByteEnumList;
            public List<sByteFlagEnum> SByteFlagEnumList;
            public List<shortEnum> ShortEnumList;
            public List<shortFlagEnum> ShortFlagEnumList;
            public List<uShortEnum> UShortEnumList;
            public List<uShortFlagEnum> UShortFlagEnumList;
            public List<intEnum> IntEnumList;
            public List<intFlagEnum> IntFlagEnumList;
            public List<uIntEnum> UIntEnumList;
            public List<uIntFlagEnum> UIntFlagEnumList;
            public List<longEnum> LongEnumList;
            public List<longFlagEnum> LongFlagEnumList;
            public List<uLongEnum> ULongEnumList;
            public List<uLongFlagEnum> ULongFlagEnumList;
            public List<KeyValuePair<string, int>> KeyValuePairList;
            public List<keyValue<int, string>> KeyValueList;

            public List<bool?> BoolNullList;
            public List<byte?> ByteNullList;
            public List<sbyte?> SByteNullList;
            public List<short?> ShortNullList;
            public List<ushort?> UShortNullList;
            public List<int?> IntNullList;
            public List<uint?> UIntNullList;
            public List<long?> LongNullList;
            public List<ulong?> ULongNullList;
            public List<DateTime?> DateTimeNullList;
            public List<float?> FloatNullList;
            public List<double?> DoubleNullList;
            public List<decimal?> DecimalNullList;
            public List<Guid?> GuidNullList;
            public List<char?> CharNullList;
            public List<byteEnum?> ByteEnumNullList;
            public List<byteFlagEnum?> ByteFlagEnumNullList;
            public List<sByteEnum?> SByteEnumNullList;
            public List<sByteFlagEnum?> SByteFlagEnumNullList;
            public List<shortEnum?> ShortEnumNullList;
            public List<shortFlagEnum?> ShortFlagEnumNullList;
            public List<uShortEnum?> UShortEnumNullList;
            public List<uShortFlagEnum?> UShortFlagEnumNullList;
            public List<intEnum?> IntEnumNullList;
            public List<intFlagEnum?> IntFlagEnumNullList;
            public List<uIntEnum?> UIntEnumNullList;
            public List<uIntFlagEnum?> UIntFlagEnumNullList;
            public List<longEnum?> LongEnumNullList;
            public List<longFlagEnum?> LongFlagEnumNullList;
            public List<uLongEnum?> ULongEnumNullList;
            public List<uLongFlagEnum?> ULongFlagEnumNullList;
            public List<KeyValuePair<string, int?>?> KeyValuePairNullList;
            public List<keyValue<int?, string>?> KeyValueNullList;

            public List<string> StringList;
            public List<subString> SubStringList;
            public List<string> StringList2;
            public List<subString> SubStringList2;
            public List<memberClass> MemberClassList;
            public List<memberClass> MemberClassList2;
            public List<noMemberClass> NoMemberClassList;
            public List<noMemberClass> NoMemberClassList2;
            public List<noMemberClass> NoMemberClassList3;
            public List<boxStruct> BoxStructList;
            public List<boxStruct> BoxStructList2;
            public List<boxClass> BoxClassList;
            public List<boxClass> BoxClassList2;

            public subArray<bool> BoolSubArray;
            public subArray<byte> ByteSubArray;
            public subArray<sbyte> SByteSubArray;
            public subArray<short> ShortSubArray;
            public subArray<ushort> UShortSubArray;
            public subArray<int> IntSubArray;
            public subArray<uint> UIntSubArray;
            public subArray<long> LongSubArray;
            public subArray<ulong> ULongSubArray;
            public subArray<DateTime> DateTimeSubArray;
            public subArray<float> FloatSubArray;
            public subArray<double> DoubleSubArray;
            public subArray<decimal> DecimalSubArray;
            public subArray<Guid> GuidSubArray;
            public subArray<char> CharSubArray;
            public subArray<byteEnum> ByteEnumSubArray;
            public subArray<byteFlagEnum> ByteFlagEnumSubArray;
            public subArray<sByteEnum> SByteEnumSubArray;
            public subArray<sByteFlagEnum> SByteFlagEnumSubArray;
            public subArray<shortEnum> ShortEnumSubArray;
            public subArray<shortFlagEnum> ShortFlagEnumSubArray;
            public subArray<uShortEnum> UShortEnumSubArray;
            public subArray<uShortFlagEnum> UShortFlagEnumSubArray;
            public subArray<intEnum> IntEnumSubArray;
            public subArray<intFlagEnum> IntFlagEnumSubArray;
            public subArray<uIntEnum> UIntEnumSubArray;
            public subArray<uIntFlagEnum> UIntFlagEnumSubArray;
            public subArray<longEnum> LongEnumSubArray;
            public subArray<longFlagEnum> LongFlagEnumSubArray;
            public subArray<uLongEnum> ULongEnumSubArray;
            public subArray<uLongFlagEnum> ULongFlagEnumSubArray;
            public subArray<KeyValuePair<string, int>> KeyValuePairSubArray;
            public subArray<keyValue<int, string>> KeyValueSubArray;

            public subArray<bool?> BoolNullSubArray;
            public subArray<byte?> ByteNullSubArray;
            public subArray<sbyte?> SByteNullSubArray;
            public subArray<short?> ShortNullSubArray;
            public subArray<ushort?> UShortNullSubArray;
            public subArray<int?> IntNullSubArray;
            public subArray<uint?> UIntNullSubArray;
            public subArray<long?> LongNullSubArray;
            public subArray<ulong?> ULongNullSubArray;
            public subArray<DateTime?> DateTimeNullSubArray;
            public subArray<float?> FloatNullSubArray;
            public subArray<double?> DoubleNullSubArray;
            public subArray<decimal?> DecimalNullSubArray;
            public subArray<Guid?> GuidNullSubArray;
            public subArray<char?> CharNullSubArray;
            public subArray<byteEnum?> ByteEnumNullSubArray;
            public subArray<byteFlagEnum?> ByteFlagEnumNullSubArray;
            public subArray<sByteEnum?> SByteEnumNullSubArray;
            public subArray<sByteFlagEnum?> SByteFlagEnumNullSubArray;
            public subArray<shortEnum?> ShortEnumNullSubArray;
            public subArray<shortFlagEnum?> ShortFlagEnumNullSubArray;
            public subArray<uShortEnum?> UShortEnumNullSubArray;
            public subArray<uShortFlagEnum?> UShortFlagEnumNullSubArray;
            public subArray<intEnum?> IntEnumNullSubArray;
            public subArray<intFlagEnum?> IntFlagEnumNullSubArray;
            public subArray<uIntEnum?> UIntEnumNullSubArray;
            public subArray<uIntFlagEnum?> UIntFlagEnumNullSubArray;
            public subArray<longEnum?> LongEnumNullSubArray;
            public subArray<longFlagEnum?> LongFlagEnumNullSubArray;
            public subArray<uLongEnum?> ULongEnumNullSubArray;
            public subArray<uLongFlagEnum?> ULongFlagEnumNullSubArray;
            public subArray<KeyValuePair<string, int?>?> KeyValuePairNullSubArray;
            public subArray<keyValue<int?, string>?> KeyValueNullSubArray;

            public subArray<string> StringSubArray;
            public subArray<subString> SubStringSubArray;
            public subArray<string> StringSubArray2;
            public subArray<subString> SubStringSubArray2;
            public subArray<memberClass> MemberClassSubArray;
            public subArray<memberClass> MemberClassSubArray2;
            public subArray<boxStruct> BoxStructSubArray;
            public subArray<boxStruct> BoxStructSubArray2;
            public subArray<boxClass> BoxClassSubArray;
            public subArray<boxClass> BoxClassSubArray2;

            public Dictionary<string, int> StringDictionary;
            public Dictionary<int, string> IntDictionary;
        }
        /// <summary>
        /// 属性数据定义(引用类型外壳)
        /// </summary>
        internal class propertyData
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Int { get; set; }
            public uint UInt { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public DateTime DateTime { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public Guid Guid { get; set; }
            public char Char { get; set; }
            public string String { get; set; }
            public bool? BoolNull { get; set; }
            public byte? ByteNull { get; set; }
            public sbyte? SByteNull { get; set; }
            public short? ShortNull { get; set; }
            public ushort? UShortNull { get; set; }
            public int? IntNull { get; set; }
            public uint? UIntNull { get; set; }
            public long? LongNull { get; set; }
            public ulong? ULongNull { get; set; }
            public DateTime? DateTimeNull { get; set; }
            public float? FloatNull { get; set; }
            public double? DoubleNull { get; set; }
            public decimal? DecimalNull { get; set; }
            public Guid? GuidNull { get; set; }
            public char? CharNull { get; set; }
            public int[] Array { get; set; }
            public List<int> List { get; set; }
            public byteEnum Enum { get; set; }
            public byteFlagEnum FlagEnum { get; set; }
            public memberClass Class { get; set; }
            public boxStruct BoxStruct { get; set; }
            public boxClass BoxClass { get; set; }
            public Dictionary<string, int> StringDictionary { get; set; }
            public Dictionary<int, string> IntDictionary { get; set; }
        }
        /// <summary>
        /// 字段数据定义(值类型外壳)
        /// </summary>
        internal struct structFiledData
        {
            public bool Bool;
            public byte Byte;
            public sbyte SByte;
            public short Short;
            public ushort UShort;
            public int Int;
            public uint UInt;
            public long Long;
            public ulong ULong;
            public DateTime DateTime;
            public float Float;
            public double Double;
            public decimal Decimal;
            public Guid Guid;
            public char Char;
            public string String;
            public bool? BoolNull;
            public byte? ByteNull;
            public sbyte? SByteNull;
            public short? ShortNull;
            public ushort? UShortNull;
            public int? IntNull;
            public uint? UIntNull;
            public long? LongNull;
            public ulong? ULongNull;
            public DateTime? DateTimeNull;
            public float? FloatNull;
            public double? DoubleNull;
            public decimal? DecimalNull;
            public Guid? GuidNull;
            public char? CharNull;
            public int[] Array;
            public List<int> List;
            public byteEnum Enum;
            public byteFlagEnum FlagEnum;
            public memberClass Class;
            public boxStruct BoxStruct;
            public boxClass BoxClass;
            public Dictionary<string, int> StringDictionary;
            public Dictionary<int, string> IntDictionary;
        }
        /// <summary>
        /// 属性数据定义(值类型外壳)
        /// </summary>
        internal struct structPropertyData
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Int { get; set; }
            public uint UInt { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public DateTime DateTime { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public Guid Guid { get; set; }
            public char Char { get; set; }
            public string String { get; set; }
            public bool? BoolNull { get; set; }
            public byte? ByteNull { get; set; }
            public sbyte? SByteNull { get; set; }
            public short? ShortNull { get; set; }
            public ushort? UShortNull { get; set; }
            public int? IntNull { get; set; }
            public uint? UIntNull { get; set; }
            public long? LongNull { get; set; }
            public ulong? ULongNull { get; set; }
            public DateTime? DateTimeNull { get; set; }
            public float? FloatNull { get; set; }
            public double? DoubleNull { get; set; }
            public decimal? DecimalNull { get; set; }
            public Guid? GuidNull { get; set; }
            public char? CharNull { get; set; }
            public int[] Array { get; set; }
            public List<int> List { get; set; }
            public byteEnum Enum { get; set; }
            public byteFlagEnum FlagEnum { get; set; }
            public memberClass Class { get; set; }
            public boxStruct BoxStruct { get; set; }
            public boxClass BoxClass { get; set; }
            public Dictionary<string, int> StringDictionary { get; set; }
            public Dictionary<int, string> IntDictionary { get; set; }
        }
    }
}
