using System;

namespace fastCSharp
{
    /// <summary>
    /// 枚举解析
    /// </summary>
    /// <typeparam name="enumType">枚举类型</typeparam>
    public static class enmuParser<enumType>
    {
        /// <summary>
        /// 枚举解析器
        /// </summary>
        public static readonly stateSearcher.ascii<enumType> Parser;
        static enmuParser()
        {
            if (typeof(enumType).IsEnum)
            {
                enumType[] values = (enumType[])System.Enum.GetValues(typeof(enumType));
                Parser = new stateSearcher.ascii<enumType>(values.getArray(value => value.ToString()), values, true);
            }
        }
    }
}
