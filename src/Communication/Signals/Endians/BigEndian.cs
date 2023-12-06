namespace Communication.Signals.Endians
{
    public class BigEndian : IEndian
    {
        public byte[] Read(in byte[] buffer, int offset, int count)
        {
            var result = new byte[count];
            for (int i = 0, j = offset + count - 1; i < count; i++, j--)
            {
                result[i] = buffer[j];
            }
            return result;
        }

        public void Write(in byte[] buffer, in byte[] data, int offset, int count)
        {
            for (int i = 0, j = offset + count - 1; i < count; i++, j--)
            {
                buffer[j] = data[i];
            }
        }
    }
}
