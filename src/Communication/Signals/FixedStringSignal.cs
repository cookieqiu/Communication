using System.Runtime.CompilerServices;
using System.Text;

namespace Communication.Signals
{
    public class FixedStringSignal : ISignal
    {
        private static readonly Encoding DefaultEncoding = Encoding.ASCII;

        private string _value;

        private readonly int _size;
        public int Size => _size;

        private readonly Encoding _encoding;

        public FixedStringSignal(int size) : this(string.Empty, size, null) { }

        public FixedStringSignal(int size, Encoding encoding) : this(string.Empty, size, encoding) { }

        public FixedStringSignal(string value, int size, Encoding? encoding)
        {
            Validate(value.Length, size);
            (_value, _size) = (value, size);
            _encoding = encoding ?? DefaultEncoding;
        }

        public void SetValue(string value)
        {
            Validate(value.Length, _size);
            _value = value;
        }

        public string GetValue()
        {
            return _value;
        }

        public static explicit operator string(FixedStringSignal signal)
            => signal._value;

        private static void Validate(int srcLength, int size)
        {
            if (srcLength > size)
            {
                throw new ArgumentOutOfRangeException(nameof(srcLength));
            }
        }

        public int Write(in byte[] buffer, int offset)
        {
            var data = _encoding.GetBytes(_value);
            var len = data.Length;
            Unsafe.InitBlock(ref buffer[offset + len], 0, (uint)(_size - len));
            Unsafe.CopyBlock(ref buffer[offset], ref data[0], (uint)len);
            return _size;
        }

        public int Read(in byte[] buffer, int offset)
        {
            _value = _encoding.GetString(buffer, offset, _size);
            return _size;
        }
    }
}
