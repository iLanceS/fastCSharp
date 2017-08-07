using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// JSON序列化测试
    /// </summary>
    class json
    {
        /// <summary>
        /// 随机对象生成参数
        /// </summary>
        private static readonly fastCSharp.emit.random.config randomConfig = new emit.random.config { IsSecondDateTime = true, IsParseFloat = true, IsNullChar = false };
        /// <summary>
        /// 带成员位图的JSON序列化参数配置
        /// </summary>
        private static readonly fastCSharp.emit.jsonSerializer.config jsonSerializeConfig = new fastCSharp.emit.jsonSerializer.config();

        /// <summary>
        /// JSON序列化测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            #region 引用类型字段成员JSON序列化测试
            data.filedData filedData = fastCSharp.emit.random<data.filedData>.Create(randomConfig);
            string json = fastCSharp.emit.jsonSerializer.ToJson(filedData);
            data.filedData newFieldData = fastCSharp.emit.jsonParser.Parse<data.filedData>(json);
            if (!fastCSharp.emit.equals<data.filedData>.Equals(filedData, newFieldData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的引用类型字段成员JSON序列化测试
            jsonSerializeConfig.MemberMap = fastCSharp.code.memberMap<data.filedData>.NewFull();
            json = fastCSharp.emit.jsonSerializer.ToJson(filedData, jsonSerializeConfig);
            newFieldData = fastCSharp.emit.jsonParser.Parse<data.filedData>(json);
            if (!fastCSharp.emit.equals<data.filedData>.MemberMapEquals(filedData, newFieldData, jsonSerializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 引用类型属性成员JSON序列化测试
            data.propertyData propertyData = fastCSharp.emit.random<data.propertyData>.Create(randomConfig);
            json = fastCSharp.emit.jsonSerializer.ToJson(propertyData);
            data.propertyData newPropertyData = fastCSharp.emit.jsonParser.Parse<data.propertyData>(json);
            if (!fastCSharp.emit.equals<data.propertyData>.Equals(propertyData, newPropertyData))
            {
                return false;
            }
            #endregion

            #region 值类型字段成员JSON序列化测试
            data.structFiledData structFiledData = fastCSharp.emit.random<data.structFiledData>.Create(randomConfig);
            json = fastCSharp.emit.jsonSerializer.ToJson(structFiledData);
            data.structFiledData newStructFiledData = fastCSharp.emit.jsonParser.Parse<data.structFiledData>(json);
            if (!fastCSharp.emit.equals<data.structFiledData>.Equals(structFiledData, newStructFiledData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的值类型字段成员JSON序列化测试
            jsonSerializeConfig.MemberMap = fastCSharp.code.memberMap<data.structFiledData>.NewFull();
            json = fastCSharp.emit.jsonSerializer.ToJson(structFiledData, jsonSerializeConfig);
            newStructFiledData = fastCSharp.emit.jsonParser.Parse<data.structFiledData>(json);
            if (!fastCSharp.emit.equals<data.structFiledData>.MemberMapEquals(structFiledData, newStructFiledData, jsonSerializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 值类型属性成员JSON序列化测试
            data.structPropertyData structPropertyData = fastCSharp.emit.random<data.structPropertyData>.Create(randomConfig);
            json = fastCSharp.emit.jsonSerializer.ToJson(structPropertyData);
            data.structPropertyData newStructPropertyData = fastCSharp.emit.jsonParser.Parse<data.structPropertyData>(json);
            if (!fastCSharp.emit.equals<data.structPropertyData>.Equals(structPropertyData, newStructPropertyData))
            {
                return false;
            }
            #endregion

            return true;
        }
    }
}
