using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    /// <summary>
    /// 储存类型，对象容器
    /// </summary>
    public class TypeItemObject
    {
        /// <summary>
        /// 存储类型的集合
        /// </summary>
        public static Dictionary<Type, TypeItemObject> TypeItemObjectCollectionObject { get; protected set; }
        /// <summary>
        /// 单例创建事件集合，用于添加单例判断条件
        /// </summary>
        public static Dictionary<Type, Func<object, bool>> SingletonGenerateEventDic { get; protected set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public Type ServiceType { get; protected set; }
        /// <summary>
        /// 实例类型
        /// </summary>
        public Type ImplementationType { get; protected set; }
        /// <summary>
        /// 生命周期
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; protected set; }
        /// <summary>
        /// 注入方式
        /// </summary>
        public InjectType InjectMode { get; protected set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        public object[] Parameters { get; protected set; }

        private object _implementationTypeInstance = null;

        private Func<Type, object> getInstanceFunc = null;
        /// <summary>
        /// 实例
        /// </summary>
        public object ImplementationTypeInstance
        {
            get
            {
                if (!CheckSingleton(ImplementationType))
                {
                    _implementationTypeInstance = getInstanceFunc(ImplementationType);
                }
                return _implementationTypeInstance;
            }
            protected set
            {
                _implementationTypeInstance = value;
            }
        }
        static TypeItemObject()
        {
            TypeItemObjectCollectionObject = new Dictionary<Type, TypeItemObject>();
            SingletonGenerateEventDic = new Dictionary<Type, Func<object, bool>>();
        }
        public TypeItemObject(object implementationTypeInstance, object[] parameters, ServiceLifetime serviceLifetime, InjectType injectMode)
          : this(implementationTypeInstance.GetType(), implementationTypeInstance, parameters, serviceLifetime, injectMode)
        {
        }
        public TypeItemObject(Type serviceType, object implementationTypeInstance, object[] parameters, ServiceLifetime serviceLifetime, InjectType injectMode)
            : this(serviceType, implementationTypeInstance.GetType(), parameters, serviceLifetime, injectMode)
        {
            _implementationTypeInstance = implementationTypeInstance;

        }
        public TypeItemObject(Type serviceType, Type implementationType, object[] parameters, ServiceLifetime serviceLifetime, InjectType injectMode)
        {
            if (!(serviceType.IsClass || serviceType.IsAbstract || serviceType.IsInterface))
            {
                throw new Exception("不是class类型");
            }
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ServiceLifetime = serviceLifetime;
            Parameters = parameters;
            InjectMode = injectMode;
            if (InjectMode == InjectType.Constructor)
            {
                getInstanceFunc = GetInstance;
            }
            else if (InjectMode == InjectType.Property)
            {
                getInstanceFunc = GetInstanceByProperty;
            }
            else if (InjectMode == (InjectType.Constructor | InjectType.Property))
            {
                getInstanceFunc = GetInstanceByPropertyAndConstructor;
            }
        }

        /// <summary>
        /// 获取实例（构造函数创建方式）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetInstance(Type type)
        {
            ConstructorInfo constructorInfo = null;
            var constructors = type.GetConstructors();
            //判断构造函数的个数
            if (constructors.Length == 1)
            {
                constructorInfo = constructors[0];
            }
            else
            {
                Type iocDefaultAttributeType = typeof(IocDefaultAttribute);
                int count = 0;
                foreach (var item in constructors)
                {
                    if (item.IsDefined(iocDefaultAttributeType))
                    {
                        count++;
                        constructorInfo = item;
                    }
                }
                if (count == 0)
                {
                    //如果有多个构造函数，但是没有标记。则取第一个构造函数
                    constructorInfo = constructors[0];
                }
                else
                if (count != 1)
                {
                    throw new Exception("有多个构造函数标记了IocDefault，或者没有标记IocDefault，会导致容器调用构造函数不明确");
                }
            }
            var res = constructorInfo.GetParameters();
            if (res.Length <= 0)
            {
                return Activator.CreateInstance(type);
            }
            //构造函数的参数注入
            object[] paramsObjects = new object[res.Length];
            int valuetypeIndex = 0;
            for (int i = 0; i < res.Length; i++)
            {
                var tmp = res[i];
                if (tmp.ParameterType.IsValueType || tmp.ParameterType == TypeConst.StringType)
                {
                    //值类型的和字符串注入
                    if (Parameters == null || Parameters.Length == 0 || valuetypeIndex >= Parameters.Length)
                    {
                        //没有配置输入参数，给予默认值
                        if (tmp.ParameterType.IsValueType)
                        {
                            paramsObjects[i] = 0;
                        }
                    }
                    else
                    {
                        //安装顺序配置输入参数
                        paramsObjects[i] = Parameters[valuetypeIndex];
                        valuetypeIndex++;
                    }
                }
                else
                {
                    //如果是类，则寻找容器里已经注册了的类型，没有则返回null
                    var interFaceAndAbstractInstance = TypeItemObjectCollectionObject.GetValue(tmp.ParameterType);
                    if (interFaceAndAbstractInstance != null)
                    {
                        paramsObjects[i] = interFaceAndAbstractInstance.ImplementationTypeInstance;
                    }
                }
            }
            return Activator.CreateInstance(type, paramsObjects);
        }
        /// <summary>
        /// 属性注入
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetInstanceByProperty(Type type)
        {
            object instance = Activator.CreateInstance(type);
            return InjectPropertyFromCollection(instance, type);
        }
        /// <summary>
        /// 属性注入
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object InjectPropertyFromCollection(object instance, Type type)
        {
            var properties = type.GetProperties();
            int valuetypeIndex = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                var item = properties[i];
                if (!item.CanWrite)
                {
                    continue;
                }
                if (item.PropertyType.IsValueType || item.PropertyType == TypeConst.StringType)
                {
                    //值类型的和字符串注入
                    if (!(Parameters == null || Parameters.Length == 0 || valuetypeIndex >= Parameters.Length))
                    {
                        //安装顺序配置输入参数
                        item.SetValue(instance, Parameters[valuetypeIndex]);
                        valuetypeIndex++;
                    }
                }
                else
                {
                    if (item.GetValue(instance) == null)
                    {
                        //如果是类，则寻找容器里已经注册了的类型，没有则返回null
                        var interFaceAndAbstractInstance = TypeItemObjectCollectionObject.GetValue(item.PropertyType);
                        if (interFaceAndAbstractInstance != null)
                        {
                            item.SetValue(instance, interFaceAndAbstractInstance.ImplementationTypeInstance);
                        }
                    }
                }
            }
            return instance;
        }
        /// <summary>
        /// 属性和构造函数注入
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetInstanceByPropertyAndConstructor(Type type)
        {
            object instance = GetInstance(type);
            return InjectPropertyFromCollection(instance, type);
        }
        /// <summary>
        /// 检查当前容器是否返回单例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckSingleton(Type type)
        {
            if (ServiceLifetime == ServiceLifetime.Singleton && _implementationTypeInstance != null)
            {
                if (SingletonGenerateEventDic.Count == 0)
                {
                    return true;
                }
                if (!SingletonGenerateEventDic.ContainsKey(type))
                {
                    return true;
                }
                if (!SingletonGenerateEventDic[type](_implementationTypeInstance))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public static class TypeItemObjectCollectionExtension
    {
        public static Dictionary<Type, TypeItemObject> Add(this Dictionary<Type, TypeItemObject> keyValuePairs, TypeItemObject typeItemObject)
        {
            keyValuePairs.AddAndSet(typeItemObject.ServiceType, typeItemObject);
            return keyValuePairs;
        }
    }
    /// <summary>
    /// 生命周期
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// 单例
        /// </summary>
        Singleton,
        /// <summary>
        /// 新建
        /// </summary>
        Transient
    }

    public enum InjectType
    {
        /// <summary>
        /// 构造函数方式
        /// </summary>
        Constructor = 0x01,
        /// <summary>
        /// 属性注入
        /// </summary>
        Property = 0x02,
    }
}
