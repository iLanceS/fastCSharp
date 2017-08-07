using System;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 数据库表格模型
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class databaseModel<valueType>
    {
#if NOJIT
        /// <summary>
        /// 获取关键字获取器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static Func<valueType, keyType> GetPrimaryKeyGetter<keyType>(string name, FieldInfo[] primaryKeys)
        {
            switch (primaryKeys.Length)
            {
                case 0: return null;
                case 1: return new primaryKey { Field = primaryKeys[0] }.Get<keyType>;
                default: return new primaryKey<keyType>(primaryKeys).Get;
            }
        }
        /// <summary>
        /// 获取关键字设置器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static Action<valueType, keyType> GetPrimaryKeySetter<keyType>(string name, FieldInfo[] primaryKeys)
        {
            switch (primaryKeys.Length)
            {
                case 0: return null;
                case 1: return new primaryKey { Field = primaryKeys[0] }.Set<keyType>;
                default: return new primaryKey<keyType>(primaryKeys).Set;
            }
        }
        /// <summary>
        /// 关键字
        /// </summary>
        private sealed class primaryKey
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public FieldInfo Field;
            /// <summary>
            /// 获取关键字
            /// </summary>
            /// <typeparam name="keyType"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public keyType Get<keyType>(valueType value)
            {
                return (keyType)Field.GetValue(value);
            }
            /// <summary>
            /// 设置关键字
            /// </summary>
            /// <typeparam name="keyType"></typeparam>
            /// <param name="value"></param>
            /// <param name="key"></param>
            public void Set<keyType>(valueType value, keyType key)
            {
                Field.SetValue(value, key);
            }
        }
        /// <summary>
        /// 关键字
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        private sealed class primaryKey<keyType>
        {
            /// <summary>
            /// 关键字集合
            /// </summary>
            private keyValue<FieldInfo, FieldInfo>[] fields;
            /// <summary>
            /// 关键字
            /// </summary>
            /// <param name="fields"></param>
            public primaryKey(FieldInfo[] fields)
            {
                this.fields = new keyValue<FieldInfo, FieldInfo>[fields.Length];
                int index = 0;
                foreach (FieldInfo field in fields) this.fields[index++].Set(field, typeof(keyType).GetField(field.Name, BindingFlags.Instance | BindingFlags.Public));
            }
            /// <summary>
            /// 获取关键字
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public keyType Get(valueType value)
            {
                object key = default(keyType);
                foreach (keyValue<FieldInfo, FieldInfo> field in fields) field.Value.SetValue(key, field.Key.GetValue(value));
                return (keyType)key;
            }
            /// <summary>
            /// 设置关键字
            /// </summary>
            /// <param name="value"></param>
            /// <param name="key"></param>
            public void Set(valueType value, keyType key)
            {
                object keyObject = key;
                foreach (keyValue<FieldInfo, FieldInfo> field in fields) field.Key.SetValue(value, field.Value.GetValue(keyObject));
            }
        }
#else
        /// <summary>
        /// 获取关键字获取器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        internal static Func<valueType, keyType> GetPrimaryKeyGetter<keyType>(string name, FieldInfo[] primaryKeys)
        {
            if (primaryKeys.Length == 0) return null;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(keyType), new Type[] { typeof(valueType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (primaryKeys.Length == 1)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, primaryKeys[0]);
            }
            else
            {
                LocalBuilder key = generator.DeclareLocal(typeof(keyType));
                generator.Emit(OpCodes.Ldloca_S, key);
                generator.Emit(OpCodes.Initobj, typeof(keyType));
                foreach (FieldInfo primaryKey in primaryKeys)
                {
                    generator.Emit(OpCodes.Ldloca_S, key);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, primaryKey);
                    generator.Emit(OpCodes.Stfld, typeof(keyType).GetField(primaryKey.Name, BindingFlags.Instance | BindingFlags.Public));
                }
                generator.Emit(OpCodes.Ldloc_0);
            }
            generator.Emit(OpCodes.Ret);
            return (Func<valueType, keyType>)dynamicMethod.CreateDelegate(typeof(Func<valueType, keyType>));
        }
        /// <summary>
        /// 获取关键字设置器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        internal static Action<valueType, keyType> GetPrimaryKeySetter<keyType>(string name, FieldInfo[] primaryKeys)
        {
            if (primaryKeys.Length == 0) return null;
            DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[] { typeof(valueType), typeof(keyType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (primaryKeys.Length == 1)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Stfld, primaryKeys[0]);
            }
            else
            {
                foreach (FieldInfo primaryKey in primaryKeys)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarga_S, 1);
                    generator.Emit(OpCodes.Ldfld, typeof(keyType).GetField(primaryKey.Name, BindingFlags.Instance | BindingFlags.Public));
                    generator.Emit(OpCodes.Stfld, primaryKey);
                }
            }
            generator.Emit(OpCodes.Ret);
            return (Action<valueType, keyType>)dynamicMethod.CreateDelegate(typeof(Action<valueType, keyType>));
        }
        /// <summary>
        /// 获取自增字段获取器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        protected static Func<valueType, int> getIdentityGetter32(string name, FieldInfo field)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(int), new Type[] { typeof(valueType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            if (field.FieldType != typeof(int) && field.FieldType != typeof(uint)) generator.Emit(OpCodes.Conv_I4);
            generator.Emit(OpCodes.Ret);
            return (Func<valueType, int>)dynamicMethod.CreateDelegate(typeof(Func<valueType, int>));
        }
        /// <summary>
        /// 获取自增字段设置器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        protected static Action<valueType, int> getIdentitySetter32(string name, FieldInfo field)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[] { typeof(valueType), typeof(int) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            if (field.FieldType == typeof(long) || field.FieldType == typeof(ulong)) generator.Emit(OpCodes.Conv_I8);
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
            return (Action<valueType, int>)dynamicMethod.CreateDelegate(typeof(Action<valueType, int>));
        }
#endif
    }
}
