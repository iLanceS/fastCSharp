//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.serializePerformance
{
        internal partial class codeFiledData : fastCSharp.code.cSharp.dataSerialize.ISerialize
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
                        _stream_.PrepLength(sizeof(int) + 4 + 168);
                        byte* _write_ = _stream_.CurrentData;
                        *(int*)_write_ = 1073741855;
                        _write_ += sizeof(int);
                        byte* _nullMap_ = _write_;
                        fastCSharp.unsafer.memory.Clear32(_write_, 4);
                        _write_ += 4;
                        if (!GuidNull.HasValue) _nullMap_[2 >> 3] |= (byte)(1 << (2 & 7));
                        else
                        {
                            *(System.Guid*)_write_ = (System.Guid)GuidNull;
                            _write_ += sizeof(System.Guid);
                        }
                        if (!DecimalNull.HasValue) _nullMap_[3 >> 3] |= (byte)(1 << (3 & 7));
                        else
                        {
                            *(decimal*)_write_ = (decimal)DecimalNull;
                            _write_ += sizeof(decimal);
                        }
                        {
                            *(System.Guid*)_write_ = (System.Guid)Guid;
                            _write_ += sizeof(System.Guid);
                        }
                        {
                            *(decimal*)_write_ = (decimal)Decimal;
                            _write_ += sizeof(decimal);
                        }
                        if (!LongNull.HasValue) _nullMap_[4 >> 3] |= (byte)(1 << (4 & 7));
                        else
                        {
                            *(long*)_write_ = (long)LongNull;
                            _write_ += sizeof(long);
                        }
                        {
                            *(long*)_write_ = (long)Long;
                            _write_ += sizeof(long);
                        }
                        if (!DoubleNull.HasValue) _nullMap_[5 >> 3] |= (byte)(1 << (5 & 7));
                        else
                        {
                            *(double*)_write_ = (double)DoubleNull;
                            _write_ += sizeof(double);
                        }
                        {
                            *(double*)_write_ = (double)Double;
                            _write_ += sizeof(double);
                        }
                        if (!DateTimeNull.HasValue) _nullMap_[6 >> 3] |= (byte)(1 << (6 & 7));
                        else
                        {
                            *(System.DateTime*)_write_ = (System.DateTime)DateTimeNull;
                            _write_ += sizeof(System.DateTime);
                        }
                        if (!ULongNull.HasValue) _nullMap_[7 >> 3] |= (byte)(1 << (7 & 7));
                        else
                        {
                            *(ulong*)_write_ = (ulong)ULongNull;
                            _write_ += sizeof(ulong);
                        }
                        {
                            *(ulong*)_write_ = (ulong)ULong;
                            _write_ += sizeof(ulong);
                        }
                        {
                            *(System.DateTime*)_write_ = (System.DateTime)DateTime;
                            _write_ += sizeof(System.DateTime);
                        }
                        if (!IntNull.HasValue) _nullMap_[8 >> 3] |= (byte)(1 << (8 & 7));
                        else
                        {
                            *(int*)_write_ = (int)IntNull;
                            _write_ += sizeof(int);
                        }
                        {
                            *(int*)_write_ = (int)Int;
                            _write_ += sizeof(int);
                        }
                        if (!UIntNull.HasValue) _nullMap_[9 >> 3] |= (byte)(1 << (9 & 7));
                        else
                        {
                            *(uint*)_write_ = (uint)UIntNull;
                            _write_ += sizeof(uint);
                        }
                        if (!FloatNull.HasValue) _nullMap_[10 >> 3] |= (byte)(1 << (10 & 7));
                        else
                        {
                            *(float*)_write_ = (float)FloatNull;
                            _write_ += sizeof(float);
                        }
                        {
                            *(float*)_write_ = (float)Float;
                            _write_ += sizeof(float);
                        }
                        {
                            *(uint*)_write_ = (uint)UInt;
                            _write_ += sizeof(uint);
                        }
                        {
                            *(ushort*)_write_ = (ushort)UShort;
                            _write_ += sizeof(ushort);
                        }
                        if (!ShortNull.HasValue) _nullMap_[11 >> 3] |= (byte)(1 << (11 & 7));
                        else
                        {
                            *(short*)_write_ = (short)ShortNull;
                            _write_ += sizeof(short);
                        }
                        {
                            *(short*)_write_ = (short)Short;
                            _write_ += sizeof(short);
                        }
                        if (!CharNull.HasValue) _nullMap_[12 >> 3] |= (byte)(1 << (12 & 7));
                        else
                        {
                            *(char*)_write_ = (char)CharNull;
                            _write_ += sizeof(char);
                        }
                        if (!UShortNull.HasValue) _nullMap_[13 >> 3] |= (byte)(1 << (13 & 7));
                        else
                        {
                            *(ushort*)_write_ = (ushort)UShortNull;
                            _write_ += sizeof(ushort);
                        }
                        {
                            *(char*)_write_ = (char)Char;
                            _write_ += sizeof(char);
                        }
                        if (!ByteNull.HasValue) _nullMap_[14 >> 3] |= (byte)(1 << (14 & 7));
                        else
                        {
                            *(byte*)_write_ = (byte)ByteNull;
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
                        if (!SByteNull.HasValue) _nullMap_[15 >> 3] |= (byte)(1 << (15 & 7));
                        else
                        {
                            *(sbyte*)_write_ = (sbyte)SByteNull;
                            _write_ += sizeof(sbyte);
                        }
                        {
                            *(sbyte*)_write_ = (sbyte)SByte;
                            _write_ += sizeof(sbyte);
                        }
                        if (Bool) _nullMap_[16 >> 3] |= (byte)(1 << (16 & 7));
                        if (String == null) _nullMap_[17 >> 3] |= (byte)(1 << (17 & 7));
                        _stream_.UnsafeAddSerializeLength((int)(_write_ - _stream_.CurrentData));
                        _stream_.PrepLength();
                        if (String != null)
                        _serializer_.MemberClassSerialize(String);
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
                if (_deSerializer_.CheckMemberCount(1073741855))
                {
                    byte* _read_ = _deSerializer_.Read;
                    byte* _nullMap_ = _read_;
                    _read_ += 4;
                    if ((_nullMap_[2 >> 3] & (1 << (2 & 7))) != 0) GuidNull = null;
                    else
                    {
                        GuidNull = (*(System.Guid*)_read_);
                        _read_ += sizeof(System.Guid);
                    }
                    if ((_nullMap_[3 >> 3] & (1 << (3 & 7))) != 0) DecimalNull = null;
                    else
                    {
                        DecimalNull = (*(decimal*)_read_);
                        _read_ += sizeof(decimal);
                    }
                    {
                        Guid = (*(System.Guid*)_read_);
                        _read_ += sizeof(System.Guid);
                    }
                    {
                        Decimal = (*(decimal*)_read_);
                        _read_ += sizeof(decimal);
                    }
                    if ((_nullMap_[4 >> 3] & (1 << (4 & 7))) != 0) LongNull = null;
                    else
                    {
                        LongNull = (*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    {
                        Long = (*(long*)_read_);
                        _read_ += sizeof(long);
                    }
                    if ((_nullMap_[5 >> 3] & (1 << (5 & 7))) != 0) DoubleNull = null;
                    else
                    {
                        DoubleNull = (*(double*)_read_);
                        _read_ += sizeof(double);
                    }
                    {
                        Double = (*(double*)_read_);
                        _read_ += sizeof(double);
                    }
                    if ((_nullMap_[6 >> 3] & (1 << (6 & 7))) != 0) DateTimeNull = null;
                    else
                    {
                        DateTimeNull = (*(System.DateTime*)_read_);
                        _read_ += sizeof(System.DateTime);
                    }
                    if ((_nullMap_[7 >> 3] & (1 << (7 & 7))) != 0) ULongNull = null;
                    else
                    {
                        ULongNull = (*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    {
                        ULong = (*(ulong*)_read_);
                        _read_ += sizeof(ulong);
                    }
                    {
                        DateTime = (*(System.DateTime*)_read_);
                        _read_ += sizeof(System.DateTime);
                    }
                    if ((_nullMap_[8 >> 3] & (1 << (8 & 7))) != 0) IntNull = null;
                    else
                    {
                        IntNull = (*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    {
                        Int = (*(int*)_read_);
                        _read_ += sizeof(int);
                    }
                    if ((_nullMap_[9 >> 3] & (1 << (9 & 7))) != 0) UIntNull = null;
                    else
                    {
                        UIntNull = (*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    if ((_nullMap_[10 >> 3] & (1 << (10 & 7))) != 0) FloatNull = null;
                    else
                    {
                        FloatNull = (*(float*)_read_);
                        _read_ += sizeof(float);
                    }
                    {
                        Float = (*(float*)_read_);
                        _read_ += sizeof(float);
                    }
                    {
                        UInt = (*(uint*)_read_);
                        _read_ += sizeof(uint);
                    }
                    {
                        UShort = (*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    if ((_nullMap_[11 >> 3] & (1 << (11 & 7))) != 0) ShortNull = null;
                    else
                    {
                        ShortNull = (*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    {
                        Short = (*(short*)_read_);
                        _read_ += sizeof(short);
                    }
                    if ((_nullMap_[12 >> 3] & (1 << (12 & 7))) != 0) CharNull = null;
                    else
                    {
                        CharNull = (*(char*)_read_);
                        _read_ += sizeof(char);
                    }
                    if ((_nullMap_[13 >> 3] & (1 << (13 & 7))) != 0) UShortNull = null;
                    else
                    {
                        UShortNull = (*(ushort*)_read_);
                        _read_ += sizeof(ushort);
                    }
                    {
                        Char = (*(char*)_read_);
                        _read_ += sizeof(char);
                    }
                    if ((_nullMap_[14 >> 3] & (1 << (14 & 7))) != 0) ByteNull = null;
                    else
                    {
                        ByteNull = (*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    if ((_nullMap_[0 >> 3] & (1 << (0 & 7))) == 0) BoolNull = null;
                    else BoolNull = ((_nullMap_[0 >> 3] & (2 << (0 & 7))) != 0);
                    {
                        Byte = (*(byte*)_read_);
                        _read_ += sizeof(byte);
                    }
                    if ((_nullMap_[15 >> 3] & (1 << (15 & 7))) != 0) SByteNull = null;
                    else
                    {
                        SByteNull = (*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    {
                        SByte = (*(sbyte*)_read_);
                        _read_ += sizeof(sbyte);
                    }
                    Bool = ((_nullMap_[16 >> 3] & (1 << (16 & 7))) != 0);
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);
                    if ((_nullMap_[17 >> 3] & (1 << (17 & 7))) == 0)
                    if (!_deSerializer_.MemberClassDeSerialize(ref String)) return;
                }
                else _deSerializer_.Error(fastCSharp.emit.dataDeSerializer.deSerializeState.MemberMap);
            }
        }
}
#endif