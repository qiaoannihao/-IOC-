using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    /// <summary>
    /// 在注册整个程序集的类型时，忽略注册的特性
    /// </summary>
    public class IocIgnoreAttribute : Attribute
    {

    }
    /// <summary>
    /// 默认的构造函数
    /// </summary>
    public class IocDefaultAttribute : Attribute
    {

    }
    /// <summary>
    /// 在注册整个程序集的类型时，设置的生命周期
    /// </summary>
    public class LiftTimeAttribute : Attribute
    {
        public ServiceLifetime ServiceLifetime { get; protected set; }
        public LiftTimeAttribute(ServiceLifetime serviceLifetime)
        {
            ServiceLifetime = serviceLifetime;
        }
    }
    /// <summary>
    /// 在注册整个程序集的类型时，设置的注入方式
    /// </summary>
    public class InjectAttribute : Attribute
    {
        public InjectType InjectMode { get; protected set; }
        public InjectAttribute(InjectType injectMode)
        {
            InjectMode = injectMode;
        }
    }
}
