using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// 二进制序列化定义
    /// </summary>
    partial class dataSerialize
    {
        /// <summary>
        /// 代码生成二进制序列化数据定义
        /// </summary>
        [fastCSharp.code.cSharp.dataSerialize]
        partial class filedDataSerialize : data.filedData
        {
        }

        /// <summary>
        /// 带成员位图的二进制序列化参数配置
        /// </summary>
        private static readonly fastCSharp.emit.dataSerializer.config serializeConfig = new fastCSharp.emit.dataSerializer.config();
        /// <summary>
        /// 二进制序列化测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            #region 引用类型二进制序列化测试(Emit模式)
            data.filedData filedData = fastCSharp.emit.random<data.filedData>.Create();
            byte[] data = fastCSharp.emit.dataSerializer.Serialize(filedData);
            data.filedData newFieldData = fastCSharp.emit.dataDeSerializer.DeSerialize<data.filedData>(data);
            if (!fastCSharp.emit.equals<data.filedData>.Equals(filedData, newFieldData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的引用类型二进制序列化测试(Emit模式)
            serializeConfig.MemberMap = fastCSharp.code.memberMap<data.filedData>.NewFull();
            data = fastCSharp.emit.dataSerializer.Serialize(filedData, serializeConfig);
            newFieldData = fastCSharp.emit.dataDeSerializer.DeSerialize<data.filedData>(data);
            if (!fastCSharp.emit.equals<data.filedData>.MemberMapEquals(filedData, newFieldData, serializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 值类型二进制序列化测试(Emit模式)
            data.structFiledData structFiledData = fastCSharp.emit.random<data.structFiledData>.Create();
            data = fastCSharp.emit.dataSerializer.Serialize(structFiledData);
            data.structFiledData newStructFiledData = fastCSharp.emit.dataDeSerializer.DeSerialize<data.structFiledData>(data);
            if (!fastCSharp.emit.equals<data.structFiledData>.Equals(structFiledData, newStructFiledData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的值类型二进制序列化测试(Emit模式)
            serializeConfig.MemberMap = fastCSharp.code.memberMap<data.structFiledData>.NewFull();
            data = fastCSharp.emit.dataSerializer.Serialize(structFiledData, serializeConfig);
            newStructFiledData = fastCSharp.emit.dataDeSerializer.DeSerialize<data.structFiledData>(data);
            if (!fastCSharp.emit.equals<data.structFiledData>.MemberMapEquals(structFiledData, newStructFiledData, serializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

#if NotFastCSharpCode
#else
            #region 二进制序列化测试(代码生成模式)
            filedDataSerialize filedDataSerialize = fastCSharp.emit.random<filedDataSerialize>.Create();
            data = filedDataSerialize.Serialize();
            filedDataSerialize newFiledDataSerialize = new filedDataSerialize();
            newFiledDataSerialize.DeSerialize(data);
            if (!fastCSharp.emit.equals<filedDataSerialize>.Equals(filedDataSerialize, newFiledDataSerialize))
            {
                return false;
            }
            #endregion
#endif
            return true;
        }
    }
}
