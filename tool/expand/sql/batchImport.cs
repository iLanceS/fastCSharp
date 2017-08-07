using System;
using fastCSharp;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.sql
{
    /// <summary>
    /// 批量导入数据
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    public sealed class batchImport<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
        /// <summary>
        /// 导入数据
        /// </summary>
        private Action importHandle;
        /// <summary>
        /// 导入数据队列集合
        /// </summary>
        private collection<valueType[]> queues;
        /// <summary>
        /// 被释放的数据队列集合
        /// </summary>
        private subArray<valueType[]> freeQueues;
        /// <summary>
        /// 导入数据队列访问锁
        /// </summary>
        private readonly object queueLock = new object();
        /// <summary>
        /// 最后一个数据队列可导入数据数量
        /// </summary>
        private int freeCount;
        /// <summary>
        /// 最大单次导入数量
        /// </summary>
        private int maxSize;
        /// <summary>
        /// 是否正在数据导入状态
        /// </summary>
        private byte isImport;
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="sqlTool">数据库表格操作工具</param>
        /// <param name="maxSize">最大单次导入数量</param>
        public batchImport(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int maxSize)
        {
            if (sqlTool == null) log.Error.Throw(log.exceptionType.Null);
            importHandle = import;
            this.maxSize = Math.Max(maxSize, 1);
            this.sqlTool = sqlTool;
            queues = new collection<valueType[]>(4);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isTask">是否任务方式</param>
        public void Append(ref subArray<valueType> values, bool isTask = false)
        {
            append(values.UnsafeArray, values.StartIndex, values.Count, isTask);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isTask">是否任务方式</param>
        public void Append(valueType[] values, bool isTask = false)
        {
            if (values != null) append(values, 0, values.Length, isTask);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="isTask">是否任务方式</param>
        private void append(valueType[] values, int index, int count, bool isTask)
        {
            Monitor.Enter(queueLock);
            byte isImport = this.isImport;
            this.isImport = 1;
            try
            {
                if (freeCount != 0)
                {
                    valueType[][] queueArray = queues.UnsafeArray;
                    int queueIndex = queues.UnsafeLastIndex;
                    if (count == 1)
                    {
                        queueArray[queueIndex][maxSize - freeCount] = values[index];
                        count = 0;
                        --freeCount;
                    }
                    else if (count < freeCount)
                    {
                        Array.Copy(values, index, queueArray[queueIndex], maxSize - freeCount, count);
                        freeCount -= count;
                        count = 0;
                    }
                    else
                    {
                        Array.Copy(values, index, queueArray[queueIndex], maxSize - freeCount, freeCount);
                        index += freeCount;
                        count -= freeCount;
                        freeCount = 0;
                    }
                }
                if (count != 0)
                {
                    valueType[] queue = freeQueues.Count == 0 ? new valueType[maxSize] : freeQueues.UnsafePop();
                    if (count == 1) queue[0] = values[index];
                    else
                    {
                        while (count > maxSize)
                        {
                            Array.Copy(values, index, queue, 0, maxSize);
                            queues.Add(queue);
                            index += maxSize;
                            count -= maxSize;
                            queue = freeQueues.Count == 0 ? new valueType[maxSize] : freeQueues.UnsafePop();
                        }
                        Array.Copy(values, index, queue, 0, count);
                    }
                    queues.Add(queue);
                    freeCount = maxSize - count;
                }
            }
            finally
            {
                Monitor.Exit(queueLock);
                if (isImport == 0)
                {
                    if (isTask) fastCSharp.threading.task.Tiny.Add(importHandle);
                    else import();
                }
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isTask">是否任务方式</param>
        public void Append(valueType value, bool isTask)
        {
            Monitor.Enter(queueLock);
            byte isImport = this.isImport;
            this.isImport = 1;
            try
            {
                if (freeCount == 0)
                {
                    valueType[] queue = freeQueues.Count == 0 ? new valueType[maxSize] : freeQueues.UnsafePop();
                    queue[0] = value;
                    queues.Add(queue);
                    freeCount = maxSize - 1;
                }
                else
                {
                    queues.UnsafeArray[queues.UnsafeLastIndex][maxSize - freeCount] = value;
                    --freeCount;
                }
            }
            finally
            {
                Monitor.Exit(queueLock);
                if (isImport == 0)
                {
                    if (isTask) fastCSharp.threading.task.Tiny.Add(importHandle);
                    else import();
                }
            }
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        private void import()
        {
            subArray<valueType> values = default(subArray<valueType>);
            do
            {
                int count;
                Monitor.Enter(queueLock);
                if (queues.Count == 0)
                {
                    isImport = 0;
                    Monitor.Exit(queueLock);
                    return;
                }
                valueType[] queue = queues.UnsafePopExpand();
                if (queues.Count == 0)
                {
                    count = maxSize - freeCount;
                    freeCount = 0;
                }
                else count = maxSize;
                Monitor.Exit(queueLock);
                try
                {
                    if (count == 1)
                    {
                        sqlTool.Insert(queue[0]);
                        queue[0] = null;
                    }
                    else
                    {
                        values.UnsafeSet(queue, 0, count);
                        sqlTool.Insert(ref values);
                        Array.Clear(queue, 0, count);
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    Array.Clear(queue, 0, count);
                }
                Monitor.Enter(queueLock);
                try
                {
                    freeQueues.Add(queue);
                }
                finally { Monitor.Exit(queueLock); }
            }
            while (true);
        }
    }
}
