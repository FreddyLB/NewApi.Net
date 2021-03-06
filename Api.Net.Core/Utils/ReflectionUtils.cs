using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class ReflectionUtils
    {
        public static PropertyInfo SelectMatch(this IEnumerable<PropertyInfo> properties, string propertyName)
        {
            return properties.FirstOrDefault(t => String.Equals(t.Name, propertyName, StringComparison.OrdinalIgnoreCase));
        }

        public static void SetValue(this object obj, string property, object value)
        {
            var _property = obj.GetType().GetProperty(property);
            if (_property == null) return;

            _property.SetValue(obj, value);
        }

        public static object GetValue(this object obj, string property)
        {
            if (obj == null) return null;
            var _property = obj.GetType().GetProperty(property);
            if (_property == null) return null;

            return _property.GetValue(obj, null);
        }

        public static T GetValue<T>(this object obj, string property)
        {
            if (obj == null)
                return default(T);

            var _property = obj.GetType().GetProperty(property);
            if (_property == null) return default(T);

            return (T)_property.GetValue(obj, null);
        }
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            // null does not have base type
            if (type == null)
            {
                return false;
            }

            // only interface or object can have null base type
            if (baseType == null)
            {
                return type.IsInterface || type == typeof(object);
            }

            // check implemented interfaces
            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
