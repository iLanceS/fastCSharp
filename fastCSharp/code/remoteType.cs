using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 远程类型
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
    public struct remoteType
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        private string assemblyName;
        /// <summary>
        /// 类型名称
        /// </summary>
        private string name;
        /// <summary>
        /// 是否空类型
        /// </summary>
        public bool IsNull
        {
            get { return assemblyName == null || name == null; }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get
            {
                Type type;
                if (TryGet(out type)) return type;
                fastCSharp.log.Default.Throw(null, "未能加载类型 : " + name + " in " + assemblyName, true);
                return null;
            }
        }
        /// <summary>
        /// 远程类型
        /// </summary>
        /// <param name="type">类型</param>
        public remoteType(Type type)
        {
            name = type.FullName;
            assemblyName = type.Assembly.FullName;
        }
        /// <summary>
        /// 类型隐式转换
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>远程类型</returns>
        public static implicit operator remoteType(Type type) { return new remoteType(type); }
        /// <summary>
        /// 尝试获取类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否成功</returns>
        public bool TryGet(out Type type)
        {
            Assembly assembly = reflection.assembly.Get(assemblyName);
            if (assembly != null)
            {
                if ((type = assembly.GetType(name)) != null) return true;
            }
            else type = null;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return assemblyName + " + " + name;
        }
    }
}
