
using SingleIoc.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    public class ApplicationBuilder
    {
        public static IApplicationServices ApplicationServices { get; protected set; }
        static object startUpInstance = null;
        static ApplicationBuilder applicationBuilder = null;
        static object applicationBuilderLock = new object();

        static ApplicationBuilder()
        {
            ApplicationServices = new ApplicationServices();
        }

        public static ApplicationBuilder Create()
        {
            if (applicationBuilder == null)
            {
                lock (applicationBuilderLock)
                {
                    if (applicationBuilder == null)
                    {
                        applicationBuilder = new ApplicationBuilder();
                        applicationBuilder.UseIocContainer(new IocContainer());
                    }
                }
            }
            return applicationBuilder;
        }
        public bool Run()
        {
            if (startUpInstance == null)
            {
                return false;
            }
            var type = startUpInstance.GetType();
            var res = type.GetMethod("ConfigureServices");
            var res1 = res.GetParameters();
            object[] paramsArr = new object[res1.Length];
            for (int i = 0; i < paramsArr.Length; i++)
            {
                paramsArr[i] = ApplicationServices.IocContainer.GetTypeInstance(res1[i].ParameterType);
            }
            res.Invoke(startUpInstance, paramsArr);
            return true;
        }
        public ApplicationBuilder UseAppconfig(Action<IAppConfig> appConfig)
        {
            ApplicationServices.ApplicationConfig = new AppConfig();
            appConfig(ApplicationServices.ApplicationConfig);
            ApplicationServices.IocContainer.AddSingleton<IAppConfig>(ApplicationServices.ApplicationConfig);
            return this;
        }
        public ApplicationBuilder UseAppconfig(IAppConfig appConfig)
        {
            ApplicationServices.ApplicationConfig = appConfig;
            ApplicationServices.IocContainer.AddSingleton<IAppConfig>(ApplicationServices.ApplicationConfig);
            return this;
        }
        public ApplicationBuilder UseIocContainer(IIoc container)
        {
            ApplicationServices.IocContainer = container;
            return this;
        }
     
        public ApplicationBuilder UseStartUp(Type startupType, params object[] paramsObj)
        {
            CreateStartUpInstance(startupType, paramsObj);
            return this;
        }
        public ApplicationBuilder UseStartUp<StartUpType>(params object[] paramsObj) where StartUpType : class
        {
            return UseStartUp(typeof(StartUpType), paramsObj);
        }
        private void CreateStartUpInstance(Type startupType, params object[] paramsObj)
        {
            if (ApplicationServices.ApplicationConfig == null)
            {
                UseAppconfig(appConfig =>
                {
                });
            }
            ApplicationServices.IocContainer.AddSingleton<IApplicationServices>(ApplicationServices);
            ApplicationServices.IocContainer.AddSingleton(startupType, startupType, paramsObj);
            startUpInstance = ApplicationServices.IocContainer.GetTypeInstance(startupType);
        }
    }

    public class ApplicationServices : IApplicationServices
    {
        public IIoc IocContainer { get; set; }
        public IAppConfig ApplicationConfig { get; set; }

    }
    public interface IApplicationServices
    {
        IIoc IocContainer { get; set; }
        IAppConfig ApplicationConfig { get; set; }
    }

}
