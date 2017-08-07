using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 常量转换
    /// </summary>
    public class constantConverter
    {
        /// <summary>
        /// 常量转换处理集合
        /// </summary>
        protected Dictionary<Type, Action<charStream, object>> converters;
        /// <summary>
        /// 获取常量转换处理函数
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>失败返回null</returns>
        public Action<charStream, object> this[Type type]
        {
            get
            {
                Action<charStream, object> value;
                return converters.TryGetValue(type, out value) ? value : null;
            }
        }
        /// <summary>
        /// 常量转换
        /// </summary>
        protected constantConverter()
        {
            converters = dictionary.CreateOnly<Type, Action<charStream, object>>();
            converters.Add(typeof(bool), convertConstantBoolTo01);
            converters.Add(typeof(bool?), convertConstantBoolNullable);
            converters.Add(typeof(byte), convertConstantByte);
            converters.Add(typeof(byte?), convertConstantByteNullable);
            converters.Add(typeof(sbyte), convertConstantSByte);
            converters.Add(typeof(sbyte?), convertConstantSByteNullable);
            converters.Add(typeof(short), convertConstantShort);
            converters.Add(typeof(short?), convertConstantShortNullable);
            converters.Add(typeof(ushort), convertConstantUShort);
            converters.Add(typeof(ushort?), convertConstantUShortNullable);
            converters.Add(typeof(int), convertConstantInt);
            converters.Add(typeof(int?), convertConstantIntNullable);
            converters.Add(typeof(uint), convertConstantUInt);
            converters.Add(typeof(uint?), convertConstantUIntNullable);
            converters.Add(typeof(long), convertConstantLong);
            converters.Add(typeof(long?), convertConstantLongNullable);
            converters.Add(typeof(ulong), convertConstantULong);
            converters.Add(typeof(ulong?), convertConstantULongNullable);
            converters.Add(typeof(float), convertConstantFloat);
            converters.Add(typeof(float?), convertConstantFloatNullable);
            converters.Add(typeof(double), convertConstantDouble);
            converters.Add(typeof(double?), convertConstantDoubleNullable);
            converters.Add(typeof(decimal), convertConstantDecimal);
            converters.Add(typeof(decimal?), convertConstantDecimalNullable);
            converters.Add(typeof(DateTime), convertConstantDateTimeMillisecond);
            converters.Add(typeof(DateTime?), convertConstantDateTimeMillisecondNullable);
            converters.Add(typeof(string), convertConstantString);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstantToString<valueType>(charStream sqlStream, valueType value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else convertString(sqlStream, value.ToString());
        }
        /// <summary>
        /// 常量转换字符串函数信息
        /// </summary>
        internal static readonly MethodInfo ConvertConstantStringMethod = typeof(constantConverter).GetMethod("convertConstantToString", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, bool value)
        {
            sqlStream.Write(value ? '1' : '0');
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantBoolTo01(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (bool)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, bool? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else sqlStream.Write((bool)value ? '1' : '0');
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantBoolNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (bool?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, byte value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantByte(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((byte)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, byte? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((byte)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantByteNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (byte?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, sbyte value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantSByte(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((sbyte)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, sbyte? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((sbyte)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantSByteNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (sbyte?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, short value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantShort(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((short)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, short? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((short)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantShortNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (short?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, ushort value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantUShort(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((ushort)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, ushort? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((ushort)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantUShortNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (ushort?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, int value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantInt(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((int)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, int? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((int)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantIntNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (int?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, uint value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantUInt(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((uint)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, uint? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((uint)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantUIntNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (uint?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, long value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantLong(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((long)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, long? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((long)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantLongNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (long?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, ulong value)
        {
            fastCSharp.number.ToString(value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantULong(charStream sqlStream, object value)
        {
            fastCSharp.number.ToString((ulong)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, ulong? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else fastCSharp.number.ToString((ulong)value, sqlStream);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantULongNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (ulong?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, float value)
        {
            sqlStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantFloat(charStream sqlStream, object value)
        {
            sqlStream.SimpleWriteNotNull(((float)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, float? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else sqlStream.SimpleWriteNotNull(((float)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantFloatNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (float?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, double value)
        {
            sqlStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDouble(charStream sqlStream, object value)
        {
            sqlStream.SimpleWriteNotNull(((double)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, double? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else sqlStream.SimpleWriteNotNull(((double)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDoubleNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (double?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, decimal value)
        {
            sqlStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDecimal(charStream sqlStream, object value)
        {
            sqlStream.SimpleWriteNotNull(((decimal)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, decimal? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else sqlStream.SimpleWriteNotNull(((decimal)value).ToString());
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDecimalNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (decimal?)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstant(charStream sqlStream, DateTime value)
        {
            sqlStream.PrepLength(fastCSharp.date.SqlMillisecondSize + 2);
            sqlStream.UnsafeWrite('\'');
            fastCSharp.date.ToSqlMillisecond((DateTime)value, sqlStream);
            sqlStream.UnsafeWrite('\'');
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDateTimeMillisecond(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (DateTime)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, DateTime? value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else convertConstant(sqlStream, (DateTime)value);
        }
        /// <summary>
        /// 常量转换字符串
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantDateTimeMillisecondNullable(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (DateTime?)value);
        }
        /// <summary>
        /// 常量转换字符串(单引号变两个)
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void convertConstant(charStream sqlStream, string value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(sqlStream);
            else convertString(sqlStream, value);
        }
        /// <summary>
        /// 常量转换字符串(单引号变两个)
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        private void convertConstantString(charStream sqlStream, object value)
        {
            convertConstant(sqlStream, (string)value);
        }
        /// <summary>
        /// SQL语句字符串格式化(单引号变两个)
        /// </summary>
        /// <param name="sqlStream">SQL字符流</param>
        /// <param name="value">常量</param>
        protected virtual unsafe void convertString(charStream sqlStream, string value)
        {
            fixed (char* valueFixed = value)
            {
                int length = 0;
                for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                {
                    if (*start == '\'') ++length;
                    else if (*start == '\\')
                    {
                        if ((*(start + 1) == '\r' || *(start + 1) == '\n') && (int)(end - start) >= 2)
                        {
                            length += 2;
                            ++start;
                        }
                    }
                }
                if (length == 0)
                {
                    sqlStream.PrepLength(value.Length + 2);
                    sqlStream.UnsafeWrite('\'');
                    sqlStream.WriteNotNull(value);
                    sqlStream.UnsafeWrite('\'');
                    return;
                }
                sqlStream.PrepLength((length += value.Length) + 2);
                sqlStream.UnsafeWrite('\'');
                byte* write = (byte*)sqlStream.CurrentChar;
                for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                {
                    if (*start != '\'')
                    {
                        if (*start == '\\')
                        {
                            if (*(start + 1) == '\n')
                            {
                                if ((int)(end - start) >= 2)
                                {
                                    *(long*)write = '\\' + ('\\' << 16) + ((long)'\n' << 32) + ((long)'\n' << 48);
                                    ++start;
                                    write += sizeof(long);
                                    continue;
                                }
                            }
                            else if (*(start + 1) == '\r' && (int)(end - start) >= 2)
                            {
                                *(long*)write = '\\' + ('\\' << 16) + ((long)'\n' << 32) + ((long)'\r' << 48);
                                ++start;
                                write += sizeof(long);
                                continue;
                            }
                        }
                        *(char*)write = *start;
                        write += sizeof(char);
                    }
                    else
                    {
                        *(int*)write = ('\'' << 16) + '\'';
                        write += sizeof(int);
                    }
                }
                sqlStream.UnsafeAddLength(length);
                sqlStream.UnsafeWrite('\'');
            }
        }
        /// <summary>
        /// 常量转换
        /// </summary>
        internal static readonly constantConverter Default = new constantConverter();
    }
}
