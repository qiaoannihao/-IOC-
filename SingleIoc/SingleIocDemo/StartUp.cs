using SingleIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleIocDemo
{
    public class StartUp
    {
        public StartUp()
        {

        }
        public void ConfigureServices(IApplicationServices services)
        {
            services.IocContainer
                .RegisterTypeByAssembly(Assembly.GetExecutingAssembly())
                .AddTransient<School>()
                .AddTransient<Class>();
        }
    }
}
