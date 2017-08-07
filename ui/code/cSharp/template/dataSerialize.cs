using System;

namespace fastCSharp.code.cSharp.template
{
    class dataSerialize : pub
    {
        #region PART CLASS
        /*NOTE*/
        public partial class /*NOTE*/@TypeNameDefinition : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            #region NOTE
            static int MemberCountVerify = 0;
            static int FixedSize = 0;
            static int NullMapSize = 0;
            static int @SerializeNullMapIndex = 0;
            type.FullName MemberName = null;
            #endregion NOTE

            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="_serializer_">对象序列化器</param>
            public unsafe void Serialize(fastCSharp.emit.dataSerializer _serializer_)
            {
                #region IF type.IsNull
                if (_serializer_.CheckPoint(this))
                #endregion IF type.IsNull
                {
                    #region IF Attribute.IsMemberMap
                    if (_serializer_.SerializeMemberMap<@type.FullName>() == null)
                    #endregion IF Attribute.IsMemberMap
                    {
                        #region IF NullMapFixedSize
                        fastCSharp.unmanagedStream _stream_ = _serializer_.Stream;
                        _stream_.PrepLength(sizeof(int) + @NullMapSize + @FixedSize);
                        byte* _write_ = _stream_.CurrentData;
                        *(int*)_write_ = @MemberCountVerify;
                        _write_ += sizeof(int);
                        #region IF NullMapSize
                        byte* _nullMap_ = _write_;
                        fastCSharp.unsafer.memory.Clear32(_write_, @NullMapSize);
                        _write_ += @NullMapSize;
                        #endregion IF NullMapSize
                        #region LOOP Members
                        #region NAME FixedSerialize
                        #region IF MemberType.IsBool
                        #region IF MemberType.IsNull
                        if (/*NOTE*/((bool?)(object)/*NOTE*/@MemberName/*NOTE*/)/*NOTE*/.HasValue)
                        {
                            if ((bool)/*NOTE*/(object)/*NOTE*/@MemberName) _nullMap_[@SerializeNullMapIndex >> 3] |= (byte)(3 << (@SerializeNullMapIndex & 7));
                            else _nullMap_[@SerializeNullMapIndex >> 3] |= (byte)(1 << (@SerializeNullMapIndex & 7));
                        }
                        #endregion IF MemberType.IsNull
                        #region NOT MemberType.IsNull
                        if (/*NOTE*/(bool)(object)/*NOTE*/@MemberName) _nullMap_[@SerializeNullMapIndex >> 3] |= (byte)(1 << (@SerializeNullMapIndex & 7));
                        #endregion NOT MemberType.IsNull
                        #endregion IF MemberType.IsBool
                        #region NOT MemberType.IsBool
                        #region IF MemberType.IsNull
                        #region IF MemberType.NullType
                        if (!/*NOTE*/((int?)(object)/*NOTE*/@MemberName/*NOTE*/)/*NOTE*/.HasValue) _nullMap_[@SerializeNullMapIndex >> 3] |= (byte)(1 << (@SerializeNullMapIndex & 7));
                        #endregion IF MemberType.NullType
                        #region NOT MemberType.NullType
                        if (@MemberName == null) _nullMap_[@SerializeNullMapIndex >> 3] |= (byte)(1 << (@SerializeNullMapIndex & 7));
                        #endregion NOT MemberType.NullType
                        #region IF SerializeFixedSize
                        else
                        #endregion IF SerializeFixedSize
                        #endregion IF MemberType.IsNull
                        #region IF SerializeFixedSize
                        {
                            *(@MemberType.StructNotNullType*)_write_ = (@MemberType.StructNotNullType)/*NOTE*/(object)/*NOTE*//*IF:MemberType.Type.IsEnum*/(@MemberType.EnumUnderlyingType.FullName)/*IF:MemberType.Type.IsEnum*/@MemberName;
                            _write_ += sizeof(@MemberType.StructNotNullType);
                        }
                        #endregion IF SerializeFixedSize
                        #endregion NOT MemberType.IsBool
                        #endregion NAME FixedSerialize
                        #endregion LOOP Members
                        _stream_.UnsafeAddSerializeLength((int)(_write_ - _stream_.CurrentData));
                        _stream_.PrepLength();
                        #endregion IF NullMapFixedSize
                        #region NOT NullMapFixedSize
                        _serializer_.Stream.Write(@MemberCountVerify);
                        #endregion NOT NullMapFixedSize
                        #region LOOP Members
                        #region NAME NotFixedSerialize
                        #region NOT SerializeFixedSize
                        #region IF MemberType.IsNull
                        #region IF MemberType.NullType
                        if (/*NOTE*/((int?)(object)/*NOTE*/@MemberName/*NOTE*/)/*NOTE*/.HasValue)
                        #endregion IF MemberType.NullType
                        #region NOT MemberType.NullType
                        if (@MemberName != null)
                        #endregion NOT MemberType.NullType
                        #endregion IF MemberType.IsNull
                            #region IF MemberType.Type.IsValueType
                            #region IF MemberType.NullType
                            _serializer_.MemberNullableSerialize(/*NOTE*/(int?)(object)/*NOTE*/@MemberName);
                            #endregion IF MemberType.NullType
                        #region NOT MemberType.NullType
                        _serializer_.MemberStructSerialize(/*NOTE*/(int)(object)/*NOTE*/@MemberName);
                        #endregion NOT MemberType.NullType
                            #endregion IF MemberType.Type.IsValueType
                        #region NOT MemberType.Type.IsValueType
                        _serializer_.MemberClassSerialize(@MemberName);
                        #endregion NOT MemberType.Type.IsValueType
                        #endregion NOT SerializeFixedSize
                        #endregion NAME NotFixedSerialize
                        #endregion LOOP Members
                    }
                    #region IF Attribute.IsMemberMap
                    else
                    {
                        #region IF NullMapFixedSize
                        fastCSharp.unmanagedStream _stream_ = _serializer_.Stream;
                        _stream_.PrepLength(@NullMapSize + @FixedSize);
                        byte* _nullMap_ = _stream_.CurrentData/*IF:FixedSize*/, _write_ = _nullMap_/*IF:FixedSize*/;
                        #region IF NullMapSize
                        fastCSharp.unsafer.memory.Clear32(_nullMap_, @NullMapSize);
                        #region IF FixedSize
                        _write_ += @NullMapSize;
                        #endregion IF FixedSize
                        #endregion IF NullMapSize
                        #region LOOP Members
                        if (_serializer_.IsMemberMap(@MemberIndex))
                        {
                            #region FROMNAME FixedSerialize
                            #endregion FROMNAME FixedSerialize
                        }
                        #endregion LOOP Members
                        #region IF FixedSize
                        _stream_.UnsafeAddLength(((int)(_write_ - _nullMap_) + 3) & (int.MaxValue - 3));
                        #endregion IF FixedSize
                        #region NOT FixedSize
                        _stream_.UnsafeAddLength(@NullMapSize);
                        #endregion NOT FixedSize
                        _stream_.PrepLength();
                        #endregion IF NullMapFixedSize
                        #region LOOP Members
                        if (_serializer_.IsMemberMap(@MemberIndex))
                        {
                            #region FROMNAME NotFixedSerialize
                            #endregion FROMNAME NotFixedSerialize
                        }
                        #endregion LOOP Members
                    }
                    #endregion IF Attribute.IsMemberMap
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="_deSerializer_">对象反序列化器</param>
            public unsafe void DeSerialize(fastCSharp.emit.dataDeSerializer _deSerializer_)
            {
                #region IF type.IsNull
                _deSerializer_.AddPoint(this);
                #endregion IF type.IsNull
                if (_deSerializer_.CheckMemberCount(@MemberCountVerify))
                {
                    #region IF NullMapFixedSize
                    byte* _read_ = _deSerializer_.Read;
                    #region IF NullMapSize
                    byte* _nullMap_ = _read_;
                    _read_ += @NullMapSize;
                    #endregion IF NullMapSize
                    #endregion IF NullMapFixedSize
                    #region LOOP Members
                    #region NAME FixedDeSerialize
                    #region IF MemberType.IsBool
                    #region IF MemberType.IsNull
                    if ((_nullMap_[@SerializeNullMapIndex >> 3] & (1 << (@SerializeNullMapIndex & 7))) == 0) @MemberName = null;
                    else @MemberName = /*NOTE*/(FullName)(object)/*NOTE*/((_nullMap_[@SerializeNullMapIndex >> 3] & (2 << (@SerializeNullMapIndex & 7))) != 0);
                    #endregion IF MemberType.IsNull
                    #region NOT MemberType.IsNull
                    @MemberName = /*NOTE*/(FullName)(object)/*NOTE*/((_nullMap_[@SerializeNullMapIndex >> 3] & (1 << (@SerializeNullMapIndex & 7))) != 0);
                    #endregion NOT MemberType.IsNull
                    #endregion IF MemberType.IsBool
                    #region NOT MemberType.IsBool
                    #region IF SerializeFixedSize
                    #region IF MemberType.IsNull
                    if ((_nullMap_[@SerializeNullMapIndex >> 3] & (1 << (@SerializeNullMapIndex & 7))) != 0) @MemberName = null;
                    else
                    #endregion IF MemberType.IsNull
                    {
                        @MemberName = /*IF:MemberType.Type.IsEnum*/(@MemberType.FullName)/*IF:MemberType.Type.IsEnum*//*NOTE*/(object)/*NOTE*/(*(@MemberType.StructNotNullType*)_read_);
                        _read_ += sizeof(@MemberType.StructNotNullType);
                    }
                    #endregion IF SerializeFixedSize
                    #endregion NOT MemberType.IsBool
                    #endregion NAME FixedDeSerialize
                    #endregion LOOP Members
                    #region IF NullMapFixedSize
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);
                    #endregion IF NullMapFixedSize
                    #region LOOP Members
                    #region NAME NotFixedDeSerialize
                    #region NOT SerializeFixedSize
                    #region IF MemberType.IsNull
                    if ((_nullMap_[@SerializeNullMapIndex >> 3] & (1 << (@SerializeNullMapIndex & 7))) == 0)
                    #endregion IF MemberType.IsNull
                        #region IF MemberType.Type.IsValueType
                        #region IF MemberType.NullType
                    {
                        @MemberType.NullType.FullName _value_ = /*NOTE*/((int?)(object)/*NOTE*/@MemberName/*NOTE*/)/*NOTE*/.HasValue ? /*NOTE*/(MemberType.NullType.FullName)(object)((int?)(object)/*NOTE*/@MemberName/*NOTE*/)/*NOTE*/.Value : default(@MemberType.NullType.FullName);
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) @MemberName = _value_;
                        else return;
                    }
                        #endregion IF MemberType.NullType
                    #region NOT MemberType.NullType
                    if (!_deSerializer_.MemberStructDeSerialize(ref @MemberName)) return;
                    #endregion NOT MemberType.NullType
                        #endregion IF MemberType.Type.IsValueType
                    #region NOT MemberType.Type.IsValueType
                    if (!_deSerializer_.MemberClassDeSerialize(ref @MemberName)) return;
                    #endregion NOT MemberType.Type.IsValueType
                    #endregion NOT SerializeFixedSize
                    #endregion NAME NotFixedDeSerialize
                    #endregion LOOP Members
                }
                #region IF Attribute.IsMemberMap
                else if (_deSerializer_.GetMemberMap<@type.FullName>() != null)
                {
                    #region IF NullMapSize
                    byte* _nullMap_ = _deSerializer_.Read;
                    #endregion IF NullMapSize
                    #region IF FixedSize
                    byte* _read_;
                    #region IF NullMapSize
                    _read_ = _nullMap_ + @NullMapSize;
                    #endregion IF NullMapSize
                    #region NOT NullMapSize
                    _read_ = _deSerializer_.Read;
                    #endregion NOT NullMapSize
                    #endregion IF FixedSize
                    #region LOOP Members
                    if (_deSerializer_.IsMemberMap(@MemberIndex))
                    {
                        #region FROMNAME FixedDeSerialize
                        #endregion FROMNAME FixedDeSerialize
                    }
                    #endregion LOOP Members
                    #region IF FixedSize
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);
                    #endregion IF FixedSize
                    #region LOOP Members
                    if (_deSerializer_.IsMemberMap(@MemberIndex))
                    {
                        #region FROMNAME NotFixedDeSerialize
                        #endregion FROMNAME NotFixedDeSerialize
                    }
                    #endregion LOOP Members
                }
                #endregion IF Attribute.IsMemberMap
                #region NOT Attribute.IsMemberMap
                else _deSerializer_.Error(fastCSharp.emit.dataDeSerializer.deSerializeState.MemberMap);
                #endregion NOT Attribute.IsMemberMap
            }
        }
        #endregion PART CLASS
    }
    #region NOTE
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 枚举基类类型
        /// </summary>
        public class EnumUnderlyingType : pub
        {
        }
    }
    #endregion NOTE
}
