using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc.Option
{
    public class AppOptions<T> : IAppOptions<T> where T : class, new()
    {
        public AppOptions(T value)
        {
            Value = value;
        }
        public T Value
        {
            get; private set;
        }
    }
}
