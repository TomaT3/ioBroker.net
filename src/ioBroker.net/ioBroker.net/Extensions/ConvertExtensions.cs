using ioBroker.net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ioBroker.net.Extensions
{
    internal static class ConvertExtensions
    {
        internal static T ConvertTo<T>(this object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal static T GetConvertedValue<T>(this State state)
        {
            var jsonElementVal = (JsonElement)state.Val;
            var value = jsonElementVal.Deserialize(typeof(T));
            return value.ConvertTo<T>();
        }
    }
}
