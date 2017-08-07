using System;
using System.Reflection;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using fastCSharp.sql.expression;
using System.Runtime.CompilerServices;
using System.Data.Common;
using fastCSharp.sql;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 数据库表格模型
    /// </summary>
    internal unsafe static class sqlModel
    {
#if NOJIT
        /// <summary>
        /// 字段信息（反射模式）
        /// </summary>
        internal struct verifyField
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
            /// <summary>
            /// 是否SQL数据验证接口
            /// </summary>
            public bool IsSqlVerify;
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
                if (typeof(fastCSharp.emit.sqlTable.ISqlVerify).IsAssignableFrom(field.Field.FieldType)) IsSqlVerify = true;
                else if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.verifyDynamicMethod.GetTypeVerifyer(field.DataType);
            }
        }
        /// <summary>
        /// 字段信息（反射模式）
        /// </summary>
        internal struct setField
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
            /// <summary>
            /// SQL列转换转换成对象
            /// </summary>
            public MethodInfo SqlColumnMethod;
            /// <summary>
            /// 设置字段信息
            /// </summary>
            /// <param name="field"></param>
            public void Set(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Field = field;
                if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.setDynamicMethod.GetTypeSetter(field.DataType);
            }
        }
        /// <summary>
        /// 字段信息（反射模式）
        /// </summary>
        internal struct toArrayField
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
            /// <summary>
            /// SQL列转换数组
            /// </summary>
            public MethodInfo SqlColumnMethod;
            /// <summary>
            /// 可空类型判断是否存在值
            /// </summary>
            public MethodInfo NullableHasValueMethod;
            /// <summary>
            /// 获取可空类型数据
            /// </summary>
            public MethodInfo NullableValueMethod;
            /// <summary>
            /// 设置字段信息
            /// </summary>
            /// <param name="field"></param>
            public void Set(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Field = field;
                if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.toArrayDynamicMethod.GetTypeToArray(field.DataType);
                else if (field.DataType != field.NullableDataType)
                {
                    NullableHasValueMethod = pub.GetNullableHasValue(field.NullableDataType);
                    NullableValueMethod = pub.GetNullableValue(field.NullableDataType);
                }
            }
        }
        /// <summary>
        /// 字段信息（反射模式）
        /// </summary>
        internal struct insertField
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
            /// <summary>
            /// SQL列转换SQL字符串
            /// </summary>
            public MethodInfo SqlColumnMethod;
            /// <summary>
            /// 设置字段信息
            /// </summary>
            /// <param name="field"></param>
            public void Set(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Field = field;
                if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.insertDynamicMethod.GetTypeInsert(field.DataType);
            }
        }
        /// <summary>
        /// 字段信息（反射模式）
        /// </summary>
        internal struct updateField
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public fastCSharp.code.cSharp.sqlModel.fieldInfo Field;
            /// <summary>
            /// SQL列转换SQL字符串
            /// </summary>
            public MethodInfo SqlColumnMethod;
            /// <summary>
            /// 设置字段信息
            /// </summary>
            /// <param name="field"></param>
            public void Set(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Field = field;
                if (field.IsSqlColumn) SqlColumnMethod = sqlColumn.updateDynamicMethod.GetTypeUpdate(field.DataType);
            }
            /// <summary>
            /// 赋值语句字符串
            /// </summary>
            /// <param name="sqlStream"></param>
            /// <param name="value"></param>
            /// <param name="converter"></param>
            /// <param name="sqlColumnParameters"></param>
            /// <param name="castParameters"></param>
            /// <param name="parameters"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(charStream sqlStream, object value, constantConverter converter, ref object[] sqlColumnParameters, ref object[] castParameters, ref object[] parameters)
            {
                if (Field.IsSqlColumn)
                {
                    if (sqlColumnParameters == null) sqlColumnParameters = new object[] { sqlStream, null, converter, null };
                    sqlColumnParameters[1] = Field.Field.GetValue(value);
                    sqlColumnParameters[3] = Field.Field.Name;
                    SqlColumnMethod.Invoke(null, sqlColumnParameters);
                }
                else
                {
                    sqlStream.SimpleWriteNotNull(Field.SqlFieldName);
                    sqlStream.Write('=');
                    object memberValue = Field.Field.GetValue(value);
                    if (Field.ToSqlCastMethod != null)
                    {
                        if (castParameters == null) castParameters = new object[1];
                        castParameters[0] = memberValue;
                        memberValue = Field.ToSqlCastMethod.Invoke(null, castParameters);
                    }
                    if (parameters == null) parameters = new object[] { sqlStream, null };
                    parameters[1] = memberValue;
                    Field.ToSqlMethod.Invoke(converter, parameters);
                }
            }
            /// <summary>
            /// 赋值语句字符串
            /// </summary>
            /// <param name="sqlStream"></param>
            /// <param name="value"></param>
            /// <param name="converter"></param>
            /// <param name="columnName"></param>
            /// <param name="sqlColumnParameters"></param>
            /// <param name="castParameters"></param>
            /// <param name="parameters"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(charStream sqlStream, object value, constantConverter converter, string columnName, ref object[] sqlColumnParameters, ref object[] castParameters, ref object[] parameters)
            {
                if (Field.IsSqlColumn)
                {
                    if (sqlColumnParameters == null) sqlColumnParameters = new object[] { sqlStream, null, converter, null };
                    sqlColumnParameters[1] = Field.Field.GetValue(value);
                    sqlColumnParameters[3] = columnName;
                    SqlColumnMethod.Invoke(null, sqlColumnParameters);
                }
                else
                {
                    sqlStream.SimpleWriteNotNull(columnName);
                    sqlStream.Write('=');
                    object memberValue = Field.Field.GetValue(value);
                    if (Field.ToSqlCastMethod != null)
                    {
                        if (castParameters == null) castParameters = new object[1];
                        castParameters[0] = memberValue;
                        memberValue = Field.ToSqlCastMethod.Invoke(null, castParameters);
                    }
                    if (parameters == null) parameters = new object[] { sqlStream, null };
                    parameters[1] = memberValue;
                    Field.ToSqlMethod.Invoke(converter, parameters);
                }
            }
        }
#else
        /// <summary>
        /// SQL数据验证函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> sqlVerifyMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// SQL数据验证调用函数信息
        /// </summary>
        /// <param name="type">数组类型</param>
        /// <returns>SQL数据验证调用函数</returns>
        internal static MethodInfo GetSqlVerifyMethod(Type type)
        {
            if (typeof(fastCSharp.emit.sqlTable.ISqlVerify).IsAssignableFrom(type))
            {
                MethodInfo method;
                if (sqlVerifyMethods.TryGetValue(type, out method)) return method;
                sqlVerifyMethods.Set(type, method = type.GetMethod("IsSqlVeify", BindingFlags.Instance | BindingFlags.Public, null, nullValue<Type>.Array, null));
                return method;
            }
            return null;
        }
        /// <summary>
        /// 数据列验证动态函数
        /// </summary>
        public struct verifyDynamicMethod
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
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public verifyDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelVerify", typeof(bool), new Type[] { type, typeof(fastCSharp.code.memberMap), typeof(fastCSharp.emit.sqlTable.sqlToolBase) }, type, true);
                generator = dynamicMethod.GetILGenerator();
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Label end = generator.DefineLabel();
                generator.memberMapIsMember(OpCodes.Ldarg_1, field.MemberMapIndex);
                generator.Emit(OpCodes.Brfalse_S, end);
                MethodInfo sqlVerifyMethod = GetSqlVerifyMethod(field.Field.FieldType);
                if (sqlVerifyMethod != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    if (field.Field.FieldType.IsValueType)
                    {
                        generator.Emit(OpCodes.Ldflda, field.Field);
                        generator.Emit(OpCodes.Call, sqlVerifyMethod);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldfld, field.Field);
                        generator.Emit(OpCodes.Brfalse_S, end);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field.Field);
                        generator.Emit(OpCodes.Callvirt, sqlVerifyMethod);
                    }
                    generator.Emit(OpCodes.Brtrue_S, end);
                }
                else if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldstr, field.Field.Name);
                    generator.Emit(OpCodes.Call, sqlColumn.verifyDynamicMethod.GetTypeVerifyer(field.DataType));
                    generator.Emit(OpCodes.Brtrue_S, end);
                }
                else if (field.DataType == typeof(string) || field.IsUnknownJson)
                {
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldstr, field.Field.Name);
                    generator.Emit(OpCodes.Ldarg_0);
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
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Brtrue_S, end);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldstr, field.Field.Name);
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
        }
        /// <summary>
        /// 数据库模型设置动态函数
        /// </summary>
        public struct setDynamicMethod
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
            /// 
            /// </summary>
            private LocalBuilder indexMember;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public setDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelSet", null, new Type[] { typeof(DbDataReader), type, typeof(fastCSharp.code.memberMap) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                indexMember = generator.DeclareLocal(typeof(int));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Stloc_0);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Label notMember = generator.DefineLabel();
                generator.memberMapIsMember(OpCodes.Ldarg_2, field.MemberMapIndex);
                generator.Emit(OpCodes.Brfalse_S, notMember);
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldflda, field.Field);
                    generator.Emit(OpCodes.Ldloca_S, indexMember);
                    generator.Emit(OpCodes.Call, sqlColumn.setDynamicMethod.GetTypeSetter(field.DataType));
                }
                else
                {
                    if (field.DataType == field.NullableDataType && (field.DataType.IsValueType || !field.DataMember.IsNull))
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Callvirt, field.DataReaderMethod);
                        if (field.ToModelCastMethod != null) generator.Emit(OpCodes.Call, field.ToModelCastMethod);
                        generator.Emit(OpCodes.Stfld, field.Field);
                    }
                    else
                    {
                        Label notNull = generator.DefineLabel(), end = generator.DefineLabel();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldloc_0);
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
                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Callvirt, field.DataReaderMethod);
                        if (field.DataType == field.NullableDataType)
                        {
                            if (field.ToModelCastMethod != null) generator.Emit(OpCodes.Call, field.ToModelCastMethod);
                        }
                        else generator.Emit(OpCodes.Newobj, pub.NullableConstructors[field.DataType]);
                        generator.Emit(OpCodes.Stfld, field.Field);
                        generator.MarkLabel(end);
                    }
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Add);
                    generator.Emit(OpCodes.Stloc_0);
                }
                generator.MarkLabel(notMember);
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
        /// <summary>
        /// 数据列转换数组动态函数
        /// </summary>
        public struct toArrayDynamicMethod
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
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public toArrayDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelToArray", null, new Type[] { type, typeof(object[]), pub.RefIntType }, type, true);
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
                    generator.Emit(OpCodes.Ldarg_0);
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
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field.Field);
                        if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                        if (!field.IsUnknownJson && field.DataType.IsValueType) generator.Emit(OpCodes.Box, field.DataType);
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                    else
                    {
                        Label end = generator.DefineLabel();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field.Field);
                        generator.Emit(OpCodes.Call, pub.GetNullableHasValue(field.NullableDataType));
                        generator.Emit(OpCodes.Brtrue_S, end);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldind_I4);
                        generator.Emit(OpCodes.Ldarg_0);
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
        }
        /// <summary>
        /// 添加数据动态函数
        /// </summary>
        public struct insertDynamicMethod
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
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public insertDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelInsert", null, new Type[] { typeof(charStream), typeof(fastCSharp.code.memberMap), type, typeof(constantConverter) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                generator.DeclareLocal(typeof(int));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Stloc_0);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Label end = generator.DefineLabel(), isNext = generator.DefineLabel(), insert = generator.DefineLabel();
                generator.memberMapIsMember(OpCodes.Ldarg_1, field.MemberMapIndex);
                generator.Emit(OpCodes.Brfalse_S, end);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Brtrue_S, isNext);
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Br_S, insert);
                generator.MarkLabel(isNext);
                generator.charStreamWriteChar(OpCodes.Ldarg_0, ',');
                generator.MarkLabel(insert);
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.Emit(OpCodes.Call, sqlColumn.insertDynamicMethod.GetTypeInsert(field.DataType));
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Callvirt, field.ToSqlMethod);
                }
                generator.MarkLabel(end);
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
        /// <summary>
        /// 更新数据动态函数
        /// </summary>
        public struct updateDynamicMethod
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
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public updateDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelUpdate", null, new Type[] { typeof(charStream), typeof(fastCSharp.code.memberMap), type, typeof(constantConverter) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                generator.DeclareLocal(typeof(int));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Stloc_0);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                Label end = generator.DefineLabel(), isNext = generator.DefineLabel(), update = generator.DefineLabel();
                generator.memberMapIsMember(OpCodes.Ldarg_1, field.MemberMapIndex);
                generator.Emit(OpCodes.Brfalse_S, end);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Brtrue_S, isNext);
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Br_S, update);
                generator.MarkLabel(isNext);
                generator.charStreamWriteChar(OpCodes.Ldarg_0, ',');
                generator.MarkLabel(update);
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.Emit(OpCodes.Ldstr, field.Field.Name);
                    generator.Emit(OpCodes.Call, sqlColumn.updateDynamicMethod.GetTypeUpdate(field.DataType));
                }
                else
                {
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_0, pub.GetNameAssignmentPool(field.SqlFieldName), field.SqlFieldName.Length + 1);
                    generator.Emit(OpCodes.Ldarg_3);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    if (field.ToSqlCastMethod != null) generator.Emit(OpCodes.Call, field.ToSqlCastMethod);
                    generator.Emit(OpCodes.Callvirt, field.ToSqlMethod);
                }
                generator.MarkLabel(end);
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
        /// <summary>
        /// 关键字条件动态函数
        /// </summary>
        public struct primaryKeyWhereDynamicMethod
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
            /// 
            /// </summary>
            private bool isNextMember;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public primaryKeyWhereDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("sqlModelPrimaryKeyWhere", null, new Type[] { typeof(charStream), type, typeof(constantConverter) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isNextMember = false;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public unsafe void Push(fastCSharp.code.cSharp.sqlModel.fieldInfo field)
            {
                if (isNextMember) generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_0, AndString.Char, 5);
                else isNextMember = true;
                if (field.IsSqlColumn)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldstr, field.Field.Name);
                    generator.Emit(OpCodes.Call, sqlColumn.updateDynamicMethod.GetTypeUpdate(field.DataType));
                }
                else
                {
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_0, pub.GetNameAssignmentPool(field.SqlFieldName), field.SqlFieldName.Length + 1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
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
        }
        /// <summary>
        /// 添加连接字符串
        /// </summary>
        internal static readonly pointer.reference AndString = new pointer { Data = namePool.Get(" and ", 0, 0) }.Reference;
#endif
    }
    /// <summary>
    /// 数据库表格模型
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class sqlModel<valueType> : databaseModel<valueType>
    {
        /// <summary>
        /// 数据列验证
        /// </summary>
        internal static class verify
        {
            /// <summary>
            /// 数据验证
            /// </summary>
            /// <param name="value"></param>
            /// <param name="memberMap"></param>
            /// <param name="sqlTool"></param>
            /// <returns></returns>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static bool Verify(valueType value, fastCSharp.code.memberMap memberMap, fastCSharp.emit.sqlTable.sqlToolBase sqlTool)
            {
#if NOJIT
                if (fields != null)
                {
                    object[] sqlColumnParameters = null, castParameters = null;
                    foreach (sqlModel.verifyField field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (memberMap.IsMember(fieldInfo.MemberMapIndex))
                        {
                            object memberValue = fieldInfo.Field.GetValue(value);
                            if (field.IsSqlVerify)
                            {
                                if (!fieldInfo.Field.FieldType.IsValueType && memberValue == null) return false;
                                if (!(bool)((fastCSharp.emit.sqlTable.ISqlVerify)memberValue).IsSqlVeify()) return false;
                            }
                            else if (fieldInfo.IsSqlColumn)
                            {
                                if (sqlColumnParameters == null) sqlColumnParameters = new object[] { null, sqlTool, null };
                                sqlColumnParameters[0] = memberValue;
                                sqlColumnParameters[2] = fieldInfo.Field.Name;
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
                                if (!sqlTool.StringVerify(fieldInfo.Field.Name, (string)memberValue, dataMember.MaxStringLength, dataMember.IsAscii, dataMember.IsNull)) return false;
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
                                    sqlTool.NullVerify(fieldInfo.Field.Name);
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
#else
                return verifyer == null || verifyer(value, memberMap, sqlTool);
#endif
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.verifyField[] fields;
#else
            /// <summary>
            /// 数据验证
            /// </summary>
            private static readonly Func<valueType, fastCSharp.code.memberMap, fastCSharp.emit.sqlTable.sqlToolBase, bool> verifyer;
#endif

            static verify()
            {
                if (attribute != null)
                {
                    subArray<fastCSharp.code.cSharp.sqlModel.fieldInfo> verifyFields = Fields.getFind(value => value.IsVerify);
                    if (verifyFields.length != 0)
                    {
#if NOJIT
                        fields = new sqlModel.verifyField[verifyFields.length];
                        int index = 0;
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in verifyFields) fields[index++].Set(member);
#else
                        sqlModel.verifyDynamicMethod dynamicMethod = new sqlModel.verifyDynamicMethod(typeof(valueType));
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in verifyFields) dynamicMethod.Push(member);
                        verifyer = (Func<valueType, fastCSharp.code.memberMap, fastCSharp.emit.sqlTable.sqlToolBase, bool>)dynamicMethod.Create<Func<valueType, fastCSharp.code.memberMap, fastCSharp.emit.sqlTable.sqlToolBase, bool>>();
#endif
                    }
                }
            }
        }
        /// <summary>
        /// 数据库模型设置
        /// </summary>
        internal static class set
        {
            /// <summary>
            /// 设置字段值
            /// </summary>
            /// <param name="reader">字段读取器物理存储</param>
            /// <param name="value">目标数据</param>
            /// <param name="memberMap">成员位图</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Set(DbDataReader reader, valueType value, fastCSharp.code.memberMap memberMap)
            {
#if NOJIT
                if (fields != null)
                {
                    int index = 0;
                    object[] sqlColumnParameters = null, castParameters = null;
                    foreach (sqlModel.setField field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (memberMap.IsMember(fieldInfo.MemberMapIndex))
                        {
                            if (fieldInfo.DataReaderMethod == null)
                            {
                                if (sqlColumnParameters == null) sqlColumnParameters = new object[] { reader, null, null };
                                sqlColumnParameters[1] = null;
                                sqlColumnParameters[2] = index;
                                field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                                fieldInfo.Field.SetValue(value, sqlColumnParameters[1]);
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
                                fieldInfo.Field.SetValue(value, memberValue);
                            }
                        }
                    }
                }
#else
                if (setter != null) setter(reader, value, memberMap);
#endif
                        }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.setField[] fields;
#else
            /// <summary>
            /// 默认数据列设置
            /// </summary>
            private static readonly Action<DbDataReader, valueType, fastCSharp.code.memberMap> setter;
#endif

            static set()
            {
                if (attribute != null)
                {
#if NOJIT
                    fields = new sqlModel.setField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlModel.setDynamicMethod dynamicMethod = new sqlModel.setDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    setter = (Action<DbDataReader, valueType, fastCSharp.code.memberMap>)dynamicMethod.Create<Action<DbDataReader, valueType, fastCSharp.code.memberMap>>();
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
            /// 导入数据列集合
            /// </summary>
            private static keyValue<string, Type>[] dataColumns;
            /// <summary>
            /// 导入数据列集合
            /// </summary>
            internal static keyValue<string, Type>[] DataColumns
            {
                get
                {
                    if (dataColumns == null)
                    {
                        subArray<keyValue<string, Type>> columns = new subArray<keyValue<string, Type>>(Fields.Length);
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                        {
                            if (field.IsSqlColumn) columns.Add(sqlColumn.toArrayDynamicMethod.GetDataColumns(field.DataType)(field.Field.Name));
                            else columns.Add(new keyValue<string, Type>(field.SqlFieldName, field.DataType));
                        }
                        dataColumns = columns.ToArray();
                    }
                    return dataColumns;
                }
            }
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
                    foreach (sqlModel.toArrayField field in fields)
                    {
                        fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                        if (fieldInfo.IsSqlColumn)
                        {
                            if (sqlColumnParameters == null) sqlColumnParameters = new object[] { null, values, null };
                            sqlColumnParameters[0] = fieldInfo.Field.GetValue(value);
                            sqlColumnParameters[2] = index;
                            field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                            index = (int)sqlColumnParameters[2];
                        }
                        else
                        {
                            object memberValue = fieldInfo.Field.GetValue(value);
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
                if (attribute != null)
                {
#if NOJIT
                    fields = new sqlModel.toArrayField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlModel.toArrayDynamicMethod dynamicMethod = new sqlModel.toArrayDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    defaultWriter = (writer)dynamicMethod.Create<writer>();
#endif
                }
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        internal static class insert
        {
            /// <summary>
            /// 获取逗号分割的列名集合
            /// </summary>
            /// <param name="sqlStream"></param>
            /// <param name="memberMap"></param>
            public static void GetColumnNames(charStream sqlStream, fastCSharp.code.memberMap memberMap)
            {
                int isNext = 0;
                foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields)
                {
                    if (memberMap.IsMember(member.MemberMapIndex) || member == Identity || member.DataMember.PrimaryKeyIndex != 0)
                    {
                        if (isNext == 0) isNext = 1;
                        else sqlStream.Write(',');
                        if (member.IsSqlColumn) sqlStream.SimpleWriteNotNull(sqlColumn.insertDynamicMethod.GetColumnNames(member.Field.FieldType)(member.Field.Name));
                        else sqlStream.SimpleWriteNotNull(member.SqlFieldName);
                    }
                }
            }
            /// <summary>
            /// 获取插入数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="memberMap">成员位图</param>
            /// <param name="value">数据</param>
            /// <param name="converter">SQL常量转换</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Insert(charStream sqlStream, fastCSharp.code.memberMap memberMap, valueType value, constantConverter converter)
            {
#if NOJIT
                if (fields != null)
                {
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    byte isNext = 0;
                    foreach (sqlModel.insertField field in fields)
                    {
                        if (memberMap.IsMember(field.Field.MemberMapIndex))
                        {
                            if (isNext == 0) isNext = 1;
                            else sqlStream.Write(',');
                            fastCSharp.code.cSharp.sqlModel.fieldInfo fieldInfo = field.Field;
                            if (fieldInfo.IsSqlColumn)
                            {
                                if (sqlColumnParameters == null) sqlColumnParameters = new object[] { sqlStream, null, converter };
                                sqlColumnParameters[1] = fieldInfo.Field.GetValue(value);
                                field.SqlColumnMethod.Invoke(null, sqlColumnParameters);
                            }
                            else
                            {
                                object memberValue = fieldInfo.Field.GetValue(value);
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
                }
#else
                if (inserter != null) inserter(sqlStream, memberMap, value, converter);
#endif
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
            private static readonly Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter> inserter;
#endif
            static insert()
            {
                if (attribute != null)
                {
#if NOJIT
                    fields = new sqlModel.insertField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlModel.insertDynamicMethod dynamicMethod = new sqlModel.insertDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    inserter = (Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter>)dynamicMethod.Create<Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter>>();
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
            /// 获取更新数据SQL表达式
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="memberMap">更新成员位图</param>
            /// <param name="value">数据</param>
            /// <param name="converter">SQL常量转换</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Update(charStream sqlStream, fastCSharp.code.memberMap memberMap, valueType value, constantConverter converter)
            {
#if NOJIT
                if (fields != null)
                {
                    byte isNext = 0;
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    foreach (sqlModel.updateField field in fields)
                    {
                        if (memberMap.IsMember(field.Field.MemberMapIndex))
                        {
                            if (isNext == 0) isNext = 1;
                            else sqlStream.Write(',');
                            field.Set(sqlStream, value, converter, ref sqlColumnParameters, ref castParameters, ref parameters);
                        }
                    }
                }
#else
                if (updater != null) updater(sqlStream, memberMap, value, converter);
#endif
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
            private static readonly Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter> updater;
#endif

            static update()
            {
                if (attribute != null)
                {
#if NOJIT
                    fields = new sqlModel.updateField[Fields.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) fields[index++].Set(member);
#else
                    sqlModel.updateDynamicMethod dynamicMethod = new sqlModel.updateDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields) dynamicMethod.Push(member);
                    updater = (Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter>)dynamicMethod.Create<Action<charStream, fastCSharp.code.memberMap, valueType, constantConverter>>();
#endif
                }
            }
        }
        /// <summary>
        /// 关键字条件
        /// </summary>
        internal static class primaryKeyWhere
        {
            /// <summary>
            /// 关键字条件SQL流
            /// </summary>
            /// <param name="sqlStream">SQL表达式流</param>
            /// <param name="value">数据列</param>
            /// <param name="converter">SQL常量转换</param>
#if NOJIT
#else
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
            public static void Where(charStream sqlStream, valueType value, constantConverter converter)
            {
#if NOJIT
                if (fields != null)
                {
                    byte isAnd = 0;
                    object[] sqlColumnParameters = null, castParameters = null, parameters = null;
                    foreach (sqlModel.updateField field in fields)
                    {
                        if (isAnd == 0) isAnd = 1;
                        else sqlStream.WriteNotNull(" and ");
                        field.Set(sqlStream, value, converter, ref sqlColumnParameters, ref castParameters, ref parameters);
                    }
                }
#else
                if (where != null) where(sqlStream, value, converter);
#endif
            }
#if NOJIT
            /// <summary>
            /// 字段集合
            /// </summary>
            private static readonly sqlModel.updateField[] fields;
#else
            /// <summary>
            /// 关键字条件SQL流
            /// </summary>
            private static readonly Action<charStream, valueType, constantConverter> where;
#endif

            static unsafe primaryKeyWhere()
            {
                if (attribute != null && PrimaryKeys.Length != 0)
                {
#if NOJIT
                    fields = new sqlModel.updateField[PrimaryKeys.Length];
                    int index = 0;
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in PrimaryKeys) fields[index++].Set(member);
#else
                    sqlModel.primaryKeyWhereDynamicMethod dynamicMethod = new sqlModel.primaryKeyWhereDynamicMethod(typeof(valueType));
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in PrimaryKeys) dynamicMethod.Push(member);
                    where = (Action<charStream, valueType, constantConverter>)dynamicMethod.Create<Action<charStream, valueType, constantConverter>>();
#endif
                }
            }
        }

        /// <summary>
        /// 数据库表格模型配置
        /// </summary>
        private static readonly fastCSharp.code.cSharp.sqlModel attribute;
        /// <summary>
        /// 字段集合
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.sqlModel.fieldInfo[] Fields;
        /// <summary>
        /// 自增字段
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.sqlModel.fieldInfo Identity;
        /// <summary>
        /// 关键字字段集合
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.sqlModel.fieldInfo[] PrimaryKeys;
        /// <summary>
        /// SQL数据成员
        /// </summary>
        internal static readonly fastCSharp.code.memberMap<valueType> MemberMap;
        /// <summary>
        /// SQL数据成员
        /// </summary>
        public static fastCSharp.code.memberMap<valueType> CopyMemberMap
        {
            get { return MemberMap.Copy(); }
        }
        /// <summary>
        /// 分组数据成员位图
        /// </summary>
        private static keyValue<fastCSharp.code.memberMap<valueType>, int>[] groupMemberMaps;
        /// <summary>
        /// 分组数据成员位图访问锁
        /// </summary>
        private static readonly object groupMemberMapLock = new object();
        /// <summary>
        /// 自增标识获取器
        /// </summary>
        public static readonly Func<valueType, long> GetIdentity;
        /// <summary>
        /// 自增标识获取器
        /// </summary>
        public static readonly Func<valueType, int> GetIdentity32;
        /// <summary>
        /// 设置自增标识
        /// </summary>
        public static readonly Action<valueType, long> SetIdentity;
        /// <summary>
        /// 获取以逗号分割的名称集合
        /// </summary>
        /// <param name="sqlStream"></param>
        /// <param name="memberMap"></param>
        internal static void GetNames(charStream sqlStream, fastCSharp.code.memberMap memberMap)
        {
            int isNext = 0;
            foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
            {
                if (memberMap.IsMember(field.MemberMapIndex))
                {
                    if (isNext == 0) isNext = 1;
                    else sqlStream.Write(',');
                    if (field.IsSqlColumn) sqlStream.SimpleWriteNotNull(field.GetSqlColumnName());
                    else sqlStream.SimpleWriteNotNull(field.SqlFieldName);
                }
            }
        }
        /// <summary>
        /// 获取表格信息
        /// </summary>
        /// <param name="type">SQL绑定类型</param>
        /// <param name="sqlTable">SQL表格信息</param>
        /// <returns>表格信息</returns>
        internal static table GetTable(Type type, sqlTable sqlTable)
        {
            client client = connection.GetConnection(sqlTable.ConnectionType).Client;
            table table = new table { Columns = new columnCollection { Name = sqlTable.GetTableName(type) } };
            column[] columns = new column[Fields.Length];
            column[] primaryKeyColumns = new column[PrimaryKeys.Length];
            int index = 0, primaryKeyIndex = 0;
            foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo member in Fields)
            {
                column column = client.GetColumn(member.Field.Name, member.Field.FieldType, member.DataMember);
                columns[index++] = column;
                if (Identity == member) table.Identity = column;
                if (member.DataMember.PrimaryKeyIndex != 0) primaryKeyColumns[primaryKeyIndex++] = column;
            }
            table.Columns.Columns = columns;
            if (primaryKeyColumns.Length != 0)
            {
                table.PrimaryKey = new columnCollection
                {
                    Columns = PrimaryKeys.getArray(value => primaryKeyColumns.firstOrDefault(column => column.Name == value.Field.Name))
                };
            }
            table.Columns.Name = sqlTable.GetTableName(type);
            return table;
        }
        /// <summary>
        /// 获取分组数据成员位图
        /// </summary>
        /// <param name="group">分组</param>
        /// <returns>分组数据成员位图</returns>
        private static fastCSharp.code.memberMap<valueType> getGroupMemberMap(int group)
        {
            if (groupMemberMaps == null)
            {
                subArray<keyValue<fastCSharp.code.memberMap<valueType>, int>> memberMaps = new subArray<keyValue<code.memberMap<valueType>, int>>();
                memberMaps.Add(new keyValue<fastCSharp.code.memberMap<valueType>, int>(MemberMap, 0));
                Monitor.Enter(groupMemberMapLock);
                if (groupMemberMaps == null)
                {
                    try
                    {
                        foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                        {
                            if (field.DataMember.Group != 0)
                            {
                                int index = memberMaps.length;
                                foreach (keyValue<fastCSharp.code.memberMap<valueType>, int> memberMap in memberMaps.array)
                                {
                                    if (memberMap.Value == field.DataMember.Group || --index == 0) break;
                                }
                                if (index == 0)
                                {
                                    fastCSharp.code.memberMap<valueType> memberMap = fastCSharp.code.memberMap<valueType>.New();
                                    memberMaps.Add(new keyValue<fastCSharp.code.memberMap<valueType>, int>(memberMap, field.DataMember.Group));
                                    memberMap.SetMember(field.MemberMapIndex);
                                }
                                else memberMaps.array[memberMaps.length - index].Key.SetMember(field.MemberMapIndex);
                            }
                        }
                        if (memberMaps.length != 1)
                        {
                            fastCSharp.code.memberMap<valueType> memberMap = memberMaps.array[0].Key = fastCSharp.code.memberMap<valueType>.New();
                            foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields)
                            {
                                if (field.DataMember.Group == 0) memberMap.SetMember(field.MemberMapIndex);
                            }
                        }
                        groupMemberMaps = memberMaps.ToArray();
                    }
                    finally { Monitor.Exit(groupMemberMapLock);}
                }
                else Monitor.Exit(groupMemberMapLock);
            }
            foreach (keyValue<fastCSharp.code.memberMap<valueType>, int> memberMap in groupMemberMaps)
            {
                if (memberMap.Value == group) return memberMap.Key;
            }
            log.Error.Add(typeof(valueType).fullName() + " 缺少缓存分组 " + group.toString(), new System.Diagnostics.StackFrame(), false);
            return null;
        }
        /// <summary>
        /// 获取分组数据成员位图
        /// </summary>
        /// <param name="group">分组</param>
        /// <returns>分组数据成员位图</returns>
        public static fastCSharp.code.memberMap<valueType> GetCacheMemberMap(int group)
        {
            fastCSharp.code.memberMap<valueType> memberMap = getGroupMemberMap(group);
            if (memberMap != null)
            {
                setIdentityOrPrimaryKeyMemberMap(memberMap = memberMap.Copy());
                return memberMap;
            }
            return null;
        }
        /// <summary>
        /// 自增标识/关键字成员位图
        /// </summary>
        /// <returns></returns>
        public static fastCSharp.code.memberMap<valueType> GetIdentityOrPrimaryKeyMemberMap()
        {
            fastCSharp.code.memberMap<valueType> memberMap = fastCSharp.code.memberMap<valueType>.NewEmpty();
            setIdentityOrPrimaryKeyMemberMap(memberMap);
            return memberMap;
        }
        /// <summary>
        /// 自增标识/关键字成员位图
        /// </summary>
        /// <param name="memberMap"></param>
        public static void SetIdentityOrPrimaryKeyMemberMap(fastCSharp.code.memberMap<valueType> memberMap)
        {
            if (memberMap == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            setIdentityOrPrimaryKeyMemberMap(memberMap);
        }
        /// <summary>
        /// 自增标识/关键字成员位图
        /// </summary>
        /// <param name="memberMap"></param>
        private static void setIdentityOrPrimaryKeyMemberMap(fastCSharp.code.memberMap<valueType> memberMap)
        {
            if (memberMap == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            if (Identity != null) memberMap.SetMember(Identity.MemberMapIndex);
            else if (PrimaryKeys.Length != 0)
            {
                foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in PrimaryKeys) memberMap.SetMember(field.MemberMapIndex);
            }
        }
        /// <summary>
        /// 获取自增标识获取器
        /// </summary>
        /// <param name="baseIdentity"></param>
        /// <returns></returns>
        public static Func<valueType, int> IdentityGetter(int baseIdentity)
        {
            if (baseIdentity == 0) return GetIdentity32;
#if NOJIT
            return new baseIdentity32(Identity.Field, baseIdentity).Get();
#else
            DynamicMethod dynamicMethod = new DynamicMethod("GetIdentity32_" + baseIdentity.toString(), typeof(int), new Type[] { typeof(valueType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, Identity.Field);
            if (Identity.Field.FieldType != typeof(int) && Identity.Field.FieldType != typeof(uint)) generator.Emit(OpCodes.Conv_I4);
            generator.int32(baseIdentity);
            generator.Emit(OpCodes.Sub);
            generator.Emit(OpCodes.Ret);
            return (Func<valueType, int>)dynamicMethod.CreateDelegate(typeof(Func<valueType, int>));
#endif
        }
        /// <summary>
        /// 获取关键字获取器
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <returns></returns>
        public static Func<valueType, keyType> PrimaryKeyGetter<keyType>()
        {
            return databaseModel<valueType>.GetPrimaryKeyGetter<keyType>("GetSqlPrimaryKey", PrimaryKeys.getArray(value => value.Field));
        }
        static sqlModel()
        {
            Type type = typeof(valueType);
            attribute = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.code.cSharp.sqlModel>(type, true, true) ?? fastCSharp.code.cSharp.sqlModel.Default;
            Fields = fastCSharp.code.cSharp.sqlModel.GetFields(fastCSharp.code.memberIndexGroup<valueType>.GetFields(attribute.MemberFilter)).ToArray();
            Identity = fastCSharp.code.cSharp.sqlModel.GetIdentity(Fields);
            PrimaryKeys = fastCSharp.code.cSharp.sqlModel.GetPrimaryKeys(Fields).ToArray();
            MemberMap = fastCSharp.code.memberMap<valueType>.New();
            foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in Fields) MemberMap.SetMember(field.MemberMapIndex);
            if (Identity != null)
            {
#if NOJIT
                new identity(Identity.Field).Get(out GetIdentity, out SetIdentity);
                Action<valueType, int> setter32;
                new identity32(Identity.Field).Get(out GetIdentity32, out setter32);
#else
                DynamicMethod dynamicMethod = new DynamicMethod("GetSqlIdentity", typeof(long), new Type[] { type }, type, true);
                ILGenerator generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, Identity.Field);
                if (Identity.Field.FieldType != typeof(long) && Identity.Field.FieldType != typeof(ulong)) generator.Emit(OpCodes.Conv_I8);
                generator.Emit(OpCodes.Ret);
                GetIdentity = (Func<valueType, long>)dynamicMethod.CreateDelegate(typeof(Func<valueType, long>));

                dynamicMethod = new DynamicMethod("SetSqlIdentity", null, new Type[] { type, typeof(long) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                if (Identity.Field.FieldType != typeof(long) && Identity.Field.FieldType != typeof(ulong)) generator.Emit(OpCodes.Conv_I4);
                generator.Emit(OpCodes.Stfld, Identity.Field);
                generator.Emit(OpCodes.Ret);
                SetIdentity = (Action<valueType, long>)dynamicMethod.CreateDelegate(typeof(Action<valueType, long>));

                GetIdentity32 = getIdentityGetter32("GetSqlIdentity32", Identity.Field);
#endif
            }
        }
#if NOJIT
        /// <summary>
        /// 自增值处理
        /// </summary>
        private sealed class identity
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            private FieldInfo field;
            /// <summary>
            /// 自增值处理
            /// </summary>
            /// <param name="field">字段信息</param>
            public identity(FieldInfo field)
            {
                this.field = field;
            }
            /// <summary>
            /// 获取自增值处理委托
            /// </summary>
            /// <param name="getter"></param>
            /// <param name="setter"></param>
            public void Get(out Func<valueType, long> getter, out Action<valueType, long> setter)
            {
                if (field.FieldType == typeof(int))
                {
                    getter = getInt;
                    setter = setInt;
                }
                else if (field.FieldType == typeof(long))
                {
                    getter = getLong;
                    setter = setLong;
                }
                else if (field.FieldType == typeof(uint))
                {
                    getter = getUInt;
                    setter = setUInt;
                }
                else if (field.FieldType == typeof(ulong))
                {
                    getter = getULong;
                    setter = setULong;
                }
                else if (field.FieldType == typeof(ushort))
                {
                    getter = getUShort;
                    setter = setUShort;
                }
                else if (field.FieldType == typeof(short))
                {
                    getter = getShort;
                    setter = setShort;
                }
                else if (field.FieldType == typeof(byte))
                {
                    getter = getByte;
                    setter = setByte;
                }
                else if (field.FieldType == typeof(sbyte))
                {
                    getter = getSByte;
                    setter = setSByte;
                }
                else
                {
                    getter = getInt;
                    setter = setInt;
                }
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getInt(valueType value)
            {
                return (long)(int)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getLong(valueType value)
            {
                return (long)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getUInt(valueType value)
            {
                return (long)(uint)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getULong(valueType value)
            {
                return (long)(ulong)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getUShort(valueType value)
            {
                return (long)(ushort)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getShort(valueType value)
            {
                return (long)(short)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getByte(valueType value)
            {
                return (long)(byte)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private long getSByte(valueType value)
            {
                return (long)(sbyte)field.GetValue(value);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setInt(valueType value, long identity)
            {
                field.SetValue(value, (int)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setLong(valueType value, long identity)
            {
                field.SetValue(value, identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setUInt(valueType value, long identity)
            {
                field.SetValue(value, (uint)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setULong(valueType value, long identity)
            {
                field.SetValue(value, (ulong)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setUShort(valueType value, long identity)
            {
                field.SetValue(value, (ushort)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setShort(valueType value, long identity)
            {
                field.SetValue(value, (short)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setByte(valueType value, long identity)
            {
                field.SetValue(value, (byte)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setSByte(valueType value, long identity)
            {
                field.SetValue(value, (sbyte)identity);
            }
        }
        /// <summary>
        /// 自增值处理
        /// </summary>
        internal sealed class identity32
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            private FieldInfo field;
            /// <summary>
            /// 自增值处理
            /// </summary>
            /// <param name="field">字段信息</param>
            public identity32(FieldInfo field)
            {
                this.field = field;
            }
            /// <summary>
            /// 获取自增值处理委托
            /// </summary>
            /// <param name="getter"></param>
            /// <param name="setter"></param>
            public void Get(out Func<valueType, int> getter, out Action<valueType, int> setter)
            {
                if (field.FieldType == typeof(int))
                {
                    getter = getInt;
                    setter = setInt;
                }
                else if (field.FieldType == typeof(long))
                {
                    getter = getLong;
                    setter = setLong;
                }
                else if (field.FieldType == typeof(uint))
                {
                    getter = getUInt;
                    setter = setUInt;
                }
                else if (field.FieldType == typeof(ulong))
                {
                    getter = getULong;
                    setter = setULong;
                }
                else if (field.FieldType == typeof(ushort))
                {
                    getter = getUShort;
                    setter = setUShort;
                }
                else if (field.FieldType == typeof(short))
                {
                    getter = getShort;
                    setter = setShort;
                }
                else if (field.FieldType == typeof(byte))
                {
                    getter = getByte;
                    setter = setByte;
                }
                else if (field.FieldType == typeof(sbyte))
                {
                    getter = getSByte;
                    setter = setSByte;
                }
                else
                {
                    getter = getInt;
                    setter = setInt;
                }
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getInt(valueType value)
            {
                return (int)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getLong(valueType value)
            {
                return (int)(long)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getUInt(valueType value)
            {
                return (int)(uint)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getULong(valueType value)
            {
                return (int)(ulong)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getUShort(valueType value)
            {
                return (int)(ushort)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getShort(valueType value)
            {
                return (int)(short)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getByte(valueType value)
            {
                return (int)(byte)field.GetValue(value);
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getSByte(valueType value)
            {
                return (int)(sbyte)field.GetValue(value);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setInt(valueType value, int identity)
            {
                field.SetValue(value, identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setLong(valueType value, int identity)
            {
                field.SetValue(value, (long)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setUInt(valueType value, int identity)
            {
                field.SetValue(value, (uint)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setULong(valueType value, int identity)
            {
                field.SetValue(value, (ulong)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setUShort(valueType value, int identity)
            {
                field.SetValue(value, (ushort)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setShort(valueType value, int identity)
            {
                field.SetValue(value, (short)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setByte(valueType value, int identity)
            {
                field.SetValue(value, (byte)identity);
            }
            /// <summary>
            /// 设置自增值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="identity"></param>
            private void setSByte(valueType value, int identity)
            {
                field.SetValue(value, (sbyte)identity);
            }
        }
        /// <summary>
        /// 自增值处理
        /// </summary>
        private sealed class baseIdentity32
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            private FieldInfo field;
            /// <summary>
            /// 
            /// </summary>
            private int baseIdentity;
            /// <summary>
            /// 自增值处理
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="baseIdentity"></param>
            public baseIdentity32(FieldInfo field, int baseIdentity)
            {
                this.field = field;
                this.baseIdentity = baseIdentity;
            }
            /// <summary>
            /// 获取自增值处理委托
            /// </summary>
            public Func<valueType, int> Get()
            {
                if (field.FieldType == typeof(int)) return getInt;
                if (field.FieldType == typeof(long)) return getLong;
                if (field.FieldType == typeof(uint)) return getUInt;
                if (field.FieldType == typeof(ulong)) return getULong;
                if (field.FieldType == typeof(ushort)) return getUShort;
                if (field.FieldType == typeof(short)) return getShort;
                if (field.FieldType == typeof(byte)) return getByte;
                if (field.FieldType == typeof(sbyte)) return getSByte;
                return getInt;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getInt(valueType value)
            {
                return (int)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getLong(valueType value)
            {
                return (int)(long)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getUInt(valueType value)
            {
                return (int)(uint)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getULong(valueType value)
            {
                return (int)(ulong)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getUShort(valueType value)
            {
                return (int)(ushort)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getShort(valueType value)
            {
                return (int)(short)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getByte(valueType value)
            {
                return (int)(byte)field.GetValue(value) - baseIdentity;
            }
            /// <summary>
            /// 获取自增值
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int getSByte(valueType value)
            {
                return (int)(sbyte)field.GetValue(value) - baseIdentity;
            }
        }
#endif
    }
}
