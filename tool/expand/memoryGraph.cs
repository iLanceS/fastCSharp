using System;
using System.Collections.Generic;
using fastCSharp.reflection;
using System.Reflection;
using System.IO;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 内存对象关系图
    /// </summary>
    public static partial class memoryGraph
    {
        /// <summary>
        /// 类型信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public sealed class type
        {
            /// <summary>
            /// 类型名称
            /// </summary>
            public string FullName;
            /// <summary>
            /// 字段名称集合集合
            /// </summary>
            public field[] Fields;
            /// <summary>
            /// 非引用类型字节尺寸
            /// </summary>
            public int Size;
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public struct field
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 类型信息
            /// </summary>
            public type Type;
            /// <summary>
            /// 设置字段信息
            /// </summary>
            /// <param name="name">字段名称</param>
            /// <param name="type">类型信息</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(string name, type type)
            {
                Name = name;
                Type = type;
            }
        }
        /// <summary>
        /// 对象值信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public sealed class value
        {
            /// <summary>
            /// 类型信息
            /// </summary>
            public type Type;
            /// <summary>
            /// 成员对象值集合
            /// </summary>
            public value[] Values;
        }
        /// <summary>
        /// 静态对象值信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public struct staticValue
        {
            /// <summary>
            /// 成员名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 对象值信息
            /// </summary>
            public value Value;
        }
        /// <summary>
        /// 静态根节点信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public struct staticType
        {
            /// <summary>
            /// 类型名称
            /// </summary>
            public string TypeName;
            /// <summary>
            /// 成员对象值集合
            /// </summary>
            public staticValue[] Values;
            /// <summary>
            /// 设置静态根节点信息
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="values">静态对象值信息集合</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(Type type, list<staticValue> values)
            {
                TypeName = type.fullName();
                Values = values.GetArray();
            }
        }
        /// <summary>
        /// 创建内存对象关系图
        /// </summary>
        private sealed class graphBuilder
        {
            /// <summary>
            /// 最大搜索深度
            /// </summary>
            private const int maxDepth = 256;
            /// <summary>
            /// 类型信息
            /// </summary>
            private sealed class typeInfo
            {
                /// <summary>
                /// 类型信息
                /// </summary>
                public type Type;
                /// <summary>
                /// 字段信息
                /// </summary>
                public list<FieldInfo> Fields;
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Add(FieldInfo field)
                {
                    if (Fields == null) Fields = new list<FieldInfo>();
                    Fields.Add(field);
                }
            }
            /// <summary>
            /// 静态根节点信息集合
            /// </summary>
            internal staticType[] StaticTypes;
            /// <summary>
            /// 类型集合
            /// </summary>
            private Dictionary<Type, typeInfo> types;
            /// <summary>
            /// 对象值集合
            /// </summary>
            private Dictionary<objectReference, value> values;
            /// <summary>
            /// 数组创建委托集合
            /// </summary>
            private Dictionary<Type, Action> arrayBuilders;

            /// <summary>
            /// 静态对象值信息集合
            /// </summary>
            private list<staticValue> staticValues;
            /// <summary>
            /// 类型名称集合
            /// </summary>
            private Dictionary<string, typeInfo> typeNames;
            /// <summary>
            /// 内存对象搜索器
            /// </summary>
            private searcher searcher;
            /// <summary>
            /// 创建内存对象关系图
            /// </summary>
            public graphBuilder()
            {
                HashSet<Type> checkTypes = hashSet.CreateOnly<Type>();
                foreach (Type type in fastCSharp.checkMemory.GetTypes().ToArray())
                {
                    if (checkTypes.Contains(type)) fastCSharp.log.Error.Add("重复类型 " + type.fullName(), new System.Diagnostics.StackFrame(), false);
                    else checkTypes.Add(type);
                }
                int count = checkTypes.Count;
                if (count != 0)
                {
                    StaticTypes = new staticType[count];
                    types = dictionary.CreateOnly<Type, typeInfo>();
                    values = dictionary<objectReference>.Create<value>();
                    arrayBuilders = dictionary.CreateOnly<Type, Action>();
                    staticValues = new list<staticValue>();
                    foreach (Type type in checkTypes)
                    {
                        currentType = type;
                        buildStatic();
                        StaticTypes[checkTypes.Count - count].Set(type, staticValues);
                        if (--count == 0) break;
                        staticValues.Empty();
                    }
                    values = null;
                    arrayBuilders = null;
                    staticValues = null;
                    checkTypes = null;
                    typeNames = dictionary.CreateOnly<string, typeInfo>();
                    foreach (typeInfo type in types.Values) typeNames[type.Type.FullName] = type;
                    types = null;
                    searcher = new searcher(StaticTypes);
                    searcher.OnType = searchTypeFields;
                    searcher.Start();
                }
            }
            /// <summary>
            /// 当前处理类型
            /// </summary>
            private Type currentType;
            /// <summary>
            /// 当前处理类型
            /// </summary>
            private typeInfo type;
            /// <summary>
            /// 当前处理对象
            /// </summary>
            private object currentValue;
            /// <summary>
            /// 深度
            /// </summary>
            private int depth;
            /// <summary>
            /// 当前处理对象
            /// </summary>
            private value value;
            /// <summary>
            /// 当前字段类型
            /// </summary>
            private Type fieldType;
            /// <summary>
            /// 创建静态类型信息
            /// </summary>
            private void buildStatic()
            {
                foreach (FieldInfo field in currentType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (!(fieldType = field.FieldType).IsPrimitive && !fieldType.IsPointer && !fieldType.IsEnum
                        && (currentValue = field.GetValue(null)) != null)
                    {
                        currentType = currentValue.GetType();
                        if (currentType.IsClass || currentType.IsInterface)
                        {
                            value = null;
                            if (values.TryGetValue(new objectReference { Value = currentValue }, out value))
                            {
                                staticValues.Add(new staticValue { Name = field.Name, Value = value });
                            }
                            else
                            {
                                values.Add(new objectReference { Value = currentValue }, value = new value());
                                staticValues.Add(new staticValue { Name = field.Name, Value = value });
                                buildValue();
                            }
                        }
                        else
                        {
                            staticValues.Add(new staticValue { Name = field.Name, Value = new value() });
                            buildValue();
                        }
                    }
                }
            }
            /// <summary>
            /// 创建对象值
            /// </summary>
            private void buildValue()
            {
                ++depth;
                if (!types.TryGetValue(currentType, out type))
                {
                    types.Add(currentType, type = new typeInfo { Type = new type() });
                    buildType();
                }
                this.value.Type = type.Type;
                if (type.Fields == null)
                {
                    if (currentType == typeof(string))
                    {
                        this.value.Type = new type { FullName = type.Type.FullName, Size = ((string)this.currentValue).Length };
                    }
                    else if (currentType.IsArray)
                    {
                        arrayType = currentType.GetElementType();
                        if (arrayType.IsPrimitive || arrayType.IsPointer || arrayType.IsEnum)
                        {
                            this.value.Type = new type { FullName = this.value.Type.FullName, Size = ((Array)this.currentValue).Length };
                        }
                        else
                        {
                            if (!arrayBuilders.TryGetValue(arrayType, out arrayBuilder))
                            {
                                arrayBuilders.Add(arrayType, arrayBuilder = (Action)Delegate.CreateDelegate(typeof(Action), this, buildArrayMethod.MakeGenericMethod(arrayType)));
                            }
                            arrayBuilder();
                        }
                    }
                }
                else if(depth < maxDepth)
                {
                    object currentValue = this.currentValue;
                    value value = this.value;
                    int count = type.Fields.Count, index = count;
                    foreach (FieldInfo field in type.Fields.UnsafeArray)
                    {
                        if ((this.currentValue = field.GetValue(currentValue)) != null)
                        {
                            if (value.Values == null) value.Values = new value[count];
                            currentType = this.currentValue.GetType();
                            if (currentType.IsClass || currentType.IsInterface)
                            {
                                if (!values.TryGetValue(new objectReference { Value = this.currentValue }, out value.Values[count - index]))
                                {
                                    values.Add(new objectReference { Value = this.currentValue }, value.Values[count - index] = this.value = new value());
                                    buildValue();
                                }
                            }
                            else
                            {
                                value.Values[count - index] = this.value = new value();
                                buildValue();
                            }
                        }
                        if (--index == 0) break;
                    }
                }
                --depth;
            }
            /// <summary>
            /// 基类类型
            /// </summary>
            private Type baseType;
            /// <summary>
            /// 值类型信息
            /// </summary>
            private typeInfo valueType;
            /// <summary>
            /// 创建类型
            /// </summary>
            private void buildType()
            {
                type.Type.FullName = currentType.fullName();
                if (currentType.IsPrimitive || currentType.IsPointer || currentType.IsEnum || currentType.IsArray || currentType == typeof(string))
                {
                    type.Type.Size = getMemorySize(currentType);
                }
                else
                {
                    if (currentType.IsValueType)
                    {
                        baseType = currentType;
                        buildFields();
                    }
                    else if (!currentType.IsInterface)
                    {
                        for (baseType = currentType; baseType != typeof(object); baseType = baseType.BaseType) buildFields();
                    }
                }
            }
            /// <summary>
            /// 创建字段信息
            /// </summary>
            private void buildFields()
            {
                foreach (FieldInfo field in baseType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if ((fieldType = field.FieldType).IsPrimitive || fieldType.IsPointer || fieldType.IsEnum)
                    {
                        type.Type.Size += getMemorySize(fieldType);
                    }
                    else if (fieldType.IsValueType)
                    {
                        valueType = buildValueType(fieldType);
                        if (valueType.Fields == null) type.Type.Size += valueType.Type.Size;
                        else type.Add(field);
                    }
                    else
                    {
                        type.Add(field);
                        if (!fieldType.IsValueType) type.Type.Size = fastCSharp.pub.MemoryBytes;
                    }
                }
            }
            /// <summary>
            /// 创建值类型
            /// </summary>
            /// <param name="type"></param>
            private typeInfo buildValueType(Type type)
            {
                typeInfo valueType;
                if (!types.TryGetValue(type, out valueType))
                {
                    valueType = new typeInfo { Type = new type { FullName = type.fullName() } };
                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if ((fieldType = field.FieldType).IsPrimitive || fieldType.IsPointer || fieldType.IsEnum)
                        {
                            valueType.Type.Size += getMemorySize(fieldType);
                        }
                        else if (fieldType.IsValueType)
                        {
                            valueType = buildValueType(fieldType);
                            if (valueType.Fields == null) valueType.Type.Size += valueType.Type.Size;
                            else valueType.Add(field);
                        }
                        else
                        {
                            valueType.Add(field);
                            if (!fieldType.IsValueType) valueType.Type.Size = fastCSharp.pub.MemoryBytes;
                        }
                    }
                }
                return valueType;
            }
            /// <summary>
            /// 数组类型
            /// </summary>
            private Type arrayType;
            /// <summary>
            /// 创建数组
            /// </summary>
            private Action arrayBuilder;
            /// <summary>
            /// 创建数组
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            private void buildArray<valueType>()
            {
                if (!arrayType.IsInterface && !types.TryGetValue(arrayType, out type))
                {
                    types.Add(currentType = arrayType, type = new typeInfo { Type = new type() });
                    buildType();
                }
                value[] values = this.value.Values = new value[((valueType[])this.currentValue).Length];
                int index = 0;
                if (arrayType == typeof(string))
                {
                    foreach (string value in (string[])this.currentValue)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            values[index] = new value { Type = new type { FullName = type.Type.FullName, Size = value.Length } };
                        }
                        ++index;
                    }
                }
                else if (depth < maxDepth)
                {
                    if (arrayType.IsClass || arrayType.IsInterface)
                    {
                        foreach (valueType value in (valueType[])this.currentValue)
                        {
                            if (value != null)
                            {
                                currentType = value.GetType();
                                if (currentType.IsClass || currentType.IsInterface)
                                {
                                    if (!this.values.TryGetValue(new objectReference { Value = value }, out values[index]))
                                    {
                                        this.values.Add(new objectReference { Value = this.currentValue = value }, values[index] = this.value = new value());
                                        buildValue();
                                    }
                                }
                                else
                                {
                                    this.currentValue = value;
                                    values[index] = this.value = new value();
                                    buildValue();
                                }
                            }
                            ++index;
                        }
                    }
                    else
                    {
                        foreach (valueType value in (valueType[])this.currentValue)
                        {
                            this.currentValue = value;
                            currentType = typeof(valueType);
                            values[index++] = this.value = new value();
                            buildValue();
                        }
                    }
                }
            }
            /// <summary>
            /// 设置类型字段信息
            /// </summary>
            private void searchTypeFields()
            {
                typeInfo type;
                if (typeNames.TryGetValue(searcher.Value.Type.FullName, out type) && type.Fields != null)
                {
                    int count = type.Fields.Count;
                    field[] fields = type.Type.Fields = new field[count];
                    foreach (FieldInfo field in type.Fields.UnsafeArray)
                    {
                        fields[type.Fields.Count - count].Set(field.Name, type.Type);
                        if (--count == 0) break;
                    }
                }
            }
            /// <summary>
            /// 创建数组函数信息
            /// </summary>
            private static readonly MethodInfo buildArrayMethod = typeof(graphBuilder).GetMethod("buildArray", BindingFlags.Instance | BindingFlags.NonPublic, null, nullValue<Type>.Array, null);
            /// <summary>
            /// 基本类型占用内存字节数
            /// </summary>
            private static readonly Dictionary<Type, int> memorySizes;
            /// <summary>
            /// 获取基本类型占用内存字节数
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static int getMemorySize(Type type)
            {
                int size;
                return memorySizes.TryGetValue(type, out size) ? size : fastCSharp.pub.MemoryBytes;
            }
            static unsafe graphBuilder()
            {
                memorySizes = dictionary.CreateOnly<Type, int>();
                memorySizes.Add(typeof(bool), sizeof(bool));
                memorySizes.Add(typeof(byte), sizeof(byte));
                memorySizes.Add(typeof(sbyte), sizeof(sbyte));
                memorySizes.Add(typeof(short), sizeof(short));
                memorySizes.Add(typeof(ushort), sizeof(ushort));
                memorySizes.Add(typeof(int), sizeof(int));
                memorySizes.Add(typeof(uint), sizeof(uint));
                memorySizes.Add(typeof(long), sizeof(long));
                memorySizes.Add(typeof(ulong), sizeof(ulong));
                memorySizes.Add(typeof(char), sizeof(char));
                memorySizes.Add(typeof(DateTime), sizeof(long));
                memorySizes.Add(typeof(float), sizeof(float));
                memorySizes.Add(typeof(double), sizeof(double));
                memorySizes.Add(typeof(decimal), sizeof(decimal));
                memorySizes.Add(typeof(Guid), sizeof(Guid));
            }
        }
        /// <summary>
        /// 获取静态根节点信息集合
        /// </summary>
        /// <returns></returns>
        public static staticType[] Get()
        {
            return new graphBuilder().StaticTypes;
        }
        /// <summary>
        /// 内存对象搜索器
        /// </summary>
        public sealed class searcher
        {
            /// <summary>
            /// 静态根节点信息集合
            /// </summary>
            private staticType[] staticTypes;
            /// <summary>
            /// 当前静态根节点信息
            /// </summary>
            public staticType StaticType { get; private set; }
            /// <summary>
            /// 当前静态对象值信息
            /// </summary>
            public staticValue StaticValue { get; private set; }
            /// <summary>
            /// 当前处理对象
            /// </summary>
            public value Value { get; private set; }
            /// <summary>
            /// 历史类型集合
            /// </summary>
            private HashSet<hashString> types;
            /// <summary>
            /// 历史对象集合
            /// </summary>
            private HashSet<value> values;
            /// <summary>
            /// 历史对象名称集合
            /// </summary>
            private list<string> path;
            /// <summary>
            /// 历史对象名称集合
            /// </summary>
            public IEnumerable<string> Path
            {
                get { return path; }
            }
            /// <summary>
            /// 搜索对象事件
            /// </summary>
            public Action OnValue;
            /// <summary>
            /// 搜索新对象事件
            /// </summary>
            public Action OnNewValue;
            /// <summary>
            /// 搜索新类型事件
            /// </summary>
            public Action OnType;
            /// <summary>
            /// 是否停止搜索
            /// </summary>
            public bool IsStop;
            /// <summary>
            /// 内存对象搜索器
            /// </summary>
            /// <param name="staticTypes">静态根节点信息集合</param>
            public searcher(staticType[] staticTypes)
            {
                this.staticTypes = staticTypes ?? nullValue<staticType>.Array;
            }
            /// <summary>
            /// 开始搜索
            /// </summary>
            public void Start()
            {
                IsStop = false;
                if (types == null) types = hashSet.CreateHashString();
                else types.Clear();
                if (values == null) values = hashSet.CreateOnly<value>();
                else values.Clear();
                if (path == null) path = new list<string>();
                else path.Clear();
                foreach (staticType type in staticTypes)
                {
                    if (IsStop) return;
                    StaticType = type;
                    foreach (staticValue value in type.Values.notNull())
                    {
                        if (IsStop) return;
                        StaticValue = value;
                        if (value.Value != null)
                        {
                            Value = value.Value;
                            searchValue();
                        }
                    }
                }
            }
            /// <summary>
            /// 搜索对象
            /// </summary>
            private void searchValue()
            {
                if (OnValue != null)
                {
                    OnValue();
                    if (IsStop) return;
                }
                if (!values.Contains(Value))
                {
                    values.Add(Value);
                    if (OnNewValue != null)
                    {
                        OnNewValue();
                        if (IsStop) return;
                    }
                    hashString typeKey = Value.Type.FullName;
                    if (!types.Contains(typeKey))
                    {
                        types.Add(typeKey);
                        if (OnType != null)
                        {
                            OnType();
                            if (IsStop) return;
                        }
                    }
                    if (Value.Values != null)
                    {
                        field[] fields = Value.Type.Fields;
                        int index = 0;
                        if (fields == null)
                        {
                            foreach (value value in Value.Values)
                            {
                                if (IsStop) break;
                                if (value != null)
                                {
                                    path.Add(index.toString());
                                    Value = value;
                                    searchValue();
                                    path.UnsafePop();
                                }
                                ++index;
                            }
                        }
                        else
                        {
                            foreach (value value in Value.Values)
                            {
                                if (IsStop) break;
                                if (value != null)
                                {
                                    path.Add(fields[index].Name);
                                    Value = value;
                                    searchValue();
                                    path.UnsafePop();
                                }
                                ++index;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取静态根节点信息集合并序列化保存到文件访问锁
        /// </summary>
        private static int saveFileLock;
        /// <summary>
        /// 获取静态根节点信息集合并序列化保存到文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void SaveFile(string fileName)
        {
            if (Interlocked.CompareExchange(ref saveFileLock, 1, 0) == 0)
            {
                fastCSharp.threading.threadPool.TinyPool.Start(saveFile, fileName);
            }
        }
        /// <summary>
        /// 获取静态根节点信息集合并序列化保存到文件
        /// </summary>
        /// <param name="fileName"></param>
        private static void saveFile(string fileName)
        {
            try
            {
                GC.Collect();
                using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (unmanagedStreamProxy stream = new unmanagedStreamProxy(fileStream))
                {
                    log.Default.Add("开始生成内存对象关系数据", new System.Diagnostics.StackFrame(), false);
                    staticType[] values = Get();
                    GC.Collect();
                    fastCSharp.emit.dataSerializer.Serialize(values, stream);
                    log.Default.Add("内存对象关系数据生成完毕", new System.Diagnostics.StackFrame(), false);
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally
            {
                saveFileLock = 0;
                GC.Collect();
            }
        }
#if MONO
#else
        /// <summary>
        /// 内存使用信息
        /// </summary>
        public struct memoryInfo
        {
            /// <summary>
            /// 物理内存大小
            /// </summary>
            public ulong Total;
            /// <summary>
            /// 可用物理内存大小
            /// </summary>
            public ulong Avail;
        }
        /// <summary>
        /// 获取内存使用信息
        /// </summary>
        /// <returns>内存使用信息</returns>
        public static memoryInfo GetMemoryInfo()
        {
            win32.kernel32.memoryStatuExpand memory = new win32.kernel32.memoryStatuExpand();
            if (win32.kernel32.GlobalMemoryStatusEx(memory))
            {
                return new memoryInfo { Total = memory.TotalPhys, Avail = memory.AvailPhys };
            }
            return default(memoryInfo);
        }
#endif
    }
}
