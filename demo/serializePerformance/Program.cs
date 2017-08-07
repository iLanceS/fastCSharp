using System;
using System.Diagnostics;

namespace fastCSharp.demo.serializePerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                propertyData propertyData = fastCSharp.emit.random<propertyData>.Create(randomConfig);
                Json(propertyData);

                codeFiledData filedData = fastCSharp.emit.random<codeFiledData>.Create(randomConfig);
                Json(filedData);

                XmlSerialize(filedData);

                IndexSerialize(filedData);
                Serialize(filedData);
                CodeSerialize(filedData);

                Console.WriteLine("Press quit to exit.");
            }
            while (Console.ReadLine() != "quit");
        }

        /// <summary>
        /// 测试对象数量
        /// </summary>
        const int count = 10 * 10000;
        /// <summary>
        /// 随机生成对象参数
        /// </summary>
        static readonly fastCSharp.emit.random.config randomConfig = new fastCSharp.emit.random.config { IsSecondDateTime = true, IsParseFloat = true, IsNullChar = false };

        /// <summary>
        /// JSON序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void Json(propertyData value)
        {
            string[] jsons = new string[count];
            propertyData[] datas = new propertyData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                jsons[index] = fastCSharp.emit.jsonSerializer.ToJson(value);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Property ToJson " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                datas[index] = fastCSharp.emit.jsonParser.Parse<propertyData>(jsons[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Property Parse " + time.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// JSON序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void Json(filedData value)
        {
            string[] jsons = new string[count];
            filedData[] datas = new filedData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                jsons[index] = fastCSharp.emit.jsonSerializer.ToJson(value);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Filed ToJson " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                datas[index] = fastCSharp.emit.jsonParser.Parse<filedData>(jsons[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Filed Parse " + time.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// XML序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void XmlSerialize(filedData value)
        {
            string[] xmls = new string[count];
            filedData[] datas = new filedData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                xmls[index] = fastCSharp.emit.xmlSerializer.ToXml(value);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object XML Serialize " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                datas[index] = fastCSharp.emit.xmlParser.Parse<filedData>(xmls[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object XML DeSerialize " + time.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// 二进制序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void IndexSerialize(filedData value)
        {
            byte[][] bytes = new byte[count][];
            filedData[] datas = new filedData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                bytes[index] = fastCSharp.emit.indexSerializer.Serialize(value);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object IndexSerialize " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                datas[index] = fastCSharp.emit.indexDeSerializer.DeSerialize<filedData>(bytes[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object IndexDeSerialize " + time.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// 二进制序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void Serialize(filedData value)
        {
            byte[][] bytes = new byte[count][];
            filedData[] datas = new filedData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                bytes[index] = fastCSharp.emit.dataSerializer.Serialize(value);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Emit Serialize " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                datas[index] = fastCSharp.emit.dataDeSerializer.DeSerialize<filedData>(bytes[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Emit DeSerialize " + time.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// 代码生成二进制序列化测试
        /// </summary>
        /// <param name="value"></param>
        static void CodeSerialize(codeFiledData value)
        {
            byte[][] bytes = new byte[count][];
            codeFiledData[] datas = new codeFiledData[count];
            Stopwatch time = new Stopwatch();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                bytes[index] = value.Serialize();
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Code Serialize " + time.ElapsedMilliseconds.ToString() + "ms");

            time.Reset();
            time.Start();
            for (int index = count; index != 0;)
            {
                --index;
                (datas[index] = new codeFiledData()).DeSerialize(bytes[index]);
            }
            time.Stop();
            Console.WriteLine((count / 10000).toString() + "W object Code DeSerialize " + time.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
