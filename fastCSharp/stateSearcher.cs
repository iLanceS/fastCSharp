using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 状态搜索器
    /// </summary>
    public unsafe static class stateSearcher
    {
        /// <summary>
        /// 字节数组比较大小
        /// </summary>
        private static Func<keyValue<byte[], int>, keyValue<byte[], int>, int> compareHanlde = compare;
        /// <summary>
        /// 字节数组比较大小
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static int compare(keyValue<byte[], int> left, keyValue<byte[], int> right)
        {
            int length = Math.Min(left.Key.Length, right.Key.Length);
            fixed (byte* leftFixed = left.Key, rightFixed = right.Key)
            {
                for (byte* start = leftFixed, end = leftFixed + length, read = rightFixed; start != end; ++start, ++read)
                {
                    if (*start != *read) return *start - *read;
                }
            }
            return left.Key.Length - right.Key.Length;
        }
        /// <summary>
        /// 字符串比较大小
        /// </summary>
        private static Func<keyValue<string, int>, keyValue<string, int>, int> stringCompare = compare;
        /// <summary>
        /// 字符串比较大小
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static int compare(keyValue<string, int> left, keyValue<string, int> right)
        {
            return string.CompareOrdinal(left.Key, right.Key);
        }
        /// <summary>
        /// 状态检测
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static bool check(keyValue<byte[], int>[] values)
        {
            if (values.Length > 1)
            {
                byte[] value = null;
                for (int index = values.Length; index != 0; )
                {
                    byte[] newValue = values[--index].Key;
                    if (newValue.equal(value))
                    {
                        log.Default.Add("状态冲突 " + value.deSerialize(), null, false);
                        return false;
                    }
                    value = newValue;
                }
            }
            return true;
        }
        /// <summary>
        /// 状态检测
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static bool check(keyValue<string, int>[] values)
        {
            if (values.Length > 1)
            {
                string value = null;
                for (int index = values.Length; index != 0; )
                {
                    string newValue = values[--index].Key;
                    if (newValue == value)
                    {
                        log.Default.Add("状态冲突 " + value, null, false);
                        return false;
                    }
                    value = newValue;
                }
            }
            return true;
        }
        /// <summary>
        /// ASCII字节搜索器
        /// </summary>
        internal struct asciiSearcher
        {
            /// <summary>
            /// 状态集合
            /// </summary>
            private byte* state;
            /// <summary>
            /// ASCII字符查找表
            /// </summary>
            private byte* charsAscii;
            /// <summary>
            /// 当前状态
            /// </summary>
            private byte* currentState;
            /// <summary>
            /// 查询矩阵单位尺寸类型
            /// </summary>
            private byte tableType;
            /// <summary>
            /// ASCII字节搜索器
            /// </summary>
            /// <param name="data">数据起始位置</param>
            public asciiSearcher(ref pointer.size data)
            {
                if (data.Data == null)
                {
                    state = charsAscii = currentState = null;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    charsAscii = state + stateCount * 3 * sizeof(int);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// ASCII字节搜索器
            /// </summary>
            /// <param name="data">数据起始位置</param>
            public asciiSearcher(pointer.reference data)
            {
                if (data.Data == null)
                {
                    state = charsAscii = currentState = null;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    charsAscii = state + stateCount * 3 * sizeof(int);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int Search(byte* start, byte* end)
            {
                if (state == null || start >= end) return -1;
                currentState = state;
                do
                {
                    for (byte* prefix = currentState + *(int*)currentState; *prefix != 0; ++prefix, ++start)
                    {
                        if (start == end || *start != *prefix) return -1;
                    }
                    if (start == end) return *(int*)(currentState + sizeof(int) * 2);
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = (int)*(charsAscii + *start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="data">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(byte[] data)
            {
                if (data != null && data.Length != 0)
                {
                    fixed (byte* dataFixed = data) return Search(dataFixed, dataFixed + data.Length);
                }
                return -1;
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="data">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(ref subArray<byte> data)
            {
                if (data.length != 0)
                {
                    fixed (byte* dataFixed = data.array)
                    {
                        byte* start = dataFixed + data.startIndex;
                        return Search(start, start + data.length);
                    }
                }
                return -1;
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="data">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(subArray<byte> data)
            {
                return Search(ref data);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int Search(char* start, char* end)
            {
                if (state == null || start >= end) return -1;
                currentState = state;
                do
                {
                    for (byte* prefix = currentState + *(int*)currentState; *prefix != 0; ++prefix, ++start)
                    {
                        if (start == end || *start != *prefix) return -1;
                    }
                    if (start == end) return *(int*)(currentState + sizeof(int) * 2);
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = (int)*(charsAscii + *start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int UnsafeSearch(ref char* start)
            {
                currentState = state;
                do
                {
                    for (byte* prefix = currentState + *(int*)currentState; *prefix != 0; ++prefix, ++start)
                    {
                        if (*start != *prefix) return -1;
                    }
                    if (*(int*)(currentState + sizeof(int)) == 0) return *(int*)(currentState + sizeof(int) * 2);
                    int index = (int)*(charsAscii + *start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 删除状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int Remove(char* start, char* end)
            {
                if (state == null || start >= end) return -1;
                currentState = state;
                do
                {
                    for (byte* prefix = currentState + *(int*)currentState; *prefix != 0; ++prefix, ++start)
                    {
                        if (start == end || *start != *prefix) return -1;
                    }
                    if (start == end)
                    {
                        int removeIndex = *(int*)(currentState + sizeof(int) * 2);
                        *(int*)(currentState + sizeof(int) * 2) = -1;
                        return removeIndex;
                    }
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = (int)*(charsAscii + *start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="value">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(string value)
            {
                if (value != null && value.Length != 0)
                {
                    fixed (char* valueFixed = value) return Search(valueFixed, valueFixed + value.Length);
                }
                return -1;
            }
            /// <summary>
            /// 删除状态索引
            /// </summary>
            /// <param name="value">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Remove(string value)
            {
                if (value != null && value.Length != 0)
                {
                    fixed (char* valueFixed = value) return Remove(valueFixed, valueFixed + value.Length);
                }
                return -1;
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="value">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(ref subString value)
            {
                if (value.Length != 0)
                {
                    fixed (char* valueFixed = value.value)
                    {
                        char* start = valueFixed + value.StartIndex;
                        return Search(start, start + value.Length);
                    }
                }
                return -1;
            }
            /// <summary>
            /// 状态数据创建器
            /// </summary>
            internal struct stringBuilder
            {
                /// <summary>
                /// 状态集合
                /// </summary>
                private keyValue<string, int>[] values;
                /// <summary>
                /// 状态数据
                /// </summary>
                public pointer.size Data;
                /// <summary>
                /// 状态字符集合
                /// </summary>
                private fixedMap chars;
                /// <summary>
                /// 状态集合
                /// </summary>
                private byte* state;
                /// <summary>
                /// ASCII字符查找表
                /// </summary>
                private byte* charsAscii;
                /// <summary>
                /// 前缀集合
                /// </summary>
                private byte* prefix;
                /// <summary>
                /// 状态矩阵
                /// </summary>
                private byte* table;
                /// <summary>
                /// 状态数量
                /// </summary>
                private int stateCount;
                /// <summary>
                /// 矩阵状态数量
                /// </summary>
                private int tableCount;
                /// <summary>
                /// 前缀数量
                /// </summary>
                private int prefixSize;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private int tableType;
                /// <summary>
                /// 状态字符数量
                /// </summary>
                private int charCount;
                /// <summary>
                /// 状态数据创建器
                /// </summary>
                /// <param name="values">状态集合</param>
                /// <param name="isStaticUnmanaged">是否固定内存申请</param>
                public stringBuilder(keyValue<string, int>[] values, bool isStaticUnmanaged)
                {
                    this.values = values;
                    prefixSize = tableCount = stateCount = tableType = charCount = 0;
                    state = charsAscii = prefix = table = null;
                    if (values.Length > 1)
                    {
                        byte* chars = stackalloc byte[128 >> 3];
                        this.chars = new fixedMap(chars, 128 >> 3, 0);
                        Data = new pointer.size();
                        count(0, values.Length, 0);
                        for (byte* start = chars, end = chars + (128 >> 3); start != end; start += sizeof(int))
                        {
                            charCount += (*(uint*)start).bitCount();
                        }
                        int size = (1 + (stateCount += tableCount) * 3) * sizeof(int) + 128 + 4 + (prefixSize & (int.MaxValue - 3));
                        if (stateCount < 256) size += tableCount * (charCount + 1);
                        else if (stateCount < 65536)
                        {
                            size += tableCount * (charCount + 1) * sizeof(ushort);
                            tableType = 1;
                        }
                        else
                        {
                            size += tableCount * (charCount + 1) * sizeof(int);
                            tableType = 2;
                        }
                        Data = unmanaged.Get(size, true, isStaticUnmanaged);
                        *Data.Int = stateCount;//状态数量[int]
                        state = Data.Byte + sizeof(int);//状态集合[stateCount*(前缀位置[int]+状态位置[int]+名称索引[int])]
                        charsAscii = state + (stateCount * 3) * sizeof(int);//ascii字符查找表[128*byte]
                        byte charIndex = 0;
                        for (byte index = 1; index != 128; ++index)
                        {
                            if (this.chars.Get(index)) *(charsAscii + index) = ++charIndex;
                        }
                        prefix = charsAscii + 128;//前缀集合
                        table = prefix + ((prefixSize & (int.MaxValue - 3)) + 4);//状态矩阵[tableCount*(charCount+1)*[byte/ushort/int]]
                        *prefix++ = (byte)charCount;//字符数量
                        stateCount = 0;
                        create(0, values.Length, 0);
                    }
                    else
                    {
                        chars = new fixedMap();
                        string value = values[0].Key;
                        fixed (char* valueFixed = value)
                        {
                            if (values[0].Key.Length <= 128)
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 + 1, false, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3;//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                prefix = Data.Byte + sizeof(int) * 4;
                                fastCSharp.unsafer.String.WriteBytes(valueFixed, value.Length, prefix);
                                *(prefix + value.Length) = *(prefix + 128) = 0;
                            }
                            else
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 + 1 + value.Length + 1, true, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3 + 128 + 1;//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                fastCSharp.unsafer.String.WriteBytes(valueFixed, value.Length, Data.Byte + sizeof(int) * 3 + 128 + 1);
                            }
                        }
                    }
                }
                /// <summary>
                /// 计算状态数量
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void count(int start, int end, int current)
                {
                    ++tableCount;
                    int index = start, prefixSize = 0;
                    char value = (char)0;
                    while (values[start].Key.Length != current)
                    {
                        value = values[start].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                        if (index != end) break;
                        ++prefixSize;
                        index = start;
                        ++current;
                    }
                    if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            if (value >= 128 || value == 0) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                            chars.Set(value);
                            if (count == 1)
                            {
                                ++stateCount;
                                prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                            }
                            else this.count(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
                /// <summary>
                /// 创建状态数据
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void create(int start, int end, int current)
                {
                    byte* prefix = this.prefix, table = this.table;
                    *(int*)(state + sizeof(int)) = (int)(table - state);
                    int index = start;
                    char value = (char)0;
                    if (values[start].Key.Length == current) *(int*)(state + sizeof(int) * 2) = values[start].Value;
                    else
                    {
                        do
                        {
                            value = values[index].Key[current];
                            while (++index != end && values[index].Key[current] == value) ;
                            if (index != end)
                            {
                                *(int*)(state + sizeof(int) * 2) = -1;
                                break;
                            }
                            *this.prefix++ = (byte)value;
                            if (values[index = start].Key.Length == ++current)
                            {
                                *(int*)(state + sizeof(int) * 2) = values[index].Value;
                                break;
                            }
                        }
                        while (true);
                    }
                    if (prefix == this.prefix) *(int*)state = (int)(charsAscii - state);
                    else
                    {
                        *this.prefix++ = 0;
                        *(int*)state = (int)(prefix - state);
                    }
                    state += sizeof(int) * 3;
                    ++stateCount;
                    if (tableType == 0) this.table += charCount + 1;
                    else if (tableType == 1) this.table += (charCount + 1) * sizeof(ushort);
                    else this.table += (charCount + 1) * sizeof(int);
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            int charIndex = (int)*(charsAscii + value);
                            if (tableType == 0) *(table + charIndex) = (byte)stateCount;
                            else if (tableType == 1) *(ushort*)(table + charIndex * sizeof(ushort)) = (ushort)stateCount;
                            else *(int*)(table + charIndex * sizeof(int)) = stateCount * 3 * sizeof(int);
                            if (count == 1)
                            {
                                int prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize == 0) *(int*)state = (int)(charsAscii - state);
                                else
                                {
                                    *(int*)state = (int)(this.prefix - state);
                                    fixed (char* charFixed = values[start].Key)
                                    {
                                        fastCSharp.unsafer.String.WriteBytes(charFixed + current + 1, prefixSize, this.prefix);
                                        *(this.prefix += prefixSize) = 0;
                                        ++this.prefix;
                                    }
                                }
                                *(int*)(state + sizeof(int) * 2) = values[start].Value;
                                ++stateCount;
                                state += sizeof(int) * 3;
                            }
                            else create(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 状态数据创建器
            /// </summary>
            private struct byteArrayBuilder
            {
                /// <summary>
                /// 状态集合
                /// </summary>
                private keyValue<byte[], int>[] values;
                /// <summary>
                /// 状态数据
                /// </summary>
                public pointer.size Data;
                /// <summary>
                /// 状态字符集合
                /// </summary>
                private fixedMap chars;
                /// <summary>
                /// 状态集合
                /// </summary>
                private byte* state;
                /// <summary>
                /// ASCII字符查找表
                /// </summary>
                private byte* charsAscii;
                /// <summary>
                /// 前缀集合
                /// </summary>
                private byte* prefix;
                /// <summary>
                /// 状态矩阵
                /// </summary>
                private byte* table;
                /// <summary>
                /// 状态数量
                /// </summary>
                private int stateCount;
                /// <summary>
                /// 矩阵状态数量
                /// </summary>
                private int tableCount;
                /// <summary>
                /// 前缀数量
                /// </summary>
                private int prefixSize;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private int tableType;
                /// <summary>
                /// 状态字符数量
                /// </summary>
                private int charCount;
                /// <summary>
                /// 状态数据创建器
                /// </summary>
                /// <param name="values">状态集合</param>
                /// <param name="isStaticUnmanaged">是否固定内存申请</param>
                public byteArrayBuilder(keyValue<byte[], int>[] values, bool isStaticUnmanaged)
                {
                    this.values = values;
                    prefixSize = tableCount = stateCount = tableType = charCount = 0;
                    state = charsAscii = prefix = table = null;
                    if (values.Length > 1)
                    {
                        byte* chars = stackalloc byte[128 >> 3];
                        this.chars = new fixedMap(chars, 128 >> 3, 0);
                        Data = new pointer.size();
                        count(0, values.Length, 0);
                        for (byte* start = chars, end = chars + (128 >> 3); start != end; start += sizeof(int))
                        {
                            charCount += (*(uint*)start).bitCount();
                        }
                        int size = (1 + (stateCount += tableCount) * 3) * sizeof(int) + 128 + 4 + (prefixSize & (int.MaxValue - 3));
                        if (stateCount < 256) size += tableCount * (charCount + 1);
                        else if (stateCount < 65536)
                        {
                            size += tableCount * (charCount + 1) * sizeof(ushort);
                            tableType = 1;
                        }
                        else
                        {
                            size += tableCount * (charCount + 1) * sizeof(int);
                            tableType = 2;
                        }
                        Data = unmanaged.Get(size, true, isStaticUnmanaged);
                        *Data.Int = stateCount;//状态数量[int]
                        state = Data.Byte + sizeof(int);//状态集合[stateCount*(前缀位置[int]+状态位置[int]+名称索引[int])]
                        charsAscii = state + (stateCount * 3) * sizeof(int);//ascii字符查找表[128*byte]
                        byte charIndex = 0;
                        for (byte index = 1; index != 128; ++index)
                        {
                            if (this.chars.Get(index)) *(charsAscii + index) = ++charIndex;
                        }
                        prefix = charsAscii + 128;//前缀集合
                        table = prefix + ((prefixSize & (int.MaxValue - 3)) + 4);//状态矩阵[tableCount*(charCount+1)*[byte/ushort/int]]
                        *prefix++ = (byte)charCount;//字符数量
                        stateCount = 0;
                        create(0, values.Length, 0);
                    }
                    else
                    {
                        chars = new fixedMap();
                        byte[] value = values[0].Key;
                        fixed (byte* valueFixed = value)
                        {
                            if (values[0].Key.Length <= 128)
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 + 1, false, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3;//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                prefix = Data.Byte + sizeof(int) * 4;
                                fastCSharp.unsafer.memory.SimpleCopy(valueFixed, prefix, value.Length);
                                *(prefix + value.Length) = *(prefix + 128) = 0;
                            }
                            else
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 + 1 + value.Length + 1, true, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3 + 128 + 1;//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                fastCSharp.unsafer.memory.SimpleCopy(valueFixed, Data.Byte + sizeof(int) * 3 + 128 + 1, value.Length);
                            }
                        }
                    }
                }
                /// <summary>
                /// 计算状态数量
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void count(int start, int end, int current)
                {
                    ++tableCount;
                    int index = start, prefixSize = 0;
                    byte value = 0;
                    while (values[start].Key.Length != current)
                    {
                        value = values[start].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                        if (index != end) break;
                        ++prefixSize;
                        index = start;
                        ++current;
                    }
                    if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            if (value >= 128 || value == 0) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                            chars.Set(value);
                            if (count == 1)
                            {
                                ++stateCount;
                                prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                            }
                            else this.count(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
                /// <summary>
                /// 创建状态数据
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void create(int start, int end, int current)
                {
                    byte* prefix = this.prefix, table = this.table;
                    *(int*)(state + sizeof(int)) = (int)(table - state);
                    int index = start;
                    byte value = 0;
                    if (values[start].Key.Length == current) *(int*)(state + sizeof(int) * 2) = values[start].Value;
                    else
                    {
                        do
                        {
                            value = values[index].Key[current];
                            while (++index != end && values[index].Key[current] == value) ;
                            if (index != end)
                            {
                                *(int*)(state + sizeof(int) * 2) = -1;
                                break;
                            }
                            *this.prefix++ = (byte)value;
                            if (values[index = start].Key.Length == ++current)
                            {
                                *(int*)(state + sizeof(int) * 2) = values[index].Value;
                                break;
                            }
                        }
                        while (true);
                    }
                    if (prefix == this.prefix) *(int*)state = (int)(charsAscii - state);
                    else
                    {
                        *this.prefix++ = 0;
                        *(int*)state = (int)(prefix - state);
                    }
                    state += sizeof(int) * 3;
                    ++stateCount;
                    if (tableType == 0) this.table += charCount + 1;
                    else if (tableType == 1) this.table += (charCount + 1) * sizeof(ushort);
                    else this.table += (charCount + 1) * sizeof(int);
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            int charIndex = (int)*(charsAscii + value);
                            if (tableType == 0) *(table + charIndex) = (byte)stateCount;
                            else if (tableType == 1) *(ushort*)(table + charIndex * sizeof(ushort)) = (ushort)stateCount;
                            else *(int*)(table + charIndex * sizeof(int)) = stateCount * 3 * sizeof(int);
                            if (count == 1)
                            {
                                int prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize == 0) *(int*)state = (int)(charsAscii - state);
                                else
                                {
                                    *(int*)state = (int)(this.prefix - state);
                                    fixed (byte* charFixed = values[start].Key)
                                    {
                                        fastCSharp.unsafer.memory.SimpleCopy(charFixed + current + 1, this.prefix, prefixSize);
                                        *(this.prefix += prefixSize) = 0;
                                        ++this.prefix;
                                    }
                                }
                                *(int*)(state + sizeof(int) * 2) = values[start].Value;
                                ++stateCount;
                                state += sizeof(int) * 3;
                            }
                            else create(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 创建状态查找数据
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            /// <returns>状态查找数据</returns>
            public static pointer.size Create(string[] states, bool isStaticUnmanaged)
            {
                if (states.length() != 0)
                {
                    int index = 0;
                    keyValue<string, int>[] strings = new keyValue<string, int>[states.Length];
                    foreach (string name in states)
                    {
                        strings[index].Set(name, index);
                        ++index;
                    }
                    strings = strings.sort(stringCompare);
                    return new stringBuilder(check(strings) ? strings : nullValue<keyValue<string, int>>.Array, isStaticUnmanaged).Data;
                }
                return new pointer.size();
            }
            /// <summary>
            /// 创建状态查找数据
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            /// <returns>状态查找数据</returns>
            public static pointer.size Create(byte[][] states, bool isStaticUnmanaged)
            {
                if (states.length() != 0)
                {
                    int index = 0;
                    keyValue<byte[], int>[] datas = new keyValue<byte[], int>[states.Length];
                    foreach (byte[] name in states)
                    {
                        datas[index].Set(name, index);
                        ++index;
                    }
                    datas = datas.sort(compareHanlde);
                    return new byteArrayBuilder(check(datas) ? datas : nullValue<keyValue<byte[], int>>.Array, isStaticUnmanaged).Data;
                }
                return new pointer.size();
            }
        }
        /// <summary>
        /// ASCII字节状态搜索
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public sealed class ascii<valueType> : IDisposable
        {
            /// <summary>
            /// 状态搜索数据
            /// </summary>
            private pointer.size data;
            /// <summary>
            /// 状态数据集合
            /// </summary>
            private valueType[] values;
            /// <summary>
            /// 是否固定内存申请
            /// </summary>
            private bool isStaticUnmanaged;
            /// <summary>
            /// ASCII字节状态搜索
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="values">状态数据集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            public ascii(string[] states, valueType[] values, bool isStaticUnmanaged = false)
            {
                if (states == null || values == null) log.Default.Throw(log.exceptionType.Null);
                if (states.Length > values.Length) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                this.values = values;
                data = asciiSearcher.Create(states, this.isStaticUnmanaged = isStaticUnmanaged);
            }
            /// <summary>
            /// ASCII字节状态搜索
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="values">状态数据集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            public ascii(byte[][] states, valueType[] values, bool isStaticUnmanaged = false)
            {
                if (states == null || values == null) log.Default.Throw(log.exceptionType.Null);
                if (states.Length > values.Length) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                this.values = values;
                data = asciiSearcher.Create(states, isStaticUnmanaged);
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal valueType Get(byte* start, byte* end, valueType nullValue = default(valueType))
            {
                int index = new asciiSearcher(ref data).Search(start, end);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(byte[] state, valueType nullValue = default(valueType))
            {
                int index = new asciiSearcher(ref data).Search(state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="value">目标数据</param>
            /// <returns>是否存在匹配状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(byte[] state, ref valueType value)
            {
                int index = new asciiSearcher(ref data).Search(state);
                if (index >= 0)
                {
                    value = values[index];
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(subArray<byte> state, valueType nullValue = default(valueType))
            {
                return Get(ref state, nullValue);
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(ref subArray<byte> state, valueType nullValue = default(valueType))
            {
                int index = new asciiSearcher(ref data).Search(ref state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="value">目标数据</param>
            /// <returns>是否存在匹配状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(subArray<byte> state, ref valueType value)
            {
                return Get(ref state, ref value);
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="value">目标数据</param>
            /// <returns>是否存在匹配状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(ref subArray<byte> state, ref valueType value)
            {
                int index = new asciiSearcher(ref data).Search(ref state);
                if (index >= 0)
                {
                    value = values[index];
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(string state, valueType nullValue = default(valueType))
            {
                int index = new asciiSearcher(ref data).Search(state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 判断是否存在状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <returns>是否存在状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool ContainsKey(string state)
            {
                return new asciiSearcher(ref data).Search(state) >= 0;
            }
            /// <summary>
            /// 删除状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <returns>是否存在状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Remove(string state)
            {
                return new asciiSearcher(ref data).Remove(state) >= 0;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(ref subString state, valueType nullValue = default(valueType))
            {
                int index = new asciiSearcher(ref data).Search(ref state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 判断是否存在状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <returns>是否存在状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool ContainsKey(ref subString state)
            {
                return new asciiSearcher(ref data).Search(ref state) >= 0;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (isStaticUnmanaged) unmanaged.FreeStatic(ref data);
                else unmanaged.Free(ref data);
                values = null;
            }
        }
        /// <summary>
        /// 字节数组搜索器
        /// </summary>
        internal struct byteArraySearcher
        {
            /// <summary>
            /// 状态集合
            /// </summary>
            private byte* state;
            /// <summary>
            /// 字节查找表
            /// </summary>
            private byte* bytes;
            /// <summary>
            /// 当前状态
            /// </summary>
            private byte* currentState;
            /// <summary>
            /// 查询矩阵单位尺寸类型
            /// </summary>
            private byte tableType;
            /// <summary>
            /// ASCII字节搜索器
            /// </summary>
            /// <param name="data">数据起始位置</param>
            public byteArraySearcher(ref pointer.size data)
            {
                if (data.Data == null)
                {
                    state = bytes = currentState = null;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    bytes = state + stateCount * 3 * sizeof(int);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int Search(byte* start, byte* end)
            {
                if (state == null || start >= end) return -1;
                currentState = state;
                do
                {
                    byte* prefix = currentState + *(int*)currentState;
                    int prefixSize = *(ushort*)(prefix - sizeof(ushort));
                    if (prefixSize != 0)
                    {
                        for (byte* endPrefix = prefix + prefixSize; prefix != endPrefix; ++prefix, ++start)
                        {
                            if (start == end || *start != *prefix) return -1;
                        }
                    }
                    if (start == end) return *(int*)(currentState + sizeof(int) * 2);
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = (int)*(bytes + *start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="data">匹配状态</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(ref subArray<byte> data)
            {
                if (data.length != 0)
                {
                    fixed (byte* dataFixed = data.array)
                    {
                        byte* start = dataFixed + data.startIndex;
                        return Search(start, start + data.length);
                    }
                }
                return -1;
            }
            /// <summary>
            /// 状态数据创建器
            /// </summary>
            private struct builder
            {
                /// <summary>
                /// 状态集合
                /// </summary>
                private keyValue<byte[], int>[] values;
                /// <summary>
                /// 状态数据
                /// </summary>
                public pointer.size Data;
                /// <summary>
                /// 状态字符集合
                /// </summary>
                private fixedMap chars;
                /// <summary>
                /// 状态集合
                /// </summary>
                private byte* state;
                /// <summary>
                /// ASCII字符查找表
                /// </summary>
                private byte* bytes;
                /// <summary>
                /// 前缀集合
                /// </summary>
                private byte* prefix;
                /// <summary>
                /// 空前缀
                /// </summary>
                private byte* nullPrefix;
                /// <summary>
                /// 状态矩阵
                /// </summary>
                private byte* table;
                /// <summary>
                /// 状态数量
                /// </summary>
                private int stateCount;
                /// <summary>
                /// 矩阵状态数量
                /// </summary>
                private int tableCount;
                /// <summary>
                /// 前缀数量
                /// </summary>
                private int prefixSize;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private int tableType;
                /// <summary>
                /// 状态字符数量
                /// </summary>
                private int charCount;
                /// <summary>
                /// 状态数据创建器
                /// </summary>
                /// <param name="values">状态集合</param>
                /// <param name="isStaticUnmanaged">是否固定内存申请</param>
                public builder(keyValue<byte[], int>[] values, bool isStaticUnmanaged)
                {
                    this.values = values;
                    prefixSize = tableCount = stateCount = tableType = charCount = 0;
                    state = bytes = nullPrefix = prefix = table = null;
                    if (values.Length > 1)
                    {
                        byte* chars = stackalloc byte[256 >> 3];
                        this.chars = new fixedMap(chars, 256 >> 3, 0);
                        Data = new pointer.size();
                        count(0, values.Length, 0);
                        for (byte* start = chars, end = chars + (256 >> 3); start != end; start += sizeof(int))
                        {
                            charCount += (*(uint*)start).bitCount();
                        }
                        int size = (1 + (stateCount += tableCount) * 3) * sizeof(int) + 256 + 4 + ((prefixSize + 3) & (int.MaxValue - 3));
                        if (stateCount < 256) size += tableCount * charCount;
                        else if (stateCount < 65536)
                        {
                            size += tableCount * charCount * sizeof(ushort);
                            tableType = 1;
                        }
                        else
                        {
                            size += tableCount * charCount * sizeof(int);
                            tableType = 2;
                        }
                        Data = unmanaged.Get(size, true, isStaticUnmanaged);
                        *Data.Int = stateCount;//状态数量[int]
                        state = Data.Byte + sizeof(int);//状态集合[stateCount*(前缀位置[int]+状态位置[int]+名称索引[int])]
                        bytes = state + (stateCount * 3) * sizeof(int);//字节查找表[256*byte]
                        byte charIndex = 0;
                        for (int index = 0; index != 256; ++index)
                        {
                            if (this.chars.Get(index)) *(bytes + index) = charIndex++;
                        }
                        nullPrefix = bytes + 256;//空前缀
                        table = nullPrefix + (((prefixSize + 3) & (int.MaxValue - 3)) + 4);//状态矩阵[tableCount*charCount*[byte/ushort/int]]
                        *(ushort*)nullPrefix = (ushort)charCount;//字符数量
                        prefix = nullPrefix + sizeof(int) + sizeof(ushort);//前缀集合
                        nullPrefix += sizeof(int);
                        stateCount = 0;
                        create(0, values.Length, 0);
                    }
                    else
                    {
                        chars = new fixedMap();
                        byte[] value = values[0].Key;
                        fixed (byte* valueFixed = value)
                        {
                            if (values[0].Key.Length <= 254)
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 256 + 2, false, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3 + sizeof(ushort);//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                prefix = Data.Byte + sizeof(int) * 4;
                                *(ushort*)prefix = (ushort)value.Length;
                                fastCSharp.unsafer.memory.SimpleCopy(valueFixed, prefix + sizeof(ushort), value.Length);
                                *(ushort*)(prefix + 256) = 0;
                            }
                            else
                            {
                                Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 256 + 4 + 2 + value.Length, true, isStaticUnmanaged);
                                *Data.Int = 1;//状态数量
                                state = Data.Byte + sizeof(int);
                                *(int*)state = sizeof(int) * 3 + 256 + 4 + sizeof(ushort);//前缀位置
                                *(int*)(state + sizeof(int)) = 0;//状态位置
                                *(int*)(state + sizeof(int) * 2) = values[0].Value;//名称索引
                                prefix = Data.Byte + sizeof(int) * 4 + 256 + 4;
                                *(ushort*)prefix = (ushort)value.Length;
                                fastCSharp.unsafer.memory.SimpleCopy(valueFixed, prefix + sizeof(ushort), value.Length);
                            }
                        }
                    }
                }
                /// <summary>
                /// 计算状态数量
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void count(int start, int end, int current)
                {
                    ++tableCount;
                    int index = start, prefixSize = 0;
                    byte value = 0;
                    while (values[start].Key.Length != current)
                    {
                        value = values[start].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                        if (index != end) break;
                        ++prefixSize;
                        index = start;
                        ++current;
                    }
                    if (prefixSize != 0) this.prefixSize += (prefixSize + 3) & (int.MaxValue - 1);
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            chars.Set(value);
                            if (count == 1)
                            {
                                ++stateCount;
                                prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize != 0) this.prefixSize += (prefixSize + 3) & (int.MaxValue - 1);
                            }
                            else this.count(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
                /// <summary>
                /// 创建状态数据
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void create(int start, int end, int current)
                {
                    byte* prefix = this.prefix, table = this.table;
                    *(int*)(state + sizeof(int)) = (int)(table - state);
                    int index = start;
                    byte value = 0;
                    if (values[start].Key.Length == current) *(int*)(state + sizeof(int) * 2) = values[start].Value;
                    else
                    {
                        do
                        {
                            value = values[index].Key[current];
                            while (++index != end && values[index].Key[current] == value) ;
                            if (index != end)
                            {
                                *(int*)(state + sizeof(int) * 2) = -1;
                                break;
                            }
                            *this.prefix++ = (byte)value;
                            if (values[index = start].Key.Length == ++current)
                            {
                                *(int*)(state + sizeof(int) * 2) = values[index].Value;
                                break;
                            }
                        }
                        while (true);
                    }
                    int prefixSize = (int)(this.prefix - prefix);
                    if (prefixSize == 0) *(int*)state = (int)(nullPrefix - state);
                    else
                    {
                        *(ushort*)(prefix - sizeof(ushort)) = (ushort)prefixSize;
                        *(int*)state = (int)(prefix - state);
                        this.prefix += sizeof(ushort) + (prefixSize & 1);
                    }
                    state += sizeof(int) * 3;
                    ++stateCount;
                    if (tableType == 0) this.table += charCount;
                    else if (tableType == 1) this.table += charCount * sizeof(ushort);
                    else this.table += charCount * sizeof(int);
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            int charIndex = (int)*(bytes + value);
                            if (tableType == 0) *(table + charIndex) = (byte)stateCount;
                            else if (tableType == 1) *(ushort*)(table + charIndex * sizeof(ushort)) = (ushort)stateCount;
                            else *(int*)(table + charIndex * sizeof(int)) = stateCount * 3 * sizeof(int);
                            if (count == 1)
                            {
                                prefixSize = values[start].Key.Length - current - 1;
                                if (prefixSize == 0) *(int*)state = (int)(nullPrefix - state);
                                else
                                {
                                    *(int*)state = (int)(this.prefix - state);
                                    *(ushort*)(this.prefix - sizeof(ushort)) = (ushort)prefixSize;
                                    fixed (byte* charFixed = values[start].Key)
                                    {
                                        fastCSharp.unsafer.memory.SimpleCopy(charFixed + current + 1, this.prefix, prefixSize);
                                        this.prefix += (prefixSize + 3) & (int.MaxValue - 1);
                                    }
                                }
                                *(int*)(state + sizeof(int) * 2) = values[start].Value;
                                ++stateCount;
                                state += sizeof(int) * 3;
                            }
                            else create(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = values[start = index].Key[current];
                        while (++index != end && values[index].Key[current] == value) ;
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 创建状态查找数据
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            /// <returns>状态查找数据</returns>
            public static pointer.size Create(byte[][] states, bool isStaticUnmanaged)
            {
                if (states.length() != 0)
                {
                    int index = 0;
                    keyValue<byte[], int>[] strings = new keyValue<byte[], int>[states.Length];
                    foreach (byte[] name in states)
                    {
                        if (name.Length >= 65536) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                        strings[index].Set(name, index);
                        ++index;
                    }
                    strings = strings.sort(compareHanlde);
                    return new builder(check(strings) ? strings : nullValue<keyValue<byte[], int>>.Array, isStaticUnmanaged).Data;
                }
                return new pointer.size();
            }
        }
        /// <summary>
        /// 字节数组状态搜索
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public sealed class byteArray<valueType> : IDisposable
        {
            /// <summary>
            /// 状态搜索数据
            /// </summary>
            private pointer.size data;
            /// <summary>
            /// 状态数据集合
            /// </summary>
            private valueType[] values;
            /// <summary>
            /// 是否固定内存申请
            /// </summary>
            private bool isStaticUnmanaged;
            /// <summary>
            /// 字节数组状态搜索
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="values">状态数据集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            public byteArray(byte[][] states, valueType[] values, bool isStaticUnmanaged)
            {
                if (states == null || values == null) log.Default.Throw(log.exceptionType.Null);
                if (states.Length > values.Length) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                this.values = values;
                data = byteArraySearcher.Create(states, this.isStaticUnmanaged = isStaticUnmanaged);
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(ref subArray<byte> state, valueType nullValue = default(valueType))
            {
                int index = new byteArraySearcher(ref data).Search(ref state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="value">目标数据</param>
            /// <returns>是否存在匹配状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(ref subArray<byte> state, ref valueType value)
            {
                int index = new byteArraySearcher(ref data).Search(ref state);
                if (index >= 0)
                {
                    value = values[index];
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="length">状态长度</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(void* state, int length, valueType nullValue = default(valueType))
            {
                int index = new byteArraySearcher(ref data).Search((byte*)state, (byte*)state + length);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (isStaticUnmanaged) unmanaged.FreeStatic(ref data);
                else unmanaged.Free(ref data);
                values = null;
            }
        }
        /// <summary>
        /// 字符搜索器
        /// </summary>
        public struct charsSearcher
        {
            /// <summary>
            /// 状态集合
            /// </summary>
            private byte* state;
            /// <summary>
            /// ASCII字符查找表
            /// </summary>
            private byte* charsAscii;
            /// <summary>
            /// 特殊字符串查找表
            /// </summary>
            private byte* charStart;
            /// <summary>
            /// 特殊字符串查找表结束位置
            /// </summary>
            private byte* charEnd;
            /// <summary>
            /// 当前状态
            /// </summary>
            private byte* currentState;
            /// <summary>
            /// 特殊字符起始值
            /// </summary>
            private int charIndex;
            /// <summary>
            /// 查询矩阵单位尺寸类型
            /// </summary>
            private byte tableType;
            /// <summary>
            /// 字符搜索器
            /// </summary>
            /// <param name="data">数据起始位置</param>
            public charsSearcher(pointer.reference data)
            {
                if (data.Data == null)
                {
                    state = charsAscii = charStart = charEnd = currentState = null;
                    charIndex = 0;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    charsAscii = state + stateCount * 3 * sizeof(int);
                    charStart = charsAscii + 128 * sizeof(ushort);
                    charIndex = *(ushort*)charStart;
                    charStart += sizeof(ushort) * 2;
                    charEnd = charStart + *(ushort*)(charStart - sizeof(ushort)) * sizeof(ushort);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// 字符搜索器
            /// </summary>
            /// <param name="data">数据起始位置</param>
            public charsSearcher(ref pointer.size data)
            {
                if (data.Data == null)
                {
                    state = charsAscii = charStart = charEnd = currentState = null;
                    charIndex = 0;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    charsAscii = state + stateCount * 3 * sizeof(int);
                    charStart = charsAscii + 128 * sizeof(ushort);
                    charIndex = *(ushort*)charStart;
                    charStart += sizeof(ushort) * 2;
                    charEnd = charStart + *(ushort*)(charStart - sizeof(ushort)) * sizeof(ushort);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Search(char* start, char* end)
            {
                return state == null || start >= end ? -1 : UnsafeSearch(start, end);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="length">匹配长度</param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int UnsafeSearch(char* start, int length)
            {
                return UnsafeSearch(start, start + length);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="start">匹配起始位置</param>
            /// <param name="end">匹配结束位置</param>
            /// <returns>状态索引,失败返回-1</returns>
            public int UnsafeSearch(char* start, char* end)
            {
                if (state == null) return -1;
                currentState = state;
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (start == end || *start != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (++start == end || *start != *prefix) return -1;
                        }
                        ++start;
                    }
                    if (start == end) return *(int*)(currentState + sizeof(int) * 2);
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = *start < 128 ? (int)*(ushort*)(charsAscii + (*start << 1)) : getCharIndex(*start);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                    ++start;
                }
                while (true);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="value"></param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe int Search(string value)
            {
                if (state == null || value == null) return -1;
                fixed (char* valueFixed = value) return UnsafeSearch(valueFixed, valueFixed + value.Length);
            }
            /// <summary>
            /// 获取状态索引
            /// </summary>
            /// <param name="value"></param>
            /// <returns>状态索引,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe int Search(ref subString value)
            {
                if (state == null) return -1;
                fixed (char* valueFixed = value.value)
                {
                    char* start = valueFixed + value.StartIndex;
                    return UnsafeSearch(start, start + value.Length);
                }
            }
            /// <summary>
            /// 获取特殊字符索引值
            /// </summary>
            /// <param name="value">特殊字符</param>
            /// <returns>索引值,匹配失败返回0</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private int getCharIndex(char value)
            {
                char* current = GetCharIndex((char*)charStart, (char*)charEnd, value);
                return current == null ? 0 : (charIndex + (int)(current - (char*)charStart));
            }
            /// <summary>
            /// 获取特殊字符索引值
            /// </summary>
            /// <param name="charStart">特殊字符串查找表</param>
            /// <param name="charEnd">特殊字符串查找表结束位置</param>
            /// <param name="value">特殊字符</param>
            /// <returns>特殊字符位置,匹配失败返回null</returns>
            internal static char* GetCharIndex(char* charStart, char* charEnd, char value)
            {
                char* current = charStart + ((int)(charEnd - charStart) >> 1);
                while (*current != value)
                {
                    if (value < *current)
                    {
                        if (current == charStart) return null;
                        charEnd = current;
                        current = charStart + ((int)(charEnd - charStart) >> 1);
                    }
                    else
                    {
                        if ((charStart = current + 1) == charEnd) return null;
                        current = charStart + ((int)(charEnd - charStart) >> 1);
                    }
                }
                return current;
            }
            /// <summary>
            /// 状态数据创建器
            /// </summary>
            private struct builder
            {
                /// <summary>
                /// 名称集合
                /// </summary>
                private keyValue<string, int>[] names;
                /// <summary>
                /// 状态数据
                /// </summary>
                public pointer.size Data;
                /// <summary>
                /// 状态集合
                /// </summary>
                private byte* state;
                /// <summary>
                /// ASCII字符查找表
                /// </summary>
                private byte* charsAscii;
                /// <summary>
                /// 特殊字符串查找表
                /// </summary>
                private byte* charStart;
                /// <summary>
                /// 特殊字符串查找表结束位置
                /// </summary>
                private byte* charEnd;
                /// <summary>
                /// 前缀集合
                /// </summary>
                private byte* prefix;
                /// <summary>
                /// 状态矩阵
                /// </summary>
                private byte* table;
                /// <summary>
                /// 状态数量
                /// </summary>
                private int stateCount;
                /// <summary>
                /// 矩阵状态数量
                /// </summary>
                private int tableCount;
                /// <summary>
                /// 前缀数量
                /// </summary>
                private int prefixSize;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private int tableType;
                /// <summary>
                /// 状态字符集合
                /// </summary>
                private list<char> chars;
                /// <summary>
                /// 状态数据创建器
                /// </summary>
                /// <param name="names">名称集合</param>
                /// <param name="isStaticUnmanaged">是否固定内存申请</param>
                public builder(keyValue<string, int>[] names, bool isStaticUnmanaged)
                {
                    this.names = names;
                    prefixSize = tableCount = stateCount = tableType = 0;
                    state = charsAscii = charStart = charEnd = prefix = table = null;
                    if (names.Length > 1)
                    {
                        chars = new list<char>();
                        Data = new pointer.size();
                        count(0, names.Length, 0);
                        char[] charArray = chars.array;
                        int charCount, asciiCount;
                        Array.Sort(charArray, 0, chars.length);
                        fixed (char* charFixed = charArray)
                        {
                            char* start = charFixed + 1, end = charFixed + chars.length, write = start;
                            char value = *charFixed;
                            if (*(end - 1) < 128)
                            {
                                while (start != end)
                                {
                                    if (*start != value) *write++ = value = *start;
                                    ++start;
                                }
                                asciiCount = (int)(write - charFixed);
                                charCount = 0;
                            }
                            else
                            {
                                while (value < 128)
                                {
                                    while (*start == value) ++start;
                                    *write++ = value = *start++;
                                }
                                asciiCount = (int)(write - charFixed) - 1;
                                while (start != end)
                                {
                                    if (*start != value) *write++ = value = *start;
                                    ++start;
                                }
                                charCount = (int)(write - charFixed) - asciiCount;
                            }
                            chars.UnsafeAddLength(asciiCount + charCount - chars.length);
                            int size = (1 + (stateCount += tableCount) * 3) * sizeof(int) + (128 + 2 + charCount + prefixSize) * sizeof(ushort);
                            if (stateCount < 256) size += tableCount * (chars.length + 1);
                            else if (stateCount < 65536)
                            {
                                size += tableCount * (chars.length + 1) * sizeof(ushort);
                                tableType = 1;
                            }
                            else
                            {
                                size += tableCount * (chars.length + 1) * sizeof(int);
                                tableType = 2;
                            }
                            Data = unmanaged.Get(size, true, isStaticUnmanaged);
                            *Data.Int = stateCount;//状态数量[int]
                            state = Data.Byte + sizeof(int);//状态集合[stateCount*(前缀位置[int]+状态位置[int]+名称索引[int])]
                            charsAscii = state + (stateCount * 3) * sizeof(int);//ascii字符查找表[128*ushort]
                            charStart = charsAscii + 128 * sizeof(ushort);
                            *(ushort*)charStart = (ushort)(asciiCount + 1);//特殊字符起始值[ushort]
                            *(ushort*)(charStart + sizeof(ushort)) = (ushort)charCount;//特殊字符数量[ushort]
                            charStart += sizeof(ushort) * 2;
                            ushort charIndex = 0;
                            for (start = charFixed, end = charFixed + asciiCount; start != end; ++start)
                            {
                                *(ushort*)(charsAscii + (*start << 1)) = ++charIndex;
                            }
                            charEnd = charStart;
                            if (charCount != 0)
                            {//特殊字符二分查找表[charCount*char]
                                fastCSharp.unsafer.memory.Copy((byte*)start, charStart, charCount << 1);
                                charEnd += charCount << 1;
                            }
                            prefix = charStart + charCount * sizeof(ushort);//前缀集合
                            table = prefix + prefixSize * sizeof(ushort);//状态矩阵[tableCount*(chars.Count+1)*[byte/ushort/int]]
                        }
                        stateCount = 0;
                        create(0, names.Length, 0);
                    }
                    else
                    {
                        chars = null;
                        if (names.Length == 0) Data = new pointer.size();
                        else if (names[0].Key.Length <= 128)
                        {
                            Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 * sizeof(ushort) + 2 * sizeof(ushort), false, isStaticUnmanaged);
                            *Data.Int = 1;//状态数量
                            state = Data.Byte + sizeof(int);
                            *(int*)state = sizeof(int) * 3;//前缀位置
                            *(int*)(state + sizeof(int)) = 0;//状态位置
                            *(int*)(state + sizeof(int) * 2) = names[0].Value;//名称索引
                            prefix = Data.Byte + sizeof(int) * 4;
                            fastCSharp.unsafer.String.SimpleCopy(names[0].Key, prefix);
                            *(char*)(prefix + (names[0].Key.Length << 1)) = (char)0;
                            *(int*)(Data.Byte + sizeof(int) * 4 + 128 * sizeof(ushort)) = 0;
                        }
                        else
                        {
                            Data = unmanaged.Get(sizeof(int) + sizeof(int) * 3 + 128 * sizeof(ushort) + 2 * sizeof(ushort) + names[0].Key.Length * sizeof(char) + sizeof(char), true, isStaticUnmanaged);
                            *Data.Int = 1;//状态数量
                            state = Data.Byte + sizeof(int);
                            *(int*)state = sizeof(int) * 3 + 128 * sizeof(ushort) + 2 * sizeof(ushort);//前缀位置
                            *(int*)(state + sizeof(int)) = 0;//状态位置
                            *(int*)(state + sizeof(int) * 2) = names[0].Value;//名称索引
                            fastCSharp.unsafer.String.SimpleCopy(names[0].Key, state + *(int*)state);
                        }
                    }
                }
                /// <summary>
                /// 计算状态数量
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void count(int start, int end, int current)
                {
                    ++tableCount;
                    int index = start, prefixSize = 0;
                    char value = (char)0;
                    while (names[start].Key.Length != current)
                    {
                        value = names[start].Key[current];
                        while (++index != end && names[index].Key[current] == value) ;
                        if (index != end) break;
                        ++prefixSize;
                        index = start;
                        ++current;
                    }
                    if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            if (value == 0) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                            chars.Add(value);
                            if (count == 1)
                            {
                                ++stateCount;
                                prefixSize = names[start].Key.Length - current - 1;
                                if (prefixSize != 0) this.prefixSize += prefixSize + 1;
                            }
                            else this.count(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = names[start = index].Key[current];
                        while (++index != end && names[index].Key[current] == value) ;
                    }
                    while (true);
                }
                /// <summary>
                /// 创建状态数据
                /// </summary>
                /// <param name="start">起始名称位置</param>
                /// <param name="end">结束名称位置</param>
                /// <param name="current"></param>
                private void create(int start, int end, int current)
                {
                    byte* prefix = this.prefix, table = this.table;
                    *(int*)(state + sizeof(int)) = (int)(table - state);
                    int index = start;
                    char value = (char)0;
                    if (names[start].Key.Length == current) *(int*)(state + sizeof(int) * 2) = names[start].Value;
                    else
                    {
                        do
                        {
                            value = names[index].Key[current];
                            while (++index != end && names[index].Key[current] == value) ;
                            if (index != end)
                            {
                                *(int*)(state + sizeof(int) * 2) = -1;
                                break;
                            }
                            *(char*)this.prefix = value;
                            this.prefix += 2;
                            if (names[index = start].Key.Length == ++current)
                            {
                                *(int*)(state + sizeof(int) * 2) = names[index].Value;
                                break;
                            }
                        }
                        while (true);
                    }
                    if (prefix == this.prefix) *(int*)state = (int)(charsAscii - state);
                    else
                    {
                        *(char*)this.prefix = (char)0;
                        *(int*)state = (int)(prefix - state);
                        this.prefix += 2;
                    }
                    state += sizeof(int) * 3;
                    ++stateCount;
                    if (tableType == 0) this.table += chars.length + 1;
                    else if (tableType == 1) this.table += (chars.length + 1) * sizeof(ushort);
                    else this.table += (chars.length + 1) * sizeof(int);
                    do
                    {
                        int count = index - start;
                        if (count == 0) index = ++start;
                        else
                        {
                            int charIndex;
                            if (value < 128) charIndex = (int)*(ushort*)(charsAscii + (value << 1));
                            else
                            {
                                char* charStart = (char*)this.charStart, charEnd = (char*)this.charEnd, charCurrent = charStart + ((int)(charEnd - charStart) >> 1);
                                while (*charCurrent != value)
                                {
                                    if (value < *charCurrent) charEnd = charCurrent;
                                    else charStart = charCurrent + 1;
                                    charCurrent = charStart + ((int)(charEnd - charStart) >> 1);
                                }
                                charIndex = (int)*(ushort*)(this.charStart - sizeof(int)) + (int)(charCurrent - (char*)this.charStart);
                            }
                            if (tableType == 0) *(table + charIndex) = (byte)stateCount;
                            else if (tableType == 1) *(ushort*)(table + charIndex * sizeof(ushort)) = (ushort)stateCount;
                            else *(int*)(table + charIndex * sizeof(int)) = stateCount * 3 * sizeof(int);
                            if (count == 1)
                            {
                                int prefixSize = names[start].Key.Length - current - 1;
                                if (prefixSize == 0) *(int*)state = (int)(charsAscii - state);
                                else
                                {
                                    *(int*)state = (int)(this.prefix - state);
                                    fixed (char* charFixed = names[start].Key)
                                    {
                                        fastCSharp.unsafer.memory.SimpleCopy((byte*)(charFixed + current + 1), this.prefix, prefixSize <<= 1);
                                        *(char*)(this.prefix += prefixSize) = (char)0;
                                        this.prefix += sizeof(char);
                                    }
                                }
                                *(int*)(state + sizeof(int) * 2) = names[start].Value;
                                ++stateCount;
                                state += sizeof(int) * 3;
                            }
                            else create(start, index, current + 1);
                        }
                        if (index == end) break;
                        value = names[start = index].Key[current];
                        while (++index != end && names[index].Key[current] == value) ;
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 创建名称查找数据
            /// </summary>
            /// <param name="names">名称集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            /// <returns>名称查找数据</returns>
            public static pointer.size Create(string[] names, bool isStaticUnmanaged)
            {
                int index = 0;
                keyValue<string, int>[] strings = new keyValue<string, int>[names.Length];
                foreach (string name in names)
                {
                    strings[index].Set(name, index);
                    ++index;
                }
                strings = strings.sort(stringCompare);
                return new builder(check(strings) ? strings : nullValue<keyValue<string, int>>.Array, isStaticUnmanaged).Data;
            }
        }
        /// <summary>
        /// 字符搜索器
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        public sealed class chars<valueType> : IDisposable
        {
            /// <summary>
            /// 状态搜索数据
            /// </summary>
            private pointer.size data;
            /// <summary>
            /// 状态数据集合
            /// </summary>
            private valueType[] values;
            /// <summary>
            /// 是否固定内存申请
            /// </summary>
            private bool isStaticUnmanaged;
            /// <summary>
            /// ASCII字节状态搜索
            /// </summary>
            /// <param name="states">状态集合</param>
            /// <param name="values">状态数据集合</param>
            /// <param name="isStaticUnmanaged">是否固定内存申请</param>
            public chars(string[] states, valueType[] values, bool isStaticUnmanaged)
            {
                if (states == null || values == null) log.Default.Throw(log.exceptionType.Null);
                if (states.Length > values.Length) log.Default.Throw(log.exceptionType.IndexOutOfRange);
                this.values = values;
                data = charsSearcher.Create(states, this.isStaticUnmanaged = isStaticUnmanaged);
            }
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(string state, valueType nullValue = default(valueType))
            {
                int index = new charsSearcher(ref data).Search(state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 判断是否存在状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <returns>是否存在状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool ContainsKey(string state)
            {
                return new charsSearcher(ref data).Search(state) >= 0;
            }
            ///// <summary>
            ///// 删除状态数据
            ///// </summary>
            ///// <param name="state">查询状态</param>
            ///// <returns>是否存在状态数据</returns>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //public bool Remove(string state)
            //{
            //    return new chars(data).Remove(state) >= 0;
            //}
            /// <summary>
            /// 获取状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <param name="nullValue">默认空值</param>
            /// <returns>状态数据,失败返回默认空值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Get(ref subString state, valueType nullValue = default(valueType))
            {
                int index = new charsSearcher(ref data).Search(ref state);
                return index >= 0 ? values[index] : nullValue;
            }
            /// <summary>
            /// 判断是否存在状态数据
            /// </summary>
            /// <param name="state">查询状态</param>
            /// <returns>是否存在状态数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool ContainsKey(ref subString state)
            {
                return new charsSearcher(ref data).Search(ref state) >= 0;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (isStaticUnmanaged) unmanaged.FreeStatic(ref data);
                else unmanaged.Free(ref data);
                values = null;
            }
        }
    }
}
