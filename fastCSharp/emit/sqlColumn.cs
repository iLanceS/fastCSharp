using System;
using fastCSharp.code;
using System.Data.Common;
using fastCSharp.code.cSharp;
using fastCSharp.reflection;
using System.Reflection;
using fastCSharp.threading;
using System.Collections.Generic;
using fastCSharp.sql.expression;
using System.Runtime.CompilerServices;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// SQL列配置
    /// </summary>
    public class sqlColumn : fastCSharp.code.cSharp.sqlModel
    {
        /// <summary>
        /// 默认空属性
        /// </summary>
        internal new static readonly sqlColumn Default = new sqlColumn();
        /// <summary>
        /// 自定义类型标识配置
        /// </summary>
        public sealed class custom : Attribute
        {
        }
#if NOJIT
        /// <summary>
        /// 自定义类型处理接口
        /// </summary>
        public interface ICustom
        {
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="index">当前读取位置</param>
            void Set(DbDataReader reader, object value, ref int index);
            /// <summary>
            /// 数据验证
            /// </summary>
            /// <param name="value"></param>
            /// <param name="sqlTool"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
            bool Verify(object value, fastCSharp.emit.sqlTable.sqlToolBase sqlTool, string columnName);
            /// <summary>
            /// 获取,分割列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            string GetColumnNames(string name);
            /// <summary>
            /// 获取成员名称与类型集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            keyValue<string, Type>[] GetDataColumns(string name);
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            void Insert(charStream sqlStream, object value, constantConverter converter);
            /// <summary>
            /// 获取更新数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
            void Update(charStream sqlStream, object value, constantConverter converter, string columnName);
            /// <summary>
            /// 读取字段值
            /// </summary>
            /// <param name="value">数据列</param>
            /// <param name="values">目标数组</param>
            /// <param name="index">当前写入位置</param>
            void ToArray(object value, object[] values, ref int index);
            /// <summary>
            /// 获取添加SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
            void Where(charStream sqlStream, object value, constantConverter converter, string columnName);
        }
#else
        /// <summary>
        /// 自定义类型处理接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        public interface ICustom<valueType>
        {
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="index">当前读取位置</param>
            void Set(DbDataReader reader, ref valueType value, ref int index);
            /// <summary>
            /// 数据验证
            /// </summary>
            /// <param name="value"></param>
            /// <param name="sqlTool"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
            bool Verify(valueType value, fastCSharp.emit.sqlTable.sqlToolBase sqlTool, string columnName);
            /// <summary>
            /// 获取,分割列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            string GetColumnNames(string name);
            /// <summary>
            /// 获取成员名称与类型集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            keyValue<string, Type>[] GetDataColumns(string name);
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            void Insert(charStream sqlStream, valueType value, constantConverter converter);
            /// <summary>
            /// 获取更新数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
            void Update(charStream sqlStream, valueType value, constantConverter converter, string columnName);
            /// <summary>
            /// 读取字段值
            /// </summary>
            /// <param name="value">数据列</param>
            /// <param name="values">目标数组</param>
            /// <param name="index">当前写入位置</param>
            void ToArray(valueType value, object[] values, ref int index);
            /// <summary>
            /// 获取添加SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
            void Where(charStream sqlStream, valueType value, constantConverter converter, string columnName);
        }
#endif

        /// <summary>
        /// 动态函数
        /// </summary>
        public struct verifyDynamicMethod
        {
#if NOJIT
            /// <summary>
            /// 字段信息（反射模式）
            /// </summary>
            internal struct field
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
                /// <summary>
                /// SQL列验证
                /// </summary>
                public MethodInfo SqlColumnMethod;
                /// <summary>
                /// 设置字段信息
                /// </summary>
                /// <param name="field"></param>
                public void Set(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
                {
                    Field = field;
                    if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.verifyDynamicMethod.GetTypeVerifyer(field.DataType);
                }
            }
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public verifyDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnVerify", typeof(bool), new Type[] { type, typeof(fastCSharp.emit.sqlTable.sqlToolBase), typeof(string[]) }, type, true);
                generator = dynamicMethod.GetILGenerator();
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="index">名称序号</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field, int index)
            {
                Label end = generator.DefineLabel();
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarga_S, 0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.int32(index);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Call, GetTypeVerifyer(field.DataType));
                    generator.Emit(OpCodes.Brtrue_S, end);
                }
                else if (field.DataType == typeof(string) || field.IsUnknownJson)
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.int32(index);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Ldarga_S, 0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.int32(field.DataMember.MaxStringLength);
                    generator.Emit(field.DataMember.IsAscii ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    generator.Emit(field.DataMember.IsNull ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    generator.Emit(OpCodes.Callvirt, fastCSharp.emit.sqlTable.sqlToolBase.StringVerifyMethod);
                    generator.Emit(OpCodes.Brtrue_S, end);
                }
                else
                {
                    generator.Emit(OpCodes.Ldarga_S, 0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Brtrue_S, end);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.int32(index);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Callvirt, fastCSharp.emit.sqlTable.sqlToolBase.NullVerifyMethod);
                }
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ret);
                generator.MarkLabel(end);
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
#endif
            /// <summary>
            /// 类型调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeVerifyers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 类型委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>类型委托调用函数信息</returns>
            public static MethodInfo GetTypeVerifyer(Type type)
            {
                MethodInfo method;
                if (typeVerifyers.TryGetValue(type, out method)) return method;
                typeVerifyers.Set(type, method = verifyMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 数据验证
            /// </summary>
            /// <param name="value"></param>
            /// <param name="sqlTool"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static bool verify<valueType>(valueType value, fastCSharp.emit.sqlTable.sqlToolBase sqlTool, string columnName)
            {
                return sqlColumn<valueType>.verify.Verify(value, sqlTool, columnName);
            }
            /// <summary>
            /// 数据验证函数信息
            /// </summary>
            private static readonly MethodInfo verifyMethod = typeof(verifyDynamicMethod).GetMethod("verify", BindingFlags.Static | BindingFlags.NonPublic);
            /// <summary>
            /// 获取列名委托集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Action<list<string>, string>> getColumnNameMethods = new interlocked.dictionary<Type, Action<list<string>, string>>();
            /// <summary>
            /// 获取列名委托
            /// </summary>
            /// <param name="type">数据列类型</param>
            /// <returns>获取列名委托</returns>
            public static Action<list<string>, string> GetColumnNames(Type type)
            {
                Action<list<string>, string> getColumnName;
                if (getColumnNameMethods.TryGetValue(type, out getColumnName)) return getColumnName;
                getColumnName = (Action<list<string>, string>)Delegate.CreateDelegate(typeof(Action<list<string>, string>), getColumnNamesMethod.MakeGenericMethod(type));
                getColumnNameMethods.Set(type, getColumnName);
                return getColumnName;
            }
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="names">列名集合</param>
            /// <param name="name">列名前缀</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void getColumnNames<valueType>(list<string> names, string name)
            {
                names.Add(sqlColumn<valueType>.verify.GetColumnNames(name));
            }
            /// <summary>
            /// 获取列名集合函数信息
            /// </summary>
            private static readonly MethodInfo getColumnNamesMethod = typeof(verifyDynamicMethod).GetMethod("getColumnNames", BindingFlags.Static | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 动态函数
        /// </summary>
        public struct setDynamicMethod
        {
#if NOJIT
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public setDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnSet", null, new Type[] { typeof(DbDataReader), type.MakeByRefType(), pub.RefIntType }, type, true);
                generator = dynamicMethod.GetILGenerator();
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                if (field.DataReaderMethod == null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldflda, field.Field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Call, GetTypeSetter(field.DataType));
                }
                else
                {
                    if (field.DataType == field.NullableDataType && (field.DataType.IsValueType || !field.DataMember.IsNull))
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Callvirt, field.DataReaderMethod);
                        //if (field.IsUnknownJson)
                        generator.Emit(OpCodes.Call, field.ToModelCastMethod);
                        generator.Emit(OpCodes.Stfld, field.Field);
                    }
                    else
                    {
                        Label notNull = generator.DefineLabel(), end = generator.DefineLabel();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Callvirt, pub.DataReaderIsDBNullMethod);
                        generator.Emit(OpCodes.Brfalse_S, notNull);

                        generator.Emit(OpCodes.Ldarg_1);
                        if (field.DataType == field.NullableDataType)
                        {
                            generator.Emit(OpCodes.Ldnull);
                            generator.Emit(OpCodes.Stfld, field.Field);
                        }
                        else
                        {
                            generator.Emit(OpCodes.Ldflda, field.Field);
                            generator.Emit(OpCodes.Initobj, field.Field.FieldType);
                        }
                        generator.Emit(OpCodes.Br_S, end);

                        generator.MarkLabel(notNull);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Callvirt, field.DataReaderMethod);
                        if (field.DataType == field.NullableDataType)
                        {
                            if (field.ToModelCastMethod != null) generator.Emit(OpCodes.Call, field.ToModelCastMethod);
                        }
                        else generator.Emit(OpCodes.Newobj, pub.NullableConstructors[field.DataType]);
                        generator.Emit(OpCodes.Stfld, field.Field);
                        generator.MarkLabel(end);
                    }
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Ldind_I4);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Add);
                    generator.Emit(OpCodes.Stind_I4);
                }
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
#endif

            /// <summary>
            /// 类型调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeSetters = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 类型委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>类型委托调用函数信息</returns>
            internal static MethodInfo GetTypeSetter(Type type)
            {
                MethodInfo method;
                if (typeSetters.TryGetValue(type, out method)) return method;
                typeSetters.Set(type, method = setMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="index">当前读取位置</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void set<valueType>(DbDataReader reader, ref valueType value, ref int index)
            {
                sqlColumn<valueType>.set.Set(reader, ref value, ref index);
            }
            /// <summary>
            /// 设置字段值函数信息
            /// </summary>
            private static readonly MethodInfo setMethod = typeof(setDynamicMethod).GetMethod("set", BindingFlags.Static | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 数据列转换数组动态函数
        /// </summary>
        public struct toArrayDynamicMethod
        {
#if NOJIT
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public toArrayDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnToArray", null, new Type[] { type, typeof(object[]), pub.RefIntType }, type, true);
                generator = dynamicMethod.GetILGenerator();
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarga_S, 0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Call, sqlColumn.toArrayDynamicMethod.GetTypeToArray(field.DataType));
                }
                else
                {
                    if (field.DataType == field.NullableDataType)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Ldarga_S, 0);
                        generator.Emit(OpCodes.Ldfld, field.Field);
                        if (field.ToSqlCastMethod != null)
                        {
                            generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                            if (!field.IsUnknownJson && field.DataType.IsValueType) generator.Emit(OpCodes.Box, field.DataType);
                        }
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                    else
                    {
                        Label end = generator.DefineLabel();
                        generator.Emit(OpCodes.Ldarga_S, 0);
                        generator.Emit(OpCodes.Ldflda, field.Field);
                        generator.Emit(OpCodes.Call, pub.GetNullableHasValue(field.NullableDataType));
                        generator.Emit(OpCodes.Brtrue_S, end);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Ldarga_S, 0);
                        generator.Emit(OpCodes.Ldflda, field.Field);
                        generator.Emit(OpCodes.Call, pub.GetNullableValue(field.NullableDataType));
                        generator.Emit(OpCodes.Box, field.DataType);
                        generator.Emit(OpCodes.Stelem_Ref);
                        generator.MarkLabel(end);
                    }
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Ldind_I4);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Add);
                    generator.Emit(OpCodes.Stind_I4);
                }
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
#endif

            /// <summary>
            /// 类型调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeToArrays = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 类型委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>类型委托调用函数信息</returns>
            public static MethodInfo GetTypeToArray(Type type)
            {
                MethodInfo method;
                if (typeToArrays.TryGetValue(type, out method)) return method;
                typeToArrays.Set(type, method = toArrayMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 数据列转换数组
            /// </summary>
            /// <param name="values">目标数组</param>
            /// <param name="value">数据列</param>
            /// <param name="index">当前读取位置</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void toArray<valueType>(valueType value, object[] values, ref int index)
            {
                sqlColumn<valueType>.toArray.ToArray(value, values, ref index);
            }
            /// <summary>
            /// 数据列转换数组函数信息
            /// </summary>
            private static readonly MethodInfo toArrayMethod = typeof(toArrayDynamicMethod).GetMethod("toArray", BindingFlags.Static | BindingFlags.NonPublic);

            /// <summary>
            /// 获取列名与类型委托集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Func<string, keyValue<string, Type>[]>> getDataColumns = new interlocked.dictionary<Type, Func<string, keyValue<string, Type>[]>>();
            /// <summary>
            /// 获取列名与类型委托
            /// </summary>
            /// <param name="type">数据列类型</param>
            /// <returns>获取列名与类型委托</returns>
            public static Func<string, keyValue<string, Type>[]> GetDataColumns(Type type)
            {
                Func<string, keyValue<string, Type>[]> getDataColumn;
                if (getDataColumns.TryGetValue(type, out getDataColumn)) return getDataColumn;
                getDataColumn = (Func<string, keyValue<string, Type>[]>)Delegate.CreateDelegate(typeof(Func<string, keyValue<string, Type>[]>), getDataColumnsMethod.MakeGenericMethod(type));
                getDataColumns.Set(type, getDataColumn);
                return getDataColumn;
            }
        }
        /// <summary>
        /// 动态函数
        /// </summary>
        public struct insertDynamicMethod
        {
#if NOJIT
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 
            /// </summary>
            private bool isNextMember;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public insertDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnInsert", null, new Type[] { typeof(charStream), type, typeof(constantConverter) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isNextMember = false;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                if (isNextMember) generator.charStreamWriteChar(OpCodes.Ldarg_0, ',');
                else isNextMember = true;
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarga_S, 1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Call, GetTypeInsert(field.DataType));
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarga_S, 1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Callvirt, field.ToSqlMethod);
                }
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
#endif
            /// <summary>
            /// 类型调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeInserts = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 类型委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>类型委托调用函数信息</returns>
            public static MethodInfo GetTypeInsert(Type type)
            {
                MethodInfo method;
                if (typeInserts.TryGetValue(type, out method)) return method;
                typeInserts.Set(type, method = insertMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void insert<valueType>(charStream sqlStream, valueType value, constantConverter converter)
            {
                sqlColumn<valueType>.insert.Insert(sqlStream, value, converter);
            }
            /// <summary>
            /// 获取插入数据SQL表达式函数信息
            /// </summary>
            private static readonly MethodInfo insertMethod = typeof(insertDynamicMethod).GetMethod("insert", BindingFlags.Static | BindingFlags.NonPublic);

            /// <summary>
            /// 获取列名委托集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Func<string, string>> getColumnNameMethods = new interlocked.dictionary<Type, Func<string, string>>();
            /// <summary>
            /// 获取列名委托
            /// </summary>
            /// <param name="type">数据列类型</param>
            /// <returns>获取列名委托</returns>
            public static Func<string, string> GetColumnNames(Type type)
            {
                Func<string, string> getColumnName;
                if (getColumnNameMethods.TryGetValue(type, out getColumnName)) return getColumnName;
                getColumnName = (Func<string, string>)Delegate.CreateDelegate(typeof(Func<string, string>), getColumnNamesMethod.MakeGenericMethod(type));
                getColumnNameMethods.Set(type, getColumnName);
                return getColumnName;
            }
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static string getColumnNames<valueType>(string name)
            {
                return sqlColumn<valueType>.insert.GetColumnNames(name);
            }
            /// <summary>
            /// 获取列名集合函数信息
            /// </summary>
            private static readonly MethodInfo getColumnNamesMethod = typeof(insertDynamicMethod).GetMethod("getColumnNames", BindingFlags.Static | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 更新数据动态函数
        /// </summary>
        public struct updateDynamicMethod
        {
#if NOJIT
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 
            /// </summary>
            private bool isNextMember;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public updateDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnUpdate", null, new Type[] { typeof(charStream), type, typeof(constantConverter), typeof(string[]) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isNextMember = false;
            }
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="dynamicMethod"></param>
            /// <param name="generator"></param>
            public updateDynamicMethod(DynamicMethod dynamicMethod, ILGenerator generator)
            {
                this.dynamicMethod = dynamicMethod;
                this.generator = generator;
                isNextMember = false;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="index">字段名称序号</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field, int index)
            {
                if (isNextMember) generator.charStreamWriteChar(OpCodes.Ldarg_0, ',');
                else isNextMember = true;
                PushOnly(field, index);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="index">字段名称序号</param>
            public void PushOnly(fastCSharp.code.cSharp.sqlModel.fieldInfo field, int index)
            {
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarga_S, 1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.int32(index);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Call, GetTypeUpdate(field.DataType));
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.int32(index);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.charStreamSimpleWriteNotNull();
                    generator.charStreamWriteChar(OpCodes.Ldarg_0, '=');

                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarga_S, 1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Callvirt, field.ToSqlMethod);
                }
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
#endif
            /// <summary>
            /// 类型调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeUpdates = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 类型委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>类型委托调用函数信息</returns>
            public static MethodInfo GetTypeUpdate(Type type)
            {
                MethodInfo method;
                if (typeUpdates.TryGetValue(type, out method)) return method;
                typeUpdates.Set(type, method = updateMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 获取更新数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void update<valueType>(charStream sqlStream, valueType value, constantConverter converter, string columnName)
            {
                sqlColumn<valueType>.update.Update(sqlStream, value, converter, columnName);
            }
            /// <summary>
            /// 获取更新数据SQL表达式函数信息
            /// </summary>
            private static readonly MethodInfo updateMethod = typeof(updateDynamicMethod).GetMethod("update", BindingFlags.Static | BindingFlags.NonPublic);
            /// <summary>
            /// 获取列名委托集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Action<list<string>, string>> getColumnNameMethods = new interlocked.dictionary<Type, Action<list<string>, string>>();
            /// <summary>
            /// 获取列名委托
            /// </summary>
            /// <param name="type">数据列类型</param>
            /// <returns>获取列名委托</returns>
            public static Action<list<string>, string> GetColumnNames(Type type)
            {//showjim
                Action<list<string>, string> getColumnName;
                if (getColumnNameMethods.TryGetValue(type, out getColumnName)) return getColumnName;
                getColumnName = (Action<list<string>, string>)Delegate.CreateDelegate(typeof(Action<list<string>, string>), getColumnNamesMethod.MakeGenericMethod(type));
                getColumnNameMethods.Set(type, getColumnName);
                return getColumnName;
            }
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="names">列名集合</param>
            /// <param name="name">列名前缀</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void getColumnNames<valueType>(list<string> names, string name)
            {
                names.Add(sqlColumn<valueType>.update.GetColumnNames(name));
            }
            /// <summary>
            /// 获取列名集合函数信息
            /// </summary>
            private static readonly MethodInfo getColumnNamesMethod = typeof(updateDynamicMethod).GetMethod("getColumnNames", BindingFlags.Static | BindingFlags.NonPublic);
        }
#if NOJIT
#else
        /// <summary>
        /// 关键字条件动态函数
        /// </summary>
        public struct whereDynamicMethod
        {
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 更新数据动态函数
            /// </summary>
            private updateDynamicMethod updateDynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private bool isNextMember;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public whereDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlColumnWhere", null, new Type[] { typeof(charStream), type, typeof(constantConverter), typeof(string[]) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isNextMember = false;
                updateDynamicMethod = new updateDynamicMethod(dynamicMethod, generator);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="index">字段名称序号</param>
            public unsafe void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field, int index)
            {
                if (isNextMember) generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_0, sqlModel.AndString.Char, 5);
                else isNextMember = true;
                updateDynamicMethod.PushOnly(field, index);
            }
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
        }
#endif
        /// <summary>
        /// 获取成员名称与类型集合
        /// </summary>
        /// <param name="name">列名前缀</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static keyValue<string, Type>[] getDataColumns<valueType>(string name)
        {
            return sqlColumn<valueType>.GetDataColumns(name);
        }
        /// <summary>
        /// 获取成员名称与类型集合函数信息
        /// </summary>
        private static readonly MethodInfo getDataColumnsMethod = typeof(sqlColumn).GetMethod("getDataColumns", BindingFlags.Static | BindingFlags.NonPublic);
    }
    /// <summary>
    /// 数据列
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    internal static class sqlColumn<valueType>
    {
        /// <summary>
        /// 数据列验证
        /// </summary>
        internal static class verify
        {
            /// <summary>
            /// 数据列名集合
            /// </summary>
            private static readonly interlocked.lastDictionary<hashString, string[]> columnNames;
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            internal static string[] GetColumnNames(string name)
            {
                string[] names;
                hashString nameKey = name;
                if (columnNames.TryGetValue(ref nameKey, out names)) return names;
                list<string> nameList = new list<string>(verifyFields.Length);
                foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in verifyFields)
                {
                    if (field.IsSqlColumn) sqlColumn.verifyDynamicMethod.GetColumnNames(field.Field.FieldType)(nameList, name + "_" + field.Field.Name);
                    else nameList.Add(name + "_" + field.Field.Name);
                }
                columnNames.Set(ref nameKey, names = nameList.ToArray());
                return names;
            }
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly fastCSharp.code.cSharp.sqlModel.fieldInfo[] verifyFields;
            /// <summary>
            /// 数据验证
            /// </summary>
            /// <param name="value"></param>
            /// <param name="sqlTool"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static bool Verify(valueType value, fastCSharp.emit.sqlTable.sqlToolBase sqlTool, string columnName)
            {
#if NOJIT
                if (fields != null)
                {
                    string[] columnNames = GetColumnNames(columnName);
                    object[] sqlColumnParameters = null, castParameters = null;
                    object objectValue = value;
                    int index = 0;
                    foreach (sqlColumn.verifyDynamicMethod.field field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        object memberValue = fieldInfo.Field.GetValue(objectValue);
                        if (fieldInfo.IsSqlColumn)
                        {
                            if (sqlColumnParameters == null) sqlColumnParameters = new object[] { null, sqlTool, null };
                            sqlColumnParameters[0] = memberValue;
                            sqlColumnParameters[2] = columnNames[index];
                            if (!(bool)field.SqlColumnMethod.Invoke(null, sqlColumnParameters)) return false;
                        }
                        else if (fieldInfo.DataType == typeof(string))
                        {
                            if (fieldInfo.ToSqlCastMethod != null)
                            {
                                if (castParameters == null) castParameters = new object[1];
                                castParameters[0] = memberValue;
                                memberValue = fieldInfo.ToSqlCastMethod.Invoke(null, castParameters);
                            }
                            dataMember dataMember = fieldInfo.DataMember;
                            if (!sqlTool.StringVerify(columnNames[index], (string)memberValue, dataMember.MaxStringLength, dataMember.IsAscii, dataMember.IsNull)) return false;
                        }
                        else
                        {
                            if (fieldInfo.ToSqlCastMethod != null && !fieldInfo.IsUnknownJson)
                            {
                                if (castParameters == null) castParameters = new object[1];
                                castParameters[0] = memberValue;
                                memberValue = fieldInfo.ToSqlCastMethod.Invoke(null, castParameters);
                            }
                            if (memberValue == null)
                            {
                                sqlTool.NullVerify(columnNames[index]);
                                return false;
                            }
                        }
                        ++index;
                    }
                    return true;
                }
#else
                if (verifyer != null) return verifyer(value, sqlTool, GetColumnNames(columnName));
#endif
                return custom == null || custom.Verify(value, sqlTool, columnName);
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlColumn.verifyDynamicMethod.field[] fields;
#else
            /// <summary>
            /// 数据验证
            /// </summary>
            private static readonly Func<valueType, fastCSharp.emit.sqlTable.sqlToolBase, string[], bool> verifyer;
#endif
            static verify()
            {
                if (attribute != null && custom == null && Fields != null)
                {
                    subArray<fastCSharp.code.cSharp.sqlModel.fieldInfo> verifyFields = Fields.getFind(value => value.IsVerify);
                    if (verifyFields.length != 0)
                    {
                        columnNames = new interlocked.lastDictionary<hashString,string[]>();
                        int index = 0;
                        verify.verifyFields = verifyFields.ToArray();
#if NOJIT
                        fields = new sqlColumn.verifyDynamicMethod.field[verifyFields.length];
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in verify.verifyFields) fields[index++].Set(member);
#else
                        sqlColumn.verifyDynamicMethod dynamicMethod = new sqlColumn.verifyDynamicMethod(typeof(valueType));
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in verify.verifyFields) dynamicMethod.Push(member, index++);
                        verifyer = (Func<valueType, fastCSharp.emit.sqlTable.sqlToolBase, string[], bool>)dynamicMethod.Create<Func<valueType, fastCSharp.emit.sqlTable.sqlToolBase, string[], bool>>();
#endif
                    }
                }
            }
        }
        /// <summary>
        /// 数据列设置
        /// </summary>
        internal static class set
        {
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="index">当前读取位置</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Set(DbDataReader reader, ref valueType value, ref int index)
            {
#if NOJIT
                if (fields != null)
                {
                    object[] sqlColumnParameters = null, castParameters = null;
                    object objectValue = value;
                    foreach (sqlModel.setField field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (fieldInfo.DataReaderMethod == null)
                        {
                            if (sqlColumnParameters == null) sqlColumnParameters = new object[] { reader, null, null };
                            sqlColumnParameters[1] = null;
                            sqlColumnParameters[2] = index;
                            field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                            fieldInfo.Field.SetValue(objectValue, sqlColumnParameters[1]);
                            index = (int)sqlColumnParameters[2];
                        }
                        else
                        {
                            object memberValue;
                            if (fieldInfo.DataType == fieldInfo.NullableDataType && (fieldInfo.DataType.IsValueType || !fieldInfo.DataMember.IsNull))
                            {
                                memberValue = reader[index++];
                                if (fieldInfo.ToModelCastMethod != null)
                                {
                                    if (castParameters == null) castParameters = new object[1];
                                    castParameters[0] = memberValue;
                                    memberValue = fieldInfo.ToModelCastMethod.Invoke(null, castParameters);
                                }
                            }
                            else if (reader.IsDBNull(index))
                            {
                                memberValue = null;
                                ++index;
                            }
                            else
                            {
                                memberValue = reader[index++];
                                if (fieldInfo.ToModelCastMethod != null && fieldInfo.DataType == fieldInfo.NullableDataType)
                                {
                                    if (castParameters == null) castParameters = new object[1];
                                    castParameters[0] = memberValue;
                                    memberValue = fieldInfo.ToModelCastMethod.Invoke(null, castParameters);
                                }
                            }
                            fieldInfo.Field.SetValue(objectValue, memberValue);
                        }
                    }
                    value = (valueType)objectValue;
                }
                else if (custom != null)
                {
                    object objectValue = value;
                    custom.Set(reader, objectValue, ref index);
                    value = (valueType)objectValue;
                }
#else
                if (defaultSetter != null) defaultSetter(reader, ref value, ref index);
                else if (custom != null) custom.Set(reader, ref value, ref index);
#endif
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.setField[] fields;
#else
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="index">当前读取位置</param>
            private delegate void setter(DbDataReader reader, ref valueType value, ref int index);
            /// <summary>
            /// 默认数据列设置
            /// </summary>
            private static readonly setter defaultSetter;
#endif

            static set()
            {
                if (attribute != null && custom == null && Fields != null)
                {
#if NOJIT
                    fields = new sqlModel.setField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlColumn.setDynamicMethod dynamicMethod = new sqlColumn.setDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    defaultSetter = (setter)dynamicMethod.Create<setter>();
#endif
                }
            }
        }
        /// <summary>
        /// 数据列转换数组
        /// </summary>
        internal static class toArray
        {
            /// <summary>
            /// 数据列转换数组
            /// </summary>
            /// <param name="values">目标数组</param>
            /// <param name="value">数据列</param>
            /// <param name="index">当前读取位置</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void ToArray(valueType value, object[] values, ref int index)
            {
#if NOJIT
                if (fields != null)
                {
                    object[] sqlColumnParameters = null, castParameters = null;
                    object objectValue = value;
                    foreach (sqlModel.toArrayField field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (fieldInfo.IsSqlColumn)
                        {
                            if (sqlColumnParameters == null) sqlColumnParameters = new object[] { null, values, null };
                            sqlColumnParameters[0] = fieldInfo.Field.GetValue(objectValue);
                            sqlColumnParameters[2] = index;
                            field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                            index = (int)sqlColumnParameters[2];
                        }
                        else
                        {
                            object memberValue = fieldInfo.Field.GetValue(objectValue);
                            if (field.NullableHasValueMethod == null)
                            {
                                if (fieldInfo.ToSqlCastMethod != null)
                                {
                                    if (castParameters == null) castParameters = new object[1];
                                    castParameters[0] = memberValue;
                                    memberValue = fieldInfo.ToSqlCastMethod.Invoke(null, castParameters);
                                }
                            }
                            else
                            {
                                memberValue = (bool)field.NullableHasValueMethod.Invoke(memberValue, null) ? field.NullableValueMethod.Invoke(memberValue, null) : null;
                            }
                            values[index++] = memberValue;
                        }
                    }
                }
#else
                if (defaultWriter != null) defaultWriter(value, values, ref index);
#endif
                else if (custom != null) custom.ToArray(value, values, ref index);
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.toArrayField[] fields;
#else
            /// <summary>
            /// 数据列转换数组
            /// </summary>
            /// <param name="values">目标数组</param>
            /// <param name="value">数据列</param>
            /// <param name="index">当前读取位置</param>
            private delegate void writer(valueType value, object[] values, ref int index);
            /// <summary>
            /// 数据列转换数组
            /// </summary>
            private static readonly writer defaultWriter;
#endif

            static toArray()
            {
                if (attribute != null && custom == null && Fields != null)
                {
#if NOJIT
                    fields = new sqlModel.toArrayField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlColumn.toArrayDynamicMethod dynamicMethod = new sqlColumn.toArrayDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    defaultWriter = (writer)dynamicMethod.Create<writer>();
#endif
                }
            }
        }
        /// <summary>
        /// 数据列添加SQL流
        /// </summary>
        internal static class insert
        {
            /// <summary>
            /// 数据列名集合
            /// </summary>
            private static readonly interlocked.lastDictionary<hashString, string> columnNames;
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            public unsafe static string GetColumnNames(string name)
            {
                if (custom != null) return custom.GetColumnNames(name);
                if (columnNames != null)
                {
                    string names;
                    hashString nameKey = name;
                    if (columnNames.TryGetValue(ref nameKey, out names)) return names;
                    int isNext = 0;
                    pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
                    try
                    {
                        using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                        {
                            foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                            {
                                if (field.IsSqlColumn)
                                {
                                    if ((names = sqlColumn.insertDynamicMethod.GetColumnNames(field.Field.FieldType)(name + "_" + field.Field.Name)) != null)
                                    {
                                        if (isNext == 0) isNext = 1;
                                        else sqlStream.Write(',');
                                        sqlStream.Write(names);
                                    }
                                }
                                else
                                {
                                    if (isNext == 0) isNext = 1;
                                    else sqlStream.Write(',');
                                    sqlStream.PrepLength(name.Length + field.Field.Name.Length + 1);
                                    sqlStream.SimpleWriteNotNull(name);
                                    sqlStream.Write('_');
                                    sqlStream.SimpleWriteNotNull(field.Field.Name);
                                }
                            }
                            names = sqlStream.Length == 0 ? null : sqlStream.ToString();
                            columnNames.Set(ref nameKey, names);
                        }
                    }
                    finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
                    return names;
                }
                return null;
            }
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Insert(charStream sqlStream, valueType value, constantConverter converter)
            {
#if NOJIT
                if (fields != null)
                {
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    object objectValue = value;
                    byte isNext = 0;
                    foreach (sqlModel.insertField field in fields)
                    {
                        if (isNext == 0) isNext = 1;
                        else sqlStream.Write(',');
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (fieldInfo.IsSqlColumn)
                        {
                            if (sqlColumnParameters == null) sqlColumnParameters = new object[] { sqlStream, null, converter };
                            sqlColumnParameters[1] = fieldInfo.Field.GetValue(objectValue);
                            field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                        }
                        else
                        {
                            object memberValue = fieldInfo.Field.GetValue(objectValue);
                            if (fieldInfo.ToSqlCastMethod != null)
                            {
                                if (castParameters == null) castParameters = new object[1];
                                castParameters[0] = memberValue;
                                memberValue = fieldInfo.ToSqlCastMethod.Invoke(null, castParameters);
                            }
                            if (parameters == null) parameters = new object[] { sqlStream, null };
                            parameters[1] = memberValue;
                            fieldInfo.ToSqlMethod.Invoke(converter, parameters);
                        }
                    }
                }
#else
                if (inserter != null) inserter(sqlStream, value, converter);
#endif
                else if (custom != null) custom.Insert(sqlStream, value, converter);
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.insertField[] fields;
#else
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            private static readonly Action<charStream, valueType, constantConverter> inserter;
#endif

            static insert()
            {
                if (attribute != null && custom == null && Fields != null)
                {
                    columnNames = new interlocked.lastDictionary<hashString, string>();
#if NOJIT
                    fields = new sqlModel.insertField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlColumn.insertDynamicMethod dynamicMethod = new sqlColumn.insertDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    inserter = (Action<charStream, valueType, constantConverter>)dynamicMethod.Create<Action<charStream, valueType, constantConverter>>();
#endif
                }
            }
        }
        /// <summary>
        /// 数据列更新SQL流
        /// </summary>
        internal static class update
        {
            /// <summary>
            /// 数据列名集合
            /// </summary>
            private static readonly interlocked.lastDictionary<hashString, string[]> columnNames;
            /// <summary>
            /// 获取列名集合
            /// </summary>
            /// <param name="name">列名前缀</param>
            /// <returns></returns>
            public unsafe static string[] GetColumnNames(string name)
            {
                string[] names;
                hashString nameKey = name;
                if (columnNames.TryGetValue(ref nameKey, out names)) return names;
                list<string> nameList = new list<string>(Fields.Length);
                foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                {
                    if (field.IsSqlColumn) sqlColumn.updateDynamicMethod.GetColumnNames(field.Field.FieldType)(nameList, name + "_" + field.Field.Name);
                    else nameList.Add(name + "_" + field.Field.Name);
                }
                columnNames.Set(ref nameKey, names = nameList.ToArray());
                return names;
            }
            /// <summary>
            /// 获取更新数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Update(charStream sqlStream, valueType value, constantConverter converter, string columnName)
            {
#if NOJIT
                if (fields != null)
                {
                    string[] columnNames = GetColumnNames(columnName);
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    object objectValue = value;
                    int index = 0;
                    foreach (sqlModel.updateField field in fields)
                    {
                        if (index != 0) sqlStream.Write(',');
                        field.Set(sqlStream, objectValue, converter, columnNames[index++], ref sqlColumnParameters, ref castParameters, ref parameters);
                    }
                }
#else
                if (updater != null) updater(sqlStream, value, converter, GetColumnNames(columnName));
#endif
                else if (custom != null) custom.Update(sqlStream, value, converter, columnName);
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.updateField[] fields;
#else
            /// <summary>
            /// 获取更新数据SQL表达式
            /// </summary>
            private static readonly Action<charStream, valueType, constantConverter, string[]> updater;
#endif

            static update()
            {
                if (attribute != null && custom == null && Fields != null)
                {
                    columnNames = new interlocked.lastDictionary<hashString, string[]>();
                    int index = 0;
#if NOJIT
                    fields = new sqlModel.updateField[Fields.Length];
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlColumn.updateDynamicMethod dynamicMethod = new sqlColumn.updateDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member, index++);
                    updater = (Action<charStream, valueType, constantConverter, string[]>)dynamicMethod.Create<Action<charStream, valueType, constantConverter, string[]>>();
#endif
                }
            }
        }
        /// <summary>
        /// 条件
        /// </summary>
        internal static class where
        {
            /// <summary>
            /// 条件SQL流
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
            /// <param name="columnName">列名前缀</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Where(charStream sqlStream, valueType value, constantConverter converter, string columnName)
            {
#if NOJIT
                if (fields != null)
                {
                    string[] columnNames = update.GetColumnNames(columnName);
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    object objectValue = value;
                    int index = 0;
                    foreach (sqlModel.updateField field in fields)
                    {
                        if (index != 0) sqlStream.WriteNotNull(" and ");
                        field.Set(sqlStream, objectValue, converter, columnNames[index++], ref sqlColumnParameters, ref castParameters, ref parameters);
                    }
                }
#else
                if (getWhere != null) getWhere(sqlStream, value, converter, update.GetColumnNames(columnName));
#endif
                else if (custom != null) custom.Where(sqlStream, value, converter, columnName);
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.updateField[] fields;
#else
            /// <summary>
            /// 条件SQL流
            /// </summary>
            private static readonly Action<charStream, valueType, constantConverter, string[]> getWhere;
#endif

            static where()
            {
                if (attribute != null && custom == null && Fields != null)
                {
                    int index = 0;
#if NOJIT
                    fields = new sqlModel.updateField[Fields.Length];
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlColumn.whereDynamicMethod dynamicMethod = new sqlColumn.whereDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member, index++);
                    getWhere = (Action<charStream, valueType, constantConverter, string[]>)dynamicMethod.Create<Action<charStream, valueType, constantConverter, string[]>>();
#endif
                }
            }
        }

        /// <summary>
        /// 数据列名与类型集合
        /// </summary>
        private static readonly interlocked.lastDictionary<hashString, keyValue<string, Type>[]> dataColumns = new interlocked.lastDictionary<hashString, keyValue<string, Type>[]>();
        /// <summary>
        /// 获取成员名称与类型集合
        /// </summary>
        /// <param name="name">列名前缀</param>
        /// <returns></returns>
        internal static keyValue<string, Type>[] GetDataColumns(string name)
        {
            if (custom != null) return custom.GetDataColumns(name);
            if (Fields != null)
            {
                keyValue<string, Type>[] values;
                hashString nameKey = name;
                if (dataColumns.TryGetValue(ref nameKey, out values)) return values;
                subArray<keyValue<string, Type>> columns = new subArray<keyValue<string, Type>>(Fields.Length);
                foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                {
                    if (field.IsSqlColumn) sqlColumn.toArrayDynamicMethod.GetDataColumns(field.DataType)(name + "_" + field.Field.Name);
                    else columns.Add(new keyValue<string, Type>(name + "_" + field.Field.Name, field.DataType));
                }
                values = columns.ToArray();
                dataColumns.Set(ref nameKey, values);
                return values;
            }
            return null;
        }
        /// <summary>
        /// SQL列配置
        /// </summary>
        private static readonly sqlColumn attribute;
        /// <summary>
        /// 自定义类型处理接口
        /// </summary>
#if NOJIT
        private static readonly sqlColumn.ICustom custom;
#else
        private static readonly sqlColumn.ICustom<valueType> custom;
#endif
        /// <summary>
        /// 字段集合
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.sqlModel.fieldInfo[] Fields;
        static sqlColumn()
        {
            Type type = typeof(valueType);
            if (type.IsEnum || !type.IsValueType)
            {
                log.Error.Add(type.fullName() + " 非值类型，不能用作数据列", new System.Diagnostics.StackFrame(), false);
                return;
            }
            attribute = fastCSharp.code.typeAttribute.GetAttribute<sqlColumn>(type, true, true) ?? sqlColumn.Default;
            foreach (fastCSharp.code.attributeMethod method in fastCSharp.code.attributeMethod.GetStatic(type))
            {
#if NOJIT
                if (typeof(sqlColumn.ICustom).IsAssignableFrom(method.Method.ReflectedType)
#else
                if (typeof(sqlColumn.ICustom<valueType>).IsAssignableFrom(method.Method.ReflectedType)
#endif
                    && method.Method.GetParameters().Length == 0 && method.GetAttribute<sqlColumn.custom>(true) != null)
                {
                    object customValue = method.Method.Invoke(null, null);
                    if (customValue != null)
                    {
#if NOJIT
                        custom = (sqlColumn.ICustom)customValue;
#else
                        custom = (sqlColumn.ICustom<valueType>)customValue;
#endif
                        return;
                    }
                }
            }
            Fields = fastCSharp.code.cSharp.sqlModel.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter)).ToArray();
        }
    }
}
