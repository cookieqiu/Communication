using Communication.Signals;
using Communication.Signals.Endians;
using System.Runtime.CompilerServices;

namespace Communication.Utils
{
    public class BooleanAccessor
    {
        public static void Set<T>(ref T value, int offset, bool isSet = true) where T : struct
        {
            Validate<T>(offset);
            unsafe
            {
                var pValue = (byte*)Unsafe.AsPointer(ref value);
                if (isSet)
                {
                    pValue[offset >> 3] |= (byte)(1 << (offset & 7));
                }
                else
                {
                    pValue[offset >> 3] &= (byte)~(1 << (offset & 7));
                }
            }
        }

        public static void Set<T, TEndian>(in NumericSignal<T, TEndian> numericSignal, int offset, bool isSet = true)
            where T : struct
            where TEndian : IEndian, new()
        {
            var value = numericSignal.GetValue();
            Set(ref value, offset, isSet);
            numericSignal.SetValue(value);
        }

        public static bool Get<T>(T value, int offset) where T : struct
        {
            Validate<T>(offset);
            unsafe
            {
                var pValue = (byte*)Unsafe.AsPointer(ref value);
                return (pValue[offset >> 3] >> (offset & 7) & 1) == 1;
            }
        }

        public static bool Get<T, TEndian>(in NumericSignal<T, TEndian> numericSignal, int offset)
            where T : struct
            where TEndian : IEndian, new()
        {
            var value = numericSignal.GetValue();
            return Get(value, offset);
        }

        private static void Validate<T>(int offset) where T : struct
        {
            if (offset < 0 || offset >= Unsafe.SizeOf<T>() << 3)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
        }
    }
}
