using System;
using System.Threading;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 自增缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    public abstract class identity<valueType, memberType> : loadCache<valueType, memberType, int>
        where valueType : class, fastCSharp.data.IPrimaryKey<int>
        where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        protected valueLock[] array;
        /// <summary>
        /// 对象数量
        /// </summary>
        protected int count;
        /// <summary>
        /// 当前自增值
        /// </summary>
        protected int currentIdentity;
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        public int NextIdentity()
        {
            waitLoad();
            return Interlocked.Increment(ref currentIdentity);
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        protected override void loaded()
        {
            if (array == null) array = new valueLock[identityArrayLength];
        }
    }
}
