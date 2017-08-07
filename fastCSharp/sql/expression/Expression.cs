using System;
using System.Reflection;
using System.Collections.Generic;
using fastCSharp;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 表达式
    /// </summary>
    internal abstract class Expression
    {
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ExpressionType NodeType { get; protected set; }
        /// <summary>
        /// 是否简单表达式
        /// </summary>
        public bool IsSimple { get; protected set; }
        /// <summary>
        /// 是否常量表达式
        /// </summary>
        public bool IsConstant { get; protected set; }
        /// <summary>
        /// 常量值是否为null
        /// </summary>
        public bool IsConstantNull
        {
            get
            {
                return IsConstant && ((ConstantExpression)this).Value == null;
            }
        }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public virtual Expression SimpleExpression
        {
            get { return this; }
        }
        /// <summary>
        /// 表达式引用计数
        /// </summary>
        internal int ExpressionCount;
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void PushPool()
        {
            if (ExpressionCount == 0) pushPool();
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void PushCountPool()
        {
            if (--ExpressionCount == 0) pushPool();
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal abstract void pushPool();

        /// <summary>
        /// lambda隐式转换成表达式
        /// </summary>
        /// <typeparam name="DelegateType">委托类型</typeparam>
        /// <param name="body">表达式主体</param>
        /// <param name="parameters">参数</param>
        /// <returns>委托关联表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Expression<DelegateType> Lambda<DelegateType>(Expression body, params ParameterExpression[] parameters)
        {
            return Expression<DelegateType>.Get(body, parameters);
        }
        /// <summary>
        /// 表达式参数
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="name">参数名称</param>
        /// <returns>表达式参数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ParameterExpression Parameter(Type type, string name)
        {
            return ParameterExpression.Get(type, name);
        }
        /// <summary>
        /// 创建字段表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="field">字段信息</param>
        /// <returns>字段表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MemberExpression Field(Expression expression, FieldInfo field)
        {
            if (field.IsStatic ^ expression == null)
            {
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
            }
            return fieldExpression.Get(expression, field);
        }
        /// <summary>
        /// 创建字段表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">字段所属类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段表达式</returns>
        public static MemberExpression Field(Expression expression, Type type, string fieldName)
        {
            FieldInfo field = type.getField(fieldName);
            if (field == null) fastCSharp.log.Error.Throw(type.ToString() + " 未找到字段 " + fieldName, null, true);
            return Field(expression, field);
        }
        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="property">属性信息</param>
        /// <returns>属性表达式</returns>
        public static MemberExpression Property(Expression expression, PropertyInfo property)
        {
            MethodInfo methodInfo = property.GetGetMethod(true);
            if (methodInfo == null || (methodInfo.IsStatic ^ expression == null))
            {
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
            }
            return propertyExpression.Get(expression, property);
        }
        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">属性所属类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性表达式</returns>
        public static MemberExpression Property(Expression expression, Type type, string propertyName)
        {
            PropertyInfo property = type.getProperty(propertyName);
            if (property == null) fastCSharp.log.Error.Throw(type.ToString() + " 未找到属性 " + propertyName, null, true);
            return Property(expression, property);
        }
        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="propertyAccessor">属性关联函数信息</param>
        /// <returns>属性表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
        {
            return Property(expression, getProperty(propertyAccessor));
        }
        /// <summary>
        /// 根据函数信息获取关联属性信息
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <returns>属性信息</returns>
        private static PropertyInfo getProperty(MethodInfo method)
        {
            foreach (PropertyInfo property in method.DeclaringType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance)))
            {
                if (property.CanRead && checkMethod(method, property.GetGetMethod(true))) return property;
            }
            fastCSharp.log.Error.Throw(method.DeclaringType.fullName() + "." + method.Name + " 未找到对应属性", null, true);
            return null;
        }
        /// <summary>
        /// 检测函数信息与属性函数信息是否匹配
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <param name="propertyMethod">属性函数信息</param>
        /// <returns>是否关联属性</returns>
        private static bool checkMethod(MethodInfo method, MethodInfo propertyMethod)
        {
            if (method == propertyMethod) return true;
            Type declaringType = method.DeclaringType;
            return ((declaringType.IsInterface && (method.Name == propertyMethod.Name)) && (declaringType.GetMethod(method.Name) == propertyMethod));
        }
        /// <summary>
        /// 创建二元||表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元||表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression OrElse(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.OrElse, left, right, null);
        }
        /// <summary>
        /// 创建||二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元||表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.OrElse, left, right, method);
        }
        /// <summary>
        /// 创建&amp;&amp;二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元&amp;&amp;表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression AndAlso(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.AndAlso, left, right, null);
        }
        /// <summary>
        /// 创建&amp;&amp;二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元&amp;&amp;表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.AndAlso, left, right, method);
        }
        /// <summary>
        /// 创建!一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>!一元表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Not(Expression expression)
        {
            return UnaryExpression.Get(ExpressionType.Not, expression, null);
        }
        /// <summary>
        /// 创建!二元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元!表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Not(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.Not, expression, method);
        }
        /// <summary>
        /// 创建==二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元==表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Equal(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.Equal, left, right, null);
        }
        /// <summary>
        /// 创建==二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元==表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Equal, left, right, method);
        }
        /// <summary>
        /// 创建!=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元!=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression NotEqual(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.NotEqual, left, right, null);
        }
        /// <summary>
        /// 创建!=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元!=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.NotEqual, left, right, method);
        }
        /// <summary>
        /// 创建>=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元>=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.GreaterThanOrEqual, left, right, null);
        }
        /// <summary>
        /// 创建>=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元>=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.GreaterThanOrEqual, left, right, method);
        }
        /// <summary>
        /// 创建>二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元>表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression GreaterThan(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.GreaterThan, left, right, null);
        }
        /// <summary>
        /// 创建>二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元>表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.GreaterThan, left, right, method);
        }
        /// <summary>
        /// 创建[二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元[表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LessThan(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.LessThan, left, right, null);
        }
        /// <summary>
        /// 创建[二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元[表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.LessThan, left, right, method);
        }
        /// <summary>
        /// 创建[=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元[=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
        {
            return BinaryExpression.Get(ExpressionType.LessThanOrEqual, left, right, null);
        }
        /// <summary>
        /// 创建[=二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="liftToNull">是否可空类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元[=表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.LessThanOrEqual, left, right, method);
        }
        /// <summary>
        /// 创建常量表达式
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>运算符表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ConstantExpression Constant(object value)
        {
            return ConstantExpression.Get(value);
        }
        /// <summary>
        /// 创建常量表达式
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="type">数据类型</param>
        /// <returns>运算符表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ConstantExpression Constant(object value, Type type)
        {
            return ConstantExpression.Get(value);
        }
        /// <summary>
        /// 创建拆箱表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">数据类型</param>
        /// <returns>拆箱表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Unbox(Expression expression, Type type)
        {
            return UnaryExpression.Get(ExpressionType.Unbox, expression, null);
        }
        /// <summary>
        /// 创建-一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>一元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Negate(Expression expression)
        {
            return Negate(expression, null);
        }
        /// <summary>
        /// 创建-一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Negate(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.Negate, expression, method);
        }
        /// <summary>
        /// 创建-一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>一元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression NegateChecked(Expression expression)
        {
            return NegateChecked(expression, null);
        }
        /// <summary>
        /// 创建-一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.NegateChecked, expression, method);
        }
        /// <summary>
        /// 创建+一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>一元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression UnaryPlus(Expression expression)
        {
            return UnaryPlus(expression, null);
        }
        /// <summary>
        /// 创建+一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.UnaryPlus, expression, method);
        }
        /// <summary>
        /// 创建+二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Add(Expression left, Expression right)
        {
            return Add(left, right, null);
        }
        /// <summary>
        /// 创建+二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Add, left, right, method);
        }
        /// <summary>
        /// 创建+二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression AddChecked(Expression left, Expression right)
        {
            return AddChecked(left, right, null);
        }
        /// <summary>
        /// 创建+二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元+表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.AddChecked, left, right, method);
        }
        /// <summary>
        /// 创建-二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            return Subtract(left, right, null);
        }
        /// <summary>
        /// 创建-二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Subtract, left, right, method);
        }
        /// <summary>
        /// 创建-二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression SubtractChecked(Expression left, Expression right)
        {
            return SubtractChecked(left, right, null);
        }
        /// <summary>
        /// 创建-二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元-表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.SubtractChecked, left, right, method);
        }
        /// <summary>
        /// 创建*二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元*表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            return Multiply(left, right, null);
        }
        /// <summary>
        /// 创建*二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元*表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Multiply, left, right, method);
        }
        /// <summary>
        /// 创建*二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元*表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression MultiplyChecked(Expression left, Expression right)
        {
            return MultiplyChecked(left, right, null);
        }
        /// <summary>
        /// 创建*二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元*表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.MultiplyChecked, left, right, method);
        }
        /// <summary>
        /// 创建/二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元/表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Divide(Expression left, Expression right)
        {
            return Divide(left, right, null);
        }
        /// <summary>
        /// 创建/二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元/表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Divide, left, right, method);
        }
        /// <summary>
        /// 创建%二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元%表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Modulo(Expression left, Expression right)
        {
            return Modulo(left, right, null);
        }
        /// <summary>
        /// 创建%二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元%表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Modulo, left, right, method);
        }
        /// <summary>
        /// 创建**二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元**表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Power(Expression left, Expression right)
        {
            return Power(left, right, null);
        }
        /// <summary>
        /// 创建**二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元**表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Power, left, right, method);
        }
        /// <summary>
        /// 创建|二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元|表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Or(Expression left, Expression right)
        {
            return Or(left, right, null);
        }
        /// <summary>
        /// 创建|二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元|表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.Or, left, right, method);
        }
        /// <summary>
        /// 创建&amp;二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元&amp;表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression And(Expression left, Expression right)
        {
            return And(left, right, null);
        }
        /// <summary>
        /// 创建&amp;二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元&amp;表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.And, left, right, method);
        }
        /// <summary>
        /// 创建^二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元^表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression ExclusiveOr(Expression left, Expression right)
        {
            return ExclusiveOr(left, right, null);
        }
        /// <summary>
        /// 创建^二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元^表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.ExclusiveOr, left, right, method);
        }
        /// <summary>
        /// 创建[[二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元[[表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LeftShift(Expression left, Expression right)
        {
            return LeftShift(left, right, null);
        }
        /// <summary>
        /// 创建[[二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元[[表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.LeftShift, left, right, method);
        }
        /// <summary>
        /// 创建>>二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <returns>二元>>表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression RightShift(Expression left, Expression right)
        {
            return RightShift(left, right, null);
        }
        /// <summary>
        /// 创建>>二元表达式
        /// </summary>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元>>表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
        {
            return BinaryExpression.Get(ExpressionType.RightShift, left, right, method);
        }
        /// <summary>
        /// 创建类型转换一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">目标类型</param>
        /// <returns>类型转换一元表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Convert(Expression expression, Type type)
        {
            return ConvertExpression.Get(expression, type, null);
        }
        /// <summary>
        /// 创建类型转换一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">目标类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元类型转换表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
        {
            return ConvertExpression.Get(expression, type, method);
        }
        /// <summary>
        /// 创建类型转换一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">目标类型</param>
        /// <returns>类型转换一元表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression ConvertChecked(Expression expression, Type type)
        {
            return UnaryExpression.Get(ExpressionType.ConvertChecked, expression, null);
        }
        /// <summary>
        /// 创建类型转换一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="type">目标类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元类型转换表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.ConvertChecked, expression, method);
        }
        /// <summary>
        /// 创建条件表达式
        /// </summary>
        /// <param name="test">测试条件</param>
        /// <param name="ifTrue">真表达式</param>
        /// <param name="ifFalse">假表达式</param>
        /// <returns>条件表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
        {
            return ConditionalExpression.Get(test, ifTrue, ifFalse);
        }
        /// <summary>
        /// 创建条件表达式
        /// </summary>
        /// <param name="test">测试条件</param>
        /// <param name="ifTrue">真表达式</param>
        /// <param name="ifFalse">假表达式</param>
        /// <param name="type">表达式结果类型</param>
        /// <returns>条件表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse, Type type)
        {
            return ConditionalExpression.Get(test, ifTrue, ifFalse);
        }
        /// <summary>
        /// 创建真值判定一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>真值判定一元表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression IsTrue(Expression expression)
        {
            return UnaryExpression.Get(ExpressionType.IsTrue, expression, null);
        }
        /// <summary>
        /// 创建真值判定一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元真值判定表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression IsTrue(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.IsTrue, expression, method);
        }
        /// <summary>
        /// 创建假值判定一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>假值判定一元表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression IsFalse(Expression expression)
        {
            return UnaryExpression.Get(ExpressionType.IsFalse, expression, null);
        }
        /// <summary>
        /// 创建假值判定一元表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>假元真值判定表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static UnaryExpression IsFalse(Expression expression, MethodInfo method)
        {
            return UnaryExpression.Get(ExpressionType.IsFalse, expression, method);
        }
        /// <summary>
        /// 创建函数调用表达式
        /// </summary>
        /// <param name="instance">动态函数对象表达式</param>
        /// <param name="method">函数信息</param>
        /// <returns>函数调用表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MethodCallExpression Call(Expression instance, MethodInfo method)
        {
            return MethodCallExpression.Get(method, instance, null);
        }
        /// <summary>
        /// 创建函数调用表达式
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <param name="arguments">调用参数</param>
        /// <returns>函数调用表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
        {
            return MethodCallExpression.Get(method, null, arguments);
        }
        /// <summary>
        /// 创建函数调用表达式
        /// </summary>
        /// <param name="instance">动态函数对象表达式</param>
        /// <param name="method">函数信息</param>
        /// <param name="arguments">调用参数</param>
        /// <returns>函数调用表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
        {
            return MethodCallExpression.Get(method, instance, arguments);
        }
        /// <summary>
        /// 创建函数调用表达式
        /// </summary>
        /// <param name="instance">动态函数对象表达式</param>
        /// <param name="method">函数信息</param>
        /// <param name="arguments">调用参数</param>
        /// <returns>函数调用表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
        {
            return MethodCallExpression.Get(method, instance, arguments.getArray());
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式</returns>
        internal static Expression convert(System.Linq.Expressions.Expression expression)
        {
            Func<System.Linq.Expressions.Expression, Expression> converter = converters[(int)expressionTypes[(int)expression.NodeType]];
            if (converter == null) fastCSharp.log.Default.Throw("不可识别的表达式类型 : " + expression.NodeType.ToString(), new System.Diagnostics.StackFrame(), true);
            return converter(expression);
        }
        /// <summary>
        /// 表达式类型转换集合
        /// </summary>
        private static ExpressionType[] expressionTypes;
        /// <summary>
        /// 表达式类型集合
        /// </summary>
        private static Func<System.Linq.Expressions.Expression, Expression>[] converters;

        static Expression()
        {
            #region 表达式类型转换集合
            expressionTypes = new ExpressionType[Enum.GetMaxValue<System.Linq.Expressions.ExpressionType>(-1) + 1];
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Add] = ExpressionType.Add;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.AddChecked] = ExpressionType.AddChecked;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.And] = ExpressionType.And;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.AndAlso] = ExpressionType.AndAlso;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.ArrayLength] = ExpressionType.ArrayLength;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.ArrayIndex] = ExpressionType.ArrayIndex;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Call] = ExpressionType.Call;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Coalesce] = ExpressionType.Coalesce;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Conditional] = ExpressionType.Conditional;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Constant] = ExpressionType.Constant;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Convert] = ExpressionType.Convert;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.ConvertChecked] = ExpressionType.ConvertChecked;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Divide] = ExpressionType.Divide;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Equal] = ExpressionType.Equal;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.ExclusiveOr] = ExpressionType.ExclusiveOr;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.GreaterThan] = ExpressionType.GreaterThan;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.GreaterThanOrEqual] = ExpressionType.GreaterThanOrEqual;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Invoke] = ExpressionType.Invoke;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Lambda] = ExpressionType.Lambda;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.LeftShift] = ExpressionType.LeftShift;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.LessThan] = ExpressionType.LessThan;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.LessThanOrEqual] = ExpressionType.LessThanOrEqual;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.ListInit] = ExpressionType.ListInit;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.MemberAccess] = ExpressionType.MemberAccess;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.MemberInit] = ExpressionType.MemberInit;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Modulo] = ExpressionType.Modulo;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Multiply] = ExpressionType.Multiply;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.MultiplyChecked] = ExpressionType.MultiplyChecked;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Negate] = ExpressionType.Negate;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.UnaryPlus] = ExpressionType.UnaryPlus;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.NegateChecked] = ExpressionType.NegateChecked;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.New] = ExpressionType.New;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.NewArrayInit] = ExpressionType.NewArrayInit;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.NewArrayBounds] = ExpressionType.NewArrayBounds;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Not] = ExpressionType.Not;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.NotEqual] = ExpressionType.NotEqual;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Or] = ExpressionType.Or;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.OrElse] = ExpressionType.OrElse;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Parameter] = ExpressionType.Parameter;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Power] = ExpressionType.Power;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Quote] = ExpressionType.Quote;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.RightShift] = ExpressionType.RightShift;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.Subtract] = ExpressionType.Subtract;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.SubtractChecked] = ExpressionType.SubtractChecked;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.TypeAs] = ExpressionType.TypeAs;
            expressionTypes[(int)System.Linq.Expressions.ExpressionType.TypeIs] = ExpressionType.TypeIs;
            #endregion

            #region 表达式类型集合
            converters = new Func<System.Linq.Expressions.Expression, Expression>[Enum.GetMaxValue<ExpressionType>(-1) + 1];
            converters[(int)ExpressionType.Add] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Add(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Add(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.AddChecked] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return AddChecked(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return AddChecked(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.And] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return And(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return And(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.AndAlso] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return AndAlso(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return AndAlso(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Call] = expression =>
            {
                System.Linq.Expressions.MethodCallExpression methodCallExpression = (System.Linq.Expressions.MethodCallExpression)expression;
                if (methodCallExpression.Object == null)
                {
                    return Call(methodCallExpression.Method, methodCallExpression.Arguments.getArray(value => convert(value)));
                }
                if (methodCallExpression.Arguments.count() == 0)
                {
                    return Call(convert(methodCallExpression.Object), methodCallExpression.Method);
                }
                return Call(convert(methodCallExpression.Object), methodCallExpression.Method, methodCallExpression.Arguments.getArray(value => convert(value)));
            };
            converters[(int)ExpressionType.Conditional] = expression =>
            {
                System.Linq.Expressions.ConditionalExpression conditionalExpression = (System.Linq.Expressions.ConditionalExpression)expression;
                if (conditionalExpression.Type == null)
                {
                    return Condition(convert(conditionalExpression.Test), convert(conditionalExpression.IfTrue), convert(conditionalExpression.IfFalse));
                }
                return Condition(convert(conditionalExpression.Test), convert(conditionalExpression.IfTrue), convert(conditionalExpression.IfFalse), conditionalExpression.Type);
            };
            converters[(int)ExpressionType.Constant] = expression =>
            {
                System.Linq.Expressions.ConstantExpression constantExpression = (System.Linq.Expressions.ConstantExpression)expression;
                if (constantExpression.Type == null)
                {
                    return Constant(constantExpression.Value);
                }
                return Constant(constantExpression.Value, constantExpression.Type);
            };
            converters[(int)ExpressionType.Convert] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return Convert(convert(unaryExpression.Operand), unaryExpression.Type);
                }
                return Convert(convert(unaryExpression.Operand), unaryExpression.Type, unaryExpression.Method);
            };
            converters[(int)ExpressionType.ConvertChecked] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return ConvertChecked(convert(unaryExpression.Operand), unaryExpression.Type);
                }
                return ConvertChecked(convert(unaryExpression.Operand), unaryExpression.Type, unaryExpression.Method);
            };
            converters[(int)ExpressionType.Divide] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Divide(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Divide(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Equal] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Equal(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Equal(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.ExclusiveOr] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return ExclusiveOr(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return ExclusiveOr(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.GreaterThan] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return GreaterThan(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return GreaterThan(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.GreaterThanOrEqual] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return GreaterThanOrEqual(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return GreaterThanOrEqual(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.Lambda] = expression =>
            {
                System.Linq.Expressions.LambdaExpression lambdaExpression = (System.Linq.Expressions.LambdaExpression)expression;
                return LambdaExpression.Get(convert(lambdaExpression.Body), lambdaExpression.Parameters.getArray(value => ParameterExpression.convert(value)));
            };
            converters[(int)ExpressionType.LeftShift] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return LeftShift(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return LeftShift(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.LessThan] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return LessThan(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return LessThan(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.LessThanOrEqual] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return LessThanOrEqual(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return LessThanOrEqual(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.MemberAccess] = expression =>
            {
                System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)expression;
                FieldInfo field = memberExpression.Member as FieldInfo;
                if (field != null)
                {
                    return Field(convert(memberExpression.Expression), field);
                }
                return Property(convert(memberExpression.Expression), (PropertyInfo)memberExpression.Member);
            };
            converters[(int)ExpressionType.Modulo] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Modulo(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Modulo(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Multiply] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Multiply(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Multiply(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.MultiplyChecked] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return MultiplyChecked(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return MultiplyChecked(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Negate] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return Negate(convert(unaryExpression.Operand));
                }
                return Negate(convert(unaryExpression.Operand), unaryExpression.Method);
            };
            converters[(int)ExpressionType.UnaryPlus] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return UnaryPlus(convert(unaryExpression.Operand));
                }
                return UnaryPlus(convert(unaryExpression.Operand), unaryExpression.Method);
            };
            converters[(int)ExpressionType.NegateChecked] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return NegateChecked(convert(unaryExpression.Operand));
                }
                return NegateChecked(convert(unaryExpression.Operand), unaryExpression.Method);
            };
            converters[(int)ExpressionType.Not] = expression =>
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)expression;
                if (unaryExpression.Method == null)
                {
                    return Not(convert(unaryExpression.Operand));
                }
                return Not(convert(unaryExpression.Operand), unaryExpression.Method);
            };
            converters[(int)ExpressionType.NotEqual] =  expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return NotEqual(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return NotEqual(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.IsLiftedToNull, binaryExpression.Method);
            };
            converters[(int)ExpressionType.Or] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Or(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Or(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.OrElse] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return OrElse(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return OrElse(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Parameter] = expression =>
            {
                System.Linq.Expressions.ParameterExpression parameterExpression = (System.Linq.Expressions.ParameterExpression)expression;
                return Parameter(parameterExpression.Type, parameterExpression.Name);
            };
            converters[(int)ExpressionType.Power] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Power(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Power(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.RightShift] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return RightShift(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return RightShift(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.Subtract] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return Subtract(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return Subtract(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            converters[(int)ExpressionType.SubtractChecked] = expression =>
            {
                System.Linq.Expressions.BinaryExpression binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
                if (binaryExpression.Method == null)
                {
                    return SubtractChecked(convert(binaryExpression.Left), convert(binaryExpression.Right));
                }
                return SubtractChecked(convert(binaryExpression.Left), convert(binaryExpression.Right), binaryExpression.Method);
            };
            #endregion
        }
    }
    /// <summary>
    /// 委托关联表达式
    /// </summary>
    /// <typeparam name="DelegateType">委托类型</typeparam>
    internal sealed class Expression<DelegateType> : LambdaExpression
    {
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            clear();
            typePool<Expression<DelegateType>>.PushNotNull(this);
        }
        /// <summary>
        /// 获取委托关联表达式
        /// </summary>
        /// <param name="body">表达式主体</param>
        /// <param name="parameters">参数</param>
        /// <returns>委托关联表达式</returns>
        internal new static Expression<DelegateType> Get(Expression body, ParameterExpression[] parameters)
        {
            Expression<DelegateType> expression = typePool<Expression<DelegateType>>.Pop() ?? new Expression<DelegateType>();
            expression.set(body, parameters);
            return expression;
        }
    }
}
