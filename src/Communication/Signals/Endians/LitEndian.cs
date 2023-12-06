using System.Runtime.CompilerServices;

namespace Communication.Signals.Endians
{
    public class LitEndian : IEndian
    {
        public byte[] Read(in byte[] buffer, int offset, int count)
        {
            var result = new byte[count];
            Unsafe.CopyBlock(ref result[0], ref buffer[offset], (uint)count);
            return result;
        }

        public void Write(in byte[] buffer, in byte[] data, int offset, int count)
        {
            Unsafe.CopyBlock(ref buffer[offset], ref data[0], (uint)count);
        }
    }
}
