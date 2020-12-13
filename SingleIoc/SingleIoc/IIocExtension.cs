using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    public static class IIocExtension
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <typeparam name="ImplementationType"></typeparam>
        /// <returns></returns>
        public static IIoc AddSingleton<ServiceType, ImplementationType>(this IIoc ioc, object[] parameters = null)
            where ServiceType : class
            where ImplementationType : class
        {
            ioc.AddSingleton(typeof(ServiceType), typeof(ImplementationType), parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public static IIoc AddSingleton(this IIoc ioc, Type serviceType, Type implementationType, object[] parameters = null)
        {
            ioc.AddType(serviceType, implementationType, ServiceLifetime.Singleton, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationTypeInstance"></param>
        /// <returns></returns>
        public static IIoc AddSingleton(this IIoc ioc, Type serviceType, object implementationTypeInstance, object[] parameters = null)
        {
            ioc.AddType(serviceType, implementationTypeInstance, ServiceLifetime.Singleton, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="implementationTypeInstance"></param>
        /// <returns></returns>
        public static IIoc AddSingleton<ServiceType>(this IIoc ioc, ServiceType implementationTypeInstance, object[] parameters = null) where ServiceType : class
        {
            ioc.AddType(implementationTypeInstance, ServiceLifetime.Singleton, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <returns></returns>
        public static IIoc AddSingleton<ServiceType>(this IIoc ioc, object[] parameters = null) where ServiceType : class
        {
            ioc.AddType<ServiceType, ServiceType>(ServiceLifetime.Singleton, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <typeparam name="ImplementationType"></typeparam>
        /// <returns></returns>
        public static IIoc AddTransient<ServiceType, ImplementationType>(this IIoc ioc, object[] parameters = null)
            where ServiceType : class
            where ImplementationType : class
        {
            ioc.AddTransient(typeof(ServiceType), typeof(ImplementationType), parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public static IIoc AddTransient(this IIoc ioc, Type serviceType, Type implementationType, object[] parameters = null)
        {
            ioc.AddType(serviceType, implementationType, ServiceLifetime.Transient, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationTypeInstance"></param>
        /// <returns></returns>
        public static IIoc AddTransient(this IIoc ioc, Type serviceType, object implementationTypeInstance, object[] parameters = null)
        {
            ioc.AddType(serviceType, implementationTypeInstance, ServiceLifetime.Transient, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="implementationTypeInstance"></param>
        /// <returns></returns>
        public static IIoc AddTransient<ServiceType>(this IIoc ioc, ServiceType implementationTypeInstance, object[] parameters = null) where ServiceType : class
        {
            ioc.AddType(implementationTypeInstance, ServiceLifetime.Transient, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <returns></returns>
        public static IIoc AddTransient<ServiceType>(this IIoc ioc, object[] parameters = null) where ServiceType : class
        {
            ioc.AddType<ServiceType, ServiceType>(ServiceLifetime.Transient, InjectType.Constructor, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <typeparam name="ImplementationType"></typeparam>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IIoc AddType<ServiceType, ImplementationType>(this IIoc ioc, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null)
            where ServiceType : class
            where ImplementationType : class
        {
            ioc.AddType(typeof(ServiceType), typeof(ImplementationType), serviceLifetime, injectMode, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="implementationTypeInstance"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IIoc AddType<ServiceType>(this IIoc ioc, ServiceType implementationTypeInstance, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null)
            where ServiceType : class
        {
            ioc.AddType(typeof(ServiceType), implementationTypeInstance, serviceLifetime, injectMode, parameters);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IIoc RegisterTypeByAssembly(this IIoc ioc, params Assembly[] assemblies)
        {
            ioc.RegisterTypeByAssembly(ServiceLifetime.Transient, InjectType.Constructor, assemblies);
            return ioc;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IIoc RegisterTypeByAssembly(this IIoc ioc, ServiceLifetime serviceLifetime, InjectType injectMode, params Assembly[] assemblies)
        {
            if (assemblies.Length <= 0)
            {
                return ioc;
            }
            Type iocIgnoreAttributeType = typeof(IocIgnoreAttribute);
            Type liftTimeAttributeType = typeof(LiftTimeAttribute);
            Type injectType = typeof(InjectAttribute);
            ServiceLifetime liftTime;
            InjectType injectModeTmp;
            foreach (var item in assemblies)
            {
                foreach (var item1 in item.ExportedTypes)
                {
                    if (item1.IsClass && !item1.IsAbstract)
                    {
                        if (item1.IsDefined(iocIgnoreAttributeType))
                        {
                            continue;
                        }
                        if (item1.IsDefined(liftTimeAttributeType))
                        {
                            var liftTimeAttribute = item1.GetCustomAttribute<LiftTimeAttribute>();
                            liftTime = liftTimeAttribute.ServiceLifetime;
                        }
                        else
                        {
                            liftTime = serviceLifetime;
                        }
                        if (item1.IsDefined(injectType))
                        {
                            var obj = item1.GetCustomAttribute<InjectAttribute>();
                            injectModeTmp = obj.InjectMode;
                        }
                        else
                        {
                            injectModeTmp = injectMode;
                        }
                        ioc.AddType(item1, item1, liftTime, injectModeTmp);
                    }
                }
            }
            return ioc;
        }

        /// <summary>
        /// 添加单例判断条件
        /// </summary>
        /// <typeparam name="ConditionType">需要特殊配置单例条件的类型</typeparam>
        /// <param name="condition">输入参数：当前实例。输出参数：返回True为创建新对象</param>
        /// <returns></returns>
        public static IIoc AddSingletonCondition<ConditionType>(this IIoc ioc, Func<ConditionType, bool> condition) where ConditionType : class
        {
            ioc.AddSingletonCondition(typeof(ConditionType), objectInstance =>
            {
                return condition(objectInstance as ConditionType);
            });
            return ioc;
        }
    }
}
