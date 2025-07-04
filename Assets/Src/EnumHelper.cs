using System;


namespace FUI {

    namespace Gears {
        public class EnumHelper<T> where T : struct, Enum {            
            string[] _names;
            T[] _values;
            public string[] Names => _names;
            public EnumHelper() {
                _names = Enum.GetNames(typeof(T));
                _values = (T[])Enum.GetValues(typeof(T));
            }

            public int ValueToIndex(T value) {
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

            public T IndexToValue(int index) {
                if (index < 0 || index >= _values.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for enum values.");
                return _values[index];
            }

        }
    }
}
