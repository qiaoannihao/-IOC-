using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc.Config
{
    public class AppConfig : IAppConfig, IAppConfigBuilder
    {
        public string this[string key]
        {
            get
            {
                return GetSection(key);
            }
            set
            {
                SetSection(key, value);
            }
        }
        Dictionary<string, object> KeyValue;
        public string BasePath { get; set; }
        public IList<string> Sources { get; private set; }
        public AppConfig()
        {
            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            Sources = new List<string>();
            Sources.Add(string.Format("{0}appsettings.json", BasePath));
        }
        public void LoadConfig()
        {
            KeyValue = new Dictionary<string, object>();
            for (int i = 0; i < Sources.Count; i++)
            {
                var item = Sources[i];
                if (File.Exists(item))
                {
                    var str = File.ReadAllText(item);
                    var intance = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
                    foreach (var item1 in intance)
                    {
                        KeyValue.Add(item1.Key, item1.Value);
                    }
                }
            }
        }

        public string GetSection(string key)
        {
            if (KeyValue == null)
            {
                LoadConfig();
            }
            if (KeyValue.ContainsKey(key))
            {
                return KeyValue[key].ToString();
            }
            return null;
        }

        public IAppConfig Binding<T>(Action<T> bindingAction) where T : class, new()
        {
            if (KeyValue == null)
            {
                LoadConfig();
            }
            var type = typeof(T);
            if (KeyValue.ContainsKey(type.Name))
            {
                bindingAction((KeyValue[type.Name] as JToken).ToObject(type) as T);
            }
            return this;
        }

        public void SetSection(string key, string value)
        {
            if (KeyValue == null)
            {
                LoadConfig();
            }
            if (KeyValue.ContainsKey(key))
            {
                KeyValue[key] = value;
            }
            else
            {
                KeyValue.Add(key, value);
            }
        }


        public IAppConfigBuilder AddJsonFile(string path)
        {
            Sources.Add(string.Format("{0}{1}", BasePath, path));
            return this;
        }

        public IAppConfigBuilder SetBasePath(string path)
        {
            BasePath = path;
            return this;
        }
    }
}
