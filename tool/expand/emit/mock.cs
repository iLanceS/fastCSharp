using System;
using System.Reflection;
using System.Reflection.Emit;
using fastCSharp.reflection;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// 模拟
    /// </summary>
    internal static class mock
    {
        /// <summary>
        /// 模拟类型
        /// </summary>
        internal enum type
        {
            /// <summary>
            /// 不模拟
            /// </summary>
            None,
            /// <summary>
            /// 随机对象
            /// </summary>
            Random,
            /// <summary>
            /// 接口模拟
            /// </summary>
            Interface,
            /// <summary>
            /// 模拟调用
            /// </summary>
            Mock,
        }
        /// <summary>
        /// 模拟类型信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, type> types = new interlocked.dictionary<Type, type>();
        /// <summary>
        /// 获取模拟类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        internal static type GetType(Type type)
        {
            type mockType;
            if (types.TryGetValue(type, out mockType)) return mockType;
            types.Set(type, mockType = (type)typeMethod.MakeGenericMethod(type).Invoke(null, null));
            return mockType;
        }
        /// <summary>
        /// 模拟调用
        /// </summary>
        private static readonly MethodInfo typeMethod = typeof(mock).GetMethod("getType", BindingFlags.NonPublic | BindingFlags.Static);
        /// <summary>
        /// 获取模拟类型
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static type getType<valueType>()
        {
            return mock<valueType>.Type;
        }
        /// <summary>
        /// 随机函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> randomMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取随机函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo GetRandom(Type type)
        {
            MethodInfo method;
            if (randomMethods.TryGetValue(type, out method)) return method;
            randomMethods.Set(type, method = randomMethod.MakeGenericMethod(type));
            return method;
        }
        /// <summary>
        /// 随机函数
        /// </summary>
        private static readonly MethodInfo randomMethod = typeof(mock).GetMethod("getRandom", BindingFlags.Static | BindingFlags.NonPublic, null, nullValue<Type>.Array, null);
        /// <summary>
        /// 随机函数
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static valueType getRandom<valueType>()
        {
            return random<valueType>.CreateNotNull(random.DefaultConfig);
        }
        /// <summary>
        /// 接口模拟包装函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> interfaceMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取接口模拟包装
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo GetInterface(Type type)
        {
            MethodInfo method;
            if (interfaceMethods.TryGetValue(type, out method)) return method;
            interfaceMethods.Set(type, method = interfaceMethod.MakeGenericMethod(type));
            return method;
        }
        /// <summary>
        /// 接口模拟包装
        /// </summary>
        private static readonly MethodInfo interfaceMethod = typeof(mock).GetMethod("getInterface", BindingFlags.NonPublic | BindingFlags.Static);
        /// <summary>
        /// 接口模拟包装
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static valueType getInterface<valueType>(valueType value)
        {
            return mock<valueType>.Interface.Box(value);
        }
        /// <summary>
        /// 模拟调用函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> mockMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取模拟调用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo GetMock(Type type)
        {
            MethodInfo method;
            if (mockMethods.TryGetValue(type, out method)) return method;
            mockMethods.Set(type, method = mockMethod.MakeGenericMethod(type));
            return method;
        }
        /// <summary>
        /// 模拟调用
        /// </summary>
        private static readonly MethodInfo mockMethod = typeof(mock).GetMethod("getMock", BindingFlags.NonPublic | BindingFlags.Static);
        /// <summary>
        /// 模拟调用
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static valueType getMock<valueType>(valueType value)
        {
            return mock<valueType>.Mock(value);
        }
    }
    /// <summary>
    /// 接口调用模拟
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    internal static class mock<valueType>
    {
        /// <summary>
        /// 接口模拟包装
        /// </summary>
        internal static class Interface
        {
            /// <summary>
            /// 接口模拟包装器
            /// </summary>
            internal static readonly Func<valueType, valueType> Box;

            static Interface()
            {
                Type type = typeof(valueType);
                if (type.IsInterface)
                {
                    if (type.IsVisible)
                    {
                        Type[] typeParameter = new Type[] { type };
                        TypeBuilder typeBuilder = pub.ModuleBuilder.DefineType("fastCSharpMockInterface_" + type.FullName, TypeAttributes.Public, typeof(object), typeParameter);
                        typeBuilder.AddInterfaceImplementation(type);

                        ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Private, CallingConventions.Standard, typeParameter);
                        FieldBuilder field = typeBuilder.DefineField("_fastCSharpMockInterfaceValue_", type, FieldAttributes.Private);

                        ILGenerator generator = constructor.GetILGenerator();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, typeof(object).GetConstructor(nullValue<Type>.Array));
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Stfld, field);
                        generator.Emit(OpCodes.Ret);

                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            ParameterInfo[] parameters = property.GetIndexParameters();
                            typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, CallingConventions.HasThis, property.PropertyType, property.GetRequiredCustomModifiers(), property.GetOptionalCustomModifiers(), parameters.getArray(parameter => parameter.ParameterType), parameters.getArray(parameter => parameter.GetRequiredCustomModifiers()), parameters.getArray(parameter => parameter.GetOptionalCustomModifiers()));
                        }

                        foreach (MethodInfo method in type.GetMethods())
                        {
                            ParameterInfo returnParameter = method.ReturnParameter;
                            ParameterInfo[] parameters = method.GetParameters();
                            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, method.ReturnType, returnParameter.GetRequiredCustomModifiers(), returnParameter.GetOptionalCustomModifiers(), parameters.getArray(parameter => parameter.ParameterType), parameters.getArray(parameter => parameter.GetRequiredCustomModifiers()), parameters.getArray(parameter => parameter.GetOptionalCustomModifiers()));
                            generator = methodBuilder.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldfld, field);
                            int parameterIndex = 0;
                            foreach (ParameterInfo parameter in parameters)
                            {//showjim +参数Attribute处理+ref参数处理
                                ++parameterIndex;
                                switch (mock.GetType(parameter.ParameterType))
                                {
                                    case mock.type.Random:
                                        generator.Emit(OpCodes.Call, mock.GetRandom(parameter.ParameterType));
                                        break;
                                    case mock.type.Interface:
                                        generator.loadArgument(parameterIndex);
                                        generator.Emit(OpCodes.Call, mock.GetInterface(parameter.ParameterType));
                                        break;
                                    case  mock.type.Mock:
                                        generator.loadArgument(parameterIndex);
                                        generator.Emit(OpCodes.Call, mock.GetMock(parameter.ParameterType));
                                        break;
                                    default:
                                        generator.loadArgument(parameterIndex);
                                        break;
                                }
                            }
                            generator.Emit(OpCodes.Callvirt, method);
                            switch (mock.GetType(method.ReturnType))
                            {
                                case mock.type.Interface:
                                    generator.Emit(OpCodes.Call, mock.GetInterface(method.ReturnType));
                                    break;
                                case mock.type.Mock:
                                    generator.Emit(OpCodes.Call, mock.GetMock(method.ReturnType));
                                    break;
                            }
                            generator.Emit(OpCodes.Ret);

                            if (method.IsGenericMethodDefinition) methodBuilder.DefineGenericParameters(method.GetGenericArguments().getArray(value => value.Name));
                            typeBuilder.DefineMethodOverride(methodBuilder, method);
                        }
                        Type mockType = typeBuilder.CreateType();

                        DynamicMethod dynamicMethod = new DynamicMethod("constructor", type, typeParameter, mockType, true);
                        generator = dynamicMethod.GetILGenerator();
                        Label isNull = generator.DefineLabel();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Brfalse_S, isNull);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Newobj, mockType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, typeParameter, null));
                        generator.Emit(OpCodes.Ret);
                        generator.MarkLabel(isNull);
                        generator.Emit(OpCodes.Ldnull);
                        generator.Emit(OpCodes.Ret);
                        Box = (Func<valueType, valueType>)dynamicMethod.CreateDelegate(typeof(Func<valueType, valueType>));
                        return;
                    }
                    log.Error.Add(type.fullName() + " 不可访问", new System.Diagnostics.StackFrame(), false);
                }
                Box = unchange;
            }
        }
        /// <summary>
        /// 模拟类型
        /// </summary>
        internal static readonly mock.type Type;
        /// <summary>
        /// 模拟调用
        /// </summary>
        internal static readonly Func<valueType, valueType> Mock;
        /// <summary>
        /// 保持对象不变(没有模拟操作)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static valueType unchange(valueType value)
        {
            return value;
        }

        static mock()
        {
            Type = mock.type.None;
            Mock = unchange;
        }
    }
}
