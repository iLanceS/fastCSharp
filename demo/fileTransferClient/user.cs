using System;
using fastCSharp.code;
using System.Collections.Generic;

namespace fastCSharp.demo.fileTransferClient
{
    /// <summary>
    /// 客户端用户
    /// </summary>
    internal partial class user
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public int Id;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 主机名
        /// </summary>
        public string Host;
        /// <summary>
        /// 服务端口
        /// </summary>
        public ushort Port;
        /// <summary>
        /// 使用次数
        /// </summary>
        public uint UseCount;

        /// <summary>
        /// 自动匹配路径集合
        /// </summary>
        [fastCSharp.code.ignore]
        internal Dictionary<hashString, autoPath> AutoPathCache;
        /// <summary>
        /// 内存数据库表格
        /// </summary>
        internal static readonly fastCSharp.emit.memoryDatabaseModelTable<user> Table = new emit.memoryDatabaseModelTable<user>(new fastCSharp.memoryDatabase.cache.identityArray<user>());
        /// <summary>
        /// 成员选择+密码
        /// </summary>
        private static readonly memberMap<user> updatePasswordMember = Table.CreateMemberMap().Append(value => value.Password);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="password">密码</param>
        public void ReworkPassword(string password)
        {
            user user = (user)this.MemberwiseClone();
            user.Password = password;
            Table.Update(user, updatePasswordMember);
        }
        /// <summary>
        /// 成员选择+使用次数
        /// </summary>
        private static readonly memberMap<user> updateUseCountMember = Table.CreateMemberMap().Append(value => value.UseCount);
        /// <summary>
        /// 增加使用次数
        /// </summary>
        public void IncUserCount()
        {
            ++UseCount;
            Table.Update(this, updateUseCountMember);
        }
        /// <summary>
        /// 确认是否同一用户
        /// </summary>
        /// <param name="other">用户</param>
        /// <returns>是否同一用户</returns>
        public bool Equals(user other)
        {
            return other != null && other.Port == Port && other.Host == Host && other.Name == Name;
        }
        /// <summary>
        /// 获取HASH值
        /// </summary>
        /// <returns>HASH值</returns>
        public override int GetHashCode()
        {
            return Host.GetHashCode() ^ Port ^ Name.GetHashCode();
        }
        /// <summary>
        /// 确认是否同一用户
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            user user = obj as user;
            return user != null && Equals(user);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Host + ":" + Port.toString() + " " + Name;
        }
    }
}
