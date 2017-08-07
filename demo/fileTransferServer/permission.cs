using System;
using System.IO;

namespace fastCSharp.demo.fileTransferServer
{
    /// <summary>
    /// 权限
    /// </summary>
    [fastCSharp.code.cSharp.memoryDatabaseModel]
    internal partial class permission
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1)]
        public string UserName;
        /// <summary>
        /// 操作路径
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 2)]
        public string Path;
        /// <summary>
        /// 备份路径
        /// </summary>
        public string BackupPath;
        /// <summary>
        /// 权限类型
        /// </summary>
        public permissionType Type;
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="file">文件信息</param>
        public void Backup(FileInfo file, int backupIdentity)
        {
            if (BackupPath.Length == 0) file.Delete();
            else
            {
                string fileName = BackupPath + ((ulong)fastCSharp.pub.StartTime.Ticks).toHex16() + "_" + backupIdentity.toString() + "\\" + file.FullName.Substring(Path.Length);
                DirectoryInfo directory = new FileInfo(fileName).Directory;
                if (directory.Exists)
                {
                    if (File.Exists(fileName)) File.Delete(fileName);
                }
                else
                {
                    directory.Create();
                }
                File.Move(file.FullName, fileName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new server.pathPermission { Path = Path, Permission = Type }.ToString();
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// 内存数据库表格
        /// </summary>
        internal static readonly fastCSharp.emit.memoryDatabaseModelTable<permission, primaryKey> Table = new emit.memoryDatabaseModelTable<permission, primaryKey>(new fastCSharp.memoryDatabase.cache.dictionary<permission, primaryKey>());
#endif
        /// <summary>
        /// 用户权限缓存
        /// </summary>
        private static fastCSharp.memoryDatabase.cache.index.memberDictionary<permission, string, hashString, user> userCache;
        /// <summary>
        /// 用户权限缓存
        /// </summary>
        internal static fastCSharp.memoryDatabase.cache.index.memberDictionary<permission, string, hashString, user> UserCache
        {
            get
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                if (userCache == null) userCache = new memoryDatabase.cache.index.memberDictionary<permission, string, hashString, user>(Table.Cache, value => value.UserName, value => user.Table.Cache.Get(value), value => value.PermissionCache, value => value.Path);
#endif
                return userCache;
            }
        }
    }
}
