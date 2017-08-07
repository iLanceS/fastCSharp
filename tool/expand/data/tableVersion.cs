using System;
using System.IO;
using fastCSharp;
using fastCSharp.code.cSharp;
using fastCSharp.io;
using fastCSharp.threading;

namespace fastCSharp.data
{
    /// <summary>
    /// 表格版本(只能绑定一个客户端)
    /// </summary>
    public unsafe sealed partial class tableVersion : IDisposable
    {
        /// <summary>
        /// 表格版本
        /// </summary>
        public struct timeVersions
        {
            /// <summary>
            /// 时间戳版本
            /// </summary>
            public timeVersion Version;
            /// <summary>
            /// 表格版本集合
            /// </summary>
            public ulong[] Versions;
        }
        /// <summary>
        /// 时间戳版本
        /// </summary>
        [fastCSharp.code.cSharp.serialize(IsBaseSerialize = true)]
        public partial struct timeVersion
        {
            /// <summary>
            /// 全局版本
            /// </summary>
            public ulong Version;
            /// <summary>
            /// 时间戳
            /// </summary>
            public uint Time;
        }
        /// <summary>
        /// 表格版本更新信息
        /// </summary>
        [fastCSharp.code.cSharp.serialize(IsBaseSerialize = true)]
        public partial struct updateInfo
        {
            /// <summary>
            /// 全局版本
            /// </summary>
            public ulong Version;
            /// <summary>
            /// 表格编号
            /// </summary>
            public int Table;
            /// <summary>
            /// 表格版本集合
            /// </summary>
            public ulong[] Versions;
            /// <summary>
            /// 表格编号集合
            /// </summary>
            public int[] Tables;
        }
        /// <summary>
        /// 更新事件绑定
        /// </summary>
        /// <typeparam name="valueType">表格绑定类型</typeparam>
        /// <typeparam name="memberType">成员位图类型</typeparam>
        private sealed class sqlTool<valueType, memberType>
            where valueType : class
            where memberType : IMemberMap<memberType>
        {
            /// <summary>
            /// 表格版本
            /// </summary>
            private tableVersion version;
            /// <summary>
            /// 表格编号
            /// </summary>
            private int table;
            /// <summary>
            /// 更新事件绑定
            /// </summary>
            /// <param name="version">表格版本</param>
            /// <param name="table">表格编号</param>
            public sqlTool(tableVersion version, int table)
            {
                this.version = version;
                this.table = table;
            }
            /// <summary>
            /// 更新事件绑定
            /// </summary>
            /// <param name="sqlTool">SQL操作工具类</param>
            public void Set(sqlTable.sqlToolBase<valueType, memberType> sqlTool)
            {
                sqlTool.OnInserted += update;
                sqlTool.OnUpdated += update;
                sqlTool.OnDeleted += update;
            }
            /// <summary>
            /// 更新事件处理
            /// </summary>
            /// <param name="value"></param>
            private void update(valueType value)
            {
                version.update(table);
            }
            /// <summary>
            /// 更新事件处理
            /// </summary>
            /// <param name="value"></param>
            /// <param name="oldValue"></param>
            /// <param name="memberMap"></param>
            private void update(valueType value, valueType oldValue, memberType memberMap)
            {
                version.update(table);
            }
        }
        /// <summary>
        /// 客户端表格版本
        /// </summary>
        public sealed unsafe class client : IDisposable
        {
            /// <summary>
            /// 时间戳版本
            /// </summary>
            internal timeVersion Version;
            /// <summary>
            /// 表格版本数量
            /// </summary>
            private int count;
            /// <summary>
            /// 表格版本集合
            /// </summary>
            internal ulong* Versions;
            /// <summary>
            /// 初始化获取委托
            /// </summary>
            private Func<asynchronousMethod.returnValue<timeVersions>> getVersion;
            /// <summary>
            /// 获取更新信息委托
            /// </summary>
            private Action<timeVersion, Action<asynchronousMethod.returnValue<tableVersion.updateInfo>>> getUpdate;
            /// <summary>
            /// 初始化获取委托
            /// </summary>
            private Action tryGet;
            /// <summary>
            /// 获取更新信息
            /// </summary>
            private Action<asynchronousMethod.returnValue<tableVersion.updateInfo>> onUpdate;
            /// <summary>
            /// 客户端表格版本
            /// </summary>
            /// <param name="getVersion">初始化获取委托</param>
            /// <param name="getUpdate">获取更新信息委托</param>
            public client(Func<asynchronousMethod.returnValue<timeVersions>> getVersion
                , Action<timeVersion, Action<asynchronousMethod.returnValue<tableVersion.updateInfo>>> getUpdate)
            {
                this.getVersion = getVersion;
                this.getUpdate = getUpdate;
                tryGet = get;
                onUpdate = onGet;
            }
            /// <summary>
            /// 初始化获取表格版本
            /// </summary>
            private void get()
            {
                try
                {
                    timeVersions version = getVersion();
                    if (count != version.Versions.Length)
                    {
                        if (Versions != null)
                        {
                            unmanaged.Free(Versions);
                            Versions = null;
                            count = 0;
                        }
                        Versions = unmanaged.Get(version.Versions.Length * sizeof(ulong), false).ULong;
                        count = version.Versions.Length;
                    }
                    fixed (ulong* versionFixed = version.Versions)
                    {
                        for (ulong* start = versionFixed, end = versionFixed + version.Versions.Length, write = Versions; start != end; *write++ = *start++) ;
                    }
                    Version.Version = version.Version.Version;
                    Version.Time = version.Version.Time;
                    getUpdate(Version, onUpdate);
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Version.Time = 0;
                fastCSharp.threading.timerTask.Default.Add(tryGet, date.NowSecond.AddSeconds(2));
            }
            /// <summary>
            /// 获取更新信息
            /// </summary>
            /// <param name="updateInfo">更新信息</param>
            private void onGet(asynchronousMethod.returnValue<tableVersion.updateInfo> updateInfo)
            {
                if (updateInfo.IsReturn && update(updateInfo))
                {
                    try
                    {
                        getUpdate(Version, onUpdate);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                Version.Time = 0;
                fastCSharp.threading.timerTask.Default.Add(tryGet, date.NowSecond.AddSeconds(2));
            }
            /// <summary>
            /// 更新表格版本
            /// </summary>
            /// <param name="updateInfo">更新信息</param>
            /// <returns>是否成功</returns>
            private unsafe bool update(asynchronousMethod.returnValue<tableVersion.updateInfo> updateInfo)
            {
                if (updateInfo.Value.Tables == null)
                {
                    if (updateInfo.Value.Version != 0)
                    {
                        if (updateInfo.Value.Table >= count) newVersions(updateInfo.Value.Table + 1);
                        Versions[updateInfo.Value.Table] = updateInfo.Value.Version;
                        Version.Version = updateInfo.Value.Version;
                        return true;
                    }
                    return false;
                }
                if (updateInfo.Value.Table > count) newVersions(updateInfo.Value.Table);
                ulong maxVersion = Version.Version;
                fixed (int* tableFixed = updateInfo.Value.Tables)
                fixed (ulong* versionFixed = updateInfo.Value.Versions)
                {
                    ulong* versionRead = versionFixed + updateInfo.Value.Versions.Length;
                    for (int* table = tableFixed + updateInfo.Value.Tables.Length; table != tableFixed; Versions[*--table] = *versionRead)
                    {
                        if (*--versionRead > maxVersion) maxVersion = *versionRead;
                    }
                }
                Version.Version = maxVersion;
                return true;
            }
            /// <summary>
            /// 表格版本集合数量更新
            /// </summary>
            /// <param name="count">表格版本集合数量</param>
            private unsafe void newVersions(int count)
            {
                ulong* versions = unmanaged.Get(count * sizeof(ulong), false).ULong, write = versions;
                for (ulong* read = this.Versions, end = this.Versions + this.count; read != end; *write++ = *read++) ;
                unmanaged.Free(this.Versions);
                for (ulong* end = versions + count; write != end; *write++ = 0) ;
                this.Versions = versions;
                this.count = count;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (Versions != null)
                {
                    unmanaged.Free(Versions);
                    count = 0;
                }
                Version.Time = 0;
            }
        }
        /// <summary>
        /// 客户端表格版本过滤
        /// </summary>
        public sealed unsafe class clientFilter : IDisposable
        {
            /// <summary>
            /// 客户端表格版本
            /// </summary>
            private client client;
            /// <summary>
            /// 时间戳版本
            /// </summary>
            private timeVersion version;
            /// <summary>
            /// 当前版本
            /// </summary>
            public timeVersion Version
            {
                get
                {
                    if (client.Version.Time != 0)
                    {
                        if (client.Version.Time != version.Time || client.Version.Version != checkVersion)
                        {
                            if (tables == null) version.Version = client.Version.Version;
                            else
                            {
                                int* table = tables + count;
                                ulong maxVersion = client.Versions[*--table];
                                while (table != tables)
                                {
                                    if (client.Versions[*--table] > maxVersion) maxVersion = client.Versions[*table];
                                }
                                version.Version = maxVersion;
                            }
                            version.Time = client.Version.Time;
                            checkVersion = client.Version.Version;
                        }
                        return version;
                    }
                    return new timeVersion();
                }
            }
            /// <summary>
            /// 当前检测版本
            /// </summary>
            private ulong checkVersion;
            /// <summary>
            /// 表格编号集合
            /// </summary>
            private int* tables;
            /// <summary>
            /// 表格数量
            /// </summary>
            private int count;
            /// <summary>
            /// 客户端表格版本过滤
            /// </summary>
            /// <param name="client">客户端表格版本</param>
            /// <param name="tables">表格编号集合</param>
            public clientFilter(client client, params int[] tables)
            {
                this.client = client;
                if (tables.length() != 0)
                {
                    this.tables = unmanaged.Get(tables.Length * sizeof(int), false).Int;
                    count = tables.Length;
                    fixed (int* tableFixed = tables)
                    {
                        for (int* start = tableFixed, end = tableFixed + count, write = this.tables; start != end; *write++ = *start++) ;
                    }
                }
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                unmanaged.Free(tables);
                tables = null;
                count = 0;
            }
        }
        /// <summary>
        /// 文件名
        /// </summary>
        private string fileName;
        /// <summary>
        /// 表格版本
        /// </summary>
        private timeVersions version;
        /// <summary>
        /// 表格版本集合
        /// </summary>
        private ulong* versions;
        /// <summary>
        /// 表格版本访问锁
        /// </summary>
        private int versionLock;
        /// <summary>
        /// 表格版本更新处理委托
        /// </summary>
        private Func<asynchronousMethod.returnValue<updateInfo>, bool> onUpdate;
        /// <summary>
        /// 当前更新版本
        /// </summary>
        private ulong updateVersion;
        /// <summary>
        /// 更新表格列表
        /// </summary>
        private int* tables;
        /// <summary>
        /// 当前处理表格
        /// </summary>
        private int* currentTable;
        /// <summary>
        /// 表格版本
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="tableCount">表格数量</param>
        public tableVersion(string fileName, int tableCount)
        {
            this.fileName = fileName;
            versions = unmanaged.Get(tableCount * sizeof(ulong), true).ULong;
            currentTable = tables = unmanaged.Get(tableCount * sizeof(int), true).Int;
            int isFile = 0;
            try
            {
                if (File.Exists(fileName))
                {
                    byte[] data = File.ReadAllBytes(fileName);
                    fileName = file.MoveBak(fileName);
                    version = serialize.deSerialize.Get<timeVersions>(data);
                    fixed (ulong* versionFixed = version.Versions)
                    {
                        unsafer.memory.Copy(versionFixed, versions, Math.Min(tableCount, version.Versions.Length) * sizeof(ulong));
                    }
                    if (version.Versions.Length < tableCount) version.Versions = new ulong[tableCount];
                    isFile = 1;
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, "表格版本加载失败 " + fileName, false);
            }
            if (isFile == 0)
            {
                DateTime time = fastCSharp.pub.StartTime;
                version = new timeVersions { Version = new timeVersion { Time = (((uint)time.Second << 26) | ((uint)time.Minute << 20) | ((uint)time.Hour << 16) | ((uint)time.Day << 11) | ((uint)time.Month << 7)) ^ ((uint)time.Year) }, Versions = new ulong[tableCount] };
            }
        }
        /// <summary>
        /// 更新事件绑定
        /// </summary>
        /// <typeparam name="valueType">表格绑定类型</typeparam>
        /// <typeparam name="memberType">成员位图类型</typeparam>
        /// <param name="sqlTool">SQL操作工具类</param>
        /// <param name="table">表格编号</param>
        public void Set<valueType, memberType>(sqlTable.sqlToolBase<valueType, memberType> sqlTool, int table)
            where valueType : class
            where memberType : IMemberMap<memberType>
        {
            new sqlTool<valueType, memberType>(this, table).Set(sqlTool);
        }
        /// <summary>
        /// 获取表格版本
        /// </summary>
        /// <returns>表格版本</returns>
        public timeVersions Get()
        {
            fixed (ulong* versionFixed = version.Versions)
            {
                interlocked.CompareSetSleep0(ref versionLock);
                unsafer.memory.Copy(versions, versionFixed, version.Versions.Length * sizeof(ulong));
                timeVersions value = version;
                updateVersion = version.Version.Version;
                currentTable = tables;
                versionLock = 0;
                return value;
            }
        }
        /// <summary>
        /// 版本更新
        /// </summary>
        /// <param name="table">表格编号</param>
        private void update(int table)
        {
            ulong* tableVersion = versions + table;
            interlocked.CompareSetSleep0(ref versionLock);
            Func<asynchronousMethod.returnValue<updateInfo>, bool> onUpdate = this.onUpdate;
            ulong version = ++this.version.Version.Version;
            this.onUpdate = null;
            if (onUpdate == null)
            {
                if (*tableVersion <= updateVersion) *currentTable++ = table;
                *tableVersion = version;
                versionLock = 0;
            }
            else
            {
                currentTable = tables;
                *tableVersion = updateVersion = version;
                versionLock = 0;
                onUpdate(new updateInfo { Version = version, Table = table });
            }
        }
        /// <summary>
        /// 获取更新信息
        /// </summary>
        /// <param name="version">全局版本</param>
        /// <param name="onUpdate">表格版本更新处理委托</param>
        public void Get(timeVersion version, Func<asynchronousMethod.returnValue<updateInfo>, bool> onUpdate)
        {
            updateInfo value = new updateInfo();
            interlocked.CompareSetSleep0(ref versionLock);
            Func<asynchronousMethod.returnValue<updateInfo>, bool> oldOnUpdate = this.onUpdate;
            if (version.Time == this.version.Version.Time)
            {
                if (version.Version == this.version.Version.Version)
                {
                    this.onUpdate = onUpdate;
                    onUpdate = null;
                    versionLock = 0;
                }
                else if (version.Version == updateVersion)
                {
                    int count = (int)(currentTable - tables);
                    if (count == 1)
                    {
                        value.Table = *tables;
                        value.Version = updateVersion = this.version.Version.Version;
                        currentTable = tables;
                        versionLock = 0;
                    }
                    else
                    {
                        value.Table = this.version.Versions.Length;
                        try
                        {
                            fixed (int* tableFixed = value.Tables = new int[count])
                            fixed (ulong* versionFixed = value.Versions = new ulong[count])
                            {
                                ulong* versionWrite = versionFixed;
                                int* tableWrite = tableFixed;
                                do
                                {
                                    *versionWrite++ = *(versions + (*tableWrite = *--currentTable));
                                }
                                while (currentTable != tables);
                            }
                            updateVersion = this.version.Version.Version;
                        }
                        finally { versionLock = 0; }
                    }
                }
                else versionLock = 0;
            }
            else versionLock = 0;
            try
            {
                if (onUpdate != null) onUpdate(value);
            }
            finally
            {
                if (oldOnUpdate != null) onUpdate(new updateInfo());
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public unsafe void Dispose()
        {
            interlocked.CompareSetSleep0(ref versionLock);
            try
            {
                if (versions != null)
                {
                    if (File.Exists(fileName)) file.MoveBak(fileName);
                    fixed (ulong* versionFixed = version.Versions)
                    {
                        unsafer.memory.Copy(versions, versionFixed, version.Versions.Length * sizeof(ulong));
                    }
                    File.WriteAllBytes(fileName, serialize.dataSerialize.Get(version, code.memberFilters.InstanceField, default(memberMap<timeVersions>)));
                    unmanaged.Free(tables);
                    tables = null;
                    unmanaged.Free(versions);
                    versions = null;
                }
            }
            finally { versionLock = 0; }
        }
    }
}
