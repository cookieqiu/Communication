using System.Runtime.CompilerServices;
using System.Text;

namespace Communication.Signals
{
    public class AutoStringSignal : ISignal
    {
        private static readonly Encoding DefaultEncoding = Encoding.ASCII;

        private string _value;

        private readonly Encoding _encoding;

        public int Size => _value.Length;

        public AutoStringSignal() : this(string.Empty, null) { }

        public AutoStringSignal(Encoding encoding) : this(string.Empty, encoding) { }

        public AutoStringSignal(string line, Encoding? encoding)
        {
            _value = line;
            _encoding = encoding ?? DefaultEncoding;
        }

        public void SetValue(string value) => _value = value;

        public string GetValue() => _value;

        public static explicit operator string(AutoStringSignal signal)
            => signal._value;

        public int Write(in byte[] buffer, int offset)
        {
            var data = _encoding.GetBytes(_value);
            var size = data.Length;
            Unsafe.CopyBlock(ref buffer[offset], ref data[0], (uint)size);
            return size;
        }

        public int Read(in byte[] buffer, int offset)
        {
            var size = Array.IndexOf<byte>(buffer, 0, offset) - offset;
            if (size < 0) size = buffer.Length - offset;
            _value = _encoding.GetString(buffer, offset, size);
            return size;
        }
    }
}
