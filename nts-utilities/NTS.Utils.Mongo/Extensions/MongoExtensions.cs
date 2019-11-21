using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.Mongo.Extensions
{
    public static class MongoExtensions
    {
        public static void ThrowIfNullOrEmpty(this string @object, string parameterName)
        {
            @object.ThrowIfNull(parameterName);
            if (string.IsNullOrWhiteSpace(@object))
                throw new ArgumentException("Argument can't be null or empty", parameterName);
        }

        public static void ThrowIfNull<T>(this T value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static string GetStringValue<T>(this IEnumerable<T> values, string separator = ",")
        {
            return string.Join(separator, values.EmptyIfNull().Select(e => e.ToString()));
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> xs)
        {
            if (xs == null)
            {
                return new T[0];
            }

            return xs;
        }

        public static string F(this string format, params object[] args)
        {
            format.ThrowIfNull("format");
            return string.Format(format, args);
        }
    }
}
