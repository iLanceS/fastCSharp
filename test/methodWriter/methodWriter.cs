using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using fastCSharp;
using fastCSharp.reflection;
using fastCSharp.threading;

namespace fastCSharp.test.methodWriter
{
    /// <summary>
    /// 方法重写，参考 http://bbs.csdn.net/topics/391958344 与 https://github.com/bigbaldy1128/DotNetDetour
    /// </summary>
    public abstract unsafe class methodWriter : IDisposable
    {
        /// <summary>
        /// 方法名称
        /// </summary>
        private abstract class methodName
        {
            /// <summary>
            /// 方法名称序号
            /// </summary>
            public static int Identity;
        }
        /// <summary>
        /// 重定向函数内容
        /// </summary>
        private readonly byte[] newMethodData;
        /// <summary>
        /// 原函数地址
        /// </summary>
        private readonly byte* methodPoint;
        /// <summary>
        /// 重定向函数地址
        /// </summary>
        private readonly byte* newMethodPoint;
        /// <summary>
        /// 重定向函数
        /// </summary>
        protected readonly MulticastDelegate newMethod;
        /// <summary>
        /// 函数访问锁
        /// </summary>
        private int methodLock;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="delegateType"></param>
        protected methodWriter(MethodInfo methodInfo, Type delegateType, Type targetType)
        {
            
            isDisposed = 1;
            RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
            int size = (int)DotNetDetour.LDasm.SizeofMin5Byte(methodPoint = (byte*)methodInfo.MethodHandle.GetFunctionPointer().ToPointer());
            Type[] parameterTypes = methodInfo.GetParameters().getArray(value => value.ParameterType);
            if (targetType != null) parameterTypes = new Type[] { targetType }.concat(parameterTypes);
            Type returnType = methodInfo.ReturnType;
            DynamicMethod dynamicMethod = new DynamicMethod("bigbaldy." + methodInfo.fullName() + "." + Interlocked.Increment(ref methodName.Identity).toString(), returnType, parameterTypes, typeof(methodName), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (returnType != typeof(void))
            {
                if (returnType.IsValueType)
                {
                    LocalBuilder loadMember = generator.DeclareLocal(returnType);
                    generator.Emit(OpCodes.Ldloca_S, loadMember);
                    generator.Emit(OpCodes.Initobj, returnType);
                    generator.Emit(OpCodes.Ldloc_0);
                }
                else generator.Emit(OpCodes.Ldnull);
            }
            for (int nopCount = (size + 5 | 7); nopCount != 0; --nopCount) generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);
            newMethod = (MulticastDelegate)dynamicMethod.CreateDelegate(delegateType);
            newMethod.DynamicInvoke(parameterTypes.getArray(value => (object)null));
            newMethodPoint = getPoint(newMethod);
            //if (*newMethodPoint == 0xe9)
            //{
            //    newMethodPoint += *(uint*)(newMethodPoint + 1) - 5;
            //}
            newMethodData = new byte[size + 5];
            fastCSharp.unsafer.memory.Copy(newMethodPoint, newMethodData, newMethodData.Length);
            for (ulong* read = (ulong*)methodPoint, write = (ulong*)newMethodPoint, end = (ulong*)(methodPoint + ((size + 7) & (int.MaxValue - 7))); read != end; *write++ = *read++) ;
            jmp(newMethodPoint + size, jmp(newMethodPoint, methodPoint));
            jmp(newMethodPoint);
            isDisposed = 0;
        }
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">待写入的方法委托</param>
        public void Set(MulticastDelegate method)
        {
            MethodInfo methodInfo = (method as MulticastDelegate).Method;
            RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
            byte* methodPoint = (byte*)methodInfo.MethodHandle.GetFunctionPointer().ToPointer();
            interlocked.CompareSetYield(ref methodLock);
            try
            {
                if (isDisposed == 0) jmp(methodPoint);
            }
            finally { methodLock = 0; }
        }
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">待写入的方法委托</param>
        public void Set<delegateType>(delegateType method)
        {
            Set(method as MulticastDelegate);
        }
        /// <summary>
        /// 恢复重定向函数
        /// </summary>
        public void Set()
        {
            interlocked.CompareSetYield(ref methodLock);
            try
            {
                if (isDisposed == 0) jmp(*(ulong*)newMethodPoint & 0xffffffffffUL);
            }
            finally { methodLock = 0; }
        }
        /// <summary>
        /// 释放方法指针
        /// </summary>
        public void Dispose()
        {
            interlocked.CompareSetYield(ref methodLock);
            try
            {
                if (isDisposed == 0)
                {
                    jmp(*(ulong*)newMethodPoint & 0xffffffffffUL);
                    isDisposed = 1;
                    fastCSharp.unsafer.memory.Copy(newMethodData, newMethodPoint, newMethodData.Length);
                }
            }
            finally { methodLock = 0; }
        }
        /// <summary>
        /// 写入跳转指令
        /// </summary>
        /// <param name="toMethodPoint"></param>
        /// <returns></returns>
        private void jmp(byte* toMethodPoint)
        {
            jmp(jmp(methodPoint, toMethodPoint));

        }
        /// <summary>
        /// 写入跳转指令
        /// </summary>
        /// <param name="methodPoint"></param>
        /// <param name="code"></param>
        private void jmp(ulong code)
        {
            jmp(methodPoint, code);
        }
        /// <summary>
        /// 获取跳转指令数据
        /// </summary>
        /// <param name="fromMethodPoint"></param>
        /// <param name="toMethodPoint"></param>
        /// <returns></returns>
        private static ulong jmp(byte* fromMethodPoint, byte* toMethodPoint)
        {
            long offset = toMethodPoint - fromMethodPoint - 5;
            if (offset > int.MaxValue || offset < int.MinValue) log.Error.Throw(log.exceptionType.ErrorOperation);
            return ((ulong)(uint)offset << 8) + 0xe9;
        }
        /// <summary>
        /// 写入跳转指令
        /// </summary>
        /// <param name="methodPoint"></param>
        /// <param name="code"></param>
        private static void jmp(byte* methodPoint, ulong code)
        {
            *(ulong*)methodPoint = (*(ulong*)methodPoint & 0xffffff0000000000UL) | code;
        }
        /// <summary>
        /// 获取函数指针
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static byte* getPoint(MulticastDelegate method)
        {
            IntPtr point = multicastDelegate.GetMethodPtrAux(method);
            return (byte*)(point == IntPtr.Zero ? multicastDelegate.GetMethodPtr(method).ToPointer() : point.ToPointer());
        }
    }
    /// <summary>
    /// 方法重写
    /// </summary>
    /// <typeparam name="delegateType">委托类型</typeparam>
    public sealed unsafe class methodWriter<delegateType> : methodWriter
    {
        /// <summary>
        /// 重定向函数
        /// </summary>
        public readonly delegateType Method;
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">被重写方法委托</param>
        /// <param name="setMethod">待写入的方法委托</param>
        public methodWriter(delegateType method)
            : base((method as MulticastDelegate).Method, typeof(delegateType), null)
        {
            Method = (delegateType)(object)newMethod;
        }
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">待写入的方法委托</param>
        public void Set(delegateType method)
        {
            Set(method as MulticastDelegate);
        }
    }
    /// <summary>
    /// 方法重写
    /// </summary>
    /// <typeparam name="delegateType">委托类型</typeparam>
    /// <typeparam name="targetType">第一个参数类型</typeparam>
    public sealed unsafe class methodWriter<delegateType, targetType> : methodWriter
    {
        /// <summary>
        /// 重定向函数
        /// </summary>
        public readonly delegateType Method;
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">被重写方法委托</param>
        /// <param name="setMethod">待写入的方法委托</param>
        public methodWriter(delegateType method)
            : base((method as MulticastDelegate).Method, typeof(delegateType), typeof(targetType))
        {
            Method = (delegateType)(object)newMethod;
        }
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">待写入的方法委托</param>
        public void Set(delegateType method)
        {
            Set(method as MulticastDelegate);
        }
    }
    /// <summary>
    /// 方法重写
    /// </summary>
    /// <typeparam name="delegateType">委托类型</typeparam>
    /// <typeparam name="createDelegateType">委托类型</typeparam>
    /// <typeparam name="targetType">第一个参数类型</typeparam>
    public sealed unsafe class methodWriter<delegateType, createDelegateType, targetType> : methodWriter
    {
        /// <summary>
        /// 重定向函数
        /// </summary>
        public readonly createDelegateType Method;
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">被重写方法委托</param>
        /// <param name="setMethod">待写入的方法委托</param>
        public methodWriter(delegateType method)
            : base((method as MulticastDelegate).Method, typeof(createDelegateType), typeof(targetType))
        {
            Method = (createDelegateType)(object)newMethod;
        }
        /// <summary>
        /// 方法重写
        /// </summary>
        /// <param name="method">待写入的方法委托</param>
        public void Set(delegateType method)
        {
            Set(method as MulticastDelegate);
        }
    }
}
