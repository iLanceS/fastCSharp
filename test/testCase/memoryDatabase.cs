using System;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using fastCSharp.memoryDatabase.cache;
using fastCSharp.emit;
using fastCSharp.code;

namespace fastCSharp.testCase
{
    /// <summary>
    /// 内存数据库测试
    /// </summary>
    internal partial class memoryDatabase
    {
        /// <summary>
        /// 测试数据
        /// </summary>
        public abstract class data
        {
            public bool Bool;
            public byte Byte;
            public sbyte SByte;
            public short Short;
            public ushort UShort;
            public int Int;
            public uint UInt;
            public long Long;
            public ulong ULong;
            public DateTime DateTime;
            public float Float;
            public double Double;
            public decimal Decimal;
            public Guid Guid;
            public char Char;
            public string String;
            public bool? BoolNull;
            public byte? ByteNull;
            public sbyte? SByteNull;
            public short? ShortNull;
            public ushort? UShortNull;
            public int? IntNull;
            public uint? UIntNull;
            public long? LongNull;
            public ulong? ULongNull;
            public DateTime? DateTimeNull;
            public float? FloatNull;
            public double? DoubleNull;
            public decimal? DecimalNull;
            public Guid? GuidNull;
            public char? CharNull;
        }
        /// <summary>
        /// 内存数据库TCP服务端配置参数
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.tcpServer tcpServer;
        /// <summary>
        /// 
        /// </summary>
        static memoryDatabase()
        {
            tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("memoryDatabasePhysical", typeof(fastCSharp.memoryDatabase.physicalServer));
            tcpServer.Host = "127.0.0.1";
            tcpServer.Port = 12345;
            //tcpServer.MergeStreamSize = 128;
            //tcpServer.ShareMemorySize = 128;
            //tcpServer.IsOnlyShareMemoryClient = true;
        }
        /// <summary>
        /// 自增主键测试
        /// </summary>
        public partial class identity : data
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            public int Id;
            /// <summary>
            /// 缓存
            /// </summary>
            private static identityArray<identity> cache;
            /// <summary>
            /// 测试更新成员位图
            /// </summary>
            private static memberMap<identity> updateMember;
            /// <summary>
            /// 远程模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool RemoteTestCase()
            {
                typeof(fastCSharp.config.pub).GetProperty("IsDebug", BindingFlags.Instance | BindingFlags.Public).SetValue(fastCSharp.config.pub.Default, true, null);
                using (fastCSharp.memoryDatabase.physicalServer.tcpServer server = new fastCSharp.memoryDatabase.physicalServer.tcpServer(tcpServer))
                {
                    if (server.Start())
                    {
                        using (memoryDatabaseModelTable<identity>.remote table = new memoryDatabaseModelTable<identity>.remote(new fastCSharp.memoryDatabase.physicalServer.tcpClient(tcpServer), cache = new identityArray<identity>()))
                        {
                            updateMember = table.CreateMemberMap().Append(value => value.Int);
                            cache.WaitLoad();
                            if (cache.Count == 0)
                            {
                                identity int1 = table.Insert(new identity { Int = 1, String = "A大A" }, false);
                                identity intOld2 = new identity { Int = 2 };
                                identity int2 = table.Insert(intOld2);
                                identity int4 = table.Insert(new identity { Int = 4 });
                                if (int1 == null) return false;
                                if (int2 == null) return false;
                                if (int4 == null) return false;

                                intOld2.Int = 3;
                                identity int3 = table.Update(intOld2, updateMember);
                                if (int3 == null) return false;
                                identity delete = table.Delete(int4.Id);
                                if (delete == null) return false;

                                return check(table, false);
                            }
                            return check(table, true);
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 远程模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<identity>.remote table, bool isDelete)
            {
                subArray<identity> array = cache.Values.getSubArray();
                if (array.Count != 2) return false;
                if (array[0].Int != 1) return false;
                if (array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (identity identity in array) table.Delete(identity.Id);
                }
                table.Flush(false);
                return true;
            }
            /// <summary>
            /// 本地模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool TestCase()
            {
                using (memoryDatabaseModelTable<identity> table = new memoryDatabaseModelTable<identity>(cache = new identityArray<identity>(), memoryDatabaseTable.serializeType.Index, "localIdentity"))
                {
                    updateMember = table.CreateMemberMap().Append(value => value.Int);
                    cache.WaitLoad();
                    if (cache.Count == 0)
                    {
                        identity int1 = table.Insert(new identity { Int = 1, String = "A大A" }, false);
                        identity intOld2 = new identity { Int = 2 };
                        identity int2 = table.Insert(intOld2);
                        identity int4 = table.Insert(new identity { Int = 4 });
                        if (int1 == null) return false;
                        if (int2 == null) return false;
                        if (int4 == null) return false;

                        intOld2.Int = 3;
                        identity int3 = table.Update(intOld2, updateMember);
                        if (int3 == null) return false;
                        identity delete = table.Delete(int4.Id);
                        if (delete == null) return false;

                        return check(table, false);
                    }
                    return check(table, true);
                }
            }
            /// <summary>
            /// 本地模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<identity> table, bool isDelete)
            {
                subArray<identity> array = cache.Values.getSubArray();
                if (array.Count != 2) return false;
                if (array[0].Int != 1) return false;
                if (array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (identity identity in array) table.Delete(identity.Id);
                }
                table.Flush(false);
                return true;
            }
        }
        /// <summary>
        /// 单关键字测试
        /// </summary>
        public partial class primaryKey : data
        {
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1)]
            public int Key;
            /// <summary>
            /// 缓存
            /// </summary>
            private static fastCSharp.memoryDatabase.cache.searchTree<primaryKey, int> cache;
            /// <summary>
            /// 测试更新成员位图
            /// </summary>
            private static memberMap<primaryKey> updateMember;
            /// <summary>
            /// 远程模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool RemoteTestCase()
            {
                typeof(fastCSharp.config.pub).GetProperty("IsDebug", BindingFlags.Instance | BindingFlags.Public).SetValue(fastCSharp.config.pub.Default, true, null);
                using (fastCSharp.memoryDatabase.physicalServer.tcpServer server = new fastCSharp.memoryDatabase.physicalServer.tcpServer(tcpServer))
                {
                    if (server.Start())
                    {
                        using (memoryDatabaseModelTable<primaryKey, int>.remote table = new memoryDatabaseModelTable<primaryKey, int>.remote(new fastCSharp.memoryDatabase.physicalServer.tcpClient(tcpServer), cache = new fastCSharp.memoryDatabase.cache.searchTree<primaryKey, int>(), memoryDatabaseTable.serializeType.Data))
                        {
                            updateMember = table.CreateMemberMap().Append(value => value.Int);
                            cache.WaitLoad();
                            if (cache.Count == 0)
                            {
                                primaryKey int1 = table.Insert(new primaryKey { Key = 1, Int = 1, String = "A大A" }, false);
                                primaryKey intOld2 = new primaryKey { Key = 2, Int = 2 };
                                primaryKey int2 = table.Insert(intOld2);
                                primaryKey int4 = table.Insert(new primaryKey { Key = 3, Int = 4 });
                                if (int1 == null) return false;
                                if (int2 == null) return false;
                                if (int4 == null) return false;

                                intOld2.Int = 3;
                                primaryKey int3 = table.Update(intOld2, updateMember);
                                if (int3 == null) return false;
                                primaryKey delete = table.Delete(int4.Key);
                                if (delete == null) return false;

                                return check(table, false);
                            }
                            return check(table, true);
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 远程模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<primaryKey, int>.remote table, bool isDelete)
            {
                subArray<primaryKey> subArray = cache.Values.getSubArray();
                if (subArray.Count != 2) return false;
                primaryKey[] array = subArray.getSort(value => value.Key);
                if (array[0].Key != 1 || array[0].Int != 1) return false;
                if (array[1].Key != 2 || array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (primaryKey identity in array) table.Delete(identity.Key);
                }
                table.Flush(false);
                return true;
            }
            /// <summary>
            /// 本地模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool TestCase()
            {
                using (memoryDatabaseModelTable<primaryKey, int> table = new memoryDatabaseModelTable<primaryKey, int>(cache = new fastCSharp.memoryDatabase.cache.searchTree<primaryKey, int>(), memoryDatabaseTable.serializeType.Data, "localPrimaryKey"))
                {
                    updateMember = table.CreateMemberMap().Append(value => value.Int);
                    cache.WaitLoad();
                    if (cache.Count == 0)
                    {
                        primaryKey int1 = table.Insert(new primaryKey { Key = 1, Int = 1, String = "A大A" }, false);
                        primaryKey intOld2 = new primaryKey { Key = 2, Int = 2 };
                        primaryKey int2 = table.Insert(intOld2);
                        primaryKey int4 = table.Insert(new primaryKey { Key = 3, Int = 4 });
                        if (int1 == null) return false;
                        if (int2 == null) return false;
                        if (int4 == null) return false;

                        intOld2.Int = 3;
                        primaryKey int3 = table.Update(intOld2, updateMember);
                        if (int3 == null) return false;
                        primaryKey delete = table.Delete(int4.Key);
                        if (delete == null) return false;

                        return check(table, false);
                    }
                    return check(table, true);
                }
            }
            /// <summary>
            /// 本地模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<primaryKey, int> table, bool isDelete)
            {
                subArray<primaryKey> subArray = cache.Values.getSubArray();
                if (subArray.Count != 2) return false;
                primaryKey[] array = subArray.getSort(value => value.Key);
                if (array[0].Key != 1 || array[0].Int != 1) return false;
                if (array[1].Key != 2 || array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (primaryKey identity in array) table.Delete(identity.Key);
                }
                table.Flush(false);
                return true;
            }
        }
        /// <summary>
        /// 多关键字测试
        /// </summary>
        [fastCSharp.code.cSharp.memoryDatabaseModel]
        public partial class primaryKey3 : data
        {
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1)]
            public int Key1;
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.emit.dataMember(PrimaryKeyIndex = 2)]
            public string Key2;
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.emit.dataMember(PrimaryKeyIndex = 3)]
            public Guid Key3;

#if NotFastCSharpCode
#else
            /// <summary>
            /// 缓存
            /// </summary>
            private static dictionary<primaryKey3, primaryKey> cache;
            /// <summary>
            /// 测试更新成员位图
            /// </summary>
            private static memberMap<primaryKey3> updateMember;
            /// <summary>
            /// 远程模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool RemoteTestCase()
            {
                typeof(fastCSharp.config.pub).GetProperty("IsDebug", BindingFlags.Instance | BindingFlags.Public).SetValue(fastCSharp.config.pub.Default, true, null);
                using (fastCSharp.memoryDatabase.physicalServer.tcpServer server = new fastCSharp.memoryDatabase.physicalServer.tcpServer(tcpServer))
                {
                    if (server.Start())
                    {
                        using (memoryDatabaseModelTable<primaryKey3, primaryKey>.remote table = new memoryDatabaseModelTable<primaryKey3, primaryKey>.remote(new fastCSharp.memoryDatabase.physicalServer.tcpClient(tcpServer), cache = new dictionary<primaryKey3, primaryKey>(), memoryDatabaseTable.serializeType.Json))
                        {
                            updateMember = table.CreateMemberMap().Append(value => value.Int);
                            cache.WaitLoad();
                            if (cache.Count == 0)
                            {
                                primaryKey3 int1 = table.Insert(new primaryKey3 { Key1 = 1, Key2 = "1", Key3 = Guid.NewGuid(), Int = 1, String = "A大A" }, false);
                                primaryKey3 intOld2 = new primaryKey3 { Key1 = 2, Key2 = "2", Key3 = Guid.NewGuid(), Int = 2 };
                                primaryKey3 int2 = table.Insert(intOld2);
                                primaryKey3 int4 = table.Insert(new primaryKey3 { Key1 = 2, Key2 = "2", Key3 = Guid.NewGuid(), Int = 4 });
                                if (int1 == null) return false;
                                if (int2 == null) return false;
                                if (int4 == null) return false;

                                intOld2.Int = 3;
                                primaryKey3 int3 = table.Update(intOld2, updateMember);
                                if (int3 == null) return false;
                                primaryKey3 delete = table.Delete(int4);
                                if (delete == null) return false;

                                return check(table, false);
                            }
                            return check(table, true);
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 远程模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<primaryKey3, primaryKey>.remote table, bool isDelete)
            {
                subArray<primaryKey3> subArray = cache.Values.getSubArray();
                if (subArray.Count != 2) return false;
                primaryKey3[] array = subArray.getSort(value => value.Key1);
                if (array[0].Key1 != 1 || array[0].Key2 != "1" || array[0].Int != 1) return false;
                if (array[1].Key1 != 2 || array[1].Key2 != "2" || array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (primaryKey3 identity in array) table.Delete(identity);
                }
                table.Flush(false);
                return true;
            }
            /// <summary>
            /// 本地模式测试
            /// </summary>
            /// <returns></returns>
            [fastCSharp.code.testCase]
            internal static bool TestCase()
            {
                using (memoryDatabaseModelTable<primaryKey3, primaryKey> table = new memoryDatabaseModelTable<primaryKey3, primaryKey>(cache = new dictionary<primaryKey3, primaryKey>(), memoryDatabaseTable.serializeType.Json, "localPrimaryKey3"))
                {
                    updateMember = table.CreateMemberMap().Append(value => value.Int);
                    cache.WaitLoad();
                    if (cache.Count == 0)
                    {
                        primaryKey3 int1 = table.Insert(new primaryKey3 { Key1 = 1, Key2 = "1", Key3 = Guid.NewGuid(), Int = 1, String = "A大A" }, false);
                        primaryKey3 intOld2 = new primaryKey3 { Key1 = 2, Key2 = "2", Key3 = Guid.NewGuid(), Int = 2 };
                        primaryKey3 int2 = table.Insert(intOld2);
                        primaryKey3 int4 = table.Insert(new primaryKey3 { Key1 = 2, Key2 = "2", Key3 = Guid.NewGuid(), Int = 4 });
                        if (int1 == null) return false;
                        if (int2 == null) return false;
                        if (int4 == null) return false;

                        intOld2.Int = 3;
                        primaryKey3 int3 = table.Update(intOld2, updateMember);
                        if (int3 == null) return false;
                        primaryKey3 delete = table.Delete(int4);
                        if (delete == null) return false;

                        return check(table, false);
                    }
                    return check(table, true);
                }
            }
            /// <summary>
            /// 本地模式测试验证
            /// </summary>
            /// <param name="table"></param>
            /// <param name="isDelete"></param>
            /// <returns></returns>
            private static bool check(memoryDatabaseModelTable<primaryKey3, primaryKey> table, bool isDelete)
            {
                subArray<primaryKey3> subArray = cache.Values.getSubArray();
                if (subArray.Count != 2) return false;
                primaryKey3[] array = subArray.getSort(value => value.Key1);
                if (array[0].Key1 != 1 || array[0].Key2 != "1" || array[0].Int != 1) return false;
                if (array[1].Key1 != 2 || array[1].Key2 != "2" || array[1].Int != 3) return false;
                if (isDelete)
                {
                    foreach (primaryKey3 identity in array) table.Delete(identity);
                }
                table.Flush(false);
                return true;
            }
#endif
        }
    }
}
