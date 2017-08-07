//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.testCase
{
    internal partial class dataSerialize
    {
        private partial class filedDataSerialize : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {

            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="_serializer_">对象序列化器</param>
            public unsafe void Serialize(fastCSharp.emit.dataSerializer _serializer_)
            {
                if (_serializer_.CheckPoint(this))
                {
                    {
                        fastCSharp.unmanagedStream _stream_ = _serializer_.Stream;
                        _stream_.PrepLength(sizeof(int) + 36 + 228);
                        byte* _write_ = _stream_.CurrentData;
                        *(int*)_write_ = 1073742205;
                        _write_ += sizeof(int);
                        byte* _nullMap_ = _write_;
                        fastCSharp.unsafer.memory.Clear32(_write_, 36);
                        _write_ += 36;
                        {
                            *(System.Guid*)_write_ = (System.Guid)Guid;
                            _write_ += sizeof(System.Guid);
                        }
                        {
                            *(decimal*)_write_ = (decimal)Decimal;
                            _write_ += sizeof(decimal);
                        }
                        if (!DecimalNull.HasValue) _nullMap_[2 >> 3] |= (byte)(1 << (2 & 7));
                        else
                        {
                            *(decimal*)_write_ = (decimal)DecimalNull;
                            _write_ += sizeof(decimal);
                        }
                        if (!GuidNull.HasValue) _nullMap_[3 >> 3] |= (byte)(1 << (3 & 7));
                        else
                        {
                            *(System.Guid*)_write_ = (System.Guid)GuidNull;
                            _write_ += sizeof(System.Guid);
                        }
                        if (!DateTimeNull.HasValue) _nullMap_[4 >> 3] |= (byte)(1 << (4 & 7));
                        else
                        {
                            *(System.DateTime*)_write_ = (System.DateTime)DateTimeNull;
                            _write_ += sizeof(System.DateTime);
                        }
                        {
                            *(double*)_write_ = (double)Double;
                            _write_ += sizeof(double);
                        }
                        if (!DoubleNull.HasValue) _nullMap_[5 >> 3] |= (byte)(1 << (5 & 7));
                        else
                        {
                            *(double*)_write_ = (double)DoubleNull;
                            _write_ += sizeof(double);
                        }
                        {
                            *(ulong*)_write_ = (ulong)ULong;
                            _write_ += sizeof(ulong);
                        }
                        {
                            *(long*)_write_ = (long)Long;
                            _write_ += sizeof(long);
                        }
                        {
                            *(long*)_write_ = (long)(long)LongEnum;
                            _write_ += sizeof(long);
                        }
                        {
                            *(ulong*)_write_ = (ulong)(ulong)ULongFlagEnum;
                            _write_ += sizeof(ulong);
                        }
                        if (!LongNull.HasValue) _nullMap_[6 >> 3] |= (byte)(1 << (6 & 7));
                        else
                        {
                            *(long*)_write_ = (long)LongNull;
                            _write_ += sizeof(long);
                        }
                        if (!ULongNull.HasValue) _nullMap_[7 >> 3] |= (byte)(1 << (7 & 7));
                        else
                        {
                            *(ulong*)_write_ = (ulong)ULongNull;
                            _write_ += sizeof(ulong);
                        }
                        {
                            *(long*)_write_ = (long)(long)LongFlagEnum;
                            _write_ += sizeof(long);
                        }
                        {
                            *(ulong*)_write_ = (ulong)(ulong)ULongEnum;
                            _write_ += sizeof(ulong);
                        }
                        {
                            *(System.DateTime*)_write_ = (System.DateTime)DateTime;
                            _write_ += sizeof(System.DateTime);
                        }
                        if (!FloatNull.HasValue) _nullMap_[8 >> 3] |= (byte)(1 << (8 & 7));
                        else
                        {
                            *(float*)_write_ = (float)FloatNull;
                            _write_ += sizeof(float);
                        }
                        {
                            *(float*)_write_ = (float)Float;
                            _write_ += sizeof(float);
                        }
                        if (!UIntNull.HasValue) _nullMap_[9 >> 3] |= (byte)(1 << (9 & 7));
                        else
                        {
                            *(uint*)_write_ = (uint)UIntNull;
                            _write_ += sizeof(uint);
                        }
                        {
                            *(uint*)_write_ = (uint)(uint)UIntFlagEnum;
                            _write_ += sizeof(uint);
                        }
                        {
                            *(uint*)_write_ = (uint)UInt;
                            _write_ += sizeof(uint);
                        }
                        {
                            *(uint*)_write_ = (uint)(uint)UIntEnum;
                            _write_ += sizeof(uint);
                        }
                        {
                            *(int*)_write_ = (int)(int)IntFlagEnum;
                            _write_ += sizeof(int);
                        }
                        if (!IntNull.HasValue) _nullMap_[10 >> 3] |= (byte)(1 << (10 & 7));
                        else
                        {
                            *(int*)_write_ = (int)IntNull;
                            _write_ += sizeof(int);
                        }
                        {
                            *(int*)_write_ = (int)(int)IntEnum;
                            _write_ += sizeof(int);
                        }
                        {
                            *(int*)_write_ = (int)Int;
                            _write_ += sizeof(int);
                        }
                        if (!CharNull.HasValue) _nullMap_[11 >> 3] |= (byte)(1 << (11 & 7));
                        else
                        {
                            *(char*)_write_ = (char)CharNull;
                            _write_ += sizeof(char);
                        }
                        {
                            *(char*)_write_ = (char)Char;
                            _write_ += sizeof(char);
                        }
                        {
                            *(short*)_write_ = (short)Short;
                            _write_ += sizeof(short);
                        }
                        if (!UShortNull.HasValue) _nullMap_[12 >> 3] |= (byte)(1 << (12 & 7));
                        else
                        {
                            *(ushort*)_write_ = (ushort)UShortNull;
                            _write_ += sizeof(ushort);
                        }
                        {
                            *(ushort*)_write_ = (ushort)UShort;
                            _write_ += sizeof(ushort);
                        }
                        {
                            *(short*)_write_ = (short)(short)ShortEnum;
                            _write_ += sizeof(short);
                        }
                        {
                            *(short*)_write_ = (short)(short)ShortFlagEnum;
                            _write_ += sizeof(short);
                        }
                        if (!ShortNull.HasValue) _nullMap_[13 >> 3] |= (byte)(1 << (13 & 7));
                        else
                        {
                            *(short*)_write_ = (short)ShortNull;
                            _write_ += sizeof(short);
                        }
                        {
                            *(ushort*)_write_ = (ushort)(ushort)UShortFlagEnum;
                            _write_ += sizeof(ushort);
                        }
                        {
                            *(ushort*)_write_ = (ushort)(ushort)UShortEnum;
                            _write_ += sizeof(ushort);
                        }
                        if (Bool) _nullMap_[14 >> 3] |= (byte)(1 << (14 & 7));
                        {
                            *(sbyte*)_write_ = (sbyte)SByte;
                            _write_ += sizeof(sbyte);
                        }
                        {
                            *(sbyte*)_write_ = (sbyte)(sbyte)SByteEnum;
                            _write_ += sizeof(sbyte);
                        }
                        {
                            *(sbyte*)_write_ = (sbyte)(sbyte)SByteFlagEnum;
                            _write_ += sizeof(sbyte);
                        }
                        if (!SByteNull.HasValue) _nullMap_[15 >> 3] |= (byte)(1 << (15 & 7));
                        else
                        {
                            *(sbyte*)_write_ = (sbyte)SByteNull;
                            _write_ += sizeof(sbyte);
                        }
                        if (!ByteNull.HasValue) _nullMap_[16 >> 3] |= (byte)(1 << (16 & 7));
                        else
                        {
                            *(byte*)_write_ = (byte)ByteNull;
                            _write_ += sizeof(byte);
                        }
                        {
                            *(byte*)_write_ = (byte)(byte)ByteEnum;
                            _write_ += sizeof(byte);
                        }
                        {
                            *(byte*)_write_ = (byte)(byte)ByteFlagEnum;
                            _write_ += sizeof(byte);
                        }
                        if (BoolNull.HasValue)
                        {
                            if ((bool)BoolNull) _nullMap_[0 >> 3] |= (byte)(3 << (0 & 7));
                            else _nullMap_[0 >> 3] |= (byte)(1 << (0 & 7));
                        }
                        {
                            *(byte*)_write_ = (byte)Byte;
                            _write_ += sizeof(byte);
                        }
                        if (!ShortEnumNull.HasValue) _nullMap_[17 >> 3] |= (byte)(1 << (17 & 7));
                        if (ShortEnumNullArray == null) _nullMap_[18 >> 3] |= (byte)(1 << (18 & 7));
                        if (ShortEnumNullArray2 == null) _nullMap_[19 >> 3] |= (byte)(1 << (19 & 7));
                        if (ShortEnumNullList == null) _nullMap_[20 >> 3] |= (byte)(1 << (20 & 7));
                        if (ShortEnumList == null) _nullMap_[21 >> 3] |= (byte)(1 << (21 & 7));
                        if (ShortEnumArray2 == null) _nullMap_[22 >> 3] |= (byte)(1 << (22 & 7));
                        if (ShortEnumArray == null) _nullMap_[23 >> 3] |= (byte)(1 << (23 & 7));
                        if (SByteNullList == null) _nullMap_[24 >> 3] |= (byte)(1 << (24 & 7));
                        if (UShortFlagEnumNullList == null) _nullMap_[25 >> 3] |= (byte)(1 << (25 & 7));
                        if (ShortArray == null) _nullMap_[26 >> 3] |= (byte)(1 << (26 & 7));
                        if (UShortFlagEnumNullArray2 == null) _nullMap_[27 >> 3] |= (byte)(1 << (27 & 7));
                        if (ShortArray2 == null) _nullMap_[28 >> 3] |= (byte)(1 << (28 & 7));
                        if (UShortFlagEnumNullArray == null) _nullMap_[29 >> 3] |= (byte)(1 << (29 & 7));
                        if (ShortFlagEnumArray == null) _nullMap_[30 >> 3] |= (byte)(1 << (30 & 7));
                        if (UShortFlagEnumArray2 == null) _nullMap_[31 >> 3] |= (byte)(1 << (31 & 7));
                        if (ShortNullArray == null) _nullMap_[32 >> 3] |= (byte)(1 << (32 & 7));
                        if (UShortFlagEnumList == null) _nullMap_[33 >> 3] |= (byte)(1 << (33 & 7));
                        if (ShortNullArray2 == null) _nullMap_[34 >> 3] |= (byte)(1 << (34 & 7));
                        if (ShortNullList == null) _nullMap_[35 >> 3] |= (byte)(1 << (35 & 7));
                        if (String == null) _nullMap_[36 >> 3] |= (byte)(1 << (36 & 7));
                        if (ShortList == null) _nullMap_[37 >> 3] |= (byte)(1 << (37 & 7));
                        if (ShortFlagEnumList == null) _nullMap_[38 >> 3] |= (byte)(1 << (38 & 7));
                        if (!UShortFlagEnumNull.HasValue) _nullMap_[39 >> 3] |= (byte)(1 << (39 & 7));
                        if (ShortFlagEnumArray2 == null) _nullMap_[40 >> 3] |= (byte)(1 << (40 & 7));
                        if (!ShortFlagEnumNull.HasValue) _nullMap_[41 >> 3] |= (byte)(1 << (41 & 7));
                        if (ShortFlagEnumNullArray == null) _nullMap_[42 >> 3] |= (byte)(1 << (42 & 7));
                        if (ShortFlagEnumNullList == null) _nullMap_[43 >> 3] |= (byte)(1 << (43 & 7));
                        if (ShortFlagEnumNullArray2 == null) _nullMap_[44 >> 3] |= (byte)(1 << (44 & 7));
                        if (SByteNullArray2 == null) _nullMap_[45 >> 3] |= (byte)(1 << (45 & 7));
                        if (SByteNullArray == null) _nullMap_[46 >> 3] |= (byte)(1 << (46 & 7));
                        if (UShortNullList == null) _nullMap_[47 >> 3] |= (byte)(1 << (47 & 7));
                        if (SByteArray == null) _nullMap_[48 >> 3] |= (byte)(1 << (48 & 7));
                        if (SByteArray2 == null) _nullMap_[49 >> 3] |= (byte)(1 << (49 & 7));
                        if (UShortNullArray2 == null) _nullMap_[50 >> 3] |= (byte)(1 << (50 & 7));
                        if (SByteEnumArray2 == null) _nullMap_[51 >> 3] |= (byte)(1 << (51 & 7));
                        if (SByteEnumArray == null) _nullMap_[52 >> 3] |= (byte)(1 << (52 & 7));
                        if (NoMemberClassList3 == null) _nullMap_[53 >> 3] |= (byte)(1 << (53 & 7));
                        if (NoMemberClassList2 == null) _nullMap_[54 >> 3] |= (byte)(1 << (54 & 7));
                        if (NoMemberClassList == null) _nullMap_[55 >> 3] |= (byte)(1 << (55 & 7));
                        if (NoMemberClass == null) _nullMap_[56 >> 3] |= (byte)(1 << (56 & 7));
                        if (NoMemberClass2 == null) _nullMap_[57 >> 3] |= (byte)(1 << (57 & 7));
                        if (NoMemberClass3 == null) _nullMap_[58 >> 3] |= (byte)(1 << (58 & 7));
                        if (NoMemberClassArray == null) _nullMap_[59 >> 3] |= (byte)(1 << (59 & 7));
                        if (NoMemberClassArray3 == null) _nullMap_[60 >> 3] |= (byte)(1 << (60 & 7));
                        if (NoMemberClassArray2 == null) _nullMap_[61 >> 3] |= (byte)(1 << (61 & 7));
                        if (SByteEnumList == null) _nullMap_[62 >> 3] |= (byte)(1 << (62 & 7));
                        if (!SByteEnumNull.HasValue) _nullMap_[63 >> 3] |= (byte)(1 << (63 & 7));
                        if (SByteEnumNullArray == null) _nullMap_[64 >> 3] |= (byte)(1 << (64 & 7));
                        if (SByteFlagEnumNullArray2 == null) _nullMap_[65 >> 3] |= (byte)(1 << (65 & 7));
                        if (UShortList == null) _nullMap_[66 >> 3] |= (byte)(1 << (66 & 7));
                        if (SByteFlagEnumNullList == null) _nullMap_[67 >> 3] |= (byte)(1 << (67 & 7));
                        if (SByteList == null) _nullMap_[68 >> 3] |= (byte)(1 << (68 & 7));
                        if (SByteFlagEnumNullArray == null) _nullMap_[69 >> 3] |= (byte)(1 << (69 & 7));
                        if (!SByteFlagEnumNull.HasValue) _nullMap_[70 >> 3] |= (byte)(1 << (70 & 7));
                        if (SByteFlagEnumList == null) _nullMap_[71 >> 3] |= (byte)(1 << (71 & 7));
                        if (SByteEnumNullList == null) _nullMap_[72 >> 3] |= (byte)(1 << (72 & 7));
                        if (SByteEnumNullArray2 == null) _nullMap_[73 >> 3] |= (byte)(1 << (73 & 7));
                        if (UShortNullArray == null) _nullMap_[74 >> 3] |= (byte)(1 << (74 & 7));
                        if (SByteFlagEnumArray2 == null) _nullMap_[75 >> 3] |= (byte)(1 << (75 & 7));
                        if (SByteFlagEnumArray == null) _nullMap_[76 >> 3] |= (byte)(1 << (76 & 7));
                        if (String2 == null) _nullMap_[77 >> 3] |= (byte)(1 << (77 & 7));
                        if (StringArray == null) _nullMap_[78 >> 3] |= (byte)(1 << (78 & 7));
                        if (ULongNullList == null) _nullMap_[79 >> 3] |= (byte)(1 << (79 & 7));
                        if (ULongArray2 == null) _nullMap_[80 >> 3] |= (byte)(1 << (80 & 7));
                        if (ULongArray == null) _nullMap_[81 >> 3] |= (byte)(1 << (81 & 7));
                        if (UIntNullList == null) _nullMap_[82 >> 3] |= (byte)(1 << (82 & 7));
                        if (UIntNullArray2 == null) _nullMap_[83 >> 3] |= (byte)(1 << (83 & 7));
                        if (UIntNullArray == null) _nullMap_[84 >> 3] |= (byte)(1 << (84 & 7));
                        if (UIntFlagEnumNullList == null) _nullMap_[85 >> 3] |= (byte)(1 << (85 & 7));
                        if (UIntFlagEnumNullArray2 == null) _nullMap_[86 >> 3] |= (byte)(1 << (86 & 7));
                        if (UShortArray2 == null) _nullMap_[87 >> 3] |= (byte)(1 << (87 & 7));
                        if (UShortArray == null) _nullMap_[88 >> 3] |= (byte)(1 << (88 & 7));
                        if (UIntList == null) _nullMap_[89 >> 3] |= (byte)(1 << (89 & 7));
                        if (ULongNullArray2 == null) _nullMap_[90 >> 3] |= (byte)(1 << (90 & 7));
                        if (ULongEnumArray == null) _nullMap_[91 >> 3] |= (byte)(1 << (91 & 7));
                        if (ULongEnumArray2 == null) _nullMap_[92 >> 3] |= (byte)(1 << (92 & 7));
                        if (ULongFlagEnumArray2 == null) _nullMap_[93 >> 3] |= (byte)(1 << (93 & 7));
                        if (ULongFlagEnumArray == null) _nullMap_[94 >> 3] |= (byte)(1 << (94 & 7));
                        if (ULongFlagEnumList == null) _nullMap_[95 >> 3] |= (byte)(1 << (95 & 7));
                        if (!ULongFlagEnumNull.HasValue) _nullMap_[96 >> 3] |= (byte)(1 << (96 & 7));
                        if (ULongFlagEnumNullArray == null) _nullMap_[97 >> 3] |= (byte)(1 << (97 & 7));
                        if (ULongFlagEnumNullList == null) _nullMap_[98 >> 3] |= (byte)(1 << (98 & 7));
                        if (ULongFlagEnumNullArray2 == null) _nullMap_[99 >> 3] |= (byte)(1 << (99 & 7));
                        if (ULongList == null) _nullMap_[100 >> 3] |= (byte)(1 << (100 & 7));
                        if (ULongEnumList == null) _nullMap_[101 >> 3] |= (byte)(1 << (101 & 7));
                        if (ULongNullArray == null) _nullMap_[102 >> 3] |= (byte)(1 << (102 & 7));
                        if (!ULongEnumNull.HasValue) _nullMap_[103 >> 3] |= (byte)(1 << (103 & 7));
                        if (ULongEnumNullArray == null) _nullMap_[104 >> 3] |= (byte)(1 << (104 & 7));
                        if (ULongEnumNullArray2 == null) _nullMap_[105 >> 3] |= (byte)(1 << (105 & 7));
                        if (ULongEnumNullList == null) _nullMap_[106 >> 3] |= (byte)(1 << (106 & 7));
                        if (UIntFlagEnumNullArray == null) _nullMap_[107 >> 3] |= (byte)(1 << (107 & 7));
                        if (!UIntFlagEnumNull.HasValue) _nullMap_[108 >> 3] |= (byte)(1 << (108 & 7));
                        if (UIntFlagEnumList == null) _nullMap_[109 >> 3] |= (byte)(1 << (109 & 7));
                        if (SubStringArray2 == null) _nullMap_[110 >> 3] |= (byte)(1 << (110 & 7));
                        if (SubStringArray == null) _nullMap_[111 >> 3] |= (byte)(1 << (111 & 7));
                        if (SubStringList == null) _nullMap_[112 >> 3] |= (byte)(1 << (112 & 7));
                        if (SubStringList2 == null) _nullMap_[113 >> 3] |= (byte)(1 << (113 & 7));
                        if (UShortEnumNullList == null) _nullMap_[114 >> 3] |= (byte)(1 << (114 & 7));
                        if (StringDictionary == null) _nullMap_[115 >> 3] |= (byte)(1 << (115 & 7));
                        if (StringArray2 == null) _nullMap_[116 >> 3] |= (byte)(1 << (116 & 7));
                        if (UShortFlagEnumArray == null) _nullMap_[117 >> 3] |= (byte)(1 << (117 & 7));
                        if (StringList == null) _nullMap_[118 >> 3] |= (byte)(1 << (118 & 7));
                        if (StringList2 == null) _nullMap_[119 >> 3] |= (byte)(1 << (119 & 7));
                        if (UShortEnumNullArray2 == null) _nullMap_[120 >> 3] |= (byte)(1 << (120 & 7));
                        if (UIntArray == null) _nullMap_[121 >> 3] |= (byte)(1 << (121 & 7));
                        if (UIntEnumNullList == null) _nullMap_[122 >> 3] |= (byte)(1 << (122 & 7));
                        if (UShortEnumArray2 == null) _nullMap_[123 >> 3] |= (byte)(1 << (123 & 7));
                        if (UIntFlagEnumArray == null) _nullMap_[124 >> 3] |= (byte)(1 << (124 & 7));
                        if (UIntFlagEnumArray2 == null) _nullMap_[125 >> 3] |= (byte)(1 << (125 & 7));
                        if (UShortEnumArray == null) _nullMap_[126 >> 3] |= (byte)(1 << (126 & 7));
                        if (UIntEnumNullArray2 == null) _nullMap_[127 >> 3] |= (byte)(1 << (127 & 7));
                        if (UIntEnumNullArray == null) _nullMap_[128 >> 3] |= (byte)(1 << (128 & 7));
                        if (!UIntEnumNull.HasValue) _nullMap_[129 >> 3] |= (byte)(1 << (129 & 7));
                        if (UShortEnumNullArray == null) _nullMap_[130 >> 3] |= (byte)(1 << (130 & 7));
                        if (UIntArray2 == null) _nullMap_[131 >> 3] |= (byte)(1 << (131 & 7));
                        if (!UShortEnumNull.HasValue) _nullMap_[132 >> 3] |= (byte)(1 << (132 & 7));
                        if (UIntEnumArray == null) _nullMap_[133 >> 3] |= (byte)(1 << (133 & 7));
                        if (UIntEnumArray2 == null) _nullMap_[134 >> 3] |= (byte)(1 << (134 & 7));
                        if (UShortEnumList == null) _nullMap_[135 >> 3] |= (byte)(1 << (135 & 7));
                        if (UIntEnumList == null) _nullMap_[136 >> 3] |= (byte)(1 << (136 & 7));
                        if (MemberClassList2 == null) _nullMap_[137 >> 3] |= (byte)(1 << (137 & 7));
                        if (DateTimeArray == null) _nullMap_[138 >> 3] |= (byte)(1 << (138 & 7));
                        if (DateTimeArray2 == null) _nullMap_[139 >> 3] |= (byte)(1 << (139 & 7));
                        if (DateTimeList == null) _nullMap_[140 >> 3] |= (byte)(1 << (140 & 7));
                        if (DateTimeNullArray == null) _nullMap_[141 >> 3] |= (byte)(1 << (141 & 7));
                        if (DateTimeNullList == null) _nullMap_[142 >> 3] |= (byte)(1 << (142 & 7));
                        if (DateTimeNullArray2 == null) _nullMap_[143 >> 3] |= (byte)(1 << (143 & 7));
                        if (CharNullList == null) _nullMap_[144 >> 3] |= (byte)(1 << (144 & 7));
                        if (CharNullArray2 == null) _nullMap_[145 >> 3] |= (byte)(1 << (145 & 7));
                        if (ByteNullList == null) _nullMap_[146 >> 3] |= (byte)(1 << (146 & 7));
                        if (ByteNullArray2 == null) _nullMap_[147 >> 3] |= (byte)(1 << (147 & 7));
                        if (ByteNullArray == null) _nullMap_[148 >> 3] |= (byte)(1 << (148 & 7));
                        if (CharArray == null) _nullMap_[149 >> 3] |= (byte)(1 << (149 & 7));
                        if (CharNullArray == null) _nullMap_[150 >> 3] |= (byte)(1 << (150 & 7));
                        if (CharList == null) _nullMap_[151 >> 3] |= (byte)(1 << (151 & 7));
                        if (CharArray2 == null) _nullMap_[152 >> 3] |= (byte)(1 << (152 & 7));
                        if (DecimalArray == null) _nullMap_[153 >> 3] |= (byte)(1 << (153 & 7));
                        if (DoubleNullList == null) _nullMap_[154 >> 3] |= (byte)(1 << (154 & 7));
                        if (DoubleNullArray2 == null) _nullMap_[155 >> 3] |= (byte)(1 << (155 & 7));
                        if (FloatArray == null) _nullMap_[156 >> 3] |= (byte)(1 << (156 & 7));
                        if (FloatArray2 == null) _nullMap_[157 >> 3] |= (byte)(1 << (157 & 7));
                        if (FloatNullArray2 == null) _nullMap_[158 >> 3] |= (byte)(1 << (158 & 7));
                        if (FloatNullArray == null) _nullMap_[159 >> 3] |= (byte)(1 << (159 & 7));
                        if (FloatList == null) _nullMap_[160 >> 3] |= (byte)(1 << (160 & 7));
                        if (DoubleNullArray == null) _nullMap_[161 >> 3] |= (byte)(1 << (161 & 7));
                        if (DoubleList == null) _nullMap_[162 >> 3] |= (byte)(1 << (162 & 7));
                        if (DecimalNullArray == null) _nullMap_[163 >> 3] |= (byte)(1 << (163 & 7));
                        if (DecimalList == null) _nullMap_[164 >> 3] |= (byte)(1 << (164 & 7));
                        if (DecimalArray2 == null) _nullMap_[165 >> 3] |= (byte)(1 << (165 & 7));
                        if (DecimalNullArray2 == null) _nullMap_[166 >> 3] |= (byte)(1 << (166 & 7));
                        if (DecimalNullList == null) _nullMap_[167 >> 3] |= (byte)(1 << (167 & 7));
                        if (DoubleArray2 == null) _nullMap_[168 >> 3] |= (byte)(1 << (168 & 7));
                        if (DoubleArray == null) _nullMap_[169 >> 3] |= (byte)(1 << (169 & 7));
                        if (ByteList == null) _nullMap_[170 >> 3] |= (byte)(1 << (170 & 7));
                        if (BoxClassList2 == null) _nullMap_[171 >> 3] |= (byte)(1 << (171 & 7));
                        if (BoxClassList == null) _nullMap_[172 >> 3] |= (byte)(1 << (172 & 7));
                        if (BoxClassArray2 == null) _nullMap_[173 >> 3] |= (byte)(1 << (173 & 7));
                        if (BoxStructArray2 == null) _nullMap_[174 >> 3] |= (byte)(1 << (174 & 7));
                        if (BoxStructArray == null) _nullMap_[175 >> 3] |= (byte)(1 << (175 & 7));
                        if (BoxClassArray == null) _nullMap_[176 >> 3] |= (byte)(1 << (176 & 7));
                        if (BoolList == null) _nullMap_[177 >> 3] |= (byte)(1 << (177 & 7));
                        if (BoolArray2 == null) _nullMap_[178 >> 3] |= (byte)(1 << (178 & 7));
                        if (BoolArray == null) _nullMap_[179 >> 3] |= (byte)(1 << (179 & 7));
                        if (BoolNullArray == null) _nullMap_[180 >> 3] |= (byte)(1 << (180 & 7));
                        if (BoolNullArray2 == null) _nullMap_[181 >> 3] |= (byte)(1 << (181 & 7));
                        if (BoolNullList == null) _nullMap_[182 >> 3] |= (byte)(1 << (182 & 7));
                        if (BoxStructList == null) _nullMap_[183 >> 3] |= (byte)(1 << (183 & 7));
                        if (BoxStructList2 == null) _nullMap_[184 >> 3] |= (byte)(1 << (184 & 7));
                        if (ByteFlagEnumArray == null) _nullMap_[185 >> 3] |= (byte)(1 << (185 & 7));
                        if (ByteFlagEnumArray2 == null) _nullMap_[186 >> 3] |= (byte)(1 << (186 & 7));
                        if (ByteFlagEnumList == null) _nullMap_[187 >> 3] |= (byte)(1 << (187 & 7));
                        if (!ByteFlagEnumNull.HasValue) _nullMap_[188 >> 3] |= (byte)(1 << (188 & 7));
                        if (ByteFlagEnumNullList == null) _nullMap_[189 >> 3] |= (byte)(1 << (189 & 7));
                        if (ByteFlagEnumNullArray2 == null) _nullMap_[190 >> 3] |= (byte)(1 << (190 & 7));
                        if (ByteFlagEnumNullArray == null) _nullMap_[191 >> 3] |= (byte)(1 << (191 & 7));
                        if (ByteEnumNullList == null) _nullMap_[192 >> 3] |= (byte)(1 << (192 & 7));
                        if (ByteEnumNullArray2 == null) _nullMap_[193 >> 3] |= (byte)(1 << (193 & 7));
                        if (ByteArray == null) _nullMap_[194 >> 3] |= (byte)(1 << (194 & 7));
                        if (ByteArray2 == null) _nullMap_[195 >> 3] |= (byte)(1 << (195 & 7));
                        if (ByteEnumArray == null) _nullMap_[196 >> 3] |= (byte)(1 << (196 & 7));
                        if (ByteEnumArray2 == null) _nullMap_[197 >> 3] |= (byte)(1 << (197 & 7));
                        if (ByteEnumNullArray == null) _nullMap_[198 >> 3] |= (byte)(1 << (198 & 7));
                        if (!ByteEnumNull.HasValue) _nullMap_[199 >> 3] |= (byte)(1 << (199 & 7));
                        if (ByteEnumList == null) _nullMap_[200 >> 3] |= (byte)(1 << (200 & 7));
                        if (FloatNullList == null) _nullMap_[201 >> 3] |= (byte)(1 << (201 & 7));
                        if (LongEnumArray == null) _nullMap_[202 >> 3] |= (byte)(1 << (202 & 7));
                        if (LongArray2 == null) _nullMap_[203 >> 3] |= (byte)(1 << (203 & 7));
                        if (LongArray == null) _nullMap_[204 >> 3] |= (byte)(1 << (204 & 7));
                        if (LongEnumArray2 == null) _nullMap_[205 >> 3] |= (byte)(1 << (205 & 7));
                        if (LongEnumList == null) _nullMap_[206 >> 3] |= (byte)(1 << (206 & 7));
                        if (!LongEnumNull.HasValue) _nullMap_[207 >> 3] |= (byte)(1 << (207 & 7));
                        if (LongEnumNullList == null) _nullMap_[208 >> 3] |= (byte)(1 << (208 & 7));
                        if (LongEnumNullArray2 == null) _nullMap_[209 >> 3] |= (byte)(1 << (209 & 7));
                        if (LongEnumNullArray == null) _nullMap_[210 >> 3] |= (byte)(1 << (210 & 7));
                        if (KeyValuePairArray2 == null) _nullMap_[211 >> 3] |= (byte)(1 << (211 & 7));
                        if (KeyValuePairArray == null) _nullMap_[212 >> 3] |= (byte)(1 << (212 & 7));
                        if (KeyValuePairList == null) _nullMap_[213 >> 3] |= (byte)(1 << (213 & 7));
                        if (!KeyValuePairNull.HasValue) _nullMap_[214 >> 3] |= (byte)(1 << (214 & 7));
                        if (KeyValuePairNullArray == null) _nullMap_[215 >> 3] |= (byte)(1 << (215 & 7));
                        if (KeyValuePairNullList == null) _nullMap_[216 >> 3] |= (byte)(1 << (216 & 7));
                        if (KeyValuePairNullArray2 == null) _nullMap_[217 >> 3] |= (byte)(1 << (217 & 7));
                        if (LongNullList == null) _nullMap_[218 >> 3] |= (byte)(1 << (218 & 7));
                        if (LongNullArray2 == null) _nullMap_[219 >> 3] |= (byte)(1 << (219 & 7));
                        if (MemberClass == null) _nullMap_[220 >> 3] |= (byte)(1 << (220 & 7));
                        if (MemberClass2 == null) _nullMap_[221 >> 3] |= (byte)(1 << (221 & 7));
                        if (MemberClassList == null) _nullMap_[222 >> 3] |= (byte)(1 << (222 & 7));
                        if (MemberClassArray2 == null) _nullMap_[223 >> 3] |= (byte)(1 << (223 & 7));
                        if (MemberClassArray == null) _nullMap_[224 >> 3] |= (byte)(1 << (224 & 7));
                        if (LongNullArray == null) _nullMap_[225 >> 3] |= (byte)(1 << (225 & 7));
                        if (LongList == null) _nullMap_[226 >> 3] |= (byte)(1 << (226 & 7));
                        if (LongFlagEnumList == null) _nullMap_[227 >> 3] |= (byte)(1 << (227 & 7));
                        if (LongFlagEnumArray2 == null) _nullMap_[228 >> 3] |= (byte)(1 << (228 & 7));
                        if (LongFlagEnumArray == null) _nullMap_[229 >> 3] |= (byte)(1 << (229 & 7));
                        if (!LongFlagEnumNull.HasValue) _nullMap_[230 >> 3] |= (byte)(1 << (230 & 7));
                        if (LongFlagEnumNullArray == null) _nullMap_[231 >> 3] |= (byte)(1 << (231 & 7));
                        if (LongFlagEnumNullArray2 == null) _nullMap_[232 >> 3] |= (byte)(1 << (232 & 7));
                        if (LongFlagEnumNullList == null) _nullMap_[233 >> 3] |= (byte)(1 << (233 & 7));
                        if (KeyValueNullList == null) _nullMap_[234 >> 3] |= (byte)(1 << (234 & 7));
                        if (KeyValueNullArray2 == null) _nullMap_[235 >> 3] |= (byte)(1 << (235 & 7));
                        if (IntEnumArray2 == null) _nullMap_[236 >> 3] |= (byte)(1 << (236 & 7));
                        if (IntEnumArray == null) _nullMap_[237 >> 3] |= (byte)(1 << (237 & 7));
                        if (IntDictionary == null) _nullMap_[238 >> 3] |= (byte)(1 << (238 & 7));
                        if (IntEnumList == null) _nullMap_[239 >> 3] |= (byte)(1 << (239 & 7));
                        if (!IntEnumNull.HasValue) _nullMap_[240 >> 3] |= (byte)(1 << (240 & 7));
                        if (IntEnumNullArray == null) _nullMap_[241 >> 3] |= (byte)(1 << (241 & 7));
                        if (IntEnumNullList == null) _nullMap_[242 >> 3] |= (byte)(1 << (242 & 7));
                        if (IntEnumNullArray2 == null) _nullMap_[243 >> 3] |= (byte)(1 << (243 & 7));
                        if (IntArray2 == null) _nullMap_[244 >> 3] |= (byte)(1 << (244 & 7));
                        if (IntArray == null) _nullMap_[245 >> 3] |= (byte)(1 << (245 & 7));
                        if (GuidArray2 == null) _nullMap_[246 >> 3] |= (byte)(1 << (246 & 7));
                        if (GuidArray == null) _nullMap_[247 >> 3] |= (byte)(1 << (247 & 7));
                        if (GuidList == null) _nullMap_[248 >> 3] |= (byte)(1 << (248 & 7));
                        if (GuidNullArray == null) _nullMap_[249 >> 3] |= (byte)(1 << (249 & 7));
                        if (GuidNullArray2 == null) _nullMap_[250 >> 3] |= (byte)(1 << (250 & 7));
                        if (GuidNullList == null) _nullMap_[251 >> 3] |= (byte)(1 << (251 & 7));
                        if (IntFlagEnumArray == null) _nullMap_[252 >> 3] |= (byte)(1 << (252 & 7));
                        if (IntNullList == null) _nullMap_[253 >> 3] |= (byte)(1 << (253 & 7));
                        if (KeyValueArray == null) _nullMap_[254 >> 3] |= (byte)(1 << (254 & 7));
                        if (KeyValueArray2 == null) _nullMap_[255 >> 3] |= (byte)(1 << (255 & 7));
                        if (KeyValueNullArray == null) _nullMap_[256 >> 3] |= (byte)(1 << (256 & 7));
                        if (!KeyValueNull.HasValue) _nullMap_[257 >> 3] |= (byte)(1 << (257 & 7));
                        if (KeyValueList == null) _nullMap_[258 >> 3] |= (byte)(1 << (258 & 7));
                        if (IntNullArray2 == null) _nullMap_[259 >> 3] |= (byte)(1 << (259 & 7));
                        if (IntNullArray == null) _nullMap_[260 >> 3] |= (byte)(1 << (260 & 7));
                        if (!IntFlagEnumNull.HasValue) _nullMap_[261 >> 3] |= (byte)(1 << (261 & 7));
                        if (IntFlagEnumList == null) _nullMap_[262 >> 3] |= (byte)(1 << (262 & 7));
                        if (IntFlagEnumArray2 == null) _nullMap_[263 >> 3] |= (byte)(1 << (263 & 7));
                        if (IntFlagEnumNullArray == null) _nullMap_[264 >> 3] |= (byte)(1 << (264 & 7));
                        if (IntFlagEnumNullArray2 == null) _nullMap_[265 >> 3] |= (byte)(1 << (265 & 7));
                        if (IntFlagEnumNullList == null) _nullMap_[266 >> 3] |= (byte)(1 << (266 & 7));
                        if (IntList == null) _nullMap_[267 >> 3] |= (byte)(1 << (267 & 7));
                        _stream_.UnsafeAddSerializeLength((int)(_write_ - _stream_.CurrentData));
                        _stream_.PrepLength();
                        _serializer_.MemberStructSerialize(ULongFlagEnumNullSubArray);
                        if (ShortEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ShortEnumNull);
                        if (ShortEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ShortEnumNullArray);
                        if (ShortEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ShortEnumNullArray2);
                        _serializer_.MemberStructSerialize(ShortEnumNullSubArray);
                        if (ShortEnumNullList != null)
                        _serializer_.MemberClassSerialize(ShortEnumNullList);
                        if (ShortEnumList != null)
                        _serializer_.MemberClassSerialize(ShortEnumList);
                        if (ShortEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ShortEnumArray2);
                        if (ShortEnumArray != null)
                        _serializer_.MemberClassSerialize(ShortEnumArray);
                        _serializer_.MemberStructSerialize(SByteNullSubArray);
                        if (SByteNullList != null)
                        _serializer_.MemberClassSerialize(SByteNullList);
                        _serializer_.MemberStructSerialize(SByteSubArray);
                        if (UShortFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumNullList);
                        if (ShortArray != null)
                        _serializer_.MemberClassSerialize(ShortArray);
                        if (UShortFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumNullArray2);
                        if (ShortArray2 != null)
                        _serializer_.MemberClassSerialize(ShortArray2);
                        _serializer_.MemberStructSerialize(ShortEnumSubArray);
                        if (UShortFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumNullArray);
                        if (ShortFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumArray);
                        if (UShortFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumArray2);
                        if (ShortNullArray != null)
                        _serializer_.MemberClassSerialize(ShortNullArray);
                        if (UShortFlagEnumList != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumList);
                        if (ShortNullArray2 != null)
                        _serializer_.MemberClassSerialize(ShortNullArray2);
                        if (ShortNullList != null)
                        _serializer_.MemberClassSerialize(ShortNullList);
                        if (String != null)
                        _serializer_.MemberClassSerialize(String);
                        _serializer_.MemberStructSerialize(ShortSubArray);
                        _serializer_.MemberStructSerialize(ShortNullSubArray);
                        if (ShortList != null)
                        _serializer_.MemberClassSerialize(ShortList);
                        _serializer_.MemberStructSerialize(ShortFlagEnumSubArray);
                        if (ShortFlagEnumList != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumList);
                        if (UShortFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(UShortFlagEnumNull);
                        if (ShortFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumArray2);
                        if (ShortFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ShortFlagEnumNull);
                        if (ShortFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumNullArray);
                        _serializer_.MemberStructSerialize(ShortFlagEnumNullSubArray);
                        if (ShortFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumNullList);
                        if (ShortFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ShortFlagEnumNullArray2);
                        if (SByteNullArray2 != null)
                        _serializer_.MemberClassSerialize(SByteNullArray2);
                        if (SByteNullArray != null)
                        _serializer_.MemberClassSerialize(SByteNullArray);
                        _serializer_.MemberStructSerialize(UShortFlagEnumNullSubArray);
                        if (UShortNullList != null)
                        _serializer_.MemberClassSerialize(UShortNullList);
                        _serializer_.MemberStructSerialize(UShortNullSubArray);
                        if (SByteArray != null)
                        _serializer_.MemberClassSerialize(SByteArray);
                        if (SByteArray2 != null)
                        _serializer_.MemberClassSerialize(SByteArray2);
                        if (UShortNullArray2 != null)
                        _serializer_.MemberClassSerialize(UShortNullArray2);
                        if (SByteEnumArray2 != null)
                        _serializer_.MemberClassSerialize(SByteEnumArray2);
                        if (SByteEnumArray != null)
                        _serializer_.MemberClassSerialize(SByteEnumArray);
                        if (NoMemberClassList3 != null)
                        _serializer_.MemberClassSerialize(NoMemberClassList3);
                        if (NoMemberClassList2 != null)
                        _serializer_.MemberClassSerialize(NoMemberClassList2);
                        if (NoMemberClassList != null)
                        _serializer_.MemberClassSerialize(NoMemberClassList);
                        if (NoMemberClass != null)
                        _serializer_.MemberClassSerialize(NoMemberClass);
                        _serializer_.MemberStructSerialize(MemberClassSubArray2);
                        if (NoMemberClass2 != null)
                        _serializer_.MemberClassSerialize(NoMemberClass2);
                        if (NoMemberClass3 != null)
                        _serializer_.MemberClassSerialize(NoMemberClass3);
                        if (NoMemberClassArray != null)
                        _serializer_.MemberClassSerialize(NoMemberClassArray);
                        if (NoMemberClassArray3 != null)
                        _serializer_.MemberClassSerialize(NoMemberClassArray3);
                        if (NoMemberClassArray2 != null)
                        _serializer_.MemberClassSerialize(NoMemberClassArray2);
                        if (SByteEnumList != null)
                        _serializer_.MemberClassSerialize(SByteEnumList);
                        if (SByteEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(SByteEnumNull);
                        if (SByteEnumNullArray != null)
                        _serializer_.MemberClassSerialize(SByteEnumNullArray);
                        if (SByteFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumNullArray2);
                        if (UShortList != null)
                        _serializer_.MemberClassSerialize(UShortList);
                        if (SByteFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumNullList);
                        _serializer_.MemberStructSerialize(UShortFlagEnumSubArray);
                        _serializer_.MemberStructSerialize(SByteFlagEnumNullSubArray);
                        if (SByteList != null)
                        _serializer_.MemberClassSerialize(SByteList);
                        _serializer_.MemberStructSerialize(SByteFlagEnumSubArray);
                        if (SByteFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumNullArray);
                        if (SByteFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(SByteFlagEnumNull);
                        if (SByteFlagEnumList != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumList);
                        if (SByteEnumNullList != null)
                        _serializer_.MemberClassSerialize(SByteEnumNullList);
                        if (SByteEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(SByteEnumNullArray2);
                        _serializer_.MemberStructSerialize(SByteEnumNullSubArray);
                        _serializer_.MemberStructSerialize(SByteEnumSubArray);
                        if (UShortNullArray != null)
                        _serializer_.MemberClassSerialize(UShortNullArray);
                        if (SByteFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumArray2);
                        if (SByteFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(SByteFlagEnumArray);
                        if (String2 != null)
                        _serializer_.MemberClassSerialize(String2);
                        if (StringArray != null)
                        _serializer_.MemberClassSerialize(StringArray);
                        _serializer_.MemberStructSerialize(UIntNullSubArray);
                        _serializer_.MemberStructSerialize(ULongSubArray);
                        _serializer_.MemberStructSerialize(UIntSubArray);
                        _serializer_.MemberStructSerialize(ULongNullSubArray);
                        if (ULongNullList != null)
                        _serializer_.MemberClassSerialize(ULongNullList);
                        if (ULongArray2 != null)
                        _serializer_.MemberClassSerialize(ULongArray2);
                        if (ULongArray != null)
                        _serializer_.MemberClassSerialize(ULongArray);
                        if (UIntNullList != null)
                        _serializer_.MemberClassSerialize(UIntNullList);
                        if (UIntNullArray2 != null)
                        _serializer_.MemberClassSerialize(UIntNullArray2);
                        if (UIntNullArray != null)
                        _serializer_.MemberClassSerialize(UIntNullArray);
                        if (UIntFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumNullList);
                        if (UIntFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumNullArray2);
                        _serializer_.MemberStructSerialize(UIntFlagEnumNullSubArray);
                        _serializer_.MemberStructSerialize(UIntFlagEnumSubArray);
                        if (UShortArray2 != null)
                        _serializer_.MemberClassSerialize(UShortArray2);
                        if (UShortArray != null)
                        _serializer_.MemberClassSerialize(UShortArray);
                        if (UIntList != null)
                        _serializer_.MemberClassSerialize(UIntList);
                        if (ULongNullArray2 != null)
                        _serializer_.MemberClassSerialize(ULongNullArray2);
                        if (ULongEnumArray != null)
                        _serializer_.MemberClassSerialize(ULongEnumArray);
                        if (ULongEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ULongEnumArray2);
                        if (ULongFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumArray2);
                        if (ULongFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumArray);
                        if (ULongFlagEnumList != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumList);
                        if (ULongFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ULongFlagEnumNull);
                        if (ULongFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumNullArray);
                        if (ULongFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumNullList);
                        if (ULongFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ULongFlagEnumNullArray2);
                        _serializer_.MemberStructSerialize(ULongFlagEnumSubArray);
                        if (ULongList != null)
                        _serializer_.MemberClassSerialize(ULongList);
                        _serializer_.MemberStructSerialize(ULongEnumSubArray);
                        if (ULongEnumList != null)
                        _serializer_.MemberClassSerialize(ULongEnumList);
                        if (ULongNullArray != null)
                        _serializer_.MemberClassSerialize(ULongNullArray);
                        if (ULongEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ULongEnumNull);
                        if (ULongEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ULongEnumNullArray);
                        if (ULongEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ULongEnumNullArray2);
                        _serializer_.MemberStructSerialize(ULongEnumNullSubArray);
                        if (ULongEnumNullList != null)
                        _serializer_.MemberClassSerialize(ULongEnumNullList);
                        if (UIntFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumNullArray);
                        if (UIntFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(UIntFlagEnumNull);
                        if (UIntFlagEnumList != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumList);
                        if (SubStringArray2 != null)
                        _serializer_.MemberClassSerialize(SubStringArray2);
                        if (SubStringArray != null)
                        _serializer_.MemberClassSerialize(SubStringArray);
                        _serializer_.MemberStructSerialize(UShortEnumNullSubArray);
                        if (SubStringList != null)
                        _serializer_.MemberClassSerialize(SubStringList);
                        if (SubStringList2 != null)
                        _serializer_.MemberClassSerialize(SubStringList2);
                        if (UShortEnumNullList != null)
                        _serializer_.MemberClassSerialize(UShortEnumNullList);
                        _serializer_.MemberStructSerialize(SubStringSubArray);
                        _serializer_.MemberStructSerialize(SubString2);
                        _serializer_.MemberStructSerialize(SubString);
                        _serializer_.MemberStructSerialize(StringSubArray2);
                        if (StringDictionary != null)
                        _serializer_.MemberClassSerialize(StringDictionary);
                        if (StringArray2 != null)
                        _serializer_.MemberClassSerialize(StringArray2);
                        if (UShortFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(UShortFlagEnumArray);
                        if (StringList != null)
                        _serializer_.MemberClassSerialize(StringList);
                        if (StringList2 != null)
                        _serializer_.MemberClassSerialize(StringList2);
                        _serializer_.MemberStructSerialize(UShortEnumSubArray);
                        _serializer_.MemberStructSerialize(StringSubArray);
                        _serializer_.MemberStructSerialize(SubStringSubArray2);
                        if (UShortEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(UShortEnumNullArray2);
                        if (UIntArray != null)
                        _serializer_.MemberClassSerialize(UIntArray);
                        _serializer_.MemberStructSerialize(UIntEnumNullSubArray);
                        if (UIntEnumNullList != null)
                        _serializer_.MemberClassSerialize(UIntEnumNullList);
                        if (UShortEnumArray2 != null)
                        _serializer_.MemberClassSerialize(UShortEnumArray2);
                        _serializer_.MemberStructSerialize(UIntEnumSubArray);
                        if (UIntFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumArray);
                        if (UIntFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(UIntFlagEnumArray2);
                        if (UShortEnumArray != null)
                        _serializer_.MemberClassSerialize(UShortEnumArray);
                        if (UIntEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(UIntEnumNullArray2);
                        if (UIntEnumNullArray != null)
                        _serializer_.MemberClassSerialize(UIntEnumNullArray);
                        if (UIntEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(UIntEnumNull);
                        if (UShortEnumNullArray != null)
                        _serializer_.MemberClassSerialize(UShortEnumNullArray);
                        if (UIntArray2 != null)
                        _serializer_.MemberClassSerialize(UIntArray2);
                        if (UShortEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(UShortEnumNull);
                        if (UIntEnumArray != null)
                        _serializer_.MemberClassSerialize(UIntEnumArray);
                        if (UIntEnumArray2 != null)
                        _serializer_.MemberClassSerialize(UIntEnumArray2);
                        if (UShortEnumList != null)
                        _serializer_.MemberClassSerialize(UShortEnumList);
                        if (UIntEnumList != null)
                        _serializer_.MemberClassSerialize(UIntEnumList);
                        _serializer_.MemberStructSerialize(MemberClassSubArray);
                        if (MemberClassList2 != null)
                        _serializer_.MemberClassSerialize(MemberClassList2);
                        if (DateTimeArray != null)
                        _serializer_.MemberClassSerialize(DateTimeArray);
                        _serializer_.MemberStructSerialize(CharSubArray);
                        _serializer_.MemberStructSerialize(CharNullSubArray);
                        if (DateTimeArray2 != null)
                        _serializer_.MemberClassSerialize(DateTimeArray2);
                        if (DateTimeList != null)
                        _serializer_.MemberClassSerialize(DateTimeList);
                        if (DateTimeNullArray != null)
                        _serializer_.MemberClassSerialize(DateTimeNullArray);
                        _serializer_.MemberStructSerialize(DateTimeNullSubArray);
                        if (DateTimeNullList != null)
                        _serializer_.MemberClassSerialize(DateTimeNullList);
                        if (DateTimeNullArray2 != null)
                        _serializer_.MemberClassSerialize(DateTimeNullArray2);
                        if (CharNullList != null)
                        _serializer_.MemberClassSerialize(CharNullList);
                        if (CharNullArray2 != null)
                        _serializer_.MemberClassSerialize(CharNullArray2);
                        if (ByteNullList != null)
                        _serializer_.MemberClassSerialize(ByteNullList);
                        if (ByteNullArray2 != null)
                        _serializer_.MemberClassSerialize(ByteNullArray2);
                        if (ByteNullArray != null)
                        _serializer_.MemberClassSerialize(ByteNullArray);
                        _serializer_.MemberStructSerialize(ByteNullSubArray);
                        _serializer_.MemberStructSerialize(ByteSubArray);
                        if (CharArray != null)
                        _serializer_.MemberClassSerialize(CharArray);
                        if (CharNullArray != null)
                        _serializer_.MemberClassSerialize(CharNullArray);
                        if (CharList != null)
                        _serializer_.MemberClassSerialize(CharList);
                        if (CharArray2 != null)
                        _serializer_.MemberClassSerialize(CharArray2);
                        _serializer_.MemberStructSerialize(DateTimeSubArray);
                        if (DecimalArray != null)
                        _serializer_.MemberClassSerialize(DecimalArray);
                        _serializer_.MemberStructSerialize(DoubleNullSubArray);
                        if (DoubleNullList != null)
                        _serializer_.MemberClassSerialize(DoubleNullList);
                        if (DoubleNullArray2 != null)
                        _serializer_.MemberClassSerialize(DoubleNullArray2);
                        _serializer_.MemberStructSerialize(DoubleSubArray);
                        if (FloatArray != null)
                        _serializer_.MemberClassSerialize(FloatArray);
                        if (FloatArray2 != null)
                        _serializer_.MemberClassSerialize(FloatArray2);
                        if (FloatNullArray2 != null)
                        _serializer_.MemberClassSerialize(FloatNullArray2);
                        if (FloatNullArray != null)
                        _serializer_.MemberClassSerialize(FloatNullArray);
                        if (FloatList != null)
                        _serializer_.MemberClassSerialize(FloatList);
                        if (DoubleNullArray != null)
                        _serializer_.MemberClassSerialize(DoubleNullArray);
                        if (DoubleList != null)
                        _serializer_.MemberClassSerialize(DoubleList);
                        if (DecimalNullArray != null)
                        _serializer_.MemberClassSerialize(DecimalNullArray);
                        if (DecimalList != null)
                        _serializer_.MemberClassSerialize(DecimalList);
                        if (DecimalArray2 != null)
                        _serializer_.MemberClassSerialize(DecimalArray2);
                        if (DecimalNullArray2 != null)
                        _serializer_.MemberClassSerialize(DecimalNullArray2);
                        if (DecimalNullList != null)
                        _serializer_.MemberClassSerialize(DecimalNullList);
                        _serializer_.MemberStructSerialize(DecimalNullSubArray);
                        if (DoubleArray2 != null)
                        _serializer_.MemberClassSerialize(DoubleArray2);
                        if (DoubleArray != null)
                        _serializer_.MemberClassSerialize(DoubleArray);
                        _serializer_.MemberStructSerialize(DecimalSubArray);
                        if (ByteList != null)
                        _serializer_.MemberClassSerialize(ByteList);
                        _serializer_.MemberStructSerialize(ByteFlagEnumSubArray);
                        _serializer_.MemberStructSerialize(ByteFlagEnumNullSubArray);
                        if (BoxClassList2 != null)
                        _serializer_.MemberClassSerialize(BoxClassList2);
                        if (BoxClassList != null)
                        _serializer_.MemberClassSerialize(BoxClassList);
                        if (BoxClassArray2 != null)
                        _serializer_.MemberClassSerialize(BoxClassArray2);
                        _serializer_.MemberStructSerialize(BoxClassSubArray);
                        _serializer_.MemberStructSerialize(BoxClassSubArray2);
                        _serializer_.MemberStructSerialize(BoxStruct);
                        if (BoxStructArray2 != null)
                        _serializer_.MemberClassSerialize(BoxStructArray2);
                        if (BoxStructArray != null)
                        _serializer_.MemberClassSerialize(BoxStructArray);
                        _serializer_.MemberStructSerialize(BoxStruct2);
                        if (BoxClassArray != null)
                        _serializer_.MemberClassSerialize(BoxClassArray);
                        _serializer_.MemberStructSerialize(BoxClass2);
                        if (BoolList != null)
                        _serializer_.MemberClassSerialize(BoolList);
                        if (BoolArray2 != null)
                        _serializer_.MemberClassSerialize(BoolArray2);
                        if (BoolArray != null)
                        _serializer_.MemberClassSerialize(BoolArray);
                        if (BoolNullArray != null)
                        _serializer_.MemberClassSerialize(BoolNullArray);
                        if (BoolNullArray2 != null)
                        _serializer_.MemberClassSerialize(BoolNullArray2);
                        if (BoolNullList != null)
                        _serializer_.MemberClassSerialize(BoolNullList);
                        _serializer_.MemberStructSerialize(BoxClass);
                        _serializer_.MemberStructSerialize(BoolSubArray);
                        _serializer_.MemberStructSerialize(BoolNullSubArray);
                        if (BoxStructList != null)
                        _serializer_.MemberClassSerialize(BoxStructList);
                        if (BoxStructList2 != null)
                        _serializer_.MemberClassSerialize(BoxStructList2);
                        if (ByteFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumArray);
                        _serializer_.MemberStructSerialize(ByteEnumSubArray);
                        _serializer_.MemberStructSerialize(ByteEnumNullSubArray);
                        if (ByteFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumArray2);
                        if (ByteFlagEnumList != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumList);
                        if (ByteFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ByteFlagEnumNull);
                        if (ByteFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumNullList);
                        if (ByteFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumNullArray2);
                        if (ByteFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ByteFlagEnumNullArray);
                        if (ByteEnumNullList != null)
                        _serializer_.MemberClassSerialize(ByteEnumNullList);
                        if (ByteEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(ByteEnumNullArray2);
                        if (ByteArray != null)
                        _serializer_.MemberClassSerialize(ByteArray);
                        _serializer_.MemberStructSerialize(BoxStructSubArray2);
                        _serializer_.MemberStructSerialize(BoxStructSubArray);
                        if (ByteArray2 != null)
                        _serializer_.MemberClassSerialize(ByteArray2);
                        if (ByteEnumArray != null)
                        _serializer_.MemberClassSerialize(ByteEnumArray);
                        if (ByteEnumArray2 != null)
                        _serializer_.MemberClassSerialize(ByteEnumArray2);
                        if (ByteEnumNullArray != null)
                        _serializer_.MemberClassSerialize(ByteEnumNullArray);
                        if (ByteEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(ByteEnumNull);
                        if (ByteEnumList != null)
                        _serializer_.MemberClassSerialize(ByteEnumList);
                        if (FloatNullList != null)
                        _serializer_.MemberClassSerialize(FloatNullList);
                        _serializer_.MemberStructSerialize(FloatNullSubArray);
                        if (LongEnumArray != null)
                        _serializer_.MemberClassSerialize(LongEnumArray);
                        if (LongArray2 != null)
                        _serializer_.MemberClassSerialize(LongArray2);
                        if (LongArray != null)
                        _serializer_.MemberClassSerialize(LongArray);
                        if (LongEnumArray2 != null)
                        _serializer_.MemberClassSerialize(LongEnumArray2);
                        if (LongEnumList != null)
                        _serializer_.MemberClassSerialize(LongEnumList);
                        if (LongEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(LongEnumNull);
                        if (LongEnumNullList != null)
                        _serializer_.MemberClassSerialize(LongEnumNullList);
                        if (LongEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(LongEnumNullArray2);
                        if (LongEnumNullArray != null)
                        _serializer_.MemberClassSerialize(LongEnumNullArray);
                        _serializer_.MemberStructSerialize(KeyValueSubArray);
                        _serializer_.MemberStructSerialize(KeyValuePairSubArray);
                        if (KeyValuePairArray2 != null)
                        _serializer_.MemberClassSerialize(KeyValuePairArray2);
                        if (KeyValuePairArray != null)
                        _serializer_.MemberClassSerialize(KeyValuePairArray);
                        _serializer_.MemberStructSerialize(KeyValuePair);
                        if (KeyValuePairList != null)
                        _serializer_.MemberClassSerialize(KeyValuePairList);
                        if (KeyValuePairNull.HasValue)
                            _serializer_.MemberNullableSerialize(KeyValuePairNull);
                        if (KeyValuePairNullArray != null)
                        _serializer_.MemberClassSerialize(KeyValuePairNullArray);
                        _serializer_.MemberStructSerialize(KeyValuePairNullSubArray);
                        if (KeyValuePairNullList != null)
                        _serializer_.MemberClassSerialize(KeyValuePairNullList);
                        if (KeyValuePairNullArray2 != null)
                        _serializer_.MemberClassSerialize(KeyValuePairNullArray2);
                        _serializer_.MemberStructSerialize(LongEnumNullSubArray);
                        _serializer_.MemberStructSerialize(LongEnumSubArray);
                        _serializer_.MemberStructSerialize(LongNullSubArray);
                        if (LongNullList != null)
                        _serializer_.MemberClassSerialize(LongNullList);
                        if (LongNullArray2 != null)
                        _serializer_.MemberClassSerialize(LongNullArray2);
                        _serializer_.MemberStructSerialize(LongSubArray);
                        if (MemberClass != null)
                        _serializer_.MemberClassSerialize(MemberClass);
                        if (MemberClass2 != null)
                        _serializer_.MemberClassSerialize(MemberClass2);
                        if (MemberClassList != null)
                        _serializer_.MemberClassSerialize(MemberClassList);
                        if (MemberClassArray2 != null)
                        _serializer_.MemberClassSerialize(MemberClassArray2);
                        if (MemberClassArray != null)
                        _serializer_.MemberClassSerialize(MemberClassArray);
                        if (LongNullArray != null)
                        _serializer_.MemberClassSerialize(LongNullArray);
                        if (LongList != null)
                        _serializer_.MemberClassSerialize(LongList);
                        if (LongFlagEnumList != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumList);
                        if (LongFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumArray2);
                        if (LongFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumArray);
                        if (LongFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(LongFlagEnumNull);
                        if (LongFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumNullArray);
                        if (LongFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumNullArray2);
                        _serializer_.MemberStructSerialize(LongFlagEnumSubArray);
                        _serializer_.MemberStructSerialize(LongFlagEnumNullSubArray);
                        if (LongFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(LongFlagEnumNullList);
                        _serializer_.MemberStructSerialize(KeyValueNullSubArray);
                        if (KeyValueNullList != null)
                        _serializer_.MemberClassSerialize(KeyValueNullList);
                        if (KeyValueNullArray2 != null)
                        _serializer_.MemberClassSerialize(KeyValueNullArray2);
                        if (IntEnumArray2 != null)
                        _serializer_.MemberClassSerialize(IntEnumArray2);
                        if (IntEnumArray != null)
                        _serializer_.MemberClassSerialize(IntEnumArray);
                        if (IntDictionary != null)
                        _serializer_.MemberClassSerialize(IntDictionary);
                        if (IntEnumList != null)
                        _serializer_.MemberClassSerialize(IntEnumList);
                        if (IntEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(IntEnumNull);
                        if (IntEnumNullArray != null)
                        _serializer_.MemberClassSerialize(IntEnumNullArray);
                        _serializer_.MemberStructSerialize(IntEnumNullSubArray);
                        if (IntEnumNullList != null)
                        _serializer_.MemberClassSerialize(IntEnumNullList);
                        if (IntEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(IntEnumNullArray2);
                        if (IntArray2 != null)
                        _serializer_.MemberClassSerialize(IntArray2);
                        if (IntArray != null)
                        _serializer_.MemberClassSerialize(IntArray);
                        if (GuidArray2 != null)
                        _serializer_.MemberClassSerialize(GuidArray2);
                        if (GuidArray != null)
                        _serializer_.MemberClassSerialize(GuidArray);
                        _serializer_.MemberStructSerialize(FloatSubArray);
                        if (GuidList != null)
                        _serializer_.MemberClassSerialize(GuidList);
                        if (GuidNullArray != null)
                        _serializer_.MemberClassSerialize(GuidNullArray);
                        if (GuidNullArray2 != null)
                        _serializer_.MemberClassSerialize(GuidNullArray2);
                        _serializer_.MemberStructSerialize(GuidSubArray);
                        _serializer_.MemberStructSerialize(GuidNullSubArray);
                        if (GuidNullList != null)
                        _serializer_.MemberClassSerialize(GuidNullList);
                        _serializer_.MemberStructSerialize(IntEnumSubArray);
                        if (IntFlagEnumArray != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumArray);
                        _serializer_.MemberStructSerialize(IntSubArray);
                        _serializer_.MemberStructSerialize(IntNullSubArray);
                        if (IntNullList != null)
                        _serializer_.MemberClassSerialize(IntNullList);
                        _serializer_.MemberStructSerialize(KeyValue);
                        if (KeyValueArray != null)
                        _serializer_.MemberClassSerialize(KeyValueArray);
                        if (KeyValueArray2 != null)
                        _serializer_.MemberClassSerialize(KeyValueArray2);
                        if (KeyValueNullArray != null)
                        _serializer_.MemberClassSerialize(KeyValueNullArray);
                        if (KeyValueNull.HasValue)
                            _serializer_.MemberNullableSerialize(KeyValueNull);
                        if (KeyValueList != null)
                        _serializer_.MemberClassSerialize(KeyValueList);
                        if (IntNullArray2 != null)
                        _serializer_.MemberClassSerialize(IntNullArray2);
                        if (IntNullArray != null)
                        _serializer_.MemberClassSerialize(IntNullArray);
                        if (IntFlagEnumNull.HasValue)
                            _serializer_.MemberNullableSerialize(IntFlagEnumNull);
                        if (IntFlagEnumList != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumList);
                        if (IntFlagEnumArray2 != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumArray2);
                        if (IntFlagEnumNullArray != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumNullArray);
                        if (IntFlagEnumNullArray2 != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumNullArray2);
                        if (IntFlagEnumNullList != null)
                        _serializer_.MemberClassSerialize(IntFlagEnumNullList);
                        if (IntList != null)
                        _serializer_.MemberClassSerialize(IntList);
                        _serializer_.MemberStructSerialize(IntFlagEnumSubArray);
                        _serializer_.MemberStructSerialize(IntFlagEnumNullSubArray);
                        _serializer_.MemberStructSerialize(UShortSubArray);
                    }
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="_deSerializer_">对象反序列化器</param>
            public unsafe void DeSerialize(fastCSharp.emit.dataDeSerializer _deSerializer_)
            {
                _deSerializer_.AddPoint(this);
                if (_deSerializer_.CheckMemberCount(1073742205))
                {
                    byte* _read_ = _deSerializer_.Read;
                    byte* _nullMap_ = _read_;
                    _read_ += 36;
                    {
                        Guid = (*(System.Guid*)_read_);
                        _read_ += sizeof(System.Guid);
                    }
                    {
                        Decimal = (*(decimal*)_read_);
                        _read_ += sizeof(decimal);
                    }
                    if ((_nullMap_[2 >> 3] & (1 << (2 & 7))) != 0) DecimalNull = null;
                    else
                    {
                        DecimalNull = (*(decimal*)_read_);
                        _read_ += sizeof(decimal);
                    }
                    if ((_nullMap_[3 >> 3] & (1 << (3 & 7))) != 0) GuidNull = null;
                    else
                    {
                        GuidNull = (*(System.Guid*)_read_);
                        _read_ += sizeof(System.Guid);
                    }
                    if ((_nullMap_[4 >> 3] & (1 << (4 & 7))) != 0) DateTimeNull = null;
                    else
                    {
                        DateTimeNull = (*(System.DateTime*)_read_);
                        _read_ += sizeof(System.DateTime);
                    }
                    {
                        Double = (*(double*)_read_);
                        _read_ += sizeof(double);
                    }
                    if ((_nullMap_[5 >> 3] & (1 << (5 & 7))) != 0) DoubleNull = null;
                    else
                    {
                        DoubleNull = (*(double*)_read_);
                        _read_ += sizeof(double);
                    }
                    {
                        ULong = (*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    {
                        Long = (*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    {
                        LongEnum = (fastCSharp.testCase.data.longEnum)(*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    {
                        ULongFlagEnum = (fastCSharp.testCase.data.uLongFlagEnum)(*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    if ((_nullMap_[6 >> 3] & (1 << (6 & 7))) != 0) LongNull = null;
                    else
                    {
                        LongNull = (*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    if ((_nullMap_[7 >> 3] & (1 << (7 & 7))) != 0) ULongNull = null;
                    else
                    {
                        ULongNull = (*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    {
                        LongFlagEnum = (fastCSharp.testCase.data.longFlagEnum)(*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    {
                        ULongEnum = (fastCSharp.testCase.data.uLongEnum)(*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    {
                        DateTime = (*(System.DateTime*)_read_);
                        _read_ += sizeof(System.DateTime);
                    }
                    if ((_nullMap_[8 >> 3] & (1 << (8 & 7))) != 0) FloatNull = null;
                    else
                    {
                        FloatNull = (*(float*)_read_);
                        _read_ += sizeof(float);
                    }
                    {
                        Float = (*(float*)_read_);
                        _read_ += sizeof(float);
                    }
                    if ((_nullMap_[9 >> 3] & (1 << (9 & 7))) != 0) UIntNull = null;
                    else
                    {
                        UIntNull = (*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    {
                        UIntFlagEnum = (fastCSharp.testCase.data.uIntFlagEnum)(*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    {
                        UInt = (*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    {
                        UIntEnum = (fastCSharp.testCase.data.uIntEnum)(*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    {
                        IntFlagEnum = (fastCSharp.testCase.data.intFlagEnum)(*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    if ((_nullMap_[10 >> 3] & (1 << (10 & 7))) != 0) IntNull = null;
                    else
                    {
                        IntNull = (*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    {
                        IntEnum = (fastCSharp.testCase.data.intEnum)(*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    {
                        Int = (*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    if ((_nullMap_[11 >> 3] & (1 << (11 & 7))) != 0) CharNull = null;
                    else
                    {
                        CharNull = (*(char*)_read_);
                        _read_ += sizeof(char);
                    }
                    {
                        Char = (*(char*)_read_);
                        _read_ += sizeof(char);
                    }
                    {
                        Short = (*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    if ((_nullMap_[12 >> 3] & (1 << (12 & 7))) != 0) UShortNull = null;
                    else
                    {
                        UShortNull = (*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    {
                        UShort = (*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    {
                        ShortEnum = (fastCSharp.testCase.data.shortEnum)(*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    {
                        ShortFlagEnum = (fastCSharp.testCase.data.shortFlagEnum)(*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    if ((_nullMap_[13 >> 3] & (1 << (13 & 7))) != 0) ShortNull = null;
                    else
                    {
                        ShortNull = (*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    {
                        UShortFlagEnum = (fastCSharp.testCase.data.uShortFlagEnum)(*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    {
                        UShortEnum = (fastCSharp.testCase.data.uShortEnum)(*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    Bool = ((_nullMap_[14 >> 3] & (1 << (14 & 7))) != 0);
                    {
                        SByte = (*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    {
                        SByteEnum = (fastCSharp.testCase.data.sByteEnum)(*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    {
                        SByteFlagEnum = (fastCSharp.testCase.data.sByteFlagEnum)(*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    if ((_nullMap_[15 >> 3] & (1 << (15 & 7))) != 0) SByteNull = null;
                    else
                    {
                        SByteNull = (*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    if ((_nullMap_[16 >> 3] & (1 << (16 & 7))) != 0) ByteNull = null;
                    else
                    {
                        ByteNull = (*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    {
                        ByteEnum = (fastCSharp.testCase.data.byteEnum)(*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    {
                        ByteFlagEnum = (fastCSharp.testCase.data.byteFlagEnum)(*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    if ((_nullMap_[0 >> 3] & (1 << (0 & 7))) == 0) BoolNull = null;
                    else BoolNull = ((_nullMap_[0 >> 3] & (2 << (0 & 7))) != 0);
                    {
                        Byte = (*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongFlagEnumNullSubArray)) return;
                    if ((_nullMap_[17 >> 3] & (1 << (17 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.shortEnum _value_ = ShortEnumNull.HasValue ? ShortEnumNull.Value : default(fastCSharp.testCase.data.shortEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ShortEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[18 >> 3] & (1 << (18 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumNullArray)) return;
                    if ((_nullMap_[19 >> 3] & (1 << (19 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortEnumNullSubArray)) return;
                    if ((_nullMap_[20 >> 3] & (1 << (20 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumNullList)) return;
                    if ((_nullMap_[21 >> 3] & (1 << (21 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumList)) return;
                    if ((_nullMap_[22 >> 3] & (1 << (22 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumArray2)) return;
                    if ((_nullMap_[23 >> 3] & (1 << (23 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortEnumArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteNullSubArray)) return;
                    if ((_nullMap_[24 >> 3] & (1 << (24 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteSubArray)) return;
                    if ((_nullMap_[25 >> 3] & (1 << (25 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumNullList)) return;
                    if ((_nullMap_[26 >> 3] & (1 << (26 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortArray)) return;
                    if ((_nullMap_[27 >> 3] & (1 << (27 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumNullArray2)) return;
                    if ((_nullMap_[28 >> 3] & (1 << (28 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortEnumSubArray)) return;
                    if ((_nullMap_[29 >> 3] & (1 << (29 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumNullArray)) return;
                    if ((_nullMap_[30 >> 3] & (1 << (30 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumArray)) return;
                    if ((_nullMap_[31 >> 3] & (1 << (31 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumArray2)) return;
                    if ((_nullMap_[32 >> 3] & (1 << (32 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortNullArray)) return;
                    if ((_nullMap_[33 >> 3] & (1 << (33 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumList)) return;
                    if ((_nullMap_[34 >> 3] & (1 << (34 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortNullArray2)) return;
                    if ((_nullMap_[35 >> 3] & (1 << (35 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortNullList)) return;
                    if ((_nullMap_[36 >> 3] & (1 << (36 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref String)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortNullSubArray)) return;
                    if ((_nullMap_[37 >> 3] & (1 << (37 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortFlagEnumSubArray)) return;
                    if ((_nullMap_[38 >> 3] & (1 << (38 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumList)) return;
                    if ((_nullMap_[39 >> 3] & (1 << (39 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uShortFlagEnum _value_ = UShortFlagEnumNull.HasValue ? UShortFlagEnumNull.Value : default(fastCSharp.testCase.data.uShortFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) UShortFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[40 >> 3] & (1 << (40 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumArray2)) return;
                    if ((_nullMap_[41 >> 3] & (1 << (41 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.shortFlagEnum _value_ = ShortFlagEnumNull.HasValue ? ShortFlagEnumNull.Value : default(fastCSharp.testCase.data.shortFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ShortFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[42 >> 3] & (1 << (42 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ShortFlagEnumNullSubArray)) return;
                    if ((_nullMap_[43 >> 3] & (1 << (43 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumNullList)) return;
                    if ((_nullMap_[44 >> 3] & (1 << (44 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ShortFlagEnumNullArray2)) return;
                    if ((_nullMap_[45 >> 3] & (1 << (45 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteNullArray2)) return;
                    if ((_nullMap_[46 >> 3] & (1 << (46 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortFlagEnumNullSubArray)) return;
                    if ((_nullMap_[47 >> 3] & (1 << (47 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortNullSubArray)) return;
                    if ((_nullMap_[48 >> 3] & (1 << (48 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteArray)) return;
                    if ((_nullMap_[49 >> 3] & (1 << (49 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteArray2)) return;
                    if ((_nullMap_[50 >> 3] & (1 << (50 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortNullArray2)) return;
                    if ((_nullMap_[51 >> 3] & (1 << (51 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumArray2)) return;
                    if ((_nullMap_[52 >> 3] & (1 << (52 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumArray)) return;
                    if ((_nullMap_[53 >> 3] & (1 << (53 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassList3)) return;
                    if ((_nullMap_[54 >> 3] & (1 << (54 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassList2)) return;
                    if ((_nullMap_[55 >> 3] & (1 << (55 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassList)) return;
                    if ((_nullMap_[56 >> 3] & (1 << (56 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClass)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref MemberClassSubArray2)) return;
                    if ((_nullMap_[57 >> 3] & (1 << (57 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClass2)) return;
                    if ((_nullMap_[58 >> 3] & (1 << (58 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClass3)) return;
                    if ((_nullMap_[59 >> 3] & (1 << (59 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassArray)) return;
                    if ((_nullMap_[60 >> 3] & (1 << (60 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassArray3)) return;
                    if ((_nullMap_[61 >> 3] & (1 << (61 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref NoMemberClassArray2)) return;
                    if ((_nullMap_[62 >> 3] & (1 << (62 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumList)) return;
                    if ((_nullMap_[63 >> 3] & (1 << (63 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.sByteEnum _value_ = SByteEnumNull.HasValue ? SByteEnumNull.Value : default(fastCSharp.testCase.data.sByteEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) SByteEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[64 >> 3] & (1 << (64 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumNullArray)) return;
                    if ((_nullMap_[65 >> 3] & (1 << (65 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumNullArray2)) return;
                    if ((_nullMap_[66 >> 3] & (1 << (66 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortList)) return;
                    if ((_nullMap_[67 >> 3] & (1 << (67 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortFlagEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteFlagEnumNullSubArray)) return;
                    if ((_nullMap_[68 >> 3] & (1 << (68 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteFlagEnumSubArray)) return;
                    if ((_nullMap_[69 >> 3] & (1 << (69 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumNullArray)) return;
                    if ((_nullMap_[70 >> 3] & (1 << (70 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.sByteFlagEnum _value_ = SByteFlagEnumNull.HasValue ? SByteFlagEnumNull.Value : default(fastCSharp.testCase.data.sByteFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) SByteFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[71 >> 3] & (1 << (71 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumList)) return;
                    if ((_nullMap_[72 >> 3] & (1 << (72 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumNullList)) return;
                    if ((_nullMap_[73 >> 3] & (1 << (73 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteEnumNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SByteEnumSubArray)) return;
                    if ((_nullMap_[74 >> 3] & (1 << (74 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortNullArray)) return;
                    if ((_nullMap_[75 >> 3] & (1 << (75 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumArray2)) return;
                    if ((_nullMap_[76 >> 3] & (1 << (76 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SByteFlagEnumArray)) return;
                    if ((_nullMap_[77 >> 3] & (1 << (77 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref String2)) return;
                    if ((_nullMap_[78 >> 3] & (1 << (78 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref StringArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongNullSubArray)) return;
                    if ((_nullMap_[79 >> 3] & (1 << (79 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongNullList)) return;
                    if ((_nullMap_[80 >> 3] & (1 << (80 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongArray2)) return;
                    if ((_nullMap_[81 >> 3] & (1 << (81 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongArray)) return;
                    if ((_nullMap_[82 >> 3] & (1 << (82 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntNullList)) return;
                    if ((_nullMap_[83 >> 3] & (1 << (83 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntNullArray2)) return;
                    if ((_nullMap_[84 >> 3] & (1 << (84 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntNullArray)) return;
                    if ((_nullMap_[85 >> 3] & (1 << (85 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumNullList)) return;
                    if ((_nullMap_[86 >> 3] & (1 << (86 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntFlagEnumNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntFlagEnumSubArray)) return;
                    if ((_nullMap_[87 >> 3] & (1 << (87 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortArray2)) return;
                    if ((_nullMap_[88 >> 3] & (1 << (88 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortArray)) return;
                    if ((_nullMap_[89 >> 3] & (1 << (89 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntList)) return;
                    if ((_nullMap_[90 >> 3] & (1 << (90 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongNullArray2)) return;
                    if ((_nullMap_[91 >> 3] & (1 << (91 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumArray)) return;
                    if ((_nullMap_[92 >> 3] & (1 << (92 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumArray2)) return;
                    if ((_nullMap_[93 >> 3] & (1 << (93 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumArray2)) return;
                    if ((_nullMap_[94 >> 3] & (1 << (94 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumArray)) return;
                    if ((_nullMap_[95 >> 3] & (1 << (95 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumList)) return;
                    if ((_nullMap_[96 >> 3] & (1 << (96 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uLongFlagEnum _value_ = ULongFlagEnumNull.HasValue ? ULongFlagEnumNull.Value : default(fastCSharp.testCase.data.uLongFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ULongFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[97 >> 3] & (1 << (97 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumNullArray)) return;
                    if ((_nullMap_[98 >> 3] & (1 << (98 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumNullList)) return;
                    if ((_nullMap_[99 >> 3] & (1 << (99 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongFlagEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongFlagEnumSubArray)) return;
                    if ((_nullMap_[100 >> 3] & (1 << (100 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongEnumSubArray)) return;
                    if ((_nullMap_[101 >> 3] & (1 << (101 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumList)) return;
                    if ((_nullMap_[102 >> 3] & (1 << (102 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongNullArray)) return;
                    if ((_nullMap_[103 >> 3] & (1 << (103 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uLongEnum _value_ = ULongEnumNull.HasValue ? ULongEnumNull.Value : default(fastCSharp.testCase.data.uLongEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ULongEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[104 >> 3] & (1 << (104 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumNullArray)) return;
                    if ((_nullMap_[105 >> 3] & (1 << (105 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ULongEnumNullSubArray)) return;
                    if ((_nullMap_[106 >> 3] & (1 << (106 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ULongEnumNullList)) return;
                    if ((_nullMap_[107 >> 3] & (1 << (107 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumNullArray)) return;
                    if ((_nullMap_[108 >> 3] & (1 << (108 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uIntFlagEnum _value_ = UIntFlagEnumNull.HasValue ? UIntFlagEnumNull.Value : default(fastCSharp.testCase.data.uIntFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) UIntFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[109 >> 3] & (1 << (109 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumList)) return;
                    if ((_nullMap_[110 >> 3] & (1 << (110 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SubStringArray2)) return;
                    if ((_nullMap_[111 >> 3] & (1 << (111 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SubStringArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortEnumNullSubArray)) return;
                    if ((_nullMap_[112 >> 3] & (1 << (112 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SubStringList)) return;
                    if ((_nullMap_[113 >> 3] & (1 << (113 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref SubStringList2)) return;
                    if ((_nullMap_[114 >> 3] & (1 << (114 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SubStringSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SubString2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SubString)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref StringSubArray2)) return;
                    if ((_nullMap_[115 >> 3] & (1 << (115 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref StringDictionary)) return;
                    if ((_nullMap_[116 >> 3] & (1 << (116 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref StringArray2)) return;
                    if ((_nullMap_[117 >> 3] & (1 << (117 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortFlagEnumArray)) return;
                    if ((_nullMap_[118 >> 3] & (1 << (118 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref StringList)) return;
                    if ((_nullMap_[119 >> 3] & (1 << (119 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref StringList2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref StringSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref SubStringSubArray2)) return;
                    if ((_nullMap_[120 >> 3] & (1 << (120 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumNullArray2)) return;
                    if ((_nullMap_[121 >> 3] & (1 << (121 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntEnumNullSubArray)) return;
                    if ((_nullMap_[122 >> 3] & (1 << (122 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumNullList)) return;
                    if ((_nullMap_[123 >> 3] & (1 << (123 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UIntEnumSubArray)) return;
                    if ((_nullMap_[124 >> 3] & (1 << (124 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumArray)) return;
                    if ((_nullMap_[125 >> 3] & (1 << (125 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntFlagEnumArray2)) return;
                    if ((_nullMap_[126 >> 3] & (1 << (126 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumArray)) return;
                    if ((_nullMap_[127 >> 3] & (1 << (127 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumNullArray2)) return;
                    if ((_nullMap_[128 >> 3] & (1 << (128 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumNullArray)) return;
                    if ((_nullMap_[129 >> 3] & (1 << (129 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uIntEnum _value_ = UIntEnumNull.HasValue ? UIntEnumNull.Value : default(fastCSharp.testCase.data.uIntEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) UIntEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[130 >> 3] & (1 << (130 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumNullArray)) return;
                    if ((_nullMap_[131 >> 3] & (1 << (131 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntArray2)) return;
                    if ((_nullMap_[132 >> 3] & (1 << (132 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.uShortEnum _value_ = UShortEnumNull.HasValue ? UShortEnumNull.Value : default(fastCSharp.testCase.data.uShortEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) UShortEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[133 >> 3] & (1 << (133 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumArray)) return;
                    if ((_nullMap_[134 >> 3] & (1 << (134 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumArray2)) return;
                    if ((_nullMap_[135 >> 3] & (1 << (135 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UShortEnumList)) return;
                    if ((_nullMap_[136 >> 3] & (1 << (136 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref UIntEnumList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref MemberClassSubArray)) return;
                    if ((_nullMap_[137 >> 3] & (1 << (137 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClassList2)) return;
                    if ((_nullMap_[138 >> 3] & (1 << (138 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref CharSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref CharNullSubArray)) return;
                    if ((_nullMap_[139 >> 3] & (1 << (139 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeArray2)) return;
                    if ((_nullMap_[140 >> 3] & (1 << (140 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeList)) return;
                    if ((_nullMap_[141 >> 3] & (1 << (141 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DateTimeNullSubArray)) return;
                    if ((_nullMap_[142 >> 3] & (1 << (142 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeNullList)) return;
                    if ((_nullMap_[143 >> 3] & (1 << (143 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DateTimeNullArray2)) return;
                    if ((_nullMap_[144 >> 3] & (1 << (144 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharNullList)) return;
                    if ((_nullMap_[145 >> 3] & (1 << (145 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharNullArray2)) return;
                    if ((_nullMap_[146 >> 3] & (1 << (146 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteNullList)) return;
                    if ((_nullMap_[147 >> 3] & (1 << (147 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteNullArray2)) return;
                    if ((_nullMap_[148 >> 3] & (1 << (148 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteSubArray)) return;
                    if ((_nullMap_[149 >> 3] & (1 << (149 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharArray)) return;
                    if ((_nullMap_[150 >> 3] & (1 << (150 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharNullArray)) return;
                    if ((_nullMap_[151 >> 3] & (1 << (151 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharList)) return;
                    if ((_nullMap_[152 >> 3] & (1 << (152 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref CharArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DateTimeSubArray)) return;
                    if ((_nullMap_[153 >> 3] & (1 << (153 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DoubleNullSubArray)) return;
                    if ((_nullMap_[154 >> 3] & (1 << (154 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleNullList)) return;
                    if ((_nullMap_[155 >> 3] & (1 << (155 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DoubleSubArray)) return;
                    if ((_nullMap_[156 >> 3] & (1 << (156 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatArray)) return;
                    if ((_nullMap_[157 >> 3] & (1 << (157 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatArray2)) return;
                    if ((_nullMap_[158 >> 3] & (1 << (158 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatNullArray2)) return;
                    if ((_nullMap_[159 >> 3] & (1 << (159 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatNullArray)) return;
                    if ((_nullMap_[160 >> 3] & (1 << (160 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatList)) return;
                    if ((_nullMap_[161 >> 3] & (1 << (161 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleNullArray)) return;
                    if ((_nullMap_[162 >> 3] & (1 << (162 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleList)) return;
                    if ((_nullMap_[163 >> 3] & (1 << (163 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalNullArray)) return;
                    if ((_nullMap_[164 >> 3] & (1 << (164 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalList)) return;
                    if ((_nullMap_[165 >> 3] & (1 << (165 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalArray2)) return;
                    if ((_nullMap_[166 >> 3] & (1 << (166 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalNullArray2)) return;
                    if ((_nullMap_[167 >> 3] & (1 << (167 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DecimalNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DecimalNullSubArray)) return;
                    if ((_nullMap_[168 >> 3] & (1 << (168 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleArray2)) return;
                    if ((_nullMap_[169 >> 3] & (1 << (169 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref DoubleArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref DecimalSubArray)) return;
                    if ((_nullMap_[170 >> 3] & (1 << (170 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteFlagEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteFlagEnumNullSubArray)) return;
                    if ((_nullMap_[171 >> 3] & (1 << (171 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxClassList2)) return;
                    if ((_nullMap_[172 >> 3] & (1 << (172 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxClassList)) return;
                    if ((_nullMap_[173 >> 3] & (1 << (173 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxClassArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxClassSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxClassSubArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxStruct)) return;
                    if ((_nullMap_[174 >> 3] & (1 << (174 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxStructArray2)) return;
                    if ((_nullMap_[175 >> 3] & (1 << (175 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxStructArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxStruct2)) return;
                    if ((_nullMap_[176 >> 3] & (1 << (176 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxClassArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxClass2)) return;
                    if ((_nullMap_[177 >> 3] & (1 << (177 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolList)) return;
                    if ((_nullMap_[178 >> 3] & (1 << (178 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolArray2)) return;
                    if ((_nullMap_[179 >> 3] & (1 << (179 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolArray)) return;
                    if ((_nullMap_[180 >> 3] & (1 << (180 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolNullArray)) return;
                    if ((_nullMap_[181 >> 3] & (1 << (181 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolNullArray2)) return;
                    if ((_nullMap_[182 >> 3] & (1 << (182 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoolNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxClass)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoolSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoolNullSubArray)) return;
                    if ((_nullMap_[183 >> 3] & (1 << (183 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxStructList)) return;
                    if ((_nullMap_[184 >> 3] & (1 << (184 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref BoxStructList2)) return;
                    if ((_nullMap_[185 >> 3] & (1 << (185 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref ByteEnumNullSubArray)) return;
                    if ((_nullMap_[186 >> 3] & (1 << (186 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumArray2)) return;
                    if ((_nullMap_[187 >> 3] & (1 << (187 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumList)) return;
                    if ((_nullMap_[188 >> 3] & (1 << (188 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.byteFlagEnum _value_ = ByteFlagEnumNull.HasValue ? ByteFlagEnumNull.Value : default(fastCSharp.testCase.data.byteFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ByteFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[189 >> 3] & (1 << (189 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumNullList)) return;
                    if ((_nullMap_[190 >> 3] & (1 << (190 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumNullArray2)) return;
                    if ((_nullMap_[191 >> 3] & (1 << (191 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteFlagEnumNullArray)) return;
                    if ((_nullMap_[192 >> 3] & (1 << (192 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumNullList)) return;
                    if ((_nullMap_[193 >> 3] & (1 << (193 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumNullArray2)) return;
                    if ((_nullMap_[194 >> 3] & (1 << (194 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxStructSubArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref BoxStructSubArray)) return;
                    if ((_nullMap_[195 >> 3] & (1 << (195 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteArray2)) return;
                    if ((_nullMap_[196 >> 3] & (1 << (196 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumArray)) return;
                    if ((_nullMap_[197 >> 3] & (1 << (197 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumArray2)) return;
                    if ((_nullMap_[198 >> 3] & (1 << (198 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumNullArray)) return;
                    if ((_nullMap_[199 >> 3] & (1 << (199 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.byteEnum _value_ = ByteEnumNull.HasValue ? ByteEnumNull.Value : default(fastCSharp.testCase.data.byteEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ByteEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[200 >> 3] & (1 << (200 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref ByteEnumList)) return;
                    if ((_nullMap_[201 >> 3] & (1 << (201 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref FloatNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref FloatNullSubArray)) return;
                    if ((_nullMap_[202 >> 3] & (1 << (202 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumArray)) return;
                    if ((_nullMap_[203 >> 3] & (1 << (203 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongArray2)) return;
                    if ((_nullMap_[204 >> 3] & (1 << (204 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongArray)) return;
                    if ((_nullMap_[205 >> 3] & (1 << (205 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumArray2)) return;
                    if ((_nullMap_[206 >> 3] & (1 << (206 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumList)) return;
                    if ((_nullMap_[207 >> 3] & (1 << (207 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.longEnum _value_ = LongEnumNull.HasValue ? LongEnumNull.Value : default(fastCSharp.testCase.data.longEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) LongEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[208 >> 3] & (1 << (208 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumNullList)) return;
                    if ((_nullMap_[209 >> 3] & (1 << (209 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumNullArray2)) return;
                    if ((_nullMap_[210 >> 3] & (1 << (210 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongEnumNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValueSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValuePairSubArray)) return;
                    if ((_nullMap_[211 >> 3] & (1 << (211 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairArray2)) return;
                    if ((_nullMap_[212 >> 3] & (1 << (212 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValuePair)) return;
                    if ((_nullMap_[213 >> 3] & (1 << (213 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairList)) return;
                    if ((_nullMap_[214 >> 3] & (1 << (214 & 7))) == 0)
                    {
                        System.Collections.Generic.KeyValuePair<string,int?> _value_ = KeyValuePairNull.HasValue ? KeyValuePairNull.Value : default(System.Collections.Generic.KeyValuePair<string,int?>);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) KeyValuePairNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[215 >> 3] & (1 << (215 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValuePairNullSubArray)) return;
                    if ((_nullMap_[216 >> 3] & (1 << (216 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairNullList)) return;
                    if ((_nullMap_[217 >> 3] & (1 << (217 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValuePairNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongEnumNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongNullSubArray)) return;
                    if ((_nullMap_[218 >> 3] & (1 << (218 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongNullList)) return;
                    if ((_nullMap_[219 >> 3] & (1 << (219 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongSubArray)) return;
                    if ((_nullMap_[220 >> 3] & (1 << (220 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClass)) return;
                    if ((_nullMap_[221 >> 3] & (1 << (221 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClass2)) return;
                    if ((_nullMap_[222 >> 3] & (1 << (222 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClassList)) return;
                    if ((_nullMap_[223 >> 3] & (1 << (223 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClassArray2)) return;
                    if ((_nullMap_[224 >> 3] & (1 << (224 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref MemberClassArray)) return;
                    if ((_nullMap_[225 >> 3] & (1 << (225 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongNullArray)) return;
                    if ((_nullMap_[226 >> 3] & (1 << (226 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongList)) return;
                    if ((_nullMap_[227 >> 3] & (1 << (227 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumList)) return;
                    if ((_nullMap_[228 >> 3] & (1 << (228 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumArray2)) return;
                    if ((_nullMap_[229 >> 3] & (1 << (229 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumArray)) return;
                    if ((_nullMap_[230 >> 3] & (1 << (230 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.longFlagEnum _value_ = LongFlagEnumNull.HasValue ? LongFlagEnumNull.Value : default(fastCSharp.testCase.data.longFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) LongFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[231 >> 3] & (1 << (231 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumNullArray)) return;
                    if ((_nullMap_[232 >> 3] & (1 << (232 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongFlagEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref LongFlagEnumNullSubArray)) return;
                    if ((_nullMap_[233 >> 3] & (1 << (233 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref LongFlagEnumNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValueNullSubArray)) return;
                    if ((_nullMap_[234 >> 3] & (1 << (234 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueNullList)) return;
                    if ((_nullMap_[235 >> 3] & (1 << (235 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueNullArray2)) return;
                    if ((_nullMap_[236 >> 3] & (1 << (236 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumArray2)) return;
                    if ((_nullMap_[237 >> 3] & (1 << (237 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumArray)) return;
                    if ((_nullMap_[238 >> 3] & (1 << (238 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntDictionary)) return;
                    if ((_nullMap_[239 >> 3] & (1 << (239 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumList)) return;
                    if ((_nullMap_[240 >> 3] & (1 << (240 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.intEnum _value_ = IntEnumNull.HasValue ? IntEnumNull.Value : default(fastCSharp.testCase.data.intEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) IntEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[241 >> 3] & (1 << (241 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumNullArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntEnumNullSubArray)) return;
                    if ((_nullMap_[242 >> 3] & (1 << (242 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumNullList)) return;
                    if ((_nullMap_[243 >> 3] & (1 << (243 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntEnumNullArray2)) return;
                    if ((_nullMap_[244 >> 3] & (1 << (244 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntArray2)) return;
                    if ((_nullMap_[245 >> 3] & (1 << (245 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntArray)) return;
                    if ((_nullMap_[246 >> 3] & (1 << (246 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidArray2)) return;
                    if ((_nullMap_[247 >> 3] & (1 << (247 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref FloatSubArray)) return;
                    if ((_nullMap_[248 >> 3] & (1 << (248 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidList)) return;
                    if ((_nullMap_[249 >> 3] & (1 << (249 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidNullArray)) return;
                    if ((_nullMap_[250 >> 3] & (1 << (250 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidNullArray2)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref GuidSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref GuidNullSubArray)) return;
                    if ((_nullMap_[251 >> 3] & (1 << (251 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref GuidNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntEnumSubArray)) return;
                    if ((_nullMap_[252 >> 3] & (1 << (252 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntNullSubArray)) return;
                    if ((_nullMap_[253 >> 3] & (1 << (253 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntNullList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref KeyValue)) return;
                    if ((_nullMap_[254 >> 3] & (1 << (254 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueArray)) return;
                    if ((_nullMap_[255 >> 3] & (1 << (255 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueArray2)) return;
                    if ((_nullMap_[256 >> 3] & (1 << (256 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueNullArray)) return;
                    if ((_nullMap_[257 >> 3] & (1 << (257 & 7))) == 0)
                    {
                        fastCSharp.keyValue<int?,string> _value_ = KeyValueNull.HasValue ? KeyValueNull.Value : default(fastCSharp.keyValue<int?,string>);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) KeyValueNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[258 >> 3] & (1 << (258 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref KeyValueList)) return;
                    if ((_nullMap_[259 >> 3] & (1 << (259 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntNullArray2)) return;
                    if ((_nullMap_[260 >> 3] & (1 << (260 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntNullArray)) return;
                    if ((_nullMap_[261 >> 3] & (1 << (261 & 7))) == 0)
                    {
                        fastCSharp.testCase.data.intFlagEnum _value_ = IntFlagEnumNull.HasValue ? IntFlagEnumNull.Value : default(fastCSharp.testCase.data.intFlagEnum);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) IntFlagEnumNull = _value_;
                        else return;
                    }
                    if ((_nullMap_[262 >> 3] & (1 << (262 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumList)) return;
                    if ((_nullMap_[263 >> 3] & (1 << (263 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumArray2)) return;
                    if ((_nullMap_[264 >> 3] & (1 << (264 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumNullArray)) return;
                    if ((_nullMap_[265 >> 3] & (1 << (265 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumNullArray2)) return;
                    if ((_nullMap_[266 >> 3] & (1 << (266 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntFlagEnumNullList)) return;
                    if ((_nullMap_[267 >> 3] & (1 << (267 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref IntList)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntFlagEnumSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref IntFlagEnumNullSubArray)) return;
                    if (!_deSerializer_.MemberStructDeSerialize(ref UShortSubArray)) return;
                }
                else _deSerializer_.Error(fastCSharp.emit.dataDeSerializer.deSerializeState.MemberMap);
            }
        }
    }
}namespace fastCSharp.testCase
{
    internal partial class memoryDatabase
    {
        public partial class primaryKey3
        {
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>
            {
                /// <summary>
                /// 关键字
                /// </summary>
                public int Key1;
                /// <summary>
                /// 关键字
                /// </summary>
                public string Key2;
                /// <summary>
                /// 关键字
                /// </summary>
                public System.Guid Key3;
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="other">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    return Key1/**/.Equals(other.Key1) && Key2/**/.Equals(other.Key2) && Key3/**/.Equals(other.Key3);
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return Key1.GetHashCode() ^ Key2/**/.GetHashCode() ^ Key3/**/.GetHashCode();
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }
            }
        }
    }
}namespace fastCSharp.testCase
{
        internal partial class addTcpCall
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static int _M0(int a, int b)
                {

                    
                    return fastCSharp.testCase.addTcpCall/**/.add(a, b);
                }
            }
        }
}namespace fastCSharp.testCase
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 跨类型静态调用测试[IsServer = true]表示主配置
            /// </summary>
            public partial class addTcpCall
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<int> add(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0> _wait_ = fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0>.Get();
                    if (_wait_ != null)
                    {
                        
                        add(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private static void add(int a, int b, Action<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.testCase.tcpClient/**/.TcpCallService/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i0 _inputParameter_ = new fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i0
                            {
                                a = a,
                                b = b,
                            };
                            
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }
        }
}namespace fastCSharp.testCase
{
        internal partial class subTcpCall
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static int _M1(int a, int b)
                {

                    
                    return fastCSharp.testCase.subTcpCall/**/.sub(a, b);
                }
            }
        }
}namespace fastCSharp.testCase
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 跨类型静态调用测试，绑定配置[TcpCallService]
            /// </summary>
            public partial class subTcpCall
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<int> sub(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1> _wait_ = fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1>.Get();
                    if (_wait_ != null)
                    {
                        
                        sub(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private static void sub(int a, int b, Action<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.testCase.tcpClient/**/.TcpCallService/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i1 _inputParameter_ = new fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i1
                            {
                                a = a,
                                b = b,
                            };
                            
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }
        }
}namespace fastCSharp.testCase
{
        internal partial class xorTcpCall
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static int _M2(int a, int b)
                {

                    
                    return fastCSharp.testCase.xorTcpCall/**/.xor(a, b);
                }
            }
        }
}namespace fastCSharp.testCase
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 跨类型静态调用测试，绑定配置[TcpCallService]
            /// </summary>
            public partial class xorTcpCall
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<int> xor(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2> _wait_ = fastCSharp.net.waitCall<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2>.Get();
                    if (_wait_ != null)
                    {
                        
                        xor(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private static void xor(int a, int b, Action<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.testCase.tcpClient/**/.TcpCallService/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i2 _inputParameter_ = new fastCSharp.testCase.tcpServer/**/.TcpCallService/**/._i2
                            {
                                a = a,
                                b = b,
                            };
                            
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }
        }
}
namespace fastCSharp.testCase.tcpServer
{

        /// <summary>
        /// TCP调用服务端
        /// </summary>
        public partial class TcpCallService : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            /// <param name="verify">TCP验证实例</param>
            public TcpCallService(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("TcpCallService", typeof(fastCSharp.testCase.addTcpCall)), verify)
            {
                setCommands(3);
                identityOnCommands[0 + 128].Set(0 + 128);
                identityOnCommands[1 + 128].Set(1 + 128);
                identityOnCommands[2 + 128].Set(2 + 128);
            }
            /// <summary>
            /// 命令处理
            /// </summary>
            /// <param name="index"></param>
            /// <param name="socket"></param>
            /// <param name="data"></param>
            protected override void doCommand(int index, socket socket, ref subArray<byte> data)
            {
                if (index < 128) base.doCommand(index, socket, ref data);
                else
                {
                    switch (index - 128)
                    {
                        case 0: _M0(socket, ref data); return;
                        case 1: _M1(socket, ref data); return;
                        case 2: _M2(socket, ref data); return;
                        default: return;
                    }
                }
            }
            /// <summary>
            /// 忽略分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            protected override void ignoreGroup(int groupId)
            {
            }

            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i0
            {
                public int a;
                public int b;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public int Ret;
                public int Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (int)value; }
                }
#endif
            }
            sealed class _s0 : fastCSharp.net.tcp.commandServer.socketCall<_s0, _i0>
            {
                private void get(ref fastCSharp.net.returnValue<_o0> value)
                {
                    try
                    {
                        
                        int Return;


                        
                        Return = fastCSharp.testCase.addTcpCall/**/.tcpServer/**/._M0(inputParameter.a, inputParameter.b);

                        value.Value = new _o0
                        {
                            Return = Return
                        };
                        value.Type = fastCSharp.net.returnValue.type.Success;
                    }
                    catch (Exception error)
                    {
                        value.Type = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                }
                public override void Call()
                {
                    fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                    if (isVerify == 0)
                    {
                        get(ref value);
                        socket.SendStream(ref identity, ref value, flags);
                    }
                    fastCSharp.typePool<_s0>.PushNotNull(this);
                }
            }
            private void _M0(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    try
                    {
                        _i0 inputParameter = new _i0();
                        if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                        {

                            fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, ref inputParameter));
                            return;
                        }
                        returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                    }
                    catch (Exception error)
                    {
                        returnType = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                socket.SendStream(socket.Identity, returnType);
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i1
            {
                public int a;
                public int b;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public int Ret;
                public int Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (int)value; }
                }
#endif
            }
            sealed class _s1 : fastCSharp.net.tcp.commandServer.socketCall<_s1, _i1>
            {
                private void get(ref fastCSharp.net.returnValue<_o1> value)
                {
                    try
                    {
                        
                        int Return;


                        
                        Return = fastCSharp.testCase.subTcpCall/**/.tcpServer/**/._M1(inputParameter.a, inputParameter.b);

                        value.Value = new _o1
                        {
                            Return = Return
                        };
                        value.Type = fastCSharp.net.returnValue.type.Success;
                    }
                    catch (Exception error)
                    {
                        value.Type = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                }
                public override void Call()
                {
                    fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
                    if (isVerify == 0)
                    {
                        get(ref value);
                        socket.SendStream(ref identity, ref value, flags);
                    }
                    fastCSharp.typePool<_s1>.PushNotNull(this);
                }
            }
            private void _M1(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    try
                    {
                        _i1 inputParameter = new _i1();
                        if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                        {

                            fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, ref inputParameter));
                            return;
                        }
                        returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                    }
                    catch (Exception error)
                    {
                        returnType = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                socket.SendStream(socket.Identity, returnType);
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i2
            {
                public int a;
                public int b;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public int Ret;
                public int Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (int)value; }
                }
#endif
            }
            sealed class _s2 : fastCSharp.net.tcp.commandServer.socketCall<_s2, _i2>
            {
                private void get(ref fastCSharp.net.returnValue<_o2> value)
                {
                    try
                    {
                        
                        int Return;


                        
                        Return = fastCSharp.testCase.xorTcpCall/**/.tcpServer/**/._M2(inputParameter.a, inputParameter.b);

                        value.Value = new _o2
                        {
                            Return = Return
                        };
                        value.Type = fastCSharp.net.returnValue.type.Success;
                    }
                    catch (Exception error)
                    {
                        value.Type = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                }
                public override void Call()
                {
                    fastCSharp.net.returnValue<_o2> value = new fastCSharp.net.returnValue<_o2>();
                    if (isVerify == 0)
                    {
                        get(ref value);
                        socket.SendStream(ref identity, ref value, flags);
                    }
                    fastCSharp.typePool<_s2>.PushNotNull(this);
                }
            }
            private void _M2(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    try
                    {
                        _i2 inputParameter = new _i2();
                        if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                        {

                            fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, ref inputParameter));
                            return;
                        }
                        returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                    }
                    catch (Exception error)
                    {
                        returnType = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                socket.SendStream(socket.Identity, returnType);
            }
        }
}
namespace fastCSharp.testCase.tcpClient
{

        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public class TcpCallService
        {
            /// <summary>
            /// 默认TCP调用服务器端配置信息
            /// </summary>
            protected internal static readonly fastCSharp.code.cSharp.tcpServer defaultTcpServer;
            /// <summary>
            /// 默认客户端TCP调用
            /// </summary>
            public static readonly fastCSharp.net.tcp.commandClient Default;
            static TcpCallService()
            {
                defaultTcpServer = fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("TcpCallService", typeof(fastCSharp.testCase.addTcpCall));
                if (defaultTcpServer.IsServer) fastCSharp.log.Error.Add("请确认 TcpCallService 服务器端是否本地调用", null, false);
                Default = new fastCSharp.net.tcp.commandClient(defaultTcpServer, 28);
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(TcpCallService));
            }
            /// <summary>
            /// 忽略TCP分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            /// <returns>是否调用成功</returns>
            public static fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
            {
                fastCSharp.net.tcp.commandClient client = Default;
                return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
            }
        }
}namespace fastCSharp.testCase
{
        internal partial class tcpGeneric
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpGeneric _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpGeneric)), verify)
                {
                    _value_ =new fastCSharp.testCase.tcpGeneric();
                    setCommands(1);
                    identityOnCommands[0 + 128].Set(0 + 128);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            default: return;
                        }
                    }
                }
                static tcpServer()
                {
                    System.Collections.Generic.Dictionary<fastCSharp.code.cSharp.tcpBase.genericMethod, System.Reflection.MethodInfo> genericMethods = fastCSharp.code.cSharp.tcpServer.GetGenericMethods(typeof(fastCSharp.testCase.tcpGeneric));
                    _g0 = genericMethods[new fastCSharp.code.cSharp.tcpBase.genericMethod("get", 1)];
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i0
                {
                    public fastCSharp.code.remoteType[] _GenericParameterTypes_;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<object>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public object Ret;
                    public object Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (object)value; }
                    }
#endif
                }
                private static readonly System.Reflection.MethodInfo _g0;
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpGeneric, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            object[] invokeParameter = new object[] { };
                            
                            object Return;


                            
                            Return =  (object)fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(serverValue, _g0, inputParameter._GenericParameterTypes_, invokeParameter);

                            value.Value = new _o0
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpGeneric)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<valueType> get<valueType>()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get<valueType>(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return (valueType)_outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<valueType>{ Type = _returnType_ };
                }


                private void get<valueType>(Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                _GenericParameterTypes_ = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0, typeof(valueType)),
                            };
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        internal partial class tcpGeneric<valueType>
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpGeneric<valueType> _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpGeneric<valueType>)), verify)
                {
                    _value_ =new fastCSharp.testCase.tcpGeneric<valueType>();
                    setCommands(2);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128, 0);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i0
                {
                    public valueType value;
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpGeneric<valueType>, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.set(inputParameter.value);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<valueType>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public valueType Ret;
                    public valueType Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (valueType)value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpGeneric<valueType>>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            valueType Return;


                            
                            Return = serverValue.get();

                            value.Value = new _o1
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpGeneric<valueType>)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue set(valueType value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.set(value, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void set(valueType value, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    set(value, _onReturn_, null, true);
                }
                private void set(valueType value, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                value = value,
                            };
                            _socket_.Call<tcpServer._i0>(_onReturn_, _callback_, _c0, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<valueType> get()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<valueType>{ Type = _returnType_ };
                }


                private void get(Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        internal partial class tcpHttp
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpHttp _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpHttp)), verify)
                {
                    _value_ =new fastCSharp.testCase.tcpHttp();
                    setCommands(13);
                    identityOnCommands[0 + 128].Set(0 + 128, 0);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128, 0);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128);
                    identityOnCommands[6 + 128].Set(6 + 128);
                    identityOnCommands[7 + 128].Set(7 + 128);
                    identityOnCommands[8 + 128].Set(8 + 128);
                    identityOnCommands[9 + 128].Set(9 + 128);
                    identityOnCommands[10 + 128].Set(10 + 128);
                    identityOnCommands[11 + 128].Set(11 + 128, 0);
                    identityOnCommands[12 + 128].Set(12 + 128, 4194304);
                    int httpCount = 13;
                    string[] httpNames = new string[httpCount];
                    httpCommand[] httpCommands = new httpCommand[httpCount];
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "Inc");
                    httpCommands[httpCount].Set(0 + 128, false, 0);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "Set");
                    httpCommands[httpCount].Set(1 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "Add");
                    httpCommands[httpCount].Set(2 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "inc");
                    httpCommands[httpCount].Set(3 + 128, false, 0);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "inc1");
                    httpCommands[httpCount].Set(4 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "add");
                    httpCommands[httpCount].Set(5 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "inc2");
                    httpCommands[httpCount].Set(6 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "inc3");
                    httpCommands[httpCount].Set(7 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "add1");
                    httpCommands[httpCount].Set(8 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "setCookie");
                    httpCommands[httpCount].Set(9 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "setSession");
                    httpCommands[httpCount].Set(10 + 128, false);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "getSession");
                    httpCommands[httpCount].Set(11 + 128, false, 0);
                    fastCSharp.String.getBytes(httpNames[--httpCount] = "httpUpload");
                    httpCommands[httpCount].Set(12 + 128, true, 4194304);
                    maxCommandLength = 28;
                    this.httpCommands = new fastCSharp.stateSearcher.ascii<httpCommand>(httpNames, httpCommands);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            case 2: _M2(socket, ref data); return;
                            case 3: _M3(socket, ref data); return;
                            case 4: _M4(socket, ref data); return;
                            case 5: _M5(socket, ref data); return;
                            case 6: _M6(socket, ref data); return;
                            case 7: _M7(socket, ref data); return;
                            case 8: _M8(socket, ref data); return;
                            case 9: _M9(socket, ref data); return;
                            case 10: _M10(socket, ref data); return;
                            case 11: _M11(socket, ref data); return;
                            case 12: _M12(socket, ref data); return;
                            default: return;
                        }
                    }
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                protected override void doHttpCommand(int index, fastCSharp.net.tcp.http.socketBase socket)
                {
                    switch (index - 128)
                    {
                        case 0: _M0(socket); return;
                        case 1: _M1(socket); return;
                        case 2: _M2(socket); return;
                        case 3: _M3(socket); return;
                        case 4: _M4(socket); return;
                        case 5: _M5(socket); return;
                        case 6: _M6(socket); return;
                        case 7: _M7(socket); return;
                        case 8: _M8(socket); return;
                        case 9: _M9(socket); return;
                        case 10: _M10(socket); return;
                        case 11: _M11(socket); return;
                        case 12: _M12(socket); return;
                        default: return;
                    }
                }

                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpHttp>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Inc();

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                struct _m0
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public fastCSharp.net.returnValue Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            ServerValue.Inc();
                            
                            return new fastCSharp.net.returnValue{ Type = fastCSharp.net.returnValue.type.Success };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue{ Type = returnType };
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M0(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            {
                                httpPage.Response(new _m0 { Socket = commandSocket, ServerValue = _value_ }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
                internal struct _i1
                {
                    public int a;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpHttp, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Set(inputParameter.a);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                struct _m1
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i1 InputParameter;
                    public fastCSharp.net.returnValue Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            ServerValue.Set(InputParameter.a);
                            
                            return new fastCSharp.net.returnValue{ Type = fastCSharp.net.returnValue.type.Success };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue{ Type = returnType };
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M1(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i1 inputParameter = new _i1();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m1 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i2
                {
                    public int a;
                    public int b;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.testCase.tcpHttp, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Add(inputParameter.a, inputParameter.b);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                struct _m2
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i2 InputParameter;
                    public fastCSharp.net.returnValue Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            ServerValue.Add(InputParameter.a, InputParameter.b);
                            
                            return new fastCSharp.net.returnValue{ Type = fastCSharp.net.returnValue.type.Success };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue{ Type = returnType };
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M2(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i2 inputParameter = new _i2();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m2 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.testCase.tcpHttp>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc();

                            value.Value = new _o3
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s3>.PushNotNull(this);
                    }
                }
                struct _m3
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public fastCSharp.net.returnValue<_o3> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.inc();
                            
                            return new _o3
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o3>{ Type = returnType };
                    }
                }
                private void _M3(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M3(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            {
                                httpPage.Response(new _m3 { Socket = commandSocket, ServerValue = _value_ }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
                internal struct _i4
                {
                    public int a;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.testCase.tcpHttp, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(inputParameter.a);

                            value.Value = new _o4
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                struct _m4
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i4 InputParameter;
                    public fastCSharp.net.returnValue<_o4> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.inc(InputParameter.a);
                            
                            return new _o4
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o4>{ Type = returnType };
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M4(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i4 inputParameter = new _i4();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m4 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i5
                {
                    public int a;
                    public int b;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.testCase.tcpHttp, _i5>
                {
                    private void get(ref fastCSharp.net.returnValue<_o5> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.add(inputParameter.a, inputParameter.b);

                            value.Value = new _o5
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o5> value = new fastCSharp.net.returnValue<_o5>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                struct _m5
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i5 InputParameter;
                    public fastCSharp.net.returnValue<_o5> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.add(InputParameter.a, InputParameter.b);
                            
                            return new _o5
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o5>{ Type = returnType };
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s5/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M5(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i5 inputParameter = new _i5();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m5 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i6
                {
                    public int outValue;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int outValue;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.testCase.tcpHttp, _i6>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(out inputParameter.outValue);

                            value.Value = new _o6
                            {
                                outValue = inputParameter.outValue,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o6> value = new fastCSharp.net.returnValue<_o6>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s6>.PushNotNull(this);
                    }
                }
                struct _m6
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i6 InputParameter;
                    public fastCSharp.net.returnValue<_o6> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.inc(out InputParameter.outValue);
                            
                            return new _o6
                            {
                                outValue = InputParameter.outValue,
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o6>{ Type = returnType };
                    }
                }
                private void _M6(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i6 inputParameter = new _i6();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s6/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M6(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i6 inputParameter = new _i6();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m6 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i7
                {
                    public int a;
                    public int outValue;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int outValue;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s7 : fastCSharp.net.tcp.commandServer.serverCall<_s7, fastCSharp.testCase.tcpHttp, _i7>
                {
                    private void get(ref fastCSharp.net.returnValue<_o7> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(inputParameter.a, out inputParameter.outValue);

                            value.Value = new _o7
                            {
                                outValue = inputParameter.outValue,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o7> value = new fastCSharp.net.returnValue<_o7>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s7>.PushNotNull(this);
                    }
                }
                struct _m7
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i7 InputParameter;
                    public fastCSharp.net.returnValue<_o7> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.inc(InputParameter.a, out InputParameter.outValue);
                            
                            return new _o7
                            {
                                outValue = InputParameter.outValue,
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o7>{ Type = returnType };
                    }
                }
                private void _M7(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i7 inputParameter = new _i7();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s7/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M7(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i7 inputParameter = new _i7();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m7 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i8
                {
                    public int a;
                    public int b;
                    public int outValue;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int outValue;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s8 : fastCSharp.net.tcp.commandServer.serverCall<_s8, fastCSharp.testCase.tcpHttp, _i8>
                {
                    private void get(ref fastCSharp.net.returnValue<_o8> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.add(inputParameter.a, inputParameter.b, out inputParameter.outValue);

                            value.Value = new _o8
                            {
                                outValue = inputParameter.outValue,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o8> value = new fastCSharp.net.returnValue<_o8>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s8>.PushNotNull(this);
                    }
                }
                struct _m8
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i8 InputParameter;
                    public fastCSharp.net.returnValue<_o8> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            int Return = 
                            
                            Return = ServerValue.add(InputParameter.a, InputParameter.b, out InputParameter.outValue);
                            
                            return new _o8
                            {
                                outValue = InputParameter.outValue,
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o8>{ Type = returnType };
                    }
                }
                private void _M8(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i8 inputParameter = new _i8();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s8/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M8(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i8 inputParameter = new _i8();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m8 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i9
                {
                    public string name;
                    public string value;
                }
                sealed class _s9 : fastCSharp.net.tcp.commandServer.serverCall<_s9, fastCSharp.testCase.tcpHttp, _i9>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.setCookie(socket, inputParameter.name, inputParameter.value);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s9>.PushNotNull(this);
                    }
                }
                struct _m9
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i9 InputParameter;
                    public fastCSharp.net.returnValue Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            ServerValue.setCookie(Socket, InputParameter.name, InputParameter.value);
                            
                            return new fastCSharp.net.returnValue{ Type = fastCSharp.net.returnValue.type.Success };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue{ Type = returnType };
                    }
                }
                private void _M9(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i9 inputParameter = new _i9();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s9/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M9(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i9 inputParameter = new _i9();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m9 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
                internal struct _i10
                {
                    public string value;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o10 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o10 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s10 : fastCSharp.net.tcp.commandServer.serverCall<_s10, fastCSharp.testCase.tcpHttp, _i10>
                {
                    private void get(ref fastCSharp.net.returnValue<_o10> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.setSession(socket, inputParameter.value);

                            value.Value = new _o10
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o10> value = new fastCSharp.net.returnValue<_o10>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s10>.PushNotNull(this);
                    }
                }
                struct _m10
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public _i10 InputParameter;
                    public fastCSharp.net.returnValue<_o10> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            bool Return = 
                            
                            Return = ServerValue.setSession(Socket, InputParameter.value);
                            
                            return new _o10
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o10>{ Type = returnType };
                    }
                }
                private void _M10(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i10 inputParameter = new _i10();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s10/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M10(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            _i10 inputParameter = new _i10();
                            if (httpPage.DeSerialize(ref inputParameter))
                            {
                                httpPage.Response(new _m10 { Socket = commandSocket, ServerValue = _value_, InputParameter = inputParameter }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o11 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o11 : fastCSharp.net.asynchronousMethod.IReturnParameter<string>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public string Ret;
                    public string Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (string)value; }
                    }
#endif
                }
                sealed class _s11 : fastCSharp.net.tcp.commandServer.serverCall<_s11, fastCSharp.testCase.tcpHttp>
                {
                    private void get(ref fastCSharp.net.returnValue<_o11> value)
                    {
                        try
                        {
                            
                            string Return;


                            
                            Return = serverValue.getSession(socket);

                            value.Value = new _o11
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o11> value = new fastCSharp.net.returnValue<_o11>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s11>.PushNotNull(this);
                    }
                }
                struct _m11
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public fastCSharp.net.returnValue<_o11> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            string Return = 
                            
                            Return = ServerValue.getSession(Socket);
                            
                            return new _o11
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o11>{ Type = returnType };
                    }
                }
                private void _M11(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s11/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M11(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            {
                                httpPage.Response(new _m11 { Socket = commandSocket, ServerValue = _value_ }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o12 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o12 : fastCSharp.net.asynchronousMethod.IReturnParameter<long>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public long Ret;
                    public long Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (long)value; }
                    }
#endif
                }
                sealed class _s12 : fastCSharp.net.tcp.commandServer.serverCall<_s12, fastCSharp.testCase.tcpHttp>
                {
                    private void get(ref fastCSharp.net.returnValue<_o12> value)
                    {
                        try
                        {
                            
                            long Return;


                            
                            Return = serverValue.httpUpload(socket);

                            value.Value = new _o12
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o12> value = new fastCSharp.net.returnValue<_o12>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s12>.PushNotNull(this);
                    }
                }
                struct _m12
                {
                    public fastCSharp.testCase.tcpHttp ServerValue;
                    public socket Socket;
                    public fastCSharp.net.returnValue<_o12> Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            
                            long Return = 
                            
                            Return = ServerValue.httpUpload(Socket);
                            
                            return new _o12
                            {
                                Return = Return
                            };
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue<_o12>{ Type = returnType };
                    }
                }
                private void _M12(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s12/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                private void _M12(fastCSharp.net.tcp.http.socketBase socket)
                {
                    long identity = int.MinValue;
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            {
                                httpPage.Response(new _m12 { Socket = commandSocket, ServerValue = _value_ }.Get());
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpHttp)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 无参数无返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue Inc()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Inc(null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 无参数无返回值调用测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Inc(Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Inc(_onReturn_, null, true);
                }
                private void Inc(Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Call(_onReturn_, _callback_, _c0, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 单参数无返回值调用测试+输入参数包装处理测试
                /// </summary>
                public fastCSharp.net.returnValue Set(int a)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Set(a, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 单参数无返回值调用测试+输入参数包装处理测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(int a, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Set(a, _onReturn_, null, true);
                }
                private void Set(int a, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                a = a,
                            };
                            _socket_.Call<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 多参数无返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue Add(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Add(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 多参数无返回值调用测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Add(int a, int b, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Add(a, b, _onReturn_, null, true);
                }
                private void Add(int a, int b, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.Call<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 无参数有返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 单参数有返回值调用测试+输入参数包装处理测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(int a)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(a, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(int a, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                a = a,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 多参数有返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue<int> add(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o5> _wait_ = fastCSharp.net.waitCall<tcpServer._o5>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.add(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o5> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void add(int a, int b, Action<fastCSharp.net.returnValue<tcpServer._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o5> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c5, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(out int outValue)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o6> _wait_ = fastCSharp.net.waitCall<tcpServer._o6>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(out outValue, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            outValue = _outputParameter_.Value.outValue;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    outValue = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(out int outValue, Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    outValue = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i6 _inputParameter_ = new tcpServer._i6
                            {
                            };
                            _socket_.Get(_onReturn_, _callback_, _c6, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c7 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 7 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 混合输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(int a, out int outValue)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o7> _wait_ = fastCSharp.net.waitCall<tcpServer._o7>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(a, out outValue, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o7> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            outValue = _outputParameter_.Value.outValue;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    outValue = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(int a, out int outValue, Action<fastCSharp.net.returnValue<tcpServer._o7>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o7>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o7> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o7>();
                    outValue = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i7 _inputParameter_ = new tcpServer._i7
                            {
                                a = a,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c7, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c8 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 8 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 混合输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> add(int a, int b, out int outValue)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o8> _wait_ = fastCSharp.net.waitCall<tcpServer._o8>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.add(a, b, out outValue, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o8> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            outValue = _outputParameter_.Value.outValue;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    outValue = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void add(int a, int b, out int outValue, Action<fastCSharp.net.returnValue<tcpServer._o8>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o8>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o8> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o8>();
                    outValue = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i8 _inputParameter_ = new tcpServer._i8
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c8, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c9 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 9 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写HTTP客户端Cookie测试
                /// </summary>
                public fastCSharp.net.returnValue setCookie(string name, string value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.setCookie(name, value, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 写HTTP客户端Cookie测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void setCookie(string name, string value, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    setCookie(name, value, _onReturn_, null, true);
                }
                private void setCookie(string name, string value, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i9 _inputParameter_ = new tcpServer._i9
                            {
                                name = name,
                                value = value,
                            };
                            _socket_.Call<tcpServer._i9>(_onReturn_, _callback_, _c9, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c10 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 10 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写HTTP客户端Session测试
                /// </summary>
                public fastCSharp.net.returnValue<bool> setSession(string value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o10> _wait_ = fastCSharp.net.waitCall<tcpServer._o10>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.setSession(value, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o10> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void setSession(string value, Action<fastCSharp.net.returnValue<tcpServer._o10>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o10>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o10> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o10>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i10 _inputParameter_ = new tcpServer._i10
                            {
                                value = value,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c10, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c11 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 11 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 读取HTTP客户端Session测试
                /// </summary>
                public fastCSharp.net.returnValue<string> getSession()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o11> _wait_ = fastCSharp.net.waitCall<tcpServer._o11>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.getSession(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o11> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<string>{ Type = _returnType_ };
                }


                private void getSession(Action<fastCSharp.net.returnValue<tcpServer._o11>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o11>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o11> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o11>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c11, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c12 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 12 + 128, MaxInputSize = 4194304, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// HTTP客户端文件上传测试
                /// </summary>
                public fastCSharp.net.returnValue<long> httpUpload()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o12> _wait_ = fastCSharp.net.waitCall<tcpServer._o12>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.httpUpload(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o12> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<long>{ Type = _returnType_ };
                }


                private void httpUpload(Action<fastCSharp.net.returnValue<tcpServer._o12>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o12>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o12> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o12>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c12, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        internal partial class tcpJson
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpJson _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpJson)), verify)
                {
                    _value_ =new fastCSharp.testCase.tcpJson();
                    setCommands(9);
                    identityOnCommands[0 + 128].Set(0 + 128, 0);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128, 0);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128);
                    identityOnCommands[6 + 128].Set(6 + 128);
                    identityOnCommands[7 + 128].Set(7 + 128);
                    identityOnCommands[8 + 128].Set(8 + 128);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            case 2: _M2(socket, ref data); return;
                            case 3: _M3(socket, ref data); return;
                            case 4: _M4(socket, ref data); return;
                            case 5: _M5(socket, ref data); return;
                            case 6: _M6(socket, ref data); return;
                            case 7: _M7(socket, ref data); return;
                            case 8: _M8(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpJson>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Inc();

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i1
                {
                    public int a;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpJson, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Set(inputParameter.a);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i2
                {
                    public int a;
                    public int b;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.testCase.tcpJson, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Add(inputParameter.a, inputParameter.b);

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.testCase.tcpJson>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc();

                            value.Value = new _o3
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s3>.PushNotNull(this);
                    }
                }
                private void _M3(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i4
                {
                    public int a;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.testCase.tcpJson, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(inputParameter.a);

                            value.Value = new _o4
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i5
                {
                    public int a;
                    public int b;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.testCase.tcpJson, _i5>
                {
                    private void get(ref fastCSharp.net.returnValue<_o5> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.add(inputParameter.a, inputParameter.b);

                            value.Value = new _o5
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o5> value = new fastCSharp.net.returnValue<_o5>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s5/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i6
                {
                    public int a;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int a;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.testCase.tcpJson, _i6>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(out inputParameter.a);

                            value.Value = new _o6
                            {
                                a = inputParameter.a,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o6> value = new fastCSharp.net.returnValue<_o6>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s6>.PushNotNull(this);
                    }
                }
                private void _M6(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i6 inputParameter = new _i6();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s6/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i7
                {
                    public int a;
                    public int b;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int b;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s7 : fastCSharp.net.tcp.commandServer.serverCall<_s7, fastCSharp.testCase.tcpJson, _i7>
                {
                    private void get(ref fastCSharp.net.returnValue<_o7> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.inc(inputParameter.a, out inputParameter.b);

                            value.Value = new _o7
                            {
                                b = inputParameter.b,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o7> value = new fastCSharp.net.returnValue<_o7>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s7>.PushNotNull(this);
                    }
                }
                private void _M7(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i7 inputParameter = new _i7();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s7/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i8
                {
                    public int a;
                    public int b;
                    public int c;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    public int c;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s8 : fastCSharp.net.tcp.commandServer.serverCall<_s8, fastCSharp.testCase.tcpJson, _i8>
                {
                    private void get(ref fastCSharp.net.returnValue<_o8> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.add(inputParameter.a, inputParameter.b, out inputParameter.c);

                            value.Value = new _o8
                            {
                                c = inputParameter.c,
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o8> value = new fastCSharp.net.returnValue<_o8>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s8>.PushNotNull(this);
                    }
                }
                private void _M8(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i8 inputParameter = new _i8();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s8/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpJson)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 无参数无返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue Inc()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Inc(null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 无参数无返回值调用测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Inc(Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Inc(_onReturn_, null, true);
                }
                private void Inc(Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Call(_onReturn_, _callback_, _c0, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 单参数无返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue Set(int a)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Set(a, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 单参数无返回值调用测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(int a, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Set(a, _onReturn_, null, true);
                }
                private void Set(int a, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                a = a,
                            };
                            _socket_.CallJson<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 多参数无返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue Add(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Add(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 多参数无返回值调用测试
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Add(int a, int b, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Add(a, b, _onReturn_, null, true);
                }
                private void Add(int a, int b, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.CallJson<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 无参数有返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.GetJson(_onReturn_, _callback_, _c3, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 单参数有返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(int a)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(a, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(int a, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                a = a,
                            };
                            _socket_.GetJson(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 多参数有返回值调用测试
                /// </summary>
                public fastCSharp.net.returnValue<int> add(int a, int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o5> _wait_ = fastCSharp.net.waitCall<tcpServer._o5>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.add(a, b, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o5> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void add(int a, int b, Action<fastCSharp.net.returnValue<tcpServer._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o5> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.GetJson(_onReturn_, _callback_, _c5, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(out int a)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o6> _wait_ = fastCSharp.net.waitCall<tcpServer._o6>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(out a, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            a = _outputParameter_.Value.a;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    a = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(out int a, Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    a = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i6 _inputParameter_ = new tcpServer._i6
                            {
                            };
                            _socket_.GetJson(_onReturn_, _callback_, _c6, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c7 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 7 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 混合输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> inc(int a, out int b)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o7> _wait_ = fastCSharp.net.waitCall<tcpServer._o7>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.inc(a, out b, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o7> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            b = _outputParameter_.Value.b;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    b = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void inc(int a, out int b, Action<fastCSharp.net.returnValue<tcpServer._o7>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o7>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o7> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o7>();
                    b = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i7 _inputParameter_ = new tcpServer._i7
                            {
                                a = a,
                            };
                            _socket_.GetJson(_onReturn_, _callback_, _c7, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c8 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 8 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 混合输出参数测试
                /// </summary>
                public fastCSharp.net.returnValue<int> add(int a, int b, out int c)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o8> _wait_ = fastCSharp.net.waitCall<tcpServer._o8>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.add(a, b, out c, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o8> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            c = _outputParameter_.Value.c;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    c = default(int);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void add(int a, int b, out int c, Action<fastCSharp.net.returnValue<tcpServer._o8>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o8>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o8> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o8>();
                    c = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i8 _inputParameter_ = new tcpServer._i8
                            {
                                a = a,
                                b = b,
                            };
                            _socket_.GetJson(_onReturn_, _callback_, _c8, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        public partial class tcpMember
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpMember _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null, fastCSharp.testCase.tcpMember value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpMember)), verify)
                {
                    _value_ = value ?? new fastCSharp.testCase.tcpMember();
                    setCommands(8);
                    identityOnCommands[0 + 128].Set(0 + 128, 0);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128, 0);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128);
                    identityOnCommands[6 + 128].Set(6 + 128, 0);
                    identityOnCommands[7 + 128].Set(7 + 128);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            case 2: _M2(socket, ref data); return;
                            case 3: _M3(socket, ref data); return;
                            case 4: _M4(socket, ref data); return;
                            case 5: _M5(socket, ref data); return;
                            case 6: _M6(socket, ref data); return;
                            case 7: _M7(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpMember>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            int Return;

                            Return = serverValue.field;


                            value.Value = new _o0
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i1
                {
                    public int value;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpMember, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            

                            serverValue.field = inputParameter.value;


                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.testCase.tcpMember>
                {
                    private void get(ref fastCSharp.net.returnValue<_o2> value)
                    {
                        try
                        {
                            
                            int Return;

                            Return = serverValue.getProperty;


                            value.Value = new _o2
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o2> value = new fastCSharp.net.returnValue<_o2>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i3
                {
                    public int index;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.testCase.tcpMember, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            int Return;

                            Return = serverValue[inputParameter.index];


                            value.Value = new _o3
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s3>.PushNotNull(this);
                    }
                }
                private void _M3(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i3 inputParameter = new _i3();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i4
                {
                    public int left;
                    public int right;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.testCase.tcpMember, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            int Return;

                            Return = serverValue[inputParameter.left, inputParameter.right];


                            value.Value = new _o4
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i5
                {
                    public int left;
                    public int right;
                    public int value;
                }
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.testCase.tcpMember, _i5>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            

                            serverValue[inputParameter.left, inputParameter.right] = inputParameter.value;


                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s5/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.testCase.tcpMember>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            int Return;

                            Return = serverValue.property;


                            value.Value = new _o6
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o6> value = new fastCSharp.net.returnValue<_o6>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s6>.PushNotNull(this);
                    }
                }
                private void _M6(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s6/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i7
                {
                    public int value;
                }
                sealed class _s7 : fastCSharp.net.tcp.commandServer.serverCall<_s7, fastCSharp.testCase.tcpMember, _i7>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            

                            serverValue.property = inputParameter.value;


                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s7>.PushNotNull(this);
                    }
                }
                private void _M7(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i7 inputParameter = new _i7();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s7/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpMember)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };


                /// <summary>
                /// 测试字段
                /// </summary>
                public fastCSharp.net.returnValue<int> field
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get_field(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                        return new fastCSharp.net.returnValue<int> { Type = _returnType_ };
                    }
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.set_field(value, null, _wait_, false);
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;
                    }
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }
                }

                private void get_field(Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };



                /// <summary>
                /// 测试字段
                /// </summary>
                /// <param name="_onReturn_">测试字段</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void set_field(int value, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    set_field(value, _onReturn_, null, true);
                }
                private void set_field(int value, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                value = value,
                            };
                            _socket_.Call<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };


                /// <summary>
                /// 只读属性[不支持不可读属性]
                /// </summary>
                public fastCSharp.net.returnValue<int> getProperty
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o2> _wait_ = fastCSharp.net.waitCall<tcpServer._o2>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get_getProperty(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                        return new fastCSharp.net.returnValue<int> { Type = _returnType_ };
                    }
                }

                private void get_getProperty(Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };


                public fastCSharp.net.returnValue<int> this[int index]
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get_Item(index, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                        return new fastCSharp.net.returnValue<int> { Type = _returnType_ };
                    }
                }

                private void get_Item(int index, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                index = index,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };


                public fastCSharp.net.returnValue<int> this[int left, int right]
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get_Item(left, right, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                        return new fastCSharp.net.returnValue<int> { Type = _returnType_ };
                    }
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.set_Item(left, right, value, null, _wait_, false);
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;
                    }
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }
                }

                private void get_Item(int left, int right, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                left = left,
                                right = right,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };



                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void set_Item(int left, int right, int value, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    set_Item(left, right, value, _onReturn_, null, true);
                }
                private void set_Item(int left, int right, int value, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                left = left,
                                right = right,
                                value = value,
                            };
                            _socket_.Call<tcpServer._i5>(_onReturn_, _callback_, _c5, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };


                /// <summary>
                /// 测试属性
                /// </summary>
                public fastCSharp.net.returnValue<int> property
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o6> _wait_ = fastCSharp.net.waitCall<tcpServer._o6>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get_property(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                        return new fastCSharp.net.returnValue<int> { Type = _returnType_ };
                    }
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.set_property(value, null, _wait_, false);
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;
                    }
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }
                }

                private void get_property(Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c6, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c7 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 7 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };



                /// <summary>
                /// 测试属性
                /// </summary>
                /// <param name="_onReturn_">测试属性</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void set_property(int value, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    set_property(value, _onReturn_, null, true);
                }
                private void set_property(int value, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i7 _inputParameter_ = new tcpServer._i7
                            {
                                value = value,
                            };
                            _socket_.Call<tcpServer._i7>(_onReturn_, _callback_, _c7, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        internal partial class tcpSession
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpSession _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpSession)))
                {
                    _value_ =new fastCSharp.testCase.tcpSession();
                    setCommands(2);
                    identityOnCommands[verifyCommandIdentity = 0 + 128].Set(0 + 128, 1024);
                    identityOnCommands[1 + 128].Set(1 + 128, 0);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i0
                {
                    public string user;
                    public string password;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpSession, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.login(socket, inputParameter.user, inputParameter.password);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o0
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<string>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public string Ret;
                    public string Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (string)value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpSession>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            string Return;


                            
                            Return = serverValue.myName(socket);

                            value.Value = new _o1
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_ ));
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpSession)), 28, verifyMethod, this);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端写客户端标识测试+服务器端验证函数测试
                /// </summary>
                public fastCSharp.net.returnValue<bool> login(string user, string password)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.login(user, password, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void login(string user, string password, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                user = user,
                                password = password,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端读客户端标识测试
                /// </summary>
                public fastCSharp.net.returnValue<string> myName()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.myName(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<string>{ Type = _returnType_ };
                }


                private void myName(Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}namespace fastCSharp.testCase
{
        internal partial class tcpStream
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.testCase.tcpStream _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpStream)), verify)
                {
                    _value_ =new fastCSharp.testCase.tcpStream();
                    setCommands(5);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            case 1: _M1(socket, ref data); return;
                            case 2: _M2(socket, ref data); return;
                            case 3: _M3(socket, ref data); return;
                            case 4: _M4(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i0
                {
                    public fastCSharp.code.cSharp.tcpBase.tcpStream stream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.testCase.tcpStream, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.send(socket.GetTcpStream(ref inputParameter.stream));

                            value.Value = new _o0
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i1
                {
                    public fastCSharp.code.cSharp.tcpBase.tcpStream stream;
                    public int value;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.testCase.tcpStream, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.send(socket.GetTcpStream(ref inputParameter.stream), inputParameter.value);

                            value.Value = new _o1
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i2
                {
                    public string fileName;
                    public fastCSharp.code.cSharp.tcpBase.tcpStream stream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.testCase.tcpStream, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue<_o2> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.readFile(inputParameter.fileName, socket.GetTcpStream(ref inputParameter.stream));

                            value.Value = new _o2
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o2> value = new fastCSharp.net.returnValue<_o2>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i3
                {
                    public string fileName;
                    public fastCSharp.code.cSharp.tcpBase.tcpStream stream;
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.testCase.tcpStream, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.writeFile(inputParameter.fileName, socket.GetTcpStream(ref inputParameter.stream));

                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s3>.PushNotNull(this);
                    }
                }
                private void _M3(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i3 inputParameter = new _i3();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i4
                {
                    public string fileName;
                    public fastCSharp.code.cSharp.tcpBase.tcpStream stream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.testCase.tcpStream, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.partFile(inputParameter.fileName, socket.GetTcpStream(ref inputParameter.stream));

                            value.Value = new _o4
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.testCase.tcpStream)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端读取客户端Stream测试
                /// </summary>
                /// <param name="stream">客户端流</param>
                public fastCSharp.net.returnValue<bool> send(System.IO.Stream stream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.send(stream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void send(System.IO.Stream stream, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                stream = TcpCommandClient.GetTcpStream(stream),
                            };
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端读取客户端Stream测试(混合参数模式)
                /// </summary>
                /// <param name="stream">客户端流</param>
                public fastCSharp.net.returnValue<bool> send(System.IO.Stream stream, int value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.send(stream, value, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void send(System.IO.Stream stream, int value, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                stream = TcpCommandClient.GetTcpStream(stream),
                                value = value,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端读取客户端FileStream测试
                /// </summary>
                /// <param name="fileName">文件名称</param>
                /// <param name="stream">客户端文件流</param>
                public fastCSharp.net.returnValue<bool> readFile(string fileName, System.IO.Stream stream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o2> _wait_ = fastCSharp.net.waitCall<tcpServer._o2>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.readFile(fileName, stream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void readFile(string fileName, System.IO.Stream stream, Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                fileName = fileName,
                                stream = TcpCommandClient.GetTcpStream(stream),
                            };
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 服务器端写客户端FileStream测试
                /// </summary>
                /// <param name="fileName">文件名称</param>
                /// <param name="stream">客户端文件流</param>
                public fastCSharp.net.returnValue writeFile(string fileName, System.IO.Stream stream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.writeFile(fileName, stream, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 服务器端写客户端FileStream测试
                /// </summary>
                /// <param name="fileName">文件名称</param>
                /// <param name="stream">客户端文件流</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void writeFile(string fileName, System.IO.Stream stream, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    writeFile(fileName, stream, _onReturn_, null, true);
                }
                private void writeFile(string fileName, System.IO.Stream stream, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                fileName = fileName,
                                stream = TcpCommandClient.GetTcpStream(stream),
                            };
                            _socket_.Call<tcpServer._i3>(_onReturn_, _callback_, _c3, ref _inputParameter_, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 断点续传测试
                /// </summary>
                /// <param name="fileName">文件名称</param>
                /// <param name="stream">客户端文件流</param>
                public fastCSharp.net.returnValue<bool> partFile(string fileName, System.IO.Stream stream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.partFile(fileName, stream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void partFile(string fileName, System.IO.Stream stream, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                fileName = fileName,
                                stream = TcpCommandClient.GetTcpStream(stream),
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}
#endif