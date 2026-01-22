using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TestingNumbers.Extensions {
    public static class ReflectionExtensions {
        public static T GetPrivateField<T>(this object obj, string propertyName) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (propertyName == null) {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var fieldInfo = obj.GetType().GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null) {
                throw new ArgumentOutOfRangeException("propertyName", $"Couldn't find property {propertyName} in type {obj.GetType().FullName}");
            }

            return (T)fieldInfo.GetValue(obj);
        }
    }
}
