using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    /// <summary>
    /// 简单IOC容器，不支持单一线程
    /// </summary>
    public class IocContainer : IIoc
    {
        public Guid Guid { get; set; }
        public IocContainer()
        {
            Guid = Guid.NewGuid();
            this.AddType<IIoc>(this, ServiceLifetime.Singleton, InjectType.Constructor);
        }
        
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public IIoc AddType(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null)
        {
            TypeItemObject typeItemObject = new TypeItemObject(serviceType, implementationType, parameters, serviceLifetime, injectMode);
            TypeItemObject.TypeItemObjectCollectionObject.Add(typeItemObject);
            return this;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationTypeInstance"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public IIoc AddType(Type serviceType, object implementationTypeInstance, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null)
        {
            TypeItemObject typeItemObject = new TypeItemObject(serviceType, implementationTypeInstance, parameters, serviceLifetime, injectMode);
            TypeItemObject.TypeItemObjectCollectionObject.Add(typeItemObject);
            return this;
        }               
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <returns></returns>
        public ServiceType GetTypeInstance<ServiceType>() where ServiceType : class
        {
            var tmp = TypeItemObject.TypeItemObjectCollectionObject.GetValue(typeof(ServiceType));
            if (tmp == null)
            {
                return null;
            }
            return tmp.ImplementationTypeInstance as ServiceType;
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetTypeInstance(Type type)
        {
            var tmp = TypeItemObject.TypeItemObjectCollectionObject.GetValue(type);
            if (tmp == null)
            {
                return null;
            }
            return tmp.ImplementationTypeInstance;
        }
       
        /// <summary>
        /// 添加单例判断条件
        /// </summary>
        /// <param name="type">需要特殊配置单例条件的类型</param>
        /// <param name="condition">输入参数：当前实例。输出参数：返回True为创建新对象</param>
        /// <returns></returns>
        public IIoc AddSingletonCondition(Type type, Func<object, bool> condition)
        {
            foreach (var item in TypeItemObject.TypeItemObjectCollectionObject)
            {
                if (item.Value.ImplementationType.CheckType(type))
                {
                    if (TypeItemObject.SingletonGenerateEventDic.ContainsKey(item.Value.ImplementationType))
                    {
                        TypeItemObject.SingletonGenerateEventDic[item.Value.ImplementationType] += condition;
                    }
                    else
                    {
                        TypeItemObject.SingletonGenerateEventDic.Add(item.Value.ImplementationType, condition);
                    }
                }
            }
            return this;
        }

        public IIoc ClearSingletonCondition()
        {
            TypeItemObject.SingletonGenerateEventDic.Clear();
            return this;
        }
    }
}
