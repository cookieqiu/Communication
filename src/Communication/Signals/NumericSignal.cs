using Communication.Signals.Endians;
using System.Runtime.CompilerServices;

namespace Communication.Signals
{
    public class NumericSignal<T, TEndian> : ISignal
        where T : struct
        where TEndian : IEndian, new()
    {
        private T _value;
        private readonly TEndian _endian;

        private readonly int _size;
        public int Size => _size;

        public NumericSignal() : this(default) { }

        public NumericSignal(T value)
        {
            _value = value;
            _endian = new TEndian();
            _size = Unsafe.SizeOf<T>();
        }

        public void SetValue(T value) => _value = value;

        public T GetValue() => _value;

        public int Write(in byte[] buffer, int offset)
        {
            var data = new byte[_size];
            Unsafe.WriteUnaligned(ref data[0], _value);
            _endian.Write(buffer, data, offset, _size);
            return _size;
        }

        public int Read(in byte[] buffer, int offset)
        {
            var data = _endian.Read(buffer, offset, _size);
            _value = Unsafe.ReadUnaligned<T>(ref data[0]);
            return _size;
        }

        public static explicit operator T(NumericSignal<T, TEndian> signal)
        {
            return signal._value;
        }
    }

    public class NumericSignal<T> : NumericSignal<T, LitEndian>
        where T : struct
    {

    }
}
