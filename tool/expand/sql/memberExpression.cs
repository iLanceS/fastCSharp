using System;
using System.Reflection;
using System.Linq.Expressions;

namespace fastCSharp.sql
{
    /// <summary>
    /// 成员表达式操作
    /// </summary>
    public static class memberExpression
    {
        /// <summary>
        /// 创建成员操作委托
        /// </summary>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="member"></param>
        /// <param name="getMember"></param>
        /// <param name="setMember"></param>
        public static void Create<targetType, valueType>(Expression<Func<targetType, valueType>> member, out Func<targetType, valueType> getMember, out Action<targetType, valueType> setMember)
            where targetType : class
        {
            memberExpression<targetType, valueType> expression = new memberExpression<targetType, valueType>(member);
            getMember = expression.GetMember;
            setMember = expression.SetMember;
        }
        /// <summary>
        /// 创建成员操作委托
        /// </summary>
        /// <typeparam name="targetType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Func<targetType, valueType> CreateGetMember<targetType, valueType>(Expression<Func<targetType, valueType>> member)
            where targetType : class
        {
            return new memberExpression<targetType, valueType>(member).GetMember;
        }
    }
    /// <summary>
    /// 成员表达式
    /// </summary>
    /// <typeparam name="targetType"></typeparam>
    /// <typeparam name="valueType"></typeparam>
    internal struct memberExpression<targetType, valueType>
        where targetType : class
    {
        /// <summary>
        /// 字段成员
        /// </summary>
        public FieldInfo Field;
        /// <summary>
        /// 获取成员
        /// </summary>
        public Func<targetType, valueType> GetMember
        {
            get
            {
                return fastCSharp.emit.pub.UnsafeGetField<targetType, valueType>(Field);
                //DynamicMethod dynamicMethod = new DynamicMethod("getMember" + Field.Name, typeof(valueType), new Type[] { typeof(targetType) }, typeof(targetType), true);
                //ILGenerator generator = dynamicMethod.GetILGenerator();
                //generator.Emit(OpCodes.Ldarg_0);
                //generator.Emit(OpCodes.Ldfld, Field);
                //generator.Emit(OpCodes.Ret);
                //return (Func<targetType, valueType>)dynamicMethod.CreateDelegate(typeof(Func<targetType, valueType>));
            }
        }
        /// <summary>
        /// 设置成员
        /// </summary>
        public Action<targetType, valueType> SetMember
        {
            get
            {
                return fastCSharp.emit.pub.UnsafeSetField<targetType, valueType>(Field);
            }
        }
        /// <summary>
        /// 成员表达式
        /// </summary>
        /// <param name="member">成员表达式</param>
        public memberExpression(Expression<Func<targetType, valueType>> member)
        {
            Expression expression = member.Body;
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                FieldInfo field = ((MemberExpression)expression).Member as FieldInfo;
                if (field != null && !field.IsStatic && field.DeclaringType.IsAssignableFrom(typeof(targetType)) && field.FieldType == typeof(valueType)) Field = field;
                else Field = null;
            }
            else Field = null;
        }
    }
}
