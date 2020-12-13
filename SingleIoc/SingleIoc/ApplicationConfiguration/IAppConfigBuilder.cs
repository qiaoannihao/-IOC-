using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc.Config
{
    public interface IAppConfigBuilder
    {
        IList<string> Sources { get; }
        IAppConfigBuilder AddJsonFile(string path);
        IAppConfigBuilder SetBasePath(string path);
    }
}
