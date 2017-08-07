using System;
using System.Collections.Generic;

namespace fastCSharp.emit
{
    /// <summary>
    /// 枚举值解析
    /// </summary>
    /// <typeparam name="valueType">目标类型</typeparam>
    internal abstract class enumBase<valueType>
    {
        /// <summary>
        /// 枚举值集合
        /// </summary>
        protected static readonly valueType[] enumValues;
        /// <summary>
        /// 枚举名称查找数据
        /// </summary>
        protected static readonly pointer.reference enumSearcher;
        static enumBase()
        {
            Dictionary<string, valueType> values = ((valueType[])System.Enum.GetValues(typeof(valueType))).getDictionary(value => value.ToString());
            enumValues = values.getArray(value => value.Value);
            enumSearcher = fastCSharp.stateSearcher.charsSearcher.Create(values.getArray(value => value.Key), true).Reference;
        }
    }
}
