using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using fastCSharp.web;
using fastCSharp.net.tcp.http;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Net;
using System.Threading;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// WEB视图配置
    /// </summary>
    internal sealed partial class webView
    {
        /// <summary>
        /// HTML模板解析
        /// </summary>
        internal class template : template<fastCSharp.code.cSharp.webView.treeBuilder.node>
        {
            /// <summary>
            /// AJAX引号替换
            /// </summary>
            private static readonly Regex ajaxQuoteRegex = new Regex(@"(js\.WriteNotNull\(@"".?""\);)", RegexOptions.Compiled);
            /// <summary>
            /// 视图AJAX成员节点有序名称排序
            /// </summary>
            private static readonly Func<keyValue<string, memberNode>, keyValue<string, memberNode>, int> ajaxNameSortHandle = ajaxNameSort;
            /// <summary>
            /// HTML模板建树器
            /// </summary>
            private fastCSharp.code.cSharp.webView.treeBuilder treeBuilder;
            /// <summary>
            /// HTML片段数量
            /// </summary>
            internal int HtmlCount
            {
                get { return treeBuilder.HtmlCount; }
            }
            /// <summary>
            /// 集合是否支持length属性
            /// </summary>
            protected override bool isCollectionLength { get { return true; } }
            /// <summary>
            /// 视图查询参数名称
            /// </summary>
            private string viewQueryName;
            /// <summary>
            /// HTML模板解析
            /// </summary>
            /// <param name="type">模板关联视图类型</param>
            /// <param name="html">HTML模板</param>
            public template(Type type, string html)
                : base(type, error.Add, error.Message)
            {
                creators[fastCSharp.code.cSharp.webView.command.Note.ToString()] = creators[fastCSharp.code.cSharp.webView.command.Client.ToString()] = note;
                creators[fastCSharp.code.cSharp.webView.command.Loop.ToString()] = creators[fastCSharp.code.cSharp.webView.command.For.ToString()] = loop;
                creators[fastCSharp.code.cSharp.webView.command.At.ToString()] = at;
                creators[fastCSharp.code.cSharp.webView.command.Value.ToString()] = push;
                creators[fastCSharp.code.cSharp.webView.command.If.ToString()] = ifThen;
                creators[fastCSharp.code.cSharp.webView.command.Not.ToString()] = not;
                keyValue<fastCSharp.code.methodInfo, fastCSharp.code.cSharp.webView> loadMethod = fastCSharp.code.cSharp.webView.GetLoadMethod(type);
                if (loadMethod.Value != null && type.GetField(loadMethod.Value.QueryName, BindingFlags.Instance | BindingFlags.NonPublic) == null)
                {
                    viewQueryName = loadMethod.Value.QueryName + ".";
                }
                skin((treeBuilder = new fastCSharp.code.cSharp.webView.treeBuilder(html, 0)).Boot);
            }
            /// <summary>
            /// 检测错误成员名称
            /// </summary>
            /// <param name="memberName">成员名称</param>
            /// <returns>是否忽略错误</returns>
            protected override bool checkErrorMemberName(string memberName)
            {
                return viewQueryName != null && memberName.StartsWith(viewQueryName, StringComparison.Ordinal);
            }
            /// <summary>
            /// 添加代码
            /// </summary>
            /// <param name="code">代码</param>
            protected override void pushCode(string code)
            {
                if (code != null && ignoreCode == 0)
                {
                    this.code.Append(@"
                    response(htmls[", treeBuilder.GetHtmlIndex(code).toString(), "]);");
                }
            }
            /// <summary>
            /// 检测成员名称
            /// </summary>
            /// <param name="memberName"></param>
            /// <returns></returns>
            protected override string checkMemberName(string memberName, ref bool isClient)
            {
                int index = memberName.IndexOf('#');
                if (index == -1) return memberName;
                isClient = true;
                return memberName.Substring(0, index);
            }
            /// <summary>
            /// 输出绑定的数据
            /// </summary>
            /// <param name="node">代码树节点</param>
            protected override void at(fastCSharp.code.cSharp.webView.treeBuilder.node node)
            {
                bool isToHtml = false, isToTextArea = false, isClient = false, isDepth;
                string memberName = node.TemplateMemberName;
                if (memberName.Length != 0)
                {
                    if (memberName[0] == '$') return;
                    if (memberName[0] == '@')
                    {
                        memberName = memberName.Substring(1);
                        isToHtml = true;
                    }
                    else if (memberName[0] == '*')
                    {
                        memberName = memberName.Substring(1);
                        isToTextArea = true;
                    }
                    int clientIndex = memberName.IndexOf('#');
                    if (clientIndex != -1)
                    {
                        memberName = memberName.Substring(0, clientIndex);
                        isClient = true;
                    }
                }
                memberNode member = null;
                if (memberName.Length == 0) isDepth = false;
                else member = getMember(memberName, out isDepth);
                if (member != null && !isClient) ifThen(member, memberName, isDepth, value => at(value, isToHtml, isToTextArea));
            }
            /// <summary>
            /// 输出绑定的数据
            /// </summary>
            /// <param name="member">成员节点</param>
            protected void at(memberNode member, bool isToHtml, bool isToTextArea)
            {
                if (ignoreCode == 0)
                {
                    string call = "response";
                    if (isToHtml) call = "responseHtml";
                    if (isToTextArea) call = "responseTextArea";
                    if (member.Type.IsString || member.Type.IsSubString || member.Type.IsHashUrl || member.Type.IsChar)
                    {
                        code.Append(@"
                        ", call, "(", member.Path, ");");
                    }
                    else
                    {
                        code.Append(@"
                        ", call, "(", member.Path, ".ToString());");
                    }
                }
            }
            /// <summary>
            /// 视图AJAX代码
            /// </summary>
            private charStream ajaxCode;
            /// <summary>
            /// 视图AJAX输出
            /// </summary>
            /// <returns>AJAX字符串</returns>
            public string Ajax()
            {
                using (ajaxCode = new charStream())
                {
                    ajaxCode.WriteNotNull(@"js.WriteNotNull(@""");
                    ajax(currentMembers[0], null);
                    ajaxCode.WriteNotNull(@""");");
                    return ajaxQuoteRegex.Replace(ajaxCode.ToString(), ajaxQuote);
                }
            }
            /// <summary>
            /// 获取忽略输出变量名称
            /// </summary>
            /// <param name="index">忽略输出变量层次</param>
            /// <returns>忽略输出变量名称</returns>
            private string memberIgnore(int index)
            {
                return "_memberIgnore" + (index == 0 ? (currentMembers.Count - 1) : index).ToString() + "_";
            }
            /// <summary>
            /// 视图AJAX成员节点代码
            /// </summary>
            /// <param name="node">成员节点</param>
            /// <param name="parentPath">父节点路径</param>
            private void ajax(memberNode node, string parentPath)
            {
                keyValue<string, memberNode>[] members = ajaxName(node);
                keyValue<string, memberNode> loopMember = default(KeyValuePair<string, memberNode>);
                foreach (keyValue<string, memberNode> member in members)
                {
                    if (member.Key.Length == 0)
                    {
                        loopMember = member;
                        break;
                    }
                }
                if (loopMember.Key != null)
                {
                    bool isDepth;
                    memberNode member = getMember(string.Empty, out isDepth);
                    fastCSharp.code.memberType type = member.Type.EnumerableArgumentType;
                    ajaxCode.Write(@"["");
                    {
                        int ", loopIndex(0), @" = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (", type.FullName, " ", path(currentMembers.Count) + " in ", (parentPath ?? member.Path), @")
                        {");
                    if (loopMember.Value.IsNextPath)
                    {
                        ajaxCode.WriteNotNull(@"
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull(""");
                        ajaxLoopNames(loopMember.Value);
                        ajaxCode.WriteNotNull(@""");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');");
                        if (type.IsNull)
                        {
                            ajaxCode.Write(@"
                            if (", path(currentMembers.Count), @" == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {");
                        }
                        ajaxCode.WriteNotNull(@"
                                js.WriteNotNull(@""[");
                        pushMember(member.Get(string.Empty));
                        ajaxLoop(loopMember.Value);
                        currentMembers.Pop();
                        ajaxCode.WriteNotNull(@"]"");");
                        if (type.IsNull)
                        {
                            ajaxCode.WriteNotNull(@"
                            }");
                        }
                        ajaxCode.Write(@"
                            ++_loopIndex_;
                        }
                        _loopIndex_ = ", loopIndex(0), @";
                    }
                    js.WriteNotNull(@""]", fastCSharp.web.ajax.FormatView);
                    }
                    else
                    {
                        ajaxCode.WriteNotNull(@"
                            if (_loopIndex_ != 0) js.Write(',');");
                        if (type.IsNull)
                        {
                            ajaxCode.Write(@"
                                if (", path(currentMembers.Count), @" == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {");
                        }
                        ajaxValue(type, path(currentMembers.Count));
                        if (type.IsNull)
                        {
                            ajaxCode.WriteNotNull(@"
                                }");
                        }
                        ajaxCode.Write(@"
                            ++_loopIndex_;
                        }
                        _loopIndex_ = ", loopIndex(0), @";
                    }
                    js.WriteNotNull(@""]");
                    }
                }
                else
                {
                    int memberIndex = 0;
                    bool isDepth;
                    string clientTypeName = node.Type.ClientViewTypeName, clientMemberName = null;
                    if (clientTypeName != null)
                    {
                        clientMemberName = node.Type.ClientViewMemberName;
                        if (clientMemberName == null)
                        {
                            ajaxCode.WriteNotNull("new ");
                            ajaxCode.WriteNotNull(clientTypeName);
                            ajaxCode.Write('(');
                        }
                        else
                        {
                            ajaxCode.WriteNotNull(clientTypeName);
                            ajaxCode.WriteNotNull(".Get(");
                        }
                    }
                    ajaxCode.Write('{');
                    foreach (KeyValuePair<string, memberNode> name in members)
                    {
                        if (!name.Value.IsIgnoreNull)
                        {
                            if (memberIndex++ != 0) ajaxCode.Write(',');
                            ajaxCode.WriteNotNull(name.Key);
                            ajaxCode.Write(':');
                            memberNode member = getMember(name.Key, out isDepth);
                            pushMember(member);
                            ajaxCode.Write(@""");
                    {
                        ", member.Type.FullName, " ", path(0), " = ", member.Path, ";");
                            if (member.Type.IsNull)
                            {
                                ajaxCode.Write(@"
                        if (", path(0), @" == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {");
                            }
                            if (name.Value.IsNextPath)
                            {
                                ajaxCode.WriteNotNull(@"
                            js.WriteNotNull(@""");
                                ajax(name.Value, path(0));
                                ajaxCode.WriteNotNull(@""");");
                            }
                            else ajaxValue(member.Type, path(0));
                            if (member.Type.IsNull)
                            {
                                ajaxCode.WriteNotNull(@"
                        }");
                            }
                            ajaxCode.WriteNotNull(@"
                    }
                    js.WriteNotNull(@""");
                            currentMembers.Pop();
                        }
                    }
                    if (memberIndex == 0)
                    {
                        string memberIgnoreName = memberIgnore(0);
                        ajaxCode.Write(@""");
                    bool ", memberIgnoreName, @" = false;");
                        foreach (KeyValuePair<string, memberNode> name in members)
                        {
                            if (name.Value.IsIgnoreNull)
                            {
                                memberNode member = getMember(name.Key, out isDepth);
                                pushMember(member);
                                ajaxCode.Write(@"
                    {
                        ", member.Type.FullName, " ", path(0), " = ", member.Path, ";");
                                if (member.Type.IsNull)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @" != null)
                        {");
                                }
                                else if (member.Type.IsNumber)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @" != 0)
                        {");
                                }
                                else if (member.Type.IsBool)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @")
                        {");
                                }
                                ajaxCode.Write(@"
                            if (", memberIgnoreName, @") js.Write(',');
                            else ", memberIgnoreName, @" = true;
                            js.WriteNotNull(@""");
                                ajaxCode.WriteNotNull(name.Key);
                                ajaxCode.Write(':');
                                if (name.Value.IsNextPath)
                                {
                                    ajax(name.Value, path(0));
                                    ajaxCode.WriteNotNull(@""");");
                                }
                                else
                                {
                                    ajaxCode.WriteNotNull(@""");");
                                    ajaxValue(member.Type, path(0));
                                }
                                if (member.Type.IsNull || member.Type.IsNumber || member.Type.IsBool)
                                {
                                    ajaxCode.WriteNotNull(@"
                        }");
                                }
                                ajaxCode.WriteNotNull(@"
                    }");
                                currentMembers.Pop();
                            }
                        }
                        ajaxCode.WriteNotNull(@"
                    js.WriteNotNull(@""");
                    }
                    else
                    {
                        foreach (KeyValuePair<string, memberNode> name in members)
                        {
                            if (name.Value.IsIgnoreNull)
                            {
                                memberNode member = getMember(name.Key, out isDepth);
                                pushMember(member);
                                ajaxCode.Write(@""");
                    {
                        ", member.Type.FullName, " ", path(0), " = ", member.Path, ";");
                                if (member.Type.IsNull)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @" != null)
                        {");
                                }
                                else if (member.Type.IsNumber)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @" != 0)
                        {");
                                }
                                else if (member.Type.IsBool)
                                {
                                    ajaxCode.Write(@"
                        if (", path(0), @")
                        {");
                                }
                                ajaxCode.WriteNotNull(@"
                            js.WriteNotNull(@"",");
                                ajaxCode.WriteNotNull(name.Key);
                                ajaxCode.Write(':');
                                if (name.Value.IsNextPath)
                                {
                                    ajax(name.Value, path(0));
                                    ajaxCode.WriteNotNull(@""");");
                                }
                                else
                                {
                                    ajaxCode.WriteNotNull(@""");");
                                    ajaxValue(member.Type, path(0));
                                }
                                if (member.Type.IsNull || member.Type.IsNumber || member.Type.IsBool)
                                {
                                    ajaxCode.WriteNotNull(@"
                        }");
                                }
                                ajaxCode.WriteNotNull(@"
                    }
                    js.WriteNotNull(@""");
                                currentMembers.Pop();
                            }
                        }
                    }
                    ajaxCode.Write('}');
                    if (clientTypeName != null) ajaxCode.Write(')');
                }
            }
            /// <summary>
            /// 视图AJAX成员节点有序名称集合
            /// </summary>
            /// <param name="node">成员节点</param>
            /// <returns>视图AJAX成员节点有序名称集合</returns>
            private keyValue<string, memberNode>[] ajaxName(memberNode node)
            {
                Dictionary<string, memberNode> members;
                return memberPaths.TryGetValue(node, out members) ? members.getArray(value => new keyValue<string, memberNode>(value.Key, value.Value)).sort(ajaxNameSortHandle) : nullValue<keyValue<string, memberNode>>.Array;
            }
            /// <summary>
            /// 视图AJAX叶子成员节点代码
            /// </summary>
            /// <param name="type">成员类型</param>
            /// <param name="name">成员名称</param>
            private void ajaxValue(fastCSharp.code.memberType type, string name)
            {
                if (type.IsAjaxToString || type.IsDateTime)
                {
                    ajaxCode.Write(@"
                                    fastCSharp.web.ajax.ToString((", type.NotNullType.FullName, ")", name, ", js);");
                }
                else if (type.IsString || type.IsSubString)
                {
                    ajaxCode.Write(@"
                                    fastCSharp.web.ajax.ToString(", name, ", js);");
                }
                else if (type.Type.IsEnum)
                {
                    ajaxCode.Write(@"
                                    fastCSharp.web.ajax.ToString(", name, @".ToString(), js);");
                }
                else if (type.Type == typeof(fastCSharp.code.cSharp.webView.hashUrl))
                {
                    ajaxCode.Write(@"
                                    ", name, @".ToJson(js);");
                }
                else
                {
                    string clientTypeName = type.ClientViewTypeName, clientMemberName = null;
                    if (clientTypeName != null)
                    {
                        ajaxCode.WriteNotNull(@"
                                    js.WriteNotNull(@""");
                        clientMemberName = type.ClientViewMemberName;
                        if (clientMemberName == null)
                        {
                            ajaxCode.WriteNotNull("new ");
                            ajaxCode.WriteNotNull(clientTypeName);
                            ajaxCode.Write('(');
                        }
                        else
                        {
                            ajaxCode.WriteNotNull(clientTypeName);
                            ajaxCode.WriteNotNull(".Get(");
                        }
                        ajaxCode.Write(@"{})"");");
                    }
                    else
                    {
                        ajaxCode.WriteNotNull(@"
                                    fastCSharp.web.ajax.WriteObject(js);");
                    }
                }
            }
            /// <summary>
            /// 视图AJAX循环成员节点名称
            /// </summary>
            /// <param name="node">成员节点</param>
            private void ajaxLoopNames(memberNode node)
            {
                keyValue<string, memberNode>[] members = ajaxName(node);
                foreach (keyValue<string, memberNode> member in members)
                {
                    if (member.Key.Length == 0)
                    {
                        ajaxCode.Write('[');
                        if (member.Value.IsNextPath) ajaxLoopNames(member.Value);
                        ajaxCode.Write(']');
                        return;
                    }
                }
                int memberIndex = 0;
                string clientTypeName = node.Type.ClientViewTypeName;
                if (clientTypeName != null)
                {
                    memberIndex = 1;
                    ajaxCode.Write(fastCSharp.web.ajax.ViewClientType);
                    if (node.Type.ClientViewMemberName != null) ajaxCode.Write(fastCSharp.web.ajax.ViewClientMember);
                    ajaxCode.WriteNotNull(clientTypeName);
                    ajaxCode.Write(',');
                }
                foreach (keyValue<string, memberNode> name in members)
                {
                    if (memberIndex != 0) ajaxCode.Write(',');
                    ajaxCode.WriteNotNull(name.Key);
                    if (name.Value.IsNextPath)
                    {
                        ajaxCode.Write('[');
                        ajaxLoopNames(name.Value);
                        ajaxCode.Write(']');
                        memberIndex = 0;
                    }
                    else memberIndex = 1;
                }
            }
            /// <summary>
            /// 视图AJAX循环成员节点数据
            /// </summary>
            /// <param name="node">成员节点</param>
            private void ajaxLoop(memberNode node)
            {
                keyValue<string, memberNode>[] members = ajaxName(node);
                keyValue<string, memberNode> loopMember = default(KeyValuePair<string, memberNode>);
                foreach (keyValue<string, memberNode> member in members)
                {
                    if (member.Key.Length == 0)
                    {
                        loopMember = member;
                        break;
                    }
                }
                if (loopMember.Key != null)
                {
                    string parentPath = path(0);
                    bool isDepth;
                    memberNode member = getMember(string.Empty, out isDepth);
                    System.Type type = member.Type.EnumerableArgumentType;
                    //if (memberIndex != 0) ajaxCode.Write(',');
                    ajaxCode.Write(@"["");
                    {
                        int ", loopIndex(0).ToString(), @" = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (", type.fullName(), " " + path(currentMembers.Count) + " in ", (parentPath ?? member.Path), @")
                        {
                            if (_loopIndex_ != 0) js.Write(',');");
                    if (type.isNull())
                    {
                        ajaxCode.Write(@"
                            if (", path(currentMembers.Count), @" == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {");
                    }
                    if (loopMember.Value.IsNextPath)
                    {
                        ajaxCode.WriteNotNull(@"
                                js.Write('[');");
                        pushMember(member.Get(string.Empty));
                        ajaxCode.WriteNotNull(@"
                                js.WriteNotNull(@""");
                        ajaxLoop(loopMember.Value);
                        currentMembers.Pop();
                        ajaxCode.WriteNotNull(@"]"");");
                    }
                    else ajaxValue(type, path(currentMembers.Count));
                    if (type.isNull())
                    {
                        ajaxCode.WriteNotNull(@"
                            }");
                    }
                    ajaxCode.Write(@"
                            ++_loopIndex_;
                        }
                        _loopIndex_ = ", loopIndex(0), @";
                    }
                    js.WriteNotNull(@""]");
                }
                else
                {
                    int memberIndex = 0;
                    bool isDepth;
                    foreach (KeyValuePair<string, memberNode> name in members)
                    {
                        if (memberIndex++ != 0) ajaxCode.Write(',');
                        memberNode member = getMember(name.Key, out isDepth);
                        pushMember(member);
                        ajaxCode.Write(@""");
                    {
                        ", member.Type.FullName, " ", path(0), " = ", member.Path, ";");
                        if (member.Type.IsNull)
                        {
                            ajaxCode.Write(@"
                                if (", path(0), @" == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {");
                        }
                        if (name.Value.IsNextPath)
                        {
                            ajaxCode.WriteNotNull(@"
                                    js.WriteNotNull(@""[");
                            ajaxLoop(name.Value);
                            ajaxCode.WriteNotNull(@"]"");");
                        }
                        else ajaxValue(member.Type, path(0));
                        if (member.Type.IsNull)
                        {
                            ajaxCode.WriteNotNull(@"
                                }");
                        }
                        ajaxCode.WriteNotNull(@"
                    }
                    js.WriteNotNull(@""");
                        currentMembers.Pop();
                    }
                }
            }

            //private string ajaxLoopNames(memberNode node)
            //{
            //    using (charStream nameBuilder = new charStream())
            //    {
            //        ajaxLoopNames(node, nameBuilder);
            //        return nameBuilder.ToString();
            //    }
            //}
            //private void ajaxLoopNames(memberNode path, charStream nameBuilder)
            //{
            //    KeyValuePair<string, memberNode>[] members = ajaxName(path);
            //    KeyValuePair<string, memberNode> loopMember = default(KeyValuePair<string, memberNode>);
            //    foreach (KeyValuePair<string, memberNode> member in members)
            //    {
            //        if (member.Key.Length == 0)
            //        {
            //            loopMember = member;
            //            break;
            //        }
            //    }
            //    if (loopMember.Key != null)
            //    {
            //        nameBuilder.Write('[');
            //        if (loopMember.Value.IsNextPath) ajaxLoopNames(loopMember.Value, nameBuilder);
            //        nameBuilder.Write(']');
            //    }
            //    else
            //    {
            //        int memberIndex = 0;
            //        foreach (KeyValuePair<string, memberNode> name in members)
            //        {
            //            if (memberIndex != 0) nameBuilder.Write(',');
            //            nameBuilder.Write(name.Key);
            //            if (name.Value.IsNextPath)
            //            {
            //                nameBuilder.Write('[');
            //                ajaxLoopNames(name.Value, nameBuilder);
            //                nameBuilder.Write(']');
            //                memberIndex = 0;
            //            }
            //            else memberIndex = 1;
            //        }
            //    }
            //}
            /// <summary>
            /// 视图AJAX成员节点有序名称排序
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            private static int ajaxNameSort(keyValue<string, memberNode> left, keyValue<string, memberNode> right)
            {
                return left.Key.CompareTo(right.Key);
            }
            /// <summary>
            /// AJAX引号替换
            /// </summary>
            private static string ajaxQuote(Match match)
            {
                return match.Length == 22 ? "js.Write('" + match.Value[18] + "');" : string.Empty;
            }
        }
        /// <summary>
        /// WEB代码生成
        /// </summary>
        /// <typeparam name="attributeType"></typeparam>
        internal abstract class cSharp<attributeType> : cSharper<attributeType>, IAuto
            where attributeType : Attribute
        {
            /// <summary>
            /// 是否WEB视图服务代码生成
            /// </summary>
            public bool IsServer;
            /// <summary>
            /// 视图加载函数
            /// </summary>
            public methodInfo LoadMethod;
            /// <summary>
            /// 视图加载函数配置
            /// </summary>
            public attributeType LoadAttribute;
            /// <summary>
            /// Session类型
            /// </summary>
            public memberType SessionType;
            /// <summary>
            /// 安装入口
            /// </summary>
            /// <param name="parameter">安装参数</param>
            /// <returns>是否安装成功</returns>
            public override bool Run(auto.parameter parameter)
            {
                if (parameter.WebConfig == null) return true;
                SessionType = parameter.WebConfig.SessionType;
                return base.Run(parameter);
            }
        }
        /// <summary>
        /// WEB视图代码生成
        /// </summary
        [auto(Name = "WEB视图", DependType = typeof(html), IsAuto = true)]
        internal partial class cSharp : cSharp<fastCSharp.code.cSharp.webView>, IAuto
        {
            /// <summary>
            /// WEB视图 API代码生成
            /// </summary>
            private static readonly ts ts = new ts();
            /// <summary>
            /// WEB视图类型信息
            /// </summary>
            public class viewType
            {
                /// <summary>
                /// WEB视图类型
                /// </summary>
                public memberType type;
                /// <summary>
                /// WEB视图类型
                /// </summary>
                public memberType WebViewMethodType
                {
                    get { return type; }
                }
                /// <summary>
                /// WEB视图配置
                /// </summary>
                public fastCSharp.code.cSharp.webView Attribute;
                /// <summary>
                /// 序号
                /// </summary>
                public int Index;
                /// <summary>
                /// 页面序号
                /// </summary>
                public int PageIndex;
                /// <summary>
                /// 默认程序集名称
                /// </summary>
                public string DefaultNamespace;
                /// <summary>
                /// WEB视图调用类型名称
                /// </summary>
                private string callTypeName
                {
                    get
                    {
                        string callName = type.FullName;
                        if (callName.StartsWith(DefaultNamespace, StringComparison.Ordinal)) callName = callName.Substring(DefaultNamespace.Length - 1);
                        else callName = "/" + callName;
                        return callName.replace('.', '/');
                    }
                }
                /// <summary>
                /// 是否忽略大小写
                /// </summary>
                public bool IgnoreCase;
                /// <summary>
                /// WEB视图调用名称
                /// </summary>
                private string callName;
                /// <summary>
                /// WEB视图调用名称
                /// </summary>
                public string CallName
                {
                    get
                    {
                        if (callName == null)
                        {
                            if ((callName = Attribute.TypeCallName) == null) callName = callTypeName;
                            else if (callName.Length == 0 || callName[0] != '/') callName = "/" + callName;
                            if (Attribute.MethodName != null) callName += "/" + Attribute.MethodName.replace('.', '/');
                            callName += ".html";
                            if (IgnoreCase) callName = callName.toLower();
                        }
                        return callName;
                    }
                }
                /// <summary>
                /// 来源重写js文件重定向
                /// </summary>
                public string RewriteJs
                {
                    get
                    {
                        string path = CallName;
                        return (path.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ? path.Substring(0, path.Length - 5) : path) + ".js";
                    }
                }
                /// <summary>
                /// WEB视图重写路径
                /// </summary>
                public string RewritePath
                {
                    get
                    {
                        //if (Attribute.RewritePath != null) return Attribute.RewritePath;
                        string path = callTypeName;
                        if (path.EndsWith("/index")) path = path.Substring(0, path.Length - 5);
                        return IgnoreCase ? path.toLower() : path;
                    }
                }
            }
            /// <summary>
            /// WEB视图类型集合
            /// </summary>
            private list<viewType> views = new list<viewType>();
            /// <summary>
            /// WEB视图类型集合
            /// </summary>
            public viewType[] Views;
            /// <summary>
            /// WEB视图页面数量
            /// </summary>
            public int ViewPageCount;
            /// <summary>
            /// 来源路径重写数量
            /// </summary>
            public int RewriteViewCount;
            /// <summary>
            /// HTML文件名
            /// </summary>
            public string HtmlFile
            {
                get
                {
                    string value = type.FullName;
                    if (value.StartsWith(AutoParameter.DefaultNamespace, StringComparison.Ordinal) && value[AutoParameter.DefaultNamespace.Length] == '.')
                    {
                        value = value.Substring(AutoParameter.DefaultNamespace.Length + 1).replace('.', Path.DirectorySeparatorChar);
                    }
                    else value = value.Replace('.', Path.DirectorySeparatorChar);
                    return value + ".html";
                }
            }
            /// <summary>
            /// HTML片段数量
            /// </summary>
            public int HtmlCount;
            /// <summary>
            /// 服务器端页面代码
            /// </summary>
            public string PageCode;
            /// <summary>
            /// AJAX输出
            /// </summary>
            public string AjaxCode;
            /// <summary>
            /// 查询成员信息集合
            /// </summary>
            public code.memberInfo[] QueryMembers;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                string fileName = AutoParameter.ProjectPath + HtmlFile;
                if (!File.Exists(fileName)) error.Add("未找到HTML页面文件 " + fileName);
                else
                {
                    keyValue<methodInfo, fastCSharp.code.cSharp.webView> loadMethod = fastCSharp.code.cSharp.webView.GetLoadMethod(type.Type);
                    LoadMethod = loadMethod.Key;
                    LoadAttribute = loadMethod.Value;
                    QueryMembers = code.memberInfo.GetMembers<fastCSharp.code.cSharp.webView.query>(type, memberFilters.Instance, true, true, true);
                    if (QueryMembers.Length != 0 && LoadMethod != null)
                    {
                        foreach (parameterInfo parameter in LoadMethod.Parameters)
                        {
                            if (QueryMembers.any(member => member.MemberName == parameter.ParameterName))
                            {
                                error.Add(type.FullName + " 查询名称冲突 " + parameter.ParameterName);
                                return;
                            }
                        }
                    }
                    try
                    {
                        //template template = new template(type, File.ReadAllText(fileName, fastCSharp.config.appSetting.Encoding));
                        template template = new template(type, File.ReadAllText(fileName, AutoParameter.WebConfig.Encoding));
                        if (Attribute.IsPage) PageCode = template.Code;
                        if (Attribute.IsAjax) AjaxCode = template.Ajax();
                        HtmlCount = template.HtmlCount;
                        viewType view = new viewType { type = type, Attribute = Attribute, DefaultNamespace = AutoParameter.DefaultNamespace + ".", Index = views.Count, IgnoreCase = AutoParameter.WebConfig.IgnoreCase };
                        views.Add(view);
                        IsServer = false;
                        create(true);
                        if (Attribute.IsAjax && Attribute.IsExportTypeScript) ts.Create(AutoParameter, type, view, LoadMethod);
                    }
                    catch (Exception exception)
                    {
                        error.Add(exception);
                        throw new Exception(fileName);
                    }
                }
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
                if (views.Count != 0)
                {
                    Views = views.ToArray();
                    HashSet<string> rewritePaths = new HashSet<string>();
                    ViewPageCount = RewriteViewCount = 0;
                    foreach (viewType view in Views)
                    {
                        if (view.Attribute.IsPage) view.PageIndex = ViewPageCount++;
                        string rewritePath = view.RewritePath;
                        if (rewritePaths.Contains(rewritePath))
                        {
                            error.Add("URL 重写冲突 " + rewritePath);
                            throw new Exception("URL 重写冲突" + rewritePath);
                        }
                        rewritePaths.Add(rewritePath);
                        if (view.Attribute.RewritePath != null)
                        {
                            string path = view.IgnoreCase ? view.Attribute.RewritePath.toLower() : view.Attribute.RewritePath;
                            if (view.Attribute.IsRewriteHtml) rewritePath = path + ".html";
                            //{
                            //    if (path.EndsWith(".html", view.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) path = path.Substring(0, path.Length - 5);
                            //    rewritePath = path + ".html";
                            //}
                            else rewritePath = path;
                            if (rewritePaths.Contains(rewritePath = path))
                            {
                                error.Add("URL 重写冲突 " + rewritePath);
                                throw new Exception("URL 重写冲突" + rewritePath);
                            }
                            rewritePaths.Add(rewritePath);
                            if (rewritePaths.Contains(rewritePath = path + ".js"))
                            {
                                error.Add("URL 重写冲突 " + rewritePath);
                                throw new Exception("URL 重写冲突" + rewritePath);
                            }
                            rewritePaths.Add(rewritePath);
                            ++RewriteViewCount;
                        }
                    }
                    IsServer = true;
                    _code_.Empty();
                    create(false);
                    fastCSharp.code.coder.Add(@"
namespace " + AutoParameter.DefaultNamespace + @"
{
" + _code_.ToString() + @"
}");
                }
            }
        }
        /// <summary>
        /// AJAX API代码生成
        /// </summary>
        [auto(Name = "WEB视图 API", Language = auto.language.TypeScript)]
        internal sealed partial class ts : cSharp
        {
            /// <summary>
            /// 类型
            /// </summary>
            private struct typeKey : IEquatable<typeKey>
            {
                /// <summary>
                /// 命名空间
                /// </summary>
                public string Namespace;
                /// <summary>
                /// 类型名称
                /// </summary>
                public string Name;
                /// <summary>
                /// 哈希值
                /// </summary>
                public int HashCode;
                /// <summary>
                /// 类型
                /// </summary>
                /// <param name="parameter"></param>
                /// <param name="type"></param>
                public typeKey(auto.parameter parameter, memberType type)
                {
                    Namespace = type.Type.Namespace;
                    if (Namespace == parameter.DefaultNamespace)
                    {
                        Namespace = ajax.ts.fastCSharpAPI;
                        Name = "webView";
                    }
                    else
                    {
                        if (Namespace.StartsWith(parameter.DefaultNamespace, StringComparison.Ordinal) && Namespace[parameter.DefaultNamespace.Length] == '.') Namespace = Namespace.Substring(parameter.DefaultNamespace.Length);
                        int index = Namespace.IndexOf('.');
                        if (index == -1)
                        {
                            Name = Namespace;
                            Namespace = ajax.ts.fastCSharpAPI;
                        }
                        else
                        {
                            Name = Namespace.Substring(index + 1);
                            Namespace = ajax.ts.fastCSharpAPI + Namespace.Substring(0, index);
                        }
                    }
                    HashCode = Namespace.GetHashCode() ^ Name.GetHashCode();
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="other"></param>
                /// <returns></returns>
                public bool Equals(typeKey other)
                {
                    return HashCode == other.HashCode && Namespace == other.Namespace && Name == other.Name;
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((typeKey)obj);
                }
                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return HashCode;
                }
            }
            /// <summary>
            /// 代码集合
            /// </summary>
            private static readonly Dictionary<typeKey, stringBuilder> codes = dictionary<typeKey>.Create<stringBuilder>();
            /// <summary>
            /// 类型集合
            /// </summary>
            private static readonly Dictionary<string, typeKey> types = dictionary.CreateOnly<string, typeKey>();
            /// <summary>
            /// API生成
            /// </summary>
            internal static string Code
            {
                get
                {
                    if (codes.Count != 0)
                    {
                        stringBuilder code = new stringBuilder();
                        foreach (KeyValuePair<typeKey, stringBuilder> typeCode in codes)
                        {
                            code.Append(@"module ", typeCode.Key.Namespace, @" {
	export class ", typeCode.Key.Name, @" {");
                            code.Add(typeCode.Value);
                            code.Append(@"
	}
}
");
                        }
                        codes.Clear();
                        return code.ToString();
                    }
                    return null;
                }
            }
            /// <summary>
            /// WEB视图类型信息
            /// </summary>
            public viewType View;
            /// <summary>
            /// 创建代码
            /// </summary>
            /// <param name="type"></param>
            /// <param name="view"></param>
            /// <param name="loadMethod"></param>
            public void Create(auto.parameter parameter, memberType type, viewType view, methodInfo loadMethod)
            {
                AutoParameter = parameter;
                View = view;
                this.type = type;
                LoadMethod = loadMethod;

                typeKey typeKey;
                if (!types.TryGetValue(type.Type.Namespace, out typeKey)) types.Add(type.Type.Namespace, typeKey = new typeKey(parameter, type));

                _code_.Empty();
                create(false);
                stringBuilder code;
                if (!codes.TryGetValue(typeKey, out code)) codes.Add(typeKey, code = new stringBuilder());
                code.Add(_code_);
            }
        }
    }
}