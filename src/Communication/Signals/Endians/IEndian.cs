namespace Communication.Signals.Endians
{
    public interface IEndian
    {
        byte[] Read(in byte[] buffer, int offset, int count);

        void Write(in byte[] buffer, in byte[] data, int offset, int count);
    }
}
