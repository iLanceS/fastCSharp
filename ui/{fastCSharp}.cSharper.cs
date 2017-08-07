//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable

namespace fastCSharp.code
{
    internal partial class ajax
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _code_.Add(@"
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
            [fastCSharp.code.cSharp.webCall(FullName = """);
            _code_.Add(AjaxName);
            _code_.Add(@""")]
            public void Load()
            {
                load(methods);
            }
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name=""callIndex""></param>
            /// <param name=""loader""></param>
            protected override void callAjax(int callIndex, fastCSharp.code.cSharp.ajax.loader loader)
            {
                switch (callIndex)
                {");
                {
                    fastCSharp.code.ajax.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.methodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@":
                        {
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" view = ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.ajax _value3_ = _value2_.TypeAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.typePool<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Pop() ?? ");
            }
            _code_.Add(@"new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"();");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@" parameter = new ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@"();");
            }
            _code_.Add(@"
                            fastCSharp.net.tcp.http.response response = loader.Load(view");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref parameter");
            }
                {
                    fastCSharp.code.cSharp.ajax _value3_ = default(fastCSharp.code.cSharp.ajax);
                    _value3_ = _value2_.TypeAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsPool ? "true" : "false");
            }
                }
            _code_.Add(@");
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;");
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.AsyncIndexName);
            _code_.Add(@" callback = typePool<");
            _code_.Add(_value2_.AsyncIndexName);
            _code_.Add(@">.Pop() ?? new ");
            _code_.Add(_value2_.AsyncIndexName);
            _code_.Add(@"();");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                callback.Parameter = parameter;");
            }
            _code_.Add(@"
                                view.");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"parameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"callback.Get(view, response));");
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                try
                                {
                                    ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    parameter.");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
            _code_.Add(@" view.");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"parameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");
                                }
                                finally
                                {");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    if (responseIdentity == view.ResponseIdentity) view.AjaxResponse(ref parameter, ref response);
                                    else view.AjaxResponse(ref response);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    view.AjaxResponse(ref response);");
            }
            _code_.Add(@"
                                }");
            }
            _code_.Add(@"
                            }
                        }
                        return;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                {
                    fastCSharp.code.ajax.cSharp.viewMethodIndex[] _value1_;
                    _value1_ = ViewMethods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.viewMethodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@": loader.LoadView(");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.typePool<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Pop() ??");
            }
            _code_.Add(@"new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"()");
                {
                    fastCSharp.code.cSharp.webView _value3_ = default(fastCSharp.code.cSharp.webView);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsPool ? "true" : "false");
            }
                }
            _code_.Add(@"); return;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                if (!(bool)IsPubError)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    case ");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@" - 1: pubError(loader); return;");
            }
            _code_.Add(@"
                }
            }");
                {
                    fastCSharp.code.ajax.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@"
            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonSerialize.member]");
            }
            _if_ = false;
                if (!(bool)_value4_.IsOut)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonParse.member]");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                {
                    fastCSharp.code.memberType _value6_ = _value5_.GenericParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonSerialize.member]
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.GenericParameterType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@";");
            }
            _code_.Add(@"
            }");
            }
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            sealed class ");
            _code_.Add(_value2_.AsyncIndexName);
            _code_.Add(@" : fastCSharp.code.cSharp.ajax.callback<");
            _code_.Add(_value2_.AsyncIndexName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.GenericParameterType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            }
            _code_.Add(@">
            {");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@" Parameter;");
            }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected override void onReturnValue(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.GenericParameterType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" value)
                {
                    ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    Parameter.");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
            _code_.Add(@"value;
                    ajax.AjaxResponse(");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"ref Parameter, ");
            }
            _code_.Add(@"ref response);
                }");
            }
            _code_.Add(@"
            }");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            static ajaxLoader()
            {
                string[] names = new string[");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@"];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@"];");
                {
                    fastCSharp.code.ajax.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.methodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                names[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";
                callMethods[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = new fastCSharp.code.cSharp.ajax.call(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxPostDataSize.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxMemoryStreamSize.ToString());
            _code_.Add(@", false");
                {
                    fastCSharp.code.cSharp.ajax _value3_ = default(fastCSharp.code.cSharp.ajax);
                    _value3_ = _value2_.DefaultAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsOnlyPost ? "true" : "false");
            _if_ = false;
                if (!(bool)_value3_.IsReferer)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", false");
            }
            }
                }
            _code_.Add(@");");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                {
                    fastCSharp.code.ajax.cSharp.viewMethodIndex[] _value1_;
                    _value1_ = ViewMethods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.viewMethodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                names[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                callMethods[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = new fastCSharp.code.cSharp.ajax.call(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxPostDataSize.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxMemoryStreamSize.ToString());
            _code_.Add(@", true, false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsReferer)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", false");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsPool)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                callMethods[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = new fastCSharp.code.cSharp.ajax.call(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxPostDataSize.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxMemoryStreamSize.ToString());
            _code_.Add(@", true, false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsReferer)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", false");
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                if (!(bool)IsPubError)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                names[");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@" - 1] = fastCSharp.code.cSharp.ajax.PubErrorCallName");
            _if_ = false;
                {
                    fastCSharp.code.auto.parameter _value1_ = AutoParameter;
                    if (_value1_ != null)
                    {
                {
                    fastCSharp.code.webConfig _value2_ = _value1_.WebConfig;
                    if (_value2_ != null)
                    {
                    if (_value2_.IgnoreCase)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@".ToLower()");
            }
            _code_.Add(@";
                callMethods[");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@" - 1] = new fastCSharp.code.cSharp.ajax.call(");
            _code_.Add(MethodCount.ToString());
            _code_.Add(@" - 1, 2048, 0, false, false);");
            }
            _code_.Add(@"
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class ajax
    {
    internal partial class ts
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.TypeScript, _isOut_))
            {
                
            _code_.Add(@"module ");
            _code_.Add(Namespace);
            _code_.Add(@" {
	export class ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
            _code_.Add(_value2_.Name);
                    }
                }
                    }
                }
            _code_.Add(@" {
		");
                {
                    fastCSharp.code.ajax.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.ajax.cSharp.methodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
		static ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.Name);
                    }
                }
                    }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Callback = null) {
			fastCSharp.Pub.GetAjaxPost()('");
            _code_.Add(_value2_.CallName);
            _code_.Add(@"',");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"{");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@": ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@" }");
            }
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                if (_value3_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"null");
            }
            _code_.Add(@", Callback);	
		}
		");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
	}
}");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class dataSerialize
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@" : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {

            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name=""_serializer_"">对象序列化器</param>
            public unsafe void Serialize(fastCSharp.emit.dataSerializer _serializer_)
            {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                if (_serializer_.CheckPoint(this))");
            }
            _code_.Add(@"
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.dataSerialize _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsMemberMap)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (_serializer_.SerializeMemberMap<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">() == null)");
            }
            _code_.Add(@"
                    {");
            _if_ = false;
                    if (NullMapFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.unmanagedStream _stream_ = _serializer_.Stream;
                        _stream_.PrepLength(sizeof(int) + ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@" + ");
            _code_.Add(FixedSize.ToString());
            _code_.Add(@");
                        byte* _write_ = _stream_.CurrentData;
                        *(int*)_write_ = ");
            _code_.Add(MemberCountVerify.ToString());
            _code_.Add(@";
                        _write_ += sizeof(int);");
            _if_ = false;
                    if (NullMapSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        byte* _nullMap_ = _write_;
                        fastCSharp.unsafer.memory.Clear32(_write_, ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@");
                        _write_ += ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@";");
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsBool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue)
                        {
                            if ((bool)");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@") _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(3 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));
                            else _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));
                        }");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsNull)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@") _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsBool)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (!");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue) _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" == null) _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        else");
            }
            }
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        {
                            *(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@"*)_write_ = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@")");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsEnum)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.EnumUnderlyingType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";
                            _write_ += sizeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@");
                        }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                        _stream_.UnsafeAddSerializeLength((int)(_write_ - _stream_.CurrentData));
                        _stream_.PrepLength();");
            }
            _if_ = false;
                if (NullMapFixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        _serializer_.Stream.Write(");
            _code_.Add(MemberCountVerify.ToString());
            _code_.Add(@");");
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                if (_value2_.SerializeFixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue)");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" != null)");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsValueType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            _serializer_.MemberNullableSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        _serializer_.MemberStructSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsValueType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        _serializer_.MemberClassSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.dataSerialize _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsMemberMap)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    else
                    {");
            _if_ = false;
                    if (NullMapFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.unmanagedStream _stream_ = _serializer_.Stream;
                        _stream_.PrepLength(");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@" + ");
            _code_.Add(FixedSize.ToString());
            _code_.Add(@");
                        byte* _nullMap_ = _stream_.CurrentData");
            _if_ = false;
                    if (FixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", _write_ = _nullMap_");
            }
            _code_.Add(@";");
            _if_ = false;
                    if (NullMapSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.unsafer.memory.Clear32(_nullMap_, ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@");");
            _if_ = false;
                    if (FixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        _write_ += ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@";");
            }
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                        if (_serializer_.IsMemberMap(");
            _code_.Add(_value2_.MemberIndex.ToString());
            _code_.Add(@"))
                        {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsBool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue)
                        {
                            if ((bool)");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@") _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(3 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));
                            else _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));
                        }");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsNull)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@") _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsBool)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (!");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue) _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" == null) _nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] |= (byte)(1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7));");
            }
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        else");
            }
            }
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        {
                            *(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@"*)_write_ = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@")");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsEnum)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.EnumUnderlyingType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";
                            _write_ += sizeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@");
                        }");
            }
            }
            _code_.Add(@"
                        }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                    if (FixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        _stream_.UnsafeAddLength(((int)(_write_ - _nullMap_) + 3) & (int.MaxValue - 3));");
            }
            _if_ = false;
                if (FixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        _stream_.UnsafeAddLength(");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@");");
            }
            _code_.Add(@"
                        _stream_.PrepLength();");
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                        if (_serializer_.IsMemberMap(");
            _code_.Add(_value2_.MemberIndex.ToString());
            _code_.Add(@"))
                        {");
            _if_ = false;
                if (_value2_.SerializeFixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue)");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" != null)");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsValueType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            _serializer_.MemberNullableSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        _serializer_.MemberStructSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsValueType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        _serializer_.MemberClassSerialize(");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                        }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    }");
            }
            _code_.Add(@"
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name=""_deSerializer_"">对象反序列化器</param>
            public unsafe void DeSerialize(fastCSharp.emit.dataDeSerializer _deSerializer_)
            {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                _deSerializer_.AddPoint(this);");
            }
            _code_.Add(@"
                if (_deSerializer_.CheckMemberCount(");
            _code_.Add(MemberCountVerify.ToString());
            _code_.Add(@"))
                {");
            _if_ = false;
                    if (NullMapFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    byte* _read_ = _deSerializer_.Read;");
            _if_ = false;
                    if (NullMapSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    byte* _nullMap_ = _read_;
                    _read_ += ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@";");
            }
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsBool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) == 0) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = null;
                    else ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (2 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0);");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsNull)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0);");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsBool)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = null;
                    else");
            }
            _code_.Add(@"
                    {
                        ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsEnum)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"(*(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@"*)_read_);
                        _read_ += sizeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@");
                    }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                    if (NullMapFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);");
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                if (_value2_.SerializeFixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) == 0)");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsValueType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    {
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.NullType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" _value_ = ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue ? ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".Value : default(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.NullType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@");
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = _value_;
                        else return;
                    }");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (!_deSerializer_.MemberStructDeSerialize(ref ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")) return;");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsValueType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (!_deSerializer_.MemberClassDeSerialize(ref ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")) return;");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.dataSerialize _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsMemberMap)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                else if (_deSerializer_.GetMemberMap<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">() != null)
                {");
            _if_ = false;
                    if (NullMapSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    byte* _nullMap_ = _deSerializer_.Read;");
            }
            _if_ = false;
                    if (FixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    byte* _read_;");
            _if_ = false;
                    if (NullMapSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _read_ = _nullMap_ + ");
            _code_.Add(NullMapSize.ToString());
            _code_.Add(@";");
            }
            _if_ = false;
                if (NullMapSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _read_ = _deSerializer_.Read;");
            }
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    if (_deSerializer_.IsMemberMap(");
            _code_.Add(_value2_.MemberIndex.ToString());
            _code_.Add(@"))
                    {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsBool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) == 0) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = null;
                    else ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (2 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0);");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsNull)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0);");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsBool)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.SerializeFixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) != 0) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = null;
                    else");
            }
            _code_.Add(@"
                    {
                        ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsEnum)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"(*(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@"*)_read_);
                        _read_ += sizeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.StructNotNullType);
                    }
                }
            _code_.Add(@");
                    }");
            }
            }
            _code_.Add(@"
                    }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                    if (FixedSize != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _deSerializer_.Read = _read_ + ((int)(_deSerializer_.Read - _read_) & 3);");
            }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = Members;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    if (_deSerializer_.IsMemberMap(");
            _code_.Add(_value2_.MemberIndex.ToString());
            _code_.Add(@"))
                    {");
            _if_ = false;
                if (_value2_.SerializeFixedSize == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsNull)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if ((_nullMap_[");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" >> 3] & (1 << (");
            _code_.Add(_value2_.SerializeNullMapIndex.ToString());
            _code_.Add(@" & 7))) == 0)");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsValueType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.NullType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    {
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.NullType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" _value_ = ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".HasValue ? ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@".Value : default(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.NullType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@");
                        if (_deSerializer_.MemberStructDeSerialize(ref _value_)) ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = _value_;
                        else return;
                    }");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                if (_value3_.NullType == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (!_deSerializer_.MemberStructDeSerialize(ref ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")) return;");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsValueType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (!_deSerializer_.MemberClassDeSerialize(ref ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")) return;");
            }
            }
            _code_.Add(@"
                    }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.dataSerialize _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsMemberMap)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                else _deSerializer_.Error(fastCSharp.emit.dataDeSerializer.deSerializeState.MemberMap);");
            }
            _code_.Add(@"
            }
        }");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class memoryDatabaseModel
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@"
        {");
            _if_ = false;
                    if (IsManyPrimaryKey)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.memoryDatabaseModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsComparable)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", IComparable<primaryKey>");
            }
            _code_.Add(@"
            {");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = PrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""other"">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    return ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            _code_.Add(@"/**/.Equals(other.");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@")");
            }
                }
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@" && ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.Equals(other.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@";
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            }
                }
            _code_.Add(@".GetHashCode()");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@" ^ ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.GetHashCode()");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@";
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""obj""></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.memoryDatabaseModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsComparable)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""other""></param>
                /// <returns></returns>
                public int CompareTo(primaryKey other)
                {
                    int _value_ = ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            _code_.Add(@"/**/.CompareTo(other.");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@")");
            }
                }
            _code_.Add(@";");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    if (_value_ == 0)
                    {
                        _value_ = ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.CompareTo(other.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return _value_;
                }");
            }
            _code_.Add(@"
            }");
            }
            _code_.Add(@"
        }");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class sqlModel
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsDefaultSerialize)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
        [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
        [fastCSharp.emit.jsonParse(IsAllMember = true)]
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false");
            _if_ = false;
                if (!(bool)IsDefaultSerializeIsMemberMap)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", IsMemberMap = false");
            }
            _code_.Add(@")]");
            }
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@"
        {");
            _if_ = false;
                    if (IsManyPrimaryKey)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsComparable)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", IComparable<primaryKey>");
            }
            _code_.Add(@"
            {");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = PrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""other"">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    return ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            _code_.Add(@"/**/.Equals(other.");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@")");
            }
                }
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@" && ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.Equals(other.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@";
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            }
                }
            _code_.Add(@".GetHashCode()");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@" ^ ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.GetHashCode()");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@";
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""obj""></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsComparable)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name=""other""></param>
                /// <returns></returns>
                public int CompareTo(primaryKey other)
                {
                    int _value_ = ");
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = PrimaryKey0;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.MemberName);
            _code_.Add(@"/**/.CompareTo(other.");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@")");
            }
                }
            _code_.Add(@";");
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    if (_value_ == 0)
                    {
                        _value_ = ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@"/**/.CompareTo(other.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_;
                    _value1_ = NextPrimaryKeys;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                    }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return _value_;
                }");
            }
            _code_.Add(@"
            }");
            }
            _code_.Add(@"

            /// <summary>
            /// 数据库表格模型
            /// </summary>
            /// <typeparam name=""tableType"">表格映射类型</typeparam>");
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <typeparam name=""memberCacheType"">成员绑定缓存类型</typeparam>");
            }
            _code_.Add(@"
            public abstract class sqlModel<tableType");
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", memberCacheType");
            }
            _code_.Add(@"> : ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"
                where tableType : sqlModel<tableType");
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", memberCacheType");
            }
            _code_.Add(@">");
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                where memberCacheType : ");
            _code_.Add(MemberCacheBaseType);
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreateIdentityCounterMemberQueue")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.sql.cache.whole.memberCacheCounter<tableType, memberCacheType>");
            }
            }
            _code_.Add(@"
            {
                /// <summary>
                /// SQL表格操作工具
                /// </summary>
                protected static readonly fastCSharp.emit.sqlTable<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _if_ = false;
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_ = PrimaryKeys;
                    {
                    if (_value1_.Count != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            }
            _code_.Add(@"> ");
            _code_.Add(SqlTableName);
            _code_.Add(@" = fastCSharp.emit.sqlTable<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _if_ = false;
                {
                    fastCSharp.subArray<fastCSharp.code.memberInfo> _value1_ = PrimaryKeys;
                    {
                    if (_value1_.Count != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            }
            _code_.Add(@">.Get();
                private static bool isSqlLoaded;
                /// <summary>
                /// 等待数据初始化完成
                /// </summary>
                public static void WaitSqlLoaded()
                {
                    if (!isSqlLoaded)
                    {
                        ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.LoadWait.Wait();
                        isSqlLoaded = true;
                    }
                }");
            _if_ = false;
                    if (IsEventCacheLoaded)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                private static bool isEventCacheLoaded;");
            _if_ = false;
                    if (IsCreateEventCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly fastCSharp.threading.waitHandle eventCacheLoadWait = new fastCSharp.threading.waitHandle(false);");
            }
            _code_.Add(@"
                /// <summary>
                /// 等待数据事件缓存数据初始化完成
                /// </summary>
                public static void WaitEventCacheLoaded()
                {
                    if (!isEventCacheLoaded)
                    {");
            _if_ = false;
                    if (IsCreateEventCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        eventCacheLoadWait.Wait();");
            }
            _if_ = false;
                if (!(bool)IsCreateEventCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@" == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);");
            }
            _code_.Add(@"
                        isEventCacheLoaded = true;
                    }
                }");
            }
            _if_ = false;
                    if (IsSqlLoaded)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 数据加载完成
                /// </summary>");
            _if_ = false;
                    if (IsSqlCacheLoaded)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""onInserted"">添加记录事件</param>
                /// <param name=""onUpdated"">更新记录事件</param>
                /// <param name=""onDeleted"">删除记录事件</param>
                #region Attribute.LogTcpCallService
                /// <param name=""isMemberMap"">是否支持成员位图</param>
                #endregion Attribute.LogTcpCallService");
            }
            _code_.Add(@"
                protected static void sqlLoaded(");
            _if_ = false;
                    if (IsSqlCacheLoaded)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Action<tableType> onInserted = null, Action<");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsLoadedCache)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"tableType, ");
            }
            _code_.Add(@"tableType, tableType, fastCSharp.code.memberMap<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">> onUpdated = null, Action<tableType> onDeleted = null");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.LogTcpCallService != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", bool isMemberMap = true");
            }
            }
            _code_.Add(@")
                {");
            _if_ = false;
                    if (IsSqlCacheLoaded)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.LogTcpCallService != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(SqlStreamName);
            _code_.Add(@" = ");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@"/**/.GetLogStream(");
            _code_.Add(SqlStreamCountName);
            _code_.Add(@", isMemberMap);");
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                if (_value1_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(SqlStreamName);
            _code_.Add(@" = ");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@"/**/.GetLogStream(null, isMemberMap);");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsLoadedCache)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@"/**/.Loaded(onInserted, onUpdated, onDeleted");
            _if_ = false;
                    if (SqlStreamTypeCount != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", false");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsLoadedCache)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.Loaded(onInserted, onUpdated, onDeleted");
            _if_ = false;
                    if (SqlStreamTypeCount != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", false");
            }
            _code_.Add(@");");
            }
            }
            _if_ = false;
                    if (SqlStreamTypeCount != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.emit.sqlTable.sqlStreamCountLoaderType.Add(typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"), ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.TableNumber, sqlStreamLoad._LoadCount_");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamCountType[] _value1_;
                    _value1_ = SqlStreamCountTypes;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamCountType _value2_ in _value1_)
                        {
            _code_.Add(@", new fastCSharp.emit.sqlTable.sqlStreamCountLoaderType(typeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.SqlStreamCountType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"), ");
            _code_.Add(_value2_.SqlStreamCountTypeNumber.ToString());
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@");");
            }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                if (CacheType.ToString() == @"IdentityArray")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> ");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null);");
            }
            _if_ = false;
                if (CacheType.ToString() == @"IdentityTree")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> ");
            _code_.Add(IdentityTreeCacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null);");
            }
            _if_ = false;
                if (CacheType.ToString() == @"PrimaryKey")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(PrimaryKeyCacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null);");
            }
            _code_.Add(@"
");
            _if_ = false;
                if (CacheType.ToString() == @"CreateIdentityArray")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> ");
            _code_.Add(CreateIdentityArrayMemberCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name=""memberCacheType""></typeparam>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <param name=""isReset"">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0, bool isReset = true)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreateIdentityArrayMemberCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", memberCache, group, baseIdentity, isReset);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreateIdentityArrayMemberCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    createCounter(");
            _code_.Add(CreateIdentityArrayMemberCacheName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreateIdentityArrayMemberCacheName);
            _code_.Add(@";
                }");
            }
            _if_ = false;
                if (!(bool)IsMemberCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> ");
            _code_.Add(CreateIdentityArrayCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""isReset"">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> createCache(int group = 0, int baseIdentity = 0, bool isReset = true)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreateIdentityArrayCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.identityArray<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null, group, baseIdentity, isReset);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreateIdentityArrayCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreateIdentityArrayCacheName);
            _code_.Add(@";
                }");
            }
            }
            _if_ = false;
                if (CacheType.ToString() == @"CreateIdentityArrayWhere")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <param name=""isReset"">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, Func<tableType, bool> isValue, int group = 0, int baseIdentity = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    return new fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", memberCache, isValue, group, baseIdentity);
                }");
            }
            _if_ = false;
                if (!(bool)IsMemberCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""isValue"">数据匹配器,必须保证更新数据的匹配一致性</param>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <param name=""group"">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> createCache(Func<tableType, bool> isValue, int group = 0, int baseIdentity = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    return new fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null, isValue, group, baseIdentity);
                }");
            }
            }
            _if_ = false;
                if (CacheType.ToString() == @"CreateIdentityTree")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> ");
            _code_.Add(CreateIdentityTreeMemberCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreateIdentityTreeMemberCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", memberCache, group, baseIdentity);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreateIdentityTreeMemberCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    createCounter(");
            _code_.Add(CreateIdentityTreeMemberCacheName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreateIdentityTreeMemberCacheName);
            _code_.Add(@";
                }");
            }
            _if_ = false;
                if (!(bool)IsMemberCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> ");
            _code_.Add(CreateIdentityTreeCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""baseIdentity"">基础ID</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> createCache(int group = 0, int baseIdentity = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreateIdentityTreeCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.identityTree<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null, group, baseIdentity);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreateIdentityTreeCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreateIdentityTreeCacheName);
            _code_.Add(@";
                }");
            }
            }
            _if_ = false;
                if (CacheType.ToString() == @"CreatePrimaryKey")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(CreatePrimaryKeyMemberCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name=""memberCacheType""></typeparam>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""group"">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreatePrimaryKeyMemberCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", memberCache, group);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreatePrimaryKeyMemberCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreatePrimaryKeyMemberCacheName);
            _code_.Add(@";
                }");
            }
            _if_ = false;
                if (!(bool)IsMemberCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(CreatePrimaryKeyCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> createCache(int group = 0)
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    ");
            _code_.Add(CreatePrimaryKeyCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.primaryKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null, group);
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(");
            _code_.Add(CreatePrimaryKeyCacheName);
            _code_.Add(@"/**/.Values);");
            }
            _code_.Add(@"
                    return ");
            _code_.Add(CreatePrimaryKeyCacheName);
            _code_.Add(@";
                }");
            }
            }
            _if_ = false;
                if (CacheType.ToString() == @"CreateMemberKey")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.cache<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> ");
            _code_.Add(CreateMemberKeyMemberCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name=""targetType""></typeparam>
                /// <typeparam name=""targetModelType""></typeparam>
                /// <typeparam name=""targetMemberCacheType""></typeparam>
                /// <typeparam name=""keyType""></typeparam>
                /// <typeparam name=""memberKeyType""></typeparam>
                /// <param name=""targetCache"">目标缓存</param>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""getKey"">键值获取器</param>
                /// <param name=""getMemberKey"">成员缓存键值获取器</param>
                /// <param name=""member"">缓存成员</param>
                /// <param name=""group"">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, keyType, memberKeyType, targetMemberCacheType> createCache<targetType, targetModelType, targetMemberCacheType, keyType, memberKeyType>(fastCSharp.sql.cache.whole.events.key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType> getKey, Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberKeyType> getMemberKey, System.Linq.Expressions.Expression<Func<targetMemberCacheType, keyValue<System.Collections.Generic.Dictionary<fastCSharp.sql.randomKey<memberKeyType>, tableType>, int>>> member, int group = 0)
                    where keyType : struct, IEquatable<keyType>
                    where memberKeyType : struct, IEquatable<memberKeyType>
                    where targetType : class, targetModelType
                    where targetModelType : class
                    where targetMemberCacheType : class
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, keyType, memberKeyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, keyType, memberKeyType, targetMemberCacheType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", memberCache, getKey, getMemberKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, group);
                    ");
            _code_.Add(CreateMemberKeyMemberCacheName);
            _code_.Add(@" = cache;
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(cache.Values);");
            }
            _code_.Add(@"
                    return cache;
                }");
            }
            _if_ = false;
                if (!(bool)IsMemberCache)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.cache<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType> ");
            _code_.Add(CreateMemberKeyCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name=""targetType""></typeparam>
                /// <typeparam name=""targetModelType""></typeparam>
                /// <typeparam name=""targetMemberCacheType""></typeparam>
                /// <typeparam name=""keyType""></typeparam>
                /// <typeparam name=""memberKeyType""></typeparam>
                /// <param name=""targetCache"">目标缓存</param>
                /// <param name=""memberCache"">成员缓存</param>
                /// <param name=""getKey"">键值获取器</param>
                /// <param name=""getMemberKey"">成员缓存键值获取器</param>
                /// <param name=""member"">缓存成员</param>
                /// <param name=""group"">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, keyType, memberKeyType, targetMemberCacheType> createCache<targetType, targetModelType, targetMemberCacheType, keyType, memberKeyType>(fastCSharp.sql.cache.whole.events.key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType> getKey, Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberKeyType> getMemberKey, System.Linq.Expressions.Expression<Func<targetMemberCacheType, keyValue<System.Collections.Generic.Dictionary<fastCSharp.sql.randomKey<memberKeyType>, tableType>, int>>> member, int group = 0)
                    where keyType : struct, IEquatable<keyType>
                    where memberKeyType : struct, IEquatable<memberKeyType>
                    where targetType : class, targetModelType
                    where targetModelType : class
                    where targetMemberCacheType : class
                {
                    if (");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null) return null;
                    fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, keyType, memberKeyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.events.memberKey<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", tableType, keyType, memberKeyType, targetMemberCacheType>(");
            _code_.Add(SqlTableName);
            _code_.Add(@", null, getKey, getMemberKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, group);
                    ");
            _code_.Add(CreateMemberKeyCacheName);
            _code_.Add(@" = cache;
                    eventCacheLoadWait.Set();");
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    loadNowTime(cache.Values);");
            }
            _code_.Add(@"
                    return cache;
                }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                if (CacheCounterType.ToString() == @"IdentityCounter")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>");
            _if_ = false;
                    if (IsIdentity64)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static readonly fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(IdentityCounterCacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", 1);");
            }
            _if_ = false;
                if (!(bool)IsIdentity64)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static readonly fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(IdentityCounter32CacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", 1);");
            }
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"PrimaryKeyCounter")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(PrimaryKeyCounterCacheName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", 1);");
            }
            _code_.Add(@"
");
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreateIdentityCounterMemberQueue")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.memberIdentityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> ");
            _code_.Add(CreateIdentityCounterMemberQueueCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""cache"">自增ID整表缓存</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.part.memberQueue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType, int> createCounterMemberQueue(fastCSharp.sql.cache.whole.events.identityMemberMap<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> cache, int group = 1, int maxCount = 0)
                {
                    ");
            _code_.Add(CreateIdentityCounterMemberQueueCacheName);
            _code_.Add(@" = cache.CreateCounter(value => value.Counter, group);
                    return ");
            _code_.Add(CreateIdentityCounterMemberQueueCacheName);
            _code_.Add(@"/**/.CreateMemberQueue(value => value.NodeValue, value => value.PreviousNode, value => value.NextNode, maxCount);
                }");
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreateIdentityCounterQueue")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>");
            _if_ = false;
                    if (IsIdentity64)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(CreateIdentityCounterQueueCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", long> createIdentityCounterQueue(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", long>(");
            _code_.Add(CreateIdentityCounterQueueCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.GetByIdentity, maxCount);
                }");
            }
            _if_ = false;
                if (!(bool)IsIdentity64)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(CreateIdentityCounter32QueueCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", int> ");
            _code_.Add(CreateIdentityCounterQueueMethodName);
            _code_.Add(@"(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", int>(");
            _code_.Add(CreateIdentityCounter32QueueCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.GetByIdentity, maxCount);
                }");
            }
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreateIdentityCounterQueueList")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>");
            _if_ = false;
                    if (IsIdentity64)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(CreateIdentityCounterQueueListCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", long, keyType> createIdentityCounterQueueList<keyType>(System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", bool>>> getWhere, int group = 0, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", long, keyType>(");
            _code_.Add(CreateIdentityCounterQueueListCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.identityCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), getKey, getWhere, maxCount);
                }");
            }
            _if_ = false;
                if (!(bool)IsIdentity64)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                protected static fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(CreateIdentityCounter32QueueListCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", int, keyType> ");
            _code_.Add(CreateIdentityCounterQueueListMethodName);
            _code_.Add(@"<keyType>(System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", bool>>> getWhere, int group = 0, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", int, keyType>(");
            _code_.Add(CreateIdentityCounter32QueueCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.identityCounter32<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), getKey, getWhere, maxCount);
                }");
            }
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreatePrimaryKeyCounterQueue")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(CreatePrimaryKeyCounterQueueCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> createPrimaryKeyCounterQueue(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(CreatePrimaryKeyCounterQueueCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), ");
            _code_.Add(SqlTableName);
            _code_.Add(@"/**/.GetByPrimaryKey, maxCount);
                }");
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreatePrimaryKeyCounterQueueList")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(CreatePrimaryKeyCounterQueueListCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""getKey"">缓存关键字获取器</param>
                /// <param name=""getWhere"">条件表达式获取器</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@", keyType> createPrimaryKeyCounterQueueList<keyType>(System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", bool>>> getWhere, int group = 1, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@", keyType>(");
            _code_.Add(CreatePrimaryKeyCounterQueueListCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), getKey, getWhere, maxCount);
                }");
            }
            _if_ = false;
                if (CacheCounterType.ToString() == @"CreatePrimaryKeyCounterQueueDictionary")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@"> ");
            _code_.Add(CreatePrimaryKeyCounterQueueDictionaryCacheName);
            _code_.Add(@";
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name=""getKey"">缓存关键字获取器</param>
                /// <param name=""getWhere"">条件表达式获取器</param>
                /// <param name=""getDictionaryKey"">缓存字典关键字获取器</param>
                /// <param name=""group"">数据分组</param>
                /// <param name=""maxCount"">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueDictionary<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@", keyType, dictionaryKeyType> createPrimaryKeyCounterQueueDictionary<keyType, dictionaryKeyType>(System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", bool>>> getWhere, Func<tableType, dictionaryKeyType> getDictionaryKey, int group = 1, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                    where dictionaryKeyType : IEquatable<dictionaryKeyType>
                {
                    return new fastCSharp.sql.cache.part.queueDictionary<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@", keyType, dictionaryKeyType>(");
            _code_.Add(CreatePrimaryKeyCounterQueueDictionaryCacheName);
            _code_.Add(@" = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", ");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@">(");
            _code_.Add(SqlTableName);
            _code_.Add(@", group), getKey, getWhere, getDictionaryKey, maxCount);
                }");
            }
            _code_.Add(@"
");
            _if_ = false;
                {
                    fastCSharp.code.memberInfo[] _value1_ = IndexMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 成员索引定义
                /// </summary>
                protected static class memberIndexs
                {");
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = IndexMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
                {
                    System.Reflection.MemberInfo _value3_ = default(System.Reflection.MemberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@" (成员索引)
                    /// </summary>");
            }
            _code_.Add(@"
                    public static readonly fastCSharp.code.memberMap.memberIndex ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = fastCSharp.code.memberMap<");
                {
                    fastCSharp.code.memberType _value4_ = type;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@">.CreateMemberIndex(value => value.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@");");
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.LogTcpCallService != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 日志
                /// </summary>
                protected static fastCSharp.sql.logStream<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(SqlStreamName);
            _code_.Add(@";");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsLogClientQueue)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name=""onLog""></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = """);
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.LogTcpCallService);
                    }
                }
            _code_.Add(@""")]
                protected static void onSqlLog(Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">.data>, bool> onLog)
                {
                    ");
            _code_.Add(SqlStreamName);
            _code_.Add(@"/**/.OnLog(onLog);
                }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsLogClientQueue)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 获取日志流缓存数据
                /// </summary>
                /// <returns>日志流缓存数据</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = """);
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.LogTcpCallService);
                    }
                }
            _code_.Add(@""")]
                protected static fastCSharp.sql.logStream<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">.cacheIdentity getSqlCache()
                {
                    return ");
            _code_.Add(SqlStreamName);
            _code_.Add(@" == null ? new fastCSharp.sql.logStream<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">.cacheIdentity() : ");
            _code_.Add(SqlStreamName);
            _code_.Add(@"/**/.Cache;
                }
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name=""ticks"">时钟周期标识</param>
                /// <param name=""identity"">日志编号</param>
                /// <param name=""onLog""></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = """);
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.LogTcpCallService);
                    }
                }
            _code_.Add(@""")]
                protected static void onSqlLog(long ticks, int identity, Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">.data>, bool> onLog)
                {
                    ");
            _code_.Add(SqlStreamName);
            _code_.Add(@"/**/.OnLog(ticks, identity, onLog);
                }");
            _if_ = false;
                    if (Identity != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberInfo _value1_ = default(fastCSharp.code.memberInfo);
                    _value1_ = Identity;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name=""");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@""">");
            _code_.Add(_value1_.XmlDocument);
            _code_.Add(@"</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = """);
                {
                    fastCSharp.code.cSharp.sqlModel _value2_ = Attribute;
                    if (_value2_ != null)
                    {
            _code_.Add(_value2_.LogTcpCallService);
                    }
                }
            _code_.Add(@""")]
                protected static tableType getSqlCache(int ");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@")
                {
                    return ");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@"[");
            _code_.Add(_value1_.MemberName);
            _code_.Add(@"];
                }");
            }
                }
            }
            _if_ = false;
                if (Identity == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (CacheType.ToString() != @"CreateMemberKey")
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name=""key"">关键字</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = """);
                {
                    fastCSharp.code.cSharp.sqlModel _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.LogTcpCallService);
                    }
                }
            _code_.Add(@""")]
                protected static tableType getSqlCache(");
            _code_.Add(PrimaryKeyType);
            _code_.Add(@" key)
                {
                    return ");
            _code_.Add(PrimaryKeyCacheName);
            _code_.Add(@"[key];
                }");
            }
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 成员加载计数器
                /// </summary>
                protected static readonly fastCSharp.sql.logStream.memberCount ");
            _code_.Add(SqlStreamCountName);
            _code_.Add(@" = new fastCSharp.sql.logStream.memberCount(-1");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsSqlStreamCount)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.MemberIndex.ToString());
            }
                }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@");
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                public struct sqlStreamLoad
                {
                    /// <summary>
                    /// 数据对象
                    /// </summary>
                    internal sqlModel<tableType");
            _if_ = false;
                    if (IsMemberCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", memberCacheType");
            }
            _code_.Add(@"> _value_;");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    private static readonly fastCSharp.code.memberMap<");
                {
                    fastCSharp.code.memberType _value4_ = type;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(_value2_.MemberMapName);
            _code_.Add(@" = fastCSharp.sql.logStream.CreateMemberMap(");
            _code_.Add(SqlTableName);
            _code_.Add(@", value => value.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@");");
            _if_ = false;
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" (更新日志流)
                    /// </summary>
                    /// <param name=""value""></param>");
            }
            _code_.Add(@"
                    public void ");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MemberType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@" value)
                    {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MemberType;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsIEquatable)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (!value.Equals(_value_.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"))");
            }
            _code_.Add(@"
                        {
                            _value_.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@" = value;
                            ");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"();
                        }
                    }");
            _if_ = false;
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" (更新日志流)
                    /// </summary>");
            }
            _code_.Add(@"
                    public void ");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"()
                    {
                        if (");
            _code_.Add(SqlStreamName);
            _code_.Add(@" != null) ");
            _code_.Add(SqlStreamName);
            _code_.Add(@"/**/.Update((tableType)_value_, ");
            _code_.Add(_value2_.MemberMapName);
            _code_.Add(@");
                    }");
            _if_ = false;
                    if (_value2_.IsSqlStreamCount)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.SqlStreamCountType == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" 计算初始化完毕
                    /// </summary>
                    public static void ");
            _code_.Add(_value2_.MemberLoadedMethodName);
            _code_.Add(@"() { ");
            _code_.Add(SqlStreamCountName);
            _code_.Add(@"/**/.Load(");
            _code_.Add(_value3_.MemberIndex.ToString());
            _code_.Add(@"); }");
            }
            }
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                    if (SqlStreamTypeCount != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// 根据日志流计数完成类型初始化完毕
                    /// </summary>
                    /// <param name=""type""></param>
                    internal static void _LoadCount_(fastCSharp.emit.sqlTable.sqlStreamCountLoaderType type)
                    {");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = SqlStreamMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.SqlStreamCountType != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (type.Equals(typeof(");
                {
                    fastCSharp.code.memberType _value4_ = _value2_.SqlStreamCountType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@"), ");
            _code_.Add(_value2_.SqlStreamCountTypeNumber.ToString());
            _code_.Add(@")) ");
            _code_.Add(SqlStreamCountName);
            _code_.Add(@"/**/.Load(");
            _code_.Add(_value3_.MemberIndex.ToString());
            _code_.Add(@");");
            }
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    }");
            }
            _code_.Add(@"
                }
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                [fastCSharp.code.ignore]
                public sqlStreamLoad ");
            _code_.Add(SqlStreamLoadName);
            _code_.Add(@"
                {
                    get { return new sqlStreamLoad { _value_ = this }; }
                }");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_;
                    _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.nowTimeMember _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberInfo _value3_ = _value2_.Member;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.memberInfo _value3_ = _value2_.Member;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@" 当前时间
                /// </summary>");
            }
            _code_.Add(@"
                protected static readonly fastCSharp.sql.nowTime ");
            _code_.Add(_value2_.NowTimeMemberName);
            _code_.Add(@" = ");
            _code_.Add(SqlTableName);
            _code_.Add(@" == null ? null : new fastCSharp.sql.nowTime();");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                /// <summary>
                /// 初始化当前时间
                /// </summary>
                /// <param name=""values"">缓存数据</param>
                protected static void loadNowTime(System.Collections.Generic.IEnumerable<tableType> values)
                {
                    foreach (tableType value in values)
                    {");
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_;
                    _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.nowTimeMember _value2_ in _value1_)
                        {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.NowTimeMemberName);
            _code_.Add(@"/**/.SetMaxTime(");
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"value.");
            _code_.Add(_value3_.MemberName);
            }
                }
            _code_.Add(@");");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    }");
                {
                    fastCSharp.code.sqlModel.cSharp.nowTimeMember[] _value1_;
                    _value1_ = NowTimeMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.nowTimeMember _value2_ in _value1_)
                        {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.NowTimeMemberName);
            _code_.Add(@"/**/.SetMaxTime();");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            _if_ = false;
                    if (IsLoadNowTimeCache)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                static sqlModel()
                {
                    if (");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@" != null) loadNowTime(");
            _code_.Add(IdentityArrayCacheName);
            _code_.Add(@"/**/.Values);
                }");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 计数成员
                /// </summary>
                protected static class sqlCounter
                {");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    /// <summary>
                    /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" 当前时间
                    /// </summary>");
            }
            _code_.Add(@"
                    public static fastCSharp.sql.cache.whole.events.counterMember ");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@" = fastCSharp.sql.cache.whole.events.counterMember.Null;");
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.emit.dataMember _value4_ = _value2_.Attribute;
                    if (_value4_ != null)
                    {
                    if (_value4_.CounterReadServiceName != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 获取 ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.MemberName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                }
            _code_.Add(@"
                /// <returns>");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@"</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = """);
                {
                    fastCSharp.emit.dataMember _value4_ = _value2_.Attribute;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.CounterReadServiceName);
                    }
                }
            _code_.Add(@""")]
                protected static int ");
            _code_.Add(_value2_.GetCountMethodName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value5_ = _value4_.MemberType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.MemberName);
            }
                }
            _code_.Add(@")
                {
                    return sqlCounter.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"/**/.Get(");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.MemberName);
            }
                }
            _code_.Add(@");
                }");
            _if_ = false;
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" 总计数
                /// </summary>");
            }
            _code_.Add(@"
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = """);
                {
                    fastCSharp.emit.dataMember _value4_ = _value2_.Attribute;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.CounterReadServiceName);
                    }
                }
            _code_.Add(@""")]
                protected static long ");
            _code_.Add(_value2_.TotalCountMemberName);
            _code_.Add(@"
                {
                    get { return sqlCounter.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"/**/.TotalCount; }
                }");
            }
            _if_ = false;
                {
                    fastCSharp.emit.dataMember _value4_ = _value2_.Attribute;
                    if (_value4_ != null)
                    {
                    if (_value4_.CounterWriteServiceName != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value3_.XmlDocument);
            _code_.Add(@" 增加计数
                /// </summary>");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.MemberName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                }
            _code_.Add(@"
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = """);
                {
                    fastCSharp.emit.dataMember _value4_ = _value2_.Attribute;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.CounterWriteServiceName);
                    }
                }
            _code_.Add(@""")]
                protected static void ");
            _code_.Add(_value2_.IncCountMethodName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value5_ = _value4_.MemberType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.MemberName);
            }
                }
            _code_.Add(@")
                {
                    sqlCounter.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@"/**/.Inc(");
                {
                    fastCSharp.code.memberInfo _value4_ = default(fastCSharp.code.memberInfo);
                    _value4_ = Identity;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.MemberName);
            }
                }
            _code_.Add(@");
                }");
            }
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                /// <summary>
                /// 初始化计数成员
                /// </summary>
                /// <param name=""cache""></param>
                protected static void createCounter(fastCSharp.sql.cache.whole.events.identityCache<tableType, ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType> cache)
                {");
                {
                    fastCSharp.code.sqlModel.cSharp.sqlStreamMember[] _value1_;
                    _value1_ = CounterMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.sqlModel.cSharp.sqlStreamMember _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.memberInfo _value3_ = default(fastCSharp.code.memberInfo);
                    _value3_ = _value2_.Member;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    sqlCounter.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@" = new fastCSharp.sql.cache.whole.events.counterMember<tableType, ");
                {
                    fastCSharp.code.memberType _value4_ = type;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@", memberCacheType>(cache, value => value.");
            _code_.Add(_value3_.MemberName);
            _code_.Add(@", ");
                {
                    fastCSharp.emit.dataMember _value4_ = default(fastCSharp.emit.dataMember);
                    _value4_ = _value2_.Attribute;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.CounterTimeout.ToString());
            }
                }
            _code_.Add(@");");
            }
                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
                {
                    fastCSharp.subArray<fastCSharp.code.sqlModel.cSharp.webPathType> _value1_;
                    _value1_ = WebPaths;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.code.sqlModel.cSharp.webPathType _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                [fastCSharp.code.ignore]
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.PathMemberName);
            _code_.Add(@"
                {
                    get { return new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" { ");
                {
                    fastCSharp.code.memberInfo[] _value3_;
                    _value3_ = _value2_.Members;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.memberInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.MemberName);
            _code_.Add(@" = ");
            _code_.Add(_value4_.MemberName);
            _if_ = false;
                    if (_value4_.MemberIndex != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@" }; }
                }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }
        }");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class tcpCall
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _if_ = false;
                if (!(bool)IsAllType)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            stringBuilder _PART_SERVERCALL_ = _code_;
            _code_ = new stringBuilder();
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@"
        {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
                    if (_value2_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.code.ignore]
            [fastCSharp.code.cSharp.tcpCall(IsGenericTypeServerMethod = true)]");
            }
            _code_.Add(@"
            internal static partial class ");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"
            {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value4_ = _value3_.ReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.Parameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@")
                {");
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsGetMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
                {
                    fastCSharp.code.memberType _value3_ = type;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.StaticPropertyName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.Parameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsGetMember)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    return ");
                {
                    fastCSharp.code.memberType _value3_ = type;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.StaticPropertyName);
            _code_.Add(@";");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _if_ = false;
                    if (_value3_.IsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return ");
            }
                {
                    fastCSharp.code.memberType _value4_ = type;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.StaticMethodGenericName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.Parameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(_value5_.ParameterJoinRefName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@");");
            }
                }
            }
            _code_.Add(@"
                }");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public static readonly System.Reflection.MethodInfo ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@";");
            }
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
                if (!(bool)_value2_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsAnyGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                static ");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"()
                {
                    System.Collections.Generic.Dictionary<fastCSharp.code.cSharp.tcpBase.genericMethod, System.Reflection.MethodInfo> genericMethods = fastCSharp.code.cSharp.tcpCall.GetGenericMethods(typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"));");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"genericMethods[new fastCSharp.code.cSharp.tcpBase.genericMethod(""");
            _code_.Add(_value3_.MethodName);
            _code_.Add(@""", ");
                {
                    fastCSharp.code.memberType[] _value4_ = _value3_.GenericParameters;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.Length.ToString());
                    }
                }
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.Parameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(@", """);
            _code_.Add(_value5_.ParameterRef);
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@"""");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@")]");
            }
                }
            _code_.Add(@";");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
            }
            _code_.Add(@"
            }
        }");
            _partCodes_["SERVERCALL"] = _code_.ToString();
            _code_ = _PART_SERVERCALL_;
            _code_.Add(_partCodes_["SERVERCALL"]);
            stringBuilder _PART_CLIENTCALL_ = _code_;
            _code_ = new stringBuilder();
            _code_.Add(@"
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                    if (_value1_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.XmlDocument);
                    }
                }
            _code_.Add(@"
            /// </summary>");
            }
            _code_.Add(@"
            public ");
            _code_.Add(NoAccessTypeNameDefinition);
            _if_ = false;
                    if (IsTimeVerify)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod");
            }
            _code_.Add(@"
            {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsTimeVerify)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// TCP调用验证客户端
                /// </summary>
                /// <returns></returns>
                public bool Verify()
                {
                    return fastCSharp.net.tcp.timeVerifyServer.tcpCall<");
            _code_.Add(TcpTimeVerifyMethodType);
            _code_.Add(@">.Verify(");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ClientPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.Default, ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@");
                }");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _code_.Add(@" = new fastCSharp.net.tcp.commandClient.identityCommand { Command = ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@", MaxInputSize = ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            _code_.Add(@", IsKeepCallback = ");
            _code_.Add(_value2_.IsKeepCallback.ToString());
            _code_.Add(@", IsSendOnly = ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly fastCSharp.net.tcp.commandClient.dataCommand ");
            _code_.Add(_value2_.MethodDataCommand);
            _code_.Add(@" = new fastCSharp.net.tcp.commandClient.dataCommand { Command = fastCSharp.net.tcp.commandServer.GetMethodKeyNameCommand(""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyFullName);
                    }
                }
            _code_.Add(@"""), MaxInputSize = ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            _code_.Add(@", IsKeepCallback = ");
            _code_.Add(_value2_.IsKeepCallback.ToString());
            _code_.Add(@", IsSendOnly = ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@" };");
            }
            _code_.Add(@"
");
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsClientSynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</returns>");
            }
            _code_.Add(@"
                public static fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@")
                {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ != null)
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"

                            ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" _outputParameterValue_ = _outputParameter_.Value;");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value5_ = _value2_.Method;
                    if (_value5_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value6_ = _value5_.Method;
                    if (_value6_ != null)
                    {
                    if (_value6_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameterValue_.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")_outputParameterValue_.Return;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameterValue_.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                    return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = _returnType_ };
                }");
            }
            _if_ = false;
                    if (_value2_.IsClientAsynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""_onReturn_"">");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</param>");
            }
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>保持异步回调</returns>");
            }
            _code_.Add(@"
                public static ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onReturn_(new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.VersionExpired });");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.callback<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _onOutput_;");
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturnGeneric<");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@", ");
            }
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterGenericTypeName);
            _code_.Add(@">.Get(_onReturn_);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@", ");
            }
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">.Get(_onReturn_);");
            }
            _code_.Add(@"
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        return ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _onOutput_");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");
                    }");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturn_, null");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                public static fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" ");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@"
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ != null)
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"

                            ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" _outputParameterValue_ = _outputParameter_.Value;");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value5_ = _value2_.Method;
                    if (_value5_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value6_ = _value5_.Method;
                    if (_value6_ != null)
                    {
                    if (_value6_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameterValue_.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")_outputParameterValue_.Return;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameterValue_.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                        return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = _returnType_ };
                    }");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value3_ = default(fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex);
                    _value3_ = _value2_.SetMethod;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value4_ = default(fastCSharp.code.auto.parameter);
                    _value4_ = AutoParameter;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value4_ = default(fastCSharp.code.auto.parameter);
                    _value4_ = AutoParameter;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ != null)
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value4_ = default(fastCSharp.code.methodInfo);
                    _value4_ = _value3_.Method;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value4_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(_value5_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value4_ = default(fastCSharp.code.auto.parameter);
                    _value4_ = AutoParameter;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"

                            ");
                {
                    fastCSharp.code.auto.parameter _value4_ = default(fastCSharp.code.auto.parameter);
                    _value4_ = AutoParameter;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@" _outputParameterValue_ = _outputParameter_.Value;");
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value6_ = _value3_.Method;
                    if (_value6_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value7_ = _value6_.Method;
                    if (_value7_ != null)
                    {
                    if (_value7_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameterValue_.");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _if_ = false;
                    if (_value3_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@")_outputParameterValue_.Return;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameterValue_.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value3_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value3_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value4_ = default(fastCSharp.code.auto.parameter);
                    _value4_ = AutoParameter;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value4_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;");
            }
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            }
            _code_.Add(@"
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }");
            }
                }
            _code_.Add(@"
                }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                private static ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnType_ = new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"();");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ClientPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = ");
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"_client_.StreamSocket");
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"_client_.VerifyStreamSocket");
            }
            _code_.Add(@";
                        if (_socket_ != null)
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" _inputParameter_ = new ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
                            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@" = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0");
                {
                    fastCSharp.code.memberType[] _value3_ = default(fastCSharp.code.memberType[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.GenericParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.memberType _value4_ in _value3_)
                        {
            _code_.Add(@", typeof(");
            _code_.Add(_value4_.FullName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"),");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@" = typeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"),");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                if (!(bool)_value4_.IsOut)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"_client_.GetTcpStream(");
            }
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@",");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                            };");
            }
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRef)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            _returnType_.Value.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = _inputParameter_.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.ReturnInputParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            _returnType_.Value.Ret = _inputParameter_.");
            _code_.Add(_value2_.ReturnInputParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Get");
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", ref _returnType_.Value, _isTask_);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Get");
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodDataCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", ref _returnType_.Value, _isTask_);");
            }
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return;");
            }
            _code_.Add(@"
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""_onReturn_"">");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</param>");
            }
            _code_.Add(@"
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static void ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue> _onReturn_)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onReturn_(new fastCSharp.net.returnValue { Type = fastCSharp.net.returnValue.type.VersionExpired });");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturn_, null");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");");
            }
            _code_.Add(@"
                }");
            }
            _code_.Add(@"
                private static ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ClientPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = ");
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"_client_.StreamSocket");
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"_client_.VerifyStreamSocket");
            }
            _code_.Add(@";
                        if (_socket_ != null)
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" _inputParameter_ = new ");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
                            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@" = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0");
                {
                    fastCSharp.code.memberType[] _value3_ = default(fastCSharp.code.memberType[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.GenericParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.memberType _value4_ in _value3_)
                        {
            _code_.Add(@", typeof(");
            _code_.Add(_value4_.FullName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"),");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@" = typeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"),");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                if (!(bool)_value4_.IsOut)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"_client_.GetTcpStream(");
            }
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@",");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                            };");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Call");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", _isTask_);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Call");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"<");
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodDataCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", _isTask_);");
            }
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return;");
            }
            _code_.Add(@"
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            _code_.Add(@"
                }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }
        }");
            _partCodes_["CLIENTCALL"] = _code_.ToString();
            _code_ = _PART_CLIENTCALL_;
            _code_.Add(_partCodes_["CLIENTCALL"]);
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (IsAllType)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            stringBuilder _PART_SERVER_ = _code_;
            _code_ = new stringBuilder();
            _code_.Add(@"
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        public partial class ");
            _code_.Add(ServiceName);
            _code_.Add(@" : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            /// <param name=""attribute"">TCP调用服务器端配置信息</param>
            /// <param name=""verify"">TCP验证实例</param>
            public ");
            _code_.Add(ServiceName);
            _code_.Add(@"(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null");
            }
            _code_.Add(@")
                : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig(""");
            _code_.Add(ServiceName);
            _code_.Add(@"""");
            _if_ = false;
                    if (TcpServerAttributeType != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", typeof(");
            _code_.Add(TcpServerAttributeType);
            _code_.Add(@")");
            }
            _code_.Add(@")");
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verify");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@")
            {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                setCommands(");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@");");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                identityOnCommands[");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"verifyCommandIdentity = ");
            }
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@"].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                int commandIndex;
                keyValue<byte[][], command[]> onCommands = getCommands(");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@", out commandIndex);");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsNullMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                onCommands.Key[commandIndex] = formatMethodKeyName(""-");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@""");
                onCommands.Value[commandIndex++] = default(command);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                onCommands.Key[commandIndex] = ");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"verifyCommand = ");
            }
            _code_.Add(@"formatMethodKeyName(""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyFullName);
                    }
                }
            _code_.Add(@""");
                onCommands.Value[commandIndex++].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                maxCommandLength = ");
            _code_.Add(MaxCommandLength.ToString());
            _code_.Add(@";
                this.onCommands = new fastCSharp.stateSearcher.ascii<command>(onCommands.Key, onCommands.Value);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                int httpCount = ");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@";
                string[] httpNames = new string[httpCount];
                httpCommand[] httpCommands = new httpCommand[httpCount];");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsNullMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                httpNames[--httpCount] = ""-");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@""";
                httpCommands[httpCount] = default(httpCommand);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                ");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                verifyCommand = ");
            }
            _code_.Add(@"fastCSharp.String.getBytes(httpNames[--httpCount] = """);
            _code_.Add(_value2_.HttpMethodName);
            _code_.Add(@""");
                httpCommands[httpCount].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsHttpPostOnly ? "true" : "false");
            }
                }
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                maxCommandLength = ");
            _code_.Add(MaxCommandLength.ToString());
            _code_.Add(@";
                this.httpCommands = new fastCSharp.stateSearcher.ascii<httpCommand>(httpNames, httpCommands);");
            }
            _code_.Add(@"
            }
            /// <summary>
            /// 命令处理
            /// </summary>
            /// <param name=""index""></param>
            /// <param name=""socket""></param>
            /// <param name=""data""></param>
            protected override void doCommand(int index, socket socket, ref subArray<byte> data)
            {
                if (index < ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@") base.doCommand(index, socket, ref data);
                else
                {
                    switch (index - ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@")
                    {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@": ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket, ref data); return;");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                        default: return;
                    }
                }
            }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// 命令处理
            /// </summary>
            /// <param name=""index""></param>
            /// <param name=""socket""></param>
            protected override void doHttpCommand(int index, fastCSharp.net.tcp.http.socketBase socket)
            {
                switch (index - ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@")
                {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@": ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket); return;");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    default: return;
                }
            }");
            }
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodGroup[] _value1_;
                    _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodGroup _value2_ in _value1_)
                        {
            _code_.Add(@"
            private int ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@";
            private int ");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            /// <summary>
            /// 忽略分组
            /// </summary>
            /// <param name=""groupId"">分组标识</param>
            protected override void ignoreGroup(int groupId)
            {");
            _if_ = false;
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodGroup[] _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                switch (groupId)
                {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodGroup[] _value1_;
                    _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodGroup _value2_ in _value1_)
                        {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.GroupId.ToString());
            _code_.Add(@":
                        ");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" = 1;
                        while (");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@" != 0) System.Threading.Thread.Sleep(1);
                        break;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
            _code_.Add(@"
            }
");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsInputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsInputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
            internal ");
            _code_.Add(_value2_.InputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
            {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType ");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType[] ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@";");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@";");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.cSharp.tcpBase.tcpStream ");
            _code_.Add(_value4_.StreamParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                if (!(bool)_value5_.IsStream)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
            }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
#if NOJIT
            internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter");
            }
            }
            _code_.Add(@"
#else
            internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            }
            _code_.Add(@"
#endif
            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Ret;
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")value; }
                }
#endif");
            }
            }
            _code_.Add(@"
            }");
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsMethodServerCall)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            sealed class ");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@" : fastCSharp.net.tcp.commandServer.socketCall<");
            _code_.Add(_value2_.MethodStreamName);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterTypeName);
            }
            _code_.Add(@">
            {
                private void get(ref fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" value)
                {
                    try
                    {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"};");
            }
            }
            _code_.Add(@"
                        ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@";");
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsGetMember)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"();");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsGetMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@", """);
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"""");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            }
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")invokeParameter[");
            _code_.Add(_value4_.ParameterIndex.ToString());
            _code_.Add(@"];");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@") socket.SetVerifyMethod();");
            }
            _code_.Add(@"
                        value.Value = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"
                        {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            _code_.Add(_value2_.ReturnName);
            }
            _code_.Add(@"
                        };");
            }
            _code_.Add(@"
                        value.Type = fastCSharp.net.returnValue.type.Success;
                    }
                    catch (Exception error)
                    {
                        value.Type = fastCSharp.net.returnValue.type.ServerException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                }
                public override void Call()
                {
                    fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" value = new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"();");
            _if_ = false;
                    if (_value2_.IsClientSendOnly != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (isVerify == 0) get(ref value);");
            }
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (isVerify == 0)
                    {
                        get(ref value);
                        socket.SendStream(ref identity, ref value, flags);
                    }");
            }
            _code_.Add(@"
                    fastCSharp.typePool<");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@">.PushNotNull(this);
                }
            }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            struct ");
            _code_.Add(_value2_.MethodMergeName);
            _code_.Add(@"
            {
                public socket Socket;");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" InputParameter;");
            }
            _code_.Add(@"
                public fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" Get()
                {
                    fastCSharp.net.returnValue.type returnType;
                    try
                    {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"};");
            }
            }
            _code_.Add(@"
                        ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref InputParameter.");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@", """);
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"""");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", InputParameter.");
            _code_.Add(GenericParameterTypeName);
            }
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", InputParameter.");
            _code_.Add(GenericParameterTypeName);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")invokeParameter[");
            _code_.Add(_value4_.ParameterIndex.ToString());
            _code_.Add(@"];");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@") Socket.SetVerifyMethod();");
            }
            _code_.Add(@"
                        return new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"
                        {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            _code_.Add(_value2_.ReturnName);
            }
            _code_.Add(@"
                        };");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                    }
                    catch (Exception error)
                    {
                        returnType = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(error, null, true);
                    }
                    return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = returnType };
                }
            }");
            }
            }
            _code_.Add(@"
            private void ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket socket, ref subArray<byte> data)
            {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                socket.SendStream(socket.Identity, fastCSharp.net.returnValue.type.VersionExpired);");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                if (");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" == 0)
                {
                    System.Threading.Interlocked.Increment(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                    try
                    {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" inputParameter = new ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"();
                        if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))");
            }
            _code_.Add(@"
                        {");
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" outputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                            Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            }
            _code_.Add(@">.Get");
            _code_.Add(_value2_.KeepCallback);
            _code_.Add(@"(socket, ref outputParameter, ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@");
                            if (callbackReturn != null)
                            {");
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool>)");
            }
            _code_.Add(@"callbackReturn");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@", """);
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"""");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            }
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"callbackReturn);");
            }
            }
            _code_.Add(@"
                            }");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            Func<fastCSharp.net.returnValue, bool> callback");
            _if_ = false;
                    if (_value2_.IsClientSendOnly != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" = null");
            }
            _code_.Add(@";");
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            if ((callback = fastCSharp.net.tcp.commandServer.socket.callback.Get");
            _code_.Add(_value2_.KeepCallback);
            _code_.Add(@"(socket, ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@")) != null)");
            }
            _code_.Add(@"
                            {");
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue, bool>)");
            }
            _code_.Add(@"callback");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@", """);
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"""");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            }
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"callback);");
            }
            }
            _code_.Add(@"
                            }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsMethodServerCall)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsServerAsynchronousTask)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            fastCSharp.threading.task.Tiny.Add(");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@"/**/.GetCall(socket");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref inputParameter");
            }
            _code_.Add(@"));");
            }
            _if_ = false;
                if (!(bool)_value2_.IsServerAsynchronousTask)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@"/**/.Call(socket");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref inputParameter");
            }
            _code_.Add(@");");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsMethodServerCall)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@", ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                            return;
                        }");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;");
            }
            }
            _code_.Add(@"
                    }
                    catch (Exception error)
                    {");
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        returnType = fastCSharp.net.returnValue.type.ServerException;");
            }
            _code_.Add(@"
                        fastCSharp.log.Error.Add(error, null, true);
                    }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    finally
                    {
                        System.Threading.Interlocked.Decrement(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");
                    }
                }");
            }
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                socket.SendStream(socket.Identity, returnType);");
            }
            }
            _code_.Add(@"
            }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            private void ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(fastCSharp.net.tcp.http.socketBase socket)
            {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                socket.ResponseError(socket.TcpCommandSocket.HttpPage.SocketIdentity, fastCSharp.net.tcp.http.response.state.NotFound404);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                long identity = int.MinValue;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                if (");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" == 0)
                {
                    System.Threading.Interlocked.Increment(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                    try
                    {
                        socket commandSocket = socket.TcpCommandSocket;
                        fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                        identity = httpPage.SocketIdentity;");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" inputParameter = new ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"();
                        if (httpPage.DeSerialize(ref inputParameter))");
            }
            _code_.Add(@"
                        {");
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            object[] invokeParameter;");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" ouputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                            invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool>)");
            }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.IsVerifyMethod ? "true" : "false");
            }
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Get(httpPage, ref ouputParameter)");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue, bool>)");
            }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage)");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@", """);
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"""");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            }
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericType)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" outputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.IsVerifyMethod ? "true" : "false");
            }
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Get(httpPage, ref outputParameter));");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"/**/.");
            _code_.Add(GenericTypeServerName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.MethodIndexGenericName);
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage));");
            }
            }
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            httpPage.Response(new ");
            _code_.Add(_value2_.MethodMergeName);
            _code_.Add(@" { Socket = commandSocket");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", InputParameter = inputParameter");
            }
            _code_.Add(@" }.Get());");
            }
            _code_.Add(@"
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, true);
                    }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    finally
                    {
                        System.Threading.Interlocked.Decrement(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");
                    }
                }");
            }
            _code_.Add(@"
                socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);");
            }
            _code_.Add(@"
            }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
        }");
            _partCodes_["SERVER"] = _code_.ToString();
            _code_ = _PART_SERVER_;
            _code_.Add(_partCodes_["SERVER"]);
            stringBuilder _PART_CLIENT_ = _code_;
            _code_ = new stringBuilder();
            _code_.Add(@"
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public class ");
            _code_.Add(ServiceName);
            _code_.Add(@"
        {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsSegmentation)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsInputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsInputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
            internal ");
            _code_.Add(_value2_.InputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
            {");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodType;
                    if (_value3_ != null)
                    {
                {
                    System.Type _value4_ = _value3_.Type;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericType)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType ");
            _code_.Add(TypeGenericParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType[] ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@";");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.remoteType ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@";");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.code.cSharp.tcpBase.tcpStream ");
            _code_.Add(_value4_.StreamParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                if (!(bool)_value5_.IsStream)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
            }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
#if NOJIT
            internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter");
            }
            }
            _code_.Add(@"
#else
            internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            }
            _code_.Add(@"
#endif
            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Ret;
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")value; }
                }
#endif");
            }
            }
            _code_.Add(@"
            }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            }
            _code_.Add(@"
            /// <summary>
            /// 默认TCP调用服务器端配置信息
            /// </summary>
            protected internal static readonly fastCSharp.code.cSharp.tcpServer defaultTcpServer;
            /// <summary>
            /// 默认客户端TCP调用
            /// </summary>
            public static readonly fastCSharp.net.tcp.commandClient Default;
            static ");
            _code_.Add(ServiceName);
            _code_.Add(@"()
            {
                defaultTcpServer = fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig(""");
            _code_.Add(ServiceName);
            _code_.Add(@"""");
            _if_ = false;
                    if (TcpServerAttributeType != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", typeof(");
            _code_.Add(TcpServerAttributeType);
            _code_.Add(@")");
            }
            _code_.Add(@");");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsSegmentation)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                defaultTcpServer.IsServer = false;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsSegmentation)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                if (defaultTcpServer.IsServer) fastCSharp.log.Error.Add(""请确认 ");
            _code_.Add(ServiceName);
            _code_.Add(@" 服务器端是否本地调用"", null, false);");
            }
            _code_.Add(@"
                Default = new fastCSharp.net.tcp.commandClient(defaultTcpServer, ");
            _code_.Add(MaxCommandLength.ToString());
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", new ");
            _code_.Add(TcpVerifyMethodType);
            _code_.Add(@"()");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@");
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(");
            _code_.Add(ServiceName);
            _code_.Add(@"));
            }
            /// <summary>
            /// 忽略TCP分组
            /// </summary>
            /// <param name=""groupId"">分组标识</param>
            /// <returns>是否调用成功</returns>
            public static fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
            {
                fastCSharp.net.tcp.commandClient client = Default;
                return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
            }
        }");
            _partCodes_["CLIENT"] = _code_.ToString();
            _code_ = _PART_CLIENT_;
            _code_.Add(_partCodes_["CLIENT"]);
            stringBuilder _PART_REMEMBER_ = _code_;
            _code_ = new stringBuilder();
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsRememberIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
        /// <summary>
        /// TCP服务
        /// </summary>
        public partial class ");
            _code_.Add(ServiceName);
            _code_.Add(@"
        {
            /// <summary>
            /// 命令序号记忆数据
            /// </summary>
            private static fastCSharp.keyValue<string, int>[] ");
            _code_.Add(RememberIdentityCommeandName);
            _code_.Add(@"()
            {
                fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@"];");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                names[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"].Set(@""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyFullName);
                    }
                }
            _code_.Add(@""", ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                return names;
            }
        }");
            }
            }
            _partCodes_["REMEMBER"] = _code_.ToString();
            _code_ = _PART_REMEMBER_;
            _code_.Add(_partCodes_["REMEMBER"]);
            }
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class tcpServer
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _if_ = false;
                    if (IsSetCommandServer)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (IsServerCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : fastCSharp.code.cSharp.tcpServer.ICommandServer");
            }
            }
            _code_.Add(@"
        {
");
            _if_ = false;
                if (!(bool)IsRemember)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// ");
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.ServiceName);
                    }
                }
            _code_.Add(@" TCP服务");
            _if_ = false;
                if (!(bool)IsServerCode)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"参数");
            }
            _code_.Add(@"
            /// </summary>
            public sealed class tcpServer");
            _if_ = false;
                    if (IsServerCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : fastCSharp.net.tcp.commandServer");
            }
            _code_.Add(@"
            {");
            _if_ = false;
                    if (IsServerCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                private readonly ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@" _value_;
                /// <summary>
                /// ");
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.ServiceName);
                    }
                }
            _code_.Add(@" TCP调用服务端
                /// </summary>
                /// <param name=""attribute"">TCP调用服务器端配置信息</param>");
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""verify"">TCP验证实例</param>");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
                    if (_value2_.IsPublic)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""value"">TCP服务目标对象</param>");
            }
            _code_.Add(@"
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
                    if (_value2_.IsPublic)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@" value = null");
            }
            _code_.Add(@")
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig(""");
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.ServiceName);
                    }
                }
            _code_.Add(@""", typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"))");
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verify");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@")
                {
                    _value_ =");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
                    if (_value2_.IsPublic)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" value ?? ");
            }
            _code_.Add(@"new ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"();");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    setCommands(");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@");");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    identityOnCommands[");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"verifyCommandIdentity = ");
            }
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@"].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                if (!(bool)_value1_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    int commandIndex;
                    keyValue<byte[][], command[]> onCommands = getCommands(");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@", out commandIndex);");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsNullMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    onCommands.Key[commandIndex] = formatMethodKeyName(""-");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@""");
                    onCommands.Value[commandIndex++] = default(command);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    onCommands.Key[commandIndex] = ");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"verifyCommand = ");
            }
            _code_.Add(@"formatMethodKeyName(""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyName);
                    }
                }
            _code_.Add(@""");
                    onCommands.Value[commandIndex++].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    maxCommandLength = ");
            _code_.Add(MaxCommandLength.ToString());
            _code_.Add(@";
                    this.onCommands = new fastCSharp.stateSearcher.ascii<command>(onCommands.Key, onCommands.Value);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    int httpCount = ");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@";
                    string[] httpNames = new string[httpCount];
                    httpCommand[] httpCommands = new httpCommand[httpCount];");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsNullMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    httpNames[--httpCount] = ""-");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@""";
                    httpCommands[httpCount] = default(httpCommand);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    verifyCommand = ");
            }
            _code_.Add(@"fastCSharp.String.getBytes(httpNames[--httpCount] = """);
            _code_.Add(_value2_.HttpMethodName);
            _code_.Add(@""");
                    httpCommands[httpCount].Set(");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsHttpPostOnly ? "true" : "false");
            }
                }
            _if_ = false;
                    if (_value2_.IsInputParameterMaxLength)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    maxCommandLength = ");
            _code_.Add(MaxCommandLength.ToString());
            _code_.Add(@";
                    this.httpCommands = new fastCSharp.stateSearcher.ascii<httpCommand>(httpNames, httpCommands);");
            }
            _if_ = false;
                    if (IsSetCommandServer)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _value_.SetCommandServer(this);");
            }
            _code_.Add(@"
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name=""index""></param>
                /// <param name=""socket""></param>
                /// <param name=""data""></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@") base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@")
                        {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@": ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket, ref data); return;");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                            default: return;
                        }
                    }
                }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = ServiceAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name=""index""></param>
                /// <param name=""socket""></param>
                protected override void doHttpCommand(int index, fastCSharp.net.tcp.http.socketBase socket)
                {
                    switch (index - ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@")
                    {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@": ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket); return;");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                        default: return;
                    }
                }");
            }
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodGroup[] _value1_;
                    _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodGroup _value2_ in _value1_)
                        {
            _code_.Add(@"
                private int ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@";
                private int ");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodGroup[] _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 忽略分组
                /// </summary>
                /// <param name=""groupId"">分组标识</param>
                protected override void ignoreGroup(int groupId)
                {
                    switch (groupId)
                    {");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodGroup[] _value1_;
                    _value1_ = MethodGroups;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodGroup _value2_ in _value1_)
                        {
            _code_.Add(@"
                        case ");
            _code_.Add(_value2_.GroupId.ToString());
            _code_.Add(@":
                            ");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" = 1;
                            while (");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@" != 0) System.Threading.Thread.Sleep(1);
                            break;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    }
                }");
            }
            _if_ = false;
                    if (IsAnyGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                static tcpServer()
                {
                    System.Collections.Generic.Dictionary<fastCSharp.code.cSharp.tcpBase.genericMethod, System.Reflection.MethodInfo> genericMethods = fastCSharp.code.cSharp.tcpServer.GetGenericMethods(typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"));");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"genericMethods[new fastCSharp.code.cSharp.tcpBase.genericMethod(""");
            _code_.Add(_value3_.MethodName);
            _code_.Add(@""", ");
                {
                    fastCSharp.code.memberType[] _value4_ = _value3_.GenericParameters;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.Length.ToString());
                    }
                }
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.Parameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(@", """);
            _code_.Add(_value5_.ParameterRef);
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@"""");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@")]");
            }
                }
            _code_.Add(@";");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }");
            }
            }
            _code_.Add(@"
");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsInputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsInputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
                internal ");
            _code_.Add(_value2_.InputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
                {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    public fastCSharp.code.remoteType[] ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@";");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    public fastCSharp.code.cSharp.tcpBase.tcpStream ");
            _code_.Add(_value4_.StreamParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                if (!(bool)_value5_.IsStream)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    public fastCSharp.code.remoteType ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@";");
            }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.dataSerialize(IsMemberMap = false");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputSerializeReferenceMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@", IsReferenceMember = false");
            }
            _code_.Add(@")]");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
#if NOJIT
                internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter");
            }
            }
            _code_.Add(@"
#else
                internal ");
            _code_.Add(_value2_.OutputParameterClassType);
            _code_.Add(@" ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsOutputParameterClass)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.returnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.asynchronousMethod.IReturnParameter<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            }
            _code_.Add(@"
#endif
                {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                    public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsOutputParameterClass)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Ret;
                    public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")value; }
                    }
#endif");
            }
            }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                    if (IsServerCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly System.Reflection.MethodInfo ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@";");
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsMethodServerCall)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                sealed class ");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@" : fastCSharp.net.tcp.commandServer.serverCall<");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = type;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.InputParameterTypeName);
            }
            _code_.Add(@">
                {
                    private void get(ref fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" value)
                    {
                        try
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"};");
            }
            }
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@";");
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsGetMember)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                    if (_value4_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = serverValue[");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"];");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                if (_value4_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = serverValue.");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@";");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsGetMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                    if (_value4_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            serverValue[");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.PropertyParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"] = ");
                {
                    fastCSharp.code.parameterInfo _value3_ = default(fastCSharp.code.parameterInfo);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.PropertyParameter;
                    }
                }
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value3_.ParameterName);
            }
                }
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                if (_value4_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            serverValue.");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@";");
            }
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" =  (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(serverValue, ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"serverValue.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")invokeParameter[");
            _code_.Add(_value4_.ParameterIndex.ToString());
            _code_.Add(@"];");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            if (");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@") socket.SetVerifyMethod();");
            }
            _code_.Add(@"
                            value.Value = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"
                            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            _code_.Add(_value2_.ReturnName);
            }
            _code_.Add(@"
                            };");
            }
            _code_.Add(@"
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" value = new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"();");
            _if_ = false;
                    if (_value2_.IsClientSendOnly != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (isVerify == 0) get(ref value);");
            }
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }");
            }
            _code_.Add(@"
                        fastCSharp.typePool<");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@">.PushNotNull(this);
                    }
                }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                struct ");
            _code_.Add(_value2_.MethodMergeName);
            _code_.Add(@"
                {
                    public ");
                {
                    fastCSharp.code.memberType _value3_ = type;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ServerValue;
                    public socket Socket;");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    public ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" InputParameter;");
            }
            _code_.Add(@"
                    public fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"};");
            }
            }
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsGetMember)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                    if (_value4_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ServerValue[");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"];");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                if (_value4_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ServerValue.");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@";");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsGetMember)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                    if (_value4_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ServerValue[");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.PropertyParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"] = ");
                {
                    fastCSharp.code.parameterInfo _value3_ = default(fastCSharp.code.parameterInfo);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.PropertyParameter;
                    }
                }
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"InputParameter.");
            _code_.Add(_value3_.ParameterName);
            }
                }
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value4_ = _value3_.PropertyParameters;
                    if (_value4_ != null)
                    {
                if (_value4_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ServerValue.");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@";");
            }
            }
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(ServerValue, ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", InputParameter.");
            _code_.Add(GenericParameterTypeName);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", invokeParameter");
            }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"ServerValue.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"Socket");
            _if_ = false;
                {
                    fastCSharp.code.parameterInfo[] _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                    if (_value3_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@", ");
            }
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"Socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = (");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.GenericParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")invokeParameter[");
            _code_.Add(_value4_.ParameterIndex.ToString());
            _code_.Add(@"];");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            if (");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@") Socket.SetVerifyMethod();");
            }
            _code_.Add(@"
                            return new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"
                            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.OutputParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = InputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnName);
            _code_.Add(@" = ");
            _code_.Add(_value2_.ReturnName);
            }
            _code_.Add(@"
                            };");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = returnType };
                    }
                }");
            }
            }
            _code_.Add(@"
                private void ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(socket socket, ref subArray<byte> data)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    socket.SendStream(socket.Identity, fastCSharp.net.returnValue.type.VersionExpired);");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" == 0)
                    {
                        System.Threading.Interlocked.Increment(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                        try
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" inputParameter = new ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))");
            }
            _code_.Add(@"
                            {");
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" outputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                                Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            }
            _code_.Add(@">.Get");
            _code_.Add(_value2_.KeepCallback);
            _code_.Add(@"(socket, ref outputParameter, ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@");
                                if (callbackReturn != null)
                                {");
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool>)");
            }
            _code_.Add(@"callbackReturn");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                    fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                                    ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    _value_.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"callbackReturn);");
            }
            _code_.Add(@"
                                }");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                Func<fastCSharp.net.returnValue, bool> callback");
            _if_ = false;
                    if (_value2_.IsClientSendOnly != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" = null");
            }
            _code_.Add(@";");
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                if ((callback = fastCSharp.net.tcp.commandServer.socket.callback.Get");
            _code_.Add(_value2_.KeepCallback);
            _code_.Add(@"(socket, ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@")) != null)");
            }
            _code_.Add(@"
                                {");
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    object[] invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue, bool>)");
            }
            _code_.Add(@"callback");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                    fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                                    ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                    _value_.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"callback);");
            }
            _code_.Add(@"
                                }");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsMethodServerCall)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsServerAsynchronousTask)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.threading.task.Tiny.Add(");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@"/**/.GetCall(socket, _value_");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref inputParameter");
            }
            _code_.Add(@" ));");
            }
            _if_ = false;
                if (!(bool)_value2_.IsServerAsynchronousTask)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.MethodStreamName);
            _code_.Add(@"/**/.Call(socket, _value_");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref inputParameter");
            }
            _code_.Add(@" );");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsMethodServerCall)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                _value_.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"socket");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@", ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"socket.GetTcpStream(");
            _code_.Add(_value4_.Ref);
            }
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                                return;
                            }");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;");
            }
            }
            _code_.Add(@"
                        }
                        catch (Exception error)
                        {");
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            returnType = fastCSharp.net.returnValue.type.ServerException;");
            }
            _code_.Add(@"
                            fastCSharp.log.Error.Add(error, null, true);
                        }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");
                        }
                    }");
            }
            _if_ = false;
                if (_value2_.IsClientSendOnly == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    socket.SendStream(socket.Identity, returnType);");
            }
            }
            _code_.Add(@"
                }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsHttpClient)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                private void ");
            _code_.Add(_value2_.MethodIndexName);
            _code_.Add(@"(fastCSharp.net.tcp.http.socketBase socket)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    socket.ResponseError(socket.TcpCommandSocket.HttpPage.SocketIdentity, fastCSharp.net.tcp.http.response.state.NotFound404);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    long identity = int.MinValue;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    if (");
            _code_.Add(_value2_.GroupIgnoreName);
            _code_.Add(@" == 0)
                    {
                        System.Threading.Interlocked.Increment(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");");
            }
            _code_.Add(@"
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" inputParameter = new ");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"();
                            if (httpPage.DeSerialize(ref inputParameter))");
            }
            _code_.Add(@"
                            {");
            _if_ = false;
                    if (_value2_.IsAsynchronousCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsInvokeGenericMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                object[] invokeParameter;");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" invokeOutputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                                invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">, bool>)");
            }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.IsVerifyMethod ? "true" : "false");
            }
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Get(httpPage, ref invokeOutputParameter)");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                invokeParameter = new object[] { ");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@", ");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericParameterCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(Func<fastCSharp.net.returnValue, bool>)");
            }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage)");
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@" };");
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, ");
            _code_.Add(_value2_.GenericMethodInfoName);
            _code_.Add(@", inputParameter.");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@", invokeParameter);");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsGenericMethod)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@" outputParameter = new ");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@"();
                                ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                _value_.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.IsVerifyMethod ? "true" : "false");
            }
            _code_.Add(@"<");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.GenericReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Get(httpPage, ref outputParameter));");
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                _value_.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
            _if_ = false;
                    if (_value2_.ClientParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"commandSocket, ");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _if_ = false;
                    if (_value4_.IsGenericParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"inputParameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage));");
            }
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsAsynchronousCallback)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                httpPage.Response(new ");
            _code_.Add(_value2_.MethodMergeName);
            _code_.Add(@" { Socket = commandSocket, ServerValue = _value_");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", InputParameter = inputParameter");
            }
            _code_.Add(@" }.Get());");
            }
            _code_.Add(@"
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.GroupId != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref ");
            _code_.Add(_value2_.GroupCountName);
            _code_.Add(@");
                        }
                    }");
            }
            _code_.Add(@"
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);");
            }
            _code_.Add(@"
                }");
            }
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }");
            _if_ = false;
                    if (IsClientCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsLoadBalancing)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// TCP负载均衡服务
            /// </summary>
            public sealed class tcpLoadBalancing : fastCSharp.net.tcp.commandLoadBalancingServer<tcpClient>
            {
                /// <summary>
                /// TCP负载均衡服务
                /// </summary>
                /// <param name=""attribute"">TCP调用服务器端配置信息</param>");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""verifyMethod"">TCP验证方法</param>");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""verify"">TCP验证实例</param>");
            }
            _code_.Add(@"
#if NOJIT
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null");
            }
            _code_.Add(@")
#else
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null");
            }
            _code_.Add(@")
#endif
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig(""");
            _code_.Add(LoadBalancingServiceName);
            _code_.Add(@""", typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@"))");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verifyMethod");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyMethodType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyMethodType);
            _code_.Add(@"()");
            }
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verify");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@")
                {
                }
                protected override tcpClient _createClient_(fastCSharp.code.cSharp.tcpServer attribute)
                {
                    tcpClient client = new tcpClient(attribute");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", _verifyMethod_");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", _verify_");
            }
            _code_.Add(@");
                    fastCSharp.net.tcp.commandClient commandClient = client.TcpCommandClient;
                    if (commandClient != null && commandClient.StreamSocket != null) return client;
                    fastCSharp.pub.Dispose(ref client);
                    return null;
                }
                protected override DateTime _loadBalancingCheckTime_(tcpClient client)
                {
                    fastCSharp.net.tcp.commandClient tcpClient = client.TcpCommandClient;
                    return tcpClient != null ? tcpClient.LoadBalancingCheckTime : DateTime.MinValue;
                }
                protected override fastCSharp.net.returnValue.type _loadBalancingCheck_(tcpClient client)
                {
                    fastCSharp.net.tcp.commandClient tcpClient = client.TcpCommandClient;
                    return tcpClient == null ? fastCSharp.net.returnValue.type.ClientDisposed : tcpClient.LoadBalancingCheck();
                }
");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsClientSynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@")
                {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.Unknown;
                    int _tryCount_ = ");
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = default(fastCSharp.code.cSharp.tcpServer);
                    _value3_ = _value2_.ServiceAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.LoadBalancingTryCount.ToString());
            }
                }
            _code_.Add(@";
                    do
                    {
                        clientIdentity _client_ = _getClient_();
                        if (_client_.Client == null)
                        {
                            _returnType_ = fastCSharp.net.returnValue.type.ClientDisposed;
                            break;
                        }
                        try
                        {
                            fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" _return_ = _client_.Client.");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterJoinRefName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");
                            _end_(ref _client_, _return_.Type);
                            if (_return_.Type == fastCSharp.net.returnValue.type.Success || _return_.Type == fastCSharp.net.returnValue.type.VersionExpired) return _return_;
                            System.Threading.Thread.Sleep(1);
                        }
                        catch (Exception _error_)
                        {
                            _end_(ref _client_, _returnType_ = fastCSharp.net.returnValue.type.ClientException);
                            fastCSharp.log.Error.Add(_error_, null, false);
                        }
                    }
                    while (--_tryCount_ > 0);");
            }
            _code_.Add(@"
                    return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = _returnType_ };
                }");
            }
            _if_ = false;
                    if (_value2_.IsClientAsynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                sealed class ");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@" : fastCSharp.code.cSharp.tcpBase.loadBalancingCallback");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"
                {
                    private clientIdentity _client_;
                    private tcpLoadBalancing _loadBalancingServer_;");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                    private ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                    protected override void _call_()
                    {
                        fastCSharp.net.returnValue.type _returnType_;
                        try
                        {
                            _client_ = _loadBalancingServer_._getClient_();
                            if (_client_.Client != null)
                            {
                                _client_.Client.");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturnHandle_);
                                return;
                            }
                            _returnType_ = fastCSharp.net.returnValue.type.ClientDisposed;
                        }
                        catch (Exception error)
                        {
                            _loadBalancingServer_._end_(ref _client_, _returnType_ = fastCSharp.net.returnValue.type.ClientException);
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                        _push_(new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = _returnType_ });
                    }
                    protected override void _push_(fastCSharp.net.returnValue.type isReturn)
                    {
                        _loadBalancingServer_._end_(ref _client_, isReturn);
                        _loadBalancingServer_ = null;");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                        ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                        fastCSharp.typePool<");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@">.PushNotNull(this);
                    }
                    public static void _Call_(tcpLoadBalancing _loadBalancingServer_,
                        ");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_)
                    {
                        ");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@" _callback_ = fastCSharp.typePool<");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@">.Pop();
                        if (_callback_ == null)
                        {
                            try
                            {
                                _callback_ = new ");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@"();
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                                _onReturn_(new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.ClientException });");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                                return;
                            }
                        }");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                        _callback_.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                        _callback_._loadBalancingServer_ = _loadBalancingServer_;
                        _callback_._onReturn_ = _onReturn_;
                        _callback_._tryCount_ =");
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = default(fastCSharp.code.cSharp.tcpServer);
                    _value3_ = _value2_.ServiceAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.LoadBalancingTryCount.ToString());
            }
                }
            _code_.Add(@";");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                        _callback_._call_();
                    }
                }
                public void ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onReturn_(new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.VersionExpired });");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value2_.LoadBalancingCallbackName);
            _code_.Add(@"/**/._Call_(this, ");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturn_);");
            }
            _code_.Add(@"
                }");
            }
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }");
            }
            _code_.Add(@"
");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsClientInterface)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// TCP客户端操作接口
            /// </summary>
            public interface ITcpClient : IDisposable
            {
                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name=""groupId"">分组标识</param>
                /// <returns>是否调用成功</returns>
                fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId);");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> this[");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"] { get;");
            _if_ = false;
                    if (_value2_.SetMethod != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" set;");
            }
            _code_.Add(@" }");
            }
            _if_ = false;
                if (!(bool)_value2_.IsInputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@" { get;");
            _if_ = false;
                    if (_value2_.SetMethod != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" set;");
            }
            _code_.Add(@" }");
            }
            }
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsClientInterface)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsClientSynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</returns>");
            }
            _code_.Add(@"
                fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsClientAsynchronous)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""_onReturn_"">");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</param>");
            }
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>保持异步回调</returns>");
            }
            _code_.Add(@"
                ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_);");
            }
            }
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }");
            }
            _code_.Add(@"
            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsClientInterface)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"ITcpClient, ");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.ClientInterfaceType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(ClientInterfaceType);
            _code_.Add(@", ");
            }
            _if_ = false;
                    if (IsTimeVerify)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, ");
            }
            _code_.Add(@"fastCSharp.net.tcp.commandClient.IClient
            {");
            _if_ = false;
                    if (IsTimeVerify)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name=""client"">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name=""client"">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif");
            }
            _code_.Add(@"
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name=""attribute"">TCP调用服务器端配置信息</param>");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""verifyMethod"">TCP验证方法</param>");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""verify"">TCP验证实例</param>");
            }
            _code_.Add(@"
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null");
            }
            _code_.Add(@")
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null");
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null");
            }
            _code_.Add(@")
#endif
                {");
            _if_ = false;
                    if (IsServerCode)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig(""");
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.ServiceName);
                    }
                }
            _code_.Add(@""", typeof(");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@")), ");
            _code_.Add(MaxCommandLength.ToString());
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verifyMethod");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyMethodType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyMethodType);
            _code_.Add(@"()");
            }
            _code_.Add(@", this");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verify");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@");");
            }
            _if_ = false;
                if (!(bool)IsServerCode)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig(TcpServerAttribute, """);
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.ServiceName);
                    }
                }
            _code_.Add(@"""), ");
            _code_.Add(MaxCommandLength.ToString());
            _if_ = false;
                    if (IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verifyMethod");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyMethodType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyMethodType);
            _code_.Add(@"()");
            }
            _code_.Add(@", this");
            }
            _if_ = false;
                if (!(bool)IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", verify");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.VerifyType != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" ?? new ");
            _code_.Add(TcpVerifyType);
            _code_.Add(@"()");
            }
            }
            _code_.Add(@");");
            }
            _code_.Add(@"
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }
");
            _if_ = false;
                if (!(bool)IsServerCode)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// TCP服务调用配置
                /// </summary>
                public static fastCSharp.code.cSharp.tcpServer TcpServerAttribute
                {
                    get { return fastCSharp.emit.jsonParser.Parse<fastCSharp.code.cSharp.tcpServer>(@""");
            _code_.Add(AttributeJson);
            _code_.Add(@"""); }
                }");
            }
            _code_.Add(@"

                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name=""groupId"">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }
");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _code_.Add(@" = new fastCSharp.net.tcp.commandClient.identityCommand { Command = ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@" + ");
            _code_.Add(CommandStartIndex.ToString());
            _code_.Add(@", MaxInputSize = ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            _code_.Add(@", IsKeepCallback = ");
            _code_.Add(_value2_.IsKeepCallback.ToString());
            _code_.Add(@", IsSendOnly = ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@" };");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                private static readonly fastCSharp.net.tcp.commandClient.dataCommand ");
            _code_.Add(_value2_.MethodDataCommand);
            _code_.Add(@" = new fastCSharp.net.tcp.commandClient.dataCommand { Command = fastCSharp.net.tcp.commandServer.GetMethodKeyNameCommand(""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyName);
                    }
                }
            _code_.Add(@"""), MaxInputSize = ");
            _code_.Add(_value2_.InputParameterMaxLength.ToString());
            _code_.Add(@", IsKeepCallback = ");
            _code_.Add(_value2_.IsKeepCallback.ToString());
            _code_.Add(@", IsSendOnly = ");
            _code_.Add(_value2_.IsClientSendOnly.ToString());
            _code_.Add(@" };");
            }
            _code_.Add(@"
");
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsClientSynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</returns>");
            }
            _code_.Add(@"
                public fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@")
                {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        this.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value5_ = _value2_.Method;
                    if (_value5_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value6_ = _value5_.Method;
                    if (_value6_ != null)
                    {
                    if (_value6_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameter_.Value.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")_outputParameter_.Value.Return;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameter_.Value.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                    return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = _returnType_ };
                }");
            }
            _if_ = false;
                    if (_value2_.IsClientAsynchronous)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""_onReturn_"">");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</param>");
            }
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <returns>保持异步回调</returns>");
            }
            _code_.Add(@"
                public ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onReturn_(new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.VersionExpired });");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.callback<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _onOutput_;");
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturnGeneric<");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@", ");
            }
                {
                    fastCSharp.code.auto.parameter _value3_ = default(fastCSharp.code.auto.parameter);
                    _value3_ = AutoParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.DefaultNamespace);
            }
                }
            _code_.Add(@".");
            _code_.Add(ParameterPart);
            _code_.Add(@"/**/.");
            _code_.Add(ServiceName);
            _code_.Add(@"/**/.");
            _code_.Add(_value2_.OutputParameterGenericTypeName);
            _code_.Add(@">.Get(_onReturn_);");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@", ");
            }
            _code_.Add(@"tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">.Get(_onReturn_);");
            }
            _code_.Add(@"
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        return ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _onOutput_");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");
                    }");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            }
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return ");
            }
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"this.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturn_, null");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");");
            }
            }
            _code_.Add(@"
                }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> this[");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"]
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        this.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value5_ = _value2_.Method;
                    if (_value5_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value6_ = _value5_.Method;
                    if (_value6_ != null)
                    {
                    if (_value6_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameter_.Value.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")_outputParameter_.Value.Return;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameter_.Value.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                        return new fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> { Type = _returnType_ };
                    }");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value3_ = default(fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex);
                    _value3_ = _value2_.SetMethod;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value4_ = default(fastCSharp.code.methodInfo);
                    _value4_ = _value3_.Method;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        this.");
            _code_.Add(_value4_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(_value5_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value6_ = _value3_.Method;
                    if (_value6_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value7_ = _value6_.Method;
                    if (_value7_ != null)
                    {
                    if (_value7_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameter_.Value.");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            }
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@")_outputParameter_.Value.Return;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameter_.Value.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value3_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value3_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }");
            }
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            }
            _code_.Add(@"
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }");
            }
                }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                if (!(bool)_value2_.IsInputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                public fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> ");
            _code_.Add(_value2_.PropertyName);
            _code_.Add(@"
                {
                    get
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        this.");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value5_ = _value2_.Method;
                    if (_value5_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value6_ = _value5_.Method;
                    if (_value6_ != null)
                    {
                    if (_value6_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameter_.Value.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@")_outputParameter_.Value.Return;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameter_.Value.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value2_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value2_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }");
            }
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            }
            _code_.Add(@"
                        return new fastCSharp.net.returnValue<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"> { Type = _returnType_ };
                    }");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value3_ = default(fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex);
                    _value3_ = _value2_.SetMethod;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    set
                    {
                    fastCSharp.net.returnValue.type _returnType_;");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value4_ = _value3_.Attribute;
                    if (_value4_ != null)
                    {
                if (!(bool)_value4_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _wait_ = fastCSharp.net.waitCall");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@".Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        ");
                {
                    fastCSharp.code.methodInfo _value4_ = default(fastCSharp.code.methodInfo);
                    _value4_ = _value3_.Method;
            _if_ = false;
                    if (_value4_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        this.");
            _code_.Add(_value4_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _code_.Add(_value5_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            _code_.Add(@"null, _wait_, false);");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsRefOrOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value6_ = _value3_.Method;
                    if (_value6_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value7_ = _value6_.Method;
                    if (_value7_ != null)
                    {
                    if (_value7_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@")");
            }
            _code_.Add(@"_outputParameter_.Value.");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            }
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.IsGenericReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return (");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@")_outputParameter_.Value.Return;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsGenericReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return _outputParameter_.Value.Return;");
            }
            }
            _if_ = false;
                if (!(bool)_value3_.MethodIsReturn)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.MethodIsReturn)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<");
                {
                    fastCSharp.code.memberType _value4_ = _value3_.MethodReturnType;
                    if (_value4_ != null)
                    {
            _code_.Add(_value4_.FullName);
                    }
                }
            _code_.Add(@">");
            }
            _code_.Add(@"{ Type = fastCSharp.net.returnValue.type.Success };");
            }
            _code_.Add(@"
                        }
                        _returnType_ = _outputParameter_.Type;");
            }
            _if_ = false;
                if (!(bool)_value3_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value3_.MemberIndex != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return;");
            }
            _if_ = false;
                if (_value3_.MemberIndex == null)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value3_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;");
            }
            }
            _code_.Add(@"
                    }");
            }
            _if_ = false;
                    if (_value3_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
                {
                    fastCSharp.code.parameterInfo[] _value4_;
                    _value4_ = _value3_.MethodParameters;
                    if (_value4_ != null)
                    {
                        int _loopIndex4_ = _loopIndex_, _loopCount4_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value4_.Length;
                        foreach (fastCSharp.code.parameterInfo _value5_ in _value4_)
                        {
            _if_ = false;
                    if (_value5_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value5_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value6_ = _value5_.ParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex4_;
                        _loopCount_ = _loopCount4_;
                    }
                }
            }
            _code_.Add(@"
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }");
            }
                }
            _code_.Add(@"
                }");
            }
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                private ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@" _returnType_ = new fastCSharp.net.returnValue");
            _if_ = false;
                    if (_value2_.IsOutputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.OutputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"();");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsOut)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = default(");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
            _code_.Add(_value5_.FullName);
                    }
                }
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = ");
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.StreamSocket");
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.VerifyStreamSocket");
            }
            _code_.Add(@";
                        if (_socket_ != null)
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" _inputParameter_ = new tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
                            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@" = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0");
                {
                    fastCSharp.code.memberType[] _value3_ = default(fastCSharp.code.memberType[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.GenericParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.memberType _value4_ in _value3_)
                        {
            _code_.Add(@", typeof(");
            _code_.Add(_value4_.FullName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"),");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@" = typeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"),");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                if (!(bool)_value4_.IsOut)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.GetTcpStream(");
            }
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@",");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                            };");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.IsRef)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            _returnType_.Value.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = _inputParameter_.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                    if (_value2_.ReturnInputParameterName != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            _returnType_.Value.Ret = _inputParameter_.");
            _code_.Add(_value2_.ReturnInputParameterName);
            _code_.Add(@";");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Get");
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", ref _returnType_.Value, _isTask_);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Get");
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodDataCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", ref _returnType_.Value, _isTask_);");
            }
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return;");
            }
            _code_.Add(@"
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            _code_.Add(@"
                }");
            }
            _if_ = false;
                if (!(bool)_value2_.IsOutputParameter)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.XmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.XmlDocument);
                    }
                }
            _code_.Add(@"
                /// </summary>");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                    if (_value4_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@""">");
            _code_.Add(_value4_.XmlDocument);
            _code_.Add(@"</param>");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                    if (_value3_.ReturnXmlDocument != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <param name=""_onReturn_"">");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.ReturnXmlDocument);
                    }
                }
            _code_.Add(@"</param>");
            }
            _code_.Add(@"
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue> _onReturn_)
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsExpired)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    _onReturn_(new fastCSharp.net.returnValue { Type = fastCSharp.net.returnValue.type.VersionExpired });");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsExpired)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRefName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"_onReturn_, null");
                {
                    fastCSharp.code.cSharp.tcpMethod _value3_ = default(fastCSharp.code.cSharp.tcpMethod);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsClientCallbackTask ? "true" : "false");
            }
                }
            _code_.Add(@");");
            }
            _code_.Add(@"
                }");
            }
            _code_.Add(@"
                private ");
            _code_.Add(_value2_.KeepCallbackType);
            _code_.Add(@" ");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterTypeRefName);
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@", ");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = ");
            _if_ = false;
                if (!(bool)_value2_.IsVerifyMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.StreamSocket");
            }
            _if_ = false;
                    if (_value2_.IsVerifyMethod)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.VerifyStreamSocket");
            }
            _code_.Add(@";
                        if (_socket_ != null)
                        {");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@" _inputParameter_ = new tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@"
                            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
                {
                    System.Reflection.MethodInfo _value4_ = _value3_.Method;
                    if (_value4_ != null)
                    {
                    if (_value4_.IsGenericMethod)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(GenericParameterTypeName);
            _code_.Add(@" = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0");
                {
                    fastCSharp.code.memberType[] _value3_ = default(fastCSharp.code.memberType[]);
                {
                    fastCSharp.code.methodInfo _value4_ = _value2_.Method;
                    if (_value4_ != null)
                    {
                    _value3_ = _value4_.GenericParameters;
                    }
                }
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.memberType _value4_ in _value3_)
                        {
            _code_.Add(@", typeof(");
            _code_.Add(_value4_.FullName);
            _code_.Add(@")");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"),");
            }
            _if_ = false;
                    if (_value2_.IsGenericParameterCallback)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value2_.ReturnTypeName);
            _code_.Add(@" = typeof(");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MethodReturnType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"),");
            }
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _if_ = false;
                if (!(bool)_value4_.IsOut)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                                ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@" = ");
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"TcpCommandClient.GetTcpStream(");
            }
            _code_.Add(_value4_.ParameterName);
            _if_ = false;
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                    if (_value5_.IsStream)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@")");
            }
            _code_.Add(@",");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
                            };");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsIdentityCommand)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Call");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodIdentityCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", _isTask_);");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.tcpServer _value3_ = _value2_.ServiceAttribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsIdentityCommand)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            ");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return ");
            }
            _code_.Add(@"_socket_.Call");
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value2_.JsonCall);
            _code_.Add(@"<tcpServer.");
            _code_.Add(_value2_.InputParameterTypeName);
            _code_.Add(@">");
            }
            _code_.Add(@"(_onReturn_, _callback_, ");
            _code_.Add(_value2_.MethodDataCommand);
            _if_ = false;
                    if (_value2_.IsInputParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ref _inputParameter_");
            }
            _code_.Add(@", _isTask_);");
            }
            _if_ = false;
                if (_value2_.IsKeepCallback == 0)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                            return;");
            }
            _code_.Add(@"
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);");
            _if_ = false;
                    if (_value2_.IsKeepCallback != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return null;");
            }
            _code_.Add(@"
                }");
            }
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }");
            }
            }
            _code_.Add(@"
");
            stringBuilder _PART_REMEMBER_ = _code_;
            _code_ = new stringBuilder();
            _if_ = false;
                    if (IsRemember)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class ");
            _code_.Add(ServiceNameOnly);
            _code_.Add(@"
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] ");
            _code_.Add(RememberIdentityCommeandName);
            _code_.Add(@"()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@"];");
                {
                    fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex[] _value1_;
                    _value1_ = MethodIndexs;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                if (!(bool)_value2_.IsNullMethod)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    names[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"].Set(@""");
                {
                    fastCSharp.code.methodInfo _value3_ = _value2_.Method;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MethodKeyName);
                    }
                }
            _code_.Add(@""", ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@");");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return names;
                }
            }");
            }
            _partCodes_["REMEMBER"] = _code_.ToString();
            _code_ = _PART_REMEMBER_;
            _code_.Add(_partCodes_["REMEMBER"]);
            _code_.Add(@"
        }");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class webCall
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _if_ = false;
                {
                    fastCSharp.code.webCall.cSharp.methodIndex[] _value1_ = Methods;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<");
                {
                    fastCSharp.code.memberType _value1_ = SessionType;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">
        {
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected override string[] calls
            {
                get
                {
                    string[] names = new string[");
                {
                    fastCSharp.code.webCall.cSharp.methodIndex[] _value1_ = Methods;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@"];");
                {
                    fastCSharp.code.webCall.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webCall.cSharp.methodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                    names[");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return names;
                }
            }
            /// <summary>
            /// WEB调用处理
            /// </summary>
            /// <param name=""callIndex""></param>
            /// <param name=""socket""></param>
            /// <param name=""socketIdentity""></param>
            protected override void call(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (callIndex)
                {");
                {
                    fastCSharp.code.webCall.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webCall.cSharp.methodIndex _value2_ in _value1_)
                        {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.MethodIndex.ToString());
            _code_.Add(@":");
            _if_ = false;
                    if (_value2_.IsAjaxLoad)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        loadAjax<");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">(socket, socketIdentity, ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@"/**/.Get(), ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webCall _value3_ = _value2_.TypeAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" fastCSharp.typePool<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Pop() ??");
            }
            _code_.Add(@" new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"());");
            }
            _if_ = false;
                if (!(bool)_value2_.IsAjaxLoad)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                        load<");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">(socket, socketIdentity, ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@"/**/.Get(), ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webCall _value3_ = _value2_.TypeAttribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@" fastCSharp.typePool<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Pop() ??");
            }
            _code_.Add(@" new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"(), ");
            _code_.Add(_value2_.MaxPostDataSize.ToString());
            _code_.Add(@", ");
            _code_.Add(_value2_.MaxMemoryStreamSize.ToString());
                {
                    fastCSharp.code.cSharp.webCall _value3_ = default(fastCSharp.code.cSharp.webCall);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsOnlyPost ? "true" : "false");
            }
                }
                {
                    fastCSharp.code.cSharp.webCall _value3_ = default(fastCSharp.code.cSharp.webCall);
                    _value3_ = _value2_.TypeAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsPool ? "true" : "false");
            }
                }
            _code_.Add(@");");
            }
            _code_.Add(@"
                        return;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }
            }");
                {
                    fastCSharp.code.webCall.cSharp.methodIndex[] _value1_;
                    _value1_ = Methods;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webCall.cSharp.methodIndex _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webCall _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsSerializeBox)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            [fastCSharp.emit.boxSerialize]");
            }
            _code_.Add(@"
            struct ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@"
            {");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value5_ = _value4_.ParameterType;
                    if (_value5_ != null)
                    {
                {
                    fastCSharp.code.memberType _value6_ = _value5_.GenericParameterType;
                    if (_value6_ != null)
                    {
            _code_.Add(_value6_.FullName);
                    }
                }
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@"
            }");
            }
            _code_.Add(@"
            private sealed class ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@" : fastCSharp.code.cSharp.webCall.callPool<");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@", ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebCallMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value2_.ParameterTypeName);
            }
            _code_.Add(@">
            {
                private ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@"() : base() { }
                public override bool Call()
                {
                    try
                    {");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webCall _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsFirstParameter)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                        if (WebCall.ParseParameterAny(ref Parameter");
                {
                    fastCSharp.code.parameterInfo _value3_ = default(fastCSharp.code.parameterInfo);
                    _value3_ = _value2_.FristParameter;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@".");
            _code_.Add(_value3_.ParameterName);
            }
                }
            _code_.Add(@"))");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webCall _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                if (!(bool)_value3_.IsFirstParameter)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                            if (WebCall.ParseParameter(ref Parameter))");
            }
            }
            _code_.Add(@"
                            {
                                WebCall.");
                {
                    fastCSharp.code.methodInfo _value3_ = default(fastCSharp.code.methodInfo);
                    _value3_ = _value2_.Method;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.MethodGenericName);
            }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value3_;
                    _value3_ = _value2_.MethodParameters;
                    if (_value3_ != null)
                    {
                        int _loopIndex3_ = _loopIndex_, _loopCount3_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value3_.Length;
                        foreach (fastCSharp.code.parameterInfo _value4_ in _value3_)
                        {
            _code_.Add(_value4_.ParameterRef);
            _code_.Add(@"Parameter.");
            _code_.Add(_value4_.ParameterName);
            _code_.Add(_value4_.ParameterJoin);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex3_;
                        _loopCount_ = _loopCount3_;
                    }
                }
            _code_.Add(@");
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@">.PushNotNull(this);
                    }");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    return false;");
            }
            _code_.Add(@"
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@" Get()
                {
                    ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@" call = fastCSharp.typePool<");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@">.Pop();
                    if (call == null) call = new ");
            _code_.Add(_value2_.MethodCallName);
            _code_.Add(@"();");
            _if_ = false;
                    if (_value2_.IsParameter)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    else call.Parameter = new ");
            _code_.Add(_value2_.ParameterTypeName);
            _code_.Add(@"();");
            }
            _code_.Add(@"
                    return call;
                }
            }");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
        }");
            }
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class webSocket
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _if_ = false;
                if (!(bool)IsServer)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (LoadMethod != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@" : fastCSharp.code.cSharp.webSocket.IWebSocket
        {
            private struct webSocketQuery
            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value1_ = LoadMethod;
                    if (_value1_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value2_ = _value1_.Parameters;
                    if (_value2_ != null)
                    {
                    if (_value2_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]");
            }
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.ParameterType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }
            /// <summary>
            /// WebSocket调用
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadSocket()
            {
                if (base.loadSocket())
                {
                    webSocketQuery query = new webSocketQuery();
                    if (parseParameterQuery(ref query))
                    {
                        return loadSocket(");
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _code_.Add(@"query.");
            _code_.Add(_value2_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@");
                    }
                }
                return false;
            }
        }");
            }
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (IsServer)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                {
                    fastCSharp.code.webSocket.cSharp.socketType[] _value1_ = Sockets;
                    if (_value1_ != null)
                    {
                    if (_value1_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<");
                {
                    fastCSharp.code.memberType _value1_ = SessionType;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">
        {
            /// <summary>
            /// webSocket处理索引集合
            /// </summary>
            protected override string[] webSockets
            {
                get
                {
                    string[] names = new string[");
                {
                    fastCSharp.code.webSocket.cSharp.socketType[] _value1_ = Sockets;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@"];");
                {
                    fastCSharp.code.webSocket.cSharp.socketType[] _value1_;
                    _value1_ = Sockets;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webSocket.cSharp.socketType _value2_ in _value1_)
                        {
            _code_.Add(@"
                    names[");
            _code_.Add(_value2_.Index.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return names;
                }
            }
            /// <summary>
            /// webSocket调用处理
            /// </summary>
            /// <param name=""callIndex""></param>
            /// <param name=""socket""></param>
            /// <param name=""socketIdentity""></param>
            protected override void callWebSocket(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch(callIndex)
                {");
                {
                    fastCSharp.code.webSocket.cSharp.socketType[] _value1_;
                    _value1_ = Sockets;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webSocket.cSharp.socketType _value2_ in _value1_)
                        {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.Index.ToString());
            _code_.Add(@": loadWebSocket(new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebSocketMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"(), socket, socketIdentity); return;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }
            }
        }");
            }
            }
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class webView
    {
    internal partial class cSharp
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.CSharp, _isOut_))
            {
                
            _if_ = false;
                if (!(bool)IsServer)
                {
                    _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
        ");
            _code_.Add(TypeNameDefinition);
            _code_.Add(@" : fastCSharp.code.cSharp.webView.IWebView
        {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsPage)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@""");
            _code_.Add(HtmlFile);
            _code_.Add(@""", ");
            _code_.Add(HtmlCount.ToString());
            _code_.Add(@"))
                {
                    ");
            _code_.Add(PageCode);
            _code_.Add(@"
                    return true;
                }
                return false;
            }");
            }
            _code_.Add(@"
");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value1_ = Attribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.IsAjax)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name=""js"">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                ");
            _code_.Add(AjaxCode);
            _code_.Add(@"
            }");
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (LoadMethod != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            private struct webViewQuery
            {");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value1_ = LoadMethod;
                    if (_value1_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value2_ = _value1_.Parameters;
                    if (_value2_ != null)
                    {
                    if (_value2_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]");
            }
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.ParameterType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.ParameterName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = QueryMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _if_ = false;
                    if (_value2_.XmlDocument != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                /// <summary>
                /// ");
            _code_.Add(_value2_.XmlDocument);
            _code_.Add(@"
                /// </summary>");
            }
            _code_.Add(@"
                public ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.MemberType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@" ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
            }");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value1_ = LoadAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.QueryName != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// 查询参数
            /// </summary>
            private webViewQuery ");
                {
                    fastCSharp.code.cSharp.webView _value1_ = default(fastCSharp.code.cSharp.webView);
                    _value1_ = LoadAttribute;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.QueryName);
            }
                }
            _code_.Add(@";");
            }
            _code_.Add(@"
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                if (base.loadView())
                {");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value1_ = LoadAttribute;
                    if (_value1_ != null)
                    {
                    if (_value1_.QueryName != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
                {
                    fastCSharp.code.cSharp.webView _value1_ = default(fastCSharp.code.cSharp.webView);
                    _value1_ = LoadAttribute;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
                    ");
            _code_.Add(_value1_.QueryName);
            }
                }
            _code_.Add(@"= default(webViewQuery);
                    if (ParseParameter(ref ");
                {
                    fastCSharp.code.cSharp.webView _value1_ = default(fastCSharp.code.cSharp.webView);
                    _value1_ = LoadAttribute;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.QueryName);
            }
                }
            _code_.Add(@"))
                    {");
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = QueryMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = ");
                {
                    fastCSharp.code.cSharp.webView _value3_ = default(fastCSharp.code.cSharp.webView);
                    _value3_ = LoadAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.QueryName);
            }
                }
            _code_.Add(@".");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                        return loadView(");
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
                {
                    fastCSharp.code.cSharp.webView _value3_ = default(fastCSharp.code.cSharp.webView);
                    _value3_ = LoadAttribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value3_.QueryName);
            }
                }
            _code_.Add(@".");
            _code_.Add(_value2_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@");
                    }");
            }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value1_ = LoadAttribute;
                    if (_value1_ != null)
                    {
                if (_value1_.QueryName == null)
                {
                    _if_ = true;
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"
                    webViewQuery query = new webViewQuery();
                    if (ParseParameter(ref query))
                    {");
                {
                    fastCSharp.code.memberInfo[] _value1_;
                    _value1_ = QueryMembers;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.memberInfo _value2_ in _value1_)
                        {
            _code_.Add(@"
                        ");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@" = query.");
            _code_.Add(_value2_.MemberName);
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                        return loadView(");
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _code_.Add(@"query.");
            _code_.Add(_value2_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@");
                    }");
            }
            _code_.Add(@"
                }
                return false;
            }");
            }
            _code_.Add(@"
        }");
            }
            _code_.Add(@"
");
            _if_ = false;
                    if (IsServer)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<");
                {
                    fastCSharp.code.memberType _value1_ = SessionType;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.FullName);
                    }
                }
            _code_.Add(@">
        {
            
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            protected override keyValue<string[], string[]> rewrites
            {
                get
                {
                    int count = ");
                {
                    fastCSharp.code.webView.cSharp.viewType[] _value1_ = Views;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.Length.ToString());
                    }
                }
            _code_.Add(@" + ");
            _code_.Add(RewriteViewCount.ToString());
            _code_.Add(@" * 2;
                    string[] names = new string[count];
                    string[] views = new string[count];");
                {
                    fastCSharp.code.webView.cSharp.viewType[] _value1_;
                    _value1_ = Views;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webView.cSharp.viewType _value2_ in _value1_)
                        {
            _code_.Add(@"
                    names[--count] = """);
            _code_.Add(_value2_.RewritePath);
            _code_.Add(@""";
                    views[count] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.RewritePath != null)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    names[--count] = @""");
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.RewritePath);
                    }
                }
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsRewriteHtml)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@".html");
            }
            _code_.Add(@""";
                    views[count] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";
                    names[--count] = @""");
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.RewritePath);
                    }
                }
            _code_.Add(@".js"";
                    views[count] = """);
            _code_.Add(_value2_.RewriteJs);
            _code_.Add(@""";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return new keyValue<string[], string[]>(names, views);
                }
            }");
            _if_ = false;
                    if (ViewPageCount != 0)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            protected override string[] viewRewrites
            {
                get
                {
                    string[] names = new string[");
            _code_.Add(ViewPageCount.ToString());
            _code_.Add(@"];");
                {
                    fastCSharp.code.webView.cSharp.viewType[] _value1_;
                    _value1_ = Views;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webView.cSharp.viewType _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPage)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    names[");
            _code_.Add(_value2_.PageIndex.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.RewritePath);
            _code_.Add(@""";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
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
                    string[] names = new string[");
            _code_.Add(ViewPageCount.ToString());
            _code_.Add(@"];");
                {
                    fastCSharp.code.webView.cSharp.viewType[] _value1_;
                    _value1_ = Views;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webView.cSharp.viewType _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPage)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    names[");
            _code_.Add(_value2_.PageIndex.ToString());
            _code_.Add(@"] = """);
            _code_.Add(_value2_.CallName);
            _code_.Add(@""";");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                    return names;
                }
            }
            /// <summary>
            /// 视图页面处理
            /// </summary>
            /// <param name=""viewIndex""></param>
            /// <param name=""socket""></param>
            /// <param name=""socketIdentity""></param>
            protected override void request(int viewIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (viewIndex)
                {");
                {
                    fastCSharp.code.webView.cSharp.viewType[] _value1_;
                    _value1_ = Views;
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.webView.cSharp.viewType _value2_ in _value1_)
                        {
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPage)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"
                    case ");
            _code_.Add(_value2_.PageIndex.ToString());
            _code_.Add(@": load(socket, socketIdentity, ");
            _if_ = false;
                {
                    fastCSharp.code.cSharp.webView _value3_ = _value2_.Attribute;
                    if (_value3_ != null)
                    {
                    if (_value3_.IsPool)
                    {
                        _if_ = true;
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"fastCSharp.typePool<");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@">.Pop() ?? ");
            }
            _code_.Add(@"new ");
                {
                    fastCSharp.code.memberType _value3_ = _value2_.WebViewMethodType;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.FullName);
                    }
                }
            _code_.Add(@"()");
                {
                    fastCSharp.code.cSharp.webView _value3_ = default(fastCSharp.code.cSharp.webView);
                    _value3_ = _value2_.Attribute;
            _if_ = false;
                    if (_value3_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@", ");
            _code_.Add(_value3_.IsPool ? "true" : "false");
            }
                }
            _code_.Add(@"); return;");
            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
                }
            }");
            }
            _code_.Add(@"
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new ");
                {
                    fastCSharp.code.auto.parameter _value1_ = default(fastCSharp.code.auto.parameter);
                    _value1_ = AutoParameter;
            _if_ = false;
                    if (_value1_ != null)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(_value1_.DefaultNamespace);
            }
                }
            _code_.Add(@".webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }");
            }
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.code
{
    internal partial class webView
    {
    internal partial class ts
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.TypeScript, _isOut_))
            {
                
            _code_.Add(@"		static ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
            _code_.Add(_value2_.Name);
                    }
                }
                    }
                }
            _code_.Add(@"(");
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _code_.Add(_value2_.ParameterName);
            _code_.Add(@",");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"Callback = null) {
			fastCSharp.Pub.GetAjaxGet()('");
                {
                    fastCSharp.code.webView.cSharp.viewType _value1_ = View;
                    if (_value1_ != null)
                    {
            _code_.Add(_value1_.CallName);
                    }
                }
            _code_.Add(@"',");
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value1_ = LoadMethod;
                    if (_value1_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value2_ = _value1_.Parameters;
                    if (_value2_ != null)
                    {
                    if (_value2_.Length != 0)
                    {
                        _if_ = true;
                    }
                }
                    }
                }
                }
            if (_if_)
            {
            _code_.Add(@"{");
                {
                    fastCSharp.code.parameterInfo[] _value1_ = default(fastCSharp.code.parameterInfo[]);
                {
                    fastCSharp.code.methodInfo _value2_ = LoadMethod;
                    if (_value2_ != null)
                    {
                    _value1_ = _value2_.Parameters;
                    }
                }
                    if (_value1_ != null)
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.Length;
                        foreach (fastCSharp.code.parameterInfo _value2_ in _value1_)
                        {
            _code_.Add(_value2_.ParameterName);
            _code_.Add(@": ");
            _code_.Add(_value2_.ParameterJoinName);
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@" }");
            }
            _if_ = false;
                {
                    fastCSharp.code.methodInfo _value1_ = LoadMethod;
                    if (_value1_ != null)
                    {
                {
                    fastCSharp.code.parameterInfo[] _value2_ = _value1_.Parameters;
                    if (_value2_ != null)
                    {
                if (_value2_.Length == 0)
                {
                    _if_ = true;
                }
                    }
                }
                    }
                }
            if (_if_)
            {
            _code_.Add(@"null");
            }
            _code_.Add(@", Callback);	
		}");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.ui.code.cSharp
{
    internal partial class webPath
    {
    internal partial class js
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.JavaScript, _isOut_))
            {
                
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
            _code_.Add(_value2_.Name);
                    }
                }
                    }
                }
            _code_.Add(@":function(Id)
	{");
                {
                    fastCSharp.subArray<fastCSharp.ui.code.cSharp.webPath.pathMember> _value1_;
                    _value1_ = pathMembers;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.ui.code.cSharp.webPath.pathMember _value2_ in _value1_)
                        {
            _code_.Add(@"
	this.");
                {
                    fastCSharp.code.memberInfo _value3_ = _value2_.Member;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MemberName);
                    }
                }
            _code_.Add(@"='");
            _code_.Add(_value2_.Path);
            _if_ = false;
                    if (_value2_.IsIdentity)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsHash)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"#!");
            }
            _code_.Add(_value2_.OtherQuery);
            _code_.Add(_value2_.QueryName);
            _code_.Add(@"=");
            }
            _code_.Add(@"'");
            _if_ = false;
                    if (_value2_.IsIdentity)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"+Id");
            }
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
	},
");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
namespace fastCSharp.ui.code.cSharp
{
    internal partial class webPath
    {
    internal partial class ts
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.TypeScript, _isOut_))
            {
                
            _code_.Add(@"    export class ");
                {
                    fastCSharp.code.memberType _value1_ = type;
                    if (_value1_ != null)
                    {
                {
                    System.Type _value2_ = _value1_.Type;
                    if (_value2_ != null)
                    {
            _code_.Add(_value2_.Name);
                    }
                }
                    }
                }
            _code_.Add(@" {");
                {
                    fastCSharp.subArray<fastCSharp.ui.code.cSharp.webPath.pathMember> _value1_;
                    _value1_ = pathMembers;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.ui.code.cSharp.webPath.pathMember _value2_ in _value1_)
                        {
            _code_.Add(@"
        ");
                {
                    fastCSharp.code.memberInfo _value3_ = _value2_.Member;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MemberName);
                    }
                }
            _code_.Add(@": string;");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
        constructor(Id: number) {");
                {
                    fastCSharp.subArray<fastCSharp.ui.code.cSharp.webPath.pathMember> _value1_;
                    _value1_ = pathMembers;
                    {
                        int _loopIndex1_ = _loopIndex_, _loopCount1_ = _loopCount_;
                        _loopIndex_ = 0;
                        _loopCount_ = _value1_.count();
                        foreach (fastCSharp.ui.code.cSharp.webPath.pathMember _value2_ in _value1_)
                        {
            _code_.Add(@"
            this.");
                {
                    fastCSharp.code.memberInfo _value3_ = _value2_.Member;
                    if (_value3_ != null)
                    {
            _code_.Add(_value3_.MemberName);
                    }
                }
            _code_.Add(@" = '");
            _code_.Add(_value2_.Path);
            _if_ = false;
                    if (_value2_.IsIdentity)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _if_ = false;
                    if (_value2_.IsHash)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@"#!");
            }
            _code_.Add(_value2_.OtherQuery);
            _code_.Add(_value2_.QueryName);
            _code_.Add(@"=");
            }
            _code_.Add(@"'");
            _if_ = false;
                    if (_value2_.IsIdentity)
                    {
                        _if_ = true;
                }
            if (_if_)
            {
            _code_.Add(@" + Id");
            }
            _code_.Add(@";");
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                        _loopCount_ = _loopCount1_;
                    }
                }
            _code_.Add(@"
        }
    }
");
                if (_isOut_) outEnd();
            }
        }
    }
    }
}
#endif