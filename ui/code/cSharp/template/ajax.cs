using System;
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class ajax : pub
    {
        #region PART CLASS
        /// <summary>
        /// AJAX函数调用
        /// </summary>
        #region NOTE
        [fastCSharp.code.ignore]
        #endregion NOTE
        [fastCSharp.code.cSharp.webCall(IsPool = true)]
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public sealed class ajaxLoader : fastCSharp.code.cSharp.ajax.loader<ajaxLoader>
        {
            #region NOTE
            //static pub.FullName[] Methods = null;
            //static pub.FullName[] ViewMethods = null;
            const int MaxPostDataSize = 0;
            const int MaxMemoryStreamSize = 0;
            const int MethodCount = 0;
            const int MethodIndex = 0;
            const bool IsAjaxOnlyPost = false;
            const string AjaxName = null;
            const bool IsPool = false;
            const bool IsPubError = false;
            const bool IsOnlyPost = false;
            #endregion NOTE
            /// <summary>
            /// AJAX函数调用集合
            /// </summary>
            private static readonly fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call> methods;
            /// <summary>
            /// AJAX调用
            /// </summary>
            #region NOTE
            [fastCSharp.code.ignore]
            #endregion NOTE
            [fastCSharp.code.cSharp.webCall(FullName = "@AjaxName")]
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
                    #region LOOP Methods
                    case @MethodIndex:
                        {
                            @WebViewMethodType.FullName view = /*IF:TypeAttribute.IsPool*/fastCSharp.typePool<@WebViewMethodType.FullName>.Pop() ?? /*IF:TypeAttribute.IsPool*/new @WebViewMethodType.FullName();
                            #region IF IsParameter
                            @ParameterTypeName parameter = new @ParameterTypeName();
                            #endregion IF IsParameter
                            fastCSharp.net.tcp.http.response response = loader.Load(view/*IF:IsInputParameter*/, ref parameter/*IF:IsInputParameter*//*PUSH:TypeAttribute*/, @IsPool/*PUSH:TypeAttribute*/);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                #region IF IsAsynchronousCallback
                                @AsyncIndexName callback = typePool<@AsyncIndexName>.Pop() ?? new @AsyncIndexName();
                                #region IF IsParameter
                                callback.Parameter = parameter;
                                #endregion IF IsParameter
                                view./*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*//*AT:ParameterRef*/parameter.@ParameterName, /*LOOP:MethodParameters*/callback.Get(view, response));
                                #endregion IF IsAsynchronousCallback
                                #region NOT IsAsynchronousCallback
                                try
                                {
                                    /*IF:MethodIsReturn*/
                                    parameter.@ReturnName = /*NOTE*/(@MethodReturnType.GenericParameterType.FullName)/*NOTE*//*IF:MethodIsReturn*/ view./*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*//*AT:ParameterRef*/parameter.@ParameterName/*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                                }
                                finally
                                {
                                    #region IF IsOutputParameter
                                    if (responseIdentity == view.ResponseIdentity) view.AjaxResponse(ref parameter, ref response);
                                    else view.AjaxResponse(ref response);
                                    #endregion IF IsOutputParameter
                                    #region NOT IsOutputParameter
                                    view.AjaxResponse(ref response);
                                    #endregion NOT IsOutputParameter
                                }
                                #endregion NOT IsAsynchronousCallback
                            }
                        }
                        return;
                    #endregion LOOP Methods
                    #region LOOP ViewMethods
                    case @MethodIndex/*NOTE*/+ 1/*NOTE*/: loader.LoadView(/*IF:Attribute.IsPool*/fastCSharp.typePool<@WebViewMethodType.FullName>.Pop() ??/*IF:Attribute.IsPool*/new @WebViewMethodType.FullName()/*PUSH:Attribute*/, @IsPool/*PUSH:Attribute*/); return;
                    #endregion LOOP ViewMethods
                    #region NOT IsPubError
                    case @MethodCount - 1: pubError(loader); return;
                    #endregion NOT IsPubError
                }
            }
            #region LOOP Methods
            #region IF IsParameter
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct @ParameterTypeName
            {
                #region LOOP MethodParameters
                #region IF IsRefOrOut
                [fastCSharp.emit.jsonSerialize.member]
                #endregion IF IsRefOrOut
                #region NOT IsOut
                [fastCSharp.emit.jsonParse.member]
                #endregion NOT IsOut
                public @ParameterType.GenericParameterType.FullName @ParameterName;
                #endregion LOOP MethodParameters
                #region IF MethodIsReturn
                [fastCSharp.emit.jsonSerialize.member]
                public @MethodReturnType.GenericParameterType.FullName @ReturnName;
                #endregion IF MethodIsReturn
            }
            #endregion IF IsParameter
            #region IF IsAsynchronousCallback
            sealed class @AsyncIndexName : fastCSharp.code.cSharp.ajax.callback<@AsyncIndexName, @WebViewMethodType.FullName/*IF:MethodIsReturn*/, @MethodReturnType.GenericParameterType.FullName/*IF:MethodIsReturn*/>
            {
                #region IF IsParameter
                public @ParameterTypeName Parameter;
                #endregion IF IsParameter
                #region IF MethodIsReturn
                protected override void onReturnValue(@MethodReturnType.GenericParameterType.FullName value)
                {
                    /*IF:MethodIsReturn*/
                    Parameter.@ReturnName = /*IF:MethodIsReturn*/value;
                    ajax.AjaxResponse(/*IF:IsOutputParameter*/ref Parameter, /*IF:IsOutputParameter*/ref response);
                }
                #endregion IF MethodIsReturn
            }
            #endregion IF IsAsynchronousCallback
            #endregion LOOP Methods
            static ajaxLoader()
            {
                string[] names = new string[@MethodCount];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[@MethodCount];
                #region LOOP Methods
                names[@MethodIndex] = "@CallName";
                callMethods[@MethodIndex] = new fastCSharp.code.cSharp.ajax.call(@MethodIndex, @MaxPostDataSize, @MaxMemoryStreamSize, false/*PUSH:DefaultAttribute*/, @IsOnlyPost/*NOT:IsReferer*/, false/*NOT:IsReferer*//*PUSH:DefaultAttribute*/);
                #endregion LOOP Methods
                #region LOOP ViewMethods
                names[@MethodIndex] = "@CallName";
                #region IF Attribute.IsPool
                callMethods[@MethodIndex] = new fastCSharp.code.cSharp.ajax.call(@MethodIndex, @MaxPostDataSize, @MaxMemoryStreamSize, true, false/*NOT:Attribute.IsReferer*/, false/*NOT:Attribute.IsReferer*/);
                #endregion IF Attribute.IsPool
                #region NOT Attribute.IsPool
                callMethods[@MethodIndex] = new fastCSharp.code.cSharp.ajax.call(@MethodIndex, @MaxPostDataSize, @MaxMemoryStreamSize, true, false/*NOT:Attribute.IsReferer*/, false/*NOT:Attribute.IsReferer*/);
                #endregion NOT Attribute.IsPool
                #endregion LOOP ViewMethods
                #region NOT IsPubError
                names[@MethodCount - 1/*NOTE*/+ 1/*NOTE*/] = fastCSharp.code.cSharp.ajax.PubErrorCallName/*IF:AutoParameter.WebConfig.IgnoreCase*/.ToLower()/*IF:AutoParameter.WebConfig.IgnoreCase*/;
                callMethods[@MethodCount - 1/*NOTE*/+ 1/*NOTE*/] = new fastCSharp.code.cSharp.ajax.call(@MethodCount - 1, 2048, 0, false, false);
                #endregion NOT IsPubError
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }
        #endregion PART CLASS
    }
    #region NOTE
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 类型全名
        /// </summary>
        public partial class FullName
        {
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <typeparam name="valueType">输出数据类型</typeparam>
            /// <param name="value">输出数据</param>
            public void AjaxResponse<valueType>(ref valueType value) where valueType : struct { }
        }
        /// <summary>
        /// WEB页面视图
        /// </summary>
        public class page : cSharp.webView.view<page>, cSharp.webView.IWebView
        {
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView() { return false; }
        }
    }
    #endregion NOTE
}
