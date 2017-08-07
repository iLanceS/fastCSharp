using System;
using fastCSharp.code;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// 成员位图对象绑定
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct memberMapValue<valueType>
    {
        /// <summary>
        /// 成员位图
        /// </summary>
        public memberMap MemberMap;
        /// <summary>
        /// 目标数据
        /// </summary>
        public valueType Value;
        /// <summary>
        /// 对象转换成JSON字符串
        /// </summary>
        /// <param name="toJsoner">对象转换成JSON字符串</param>
        /// <param name="value">参数</param>
        [fastCSharp.emit.jsonSerialize.custom]
        internal static void ToJson(fastCSharp.emit.jsonSerializer toJsoner, memberMapValue<valueType> value)
        {
            if (value.Value == null) fastCSharp.web.ajax.WriteNull(toJsoner.CharStream);
            else
            {
                memberMap memberMap = value.MemberMap;
                if (memberMap == null || memberMap.IsDefault) fastCSharp.emit.jsonSerializer.typeToJsoner<valueType>.ToJson(toJsoner, value.Value);
                else
                {
                    fastCSharp.emit.jsonSerializer.config config = typePool<fastCSharp.emit.jsonSerializer.config>.Pop() ?? new fastCSharp.emit.jsonSerializer.config(), oldConfig = toJsoner.Config;
                    fastCSharp.emit.memberCopyer<fastCSharp.emit.jsonSerializer.config>.Copy(config, oldConfig);
                    (toJsoner.Config = config).MemberMap = memberMap;
                    try
                    {
                        fastCSharp.emit.jsonSerializer.typeToJsoner<valueType>.ToJson(toJsoner, value.Value);
                    }
                    finally
                    {
                        toJsoner.Config = oldConfig;
                        config.MemberMap = null;
                        typePool<fastCSharp.emit.jsonSerializer.config>.PushNotNull(config);
                    }
                }
            }
        }
        /// <summary>
        /// 对象转换成JSON字符串
        /// </summary>
        /// <param name="parser">Json解析器</param>
        /// <param name="value">参数</param>
        [fastCSharp.emit.jsonParse.custom]
        private static void parseJson(fastCSharp.emit.jsonParser parser, ref memberMapValue<valueType> value)
        {
            fastCSharp.emit.jsonParser.config config = typePool<fastCSharp.emit.jsonParser.config>.Pop() ?? new fastCSharp.emit.jsonParser.config(), oldConfig = parser.Config;
            fastCSharp.emit.memberCopyer<fastCSharp.emit.jsonParser.config>.Copy(config, oldConfig);
            if (value.MemberMap == null) value.MemberMap = fastCSharp.code.memberMap<valueType>.New();
            (parser.Config = config).MemberMap = value.MemberMap;
            try
            {
                fastCSharp.emit.jsonParser.typeParser<valueType>.Parse(parser, ref value.Value);
            }
            finally
            {
                parser.Config = oldConfig;
                config.Null();
                typePool<fastCSharp.emit.jsonParser.config>.PushNotNull(config);
            }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value">数据对象</param>
        [fastCSharp.emit.dataSerialize.custom]
        internal static void Serialize(fastCSharp.emit.dataSerializer serializer, memberMapValue<valueType> value)
        {
            if (value.Value == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            else
            {
                memberMap memberMap = value.MemberMap;
                if (memberMap == null || memberMap.IsDefault)
                {
                    fastCSharp.emit.dataSerializer.typeSerializer<valueType>.Serialize(serializer, value.Value);
                }
                else
                {
                    memberMap oldMemberMap = serializer.MemberMap, currentMemberMap = serializer.CurrentMemberMap, jsonMemberMap = serializer.JsonMemberMap;
                    serializer.MemberMap = memberMap;
                    try
                    {
                        fastCSharp.emit.dataSerializer.typeSerializer<valueType>.Serialize(serializer, value.Value);
                    }
                    finally
                    {
                        serializer.MemberMap = oldMemberMap;
                        serializer.CurrentMemberMap = currentMemberMap;
                        serializer.JsonMemberMap = jsonMemberMap;
                    }
                }
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="deSerializer">序列化数据</param>
        /// <param name="value">目标数据对象</param>
        [fastCSharp.emit.dataSerialize.custom]
        private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref memberMapValue<valueType> value)
        {
            if (deSerializer.CheckNull() == 0) value.Value = default(valueType);
            else
            {
                memberMap oldMemberMap = deSerializer.MemberMap;
                deSerializer.MemberMap = value.MemberMap;
                try
                {
                    fastCSharp.emit.dataDeSerializer.typeDeSerializer<valueType>.DeSerialize(deSerializer, ref value.Value);
                }
                finally
                {
                    value.MemberMap = deSerializer.MemberMap;
                    deSerializer.MemberMap = oldMemberMap;
                }
            }
        }
    }
    /// <summary>
    /// 成员位图对象绑定
    /// </summary>
    /// <typeparam name="memberMapType"></typeparam>
    /// <typeparam name="valueType"></typeparam>
    public struct memberMapValue<memberMapType, valueType> where valueType : class, memberMapType
    {
        /// <summary>
        /// 成员位图
        /// </summary>
        public memberMap<memberMapType> MemberMap;
        /// <summary>
        /// 目标数据
        /// </summary>
        public valueType Value;
        /// <summary>
        /// 对象转换成JSON字符串
        /// </summary>
        /// <param name="toJsoner">对象转换成JSON字符串</param>
        /// <param name="value">参数</param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, memberMapValue<memberMapType, valueType> value)
        {
            memberMapValue<memberMapType>.ToJson(toJsoner, new memberMapValue<memberMapType> { MemberMap = value.MemberMap, Value = value.Value });
        }
        /// <summary>
        /// 对象转换成JSON字符串
        /// </summary>
        /// <param name="parser">Json解析器</param>
        /// <param name="value">参数</param>
        [fastCSharp.emit.jsonParse.custom]
        private static void parseJson(fastCSharp.emit.jsonParser parser, ref memberMapValue<memberMapType, valueType> value)
        {
            fastCSharp.emit.jsonParser.config config = typePool<fastCSharp.emit.jsonParser.config>.Pop() ?? new fastCSharp.emit.jsonParser.config(), oldConfig = parser.Config;
            fastCSharp.emit.memberCopyer<fastCSharp.emit.jsonParser.config>.Copy(config, oldConfig);
            if (value.MemberMap == null) value.MemberMap = fastCSharp.code.memberMap<memberMapType>.New();
            (parser.Config = config).MemberMap = value.MemberMap;
            try
            {
                if (value.Value == null) fastCSharp.emit.jsonParser.typeParser<valueType>.Parse(parser, ref value.Value);
                else
                {
                    memberMapType parseValue = value.Value;
                    fastCSharp.emit.jsonParser.typeParser<memberMapType>.Parse(parser, ref parseValue);
                }
            }
            finally
            {
                parser.Config = oldConfig;
                config.Null();
                typePool<fastCSharp.emit.jsonParser.config>.PushNotNull(config);
            }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value">数据对象</param>
        [fastCSharp.emit.dataSerialize.custom]
        private static void serialize(fastCSharp.emit.dataSerializer serializer, memberMapValue<memberMapType, valueType> value)
        {
            memberMapValue<memberMapType>.Serialize(serializer, new memberMapValue<memberMapType> { MemberMap = value.MemberMap, Value = value.Value });
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="deSerializer">序列化数据</param>
        /// <param name="value">目标数据对象</param>
        [fastCSharp.emit.dataSerialize.custom]
        private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref memberMapValue<memberMapType, valueType> value)
        {
            if (deSerializer.CheckNull() == 0) value.Value = default(valueType);
            else
            {
                memberMap oldMemberMap = deSerializer.MemberMap;
                deSerializer.MemberMap = value.MemberMap;
                try
                {
                    if (value.Value == null) fastCSharp.emit.dataDeSerializer.typeDeSerializer<valueType>.DeSerialize(deSerializer, ref value.Value);
                    else
                    {
                        memberMapType parseValue = value.Value;
                        fastCSharp.emit.dataDeSerializer.typeDeSerializer<memberMapType>.DeSerialize(deSerializer, ref parseValue);
                    }
                }
                finally
                {
                    value.MemberMap = (memberMap<memberMapType>)deSerializer.MemberMap;
                    deSerializer.MemberMap = oldMemberMap;
                }
            }
        }
        /// <summary>
        /// 反序列化池对象
        /// </summary>
        public struct deSerializePool
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap<memberMapType> MemberMap;
            /// <summary>
            /// 目标数据
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换成JSON字符串</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, deSerializePool value)
            {
                memberMapValue<memberMapType>.ToJson(toJsoner, new memberMapValue<memberMapType> { MemberMap = value.MemberMap, Value = value.Value });
            }
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">参数</param>
            [fastCSharp.emit.jsonParse.custom]
            private static void parseJson(fastCSharp.emit.jsonParser parser, ref deSerializePool value)
            {
                fastCSharp.emit.jsonParser.config config = typePool<fastCSharp.emit.jsonParser.config>.Pop() ?? new fastCSharp.emit.jsonParser.config(), oldConfig = parser.Config;
                fastCSharp.emit.memberCopyer<fastCSharp.emit.jsonParser.config>.Copy(config, oldConfig);
                if (value.MemberMap == null) value.MemberMap = fastCSharp.code.memberMap<memberMapType>.New();
                (parser.Config = config).MemberMap = value.MemberMap;
                try
                {
                    if (value.Value == null)
                    {
                        valueType poolValue = value.Value = typePool<valueType>.Pop();
                        try
                        {
                            fastCSharp.emit.jsonParser.typeParser<valueType>.Parse(parser, ref value.Value);
                        }
                        finally
                        {
                            if (poolValue != value.Value) typePool<valueType>.PushOnly(poolValue);
                        } 
                    }
                    else
                    {
                        memberMapType parseValue = value.Value;
                        fastCSharp.emit.jsonParser.typeParser<memberMapType>.Parse(parser, ref parseValue);
                    }
                }
                finally
                {
                    parser.Config = oldConfig;
                    config.Null();
                    typePool<fastCSharp.emit.jsonParser.config>.PushNotNull(config);
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value">数据对象</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void serialize(fastCSharp.emit.dataSerializer serializer, deSerializePool value)
            {
                memberMapValue<memberMapType>.Serialize(serializer, new memberMapValue<memberMapType> { MemberMap = value.MemberMap, Value = value.Value });
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">序列化数据</param>
            /// <param name="value">目标数据对象</param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref deSerializePool value)
            {
                if (deSerializer.CheckNull() == 0) value.Value = default(valueType);
                else
                {
                    memberMap oldMemberMap = deSerializer.MemberMap;
                    deSerializer.MemberMap = value.MemberMap;
                    try
                    {
                        if (value.Value == null)
                        {
                            valueType poolValue = value.Value = typePool<valueType>.Pop();
                            try
                            {
                                fastCSharp.emit.dataDeSerializer.typeDeSerializer<valueType>.DeSerialize(deSerializer, ref value.Value);
                            }
                            finally
                            {
                                if (poolValue != value.Value) typePool<valueType>.PushOnly(poolValue);
                            }
                        }
                        else
                        {
                            memberMapType parseValue = value.Value;
                            fastCSharp.emit.dataDeSerializer.typeDeSerializer<memberMapType>.DeSerialize(deSerializer, ref parseValue);
                        }
                    }
                    finally
                    {
                        value.MemberMap = (memberMap<memberMapType>)deSerializer.MemberMap;
                        deSerializer.MemberMap = oldMemberMap;
                    }
                }
            }
        }
    }
}
