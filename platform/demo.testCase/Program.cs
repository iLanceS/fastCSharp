using System;
using System.Reflection;
using fastCSharp.code;
using fastCSharp.reflection;

namespace fastCSharp.demo.testCase
{
	class Program
	{
		public static void Main(string[] args)
		{
			foreach (Type type in typeof(Program).Assembly.GetTypes())
			{
				if (!type.IsGenericType && !type.IsInterface && !type.IsEnum)
				{
					foreach (code.methodInfo methodInfo in code.methodInfo.GetMethods<fastCSharp.code.testCase>(type, memberFilters.Static, false, true, false, false))
					{
						MethodInfo method = methodInfo.Method;
						if (method.IsGenericMethod)
						{
							Console.WriteLine("测试用例不能是泛型函数 " + method.fullName());
						}
						else
						{
							Type returnType = method.ReturnType;
							if ((returnType == typeof(bool) || returnType == typeof(void)) && method.GetParameters().Length == 0)
							{
								try
								{
									Console.WriteLine("正在测试 " + method.fullName());
									object returnValue = method.Invoke(null, null);
									if (method.ReturnType == typeof(bool) && !(bool)returnValue)
									{
										Console.WriteLine("测试用例调用失败 " + method.fullName());
									}
								}
								catch (Exception error)
								{
									Console.WriteLine(error.ToString());
									Console.ReadKey();
								}
							}
						}
					}
				}
			}
			Console.WriteLine("End");
			Console.ReadKey();
		}
	}
}
