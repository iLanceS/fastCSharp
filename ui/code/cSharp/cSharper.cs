using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// CSharp模板生成基类
    /// </summary>
    internal abstract class cSharper
    {
        /// <summary>
        /// 类定义生成
        /// </summary>
        public abstract class definition
        {
            /// <summary>
            /// 类定义开始
            /// </summary>
            private stringBuilder start = new stringBuilder();
            /// <summary>
            /// 类定义开始
            /// </summary>
            public string Start
            {
                get
                {
                    return start.ToString();
                }
            }
            /// <summary>
            /// 类定义结束
            /// </summary>
            private stringBuilder end = new stringBuilder();
            /// <summary>
            /// 类定义结束
            /// </summary>
            public string End
            {
                get
                {
                    return end.ToString();
                }
            }
            /// <summary>
            /// C# 类定义生成
            /// </summary>
            public sealed class cSharp : definition
            {
                /// <summary>
                /// 类定义生成
                /// </summary>
                /// <param name="type">类型</param>
                public cSharp(Type type, bool isPartial, bool isClass, string typeNamespace = null)
                {
                    create(type, isPartial, isClass, typeNamespace);
                    end.Reverse();
                }
                /// <summary>
                /// 生成类定义
                /// </summary>
                /// <param name="type">类型</param>
                /// <param name="isPartial">是否部分定义</param>
                /// <param name="isClass">是否建立类定义</param>
                private void create(Type type, bool isPartial, bool isClass, string typeNamespace)
                {
                    if (type.ReflectedType == null)
                    {
                        start.Add("namespace " + (typeNamespace ?? type.Namespace) + @"
{");
                        end.Add(@"
}");
                    }
                    else
                    {
                        create(type.ReflectedType.IsGenericType ? type.ReflectedType.MakeGenericType(type.GetGenericArguments()) : type.ReflectedType, true, true, null);
                    }
                    if (isClass)
                    {
                        start.Add(@"
    " + type.getAccessDefinition()
                  + (type.IsAbstract ? (type.IsSealed ? " static" : " abstract") : null)
                  + (isPartial ? " partial" : null)
                  + (type.IsInterface ? " interface" : " class")
                  + " " + type.Name + (type.IsGenericType ? "<" + type.GetGenericArguments().joinString(", ", x => x.fullName()) + ">" : null) + @"
    {");
                        end.Add(@"
    }");
                    }
                }
            }
            /// <summary>
            /// JavaScript 类定义生成
            /// </summary>
            public sealed class javaScript : definition
            {
                /// <summary>
                /// 类定义生成
                /// </summary>
                /// <param name="type">类型</param>
                public javaScript(Type type)
                {
                }
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        internal memberType type;
        /// <summary>
        /// 类名称定义
        /// </summary>
        public string TypeNameDefinition
        {
            get
            {
                if (type.Type == null) return null;
                return type.Type.getAccessDefinition() + " " + NoAccessTypeNameDefinition;
            }
        }
        /// <summary>
        /// 类名称定义
        /// </summary>
        public string NoAccessTypeNameDefinition
        {
            get
            {
                if (type.Type == null) return null;
                return "partial" + (type.IsNull ? " class" : " struct") + " " + type.TypeName;
            }
        }
        /// <summary>
        /// 类名称
        /// </summary>
        public string TypeName
        {
            get { return type.TypeName; }
        }
        /// <summary>
        /// 生成的代码
        /// </summary>
        protected stringBuilder _code_ = new stringBuilder();
        /// <summary>
        /// 代码段
        /// </summary>
        protected Dictionary<string, string> _partCodes_ = dictionary.CreateOnly<string, string>();
        /// <summary>
        /// 临时逻辑变量
        /// </summary>
        protected bool _if_;
        /// <summary>
        /// 当前循环索引
        /// </summary>
        protected int _loopIndex_;
        /// <summary>
        /// 当前循环数量
        /// </summary>
        protected int _loopCount_;
        /// <summary>
        /// 代码生成语言
        /// </summary>
        private fastCSharp.code.auto.language _language_;
        /// <summary>
        /// 类定义生成
        /// </summary>
        private definition _definition_;
        ///// <summary>
        ///// 当前循环集合
        ///// </summary>
        //protected object _loopValues_;
        ///// <summary>
        ///// 当前循环值
        ///// </summary>
        //protected object _loopValue_;
        /// <summary>
        /// 输出类定义开始段代码
        /// </summary>
        /// <param name="language">代码生成语言</param>
        /// <param name="isOutDefinition">是否输出类定义</param>
        /// <returns>类定义</returns>
        protected bool outStart(fastCSharp.code.auto.language language, bool isOutDefinition)
        {
            _definition_ = null;
            _language_ = language;
            if (isOutDefinition)
            {
                _code_.Empty();
                if (fastCSharp.code.coder.Add(GetType(), type.Type))
                {
                    switch (_language_)
                    {
                        case auto.language.JavaScript:
                        case auto.language.TypeScript:
                            _definition_ = new definition.javaScript(type);
                            break;
                        default: _definition_ = new definition.cSharp(type, true, false); break;
                    }
                    _code_.Add(_definition_.Start);
                    return true;
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 输出类定义结束段代码
        /// </summary>
        protected void outEnd()
        {
            _code_.Add(_definition_.End);
            switch (_language_)
            {
                case auto.language.JavaScript:
                case auto.language.TypeScript:
                    break;
                default:
                    fastCSharp.code.coder.Add(_code_.ToString(), _language_);
                    return;
            }
        }
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="isOut">是否输出代码</param>
        protected virtual void create(bool isOut)
        {
            log.Default.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 生成代码
        /// </summary>
        protected void create()
        {
            create(true);
        }
    }
    /// <summary>
    /// 自定义属性模板生成基类
    /// </summary>
    /// <typeparam name="attributeType">自定义属性类型</typeparam>
    internal abstract class cSharper<attributeType> : cSharper, IAuto
        where attributeType : Attribute
    {
        /// <summary>
        /// 自动安装参数
        /// </summary>
        public auto.parameter AutoParameter;
        /// <summary>
        /// 程序集
        /// </summary>
        protected Assembly assembly;
        /// <summary>
        /// 自定义属性
        /// </summary>
        public attributeType Attribute;
        /// <summary>
        /// 是否搜索父类属性
        /// </summary>
        public virtual bool IsBaseType
        {
            get { return false; }
        }
        /// <summary>
        /// 自定义属性是否可继承
        /// </summary>
        public virtual bool IsInheritAttribute
        {
            get { return false; }
        }
        /// <summary>
        /// 获取类型与自定义配置信息
        /// </summary>
        /// <returns></returns>
        internal keyValue<Type, attributeType>[] GetTypeAttributes()
        {
            return AutoParameter.Types.getArray(type => new keyValue<Type, attributeType>(type, fastCSharp.code.typeAttribute.GetAttribute<attributeType>(type, IsBaseType, IsInheritAttribute)))
                    .getFindArray(attribute => attribute.Value != null && attribute.Key.customAttribute<fastCSharp.code.ignore>(IsBaseType) == null);
        }
        /// <summary>
        /// 安装入口
        /// </summary>
        /// <param name="parameter">安装参数</param>
        /// <returns>是否安装成功</returns>
        public virtual bool Run(auto.parameter parameter)
        {
            if (parameter != null)
            {
                AutoParameter = parameter;
                assembly = parameter.Assembly;
                keyValue<Type, attributeType>[] types = GetTypeAttributes();
                foreach (keyValue<Type, attributeType> ajax in types)
                {
                    type = ajax.Key;
                    Attribute = ajax.Value;
                    nextCreate();
                }
                onCreated();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否可调用构造函数
        /// </summary>
        /// <returns>是否可调用构造函数</returns>
        protected bool isConstructor()
        {
            if (type.Type.IsAbstract || type.Type.IsInterface || type.Type.IsEnum)
            {
                error.Message(type.Type.fullName() + " 无法创建 接口/抽象类/枚举 的实例");
                return false;
            }
            else if (type.Type.IsClass && type.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, nullValue<Type>.Array, null) == null)
            {
                error.Message(type.Type.fullName() + " 找不到无参构造函数");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 安装下一个类型
        /// </summary>
        protected abstract void nextCreate();
        /// <summary>
        /// 安装完成处理
        /// </summary>
        protected abstract void onCreated();
    }
}
