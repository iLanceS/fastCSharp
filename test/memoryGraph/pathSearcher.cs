using System;
using System.Collections.Generic;
using System.IO;
using fastCSharp;

namespace fastCSharp.test.memoryGraph
{
    /// <summary>
    /// 内存对象文件搜索器
    /// </summary>
    class pathSearcher
    {
        /// <summary>
        /// 内存对象搜索器
        /// </summary>
        public fastCSharp.memoryGraph.searcher Searcher;
        /// <summary>
        /// 对象路径
        /// </summary>
        public Dictionary<string, HashSet<string>> Paths = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 内存对象文件搜索器
        /// </summary>
        /// <param name="file"></param>
        public pathSearcher(string file)
        {
            Searcher = new fastCSharp.memoryGraph.searcher(fastCSharp.emit.dataDeSerializer.DeSerialize<fastCSharp.memoryGraph.staticType[]>(File.ReadAllBytes(file)));
            GC.Collect();
            Searcher.OnNewValue += newValue;
            Searcher.Start();
            GC.Collect();
        }
        /// <summary>
        /// 添加新对象数据
        /// </summary>
        private void newValue()
        {
            HashSet<string> count;
            string name = Searcher.Value.Type.FullName;
            if (!Paths.TryGetValue(name, out count)) Paths.Add(name, count = new HashSet<string>());
            count.Add(Searcher.StaticType.TypeName + "." + Searcher.StaticValue.Name + "." + Searcher.Path.getArray().joinString(','));
        }
        /// <summary>
        /// 比较内存对象文件搜索器
        /// </summary>
        /// <param name="other"></param>
        public void Cmp(pathSearcher other)
        {
            foreach (KeyValuePair<string, HashSet<string>> value in Paths)
            {
                HashSet<string> count;
                if (other.Paths.TryGetValue(value.Key, out count))
                {
                    if (value.Value.Count > count.Count)
                    {
                        Console.WriteLine(value.Key + " = " + count.Count.toString() + " + " + (value.Value.Count - count.Count).toString());
                        foreach (string path in value.Value)
                        {
                            if (!count.Contains(path)) Console.WriteLine(path);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(value.Key + " + " + value.Value.Count.toString() + @"
" + value.Value.joinString(@"
"));
                }
            }
        }
        /// <summary>
        /// 比较内存对象文件搜索器
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        public static void CmpPath(string file1, string file2)
        {
            pathSearcher search2 = new pathSearcher(file2), search1 = new pathSearcher(file1);
            search2.Cmp(search1);
        }
    }
}
