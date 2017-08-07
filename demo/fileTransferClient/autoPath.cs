using System;
using fastCSharp.code;

namespace fastCSharp.demo.fileTransferClient
{
    /// <summary>
    /// 自动匹配路径
    /// </summary>
    [fastCSharp.code.cSharp.memoryDatabaseModel]//(CacheType = typeof(fastCSharp.memoryDatabase.cache.dictionary<,,>), IsEmbed = true, IsIndexSerialize = true)]
    internal partial class autoPath
    {
        /// <summary>
        /// 客户端用户标识
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1)]
        public int UserId;
        /// <summary>
        /// 服务器端路径
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 2)]
        public string ServerPath;
        /// <summary>
        /// 本地路径
        /// </summary>
        public string LocalPath;
        /// <summary>
        /// 扩展名过滤
        /// </summary>
        public string ExtensionFilter;
        /// <summary>
        /// 内存数据库表格
        /// </summary>
        internal static readonly fastCSharp.emit.memoryDatabaseModelTable<autoPath, primaryKey> Table = new emit.memoryDatabaseModelTable<autoPath, primaryKey>(new fastCSharp.memoryDatabase.cache.dictionary<autoPath, primaryKey>());
        /// <summary>
        /// 成员选择+本地路径,扩展名过滤
        /// </summary>
        private static readonly memberMap<autoPath> updateMember = Table.CreateMemberMap().Append(value => value.LocalPath).Append(value => value.ExtensionFilter);
        /// <summary>
        /// 设置自动匹配路径
        /// </summary>
        /// <param name="userId">客户端用户标识</param>
        /// <param name="serverPath">服务器端路径</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="extensionFilter">扩展名过滤</param>
        public static void Set(int userId, string serverPath, lowerName localPath, string extensionFilter)
        {
            if (user.Table.Cache.Get(userId) != null)
            {
                autoPath autoPath = Table.Cache.Get(new primaryKey { UserId = userId, ServerPath = serverPath });
                if (autoPath == null) Table.Insert(new autoPath { UserId = userId, ServerPath = serverPath, LocalPath = localPath.Name, ExtensionFilter = extensionFilter }, false);
                else if (autoPath.LocalPath.ToLower() != localPath.LowerName || autoPath.ExtensionFilter != extensionFilter)
                {
                    autoPath.LocalPath = localPath.Name;
                    autoPath.ExtensionFilter = extensionFilter;
                    Table.Update(autoPath, updateMember);
                }
            }
        }
        /// <summary>
        /// 客户端用户缓存
        /// </summary>
        private static fastCSharp.memoryDatabase.cache.index.memberDictionary<autoPath, int, hashString, user> userCache;
        /// <summary>
        /// 客户端用户缓存
        /// </summary>
        internal static fastCSharp.memoryDatabase.cache.index.memberDictionary<autoPath, int, hashString, user> UserCache
        {
            get
            {
                if (userCache == null) userCache = new memoryDatabase.cache.index.memberDictionary<autoPath, int, hashString, user>(Table.Cache, value => value.UserId, value => user.Table.Cache.Get(value), value => value.AutoPathCache, value => value.ServerPath);
                return userCache;
            }
        }
    }
}
