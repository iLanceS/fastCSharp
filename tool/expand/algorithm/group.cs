using System;
using System.Collections.Generic;

namespace fastCSharp.algorithm
{
    /// <summary>
    /// 并查集聚类
    /// </summary>
    public static class group
    {
        /// <summary>
        /// 数据组合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public struct data<valueType> where valueType : IEquatable<valueType>
        {
            /// <summary>
            /// 数据值
            /// </summary>
            public valueType Value1;
            /// <summary>
            /// 数据值
            /// </summary>
            public valueType Value2;
        }
        /// <summary>
        /// 数据组合分组结果
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public struct groupResult<valueType> where valueType : IEquatable<valueType>
        {
            /// <summary>
            /// 数据组合集合
            /// </summary>
            public data<valueType>[] Values;
            /// <summary>
            /// 数据组合分组索引集合
            /// </summary>
            public int[] Indexs;
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据组合集合</param>
        /// <returns>数据分组集合</returns>
        public static unsafe groupResult<valueType> Groups<valueType>(data<valueType>[] values)
             where valueType : IEquatable<valueType>
        {
            if (values.length() == 0) return default(groupResult<valueType>);
            int bufferLength = values.Length * 8 + 3;
            byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get(bufferLength * sizeof(int));
            try
            {
                fixed (byte* bufferFixed = buffer)
                {
                    #region 数字化
                    int* indexFixed = (int*)bufferFixed, numberFixed = (int*)bufferFixed;
                    Dictionary<valueType, int> numberHash = dictionary<valueType>.Create<int>(values.Length);
                    int indexCount = 0, sum;
                    foreach (data<valueType> value in values)
                    {
                        if (!numberHash.TryGetValue(value.Value1, out *numberFixed)) numberHash.Add(value.Value1, *numberFixed = indexCount++);
                        ++numberFixed;
                        if (!numberHash.TryGetValue(value.Value2, out *numberFixed)) numberHash.Add(value.Value2, *numberFixed = indexCount++);
                        ++numberFixed;
                    }
                    numberHash = null;
                    #endregion
                    #region 数量统计
                    int* countFixed = numberFixed + (values.Length << 1);
                    Array.Clear(buffer, (int)((byte*)countFixed - bufferFixed), indexCount * sizeof(int));
                    for (int* start = indexFixed; start != numberFixed; ++countFixed[*start++]) ;
                    #endregion
                    #region 桶排序建图
                    int* groupFixed = countFixed + indexCount;
                    sum = *countFixed;
                    for (int* start = countFixed; ++start != groupFixed; *start = (sum += *start)) ;
                    *groupFixed++ = sum;
                    for (int* start = indexFixed; start != numberFixed; ++start)
                    {
                        int index1 = *start, index2 = *++start;
                        numberFixed[--countFixed[index1]] = index2;
                        numberFixed[--countFixed[index2]] = index1;
                    }
                    #endregion
                    #region 深搜分组统计
                    int* groupEnd = groupFixed + indexCount, bufferEnd = indexFixed + bufferLength;
                    Array.Clear(buffer, (int)((byte*)groupFixed - bufferFixed), indexCount * sizeof(int));
                    int groupCount = 1;
                    for (int* group = groupFixed, searchFixed = bufferEnd; group != groupEnd; ++group)
                    {
                        if (*group == 0)
                        {
                            *--searchFixed = (int)(group - groupFixed);
                            *group = groupCount;
                            do
                            {
                                int index = *searchFixed++;
                                for (int* start = numberFixed + countFixed[index], end = numberFixed + countFixed[index + 1]; start != end; ++start)
                                {
                                    if (groupFixed[index = *start] == 0)
                                    {
                                        *--searchFixed = index;
                                        groupFixed[index] = groupCount;
                                    }
                                }
                            }
                            while (searchFixed != bufferEnd);
                            ++groupCount;
                        }
                    }
                    #endregion
                    #region 反数字化
                    Array.Clear(buffer, (int)((byte*)(countFixed--) - bufferFixed), (groupCount - 1) * sizeof(int));
                    for (int* start = indexFixed; start != numberFixed; start += 2) ++countFixed[*start = groupFixed[*start]];
                    groupResult<valueType> result = new groupResult<valueType> { Values = new data<valueType>[values.Length], Indexs = new int[groupCount] };
                    fixed (int* resultFixed = result.Indexs)
                    {
                        for (sum = 0, groupFixed = countFixed + groupCount, groupEnd = resultFixed; ++countFixed != groupFixed; *groupEnd++ = (sum += *countFixed)) ;
                        groupFixed = indexFixed;
                        countFixed = resultFixed - 1;
                        *groupEnd = values.Length;
                        foreach (data<valueType> value in values)
                        {
                            result.Values[--countFixed[*groupFixed]] = value;
                            groupFixed += 2;
                        }
                    }
                    return result;
                    #endregion
                }
            }
            finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
        }
        /// <summary>
        /// 数据分组结果
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public struct result<valueType> where valueType : IEquatable<valueType>
        {
            /// <summary>
            /// 数据集合
            /// </summary>
            public valueType[] Values;
            /// <summary>
            /// 数据分组索引集合
            /// </summary>
            public int[] Indexs;
            /// <summary>
            /// 分组数量
            /// </summary>
            public int Count
            {
                get { return Indexs != null ? Indexs.Length - 1 : 0; }
            }
            /// <summary>
            /// 获取数据分组
            /// </summary>
            /// <param name="index">数据分组索引</param>
            /// <returns>数据分组</returns>
            public valueType[] GetArray(int index)
            {
                if ((uint)index < Indexs.Length - 1)
                {
                    int startIndex = Indexs[index], length = Indexs[index + 1] - startIndex;
                    valueType[] values = new valueType[length];
                    Array.Copy(Values, startIndex, values, 0, length);
                    return values;
                }
                return nullValue<valueType>.Array;
            }
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据组合集合</param>
        /// <returns>数据分组集合</returns>
        public static unsafe result<valueType> Values<valueType>(data<valueType>[] values)
             where valueType : IEquatable<valueType>
        {
            if (values.length() == 0) return default(result<valueType>);
            byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get((values.Length * 7 + ((values.Length + 7) >> 1)) * sizeof(int));
            try
            {
                fixed (byte* bufferFixed = buffer)
                {
                    #region 数字化
                    int* indexFixed = (int*)bufferFixed, numberFixed = (int*)bufferFixed;
                    Dictionary<valueType, int> numberHash = dictionary<valueType>.Create<int>(values.Length);
                    valueType[] indexValues = new valueType[values.Length << 1];
                    int indexCount = 0, sum;
                    foreach (data<valueType> value in values)
                    {
                        if (!numberHash.TryGetValue(value.Value1, out *numberFixed))
                        {
                            indexValues[indexCount] = value.Value1;
                            numberHash.Add(value.Value1, *numberFixed = indexCount++);
                        }
                        ++numberFixed;
                        if (!numberHash.TryGetValue(value.Value2, out *numberFixed))
                        {
                            indexValues[indexCount] = value.Value2;
                            numberHash.Add(value.Value2, *numberFixed = indexCount++);
                        }
                        ++numberFixed;
                    }
                    numberHash = null;
                    #endregion
                    #region 数量统计
                    int* countFixed = numberFixed + (values.Length << 1);
                    Array.Clear(buffer, (int)((byte*)countFixed - bufferFixed), indexCount * sizeof(int));
                    for (int* start = indexFixed; start != numberFixed; ++countFixed[*start++]) ;
                    #endregion
                    #region 桶排序建图
                    byte* groupFixed = (byte*)(countFixed + indexCount);
                    sum = *countFixed;
                    for (int* start = countFixed; ++start != groupFixed; *start = (sum += *start)) ;
                    *(int*)groupFixed = sum;
                    groupFixed += sizeof(int);
                    for (int* start = indexFixed; start != numberFixed; ++start)
                    {
                        int index1 = *start, index2 = *++start;
                        numberFixed[--countFixed[index1]] = index2;
                        numberFixed[--countFixed[index2]] = index1;
                    }
                    #endregion
                    #region 深搜分组统计
                    int* groupIndexFixed = (int*)(groupFixed + ((indexCount + 3) & (int.MaxValue - 3))), groupIndexEnd = groupIndexFixed, searchFixed = indexFixed;
                    Array.Clear(buffer, (int)((byte*)groupFixed - bufferFixed), ((indexCount + 3) >> 2) << 2);
                    result<valueType> result = new result<valueType> { Values = new valueType[indexCount] };
                    int resultIndex = 0;
                    for (byte* group = groupFixed, groupEnd = groupFixed + indexCount; group != groupEnd; ++group)
                    {
                        if (*group == 0)
                        {
                            int index = (int)(group - groupFixed);
                            *groupIndexEnd++ = resultIndex;
                            *group = 1;
                            *searchFixed++ = index;
                            result.Values[resultIndex++] = indexValues[index];
                            do
                            {
                                for (int* start = numberFixed + countFixed[index = *--searchFixed], end = numberFixed + countFixed[index + 1]; start != end; ++start)
                                {
                                    if (groupFixed[index = *start] == 0)
                                    {
                                        result.Values[resultIndex++] = indexValues[index];
                                        *searchFixed++ = index;
                                        groupFixed[index] = 1;
                                    }
                                }
                            }
                            while (searchFixed != indexFixed);
                        }
                    }
                    indexValues = null;
                    int groupCount = (int)(groupIndexEnd - groupIndexFixed);
                    fixed (int* resultFixed = result.Indexs = new int[groupCount + 1])
                    {
                        unsafer.memory.Copy(groupIndexFixed, resultFixed, groupCount << 2);
                        resultFixed[groupCount] = indexCount;
                    }
                    return result;
                    #endregion
                }
            }
            finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
        }
    }
}
