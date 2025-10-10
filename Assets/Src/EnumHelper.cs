using System;
using System.Collections.Generic;


namespace FUI {

    namespace Gears {
        public static class EnumHelper {

            public static Dictionary<int, int> ValueToIndex<E>() {
                return ValueToIndex(typeof(E));
            }
            public static Dictionary<int, int> ValueToIndex(Type enumType) {
                var result = new Dictionary<int, int>();
                var enumValues = (int[])Enum.GetValues(enumType);
                for (int i = 0; i < enumValues.Length; i++) {
                    result[enumValues[i]] = i;
                }
                return result;
            }

            public static Dictionary<int, int> IndexToValue<E>() {
                return IndexToValue(typeof(E));
            }
            public static Dictionary<int, int> IndexToValue(Type enumType) {
                var result = new Dictionary<int, int>();
                var enumValues = (int[])Enum.GetValues(enumType);
                for (int i = 0; i < enumValues.Length; i++) {
                    result[i] = enumValues[i];
                }
                return result;
            }


            public static int ValueToIndex(object value) {
                int intValue = Convert.ToInt32(value);
                var _values = (int[])Enum.GetValues(value.GetType());
                var valueIndex = Array.IndexOf(_values, value);
                if (valueIndex == -1) {
                    //find the closest value
                    valueIndex = 0;
                    var diff = Math.Abs(_values[0] - intValue);
                    for (int i = 1; i < _values.Length; i++) {
                        var v = _values[i];
                        var newDiff = Math.Abs(v - intValue);
                        if (newDiff < diff) {
                            diff = newDiff;
                            valueIndex = i;
                        }
                    }
                }
                return valueIndex;
            }
            public static E IndexToValue<E>(int index) {
                var _values = (E[])Enum.GetValues(typeof(E));
                if (index < 0 || index >= _values.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for enum values.");
                return _values[index];
            }
            public static object IndexToValue(Type enumType, int index) {
                var values = Enum.GetValues(enumType);
                if (index < 0 || index >= values.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for enum values.");
                return values.GetValue(index)!;
            }

            public static string[] Names<E>() {
                return Enum.GetNames(typeof(E));
            }
            public static string[] Names(Type enumType) {
                return Enum.GetNames(enumType);
            }

            public static string[] NamesTrimmed<E>() {
                var names = Enum.GetNames(typeof(E));
                return Array.ConvertAll(names, n => n.Trim('_'));
            }
            public static string[] NamesTrimmed(Type enumType) {
                var names = Enum.GetNames(enumType);
                return Array.ConvertAll(names, n => n.Trim('_'));
            }

        }


        public class EnumHelper<E> {
            string[] _names;
            E[] _values;
            public string[] Names => _names;
            public string[] NamesTrimmed => Array.ConvertAll(_names, n => n.Trim('_'));
            public EnumHelper() {
                _names = Enum.GetNames(typeof(E));
                _values = (E[])Enum.GetValues(typeof(E));
            }

            public int ValueToIndex(E value) {
                int intValue = Convert.ToInt32(value);
                var valueIndex = Array.IndexOf(_values, value);
                if (valueIndex == -1) {
                    //find the closest value
                    valueIndex = 0;
                    var diff = Math.Abs(Convert.ToInt32(_values[0]) - intValue);
                    for (int i = 1; i < _values.Length; i++) {
                        var v = Convert.ToInt32(_values[i]);
                        var newDiff = Math.Abs(v - intValue);
                        if (newDiff < diff) {
                            diff = newDiff;
                            valueIndex = i;
                        }
                    }
                }
                return valueIndex;
            }

            public E IndexToValue(int index) {
                if (index < 0 || index >= _values.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for enum values.");
                return _values[index];
            }
        }
    }
}
