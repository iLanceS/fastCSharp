using System;

namespace fastCSharp
{
    /// <summary>
    /// 多播委托信息
    /// </summary>
    public struct multicastDelegate
    {
        /// <summary>
        /// 函数信息
        /// </summary>
        private object methodBase;
        /// <summary>
        /// 无参数函数指针
        /// </summary>
        private IntPtr methodPtr;
        /// <summary>
        /// 参数函数指针
        /// </summary>
        private IntPtr methodPtrAux;
        /// <summary>
        /// 函数指针
        /// </summary>
        public IntPtr MethodPtr
        {
            get
            {
                return methodPtrAux == IntPtr.Zero ? methodPtr : methodPtrAux;
            }
        }
        /// <summary>
        /// 第一个对象参数
        /// </summary>
        private object target;
        /// <summary>
        /// 第一个对象参数
        /// </summary>
        public object Target
        {
            get
            {
                return methodPtrAux == IntPtr.Zero ? target : null;
            }
        }
        /// <summary>
        /// 多播委托绑定数量
        /// </summary>
        private IntPtr invocationCount;
        /// <summary>
        /// 多播委托链表
        /// </summary>
        private object invocationList;
        /// <summary>
        /// 多播委托信息
        /// </summary>
        /// <param name="method"></param>
        public multicastDelegate(MulticastDelegate method)
        {
            methodBase = getMethodBase(method);
            methodPtr = GetMethodPtr(method);
            methodPtrAux = GetMethodPtrAux(method);
            target = getTarget(method);
            invocationCount = getInvocationCount(method);
            invocationList = getInvocationList(method);
        }
        /// <summary>
        /// 函数信息
        /// </summary>
        private static readonly Func<Delegate, object> getMethodBase = fastCSharp.emit.pub.GetField<Delegate, object>("_methodBase");
        /// <summary>
        /// 获取委托函数指针
        /// </summary>
        public static readonly Func<Delegate, IntPtr> GetMethodPtr = fastCSharp.emit.pub.GetField<Delegate, IntPtr>("_methodPtr");
        /// <summary>
        /// 获取委托函数指针
        /// </summary>
        public static readonly Func<Delegate, IntPtr> GetMethodPtrAux = fastCSharp.emit.pub.GetField<Delegate, IntPtr>("_methodPtrAux");
        /// <summary>
        /// 引用对象参数
        /// </summary>
        private static readonly Func<Delegate, object> getTarget = fastCSharp.emit.pub.GetField<Delegate, object>("_target");
        /// <summary>
        /// 多播委托绑定数量
        /// </summary>
        private static readonly Func<MulticastDelegate, IntPtr> getInvocationCount = fastCSharp.emit.pub.GetField<MulticastDelegate, IntPtr>("_invocationCount");
        /// <summary>
        /// 多播委托链表
        /// </summary>
        private static readonly Func<MulticastDelegate, object> getInvocationList = fastCSharp.emit.pub.GetField<MulticastDelegate, object>("_invocationList");
    }
}
