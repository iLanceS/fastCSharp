using System;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 自增ID整表排序树缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public sealed unsafe class identityTree<valueType, modelType, memberCacheType> : identityCache<valueType, modelType, memberCacheType>, IDisposable
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 排序树节点数量集合
        /// </summary>
        private pointer.size counts;
        /// <summary>
        /// 排序树容器数量
        /// </summary>
        private int size;
        /// <summary>
        /// 自增ID整表数组缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="memberCache">成员缓存</param>
        /// <param name="group">数据分组</param>
        /// <param name="baseIdentity">基础ID</param>
        public identityTree(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0)
            : base(sqlTool, memberCache, group, baseIdentity, true)
        {
            sqlTool.OnInsertedLock += onInserted;
            sqlTool.OnDeletedLock += onDelete;

            resetLock();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            unmanaged.Free(ref counts);
            size = 0;
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            valueType[] values = SqlTool.Where(null, memberMap).getArray();
            int maxIdentity = values.maxKey(value => GetKey(value), 0);
            if (memberGroup == 0) SqlTool.Identity64 = maxIdentity + baseIdentity;
            int length = maxIdentity >= identityArray.ArraySize ? 1 << ((uint)maxIdentity).bits() : identityArray.ArraySize;
            identityArray<valueType> newValues = new identityArray<valueType>(length);
            pointer.size newCounts = unmanaged.Get(length * sizeof(int), true);
            try
            {
                int* intCounts = newCounts.Int;
                foreach (valueType value in values)
                {
                    setMemberCacheAndValue(value);
                    int identity = GetKey(value);
                    newValues[identity] = value;
                    intCounts[identity] = 1;
                }
                for (int step = 2; step != length; step <<= 1)
                {
                    for (int index = step, countStep = step >> 1; index != length; index += step)
                    {
                        intCounts[index] += intCounts[index - countStep];
                    }
                }
                unmanaged.Free(ref counts);
                this.values = newValues;
                counts = newCounts;
                size = length;
                Count = values.Length;
                newCounts.Null();
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, true);
            }
            finally
            {
                unmanaged.Free(ref newCounts);
            }
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void onInserted(valueType value)
        {
            int* intCounts = counts.Int;
            int identity = GetKey(value);
            if (identity >= size)
            {
                int newLength = int.MaxValue, oldLength = size;
                if ((identity & 0x40000000) == 0 && oldLength != 0x40000000)
                {
                    for (newLength = oldLength << 1; newLength <= identity; newLength <<= 1) ;
                }
                values.ToSize(newLength);
                pointer.size newCounts = unmanaged.Get(newLength * sizeof(int), true);
                try
                {
                    unsafer.memory.Copy(intCounts, newCounts.Int, size * sizeof(int));
                    unmanaged.Free(ref counts);
                    counts = newCounts;
                    size = newLength;
                    newCounts.Null();

                    int index = oldLength, count = (intCounts = counts.Int)[--index];
                    for (int step = 1; (index -= step) != 0; step <<= 1) count += intCounts[index];
                    intCounts[oldLength] = count;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, true);
                }
                finally
                {
                    unmanaged.Free(ref newCounts);
                }
            }
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            setMemberCacheAndValue(newValue);
            values[identity] = newValue;
            for (uint index = (uint)identity, countStep = 1, length = (uint)size; index <= length; countStep <<= 1)
            {
                ++intCounts[index];
                while ((index & countStep) == 0) countStep <<= 1;
                index += countStep;
            }
            ++Count;
            callOnInserted(newValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDelete(valueType value)
        {
            int* intCounts = counts.Int;
            int identity = GetKey(value);
            valueType cacheValue = values[identity];
            --Count;
            for (uint index = (uint)identity, countStep = 1, length = (uint)size; index <= length; countStep <<= 1)
            {
                --intCounts[index];
                while ((index & countStep) == 0) countStep <<= 1;
                index += countStep;
            }
            values[identity] = null;
            callOnDeleted(cacheValue);
        }
        /// <summary>
        /// 获取记录起始位置
        /// </summary>
        /// <param name="skipCount">跳过记录数</param>
        /// <returns>起始位置</returns>
        private int getIndex(int skipCount)
        {
            if (skipCount == 0) return 1;
            int* intCounts = counts.Int;
            int index = size != int.MaxValue ? size >> 1 : 0x40000000, step = index;
            while (intCounts[index] != skipCount)
            {
                step >>= 1;
                if (intCounts[index] < skipCount)
                {
                    skipCount -= intCounts[index];
                    index += step;
                }
                else index -= step;
            }
            return index + 1;
        }
        /// <summary>
        /// 获取分页记录集合
        /// </summary>
        /// <param name="pageSize">分页长度</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">记录总数</param>
        /// <returns>分页记录集合</returns>
        public valueType[] GetPageDesc(int pageSize, int currentPage, out int count)
        {
            Monitor.Enter(SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = Count, pageSize, currentPage);
                valueType[] values = new valueType[page.CurrentPageSize];
                int startIndex = this.getIndex(Count - page.SkipCount - page.CurrentPageSize);
                for (int writeIndex = values.Length, index = startIndex; writeIndex != 0; ++index)
                {
                    valueType value = this.values[index];
                    while (value == null) value = this.values[++index];
                    values[--writeIndex] = value;
                }
                return values;
            }
            finally { Monitor.Exit(SqlTool.Lock); }
        }
        /// <summary>
        /// 获取分页记录集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页长度</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">记录总数</param>
        /// <returns></returns>
        public arrayType[] GetPageDesc<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            if (getValue == null) fastCSharp.log.Default.Throw(fastCSharp.log.exceptionType.Null);
            Monitor.Enter(SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = Count, pageSize, currentPage);
                arrayType[] values = new arrayType[page.CurrentPageSize];
                int startIndex = this.getIndex(Count - page.SkipCount - page.CurrentPageSize);
                for (int writeIndex = values.Length, index = startIndex; writeIndex != 0; ++index)
                {
                    valueType value = this.values[index];
                    while (value == null) value = this.values[++index];
                    values[--writeIndex] = getValue(value);
                }
                return values;
            }
            finally { Monitor.Exit(SqlTool.Lock); }
        }
    }
}
