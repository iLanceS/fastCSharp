using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace fastCSharp.test.methodWriter
{
    class Program
    {
        /// <summary>
        /// 方法重写测试
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Instance();
            Console.WriteLine();
            Static();
            Console.WriteLine();
            try
            {
                InstanceProperty();
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine("VS调试可能异常");
            }
            try
            {
                StaticProperty();
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine("VS调试可能异常");
            }
            Console.WriteLine("press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// 测试数据
        /// </summary>
        class destination
        {
            /// <summary>
            /// 测试数据
            /// </summary>
            public int Value;
            /// <summary>
            /// 测试属性
            /// </summary>
            public int Property
            {
                get
                {
                    //防止内联优化
                    try
                    {
                        return Value;
                    }
                    catch { }
                    finally { }
                    return Value;
                }
            }
            /// <summary>
            /// 测试属性
            /// </summary>
            /// <returns></returns>
            public static int StaticProperty
            {
                get
                {
                    //防止内联优化
                    try
                    {
                        return 1;
                    }
                    catch { }
                    finally { }
                    return 1;
                }
            }
            /// <summary>
            /// 覆盖测试目标方法
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public int Get()
            {
                return Value;
            }
            /// <summary>
            /// 覆盖测试目标方法
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static int Get1()
            {
                return 1;
            }
        }
        /// <summary>
        /// 测试数据
        /// </summary>
        class source
        {
            /// <summary>
            /// 测试数据
            /// </summary>
            public int Value;
            /// <summary>
            /// 覆盖测试源方法
            /// </summary>
            /// <returns></returns>
            public int Get2()
            {
                return 2;
            }
            /// <summary>
            /// 覆盖测试源方法
            /// </summary>
            /// <returns></returns>
            public static int Get3()
            {
                return 3;
            }
            /// <summary>
            /// 覆盖测试源方法
            /// </summary>
            /// <param name="destination"></param>
            /// <returns></returns>
            public int GetAdd1(destination destination)
            {
                return Value + 1;
            }
            /// <summary>
            /// 覆盖测试源方法
            /// </summary>
            /// <param name="destination"></param>
            /// <returns></returns>
            public static int GetAdd2(destination destination)
            {
                return destination.Value + 2;
            }
        }

        /// <summary>
        /// 实例方法重写测试
        /// </summary>
        static void Instance()
        {
            Console.WriteLine("Instance Start");
            destination destination = new destination { Value = 1 };
            using (methodWriter<Func<int>> methodWriter = new methodWriter<Func<int>>(destination.Get))
            {
                Console.WriteLine("原始调用输出1 -> " + destination.Get().toString());
                Console.WriteLine("没有绑定实例的实例方法调用输出“随机数” -> " + methodWriter.Method().toString());

                methodWriter.Set(new source().Get2);
                Console.WriteLine("重写到实例方法source.Get2后调用输出2 -> " + destination.Get().toString());

                methodWriter.Set(source.Get3);
                Console.WriteLine("重写到静态方法source.Get3后调用输出3 -> " + destination.Get().toString());
            }
            Console.WriteLine("释放重写后原始调用输出1 -> " + destination.Get().toString());
            Console.WriteLine("Instance End");
        }
        /// <summary>
        /// 静态方法重写测试
        /// </summary>
        static void Static()
        {
            Console.WriteLine("Static Start");
            using (methodWriter<Func<int>> methodWriter = new methodWriter<Func<int>>(destination.Get1))
            {
                Console.WriteLine("原始调用输出1 -> " + destination.Get1().toString());
                Console.WriteLine("静态方法原始调用输出1 -> " + methodWriter.Method().toString());

                methodWriter.Set(new source().Get2);
                Console.WriteLine("重写到实例方法source.Get2后调用输出2 -> " + destination.Get1().toString());

                methodWriter.Set(source.Get3);
                Console.WriteLine("重写到静态方法source.Get3后调用输出3 -> " + destination.Get1().toString());
            }
            Console.WriteLine("释放重写后原始调用输出1 -> " + destination.Get1().toString());
            Console.WriteLine("Static End");
        }
        /// <summary>
        /// 实例属性重写测试
        /// </summary>
        static void InstanceProperty()
        {
            Console.WriteLine("InstanceProperty Start");
            destination destination = new destination { Value = 1 };
            using (methodWriter<Func<destination, int>, destination> methodWriter = new methodWriter<Func<destination, int>, destination>((Func<destination, int>)Delegate.CreateDelegate(typeof(Func<destination, int>), typeof(destination).GetProperty("Property", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(false))))
            {
                Console.WriteLine("原始调用输出1 -> " + destination.Property.toString());
                Console.WriteLine("静态方法原始调用输出1 -> " + methodWriter.Method(destination).toString());

                methodWriter.Set(new source().GetAdd1);
                Console.WriteLine("重写到实例方法source.GetAdd1后调用输出2 -> " + destination.Property.toString());

                methodWriter.Set(source.GetAdd2);
                Console.WriteLine("重写到静态方法source.GetAdd2后调用输出3 -> " + destination.Property.toString());
            }
            Console.WriteLine("释放重写后原始调用输出1 -> " + destination.Property.toString());
            Console.WriteLine("InstanceProperty End");
        }
        /// <summary>
        /// 静态属性重写测试
        /// </summary>
        static void StaticProperty()
        {
            Console.WriteLine("StaticProperty Start");
            destination destination = new destination { Value = 1 };
            using (methodWriter<Func<int>> methodWriter = new methodWriter<Func<int>>((Func<int>)Delegate.CreateDelegate(typeof(Func<int>), typeof(destination).GetProperty("StaticProperty", BindingFlags.Static | BindingFlags.Public).GetGetMethod(false))))
            {
                Console.WriteLine("原始调用输出1 -> " + destination.StaticProperty.toString());
                Console.WriteLine("静态方法原始调用输出1 -> " + methodWriter.Method().toString());

                methodWriter.Set(new source().Get2);
                Console.WriteLine("重写到实例方法source.Get2后调用输出2 -> " + destination.StaticProperty.toString());

                methodWriter.Set(source.Get3);
                Console.WriteLine("重写到静态方法source.Get3后调用输出3 -> " + destination.StaticProperty.toString());
            }
            Console.WriteLine("释放重写后原始调用输出1 -> " + destination.StaticProperty.toString());
            Console.WriteLine("StaticProperty End");
        }
    }
}
