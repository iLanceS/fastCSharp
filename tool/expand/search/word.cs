using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace fastCSharp.search
{
    /// <summary>
    /// 默认中文分词词语
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public sealed class word
    {
        /// <summary>
        /// 词
        /// </summary>
        public string Name;
        /// <summary>
        /// 频率
        /// </summary>
        public int Frequency;
        /// <summary>
        /// 词性
        /// </summary>
        public sealed class partOfSpeech : Attribute
        {
            /// <summary>
            /// 词性简写分类
            /// </summary>
            private static Dictionary<string, category> shortNameCategorys;
            /// <summary>
            /// 根据词性简写获取词性分类
            /// </summary>
            /// <param name="name">词性简写</param>
            /// <returns>词性分类</returns>
            public static category GetCategoryByShortName(string name)
            {
                if (shortNameCategorys == null)
                {
                    shortNameCategorys = (System.Enum.GetValues(typeof(category)) as category[])
                        .getDictionary(value => (string)fastCSharp.Enum<category, partOfSpeech>.Dictionary(value).ShortName);
                }
                category category;
                return shortNameCategorys.TryGetValue(name.ToLower(), out category) ? category : category.None;
            }

            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 简写
            /// </summary>
            public string ShortName;
            /// <summary>
            /// 父级分类
            /// </summary>
            public category Parent;
        }
        /// <summary>
        /// 词性分类大小
        /// </summary>
        public const int CategoryCapacity = 0x10000;
        /// <summary>
        /// 词性分类
        /// </summary>
        public enum category
        {
            /// <summary>
            /// 未知分类
            /// </summary>
            [partOfSpeech(ShortName = "", Name = "未知", Parent = category.None)]
            None = 0,

            #region 名词
            /// <summary>
            /// 名词
            /// </summary>
            [partOfSpeech(ShortName = "n", Name = "名词", Parent = category.Noun)]
            Noun = 1,
            /// <summary>
            /// 人名
            /// </summary>
            [partOfSpeech(ShortName = "nr", Name = "人名", Parent = category.Noun)]
            Names,
            /// <summary>
            /// 汉语姓氏
            /// </summary>
            [partOfSpeech(ShortName = "nr1", Name = "汉语姓氏", Parent = category.Noun)]
            ChineseLastName,
            /// <summary>
            /// 汉语名字
            /// </summary>
            [partOfSpeech(ShortName = "nr2", Name = "汉语名字", Parent = category.Noun)]
            ChineseName,
            /// <summary>
            /// 日语人名
            /// </summary>
            [partOfSpeech(ShortName = "nrj", Name = "日语人名", Parent = category.Noun)]
            JapaneseName,
            /// <summary>
            /// 音译人名
            /// </summary>
            [partOfSpeech(ShortName = "nrf", Name = "音译人名", Parent = category.Noun)]
            TransliterationName,
            /// <summary>
            /// 地名
            /// </summary>
            [partOfSpeech(ShortName = "ns", Name = "地名", Parent = category.Noun)]
            PlaceName,
            /// <summary>
            /// 音译地名
            /// </summary>
            [partOfSpeech(ShortName = "nsf", Name = "音译地名", Parent = category.Noun)]
            TransliterationPlaceName,
            /// <summary>
            /// 机构团体名
            /// </summary>
            [partOfSpeech(ShortName = "nt", Name = "机构团体名", Parent = category.Noun)]
            OrganizationGroupName,
            /// <summary>
            /// 其它专名
            /// </summary>
            [partOfSpeech(ShortName = "nz", Name = "其它专名", Parent = category.Noun)]
            OtherProfessionalName,
            /// <summary>
            /// 名词性惯用语
            /// </summary>
            [partOfSpeech(ShortName = "nl", Name = "名词性惯用语", Parent = category.Noun)]
            NounPhrase,
            /// <summary>
            /// 名词性语素
            /// </summary>
            [partOfSpeech(ShortName = "ng", Name = "名词性语素", Parent = category.Noun)]
            NounMorpheme,
            #endregion

            #region 时间词
            /// <summary>
            /// 时间词
            /// </summary>
            [partOfSpeech(ShortName = "t", Name = "时间词", Parent = category.Time)]
            Time = 2 * CategoryCapacity,
            /// <summary>
            /// 时间词性语素
            /// </summary>
            [partOfSpeech(ShortName = "tg", Name = "时间词性语素", Parent = category.Time)]
            TimeMorpheme,
            #endregion

            #region 处所词
            /// <summary>
            /// 处所词
            /// </summary>
            [partOfSpeech(ShortName = "s", Name = "处所词", Parent = category.Premises)]
            Premises = 3 * CategoryCapacity,
            #endregion

            #region 方位词
            /// <summary>
            /// 方位词
            /// </summary>
            [partOfSpeech(ShortName = "f", Name = "方位词", Parent = category.Position)]
            Position = 4 * CategoryCapacity,
            #endregion

            #region 动词
            /// <summary>
            /// 动词
            /// </summary>
            [partOfSpeech(ShortName = "v", Name = "动词", Parent = category.Verb)]
            Verb = 5 * CategoryCapacity,
            /// <summary>
            /// 副动词
            /// </summary>
            [partOfSpeech(ShortName = "vd", Name = "副动词", Parent = category.Verb)]
            ViceVerb,
            /// <summary>
            /// 名动词
            /// </summary>
            [partOfSpeech(ShortName = "vn", Name = "名动词", Parent = category.Verb)]
            NameVerb,
            /// <summary>
            /// 动词"是"
            /// </summary>
            [partOfSpeech(ShortName = "vshi", Name = @"动词""是""", Parent = category.Verb)]
            IsVerb,
            /// <summary>
            /// 动词"有"
            /// </summary>
            [partOfSpeech(ShortName = "vyou", Name = @"动词""有""", Parent = category.Verb)]
            HaveVerb,
            /// <summary>
            /// 趋向动词
            /// </summary>
            [partOfSpeech(ShortName = "vf", Name = "趋向动词", Parent = category.Verb)]
            TendVerb,
            /// <summary>
            /// 形式动词
            /// </summary>
            [partOfSpeech(ShortName = "vx", Name = "形式动词", Parent = category.Verb)]
            FormVerb,
            /// <summary>
            /// 不及物动词（内动词）
            /// </summary>
            [partOfSpeech(ShortName = "vi", Name = "不及物动词（内动词）", Parent = category.Verb)]
            IntransitiveVerb,
            /// <summary>
            /// 动词性惯用语
            /// </summary>
            [partOfSpeech(ShortName = "vl", Name = "动词性惯用语", Parent = category.Verb)]
            VerbIdiom,
            /// <summary>
            /// 动词性语素
            /// </summary>
            [partOfSpeech(ShortName = "vg", Name = "动词性语素", Parent = category.Verb)]
            VerbMorpheme,
            #endregion

            #region 形容词
            /// <summary>
            /// 形容词
            /// </summary>
            [partOfSpeech(ShortName = "a", Name = "形容词", Parent = category.Adjective)]
            Adjective = 6 * CategoryCapacity,
            /// <summary>
            /// 副形词
            /// </summary>
            [partOfSpeech(ShortName = "ad", Name = "副形词", Parent = category.Adjective)]
            ViceAdjective,
            /// <summary>
            /// 名形词
            /// </summary>
            [partOfSpeech(ShortName = "an", Name = "名形词", Parent = category.Adjective)]
            NameAdjective,
            /// <summary>
            /// 形容词性语素
            /// </summary>
            [partOfSpeech(ShortName = "ag", Name = "形容词性语素", Parent = category.Adjective)]
            AdjectiveMorpheme,
            /// <summary>
            /// 形容词性惯用语
            /// </summary>
            [partOfSpeech(ShortName = "al", Name = "形容词性惯用语", Parent = category.Adjective)]
            AdjectivePhrase,
            #endregion

            #region 区别词
            /// <summary>
            /// 区别词
            /// </summary>
            [partOfSpeech(ShortName = "b", Name = "区别词", Parent = category.Difference)]
            Difference = 7 * CategoryCapacity,
            /// <summary>
            /// 区别词性惯用语
            /// </summary>
            [partOfSpeech(ShortName = "bl", Name = "区别词性惯用语", Parent = category.Difference)]
            DifferenceIdiom,
            #endregion

            #region 状态词
            /// <summary>
            /// 状态词
            /// </summary>
            [partOfSpeech(ShortName = "z", Name = "状态词", Parent = category.State)]
            State = 8 * CategoryCapacity,
            #endregion

            #region 代词
            /// <summary>
            /// 代词
            /// </summary>
            [partOfSpeech(ShortName = "r", Name = "代词", Parent = category.Pronoun)]
            Pronoun = 9 * CategoryCapacity,
            /// <summary>
            /// 人称代词
            /// </summary>
            [partOfSpeech(ShortName = "rr", Name = "人称代词", Parent = category.Pronoun)]
            PersonPronoun,
            /// <summary>
            /// 指示代词
            /// </summary>
            [partOfSpeech(ShortName = "rz", Name = "指示代词", Parent = category.Pronoun)]
            Demonstrative,
            /// <summary>
            /// 时间指示代词
            /// </summary>
            [partOfSpeech(ShortName = "rzt", Name = "时间指示代词", Parent = category.Pronoun)]
            TimePronoun,
            /// <summary>
            /// 处所指示代词
            /// </summary>
            [partOfSpeech(ShortName = "rzs", Name = "处所指示代词", Parent = category.Pronoun)]
            PremisesPronoun,
            /// <summary>
            /// 谓词性指示代词
            /// </summary>
            [partOfSpeech(ShortName = "rzv", Name = "谓词性指示代词", Parent = category.Pronoun)]
            PredicatePronoun,
            /// <summary>
            /// 疑问代词
            /// </summary>
            [partOfSpeech(ShortName = "ry", Name = "疑问代词", Parent = category.Pronoun)]
            InterrogativePronoun,
            /// <summary>
            /// 时间疑问代词
            /// </summary>
            [partOfSpeech(ShortName = "ryt", Name = "时间疑问代词", Parent = category.Pronoun)]
            TimeInterrogativePronoun,
            /// <summary>
            /// 处所疑问代词
            /// </summary>
            [partOfSpeech(ShortName = "rys", Name = "处所疑问代词", Parent = category.Pronoun)]
            PremisesInterrogativePronoun,
            /// <summary>
            /// 谓词性疑问代词
            /// </summary>
            [partOfSpeech(ShortName = "ryv", Name = "谓词性疑问代词", Parent = category.Pronoun)]
            VerbInterrogativePronoun,
            /// <summary>
            /// 代词性语素
            /// </summary>
            [partOfSpeech(ShortName = "rg", Name = "代词性语素", Parent = category.Pronoun)]
            PronounMorpheme,
            #endregion

            #region 数词
            /// <summary>
            /// 数词
            /// </summary>
            [partOfSpeech(ShortName = "m", Name = "数词", Parent = category.Numeral)]
            Numeral = 10 * CategoryCapacity,
            /// <summary>
            /// 数量词
            /// </summary>
            [partOfSpeech(ShortName = "mq", Name = "数量词", Parent = category.Numeral)]
            Quantifier,
            #endregion

            #region 量词
            /// <summary>
            /// 量词
            /// </summary>
            [partOfSpeech(ShortName = "q", Name = "量词", Parent = category.Measure)]
            Measure = 11 * CategoryCapacity,
            /// <summary>
            /// 动量词
            /// </summary>
            [partOfSpeech(ShortName = "qv", Name = "动量词", Parent = category.Measure)]
            Momentum,
            /// <summary>
            /// 时量词
            /// </summary>
            [partOfSpeech(ShortName = "qt", Name = "时量词", Parent = category.Measure)]
            WhenMomentum,
            #endregion

            #region 副词
            /// <summary>
            /// 副词
            /// </summary>
            [partOfSpeech(ShortName = "d", Name = "副词", Parent = category.Adverb)]
            Adverb = 12 * CategoryCapacity,
            #endregion

            #region 介词
            /// <summary>
            /// 介词
            /// </summary>
            [partOfSpeech(ShortName = "p", Name = "介词", Parent = category.Preposition)]
            Preposition = 13 * CategoryCapacity,
            /// <summary>
            /// 介词"把"
            /// </summary>
            [partOfSpeech(ShortName = "pba", Name = @"介词""把""", Parent = category.Preposition)]
            ToPreposition,
            /// <summary>
            /// 介词"被"
            /// </summary>
            [partOfSpeech(ShortName = "pbei", Name = @"介词""被""", Parent = category.Preposition)]
            ByPreposition,
            #endregion

            #region 连词
            /// <summary>
            /// 连词
            /// </summary>
            [partOfSpeech(ShortName = "c", Name = "连词", Parent = category.Conjunction)]
            Conjunction = 14 * CategoryCapacity,
            /// <summary>
            /// 并列连词
            /// </summary>
            [partOfSpeech(ShortName = "cc", Name = "并列连词", Parent = category.Conjunction)]
            CoordinatingConjunction,
            #endregion

            #region 助词
            /// <summary>
            /// 助词
            /// </summary>
            [partOfSpeech(ShortName = "u", Name = "助词", Parent = category.Particle)]
            Particle = 15 * CategoryCapacity,
            /// <summary>
            /// 着
            /// </summary>
            [partOfSpeech(ShortName = "uzhe", Name = "着", Parent = category.Particle)]
            With,
            /// <summary>
            /// 了 喽
            /// </summary>
            [partOfSpeech(ShortName = "ule", Name = "了 喽", Parent = category.Particle)]
            PastTenseMarker,
            /// <summary>
            /// 过
            /// </summary>
            [partOfSpeech(ShortName = "uguo", Name = "过", Parent = category.Particle)]
            Had,
            /// <summary>
            /// 的 底
            /// </summary>
            [partOfSpeech(ShortName = "ude1", Name = "的 底", Parent = category.Particle)]
            Of,
            /// <summary>
            /// 地
            /// </summary>
            [partOfSpeech(ShortName = "ude2", Name = "地", Parent = category.Particle)]
            AdverbialParticle,
            /// <summary>
            /// 得
            /// </summary>
            [partOfSpeech(ShortName = "ude3", Name = "得", Parent = category.Particle)]
            OughtTo,
            /// <summary>
            /// 所
            /// </summary>
            [partOfSpeech(ShortName = "usuo", Name = "所", Parent = category.Particle)]
            The,
            /// <summary>
            /// 等 等等 云云
            /// </summary>
            [partOfSpeech(ShortName = "udeng", Name = "等 等等 云云", Parent = category.Particle)]
            EtCetera,
            /// <summary>
            /// 一样 一般 似的 般
            /// </summary>
            [partOfSpeech(ShortName = "uyy", Name = "一样 一般 似的 般", Parent = category.Particle)]
            Like,
            /// <summary>
            /// 的话
            /// </summary>
            [partOfSpeech(ShortName = "udh", Name = "的话", Parent = category.Particle)]
            If,
            /// <summary>
            /// 来讲 来说 而言 说来
            /// </summary>
            [partOfSpeech(ShortName = "uls", Name = "来讲 来说 而言 说来", Parent = category.Particle)]
            InTermsOf,
            /// <summary>
            /// 之
            /// </summary>
            [partOfSpeech(ShortName = "uzhi", Name = "之", Parent = category.Particle)]
            SubordinateParticle,
            /// <summary>
            /// 连
            /// </summary>
            [partOfSpeech(ShortName = "ulian", Name = "连", Parent = category.Particle)]
            Even,
            #endregion

            #region 叹词
            /// <summary>
            /// 叹词
            /// </summary>
            [partOfSpeech(ShortName = "e", Name = "叹词", Parent = category.Interjection)]
            Interjection = 16 * CategoryCapacity,
            #endregion

            #region 语气词
            /// <summary>
            /// 语气词
            /// </summary>
            [partOfSpeech(ShortName = "y", Name = "语气词", Parent = category.Tone)]
            Tone = 17 * CategoryCapacity,
            #endregion

            #region 拟声词
            /// <summary>
            /// 拟声词
            /// </summary>
            [partOfSpeech(ShortName = "o", Name = "拟声词", Parent = category.Onomatopoeia)]
            Onomatopoeia = 18 * CategoryCapacity,
            #endregion

            #region 前缀
            /// <summary>
            /// 前缀
            /// </summary>
            [partOfSpeech(ShortName = "h", Name = "前缀", Parent = category.prefix)]
            prefix = 19 * CategoryCapacity,
            #endregion

            #region 后缀
            /// <summary>
            /// 后缀
            /// </summary>
            [partOfSpeech(ShortName = "k", Name = "后缀", Parent = category.Suffix)]
            Suffix = 20 * CategoryCapacity,
            #endregion

            #region 字符串
            /// <summary>
            /// 字符串
            /// </summary>
            [partOfSpeech(ShortName = "x", Name = "字符串", Parent = category.String)]
            String = 21 * CategoryCapacity,
            /// <summary>
            /// 非语素字
            /// </summary>
            [partOfSpeech(ShortName = "xx", Name = "非语素字", Parent = category.String)]
            NonMorpheme,
            /// <summary>
            /// 网址URL
            /// </summary>
            [partOfSpeech(ShortName = "xu", Name = "网址URL", Parent = category.String)]
            Url,
            #endregion

            #region 标点符号
            /// <summary>
            /// 标点符号
            /// </summary>
            [partOfSpeech(ShortName = "w", Name = "标点符号", Parent = category.Punctuation)]
            Punctuation = 22 * CategoryCapacity,
            /// <summary>
            /// 左括号，全角：（ 〔  ［  ｛  《 【  〖〈   半角：( [ { &lt;
            /// </summary>
            [partOfSpeech(ShortName = "wkz", Name = "左括号", Parent = category.Punctuation)]
            LeftParenthesis,
            /// <summary>
            /// 右括号，全角：） 〕  ］ ｝ 》  】 〗 〉 半角： ) ] { >
            /// </summary>
            [partOfSpeech(ShortName = "wky", Name = "右括号", Parent = category.Punctuation)]
            RightParenthesis,
            /// <summary>
            /// 左引号，全角：" ' 『 
            /// </summary>
            [partOfSpeech(ShortName = "wyz", Name = "左引号", Parent = category.Punctuation)]
            LeftQuote,
            /// <summary>
            /// 右引号，全角：" ' 』
            /// </summary>
            [partOfSpeech(ShortName = "wyy", Name = "右引号", Parent = category.Punctuation)]
            RightQuote,
            /// <summary>
            /// 句号，全角：。
            /// </summary>
            [partOfSpeech(ShortName = "wj", Name = "句号", Parent = category.Punctuation)]
            Period,
            /// <summary>
            /// 问号，全角：？ 半角：?
            /// </summary>
            [partOfSpeech(ShortName = "ww", Name = "句号", Parent = category.Punctuation)]
            QuestionMark,
            /// <summary>
            /// 叹号，全角：！ 半角：!
            /// </summary>
            [partOfSpeech(ShortName = "wt", Name = "叹号", Parent = category.Punctuation)]
            ExclamationMark,
            /// <summary>
            /// 逗号，全角：， 半角：,
            /// </summary>
            [partOfSpeech(ShortName = "wd", Name = "逗号", Parent = category.Punctuation)]
            Comma,
            /// <summary>
            /// 分号，全角：； 半角： ;
            /// </summary>
            [partOfSpeech(ShortName = "wf", Name = "分号", Parent = category.Punctuation)]
            Semicolon,
            /// <summary>
            /// 顿号，全角：、
            /// </summary>
            [partOfSpeech(ShortName = "wn", Name = "顿号", Parent = category.Punctuation)]
            CommaAnd,
            /// <summary>
            /// 冒号，全角：： 半角： :
            /// </summary>
            [partOfSpeech(ShortName = "wm", Name = "冒号", Parent = category.Punctuation)]
            Colon,
            /// <summary>
            /// 省略号，全角：……  …
            /// </summary>
            [partOfSpeech(ShortName = "ws", Name = "省略号", Parent = category.Punctuation)]
            Ellipsis,
            /// <summary>
            /// 破折号，全角：--   －－   --－   半角：---  ----
            /// </summary>
            [partOfSpeech(ShortName = "wp", Name = "破折号", Parent = category.Punctuation)]
            Dash,
            /// <summary>
            /// 百分号千分号，全角：％ ‰   半角：%
            /// </summary>
            [partOfSpeech(ShortName = "wb", Name = "百分号千分号", Parent = category.Punctuation)]
            PercentThousands,
            /// <summary>
            /// 单位符号，全角：￥ ＄ ￡  °  ℃  半角：$
            /// </summary>
            [partOfSpeech(ShortName = "wh", Name = "单位符号", Parent = category.Punctuation)]
            UnitSymbol,
            #endregion
        }
        /// <summary>
        /// 词性
        /// </summary>
        public category Catagory;

        ///// <summary>
        ///// 中文分词词语集合
        ///// </summary>
        //private readonly static word[] cache;
        /// <summary>
        /// 中文分词词语集合
        /// </summary>
        public static string[] Words { get; private set; }
        /// <summary>
        /// 中文分词词语集合写入文本文件
        /// </summary>
        private unsafe static void writeTxtFile()
        {
            string[] words = Words;
            using (unmanagedStream wordStream = new unmanagedStream())
            {
                *(int*)wordStream.Data = words.Length;
                wordStream.UnsafeAddLength(sizeof(int));
                foreach (string word in words)
                {
                    wordStream.Write(word);
                    wordStream.Write((char)0);
                }
                subArray<byte> data = io.compression.stream.Deflate.GetCompress(wordStream.GetArray(), 0);
                using (FileStream fileStream = new FileStream(fastCSharp.config.search.Default.WordTxtFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fileStream.Write(data.UnsafeArray, data.StartIndex, data.Count);
                }
            }
        }
        ///// <summary>
        ///// 从原始分词文件获取中文分词词语集合
        ///// </summary>
        ///// <param name="txt">原始分词文件内容</param>
        ///// <returns>中文分词词语集合</returns>
        //private static Dictionary<hashString, word> getWordsFormText(string txt)
        //{
        //    string[] strings = txt.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //    if ((strings.Length & 3) == 0)
        //    {
        //        Dictionary<hashString, word> values = dictionary.CreateOnly<string, word>();
        //        for (int index = 0; index != strings.Length; index += 4)
        //        {
        //            word value = new word
        //            {
        //                Name = strings[index + 1],
        //                Catagory = partOfSpeech.GetCategoryByShortName(strings[index + 3])
        //            };
        //            if (!int.TryParse(strings[index + 2], out value.Frequency)) return null;
        //            if (value.Name.Length * 3 != Encoding.UTF8.GetByteCount(value.Name))
        //            {
        //                Console.WriteLine(value.Name);
        //            }
        //            values[value.Name] = value;
        //        }
        //        return values;
        //    }
        //    return null;
        //}

        unsafe static word()
        {
            if (config.search.Default.IsSearch)
            {
                try
                {
                    string txtFile = fastCSharp.config.search.Default.WordTxtFileName;
                    if (txtFile != null && File.Exists(txtFile))
                    {
                        subArray<byte> data;
                        using (FileStream fileStream = new FileStream(txtFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            data = io.compression.stream.Deflate.GetDeCompress(fileStream);
                        }
                        fixed (byte* dataFixed = data.UnsafeArray)
                        {
                            string[] words = new string[*(int*)dataFixed];
                            int index = 0;
                            for (char* start = (char*)(dataFixed + sizeof(int)), read = start; index != words.Length; )
                            {
                                while (*(short*)read != 0) ++read;
                                words[index++] = new string(start, 0, (int)(read - start));
                                start = ++read;
                            }
                            Words = words;
                        }
                    }
                    if (Words == null)
                    {
                        string dataFile = fastCSharp.config.search.Default.WordSerializeFileName;
                        if (dataFile != null && File.Exists(dataFile))
                        {
                            subArray<byte> data;
                            using (FileStream fileStream = new FileStream(dataFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                data = io.compression.stream.Deflate.GetDeCompress(fileStream);
                            }
                            word[] words = fastCSharp.emit.dataDeSerializer.DeSerialize<word[]>(ref data);
                            Words = words.getArray(value => value.Name);
                            if (Words.Length != 0 && txtFile != null) fastCSharp.threading.threadPool.TinyPool.Start(writeTxtFile);
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
            if (Words == null) Words = nullValue<string>.Array;
        }
    }
}
