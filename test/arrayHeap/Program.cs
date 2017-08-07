using System;
using fastCSharp;

namespace fastCSharp.test.arrayHeap
{
    class Program
    {
        /// <summary>
        /// 数组模拟最小堆测试
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            arrayHeap<int, int> heap = new arrayHeap<int, int>();
            int[] array = new int[65536];
            int arrayIndex = 0;
            do
            {
                int pushCount = Math.Min(65536 - arrayIndex, fastCSharp.random.Default.NextUShort());
                for (int endIndex = arrayIndex + pushCount; arrayIndex != endIndex; ++arrayIndex)
                {
                    int value = fastCSharp.random.Default.Next();
                    array[arrayIndex] = value;
                    heap.Add(value, value);
                }
                if (heap.Count != arrayIndex)
                {
                    Console.WriteLine("arrayIndex[" + arrayIndex.ToString() + "] != heap.Count[" + heap.Count.toString() + "]");
                    break;
                }
                int popCount = Math.Min(arrayIndex, fastCSharp.random.Default.NextUShort());
                array.sortDesc(0, arrayIndex);
                for (int endIndex = arrayIndex - popCount; arrayIndex != endIndex;)
                {
                    --arrayIndex;
                    keyValue<int, int> value;
                    if (heap.Pop(out value))
                    {
                        if (value.Key != array[arrayIndex])
                        {
                            Console.WriteLine("heap Value ERROR");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("heap.Pop ERROR");
                        break;
                    }
                }
                if (heap.Count != arrayIndex)
                {
                    Console.WriteLine("arrayIndex[" + arrayIndex.ToString() + "] != heap.Count[" + heap.Count.toString() + "]");
                    break;
                }
                Console.WriteLine("pushCount[" + pushCount.ToString() + "] popCount[" + popCount.toString() + "] arrayIndex[" + arrayIndex.toString() + "]");
            }
            while (true);
            Console.WriteLine("press any key to exit.");
            Console.ReadKey();
        }
    }
}
