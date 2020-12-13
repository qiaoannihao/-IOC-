using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    public static class TypeExtension
    {
        /// <summary>
        /// 检查是否指定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool CheckType(this Type type, string comparison)
        {
            if (type.FullName == comparison)
            {
                return true;
            }
            if (type.BaseType == null)
            {
                return false;
            }
            return CheckType(type.BaseType, comparison);
        }
        /// <summary>
        /// 检查是否指定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool CheckType(this Type type, Type comparison)
        {
            if (type == comparison)
            {
                return true;
            }
            if (type.BaseType == null)
            {
                return false;
            }
            return CheckType(type.BaseType, comparison);
        }
        /// <summary>
        /// 检查是否指定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool CheckType(this Type type, Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (type.CheckType(types[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查是否指定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool CheckType(this Type type, List<Type> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                if (type.CheckType(types[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classGetter"></param>
        /// <returns></returns>
        public static object GenerateInstance(this Type type, Func<ParameterInfo, object> classGetter)
        {
            ConstructorInfo constructorInfo = null;
            var constructors = type.GetConstructors();
            //判断构造函数的个数
            if (constructors.Length > 0)
            {
                constructorInfo = constructors[0];
            }
            var res = constructorInfo.GetParameters();
            if (res.Length <= 0)
            {
                return Activator.CreateInstance(type);
            }
            //构造函数的参数注入
            object[] paramsObjects = new object[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                paramsObjects[i] = classGetter(res[i]);
            }
            return Activator.CreateInstance(type, paramsObjects);
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GenerateInstance(this Type type)
        {
            return type.GenerateInstance(parameterInfo =>
            {
                if (parameterInfo.ParameterType == TypeConst.StringType
                || parameterInfo.ParameterType.IsAbstract
                || parameterInfo.ParameterType.IsArray
                || parameterInfo.ParameterType.IsInterface)
                {
                    return null;
                }
                return parameterInfo.ParameterType.GenerateInstance();
                if (parameterInfo.ParameterType.IsValueType)
                {
                    return 0;
                }
                else if (parameterInfo.ParameterType.IsClass
                && !parameterInfo.ParameterType.IsAbstract
                && !parameterInfo.ParameterType.IsArray
                && !parameterInfo.ParameterType.IsInterface)
                {
                    if (parameterInfo.ParameterType != TypeConst.StringType)
                    {
                        return parameterInfo.ParameterType.GenerateInstance();
                    }
                }
                return null;
            });
        }
        /// <summary>
        /// 获取指定属性的获取设置方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyFunctionGetter"></param>
        /// <returns></returns>
        public static bool PropertyFunction<T, TProperty>(this Type type, string propertyName,
            Action<Func<T, TProperty>, Action<T, TProperty>> propertyFunctionGetter)
            where T : class
        {
            var getMethod = type.GetMethod(string.Format("get_{0}", propertyName));
            if (getMethod == null)
            {
                return false;
            }
            var setMethod = type.GetMethod(string.Format("set_{0}", propertyName));
            if (setMethod == null)
            {
                return false;
            }
            propertyFunctionGetter(
                Delegate.CreateDelegate(typeof(Func<T, TProperty>), getMethod) as Func<T, TProperty>,
                Delegate.CreateDelegate(typeof(Action<T, TProperty>), setMethod) as Action<T, TProperty>);

            return true;
        }
        /// <summary>
        /// 获取指定属性的改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="eventFunctionGetter"></param>
        /// <returns></returns>
        public static bool EventFunction<T, TProperty>(this Type type, string propertyName,
            Action<EventInfo> eventFunctionGetter)
            where T : class
        {
            var eventObject = type.GetEvent(propertyName);
            if (eventObject != null)
            {
                eventFunctionGetter(eventObject);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取类型指定Info数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public static void ForeachTypeInfo<T>(this Type type, Func<Type, T[]> func, Action<T> callback) where T : class
        {
            var arr = func(type);
            for (int i = 0; i < arr.Length; i++)
            {
                callback(arr[i]);
            }
        }
        /// <summary>
        /// 获取类型属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static void ForeachPropertyInfo(this Type type, Action<PropertyInfo> callback)
        {
            Func<Type, PropertyInfo[]> getFunc = t => t.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (type.BaseType != null)
            {
                type.BaseType.ForeachTypeInfo(getFunc, callback);
            }
            type.ForeachTypeInfo(getFunc, callback);
        }
        /// <summary>
        /// 获取类型属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="currentDeep">当前继承深度</param>
        /// <param name="maxDeep">最大继承深度</param>
        /// <param name="callback"></param>
        public static void ForeachPropertyInfo(this Type type, int currentDeep, int maxDeep, Action<PropertyInfo> callback)
        {
            Func<Type, PropertyInfo[]> getFunc = t => t.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (currentDeep < maxDeep)
            {
                currentDeep++;
                if (type.BaseType != null)
                {
                    type.BaseType.ForeachPropertyInfo(currentDeep, maxDeep, callback);
                }
            }
            type.ForeachTypeInfo(getFunc, callback);
        }
        /// <summary>
        /// 获取类型属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deepLevel">继承深度</param>
        /// <param name="callback"></param>
        public static void ForeachPropertyInfo(this Type type, int deepLevel, Action<PropertyInfo> callback)
        {
            type.ForeachPropertyInfo(0, deepLevel, callback);
        }
    }

    public static class TypeConst
    {
        private static Type stringType;
        public static Type StringType
        {
            get
            {
                if (stringType == null)
                {
                    stringType = typeof(string);
                }
                return stringType;
            }
        }
        private static Type delegateType;
        public static Type DelegateType
        {
            get
            {
                if (delegateType == null)
                {
                    delegateType = typeof(Delegate);
                }
                return delegateType;
            }
        }
    }
}
