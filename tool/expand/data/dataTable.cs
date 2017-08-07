using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.data
{
    /// <summary>
    /// 数据表格
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
    public sealed class dataTable
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private dataSource data;
        /// <summary>
        /// 表格名称
        /// </summary>
        private string name;
        /// <summary>
        /// 列名集合
        /// </summary>
        private string[] columnNames;
        /// <summary>
        /// 列类型集合
        /// </summary>
        private byte[] columnTypes;
        /// <summary>
        /// 空数据位图
        /// </summary>
        private byte[] dbNull;
        /// <summary>
        /// 数据行数
        /// </summary>
        private int rowCount;
        /// <summary>
        /// DataTable包装
        /// </summary>
        /// <param name="table"></param>
        /// <param name="builder">数据流包装器</param>
        private unsafe void from(DataTable table, dataWriter builder)
        {
            int index = 0;
            columnNames = new string[table.Columns.Count];
            fixed (byte* columnFixed = columnTypes = new byte[columnNames.Length])
            {
                byte* columnIndex = columnFixed;
                foreach (DataColumn column in table.Columns)
                {
                    if (!typeIndexs.TryGetValue(column.DataType, out *columnIndex)) *columnIndex = 255;
                    ++columnIndex;
                    columnNames[index++] = column.ColumnName;
                }
                fixed (byte* nullFixed = dbNull = new byte[(columnNames.Length * rowCount + 7) >> 3])
                {
                    fixedMap nullMap = new fixedMap(nullFixed);
                    index = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        columnIndex = columnFixed;
                        foreach (object value in row.ItemArray)
                        {
                            if (value == DBNull.Value) nullMap.Set(index);
                            else builder.Append(value, *columnIndex);
                            ++index;
                            ++columnIndex;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// DataTable包装
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static dataTable From(DataTable table)
        {
            if (table == null) return null;
            dataTable value = new dataTable();
            if ((value.rowCount = table.Rows.Count) != 0)
            {
                using (dataWriter builder = new dataWriter())
                {
                    value.from(table, builder);
                    value.data = builder.Get();
                }
            }
            return value;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="table"></param>
        /// <returns>序列化数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] Serialize(DataTable table)
        {
            return fastCSharp.emit.dataSerializer.Serialize(From(table));
        }
        /// <summary>
        /// DataTable包装
        /// </summary>
        /// <param name="table"></param>
        /// <param name="builder">数据流包装器</param>
        /// <returns></returns>
        internal static dataTable From(DataTable table, dataWriter builder)
        {
            dataTable value = new dataTable();
            if ((value.rowCount = table.Rows.Count) != 0) value.from(table, builder);
            value.name = table.TableName;
            return value;
        }
        /// <summary>
        /// DataTable拆包
        /// </summary>
        /// <param name="builder">数据对象拆包器</param>
        /// <returns></returns>
        internal DataTable Get(dataReader builder)
        {
            DataTable table = new DataTable(name);
            if (rowCount != 0)
            {
                try
                {
                    get(table, builder);
                }
                catch (Exception error)
                {
                    table.Dispose();
                    table = null;
                    log.Error.Add(error, null, false);
                }
            }
            return table;
        }
        /// <summary>
        /// DataTable拆包
        /// </summary>
        /// <param name="table"></param>
        /// <param name="builder">数据对象拆包器</param>
        private unsafe void get(DataTable table, dataReader builder)
        {
            int index = 0;
            DataColumn[] columns = new DataColumn[columnNames.Length];
            fixed (byte* columnFixed = columnTypes)
            {
                byte* columnIndex = columnFixed;
                foreach (string columnName in columnNames)
                {
                    columns[index++] = new DataColumn(columnName, *columnIndex < types.Length ? types[*columnIndex] : typeof(object));
                    ++columnIndex;
                }
                table.Columns.AddRange(columns);
                fixed (byte* nullFixed = dbNull)
                {
                    fixedMap nullMap = new fixedMap(nullFixed);
                    for (index = 0; rowCount != 0; --rowCount)
                    {
                        object[] values = new object[columnNames.Length];
                        columnIndex = columnFixed;
                        for (int valueIndex = 0; valueIndex != columnNames.Length; ++valueIndex)
                        {
                            values[valueIndex] = nullMap.Get(index++) ? DBNull.Value : builder.Get(*columnIndex);
                            ++columnIndex;
                        }
                        DataRow row = table.NewRow();
                        row.ItemArray = values;
                        table.Rows.Add(row);
                    }
                }
            }
        }
        /// <summary>
        /// DataTable拆包
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static DataTable Get(dataTable value)
        {
            if (value == null) return null;
            DataTable table = new DataTable(value.name);
            try
            {
                if (value.rowCount != 0)
                {
                    fixed (byte* dataFixed = value.data.Data)
                    {
                        dataReader builder = new dataReader(dataFixed, value.data.Strings, value.data.Bytes);
                        value.get(table, builder);
                    }
                }
                return table;
            }
            catch (Exception error)
            {
                table.Dispose();
                log.Error.Add(error, null, false);
            }
            return null;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DataTable DeSerialize(byte[] data)
        {
            dataTable value = fastCSharp.emit.dataDeSerializer.DeSerialize<dataTable>(data);
            return value != null ? Get(value) : null;
        }
        /// <summary>
        /// 类型集合
        /// </summary>
        private static readonly Type[] types;
        /// <summary>
        /// 类型索引集合
        /// </summary>
        private static readonly Dictionary<Type, byte> typeIndexs;
        static dataTable()
        {
            int index = 0;
            types = new Type[30];
            typeIndexs = dictionary.CreateOnly<Type, byte>();
            typeIndexs.Add(types[index] = typeof(int), (byte)index++);
            typeIndexs.Add(types[index] = typeof(int?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(string), (byte)index++);
            typeIndexs.Add(types[index] = typeof(DateTime), (byte)index++);
            typeIndexs.Add(types[index] = typeof(DateTime?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(double), (byte)index++);
            typeIndexs.Add(types[index] = typeof(double?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(float), (byte)index++);
            typeIndexs.Add(types[index] = typeof(float?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(decimal), (byte)index++);
            typeIndexs.Add(types[index] = typeof(decimal?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(Guid), (byte)index++);
            typeIndexs.Add(types[index] = typeof(Guid?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(bool), (byte)index++);
            typeIndexs.Add(types[index] = typeof(bool?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(byte), (byte)index++);
            typeIndexs.Add(types[index] = typeof(byte?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(byte[]), (byte)index++);
            typeIndexs.Add(types[index] = typeof(sbyte), (byte)index++);
            typeIndexs.Add(types[index] = typeof(sbyte?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(short), (byte)index++);
            typeIndexs.Add(types[index] = typeof(short?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(ushort), (byte)index++);
            typeIndexs.Add(types[index] = typeof(ushort?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(uint), (byte)index++);
            typeIndexs.Add(types[index] = typeof(uint?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(long), (byte)index++);
            typeIndexs.Add(types[index] = typeof(long?), (byte)index++);
            typeIndexs.Add(types[index] = typeof(ulong), (byte)index++);
            typeIndexs.Add(types[index] = typeof(ulong?), (byte)index++);
        }
    }
}
