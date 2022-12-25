using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ESCS.COMMON.ExtensionMethods
{
    public static class DictionaryConvertorExtension
    {
        public static Object GetObject(this IDictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                {
                    value = GetObject((Dictionary<string, object>)value, prop.PropertyType); // <= This line
                }

                prop.SetValue(obj, value, null);
            }
            return obj;
        }
        public static T GetObject<T>(this IDictionary<string, object> dict)
        {
            return (T)GetObject(dict, typeof(T));
        }
        public static IDictionary<string, object> ModelToDictionary<T>(this T model)
        {
            return model.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 .ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
        }
    }
}
