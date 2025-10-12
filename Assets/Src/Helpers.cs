#nullable enable
using System;

namespace FUI {
    public static partial class Helpers { 
        public static double DoubleFromString(string value) {
            if (!double.TryParse(value, out var result))
                result = default;
            return result;
        }

        public static int IntFromString(string value) {
            if (!int.TryParse(value, out var result))
                result = default;
            return result;
        }

        public static T ConvertFromString<T>(string value) {
            if (typeof(T) == typeof(string)) return (T)(object)value;
            if (string.IsNullOrEmpty(value)) return default!;

            if (typeof(T) == typeof(int)) return (T)(object)int.Parse(value);
            if (typeof(T) == typeof(double)) return (T)(object)double.Parse(value);
            if (typeof(T) == typeof(float)) return (T)(object)float.Parse(value);

            throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
        }
    }

}
