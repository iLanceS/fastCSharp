using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace fastCSharp.data
{
    /// <summary>
    /// DataSet包装
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
    public sealed class dataSet
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private dataSource data;
        /// <summary>
        /// DataSet名称
        /// </summary>
        private string name;
        /// <summary>
        /// 数据表格集合
        /// </summary>
        private dataTable[] tables;
        /// <summary>
        /// DataSet包装
        /// </summary>
        /// <param name="set"></param>
        private void from(DataSet set)
        {
            if (set.Tables.Count != 0)
            {
                using (dataWriter builder = new dataWriter())
                {
                    tables = set.Tables.toGeneric<DataTable>().getArray(table => dataTable.From(table, builder));
                    data = builder.Get();
                }
            }
            name = set.DataSetName;
        }
        /// <summary>
        /// DataSet包装
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static dataSet From(DataSet set)
        {
            if (set == null) return null;
            dataSet value = new dataSet();
            value.from(set);
            return value;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="set"></param>
        /// <returns>序列化数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] Serialize(DataSet set)
        {
            return fastCSharp.emit.dataSerializer.Serialize(From(set));
        }
        /// <summary>
        /// DataSet拆包
        /// </summary>
        /// <param name="set"></param>
        private unsafe void set(DataSet set)
        {
            if (tables.length() != 0)
            {
                fixed (byte* dataFixed = data.Data)
                {
                    dataReader builder = new dataReader(dataFixed, data.Strings, data.Bytes);
                    foreach (dataTable table in tables) set.Tables.Add(table.Get(builder));
                }
            }
        }
        /// <summary>
        /// DataSet拆包
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static DataSet Get(dataSet value)
        {
            if (value == null) return null;
            DataSet set = new DataSet(value.name);
            try
            {
                value.set(set);
                return set;
            }
            catch (Exception error)
            {
                set.Dispose();
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
        public static DataSet DeSerialize(byte[] data)
        {
            dataSet value = fastCSharp.emit.dataDeSerializer.DeSerialize<dataSet>(data);
            return value != null ? Get(value) : null;
        }
    }
}
