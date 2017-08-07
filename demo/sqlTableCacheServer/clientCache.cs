using System;

namespace fastCSharp.demo.sqlTableCacheServer
{
    /// <summary>
    /// 数据服务推送客户端缓存
    /// </summary>
    public abstract class clientCache : fastCSharp.sql.logStream.client<clientCache>
    {
        /// <summary>
        /// 数据服务推送客户端缓存初始化访问锁
        /// </summary>
        public static readonly object CacheLock = new object();
        /// <summary>
        /// 学生 客户端缓存
        /// </summary>
        public static fastCSharp.sql.logStream<Student, sqlModel.Student>.identityClient Student = fastCSharp.sql.logStream<Student, sqlModel.Student>.identityClient.Null;
        /// <summary>
        /// 班级 客户端缓存
        /// </summary>
        public static fastCSharp.sql.logStream<Class, sqlModel.Class>.identityClient Class = fastCSharp.sql.logStream<Class, sqlModel.Class>.identityClient.Null;
    }
}
