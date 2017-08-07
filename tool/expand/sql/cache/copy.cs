using System;
using System.Reflection;
using fastCSharp.code.cSharp;
using fastCSharp.reflection;
using System.Threading;

namespace fastCSharp.sql.cache
{
    /// <summary>
    /// SQL操作缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class copy<valueType, modelType> : sqlTool<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 更新缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        /// <param name="newValue">更新后的新数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="updateMemberMap">更新成员位图</param>
        protected void update(valueType value, valueType newValue, valueType oldValue, fastCSharp.code.memberMap<modelType> updateMemberMap)
        {
            using (fastCSharp.code.memberMap<modelType> memberMap = this.memberMap.Copy())
            {
                memberMap.And(updateMemberMap);
                fastCSharp.emit.memberCopyer<modelType>.Copy(value, newValue, memberMap);
                memberMap.Xor(this.memberMap);
                memberMap.And(this.memberMap);
                fastCSharp.emit.memberCopyer<modelType>.Copy(oldValue, value, memberMap);
                fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            }
        }
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        protected copy(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group)
            : base(sqlTool, group)
        {
        }
        /// <summary>
        /// 重新加载缓存事件
        /// </summary>
        public event Action OnReset;
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected void resetLock()
        {
            Monitor.Enter(SqlTool.Lock);
            try
            {
                reset();
                if (OnReset != null) OnReset();
            }
            finally { Monitor.Exit(SqlTool.Lock); }
        }
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected abstract void reset();
    }
}
