using System;
using System.Linq;

namespace Zx.Web.ApiContainer.external
{
    internal static class TypeHelpers
    {
        //public static readonly Assembly MsCorLibAssembly = typeof(string).Assembly;
        //public static readonly Assembly MvcAssembly = typeof(Controller).Assembly;
        //public static readonly Assembly SystemWebAssembly = typeof(HttpContext).Assembly;

        public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> matchesInterface = t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType;
            return (matchesInterface(queryType)) ? queryType : queryType.GetInterfaces().FirstOrDefault(matchesInterface);
        }

        public static object GetDefaultValue(Type type)
        {
            return (TypeAllowsNullValue(type)) ? null : Activator.CreateInstance(type);
        }

        public static bool IsCompatibleObject<T>(object value)
        {
            return (value is T || (value == null && TypeAllowsNullValue(typeof(T))));
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }
    }
}