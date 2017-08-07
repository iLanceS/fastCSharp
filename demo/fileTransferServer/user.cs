using System;
using fastCSharp.threading;
using fastCSharp.code;
using System.Collections.Generic;

namespace fastCSharp.demo.fileTransferServer
{
    /// <summary>
    /// 用户
    /// </summary>
    internal partial class user
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1)]
        public string Name;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 验证时间
        /// </summary>
        public DateTime VerifyTime = date.NowSecond;

        /// <summary>
        /// 权限集合
        /// </summary>
        [fastCSharp.code.ignore]
        internal Dictionary<hashString, permission> PermissionCache;
        /// <summary>
        /// 用户登录访问锁
        /// </summary>
        private static int loginLock;
        /// <summary>
        /// 内存数据库表格
        /// </summary>
        internal static readonly fastCSharp.emit.memoryDatabaseModelTable<user, string> Table = new emit.memoryDatabaseModelTable<user, string>(new fastCSharp.memoryDatabase.cache.dictionary<user, string>());
        /// <summary>
        /// 成员选择+验证时间
        /// </summary>
        private static readonly memberMap<user> updateVerifyTimeMember = Table.CreateMemberMap().Append(value => value.VerifyTime);
        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="user">登录用户</param>
        /// <returns>是否成功</returns>
        internal bool LoginVerify(string name, byte[] password, DateTime verifyTime)
        {
            if (verifyTime > VerifyTime && password.equal(server.Md5Password(Password, verifyTime)))
            {
                bool isTime = false;
                interlocked.CompareSetYield(ref loginLock);
                if (verifyTime > VerifyTime)
                {
                    VerifyTime = verifyTime;
                    isTime = true;
                }
                loginLock = 0;
                if (isTime) return Table.Update(this, updateVerifyTimeMember) != null;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
