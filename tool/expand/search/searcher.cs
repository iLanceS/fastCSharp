using System;
using System.Collections.Generic;
using System.Globalization;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.search
{
    /// <summary>
    /// 分词搜索器
    /// </summary>
    public abstract class searcher
    {
        /// <summary>
        /// 字符类型
        /// </summary>
        private enum charType : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,
            /// <summary>
            /// 其它字母
            /// </summary>
            OtherLetter,
            /// <summary>
            /// 字母
            /// </summary>
            Letter,
            /// <summary>
            /// 数字
            /// </summary>
            Number,
            /// <summary>
            /// 保留字符
            /// </summary>
            Keep,
        }
        /// <summary>
        /// 分词trie图
        /// </summary>
        protected trieGraph wordTrieGraph;
        /// <summary>
        /// 结果访问锁
        /// </summary>
        protected readonly object resultLock = new object();
        /// <summary>
        /// 最大搜索字符串长度
        /// </summary>
        protected int maxSearchSize;
        /// <summary>
        /// 总词频
        /// </summary>
        protected int wordCount = 1;
        /// <summary>
        /// 分词搜索器
        /// </summary>
        /// <param name="wordTrieGraph">分词trie图</param>
        /// <param name="maxSearchSize">最大搜索字符串长度</param>
        protected searcher(trieGraph wordTrieGraph, int maxSearchSize)
        {
            this.maxSearchSize = maxSearchSize < 1 ? 1 : maxSearchSize;
            this.wordTrieGraph = wordTrieGraph;
        }
        /// <summary>
        /// 文本分词
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="length">文本长度</param>
        /// <returns>分词结果</returns>
        private unsafe list<subString> getWords(string text, int length)
        {
            fixed (char* textFixed = text)
            {
                simplified.Format(textFixed, length);
                int count = (length + 7) >> 3;
                byte* match = stackalloc byte[count];
                fixedMap matchMap = new fixedMap(match, count, 0);
                list<subString> words = typePool<list<subString>>.Pop();
                if (words == null) words = new list<subString>();
                else if (words.Count != 0) words.Clear();
                list<keyValue<int, int>> matchs = typePool<list<keyValue<int, int>>>.Pop() ?? new list<keyValue<int, int>>();
                byte* charTypes = charTypePointer.Byte;
                subString matchWord = default(subString);
                for (char* start = textFixed, end = textFixed + length; start != end; )
                {
                    if (*start == ' ')
                    {
                        *end = '?';
                        while (*++start == ' ') ;
                    }
                    else
                    {
                        *end = ' ';
                        char* segment = start;
                        if ((uint)(*start - 0x4E00) <= 0X9FA5 - 0x4E00)
                        {
                            while ((uint)(*++start - 0x4E00) <= 0X9FA5 - 0x4E00) ;
                            if ((length = (int)(start - segment)) == 1)
                            {
                                words.Add(subString.Unsafe(text, (int)(segment - textFixed), 1));
                            }
                            else
                            {
                                int startIndex = (int)(segment - textFixed);
                                matchs.Empty();
                                matchWord.UnsafeSet(text, startIndex, length);
                                wordTrieGraph.LeftRightMatchs(ref matchWord, matchs);
                                if ((count = matchs.Count) != 0)
                                {
                                    foreach (keyValue<int, int> value in matchs.UnsafeArray)
                                    {
                                        words.Add(subString.Unsafe(text, value.Key, value.Value));
                                        matchMap.Set(value.Key, value.Value);
                                        if (--count == 0) break;
                                    }
                                }
                                int index = startIndex;
                                for (int endIndex = startIndex + length; index != endIndex; ++index)
                                {
                                    if (matchMap.Get(index))
                                    {
                                        if ((count = index - startIndex) != 1)
                                        {
                                            words.Add(subString.Unsafe(text, startIndex, count));
                                        }
                                        startIndex = index;
                                    }
                                    else words.Add(subString.Unsafe(text, index, 1));
                                }
                                if ((index -= startIndex) > 1) words.Add(subString.Unsafe(text, startIndex, index));
                            }
                        }
                        else
                        {
                            byte type = charTypes[*start];
                            if (type == (byte)charType.OtherLetter)
                            {
                                while (charTypes[*++start] == (byte)charType.OtherLetter) ;
                            }
                            else
                            {
                                char* word = start;
                                for (byte newType = charTypes[*++start]; newType >= (byte)charType.Letter; newType = charTypes[*++start])
                                {
                                    if (type != newType)
                                    {
                                        if (type != (byte)charType.Keep)
                                        {
                                            words.Add(subString.Unsafe(text, (int)(word - textFixed), (int)(start - word)));
                                        }
                                        type = newType;
                                        word = start;
                                    }
                                }
                            }
                            words.Add(subString.Unsafe(text, (int)(segment - textFixed), (int)(start - segment)));
                        }
                    }
                }
                typePool<list<keyValue<int, int>>>.PushNotNull(matchs);
                if ((count = words.Count) == 0)
                {
                    typePool<list<subString>>.PushNotNull(words);
                    return null;
                }
                return words;
            }
        }
        /// <summary>
        /// 搜索字符串分词
        /// </summary>
        /// <param name="text">搜索字符串</param>
        /// <returns>分词结果</returns>
        protected list<subString> getWords(string text)
        {
            if (text != null)
            {
                int length = text.Length;
                if (length != 0)
                {
                    list<subString> words = length <= maxSearchSize ? getWords(text + " ", length) : getWords(text, maxSearchSize);
                    if (words != null)
                    {
                        int index = words.Count;
                        if (index > 1)
                        {
                            subString[] wordArray = words.UnsafeArray;
                            int count = 0;
                            if (words.Count <= 4)
                            {
                                foreach (subString word in wordArray)
                                {
                                    if (count == 0) count = 1;
                                    else
                                    {
                                        int nextIndex = count;
                                        foreach (subString cmpWord in wordArray)
                                        {
                                            if (cmpWord.Equals(word) || --nextIndex == 0) break;
                                        }
                                        if (nextIndex == 0) wordArray[count++] = word;
                                    }
                                    if (--index == 0) break;
                                }
                            }
                            else
                            {
                                HashSet<hashString> wordHash = typePool<HashSet<hashString>>.Pop();
                                if (wordHash == null) wordHash = hashSet.CreateHashString();
                                else if (wordHash.Count != 0) wordHash.Clear();
                                foreach (subString word in wordArray)
                                {
                                    if (count == 0)
                                    {
                                        wordHash.Add(word);
                                        count = 1;
                                    }
                                    else
                                    {
                                        hashString wordKey = word;
                                        if (!wordHash.Contains(wordKey))
                                        {
                                            wordArray[count++] = word;
                                            wordHash.Add(wordKey);
                                        }
                                    }
                                    if (--index == 0) break;
                                }
                                wordHash.Clear();
                                typePool<HashSet<hashString>>.PushNotNull(wordHash);
                            }
                            words.UnsafeAddLength(count - words.Count);
                        }
                    }
                    return words;
                }
            }
            return null;
        }
        /// <summary>
        /// 数据文本分词
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>分词结果</returns>
        protected unsafe Dictionary<hashString, list<int>> getAllWords(string text)
        {
            if (text != null)
            {
                int length = text.Length;
                if (length != 0)
                {
                    list<subString> words = getWords(text + " ", length);
                    if (words != null)
                    {
                        Dictionary<hashString, list<int>> dictionary = typePool<Dictionary<hashString, list<int>>>.Pop();
                        if (dictionary == null) dictionary = fastCSharp.dictionary.CreateHashString<list<int>>();
                        else if (dictionary.Count != 0) dictionary.Clear();
                        list<int> indexs;
                        int count = words.Count;
                        foreach (subString word in words.UnsafeArray)
                        {
                            hashString wordKey = word;
                            if (!dictionary.TryGetValue(wordKey, out indexs))
                            {
                                indexs = typePool<list<int>>.Pop();
                                if (indexs == null) indexs = new list<int>();
                                else indexs.Empty();
                                dictionary.Add(wordKey, indexs);
                            }
                            indexs.Add(word.StartIndex);
                            if (--count == 0) break;
                        }
                        words.Clear();
                        typePool<list<subString>>.PushNotNull(words);
                        return dictionary;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 释放分词结果
        /// </summary>
        /// <param name="values">分词结果</param>
        protected static void free(Dictionary<hashString, list<int>> values)
        {
            foreach (list<int> indexs in values.Values) typePool<list<int>>.PushOnly(indexs);
            values.Clear();
            typePool<Dictionary<hashString, list<int>>>.PushNotNull(values);
        }
        /// <summary>
        /// 字符类型集合
        /// </summary>
        private static readonly pointer.reference charTypePointer;
        unsafe static searcher()
        {
            charTypePointer = unmanaged.GetStatic(65536, true).Reference;
            byte* start = charTypePointer.Byte, end = charTypePointer.Byte + 65536;
            for (char code = (char)0; start != end; ++start, ++code)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(code);
                if (category == UnicodeCategory.LowercaseLetter || category == UnicodeCategory.UppercaseLetter
                        || category == UnicodeCategory.TitlecaseLetter || category == UnicodeCategory.ModifierLetter)
                {
                    *start = (byte)charType.Letter;
                }
                else if (category == UnicodeCategory.DecimalDigitNumber
                        || category == UnicodeCategory.LetterNumber || category == UnicodeCategory.OtherNumber)
                {
                    *start = (byte)charType.Number;
                }
                else if (category == UnicodeCategory.OtherLetter) *start = (byte)charType.OtherLetter;
                else if (code == '&' || code == '.' || code == '+' || code == '#') *start = (byte)charType.Keep;
            }
        }
    }
    /// <summary>
    /// 分词搜索器
    /// </summary>
    /// <typeparam name="keyType">数据标识类型</typeparam>
    public sealed class searcher<keyType> : searcher where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 关键字词频与数据结果
        /// </summary>
        private struct counter
        {
            /// <summary>
            /// 词频
            /// </summary>
            public int Count;
            /// <summary>
            /// 数据结果
            /// </summary>
            public Dictionary<keyType, int[]> Values;
            /// <summary>
            /// 添加数据结果
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="values">数据结果</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Add(keyType key, int[] values)
            {
                Values.Add(key, values);
                Count += values.Length;
            }
            /// <summary>
            /// 删除关键字
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>删除结果数量</returns>
            public int Remove(keyType key)
            {
                int[] indexs;
                if (Values.TryGetValue(key, out indexs))
                {
                    Values.Remove(key);
                    Count -= indexs.Length;
                    return indexs.Length;
                }
                return 0;
            }
        }
        /// <summary>
        /// 关键字数据结果池
        /// </summary>
        private static counter[] counterPool = new counter[256];
        /// <summary>
        /// 当前分配结果索引
        /// </summary>
        private static int counterIndex;
        /// <summary>
        /// 关键字数据结果池访问锁
        /// </summary>
        private static readonly object counterLock = new object();
        /// <summary>
        /// 获取结果索引
        /// </summary>
        /// <returns>结果索引</returns>
        private static int getCounterIndex()
        {
            Monitor.Enter(counterLock);
            try
            {
                if (counterIndex == counterPool.Length)
                {
                    counter[] newPool = new counter[counterIndex << 1];
                    counterPool.CopyTo(newPool, 0);
                    counterPool = newPool;
                }
                counterPool[counterIndex].Values = dictionary<keyType>.Create<int[]>();
                return counterIndex++;
            }
            finally { Monitor.Exit(counterLock); }
        }
        /// <summary>
        /// 关键字数据结果集合
        /// </summary>
        private Dictionary<hashString, int> results = dictionary.CreateHashString<int>();
        /// <summary>
        /// 分词搜索器
        /// </summary>
        /// <param name="wordTrieGraph">分词trie图</param>
        /// <param name="values">数据集合[关键字+数据对象]</param>
        /// <param name="maxSearchSize">最大搜索字符串长度</param>
        public searcher(trieGraph wordTrieGraph, keyValue<string, keyType>[] values, int maxSearchSize = 128)
            : base(wordTrieGraph, maxSearchSize)
        {
            foreach (keyValue<string, keyType> value in values) add(value.Key, value.Value);
        }
        /// <summary>
        /// 添加新的数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="values">分词结果</param>
        private void add(keyType key, Dictionary<hashString, list<int>> values)
        {
            int counterIndex;
            foreach (KeyValuePair<hashString, list<int>> result in values)
            {
                if (!results.TryGetValue(result.Key, out counterIndex)) results.Add(result.Key, counterIndex = getCounterIndex());
                counterPool[counterIndex].Add(key, result.Value.GetArray());
                wordCount += result.Value.Count;
            }
        }
        /// <summary>
        /// 添加新的数据
        /// </summary>
        /// <param name="text">数据文本</param>
        /// <param name="key">数据标识</param>
        private void add(string text, keyType key)
        {
            Dictionary<hashString, list<int>> values = getAllWords(text);
            if (values != null)
            {
                add(key, values);
                free(values);
            }
        }
        /// <summary>
        /// 添加新的数据
        /// </summary>
        /// <param name="text">数据文本</param>
        /// <param name="key">数据标识</param>
        public void Add(string text, keyType key)
        {
            Dictionary<hashString, list<int>> values = getAllWords(text);
            if (values != null)
            {
                Monitor.Enter(resultLock);
                try
                {
                    add(key, values);
                }
                finally
                {
                    Monitor.Exit(resultLock);
                    free(values);
                }
            }
        }
        /// <summary>
        /// 删除新的数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="values">分词结果</param>
        private void remove(keyType key, Dictionary<hashString, list<int>> values)
        {
            counter[] pool = counterPool;
            int counterIndex;
            foreach (KeyValuePair<hashString, list<int>> result in values)
            {
                if (results.TryGetValue(result.Key, out counterIndex)) wordCount -= pool[counterIndex].Remove(key);
            }
        }
        /// <summary>
        /// 删除无效的数据
        /// </summary>
        /// <param name="text">数据文本</param>
        /// <param name="key">数据标识</param>
        public void Remove(string text, keyType key)
        {
            Dictionary<hashString, list<int>> values = getAllWords(text);
            if (values != null)
            {
                Monitor.Enter(resultLock);
                try
                {
                    remove(key, values);
                }
                finally
                {
                    Monitor.Exit(resultLock);
                    free(values);
                }
            }
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="text">数据文本</param>
        /// <param name="oldText">旧数据文本</param>
        /// <param name="key">数据标识</param>
        public void Update(string text, string oldText, keyType key)
        {
            Dictionary<hashString, list<int>> values = getAllWords(text);
            Dictionary<hashString, list<int>> oldValues = getAllWords(oldText);
            Monitor.Enter(resultLock);
            try
            {
                if (oldValues != null) remove(key, oldValues);
                if (values != null) add(key, values);
            }
            finally
            {
                Monitor.Exit(resultLock);
                if (oldValues != null) free(oldValues);
                if (values != null) free(values);
            }
        }
        ///// <summary>
        ///// 判断是否存在匹配项
        ///// </summary>
        ///// <param name="text">搜索文本</param>
        ///// <param name="isMatch">匹配委托</param>
        ///// <returns>是否存在匹配项</returns>
        //public bool IsMatch(string text, func<Dictionary<keyType, int[]>, bool> isMatch)
        //{
        //    list<subString> words = getWords(text);
        //    if (words != null)
        //    {
        //        counter counter;
        //        int count = words.Count;
        //        interlocked.CompareSetSleep0NoCheck(ref resultLock);
        //        try
        //        {
        //            foreach (subString word in words.Unsafer.Array)
        //            {
        //                if (!results.TryGetValue(word, out counter) || !isMatch(counter.Values)) return false;
        //                if (--count == 0) break;
        //            }
        //        }
        //        finally
        //        {
        //            resultLock = 0;
        //            words.Clear();
        //            typePool<list<subString>>.Push(words);
        //        }
        //        return true;
        //    }
        //    return false;
        //}
        /// <summary>
        /// 搜索匹配项
        /// </summary>
        /// <param name="text">搜索文本</param>
        /// <param name="list">匹配项集合</param>
        /// <param name="merge">匹配项集合归并处理</param>
        public void Search(string text, list<Dictionary<keyType, int[]>> list, Action merge)
        {
            if (list != null)
            {
                list<subString> words = getWords(text);
                if (words != null)
                {
                    counter[] pool = counterPool;
                    int count = words.Count, counterIndex;
                    Monitor.Enter(resultLock);
                    try
                    {
                        foreach (subString word in words.UnsafeArray)
                        {
                            if (results.TryGetValue(word, out counterIndex)) list.Add(pool[counterIndex].Values);
                            if (--count == 0) break;
                        }
                        if (list.Count != 0) merge();
                    }
                    finally
                    {
                        Monitor.Exit(resultLock);
                        words.Clear();
                        typePool<list<subString>>.PushNotNull(words);
                    }
                }
            }
        }
        static searcher()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
