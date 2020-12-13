using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc.Option
{
    public interface IAppOptions<T> where T : class, new()
    {
        T Value { get; }
    }
}
