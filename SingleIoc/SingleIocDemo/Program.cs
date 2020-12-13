using SingleIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIocDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationBuilder.Create().UseStartUp<StartUp>().Run();

            var school = ApplicationBuilder.ApplicationServices.IocContainer.GetTypeInstance<School>();
            var class1 = school.Class;
            

        }
    }

}
