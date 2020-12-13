using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    public interface IIoc
    {        
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IIoc AddType(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationTypeInstance"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        IIoc AddType(Type serviceType, object implementationTypeInstance, ServiceLifetime serviceLifetime, InjectType injectMode, object[] parameters = null);       
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        /// <returns></returns>
        ServiceType GetTypeInstance<ServiceType>() where ServiceType : class;
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object GetTypeInstance(Type type);        
        /// <summary>
        /// 添加单例判断条件
        /// </summary>
        /// <param name="type">需要特殊配置单例条件的类型</param>
        /// <param name="condition">输入参数：当前实例。输出参数：返回True为创建新对象</param>
        /// <returns></returns>
        IIoc AddSingletonCondition(Type type, Func<object, bool> condition);
        IIoc ClearSingletonCondition();
    }
}
