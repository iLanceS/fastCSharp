using System;
using fastCSharp;

namespace fastCSharp.test.uniqueHashCode
{
    /// <summary>
    /// 生成唯一HASH代码
    /// </summary>
    class coder
    {
        /// <summary>
        /// HASH关键字集合(字符串模式)
        /// </summary>
        private string[] keys;
        /// <summary>
        /// 32位HASH计算中每个数据需要减掉的数据最小值
        /// </summary>
        private int sub;
        /// <summary>
        /// 32位HASH计算中每个数据的有效2进制位数
        /// </summary>
        private int bit;
        /// <summary>
        /// 最优HASH值2进制位数
        /// </summary>
        private int minSize;
        /// <summary>
        /// 是否忽略最小HASH值结果，使用空间可以节省一次减法运算
        /// </summary>
        private bool isMaxCode;

        /// <summary>
        /// 关键字最大长度
        /// </summary>
        private int minKeyLength;
        /// <summary>
        /// 关键字数量的2进制长度
        /// </summary>
        private int keySize;

        /// <summary>
        /// HASH关键字集合(整数模式)
        /// </summary>
        private uint[] intKeys;
        /// <summary>
        /// 32位HASH值集合
        /// </summary>
        private uint[] codes;
        /// <summary>
        /// 可选择的数据位置信息全枚举集合
        /// </summary>
        private index[] selectIndexs;
        /// <summary>
        /// 32位HASH值数据位置信息集合，长度为0表示使用 fastCSharp.algorithm.hashCode.GetHashCode 计算字符串HASH值
        /// </summary>
        private index[] indexs;
        /// <summary>
        /// 计算最终HASH值时的移位信息集合
        /// </summary>
        private int[] shifts;
        /// <summary>
        /// 当前HASH值的移位信息集合
        /// </summary>
        private int[] currentShifts;
        /// <summary>
        /// 当前HASH值数据位置信息集合
        /// </summary>
        private index[] currentIndexs;
        /// <summary>
        /// 当前枚举HASH组合元素数量
        /// </summary>
        private int currentCount;
        /// <summary>
        /// 最小HASH值范围大小
        /// </summary>
        private uint minLength = uint.MaxValue;
        /// <summary>
        /// 最小HASH值
        /// </summary>
        private uint minCode;
        /// <summary>
        /// 最大HASH值
        /// </summary>
        private uint maxCode = uint.MaxValue;
        /// <summary>
        /// 未命中HASH值，可以做缺省默认值
        /// </summary>
        private uint lessCode;
        /// <summary>
        /// 最少运算次数
        /// </summary>
        private uint minTime = uint.MaxValue;
        /// <summary>
        /// 当前运算次数
        /// </summary>
        private uint currentTime;
        /// <summary>
        /// 生成唯一HASH代码
        /// </summary>
        /// <param name="keys">HASH关键字集合</param>
        /// <param name="bit">32位HASH计算中每个数据的有效2进制位数</param>
        /// <param name="sub">32位HASH计算中每个数据需要减掉的数据最小值</param>
        /// <param name="minSize">HASH值2进制位数初始值</param>
        /// <param name="isMaxCode">是否忽略最小HASH值结果，使用空间可以节省一次减法运算</param>
        public coder(string[] keys, int bit, int sub = 0, int minSize = 16, bool isMaxCode = true)
        {
            this.keys = keys;
            this.bit = bit;
            this.sub = sub;
            this.minSize = minSize;
            this.isMaxCode = isMaxCode;
            minKeyLength = keys.minKey(value => value.Length, int.MaxValue);
            intKeys = new uint[keys.Length];
            codes = new uint[keys.Length];
            int shiftSize = keySize = ((uint)intKeys.Length).bits();
            if ((1 << keySize) == intKeys.Length) --keySize;

            selectIndexs = new index[(minKeyLength << 1) + shiftSize];
            int index = 0;
            for (int length = 0; length != minKeyLength; ++length)
            {
                selectIndexs[index++] = new index { Type = uniqueHashCode.index.type.Left, Index = length };
                selectIndexs[index++] = new index { Type = uniqueHashCode.index.type.Right, Index = length };
            }
            for (int size = shiftSize; size != 0; --size)
            {
                selectIndexs[index++] = new index { Type = uniqueHashCode.index.type.Shift, Index = size };
            }

            for (int count = 1; count != 4; ++count) if (isHash(count)) break;
            if (indexs == null)
            {
                int keyIndex = 0;
                foreach (string key in keys) intKeys[keyIndex++] = (uint)fastCSharp.algorithm.hashCode.GetHashCode(System.Text.Encoding.Unicode.GetBytes(key.ToString()));
                for (int count = 1; count != 4; ++count) if (checkHashCode(count)) break;
            }
        }
        /// <summary>
        /// 生成唯一HASH代码
        /// </summary>
        /// <param name="intKeys">HASH关键字集合</param>
        /// <param name="bit">32位HASH计算中每个数据的有效2进制位数</param>
        /// <param name="sub">32位HASH计算中每个数据需要减掉的数据最小值</param>
        /// <param name="minSize">HASH值2进制位数初始值</param>
        /// <param name="isMaxCode">是否忽略最小HASH值结果，使用空间可以节省一次减法运算</param>
        public coder(uint[] intKeys, int bit, int sub = 0, int minSize = 16, bool isMaxCode = true)
        {
            this.intKeys = intKeys;
            this.bit = bit;
            this.sub = sub;
            this.minSize = minSize;
            this.isMaxCode = isMaxCode;
            codes = new uint[intKeys.Length];
            for (int count = 1; count != 4; ++count) if (checkHashCode(count)) break;
        }
        /// <summary>
        /// 计算HASH值相关信息
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool isHash(int count)
        {
            currentShifts = new int[currentCount = count];
            currentIndexs = new uniqueHashCode.index[count];
            int[] points = new int[count];
            for (int index = count; index != 0; currentIndexs[--index] = selectIndexs[0]) ;
            indexStack[] indexChecks = new indexStack[(int)uniqueHashCode.index.type.Shift + 1];
            for (int index = indexChecks.Length; index != 0; indexChecks[--index] = new indexStack(count)) ;
            bool isNext = true;
            do
            {
                bool isIndex = true;
                for (int index = indexChecks.Length; index != 0; indexChecks[--index].Reset()) ;
                currentTime = 0;
                foreach (uniqueHashCode.index index in currentIndexs)
                {
                    if (indexChecks[(int)index.Type].Check(index.Index))
                    {
                        isIndex = false;
                        break;
                    }
                    if (index.Type != index.type.Left) ++currentTime;
                }
                if (isIndex)
                {
                    int keyIndex = 0;
                    foreach (string key in keys)
                    {
                        uint code = 0;
                        for (int bit = 0, index = count; index != 0; bit += this.bit)
                        {
                            --index;
                            code += (uint)((currentIndexs[index].GetChar(key) - sub) << bit);
                        }
                        intKeys[keyIndex++] = code;
                    }
                    if (isUnique(intKeys.sort()) && isHash())
                    {
                        indexs = currentIndexs.copy();
                        shifts = currentShifts.copy();
                    }
                }
                int pointIndex = count - 1;
                ++points[pointIndex];
                while (points[pointIndex] == selectIndexs.Length)
                {
                    points[pointIndex] = 0;
                    if (pointIndex == 0) isNext = false;
                    else
                    {
                        currentIndexs[pointIndex] = selectIndexs[0];
                        ++points[--pointIndex];
                    }
                }
                currentIndexs[pointIndex] = selectIndexs[points[pointIndex]];
            }
            while (isNext);
            return indexs != null;
        }
        /// <summary>
        /// 判断是否可HASH处理
        /// </summary>
        /// <returns></returns>
        private bool isHash()
        {
            bool isHash = false;
            int[] shifts = new int[currentCount];
            for (int shift, index = currentCount; index != 0; shifts[--index] = shift) shift = currentCount - index;
            bool isNext = true;
            do
            {
                int codeIndex = 0;
                foreach (uint key in intKeys)
                {
                    int index = currentCount;
                    uint code = key >> shifts[--index];
                    while (index != 0) code ^= key >> shifts[--index];
                    codes[codeIndex++] = code & ((1U << minSize) - 1);
                }
                if (isUnique(codes.sort()) && isShift())
                {
                    isHash = true;
                    Array.Copy(shifts, 0, currentShifts, 0, currentCount);
                }
                int shiftIndex = currentCount - 1;
                ++shifts[shiftIndex];
                while (shifts[shiftIndex] >= bit * (currentCount - shiftIndex) || (shiftIndex != 0 && shifts[shiftIndex] >= shifts[shiftIndex - 1]))
                {
                    if (shiftIndex == 0)
                    {
                        isNext = false;
                        break;
                    }
                    shifts[shiftIndex] = currentCount - shiftIndex - 1;
                    ++shifts[--shiftIndex];
                }
            }
            while (isNext);
            return isHash;
        }
        /// <summary>
        /// 判断是否可成功移位处理
        /// </summary>
        /// <returns></returns>
        private bool isShift()
        {
            bool isShift = false;
            int size = minSize;
            do
            {
                uint min = codes[0], max = codes[codes.Length - 1], length = max - min, time = currentTime;
                if (!isMaxCode && min != 0) ++time;
                if (isMaxCode ? (max == maxCode) : (length == minLength))
                {
                    if (time >= minTime)
                    {
                        if (isMaxCode) ++max;
                        else ++length;
                    }
                }
                if (isMaxCode ? (max < maxCode) : (length < minLength))
                {
                    minCode = min;
                    maxCode = max;
                    minLength = length;
                    minSize = size;
                    minTime = time;
                    this.lessCode = max + 1;
                    uint lessCode = uint.MaxValue;
                    foreach (uint code in codes)
                    {
                        if (code != ++lessCode)
                        {
                            this.lessCode = lessCode;
                            break;
                        }
                    }
                    isShift = true;
                }
                if (--size >= keySize)
                {
                    uint and = (1U << size) - 1;
                    for (int index = codes.Length; index != 0; codes[--index] &= and) ;
                }
                else break;
            }
            while (isUnique(codes.sort()));
            return isShift;
        }
        /// <summary>
        /// 计算HASH值(整数模式)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool checkHashCode(int count)
        {
            currentShifts = new int[currentCount = count];
            if (isUnique(intKeys.sort()))
            {
                int hashBit = bit;
                do
                {
                    if (isHash())
                    {
                        indexs = fastCSharp.nullValue<index>.Array;
                        shifts = currentShifts.copy();
                        hashBit = bit;
                    }
                }
                while (++bit <= 10);
                bit = hashBit;
            }
            return indexs != null;
        }
        /// <summary>
        /// 获取C#计算哈希代码
        /// </summary>
        /// <param name="keyName">关键字变量名称</param>
        /// <returns>C#计算哈希代码</returns>
        public unsafe string Code(string keyName = "key")
        {
            if (indexs == null) return string.Empty;
            using (unmanagedStream code = new unmanagedStream())
            {
                if (indexs.Length == 0)
                {
                    code.Write(@"fixed (char* keyFixed = ");
                    code.Write(keyName);
                    code.Write(@")
{
");
                }
                code.Write("    uint code = ");
                bool isFirst = true;
                if (indexs.Length == 0)
                {
                    code.Write("(uint)fastCSharp.algorithm.hashCode.GetHashCode(keyFixed, ");
                    code.Write(keyName);
                    code.Write(@" << 1)");
                }
                else
                {
                    int shiftBit = bit * (indexs.Length - 1);
                    foreach (index index in indexs)
                    {
                        if (isFirst) isFirst = false;
                        else code.Write(" + ");
                        code.Write("(uint)");
                        if (shiftBit != 0) code.Write('(');
                        if (sub != 0) code.Write('(');
                        code.Write(keyName);
                        code.Write('[');
                        switch (index.Type)
                        {
                            case index.type.Left:
                                code.Write(index.Index.toString());
                                break;
                            case index.type.Right:
                                code.Write(keyName);
                                code.Write(".Length - ");
                                code.Write((index.Index + 1).toString());
                                break;
                            default:
                                code.Write(keyName);
                                code.Write(".Length >> ");
                                code.Write(index.Index.toString());
                                break;
                        }
                        code.Write(']');
                        if (sub != 0)
                        {
                            code.Write(" - ");
                            code.Write(sub.toString());
                            code.Write(')');
                        }
                        if (shiftBit != 0)
                        {
                            code.Write(" << ");
                            code.Write(shiftBit.toString());
                            code.Write(')');
                            shiftBit -= bit;
                        }
                    }
                }
                code.Write(@";
    return (int)((");
                isFirst = true;
                foreach (int shift in shifts)
                {
                    if (isFirst) isFirst = false;
                    else code.Write(" ^ ");
                    if (shift != 0) code.Write('(');
                    code.Write("code");
                    if (shift != 0)
                    {
                        code.Write(" >> ");
                        code.Write(shift.toString());
                        code.Write(')');
                    }
                }
                code.Write(") & ((1U << ");
                code.Write(minSize.toString());
                code.Write(") - 1))");
                if (!isMaxCode && minCode != 0)
                {
                    code.Write(" - ");
                    code.Write(minCode.toString());
                }
                code.Write(';');
                if (indexs.Length == 0)
                {
                    code.Write(@"
}");
                }
                return code.ToString();
            }
        }
        /// <summary>
        /// 判断最后添加的HASH值是否重复
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        private static bool isUnique(uint[] codes)
        {
            int index = codes.Length;
            uint lastCode = codes[--index];
            while (index != 0)
            {
                uint code = codes[--index];
                if (lastCode == code) return false;
                lastCode = code;
            }
            return true;
        }
        /// <summary>
        /// 唯一HASH信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (indexs == null) return string.Empty;
            return "indexs[" + indexs.joinString(',', value => value.ToString()) + "] sub[" + sub.toString() + "] bit[" + bit.toString() + "] shifts[" + shifts.joinString(',') + "] size[" + minSize.toString() + "] min[" + minCode.toString() + "] max[" + maxCode.toString() + "] key[" + minKeyLength.toString() + "] less[" + lessCode.toString() + "] " + (minLength).toString();
        }
    }
}
