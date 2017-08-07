using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using fastCSharp.threading;
using System.IO;
using System.Text;
using fastCSharp.emit;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// XML文档注释
    /// </summary>
    internal static class xmlDocument
    {
        /// <summary>
        /// 程序集信息
        /// </summary>
        public unsafe sealed class assembly
        {
            /// <summary>
            /// 类型：类、接口、结构、枚举、委托
            /// </summary>
            private readonly Dictionary<hashString, xmlNode> types = dictionary.CreateHashString<xmlNode>();
            /// <summary>
            /// 类型名称流
            /// </summary>
            private readonly charStream typeNameStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 类型集合访问锁
            /// </summary>
            private interlocked.classLastDictionary<Type, xmlNode> typeLock = new interlocked.classLastDictionary<Type, xmlNode>();
            /// <summary>
            /// 获取类型信息
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private xmlNode get(Type type)
            {
                if (type != null)
                {
                    xmlNode node;
                    if (typeLock.TryGetValue(type, out node)) return node;
                    try
                    {
                        hashString typeName;
                        pointer buffer = unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (typeNameStream)
                            {
                                typeNameStream.UnsafeReset(buffer.Byte, unmanagedPool.StreamBuffers.Size);
                                type.nameBuilder nameBuilder = new type.nameBuilder { NameStream = typeNameStream, IsXml = true };
                                nameBuilder.Xml(type);
                                typeName = typeNameStream.ToString();
                            }
                        }
                        finally { unmanagedPool.StreamBuffers.Push(ref buffer); }
                        if (types.TryGetValue(typeName, out node)) types.Remove(typeName);
                        typeLock.Set(type, node);
                    }
                    finally { typeLock.Exit(); }
                    return node;
                }
                return default(xmlNode);
            }
            /// <summary>
            /// 获取类型描述
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public string GetSummary(Type type)
            {
                return get(get(type), "summary");
            }
            /// <summary>
            /// 字段
            /// </summary>
            private readonly Dictionary<hashString, xmlNode> fields = dictionary.CreateHashString<xmlNode>();
            /// <summary>
            /// 字段
            /// </summary>
            private readonly charStream fieldNameStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 字段集合访问锁
            /// </summary>
            private interlocked.classLastDictionary<FieldInfo, xmlNode> fieldLock = new interlocked.classLastDictionary<FieldInfo, xmlNode>();
            /// <summary>
            /// 获取字段信息
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            private xmlNode get(FieldInfo field)
            {
                if (field != null)
                {
                    xmlNode node;
                    if (fieldLock.TryGetValue(field, out node)) return node;
                    try
                    {
                        hashString fieldName;
                        pointer buffer = unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (fieldNameStream)
                            {
                                fieldNameStream.UnsafeReset(buffer.Byte, unmanagedPool.StreamBuffers.Size);
                                type.nameBuilder nameBuilder = new type.nameBuilder { NameStream = fieldNameStream, IsXml = true };
                                nameBuilder.Xml(field.DeclaringType);
                                fieldNameStream.Write('.');
                                fieldNameStream.SimpleWriteNotNull(field.Name);
                                fieldName = fieldNameStream.ToString();
                            }
                        }
                        finally { unmanagedPool.StreamBuffers.Push(ref buffer); }
                        if (fields.TryGetValue(fieldName, out node)) fields.Remove(fieldName);
                        fieldLock.Set(field, node);
                    }
                    finally { fieldLock.Exit(); }
                    return node;
                }
                return default(xmlNode);
            }
            /// <summary>
            /// 获取字段描述
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            public string GetSummary(FieldInfo field)
            {
                return get(get(field), "summary");
            }
            /// <summary>
            /// 属性（包括索引程序或其他索引属性）
            /// </summary>
            private readonly Dictionary<hashString, xmlNode> properties = dictionary.CreateHashString<xmlNode>();
            /// <summary>
            /// 属性
            /// </summary>
            private readonly charStream propertyNameStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 属性集合访问锁
            /// </summary>
            private interlocked.classLastDictionary<PropertyInfo, xmlNode> propertyLock = new interlocked.classLastDictionary<PropertyInfo, xmlNode>();
            /// <summary>
            /// 获取属性信息
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            private xmlNode get(PropertyInfo property)
            {
                if (property != null)
                {
                    xmlNode node;
                    if (propertyLock.TryGetValue(property, out node)) return node;
                    try
                    {
                        hashString propertyName;
                        pointer buffer = unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (propertyNameStream)
                            {
                                propertyNameStream.UnsafeReset(buffer.Byte, unmanagedPool.StreamBuffers.Size);
                                type.nameBuilder nameBuilder = new type.nameBuilder { NameStream = propertyNameStream, IsXml = true };
                                nameBuilder.Xml(property.DeclaringType);
                                propertyNameStream.Write('.');
                                propertyNameStream.SimpleWriteNotNull(property.Name);
                                propertyName = propertyNameStream.ToString();
                            }
                        }
                        finally { unmanagedPool.StreamBuffers.Push(ref buffer); }
                        if (properties.TryGetValue(propertyName, out node)) properties.Remove(propertyName);
                        propertyLock.Set(property, node);
                    }
                    finally { propertyLock.Exit(); }
                    return node;
                }
                return default(xmlNode);
            }
            /// <summary>
            /// 获取属性描述
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            public string GetSummary(PropertyInfo property)
            {
                return get(get(property), "summary");
            }
            /// <summary>
            /// 方法（包括一些特殊方法，例如构造函数、运算符等）
            /// </summary>
            private readonly Dictionary<hashString, xmlNode> methods = dictionary.CreateHashString<xmlNode>();
            /// <summary>
            /// 方法名称流
            /// </summary>
            private readonly charStream methodNameStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 方法集合访问锁
            /// </summary>
            private interlocked.classLastDictionary<MethodInfo, xmlNode> methodLock = new interlocked.classLastDictionary<MethodInfo, xmlNode>();
            /// <summary>
            /// 获取方法信息
            /// </summary>
            /// <param name="method"></param>
            /// <returns></returns>
            private xmlNode get(MethodInfo method)
            {
                if (method != null)
                {
                    xmlNode node;
                    if (methodLock.TryGetValue(method, out node)) return node;
                    try
                    {
                        hashString methodName;
                        pointer buffer = unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (methodNameStream)
                            {
                                methodNameStream.UnsafeReset(buffer.Byte, unmanagedPool.StreamBuffers.Size);
                                type.nameBuilder nameBuilder = new type.nameBuilder { NameStream = methodNameStream, IsXml = true };
                                nameBuilder.Xml(method.DeclaringType);
                                methodNameStream.Write('.');
                                string name = method.Name;
                                if (name[0] == '.')
                                {
                                    methodNameStream.Write('#');
                                    methodNameStream.Write(subString.Unsafe(name, 1, name.Length - 1));
                                }
                                else methodNameStream.SimpleWriteNotNull(name);
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length != 0)
                                {
                                    bool isFirst = true;
                                    methodNameStream.Write('(');
                                    foreach (ParameterInfo parameter in parameters)
                                    {
                                        if (isFirst) isFirst = false;
                                        else methodNameStream.Write(',');
                                        nameBuilder.Xml(parameter.ParameterType);
                                    }
                                    methodNameStream.Write(')');
                                }
                                formatName(methodNameStream.Char, methodNameStream.CurrentChar);
                                methodName = methodNameStream.ToString();
                            }
                        }
                        finally { unmanagedPool.StreamBuffers.Push(ref buffer); }
                        if (methods.TryGetValue(methodName, out node)) methods.Remove(methodName);
                        methodLock.Set(method, node);
                    }
                    finally { methodLock.Exit(); }
                    return node;
                }
                return default(xmlNode);
            }
            /// <summary>
            /// 获取方法描述
            /// </summary>
            /// <param name="method"></param>
            /// <returns></returns>
            public string GetSummary(MethodInfo method)
            {
                return get(get(method), "summary");
            }
            /// <summary>
            /// 获取方法返回值描述
            /// </summary>
            /// <param name="method"></param>
            /// <returns></returns>
            public string GetReturn(MethodInfo method)
            {
                return get(get(method), "returns");
            }
            /// <summary>
            /// 获取参数描述
            /// </summary>
            /// <param name="method"></param>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public unsafe string Get(MethodInfo method, ParameterInfo parameter)
            {
                xmlNode xmlNode = get(method);
                if (xmlNode.Type == emit.xmlNode.type.Node)
                {
                    string parameterName = parameter.Name;
                    xmlParser.attributeIndex attribute = default(xmlParser.attributeIndex);
                    fixed (char* nameFixed = "name", parameterFixed = parameterName)
                    {
                        foreach (keyValue<subString, xmlNode> node in xmlNode.Nodes)
                        {
                            if (node.Value.Type != emit.xmlNode.type.Node && node.Key.Equals("param")
                                && node.Value.GetAttribute(nameFixed, 4, ref attribute)
                                && attribute.Length == parameterName.Length)
                            {
                                fixed (char* attributeFixed = node.Key.value)
                                {
                                    if (fastCSharp.unsafer.memory.SimpleEqual((byte*)parameterFixed, (byte*)(attributeFixed + attribute.StartIndex), parameterName.Length << 1))
                                    {
                                        return node.Value.String.Length == 0 ? string.Empty : node.Value.String.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return string.Empty;
            }
            /// <summary>
            /// 加载数据记录
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="node"></param>
            public void LoadMember(subString name, xmlNode node)
            {//https://msdn.microsoft.com/zh-cn/library/fsbx0t7x(v=vs.80).aspx
            //对于泛型类型，类型名称后跟反勾号，再跟一个数字，指示泛型类型参数的个数。例如，
            //<member name="T:SampleClass`2"> 是定义为 public class SampleClass<T, U> 的类型的标记。
                if (name[1] == ':')
                {
                    char code = name[0];
                    switch (code & 7)
                    {
                        case 'T' & 7://类型：类、接口、结构、枚举、委托
                            if (code == 'T') types[name.Substring(2)] = node;
                            break;
                        case 'F' & 7://字段
                            if (code == 'F') fields[name.Substring(2)] = node;
                            break;
                        case 'P' & 7://属性（包括索引程序或其他索引属性）
                            if (code == 'P') properties[name.Substring(2)] = node;
                            break;
                        case 'M' & 7://方法（包括一些特殊方法，例如构造函数、运算符等）
                            if (code == 'M') methods[name.Substring(2)] = node;
                            break;
                        //case 'E' & 7://事件
                        //    break;
                        //case 'N' & 7://命名空间
                        //case '!' & 7://错误字符串
                        //break;
                    }
                }
            }
            /// <summary>
            /// 获取节点字符串
            /// </summary>
            /// <param name="node">成员节点</param>
            /// <param name="name">节点名称</param>
            /// <returns>字符串</returns>
            private static string get(xmlNode node, string name)
            {
                return (node = node[name]).String.Length != 0 ? node.String.ToString() : string.Empty;
            }
            /// <summary>
            /// 名称格式化
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            private static void formatName(char* start, char* end)
            {
                do
                {
                    if (*start == '&') *start = '@';
                }
                while (++start != end);
            }
        }
        /// <summary>
        /// 程序集信息集合
        /// </summary>
        private static readonly interlocked.classLastDictionary<Assembly, assembly> assemblyLock = new interlocked.classLastDictionary<Assembly,assembly>();
        /// <summary>
        /// XML解析配置
        /// </summary>
        private static fastCSharp.emit.xmlParser.config xmlParserConfig = new fastCSharp.emit.xmlParser.config { BootNodeName = "doc", IsAttribute = true };
        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private unsafe static assembly get(Assembly assembly)
        {
            if (assembly != null)
            {
                assembly value;
                if (assemblyLock.TryGetValue(assembly, out value)) return value;
                try
                {
                    string fileName = assembly.Location;
                    if (fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(fileName = fileName.Substring(0, fileName.Length - 3) + "xml"))
                        {
                            xmlNode xmlNode = fastCSharp.emit.xmlParser.Parse<xmlNode>(File.ReadAllText(fileName, Encoding.UTF8), xmlParserConfig)["members"];
                            if (xmlNode.Type == emit.xmlNode.type.Node)
                            {
                                fixed (char* nameFixed = "name")
                                {
                                    value = new assembly();
                                    xmlParser.attributeIndex attribute = default(xmlParser.attributeIndex);
                                    foreach (keyValue<subString, xmlNode> node in xmlNode.Nodes)
                                    {
                                        if (node.Value.Type == emit.xmlNode.type.Node && node.Key.Equals("member"))
                                        {
                                            if (node.Value.GetAttribute(nameFixed, 4, ref attribute) && attribute.Length > 2)
                                            {
                                                value.LoadMember(subString.Unsafe(node.Key.value, attribute.StartIndex, attribute.Length), node.Value);
                                            }
                                        }
                                    }
                                }
                            }
                            else log.Error.Real("XML文档解析失败 " + fileName, new System.Diagnostics.StackFrame(), false);
                        }
                        else log.Default.Real("没有找到XML文档注释 " + fileName, new System.Diagnostics.StackFrame(), false);
                    }
                    assemblyLock.Set(assembly, value);
                }
                finally { assemblyLock.Exit(); }
                return value;
            }
            return null;
        }
        /// <summary>
        /// 获取类型描述
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Get(Type type)
        {
            assembly assembly = get(type.Assembly);
            return assembly == null ? string.Empty : assembly.GetSummary(type);
        }
        /// <summary>
        /// 获取字段描述
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string Get(FieldInfo field)
        {
            assembly assembly = get(field.DeclaringType.Assembly);
            return assembly == null ? string.Empty : assembly.GetSummary(field);
        }
        /// <summary>
        /// 获取属性描述
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string Get(PropertyInfo property)
        {
            assembly assembly = get(property.DeclaringType.Assembly);
            return assembly == null ? string.Empty : assembly.GetSummary(property);
        }
        /// <summary>
        /// 获取方法描述
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string Get(MethodInfo method)
        {
            assembly assembly = get(method.DeclaringType.Assembly);
            return assembly == null ? string.Empty : assembly.GetSummary(method);
        }
        /// <summary>
        /// 获取方法返回值描述
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetReturn(MethodInfo method)
        {
            assembly assembly = get(method.DeclaringType.Assembly);
            return assembly == null ? string.Empty : assembly.GetReturn(method);
        }
        /// <summary>
        /// 获取参数描述
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string Get(MethodInfo method, ParameterInfo parameter)
        {
            assembly assembly = get(method.DeclaringType.Assembly);
            return assembly == null ? string.Empty : assembly.Get(method, parameter);
        }
    }
}
