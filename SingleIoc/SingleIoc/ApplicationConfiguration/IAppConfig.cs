using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc.Config
{
    public interface IAppConfig
    {
        string this[string key] { get; set; }
        void LoadConfig();
        string GetSection(string key);
        IAppConfig Binding<T>(Action<T> bindingAction) where T : class, new();
    }
}
