namespace Communication.Signals
{
    public interface ISignal
    {
        int Write(in byte[] buffer, int offset);

        int Read(in byte[] buffer, int offset);

        int Size { get; }
    }
}
