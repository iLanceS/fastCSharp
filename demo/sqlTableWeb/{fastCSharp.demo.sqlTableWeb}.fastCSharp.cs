//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.sqlTableWeb
{
        internal partial class Class : fastCSharp.code.cSharp.webView.IWebView
        {
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@"Class.html", 24))
                {
                    
                    response(htmls[0]);
                    response(htmls[2]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class _value1_ = ClassInfo;
                    if (_value1_ != null)
                    {
                        responseHtml(_value1_.Name);
                    }
                }
                    response(htmls[3]);
            _if_ = false;
                    if (ViewKeywords != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[4]);
                        responseHtml(ViewKeywords);
                    response(htmls[5]);
            }
                    response(htmls[6]);
            _if_ = false;
                    if (ViewDescription != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[7]);
                        responseHtml(ViewDescription);
                    response(htmls[5]);
            }
                    response(htmls[8]);
            _if_ = false;
                if (!(bool)FalseFlag)
                {
                    _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[9]);
                {
                    fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                    {
                        response(_value1_.ClassList);
                    }
                }
                    response(htmls[10]);
            }
                    response(htmls[11]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class _value1_ = default(fastCSharp.demo.sqlTableCacheServer.Class);
                    _value1_ = ClassInfo;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[12]);
                        responseHtml(_value1_.Name);
                    response(htmls[13]);
                        response(_value1_.Discipline.ToString());
                    response(htmls[14]);
                        response(_value1_.StudentCount.ToString());
                    response(htmls[15]);
                    response(htmls[16]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student[] _value2_ = default(fastCSharp.demo.sqlTableCacheServer.Student[]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class.remote _value3_ = _value1_.Remote;
                    {
                    _value2_ = _value3_.Students;
                    }
                }
                    if (_value2_ != null)
                    {
                        int _loopIndex2_ = _loopIndex_, _loopCount2_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value2_.Length;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student _value3_ in _value2_)
                        {
                    response(htmls[17]);
                {
                    fastCSharp.demo.sqlModel.path.Student _value4_ = _value3_.Path;
                    {
                        responseHtml(_value4_.Index);
                    }
                }
                    response(htmls[18]);
                        responseHtml(_value3_.Name);
                    response(htmls[19]);
                        responseHtml(_value3_.Email);
                    response(htmls[20]);
                        responseHtml(_value3_.Gender.ToString());
                    response(htmls[21]);
                    response(htmls[16]);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex2_;
                        _loopCount_ = _loopCount2_;
                    }
                }
                    response(htmls[22]);
            }
                }
                    response(htmls[23]);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{ClassInfo:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Class _value1_ = ClassInfo;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"Demo.Class.Get({DateRange:");
                    {
                        fastCSharp.demo.sqlModel.member.dateRange _value2_ = _value1_.DateRange;
                            js.WriteNotNull(@"{Start:");
                    {
                        fastCSharp.sql.member.intDate _value3_ = _value2_.Start;
                            js.WriteNotNull(@"{DateTime:");
                    {
                        System.DateTime _value4_ = _value3_.DateTime;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value4_, js);
                    }
                    js.WriteNotNull(@",Value:");
                    {
                        int _value4_ = _value3_.Value;
                                    fastCSharp.web.ajax.ToString((int)_value4_, js);
                    }
                    js.Write('}');
                    }
                    js.Write('}');
                    }
                    js.WriteNotNull(@",Discipline:");
                    {
                        fastCSharp.demo.sqlModel.member.discipline _value2_ = _value1_.Discipline;
                                    fastCSharp.web.ajax.ToString(_value2_.ToString(), js);
                    }
                    js.WriteNotNull(@",Id:");
                    {
                        int _value2_ = _value1_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value2_, js);
                    }
                    js.WriteNotNull(@",Name:");
                    {
                        string _value2_ = _value1_.Name;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value2_, js);
                        }
                    }
                    js.WriteNotNull(@",Remote:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Class.remote _value2_ = _value1_.Remote;
                            js.WriteNotNull(@"{Students:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Student[] _value3_ = _value2_.Students;
                        if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex3_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student _value4_ in _value3_)
                        {
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull("@.Demo.Student,,Birthday[DateTime,Value]Email,Gender,Id,Name");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');
                            if (_value4_ == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {
                                js.Write('[');
                    {
                        fastCSharp.sql.member.intDate _value5_ = _value4_.Birthday;
                                    js.Write('[');
                    {
                        System.DateTime _value6_ = _value5_.DateTime;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value6_, js);
                    }
                    js.Write(',');
                    {
                        int _value6_ = _value5_.Value;
                                    fastCSharp.web.ajax.ToString((int)_value6_, js);
                    }
                    js.Write(']');
                    }
                    js.Write(',');
                    {
                        string _value5_ = _value4_.Email;
                                if (_value5_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value5_, js);
                                }
                    }
                    js.Write(',');
                    {
                        fastCSharp.demo.sqlModel.member.gender _value5_ = _value4_.Gender;
                                    fastCSharp.web.ajax.ToString(_value5_.ToString(), js);
                    }
                    js.Write(',');
                    {
                        int _value5_ = _value4_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value5_, js);
                    }
                    js.Write(',');
                    {
                        string _value5_ = _value4_.Name;
                                if (_value5_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value5_, js);
                                }
                    }
                    js.Write(']');
                            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                    }
                    js.WriteNotNull(@"].FormatView()");
                        }
                    }
                    js.Write('}');
                    }
                    js.WriteNotNull(@",StudentCount:");
                    {
                        int _value2_ = _value1_.StudentCount;
                                    fastCSharp.web.ajax.ToString((int)_value2_, js);
                    }
                    js.WriteNotNull(@"})");
                        }
                    }
                    js.WriteNotNull(@",PubPath:");
                    {
                        fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                                    js.WriteNotNull(@"new fastCSharpPath.Pub({})");
                    }
                    js.Write('}');
            }

            private struct webViewQuery
            {
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                public int ClassId;
            }
            /// <summary>
            /// 查询参数
            /// </summary>
            private webViewQuery query;
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                if (base.loadView())
                {
                    
                    query= default(webViewQuery);
                    if (ParseParameter(ref query))
                    {
                        return loadView(query.ClassId);
                    }
                }
                return false;
            }
        }

}namespace fastCSharp.demo.sqlTableWeb
{
        internal partial class ClassList : fastCSharp.code.cSharp.webView.IWebView
        {
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@"ClassList.html", 20))
                {
                    
                    response(htmls[0]);
                    response(htmls[2]);
            _if_ = false;
                    if (ViewKeywords != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[3]);
                        responseHtml(ViewKeywords);
                    response(htmls[4]);
            }
                    response(htmls[5]);
            _if_ = false;
                    if (ViewDescription != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[6]);
                        responseHtml(ViewDescription);
                    response(htmls[4]);
            }
                    response(htmls[7]);
            _if_ = false;
                if (!(bool)IsClassList)
                {
                    _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[8]);
                {
                    fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                    {
                        response(_value1_.ClassList);
                    }
                }
                    response(htmls[9]);
            }
                    response(htmls[10]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class[] _value1_;
                    _value1_ = Classes;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Class _value2_ in _value1_)
                        {
                    response(htmls[11]);
                {
                    fastCSharp.demo.sqlModel.path.Class _value3_ = _value2_.Path;
                    {
                        responseHtml(_value3_.Index);
                    }
                }
                    response(htmls[12]);
                        responseHtml(_value2_.Name);
                    response(htmls[13]);
                        response(_value2_.Discipline.ToString());
                    response(htmls[14]);
                        response(_value2_.StudentCount.ToString());
                    response(htmls[15]);
                    response(htmls[16]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student[] _value3_ = default(fastCSharp.demo.sqlTableCacheServer.Student[]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class.remote _value4_ = _value2_.Remote;
                    {
                    _value3_ = _value4_.Students;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student _value4_ in _value3_)
                        {
                    response(htmls[8]);
                {
                    fastCSharp.demo.sqlModel.path.Student _value5_ = _value4_.Path;
                    {
                        responseHtml(_value5_.Index);
                    }
                }
                    response(htmls[12]);
                        responseHtml(_value4_.Name);
                    response(htmls[17]);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
                    response(htmls[18]);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                    response(htmls[19]);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{Classes:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Class[] _value1_ = Classes;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex1_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Class _value2_ in _value1_)
                        {
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull("@.Demo.Class,,DateRange[Start[DateTime,Value]]Discipline,Id,Name,Remote[Students[[@.Demo.Student,,Id,Name]]]StudentCount");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');
                            if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {
                                js.Write('[');
                    {
                        fastCSharp.demo.sqlModel.member.dateRange _value3_ = _value2_.DateRange;
                                    js.Write('[');
                    {
                        fastCSharp.sql.member.intDate _value4_ = _value3_.Start;
                                    js.Write('[');
                    {
                        System.DateTime _value5_ = _value4_.DateTime;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value5_, js);
                    }
                    js.Write(',');
                    {
                        int _value5_ = _value4_.Value;
                                    fastCSharp.web.ajax.ToString((int)_value5_, js);
                    }
                    js.Write(']');
                    }
                    js.Write(']');
                    }
                    js.Write(',');
                    {
                        fastCSharp.demo.sqlModel.member.discipline _value3_ = _value2_.Discipline;
                                    fastCSharp.web.ajax.ToString(_value3_.ToString(), js);
                    }
                    js.Write(',');
                    {
                        int _value3_ = _value2_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value3_, js);
                    }
                    js.Write(',');
                    {
                        string _value3_ = _value2_.Name;
                                if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value3_, js);
                                }
                    }
                    js.Write(',');
                    {
                        fastCSharp.demo.sqlTableCacheServer.Class.remote _value3_ = _value2_.Remote;
                                    js.Write('[');
                    {
                        fastCSharp.demo.sqlTableCacheServer.Student[] _value4_ = _value3_.Students;
                                if (_value4_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    js.WriteNotNull(@"[[");
                    {
                        int _loopIndex4_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student _value5_ in _value4_)
                        {
                            if (_loopIndex_ != 0) js.Write(',');
                            if (_value5_ == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {
                                js.Write('[');
                                
                    {
                        int _value6_ = _value5_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value6_, js);
                    }
                    js.Write(',');
                    {
                        string _value6_ = _value5_.Name;
                                if (_value6_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value6_, js);
                                }
                    }
                    js.Write(']');
                            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                    }
                    js.WriteNotNull(@"]]");
                                }
                    }
                    js.Write(']');
                    }
                    js.Write(',');
                    {
                        int _value3_ = _value2_.StudentCount;
                                    fastCSharp.web.ajax.ToString((int)_value3_, js);
                    }
                    js.Write(']');
                            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                    }
                    js.WriteNotNull(@"].FormatView()");
                        }
                    }
                    js.WriteNotNull(@",IsClassList:");
                    {
                        bool _value1_ = IsClassList;
                                    fastCSharp.web.ajax.ToString((bool)_value1_, js);
                    }
                    js.WriteNotNull(@",PubPath:");
                    {
                        fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                                    js.WriteNotNull(@"new fastCSharpPath.Pub({})");
                    }
                    js.Write('}');
            }

        }

}namespace fastCSharp.demo.sqlTableWeb
{
        internal partial class Student : fastCSharp.code.cSharp.webView.IWebView
        {
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@"Student.html", 25))
                {
                    
                    response(htmls[0]);
                    response(htmls[2]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student _value1_ = StudentInfo;
                    if (_value1_ != null)
                    {
                        responseHtml(_value1_.Name);
                    }
                }
                    response(htmls[3]);
            _if_ = false;
                    if (ViewKeywords != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[4]);
                        responseHtml(ViewKeywords);
                    response(htmls[5]);
            }
                    response(htmls[6]);
            _if_ = false;
                    if (ViewDescription != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[7]);
                        responseHtml(ViewDescription);
                    response(htmls[5]);
            }
                    response(htmls[8]);
            _if_ = false;
                if (!(bool)FalseFlag)
                {
                    _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[9]);
                {
                    fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                    {
                        response(_value1_.ClassList);
                    }
                }
                    response(htmls[10]);
            }
                    response(htmls[11]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student _value1_ = default(fastCSharp.demo.sqlTableCacheServer.Student);
                    _value1_ = StudentInfo;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[12]);
                        responseHtml(_value1_.Name);
                    response(htmls[13]);
                        responseHtml(_value1_.Email);
                    response(htmls[14]);
                        responseHtml(_value1_.Gender.ToString());
                    response(htmls[15]);
                    response(htmls[16]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student.remote.classDate[] _value2_ = default(fastCSharp.demo.sqlTableCacheServer.Student.remote.classDate[]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Student.remote _value3_ = _value1_.Remote;
                    {
                    _value2_ = _value3_.Classes;
                    }
                }
                    if (_value2_ != null)
                    {
                        int _loopIndex2_ = _loopIndex_, _loopCount2_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value2_.Length;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student.remote.classDate _value3_ in _value2_)
                        {
                    response(htmls[17]);
                    response(htmls[18]);
                {
                    fastCSharp.demo.sqlTableCacheServer.Class _value4_ = default(fastCSharp.demo.sqlTableCacheServer.Class);
                    _value4_ = _value3_.Class;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                    response(htmls[9]);
                {
                    fastCSharp.demo.sqlModel.path.Class _value5_ = _value4_.Path;
                    {
                        responseHtml(_value5_.Index);
                    }
                }
                    response(htmls[19]);
                        responseHtml(_value4_.Name);
                    response(htmls[20]);
                        response(_value4_.Discipline.ToString());
                    response(htmls[21]);
            }
                }
                    response(htmls[22]);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex2_;
                        _loopCount_ = _loopCount2_;
                    }
                }
                    response(htmls[23]);
            }
                }
                    response(htmls[24]);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{PubPath:");
                    {
                        fastCSharp.demo.sqlModel.path.Pub _value1_ = PubPath;
                                    js.WriteNotNull(@"new fastCSharpPath.Pub({})");
                    }
                    js.WriteNotNull(@",StudentInfo:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Student _value1_ = StudentInfo;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"Demo.Student.Get({Birthday:");
                    {
                        fastCSharp.sql.member.intDate _value2_ = _value1_.Birthday;
                            js.WriteNotNull(@"{DateTime:");
                    {
                        System.DateTime _value3_ = _value2_.DateTime;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value3_, js);
                    }
                    js.WriteNotNull(@",Value:");
                    {
                        int _value3_ = _value2_.Value;
                                    fastCSharp.web.ajax.ToString((int)_value3_, js);
                    }
                    js.Write('}');
                    }
                    js.WriteNotNull(@",Email:");
                    {
                        string _value2_ = _value1_.Email;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value2_, js);
                        }
                    }
                    js.WriteNotNull(@",Gender:");
                    {
                        fastCSharp.demo.sqlModel.member.gender _value2_ = _value1_.Gender;
                                    fastCSharp.web.ajax.ToString(_value2_.ToString(), js);
                    }
                    js.WriteNotNull(@",Id:");
                    {
                        int _value2_ = _value1_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value2_, js);
                    }
                    js.WriteNotNull(@",Name:");
                    {
                        string _value2_ = _value1_.Name;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value2_, js);
                        }
                    }
                    js.WriteNotNull(@",Remote:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Student.remote _value2_ = _value1_.Remote;
                            js.WriteNotNull(@"{Classes:");
                    {
                        fastCSharp.demo.sqlTableCacheServer.Student.remote.classDate[] _value3_ = _value2_.Classes;
                        if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex3_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.sqlTableCacheServer.Student.remote.classDate _value4_ in _value3_)
                        {
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull("Class[@.Demo.Class,,Discipline,Id,Name]ClassDate[Date[DateTime,Value]]");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');
                                js.Write('[');
                    {
                        fastCSharp.demo.sqlTableCacheServer.Class _value5_ = _value4_.Class;
                                if (_value5_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    js.Write('[');
                    {
                        fastCSharp.demo.sqlModel.member.discipline _value6_ = _value5_.Discipline;
                                    fastCSharp.web.ajax.ToString(_value6_.ToString(), js);
                    }
                    js.Write(',');
                    {
                        int _value6_ = _value5_.Id;
                                    fastCSharp.web.ajax.ToString((int)_value6_, js);
                    }
                    js.Write(',');
                    {
                        string _value6_ = _value5_.Name;
                                if (_value6_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value6_, js);
                                }
                    }
                    js.Write(']');
                                }
                    }
                    js.Write(',');
                    {
                        fastCSharp.demo.sqlModel.member.classDate _value5_ = _value4_.ClassDate;
                                    js.Write('[');
                    {
                        fastCSharp.sql.member.intDate _value6_ = _value5_.Date;
                                    js.Write('[');
                    {
                        System.DateTime _value7_ = _value6_.DateTime;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value7_, js);
                    }
                    js.Write(',');
                    {
                        int _value7_ = _value6_.Value;
                                    fastCSharp.web.ajax.ToString((int)_value7_, js);
                    }
                    js.Write(']');
                    }
                    js.Write(']');
                    }
                    js.Write(']');
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                    }
                    js.WriteNotNull(@"].FormatView()");
                        }
                    }
                    js.Write('}');
                    }
                    js.WriteNotNull(@"})");
                        }
                    }
                    js.Write('}');
            }

            private struct webViewQuery
            {
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                public int StudentId;
            }
            /// <summary>
            /// 查询参数
            /// </summary>
            private webViewQuery query;
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                if (base.loadView())
                {
                    
                    query= default(webViewQuery);
                    if (ParseParameter(ref query))
                    {
                        return loadView(query.StudentId);
                    }
                }
                return false;
            }
        }

}
namespace fastCSharp.demo.sqlTableWeb
{


        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<int>
        {
            
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            protected override keyValue<string[], string[]> rewrites
            {
                get
                {
                    int count = 3 + 0 * 2;
                    string[] names = new string[count];
                    string[] views = new string[count];
                    names[--count] = "/Class";
                    views[count] = "/Class.html";
                    names[--count] = "/ClassList";
                    views[count] = "/ClassList.html";
                    names[--count] = "/Student";
                    views[count] = "/Student.html";
                    return new keyValue<string[], string[]>(names, views);
                }
            }
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            protected override string[] viewRewrites
            {
                get
                {
                    string[] names = new string[3];
                    names[0] = "/Class";
                    names[1] = "/ClassList";
                    names[2] = "/Student";
                    return names;
                }
            }
            /// <summary>
            /// WEB视图页面索引集合
            /// </summary>
            protected override string[] views
            {
                get
                {
                    string[] names = new string[3];
                    names[0] = "/Class.html";
                    names[1] = "/ClassList.html";
                    names[2] = "/Student.html";
                    return names;
                }
            }
            /// <summary>
            /// 视图页面处理
            /// </summary>
            /// <param name="viewIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void request(int viewIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (viewIndex)
                {
                    case 0: load(socket, socketIdentity, fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.Class>.Pop() ?? new fastCSharp.demo.sqlTableWeb.Class(), true); return;
                    case 1: load(socket, socketIdentity, fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.ClassList>.Pop() ?? new fastCSharp.demo.sqlTableWeb.ClassList(), true); return;
                    case 2: load(socket, socketIdentity, fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.Student>.Pop() ?? new fastCSharp.demo.sqlTableWeb.Student(), true); return;
                }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new fastCSharp.demo.sqlTableWeb.webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }
}namespace fastCSharp.demo.sqlTableWeb
{

        /// <summary>
        /// AJAX函数调用
        /// </summary>
        [fastCSharp.code.cSharp.webCall(IsPool = true)]
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public sealed class ajaxLoader : fastCSharp.code.cSharp.ajax.loader<ajaxLoader>
        {
            /// <summary>
            /// AJAX函数调用集合
            /// </summary>
            private static readonly fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call> methods;
            /// <summary>
            /// AJAX调用
            /// </summary>
            [fastCSharp.code.cSharp.webCall(FullName = "/ajax")]
            public void Load()
            {
                load(methods);
            }
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="loader"></param>
            protected override void callAjax(int callIndex, fastCSharp.code.cSharp.ajax.loader loader)
            {
                switch (callIndex)
                {
                    case 0: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.Class>.Pop() ??new fastCSharp.demo.sqlTableWeb.Class(), true); return;
                    case 1: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.ClassList>.Pop() ??new fastCSharp.demo.sqlTableWeb.ClassList(), true); return;
                    case 2: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.Student>.Pop() ??new fastCSharp.demo.sqlTableWeb.Student(), true); return;
                    case 4 - 1: pubError(loader); return;
                }
            }
            static ajaxLoader()
            {
                string[] names = new string[4];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[4];
                names[0] = "/Class.html";
                callMethods[0] = new fastCSharp.code.cSharp.ajax.call(0, 4194304, 65536, true, false);
                names[1] = "/ClassList.html";
                callMethods[1] = new fastCSharp.code.cSharp.ajax.call(1, 4194304, 65536, true, false);
                names[2] = "/Student.html";
                callMethods[2] = new fastCSharp.code.cSharp.ajax.call(2, 4194304, 65536, true, false);
                names[4 - 1] = fastCSharp.code.cSharp.ajax.PubErrorCallName;
                callMethods[4 - 1] = new fastCSharp.code.cSharp.ajax.call(4 - 1, 2048, 0, false, false);
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }
}namespace fastCSharp.demo.sqlTableWeb
{

        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<int>
        {
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected override string[] calls
            {
                get
                {
                    string[] names = new string[2];
                    names[0] = "/ajax";
                    names[1] = "/";
                    return names;
                }
            }
            /// <summary>
            /// WEB调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void call(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (callIndex)
                {
                    case 0:
                        loadAjax<_c0, fastCSharp.demo.sqlTableWeb.ajaxLoader>(socket, socketIdentity, _c0/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.ajaxLoader>.Pop() ?? new fastCSharp.demo.sqlTableWeb.ajaxLoader());
                        return;
                    case 1:
                        load<_c1, fastCSharp.demo.sqlTableWeb.index>(socket, socketIdentity, _c1/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.sqlTableWeb.index>.Pop() ?? new fastCSharp.demo.sqlTableWeb.index(), 4194304, 65536, false, true);
                        return;
                }
            }
            private sealed class _c0 : fastCSharp.code.cSharp.webCall.callPool<_c0, fastCSharp.demo.sqlTableWeb.ajaxLoader>
            {
                private _c0() : base() { }
                public override bool Call()
                {
                    try
                    {
                            {
                                WebCall.Load();
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<_c0>.PushNotNull(this);
                    }
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c0 Get()
                {
                    _c0 call = fastCSharp.typePool<_c0>.Pop();
                    if (call == null) call = new _c0();
                    return call;
                }
            }
            private sealed class _c1 : fastCSharp.code.cSharp.webCall.callPool<_c1, fastCSharp.demo.sqlTableWeb.index>
            {
                private _c1() : base() { }
                public override bool Call()
                {
                    try
                    {
                            {
                                WebCall.Load();
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<_c1>.PushNotNull(this);
                    }
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c1 Get()
                {
                    _c1 call = fastCSharp.typePool<_c1>.Pop();
                    if (call == null) call = new _c1();
                    return call;
                }
            }
        }
}
#endif