using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleIoc.Option;
using SingleIoc.Config;

namespace SingleIoc.OptionsExtension
{
    public static class OptionsExtension
    {
        public static IIoc Configure<T>(this IIoc ioc, IAppConfig appConfig) where T : class, new()
        {
            appConfig.Binding<T>(obj =>
            {
                ioc.AddSingleton<IAppOptions<T>>(new AppOptions<T>(obj));
            });
            return ioc;
        }
    }
}
